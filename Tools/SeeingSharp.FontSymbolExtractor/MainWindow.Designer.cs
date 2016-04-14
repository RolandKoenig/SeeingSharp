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
            this.m_cmdConvert = new System.Windows.Forms.Button();
            this.m_menu = new System.Windows.Forms.MenuStrip();
            this.m_dlgSelectTargetFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.m_txtOutputDirectory = new System.Windows.Forms.TextBox();
            this.m_lblOutputDirectory = new System.Windows.Forms.Label();
            this.m_cmdOutputDirectory = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.m_lblCharacters = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_cmbFont
            // 
            this.m_cmbFont.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_cmbFont.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_cmbFont.FormattingEnabled = true;
            this.m_cmbFont.Location = new System.Drawing.Point(165, 3);
            this.m_cmbFont.Name = "m_cmbFont";
            this.m_cmbFont.Size = new System.Drawing.Size(436, 24);
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
            this.m_txtSizeWidth.Location = new System.Drawing.Point(165, 89);
            this.m_txtSizeWidth.Name = "m_txtSizeWidth";
            this.m_txtSizeWidth.Size = new System.Drawing.Size(46, 22);
            this.m_txtSizeWidth.TabIndex = 2;
            this.m_txtSizeWidth.Text = "32";
            // 
            // m_txtSizeHeight
            // 
            this.m_txtSizeHeight.Location = new System.Drawing.Point(237, 89);
            this.m_txtSizeHeight.Name = "m_txtSizeHeight";
            this.m_txtSizeHeight.Size = new System.Drawing.Size(46, 22);
            this.m_txtSizeHeight.TabIndex = 3;
            this.m_txtSizeHeight.Text = "32";
            // 
            // m_lblSize
            // 
            this.m_lblSize.AutoSize = true;
            this.m_lblSize.Location = new System.Drawing.Point(2, 92);
            this.m_lblSize.Name = "m_lblSize";
            this.m_lblSize.Size = new System.Drawing.Size(114, 17);
            this.m_lblSize.TabIndex = 4;
            this.m_lblSize.Text = "Output Size (px):";
            // 
            // m_lblSizeX
            // 
            this.m_lblSizeX.AutoSize = true;
            this.m_lblSizeX.Location = new System.Drawing.Point(217, 92);
            this.m_lblSizeX.Name = "m_lblSizeX";
            this.m_lblSizeX.Size = new System.Drawing.Size(14, 17);
            this.m_lblSizeX.TabIndex = 5;
            this.m_lblSizeX.Text = "x";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.textBox3);
            this.panel1.Controls.Add(this.m_lblCharacters);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.m_cmdOutputDirectory);
            this.panel1.Controls.Add(this.m_lblOutputDirectory);
            this.panel1.Controls.Add(this.m_txtOutputDirectory);
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
            // m_cmdConvert
            // 
            this.m_cmdConvert.Location = new System.Drawing.Point(165, 179);
            this.m_cmdConvert.Name = "m_cmdConvert";
            this.m_cmdConvert.Size = new System.Drawing.Size(136, 29);
            this.m_cmdConvert.TabIndex = 6;
            this.m_cmdConvert.Text = "Convert!";
            this.m_cmdConvert.UseVisualStyleBackColor = true;
            this.m_cmdConvert.Click += new System.EventHandler(this.OnCmdConvert_Click);
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
            // m_txtOutputDirectory
            // 
            this.m_txtOutputDirectory.Location = new System.Drawing.Point(165, 33);
            this.m_txtOutputDirectory.Name = "m_txtOutputDirectory";
            this.m_txtOutputDirectory.Size = new System.Drawing.Size(399, 22);
            this.m_txtOutputDirectory.TabIndex = 7;
            // 
            // m_lblOutputDirectory
            // 
            this.m_lblOutputDirectory.AutoSize = true;
            this.m_lblOutputDirectory.Location = new System.Drawing.Point(2, 36);
            this.m_lblOutputDirectory.Name = "m_lblOutputDirectory";
            this.m_lblOutputDirectory.Size = new System.Drawing.Size(116, 17);
            this.m_lblOutputDirectory.TabIndex = 8;
            this.m_lblOutputDirectory.Text = "Output Directory:";
            // 
            // m_cmdOutputDirectory
            // 
            this.m_cmdOutputDirectory.Location = new System.Drawing.Point(570, 33);
            this.m_cmdOutputDirectory.Name = "m_cmdOutputDirectory";
            this.m_cmdOutputDirectory.Size = new System.Drawing.Size(31, 23);
            this.m_cmdOutputDirectory.TabIndex = 9;
            this.m_cmdOutputDirectory.Text = "...";
            this.m_cmdOutputDirectory.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(165, 61);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(76, 22);
            this.textBox1.TabIndex = 10;
            this.textBox1.Text = "E000";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(289, 61);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(76, 22);
            this.textBox2.TabIndex = 11;
            this.textBox2.Text = "E21D";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(247, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 17);
            this.label1.TabIndex = 12;
            this.label1.Text = "..to..";
            // 
            // m_lblCharacters
            // 
            this.m_lblCharacters.AutoSize = true;
            this.m_lblCharacters.Location = new System.Drawing.Point(3, 64);
            this.m_lblCharacters.Name = "m_lblCharacters";
            this.m_lblCharacters.Size = new System.Drawing.Size(123, 17);
            this.m_lblCharacters.TabIndex = 13;
            this.m_lblCharacters.Text = "Characters (HEX):";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(165, 117);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(46, 22);
            this.textBox3.TabIndex = 14;
            this.textBox3.Text = "16";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 120);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 17);
            this.label2.TabIndex = 15;
            this.label2.Text = "Font Size (pt):";
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label m_lblCharacters;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button m_cmdOutputDirectory;
        private System.Windows.Forms.Label m_lblOutputDirectory;
        private System.Windows.Forms.TextBox m_txtOutputDirectory;
    }
}

