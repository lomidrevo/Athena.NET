using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using Athena.Core.Tools;
using Athena.Core.DataTypes;
using System.Diagnostics;
using Athena.Core;
using System.Collections.Concurrent;
using Athena.Renderer.RayTracers;

namespace Athena.Renderer
{
	public class RenderingThread
	{
		#region static properties

		/// <summary>
		/// 
		/// </summary>
		public static bool ThreadDebugging { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public static bool JobDebugging { get; set; }

		#endregion

		#region private variables

		private static ConcurrentDictionary<int, ColorRGB> threadColors = new ConcurrentDictionary<int, ColorRGB>();
		private static Random random = new Random();

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="jobInfo"></param>
		public void Render(object jobInfo)
		{
			var job = (RenderingJob)jobInfo;

			try
			{
				RenderJob(job);
			}
			finally
			{
				job.DoneEvent.Signal();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="job"></param>
		private unsafe void RenderJob(RenderingJob job)
		{
			var threadColor = GetThreadColor();
			var pixelColor = (ColorRGB*)job.OutputOffset;

			var widthIterator = job.Camera.PixelWidthIterator;
			var heightIterator = job.Camera.PixelHeightIterator;
			var imageOrigin = job.Camera.ImageOrigin;
			var cameraPosition = job.Camera.Position;

			var jobOffsetPosition = job.OffsetPosition;
			var jobSize = job.Size;

			var ray = new Ray();

			var threadDebugging = ThreadDebugging;
			if (!threadDebugging)
				threadColor = ColorRGB.White;

			var jobColorBuffer = job.ColorBuffer;
			var jobColorBufferIndex = 0;
			var cornerSize = 8;

			var jobDebugging = JobDebugging;
			if (jobDebugging)
			{
				#region paint margin as white rectangle

				//jobColorBuffer = new ColorRGB[jobSize.X * 2 + (jobSize.Y - 2) * 2];

				//// bottom line
				//for (var jobx = 0; jobx < jobSize.X; jobx++, pixelColor++)
				//    (*pixelColor) = marginColor;
				//pixelColor += job.NextOutputLineOffset;

				//// left and right margin
				//for (var joby = 1; joby < jobSize.Y - 1; joby++, pixelColor += job.NextOutputLineOffset + 1)
				//{
				//    (*pixelColor) = marginColor;
				//    pixelColor += (jobSize.X - 1);
				//    (*pixelColor) = marginColor;
				//}

				//// top line
				//for (var jobx = 0; jobx < jobSize.X; jobx++, pixelColor++)
				//    (*pixelColor) = marginColor;

				#endregion

				#region paint corners of job

				// bottom parts of corners
				for (var jobx = 0; jobx < cornerSize; jobx++, pixelColor++)
					(*pixelColor) = threadColor;
				pixelColor += jobSize.X - cornerSize * 2;

				for (var jobx = 0; jobx < cornerSize; jobx++, pixelColor++)
					(*pixelColor) = threadColor;
				pixelColor += job.NextOutputLineOffset;

				// bottom left and right parts of corners
				for (var joby = 1; joby < cornerSize; joby++, pixelColor += job.NextOutputLineOffset + 1)
				{
					(*pixelColor) = threadColor;
					pixelColor += (jobSize.X - 1);
					(*pixelColor) = threadColor;
				}

				// top left and right parts of corners
				pixelColor += ((job.NextOutputLineOffset + 1) + (jobSize.X - 1)) * (jobSize.Y - cornerSize * 2);
				for (var joby = 1; joby < cornerSize; joby++, pixelColor += job.NextOutputLineOffset + 1)
				{
					(*pixelColor) = threadColor;
					pixelColor += (jobSize.X - 1);
					(*pixelColor) = threadColor;
				}

				// top parts of corners
				for (var jobx = 0; jobx < cornerSize; jobx++, pixelColor++)
					(*pixelColor) = threadColor;
				pixelColor += jobSize.X - cornerSize * 2;

				for (var jobx = 0; jobx < cornerSize; jobx++, pixelColor++)
					(*pixelColor) = threadColor;
				pixelColor += job.NextOutputLineOffset;

				#endregion

				pixelColor = (ColorRGB*)job.OutputOffset;
			}

			//  .
			// /|\
			//  |
			for (var joby = 0; joby < jobSize.Y; joby++, pixelColor += job.NextOutputLineOffset)
			{
				//    \
				// -----
				//    /
				for (var jobx = 0; jobx < jobSize.X; jobx++, pixelColor++)
				{
					// create ray for pixel at [x,y]
					ray.Prepare(cameraPosition, ((widthIterator * (jobx + jobOffsetPosition.X) + heightIterator * (joby + jobOffsetPosition.Y) + imageOrigin) - cameraPosition).Normalize());
					//ray.Pixel.X = jobx + job.OffsetPosition.X;
					//ray.Pixel.Y = joby + job.OffsetPosition.Y;

					// trace scene
					RayTracer.Trace(ray);

					if (threadDebugging)
					    ray.Color *= threadColor;

					if (jobDebugging)
					{
						#region paint margin with real colors

						//if (jobx == 0 || jobx == jobSize.X - 1 || joby == 0 || joby == jobSize.Y - 1)
						//{
						//    jobColorBuffer[colorBufferIndex] = ray.Color;
						//    colorBufferIndex++;
						//    continue;
						//}

						#endregion

						#region corner colors

						if (((joby == 0 || joby == jobSize.Y - 1) && (jobx < cornerSize || jobx >= jobSize.X - cornerSize)) || ((jobx == 0 || jobx == jobSize.X - 1) && (joby < cornerSize || joby >= jobSize.Y - cornerSize)))
						{
							jobColorBuffer[jobColorBufferIndex] = ray.Color;
							jobColorBufferIndex++;
							continue;
						}

						#endregion
					}
					
					(*pixelColor) = ray.Color;
				}
			}

			if (jobDebugging)
			{
				pixelColor = (ColorRGB*)job.OutputOffset;
				jobColorBufferIndex = 0;

				#region paint margin with real colors

				//// bottom line
				//for (var jobx = 0; jobx < jobSize.X; jobx++, pixelColor++, colorBufferIndex++)
				//    (*pixelColor) = jobColorBuffer[colorBufferIndex];
				//pixelColor += job.NextOutputLineOffset;

				//// left and right margin
				//for (var joby = 1; joby < jobSize.Y - 1; joby++, pixelColor += job.NextOutputLineOffset + 1)
				//{
				//    (*pixelColor) = jobColorBuffer[colorBufferIndex];
				//    colorBufferIndex++;

				//    pixelColor += (jobSize.X - 1);

				//    (*pixelColor) = jobColorBuffer[colorBufferIndex];
				//    colorBufferIndex++;
				//}

				//// top line
				//for (var jobx = 0; jobx < jobSize.X; jobx++, pixelColor++, colorBufferIndex++)
				//    (*pixelColor) = jobColorBuffer[colorBufferIndex];

				#endregion

				#region paint corners of job

				// bottom parts of corners
				for (var jobx = 0; jobx < cornerSize; jobx++, pixelColor++, jobColorBufferIndex++)
					(*pixelColor) = jobColorBuffer[jobColorBufferIndex];
				pixelColor += jobSize.X - cornerSize * 2;

				for (var jobx = 0; jobx < cornerSize; jobx++, pixelColor++, jobColorBufferIndex++)
					(*pixelColor) = jobColorBuffer[jobColorBufferIndex];
				pixelColor += job.NextOutputLineOffset;

				// bottom left and right parts of corners
				for (var joby = 1; joby < cornerSize; joby++, pixelColor += job.NextOutputLineOffset + 1)
				{
					(*pixelColor) = jobColorBuffer[jobColorBufferIndex];
					jobColorBufferIndex++;

					pixelColor += (jobSize.X - 1);

					(*pixelColor) = jobColorBuffer[jobColorBufferIndex];
					jobColorBufferIndex++;
				}

				// top left and right parts of corners
				pixelColor += ((job.NextOutputLineOffset + 1) + (jobSize.X - 1)) * (jobSize.Y - cornerSize * 2);
				for (var joby = 1; joby < cornerSize; joby++, pixelColor += job.NextOutputLineOffset + 1)
				{
					(*pixelColor) = jobColorBuffer[jobColorBufferIndex];
					jobColorBufferIndex++;

					pixelColor += (jobSize.X - 1);

					(*pixelColor) = jobColorBuffer[jobColorBufferIndex];
					jobColorBufferIndex++;
				}

				// top parts of corners
				for (var jobx = 0; jobx < cornerSize; jobx++, pixelColor++, jobColorBufferIndex++)
					(*pixelColor) = jobColorBuffer[jobColorBufferIndex];
				pixelColor += jobSize.X - cornerSize * 2;

				for (var jobx = 0; jobx < cornerSize; jobx++, pixelColor++, jobColorBufferIndex++)
					(*pixelColor) = jobColorBuffer[jobColorBufferIndex];
				pixelColor += job.NextOutputLineOffset;

				#endregion
			}

			if (threadDebugging)
			{
				var invThreaColor = threadColor.Inv();
				pixelColor = (ColorRGB*)job.OutputOffset;
				for (var joby = 0; joby < jobSize.Y; joby++, pixelColor += job.NextOutputLineOffset)
					for (var jobx = 0; jobx < jobSize.X; jobx++, pixelColor++)
						(*pixelColor) = (*pixelColor) * invThreaColor;
			}
		}

		#region thread colors (for debugging)

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private static ColorRGB GetThreadColor()
		{
			ColorRGB color;
			if (!threadColors.TryGetValue(Thread.CurrentThread.ManagedThreadId, out color))
			{
				color = new ColorRGB((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
				threadColors.TryAdd(Thread.CurrentThread.ManagedThreadId, color);
			}

			return color;
		}

		/// <summary>
		/// 
		/// </summary>
		public static void PrintParticipatedThreadColors()
		{
			Log.Instance.AddMsg(LogLevel.Debug, string.Format("--[ {0} threads participated in rendering ]-----------------------------------------------------------------------------", threadColors.Count));
			foreach (var color in threadColors)
				Log.Instance.AddMsg(LogLevel.Debug, string.Format("Thread [ManagedId: {0}; DebugColor: {1}]", color.Key, color.Value.ToString()));
			Log.Instance.AddMsg(LogLevel.Debug, "--------------------------------------------------------------------------------------------------------------------");
		}

		#endregion
	}
}
