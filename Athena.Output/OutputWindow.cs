using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Athena.Core.Tools;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenTK.Graphics;
using Athena.Core.DataTypes;
using Athena.Renderer;
using Athena.Renderer.RayTracers;
using Athena.Core;

namespace Athena.Output
{
	public class OutputWindow : GameWindow
	{
		public Vector2i OutputSize { get; private set; }

		#region Private variables

		/// <summary>
		/// 
		/// </summary>
		private int lastFrameDrawn;

		/// <summary>
		/// 
		/// </summary>
		private bool leftMouseButtonDown = false;

		#endregion

		/// <summary>
		/// 
		/// </summary>
		public OutputWindow(string windowTitle, Vector2i outputSize, VSyncMode vSyncMode) : base(outputSize.X, outputSize.Y, GraphicsMode.Default, windowTitle)
		{
			Keyboard.KeyDown += OnKeyDown;
			Mouse.ButtonDown += OnMouseButtonDown;
			Mouse.ButtonUp += OnMouseButtonUp;
			Mouse.Move += OnMouseMove;
			Closing += OnClosing;

			OutputSize = outputSize;
			WindowBorder = OpenTK.WindowBorder.Fixed;
			VSync = vSyncMode;

			lastFrameDrawn = 0;

			Log.Instance.AddMsg(LogLevel.Debug, string.Format("VSync: {0}", VSync.ToString()));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fullscreen"></param>
		public void Show(bool fullscreen)
		{
			this.WindowState = fullscreen ? OpenTK.WindowState.Fullscreen : OpenTK.WindowState.Normal;

			Run(60, 60);
		}

		#region Events

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseMove(object sender, MouseMoveEventArgs e)
		{
			if (leftMouseButtonDown && (e.XDelta != 0 || e.YDelta != 0))
				Scene.Current.Camera.RotateAroundTarget(new Athena.Core.DataTypes.Vector3(e.YDelta, e.XDelta, 0));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.Button.HasFlag(MouseButton.Left))
			{
				RenderingManager.Instance.SetQuality(RenderingQuality.Fast);
				leftMouseButtonDown = true;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (e.Button.HasFlag(MouseButton.Left))
			{
				RenderingManager.Instance.SetQuality(RenderingQuality.Best);
				leftMouseButtonDown = false;
			}
		}

		/// <summary>
		/// Occurs when a key is pressed.
		/// </summary>
		/// <param name="sender">The KeyboardDevice which generated this event.</param>
		/// <param name="e">The key that was pressed.</param>
		private void OnKeyDown(object sender, KeyboardKeyEventArgs e)
		{
			if (e.Key == Key.Escape)
				this.Exit();

			if (e.Key == Key.KeypadAdd)
			{
				RayTracer.MaxOctreeDepth++;
				Log.Instance.AddMsg(LogLevel.Debug, string.Format("RayTracer.MaxOctreeDepth: {0}", RayTracer.MaxOctreeDepth));
			}

			else if (e.Key == Key.KeypadSubtract && RayTracer.MaxOctreeDepth > 0)
			{
				RayTracer.MaxOctreeDepth--;
				Log.Instance.AddMsg(LogLevel.Debug, string.Format("RayTracer.MaxOctreeDepth: {0}", RayTracer.MaxOctreeDepth));
			}

			else if (e.Key == Key.F1)
			{
				Log.Instance.AddMsg(LogLevel.Info, "--[ Help ]----------------------------------------------------------------------------------------------------------");
				Log.Instance.AddMsg(LogLevel.Info, "F2:  Toggle thread debugging (show which job was done by which thread)");
				Log.Instance.AddMsg(LogLevel.Info, "F4:  Toggle job debugging (show jobs)");
				Log.Instance.AddMsg(LogLevel.Info, "F5:  Toggle voxel normal debugging (show voxel normal as color instead of diffuse color)");
				Log.Instance.AddMsg(LogLevel.Info, "F11: Toggle fullscreen/window mode");
				Log.Instance.AddMsg(LogLevel.Info, "ESC: Stop rendering and close output window");
				Log.Instance.AddMsg(LogLevel.Info, "--------------------------------------------------------------------------------------------------------------------");
			}

			else if (e.Key == Key.F2)
			{
				RenderingThread.ThreadDebugging = !RenderingThread.ThreadDebugging;
				Log.Instance.AddMsg(LogLevel.Debug, string.Format("RenderingThread.ThreadDebugging: {0}", RenderingThread.ThreadDebugging));
			}

			else if (e.Key == Key.F3)
			{

			}

			else if (e.Key == Key.F4)
			{
				RenderingThread.JobDebugging = !RenderingThread.JobDebugging;
				Log.Instance.AddMsg(LogLevel.Debug, string.Format("RenderingThread.JobDebugging: {0}", RenderingThread.JobDebugging));
			}

			else if (e.Key == Key.F5)
			{
				RayTracer.VoxelNormalDebugging = !RayTracer.VoxelNormalDebugging;
				Log.Instance.AddMsg(LogLevel.Debug, string.Format("RayTracer.VoxelNormalDebugging: {0}", RayTracer.VoxelNormalDebugging));
			}

			else if (e.Key == Key.F6)
			{
				
			}

			else if (e.Key == Key.F7)
			{

			}

			else if (e.Key == Key.F8)
			{

			}

			else if (e.Key == Key.F9)
			{

			}

			else if (e.Key == Key.F10)
			{

			}

			else if (e.Key == Key.F11)
			{
				if (this.WindowState == WindowState.Fullscreen)
					this.WindowState = WindowState.Normal;
				else
					this.WindowState = WindowState.Fullscreen;
			}

			else if (e.Key == Key.F12)
			{

			}

			if (e.Key.HasFlag(Key.A))
			{
				Scene.Current.Camera.Move(new Athena.Core.DataTypes.Vector3(-1, 0, 0));
			}
			if (e.Key.HasFlag(Key.D))
			{
				Scene.Current.Camera.Move(new Athena.Core.DataTypes.Vector3(1, 0, 0));
			}
			if (e.Key.HasFlag(Key.W))
			{
				Scene.Current.Camera.Move(new Athena.Core.DataTypes.Vector3(0, 0, 1));
			}
			if (e.Key.HasFlag(Key.S))
			{
				Scene.Current.Camera.Move(new Athena.Core.DataTypes.Vector3(0, 0, -1));
			}
			if (e.Key.HasFlag(Key.Q))
			{
				Scene.Current.Camera.Move(new Athena.Core.DataTypes.Vector3(0, 1, 0));
			}
			if (e.Key.HasFlag(Key.E))
			{
				Scene.Current.Camera.Move(new Athena.Core.DataTypes.Vector3(0, -1, 0));
			}

			//if (e.Key.HasFlag(Key.Left))
			//{
			//    var camera = Scene.Current.Camera;
			//    camera.RotateAroundTarget(Athena.Core.DataTypes.Vector3.UnitY, -camera.Speed);
			//}
			//if (e.Key.HasFlag(Key.Right))
			//{
			//    var camera = Scene.Current.Camera;
			//    camera.RotateAroundTarget(Athena.Core.DataTypes.Vector3.UnitY, camera.Speed);
			//}
			//if (e.Key.HasFlag(Key.Up))
			//{
			//    var camera = Scene.Current.Camera;
			//    camera.RotateAroundTarget(Athena.Core.DataTypes.Vector3.UnitX, camera.Speed);
			//}
			//if (e.Key.HasFlag(Key.Down))
			//{
			//    var camera = Scene.Current.Camera;
			//    camera.RotateAroundTarget(Athena.Core.DataTypes.Vector3.UnitX, -camera.Speed);
			//}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		private void OnClosing(object sender, CancelEventArgs e)
		{
			RenderingManager.Instance.FrameRedrawn();
		}

		#endregion

		#region overrides

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e">Contains timing information.</param>
		/// <remarks>There is no need to call the base implementation.</remarks>
		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			Application.DoEvents();
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="e">Contains timing information.</param>
		/// <remarks>There is no need to call the base implementation.</remarks>
		protected override void OnRenderFrame(FrameEventArgs e)
		{
			var renderingManager = RenderingManager.Instance;

			try
			{
				if (!renderingManager.ShouldRedraw(lastFrameDrawn))
				{
					Title = string.Format("render: {0}ms ({1}fps); frames: {2}; progress: {3}%",
						(int)(renderingManager.LastFrameDuration),
						(1000f / renderingManager.LastFrameDuration).ToString("0.###"),
						renderingManager.FramesRendered,
						renderingManager.FrameProgress.ToString("0.##"));

					return;
				}

				#region draw fullscreen quad

				GL.UnmapBuffer(BufferTarget.PixelUnpackBuffer);
				GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, renderingManager.OutputBufferSize.X, renderingManager.OutputBufferSize.Y, OpenTK.Graphics.OpenGL.PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);

				GL.Clear(ClearBufferMask.ColorBufferBit);

				GL.Color3(Color.White);
				GL.Begin(BeginMode.Quads);
				GL.TexCoord2(0.0, 0.0); GL.Vertex2(-1.0, -1.0);
				GL.TexCoord2(0.0, 1.0); GL.Vertex2(-1.0, 1.0);
				GL.TexCoord2(1.0, 1.0); GL.Vertex2(1.0, 1.0);
				GL.TexCoord2(1.0, 0.0); GL.Vertex2(1.0, -1.0);
				GL.End();

				this.SwapBuffers();

				#endregion

				Title = string.Format("render: {0}ms ({1}fps); frames: {2}", (int)(renderingManager.LastFrameDuration), (1000f / renderingManager.LastFrameDuration).ToString("0.###"), renderingManager.FramesRendered);

				if (renderingManager.FramesRendered - lastFrameDrawn > 1)
					Log.Instance.AddMsg(LogLevel.Warning, string.Format("{0} frame/s missed", renderingManager.FramesRendered - lastFrameDrawn - 1));

				lastFrameDrawn = renderingManager.FramesRendered;
				renderingManager.FrameRedrawn();
			}
			catch (Exception ex)
			{
				Log.Instance.AddException(ex);
			}
		}

		/// <summary>
		/// Respond to resize events here.
		/// </summary>
		/// <param name="e">Contains information on the new GameWindow size.</param>
		/// <remarks>There is no need to call the base implementation.</remarks>
		protected override void OnResize(EventArgs e)
		{
			GL.Viewport(0, 0, Width, Height);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
		}

		/// <summary>
		/// Setup OpenGL and load resources here.
		/// </summary>
		/// <param name="e">Not used.</param>
		protected override void OnLoad(EventArgs e)
		{
			GL.ClearColor(Color.Black);
			GL.ShadeModel(ShadingModel.Flat);
			GL.Enable(EnableCap.Texture2D);

			//TextureId = GL.GenTexture();
			//GL.BindTexture(TextureTarget.Texture2D, TextureId);
			//var bmp = new Bitmap(@"d:\ghosts.bmp");
			//BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			//GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);
			//bmp.UnlockBits(bmp_data);
			//GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			//GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
		}

		#endregion
	}
}
