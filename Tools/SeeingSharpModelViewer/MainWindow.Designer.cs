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
            this.m_barStatus = new System.Windows.Forms.StatusStrip();
            this.m_dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.m_panGraphics = new SeeingSharp.Multimedia.Views.SeeingSharpRendererControl();
            this.m_viewFreeMovingCamera = new SeeingSharp.Multimedia.Components.FreeMovingCameraComponent();
            this.m_barTools.SuspendLayout();
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
            this.m_barTools.Size = new System.Drawing.Size(712, 27);
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
            this.m_cmdClose.Image = ((System.Drawing.Image)(resources.GetObject("m_cmdClose.Image")));
            this.m_cmdClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_cmdClose.Name = "m_cmdClose";
            this.m_cmdClose.Size = new System.Drawing.Size(24, 24);
            this.m_cmdClose.Text = "Close";
            this.m_cmdClose.Click += new System.EventHandler(this.OnCmdClose_Click);
            // 
            // m_barStatus
            // 
            this.m_barStatus.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.m_barStatus.Location = new System.Drawing.Point(0, 410);
            this.m_barStatus.Name = "m_barStatus";
            this.m_barStatus.Size = new System.Drawing.Size(712, 22);
            this.m_barStatus.TabIndex = 2;
            this.m_barStatus.Text = "statusStrip1";
            // 
            // m_panGraphics
            // 
            this.m_panGraphics.DiscardRendering = true;
            this.m_panGraphics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_panGraphics.Location = new System.Drawing.Point(0, 27);
            this.m_panGraphics.Name = "m_panGraphics";
            this.m_panGraphics.SceneComponents.Add(this.m_viewFreeMovingCamera);
            this.m_panGraphics.Size = new System.Drawing.Size(712, 383);
            this.m_panGraphics.TabIndex = 0;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(712, 432);
            this.Controls.Add(this.m_panGraphics);
            this.Controls.Add(this.m_barStatus);
            this.Controls.Add(this.m_barTools);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "Form1";
            this.m_barTools.ResumeLayout(false);
            this.m_barTools.PerformLayout();
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
        private SeeingSharp.Multimedia.Components.FreeMovingCameraComponent m_viewFreeMovingCamera;
    }
}

