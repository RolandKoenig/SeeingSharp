namespace SeeingSharpModelViewer
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
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.m_cmdCopyScreenshot = new System.Windows.Forms.ToolStripButton();
            this.m_toolSeparator_01 = new System.Windows.Forms.ToolStripSeparator();
            this.m_cboDevice = new System.Windows.Forms.ToolStripDropDownButton();
            this.m_toolSeparator_02 = new System.Windows.Forms.ToolStripSeparator();
            this.m_toolSeparator_03 = new System.Windows.Forms.ToolStripButton();
            this.m_barStatus = new System.Windows.Forms.StatusStrip();
            this.m_lblCurrentFileDesc = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_lblCurrentFile = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_lblProgress = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_barProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.m_dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.m_panGraphics = new SeeingSharp.Multimedia.Views.SeeingSharpRendererControl();
            this.m_camera = new SeeingSharp.Multimedia.Components.FreeMovingCameraComponent();
            this.m_splitter = new System.Windows.Forms.SplitContainer();
            this.m_splitterLeft = new System.Windows.Forms.SplitContainer();
            this.m_grpImporter = new System.Windows.Forms.GroupBox();
            this.m_propertiesImporter = new System.Windows.Forms.PropertyGrid();
            this.m_cmdReloadObject = new System.Windows.Forms.Button();
            this.m_chkShowGround = new System.Windows.Forms.ToolStripButton();
            this.m_toolSeparator_04 = new System.Windows.Forms.ToolStripSeparator();
            this.m_barTools.SuspendLayout();
            this.m_barStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_splitter)).BeginInit();
            this.m_splitter.Panel1.SuspendLayout();
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
            this.m_cmdClose,
            this.toolStripSeparator1,
            this.m_cmdCopyScreenshot,
            this.m_toolSeparator_01,
            this.m_cboDevice,
            this.m_toolSeparator_02,
            this.m_chkShowGround,
            this.m_toolSeparator_04,
            this.m_toolSeparator_03});
            this.m_barTools.Location = new System.Drawing.Point(0, 0);
            this.m_barTools.Name = "m_barTools";
            this.m_barTools.Size = new System.Drawing.Size(789, 27);
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
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // m_cmdCopyScreenshot
            // 
            this.m_cmdCopyScreenshot.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_cmdCopyScreenshot.Image = global::SeeingSharpModelViewer.Properties.Resources.Icon_Screenshot16x16;
            this.m_cmdCopyScreenshot.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_cmdCopyScreenshot.Name = "m_cmdCopyScreenshot";
            this.m_cmdCopyScreenshot.Size = new System.Drawing.Size(24, 24);
            this.m_cmdCopyScreenshot.Text = "Copy screenshot";
            this.m_cmdCopyScreenshot.Click += new System.EventHandler(this.OnCmdCopyScreenshot_Click);
            // 
            // m_toolSeparator_01
            // 
            this.m_toolSeparator_01.Name = "m_toolSeparator_01";
            this.m_toolSeparator_01.Size = new System.Drawing.Size(6, 27);
            // 
            // m_cboDevice
            // 
            this.m_cboDevice.Image = global::SeeingSharpModelViewer.Properties.Resources.Icon_Adapter16x16;
            this.m_cboDevice.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_cboDevice.Name = "m_cboDevice";
            this.m_cboDevice.Size = new System.Drawing.Size(108, 24);
            this.m_cboDevice.Text = "<Device>";
            // 
            // m_toolSeparator_02
            // 
            this.m_toolSeparator_02.Name = "m_toolSeparator_02";
            this.m_toolSeparator_02.Size = new System.Drawing.Size(6, 27);
            // 
            // m_toolSeparator_03
            // 
            this.m_toolSeparator_03.CheckOnClick = true;
            this.m_toolSeparator_03.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_toolSeparator_03.Image = global::SeeingSharpModelViewer.Properties.Resources.Icon_Wireframe16x16;
            this.m_toolSeparator_03.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_toolSeparator_03.Name = "m_toolSeparator_03";
            this.m_toolSeparator_03.Size = new System.Drawing.Size(24, 24);
            this.m_toolSeparator_03.Text = "toolStripButton1";
            this.m_toolSeparator_03.ToolTipText = "Enables / disables wireframe rendering";
            this.m_toolSeparator_03.Click += new System.EventHandler(this.OnChkWireFrame_Click);
            // 
            // m_barStatus
            // 
            this.m_barStatus.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.m_barStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_lblCurrentFileDesc,
            this.m_lblCurrentFile,
            this.m_lblProgress,
            this.m_barProgress});
            this.m_barStatus.Location = new System.Drawing.Point(0, 488);
            this.m_barStatus.Name = "m_barStatus";
            this.m_barStatus.Size = new System.Drawing.Size(789, 29);
            this.m_barStatus.TabIndex = 2;
            this.m_barStatus.Text = "statusStrip1";
            // 
            // m_lblCurrentFileDesc
            // 
            this.m_lblCurrentFileDesc.Name = "m_lblCurrentFileDesc";
            this.m_lblCurrentFileDesc.Size = new System.Drawing.Size(87, 24);
            this.m_lblCurrentFileDesc.Text = "Current File:";
            // 
            // m_lblCurrentFile
            // 
            this.m_lblCurrentFile.Name = "m_lblCurrentFile";
            this.m_lblCurrentFile.Size = new System.Drawing.Size(92, 24);
            this.m_lblCurrentFile.Text = "<FileName>";
            // 
            // m_lblProgress
            // 
            this.m_lblProgress.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.m_lblProgress.Name = "m_lblProgress";
            this.m_lblProgress.Size = new System.Drawing.Size(76, 24);
            this.m_lblProgress.Text = "Loading...";
            // 
            // m_barProgress
            // 
            this.m_barProgress.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.m_barProgress.Name = "m_barProgress";
            this.m_barProgress.Size = new System.Drawing.Size(100, 23);
            this.m_barProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            // 
            // m_panGraphics
            // 
            this.m_panGraphics.DiscardRendering = true;
            this.m_panGraphics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_panGraphics.Location = new System.Drawing.Point(0, 0);
            this.m_panGraphics.Name = "m_panGraphics";
            this.m_panGraphics.SceneComponents.Add(this.m_camera);
            this.m_panGraphics.Size = new System.Drawing.Size(545, 461);
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
            this.m_splitter.Panel2.Controls.Add(this.m_panGraphics);
            this.m_splitter.Size = new System.Drawing.Size(789, 461);
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
            this.m_splitterLeft.Size = new System.Drawing.Size(240, 461);
            this.m_splitterLeft.SplitterDistance = 238;
            this.m_splitterLeft.TabIndex = 0;
            // 
            // m_grpImporter
            // 
            this.m_grpImporter.Controls.Add(this.m_propertiesImporter);
            this.m_grpImporter.Controls.Add(this.m_cmdReloadObject);
            this.m_grpImporter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_grpImporter.Location = new System.Drawing.Point(0, 0);
            this.m_grpImporter.Name = "m_grpImporter";
            this.m_grpImporter.Size = new System.Drawing.Size(240, 238);
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
            this.m_propertiesImporter.Size = new System.Drawing.Size(234, 194);
            this.m_propertiesImporter.TabIndex = 0;
            this.m_propertiesImporter.ToolbarVisible = false;
            // 
            // m_cmdReloadObject
            // 
            this.m_cmdReloadObject.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_cmdReloadObject.Location = new System.Drawing.Point(3, 18);
            this.m_cmdReloadObject.Name = "m_cmdReloadObject";
            this.m_cmdReloadObject.Size = new System.Drawing.Size(234, 23);
            this.m_cmdReloadObject.TabIndex = 1;
            this.m_cmdReloadObject.Text = "Reload Object";
            this.m_cmdReloadObject.UseVisualStyleBackColor = true;
            this.m_cmdReloadObject.Click += new System.EventHandler(this.OnCmdReloadObject_Click);
            // 
            // m_chkShowGround
            // 
            this.m_chkShowGround.Checked = true;
            this.m_chkShowGround.CheckOnClick = true;
            this.m_chkShowGround.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_chkShowGround.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_chkShowGround.Image = global::SeeingSharpModelViewer.Properties.Resources.Icon_Floor16x16;
            this.m_chkShowGround.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_chkShowGround.Name = "m_chkShowGround";
            this.m_chkShowGround.Size = new System.Drawing.Size(24, 24);
            this.m_chkShowGround.Text = "Show floor";
            this.m_chkShowGround.CheckedChanged += new System.EventHandler(this.OnChkShowGround_CheckedChanged);
            // 
            // m_toolSeparator_04
            // 
            this.m_toolSeparator_04.Name = "m_toolSeparator_04";
            this.m_toolSeparator_04.Size = new System.Drawing.Size(6, 27);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 517);
            this.Controls.Add(this.m_splitter);
            this.Controls.Add(this.m_barStatus);
            this.Controls.Add(this.m_barTools);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "Form1";
            this.m_barTools.ResumeLayout(false);
            this.m_barTools.PerformLayout();
            this.m_barStatus.ResumeLayout(false);
            this.m_barStatus.PerformLayout();
            this.m_splitter.Panel1.ResumeLayout(false);
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
        private System.Windows.Forms.Button m_cmdReloadObject;
        private System.Windows.Forms.ToolStripSeparator m_toolSeparator_01;
        private System.Windows.Forms.ToolStripStatusLabel m_lblCurrentFileDesc;
        private System.Windows.Forms.ToolStripStatusLabel m_lblCurrentFile;
        private System.Windows.Forms.ToolStripDropDownButton m_cboDevice;
        private System.Windows.Forms.ToolStripStatusLabel m_lblProgress;
        private System.Windows.Forms.ToolStripProgressBar m_barProgress;
        private System.Windows.Forms.ToolStripButton m_toolSeparator_03;
        private System.Windows.Forms.ToolStripSeparator m_toolSeparator_02;
        private System.Windows.Forms.ToolStripButton m_cmdCopyScreenshot;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton m_chkShowGround;
        private System.Windows.Forms.ToolStripSeparator m_toolSeparator_04;
    }
}

