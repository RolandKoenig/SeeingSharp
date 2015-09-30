namespace RKRocket.View
{
    partial class GameOverForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameOverForm));
            this.m_lblReachedScore = new System.Windows.Forms.Label();
            this.m_txtReachedScore = new System.Windows.Forms.TextBox();
            this.m_dataSource = new System.Windows.Forms.BindingSource(this.components);
            this.m_txtName = new System.Windows.Forms.TextBox();
            this.m_lblName = new System.Windows.Forms.Label();
            this.m_cmdOK = new System.Windows.Forms.Button();
            this.m_cmdCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataSource)).BeginInit();
            this.SuspendLayout();
            // 
            // m_lblReachedScore
            // 
            this.m_lblReachedScore.AutoSize = true;
            this.m_lblReachedScore.Location = new System.Drawing.Point(17, 15);
            this.m_lblReachedScore.Name = "m_lblReachedScore";
            this.m_lblReachedScore.Size = new System.Drawing.Size(106, 17);
            this.m_lblReachedScore.TabIndex = 0;
            this.m_lblReachedScore.Text = "Reached Score";
            // 
            // m_txtReachedScore
            // 
            this.m_txtReachedScore.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.m_dataSource, "ReachedScore", true));
            this.m_txtReachedScore.Location = new System.Drawing.Point(189, 12);
            this.m_txtReachedScore.Name = "m_txtReachedScore";
            this.m_txtReachedScore.ReadOnly = true;
            this.m_txtReachedScore.Size = new System.Drawing.Size(56, 22);
            this.m_txtReachedScore.TabIndex = 50;
            // 
            // m_dataSource
            // 
            this.m_dataSource.DataSource = typeof(RKRocket.ViewModel.GameOverViewModel);
            // 
            // m_txtName
            // 
            this.m_txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_txtName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.m_dataSource, "Name", true));
            this.m_txtName.Location = new System.Drawing.Point(189, 40);
            this.m_txtName.Name = "m_txtName";
            this.m_txtName.Size = new System.Drawing.Size(286, 22);
            this.m_txtName.TabIndex = 1;
            // 
            // m_lblName
            // 
            this.m_lblName.AutoSize = true;
            this.m_lblName.Location = new System.Drawing.Point(17, 43);
            this.m_lblName.Name = "m_lblName";
            this.m_lblName.Size = new System.Drawing.Size(50, 17);
            this.m_lblName.TabIndex = 3;
            this.m_lblName.Text = "Name*";
            // 
            // m_cmdOK
            // 
            this.m_cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_cmdOK.Location = new System.Drawing.Point(247, 90);
            this.m_cmdOK.Name = "m_cmdOK";
            this.m_cmdOK.Size = new System.Drawing.Size(111, 23);
            this.m_cmdOK.TabIndex = 2;
            this.m_cmdOK.Text = "OK";
            this.m_cmdOK.UseVisualStyleBackColor = true;
            this.m_cmdOK.Click += new System.EventHandler(this.OnCmdOK_Click);
            // 
            // m_cmdCancel
            // 
            this.m_cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_cmdCancel.Location = new System.Drawing.Point(364, 90);
            this.m_cmdCancel.Name = "m_cmdCancel";
            this.m_cmdCancel.Size = new System.Drawing.Size(111, 23);
            this.m_cmdCancel.TabIndex = 3;
            this.m_cmdCancel.Text = "Cancel";
            this.m_cmdCancel.UseVisualStyleBackColor = true;
            this.m_cmdCancel.Click += new System.EventHandler(this.OnCmdCancel_Click);
            // 
            // GameOverForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 125);
            this.Controls.Add(this.m_cmdCancel);
            this.Controls.Add(this.m_cmdOK);
            this.Controls.Add(this.m_lblName);
            this.Controls.Add(this.m_txtName);
            this.Controls.Add(this.m_txtReachedScore);
            this.Controls.Add(this.m_lblReachedScore);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GameOverForm";
            this.Text = "Gameover!";
            ((System.ComponentModel.ISupportInitialize)(this.m_dataSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lblReachedScore;
        private System.Windows.Forms.TextBox m_txtReachedScore;
        private System.Windows.Forms.TextBox m_txtName;
        private System.Windows.Forms.Label m_lblName;
        private System.Windows.Forms.Button m_cmdOK;
        private System.Windows.Forms.Button m_cmdCancel;
        private System.Windows.Forms.BindingSource m_dataSource;
    }
}