using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Athena.Core.DataTypes;
using Athena.Core.Tools;
using Athena.Core.Math;
using System.Drawing;
using System.Drawing.Imaging;

namespace Athena.Core.Objects.Meshes
{
	/// <summary>
	/// 
	/// </summary>
	public struct HeightMapLevel
	{
		public float From;
		public float To;
		public ColorRGB Color;
	}

	/// <summary>
	/// 
	/// </summary>
	public class Terrain : Mesh
	{
		public Terrain(float size, float maxHeight, float[] heightMap) : base()
		{
			CreateFromHeightMap(size, maxHeight, heightMap, null);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="terrainSize"></param>
		/// <param name="maxTerrainHeight"></param>
		/// <param name="heightMap"></param>
		/// <param name="textureMap"></param>
		/// <returns></returns>
		public void CreateFromHeightMap(float terrainSize, float maxTerrainHeight, float[] heightMap, ColorRGB[] textureMap)
		{
			var heightMapSize = (int)System.Math.Sqrt(heightMap.Length);
			var triangleSize = terrainSize / heightMapSize;

			var mapIndex = 0;
			var terrainSizeLimit = terrainSize * .5f;
			var actualMaxHeight = 0f;

			// if no texture map is specified, color of vertex height is used
			if (textureMap == null)
			{
				textureMap = new ColorRGB[heightMap.Length];
				for (var i = 0; i < heightMap.Length; i++)
					textureMap[i] = new ColorRGB(heightMap[i]);
			}

			#region generate vertices

			for (var z = -terrainSizeLimit + triangleSize * .5f; z < terrainSizeLimit; z += triangleSize)
				for (var x = -terrainSizeLimit + triangleSize * .5f; x < terrainSizeLimit; x += triangleSize, mapIndex++)
				{
					var vertexHeight = heightMap[mapIndex] * maxTerrainHeight;
					if (vertexHeight > actualMaxHeight)
						actualMaxHeight = vertexHeight;

					Vertices.Add(new Vector3(x, vertexHeight, z));
				}

			#endregion

			// set bounding box (center will be at [0,0,0]
			actualMaxHeight *= .5f;
			BoundingBox = new AACell 
			{
				Min = new Vector3(-terrainSizeLimit + triangleSize * .5f, -actualMaxHeight, -terrainSizeLimit + triangleSize * .5f), 
				Max = new Vector3(terrainSizeLimit - triangleSize * .5f, actualMaxHeight, terrainSizeLimit - triangleSize * .5f)
			};

			// update vertex heights(Y) (so bounding box center will be at [0,0,0])
			for (var i = 0; i < Vertices.Count; i++)
			{
				var vertex = Vertices[i];
				vertex.Y -= actualMaxHeight;
				Vertices[i] = vertex;
			}

			#region generate faces

			for (var y = 0; y < heightMapSize - 1; y++)
				for (var x = 0; x < heightMapSize - 1; x++)
				{
					var lowerLeft = x + y * heightMapSize;
					var lowerRight = (x + 1) + y * heightMapSize;
					var topLeft = x + (y + 1) * heightMapSize;
					var topRight = (x + 1) + (y + 1) * heightMapSize;

					Faces.Add(new Face(this, topLeft, lowerRight, lowerLeft, -1, -1, -1, -1, -1, -1));
					Faces.Add(new Face(this, topLeft, topRight, lowerRight, -1, -1, -1, -1, -1, -1));
				}

			#endregion
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class HeightMapGenerator
	{
		#region private variables

		private static int mapSize;
		private static float mapRoughness;
		private static Random rnd;

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="size"></param>
		/// <param name="roughness"></param>
		/// <param name="seed"></param>
		/// <returns></returns>
		public static float[] GenerateWithMidPointDisplacement(int size, float roughness, int? seed)
		{
			Log.Instance.AddMsg(LogLevel.Info, string.Format("Generating height map with mid point displacement [size: {0}x{0}, roughness: {1}, seed: {2}] ...", size, roughness.ToString(".##"), seed.HasValue ? seed.Value.ToString() : "null" ));

			rnd = seed.HasValue ? new Random(seed.Value) : new Random();
			mapRoughness = roughness;
			mapSize = size;

			// assign the four corners of the intial grid random color values
			// these will end up being the colors of the four corners
			var corner1 = (float)rnd.NextDouble();
			var corner2 = (float)rnd.NextDouble();
			var corner3 = (float)rnd.NextDouble();
			var corner4 = (float)rnd.NextDouble();

			var points = new float[mapSize * mapSize];

			DivideGrid(ref points, 0, 0, mapSize, corner1, corner2, corner3, corner4);

			return points;
		}

		#region GenerateWithMidPointDisplacement helpers

		/// <summary>
		/// 
		/// </summary>
		/// <param name="points"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="size"></param>
		/// <param name="corner1"></param>
		/// <param name="corner2"></param>
		/// <param name="corner3"></param>
		/// <param name="corner4"></param>
		private static void DivideGrid(ref float[] points, int x, int y, int size, float corner1, float corner2, float corner3, float corner4)
		{
			if (size > 1)
			{
				var newSize = size >> 1;

				// randomly displace the midpoint!
				var middle = (corner1 + corner2 + corner3 + corner4) * 0.25f + Displace(newSize + newSize);

				// calculate the edges by averaging the two corners of each edge.
				var edge1 = (corner1 + corner2) * 0.5f;
				var edge2 = (corner2 + corner3) * 0.5f;
				var edge3 = (corner3 + corner4) * 0.5f;
				var edge4 = (corner4 + corner1) * 0.5f;

				// make sure that the midpoint doesn't accidentally "randomly displaced" past the boundaries!
				middle = Functions.Clamp(middle, 0, 1);
				edge1 = Functions.Clamp(edge1, 0, 1);
				edge2 = Functions.Clamp(edge2, 0, 1);
				edge3 = Functions.Clamp(edge3, 0, 1);
				edge4 = Functions.Clamp(edge4, 0, 1);
	
				// do the operation over again for each of the four new grids.
				DivideGrid(ref points, x, y, newSize, corner1, edge1, middle, edge4);
				DivideGrid(ref points, x + newSize, y, newSize, edge1, corner2, edge2, middle);
				DivideGrid(ref points, x + newSize, y + newSize, newSize, middle, edge2, corner3, edge3);
				DivideGrid(ref points, x, y + newSize, newSize, edge4, middle, edge3, corner4);
			}

			// this is the "base case," where each grid piece is less than the size of a pixel.
			else	
				// the four corners of the grid piece will be averaged and drawn as a single pixel.
				points[x + y * mapSize] = (corner1 + corner2 + corner3 + corner4) * 0.25f;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="smallSize"></param>
		/// <returns></returns>
		private static float Displace(float smallSize)
		{
			return ((float)rnd.NextDouble() - 0.5f) * smallSize / (mapSize + mapSize) * mapRoughness;
		}

		#endregion

		/// <summary>
		/// height interval (0, 1)
		/// </summary>
		/// <param name="size"></param>
		/// <param name="frequency"></param>
		/// <param name="amplitude"></param>
		/// <param name="persistance"></param>
		/// <param name="octaves"></param>
		/// <param name="seed"></param>
		/// <returns></returns>
		public static float[] GenerateWithPerlinNoise(int size, float frequency, float amplitude, float persistance, int octaves, int? seed)
		{
			Log.Instance.AddMsg(LogLevel.Info, string.Format("Generating height map with perlin noise [size: {0}x{0}, frequency: {1}, amplitude: {2}, persistance: {3}, octaves: {4}, seed: {5}] ...", size, frequency.ToString(".####"), amplitude.ToString(".##"), persistance.ToString(".##"), octaves, seed.HasValue ? seed.Value.ToString() : "null"));

			rnd = seed.HasValue ? new Random(seed.Value) : new Random();
			mapSize = size;

			var result = new float[size * size];

		    var noise = GenerateNoise();
			for (var x = 0; x < mapSize; ++x)
		        for (var y = 0; y < mapSize; ++y)
		            result[x + y * mapSize] = GetValue(x, y, frequency, amplitude, persistance, octaves, noise) * 0.5f + 0.5f;

			return result;
		}

		#region GenerateWithPerlinNoise helpers

		/// <summary>
		/// height interval (-1, 1)
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="frequency"></param>
		/// <param name="amplitude"></param>
		/// <param name="persistance"></param>
		/// <param name="octaves"></param>
		/// <param name="noise"></param>
		/// <returns></returns>
		private static float GetValue(int x, int y, float frequency, float amplitude, float persistance, int octaves, float[] noise)
		{
			var finalValue = 0f;

			for (var i = 0; i < octaves; ++i)
			{
				finalValue += GetSmoothNoise(x * frequency, y * frequency, noise) * amplitude;
				frequency *= 2f;
				amplitude *= persistance;
			}

			return Functions.Clamp(finalValue, -1, 1);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="noise"></param>
		/// <returns></returns>
		private static float GetSmoothNoise(float x, float y, float[] noise)
		{
			var fractionX = x - (int)x;
			var fractionY = y - (int)y;

			var x1 = ((int)x + mapSize) % mapSize;
			var y1 = ((int)y + mapSize) % mapSize;
			var x2 = ((int)x + mapSize - 1) % mapSize;
			var y2 = ((int)y + mapSize - 1) % mapSize;

			var finalValue = 0f;
			finalValue += fractionX * fractionY * noise[x1 + y1 * mapSize];
			finalValue += fractionX * (1 - fractionY) * noise[x1 + y2 * mapSize];
			finalValue += (1 - fractionX) * fractionY * noise[x2 + y1 * mapSize];
			finalValue += (1 - fractionX) * (1 - fractionY) * noise[x2 + y2 * mapSize];

			return finalValue;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private static float[] GenerateNoise()
		{
			var noise = new float[mapSize * mapSize];

			for (var i = 0; i < noise.Length; ++i)
				noise[i] = ((float)rnd.NextDouble() - 0.5f) * 2.0f;

			return noise;
		}

		#endregion

		/// <summary>
		/// adds two maps together
		/// </summary>
		/// <param name="result"></param>
		/// <param name="map1"></param>
		/// <param name="map2"></param>
		public static float[] Add(float[] result, float[] map1, float[] map2)
		{
			if (result == null)
				result = new float[map1.Length];

			for (var i = 0; i < result.Length; i++)
				result[i] = map1[i] + map2[i];

			return result;
		}

		/// <summary>
		/// multiplies two maps
		/// </summary>
		/// <param name="result"></param>
		/// <param name="map1"></param>
		/// <param name="map2"></param>
		public static float[] Multiply(float[] result, float[] map1, float[] map2)
		{
			if (result == null)
				result = new float[map1.Length];

			for (var i = 0; i < result.Length; i++)
				result[i] = map1[i] * map2[i];

			return result;
		}

		/// <summary>
		/// create bitmap from heightmap
		/// </summary>
		/// <param name="heightMap"></param>
		/// <param name="size"></param>
		/// <param name="minRGBValue">0-255</param>
		/// <param name="maxRGBValue">0-255</param>
		/// <returns></returns>
		public static Bitmap CreateBitmap(float[] heightMap, int size, int minRGBValue, int maxRGBValue)
		{
			var bitmap = new Bitmap(size, size);
			var imageData = LockImage(bitmap);
			int imagePixelSize = GetPixelSize(imageData);

			for (var x = 0; x < size; ++x)
				for (var y = 0; y < size; ++y)
				{
					var RGBValue = Functions.Clamp((int)(heightMap[x + y * size] * 255), minRGBValue, maxRGBValue);
					SetPixel(imageData, x, y, Color.FromArgb(RGBValue, RGBValue, RGBValue), imagePixelSize);
				}

			UnlockImage(bitmap, imageData);

			return bitmap;
		}

		#region CreateBitmap helpers

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		internal static int GetPixelSize(BitmapData data)
		{
			if (data.PixelFormat == PixelFormat.Format24bppRgb)
				return 3;

			if (data.PixelFormat == PixelFormat.Format32bppArgb || data.PixelFormat == PixelFormat.Format32bppPArgb || data.PixelFormat == PixelFormat.Format32bppRgb)
				return 4;

			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		internal static BitmapData LockImage(Bitmap image)
		{
			return image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, image.PixelFormat);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="image"></param>
		/// <param name="ImageData"></param>
		internal static void UnlockImage(Bitmap image, BitmapData ImageData)
		{
			image.UnlockBits(ImageData);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="pixelSizeInBytes"></param>
		/// <returns></returns>
		internal static unsafe Color GetPixel(BitmapData data, int x, int y, int pixelSizeInBytes)
		{
			var dataPointer = (byte*)data.Scan0;
			dataPointer = dataPointer + (y * data.Stride) + (x * pixelSizeInBytes);
			if (pixelSizeInBytes == 3)
				return Color.FromArgb(dataPointer[2], dataPointer[1], dataPointer[0]);

			return Color.FromArgb(dataPointer[3], dataPointer[2], dataPointer[1], dataPointer[0]);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="pixelColor"></param>
		/// <param name="pixelSizeInBytes"></param>
		internal static unsafe void SetPixel(BitmapData data, int x, int y, Color pixelColor, int pixelSizeInBytes)
		{
			var dataPointer = (byte*)data.Scan0;
			dataPointer = dataPointer + (y * data.Stride) + (x * pixelSizeInBytes);

			if (pixelSizeInBytes == 3)
			{
				dataPointer[2] = pixelColor.R;
				dataPointer[1] = pixelColor.G;
				dataPointer[0] = pixelColor.B;
				return;
			}

			dataPointer[3] = pixelColor.A;
			dataPointer[2] = pixelColor.R;
			dataPointer[1] = pixelColor.G;
			dataPointer[0] = pixelColor.B;
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="heightMap"></param>
		/// <param name="heightMapLevels"></param>
		/// <returns></returns>
		public static ColorRGB[] GenerateTextureMap(float[] heightMap, int mapSize, HeightMapLevel[] heightMapLevels)
		{
			Log.Instance.AddMsg(LogLevel.Info, string.Format("Generating texture map from height map [size: {0}x{0}, heightMapLevels: {1}] ...", mapSize, heightMapLevels != null ? heightMapLevels.Length.ToString() : "null"));

			var textureMap = new ColorRGB[heightMap.Length];

			if (heightMapLevels == null)
				for (var i = 0; i < heightMap.Length; i++)
					textureMap[i] = new ColorRGB(heightMap[i], heightMap[i], heightMap[i]);

			else
				for (var i = 0; i < heightMap.Length; i++)
					textureMap[i] = GetColorByHeight(heightMap[i], i, heightMapLevels);

			return textureMap;
		}

		#region GenerateTextureMap helpers

		/// <summary>
		/// 
		/// </summary>
		/// <param name="heightMapValue"></param>
		/// <param name="textureIndex"></param>
		/// <param name="heightMapLevels"></param>
		/// <returns></returns>
		private static ColorRGB GetColorByHeight(float heightMapValue, int textureIndex, HeightMapLevel[] heightMapLevels)
		{
			for (var i = 0; i < heightMapLevels.Length - 1; i++)
			{
				if (heightMapValue < heightMapLevels[i].To && heightMapValue > heightMapLevels[i + 1].From)
				{
					var ratio = (heightMapLevels[i].To - heightMapValue) / (heightMapLevels[i].To - heightMapLevels[i + 1].From);

					return new ColorRGB(
						(heightMapLevels[i].Color.R * ratio + (1 - ratio) * heightMapLevels[i + 1].Color.R),
						(heightMapLevels[i].Color.G * ratio + (1 - ratio) * heightMapLevels[i + 1].Color.G),
						(heightMapLevels[i].Color.B * ratio + (1 - ratio) * heightMapLevels[i + 1].Color.B)
					);
				}

				if (heightMapValue > heightMapLevels[i].From && heightMapValue < heightMapLevels[i].To)
				{
					if (i > 0 && heightMapValue < heightMapLevels[i + 1].From && heightMapValue > heightMapLevels[i - 1].To)
						return heightMapLevels[i].Color;

					if (i == 0 && heightMapValue < heightMapLevels[i + 1].From)
						return heightMapLevels[i].Color;
				}
			}

			return heightMapLevels[heightMapLevels.Length - 1].Color;
		}

		#endregion
	}
}
