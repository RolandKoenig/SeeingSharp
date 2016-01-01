namespace SeeingSharp.Multimedia.UI
{
    partial class LoadExternalModelFormsDlg
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
            this.m_txtFilePath = new System.Windows.Forms.TextBox();
            this.m_lblFilePath = new System.Windows.Forms.Label();
            this.m_cmdFilePath = new System.Windows.Forms.Button();
            this.m_dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.m_cmdCancel = new System.Windows.Forms.Button();
            this.m_cmdOK = new System.Windows.Forms.Button();
            this.m_panBottomBar = new System.Windows.Forms.Panel();
            this.m_panTopBar = new System.Windows.Forms.Panel();
            this.m_splitter = new System.Windows.Forms.SplitContainer();
            this.m_ctrlRenderer = new SeeingSharp.Multimedia.Views.SeeingSharpRendererControl();
            this.m_splitterPropertiesAndTree = new System.Windows.Forms.SplitContainer();
            this.m_gridProperties = new System.Windows.Forms.PropertyGrid();
            this.m_treeScene = new System.Windows.Forms.TreeView();
            this.m_panLoading = new System.Windows.Forms.Panel();
            this.m_progressLoading = new System.Windows.Forms.ProgressBar();
            this.m_lblLoading = new System.Windows.Forms.Label();
            this.m_cmdProgressCancel = new System.Windows.Forms.Button();
            this.m_dataSource = new System.Windows.Forms.BindingSource(this.components);
            this.m_panBottomBar.SuspendLayout();
            this.m_panTopBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_splitter)).BeginInit();
            this.m_splitter.Panel1.SuspendLayout();
            this.m_splitter.Panel2.SuspendLayout();
            this.m_splitter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_splitterPropertiesAndTree)).BeginInit();
            this.m_splitterPropertiesAndTree.Panel1.SuspendLayout();
            this.m_splitterPropertiesAndTree.Panel2.SuspendLayout();
            this.m_splitterPropertiesAndTree.SuspendLayout();
            this.m_panLoading.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataSource)).BeginInit();
            this.SuspendLayout();
            // 
            // m_txtFilePath
            // 
            this.m_txtFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_txtFilePath.Location = new System.Drawing.Point(135, 13);
            this.m_txtFilePath.Name = "m_txtFilePath";
            this.m_txtFilePath.Size = new System.Drawing.Size(565, 22);
            this.m_txtFilePath.TabIndex = 0;
            // 
            // m_lblFilePath
            // 
            this.m_lblFilePath.AutoSize = true;
            this.m_lblFilePath.Location = new System.Drawing.Point(12, 16);
            this.m_lblFilePath.Name = "m_lblFilePath";
            this.m_lblFilePath.Size = new System.Drawing.Size(66, 17);
            this.m_lblFilePath.TabIndex = 1;
            this.m_lblFilePath.Text = "File path:";
            // 
            // m_cmdFilePath
            // 
            this.m_cmdFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_cmdFilePath.Location = new System.Drawing.Point(706, 13);
            this.m_cmdFilePath.Name = "m_cmdFilePath";
            this.m_cmdFilePath.Size = new System.Drawing.Size(29, 23);
            this.m_cmdFilePath.TabIndex = 2;
            this.m_cmdFilePath.Text = "...";
            this.m_cmdFilePath.UseVisualStyleBackColor = true;
            this.m_cmdFilePath.Click += new System.EventHandler(this.OnCmdFilePath_Click);
            // 
            // m_cmdCancel
            // 
            this.m_cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_cmdCancel.Location = new System.Drawing.Point(630, 11);
            this.m_cmdCancel.Name = "m_cmdCancel";
            this.m_cmdCancel.Size = new System.Drawing.Size(105, 23);
            this.m_cmdCancel.TabIndex = 3;
            this.m_cmdCancel.Text = "Cancel";
            this.m_cmdCancel.UseVisualStyleBackColor = true;
            this.m_cmdCancel.Click += new System.EventHandler(this.OnCmdCancel_Click);
            // 
            // m_cmdOK
            // 
            this.m_cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_cmdOK.Location = new System.Drawing.Point(519, 11);
            this.m_cmdOK.Name = "m_cmdOK";
            this.m_cmdOK.Size = new System.Drawing.Size(105, 23);
            this.m_cmdOK.TabIndex = 4;
            this.m_cmdOK.Text = "OK";
            this.m_cmdOK.UseVisualStyleBackColor = true;
            this.m_cmdOK.Click += new System.EventHandler(this.OnCmdOK_Click);
            // 
            // m_panBottomBar
            // 
            this.m_panBottomBar.Controls.Add(this.m_cmdCancel);
            this.m_panBottomBar.Controls.Add(this.m_cmdOK);
            this.m_panBottomBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.m_panBottomBar.Location = new System.Drawing.Point(0, 447);
            this.m_panBottomBar.Name = "m_panBottomBar";
            this.m_panBottomBar.Size = new System.Drawing.Size(747, 46);
            this.m_panBottomBar.TabIndex = 5;
            // 
            // m_panTopBar
            // 
            this.m_panTopBar.Controls.Add(this.m_txtFilePath);
            this.m_panTopBar.Controls.Add(this.m_lblFilePath);
            this.m_panTopBar.Controls.Add(this.m_cmdFilePath);
            this.m_panTopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_panTopBar.Location = new System.Drawing.Point(0, 0);
            this.m_panTopBar.Name = "m_panTopBar";
            this.m_panTopBar.Size = new System.Drawing.Size(747, 46);
            this.m_panTopBar.TabIndex = 6;
            // 
            // m_splitter
            // 
            this.m_splitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_splitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.m_splitter.Location = new System.Drawing.Point(0, 46);
            this.m_splitter.Name = "m_splitter";
            // 
            // m_splitter.Panel1
            // 
            this.m_splitter.Panel1.Controls.Add(this.m_splitterPropertiesAndTree);
            // 
            // m_splitter.Panel2
            // 
            this.m_splitter.Panel2.Controls.Add(this.m_ctrlRenderer);
            this.m_splitter.Size = new System.Drawing.Size(747, 372);
            this.m_splitter.SplitterDistance = 228;
            this.m_splitter.TabIndex = 7;
            // 
            // m_ctrlRenderer
            // 
            this.m_ctrlRenderer.DiscardRendering = true;
            this.m_ctrlRenderer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_ctrlRenderer.InputMode = SeeingSharp.Multimedia.Input.SeeingSharpInputMode.FreeCameraMovement;
            this.m_ctrlRenderer.Location = new System.Drawing.Point(0, 0);
            this.m_ctrlRenderer.Name = "m_ctrlRenderer";
            this.m_ctrlRenderer.Size = new System.Drawing.Size(515, 372);
            this.m_ctrlRenderer.TabIndex = 0;
            this.m_ctrlRenderer.ViewConfiguration.AccentuationFactor = 0F;
            this.m_ctrlRenderer.ViewConfiguration.AlphaEnabledSwapChain = false;
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
            // m_splitterPropertiesAndTree
            // 
            this.m_splitterPropertiesAndTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_splitterPropertiesAndTree.Location = new System.Drawing.Point(0, 0);
            this.m_splitterPropertiesAndTree.Name = "m_splitterPropertiesAndTree";
            this.m_splitterPropertiesAndTree.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // m_splitterPropertiesAndTree.Panel1
            // 
            this.m_splitterPropertiesAndTree.Panel1.Controls.Add(this.m_treeScene);
            // 
            // m_splitterPropertiesAndTree.Panel2
            // 
            this.m_splitterPropertiesAndTree.Panel2.Controls.Add(this.m_gridProperties);
            this.m_splitterPropertiesAndTree.Size = new System.Drawing.Size(228, 372);
            this.m_splitterPropertiesAndTree.SplitterDistance = 189;
            this.m_splitterPropertiesAndTree.TabIndex = 0;
            // 
            // m_gridProperties
            // 
            this.m_gridProperties.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.m_gridProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_gridProperties.Location = new System.Drawing.Point(0, 0);
            this.m_gridProperties.Name = "m_gridProperties";
            this.m_gridProperties.Size = new System.Drawing.Size(228, 179);
            this.m_gridProperties.TabIndex = 0;
            this.m_gridProperties.ToolbarVisible = false;
            // 
            // m_treeScene
            // 
            this.m_treeScene.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_treeScene.Location = new System.Drawing.Point(0, 0);
            this.m_treeScene.Name = "m_treeScene";
            this.m_treeScene.Size = new System.Drawing.Size(228, 189);
            this.m_treeScene.TabIndex = 0;
            // 
            // m_panLoading
            // 
            this.m_panLoading.Controls.Add(this.m_cmdProgressCancel);
            this.m_panLoading.Controls.Add(this.m_lblLoading);
            this.m_panLoading.Controls.Add(this.m_progressLoading);
            this.m_panLoading.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.m_panLoading.Location = new System.Drawing.Point(0, 418);
            this.m_panLoading.Name = "m_panLoading";
            this.m_panLoading.Size = new System.Drawing.Size(747, 29);
            this.m_panLoading.TabIndex = 8;
            this.m_panLoading.Visible = false;
            // 
            // m_progressLoading
            // 
            this.m_progressLoading.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_progressLoading.Location = new System.Drawing.Point(175, 3);
            this.m_progressLoading.Name = "m_progressLoading";
            this.m_progressLoading.Size = new System.Drawing.Size(525, 23);
            this.m_progressLoading.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.m_progressLoading.TabIndex = 0;
            // 
            // m_lblLoading
            // 
            this.m_lblLoading.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_lblLoading.AutoSize = true;
            this.m_lblLoading.Location = new System.Drawing.Point(12, 6);
            this.m_lblLoading.Name = "m_lblLoading";
            this.m_lblLoading.Size = new System.Drawing.Size(71, 17);
            this.m_lblLoading.TabIndex = 1;
            this.m_lblLoading.Text = "Loading...";
            // 
            // m_cmdProgressCancel
            // 
            this.m_cmdProgressCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_cmdProgressCancel.Image = global::SeeingSharp.Multimedia.Properties.Resources.Cancel_16x16;
            this.m_cmdProgressCancel.Location = new System.Drawing.Point(706, 3);
            this.m_cmdProgressCancel.Name = "m_cmdProgressCancel";
            this.m_cmdProgressCancel.Size = new System.Drawing.Size(29, 23);
            this.m_cmdProgressCancel.TabIndex = 5;
            this.m_cmdProgressCancel.UseVisualStyleBackColor = true;
            // 
            // m_dataSource
            // 
            this.m_dataSource.DataSource = typeof(SeeingSharp.Multimedia.UI.LoadExternalModelVM);
            // 
            // LoadExternalModelFormsDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(747, 493);
            this.Controls.Add(this.m_splitter);
            this.Controls.Add(this.m_panTopBar);
            this.Controls.Add(this.m_panLoading);
            this.Controls.Add(this.m_panBottomBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "LoadExternalModelFormsDlg";
            this.Text = "Import 3D model";
            this.m_panBottomBar.ResumeLayout(false);
            this.m_panTopBar.ResumeLayout(false);
            this.m_panTopBar.PerformLayout();
            this.m_splitter.Panel1.ResumeLayout(false);
            this.m_splitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_splitter)).EndInit();
            this.m_splitter.ResumeLayout(false);
            this.m_splitterPropertiesAndTree.Panel1.ResumeLayout(false);
            this.m_splitterPropertiesAndTree.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_splitterPropertiesAndTree)).EndInit();
            this.m_splitterPropertiesAndTree.ResumeLayout(false);
            this.m_panLoading.ResumeLayout(false);
            this.m_panLoading.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox m_txtFilePath;
        private System.Windows.Forms.Label m_lblFilePath;
        private System.Windows.Forms.Button m_cmdFilePath;
        private System.Windows.Forms.BindingSource m_dataSource;
        private System.Windows.Forms.OpenFileDialog m_dlgOpenFile;
        private System.Windows.Forms.Button m_cmdCancel;
        private System.Windows.Forms.Button m_cmdOK;
        private System.Windows.Forms.Panel m_panBottomBar;
        private System.Windows.Forms.Panel m_panTopBar;
        private System.Windows.Forms.SplitContainer m_splitter;
        private System.Windows.Forms.SplitContainer m_splitterPropertiesAndTree;
        private System.Windows.Forms.TreeView m_treeScene;
        private System.Windows.Forms.PropertyGrid m_gridProperties;
        private Views.SeeingSharpRendererControl m_ctrlRenderer;
        private System.Windows.Forms.Panel m_panLoading;
        private System.Windows.Forms.Button m_cmdProgressCancel;
        private System.Windows.Forms.Label m_lblLoading;
        private System.Windows.Forms.ProgressBar m_progressLoading;
    }
}