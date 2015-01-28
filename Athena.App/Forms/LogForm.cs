using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Athena.Core.Tools;

namespace Athena.App.Forms
{
	/// <summary>
	/// 
	/// </summary>
	public partial class LogForm : Form
	{
		private delegate void LogDelegate(string logLine);

		/// <summary>
		/// 
		/// </summary>
		public LogForm()
		{
			InitializeComponent();

			Log.Instance.LogMessage += new Log.LogHandler(OnNewLogMessage);
		}

		#region Log methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="logLevel"></param>
		/// <param name="message"></param>
		private void OnNewLogMessage(string logLine)
		{
			if (!this.Visible)
				return;

			if (logListBox.InvokeRequired)
				BeginInvoke(new LogDelegate(AddLogMsg), logLine);
			else
				AddLogMsg(logLine);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="logLevel"></param>
		/// <param name="message"></param>
		private void AddLogMsg(string logLine)
		{
			logListBox.Items.Add(logLine);

			logListBox.SelectedIndex = logListBox.Items.Count - 1;
			logListBox.SelectedIndex = -1;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void clearLogListBtn_Click(object sender, EventArgs e)
		{
			logListBox.Items.Clear();
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LogForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.Hide();
			e.Cancel = true;
		}

		/// <summary>
		/// 
		/// </summary>
		internal void ClearLog()
		{
			logListBox.Items.Clear();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LogForm_VisibleChanged(object sender, EventArgs e)
		{
			if (this.Visible)
			{
				ClearLog();
				logListBox.Items.AddRange(Log.Instance.History);
			}
		}
	}
}
