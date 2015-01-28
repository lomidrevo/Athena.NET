using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Athena.Core.Tools;
using Athena.Core.DataTypes;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenTK.Graphics;
using Athena.Core;
using Athena.Core.Objects;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using Athena.Core.AccStructs;
using Athena.Renderer.RayTracers;

[assembly: CLSCompliant(true)]
namespace Athena.Renderer
{
	/// <summary>
	/// 
	/// </summary>
	public enum RenderingQuality
	{
		Fast,
		Best
	}

	/// <summary>
	/// Athena rendering management class
	/// </summary>
	public sealed class RenderingManager
	{
		#region singleton

		/// <summary>
		/// instance of type T
		/// </summary>
		private static volatile RenderingManager instance;

		/// <summary>
		/// sync object for multithread access
		/// </summary>
		private static object syncRoot = new Object();

		/// <summary>
		/// singleton instance property
		/// </summary>
		public static RenderingManager Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncRoot)
					{
						if (instance == null)
							instance = new RenderingManager();
					}
				}

				return instance;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private RenderingManager()
		{

		}

		#endregion

		#region public properties

		/// <summary>
		/// size of output buffer
		/// </summary>
		public Vector2i OutputBufferSize { get; private set; }

		/// <summary>
		/// duration of last frame (in miliseconds)
		/// </summary>
		public double LastFrameDuration { get; private set; }

		/// <summary>
		/// number of frames rendered since StartRendering() was called
		/// </summary>
		public int FramesRendered { get; private set; }

		/// <summary>
		/// duration of rendering (in miliseconds, updated after each rendered frame)
		/// </summary>
		public double RenderingDuration { get; private set; }

		/// <summary>
		/// returns rendering progress of actual frame in %
		/// </summary>
		public float FrameProgress { get { return (1f - (float)frameRenderedEvent.CurrentCount / frameRenderedEvent.InitialCount) * 100; } }

		/// <summary>
		/// output image filename
		/// </summary>
		public string OutputImageFilename
		{
			get
			{
				if (string.IsNullOrEmpty(outputImageFilename))
					outputImageFilename = Path.Combine(Config.DefaultOutputDirectory, DateTime.Now.ToString("yyyyddMM.HHmmss") + ".athena.output.png");

				return outputImageFilename;
			}

			set
			{
				outputImageFilename = value;
			}
		}

		/// <summary>
		/// returns true if rendering is in progress
		/// </summary>
		public bool IsRendering { get { return managementThread != null; } }

		/// <summary>
		/// 
		/// </summary>
		public RenderingQuality Quality { get; private set; }

		#endregion

		#region private variables

		private int? framesToRender;

		private IntPtr outputBuffer;
		private int outputBufferId;
		private int outputBufferLength;

		private List<RenderingJob> renderingJobs;

		private CountdownEvent frameRenderedEvent;
		private ManualResetEvent endOfRenderingEvent;
		private bool stopRendering = true;

		/// <summary>
		/// event for signalig end of frame drawing
		/// </summary>
		private ManualResetEvent frameRedrawnEvent;

		/// <summary>
		/// when true rendering of next frame will start after output is redrawn (FrameRedrawnEvent is signaled)
		/// </summary>
		private bool waitForOutputRedraw;

		private Thread managementThread;

		private string outputImageFilename;

		#endregion

		/// <summary>
		/// initialization, shoud be called first (creates jobs, output buffers, ...)
		/// </summary>
		/// <param name="outputSize"></param>
		/// <param name="numberOfJobs"></param>
		/// <param name="numberOfThreads"></param>
		public void Initialize(Vector2i outputSize, Vector2i numberOfJobs, int? numberOfThreads)
		{
			Quality = RenderingQuality.Best;

			renderingJobs = new List<RenderingJob>();
			frameRedrawnEvent = new ManualResetEvent(false);
			endOfRenderingEvent = new ManualResetEvent(false);

			OutputBufferSize = outputSize;
			CreateOutputBuffers(outputSize.X, outputSize.Y, sizeof(float));

			Log.Instance.AddMsg(LogLevel.Info, string.Format("OutputWindow [size: {0}x{1} pixels]", outputSize.X, outputSize.Y));

			var jobSize = new Vector2i(outputSize.X / numberOfJobs.X, outputSize.Y / numberOfJobs.Y);
			var jobBufferLength = (outputSize.X * outputSize.Y * 3/*RGB*/) / (numberOfJobs.X * numberOfJobs.Y);

			#region create rendering jobs

			frameRenderedEvent = new CountdownEvent(numberOfJobs.X * numberOfJobs.Y);

			for (var y = 0; y < numberOfJobs.Y; y++)
			{
				for (var x = 0; x < numberOfJobs.X; x++)
				{
					var offset = IntPtr.Zero;
					unsafe { offset = (IntPtr)((ColorRGB*)outputBuffer + (x * jobSize.X + y * (jobSize.Y * OutputBufferSize.X))); }

					renderingJobs.Add(new RenderingJob(offset, new Vector2i(x * jobSize.X, y * jobSize.Y), new Vector2i(x, y), jobSize, OutputBufferSize.X - jobSize.X, Scene.Current.Camera, frameRenderedEvent));
				}
			}

			#endregion

			#region set max number of threads in ThreadPool

			int wThreads, cThreads;
			if (numberOfThreads.HasValue)
			{
				// if numberOfThreads is 0, set max number of threads in thread pool to number of cores
				if (numberOfThreads.Value == 0)
					numberOfThreads = Environment.ProcessorCount;

				if (!ThreadPool.SetMaxThreads(numberOfThreads.Value, numberOfThreads.Value))
					throw new NotSupportedException();

				wThreads = cThreads = numberOfThreads.Value;
			}
			else
				ThreadPool.GetMaxThreads(out wThreads, out cThreads);

			int availableWThreads, availableCThreads;
			ThreadPool.GetAvailableThreads(out availableWThreads, out availableCThreads);

			#endregion
			
			Log.Instance.AddMsg(LogLevel.Info, string.Format("ThreadPool [available workerThreads: {0}/{1}; maxCompletionPortThreads: {2}/{3}]", availableWThreads, wThreads, availableCThreads, cThreads));
			Log.Instance.AddMsg(LogLevel.Info, string.Format("RenderingJobs [count: {0}x{1}({2}); size: {3}x{4} pixels]", numberOfJobs.X.ToString(), numberOfJobs.Y, numberOfJobs.X * numberOfJobs.Y, jobSize.X, jobSize.Y));
		}

		/// <summary>
		/// start rendering using with background threads
		/// </summary>
		/// <param name="waitForOutputRedraw"></param>
		/// <param name="framesToRender"></param>
		public void StartRendering(bool waitForOutputRedraw, int? framesToRender)
		{
			Scene.Current.PrepareSceneForRendering();

			stopRendering = false;
			FramesRendered = 0;
			RenderingDuration = LastFrameDuration = 0;
			this.framesToRender = framesToRender;
			this.waitForOutputRedraw = waitForOutputRedraw;

			Log.Instance.AddMsg(LogLevel.Info, "Rendering started ...");

			// start rendering management thread
			managementThread = new Thread(new ThreadStart(Render));
			managementThread.Start();
		}

		/// <summary>
		/// method for rendering threads management
		/// </summary>
		private void Render()
		{
			// create WaitCallBack instances with proper rendering methods
			var renderingCallBacks = new List<WaitCallback>();
			foreach (var renderingJob in renderingJobs)
				renderingCallBacks.Add(new WaitCallback(new RenderingThread().Render));

			var camera = Scene.Current.Camera;

			var frameTimer = new HighPerformanceTimer();
			do
			{
				frameTimer.Start();

				frameRedrawnEvent.Reset();
				frameRenderedEvent.Reset();

				// update camera view
				camera.Update(OutputBufferSize);

				// render all jobs
				for (var i = 0; i < renderingCallBacks.Count; i++)
					ThreadPool.QueueUserWorkItem(renderingCallBacks[i], renderingJobs[i]);

				// wait for end of rendering
				frameRenderedEvent.Wait();

				FramesRendered++;
				if (framesToRender.HasValue && framesToRender.Value > 0)
					framesToRender--;

				frameTimer.Stop();
				LastFrameDuration = frameTimer.Duration;
				RenderingDuration += LastFrameDuration;

				if (waitForOutputRedraw)
					// wait while output window is redrawn
					frameRedrawnEvent.WaitOne();
			}
			while (!stopRendering && (!framesToRender.HasValue || framesToRender.Value > 0));

			endOfRenderingEvent.Set();
		}

		/// <summary>
		/// stop rendering (will wait for next frame to finish rendering)
		/// </summary>
		/// <param name="saveOutput"></param>
		/// <param name="outputImageFilename"></param>
		public void StopRendering(bool saveOutput, string outputImageFilename)
		{
			if (stopRendering)
				return;

			Log.Instance.AddMsg(LogLevel.Info, "Stopping rendering ...");

			stopRendering = true;
			frameRedrawnEvent.Set();

			new Thread(delegate()
			{
				// wait 10 seconds for end of rendering
				if (!endOfRenderingEvent.WaitOne(10 * 1000))
				{
					Log.Instance.AddMsg(LogLevel.Warning, "RenderingManager thread aborted!");
					managementThread.Abort();
				}

				Log.Instance.AddMsg(LogLevel.Info, string.Format("Rendering finished [framesRendered: {0}; duration: {1} (avg {2}fps)]", FramesRendered, FormatString.GetDuration((int)(RenderingDuration)), ((double)FramesRendered / RenderingDuration * 1000).ToString("0.###")));
				managementThread = null;

				RenderingThread.PrintParticipatedThreadColors();

				if (saveOutput)
					RenderingManager.SaveBufferToFile(outputBuffer, OutputBufferSize, outputImageFilename ?? OutputImageFilename);

			}) { IsBackground = true }.Start();
		}

		/// <summary>
		/// clears given buffer with given color, or if no color is set, uses default(ColorRGB)
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="bufferSize"></param>
		/// <param name="color"></param>	
		public unsafe static void ClearBuffer(IntPtr buffer, int bufferSize, ColorRGB? color)
		{
			var pixel = (ColorRGB*)buffer;

			for (var i = 0; i < bufferSize; i++, pixel++)
				*pixel = color.HasValue ? color.Value : ColorRGB.Black;
		}

		/// <summary>
		/// save content of outputBuffer to *.png file
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="bufferSize"></param>
		/// <param name="filename"></param>
		public static void SaveBufferToFile(IntPtr buffer, Vector2i bufferSize, string filename)
		{
			// save file in background
			new Thread(delegate()
			{
				try
				{
					var bitmap = new Bitmap(bufferSize.X, bufferSize.Y, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
					unsafe
					{
						var pixelPtr = (ColorRGB*)buffer;
						for (var y = 0; y < bufferSize.Y; y++)
							for (var x = 0; x < bufferSize.X; x++, pixelPtr++)
								bitmap.SetPixel(x, (bufferSize.Y - 1) - y, Color.FromArgb((byte)(255f * (*pixelPtr).R), (byte)(255f * (*pixelPtr).G), (byte)(255f * (*pixelPtr).B)));
					}

					bitmap.Save(filename);
					Log.Instance.AddMsg(LogLevel.Info, "Output image saved as '" + filename + "'");
				}
				catch (Exception ex)
				{
					Log.Instance.AddException(ex);
				}

			}) { IsBackground = true }.Start();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="newQuality"></param>
		public void SetQuality(RenderingQuality newQuality)
		{
			if (newQuality == Quality)
				return;

			Quality = newQuality;

			if (newQuality == RenderingQuality.Fast)
			{
				RayTracer.MaxOctreeDepth = 0;
			}
			else if (newQuality == RenderingQuality.Best)
			{
				// TODO restore MaxOctreeDepth that was set before
				RayTracer.MaxOctreeDepth = 10;
			}
		}

		#region private methods

		/// <summary>
		/// creates output buffer for rendering
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="bpp"></param>
		private void CreateOutputBuffers(int width, int height, int bpp)
		{
			outputBufferLength = width * height * 3/*RGB*/* bpp;

			// gpu buffer
			GL.GenBuffers(1, out outputBufferId);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, width, height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.BindBuffer(BufferTarget.PixelUnpackBuffer, outputBufferId);
			GL.BufferData(BufferTarget.PixelUnpackBuffer, (IntPtr)outputBufferLength, IntPtr.Zero, BufferUsageHint.StreamDraw);
			GL.BindBuffer(BufferTarget.PixelUnpackBuffer, outputBufferId);

			outputBuffer = GL.MapBuffer(BufferTarget.PixelUnpackBuffer, BufferAccess.WriteOnly);
		}

		#endregion

		/// <summary>
		///
		/// </summary>
		public void FrameRedrawn()
		{
			if (waitForOutputRedraw)
				frameRedrawnEvent.Set();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lastFrameDrawn"></param>
		/// <returns></returns>
		public bool ShouldRedraw(int lastFrameDrawn)
		{
			return (!waitForOutputRedraw || FramesRendered > lastFrameDrawn);
		}
	}
}
