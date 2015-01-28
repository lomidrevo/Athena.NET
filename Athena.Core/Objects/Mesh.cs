using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Athena.Core.DataTypes;
using System.Xml.Serialization;
using Athena.Core.Tools;
using System.IO;
using Athena.Core.AccStructs;
using Athena.Core.Objects.Interfaces;

namespace Athena.Core.Objects
{
	/// <summary>
	/// 
	/// </summary>
	public class Mesh : SceneObject, IVoxelizable
	{
		/// <summary>
		/// 
		/// </summary>
		public Mesh() : base()
		{
			Vertices = new List<Vector3>();
			Normals = new List<Vector3>();
			TextureCoords = new List<Vector2>();
			Faces = new List<Face>();
		}

		#region public properties

		/// <summary>
		/// 
		/// </summary>
		[XmlIgnore]
		public List<Vector2> TextureCoords { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[XmlIgnore]
		public List<Vector3> Vertices { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[XmlIgnore]
		public List<Vector3> Normals { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[XmlIgnore]
		public List<Face> Faces { get; set; }

		#endregion

		#region IVoxelizable Members

		public VoxelOctree Octree { get; set; }

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="maxDepth"></param>
		/// <param name="voxelSizeThreshold"></param>
		public void GenerateOctree(int maxDepth, float voxelSizeThreshold)
		{
			Octree = VoxelOctree.Create(this, maxDepth, voxelSizeThreshold);
		}
	}
}