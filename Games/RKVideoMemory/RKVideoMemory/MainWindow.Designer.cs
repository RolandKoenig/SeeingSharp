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
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.m_cmdClose = new System.Windows.Forms.ToolStripMenuItem();
            this.konfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_chkFullscreen = new System.Windows.Forms.ToolStripMenuItem();
            this.m_dlgOpenDir = new System.Windows.Forms.FolderBrowserDialog();
            this.m_timerPicking = new System.Windows.Forms.Timer(this.components);
            this.m_barStatus = new System.Windows.Forms.StatusStrip();
            this.m_lblLoading = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_prgLoading = new System.Windows.Forms.ToolStripProgressBar();
            this.m_ctrlRenderer = new SeeingSharp.Multimedia.Views.SeeingSharpRendererControl();
            this.m_mediaPlayer = new SeeingSharp.Multimedia.Views.MediaPlayerComponent();
            this.m_behaviorHideMenubar = new RKVideoMemory.Behaviors.HideControlOnInactivityBehavior(this.components);
            this.m_mainMenu.SuspendLayout();
            this.m_barStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_mainMenu
            // 
            this.m_mainMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.m_mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.konfigurationToolStripMenuItem});
            this.m_mainMenu.Location = new System.Drawing.Point(0, 0);
            this.m_mainMenu.Name = "m_mainMenu";
            this.m_mainMenu.Size = new System.Drawing.Size(668, 28);
            this.m_mainMenu.TabIndex = 1;
            this.m_mainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_cmdLoadLevel,
            this.toolStripSeparator1,
            this.m_cmdClose});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(57, 24);
            this.fileToolStripMenuItem.Text = "Datei";
            // 
            // m_cmdLoadLevel
            // 
            this.m_cmdLoadLevel.Image = global::RKVideoMemory.Properties.Resources.Icon_Open16x16;
            this.m_cmdLoadLevel.Name = "m_cmdLoadLevel";
            this.m_cmdLoadLevel.Size = new System.Drawing.Size(160, 26);
            this.m_cmdLoadLevel.Text = "Level Laden";
            this.m_cmdLoadLevel.Click += new System.EventHandler(this.OnCmdLoadLevel_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(157, 6);
            // 
            // m_cmdClose
            // 
            this.m_cmdClose.Name = "m_cmdClose";
            this.m_cmdClose.Size = new System.Drawing.Size(160, 26);
            this.m_cmdClose.Text = "Beenden";
            this.m_cmdClose.Click += new System.EventHandler(this.OnCmdClose_Click);
            // 
            // konfigurationToolStripMenuItem
            // 
            this.konfigurationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_chkFullscreen});
            this.konfigurationToolStripMenuItem.Name = "konfigurationToolStripMenuItem";
            this.konfigurationToolStripMenuItem.Size = new System.Drawing.Size(112, 24);
            this.konfigurationToolStripMenuItem.Text = "Konfiguration";
            // 
            // m_chkFullscreen
            // 
            this.m_chkFullscreen.Image = global::RKVideoMemory.Properties.Resources.Icon_Output16x16;
            this.m_chkFullscreen.Name = "m_chkFullscreen";
            this.m_chkFullscreen.Size = new System.Drawing.Size(134, 26);
            this.m_chkFullscreen.Text = "Vollbild";
            this.m_chkFullscreen.Click += new System.EventHandler(this.OnChkFullscreen_Click);
            // 
            // m_timerPicking
            // 
            this.m_timerPicking.Enabled = true;
            this.m_timerPicking.Tick += new System.EventHandler(this.OnTimerPicking_Tick);
            // 
            // m_barStatus
            // 
            this.m_barStatus.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.m_barStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_lblLoading,
            this.m_prgLoading});
            this.m_barStatus.Location = new System.Drawing.Point(0, 374);
            this.m_barStatus.Name = "m_barStatus";
            this.m_barStatus.Size = new System.Drawing.Size(668, 25);
            this.m_barStatus.TabIndex = 2;
            this.m_barStatus.Visible = false;
            // 
            // m_lblLoading
            // 
            this.m_lblLoading.Name = "m_lblLoading";
            this.m_lblLoading.Size = new System.Drawing.Size(50, 20);
            this.m_lblLoading.Text = "Lade...";
            // 
            // m_prgLoading
            // 
            this.m_prgLoading.MarqueeAnimationSpeed = 20;
            this.m_prgLoading.Name = "m_prgLoading";
            this.m_prgLoading.Size = new System.Drawing.Size(200, 19);
            this.m_prgLoading.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            // 
            // m_ctrlRenderer
            // 
            this.m_ctrlRenderer.DiscardRendering = true;
            this.m_ctrlRenderer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_ctrlRenderer.InputMode = SeeingSharp.Multimedia.Input.SeeingSharpInputMode.NoInput;
            this.m_ctrlRenderer.Location = new System.Drawing.Point(0, 0);
            this.m_ctrlRenderer.Name = "m_ctrlRenderer";
            this.m_ctrlRenderer.Size = new System.Drawing.Size(668, 399);
            this.m_ctrlRenderer.TabIndex = 0;
            this.m_ctrlRenderer.Text = "frozenSkyRendererControl1";
            this.m_ctrlRenderer.ViewConfiguration.AccentuationFactor = 1F;
            this.m_ctrlRenderer.ViewConfiguration.AmbientFactor = 0.5F;
            this.m_ctrlRenderer.ViewConfiguration.AntialiasingEnabled = true;
            this.m_ctrlRenderer.ViewConfiguration.AntialiasingQuality = SeeingSharp.Multimedia.Core.AntialiasingQualityLevel.Medium;
            this.m_ctrlRenderer.ViewConfiguration.GeneratedBorderFactor = 1F;
            this.m_ctrlRenderer.ViewConfiguration.GeneratedColorGradientFactor = 1F;
            this.m_ctrlRenderer.ViewConfiguration.LightPower = 0.8F;
            this.m_ctrlRenderer.ViewConfiguration.Overlay2DEnabled = true;
            this.m_ctrlRenderer.ViewConfiguration.ShowTextures = true;
            this.m_ctrlRenderer.ViewConfiguration.StrongLightFactor = 1.5F;
            this.m_ctrlRenderer.ViewConfiguration.WireframeEnabled = false;
            this.m_ctrlRenderer.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnCtrlRenderer_MouseClick);
            // 
            // m_mediaPlayer
            // 
            this.m_mediaPlayer.RestartVideoWhenFinished = false;
            this.m_mediaPlayer.TargetControl = this.m_ctrlRenderer;
            this.m_mediaPlayer.VideoFinished += new System.EventHandler(this.OnMediaPlayer_VideoFinished);
            // 
            // m_behaviorHideMenubar
            // 
            this.m_behaviorHideMenubar.ControlToHide = this.m_mainMenu;
            this.m_behaviorHideMenubar.InactivitySecs = 2D;
            this.m_behaviorHideMenubar.ObservedControl = this.m_ctrlRenderer;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 399);
            this.Controls.Add(this.m_barStatus);
            this.Controls.Add(this.m_mainMenu);
            this.Controls.Add(this.m_ctrlRenderer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.m_mainMenu;
            this.Name = "MainWindow";
            this.Text = "RK Video Memory";
            this.m_mainMenu.ResumeLayout(false);
            this.m_mainMenu.PerformLayout();
            this.m_barStatus.ResumeLayout(false);
            this.m_barStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SeeingSharp.Multimedia.Views.SeeingSharpRendererControl m_ctrlRenderer;
        private System.Windows.Forms.MenuStrip m_mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m_cmdLoadLevel;
        private System.Windows.Forms.FolderBrowserDialog m_dlgOpenDir;
        private System.Windows.Forms.Timer m_timerPicking;
        private SeeingSharp.Multimedia.Views.MediaPlayerComponent m_mediaPlayer;
        private System.Windows.Forms.ToolStripMenuItem m_cmdClose;
        private System.Windows.Forms.ToolStripMenuItem konfigurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m_chkFullscreen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private Behaviors.HideControlOnInactivityBehavior m_behaviorHideMenubar;
        private System.Windows.Forms.StatusStrip m_barStatus;
        private System.Windows.Forms.ToolStripStatusLabel m_lblLoading;
        private System.Windows.Forms.ToolStripProgressBar m_prgLoading;
    }
}

