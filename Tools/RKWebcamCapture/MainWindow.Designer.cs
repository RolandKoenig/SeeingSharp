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
            this.m_cmdCapture = new System.Windows.Forms.Button();
            this.m_picBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.m_picBox)).BeginInit();
            this.SuspendLayout();
            // 
            // m_lblDevice
            // 
            this.m_lblDevice.AutoSize = true;
            this.m_lblDevice.Location = new System.Drawing.Point(12, 9);
            this.m_lblDevice.Name = "m_lblDevice";
            this.m_lblDevice.Size = new System.Drawing.Size(55, 17);
            this.m_lblDevice.TabIndex = 0;
            this.m_lblDevice.Text = "Device:";
            // 
            // m_cboDevice
            // 
            this.m_cboDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_cboDevice.FormattingEnabled = true;
            this.m_cboDevice.Location = new System.Drawing.Point(143, 6);
            this.m_cboDevice.Name = "m_cboDevice";
            this.m_cboDevice.Size = new System.Drawing.Size(245, 24);
            this.m_cboDevice.TabIndex = 1;
            this.m_cboDevice.SelectedIndexChanged += new System.EventHandler(this.OnCboDevice_SelectedIndexChanged);
            // 
            // m_cmdCapture
            // 
            this.m_cmdCapture.Location = new System.Drawing.Point(394, 6);
            this.m_cmdCapture.Name = "m_cmdCapture";
            this.m_cmdCapture.Size = new System.Drawing.Size(75, 24);
            this.m_cmdCapture.TabIndex = 2;
            this.m_cmdCapture.Text = "Capture!";
            this.m_cmdCapture.UseVisualStyleBackColor = true;
            this.m_cmdCapture.Click += new System.EventHandler(this.OnCmdCapture_Click);
            // 
            // m_picBox
            // 
            this.m_picBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_picBox.BackColor = System.Drawing.Color.LightSteelBlue;
            this.m_picBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_picBox.Location = new System.Drawing.Point(12, 36);
            this.m_picBox.Name = "m_picBox";
            this.m_picBox.Size = new System.Drawing.Size(699, 324);
            this.m_picBox.TabIndex = 3;
            this.m_picBox.TabStop = false;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(723, 372);
            this.Controls.Add(this.m_picBox);
            this.Controls.Add(this.m_cmdCapture);
            this.Controls.Add(this.m_cboDevice);
            this.Controls.Add(this.m_lblDevice);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "RK Webcam Capture";
            ((System.ComponentModel.ISupportInitialize)(this.m_picBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lblDevice;
        private System.Windows.Forms.ComboBox m_cboDevice;
        private System.Windows.Forms.Button m_cmdCapture;
        private System.Windows.Forms.PictureBox m_picBox;
    }
}

