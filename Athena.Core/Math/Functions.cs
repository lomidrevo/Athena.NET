using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Athena.Core.Math
{
	public class Functions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static int Clamp(int value, int min, int max)
		{
			return value < min ? min : (value > max ? max : value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static float Clamp(float value, float min, float max)
		{
			return value < min ? min : (value > max ? max : value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="f0"></param>
		/// <param name="f1"></param>
		/// <param name="f2"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public static void MinMax(float f0, float f1, float f2, out float min, out float max)
		{
			min = max = f0;
			if (f1 < min)
				min = f1;

			if (f1 > max)
				max = f1;

			if (f2 < min)
				min = f2;

			if (f2 > max)
				max = f2;
		}
	}
}
