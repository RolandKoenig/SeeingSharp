namespace RKRocket
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.m_barStatus = new System.Windows.Forms.StatusStrip();
            this.m_lblLevel = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_lblLevelValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_lblScore = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_lblScoreValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_barMenu = new System.Windows.Forms.MenuStrip();
            this.m_renderPanel = new SeeingSharp.Multimedia.Views.SeeingSharpRendererControl();
            this.m_dataSource = new System.Windows.Forms.BindingSource(this.components);
            this.m_lblHealth = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_lblHealthValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_barStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataSource)).BeginInit();
            this.SuspendLayout();
            // 
            // m_barStatus
            // 
            this.m_barStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_barStatus.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.m_barStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_lblLevel,
            this.m_lblLevelValue,
            this.m_lblScore,
            this.m_lblScoreValue,
            this.m_lblHealth,
            this.m_lblHealthValue});
            this.m_barStatus.Location = new System.Drawing.Point(0, 24);
            this.m_barStatus.Name = "m_barStatus";
            this.m_barStatus.Size = new System.Drawing.Size(587, 46);
            this.m_barStatus.TabIndex = 0;
            this.m_barStatus.Text = "statusStrip1";
            // 
            // m_lblLevel
            // 
            this.m_lblLevel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblLevel.ForeColor = System.Drawing.Color.Gray;
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
            // m_lblScore
            // 
            this.m_lblScore.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.m_lblScore.ForeColor = System.Drawing.Color.Gray;
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
            // m_barMenu
            // 
            this.m_barMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.m_barMenu.Location = new System.Drawing.Point(0, 0);
            this.m_barMenu.Name = "m_barMenu";
            this.m_barMenu.Size = new System.Drawing.Size(587, 24);
            this.m_barMenu.TabIndex = 1;
            this.m_barMenu.Text = "menuStrip1";
            // 
            // m_renderPanel
            // 
            this.m_renderPanel.DiscardRendering = true;
            this.m_renderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_renderPanel.InputMode = SeeingSharp.Multimedia.Input.SeeingSharpInputMode.FreeCameraMovement;
            this.m_renderPanel.Location = new System.Drawing.Point(0, 70);
            this.m_renderPanel.Name = "m_renderPanel";
            this.m_renderPanel.Size = new System.Drawing.Size(587, 342);
            this.m_renderPanel.TabIndex = 2;
            this.m_renderPanel.Text = "seeingSharpRendererControl1";
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
            // 
            // m_dataSource
            // 
            this.m_dataSource.DataSource = typeof(RKRocket.ViewModel.MainUIViewModel);
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
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 412);
            this.Controls.Add(this.m_renderPanel);
            this.Controls.Add(this.m_barStatus);
            this.Controls.Add(this.m_barMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.m_barMenu;
            this.Name = "MainWindow";
            this.Text = "RK Rocket";
            this.m_barStatus.ResumeLayout(false);
            this.m_barStatus.PerformLayout();
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
        private System.Windows.Forms.ToolStripStatusLabel m_lblScoreValue;
        private System.Windows.Forms.ToolStripStatusLabel m_lblHealth;
        private System.Windows.Forms.ToolStripStatusLabel m_lblHealthValue;
    }
}

