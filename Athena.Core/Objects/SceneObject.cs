using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Athena.Core.Objects.Lights;
using Athena.Core.DataTypes;

namespace Athena.Core.Objects
{
	// TODO material do SceneObject

	/// <summary>
	/// 
	/// </summary>
	public abstract class SceneObject
	{
		/// <summary>
		/// 
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Vector3 Position { get; set; }

		/// <summary>
		/// 
		/// </summary>
		//public Vector3 Scale { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public AACell BoundingBox { get; set; }

		/// <summary>
		/// 
		/// </summary>
		protected SceneObject()
		{

		}
	}
}
