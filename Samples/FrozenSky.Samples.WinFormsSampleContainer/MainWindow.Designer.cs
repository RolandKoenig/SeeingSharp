namespace FrozenSky.Samples.WinFormsSampleContainer
{
    partial class MainWindow
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.m_barTools = new System.Windows.Forms.ToolStrip();
            this.m_cboDevice = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.m_cmdCopy = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.m_cmdShowPerformance = new System.Windows.Forms.ToolStripButton();
            this.m_tabControlSamples = new System.Windows.Forms.TabControl();
            this.m_barStatus = new System.Windows.Forms.StatusStrip();
            this.m_lblRenderResolution = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_lblRenderResolutionValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_lblCountObjects = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_lblCountObjectsValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_refreshTimer = new System.Windows.Forms.Timer(this.components);
            this.m_menuMain = new System.Windows.Forms.MenuStrip();
            this.renderingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeRenderResolutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toBigWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.to800x600ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.to1024x768ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.to1280x1024ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.to1600x1200ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_ctrlRenderer = new FrozenSky.Multimedia.Views.FrozenSkyRendererControl();
            this.m_barTools.SuspendLayout();
            this.m_barStatus.SuspendLayout();
            this.m_menuMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_barTools
            // 
            this.m_barTools.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.m_barTools.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.m_barTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_cboDevice,
            this.toolStripSeparator1,
            this.m_cmdCopy,
            this.toolStripSeparator3,
            this.m_cmdShowPerformance});
            this.m_barTools.Location = new System.Drawing.Point(0, 28);
            this.m_barTools.Name = "m_barTools";
            this.m_barTools.Size = new System.Drawing.Size(762, 27);
            this.m_barTools.TabIndex = 0;
            this.m_barTools.Text = "toolStrip1";
            // 
            // m_cboDevice
            // 
            this.m_cboDevice.Image = global::FrozenSky.Samples.WinFormsSampleContainer.Properties.Resources.Adapter16x16;
            this.m_cboDevice.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_cboDevice.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_cboDevice.Name = "m_cboDevice";
            this.m_cboDevice.Size = new System.Drawing.Size(108, 24);
            this.m_cboDevice.Text = "<Device>";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // m_cmdCopy
            // 
            this.m_cmdCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_cmdCopy.Image = global::FrozenSky.Samples.WinFormsSampleContainer.Properties.Resources.Copy16x16;
            this.m_cmdCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_cmdCopy.Name = "m_cmdCopy";
            this.m_cmdCopy.Size = new System.Drawing.Size(24, 24);
            this.m_cmdCopy.Text = "Copy";
            this.m_cmdCopy.Click += new System.EventHandler(this.OnCmdCopy_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 27);
            // 
            // m_cmdShowPerformance
            // 
            this.m_cmdShowPerformance.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_cmdShowPerformance.Image = global::FrozenSky.Samples.WinFormsSampleContainer.Properties.Resources.PerformanceMeasure16x16;
            this.m_cmdShowPerformance.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_cmdShowPerformance.Name = "m_cmdShowPerformance";
            this.m_cmdShowPerformance.Size = new System.Drawing.Size(24, 24);
            this.m_cmdShowPerformance.Text = "toolStripButton1";
            this.m_cmdShowPerformance.Click += new System.EventHandler(this.OnCmdShowPerformance_Click);
            // 
            // m_tabControlSamples
            // 
            this.m_tabControlSamples.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_tabControlSamples.Location = new System.Drawing.Point(0, 55);
            this.m_tabControlSamples.Margin = new System.Windows.Forms.Padding(0);
            this.m_tabControlSamples.Name = "m_tabControlSamples";
            this.m_tabControlSamples.SelectedIndex = 0;
            this.m_tabControlSamples.Size = new System.Drawing.Size(762, 102);
            this.m_tabControlSamples.TabIndex = 3;
            // 
            // m_barStatus
            // 
            this.m_barStatus.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.m_barStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_lblRenderResolution,
            this.m_lblRenderResolutionValue,
            this.m_lblCountObjects,
            this.m_lblCountObjectsValue});
            this.m_barStatus.Location = new System.Drawing.Point(0, 493);
            this.m_barStatus.Name = "m_barStatus";
            this.m_barStatus.Size = new System.Drawing.Size(762, 29);
            this.m_barStatus.TabIndex = 4;
            // 
            // m_lblRenderResolution
            // 
            this.m_lblRenderResolution.Name = "m_lblRenderResolution";
            this.m_lblRenderResolution.Size = new System.Drawing.Size(133, 24);
            this.m_lblRenderResolution.Text = "Render Resolution:";
            // 
            // m_lblRenderResolutionValue
            // 
            this.m_lblRenderResolutionValue.Name = "m_lblRenderResolutionValue";
            this.m_lblRenderResolutionValue.Size = new System.Drawing.Size(99, 24);
            this.m_lblRenderResolutionValue.Text = "<Resolution>";
            // 
            // m_lblCountObjects
            // 
            this.m_lblCountObjects.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.m_lblCountObjects.Name = "m_lblCountObjects";
            this.m_lblCountObjects.Size = new System.Drawing.Size(75, 24);
            this.m_lblCountObjects.Text = "#Objects:";
            // 
            // m_lblCountObjectsValue
            // 
            this.m_lblCountObjectsValue.Name = "m_lblCountObjectsValue";
            this.m_lblCountObjectsValue.Size = new System.Drawing.Size(68, 24);
            this.m_lblCountObjectsValue.Text = "<Count>";
            // 
            // m_refreshTimer
            // 
            this.m_refreshTimer.Enabled = true;
            this.m_refreshTimer.Interval = 500;
            this.m_refreshTimer.Tick += new System.EventHandler(this.OnRefreshTimer_Tick);
            // 
            // m_menuMain
            // 
            this.m_menuMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.m_menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renderingToolStripMenuItem});
            this.m_menuMain.Location = new System.Drawing.Point(0, 0);
            this.m_menuMain.Name = "m_menuMain";
            this.m_menuMain.Size = new System.Drawing.Size(762, 28);
            this.m_menuMain.TabIndex = 5;
            this.m_menuMain.Text = "m_mainMenu";
            // 
            // renderingToolStripMenuItem
            // 
            this.renderingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeRenderResolutionToolStripMenuItem});
            this.renderingToolStripMenuItem.Name = "renderingToolStripMenuItem";
            this.renderingToolStripMenuItem.Size = new System.Drawing.Size(89, 24);
            this.renderingToolStripMenuItem.Text = "Rendering";
            // 
            // changeRenderResolutionToolStripMenuItem
            // 
            this.changeRenderResolutionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toBigWindowToolStripMenuItem,
            this.toolStripSeparator2,
            this.to800x600ToolStripMenuItem,
            this.to1024x768ToolStripMenuItem,
            this.to1280x1024ToolStripMenuItem,
            this.to1600x1200ToolStripMenuItem});
            this.changeRenderResolutionToolStripMenuItem.Name = "changeRenderResolutionToolStripMenuItem";
            this.changeRenderResolutionToolStripMenuItem.Size = new System.Drawing.Size(254, 24);
            this.changeRenderResolutionToolStripMenuItem.Text = "Change render resolution...";
            // 
            // toBigWindowToolStripMenuItem
            // 
            this.toBigWindowToolStripMenuItem.Name = "toBigWindowToolStripMenuItem";
            this.toBigWindowToolStripMenuItem.Size = new System.Drawing.Size(184, 24);
            this.toBigWindowToolStripMenuItem.Text = ".. to big window";
            this.toBigWindowToolStripMenuItem.Click += new System.EventHandler(this.OnCmdChangeResolutionToBigWindow_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(181, 6);
            // 
            // to800x600ToolStripMenuItem
            // 
            this.to800x600ToolStripMenuItem.Name = "to800x600ToolStripMenuItem";
            this.to800x600ToolStripMenuItem.Size = new System.Drawing.Size(184, 24);
            this.to800x600ToolStripMenuItem.Tag = "800x600";
            this.to800x600ToolStripMenuItem.Text = ".. to 800x600";
            this.to800x600ToolStripMenuItem.Click += new System.EventHandler(this.OnCmdChangeResolution_Click);
            // 
            // to1024x768ToolStripMenuItem
            // 
            this.to1024x768ToolStripMenuItem.Name = "to1024x768ToolStripMenuItem";
            this.to1024x768ToolStripMenuItem.Size = new System.Drawing.Size(184, 24);
            this.to1024x768ToolStripMenuItem.Tag = "1024x768";
            this.to1024x768ToolStripMenuItem.Text = ".. to 1024x768";
            this.to1024x768ToolStripMenuItem.Click += new System.EventHandler(this.OnCmdChangeResolution_Click);
            // 
            // to1280x1024ToolStripMenuItem
            // 
            this.to1280x1024ToolStripMenuItem.Name = "to1280x1024ToolStripMenuItem";
            this.to1280x1024ToolStripMenuItem.Size = new System.Drawing.Size(184, 24);
            this.to1280x1024ToolStripMenuItem.Tag = "1280x1024";
            this.to1280x1024ToolStripMenuItem.Text = ".. to 1280x1024";
            this.to1280x1024ToolStripMenuItem.Click += new System.EventHandler(this.OnCmdChangeResolution_Click);
            // 
            // to1600x1200ToolStripMenuItem
            // 
            this.to1600x1200ToolStripMenuItem.Name = "to1600x1200ToolStripMenuItem";
            this.to1600x1200ToolStripMenuItem.Size = new System.Drawing.Size(184, 24);
            this.to1600x1200ToolStripMenuItem.Tag = "1600x1200";
            this.to1600x1200ToolStripMenuItem.Text = ".. to 1600x1200";
            this.to1600x1200ToolStripMenuItem.Click += new System.EventHandler(this.OnCmdChangeResolution_Click);
            // 
            // m_ctrlRenderer
            // 
            this.m_ctrlRenderer.Camera = null;
            this.m_ctrlRenderer.DiscardRendering = true;
            this.m_ctrlRenderer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_ctrlRenderer.Location = new System.Drawing.Point(0, 157);
            this.m_ctrlRenderer.Name = "m_ctrlRenderer";
            this.m_ctrlRenderer.Size = new System.Drawing.Size(762, 336);
            this.m_ctrlRenderer.TabIndex = 6;
            this.m_ctrlRenderer.Text = "frozenSkyRendererControl1";
            this.m_ctrlRenderer.ViewConfiguration.AccentuationFactor = 0F;
            this.m_ctrlRenderer.ViewConfiguration.AmbientFactor = 0.2F;
            this.m_ctrlRenderer.ViewConfiguration.AntialiasingEnabled = true;
            this.m_ctrlRenderer.ViewConfiguration.AntialiasingQuality = FrozenSky.Multimedia.Core.AntialiasingQualityLevel.Medium;
            this.m_ctrlRenderer.ViewConfiguration.GeneratedBorderFactor = 1F;
            this.m_ctrlRenderer.ViewConfiguration.GeneratedColorGradientFactor = 1F;
            this.m_ctrlRenderer.ViewConfiguration.LightPower = 0.8F;
            this.m_ctrlRenderer.ViewConfiguration.Overlay2DEnabled = true;
            this.m_ctrlRenderer.ViewConfiguration.ShowTextures = true;
            this.m_ctrlRenderer.ViewConfiguration.StrongLightFactor = 1.5F;
            this.m_ctrlRenderer.ViewConfiguration.WireframeEnabled = false;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(762, 522);
            this.Controls.Add(this.m_ctrlRenderer);
            this.Controls.Add(this.m_tabControlSamples);
            this.Controls.Add(this.m_barStatus);
            this.Controls.Add(this.m_barTools);
            this.Controls.Add(this.m_menuMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.m_menuMain;
            this.Name = "MainWindow";
            this.Text = "FrozenSky Win.Forms Samples";
            this.m_barTools.ResumeLayout(false);
            this.m_barTools.PerformLayout();
            this.m_barStatus.ResumeLayout(false);
            this.m_barStatus.PerformLayout();
            this.m_menuMain.ResumeLayout(false);
            this.m_menuMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip m_barTools;
        private System.Windows.Forms.TabControl m_tabControlSamples;
        private System.Windows.Forms.ToolStripDropDownButton m_cboDevice;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton m_cmdShowPerformance;
        private System.Windows.Forms.StatusStrip m_barStatus;
        private System.Windows.Forms.ToolStripStatusLabel m_lblRenderResolution;
        private System.Windows.Forms.ToolStripStatusLabel m_lblRenderResolutionValue;
        private System.Windows.Forms.Timer m_refreshTimer;
        private System.Windows.Forms.MenuStrip m_menuMain;
        private System.Windows.Forms.ToolStripMenuItem renderingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeRenderResolutionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem to800x600ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem to1024x768ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem to1280x1024ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem to1600x1200ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toBigWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton m_cmdCopy;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripStatusLabel m_lblCountObjects;
        private System.Windows.Forms.ToolStripStatusLabel m_lblCountObjectsValue;
        private Multimedia.Views.FrozenSkyRendererControl m_ctrlRenderer;
    }
}

