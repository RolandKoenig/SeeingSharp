namespace RKRocket.View
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.m_barStatus = new System.Windows.Forms.StatusStrip();
            this.m_lblLevel = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_lblLevelValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_lblHealth = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_lblHealthValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_lblScore = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_lblScoreValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_lblMaxReached = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_lblMaxReachedValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_barMenu = new System.Windows.Forms.MenuStrip();
            this.m_mnuGame = new System.Windows.Forms.ToolStripMenuItem();
            this.m_mnuStartNew = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.m_cmdPerformanceInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.m_mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.m_mnuInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.m_mnuInfoAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.m_renderPanel = new SeeingSharp.Multimedia.Views.SeeingSharpRendererControl();
            this.m_panChildDialog = new System.Windows.Forms.Panel();
            this.m_panBorder1 = new System.Windows.Forms.Panel();
            this.m_panBorder2 = new System.Windows.Forms.Panel();
            this.m_dataSource = new System.Windows.Forms.BindingSource(this.components);
            this.m_refreshTimer = new System.Windows.Forms.Timer(this.components);
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.m_barStatus.SuspendLayout();
            this.m_barMenu.SuspendLayout();
            this.m_renderPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataSource)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(234, 6);
            // 
            // m_barStatus
            // 
            this.m_barStatus.AllowMerge = false;
            this.m_barStatus.BackColor = System.Drawing.Color.WhiteSmoke;
            this.m_barStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_barStatus.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.m_barStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_lblLevel,
            this.m_lblLevelValue,
            this.m_lblHealth,
            this.m_lblHealthValue,
            this.m_lblScore,
            this.m_lblScoreValue,
            this.m_lblMaxReached,
            this.m_lblMaxReachedValue});
            this.m_barStatus.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.m_barStatus.Location = new System.Drawing.Point(0, 29);
            this.m_barStatus.Name = "m_barStatus";
            this.m_barStatus.Size = new System.Drawing.Size(882, 46);
            this.m_barStatus.SizingGrip = false;
            this.m_barStatus.TabIndex = 0;
            this.m_barStatus.Text = "statusStrip1";
            this.m_barStatus.SizeChanged += new System.EventHandler(this.OnBarStatus_SizeChanged);
            // 
            // m_lblLevel
            // 
            this.m_lblLevel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblLevel.ForeColor = System.Drawing.Color.Gray;
            this.m_lblLevel.Margin = new System.Windows.Forms.Padding(5, 3, 0, 2);
            this.m_lblLevel.Name = "m_lblLevel";
            this.m_lblLevel.Size = new System.Drawing.Size(98, 41);
            this.m_lblLevel.Text = "Level:";
            // 
            // m_lblLevelValue
            // 
            this.m_lblLevelValue.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.m_lblLevelValue.Name = "m_lblLevelValue";
            this.m_lblLevelValue.Size = new System.Drawing.Size(35, 41);
            this.m_lblLevelValue.Text = "1";
            // 
            // m_lblHealth
            // 
            this.m_lblHealth.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.m_lblHealth.ForeColor = System.Drawing.Color.Gray;
            this.m_lblHealth.Name = "m_lblHealth";
            this.m_lblHealth.Size = new System.Drawing.Size(120, 41);
            this.m_lblHealth.Text = "Health:";
            // 
            // m_lblHealthValue
            // 
            this.m_lblHealthValue.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.m_lblHealthValue.Name = "m_lblHealthValue";
            this.m_lblHealthValue.Size = new System.Drawing.Size(35, 41);
            this.m_lblHealthValue.Text = "0";
            // 
            // m_lblScore
            // 
            this.m_lblScore.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.m_lblScore.ForeColor = System.Drawing.Color.Gray;
            this.m_lblScore.Margin = new System.Windows.Forms.Padding(50, 3, 0, 2);
            this.m_lblScore.Name = "m_lblScore";
            this.m_lblScore.Size = new System.Drawing.Size(103, 41);
            this.m_lblScore.Text = "Score:";
            // 
            // m_lblScoreValue
            // 
            this.m_lblScoreValue.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.m_lblScoreValue.Name = "m_lblScoreValue";
            this.m_lblScoreValue.Size = new System.Drawing.Size(35, 41);
            this.m_lblScoreValue.Text = "0";
            // 
            // m_lblMaxReached
            // 
            this.m_lblMaxReached.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.m_lblMaxReached.ForeColor = System.Drawing.Color.Gray;
            this.m_lblMaxReached.Name = "m_lblMaxReached";
            this.m_lblMaxReached.Size = new System.Drawing.Size(214, 41);
            this.m_lblMaxReached.Text = "Max Reached:";
            // 
            // m_lblMaxReachedValue
            // 
            this.m_lblMaxReachedValue.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.m_lblMaxReachedValue.Name = "m_lblMaxReachedValue";
            this.m_lblMaxReachedValue.Size = new System.Drawing.Size(35, 41);
            this.m_lblMaxReachedValue.Text = "0";
            // 
            // m_barMenu
            // 
            this.m_barMenu.BackColor = System.Drawing.Color.White;
            this.m_barMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.m_barMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_mnuGame,
            this.m_mnuInfo});
            this.m_barMenu.Location = new System.Drawing.Point(0, 0);
            this.m_barMenu.Name = "m_barMenu";
            this.m_barMenu.Size = new System.Drawing.Size(882, 28);
            this.m_barMenu.TabIndex = 1;
            this.m_barMenu.Text = "menuStrip1";
            // 
            // m_mnuGame
            // 
            this.m_mnuGame.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_mnuStartNew,
            this.toolStripSeparator2,
            this.m_cmdPerformanceInfo,
            toolStripSeparator1,
            this.m_mnuExit});
            this.m_mnuGame.Name = "m_mnuGame";
            this.m_mnuGame.ShortcutKeyDisplayString = "";
            this.m_mnuGame.Size = new System.Drawing.Size(60, 24);
            this.m_mnuGame.Text = "&Game";
            // 
            // m_mnuStartNew
            // 
            this.m_mnuStartNew.Name = "m_mnuStartNew";
            this.m_mnuStartNew.Size = new System.Drawing.Size(237, 26);
            this.m_mnuStartNew.Text = "Start New";
            this.m_mnuStartNew.Click += new System.EventHandler(this.OnMnuStartNew_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(234, 6);
            // 
            // m_cmdPerformanceInfo
            // 
            this.m_cmdPerformanceInfo.Name = "m_cmdPerformanceInfo";
            this.m_cmdPerformanceInfo.Size = new System.Drawing.Size(237, 26);
            this.m_cmdPerformanceInfo.Text = "Show Performance Info";
            this.m_cmdPerformanceInfo.Click += new System.EventHandler(this.OnCmdPerformanceInfo_Click);
            // 
            // m_mnuExit
            // 
            this.m_mnuExit.Name = "m_mnuExit";
            this.m_mnuExit.Size = new System.Drawing.Size(237, 26);
            this.m_mnuExit.Text = "Exit";
            this.m_mnuExit.Click += new System.EventHandler(this.OnMnuExit_Click);
            // 
            // m_mnuInfo
            // 
            this.m_mnuInfo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_mnuInfoAbout});
            this.m_mnuInfo.Name = "m_mnuInfo";
            this.m_mnuInfo.Size = new System.Drawing.Size(47, 24);
            this.m_mnuInfo.Text = "&Info";
            // 
            // m_mnuInfoAbout
            // 
            this.m_mnuInfoAbout.Name = "m_mnuInfoAbout";
            this.m_mnuInfoAbout.Size = new System.Drawing.Size(125, 26);
            this.m_mnuInfoAbout.Text = "About";
            // 
            // m_renderPanel
            // 
            this.m_renderPanel.Controls.Add(this.m_panChildDialog);
            this.m_renderPanel.DiscardRendering = true;
            this.m_renderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_renderPanel.InputMode = SeeingSharp.Multimedia.Input.SeeingSharpInputMode.FreeCameraMovement;
            this.m_renderPanel.Location = new System.Drawing.Point(0, 76);
            this.m_renderPanel.Margin = new System.Windows.Forms.Padding(0);
            this.m_renderPanel.Name = "m_renderPanel";
            this.m_renderPanel.Padding = new System.Windows.Forms.Padding(100);
            this.m_renderPanel.Size = new System.Drawing.Size(882, 477);
            this.m_renderPanel.TabIndex = 2;
            this.m_renderPanel.ViewConfiguration.AccentuationFactor = 0F;
            this.m_renderPanel.ViewConfiguration.AlphaEnabledSwapChain = false;
            this.m_renderPanel.ViewConfiguration.AmbientFactor = 0.2F;
            this.m_renderPanel.ViewConfiguration.AntialiasingEnabled = true;
            this.m_renderPanel.ViewConfiguration.AntialiasingQuality = SeeingSharp.Multimedia.Core.AntialiasingQualityLevel.Medium;
            this.m_renderPanel.ViewConfiguration.GeneratedBorderFactor = 1F;
            this.m_renderPanel.ViewConfiguration.GeneratedColorGradientFactor = 1F;
            this.m_renderPanel.ViewConfiguration.LightPower = 0.8F;
            this.m_renderPanel.ViewConfiguration.ShowTextures = true;
            this.m_renderPanel.ViewConfiguration.StrongLightFactor = 1.5F;
            this.m_renderPanel.ViewConfiguration.WireframeEnabled = false;
            this.m_renderPanel.MouseEnter += new System.EventHandler(this.OnRenderPanel_MouseEnter);
            this.m_renderPanel.MouseLeave += new System.EventHandler(this.OnRenderPanel_MouseLeave);
            // 
            // m_panChildDialog
            // 
            this.m_panChildDialog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_panChildDialog.Location = new System.Drawing.Point(100, 100);
            this.m_panChildDialog.Margin = new System.Windows.Forms.Padding(0);
            this.m_panChildDialog.Name = "m_panChildDialog";
            this.m_panChildDialog.Size = new System.Drawing.Size(682, 277);
            this.m_panChildDialog.TabIndex = 5;
            this.m_panChildDialog.Visible = false;
            // 
            // m_panBorder1
            // 
            this.m_panBorder1.BackColor = System.Drawing.Color.Gainsboro;
            this.m_panBorder1.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_panBorder1.Location = new System.Drawing.Point(0, 28);
            this.m_panBorder1.Name = "m_panBorder1";
            this.m_panBorder1.Size = new System.Drawing.Size(882, 1);
            this.m_panBorder1.TabIndex = 3;
            // 
            // m_panBorder2
            // 
            this.m_panBorder2.BackColor = System.Drawing.Color.Gainsboro;
            this.m_panBorder2.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_panBorder2.Location = new System.Drawing.Point(0, 75);
            this.m_panBorder2.Name = "m_panBorder2";
            this.m_panBorder2.Size = new System.Drawing.Size(882, 1);
            this.m_panBorder2.TabIndex = 4;
            // 
            // m_dataSource
            // 
            this.m_dataSource.DataSource = typeof(RKRocket.ViewModel.MainUIViewModel);
            // 
            // m_refreshTimer
            // 
            this.m_refreshTimer.Enabled = true;
            this.m_refreshTimer.Interval = 300;
            this.m_refreshTimer.Tick += new System.EventHandler(this.OnRefreshTimer_Tick);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(882, 553);
            this.Controls.Add(this.m_renderPanel);
            this.Controls.Add(this.m_panBorder2);
            this.Controls.Add(this.m_barStatus);
            this.Controls.Add(this.m_panBorder1);
            this.Controls.Add(this.m_barMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.m_barMenu;
            this.Name = "MainWindow";
            this.Text = "RK Rocket";
            this.m_barStatus.ResumeLayout(false);
            this.m_barStatus.PerformLayout();
            this.m_barMenu.ResumeLayout(false);
            this.m_barMenu.PerformLayout();
            this.m_renderPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_dataSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip m_barStatus;
        private System.Windows.Forms.MenuStrip m_barMenu;
        private SeeingSharp.Multimedia.Views.SeeingSharpRendererControl m_renderPanel;
        private System.Windows.Forms.ToolStripStatusLabel m_lblLevel;
        private System.Windows.Forms.ToolStripStatusLabel m_lblLevelValue;
        private System.Windows.Forms.BindingSource m_dataSource;
        private System.Windows.Forms.ToolStripStatusLabel m_lblScore;
        private System.Windows.Forms.ToolStripStatusLabel m_lblMaxReachedValue;
        private System.Windows.Forms.ToolStripStatusLabel m_lblHealth;
        private System.Windows.Forms.ToolStripStatusLabel m_lblHealthValue;
        private System.Windows.Forms.ToolStripMenuItem m_mnuGame;
        private System.Windows.Forms.ToolStripMenuItem m_mnuStartNew;
        private System.Windows.Forms.ToolStripMenuItem m_mnuExit;
        private System.Windows.Forms.Panel m_panBorder1;
        private System.Windows.Forms.Panel m_panBorder2;
        private System.Windows.Forms.ToolStripMenuItem m_mnuInfo;
        private System.Windows.Forms.ToolStripMenuItem m_mnuInfoAbout;
        private System.Windows.Forms.ToolStripStatusLabel m_lblMaxReached;
        private System.Windows.Forms.ToolStripStatusLabel m_lblScoreValue;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem m_cmdPerformanceInfo;
        private System.Windows.Forms.Panel m_panChildDialog;
        private System.Windows.Forms.Timer m_refreshTimer;
    }
}

