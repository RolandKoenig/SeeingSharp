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
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.m_tabControlSamples = new System.Windows.Forms.TabControl();
            this.m_images = new System.Windows.Forms.ImageList(this.components);
            this.m_barStatus = new System.Windows.Forms.StatusStrip();
            this.m_lblRenderResolution = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_lblRenderResolutionValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_lblCountObjects = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_lblCountObjectsValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_lblWorking = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_barProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.m_refreshTimer = new System.Windows.Forms.Timer(this.components);
            this.m_ctrlRenderer = new FrozenSky.Multimedia.Views.FrozenSkyRendererControl();
            this.m_splitter = new System.Windows.Forms.SplitContainer();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.m_cboDevice = new System.Windows.Forms.ToolStripDropDownButton();
            this.m_cboRenderResolution = new System.Windows.Forms.ToolStripDropDownButton();
            this.to800x600ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.to1024x768ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.to1280x1024ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.to1600x1200ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.m_cmdCopy = new System.Windows.Forms.ToolStripButton();
            this.m_cmdShowPerformance = new System.Windows.Forms.ToolStripButton();
            this.m_cmdShowSource = new System.Windows.Forms.ToolStripButton();
            this.m_barTools.SuspendLayout();
            this.m_barStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_splitter)).BeginInit();
            this.m_splitter.Panel1.SuspendLayout();
            this.m_splitter.Panel2.SuspendLayout();
            this.m_splitter.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_barTools
            // 
            this.m_barTools.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.m_barTools.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.m_barTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_cboDevice,
            this.toolStripSeparator2,
            this.m_cboRenderResolution,
            this.toolStripSeparator1,
            this.m_cmdShowSource,
            this.m_cmdCopy,
            this.m_cmdShowPerformance});
            this.m_barTools.Location = new System.Drawing.Point(0, 0);
            this.m_barTools.Name = "m_barTools";
            this.m_barTools.Size = new System.Drawing.Size(762, 27);
            this.m_barTools.TabIndex = 0;
            this.m_barTools.Text = "toolStrip1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // m_tabControlSamples
            // 
            this.m_tabControlSamples.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_tabControlSamples.HotTrack = true;
            this.m_tabControlSamples.ImageList = this.m_images;
            this.m_tabControlSamples.ItemSize = new System.Drawing.Size(0, 21);
            this.m_tabControlSamples.Location = new System.Drawing.Point(0, 0);
            this.m_tabControlSamples.Margin = new System.Windows.Forms.Padding(0);
            this.m_tabControlSamples.Name = "m_tabControlSamples";
            this.m_tabControlSamples.SelectedIndex = 0;
            this.m_tabControlSamples.Size = new System.Drawing.Size(762, 128);
            this.m_tabControlSamples.TabIndex = 3;
            // 
            // m_images
            // 
            this.m_images.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.m_images.ImageSize = new System.Drawing.Size(64, 64);
            this.m_images.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // m_barStatus
            // 
            this.m_barStatus.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.m_barStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_lblRenderResolution,
            this.m_lblRenderResolutionValue,
            this.m_lblCountObjects,
            this.m_lblCountObjectsValue,
            this.m_lblWorking,
            this.m_barProgress});
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
            // m_lblWorking
            // 
            this.m_lblWorking.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.m_lblWorking.Name = "m_lblWorking";
            this.m_lblWorking.Size = new System.Drawing.Size(75, 24);
            this.m_lblWorking.Text = "Working..";
            this.m_lblWorking.Visible = false;
            // 
            // m_barProgress
            // 
            this.m_barProgress.MarqueeAnimationSpeed = 20;
            this.m_barProgress.Name = "m_barProgress";
            this.m_barProgress.Size = new System.Drawing.Size(150, 23);
            this.m_barProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.m_barProgress.Visible = false;
            // 
            // m_refreshTimer
            // 
            this.m_refreshTimer.Enabled = true;
            this.m_refreshTimer.Interval = 500;
            this.m_refreshTimer.Tick += new System.EventHandler(this.OnRefreshTimer_Tick);
            // 
            // m_ctrlRenderer
            // 
            this.m_ctrlRenderer.DiscardRendering = true;
            this.m_ctrlRenderer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_ctrlRenderer.Location = new System.Drawing.Point(0, 0);
            this.m_ctrlRenderer.Name = "m_ctrlRenderer";
            this.m_ctrlRenderer.Size = new System.Drawing.Size(762, 334);
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
            // m_splitter
            // 
            this.m_splitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_splitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.m_splitter.Location = new System.Drawing.Point(0, 27);
            this.m_splitter.Name = "m_splitter";
            this.m_splitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // m_splitter.Panel1
            // 
            this.m_splitter.Panel1.Controls.Add(this.m_tabControlSamples);
            // 
            // m_splitter.Panel2
            // 
            this.m_splitter.Panel2.Controls.Add(this.m_ctrlRenderer);
            this.m_splitter.Size = new System.Drawing.Size(762, 466);
            this.m_splitter.SplitterDistance = 128;
            this.m_splitter.TabIndex = 7;
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // m_cboDevice
            // 
            this.m_cboDevice.Image = global::FrozenSky.Samples.WinFormsSampleContainer.Properties.Resources.Adapter16x16;
            this.m_cboDevice.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_cboDevice.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_cboDevice.Name = "m_cboDevice";
            this.m_cboDevice.Size = new System.Drawing.Size(108, 24);
            this.m_cboDevice.Text = "<Device>";
            this.m_cboDevice.ToolTipText = "Change current rendering device";
            // 
            // m_cboRenderResolution
            // 
            this.m_cboRenderResolution.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.to800x600ToolStripMenuItem1,
            this.to1024x768ToolStripMenuItem1,
            this.to1280x1024ToolStripMenuItem1,
            this.to1600x1200ToolStripMenuItem1});
            this.m_cboRenderResolution.Image = global::FrozenSky.Samples.WinFormsSampleContainer.Properties.Resources.Output16x16;
            this.m_cboRenderResolution.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_cboRenderResolution.Name = "m_cboRenderResolution";
            this.m_cboRenderResolution.Size = new System.Drawing.Size(167, 24);
            this.m_cboRenderResolution.Text = "Change Resolution";
            this.m_cboRenderResolution.ToolTipText = "Change rendering resolution (pixels)";
            // 
            // to800x600ToolStripMenuItem1
            // 
            this.to800x600ToolStripMenuItem1.Name = "to800x600ToolStripMenuItem1";
            this.to800x600ToolStripMenuItem1.Size = new System.Drawing.Size(167, 24);
            this.to800x600ToolStripMenuItem1.Tag = "800x600";
            this.to800x600ToolStripMenuItem1.Text = "to 800x600";
            this.to800x600ToolStripMenuItem1.Click += new System.EventHandler(this.OnCmdChangeResolution_Click);
            // 
            // to1024x768ToolStripMenuItem1
            // 
            this.to1024x768ToolStripMenuItem1.Name = "to1024x768ToolStripMenuItem1";
            this.to1024x768ToolStripMenuItem1.Size = new System.Drawing.Size(167, 24);
            this.to1024x768ToolStripMenuItem1.Tag = "1024x768";
            this.to1024x768ToolStripMenuItem1.Text = "to 1024x768";
            this.to1024x768ToolStripMenuItem1.Click += new System.EventHandler(this.OnCmdChangeResolution_Click);
            // 
            // to1280x1024ToolStripMenuItem1
            // 
            this.to1280x1024ToolStripMenuItem1.Name = "to1280x1024ToolStripMenuItem1";
            this.to1280x1024ToolStripMenuItem1.Size = new System.Drawing.Size(167, 24);
            this.to1280x1024ToolStripMenuItem1.Tag = "1280x1024";
            this.to1280x1024ToolStripMenuItem1.Text = "to 1280x1024";
            this.to1280x1024ToolStripMenuItem1.Click += new System.EventHandler(this.OnCmdChangeResolution_Click);
            // 
            // to1600x1200ToolStripMenuItem1
            // 
            this.to1600x1200ToolStripMenuItem1.Name = "to1600x1200ToolStripMenuItem1";
            this.to1600x1200ToolStripMenuItem1.Size = new System.Drawing.Size(167, 24);
            this.to1600x1200ToolStripMenuItem1.Tag = "1600x1200";
            this.to1600x1200ToolStripMenuItem1.Text = "to 1600x1200";
            this.to1600x1200ToolStripMenuItem1.Click += new System.EventHandler(this.OnCmdChangeResolution_Click);
            // 
            // m_cmdCopy
            // 
            this.m_cmdCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_cmdCopy.Image = global::FrozenSky.Samples.WinFormsSampleContainer.Properties.Resources.Camera16x16;
            this.m_cmdCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_cmdCopy.Name = "m_cmdCopy";
            this.m_cmdCopy.Size = new System.Drawing.Size(24, 24);
            this.m_cmdCopy.Text = "Copy";
            this.m_cmdCopy.ToolTipText = "Take a screenshot and copy it to Clipboard";
            this.m_cmdCopy.Click += new System.EventHandler(this.OnCmdCopy_Click);
            // 
            // m_cmdShowPerformance
            // 
            this.m_cmdShowPerformance.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_cmdShowPerformance.Image = global::FrozenSky.Samples.WinFormsSampleContainer.Properties.Resources.PerformanceMeasure16x16;
            this.m_cmdShowPerformance.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_cmdShowPerformance.Name = "m_cmdShowPerformance";
            this.m_cmdShowPerformance.Size = new System.Drawing.Size(24, 24);
            this.m_cmdShowPerformance.Text = "toolStripButton1";
            this.m_cmdShowPerformance.ToolTipText = "Show performance data";
            this.m_cmdShowPerformance.Click += new System.EventHandler(this.OnCmdShowPerformance_Click);
            // 
            // m_cmdShowSource
            // 
            this.m_cmdShowSource.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_cmdShowSource.Image = global::FrozenSky.Samples.WinFormsSampleContainer.Properties.Resources.PageCSharp16x16;
            this.m_cmdShowSource.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_cmdShowSource.Name = "m_cmdShowSource";
            this.m_cmdShowSource.Size = new System.Drawing.Size(24, 24);
            this.m_cmdShowSource.Text = "Show Source";
            this.m_cmdShowSource.Click += new System.EventHandler(this.OnCmdShowSource_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(762, 522);
            this.Controls.Add(this.m_splitter);
            this.Controls.Add(this.m_barStatus);
            this.Controls.Add(this.m_barTools);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "FrozenSky Win.Forms Samples";
            this.m_barTools.ResumeLayout(false);
            this.m_barTools.PerformLayout();
            this.m_barStatus.ResumeLayout(false);
            this.m_barStatus.PerformLayout();
            this.m_splitter.Panel1.ResumeLayout(false);
            this.m_splitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_splitter)).EndInit();
            this.m_splitter.ResumeLayout(false);
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
        private System.Windows.Forms.ToolStripButton m_cmdCopy;
        private System.Windows.Forms.ToolStripStatusLabel m_lblCountObjects;
        private System.Windows.Forms.ToolStripStatusLabel m_lblCountObjectsValue;
        private Multimedia.Views.FrozenSkyRendererControl m_ctrlRenderer;
        private System.Windows.Forms.ImageList m_images;
        private System.Windows.Forms.SplitContainer m_splitter;
        private System.Windows.Forms.ToolStripStatusLabel m_lblWorking;
        private System.Windows.Forms.ToolStripProgressBar m_barProgress;
        private System.Windows.Forms.ToolStripDropDownButton m_cboRenderResolution;
        private System.Windows.Forms.ToolStripMenuItem to800x600ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem to1024x768ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem to1280x1024ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem to1600x1200ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton m_cmdShowSource;
    }
}

