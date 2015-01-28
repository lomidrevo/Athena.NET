using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Athena.Core.AccStructs;
using Athena.Core.DataTypes;

namespace Athena.Core.Objects.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IVoxelizable
	{
		/// <summary>
		/// 
		/// </summary>
		AACell BoundingBox { get; set; }

		/// <summary>
		/// 
		/// </summary>
		VoxelOctree Octree { get; set; }
	}
}
