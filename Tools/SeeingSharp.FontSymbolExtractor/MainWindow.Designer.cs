namespace SeeingSharp.FontSymbolExtractor
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
            this.m_cmbFont = new System.Windows.Forms.ComboBox();
            this.m_lblFont = new System.Windows.Forms.Label();
            this.m_txtSizeWidth = new System.Windows.Forms.TextBox();
            this.m_txtSizeHeight = new System.Windows.Forms.TextBox();
            this.m_lblSize = new System.Windows.Forms.Label();
            this.m_lblSizeX = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.m_menu = new System.Windows.Forms.MenuStrip();
            this.m_cmdConvert = new System.Windows.Forms.Button();
            this.m_dlgSelectTargetFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_cmbFont
            // 
            this.m_cmbFont.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_cmbFont.FormattingEnabled = true;
            this.m_cmbFont.Location = new System.Drawing.Point(129, 3);
            this.m_cmbFont.Name = "m_cmbFont";
            this.m_cmbFont.Size = new System.Drawing.Size(191, 24);
            this.m_cmbFont.TabIndex = 0;
            // 
            // m_lblFont
            // 
            this.m_lblFont.AutoSize = true;
            this.m_lblFont.Location = new System.Drawing.Point(2, 6);
            this.m_lblFont.Name = "m_lblFont";
            this.m_lblFont.Size = new System.Drawing.Size(40, 17);
            this.m_lblFont.TabIndex = 1;
            this.m_lblFont.Text = "Font:";
            // 
            // m_txtSizeWidth
            // 
            this.m_txtSizeWidth.Location = new System.Drawing.Point(129, 33);
            this.m_txtSizeWidth.Name = "m_txtSizeWidth";
            this.m_txtSizeWidth.Size = new System.Drawing.Size(46, 22);
            this.m_txtSizeWidth.TabIndex = 2;
            // 
            // m_txtSizeHeight
            // 
            this.m_txtSizeHeight.Location = new System.Drawing.Point(201, 33);
            this.m_txtSizeHeight.Name = "m_txtSizeHeight";
            this.m_txtSizeHeight.Size = new System.Drawing.Size(46, 22);
            this.m_txtSizeHeight.TabIndex = 3;
            // 
            // m_lblSize
            // 
            this.m_lblSize.AutoSize = true;
            this.m_lblSize.Location = new System.Drawing.Point(2, 36);
            this.m_lblSize.Name = "m_lblSize";
            this.m_lblSize.Size = new System.Drawing.Size(72, 17);
            this.m_lblSize.TabIndex = 4;
            this.m_lblSize.Text = "Pixel Size:";
            // 
            // m_lblSizeX
            // 
            this.m_lblSizeX.AutoSize = true;
            this.m_lblSizeX.Location = new System.Drawing.Point(181, 36);
            this.m_lblSizeX.Name = "m_lblSizeX";
            this.m_lblSizeX.Size = new System.Drawing.Size(14, 17);
            this.m_lblSizeX.TabIndex = 5;
            this.m_lblSizeX.Text = "x";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.m_cmdConvert);
            this.panel1.Controls.Add(this.m_lblFont);
            this.panel1.Controls.Add(this.m_lblSizeX);
            this.panel1.Controls.Add(this.m_cmbFont);
            this.panel1.Controls.Add(this.m_lblSize);
            this.panel1.Controls.Add(this.m_txtSizeWidth);
            this.panel1.Controls.Add(this.m_txtSizeHeight);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(604, 304);
            this.panel1.TabIndex = 6;
            // 
            // m_menu
            // 
            this.m_menu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.m_menu.Location = new System.Drawing.Point(0, 0);
            this.m_menu.Name = "m_menu";
            this.m_menu.Size = new System.Drawing.Size(604, 24);
            this.m_menu.TabIndex = 7;
            this.m_menu.Text = "menuStrip1";
            // 
            // m_cmdConvert
            // 
            this.m_cmdConvert.Location = new System.Drawing.Point(129, 86);
            this.m_cmdConvert.Name = "m_cmdConvert";
            this.m_cmdConvert.Size = new System.Drawing.Size(136, 29);
            this.m_cmdConvert.TabIndex = 6;
            this.m_cmdConvert.Text = "Convert!";
            this.m_cmdConvert.UseVisualStyleBackColor = true;
            this.m_cmdConvert.Click += new System.EventHandler(this.OnCmdConvert_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 328);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.m_menu);
            this.MainMenuStrip = this.m_menu;
            this.Name = "MainWindow";
            this.Text = "Seeing# - Font Symbol Extractor";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox m_cmbFont;
        private System.Windows.Forms.Label m_lblFont;
        private System.Windows.Forms.TextBox m_txtSizeWidth;
        private System.Windows.Forms.TextBox m_txtSizeHeight;
        private System.Windows.Forms.Label m_lblSize;
        private System.Windows.Forms.Label m_lblSizeX;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button m_cmdConvert;
        private System.Windows.Forms.MenuStrip m_menu;
        private System.Windows.Forms.FolderBrowserDialog m_dlgSelectTargetFolder;
    }
}

