using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Athena.Core.DataTypes;
using Athena.Core.Objects;
using Athena.Core.AccStructs;
using Athena.Core.Objects.Lights;
using Athena.Core.Objects.Interfaces;

namespace Athena.Core
{
	public class Intersection
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ray"></param>
		/// <param name="sceneObj"></param>
		/// <param name="maxOctreeDepth"></param>
		public static IntersectionResult RayAndObject(Ray ray, SceneObject sceneObj, int maxOctreeDepth)
		{
			ray.Origin -= sceneObj.Position;

			// TODO toto mi pripada zbytocne
			Vector3 normal = Vector3.Zero;

			IntersectionResult intersectionResult = null;

			if (sceneObj is OmniLight)
			{
				#region check OmniLight intersection

				intersectionResult = new IntersectionResult 
				{ 
					Distance = Intersection.RayAndAACell(ray, sceneObj.BoundingBox, ref normal)
				};

				#endregion
			}
			else if (sceneObj is Sphere && (sceneObj as IVoxelizable).Octree == null)
			{
				#region check Sphere intersection

				intersectionResult = new IntersectionResult
				{
					Distance = Intersection.RayAndSphere(ray, (sceneObj as Sphere).Radius)
				};

				#endregion
			}
			else if (sceneObj is IVoxelizable)
			{
				#region check VoxelOctree intersection

				var octree = (sceneObj as IVoxelizable).Octree;
				intersectionResult = Intersection.RayAndVoxelNode(ray, octree.RootNode, octree.RootCell, maxOctreeDepth, ref normal);

				#endregion
			}

			ray.Origin += sceneObj.Position;
			intersectionResult.obj = sceneObj;

			return intersectionResult;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ray"></param>
		/// <param name="node"></param>
		/// <param name="maxDepth"></param>
		/// <returns></returns>
		private static IntersectionResult RayAndVoxelNode(Ray ray, OctreeNode node, AACell cell, int maxDepth, ref Vector3 normal)
		{
			var intersectionResult = new IntersectionResult
			{ 
				Voxel = node.Voxel, 
				Point = cell.Center
			};

			if (!Collision.RayAndAACell(ray, cell))
				return intersectionResult;

			var nodeInfo = node.Info;
			if (nodeInfo == OctreeNodeInfo.Empty || maxDepth == 0)
			{
				intersectionResult.Distance = Intersection.RayAndAACell(ray, cell, ref normal);
				return intersectionResult;
			}

			var resultNormal = normal;
			maxDepth--;

			#region check child nodes for intersection

			var cellHalfSize = cell.Size.X * .5f;
			var cellCenter = cell.Center;
			var nextChildNodeId = 0;
			var childCell = cell;

			#region 000

			if (nodeInfo.HasFlag(OctreeNodeInfo.x0y0z0))
			{
				childCell = cell;
				childCell.Max = cellCenter;

				var tmpResult = RayAndVoxelNode(ray, node.OctreeNodes[nextChildNodeId], childCell, maxDepth, ref normal);
				if (tmpResult.Distance < intersectionResult.Distance)
				{
					resultNormal = normal;
					intersectionResult = tmpResult;
				}

				nextChildNodeId++;
			}

			#endregion

			#region 100

			if (nodeInfo.HasFlag(OctreeNodeInfo.x1y0z0))
			{
				childCell = cell;
				childCell.Max = cellCenter;
				childCell.Min.X += cellHalfSize;
				childCell.Max.X += cellHalfSize;

				var tmpResult = RayAndVoxelNode(ray, node.OctreeNodes[nextChildNodeId], childCell, maxDepth, ref normal);
				if (tmpResult.Distance < intersectionResult.Distance)
				{
					resultNormal = normal;
					intersectionResult = tmpResult;
				}

				nextChildNodeId++;
			}

			#endregion

			#region 110

			if (nodeInfo.HasFlag(OctreeNodeInfo.x1y1z0))
			{
				childCell = cell;
				childCell.Min = cellCenter;
				childCell.Min.Z -= cellHalfSize;
				childCell.Max.Z -= cellHalfSize;

				var tmpResult = RayAndVoxelNode(ray, node.OctreeNodes[nextChildNodeId], childCell, maxDepth, ref normal);
				if (tmpResult.Distance < intersectionResult.Distance)
				{
					resultNormal = normal;
					intersectionResult = tmpResult;
				}

				nextChildNodeId++;
			}

			#endregion

			#region 010

			if (nodeInfo.HasFlag(OctreeNodeInfo.x0y1z0))
			{
				childCell = cell;
				childCell.Max = cellCenter;
				childCell.Min.Y += cellHalfSize;
				childCell.Max.Y += cellHalfSize;

				var tmpResult = RayAndVoxelNode(ray, node.OctreeNodes[nextChildNodeId], childCell, maxDepth, ref normal);
				if (tmpResult.Distance < intersectionResult.Distance)
				{
					resultNormal = normal;
					intersectionResult = tmpResult;
				}

				nextChildNodeId++;
			}

			#endregion


			#region 001

			if (nodeInfo.HasFlag(OctreeNodeInfo.x0y0z1))
			{
				childCell = cell;
				childCell.Max = cellCenter;
				childCell.Min.Z += cellHalfSize;
				childCell.Max.Z += cellHalfSize;

				var tmpResult = RayAndVoxelNode(ray, node.OctreeNodes[nextChildNodeId], childCell, maxDepth, ref normal);
				if (tmpResult.Distance < intersectionResult.Distance)
				{
					resultNormal = normal;
					intersectionResult = tmpResult;
				}

				nextChildNodeId++;
			}

			#endregion

			#region 101

			if (nodeInfo.HasFlag(OctreeNodeInfo.x1y0z1))
			{
				childCell = cell;
				childCell.Min = cellCenter;
				childCell.Min.Y -= cellHalfSize;
				childCell.Max.Y -= cellHalfSize;

				var tmpResult = RayAndVoxelNode(ray, node.OctreeNodes[nextChildNodeId], childCell, maxDepth, ref normal);
				if (tmpResult.Distance < intersectionResult.Distance)
				{
					resultNormal = normal;
					intersectionResult = tmpResult;
				}

				nextChildNodeId++;
			}

			#endregion

			#region 111

			if (nodeInfo.HasFlag(OctreeNodeInfo.x1y1z1))
			{
				childCell = cell;
				childCell.Min = cellCenter;

				var tmpResult = RayAndVoxelNode(ray, node.OctreeNodes[nextChildNodeId], childCell, maxDepth, ref normal);
				if (tmpResult.Distance < intersectionResult.Distance)
				{
					resultNormal = normal;
					intersectionResult = tmpResult;
				}

				nextChildNodeId++;
			}

			#endregion

			#region 011

			if (nodeInfo.HasFlag(OctreeNodeInfo.x0y1z1))
			{
				childCell = cell;
				childCell.Min = cellCenter;
				childCell.Min.X -= cellHalfSize;
				childCell.Max.X -= cellHalfSize;

				var tmpResult = RayAndVoxelNode(ray, node.OctreeNodes[nextChildNodeId], childCell, maxDepth, ref normal);
				if (tmpResult.Distance < intersectionResult.Distance)
				{
					resultNormal = normal;
					intersectionResult = tmpResult;
				}

				nextChildNodeId++;
			}

			#endregion

			#endregion

			if (intersectionResult.Distance < float.MaxValue)
				normal = resultNormal;

			return intersectionResult;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ray"></param>
		/// <param name="cell"></param>
		/// <param name="normal"></param>
		/// <returns></returns>
		private static float RayAndAACell(Ray ray, AACell cell, ref Vector3 normal)
		{
			var tMin = float.MaxValue;

			// ray from outside
			if (!Collision.PointInAACell(ray.Origin, cell))
			{
				#region --> cell.Min.X, cell.Max.X <--

				if (ray.Origin.X <= cell.Min.X)
				{
					var t = Intersection.RayAndPlane(ray, Vector3.UnitXNegative, System.Math.Abs(cell.Min.X));
					if (t < tMin)
					{
						var hit = ray.Origin.Y + ray.Direction.Y * t;
						if (hit >= cell.Min.Y && hit <= cell.Max.Y)
						{
							hit = ray.Origin.Z + ray.Direction.Z * t;
							if (hit >= cell.Min.Z && hit <= cell.Max.Z)
							{
								tMin = t;
								normal = Vector3.UnitXNegative;
							}
						}
					}
				}
				else if (ray.Origin.X >= cell.Max.X)
				{
					var t = Intersection.RayAndPlane(ray, Vector3.UnitX, System.Math.Abs(cell.Max.X));
					if (t < tMin)
					{
						var hit = ray.Origin.Y + ray.Direction.Y * t;
						if (hit >= cell.Min.Y && hit <= cell.Max.Y)
						{
							hit = ray.Origin.Z + ray.Direction.Z * t;
							if (hit >= cell.Min.Z && hit <= cell.Max.Z)
							{
								tMin = t;
								normal = Vector3.UnitX;
							}
						}
					}
				}

				#endregion

				#region --> cell.Min.Y, cell.Max.Y <--

				if (ray.Origin.Y <= cell.Min.Y)
				{
					var t = Intersection.RayAndPlane(ray, Vector3.UnitYNegative, System.Math.Abs(cell.Min.Y));
					if (t < tMin)
					{
						var hit = ray.Origin.X + ray.Direction.X * t;
						if (hit > cell.Min.X && hit < cell.Max.X)
						{
							hit = ray.Origin.Z + ray.Direction.Z * t;
							if (hit > cell.Min.Z && hit < cell.Max.Z)
							{
								tMin = t;
								normal = Vector3.UnitYNegative;
							}
						}
					}
				}
				else if (ray.Origin.Y >= cell.Max.Y)
				{
					var t = Intersection.RayAndPlane(ray, Vector3.UnitY, System.Math.Abs(cell.Max.Y));
					if (t < tMin)
					{
						var hit = ray.Origin.X + ray.Direction.X * t;
						if (hit > cell.Min.X && hit < cell.Max.X)
						{
							hit = ray.Origin.Z + ray.Direction.Z * t;
							if (hit > cell.Min.Z && hit < cell.Max.Z)
							{
								tMin = t;
								normal = Vector3.UnitY;
							}
						}
					}
				}

				#endregion

				#region --> cell.Min.Z, cell.Max.Z <--

				if (ray.Origin.Z <= cell.Min.Z)
				{
					var t = Intersection.RayAndPlane(ray, Vector3.UnitZNegative, System.Math.Abs(cell.Min.Z));
					if (t < tMin)
					{
						var hit = ray.Origin.X + ray.Direction.X * t;
						if (hit > cell.Min.X && hit < cell.Max.X)
						{
							hit = ray.Origin.Y + ray.Direction.Y * t;
							if (hit > cell.Min.Y && hit < cell.Max.Y)
							{
								tMin = t;
								normal = Vector3.UnitZNegative;
							}
						}
					}
				}
				else if (ray.Origin.Z >= cell.Max.Z)
				{
					var t = Intersection.RayAndPlane(ray, Vector3.UnitZ, System.Math.Abs(cell.Max.Z));
					if (t < tMin)
					{
						var hit = ray.Origin.X + ray.Direction.X * t;
						if (hit > cell.Min.X && hit < cell.Max.X)
						{
							hit = ray.Origin.Y + ray.Direction.Y * t;
							if (hit > cell.Min.Y && hit < cell.Max.Y)
							{
								tMin = t;
								normal = Vector3.UnitZ;
							}
						}
					}
				}

				#endregion
			}

			// ray from inside
			else
			{
				#region cell.Min.X <-- --> cell.Max.X

				if (ray.Direction.X > 0f)
				{
					var t = Intersection.RayAndPlane(ray, Vector3.UnitX, cell.Max.X);

					if (t < tMin)
					{
						var hit = ray.Origin.Y + ray.Direction.Y * t;
						if (hit > cell.Min.Y && hit < cell.Max.Y)
						{
							hit = ray.Origin.Z + ray.Direction.Z * t;
							if (hit > cell.Min.Z && hit < cell.Max.Z)
							{
								tMin = t;
								normal = Vector3.UnitXNegative;
							}
						}
					}
				}
				else
				{
					var t = Intersection.RayAndPlane(ray, Vector3.UnitXNegative, cell.Min.X);

					if (t < tMin)
					{
						var hit = ray.Origin.Y + ray.Direction.Y * t;
						if (hit > cell.Min.Y && hit < cell.Max.Y)
						{
							hit = ray.Origin.Z + ray.Direction.Z * t;
							if (hit > cell.Min.Z && hit < cell.Max.Z)
							{
								tMin = t;
								normal = Vector3.UnitX;
							}
						}
					}
				}

				#endregion

				#region cell.Min.Y <-- --> cell.Max.Y

				if (ray.Direction.Y > 0)
				{
					var t = Intersection.RayAndPlane(ray, Vector3.UnitY, cell.Max.Y);

					if (t < tMin)
					{
						var hit = ray.Origin.X + ray.Direction.X * t;
						if (hit > cell.Min.X && hit < cell.Max.X)
						{
							hit = ray.Origin.Z + ray.Direction.Z * t;
							if (hit > cell.Min.Z && hit < cell.Max.Z)
							{
								tMin = t;
								normal = Vector3.UnitYNegative;
							}
						}
					}
				}
				else
				{
					var t = Intersection.RayAndPlane(ray, Vector3.UnitYNegative, cell.Min.Y);

					if (t < tMin)
					{
						var hit = ray.Origin.X + ray.Direction.X * t;
						if (hit > cell.Min.X && hit < cell.Max.X)
						{
							hit = ray.Origin.Z + ray.Direction.Z * t;
							if (hit > cell.Min.Z && hit < cell.Max.Z)
							{
								tMin = t;
								normal = Vector3.UnitY;
							}
						}
					}
				}

				#endregion

				#region cell.Min.Z <-- --> cell.Max.Z

				if (ray.Direction.Z > 0)
				{
					var t = Intersection.RayAndPlane(ray, Vector3.UnitZ, cell.Max.Z);

					if (t < tMin)
					{
						var hit = ray.Origin.X + ray.Direction.X * t;
						if (hit > cell.Min.X && hit < cell.Max.X)
						{
							hit = ray.Origin.Y + ray.Direction.Y * t;
							if (hit > cell.Min.Y && hit < cell.Max.Y)
							{
								tMin = t;
								normal = Vector3.UnitZNegative;
							}
						}
					}
				}
				else
				{
					var t = Intersection.RayAndPlane(ray, Vector3.UnitZNegative, cell.Min.Z);

					if (t < tMin)
					{
						var hit = ray.Origin.X + ray.Direction.X * t;
						if (hit > cell.Min.X && hit < cell.Max.X)
						{
							hit = ray.Origin.Y + ray.Direction.Y * t;
							if (hit > cell.Min.Y && hit < cell.Max.Y)
							{
								tMin = t;
								normal = Vector3.UnitZ;
							}
						}
					}
				}
				#endregion
			}

			return tMin;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ray"></param>
		/// <param name="planeNormal"></param>
		/// <param name="planeDistance"></param>
		/// <returns></returns>
		public static float RayAndPlane(Ray ray, Vector3 planeNormal, float planeDistance)
		{
			var v0 = -(Vector3.Dot(planeNormal, ray.Origin) - planeDistance);
			var vd = Vector3.Dot(planeNormal, ray.Direction);

			return v0 / vd;
		}

		/// <summary>
		/// Geometric method
		/// 
		/// http://www.devmaster.net/wiki/Ray-sphere_intersection
		/// </summary>
		/// <param name="ray"></param>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static float RayAndSphere(Ray ray, float radius)
		{
			var radius2 = radius * radius;
			var distVector = Vector3.Zero - ray.Origin;
			var distance2 = Vector3.Dot(distVector, distVector);

			var dot = Vector3.Dot(distVector, ray.Direction);
			if (distance2 < radius2)
				return dot + (float)System.Math.Sqrt((radius2 - distance2) + (dot * dot));

			if (dot < 0)
				return float.MaxValue;

			var l2hc = (radius2 - distance2) + (dot * dot);
			return (l2hc < 0) ? float.MaxValue : dot - (float)System.Math.Sqrt(l2hc);
		}
	}
}
