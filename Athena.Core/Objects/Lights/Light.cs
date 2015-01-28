using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Athena.Core.DataTypes;

namespace Athena.Core.Objects.Lights
{
	public class Light : SceneObject
	{
		public ColorRGB Color { get; set; }

		public float Intensity { get; set; }
	}
}
