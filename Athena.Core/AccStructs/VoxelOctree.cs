using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Athena.Core.DataTypes;
using Athena.Core.Objects;
using Athena.Core.Tools;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using Athena.Core.Objects.Meshes;
using Athena.Core.Objects.Interfaces;

namespace Athena.Core.AccStructs
{
	/// <summary>
	/// 
	/// </summary>
	[FlagsAttribute]
	public enum OctreeNodeInfo : byte
	{
		Empty = 0,

		x0y0z0 = 1,
		x1y0z0 = 2,
		x1y1z0 = 4,
		x0y1z0 = 8,

		x0y0z1 = 16,
		x1y0z1 = 32,
		x1y1z1 = 64,
		x0y1z1 = 128,

		Full = 255
	};

	/// <summary>
	/// 
	/// </summary>
	public class OctreeNode
	{
		/// <summary>
		/// shows which child nodes are used,
		/// octree leaf will have Info == OctreeNodeInfo.Empty
		/// </summary>
		public OctreeNodeInfo Info { get; set; }

		/// <summary>
		/// voxel representing this node/leaf
		/// </summary>
		public Voxel Voxel { get; set; }

		/// <summary>
		/// Child nodes/leaves
		/// </summary>
		public List<OctreeNode> OctreeNodes { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public OctreeNode()
		{

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <param name="startIndex"></param>
		public OctreeNode(byte[] array, int startIndex)
		{
			Info = (OctreeNodeInfo)array[startIndex];
			Voxel = new Voxel(array, startIndex + 1);
		}

		/// <summary>
		/// 
		/// </summary>
		internal void SetVoxelAsAverage()
		{
			var color = new Vector3();
			var normal = new Vector3();
			
			foreach (var node in OctreeNodes)
			{
				color += node.Voxel.ColorAsVector3();
				normal += node.Voxel.Normal;
			}

			var countRev = 1f / (float)OctreeNodes.Count;
			Voxel = new Voxel
			{ 
				R = (byte)(color.X * countRev), 
				G = (byte)(color.Y * countRev), 
				B = (byte)(color.Z * countRev), 
				
				Flags = (byte)VoxelFlags.NotEmpty,

				Normal = (normal * countRev).Normalize()
			};
		}

		/// <summary>
		/// 
		/// </summary>
		public static int SizeOf { get { return Voxel.SizeOf + 1/*OctreeNodeInfo*/; } }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		internal byte[] ToArray()
		{
			var array = new byte[1 + Voxel.SizeOf];

			array[0] = (byte)Info;
			Voxel.ToArray().CopyTo(array, 1);
			
			return array;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class VoxelOctree
	{
		#region public properties

		/// <summary>
		/// 
		/// </summary>
		public OctreeNode RootNode { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public AACell RootCell { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public static bool GenerateOnMultipleThreads { get; set; }

		#endregion

		#region private variables

		private int maxDepth;
		private float voxelSizeThreshold;

		private static Random random = new Random((int)DateTime.Now.Ticks);

		/// <summary>
		/// input for node processing on different thread
		/// </summary>
		private class ProessNodeInfo
		{
			public IVoxelizable Obj { get; private set; }
			public AACell NodeCell { get; private set; }
			public OctreeNode Node { get; private set; }

			public CountdownEvent ProcessFinishedEvent { get; private set; }

			/// <summary>
			/// 
			/// </summary>
			/// <param name="obj"></param>
			/// <param name="nodeCell"></param>
			/// <param name="node"></param>
			/// <param name="processFinishedEvent"></param>
			public ProessNodeInfo(IVoxelizable obj, AACell nodeCell, OctreeNode node, CountdownEvent processFinishedEvent)
			{
				Obj = obj;
				NodeCell = nodeCell;
				Node = node;
				ProcessFinishedEvent = processFinishedEvent;
			}
		}

		#endregion

		#region private constructor

		/// <summary>
		/// 
		/// </summary>
		/// <param name="maxRecursion"></param>
		/// <param name="voxelSizeThreshold"></param>
		private VoxelOctree(int maxRecursion, float voxelSizeThreshold)
		{
			this.maxDepth = maxRecursion;
			this.voxelSizeThreshold = voxelSizeThreshold;
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="maxDepth"></param>
		/// <param name="voxelSizeThreshold"></param>
		/// <returns></returns>
		public static VoxelOctree Create(IVoxelizable obj, int maxDepth, float voxelSizeThreshold)
		{
			var octree = new VoxelOctree(maxDepth, voxelSizeThreshold);

			var timer = new HighPerformanceTimer();
			timer.Start();

			var cellSize = obj.BoundingBox.Size;
			var newCellSize = (cellSize.X > cellSize.Y ? (cellSize.X > cellSize.Z ? cellSize.X : cellSize.Z) : (cellSize.Y > cellSize.Z ? cellSize.Y : cellSize.Z)) * 0.5f;

			var generateNewOctree = true;

			if (obj is WavefrontObjMesh && File.Exists((obj as WavefrontObjMesh).Filename + ".octree"))
				generateNewOctree = !octree.LoadFromFile((obj as WavefrontObjMesh).Filename + ".octree", maxDepth);
			
			if (generateNewOctree)
			{
				#region calculate RootCell (equal edge sizes)

				var center = obj.BoundingBox.Center;
				octree.RootCell = new AACell { Min = new Vector3(center.X - newCellSize, center.Y - newCellSize, center.Z - newCellSize), Max = new Vector3(center.X + newCellSize, center.Y + newCellSize, center.Z + newCellSize) };

				#endregion

				// create octree recursively
				octree.RootNode = new OctreeNode();

				if (VoxelOctree.GenerateOnMultipleThreads)
				{
					Log.Instance.AddMsg(LogLevel.Info, string.Format("Generating VoxelOctree/MT [maxDepth: {0}; voxelSizeThreshold: {1}] ...", maxDepth, voxelSizeThreshold));
					octree.ProcessNodeMultithreaded(obj, octree.RootCell, octree.RootNode);
				}
				else
				{
					Log.Instance.AddMsg(LogLevel.Info, string.Format("Generating VoxelOctree/ST [maxDepth: {0}; voxelSizeThreshold: {1}] ...", maxDepth, voxelSizeThreshold));
					octree.ProcessNode(obj, octree.RootCell, octree.RootNode, 0);
				}
			}
	
			timer.Stop();

			// count all nodes
			var nodeCount = CountNodesRecursive(octree.RootNode) + 1;

			// get actual tree depth
			octree.maxDepth = VoxelOctree.GetMaxDepth(octree.RootNode);

			#region calculate smallest voxel size

			var smallestVoxelSize = newCellSize * 2f;
			for (var i = 0; i < octree.maxDepth; i++)
				smallestVoxelSize *= .5f;

			#endregion

			Log.Instance.AddMsg(LogLevel.Info, string.Format("VoxelOctree [size: {0}; nodes: {1}; depth: {2}; voxelSize: {3}] generated in {4}", newCellSize * 2f, nodeCount, octree.maxDepth, smallestVoxelSize, FormatString.GetDuration((int)timer.Duration)));

			if (obj is WavefrontObjMesh && generateNewOctree)
				octree.SaveToFile((obj as WavefrontObjMesh).Filename + ".octree");

			return octree;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		private static int GetMaxDepth(OctreeNode node)
		{
			if (node.Info == OctreeNodeInfo.Empty)
				return 0;

			var maxChildDepth = int.MinValue;
			foreach (var childNode in node.OctreeNodes)
			{
				var childDepth = GetMaxDepth(childNode);
				if (childDepth > maxChildDepth)
					maxChildDepth = childDepth;
			}

			return maxChildDepth + 1;
		}

		#region Serialization

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filename"></param>
		private void SaveToFile(string filename)
		{
			var memStream = new MemoryStream();

			memStream.Write(BitConverter.GetBytes(maxDepth), 0, sizeof(int));
			memStream.Write(BitConverter.GetBytes(voxelSizeThreshold), 0, sizeof(float));

			memStream.Write(RootCell.ToArray(), 0, AACell.SizeOf);
			memStream.Write(RootNode.ToArray(), 0, OctreeNode.SizeOf);

			SaveNodes(RootNode.OctreeNodes, memStream);

			File.WriteAllBytes(filename, memStream.ToArray());
			memStream.Close();

			Log.Instance.AddMsg(LogLevel.Info, string.Format("VoxelOctree saved to file '{0}'", filename));
		}

		/// <summary>
		/// /
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="memStream"></param>
		private void SaveNodes(List<OctreeNode> nodes, MemoryStream memStream)
		{
			foreach (var node in nodes)
			{
				memStream.Write(node.ToArray(), 0, OctreeNode.SizeOf);

				if (node.OctreeNodes != null)
					SaveNodes(node.OctreeNodes, memStream);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="desiredDepth"></param>
		private bool LoadFromFile(string filename, int desiredDepth)
		{
			var array = File.ReadAllBytes(filename);

			var index = 0;
			var depth = BitConverter.ToInt32(array, index);
			index += sizeof(int);

			if (depth < desiredDepth)
				return false;

			maxDepth = depth;

			Log.Instance.AddMsg(LogLevel.Info, string.Format("Loading VoxelOctree from '{0}.octree' ...", filename));

			voxelSizeThreshold = BitConverter.ToSingle(array, index);
			index += sizeof(float);

			RootCell = new AACell(array, index);
			index += AACell.SizeOf;

			RootNode = new OctreeNode(array, index);
			index += OctreeNode.SizeOf;

			LoadNodes(array, index, RootNode);

			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		/// <param name="parentNode"></param>
		private int LoadNodes(byte[] array, int index, OctreeNode parentNode)
		{
			var nodeCount = GetNumberOfNodes(parentNode.Info);
			if (nodeCount == 0)
				return index;

			parentNode.OctreeNodes = new List<OctreeNode>();
			
			for (var i = 0; i < nodeCount; i++)
			{
				var newNode = new OctreeNode(array, index);
				index += OctreeNode.SizeOf;

				parentNode.OctreeNodes.Add(newNode);
				
				if (newNode.Info != OctreeNodeInfo.Empty)
					index = LoadNodes(array, index, newNode);
			}

			return index;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="octreeNodeInfo"></param>
		/// <returns></returns>
		private int GetNumberOfNodes(OctreeNodeInfo octreeNodeInfo)
		{
			if (octreeNodeInfo == OctreeNodeInfo.Empty)
				return 0;

			if (octreeNodeInfo == OctreeNodeInfo.Full)
				return 8;

			var count = 0;

			if (octreeNodeInfo.HasFlag(OctreeNodeInfo.x0y0z0))
				count++;

			if (octreeNodeInfo.HasFlag(OctreeNodeInfo.x1y0z0))
				count++;

			if (octreeNodeInfo.HasFlag(OctreeNodeInfo.x1y1z0))
				count++;

			if (octreeNodeInfo.HasFlag(OctreeNodeInfo.x1y1z1))
				count++;

			if (octreeNodeInfo.HasFlag(OctreeNodeInfo.x0y1z1))
				count++;

			if (octreeNodeInfo.HasFlag(OctreeNodeInfo.x0y0z1))
				count++;

			if (octreeNodeInfo.HasFlag(OctreeNodeInfo.x0y1z0))
				count++;

			if (octreeNodeInfo.HasFlag(OctreeNodeInfo.x1y0z1))
				count++;

			return count;
		}

		#endregion

		#region private methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mesh"></param>
		/// <param name="nodeCell"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		private void ProcessNode(IVoxelizable obj, AACell nodeCell, OctreeNode node, int nodeDepth)
		{
			var center = nodeCell.Center;
			var childNodeSize = (nodeCell.Max.X - nodeCell.Min.X) * .5f;
 
			var nodeInfo = OctreeNodeInfo.Empty;

			var childCell = nodeCell;
			if (childNodeSize > voxelSizeThreshold && maxDepth > nodeDepth)
			{
				#region check child nodes

				// xyz
				childCell.Max = center;
				if (!IsNodeEmpty(obj, childCell))
					nodeInfo |= OctreeNodeInfo.x0y0z0;

				// Xyz
				childCell.Min.X += childNodeSize;
				childCell.Max.X += childNodeSize;
				if (!IsNodeEmpty(obj, childCell))
					nodeInfo |= OctreeNodeInfo.x1y0z0;

				// XYz
				childCell.Min.Y += childNodeSize;
				childCell.Max.Y += childNodeSize;
				if (!IsNodeEmpty(obj, childCell))
					nodeInfo |= OctreeNodeInfo.x1y1z0;

				// xYz
				childCell.Min.X -= childNodeSize;
				childCell.Max.X -= childNodeSize;
				if (!IsNodeEmpty(obj, childCell))
					nodeInfo |= OctreeNodeInfo.x0y1z0;

				// xyZ
				childCell.Min.Y -= childNodeSize;
				childCell.Max.Y -= childNodeSize;
				childCell.Min.Z += childNodeSize;
				childCell.Max.Z += childNodeSize;
				if (!IsNodeEmpty(obj, childCell))
					nodeInfo |= OctreeNodeInfo.x0y0z1;

				// XyZ
				childCell.Min.X += childNodeSize;
				childCell.Max.X += childNodeSize;
				if (!IsNodeEmpty(obj, childCell))
					nodeInfo |= OctreeNodeInfo.x1y0z1;

				// XYZ
				childCell.Min.Y += childNodeSize;
				childCell.Max.Y += childNodeSize;
				if (!IsNodeEmpty(obj, childCell))
					nodeInfo |= OctreeNodeInfo.x1y1z1;

				// xYZ
				childCell.Min.X -= childNodeSize;
				childCell.Max.X -= childNodeSize;
				if (!IsNodeEmpty(obj, childCell))
					nodeInfo |= OctreeNodeInfo.x0y1z1;

				#endregion
			}

			node.Info = nodeInfo;
			if (nodeInfo == OctreeNodeInfo.Empty)
			{
				node.Voxel = GetVoxel(obj, nodeCell);
			}
			else
			{
				nodeDepth++;

				var childNodes = new OctreeNode[8];

				#region process child nodes

				// xyz
				childCell.Min = nodeCell.Min;
				childCell.Max = center;
				if (nodeInfo.HasFlag(OctreeNodeInfo.x0y0z0))
				{
					childNodes[0] = new OctreeNode();
					ProcessNode(obj, childCell, childNodes[0], nodeDepth);
				}

				// Xyz
				childCell.Min.X += childNodeSize;
				childCell.Max.X += childNodeSize;
				if (nodeInfo.HasFlag(OctreeNodeInfo.x1y0z0))
				{
					childNodes[1] = new OctreeNode();
					ProcessNode(obj, childCell, childNodes[1], nodeDepth);
				}

				// XYz
				childCell.Min.Y += childNodeSize;
				childCell.Max.Y += childNodeSize;
				if (nodeInfo.HasFlag(OctreeNodeInfo.x1y1z0))
				{
					childNodes[2] = new OctreeNode();
					ProcessNode(obj, childCell, childNodes[2], nodeDepth);
				}

				// xYz
				childCell.Min.X -= childNodeSize;
				childCell.Max.X -= childNodeSize;
				if (nodeInfo.HasFlag(OctreeNodeInfo.x0y1z0))
				{
					childNodes[3] = new OctreeNode();
					ProcessNode(obj, childCell, childNodes[3], nodeDepth);
				}

				// xyZ
				childCell.Min.Y -= childNodeSize;
				childCell.Max.Y -= childNodeSize;
				childCell.Min.Z += childNodeSize;
				childCell.Max.Z += childNodeSize;
				if (nodeInfo.HasFlag(OctreeNodeInfo.x0y0z1))
				{
					childNodes[4] = new OctreeNode();
					ProcessNode(obj, childCell, childNodes[4], nodeDepth);
				}

				// XyZ
				childCell.Min.X += childNodeSize;
				childCell.Max.X += childNodeSize;
				if (nodeInfo.HasFlag(OctreeNodeInfo.x1y0z1))
				{
					childNodes[5] = new OctreeNode();
					ProcessNode(obj, childCell, childNodes[5], nodeDepth);
				}

				// XYZ
				childCell.Min.Y += childNodeSize;
				childCell.Max.Y += childNodeSize;
				if (nodeInfo.HasFlag(OctreeNodeInfo.x1y1z1))
				{
					childNodes[6] = new OctreeNode();
					ProcessNode(obj, childCell, childNodes[6], nodeDepth);
				}

				// xYZ
				childCell.Min.X -= childNodeSize;
				childCell.Max.X -= childNodeSize;
				if (nodeInfo.HasFlag(OctreeNodeInfo.x0y1z1))
				{
					childNodes[7] = new OctreeNode();
					ProcessNode(obj, childCell, childNodes[7], nodeDepth);
				}

				#endregion

				node.OctreeNodes = (from n in childNodes where n != null select n).ToList();
				node.SetVoxelAsAverage();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="nodeCell"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		private void ProcessNodeMultithreaded(IVoxelizable obj, AACell nodeCell, OctreeNode node)
		{
			var center = nodeCell.Center;
			var childNodeSize = (nodeCell.Max.X - nodeCell.Min.X) * .5f;

			var nodeInfo = OctreeNodeInfo.Empty;

			var childCell = nodeCell;
			if (childNodeSize > voxelSizeThreshold && maxDepth > 0)
			{
				#region check child nodes

				// xyz
				childCell.Max = center;
				if (!IsNodeEmpty(obj, childCell))
					nodeInfo |= OctreeNodeInfo.x0y0z0;

				// Xyz
				childCell.Min.X += childNodeSize;
				childCell.Max.X += childNodeSize;
				if (!IsNodeEmpty(obj, childCell))
					nodeInfo |= OctreeNodeInfo.x1y0z0;

				// XYz
				childCell.Min.Y += childNodeSize;
				childCell.Max.Y += childNodeSize;
				if (!IsNodeEmpty(obj, childCell))
					nodeInfo |= OctreeNodeInfo.x1y1z0;

				// xYz
				childCell.Min.X -= childNodeSize;
				childCell.Max.X -= childNodeSize;
				if (!IsNodeEmpty(obj, childCell))
					nodeInfo |= OctreeNodeInfo.x0y1z0;

				// xyZ
				childCell.Min.Y -= childNodeSize;
				childCell.Max.Y -= childNodeSize;
				childCell.Min.Z += childNodeSize;
				childCell.Max.Z += childNodeSize;
				if (!IsNodeEmpty(obj, childCell))
					nodeInfo |= OctreeNodeInfo.x0y0z1;

				// XyZ
				childCell.Min.X += childNodeSize;
				childCell.Max.X += childNodeSize;
				if (!IsNodeEmpty(obj, childCell))
					nodeInfo |= OctreeNodeInfo.x1y0z1;

				// XYZ
				childCell.Min.Y += childNodeSize;
				childCell.Max.Y += childNodeSize;
				if (!IsNodeEmpty(obj, childCell))
					nodeInfo |= OctreeNodeInfo.x1y1z1;

				// xYZ
				childCell.Min.X -= childNodeSize;
				childCell.Max.X -= childNodeSize;
				if (!IsNodeEmpty(obj, childCell))
					nodeInfo |= OctreeNodeInfo.x0y1z1;

				#endregion
			}

			node.Info = nodeInfo;
			if (nodeInfo == OctreeNodeInfo.Empty)
			{
				node.Voxel = GetVoxel(obj, nodeCell);
			}
			else
			{
				var childNodes = new OctreeNode[8];
				var childNodesProcessEvent = new CountdownEvent(8);

				#region process child nodes

				// xyz
				childCell.Min = nodeCell.Min;
				childCell.Max = center;
				if (nodeInfo.HasFlag(OctreeNodeInfo.x0y0z0))
				{
					childNodes[0] = new OctreeNode();
					ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessNodeOnThread), new ProessNodeInfo(obj, childCell, childNodes[0], childNodesProcessEvent));
				}

				// Xyz
				childCell.Min.X += childNodeSize;
				childCell.Max.X += childNodeSize;
				if (nodeInfo.HasFlag(OctreeNodeInfo.x1y0z0))
				{
					childNodes[1] = new OctreeNode();
					ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessNodeOnThread), new ProessNodeInfo(obj, childCell, childNodes[1], childNodesProcessEvent));
				}

				// XYz
				childCell.Min.Y += childNodeSize;
				childCell.Max.Y += childNodeSize;
				if (nodeInfo.HasFlag(OctreeNodeInfo.x1y1z0))
				{
					childNodes[2] = new OctreeNode();
					ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessNodeOnThread), new ProessNodeInfo(obj, childCell, childNodes[2], childNodesProcessEvent));
				}

				// xYz
				childCell.Min.X -= childNodeSize;
				childCell.Max.X -= childNodeSize;
				if (nodeInfo.HasFlag(OctreeNodeInfo.x0y1z0))
				{
					childNodes[3] = new OctreeNode();
					ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessNodeOnThread), new ProessNodeInfo(obj, childCell, childNodes[3], childNodesProcessEvent));
				}

				// xyZ
				childCell.Min.Y -= childNodeSize;
				childCell.Max.Y -= childNodeSize;
				childCell.Min.Z += childNodeSize;
				childCell.Max.Z += childNodeSize;
				if (nodeInfo.HasFlag(OctreeNodeInfo.x0y0z1))
				{
					childNodes[4] = new OctreeNode();
					ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessNodeOnThread), new ProessNodeInfo(obj, childCell, childNodes[4], childNodesProcessEvent));
				}

				// XyZ
				childCell.Min.X += childNodeSize;
				childCell.Max.X += childNodeSize;
				if (nodeInfo.HasFlag(OctreeNodeInfo.x1y0z1))
				{
					childNodes[5] = new OctreeNode();
					ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessNodeOnThread), new ProessNodeInfo(obj, childCell, childNodes[5], childNodesProcessEvent));
				}

				// XYZ
				childCell.Min.Y += childNodeSize;
				childCell.Max.Y += childNodeSize;
				if (nodeInfo.HasFlag(OctreeNodeInfo.x1y1z1))
				{
					childNodes[6] = new OctreeNode();
					ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessNodeOnThread), new ProessNodeInfo(obj, childCell, childNodes[6], childNodesProcessEvent));
				}

				// xYZ
				childCell.Min.X -= childNodeSize;
				childCell.Max.X -= childNodeSize;
				if (nodeInfo.HasFlag(OctreeNodeInfo.x0y1z1))
				{
					childNodes[7] = new OctreeNode();
					ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessNodeOnThread), new ProessNodeInfo(obj, childCell, childNodes[7], childNodesProcessEvent));
				}

				#endregion

				// wait for threads to end
				childNodesProcessEvent.Wait();

				node.OctreeNodes = (from n in childNodes where n != null select n).ToList();
				node.SetVoxelAsAverage();
			}
		}

		/// <summary>
		/// process given octree node
		/// </summary>
		/// <param name="input"></param>
		private void ProcessNodeOnThread(object input)
		{
			var processNodeInfo = input as ProessNodeInfo;
			try
			{
				ProcessNode(processNodeInfo.Obj, processNodeInfo.NodeCell, processNodeInfo.Node, 1);
			}
			finally
			{
				processNodeInfo.ProcessFinishedEvent.Signal();
			}
		}

		/// <summary>
		/// 
		/// </summary>	
		/// <param name="obj"></param>
		/// <param name="nodeCell"></param>
		/// <returns></returns>
		private static Voxel GetVoxel(IVoxelizable obj, AACell nodeCell)
		{
			// TODO get color/normal/material of voxel at collision point

			//var color = new ColorRGB((int)DateTime.Now.Ticks);
			var color = new ColorRGB((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());

			var voxel = new Voxel(color, Vector3.Zero);

			if (obj is Sphere)
				voxel.Normal = nodeCell.Center.Normalize();
			else
				// temporary, not accurate!
				voxel.Normal = nodeCell.Center.Normalize();

			return voxel;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="nodeCell"></param>
		/// <returns></returns>
		private static bool IsNodeEmpty(IVoxelizable obj, AACell nodeCell)
		{
			if (obj is Mesh)
			{
				var faces = (obj as Mesh).Faces;
				foreach (var face in faces)
					if (Collision.AACellAndFace(nodeCell, face))
						return false;
			}

			else if (obj is Sphere)
				return !Collision.AACellAndSphere(nodeCell, (obj as Sphere).Radius);

			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		private static int CountNodesRecursive(OctreeNode node)
		{
			if (node.Info == OctreeNodeInfo.Empty)
				return 0;

			var result = node.OctreeNodes.Count;
			foreach (var childNode in node.OctreeNodes)
				result += CountNodesRecursive(childNode);

			return result;
		}

		#endregion
	}
}
