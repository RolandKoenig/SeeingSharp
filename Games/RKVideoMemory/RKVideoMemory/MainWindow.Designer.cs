namespace RKVideoMemory
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
            this.m_mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_cmdLoadLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.m_dlgOpenDir = new System.Windows.Forms.FolderBrowserDialog();
            this.m_ctrlRenderer = new FrozenSky.Multimedia.Views.FrozenSkyRendererControl();
            this.m_timerPicking = new System.Windows.Forms.Timer(this.components);
            this.m_timerTrigger = new System.Windows.Forms.Timer(this.components);
            this.m_mainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_mainMenu
            // 
            this.m_mainMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.m_mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.m_mainMenu.Location = new System.Drawing.Point(0, 0);
            this.m_mainMenu.Name = "m_mainMenu";
            this.m_mainMenu.Size = new System.Drawing.Size(668, 28);
            this.m_mainMenu.TabIndex = 1;
            this.m_mainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_cmdLoadLevel});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(57, 24);
            this.fileToolStripMenuItem.Text = "Datei";
            // 
            // m_cmdLoadLevel
            // 
            this.m_cmdLoadLevel.Name = "m_cmdLoadLevel";
            this.m_cmdLoadLevel.Size = new System.Drawing.Size(156, 24);
            this.m_cmdLoadLevel.Text = "Level Laden";
            this.m_cmdLoadLevel.Click += new System.EventHandler(this.OnCmdLoadLevel_Click);
            // 
            // m_ctrlRenderer
            // 
            this.m_ctrlRenderer.DiscardRendering = true;
            this.m_ctrlRenderer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_ctrlRenderer.InputMode = FrozenSky.Multimedia.Input.FrozenSkyInputMode.FreeCameraMovement;
            this.m_ctrlRenderer.Location = new System.Drawing.Point(0, 28);
            this.m_ctrlRenderer.Name = "m_ctrlRenderer";
            this.m_ctrlRenderer.Size = new System.Drawing.Size(668, 371);
            this.m_ctrlRenderer.TabIndex = 0;
            this.m_ctrlRenderer.Text = "frozenSkyRendererControl1";
            this.m_ctrlRenderer.ViewConfiguration.AccentuationFactor = 0F;
            this.m_ctrlRenderer.ViewConfiguration.AmbientFactor = 0.5F;
            this.m_ctrlRenderer.ViewConfiguration.AntialiasingEnabled = true;
            this.m_ctrlRenderer.ViewConfiguration.AntialiasingQuality = FrozenSky.Multimedia.Core.AntialiasingQualityLevel.Medium;
            this.m_ctrlRenderer.ViewConfiguration.GeneratedBorderFactor = 1F;
            this.m_ctrlRenderer.ViewConfiguration.GeneratedColorGradientFactor = 1F;
            this.m_ctrlRenderer.ViewConfiguration.LightPower = 0.8F;
            this.m_ctrlRenderer.ViewConfiguration.Overlay2DEnabled = true;
            this.m_ctrlRenderer.ViewConfiguration.ShowTextures = true;
            this.m_ctrlRenderer.ViewConfiguration.StrongLightFactor = 1.5F;
            this.m_ctrlRenderer.ViewConfiguration.WireframeEnabled = false;
            this.m_ctrlRenderer.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnCtrlRenderer_MouseClick);
            // 
            // m_timerPicking
            // 
            this.m_timerPicking.Enabled = true;
            this.m_timerPicking.Tick += new System.EventHandler(this.OnTimerPicking_Tick);
            // 
            // m_timerTrigger
            // 
            this.m_timerTrigger.Enabled = true;
            this.m_timerTrigger.Interval = 1000;
            this.m_timerTrigger.Tick += new System.EventHandler(this.OnTimerTrigger_Tick);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 399);
            this.Controls.Add(this.m_ctrlRenderer);
            this.Controls.Add(this.m_mainMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.m_mainMenu;
            this.Name = "MainWindow";
            this.Text = "RK Video Memory";
            this.m_mainMenu.ResumeLayout(false);
            this.m_mainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FrozenSky.Multimedia.Views.FrozenSkyRendererControl m_ctrlRenderer;
        private System.Windows.Forms.MenuStrip m_mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m_cmdLoadLevel;
        private System.Windows.Forms.FolderBrowserDialog m_dlgOpenDir;
        private System.Windows.Forms.Timer m_timerPicking;
        private System.Windows.Forms.Timer m_timerTrigger;
    }
}

