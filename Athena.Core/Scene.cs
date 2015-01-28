using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using Athena.Core.Objects;
using Athena.Core.DataTypes;
using Athena.Core.Objects.Lights;
using Athena.Core.Tools;
using System.Collections.Concurrent;
using Athena.Core.AccStructs;

namespace Athena.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class Scene
	{
		public static Scene Current { get; set; }

		public Camera Camera { get; set; } 
		public ConcurrentBag<SceneObject> Objects { get; set; }

		public string Name { get; set; }

		public Scene()
		{
			Objects = new ConcurrentBag<SceneObject>();
		}

		public Scene(string name) : this()
		{
			if (string.IsNullOrEmpty(name))
				name = "scene-0";

			Name = name;
		}

		/// <summary>
		/// add new object to collection (check name collision first)
		/// </summary>
		/// <param name="newObject"></param>
		/// <returns>object added to collection</returns>
		public SceneObject AddObject(SceneObject newObject)
		{
			// check name collision
			if (string.IsNullOrEmpty(newObject.Name))
				newObject.Name = newObject.GetType().Name.ToLower() + "-" + NextId(newObject.GetType());

			else if ((from o in Objects where o.Name == newObject.Name select o).Any())
				throw new Exception("name conflict in object list! name='" + newObject.Name + "'");

			Log.Instance.AddMsg(LogLevel.Info, string.Format("Object '{0}' added to scene '{1}'", newObject.Name, Name));

			Objects.Add(newObject);

			return newObject;
		}

		/// <summary>
		/// checks & prepares scene before rendering
		/// </summary>
		/// <param name="scene"></param>
		public void PrepareSceneForRendering()
		{
			if (Camera == null)
				throw new ArgumentNullException("No camera found in scene");

			foreach (var sceneObj in Objects)
			{

			}
		}

		//private static int counter = 0;

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private string NextId(Type objType)
		{
			return (from o in Objects where o.GetType() == objType select o).Count().ToString();

			//return (++counter).ToString();
			//return Guid.NewGuid().ToString();
		}

		#region Serialization

		/// <summary>
		/// 
		/// </summary>
		/// <param name="outputFile"></param>
		public void SerializeToFile(string outputFile)
		{
			var settings = new XmlWriterSettings 
			{ 
				OmitXmlDeclaration = true, 
				Indent = true 
			};

			var ns = new XmlSerializerNamespaces();
			ns.Add("", "");

			using (var xw = XmlWriter.Create(outputFile, settings))
				new XmlSerializer(typeof(Scene)).Serialize(xw, this, ns);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="outputFile"></param>
		public string SerializeToString()
		{
			var settings = new XmlWriterSettings 
			{ 
				OmitXmlDeclaration = true, 
				Indent = true
			};

			var ns = new XmlSerializerNamespaces();
			ns.Add("", "");

			var stringBuilder = new StringBuilder();
			using (var xw = XmlWriter.Create(stringBuilder, settings))
				new XmlSerializer(typeof(Scene)).Serialize(xw, this, ns);

			return stringBuilder.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="inputFile"></param>
		/// <returns></returns>
		public static Scene DeserializeFromFile(string inputFile)
		{
			using (var streamReader = new StreamReader(inputFile))
				return new XmlSerializer(typeof(Scene)).Deserialize(streamReader) as Scene;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		public static Scene DeserializeFromContent(string content)
		{
			using (var stringReader = new StringReader(content))
				return new XmlSerializer(typeof(Scene)).Deserialize(stringReader) as Scene;
		}

		#endregion
	}
}
