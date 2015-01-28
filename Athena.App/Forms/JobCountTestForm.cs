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
	public partial class JobCountTestForm : Form
	{
		public int MinJobSize;
		public int MaxJobSize;
		public int FramesToRender;

		public JobCountTestForm()
		{
			InitializeComponent();
		}

		private void startButton_Click(object sender, EventArgs e)
		{
			try
			{
				MinJobSize = Convert.ToInt32(jobSizeMinNumeric.Value);
				MaxJobSize = Convert.ToInt32(jobSizeMaxNumeric.Value);
				FramesToRender = Convert.ToInt32(framesToRenderNumeric.Value);
			}
			catch (Exception ex)
			{
				Log.Instance.AddException(ex);
				return;
			}

			DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}

		private void JobCountTest_Load(object sender, EventArgs e)
		{
			jobSizeMinNumeric.Value = Convert.ToDecimal(MinJobSize);
			jobSizeMaxNumeric.Value = Convert.ToDecimal(MaxJobSize);
			framesToRenderNumeric.Value = Convert.ToDecimal(FramesToRender);
		}
	}
}
