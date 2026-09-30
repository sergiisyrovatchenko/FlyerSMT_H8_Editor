namespace FlyerSMT_H8_Editor
{
    partial class FeedersView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            this.gridFeeders = new System.Windows.Forms.DataGridView();
            this.colNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPackage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHoleX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHoleY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAngle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNozzleHeight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colThickness = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDistance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVision = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colVisionType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colThreshold = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNozzle1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colNozzle2 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colTakeDown = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTakeUp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPasteDown = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPasteUp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUsedBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOtherUsedBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridFeeders)).BeginInit();
            this.SuspendLayout();
            // 
            // gridFeeders
            // 
            this.gridFeeders.AllowUserToAddRows = false;
            this.gridFeeders.AllowUserToDeleteRows = false;
            this.gridFeeders.AllowUserToResizeColumns = false;
            this.gridFeeders.AllowUserToResizeRows = false;
            this.gridFeeders.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridFeeders.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridFeeders.ColumnHeadersDefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.gridFeeders.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNumber,
            this.colEnabled,
            this.colValue,
            this.colPackage,
            this.colX,
            this.colY,
            this.colHoleX,
            this.colHoleY,
            this.colAngle,
            this.colNozzleHeight,
            this.colThickness,
            this.colDistance,
            this.colSize,
            this.colVision,
            this.colVisionType,
            this.colThreshold,
            this.colNozzle1,
            this.colNozzle2,
            this.colTakeDown,
            this.colTakeUp,
            this.colPasteDown,
            this.colPasteUp,
            this.colUsedBy,
            this.colOtherUsedBy});
            this.gridFeeders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridFeeders.Location = new System.Drawing.Point(0, 0);
            this.gridFeeders.Name = "gridFeeders";
            this.gridFeeders.RowHeadersVisible = false;
            this.gridFeeders.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gridFeeders.Size = new System.Drawing.Size(1000, 500);
            this.gridFeeders.TabIndex = 0;
            // 
            // colNumber
            // 
            this.colNumber.DataPropertyName = "Number";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNumber.DefaultCellStyle = dataGridViewCellStyle9;
            this.colNumber.HeaderText = "No.";
            this.colNumber.Name = "colNumber";
            this.colNumber.ReadOnly = true;
            this.colNumber.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNumber.Width = 30;
            // 
            // colEnabled
            // 
            this.colEnabled.DataPropertyName = "Enabled";
            this.colEnabled.HeaderText = "ON";
            this.colEnabled.Name = "colEnabled";
            this.colEnabled.Width = 46;
            // 
            // colValue
            // 
            this.colValue.DataPropertyName = "Value";
            this.colValue.HeaderText = "Value";
            this.colValue.MaxInputLength = 39;
            this.colValue.Name = "colValue";
            this.colValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colValue.Width = 40;
            // 
            // colPackage
            // 
            this.colPackage.DataPropertyName = "Package";
            this.colPackage.HeaderText = "Package";
            this.colPackage.MaxInputLength = 39;
            this.colPackage.Name = "colPackage";
            this.colPackage.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colPackage.Width = 56;
            // 
            // colX
            // 
            this.colX.DataPropertyName = "X";
            dataGridViewCellStyle1.Format = "0.000";
            this.colX.DefaultCellStyle = dataGridViewCellStyle1;
            this.colX.HeaderText = "Coord X";
            this.colX.Name = "colX";
            this.colX.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colX.Width = 51;
            // 
            // colY
            // 
            this.colY.DataPropertyName = "Y";
            dataGridViewCellStyle2.Format = "0.000";
            this.colY.DefaultCellStyle = dataGridViewCellStyle2;
            this.colY.HeaderText = "Coord Y";
            this.colY.Name = "colY";
            this.colY.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colY.Width = 51;
            // 
            // colHoleX
            // 
            this.colHoleX.DataPropertyName = "HoleX";
            dataGridViewCellStyle3.Format = "0.000";
            this.colHoleX.DefaultCellStyle = dataGridViewCellStyle3;
            this.colHoleX.HeaderText = "Hole X";
            this.colHoleX.Name = "colHoleX";
            this.colHoleX.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colHoleX.Width = 45;
            // 
            // colHoleY
            // 
            this.colHoleY.DataPropertyName = "HoleY";
            dataGridViewCellStyle4.Format = "0.000";
            this.colHoleY.DefaultCellStyle = dataGridViewCellStyle4;
            this.colHoleY.HeaderText = "Hole Y";
            this.colHoleY.Name = "colHoleY";
            this.colHoleY.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colHoleY.Width = 45;
            // 
            // colAngle
            // 
            this.colAngle.DataPropertyName = "Angle";
            dataGridViewCellStyle5.Format = "0.00";
            this.colAngle.DefaultCellStyle = dataGridViewCellStyle5;
            this.colAngle.HeaderText = "Angle";
            this.colAngle.Name = "colAngle";
            this.colAngle.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colAngle.Width = 40;
            // 
            // colNozzleHeight
            // 
            this.colNozzleHeight.DataPropertyName = "NozzleHeight";
            dataGridViewCellStyle6.Format = "0.00";
            this.colNozzleHeight.DefaultCellStyle = dataGridViewCellStyle6;
            this.colNozzleHeight.HeaderText = "Nozzle H";
            this.colNozzleHeight.Name = "colNozzleHeight";
            this.colNozzleHeight.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNozzleHeight.Width = 56;
            // 
            // colThickness
            // 
            this.colThickness.DataPropertyName = "Thickness";
            dataGridViewCellStyle7.Format = "0.0";
            this.colThickness.DefaultCellStyle = dataGridViewCellStyle7;
            this.colThickness.HeaderText = "Thickness";
            this.colThickness.Name = "colThickness";
            this.colThickness.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colThickness.Width = 62;
            // 
            // colDistance
            // 
            this.colDistance.DataPropertyName = "Distance";
            dataGridViewCellStyle8.Format = "0.00";
            this.colDistance.DefaultCellStyle = dataGridViewCellStyle8;
            this.colDistance.HeaderText = "Distance";
            this.colDistance.Name = "colDistance";
            this.colDistance.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colDistance.Width = 55;
            // 
            // colSize
            //
            this.colSize.DataPropertyName = "Size";
            this.colSize.HeaderText = "Size";
            this.colSize.MaxInputLength = 20;
            this.colSize.Name = "colSize";
            this.colSize.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colSize.Width = 60;
            //
            // colVision
            // 
            this.colVision.DataPropertyName = "Vision";
            this.colVision.HeaderText = "Vision";
            this.colVision.Name = "colVision";
            this.colVision.Width = 41;
            // 
            // colVisionType
            // 
            this.colVisionType.DataPropertyName = "VisionType";
            this.colVisionType.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.colVisionType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colVisionType.HeaderText = "Type";
            this.colVisionType.Items.AddRange(new object[] {
            "NULL",
            "1feet",
            "2feet",
            "3feet",
            "TwoRowIC",
            "FourRowIC",
            "BGA",
            "High LED",
            "Currency"});
            this.colVisionType.Name = "colVisionType";
            this.colVisionType.Width = 37;
            // 
            // colThreshold
            // 
            this.colThreshold.DataPropertyName = "Threshold";
            this.colThreshold.HeaderText = "Threshold";
            this.colThreshold.MaxInputLength = 3;
            this.colThreshold.Name = "colThreshold";
            this.colThreshold.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colThreshold.Width = 60;
            // 
            // colNozzle1
            // 
            this.colNozzle1.DataPropertyName = "Nozzle1";
            this.colNozzle1.HeaderText = "N1";
            this.colNozzle1.Name = "colNozzle1";
            this.colNozzle1.Width = 27;
            // 
            // colNozzle2
            // 
            this.colNozzle2.DataPropertyName = "Nozzle2";
            this.colNozzle2.HeaderText = "N2";
            this.colNozzle2.Name = "colNozzle2";
            this.colNozzle2.Width = 27;
            // 
            // colTakeDown
            // 
            this.colTakeDown.DataPropertyName = "TakeDown";
            this.colTakeDown.HeaderText = "T ↓";
            this.colTakeDown.MaxInputLength = 3;
            this.colTakeDown.Name = "colTakeDown";
            this.colTakeDown.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colTakeDown.Width = 47;
            // 
            // colTakeUp
            // 
            this.colTakeUp.DataPropertyName = "TakeUp";
            this.colTakeUp.HeaderText = "T ↑";
            this.colTakeUp.MaxInputLength = 3;
            this.colTakeUp.Name = "colTakeUp";
            this.colTakeUp.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colTakeUp.Width = 47;
            // 
            // colPasteDown
            // 
            this.colPasteDown.DataPropertyName = "PasteDown";
            this.colPasteDown.HeaderText = "P ↓";
            this.colPasteDown.MaxInputLength = 3;
            this.colPasteDown.Name = "colPasteDown";
            this.colPasteDown.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colPasteDown.Width = 49;
            // 
            // colPasteUp
            // 
            this.colPasteUp.DataPropertyName = "PasteUp";
            this.colPasteUp.HeaderText = "P ↑";
            this.colPasteUp.MaxInputLength = 3;
            this.colPasteUp.Name = "colPasteUp";
            this.colPasteUp.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colPasteUp.Width = 49;
            // 
            // colUsedBy
            // 
            this.colUsedBy.DataPropertyName = "UsedBy";
            this.colUsedBy.HeaderText = "File 1";
            this.colUsedBy.Name = "colUsedBy";
            this.colUsedBy.ReadOnly = true;
            this.colUsedBy.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colOtherUsedBy
            // 
            this.colOtherUsedBy.DataPropertyName = "OtherUsedBy";
            this.colOtherUsedBy.HeaderText = "File 2";
            this.colOtherUsedBy.Name = "colOtherUsedBy";
            this.colOtherUsedBy.ReadOnly = true;
            this.colOtherUsedBy.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // FeedersView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridFeeders);
            this.Name = "FeedersView";
            this.Size = new System.Drawing.Size(1000, 500);
            ((System.ComponentModel.ISupportInitialize)(this.gridFeeders)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridFeeders;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNumber;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colEnabled;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPackage;
        private System.Windows.Forms.DataGridViewTextBoxColumn colX;
        private System.Windows.Forms.DataGridViewTextBoxColumn colY;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHoleX;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHoleY;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAngle;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNozzleHeight;
        private System.Windows.Forms.DataGridViewTextBoxColumn colThickness;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDistance;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSize;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colVision;
        private System.Windows.Forms.DataGridViewComboBoxColumn colVisionType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colThreshold;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colNozzle1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colNozzle2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTakeDown;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTakeUp;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPasteDown;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPasteUp;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUsedBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOtherUsedBy;
    }
}
