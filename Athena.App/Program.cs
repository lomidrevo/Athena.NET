using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Athena.App.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;

[assembly: CLSCompliant(true)]
namespace Athena.App
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainFormMDI());
		}
	}
}
