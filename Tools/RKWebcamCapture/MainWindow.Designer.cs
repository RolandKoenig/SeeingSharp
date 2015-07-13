namespace RKWebcamCapture
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
            this.m_lblDevice = new System.Windows.Forms.Label();
            this.m_cboDevice = new System.Windows.Forms.ComboBox();
            this.m_cmdPlay = new System.Windows.Forms.Button();
            this.m_panTop = new System.Windows.Forms.Panel();
            this.m_cmdSaveImage = new System.Windows.Forms.Button();
            this.m_cmdReadQRCode = new System.Windows.Forms.Button();
            this.m_cmdStop = new System.Windows.Forms.Button();
            this.m_panVideoArea = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.m_lblCurrentDevice = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_lblCurrentDeviceLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_dlgSaveImageFile = new System.Windows.Forms.SaveFileDialog();
            this.m_mediaPlayer = new SeeingSharp.Multimedia.Views.MediaPlayerComponent();
            this.m_panTop.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_lblDevice
            // 
            this.m_lblDevice.AutoSize = true;
            this.m_lblDevice.Location = new System.Drawing.Point(12, 16);
            this.m_lblDevice.Name = "m_lblDevice";
            this.m_lblDevice.Size = new System.Drawing.Size(55, 17);
            this.m_lblDevice.TabIndex = 0;
            this.m_lblDevice.Text = "Device:";
            // 
            // m_cboDevice
            // 
            this.m_cboDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_cboDevice.FormattingEnabled = true;
            this.m_cboDevice.Location = new System.Drawing.Point(134, 12);
            this.m_cboDevice.Name = "m_cboDevice";
            this.m_cboDevice.Size = new System.Drawing.Size(245, 24);
            this.m_cboDevice.TabIndex = 1;
            this.m_cboDevice.SelectedIndexChanged += new System.EventHandler(this.OnCboDevice_SelectedIndexChanged);
            // 
            // m_cmdPlay
            // 
            this.m_cmdPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_cmdPlay.Location = new System.Drawing.Point(806, 12);
            this.m_cmdPlay.Name = "m_cmdPlay";
            this.m_cmdPlay.Size = new System.Drawing.Size(75, 24);
            this.m_cmdPlay.TabIndex = 2;
            this.m_cmdPlay.Text = "Play";
            this.m_cmdPlay.UseVisualStyleBackColor = true;
            this.m_cmdPlay.Click += new System.EventHandler(this.OnCmdPlay_Click);
            // 
            // m_panTop
            // 
            this.m_panTop.Controls.Add(this.m_cmdSaveImage);
            this.m_panTop.Controls.Add(this.m_cmdReadQRCode);
            this.m_panTop.Controls.Add(this.m_cmdStop);
            this.m_panTop.Controls.Add(this.m_cboDevice);
            this.m_panTop.Controls.Add(this.m_lblDevice);
            this.m_panTop.Controls.Add(this.m_cmdPlay);
            this.m_panTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_panTop.Location = new System.Drawing.Point(0, 0);
            this.m_panTop.Name = "m_panTop";
            this.m_panTop.Size = new System.Drawing.Size(974, 50);
            this.m_panTop.TabIndex = 4;
            // 
            // m_cmdSaveImage
            // 
            this.m_cmdSaveImage.Location = new System.Drawing.Point(385, 12);
            this.m_cmdSaveImage.Name = "m_cmdSaveImage";
            this.m_cmdSaveImage.Size = new System.Drawing.Size(163, 24);
            this.m_cmdSaveImage.TabIndex = 5;
            this.m_cmdSaveImage.Text = "Save Image";
            this.m_cmdSaveImage.UseVisualStyleBackColor = true;
            this.m_cmdSaveImage.Click += new System.EventHandler(this.OnCmdSaveImage_Click);
            // 
            // m_cmdReadQRCode
            // 
            this.m_cmdReadQRCode.Location = new System.Drawing.Point(554, 12);
            this.m_cmdReadQRCode.Name = "m_cmdReadQRCode";
            this.m_cmdReadQRCode.Size = new System.Drawing.Size(163, 24);
            this.m_cmdReadQRCode.TabIndex = 4;
            this.m_cmdReadQRCode.Text = "Read QR-Code";
            this.m_cmdReadQRCode.UseVisualStyleBackColor = true;
            this.m_cmdReadQRCode.Click += new System.EventHandler(this.OnCmdReadQRCode_Click);
            // 
            // m_cmdStop
            // 
            this.m_cmdStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_cmdStop.Location = new System.Drawing.Point(887, 12);
            this.m_cmdStop.Name = "m_cmdStop";
            this.m_cmdStop.Size = new System.Drawing.Size(75, 24);
            this.m_cmdStop.TabIndex = 3;
            this.m_cmdStop.Text = "Stop";
            this.m_cmdStop.UseVisualStyleBackColor = true;
            this.m_cmdStop.Click += new System.EventHandler(this.OnCmdStop_Click);
            // 
            // m_panVideoArea
            // 
            this.m_panVideoArea.BackColor = System.Drawing.Color.AliceBlue;
            this.m_panVideoArea.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.m_panVideoArea.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_panVideoArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_panVideoArea.Location = new System.Drawing.Point(0, 50);
            this.m_panVideoArea.Name = "m_panVideoArea";
            this.m_panVideoArea.Size = new System.Drawing.Size(974, 427);
            this.m_panVideoArea.TabIndex = 5;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_lblCurrentDevice,
            this.m_lblCurrentDeviceLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 477);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(974, 25);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // m_lblCurrentDevice
            // 
            this.m_lblCurrentDevice.Name = "m_lblCurrentDevice";
            this.m_lblCurrentDevice.Size = new System.Drawing.Size(109, 20);
            this.m_lblCurrentDevice.Text = "Current Device:";
            // 
            // m_lblCurrentDeviceLabel
            // 
            this.m_lblCurrentDeviceLabel.Name = "m_lblCurrentDeviceLabel";
            this.m_lblCurrentDeviceLabel.Size = new System.Drawing.Size(65, 20);
            this.m_lblCurrentDeviceLabel.Text = "<None>";
            // 
            // m_dlgSaveImageFile
            // 
            this.m_dlgSaveImageFile.Filter = "Png-File (*.png)|*.png";
            this.m_dlgSaveImageFile.RestoreDirectory = true;
            // 
            // m_mediaPlayer
            // 
            this.m_mediaPlayer.AudioVolume = 0F;
            this.m_mediaPlayer.RestartWhenFinished = false;
            this.m_mediaPlayer.TargetControl = this.m_panVideoArea;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(974, 502);
            this.Controls.Add(this.m_panVideoArea);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.m_panTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "Webcam Capture";
            this.m_panTop.ResumeLayout(false);
            this.m_panTop.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lblDevice;
        private System.Windows.Forms.ComboBox m_cboDevice;
        private System.Windows.Forms.Button m_cmdPlay;
        private System.Windows.Forms.Panel m_panTop;
        private SeeingSharp.Multimedia.Views.MediaPlayerComponent m_mediaPlayer;
        private System.Windows.Forms.Panel m_panVideoArea;
        private System.Windows.Forms.Button m_cmdStop;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel m_lblCurrentDevice;
        private System.Windows.Forms.ToolStripStatusLabel m_lblCurrentDeviceLabel;
        private System.Windows.Forms.Button m_cmdReadQRCode;
        private System.Windows.Forms.Button m_cmdSaveImage;
        private System.Windows.Forms.SaveFileDialog m_dlgSaveImageFile;
    }
}

