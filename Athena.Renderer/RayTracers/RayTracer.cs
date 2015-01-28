using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Athena.Core.DataTypes;
using Athena.Core;
using Athena.Core.Objects;
using Athena.Core.Objects.Lights;
using Athena.Core.Objects.Interfaces;

namespace Athena.Renderer.RayTracers
{
	// TODO debug vizualizacia pri ktorej je farba bodu na zaklade poctu vnoreni do octree

	/// <summary>
	/// 
	/// </summary>
	public class RayTracer
	{
		#region properties

		/// <summary>
		/// 
		/// </summary>
		public static int MaxOctreeDepth { get; set; }

		/// <summary>
		/// instead of voxel colors shows voxel normals as color
		/// </summary>
		public static bool VoxelNormalDebugging { get; set; }

		/// <summary>
		/// color of background (when no object is hit)
		/// </summary>
		public static ColorRGB? BackgroundColor { get; set; }

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ray"></param>
		/// <param name="closestResult"></param>
		/// <returns></returns>
		public static ColorRGB Trace(Ray ray)
		{
			var closestResult = new IntersectionResult();

			#region find closest intersection (closestResult)
			
			var objects = Scene.Current.Objects;
			foreach (var obj in objects)
			{
				var result = Intersection.RayAndObject(ray, obj, MaxOctreeDepth);
				if (result.Distance < closestResult.Distance)
					closestResult = result;
			}

			#endregion

			// if nothing was hit
			if (closestResult.Distance == float.MaxValue)
			{
				#region color of background

				if (BackgroundColor.HasValue)
				{
					ray.Color = BackgroundColor.Value;
				}
				else
				{
					ray.Color.R = ray.Direction.X * .5f + .5f;
					ray.Color.G = ray.Direction.Y * .5f + .5f;
					ray.Color.B = ray.Direction.Z * .5f + .5f;
				}

				#endregion
			}
			else
			{
				// if light was hit
				if (closestResult.obj is Light)
				{
					ray.Color = (closestResult.obj as Light).Color;
				}
				// object was hit
				else
				{
					if (closestResult.obj is Sphere && (closestResult.obj as IVoxelizable).Octree == null)
					{
						#region compute collision point and normal for sphere

						closestResult.Point = ray.Direction * closestResult.Distance + ray.Origin;
						closestResult.Voxel = new Voxel(ColorRGB.White, (closestResult.Point - closestResult.obj.Position).Normalize());

						#endregion
					}
					else
					{
						closestResult.Point += closestResult.obj.Position;
					}

					if (VoxelNormalDebugging)
					{
						#region color based on normal

						ray.Color.R = closestResult.Voxel.Normal.X * .5f + .5f;
						ray.Color.G = closestResult.Voxel.Normal.Y * .5f + .5f;
						ray.Color.B = closestResult.Voxel.Normal.Z * .5f + .5f;

						#endregion
					}
					else
					{
						// transform color from byte representation in voxel (0-255) to float (0-1)
						ray.Color = new ColorRGB(closestResult.Voxel);

						#region shading

						// TODO shadows - check occlusion with light sources
						var lights = (from o in objects where o is Light select o).ToList();
						foreach (var light in lights)
						{

						}

						#endregion
					}
				}
			}

			return ray.Color;
		}
	}
}
