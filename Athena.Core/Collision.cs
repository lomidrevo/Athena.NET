using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Athena.Core.DataTypes;
using Athena.Core.Math;
using Athena.Core.Objects;

namespace Athena.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class Collision
	{
		/// <summary>
		/// AABB-triangle overlap test code
		/// by Tomas Akenine-Möller
		/// 
		/// http://jgt.akpeters.com/papers/AkenineMoller01/tribox.html
		/// </summary>
		/// <param name="cell"></param>
		/// <param name="face"></param>
		/// <returns></returns>
		public static bool AACellAndFace(AACell cell, Face face)
		{
			var cellCenter = cell.Center;
			var boxHalfSize = cell.Size * 0.5f;

			/* move everything so that the boxcenter is in (0,0,0) */
			var v0 = face.ParentMesh.Vertices[face.VertexIndices[0]] - cellCenter;
			var v1 = face.ParentMesh.Vertices[face.VertexIndices[1]] - cellCenter;
			var v2 = face.ParentMesh.Vertices[face.VertexIndices[2]] - cellCenter;

			/* compute triangle edges */
			var e0 = v1 - v0;
			var e1 = v2 - v1;
			var e2 = v0 - v2;

			/* test the 9 tests first (this was faster) */
			float min, max;

			var fex = System.Math.Abs(e0.X);
			var fey = System.Math.Abs(e0.Y);
			var fez = System.Math.Abs(e0.Z);

			#region AXISTEST_X01(e0[Z], e0[Y], fez, fey)
			
			var p0 = e0.Z * v0.Y - e0.Y * v0.Z;
			var p2 = e0.Z * v2.Y - e0.Y * v2.Z;
			if (p0 < p2) 
			{
				min = p0; 
				max = p2;
			} 
			else 
			{
				min = p2; 
				max = p0;
			}
			var rad = fez * boxHalfSize.Y + fey * boxHalfSize.Z;
			if (min > rad || max < -rad) 
				return false;
			
			#endregion

			#region AXISTEST_Y02(e0[Z], e0[X], fez, fex)

			p0 = -e0.Z * v0.X + e0.X * v0.Z;
			p2 = -e0.Z * v2.X + e0.X * v2.Z;
			if (p0 < p2) 
			{ 
				min = p0; 
				max = p2;
			}
			else
			{
				min = p2;
				max = p0;
			}
			rad = fez * boxHalfSize.X + fex * boxHalfSize.Z;
			if (min > rad || max < -rad) 
				return false;

			#endregion

			#region AXISTEST_Z12(e0[Y], e0[X], fey, fex)

			var p1 = e0.Y * v1.X - e0.X * v1.Y;
			p2 = e0.Y * v2.X - e0.X * v2.Y;
			if (p2 < p1)
			{
				min = p2;
				max = p1;
			}
			else
			{
				min = p1; 
				max = p2;
			}
			rad = fey * boxHalfSize.X + fex * boxHalfSize.Y;
			if (min > rad || max < -rad)
				return false;

			#endregion

			fex = System.Math.Abs(e1.X);
			fey = System.Math.Abs(e1.Y);
			fez = System.Math.Abs(e1.Z);

			#region AXISTEST_X01(e1[Z], e1[Y], fez, fey)
			
			p0 = e1.Z * v0.Y - e1.Y * v0.Z;
			p2 = e1.Z * v2.Y - e1.Y * v2.Z;
			if (p0 < p2)
			{
				min = p0;
				max = p2;
			}
			else
			{
				min = p2;
				max = p0;
			}
			rad = fez * boxHalfSize.Y + fey * boxHalfSize.Z;
			if (min > rad || max < -rad)
				return false;

			#endregion

			#region AXISTEST_Y02(e1[Z], e1[X], fez, fex)

			p0 = -e1.Z * v0.X + e1.X * v0.Z;
			p2 = -e1.Z * v2.X + e1.X * v2.Z;
			if (p0 < p2)
			{
				min = p0;
				max = p2;
			}
			else
			{
				min = p2;
				max = p0;
			}
			rad = fez * boxHalfSize.X + fex * boxHalfSize.Z;
			if (min > rad || max < -rad)
				return false;

			#endregion

			#region AXISTEST_Z0(e1[Y], e1[X], fey, fex)

			p0 = e1.Y * v0.X - e1.X * v0.Y;
			p1 = e1.Y * v1.X - e1.X * v1.Y;
			if (p0 < p1)
			{
				min = p0;
				max = p1;
			}
			else
			{
				min = p1;
				max = p0;
			}
			rad = fey * boxHalfSize.X + fex * boxHalfSize.Y;
			if (min > rad || max < -rad)
				return false;

			#endregion

			fex = System.Math.Abs(e2.X);
			fey = System.Math.Abs(e2.Y);
			fez = System.Math.Abs(e2.Z);

			#region AXISTEST_X2(e2[Z], e2[Y], fez, fey)

			p0 = e2.Z * v0.Y - e2.Y * v0.Z;
			p1 = e2.Z * v1.Y - e2.Y * v1.Z;
			if (p0 < p1)
			{
				min = p0;
				max = p1;
			}
			else
			{
				min = p1; 
				max = p0;
			}
			rad = fez * boxHalfSize.Y + fey * boxHalfSize.Z;
			if (min > rad || max < -rad) 
				return false;

			#endregion

			#region AXISTEST_Y1(e2[Z], e2[X], fez, fex)

			p0 = -e2.Z * v0.X + e2.X * v0.Z;
			p1 = -e2.Z * v1.X + e2.X * v1.Z;
			if (p0 < p1)
			{
				min = p0;
				max = p1;
			} 
			else 
			{
				min = p1; 
				max = p0;
			}
			rad = fez * boxHalfSize.X + fex * boxHalfSize.Z;
			if (min > rad || max < -rad) 
				return false;

			#endregion

			#region AXISTEST_Z12(e2[Y], e2[X], fey, fex)

			p1 = e2.Y * v1.X - e2.X * v1.Y;
			p2 = e2.Y * v2.X - e2.X * v2.Y;
			if (p2 < p1)
			{
				min = p2;
				max = p1;
			}
			else
			{
				min = p1;
				max = p2;
			}
			rad = fey * boxHalfSize.X + fex * boxHalfSize.Y;
			if (min > rad || max < -rad)
				return false;

			#endregion

			/*  first test overlap in the {x,y,z}-directions */
			/*  find min, max of the triangle each direction, and test for overlap in */
			/*  that direction -- this is equivalent to testing a minimal AABB around */
			/*  the triangle against the AABB */
			/* test in X-direction */
			Functions.MinMax(v0.X, v1.X, v2.X, out min, out max);
			if (min > boxHalfSize.X || max < -boxHalfSize.X)
				return false;

			/* test in Y-direction */
			Functions.MinMax(v0.Y, v1.Y, v2.Y, out min, out max);
			if (min > boxHalfSize.Y || max < -boxHalfSize.Y)
				return false;

			/* test in Z-direction */
			Functions.MinMax(v0.Z, v1.Z, v2.Z, out min, out max);
			if (min > boxHalfSize.Z || max < -boxHalfSize.Z)
				return false;

			/*  test if the box intersects the plane of the triangle */
			/*  compute plane equation of triangle: normal*x+d=0 */
			var normal = Vector3.Cross(e0, e1);
			var d = -Vector3.Dot(normal, v0);  /* plane eq: normal.x+d=0 */
			if (!PlaneAndAACell(normal, d, boxHalfSize))
				return false;

			return true;
		}

		#region AACellAndFace private

		/// <summary>
		/// Plane and Cell collision detection used by AACellAndFace
		/// 
		/// http://jgt.akpeters.com/papers/AkenineMoller01/tribox.html
		/// </summary>
		/// <param name="normal"></param>
		/// <param name="distance"></param>
		/// <param name="cellHalfSize"></param>
		/// <returns></returns>
		private static bool PlaneAndAACell(Vector3 normal, float distance, Vector3 cellHalfSize)
		{
			Vector3 vmin, vmax;

			if (normal.X > 0f)
			{
				vmin.X = -cellHalfSize.X;
				vmax.X = cellHalfSize.X;
			}
			else
			{
				vmin.X = cellHalfSize.X;
				vmax.X = -cellHalfSize.X;
			}

			if (normal.Y > 0f)
			{
				vmin.Y = -cellHalfSize.Y;
				vmax.Y = cellHalfSize.Y;
			}
			else
			{
				vmin.Y = cellHalfSize.Y;
				vmax.Y = -cellHalfSize.Y;
			}

			if (normal.Z > 0f)
			{
				vmin.Z = -cellHalfSize.Z;
				vmax.Z = cellHalfSize.Z;
			}
			else
			{
				vmin.Z = cellHalfSize.Z;
				vmax.Z = -cellHalfSize.Z;
			}

			if (Vector3.Dot(normal, vmin) + distance > 0f) 
				return false;

			if (Vector3.Dot(normal, vmax) + distance < 0f) 
				return false;

			return true;
		}

		#endregion

		/// <summary>
		/// "An Efficient and Robust Ray-Box Intersection Algorithm" - Journal of graphics tools, 10(1):49-54, 2005
		/// by Amy Williams, Steve Barrus, R. Keith Morley, and Peter Shirley
		/// 
		/// http://jgt.akpeters.com/papers/WilliamsEtAl05/
		/// </summary>
		/// <param name="ray"></param>
		/// <param name="cell"></param>
		/// <returns></returns>
		public static bool RayAndAACell(Ray ray, AACell cell)
		{
			// initial transformation, so that cell is at [0, 0, 0], also origin of ray must be moved
			var center = cell.Center;
			cell.Min -= center;
			cell.Max -= center;
			ray.Origin -= center;

			var tmin = ((ray.Sign.X == 0 ? cell.Min.X : cell.Max.X) - ray.Origin.X) * ray.InvDirection.X;
			var tmax = ((1 - ray.Sign.X == 0 ? cell.Min.X : cell.Max.X) - ray.Origin.X) * ray.InvDirection.X;

			var tymin = ((ray.Sign.Y == 0 ? cell.Min.X : cell.Max.X) - ray.Origin.Y) * ray.InvDirection.Y;
			var tymax = ((1 - ray.Sign.Y == 0 ? cell.Min.X : cell.Max.X) - ray.Origin.Y) * ray.InvDirection.Y;

			if ((tmin > tymax) || (tymin > tmax))
			{
				ray.Origin += center;
				return false;
			}

			if (tymin > tmin)
				tmin = tymin;

			if (tymax < tmax)
				tmax = tymax;

			var tzmin = ((ray.Sign.Z == 0 ? cell.Min.Z : cell.Max.Z) - ray.Origin.Z) * ray.InvDirection.Z;
			var tzmax = ((1 - ray.Sign.Z == 0 ? cell.Min.Z : cell.Max.Z) - ray.Origin.Z) * ray.InvDirection.Z;

			if ((tmin > tzmax) || (tzmin > tmax))
			{
				ray.Origin += center;
				return false;
			}


			if (tzmin > tmin)
				tmin = tzmin;

			if (tzmax < tmax)
				tmax = tzmax;

			ray.Origin += center;
			return ((tmin < float.MaxValue) && (tmax > 0f));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="point"></param>
		/// <param name="cell"></param>
		/// <returns></returns>
		public static bool PointInAACell(Vector3 point, AACell cell)
		{
			if (point.X < cell.Min.X || point.X > cell.Max.X)
				return false;

			if (point.Y < cell.Min.Y || point.Y > cell.Max.Y)
				return false;

			if (point.Z < cell.Min.Z || point.Z > cell.Max.Z)
				return false;

			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="nodeCell"></param>
		/// <param name="sphere"></param>
		/// <returns></returns>
		public static bool AACellAndSphere(AACell nodeCell, float radius)
		{
			#region Min > radius || Max < -radius

			if (nodeCell.Min.X > radius || nodeCell.Max.X < -radius)
				return false;

			if (nodeCell.Min.Y > radius || nodeCell.Max.Y < -radius)
				return false;

			if (nodeCell.Min.Z > radius || nodeCell.Max.Z < -radius)
				return false;

			#endregion

			if (nodeCell.Min.X == -radius && nodeCell.Min.Y == -radius && nodeCell.Min.Z == -radius && nodeCell.Max.X == radius && nodeCell.Max.Y == radius && nodeCell.Max.Z == radius)
				return true;

			#region Min < radius > Max

			var radius2 = radius * radius;

			var min = nodeCell.Min;
			var max = nodeCell.Max;

			if (System.Math.Abs(min.X) > System.Math.Abs(max.X))
			{
				min.X = nodeCell.Max.X;
				max.X = nodeCell.Min.X;
			}

			if (System.Math.Abs(min.Y) > System.Math.Abs(max.Y))
			{
				min.Y = nodeCell.Max.Y;
				max.Y = nodeCell.Min.Y;
			}

			if (System.Math.Abs(min.Z) > System.Math.Abs(max.Z))
			{
				min.Z = nodeCell.Max.Z;
				max.Z = nodeCell.Min.Z;
			}

			if (Vector3.DistanceSquared(min, Vector3.Zero) < radius2 && Vector3.DistanceSquared(max, Vector3.Zero) > radius2)
				return true;

			#endregion

			// TODO dorobit ostatne pripady, ked ani jeden bod nie je v guli no napriek tomu sa pretinaju

			return false;
		}

		/// <summary>
		/// Geometric method
		/// 
		/// http://www.devmaster.net/wiki/Ray-sphere_intersection
		/// </summary>
		/// <param name="ray"></param>
		/// <param name="radius"></param>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public static bool RayAndSphere(Ray ray, float radius, float from, float to)
		{
			//const Point3d distVector = position - ray->origin;
			//const real distance = distVector.GetLength2();

			//if (distance < radius2) 
			//{
			//    const real dot = Math3d::Dot(distVector, ray->direction);
			//    const real t = sqrt((radius2 - distance) + dot*dot) + dot;

			//    if (t > to || t < from)
			//        return false;

			//    return true;
			//} 

			//const real dot = Math3d::Dot(distVector, ray->direction);

			//if (dot < 0)
			//    return false;

			//const real l2hc = (radius2 - distance) + dot * dot;
			//if (l2hc < 0)
			//    return false;

			//const real t = dot - sqrt(l2hc);

			//if (t > to || t < from)
			//    return false;

			return true;
		}
	}
}
