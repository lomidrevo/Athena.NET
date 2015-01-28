using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Athena.Core.DataTypes;

namespace Athena.Core.Materials
{
	/// <summary>
	/// 
	/// </summary>
	public class ColorModel
	{		
		public ColorRGB Diffuse { get; set; }
		public ColorRGB Emissive { get; set; }
		public ColorRGB Specular { get; set; }
		public ColorRGB Ambient { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public float SpecularStrength { get; set; }
		
		/// <summary>
		/// 
		/// </summary>
		public float AmbientFactor { get; set; }

		public ColorModel()
		{
			Diffuse = ColorRGB.White;
			Emissive = ColorRGB.Black;
			Specular = ColorRGB.Black;
			Ambient = ColorRGB.Black;

			SpecularStrength = AmbientFactor = 1;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class Material
	{
		/// <summary>
		/// 
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public ColorModel Colors = new ColorModel();

		/// <summary>
		/// 
		/// </summary>
		public float ReflectionIndex { get; set; }
		
		/// <summary>
		/// 
		/// </summary>
		public float RefractionIndex { get; set; }
	}
}
