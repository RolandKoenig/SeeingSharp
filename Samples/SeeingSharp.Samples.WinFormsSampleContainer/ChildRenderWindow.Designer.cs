namespace WinFormsSampleContainer
{
    partial class ChildRenderWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChildRenderWindow));
            this.m_ctrlRenderer = new SeeingSharp.Multimedia.Views.SeeingSharpRendererControl();
            this.m_toolBar = new System.Windows.Forms.ToolStrip();
            this.m_cboDevice = new System.Windows.Forms.ToolStripDropDownButton();
            this.m_toolBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_ctrlRenderer
            // 
            this.m_ctrlRenderer.DiscardRendering = true;
            this.m_ctrlRenderer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_ctrlRenderer.InputMode = SeeingSharp.Multimedia.Input.SeeingSharpInputMode.FreeCameraMovement;
            this.m_ctrlRenderer.Location = new System.Drawing.Point(0, 27);
            this.m_ctrlRenderer.Name = "m_ctrlRenderer";
            this.m_ctrlRenderer.Size = new System.Drawing.Size(506, 365);
            this.m_ctrlRenderer.TabIndex = 0;
            this.m_ctrlRenderer.Text = "m_ctrlRenderer";
            this.m_ctrlRenderer.ViewConfiguration.AccentuationFactor = 0F;
            this.m_ctrlRenderer.ViewConfiguration.AmbientFactor = 0.2F;
            this.m_ctrlRenderer.ViewConfiguration.AntialiasingEnabled = true;
            this.m_ctrlRenderer.ViewConfiguration.AntialiasingQuality = SeeingSharp.Multimedia.Core.AntialiasingQualityLevel.Medium;
            this.m_ctrlRenderer.ViewConfiguration.GeneratedBorderFactor = 1F;
            this.m_ctrlRenderer.ViewConfiguration.GeneratedColorGradientFactor = 1F;
            this.m_ctrlRenderer.ViewConfiguration.LightPower = 0.8F;
            this.m_ctrlRenderer.ViewConfiguration.ShowTextures = true;
            this.m_ctrlRenderer.ViewConfiguration.StrongLightFactor = 1.5F;
            this.m_ctrlRenderer.ViewConfiguration.WireframeEnabled = false;
            // 
            // m_toolBar
            // 
            this.m_toolBar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.m_toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_cboDevice});
            this.m_toolBar.Location = new System.Drawing.Point(0, 0);
            this.m_toolBar.Name = "m_toolBar";
            this.m_toolBar.Size = new System.Drawing.Size(506, 27);
            this.m_toolBar.TabIndex = 1;
            // 
            // m_cboDevice
            // 
            this.m_cboDevice.Image = global::WinFormsSampleContainer.Properties.Resources.Adapter16x16;
            this.m_cboDevice.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_cboDevice.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_cboDevice.Name = "m_cboDevice";
            this.m_cboDevice.Size = new System.Drawing.Size(108, 24);
            this.m_cboDevice.Text = "<Device>";
            this.m_cboDevice.ToolTipText = "Change current rendering device";
            // 
            // ChildRenderWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 392);
            this.Controls.Add(this.m_ctrlRenderer);
            this.Controls.Add(this.m_toolBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ChildRenderWindow";
            this.Text = "Child Render Window";
            this.m_toolBar.ResumeLayout(false);
            this.m_toolBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SeeingSharp.Multimedia.Views.SeeingSharpRendererControl m_ctrlRenderer;
        private System.Windows.Forms.ToolStrip m_toolBar;
        private System.Windows.Forms.ToolStripDropDownButton m_cboDevice;
    }
}