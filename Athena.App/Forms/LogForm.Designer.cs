namespace Athena.App.Forms
{
	partial class LogForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.logListBox = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// logListBox
			// 
			this.logListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))));
			this.logListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.logListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.logListBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.logListBox.ForeColor = System.Drawing.Color.White;
			this.logListBox.FormattingEnabled = true;
			this.logListBox.HorizontalScrollbar = true;
			this.logListBox.ItemHeight = 15;
			this.logListBox.Location = new System.Drawing.Point(0, 0);
			this.logListBox.Name = "logListBox";
			this.logListBox.Size = new System.Drawing.Size(1077, 236);
			this.logListBox.TabIndex = 34;
			// 
			// LogForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1077, 236);
			this.Controls.Add(this.logListBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "LogForm";
			this.Text = "Log";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LogForm_FormClosing);
			this.VisibleChanged += new System.EventHandler(this.LogForm_VisibleChanged);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListBox logListBox;
	}
}