using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Athena.Core.Tools
{
	public class DllImports
	{
		[DllImport("Kernel32.dll")]
		[return: MarshalAs(UnmanagedType.U1)]
		public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

		[DllImport("Kernel32.dll")]
		[return: MarshalAs(UnmanagedType.U1)]
		public static extern bool QueryPerformanceFrequency(out long lpFrequency);

		[DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
		public static extern void CopyMemory(IntPtr Destination, IntPtr Source, uint Length);

		[DllImport("kernel32")]
		public static extern int GetCurrentThreadId();
	}
}
