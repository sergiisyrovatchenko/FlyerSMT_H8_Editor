namespace FlyerSMT_H8_Editor
{
    partial class PcbView
    {
        // Components created by the designer
        private System.ComponentModel.IContainer components = null;

        // Frees resources; disposing = true also frees managed ones
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        // Written by the designer: change it there, not by hand
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.gridPcbs = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridPcbs)).BeginInit();
            this.SuspendLayout();
            //
            // gridPcbs
            //
            this.gridPcbs.AllowUserToAddRows = false;
            this.gridPcbs.AllowUserToDeleteRows = false;
            this.gridPcbs.AllowUserToResizeColumns = false;
            this.gridPcbs.AllowUserToResizeRows = false;
            this.gridPcbs.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridPcbs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridPcbs.ColumnHeadersDefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.gridPcbs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colX,
            this.colY,
            this.colA,
            this.colOn});
            this.gridPcbs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPcbs.Location = new System.Drawing.Point(0, 0);
            this.gridPcbs.Name = "gridPcbs";
            this.gridPcbs.RowHeadersVisible = false;
            this.gridPcbs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gridPcbs.Size = new System.Drawing.Size(1000, 500);
            this.gridPcbs.TabIndex = 0;
            //
            // colNo
            //
            this.colNo.DataPropertyName = "No";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle4;
            this.colNo.HeaderText = "No.";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNo.Width = 30;
            //
            // colX
            //
            this.colX.DataPropertyName = "X";
            dataGridViewCellStyle1.Format = "0.000";
            this.colX.DefaultCellStyle = dataGridViewCellStyle1;
            this.colX.HeaderText = "X";
            this.colX.Name = "colX";
            this.colX.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colX.Width = 60;
            //
            // colY
            //
            this.colY.DataPropertyName = "Y";
            dataGridViewCellStyle2.Format = "0.000";
            this.colY.DefaultCellStyle = dataGridViewCellStyle2;
            this.colY.HeaderText = "Y";
            this.colY.Name = "colY";
            this.colY.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colY.Width = 60;
            //
            // colA
            //
            this.colA.DataPropertyName = "A";
            dataGridViewCellStyle3.Format = "0.000";
            this.colA.DefaultCellStyle = dataGridViewCellStyle3;
            this.colA.HeaderText = "A";
            this.colA.Name = "colA";
            this.colA.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colA.Width = 60;
            //
            // colOn
            //
            this.colOn.DataPropertyName = "On";
            this.colOn.HeaderText = "ON";
            this.colOn.Name = "colOn";
            this.colOn.Width = 30;
            //
            // PcbView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridPcbs);
            this.Name = "PcbView";
            this.Size = new System.Drawing.Size(1000, 500);
            ((System.ComponentModel.ISupportInitialize)(this.gridPcbs)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridPcbs;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colX;
        private System.Windows.Forms.DataGridViewTextBoxColumn colY;
        private System.Windows.Forms.DataGridViewTextBoxColumn colA;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colOn;
    }
}
