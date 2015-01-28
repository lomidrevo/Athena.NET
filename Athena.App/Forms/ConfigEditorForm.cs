using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Athena.Core;
using Athena.Core.Tools;
using System.Xml;
using Athena.Core.Objects;
using Athena.Core.AccStructs;
using Athena.Core.DataTypes;
using Athena.Renderer;
using OpenTK;
using Athena.Core.Objects.Lights;
using Athena.Renderer.RayTracers;
using Athena.Core.Objects.Meshes;

namespace Athena.App.Forms
{
	/// <summary>
	/// 
	/// </summary>
	public partial class ConfigEditorForm : Form
	{
		#region Rendering options

		public Vector2i OutputSize { get; private set; }
		public Vector2i NumberOfJobs { get; private set; }
		public bool WaitForOutputRedraw { get; private set; }
		public int? NumberOfThreads { get; private set; }
		public int? FramesToRender { get; private set; }
		public VSyncMode VSync { get; private set; }
		public bool SaveOutputAfterRendering { get; private set; }
		public bool Fullscreen { get; private set; }
		public string OutputImageFilename { get; set; }

		#endregion

		public string FileName
		{
			get { return fileName; }

			private set
			{
				fileName = value;
				Text = string.IsNullOrEmpty(fileName) ? "unknown configuration" : fileName;
			}
		}

		#region private variables

		private string currentXPath;

		private string fileName;

		#endregion

		#region xml represenations

		public const string CameraXml =
			"\r\n" +
			"<Camera fov=\"45\">" + "\r\n" +
				"\t<Position x=\"0\" y=\"0\" z=\"-500\" />" + "\r\n" +
				"\t<Target x=\"0\" y=\"0\" z=\"0\" />" + "\r\n" +
			"</Camera>" + "\r\n";

		public const string OutputXml =
			"\r\n" +
			"<!--" + "\r\n" +
				"\tpossible attributes:" + "\r\n" +
					"\t\t- saveAfterRendering:bool (default: false)" + "\r\n" +
					"\t\t- vSync:enum { Off, On, Adaptive } (default: Off)" + "\r\n" +	
			"-->" + "\r\n" +
			"<Output saveAfterRendering=\"false\" vSync=\"Off\">" + "\r\n" +
				"\t<Resolution width=\"1280\" height=\"720\" />" + "\r\n" +
			"</Output>" + "\r\n";

		public const string RenderingXml =
			"\r\n" +
			"<!--" + "\r\n" +
				"\tpossible attributes:" + "\r\n" +
					"\t\t- framesToRender:int (default: infinite)" + "\r\n" + 
					"\t\t- redrawMethod:enum { AfterEachFrame, AfterEachJob }" + "\r\n" +
			"-->" + "\r\n" +
			"<Rendering framesToRender=\"1\" redrawMethod=\"AfterEachFrame\">" + "\r\n" +
				"\t<Threads setAffinity=\"true\">2</Threads>" + "\r\n" +
				"\t<JobCount width=\"16\" height=\"9\" />" + "\r\n" +
				"\t<!-- JobSize and JobCount are exclusive -->" + "\r\n" +
				"\t<!-- <JobSize width=\"\" height=\"\" /> -->" + "\r\n" +
				"\r\n" +
				"\t<!--" + "\r\n" +
					"\t\tpossible attributes:" + "\r\n" +
						"\t\t\t- maxOctreeDepth:int (default: 8)" + "\r\n" + 
				"\t-->" + "\r\n" +
				"\t<RayTracer maxOctreeDepth=\"1\" />" + "\r\n" +
				"\r\n" +
			"</Rendering>" + "\r\n";

		public const string SceneXml =
			"\r\n" +
			"<Scene>" + "\r\n" +
				"\r\n" +
				"\t<!--" + "\r\n" + 
					"\t\tpossible attributes:" + "\r\n" +
						"\t\t\t- maxDepth:int (default: 8)" + "\r\n" +
						"\t\t\t- voxelSizeThreshold:int (default: 1.0)" + "\r\n" +
				"\t-->" + "\r\n" +
				"\t<VoxelOctree maxDepth=\"8\" voxelSizeThreshold=\"1\" />" + "\r\n" +
				"\r\n" +
				"\t<Objects>" + "\r\n" +
				"\t</Objects>" + "\r\n" +
			"</Scene>" + "\r\n";

		public const string SphereXml =
			"\r\n" +
			"<Sphere>" + "\r\n" +
				"\t<Position x=\"0\" y=\"0\" z=\"0\" />" + "\r\n" +
				"\t<Radius>100</Radius>" + "\r\n" +
			"</Sphere>" + "\r\n";

		#endregion

		public ConfigEditorForm(string configFilename)
		{
			InitializeComponent();

			LoadConfig(configFilename);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ConfigEditorForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			// TODO modalne okno ci chcem ulozit stary config

			this.Hide();
			e.Cancel = true;
		}

		#region configTreeView methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="child"></param>
		/// <returns></returns>
		private static int GetNodePosition(XmlNode child)
		{
			var position = 1;
			for (int i = 0; i < child.ParentNode.ChildNodes.Count; i++)
			{
				if (child.ParentNode.ChildNodes[i] == child)
					return position;

				if (child.ParentNode.ChildNodes[i].Name == child.Name)
					position++;
			}

			throw new InvalidOperationException("Child node not found in its parent's ChildNodes property.");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		private static string GetXPathToNode(XmlNode node)
		{
			if (node.ParentNode == null)
				return "";

			return string.Format(GetXPathToNode(node.ParentNode) + "/" + node.Name + "[position()=" + GetNodePosition(node) + "]");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void sceneTreeView_DoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			try
			{
				var xPath = e.Node.Tag as string;
				if (string.IsNullOrEmpty(xPath))
					return;

				var xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(Scene.Current.SerializeToString());

				var xmlContent = xmlDocument.SelectSingleNode(xPath).OuterXml;

				currentXPath = xPath;
			}
			catch (Exception ex)
			{
				Log.Instance.AddException(ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sceneContent"></param>
		private void UpdateSceneTreeView(string sceneContent)
		{
			var sceneXmlDoc = new XmlDocument();
			sceneXmlDoc.LoadXml(sceneContent);

			configTreeView.Nodes.Clear();
			configTreeView.Nodes.Add(new TreeNode(sceneXmlDoc.DocumentElement.Name) { Tag = GetXPathToNode(sceneXmlDoc.DocumentElement) });

			AddNode(sceneXmlDoc.DocumentElement, configTreeView.Nodes[0]);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="inXmlNode"></param>
		/// <param name="inTreeNode"></param>
		private void AddNode(XmlNode inXmlNode, TreeNode inTreeNode)
		{
			if (inXmlNode.HasChildNodes)
			{
				foreach (XmlNode xNode in inXmlNode.ChildNodes)
				{
					// skip string value nodes
					if (xNode.NodeType == XmlNodeType.Text)
						continue;

					var xPath = GetXPathToNode(xNode);

					var nodeName = xNode.Name;
					if (xNode.Name == "SceneObject" && xNode.Attributes != null && xNode.Attributes["p3:type"] != null)
					{
						nodeName = xNode.Attributes["p3:type"].Value;

						var xmlDoc = new XmlDocument();
						xmlDoc.LoadXml(Scene.Current.SerializeToString());

						var objectName = xmlDoc.SelectSingleNode(xPath).Attributes["name"].Value;
						nodeName += " [" + objectName + "]";
					}

					var newTreeNode = new TreeNode(nodeName) { Tag = xPath };
					inTreeNode.Nodes.Add(newTreeNode);

					AddNode(xNode, newTreeNode);
				}
			}
			else
			{
				inTreeNode.Text = (string.IsNullOrEmpty(inXmlNode.Value) ? inXmlNode.Name : inXmlNode.Value).Trim();
			}
		}

		#endregion

		#region configContextMenuStrip actions

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void addCameraToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var cursorIndex = configXmlEditBox.SelectionStart;
			configXmlEditBox.Text = configXmlEditBox.Text.Insert(cursorIndex, CameraXml);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void meshToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var cursorIndex = configXmlEditBox.SelectionStart;
			string meshFilename = null;
			try
			{
				using (var openDlg = new OpenFileDialog()
				{
					DefaultExt = "obj",
					AddExtension = true,
					InitialDirectory = Config.DefaultMeshesDirectory,
					Filter = "OBJ-files (*.obj)|*.obj"
				})
				{
					if (openDlg.ShowDialog() != DialogResult.OK)
						return;

					meshFilename = openDlg.FileName;
				}
			}
			catch (Exception ex)
			{
				Log.Instance.AddException(ex);
			}

			var xml = 
			"\r\n" +
			"<WavefrontObjMesh name=\"Box01\">" + "\r\n" +
				"\t<Position x=\"0\" y=\"0\" z=\"0\" />" + "\r\n" +
				"\t<Filename>" + meshFilename + "</Filename>" + "\r\n" +
			"</WavefrontObjMesh>" + "\r\n";

			configXmlEditBox.Text = configXmlEditBox.Text.Insert(cursorIndex, xml);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void sphereToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var cursorIndex = configXmlEditBox.SelectionStart;
			configXmlEditBox.Text = configXmlEditBox.Text.Insert(cursorIndex, SphereXml);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void omniToolStripMenuItem_Click(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void sceneToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var cursorIndex = configXmlEditBox.SelectionStart;
			configXmlEditBox.Text = configXmlEditBox.Text.Insert(cursorIndex, SceneXml);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void renderingPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var cursorIndex = configXmlEditBox.SelectionStart;
			configXmlEditBox.Text = configXmlEditBox.Text.Insert(cursorIndex, RenderingXml);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void outputToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var cursorIndex = configXmlEditBox.SelectionStart;
			configXmlEditBox.Text = configXmlEditBox.Text.Insert(cursorIndex, OutputXml);
		}

		#endregion

		#region Mesh loading with backgroundWorker

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			var worker = sender as BackgroundWorker;

			// load mesh
			worker.ReportProgress(0, "Loading mesh ...");
			var mesh = new WavefrontObjMesh(e.Argument as string);
			worker.ReportProgress(100, "Mesh loaded");

			// create voxel octree from mesh
			worker.ReportProgress(0, "Creating voxel octree ...");
			mesh.GenerateOctree(8, .1f);
			worker.ReportProgress(100, "Voxel octree created");

			e.Result = mesh;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Scene.Current.AddObject(e.Result as WavefrontObjMesh);

			//statusLabel.Text = "Idle";
			//progressBar.Visible = false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			//progressBar.Visible = true;
			//statusLabel.Text = e.UserState as string;
			//progressBar.Value = e.ProgressPercentage;
		}

		#endregion

		#region Config processing

		/// <summary>
		/// 
		/// </summary>
		internal void Parse()
		{
			var configDoc = new XmlDocument();
			configDoc.LoadXml(configXmlEditBox.Text);

			#region Log

			var logNode = configDoc.SelectSingleNode("/AthenaConfig/Log");
			if (logNode != null)
			{
				Log.Instance.MinLogLevel = logNode.Attributes["minLogLevel"] != null ? (LogLevel)Enum.Parse(typeof(LogLevel), logNode.Attributes["minLogLevel"].Value) : LogLevel.Anything;
				
				if (logNode.SelectSingleNode("Filename") != null && !string.IsNullOrEmpty(logNode.SelectSingleNode("Filename").InnerText))
					Log.Instance.Filename = logNode.SelectSingleNode("Filename").InnerText;
			}
			
			#endregion

			#region Output

			var vsyncNode = configDoc.SelectSingleNode("/AthenaConfig/Output").Attributes["vSync"];
			if (vsyncNode != null)
				VSync = (VSyncMode)Enum.Parse(typeof(VSyncMode), vsyncNode.Value);

			var saveAfterRenderingNode = configDoc.SelectSingleNode("/AthenaConfig/Output").Attributes["saveAfterRendering"];
			if (saveAfterRenderingNode != null)
				SaveOutputAfterRendering = bool.Parse(saveAfterRenderingNode.Value);

			Fullscreen = false;
			var fullscreenNode = configDoc.SelectSingleNode("/AthenaConfig/Output").Attributes["fullscreen"];
			if (fullscreenNode != null)
				Fullscreen = bool.Parse(fullscreenNode.Value);

			OutputSize = new Vector2i(Int32.Parse(configDoc.SelectSingleNode("/AthenaConfig/Output/Resolution").Attributes["width"].Value), Int32.Parse(configDoc.SelectSingleNode("/AthenaConfig/Output/Resolution").Attributes["height"].Value));

			OutputImageFilename = null;
			var outputImageFilenameNode = configDoc.SelectSingleNode("/AthenaConfig/Output/Filename");
			if (outputImageFilenameNode != null)
				OutputImageFilename = outputImageFilenameNode.Value;

			#endregion

			#region Rendering

			NumberOfThreads = null;
			if (configDoc.SelectSingleNode("/AthenaConfig/Rendering").Attributes["threads"] != null)
				NumberOfThreads = Int32.Parse(configDoc.SelectSingleNode("/AthenaConfig/Rendering").Attributes["threads"].Value);

			NumberOfJobs = new Vector2i(Int32.Parse(configDoc.SelectSingleNode("/AthenaConfig/Rendering/JobCount").Attributes["width"].Value), Int32.Parse(configDoc.SelectSingleNode("/AthenaConfig/Rendering/JobCount").Attributes["height"].Value));
			//JobSize = new Vector2i(Int32.Parse(configDoc.SelectSingleNode("/AthenaConfig/Rendering/JobSize").Attributes["width"].Value), Int32.Parse(configDoc.SelectSingleNode("/AthenaConfig/Rendering/JobSize").Attributes["height"].Value));

			FramesToRender = null;
			var framesToRenderNode = configDoc.SelectSingleNode("/AthenaConfig/Rendering").Attributes["framesToRender"];
			if (framesToRenderNode != null)
				FramesToRender = Int32.Parse(framesToRenderNode.Value);

			if (FramesToRender == 0)
				FramesToRender = null;

			WaitForOutputRedraw = false;
			var waitForOutputRedrawNode = configDoc.SelectSingleNode("/AthenaConfig/Rendering").Attributes["waitForOutputRedraw"];
			if (waitForOutputRedrawNode != null)
				WaitForOutputRedraw = bool.Parse(waitForOutputRedrawNode.Value);

			RayTracer.BackgroundColor = null;

			var backgroundNode = configDoc.SelectSingleNode("/AthenaConfig/Output/Background");
			if (backgroundNode != null)
			{
				if (backgroundNode.Attributes["useRayDirectionVector"] != null && bool.Parse(backgroundNode.Attributes["useRayDirectionVector"].Value))
					RayTracer.BackgroundColor = null;
				else if (backgroundNode.SelectSingleNode("Color") != null)
					RayTracer.BackgroundColor = ParseColorRGBNode(backgroundNode.SelectSingleNode("Color"));
				else
					RayTracer.BackgroundColor = ColorRGB.Black;
			}

			#region RayTracer

			RayTracer.MaxOctreeDepth = 8;

			var rayTracerNode = configDoc.SelectSingleNode("/AthenaConfig/Rendering/RayTracer");
			if (rayTracerNode != null)
			{
				if (rayTracerNode.Attributes["maxOctreeDepth"] != null)
					RayTracer.MaxOctreeDepth = Int32.Parse(rayTracerNode.Attributes["maxOctreeDepth"].Value);
			}

			#endregion

			#endregion

			#region Scene

			Scene.Current = null;

			var sceneNode = configDoc.SelectSingleNode("/AthenaConfig/Scene");
			if (sceneNode == null)
				throw new Exception("No scene found in configuration");

			Scene.Current = new Scene(ParseName(sceneNode));

			var sceneMaxDepth = 8;
			var sceneVoxelSizeThreshold = 1f;
			ParseVoxelOctreeNode(sceneNode.SelectSingleNode("VoxelOctree"), ref sceneMaxDepth, ref sceneVoxelSizeThreshold);

			VoxelOctree.GenerateOnMultipleThreads = false;
			var voxelOctreeNode = sceneNode.SelectSingleNode("VoxelOctree");
			if (voxelOctreeNode != null && voxelOctreeNode.Attributes["generateOnMultipleThreads"] != null)
				VoxelOctree.GenerateOnMultipleThreads = bool.Parse(voxelOctreeNode.Attributes["generateOnMultipleThreads"].Value);

			#region Camera

			var cameraNode = configDoc.SelectSingleNode("/AthenaConfig/Scene/Camera");
			if (cameraNode != null)
			{
				Scene.Current.Camera = new Camera(ParseVector3Node(cameraNode.SelectSingleNode("Position")), ParseVector3Node(cameraNode.SelectSingleNode("Target")));

				if (cameraNode.Attributes["fov"] != null)
					Scene.Current.Camera.FieldOfVision = float.Parse(cameraNode.Attributes["fov"].Value);

				Scene.Current.Camera.MovementSpeed = cameraNode.SelectSingleNode("MovementSpeed") != null ? ParseVector3Node(cameraNode.SelectSingleNode("MovementSpeed")) : new Athena.Core.DataTypes.Vector3(1);
				Scene.Current.Camera.RotationSpeed = cameraNode.SelectSingleNode("RotationSpeed") != null ? ParseVector3Node(cameraNode.SelectSingleNode("RotationSpeed")) : new Athena.Core.DataTypes.Vector3(1);
			}

			#endregion

			#region Objects

			foreach (XmlNode objNode in sceneNode.SelectSingleNode("Objects").ChildNodes)
			{
				if (objNode is XmlComment)
					continue;

				var position = ParseVector3Node(objNode.SelectSingleNode("Position"));
				var name = ParseName(objNode);

				if (objNode.Name == "WavefrontObjMesh")
				{
					#region Mesh

					var mesh = new WavefrontObjMesh(objNode.SelectSingleNode("Filename").InnerText) { Position = position, Name = name };

					if (objNode.Attributes["voxelize"] == null || bool.Parse(objNode.Attributes["voxelize"].Value))
					{
						var maxDepth = sceneMaxDepth;
						var voxelSizeThreshold = sceneVoxelSizeThreshold;
						ParseVoxelOctreeNode(objNode.SelectSingleNode("VoxelOctree"), ref maxDepth, ref voxelSizeThreshold);

						mesh.GenerateOctree(maxDepth, voxelSizeThreshold);
					}

					#endregion

					Scene.Current.AddObject(mesh);
				}

				if (objNode.Name == "Terrain")
				{
					#region Terrain

					var heightMap = null as float[];
					if (objNode.Attributes["generationMethod"].Value == "MidPointDisplacement")
					{
						var midPointDisplacementNode = objNode.SelectSingleNode("MidPointDisplacement");
						var heightMapSize = int.Parse(midPointDisplacementNode.Attributes["size"].Value);
						var roughness = float.Parse(midPointDisplacementNode.Attributes["roughness"].Value);

						int? seed = null;
						if (midPointDisplacementNode.Attributes["seed"] != null)
							seed = int.Parse(midPointDisplacementNode.Attributes["seed"].Value);

						heightMap = HeightMapGenerator.GenerateWithMidPointDisplacement(heightMapSize, roughness, seed);
					}

					var size = float.Parse(objNode.Attributes["size"].Value);
					var maxHeight = float.Parse(objNode.Attributes["maxHeight"].Value);

					var terrain = new Terrain(size, maxHeight, heightMap) { Position = position, Name = name };

					if (objNode.Attributes["voxelize"] == null || bool.Parse(objNode.Attributes["voxelize"].Value))
					{
						var maxDepth = sceneMaxDepth;
						var voxelSizeThreshold = sceneVoxelSizeThreshold;
						ParseVoxelOctreeNode(objNode.SelectSingleNode("VoxelOctree"), ref maxDepth, ref voxelSizeThreshold);

						terrain.GenerateOctree(maxDepth, voxelSizeThreshold);
					}

					#endregion

					Scene.Current.AddObject(terrain);
				}

				else if (objNode.Name == "Sphere")
				{
					#region Sphere

					var sphere = new Sphere() { Position = position, Name = name };

					sphere.Radius = float.Parse(objNode.Attributes["radius"].Value);

					if (objNode.Attributes["voxelize"] == null || bool.Parse(objNode.Attributes["voxelize"].Value))
					{
						var maxDepth = sceneMaxDepth;
						var voxelSizeThreshold = sceneVoxelSizeThreshold;
						ParseVoxelOctreeNode(objNode.SelectSingleNode("VoxelOctree"), ref maxDepth, ref voxelSizeThreshold);

						sphere.Octree = VoxelOctree.Create(sphere, maxDepth, voxelSizeThreshold);
					}

					#endregion

					Scene.Current.AddObject(sphere);
				}

				else if (objNode.Name == "OmniLight")
				{
					#region OmniLight

					var omniLight = new OmniLight(float.Parse(objNode.Attributes["radius"].Value)) { Position = position, Name = name };

					omniLight.Color = ParseColorRGBNode(objNode.SelectSingleNode("Color"));

					if (objNode.Attributes["intensity"] != null)
						omniLight.Intensity = float.Parse(objNode.Attributes["intensity"].Value);
				
					#endregion

					Scene.Current.AddObject(omniLight);
				}
			}

			#endregion

			#endregion
		}

		#region parsing helpers

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlNode"></param>
		/// <returns></returns>
		private ColorRGB ParseColorRGBNode(XmlNode xmlNode)
		{
			var color = new Athena.Core.DataTypes.ColorRGB { R = 1f, G = 1f, B = 1f };
			if (xmlNode != null)
			{
				color.R = float.Parse(xmlNode.Attributes["r"].Value);
				color.G = float.Parse(xmlNode.Attributes["g"].Value);
				color.B = float.Parse(xmlNode.Attributes["b"].Value);
			}

			return color;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objNode"></param>
		/// <returns></returns>
		private string ParseName(XmlNode objNode)
		{
			return objNode.Attributes["name"] != null ? objNode.Attributes["name"].Value : null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="vectorNode"></param>
		/// <returns></returns>
		private Core.DataTypes.Vector3 ParseVector3Node(XmlNode vectorNode)
		{
			var vector = Athena.Core.DataTypes.Vector3.Zero;
			if (vectorNode != null)
			{
				vector.X = float.Parse(vectorNode.Attributes["x"].Value);
				vector.Y = float.Parse(vectorNode.Attributes["y"].Value);
				vector.Z = float.Parse(vectorNode.Attributes["z"].Value);
			}

			return vector;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="octreeNode"></param>
		/// <param name="maxDepth"></param>
		/// <param name="voxelSizeThreshold"></param>
		private void ParseVoxelOctreeNode(XmlNode octreeNode, ref int maxDepth, ref float voxelSizeThreshold)
		{
			if (octreeNode != null)
			{
				if (octreeNode.Attributes["maxDepth"] != null)
					maxDepth = Int32.Parse(octreeNode.Attributes["maxDepth"].Value);

				if (octreeNode.Attributes["voxelSizeThreshold"] != null)
					voxelSizeThreshold = float.Parse(octreeNode.Attributes["voxelSizeThreshold"].Value);
			}
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="configFilename"></param>
		internal void LoadConfig(string configFilename)
		{
			var defaultConfig = "<AthenaConfig>\r\n</AthenaConfig>";

			configXmlEditBox.Text = string.IsNullOrEmpty(configFilename) ? defaultConfig : File.ReadAllText(configFilename);
			FileName = configFilename;
		}

		/// <summary>
		/// 
		/// </summary>
		internal void SaveConfig()
		{
			try
			{
				if (string.IsNullOrEmpty(FileName))
				{
					using (var saveFileDlg = new SaveFileDialog()
					{
						DefaultExt = Config.ConfigFileExtension,
						AddExtension = true,
						InitialDirectory = Config.DefaultConfigurationsDirectory,
						Filter = "Athena scene files (*." + Config.ConfigFileExtension + ")|*." + Config.ConfigFileExtension
					})
					{
						if (saveFileDlg.ShowDialog() == DialogResult.OK)
							FileName = saveFileDlg.FileName;
						else
							return;
					}
				}

				File.WriteAllText(FileName, configXmlEditBox.Text);
				Log.Instance.AddMsg(LogLevel.Info, string.Format("Configuration saved '{0}'", FileName));
			}
			catch (Exception ex)
			{
				Log.Instance.AddException(ex);
			}
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void configXmlEditBox_TextChanged(object sender, EventArgs e)
		{
			// text hilighting, experimental!

			var cursor = configXmlEditBox.SelectionStart;

			var nextStart = 0;
			while (true)
			{
				var commentBlockStart = configXmlEditBox.Text.IndexOf("<!--", nextStart);
				var elementStart = configXmlEditBox.Text.IndexOf("<", nextStart);
				var attributeValueStart = configXmlEditBox.Text.IndexOf("\"", nextStart);

				if (elementStart >= 0 && (elementStart < commentBlockStart || commentBlockStart < 0) && (elementStart < attributeValueStart || attributeValueStart < 0))
				{
					#region element - light green

					elementStart++;
					configXmlEditBox.Select(nextStart, elementStart - nextStart);
					configXmlEditBox.SelectionColor = Color.White;

					var elementEnd = configXmlEditBox.Text.IndexOf(" ", elementStart);
					var elementEnd2 = configXmlEditBox.Text.IndexOf(">", elementStart);

					if (elementEnd2 > 0 && (elementEnd2 < elementEnd || elementEnd < 0))
						elementEnd = elementEnd2;

					if (elementEnd > 0)
					{
						configXmlEditBox.Select(elementStart, elementEnd - elementStart);
						configXmlEditBox.SelectionColor = Color.LightGreen;
					}
					else
					{
						// nie je ukonceny element, chyba >
						break;
					}

					nextStart = elementEnd + 1;

					#endregion
				}

				else if (attributeValueStart >= 0 && (attributeValueStart < commentBlockStart || commentBlockStart < 0))
				{
					#region attribute value - yellow

					attributeValueStart++;
					configXmlEditBox.Select(nextStart, attributeValueStart - nextStart);
					configXmlEditBox.SelectionColor = Color.White;

					var attributeValueEnd = configXmlEditBox.Text.IndexOf("\"", attributeValueStart);
					if (attributeValueEnd > 0)
					{
						configXmlEditBox.Select(attributeValueStart, attributeValueEnd - attributeValueStart);
						configXmlEditBox.SelectionColor = Color.Yellow;
					}
					else
					{
						// nie je ukoncena hodnota attributu, chyba "
						break;
					}

					nextStart = attributeValueEnd + 1;

					#endregion
				}

				else if (commentBlockStart >= 0)
				{
					#region comment block

					configXmlEditBox.Select(nextStart, commentBlockStart - nextStart);
					configXmlEditBox.SelectionColor = Color.White;

					var commentBlockEnd = configXmlEditBox.Text.IndexOf("-->", commentBlockStart);
					if (commentBlockEnd > 0)
					{
						commentBlockEnd += 3;

						configXmlEditBox.Select(commentBlockStart, commentBlockEnd - commentBlockStart);
						configXmlEditBox.SelectionColor = Color.Gray;
					}
					else
					{
						// nie je ukonceny comment block, chyba -->
						break;
					}

					nextStart = commentBlockEnd + 1;

					#endregion
				}
				else
					break;
			}

			configXmlEditBox.Select(nextStart, configXmlEditBox.Text.Length - nextStart);
			configXmlEditBox.SelectionColor = Color.White;

			configXmlEditBox.Select(cursor, 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void configXmlEditBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Tab && configXmlEditBox.SelectionLength > 0)
			{
				// TODO osetrit stlacenie TABu pokial je oznacenych viacero znakov/riadkov
			}
		}
	}
}
