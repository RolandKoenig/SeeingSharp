namespace FrozenSky.Samples.VirtualTestRoom
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
            this.m_ctrlRenderer = new FrozenSky.Multimedia.Views.FrozenSkyRendererControl();
            this.m_dataSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.m_dataSource)).BeginInit();
            this.SuspendLayout();
            // 
            // m_ctrlRenderer
            // 
            this.m_ctrlRenderer.BackColor = System.Drawing.Color.LightBlue;
            this.m_ctrlRenderer.Camera = null;
            this.m_ctrlRenderer.DiscardRendering = false;
            this.m_ctrlRenderer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_ctrlRenderer.Location = new System.Drawing.Point(0, 0);
            this.m_ctrlRenderer.Name = "m_ctrlRenderer";
            this.m_ctrlRenderer.Scene = null;
            this.m_ctrlRenderer.Size = new System.Drawing.Size(792, 398);
            this.m_ctrlRenderer.TabIndex = 0;
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
            // m_dataSource
            // 
            this.m_dataSource.DataSource = typeof(FrozenSky.Samples.VirtualTestRoom.TestingRoomViewModel);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 398);
            this.Controls.Add(this.m_ctrlRenderer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "FrozenSky Samples - Virtual Test Room";
            ((System.ComponentModel.ISupportInitialize)(this.m_dataSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Multimedia.Views.FrozenSkyRendererControl m_ctrlRenderer;
        private System.Windows.Forms.BindingSource m_dataSource;
    }
}

