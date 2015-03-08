namespace FrozenSky.Samples.WinFormsSampleContainer.Views
{
    partial class PerformanceMeasureForm
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
            this.m_dataGrid = new System.Windows.Forms.DataGridView();
            this.m_refreshTimer = new System.Windows.Forms.Timer(this.components);
            this.m_barTools = new System.Windows.Forms.ToolStrip();
            this.m_cmdCopy = new System.Windows.Forms.ToolStripButton();
            this.m_dataSource = new System.Windows.Forms.BindingSource(this.components);
            this.m_colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m_colDuration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGrid)).BeginInit();
            this.m_barTools.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataSource)).BeginInit();
            this.SuspendLayout();
            // 
            // m_dataGrid
            // 
            this.m_dataGrid.AllowUserToAddRows = false;
            this.m_dataGrid.AllowUserToDeleteRows = false;
            this.m_dataGrid.AllowUserToResizeRows = false;
            this.m_dataGrid.AutoGenerateColumns = false;
            this.m_dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_dataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.m_colName,
            this.m_colDuration});
            this.m_dataGrid.DataSource = this.m_dataSource;
            this.m_dataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_dataGrid.Location = new System.Drawing.Point(0, 25);
            this.m_dataGrid.MultiSelect = false;
            this.m_dataGrid.Name = "m_dataGrid";
            this.m_dataGrid.ReadOnly = true;
            this.m_dataGrid.RowTemplate.Height = 24;
            this.m_dataGrid.Size = new System.Drawing.Size(668, 416);
            this.m_dataGrid.TabIndex = 0;
            // 
            // m_refreshTimer
            // 
            this.m_refreshTimer.Enabled = true;
            this.m_refreshTimer.Interval = 1000;
            this.m_refreshTimer.Tick += new System.EventHandler(this.OnRefreshTimerTick);
            // 
            // m_barTools
            // 
            this.m_barTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_cmdCopy});
            this.m_barTools.Location = new System.Drawing.Point(0, 0);
            this.m_barTools.Name = "m_barTools";
            this.m_barTools.Size = new System.Drawing.Size(668, 25);
            this.m_barTools.TabIndex = 1;
            this.m_barTools.Text = "toolStrip1";
            // 
            // m_cmdCopy
            // 
            this.m_cmdCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_cmdCopy.Image = global::FrozenSky.Samples.WinFormsSampleContainer.Properties.Resources.Copy16x16;
            this.m_cmdCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_cmdCopy.Name = "m_cmdCopy";
            this.m_cmdCopy.Size = new System.Drawing.Size(23, 22);
            this.m_cmdCopy.Text = "Copy";
            this.m_cmdCopy.Click += new System.EventHandler(this.OnCmdCopyClick);
            // 
            // m_dataSource
            // 
            this.m_dataSource.DataSource = typeof(FrozenSky.Util.DurationPerformanceResult);
            // 
            // m_colName
            // 
            this.m_colName.DataPropertyName = "CalculatorName";
            this.m_colName.HeaderText = "Name";
            this.m_colName.Name = "m_colName";
            this.m_colName.ReadOnly = true;
            this.m_colName.Width = 300;
            // 
            // m_colDuration
            // 
            this.m_colDuration.DataPropertyName = "SumAverageMSDouble";
            this.m_colDuration.HeaderText = "Duration";
            this.m_colDuration.Name = "m_colDuration";
            this.m_colDuration.ReadOnly = true;
            // 
            // PerformanceMeasureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 441);
            this.Controls.Add(this.m_dataGrid);
            this.Controls.Add(this.m_barTools);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PerformanceMeasureForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Performance";
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGrid)).EndInit();
            this.m_barTools.ResumeLayout(false);
            this.m_barTools.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView m_dataGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sumAverageMSDataGridViewTextBoxColumn;
        private System.Windows.Forms.Timer m_refreshTimer;
        private System.Windows.Forms.ToolStrip m_barTools;
        private System.Windows.Forms.ToolStripButton m_cmdCopy;
        private System.Windows.Forms.BindingSource m_dataSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn m_colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn m_colDuration;
    }
}