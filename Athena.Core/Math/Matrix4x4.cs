using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Athena.Core.DataTypes;

namespace Athena.Core.Math
{
	public enum MatrixIndices
	{ 
		TX = 3, 
		TY = 7, 
		TZ = 11, 
		D0 = 0, 
		D1 = 5, 
		D2 = 10, 
		D3 = 15, 
		SX = D0, 
		SY = D1, 
		SZ = D2, 
		W = D3 
	};

	public class Matrix4x4
	{
		public float[] Box { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Matrix4x4()
		{
			Box = new float[16];

			Identity();
		}

		/// <summary>
		/// 
		/// </summary>
		public void Identity()
		{
			Box[1]  = Box[2]  = Box[(int)MatrixIndices.TX] = 
			Box[4]  = Box[6]  = Box[(int)MatrixIndices.TY] =
			Box[8]  = Box[9]  = Box[(int)MatrixIndices.TZ] = 
			Box[12] = Box[13] = Box[14] = 0;

			Box[(int)MatrixIndices.D0] = Box[(int)MatrixIndices.D1] = Box[(int)MatrixIndices.D2] = Box[(int)MatrixIndices.W] = 1;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="rot"></param>
		public void Rotate(Vector3 pos, Vector3 rot)
		{
			var t = new Matrix4x4();

			t.RotateX(rot.Z);
			RotateY(rot.Y);
			Concatenate(t);
			t.RotateZ(rot.X);
			Concatenate(t);
			Translate(pos);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pos"></param>
		public void Translate(Vector3 pos)
		{
			Box[(int)MatrixIndices.TX] += pos.X;
			Box[(int)MatrixIndices.TY] += pos.Y;
			Box[(int)MatrixIndices.TZ] += pos.Z;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="m2"></param>
		public void Concatenate(Matrix4x4 m2)
		{
			var res = new Matrix4x4();

			for (var c = 0; c < 4; c++)
				for (var r = 0; r < 4; r++)
				{
					res.Box[r * 4 + c] = 
						Box[r * 4] * m2.Box[c] + 
						Box[r * 4 + 1] * m2.Box[c + 4] +
						Box[r * 4 + 2] * m2.Box[c + 8] +
						Box[r * 4 + 3] * m2.Box[c + 12];
				}

				for (var c = 0; c < 16; c++)
					Box[c] = res.Box[c];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		public Vector3 Transform(Vector3 v)
		{
			return new Vector3(Box[0] * v.X + Box[1] * v.Y + Box[2] * v.Z + Box[3], Box[4] * v.X + Box[5] * v.Y + Box[6] * v.Z + Box[7], Box[8] * v.X + Box[9] * v.Y + Box[10] * v.Z + Box[11]);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rx"></param>
		public void RotateX(float rx)
		{
			Identity();

			var sx = (float)System.Math.Sin(rx * (float)System.Math.PI / 180f);
			
			Box[5] = Box[10] = (float)System.Math.Cos(rx * (float)System.Math.PI / 180f);
			Box[6] = sx;
			Box[9] = -sx;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ry"></param>
		public void RotateY(float ry)
		{
			Identity();

			var sy = (float)System.Math.Sin(ry * (float)System.Math.PI / 180f);
			Box[0] = Box[10] = (float)System.Math.Cos(ry * (float)System.Math.PI / 180f);
			Box[2] = -sy;
			Box[8] = sy;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rz"></param>
		public void RotateZ(float rz)
		{
			Identity();

			var sz = (float)System.Math.Sin(rz * (float)System.Math.PI / 180f);
			Box[0] = Box[5] = (float)System.Math.Cos(rz * (float)System.Math.PI / 180f);
			Box[1] = sz;
			Box[4] = -sz;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Invert()
		{
			var t = new Matrix4x4();

			var tx = -Box[3];
			var ty = -Box[7];
			var tz = -Box[11];

			for (var h = 0; h < 3; h++) 
				for (var v = 0; v < 3; v++) 
					t.Box[h + v * 4] = Box[v + h * 4];

			for (var i = 0; i < 11; i++) 
				Box[i] = t.Box[i];

			Box[3]  = tx * Box[0] + ty * Box[1] + tz * Box[2];
			Box[7]  = tx * Box[4] + ty * Box[5] + tz * Box[6];
			Box[11] = tx * Box[8] + ty * Box[9] + tz * Box[10];
		}
	}
}
