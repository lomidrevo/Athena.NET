using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Athena.Core.DataTypes
{
	[FlagsAttribute]
	public enum VoxelFlags : byte
	{
		NotEmpty = 1,
	};

	/// <summary>
	/// Colored voxel
	/// </summary>
	public struct Voxel
	{
		/// <summary>
		/// 1 - if 0, voxel is empty
		/// 2
		/// 4
		/// 8
		/// 16 
		/// 32
		/// 64
		/// 128
		/// </summary>
		public byte Flags { get; set; }

		public byte R { get; set; }
		public byte G { get; set; }
		public byte B { get; set; }

		public Vector3 Normal { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public bool IsEmpty { get { return !((VoxelFlags)Flags).HasFlag(VoxelFlags.NotEmpty); } }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <param name="startIndex"></param>
		public Voxel(byte[] array, int startIndex) : this()
		{
			Flags = array[startIndex];
			R = array[startIndex + 1];
			G = array[startIndex + 2];
			B = array[startIndex + 3];

			Normal = new Vector3 
			{ 
				X = (((int)array[startIndex + 4] - 127) / 127f), 
				Y = (((int)array[startIndex + 5] - 127) / 127f), 
				Z = (((int)array[startIndex + 6] - 127) / 127f) 
			};
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="color"></param>
		/// <param name="normal"></param>
		public Voxel(ColorRGB color, Vector3 normal) : this()
		{
			Flags = (byte)VoxelFlags.NotEmpty;
			R = (byte)(color.R * 255f);
			G = (byte)(color.G * 255f);
			B = (byte)(color.B * 255f);
			Normal = normal;
		}
		
		#region serialization

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public byte[] ToArray()
		{
			var array = new byte[Voxel.SizeOf];
	
			array[0] = Flags;
			
			array[1] = R;
			array[2] = G;
			array[3] = B;

			array[4] = (byte)((int)(Normal.X * 127f) + 127);
			array[5] = (byte)((int)(Normal.Y * 127f) + 127);
			array[6] = (byte)((int)(Normal.Z * 127f) + 127);

			return array;
		}

		#endregion

		#region static properties

		public static int SizeOf { get { return /*Marshal.SizeOf(typeof(Voxel));*/ 1/*flags*/ + 3/*color*/ + 3/*normal*/; } }

		/// <summary>
		/// empty voxel
		/// </summary>
		public readonly static Voxel Empty = new Voxel();

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		internal Vector3 ColorAsVector3()
		{
			return new Vector3(R, G, B);
		}
	}
}
