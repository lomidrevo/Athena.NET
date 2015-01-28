using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;

namespace Athena.Core.Tools
{
	public class Config
	{
		public static string DefaultConfigurationsDirectory
		{
			get
			{
				var dir = ConfigurationManager.AppSettings["DefaultConfigurationsDirectory"];
				if (!string.IsNullOrEmpty(dir))
					return dir;

				return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
			}
		}

		public static string DefaultOutputDirectory
		{
			get
			{
				var dir = ConfigurationManager.AppSettings["DefaultOutputDirectory"];
				if (!string.IsNullOrEmpty(dir))
					return dir;

				return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
			}
		}

		public static string DefaultLogDirectory
		{
			get
			{
				var dir = ConfigurationManager.AppSettings["DefaultLogDirectory"];
				if (!string.IsNullOrEmpty(dir))
					return dir;

				return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
			}
		}

		public static string DefaultMeshesDirectory
		{
			get
			{
				var dir = ConfigurationManager.AppSettings["DefaultMeshesDirectory"];
				if (!string.IsNullOrEmpty(dir))
					return dir;

				return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
			}
		}

		public static string ConfigFileExtension = "acfg";
	}
}
