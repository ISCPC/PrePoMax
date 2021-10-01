using CaeGlobals;

namespace PrePoMax.Forms
{
    partial class FrmSearchContactPairs
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.gbSearch = new System.Windows.Forms.GroupBox();
            this.lGroupBy = new System.Windows.Forms.Label();
            this.cbGroupBy = new System.Windows.Forms.ComboBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.lAngle = new System.Windows.Forms.Label();
            this.lDistance = new System.Windows.Forms.Label();
            this.gbPairs = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbType = new System.Windows.Forms.ComboBox();
            this.lAdjustMesh = new System.Windows.Forms.Label();
            this.cbAbjustMesh = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbSurfaceInteraction = new System.Windows.Forms.ComboBox();
            this.cmsData = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSwapMasterSlave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMergeByMasterSlave = new System.Windows.Forms.ToolStripMenuItem();
            this.propertyGrid = new UserControls.TabbedPropertyGrid();
            this.tbAngle = new UserControls.UnitAwareTextBox();
            this.tbDistance = new UserControls.UnitAwareTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.gbSearch.SuspendLayout();
            this.gbPairs.SuspendLayout();
            this.cmsData.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvData
            // 
            this.dgvData.AllowUserToAddRows = false;
            this.dgvData.AllowUserToResizeRows = false;
            this.dgvData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvData.ContextMenuStrip = this.cmsData;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvData.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvData.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvData.Location = new System.Drawing.Point(7, 22);
            this.dgvData.Name = "dgvData";
            this.dgvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvData.Size = new System.Drawing.Size(587, 228);
            this.dgvData.TabIndex = 1;
            this.dgvData.SelectionChanged += new System.EventHandler(this.dgvData_SelectionChanged);
            // 
            // gbSearch
            // 
            this.gbSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSearch.Controls.Add(this.label2);
            this.gbSearch.Controls.Add(this.cbSurfaceInteraction);
            this.gbSearch.Controls.Add(this.lAdjustMesh);
            this.gbSearch.Controls.Add(this.cbAbjustMesh);
            this.gbSearch.Controls.Add(this.label1);
            this.gbSearch.Controls.Add(this.cbType);
            this.gbSearch.Controls.Add(this.lGroupBy);
            this.gbSearch.Controls.Add(this.cbGroupBy);
            this.gbSearch.Controls.Add(this.btnSearch);
            this.gbSearch.Controls.Add(this.tbAngle);
            this.gbSearch.Controls.Add(this.lAngle);
            this.gbSearch.Controls.Add(this.tbDistance);
            this.gbSearch.Controls.Add(this.lDistance);
            this.gbSearch.Location = new System.Drawing.Point(12, 12);
            this.gbSearch.Name = "gbSearch";
            this.gbSearch.Size = new System.Drawing.Size(906, 83);
            this.gbSearch.TabIndex = 3;
            this.gbSearch.TabStop = false;
            this.gbSearch.Text = "Search";
            // 
            // lGroupBy
            // 
            this.lGroupBy.AutoSize = true;
            this.lGroupBy.Location = new System.Drawing.Point(489, 25);
            this.lGroupBy.Name = "lGroupBy";
            this.lGroupBy.Size = new System.Drawing.Size(56, 15);
            this.lGroupBy.TabIndex = 7;
            this.lGroupBy.Text = "Group by";
            // 
            // cbGroupBy
            // 
            this.cbGroupBy.FormattingEnabled = true;
            this.cbGroupBy.Location = new System.Drawing.Point(551, 22);
            this.cbGroupBy.Name = "cbGroupBy";
            this.cbGroupBy.Size = new System.Drawing.Size(87, 23);
            this.cbGroupBy.TabIndex = 5;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(813, 48);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(87, 27);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // lAngle
            // 
            this.lAngle.AutoSize = true;
            this.lAngle.Location = new System.Drawing.Point(27, 54);
            this.lAngle.Name = "lAngle";
            this.lAngle.Size = new System.Drawing.Size(38, 15);
            this.lAngle.TabIndex = 3;
            this.lAngle.Text = "Angle";
            // 
            // lDistance
            // 
            this.lDistance.AutoSize = true;
            this.lDistance.Location = new System.Drawing.Point(13, 25);
            this.lDistance.Name = "lDistance";
            this.lDistance.Size = new System.Drawing.Size(52, 15);
            this.lDistance.TabIndex = 0;
            this.lDistance.Text = "Distance";
            // 
            // gbPairs
            // 
            this.gbPairs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbPairs.Controls.Add(this.propertyGrid);
            this.gbPairs.Controls.Add(this.dgvData);
            this.gbPairs.Location = new System.Drawing.Point(12, 101);
            this.gbPairs.Name = "gbPairs";
            this.gbPairs.Size = new System.Drawing.Size(906, 265);
            this.gbPairs.TabIndex = 8;
            this.gbPairs.TabStop = false;
            this.gbPairs.Text = "Contact Pairs";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(825, 372);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 27);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(732, 372);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(87, 27);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(254, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "Type";
            // 
            // cbType
            // 
            this.cbType.FormattingEnabled = true;
            this.cbType.Location = new System.Drawing.Point(291, 22);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(127, 23);
            this.cbType.TabIndex = 8;
            this.cbType.SelectedIndexChanged += new System.EventHandler(this.cbType_SelectedIndexChanged);
            // 
            // lAdjustMesh
            // 
            this.lAdjustMesh.AutoSize = true;
            this.lAdjustMesh.Location = new System.Drawing.Point(472, 54);
            this.lAdjustMesh.Name = "lAdjustMesh";
            this.lAdjustMesh.Size = new System.Drawing.Size(73, 15);
            this.lAdjustMesh.TabIndex = 11;
            this.lAdjustMesh.Text = "Adjust mesh";
            // 
            // cbAbjustMesh
            // 
            this.cbAbjustMesh.FormattingEnabled = true;
            this.cbAbjustMesh.Location = new System.Drawing.Point(551, 51);
            this.cbAbjustMesh.Name = "cbAbjustMesh";
            this.cbAbjustMesh.Size = new System.Drawing.Size(87, 23);
            this.cbAbjustMesh.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(179, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 15);
            this.label2.TabIndex = 13;
            this.label2.Text = "Surface interaction";
            // 
            // cbSurfaceInteraction
            // 
            this.cbSurfaceInteraction.FormattingEnabled = true;
            this.cbSurfaceInteraction.Location = new System.Drawing.Point(291, 51);
            this.cbSurfaceInteraction.Name = "cbSurfaceInteraction";
            this.cbSurfaceInteraction.Size = new System.Drawing.Size(127, 23);
            this.cbSurfaceInteraction.TabIndex = 12;
            // 
            // cmsData
            // 
            this.cmsData.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSwapMasterSlave,
            this.tsmiMergeByMasterSlave});
            this.cmsData.Name = "cmsData";
            this.cmsData.Size = new System.Drawing.Size(195, 48);
            // 
            // tsmiSwapMasterSlave
            // 
            this.tsmiSwapMasterSlave.Name = "tsmiSwapMasterSlave";
            this.tsmiSwapMasterSlave.Size = new System.Drawing.Size(194, 22);
            this.tsmiSwapMasterSlave.Text = "Swap master/slave";
            this.tsmiSwapMasterSlave.Click += new System.EventHandler(this.tsmiSwapMasterSlave_Click);
            // 
            // tsmiMergeByMasterSlave
            // 
            this.tsmiMergeByMasterSlave.Name = "tsmiMergeByMasterSlave";
            this.tsmiMergeByMasterSlave.Size = new System.Drawing.Size(194, 22);
            this.tsmiMergeByMasterSlave.Text = "Merge by master/slave";
            this.tsmiMergeByMasterSlave.Click += new System.EventHandler(this.tsmiMergeByMasterSlave_Click);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.propertyGrid.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.propertyGrid.LineColor = System.Drawing.SystemColors.Control;
            this.propertyGrid.Location = new System.Drawing.Point(600, 22);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(300, 228);
            this.propertyGrid.TabIndex = 7;
            this.propertyGrid.ToolbarVisible = false;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            // 
            // tbAngle
            // 
            this.tbAngle.Location = new System.Drawing.Point(71, 51);
            this.tbAngle.Name = "tbAngle";
            this.tbAngle.Size = new System.Drawing.Size(87, 23);
            this.tbAngle.TabIndex = 3;
            this.tbAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbAngle.UnitConverter = null;
            // 
            // tbDistance
            // 
            this.tbDistance.Location = new System.Drawing.Point(71, 22);
            this.tbDistance.Name = "tbDistance";
            this.tbDistance.Size = new System.Drawing.Size(87, 23);
            this.tbDistance.TabIndex = 2;
            this.tbDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbDistance.UnitConverter = null;
            // 
            // FrmSearchContactPairs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 411);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.gbPairs);
            this.Controls.Add(this.gbSearch);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(800, 400);
            this.Name = "FrmSearchContactPairs";
            this.Text = "Search Contact Pairs";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmSearchContactPairs_FormClosing);
            this.Load += new System.EventHandler(this.FrmSearchContactPairs_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.gbSearch.ResumeLayout(false);
            this.gbSearch.PerformLayout();
            this.gbPairs.ResumeLayout(false);
            this.cmsData.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.GroupBox gbSearch;
        private UserControls.TabbedPropertyGrid propertyGrid;
        private UserControls.UnitAwareTextBox tbAngle;
        private System.Windows.Forms.Label lAngle;
        private UserControls.UnitAwareTextBox tbDistance;
        private System.Windows.Forms.Label lDistance;
        private System.Windows.Forms.GroupBox gbPairs;
        private System.Windows.Forms.Label lGroupBy;
        private System.Windows.Forms.ComboBox cbGroupBy;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbType;
        private System.Windows.Forms.Label lAdjustMesh;
        private System.Windows.Forms.ComboBox cbAbjustMesh;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbSurfaceInteraction;
        private System.Windows.Forms.ContextMenuStrip cmsData;
        private System.Windows.Forms.ToolStripMenuItem tsmiSwapMasterSlave;
        private System.Windows.Forms.ToolStripMenuItem tsmiMergeByMasterSlave;
    }
}