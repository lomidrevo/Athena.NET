using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Athena.Core.Objects;
using System.Runtime.InteropServices;

[assembly: CLSCompliant(true)]
namespace Athena.Core.DataTypes
{
	/// <summary>
	/// 
	/// </summary>
	public struct ColorRGB
	{
		[XmlAttribute("r")]
		public float R;

		[XmlAttribute("g")]
		public float G;

		[XmlAttribute("b")]
		public float B;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="color"></param>
		public ColorRGB(float color) : this()
		{
			R = G = B = color;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="argb"></param>
		public ColorRGB(int argb)
			: this()
		{
			R = (float)(argb & 0x00ff0000 >> 32) / 255;
			G = (float)(argb & 0x0000ff00 >> 8) / 255;
			B = (float)(argb & 0x000000ff) / 255;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="r"></param>
		/// <param name="g"></param>
		/// <param name="b"></param>
		public ColorRGB(float r, float g, float b) : this()
		{
			R = r;
			G = g;
			B = b;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="voxel"></param>
		public ColorRGB(Voxel voxel)
		{
			var div = 1f / 255f;
			R = (float)voxel.R * div;
			G = (float)voxel.G * div;
			B = (float)voxel.B * div;
		}

		#region operators

		/// <summary>
		/// 
		/// </summary>
		/// <param name="left"></param>
		/// <param name="scale"></param>
		/// <returns></returns>
		public static ColorRGB operator /(ColorRGB left, float scale)
		{
			left.R /= scale;
			left.G /= scale;
			left.B /= scale;
			return left;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="left"></param>
		/// <param name="scale"></param>
		/// <returns></returns>
		public static ColorRGB operator *(ColorRGB left, float scale)
		{
			left.R *= scale;
			left.G *= scale;
			left.B *= scale;
			return left;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static ColorRGB operator *(ColorRGB left, ColorRGB right)
		{
			left.R *= right.R;
			left.G *= right.G;
			left.B *= right.B;
			return left;
		}

		#endregion

		public static readonly ColorRGB Black = new ColorRGB(0f);
		public static readonly ColorRGB White = new ColorRGB(1f);

		public static readonly ColorRGB Red = new ColorRGB(1f, 0f, 0f);
		public static readonly ColorRGB Green = new ColorRGB(0f, 1f, 0f);
		public static readonly ColorRGB Blue = new ColorRGB(0f, 0f, 1f);

		public override string ToString()
		{
			return string.Format("R={0} G={1} B={2}", R.ToString(".##"), G.ToString(".##"), B.ToString(".##"));
		}

		public ColorRGB Inv()
		{
			return new ColorRGB(1f / R, 1f / G, 1f / B);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public struct Vector2i
	{
		public Vector2i(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		[XmlAttribute("x")]
		public int X;

		[XmlAttribute("y")]
		public int Y;
	}

	/// <summary>
	/// 2d float vector
	/// </summary>
	public struct Vector2
	{
		public Vector2(float x, float y)
		{
			this.X = x;
			this.Y = y;
		}

		[XmlAttribute("x")]
		public float X;

		[XmlAttribute("y")]
		public float Y;
	}

	/// <summary>
	/// 3d float vector
	/// </summary>
	public struct Vector3
	{
		#region constructors

		public Vector3(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public Vector3(float value)
		{
			X = Y = Z = value;
		}

		public Vector3(byte[] array, int startIndex)
		{
			X = System.BitConverter.ToSingle(array, startIndex);
			Y = System.BitConverter.ToSingle(array, startIndex + sizeof(float));
			Z = System.BitConverter.ToSingle(array, startIndex + (2 * sizeof(float)));
		}

		#endregion

		[XmlAttribute("x")]
		public float X;

		[XmlAttribute("y")]
		public float Y;

		[XmlAttribute("z")]
		public float Z;

		#region Operators

		/// <summary>
		/// 
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Vector3 operator +(Vector3 left, Vector3 right)
		{
			left.X += right.X;
			left.Y += right.Y;
			left.Z += right.Z;
			return left;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Vector3 operator -(Vector3 left, Vector3 right)
		{
			left.X -= right.X;
			left.Y -= right.Y;
			left.Z -= right.Z;
			return left;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Vector3 operator *(Vector3 left, Vector3 right)
		{
			left.X *= right.X;
			left.Y *= right.Y;
			left.Z *= right.Z;
			return left;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Vector3 operator /(Vector3 left, Vector3 right)
		{
			left.X /= right.X;
			left.Y /= right.Y;
			left.Z /= right.Z;
			return left;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="left"></param>
		/// <param name="scale"></param>
		/// <returns></returns>
		public static Vector3 operator *(Vector3 left, float scale)
		{
			left.X *= scale;
			left.Y *= scale;
			left.Z *= scale;
			return left;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="left"></param>
		/// <param name="scale"></param>
		/// <returns></returns>
		public static Vector3 operator /(Vector3 left, float scale)
		{
			left.X /= scale;
			left.Y /= scale;
			left.Z /= scale;
			return left;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="vec"></param>
		/// <returns></returns>
		public static Vector3 operator -(Vector3 vec)
		{
			vec.X = -vec.X;
			vec.Y = -vec.Y;
			vec.Z = -vec.Z;
			return vec;
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Vector3 Normalize()
		{
			this *= 1f / this.Length;
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Vector3 Invert()
		{
			X = 1f / X;
			Y = 1f / Y;
			Z = 1f / Z;

			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="center"></param>
		/// <param name="axis"></param>
		/// <param name="angle"></param>
		public Vector3 RotateAround(Vector3 center, Vector3 axis, float angle)
		{
			this -= center;
	
			var cosTheta = (float)System.Math.Cos(angle);
			var sinTheta = (float)System.Math.Sin(angle);

			var zSin = axis.Z * sinTheta;
			var xSin = axis.X * sinTheta;
			var ySin = axis.Y * sinTheta;
			var xzAxis = axis.X * axis.Z;
			var xyAxis = axis.X * axis.Y;
			var yzAxis = axis.Y * axis.Z;

			var result = new Vector3((cosTheta + (1 - cosTheta) * axis.X * axis.X) * this.X, ((1 - cosTheta) * xyAxis + zSin) * this.X, ((1 - cosTheta) * xzAxis - ySin) * this.X);

			result.X += ((1 - cosTheta) * xyAxis - zSin) * this.Y;
			result.X += ((1 - cosTheta) * xzAxis + ySin) * this.Z;

			result.Y += (cosTheta + (1 - cosTheta) * axis.Y * axis.Y) * this.Y;
			result.Y += ((1 - cosTheta) * yzAxis - xSin) * this.Z;

			result.Z += ((1 - cosTheta) * yzAxis + xSin) * this.Y;
			result.Z += (cosTheta + (1 - cosTheta) * axis.Z * axis.Z) * this.Z;

			return result + center;
		}

		/// <summary>
		/// 
		/// </summary>
		public float Length
		{
			get
			{
				return (float)System.Math.Sqrt((float)(X * X + Y * Y + Z * Z));
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public float LengthSquared
		{
			get
			{
				return X * X + Y * Y + Z * Z;
			}
		}

		#region static methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		/// <returns></returns>
		public static Vector3 Cross(Vector3 v1, Vector3 v2)
		{
			return new Vector3(v1.Y * v2.Z - v1.Z * v2.Y, v1.Z * v2.X - v1.X * v2.Z, v1.X * v2.Y - v1.Y * v2.X);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		/// <returns></returns>
		public static float Dot(Vector3 v1, Vector3 v2)
		{
			return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		/// <returns></returns>
		public static float Distance(Vector3 v1, Vector3 v2)
		{
			return (float)System.Math.Sqrt((float)((v1.X - v2.X) * (v1.X - v2.X) + (v1.Y - v2.Y) * (v1.Y - v2.Y) + (v1.Z - v2.Z) * (v1.Z - v2.Z)));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		/// <returns></returns>
		public static float DistanceSquared(Vector3 v1, Vector3 v2)
		{
			return (v1.X - v2.X) * (v1.X - v2.X) + (v1.Y - v2.Y) * (v1.Y - v2.Y) + (v1.Z - v2.Z) * (v1.Z - v2.Z);
		}

		#endregion

		public static readonly Vector3 Zero = new Vector3(0, 0, 0);

		public static readonly Vector3 UnitX = new Vector3(1, 0, 0);

		public static readonly Vector3 UnitY = new Vector3(0, 1, 0);

		public static readonly Vector3 UnitZ = new Vector3(0, 0, 1);

		public static readonly Vector3 UnitXNegative = new Vector3(-1, 0, 0);

		public static readonly Vector3 UnitYNegative = new Vector3(0, -1, 0);

		public static readonly Vector3 UnitZNegative = new Vector3(0, 0, -1);

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("X={0} Y={1} Z={2}", X, Y, Z); 
		}

		#region Serialization

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		internal byte[] ToArray()
		{
			var array = new byte[3 * sizeof(float)];
			
			var tmpArray = System.BitConverter.GetBytes(X);
			array[0] = tmpArray[0]; array[1] = tmpArray[1]; array[2] = tmpArray[2]; array[3] = tmpArray[3];

			tmpArray = System.BitConverter.GetBytes(Y);
			array[4] = tmpArray[0]; array[5] = tmpArray[1]; array[6] = tmpArray[2]; array[7] = tmpArray[3];

			tmpArray = System.BitConverter.GetBytes(Z);
			array[8] = tmpArray[0]; array[9] = tmpArray[1]; array[10] = tmpArray[2]; array[11] = tmpArray[3];

			return array;
		}

		#endregion

		#region Static properties

		/// <summary>
		/// 
		/// </summary>
		public static int SizeOf { get { return Marshal.SizeOf(typeof(Vector3)); } }

		#endregion
	}

	/// <summary>
	/// 3d integer vector
	/// </summary>
	public struct Vector3i
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="vector"></param>
		public Vector3i(Vector3 vector)
		{
			X = (int)vector.X;
			Y = (int)vector.Y;
			Z = (int)vector.Z;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public Vector3i(int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="voxel"></param>
		public void Add(Voxel voxel)
		{
			X += voxel.R;
			Y += voxel.G;
			Z += voxel.B;
		}

		[XmlAttribute("x")]
		public int X;

		[XmlAttribute("y")]
		public int Y;

		[XmlAttribute("z")]
		public int Z;
	}

	/// <summary>
	/// 
	/// </summary>
	public class Face
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mesh"></param>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		/// <param name="v3"></param>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <param name="t3"></param>
		/// <param name="n1"></param>
		/// <param name="n2"></param>
		/// <param name="n3"></param>
		public Face(Mesh mesh, int v1, int v2, int v3, int t1, int t2, int t3, int n1, int n2, int n3)
		{
			ParentMesh = mesh;

			VertexIndices = new int[3] { v1, v2, v3 };
			TextureCoordIndices = new int[3] { t1, t2, t3 };
			NormalIndices = new int[3] { n1, n2, n3 };

			Normal = Vector3.Cross(ParentMesh.Vertices[v3] - ParentMesh.Vertices[v2], ParentMesh.Vertices[v1] - ParentMesh.Vertices[v2]).Normalize();
		}

		public int[] VertexIndices { get; private set; }

		public int[] TextureCoordIndices { get; private set; }

		public int[] NormalIndices { get; private set; }

		public Vector3 Normal { get; private set; }

		public Mesh ParentMesh { get; private set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public struct AACell
	{
		public Vector3 Min;

		public Vector3 Max;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <param name="startIndex"></param>
		public AACell(byte[] array, int startIndex)
		{
			Min = new Vector3(array, startIndex);
			Max = new Vector3(array, startIndex + Vector3.SizeOf);
		}

		/// <summary>
		/// 
		/// </summary>
		public Vector3 Size 
		{ 
			get { return new Vector3(Max.X - Min.X, Max.Y - Min.Y, Max.Z - Min.Z); }
		}

		/// <summary>
		/// 
		/// </summary>
		public Vector3 Center
		{
			get { return new Vector3(Min.X + (Max.X - Min.X) * 0.5f, Min.Y + (Max.Y - Min.Y) * 0.5f, Min.Z + (Max.Z - Min.Z) * 0.5f); }
		}

		/// <summary>
		/// 
		/// </summary>
		public static int SizeOf { get { return Vector3.SizeOf * 2; } }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		internal byte[] ToArray()
		{
			var array = new byte[Vector3.SizeOf * 2];

			Min.ToArray().CopyTo(array, 0);
			Max.ToArray().CopyTo(array, Vector3.SizeOf);

			return array;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class Ray
	{
		public Vector3 Origin;
		public Vector3 Direction;
		public Vector3 InvDirection;
		public Vector3i Sign;

		public Vector2i Pixel;
		public ColorRGB Color;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="origin"></param>
		/// <param name="direction"></param>
		public void Prepare(Vector3 origin, Vector3 direction)
		{
			Origin = origin;
			Direction = direction;

			InvDirection.X = 1f / Direction.X;
			InvDirection.Y = 1f / Direction.Y;
			InvDirection.Z = 1f / Direction.Z;

			Sign.X = InvDirection.X < 0 ? 1 : 0;
			Sign.Y = InvDirection.Y < 0 ? 1 : 0;
			Sign.Z = InvDirection.Z < 0 ? 1 : 0;

			// color based on ray direction
			//Color.R = (Direction.X * .5f + .5f);
			//Color.G = (Direction.Y * .5f + .5f);
			//Color.B = (Direction.Z * .5f + .5f);

			Color = ColorRGB.Black;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class IntersectionResult
	{
		public Voxel Voxel;
		public Vector3 Point;

		public float Distance;
		public SceneObject obj;

		public IntersectionResult()
		{
			Distance = float.MaxValue;
		}
	}
}
