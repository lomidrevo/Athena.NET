using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Athena.Core.Tools;
using Athena.Core.DataTypes;
using Athena.Core.Objects.Interfaces;

namespace Athena.Core.Objects.Meshes
{
	/// <summary>
	/// 
	/// </summary>
	public class WavefrontObjMesh : Mesh
	{
		/// <summary>
		/// 
		/// </summary>
		public string Filename { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filename"></param>
		public WavefrontObjMesh(string filename) : base()
		{
			LoadFromFile(filename);
		}

		/// <summary>
		/// 
		/// </summary>
		private void LoadFromFile(string objFilename)
		{
			Filename = objFilename;

			// TODO read material file *.mtl

			var lines = File.ReadAllLines(objFilename);

			var timer = new HighPerformanceTimer();
			timer.Start();

			var min = new Vector3(float.MaxValue);
			var max = new Vector3(float.MinValue);

			for (var i = 0; i < lines.Length; i++)
			{
				var line = lines[i];

				try
				{
					if (line.StartsWith("# object"))
					{
						if (string.IsNullOrEmpty(Name))
							Name = line.Substring("# object ".Length);
					}

					// vertex
					else if (line.StartsWith("v "))
					{
						var parts = line.Substring("v  ".Length).Replace('.', ',').Split(' ');
						var vertex = new Vector3(float.Parse(parts[0], System.Globalization.NumberStyles.Float), float.Parse(parts[1]), float.Parse(parts[2]));

						if (vertex.X < min.X)
							min.X = vertex.X;

						if (vertex.Y < min.Y)
							min.Y = vertex.Y;

						if (vertex.Z < min.Z)
							min.Z = vertex.Z;

						if (vertex.X > max.X)
							max.X = vertex.X;

						if (vertex.Y > max.Y)
							max.Y = vertex.Y;

						if (vertex.Z > max.Z)
							max.Z = vertex.Z;

						Vertices.Add(vertex);
					}

					// vertex normal
					else if (line.StartsWith("vn "))
					{
						var parts = line.Substring("vn ".Length).Replace('.', ',').Split(' ');
						Normals.Add(new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2])));
					}

					// vertex texture coordinates
					else if (line.StartsWith("vt "))
					{
						var parts = line.Substring("vt ".Length).Replace('.', ',').Split(' ');
						TextureCoords.Add(new Vector2(float.Parse(parts[0]), float.Parse(parts[1])));
					}

					// face indices
					else if (line.StartsWith("f "))
					{
						var parts = line.Substring("f ".Length).Split(' ');
						var v1Parts = parts[0].Split('/');
						var v2Parts = parts[1].Split('/');
						var v3Parts = parts[2].Split('/');

						Faces.Add(
							new Face(
								this,
								int.Parse(v1Parts[0]) - 1, int.Parse(v2Parts[0]) - 1, int.Parse(v3Parts[0]) - 1,
								int.Parse(v1Parts[1]) - 1, int.Parse(v2Parts[1]) - 1, int.Parse(v3Parts[1]) - 1,
								int.Parse(v1Parts[2]) - 1, int.Parse(v2Parts[2]) - 1, int.Parse(v3Parts[2]) - 1
							)
						);
					}
				}
				catch (Exception ex)
				{
					throw new Exception(string.Format("ObjImporter failed at line='{0}'", line), ex);
				}
			}

			BoundingBox = new AACell { Min = min, Max = max };

			timer.Stop();

			Log.Instance.AddMsg(LogLevel.Info, string.Format("WavefrontObjMesh '{0}' [vertices: {1}; faces: {2}; size: {3}] loaded in {4}", Name, Vertices.Count, Faces.Count, BoundingBox.Size.ToString(), FormatString.GetDuration((int)timer.Duration)));
		}
	}
}
