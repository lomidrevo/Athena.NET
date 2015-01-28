using Athena.App.Controls;
namespace Athena.App.Forms
{
	partial class ConfigEditorForm
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
			this.components = new System.ComponentModel.Container();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.configTreeView = new System.Windows.Forms.TreeView();
			this.configXmlEditBox = new Athena.App.Controls.EditorWithHilighting();
			this.configContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.outputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.renderingPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.sceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.addCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.sphereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.meshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addLightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.omniToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.configContextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.configTreeView);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.configXmlEditBox);
			this.splitContainer1.Size = new System.Drawing.Size(1178, 668);
			this.splitContainer1.SplitterDistance = 200;
			this.splitContainer1.SplitterWidth = 2;
			this.splitContainer1.TabIndex = 0;
			// 
			// configTreeView
			// 
			this.configTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))));
			this.configTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.configTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.configTreeView.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.configTreeView.ForeColor = System.Drawing.Color.White;
			this.configTreeView.LineColor = System.Drawing.Color.White;
			this.configTreeView.Location = new System.Drawing.Point(0, 0);
			this.configTreeView.Name = "configTreeView";
			this.configTreeView.Size = new System.Drawing.Size(200, 668);
			this.configTreeView.TabIndex = 6;
			this.configTreeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.sceneTreeView_DoubleClick);
			// 
			// configXmlEditBox
			// 
			this.configXmlEditBox.AcceptsTab = true;
			this.configXmlEditBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))));
			this.configXmlEditBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.configXmlEditBox.ContextMenuStrip = this.configContextMenuStrip;
			this.configXmlEditBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.configXmlEditBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.configXmlEditBox.ForeColor = System.Drawing.Color.White;
			this.configXmlEditBox.Location = new System.Drawing.Point(0, 0);
			this.configXmlEditBox.Name = "configXmlEditBox";
			this.configXmlEditBox.Size = new System.Drawing.Size(976, 668);
			this.configXmlEditBox.TabIndex = 2;
			this.configXmlEditBox.Text = "";
			this.configXmlEditBox.WordWrap = false;
			this.configXmlEditBox.TextChanged += new System.EventHandler(this.configXmlEditBox_TextChanged);
			this.configXmlEditBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.configXmlEditBox_KeyDown);
			// 
			// configContextMenuStrip
			// 
			this.configContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.outputToolStripMenuItem,
            this.renderingPropertiesToolStripMenuItem,
            this.toolStripMenuItem2,
            this.sceneToolStripMenuItem,
            this.toolStripMenuItem1,
            this.addCameraToolStripMenuItem,
            this.addObjectToolStripMenuItem,
            this.addLightToolStripMenuItem});
			this.configContextMenuStrip.Name = "sceneContextMenuStrip";
			this.configContextMenuStrip.Size = new System.Drawing.Size(185, 170);
			// 
			// outputToolStripMenuItem
			// 
			this.outputToolStripMenuItem.Name = "outputToolStripMenuItem";
			this.outputToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
			this.outputToolStripMenuItem.Text = "O&utput";
			this.outputToolStripMenuItem.Click += new System.EventHandler(this.outputToolStripMenuItem_Click);
			// 
			// renderingPropertiesToolStripMenuItem
			// 
			this.renderingPropertiesToolStripMenuItem.Name = "renderingPropertiesToolStripMenuItem";
			this.renderingPropertiesToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
			this.renderingPropertiesToolStripMenuItem.Text = "&Rendering properties";
			this.renderingPropertiesToolStripMenuItem.Click += new System.EventHandler(this.renderingPropertiesToolStripMenuItem_Click);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(181, 6);
			// 
			// sceneToolStripMenuItem
			// 
			this.sceneToolStripMenuItem.Name = "sceneToolStripMenuItem";
			this.sceneToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
			this.sceneToolStripMenuItem.Text = "&Scene";
			this.sceneToolStripMenuItem.Click += new System.EventHandler(this.sceneToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
			// 
			// addCameraToolStripMenuItem
			// 
			this.addCameraToolStripMenuItem.Name = "addCameraToolStripMenuItem";
			this.addCameraToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
			this.addCameraToolStripMenuItem.Text = "Add &Camera";
			this.addCameraToolStripMenuItem.Click += new System.EventHandler(this.addCameraToolStripMenuItem_Click);
			// 
			// addObjectToolStripMenuItem
			// 
			this.addObjectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sphereToolStripMenuItem,
            this.meshToolStripMenuItem});
			this.addObjectToolStripMenuItem.Name = "addObjectToolStripMenuItem";
			this.addObjectToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
			this.addObjectToolStripMenuItem.Text = "Add &Object";
			// 
			// sphereToolStripMenuItem
			// 
			this.sphereToolStripMenuItem.Name = "sphereToolStripMenuItem";
			this.sphereToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
			this.sphereToolStripMenuItem.Text = "&Sphere";
			this.sphereToolStripMenuItem.Click += new System.EventHandler(this.sphereToolStripMenuItem_Click);
			// 
			// meshToolStripMenuItem
			// 
			this.meshToolStripMenuItem.Name = "meshToolStripMenuItem";
			this.meshToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
			this.meshToolStripMenuItem.Text = "&WavefrontObjMesh";
			this.meshToolStripMenuItem.Click += new System.EventHandler(this.meshToolStripMenuItem_Click);
			// 
			// addLightToolStripMenuItem
			// 
			this.addLightToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.omniToolStripMenuItem});
			this.addLightToolStripMenuItem.Name = "addLightToolStripMenuItem";
			this.addLightToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
			this.addLightToolStripMenuItem.Text = "Add &Light";
			// 
			// omniToolStripMenuItem
			// 
			this.omniToolStripMenuItem.Name = "omniToolStripMenuItem";
			this.omniToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
			this.omniToolStripMenuItem.Text = "O&mni";
			this.omniToolStripMenuItem.Click += new System.EventHandler(this.omniToolStripMenuItem_Click);
			// 
			// backgroundWorker
			// 
			this.backgroundWorker.WorkerReportsProgress = true;
			this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
			this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
			this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
			// 
			// ConfigEditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1178, 668);
			this.Controls.Add(this.splitContainer1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MinimizeBox = false;
			this.Name = "ConfigEditorForm";
			this.Text = "Config editor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigEditorForm_FormClosing);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.configContextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TreeView configTreeView;
		private System.Windows.Forms.ContextMenuStrip configContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem addCameraToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addObjectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem sphereToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem meshToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addLightToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem omniToolStripMenuItem;
		private System.ComponentModel.BackgroundWorker backgroundWorker;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem renderingPropertiesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem outputToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem sceneToolStripMenuItem;
		private EditorWithHilighting configXmlEditBox;
	}
}