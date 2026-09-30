namespace FlyerSMT_H8_Editor
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        // Written by the designer: change it there, not by hand
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tableTop = new System.Windows.Forms.TableLayoutPanel();
            this.buttonOpen1 = new System.Windows.Forms.Button();
            this.buttonSave1 = new System.Windows.Forms.Button();
            this.buttonOptimize1 = new System.Windows.Forms.Button();
            this.buttonRevert1 = new System.Windows.Forms.Button();
            this.buttonRestore1 = new System.Windows.Forms.Button();
            this.buttonExport1 = new System.Windows.Forms.Button();
            this.buttonImport1 = new System.Windows.Forms.Button();
            this.textPath1 = new System.Windows.Forms.TextBox();
            this.buttonOpen2 = new System.Windows.Forms.Button();
            this.buttonSave2 = new System.Windows.Forms.Button();
            this.buttonOptimize2 = new System.Windows.Forms.Button();
            this.buttonRevert2 = new System.Windows.Forms.Button();
            this.buttonRestore2 = new System.Windows.Forms.Button();
            this.buttonExport2 = new System.Windows.Forms.Button();
            this.buttonImport2 = new System.Windows.Forms.Button();
            this.textPath2 = new System.Windows.Forms.TextBox();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabPcb1 = new System.Windows.Forms.TabPage();
            this.pcbView1 = new FlyerSMT_H8_Editor.PcbView();
            this.tabFeeders1 = new System.Windows.Forms.TabPage();
            this.feedersView1 = new FlyerSMT_H8_Editor.FeedersView();
            this.tabComponents1 = new System.Windows.Forms.TabPage();
            this.componentsView1 = new FlyerSMT_H8_Editor.ComponentsView();
            this.tabPcb2 = new System.Windows.Forms.TabPage();
            this.pcbView2 = new FlyerSMT_H8_Editor.PcbView();
            this.tabFeeders2 = new System.Windows.Forms.TabPage();
            this.feedersView2 = new FlyerSMT_H8_Editor.FeedersView();
            this.tabComponents2 = new System.Windows.Forms.TabPage();
            this.componentsView2 = new FlyerSMT_H8_Editor.ComponentsView();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusMain = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusPath = new System.Windows.Forms.ToolStripStatusLabel();
            this.tableTop.SuspendLayout();
            this.tabs.SuspendLayout();
            this.tabPcb1.SuspendLayout();
            this.tabFeeders1.SuspendLayout();
            this.tabComponents1.SuspendLayout();
            this.tabPcb2.SuspendLayout();
            this.tabFeeders2.SuspendLayout();
            this.tabComponents2.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableTop
            // 
            this.tableTop.ColumnCount = 8;
            this.tableTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableTop.Controls.Add(this.buttonOpen1, 0, 0);
            this.tableTop.Controls.Add(this.buttonSave1, 1, 0);
            this.tableTop.Controls.Add(this.buttonOptimize1, 2, 0);
            this.tableTop.Controls.Add(this.buttonRevert1, 3, 0);
            this.tableTop.Controls.Add(this.buttonRestore1, 4, 0);
            this.tableTop.Controls.Add(this.buttonExport1, 5, 0);
            this.tableTop.Controls.Add(this.buttonImport1, 6, 0);
            this.tableTop.Controls.Add(this.textPath1, 7, 0);
            this.tableTop.Controls.Add(this.buttonOpen2, 0, 1);
            this.tableTop.Controls.Add(this.buttonSave2, 1, 1);
            this.tableTop.Controls.Add(this.buttonOptimize2, 2, 1);
            this.tableTop.Controls.Add(this.buttonRevert2, 3, 1);
            this.tableTop.Controls.Add(this.buttonRestore2, 4, 1);
            this.tableTop.Controls.Add(this.buttonExport2, 5, 1);
            this.tableTop.Controls.Add(this.buttonImport2, 6, 1);
            this.tableTop.Controls.Add(this.textPath2, 7, 1);
            this.tableTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableTop.Location = new System.Drawing.Point(0, 0);
            this.tableTop.Name = "tableTop";
            this.tableTop.Padding = new System.Windows.Forms.Padding(6, 4, 6, 2);
            this.tableTop.RowCount = 2;
            this.tableTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableTop.Size = new System.Drawing.Size(1264, 68);
            this.tableTop.TabIndex = 1;
            // 
            // buttonOpen1
            // 
            this.buttonOpen1.AutoSize = true;
            this.buttonOpen1.Location = new System.Drawing.Point(9, 7);
            this.buttonOpen1.Name = "buttonOpen1";
            this.buttonOpen1.Size = new System.Drawing.Size(77, 23);
            this.buttonOpen1.TabIndex = 0;
            this.buttonOpen1.Text = "Open 1...";
            this.buttonOpen1.UseVisualStyleBackColor = true;
            // 
            // buttonSave1
            // 
            this.buttonSave1.AutoSize = true;
            this.buttonSave1.Enabled = false;
            this.buttonSave1.Location = new System.Drawing.Point(92, 7);
            this.buttonSave1.Name = "buttonSave1";
            this.buttonSave1.Size = new System.Drawing.Size(75, 23);
            this.buttonSave1.TabIndex = 1;
            this.buttonSave1.Text = "Save 1";
            this.buttonSave1.UseVisualStyleBackColor = true;
            // 
            // buttonOptimize1
            // 
            this.buttonOptimize1.AutoSize = true;
            this.buttonOptimize1.Enabled = false;
            this.buttonOptimize1.Location = new System.Drawing.Point(173, 7);
            this.buttonOptimize1.Name = "buttonOptimize1";
            this.buttonOptimize1.Size = new System.Drawing.Size(75, 23);
            this.buttonOptimize1.TabIndex = 11;
            this.buttonOptimize1.Text = "Optimize 1";
            this.buttonOptimize1.UseVisualStyleBackColor = true;
            // 
            // buttonRevert1
            // 
            this.buttonRevert1.AutoSize = true;
            this.buttonRevert1.Enabled = false;
            this.buttonRevert1.Location = new System.Drawing.Point(254, 7);
            this.buttonRevert1.Name = "buttonRevert1";
            this.buttonRevert1.Size = new System.Drawing.Size(75, 23);
            this.buttonRevert1.TabIndex = 2;
            this.buttonRevert1.Text = "Revert 1";
            this.buttonRevert1.UseVisualStyleBackColor = true;
            // 
            // buttonRestore1
            // 
            this.buttonRestore1.AutoSize = true;
            this.buttonRestore1.Enabled = false;
            this.buttonRestore1.Location = new System.Drawing.Point(335, 7);
            this.buttonRestore1.Name = "buttonRestore1";
            this.buttonRestore1.Size = new System.Drawing.Size(75, 23);
            this.buttonRestore1.TabIndex = 3;
            this.buttonRestore1.Text = "Restore 1";
            this.buttonRestore1.UseVisualStyleBackColor = true;
            // 
            // buttonExport1
            // 
            this.buttonExport1.AutoSize = true;
            this.buttonExport1.Enabled = false;
            this.buttonExport1.Location = new System.Drawing.Point(416, 7);
            this.buttonExport1.Name = "buttonExport1";
            this.buttonExport1.Size = new System.Drawing.Size(75, 23);
            this.buttonExport1.TabIndex = 21;
            this.buttonExport1.Text = "Export 1...";
            this.buttonExport1.UseVisualStyleBackColor = true;
            // 
            // buttonImport1
            // 
            this.buttonImport1.AutoSize = true;
            this.buttonImport1.Enabled = false;
            this.buttonImport1.Location = new System.Drawing.Point(497, 7);
            this.buttonImport1.Name = "buttonImport1";
            this.buttonImport1.Size = new System.Drawing.Size(75, 23);
            this.buttonImport1.TabIndex = 23;
            this.buttonImport1.Text = "Import 1...";
            this.buttonImport1.UseVisualStyleBackColor = true;
            // 
            // textPath1
            // 
            this.textPath1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textPath1.Location = new System.Drawing.Point(578, 9);
            this.textPath1.Name = "textPath1";
            this.textPath1.ReadOnly = true;
            this.textPath1.Size = new System.Drawing.Size(677, 20);
            this.textPath1.TabIndex = 4;
            // 
            // buttonOpen2
            // 
            this.buttonOpen2.AutoSize = true;
            this.buttonOpen2.Location = new System.Drawing.Point(9, 38);
            this.buttonOpen2.Name = "buttonOpen2";
            this.buttonOpen2.Size = new System.Drawing.Size(77, 23);
            this.buttonOpen2.TabIndex = 5;
            this.buttonOpen2.Text = "Open 2...";
            this.buttonOpen2.UseVisualStyleBackColor = true;
            // 
            // buttonSave2
            // 
            this.buttonSave2.AutoSize = true;
            this.buttonSave2.Enabled = false;
            this.buttonSave2.Location = new System.Drawing.Point(92, 38);
            this.buttonSave2.Name = "buttonSave2";
            this.buttonSave2.Size = new System.Drawing.Size(75, 23);
            this.buttonSave2.TabIndex = 6;
            this.buttonSave2.Text = "Save 2";
            this.buttonSave2.UseVisualStyleBackColor = true;
            // 
            // buttonOptimize2
            // 
            this.buttonOptimize2.AutoSize = true;
            this.buttonOptimize2.Enabled = false;
            this.buttonOptimize2.Location = new System.Drawing.Point(173, 38);
            this.buttonOptimize2.Name = "buttonOptimize2";
            this.buttonOptimize2.Size = new System.Drawing.Size(75, 23);
            this.buttonOptimize2.TabIndex = 12;
            this.buttonOptimize2.Text = "Optimize 2";
            this.buttonOptimize2.UseVisualStyleBackColor = true;
            // 
            // buttonRevert2
            // 
            this.buttonRevert2.AutoSize = true;
            this.buttonRevert2.Enabled = false;
            this.buttonRevert2.Location = new System.Drawing.Point(254, 38);
            this.buttonRevert2.Name = "buttonRevert2";
            this.buttonRevert2.Size = new System.Drawing.Size(75, 23);
            this.buttonRevert2.TabIndex = 7;
            this.buttonRevert2.Text = "Revert 2";
            this.buttonRevert2.UseVisualStyleBackColor = true;
            // 
            // buttonRestore2
            // 
            this.buttonRestore2.AutoSize = true;
            this.buttonRestore2.Enabled = false;
            this.buttonRestore2.Location = new System.Drawing.Point(335, 38);
            this.buttonRestore2.Name = "buttonRestore2";
            this.buttonRestore2.Size = new System.Drawing.Size(75, 23);
            this.buttonRestore2.TabIndex = 8;
            this.buttonRestore2.Text = "Restore 2";
            this.buttonRestore2.UseVisualStyleBackColor = true;
            // 
            // buttonExport2
            // 
            this.buttonExport2.AutoSize = true;
            this.buttonExport2.Enabled = false;
            this.buttonExport2.Location = new System.Drawing.Point(416, 38);
            this.buttonExport2.Name = "buttonExport2";
            this.buttonExport2.Size = new System.Drawing.Size(75, 23);
            this.buttonExport2.TabIndex = 22;
            this.buttonExport2.Text = "Export 2...";
            this.buttonExport2.UseVisualStyleBackColor = true;
            // 
            // buttonImport2
            // 
            this.buttonImport2.AutoSize = true;
            this.buttonImport2.Enabled = false;
            this.buttonImport2.Location = new System.Drawing.Point(497, 38);
            this.buttonImport2.Name = "buttonImport2";
            this.buttonImport2.Size = new System.Drawing.Size(75, 23);
            this.buttonImport2.TabIndex = 24;
            this.buttonImport2.Text = "Import 2...";
            this.buttonImport2.UseVisualStyleBackColor = true;
            // 
            // textPath2
            // 
            this.textPath2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textPath2.Location = new System.Drawing.Point(578, 40);
            this.textPath2.Name = "textPath2";
            this.textPath2.ReadOnly = true;
            this.textPath2.Size = new System.Drawing.Size(677, 20);
            this.textPath2.TabIndex = 9;
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabPcb1);
            this.tabs.Controls.Add(this.tabFeeders1);
            this.tabs.Controls.Add(this.tabComponents1);
            this.tabs.Controls.Add(this.tabPcb2);
            this.tabs.Controls.Add(this.tabFeeders2);
            this.tabs.Controls.Add(this.tabComponents2);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Location = new System.Drawing.Point(0, 68);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(1264, 671);
            this.tabs.TabIndex = 0;
            this.tabs.Deselecting += new System.Windows.Forms.TabControlCancelEventHandler(this.Tabs_Deselecting);
            // 
            // tabPcb1
            // 
            this.tabPcb1.Controls.Add(this.pcbView1);
            this.tabPcb1.Location = new System.Drawing.Point(4, 22);
            this.tabPcb1.Name = "tabPcb1";
            this.tabPcb1.Size = new System.Drawing.Size(1256, 645);
            this.tabPcb1.TabIndex = 6;
            this.tabPcb1.Text = "PCB 1";
            this.tabPcb1.UseVisualStyleBackColor = true;
            // 
            // pcbView1
            // 
            this.pcbView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pcbView1.Location = new System.Drawing.Point(0, 0);
            this.pcbView1.Name = "pcbView1";
            this.pcbView1.Size = new System.Drawing.Size(1256, 645);
            this.pcbView1.TabIndex = 0;
            // 
            // tabFeeders1
            // 
            this.tabFeeders1.Controls.Add(this.feedersView1);
            this.tabFeeders1.Location = new System.Drawing.Point(4, 22);
            this.tabFeeders1.Name = "tabFeeders1";
            this.tabFeeders1.Size = new System.Drawing.Size(1256, 645);
            this.tabFeeders1.TabIndex = 0;
            this.tabFeeders1.Text = "Feeders 1";
            this.tabFeeders1.UseVisualStyleBackColor = true;
            // 
            // feedersView1
            // 
            this.feedersView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.feedersView1.Location = new System.Drawing.Point(0, 0);
            this.feedersView1.Name = "feedersView1";
            this.feedersView1.Size = new System.Drawing.Size(1256, 645);
            this.feedersView1.TabIndex = 0;
            // 
            // tabComponents1
            // 
            this.tabComponents1.Controls.Add(this.componentsView1);
            this.tabComponents1.Location = new System.Drawing.Point(4, 22);
            this.tabComponents1.Name = "tabComponents1";
            this.tabComponents1.Size = new System.Drawing.Size(1256, 645);
            this.tabComponents1.TabIndex = 1;
            this.tabComponents1.Text = "Components 1";
            this.tabComponents1.UseVisualStyleBackColor = true;
            // 
            // componentsView1
            // 
            this.componentsView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.componentsView1.Location = new System.Drawing.Point(0, 0);
            this.componentsView1.Name = "componentsView1";
            this.componentsView1.Size = new System.Drawing.Size(1256, 645);
            this.componentsView1.TabIndex = 0;
            // 
            // tabPcb2
            // 
            this.tabPcb2.Controls.Add(this.pcbView2);
            this.tabPcb2.Location = new System.Drawing.Point(4, 22);
            this.tabPcb2.Name = "tabPcb2";
            this.tabPcb2.Size = new System.Drawing.Size(1256, 645);
            this.tabPcb2.TabIndex = 7;
            this.tabPcb2.Text = "PCB 2";
            this.tabPcb2.UseVisualStyleBackColor = true;
            // 
            // pcbView2
            // 
            this.pcbView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pcbView2.Location = new System.Drawing.Point(0, 0);
            this.pcbView2.Name = "pcbView2";
            this.pcbView2.Size = new System.Drawing.Size(1256, 645);
            this.pcbView2.TabIndex = 0;
            // 
            // tabFeeders2
            // 
            this.tabFeeders2.Controls.Add(this.feedersView2);
            this.tabFeeders2.Location = new System.Drawing.Point(4, 22);
            this.tabFeeders2.Name = "tabFeeders2";
            this.tabFeeders2.Size = new System.Drawing.Size(1256, 645);
            this.tabFeeders2.TabIndex = 3;
            this.tabFeeders2.Text = "Feeders 2";
            this.tabFeeders2.UseVisualStyleBackColor = true;
            // 
            // feedersView2
            // 
            this.feedersView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.feedersView2.Location = new System.Drawing.Point(0, 0);
            this.feedersView2.Name = "feedersView2";
            this.feedersView2.Size = new System.Drawing.Size(1256, 645);
            this.feedersView2.TabIndex = 0;
            // 
            // tabComponents2
            // 
            this.tabComponents2.Controls.Add(this.componentsView2);
            this.tabComponents2.Location = new System.Drawing.Point(4, 22);
            this.tabComponents2.Name = "tabComponents2";
            this.tabComponents2.Size = new System.Drawing.Size(1256, 645);
            this.tabComponents2.TabIndex = 4;
            this.tabComponents2.Text = "Components 2";
            this.tabComponents2.UseVisualStyleBackColor = true;
            // 
            // componentsView2
            // 
            this.componentsView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.componentsView2.Location = new System.Drawing.Point(0, 0);
            this.componentsView2.Name = "componentsView2";
            this.componentsView2.Size = new System.Drawing.Size(1256, 645);
            this.componentsView2.TabIndex = 0;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusMain,
            this.statusPath});
            this.statusStrip.Location = new System.Drawing.Point(0, 739);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1264, 22);
            this.statusStrip.TabIndex = 2;
            // 
            // statusMain
            // 
            this.statusMain.Name = "statusMain";
            this.statusMain.Size = new System.Drawing.Size(1245, 17);
            this.statusMain.Spring = true;
            this.statusMain.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusPath
            // 
            this.statusPath.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.statusPath.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.statusPath.Name = "statusPath";
            this.statusPath.Size = new System.Drawing.Size(4, 17);
            this.statusPath.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 761);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.tableTop);
            this.Controls.Add(this.statusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FlyerSMT H8 Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.tableTop.ResumeLayout(false);
            this.tableTop.PerformLayout();
            this.tabs.ResumeLayout(false);
            this.tabPcb1.ResumeLayout(false);
            this.tabFeeders1.ResumeLayout(false);
            this.tabComponents1.ResumeLayout(false);
            this.tabPcb2.ResumeLayout(false);
            this.tabFeeders2.ResumeLayout(false);
            this.tabComponents2.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableTop;
        private System.Windows.Forms.Button buttonOpen1;
        private System.Windows.Forms.Button buttonSave1;
        private System.Windows.Forms.Button buttonOptimize1;
        private System.Windows.Forms.Button buttonRevert1;
        private System.Windows.Forms.Button buttonRestore1;
        private System.Windows.Forms.Button buttonExport1;
        private System.Windows.Forms.Button buttonImport1;
        private System.Windows.Forms.TextBox textPath1;
        private System.Windows.Forms.Button buttonOpen2;
        private System.Windows.Forms.Button buttonSave2;
        private System.Windows.Forms.Button buttonOptimize2;
        private System.Windows.Forms.Button buttonRevert2;
        private System.Windows.Forms.Button buttonRestore2;
        private System.Windows.Forms.Button buttonExport2;
        private System.Windows.Forms.Button buttonImport2;
        private System.Windows.Forms.TextBox textPath2;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabPcb1;
        private FlyerSMT_H8_Editor.PcbView pcbView1;
        private System.Windows.Forms.TabPage tabPcb2;
        private FlyerSMT_H8_Editor.PcbView pcbView2;
        private System.Windows.Forms.TabPage tabFeeders1;
        private FlyerSMT_H8_Editor.FeedersView feedersView1;
        private System.Windows.Forms.TabPage tabComponents1;
        private FlyerSMT_H8_Editor.ComponentsView componentsView1;
        private System.Windows.Forms.TabPage tabFeeders2;
        private FlyerSMT_H8_Editor.FeedersView feedersView2;
        private System.Windows.Forms.TabPage tabComponents2;
        private FlyerSMT_H8_Editor.ComponentsView componentsView2;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusMain;
        private System.Windows.Forms.ToolStripStatusLabel statusPath;
    }
}
