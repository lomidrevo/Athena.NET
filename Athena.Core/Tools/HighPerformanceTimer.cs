using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace Athena.Core.Tools
{
	public class HighPerformanceTimer
	{
		private long startTime, stopTime, duration;
		private long freq;

		public HighPerformanceTimer()
		{
			startTime = stopTime = duration = freq = 0;

			if (!DllImports.QueryPerformanceFrequency(out freq))
				throw new NotSupportedException("HighPerformanceTimer");
		}

		public void Start()
		{
			DllImports.QueryPerformanceCounter(out startTime);
			duration = 0;
		}

		public void Stop()
		{
			DllImports.QueryPerformanceCounter(out stopTime);
			duration += stopTime - startTime;
		}

		public void Pause()
		{
			DllImports.QueryPerformanceCounter(out stopTime);
			duration += stopTime - startTime;
		}

		public void Continue()
		{
			DllImports.QueryPerformanceCounter(out startTime);
		}

		/// <summary>
		/// Gets duration in miliseconds
		/// </summary>
		public double Duration
		{
			get { return (double)duration / (double)freq * 1000; }
		}
	}
}
