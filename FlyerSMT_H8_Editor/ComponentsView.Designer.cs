namespace FlyerSMT_H8_Editor
{
    partial class ComponentsView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            this.gridComponents = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDesignator = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFootprint = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colZ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRotation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colValue = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colFeeder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFeederN1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colFeederN2 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colCycle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNozzle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridComponents)).BeginInit();
            this.SuspendLayout();
            // 
            // gridComponents
            // 
            this.gridComponents.AllowUserToAddRows = false;
            this.gridComponents.AllowUserToDeleteRows = false;
            this.gridComponents.AllowUserToResizeColumns = false;
            this.gridComponents.AllowUserToResizeRows = false;
            this.gridComponents.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridComponents.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridComponents.ColumnHeadersDefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.gridComponents.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colDesignator,
            this.colFootprint,
            this.colX,
            this.colY,
            this.colZ,
            this.colRotation,
            this.colValue,
            this.colFeeder,
            this.colFeederN1,
            this.colFeederN2,
            this.colCycle,
            this.colNozzle});
            this.gridComponents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridComponents.Location = new System.Drawing.Point(0, 0);
            this.gridComponents.Name = "gridComponents";
            this.gridComponents.RowHeadersVisible = false;
            this.gridComponents.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gridComponents.Size = new System.Drawing.Size(1000, 500);
            this.gridComponents.TabIndex = 0;
            // 
            // colNo
            // 
            this.colNo.DataPropertyName = "No";
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle15;
            this.colNo.HeaderText = "No.";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNo.Width = 30;
            // 
            // colDesignator
            // 
            this.colDesignator.DataPropertyName = "Designator";
            this.colDesignator.HeaderText = "Designator";
            this.colDesignator.MaxInputLength = 39;
            this.colDesignator.Name = "colDesignator";
            this.colDesignator.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colDesignator.Width = 64;
            // 
            // colFootprint
            // 
            this.colFootprint.DataPropertyName = "Footprint";
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colFootprint.DefaultCellStyle = dataGridViewCellStyle10;
            this.colFootprint.HeaderText = "Footprint";
            this.colFootprint.Name = "colFootprint";
            this.colFootprint.ReadOnly = true;
            this.colFootprint.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colFootprint.Width = 54;
            // 
            // colX
            // 
            this.colX.DataPropertyName = "X";
            dataGridViewCellStyle7.Format = "0.000";
            this.colX.DefaultCellStyle = dataGridViewCellStyle7;
            this.colX.HeaderText = "X";
            this.colX.Name = "colX";
            this.colX.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colX.Width = 20;
            // 
            // colY
            // 
            this.colY.DataPropertyName = "Y";
            dataGridViewCellStyle8.Format = "0.000";
            this.colY.DefaultCellStyle = dataGridViewCellStyle8;
            this.colY.HeaderText = "Y";
            this.colY.Name = "colY";
            this.colY.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colY.Width = 20;
            // 
            // colZ
            // 
            this.colZ.DataPropertyName = "Z";
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colZ.DefaultCellStyle = dataGridViewCellStyle14;
            this.colZ.HeaderText = "Z";
            this.colZ.Name = "colZ";
            this.colZ.ReadOnly = true;
            this.colZ.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colZ.Width = 20;
            // 
            // colRotation
            // 
            this.colRotation.DataPropertyName = "Rotation";
            dataGridViewCellStyle9.Format = "0.00";
            this.colRotation.DefaultCellStyle = dataGridViewCellStyle9;
            this.colRotation.HeaderText = "A";
            this.colRotation.Name = "colRotation";
            this.colRotation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colRotation.Width = 20;
            // 
            // colValue
            // 
            this.colValue.DataPropertyName = "Slot";
            this.colValue.DisplayMember = "Text";
            this.colValue.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.colValue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colValue.HeaderText = "Value";
            this.colValue.MaxDropDownItems = 20;
            this.colValue.Name = "colValue";
            this.colValue.ValueMember = "Slot";
            this.colValue.Width = 40;
            //
            // colFeeder
            //
            this.colFeeder.DataPropertyName = "Feeder";
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colFeeder.DefaultCellStyle = dataGridViewCellStyle11;
            this.colFeeder.HeaderText = "Feeder";
            this.colFeeder.Name = "colFeeder";
            this.colFeeder.ReadOnly = true;
            this.colFeeder.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colFeeder.Width = 40;
            //
            // colFeederN1
            //
            this.colFeederN1.DataPropertyName = "FeederN1";
            this.colFeederN1.HeaderText = "N1";
            this.colFeederN1.Name = "colFeederN1";
            this.colFeederN1.ReadOnly = true;
            this.colFeederN1.Width = 30;
            //
            // colFeederN2
            //
            this.colFeederN2.DataPropertyName = "FeederN2";
            this.colFeederN2.HeaderText = "N2";
            this.colFeederN2.Name = "colFeederN2";
            this.colFeederN2.ReadOnly = true;
            this.colFeederN2.Width = 30;
            //
            // colCycle
            //
            this.colCycle.DataPropertyName = "Cycle";
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colCycle.DefaultCellStyle = dataGridViewCellStyle12;
            this.colCycle.HeaderText = "Cycle";
            this.colCycle.Name = "colCycle";
            this.colCycle.ReadOnly = true;
            this.colCycle.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colCycle.Width = 40;
            //
            // colNozzle
            //
            this.colNozzle.DataPropertyName = "Nozzle";
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNozzle.DefaultCellStyle = dataGridViewCellStyle13;
            this.colNozzle.HeaderText = "Nozzle";
            this.colNozzle.Name = "colNozzle";
            this.colNozzle.ReadOnly = true;
            this.colNozzle.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNozzle.Width = 40;
            //
            // ComponentsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridComponents);
            this.Name = "ComponentsView";
            this.Size = new System.Drawing.Size(1000, 500);
            ((System.ComponentModel.ISupportInitialize)(this.gridComponents)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridComponents;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDesignator;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFootprint;
        private System.Windows.Forms.DataGridViewTextBoxColumn colX;
        private System.Windows.Forms.DataGridViewTextBoxColumn colY;
        private System.Windows.Forms.DataGridViewTextBoxColumn colZ;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRotation;
        private System.Windows.Forms.DataGridViewComboBoxColumn colValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFeeder;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colFeederN1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colFeederN2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCycle;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNozzle;
    }
}
