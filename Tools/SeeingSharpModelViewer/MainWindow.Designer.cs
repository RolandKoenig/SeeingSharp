﻿namespace SeeingSharpModelViewer
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.m_barTools = new System.Windows.Forms.ToolStrip();
            this.m_cmdOpen = new System.Windows.Forms.ToolStripButton();
            this.m_cmdClose = new System.Windows.Forms.ToolStripButton();
            this.m_barStatus = new System.Windows.Forms.StatusStrip();
            this.m_dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.m_panGraphics = new SeeingSharp.Multimedia.Views.SeeingSharpRendererControl();
            this.m_camera = new SeeingSharp.Multimedia.Components.FreeMovingCameraComponent();
            this.m_splitter = new System.Windows.Forms.SplitContainer();
            this.m_splitterLeft = new System.Windows.Forms.SplitContainer();
            this.m_grpImporter = new System.Windows.Forms.GroupBox();
            this.m_propertiesImporter = new System.Windows.Forms.PropertyGrid();
            this.m_cmdRefresh = new System.Windows.Forms.Button();
            this.m_barTools.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_splitter)).BeginInit();
            this.m_splitter.Panel1.SuspendLayout();
            this.m_splitter.Panel2.SuspendLayout();
            this.m_splitter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_splitterLeft)).BeginInit();
            this.m_splitterLeft.Panel1.SuspendLayout();
            this.m_splitterLeft.SuspendLayout();
            this.m_grpImporter.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_barTools
            // 
            this.m_barTools.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.m_barTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_cmdOpen,
            this.m_cmdClose});
            this.m_barTools.Location = new System.Drawing.Point(0, 0);
            this.m_barTools.Name = "m_barTools";
            this.m_barTools.Size = new System.Drawing.Size(753, 27);
            this.m_barTools.TabIndex = 1;
            this.m_barTools.Text = "toolStrip1";
            // 
            // m_cmdOpen
            // 
            this.m_cmdOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_cmdOpen.Image = global::SeeingSharpModelViewer.Properties.Resources.Icon_Open16x16;
            this.m_cmdOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_cmdOpen.Name = "m_cmdOpen";
            this.m_cmdOpen.Size = new System.Drawing.Size(24, 24);
            this.m_cmdOpen.Text = "Open";
            this.m_cmdOpen.Click += new System.EventHandler(this.OnCmdOpen_Click);
            // 
            // m_cmdClose
            // 
            this.m_cmdClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_cmdClose.Image = global::SeeingSharpModelViewer.Properties.Resources.Icon_Close16x16;
            this.m_cmdClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_cmdClose.Name = "m_cmdClose";
            this.m_cmdClose.Size = new System.Drawing.Size(24, 24);
            this.m_cmdClose.Text = "Close";
            this.m_cmdClose.Click += new System.EventHandler(this.OnCmdClose_Click);
            // 
            // m_barStatus
            // 
            this.m_barStatus.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.m_barStatus.Location = new System.Drawing.Point(0, 464);
            this.m_barStatus.Name = "m_barStatus";
            this.m_barStatus.Size = new System.Drawing.Size(753, 22);
            this.m_barStatus.TabIndex = 2;
            this.m_barStatus.Text = "statusStrip1";
            // 
            // m_panGraphics
            // 
            this.m_panGraphics.DiscardRendering = true;
            this.m_panGraphics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_panGraphics.Location = new System.Drawing.Point(0, 0);
            this.m_panGraphics.Name = "m_panGraphics";
            this.m_panGraphics.SceneComponents.Add(this.m_camera);
            this.m_panGraphics.Size = new System.Drawing.Size(509, 437);
            this.m_panGraphics.TabIndex = 0;
            // 
            // m_splitter
            // 
            this.m_splitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_splitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.m_splitter.Location = new System.Drawing.Point(0, 27);
            this.m_splitter.Name = "m_splitter";
            // 
            // m_splitter.Panel1
            // 
            this.m_splitter.Panel1.Controls.Add(this.m_splitterLeft);
            // 
            // m_splitter.Panel2
            // 
            this.m_splitter.Panel2.Controls.Add(this.m_panGraphics);
            this.m_splitter.Size = new System.Drawing.Size(753, 437);
            this.m_splitter.SplitterDistance = 240;
            this.m_splitter.TabIndex = 3;
            // 
            // m_splitterLeft
            // 
            this.m_splitterLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_splitterLeft.Location = new System.Drawing.Point(0, 0);
            this.m_splitterLeft.Name = "m_splitterLeft";
            this.m_splitterLeft.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // m_splitterLeft.Panel1
            // 
            this.m_splitterLeft.Panel1.Controls.Add(this.m_grpImporter);
            this.m_splitterLeft.Size = new System.Drawing.Size(240, 437);
            this.m_splitterLeft.SplitterDistance = 227;
            this.m_splitterLeft.TabIndex = 0;
            // 
            // m_grpImporter
            // 
            this.m_grpImporter.Controls.Add(this.m_propertiesImporter);
            this.m_grpImporter.Controls.Add(this.m_cmdRefresh);
            this.m_grpImporter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_grpImporter.Location = new System.Drawing.Point(0, 0);
            this.m_grpImporter.Name = "m_grpImporter";
            this.m_grpImporter.Size = new System.Drawing.Size(240, 227);
            this.m_grpImporter.TabIndex = 0;
            this.m_grpImporter.TabStop = false;
            this.m_grpImporter.Text = "Importer Settings";
            // 
            // m_propertiesImporter
            // 
            this.m_propertiesImporter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_propertiesImporter.HelpVisible = false;
            this.m_propertiesImporter.Location = new System.Drawing.Point(3, 41);
            this.m_propertiesImporter.Name = "m_propertiesImporter";
            this.m_propertiesImporter.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.m_propertiesImporter.Size = new System.Drawing.Size(234, 183);
            this.m_propertiesImporter.TabIndex = 0;
            this.m_propertiesImporter.ToolbarVisible = false;
            // 
            // m_cmdRefresh
            // 
            this.m_cmdRefresh.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_cmdRefresh.Location = new System.Drawing.Point(3, 18);
            this.m_cmdRefresh.Name = "m_cmdRefresh";
            this.m_cmdRefresh.Size = new System.Drawing.Size(234, 23);
            this.m_cmdRefresh.TabIndex = 1;
            this.m_cmdRefresh.Text = "Reload Object";
            this.m_cmdRefresh.UseVisualStyleBackColor = true;
            this.m_cmdRefresh.Click += new System.EventHandler(this.OnCmdReloadObject_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(753, 486);
            this.Controls.Add(this.m_splitter);
            this.Controls.Add(this.m_barStatus);
            this.Controls.Add(this.m_barTools);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "Form1";
            this.m_barTools.ResumeLayout(false);
            this.m_barTools.PerformLayout();
            this.m_splitter.Panel1.ResumeLayout(false);
            this.m_splitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_splitter)).EndInit();
            this.m_splitter.ResumeLayout(false);
            this.m_splitterLeft.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_splitterLeft)).EndInit();
            this.m_splitterLeft.ResumeLayout(false);
            this.m_grpImporter.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SeeingSharp.Multimedia.Views.SeeingSharpRendererControl m_panGraphics;
        private System.Windows.Forms.ToolStrip m_barTools;
        private System.Windows.Forms.StatusStrip m_barStatus;
        private System.Windows.Forms.ToolStripButton m_cmdOpen;
        private System.Windows.Forms.OpenFileDialog m_dlgOpenFile;
        private System.Windows.Forms.ToolStripButton m_cmdClose;
        private System.Windows.Forms.SplitContainer m_splitter;
        private System.Windows.Forms.SplitContainer m_splitterLeft;
        private System.Windows.Forms.GroupBox m_grpImporter;
        private System.Windows.Forms.PropertyGrid m_propertiesImporter;
        private SeeingSharp.Multimedia.Components.FreeMovingCameraComponent m_camera;
        private System.Windows.Forms.Button m_cmdRefresh;
    }
}

