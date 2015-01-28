using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Athena.Core.Tools;
using Athena.Core;
using Athena.Renderer;
using Athena.Output;
using Athena.Core.DataTypes;
using OpenTK;
using System.IO;
using System.Reflection;

namespace Athena.App.Forms
{
	/// <summary>
	/// 
	/// </summary>
	public partial class MainFormMDI : Form
	{
		/// <summary>
		/// 
		/// </summary>
		public MainFormMDI()
		{
			InitializeComponent();

			#region set some mdi form colors

			// text color
			menuStrip.ForeColor = Color.White;

			// background color
			foreach (Control control in Controls)
			{
				if (control is MdiClient)
					(control as MdiClient).BackColor = Color.FromArgb(20, 30, 40);
			}

			#endregion

			#region Events subscription

			Log.Instance.LogMessage += new Log.LogHandler(OnNewLogMessage);

			#endregion

			Log.Instance.AddMsg(LogLevel.Info, string.Format("Athena.NET started [{0}]", DateTime.Now.ToShortDateString()));
		}

		#region windows layout

		private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LayoutMdi(MdiLayout.Cascade);
		}

		private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LayoutMdi(MdiLayout.TileVertical);
		}

		private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LayoutMdi(MdiLayout.TileHorizontal);
		}

		private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LayoutMdi(MdiLayout.ArrangeIcons);
		}

		#endregion

		#region form actions

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainFormMDI_Load(object sender, EventArgs e)
		{
			#region create client forms

			new LogForm() { MdiParent = this, Dock = DockStyle.Bottom }.Show();

			NewConfiguration(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Configurations", "sample.acfg"));

			#endregion
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainFormMDI_FormClosing(object sender, FormClosingEventArgs e)
		{
			Exit();
			e.Cancel = false;
		}

		/// <summary>
		/// 
		/// </summary>
		private void Exit()
		{
			Log.Instance.DumpToFile();

			foreach (var child in MdiChildren)
				child.Close();

			Log.Instance.Dispose();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NewConfiguration(null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenConfiguration();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void closeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CloseConfiguration();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveConfiguration();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveAsConfiguration();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void newToolStripButton_Click(object sender, EventArgs e)
		{
			NewConfiguration(null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void saveToolStripButton_Click(object sender, EventArgs e)
		{
			SaveConfiguration();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void openToolStripButton_Click(object sender, EventArgs e)
		{
			OpenConfiguration();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Log.Instance.AddMsg(LogLevel.Info, "about ...");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.F1)
			{
				Log.Instance.AddMsg(LogLevel.Info, "--[ Help ]----------------------------------------------------------------------------------------------------------");
				Log.Instance.AddMsg(LogLevel.Info, "F5:  Start rendering");
				Log.Instance.AddMsg(LogLevel.Info, "F10: Print threads participated in rendering");
				Log.Instance.AddMsg(LogLevel.Info, "--------------------------------------------------------------------------------------------------------------------");
				return true;
			}

			if (keyData == Keys.F5)
			{
				StartRendering();
				return true;
			}

			if (keyData == Keys.F10)
			{
				RenderingThread.PrintParticipatedThreadColors();
				return true;
			}

			if (keyData.HasFlag(Keys.Control) && keyData.HasFlag(Keys.S))
			{
				SaveConfiguration();
				return true;
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}

		#endregion

		#region config editor

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private ConfigEditorForm GetConfigEditorForm()
		{
			return (from f in MdiChildren.ToList() where f is ConfigEditorForm select f).SingleOrDefault() as ConfigEditorForm;
		}

		/// <summary>
		/// 
		/// </summary>
		private void NewConfiguration(string configFilename)
		{
			try
			{
				var configEditorForm = GetConfigEditorForm();
				if (configEditorForm != null)
				{
					// TODO modalne okno ci chcem ulozit stary config
				}

				ShowConfigEditor(configFilename);
			}
			catch (Exception ex)
			{
				Log.Instance.AddException(ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void SaveAsConfiguration()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 
		/// </summary>
		private void CloseConfiguration()
		{
			var configEditorForm = GetConfigEditorForm();
			if (configEditorForm != null)
			{
				// TODO modalne okno ci chcem ulozit stary config

				configEditorForm.Close();
				configEditorForm.Dispose();
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		private void SaveConfiguration()
		{
			try
			{
				var configEditorForm = GetConfigEditorForm();
				if (configEditorForm != null)
					configEditorForm.SaveConfig();
			}
			catch (Exception ex)
			{
				Log.Instance.AddException(ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void OpenConfiguration()
		{
			try
			{
				var configEditorForm = GetConfigEditorForm();
				if (configEditorForm != null)
				{
					// TODO modalne okno ci chcem ulozit stary config
				}

				using (var openDlg = new OpenFileDialog()
				{
					DefaultExt = Config.ConfigFileExtension,
					AddExtension = true,
					InitialDirectory = Config.DefaultConfigurationsDirectory,
					Filter = "Athena scene files (*." + Config.ConfigFileExtension + ")|*." + Config.ConfigFileExtension
				})
				{
					if (openDlg.ShowDialog() == DialogResult.OK)
						ShowConfigEditor(openDlg.FileName);
				}
			}
			catch (Exception ex)
			{
				Log.Instance.AddException(ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="configFilename"></param>
		private void ShowConfigEditor(string configFilename)
		{
			var configEditorForm = GetConfigEditorForm();
			if (configEditorForm == null)
			{
				configEditorForm = new ConfigEditorForm(configFilename) { MdiParent = this, Dock = DockStyle.Fill };
			}
			else
			{
				configEditorForm.LoadConfig(configFilename);
			}

			configEditorForm.Show();
			configEditorForm.Focus();
		}

		#endregion

		#region log

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private LogForm GetLogForm()
		{
			return (from f in MdiChildren.ToList() where f is LogForm select f).SingleOrDefault() as LogForm;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void clearLogToolStripButton_Click(object sender, EventArgs e)
		{
			try
			{
				var logForm = GetLogForm();
				if (logForm != null)
					logForm.ClearLog();
			}
			catch (Exception ex)
			{
				Log.Instance.AddException(ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void logToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				var logForm = GetLogForm();
				if (logForm == null)
					logForm = new LogForm() { MdiParent = this, Dock = DockStyle.Bottom };

				logForm.Show();
				logForm.Focus();
			}
			catch (Exception ex)
			{
				Log.Instance.AddException(ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="logLevel"></param>
		/// <param name="message"></param>
		private void OnNewLogMessage(string logLine)
		{

		}

		#endregion
		
		#region rendering

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void startBtn_Click(object sender, EventArgs e)
		{
			StartRendering();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void pauseBtn_Click(object sender, EventArgs e)
		{
			PauseResumeRendering();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void stopBtn_Click(object sender, EventArgs e)
		{
			StopRendering();
		}

		/// <summary>
		/// 
		/// </summary>
		private void StartRendering()
		{
			try
			{
				var renderingManager = RenderingManager.Instance;

				if (renderingManager.IsRendering)
					return;

				var configForm = GetConfigEditorForm();
				if (configForm == null)
					throw new Exception("rendering config not found");

				configForm.Parse();

				using (var outputWindow = new OutputWindow("Athena.Output", configForm.OutputSize, configForm.VSync))
				{
					renderingManager.Initialize(configForm.OutputSize, configForm.NumberOfJobs, configForm.NumberOfThreads);
					renderingManager.StartRendering(configForm.WaitForOutputRedraw, configForm.FramesToRender);

					outputWindow.Show(configForm.Fullscreen);

					renderingManager.StopRendering(configForm.SaveOutputAfterRendering, configForm.OutputImageFilename);
				}
			}
			catch (Exception ex)
			{
				Log.Instance.AddException(ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void PauseResumeRendering()
		{
			try
			{
				//RenderingManager.PauseResumeRendering();
			}
			catch (Exception ex)
			{
				Log.Instance.AddException(ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void StopRendering()
		{
			try
			{
				RenderingManager.Instance.StopRendering(true, null);
			}
			catch (Exception ex)
			{
				Log.Instance.AddException(ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void RunJobCountTest()
		{
			//var minJobSize = 10;
			//var maxJobSize = OutputSize.Y;
			//var framesToRender = 10;

			//using (var jobTestForm = new JobCountTestForm() { MinJobSize = minJobSize, MaxJobSize = maxJobSize, FramesToRender = framesToRender })
			//{
			//    if (jobTestForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			//    {
			//        minJobSize = jobTestForm.MinJobSize;
			//        maxJobSize = jobTestForm.MaxJobSize;
			//        framesToRender = jobTestForm.FramesToRender;
			//    }
			//    else
			//        return;
			//}

			//Log.Instance.AddMsg(LogLevel.Info, "JobCountTest started [output: " + OutputSize.X + "x" + OutputSize.Y + "; jobSize: " + minJobSize + "-" + maxJobSize + "; framesToRender: " + framesToRender + "]");

			//var i = 1;
			//for (var jobSize = minJobSize; jobSize < maxJobSize; jobSize++)
			//{
			//    if (OutputSize.X % jobSize != 0 || OutputSize.Y % jobSize != 0)
			//        continue;

			//    var numberOfJobs = new Vector2i(OutputSize.X / jobSize, OutputSize.Y / jobSize);

			//    Log.Instance.MinLogLevel = LogLevel.Warning;

			//    using (var outputWindow = new OutputWindow("Athena.Output.JobCountTest", OutputSize, VSync))
			//    {
			//        RenderingManager.Initialize(OutputSize, numberOfJobs, NumberOfThreads);
			//        RenderingManager.SetCustomThreadAffinity = setAffinityCheck.Checked;
			//        RenderingManager.StartRendering(Scene.Current, RedrawMethod, framesToRender);

			//        outputWindow.Show();

			//        RenderingManager.StopRendering();
			//    }

			//    Log.Instance.MinLogLevel = LogLevel.Anything;

			//    Log.Instance.AddMsg(LogLevel.Info, "Result #" + i + " [jobSize: " + jobSize + "x" + jobSize +
			//        ", jobCount: " + numberOfJobs.X.ToString() + "x" + numberOfJobs.Y + "(" + numberOfJobs.X * numberOfJobs.Y +
			//        "), testDuration: " + RenderingManager.LastRenderingDuration.ToString("0.000") + "s (avg " + ((double)framesToRender / RenderingManager.LastRenderingDuration).ToString("0.0") + "fps)]");
			//    i++;
			//}

			//Log.Instance.AddMsg(LogLevel.Info, "JobCountTest finished");
		}

		#endregion
	}
}
