using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Athena.Core.Tools
{
	public enum MemoryUnit
	{
		B = 1,
		kB = 2,
		MB = 3,
		GB = 4,
		TB = 5,
		PB = 6
	}

	public class FormatString
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="durationInMiliseconds"></param>
		/// <returns></returns>
		public static string GetDuration(int durationInMiliseconds)
		{
			var duration = (float)durationInMiliseconds;

			var result = "";

			// day
			if (duration > 24 * 60 * 60 * 1000)
			{
				result += ((int)duration / (24 * 60 * 60 * 1000)).ToString() + "d ";
				duration -= (int)(duration / (24 * 60 * 60 * 1000)) * (24 * 60 * 60 * 1000);
			}

			if (duration > 60 * 60 * 1000)
			{
				result += ((int)duration / (60 * 60 * 1000)).ToString() + "h ";
				duration -= (int)(duration / (60 * 60 * 1000)) * (60 * 60 * 1000);
			}

			if (duration > 60 * 1000)
			{
				result += ((int)duration / (60 * 1000)).ToString() + "m ";
				duration -= (int)(duration / (60 * 1000)) * (60 * 1000);
			}

			if (duration > 1000)
			{
				result += ((int)duration / 1000).ToString() + "s ";
				duration -= (int)(duration / 1000) * 1000;
			}

			if (duration >= 0)
				result += ((int)duration).ToString() + "ms ";

			return result.Trim();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		internal static string GetMemorySize(int memSize)
		{
			var unit = MemoryUnit.B;
			var memSizeTmp = (float)memSize;

			while (memSizeTmp > 1024f)
			{
				memSizeTmp /= 1024f;
				unit = (MemoryUnit)((int)unit + 1);
			}

			return string.Format("{0}{1}", memSize, unit.ToString());
		}
	}
}
