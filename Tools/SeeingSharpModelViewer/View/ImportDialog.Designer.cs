namespace SeeingSharpModelViewer.View
{
    partial class ImportDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportDialog));
            this.m_panButtons = new System.Windows.Forms.Panel();
            this.m_cmdOK = new System.Windows.Forms.Button();
            this.m_cmdCancel = new System.Windows.Forms.Button();
            this.m_splitter = new System.Windows.Forms.SplitContainer();
            this.m_grpProperties = new System.Windows.Forms.GroupBox();
            this.m_gridProperties = new System.Windows.Forms.PropertyGrid();
            this.m_grpPreview = new System.Windows.Forms.GroupBox();
            this.m_ctrlPreviewRenderer = new SeeingSharp.Multimedia.Views.SeeingSharpRendererControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.m_txtModelPath = new System.Windows.Forms.TextBox();
            this.m_cmdModelPath = new System.Windows.Forms.Button();
            this.m_lblPath = new System.Windows.Forms.Label();
            this.m_dlgOpenModelFile = new System.Windows.Forms.OpenFileDialog();
            this.m_panButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_splitter)).BeginInit();
            this.m_splitter.Panel1.SuspendLayout();
            this.m_splitter.Panel2.SuspendLayout();
            this.m_splitter.SuspendLayout();
            this.m_grpProperties.SuspendLayout();
            this.m_grpPreview.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_panButtons
            // 
            this.m_panButtons.Controls.Add(this.m_cmdOK);
            this.m_panButtons.Controls.Add(this.m_cmdCancel);
            this.m_panButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.m_panButtons.Location = new System.Drawing.Point(0, 305);
            this.m_panButtons.Name = "m_panButtons";
            this.m_panButtons.Size = new System.Drawing.Size(517, 48);
            this.m_panButtons.TabIndex = 1;
            // 
            // m_cmdOK
            // 
            this.m_cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_cmdOK.Location = new System.Drawing.Point(336, 13);
            this.m_cmdOK.Name = "m_cmdOK";
            this.m_cmdOK.Size = new System.Drawing.Size(75, 23);
            this.m_cmdOK.TabIndex = 1;
            this.m_cmdOK.Text = "OK";
            this.m_cmdOK.UseVisualStyleBackColor = true;
            this.m_cmdOK.Click += new System.EventHandler(this.OnCmdOK_Click);
            // 
            // m_cmdCancel
            // 
            this.m_cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_cmdCancel.Location = new System.Drawing.Point(417, 13);
            this.m_cmdCancel.Name = "m_cmdCancel";
            this.m_cmdCancel.Size = new System.Drawing.Size(88, 23);
            this.m_cmdCancel.TabIndex = 0;
            this.m_cmdCancel.Text = "Cancel";
            this.m_cmdCancel.UseVisualStyleBackColor = true;
            this.m_cmdCancel.Click += new System.EventHandler(this.OnCmdCancel_Click);
            // 
            // m_splitter
            // 
            this.m_splitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_splitter.Location = new System.Drawing.Point(0, 29);
            this.m_splitter.Name = "m_splitter";
            // 
            // m_splitter.Panel1
            // 
            this.m_splitter.Panel1.Controls.Add(this.m_grpProperties);
            this.m_splitter.Panel1MinSize = 100;
            // 
            // m_splitter.Panel2
            // 
            this.m_splitter.Panel2.Controls.Add(this.m_grpPreview);
            this.m_splitter.Panel2MinSize = 100;
            this.m_splitter.Size = new System.Drawing.Size(517, 276);
            this.m_splitter.SplitterDistance = 279;
            this.m_splitter.TabIndex = 2;
            // 
            // m_grpProperties
            // 
            this.m_grpProperties.Controls.Add(this.m_gridProperties);
            this.m_grpProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_grpProperties.Location = new System.Drawing.Point(0, 0);
            this.m_grpProperties.Name = "m_grpProperties";
            this.m_grpProperties.Size = new System.Drawing.Size(279, 276);
            this.m_grpProperties.TabIndex = 0;
            this.m_grpProperties.TabStop = false;
            this.m_grpProperties.Text = "Properties";
            // 
            // m_gridProperties
            // 
            this.m_gridProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_gridProperties.Location = new System.Drawing.Point(3, 18);
            this.m_gridProperties.Name = "m_gridProperties";
            this.m_gridProperties.Size = new System.Drawing.Size(273, 255);
            this.m_gridProperties.TabIndex = 0;
            // 
            // m_grpPreview
            // 
            this.m_grpPreview.Controls.Add(this.m_ctrlPreviewRenderer);
            this.m_grpPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_grpPreview.Location = new System.Drawing.Point(0, 0);
            this.m_grpPreview.Name = "m_grpPreview";
            this.m_grpPreview.Size = new System.Drawing.Size(234, 276);
            this.m_grpPreview.TabIndex = 0;
            this.m_grpPreview.TabStop = false;
            this.m_grpPreview.Text = "Preview";
            // 
            // m_ctrlPreviewRenderer
            // 
            this.m_ctrlPreviewRenderer.DiscardRendering = true;
            this.m_ctrlPreviewRenderer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_ctrlPreviewRenderer.Location = new System.Drawing.Point(3, 18);
            this.m_ctrlPreviewRenderer.Name = "m_ctrlPreviewRenderer";
            this.m_ctrlPreviewRenderer.Size = new System.Drawing.Size(228, 255);
            this.m_ctrlPreviewRenderer.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.m_lblPath);
            this.panel1.Controls.Add(this.m_cmdModelPath);
            this.panel1.Controls.Add(this.m_txtModelPath);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(517, 29);
            this.panel1.TabIndex = 3;
            // 
            // m_txtModelPath
            // 
            this.m_txtModelPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_txtModelPath.Location = new System.Drawing.Point(77, 3);
            this.m_txtModelPath.Name = "m_txtModelPath";
            this.m_txtModelPath.Size = new System.Drawing.Size(386, 22);
            this.m_txtModelPath.TabIndex = 0;
            this.m_txtModelPath.TextChanged += new System.EventHandler(this.OnTxtModelPath_TextChanged);
            // 
            // m_cmdModelPath
            // 
            this.m_cmdModelPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_cmdModelPath.Location = new System.Drawing.Point(469, 3);
            this.m_cmdModelPath.Name = "m_cmdModelPath";
            this.m_cmdModelPath.Size = new System.Drawing.Size(45, 23);
            this.m_cmdModelPath.TabIndex = 1;
            this.m_cmdModelPath.Text = "...";
            this.m_cmdModelPath.UseVisualStyleBackColor = true;
            this.m_cmdModelPath.Click += new System.EventHandler(this.OnCmdModelPath_Click);
            // 
            // m_lblPath
            // 
            this.m_lblPath.AutoSize = true;
            this.m_lblPath.Location = new System.Drawing.Point(3, 6);
            this.m_lblPath.Name = "m_lblPath";
            this.m_lblPath.Size = new System.Drawing.Size(41, 17);
            this.m_lblPath.TabIndex = 2;
            this.m_lblPath.Text = "Path:";
            // 
            // m_dlgOpenModelFile
            // 
            this.m_dlgOpenModelFile.RestoreDirectory = true;
            // 
            // ImportDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(517, 353);
            this.Controls.Add(this.m_splitter);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.m_panButtons);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "ImportDialog";
            this.Text = "Import Model";
            this.m_panButtons.ResumeLayout(false);
            this.m_splitter.Panel1.ResumeLayout(false);
            this.m_splitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_splitter)).EndInit();
            this.m_splitter.ResumeLayout(false);
            this.m_grpProperties.ResumeLayout(false);
            this.m_grpPreview.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel m_panButtons;
        private System.Windows.Forms.SplitContainer m_splitter;
        private System.Windows.Forms.GroupBox m_grpProperties;
        private System.Windows.Forms.GroupBox m_grpPreview;
        private System.Windows.Forms.Panel panel1;
        private SeeingSharp.Multimedia.Views.SeeingSharpRendererControl m_ctrlPreviewRenderer;
        private System.Windows.Forms.Button m_cmdOK;
        private System.Windows.Forms.Button m_cmdCancel;
        private System.Windows.Forms.PropertyGrid m_gridProperties;
        private System.Windows.Forms.Label m_lblPath;
        private System.Windows.Forms.Button m_cmdModelPath;
        private System.Windows.Forms.TextBox m_txtModelPath;
        private System.Windows.Forms.OpenFileDialog m_dlgOpenModelFile;
    }
}