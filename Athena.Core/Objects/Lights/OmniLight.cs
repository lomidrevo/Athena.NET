using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Athena.Core.DataTypes;

namespace Athena.Core.Objects.Lights
{
	/// <summary>
	/// 
	/// </summary>
	public class OmniLight : Light
	{
		public OmniLight(float radius)
		{
			BoundingBox = new AACell { Min = new Vector3(-radius), Max = new Vector3(radius) };
			Color = new ColorRGB { R = 1f, G = 1f, B = 1f };
			Intensity = 1f;
		}
	}
}
