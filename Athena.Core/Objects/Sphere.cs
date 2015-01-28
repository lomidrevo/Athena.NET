using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Athena.Core.DataTypes;
using System.Xml.Serialization;
using Athena.Core.Objects.Interfaces;
using Athena.Core.AccStructs;

namespace Athena.Core.Objects
{
	/// <summary>
	/// 
	/// </summary>
	public class Sphere : SceneObject, IVoxelizable
	{
		/// <summary>
		/// 
		/// </summary>
		public float Radius
		{
			get { return radius; }

			set
			{
				radius = value;
				BoundingBox = new AACell { Min = new Vector3(-Radius), Max = new Vector3(Radius) };
			}
		}
		private float radius;

		#region IVoxelizable Members

		public VoxelOctree Octree { get; set; }

		#endregion

		/// <summary>
		/// 
		/// </summary>
		public Sphere() : base()
		{

		}
	}
}
