namespace Athena.App.Forms
{
	partial class JobCountTestForm
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
			this.label1 = new System.Windows.Forms.Label();
			this.jobSizeMinNumeric = new System.Windows.Forms.NumericUpDown();
			this.jobSizeMaxNumeric = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.startButton = new System.Windows.Forms.Button();
			this.framesToRenderNumeric = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			((System.ComponentModel.ISupportInitialize)(this.jobSizeMinNumeric)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.jobSizeMaxNumeric)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.framesToRenderNumeric)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.ForeColor = System.Drawing.Color.White;
			this.label1.Location = new System.Drawing.Point(47, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(65, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Min job size:";
			// 
			// jobSizeMinNumeric
			// 
			this.jobSizeMinNumeric.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))));
			this.jobSizeMinNumeric.ForeColor = System.Drawing.Color.White;
			this.jobSizeMinNumeric.Location = new System.Drawing.Point(118, 24);
			this.jobSizeMinNumeric.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
			this.jobSizeMinNumeric.Name = "jobSizeMinNumeric";
			this.jobSizeMinNumeric.Size = new System.Drawing.Size(120, 20);
			this.jobSizeMinNumeric.TabIndex = 1;
			// 
			// jobSizeMaxNumeric
			// 
			this.jobSizeMaxNumeric.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))));
			this.jobSizeMaxNumeric.ForeColor = System.Drawing.Color.White;
			this.jobSizeMaxNumeric.Location = new System.Drawing.Point(118, 50);
			this.jobSizeMaxNumeric.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
			this.jobSizeMaxNumeric.Name = "jobSizeMaxNumeric";
			this.jobSizeMaxNumeric.Size = new System.Drawing.Size(120, 20);
			this.jobSizeMaxNumeric.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.ForeColor = System.Drawing.Color.White;
			this.label2.Location = new System.Drawing.Point(44, 52);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(68, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Max job size:";
			// 
			// startButton
			// 
			this.startButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(90)))), ((int)(((byte)(100)))));
			this.startButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.startButton.Location = new System.Drawing.Point(156, 166);
			this.startButton.Name = "startButton";
			this.startButton.Size = new System.Drawing.Size(115, 23);
			this.startButton.TabIndex = 4;
			this.startButton.Text = "Start";
			this.startButton.UseVisualStyleBackColor = false;
			this.startButton.Click += new System.EventHandler(this.startButton_Click);
			// 
			// framesToRenderNumeric
			// 
			this.framesToRenderNumeric.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))));
			this.framesToRenderNumeric.ForeColor = System.Drawing.Color.White;
			this.framesToRenderNumeric.Location = new System.Drawing.Point(118, 76);
			this.framesToRenderNumeric.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
			this.framesToRenderNumeric.Name = "framesToRenderNumeric";
			this.framesToRenderNumeric.Size = new System.Drawing.Size(120, 20);
			this.framesToRenderNumeric.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.ForeColor = System.Drawing.Color.White;
			this.label3.Location = new System.Drawing.Point(23, 78);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(89, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Frames to render:";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.jobSizeMinNumeric);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.framesToRenderNumeric);
			this.groupBox1.Controls.Add(this.jobSizeMaxNumeric);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.ForeColor = System.Drawing.Color.White;
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(259, 139);
			this.groupBox1.TabIndex = 10;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Test properties";
			// 
			// JobCountTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(70)))), ((int)(((byte)(80)))));
			this.ClientSize = new System.Drawing.Size(285, 205);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.startButton);
			this.ForeColor = System.Drawing.Color.White;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "JobCountTestForm";
			this.Opacity = 0.95D;
			this.Text = "JobCount Test";
			this.Load += new System.EventHandler(this.JobCountTest_Load);
			((System.ComponentModel.ISupportInitialize)(this.jobSizeMinNumeric)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.jobSizeMaxNumeric)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.framesToRenderNumeric)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown jobSizeMinNumeric;
		private System.Windows.Forms.NumericUpDown jobSizeMaxNumeric;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button startButton;
		private System.Windows.Forms.NumericUpDown framesToRenderNumeric;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}