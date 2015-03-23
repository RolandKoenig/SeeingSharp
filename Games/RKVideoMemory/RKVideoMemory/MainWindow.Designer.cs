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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.frozenSkyRendererControl1 = new FrozenSky.Multimedia.Views.FrozenSkyRendererControl();
            this.m_mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_mainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // frozenSkyRendererControl1
            // 
            this.frozenSkyRendererControl1.Camera = null;
            this.frozenSkyRendererControl1.DiscardRendering = true;
            this.frozenSkyRendererControl1.Location = new System.Drawing.Point(49, 62);
            this.frozenSkyRendererControl1.Name = "frozenSkyRendererControl1";
            this.frozenSkyRendererControl1.Scene = null;
            this.frozenSkyRendererControl1.Size = new System.Drawing.Size(501, 280);
            this.frozenSkyRendererControl1.TabIndex = 0;
            this.frozenSkyRendererControl1.Text = "frozenSkyRendererControl1";
            this.frozenSkyRendererControl1.ViewConfiguration.AccentuationFactor = 0F;
            this.frozenSkyRendererControl1.ViewConfiguration.AmbientFactor = 0.2F;
            this.frozenSkyRendererControl1.ViewConfiguration.AntialiasingEnabled = true;
            this.frozenSkyRendererControl1.ViewConfiguration.AntialiasingQuality = FrozenSky.Multimedia.Core.AntialiasingQualityLevel.Medium;
            this.frozenSkyRendererControl1.ViewConfiguration.GeneratedBorderFactor = 1F;
            this.frozenSkyRendererControl1.ViewConfiguration.GeneratedColorGradientFactor = 1F;
            this.frozenSkyRendererControl1.ViewConfiguration.LightPower = 0.8F;
            this.frozenSkyRendererControl1.ViewConfiguration.Overlay2DEnabled = true;
            this.frozenSkyRendererControl1.ViewConfiguration.ShowTextures = true;
            this.frozenSkyRendererControl1.ViewConfiguration.StrongLightFactor = 1.5F;
            this.frozenSkyRendererControl1.ViewConfiguration.WireframeEnabled = false;
            // 
            // m_mainMenu
            // 
            this.m_mainMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.m_mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.m_mainMenu.Location = new System.Drawing.Point(0, 0);
            this.m_mainMenu.Name = "m_mainMenu";
            this.m_mainMenu.Size = new System.Drawing.Size(657, 28);
            this.m_mainMenu.TabIndex = 1;
            this.m_mainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(57, 24);
            this.fileToolStripMenuItem.Text = "Datei";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(657, 418);
            this.Controls.Add(this.frozenSkyRendererControl1);
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

        private FrozenSky.Multimedia.Views.FrozenSkyRendererControl frozenSkyRendererControl1;
        private System.Windows.Forms.MenuStrip m_mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    }
}

