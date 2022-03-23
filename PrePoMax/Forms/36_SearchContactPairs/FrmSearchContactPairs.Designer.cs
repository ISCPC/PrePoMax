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
            this.cmsData = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSwapMasterSlave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMergeByMasterSlave = new System.Windows.Forms.ToolStripMenuItem();
            this.gbSearch = new System.Windows.Forms.GroupBox();
            this.lSurfaceinteraction = new System.Windows.Forms.Label();
            this.cbSurfaceInteraction = new System.Windows.Forms.ComboBox();
            this.lAdjustMesh = new System.Windows.Forms.Label();
            this.cbAbjustMesh = new System.Windows.Forms.ComboBox();
            this.lType = new System.Windows.Forms.Label();
            this.cbType = new System.Windows.Forms.ComboBox();
            this.lGroupBy = new System.Windows.Forms.Label();
            this.cbGroupBy = new System.Windows.Forms.ComboBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.lAngle = new System.Windows.Forms.Label();
            this.lDistance = new System.Windows.Forms.Label();
            this.gbPairs = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.propertyGrid = new UserControls.TabEnabledPropertyGrid();
            this.tbAngle = new UserControls.UnitAwareTextBox();
            this.tbDistance = new UserControls.UnitAwareTextBox();
            this.lMethod = new System.Windows.Forms.Label();
            this.cbContactPairMethod = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.cmsData.SuspendLayout();
            this.gbSearch.SuspendLayout();
            this.gbPairs.SuspendLayout();
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
            this.dgvData.Size = new System.Drawing.Size(587, 218);
            this.dgvData.TabIndex = 1;
            this.dgvData.SelectionChanged += new System.EventHandler(this.dgvData_SelectionChanged);
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
            // gbSearch
            // 
            this.gbSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSearch.Controls.Add(this.lMethod);
            this.gbSearch.Controls.Add(this.cbContactPairMethod);
            this.gbSearch.Controls.Add(this.lSurfaceinteraction);
            this.gbSearch.Controls.Add(this.cbSurfaceInteraction);
            this.gbSearch.Controls.Add(this.lAdjustMesh);
            this.gbSearch.Controls.Add(this.cbAbjustMesh);
            this.gbSearch.Controls.Add(this.lType);
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
            this.gbSearch.Size = new System.Drawing.Size(906, 93);
            this.gbSearch.TabIndex = 3;
            this.gbSearch.TabStop = false;
            this.gbSearch.Text = "Search";
            // 
            // lSurfaceinteraction
            // 
            this.lSurfaceinteraction.AutoSize = true;
            this.lSurfaceinteraction.Location = new System.Drawing.Point(193, 42);
            this.lSurfaceinteraction.Name = "lSurfaceinteraction";
            this.lSurfaceinteraction.Size = new System.Drawing.Size(106, 15);
            this.lSurfaceinteraction.TabIndex = 13;
            this.lSurfaceinteraction.Text = "Surface interaction";
            // 
            // cbSurfaceInteraction
            // 
            this.cbSurfaceInteraction.FormattingEnabled = true;
            this.cbSurfaceInteraction.Location = new System.Drawing.Point(305, 39);
            this.cbSurfaceInteraction.Name = "cbSurfaceInteraction";
            this.cbSurfaceInteraction.Size = new System.Drawing.Size(155, 23);
            this.cbSurfaceInteraction.TabIndex = 12;
            // 
            // lAdjustMesh
            // 
            this.lAdjustMesh.AutoSize = true;
            this.lAdjustMesh.Location = new System.Drawing.Point(6, 64);
            this.lAdjustMesh.Name = "lAdjustMesh";
            this.lAdjustMesh.Size = new System.Drawing.Size(73, 15);
            this.lAdjustMesh.TabIndex = 11;
            this.lAdjustMesh.Text = "Adjust mesh";
            // 
            // cbAbjustMesh
            // 
            this.cbAbjustMesh.FormattingEnabled = true;
            this.cbAbjustMesh.Location = new System.Drawing.Point(85, 61);
            this.cbAbjustMesh.Name = "cbAbjustMesh";
            this.cbAbjustMesh.Size = new System.Drawing.Size(87, 23);
            this.cbAbjustMesh.TabIndex = 10;
            // 
            // lType
            // 
            this.lType.AutoSize = true;
            this.lType.Location = new System.Drawing.Point(268, 20);
            this.lType.Name = "lType";
            this.lType.Size = new System.Drawing.Size(31, 15);
            this.lType.TabIndex = 9;
            this.lType.Text = "Type";
            // 
            // cbType
            // 
            this.cbType.FormattingEnabled = true;
            this.cbType.Location = new System.Drawing.Point(305, 17);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(155, 23);
            this.cbType.TabIndex = 8;
            this.cbType.SelectedIndexChanged += new System.EventHandler(this.cbType_SelectedIndexChanged);
            // 
            // lGroupBy
            // 
            this.lGroupBy.AutoSize = true;
            this.lGroupBy.Location = new System.Drawing.Point(488, 20);
            this.lGroupBy.Name = "lGroupBy";
            this.lGroupBy.Size = new System.Drawing.Size(56, 15);
            this.lGroupBy.TabIndex = 7;
            this.lGroupBy.Text = "Group by";
            // 
            // cbGroupBy
            // 
            this.cbGroupBy.FormattingEnabled = true;
            this.cbGroupBy.Location = new System.Drawing.Point(550, 17);
            this.cbGroupBy.Name = "cbGroupBy";
            this.cbGroupBy.Size = new System.Drawing.Size(87, 23);
            this.cbGroupBy.TabIndex = 5;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(813, 58);
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
            this.lAngle.Location = new System.Drawing.Point(41, 42);
            this.lAngle.Name = "lAngle";
            this.lAngle.Size = new System.Drawing.Size(38, 15);
            this.lAngle.TabIndex = 3;
            this.lAngle.Text = "Angle";
            // 
            // lDistance
            // 
            this.lDistance.AutoSize = true;
            this.lDistance.Location = new System.Drawing.Point(27, 20);
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
            this.gbPairs.Location = new System.Drawing.Point(12, 111);
            this.gbPairs.Name = "gbPairs";
            this.gbPairs.Size = new System.Drawing.Size(906, 255);
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
            // propertyGrid
            // 
            this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.propertyGrid.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.propertyGrid.LineColor = System.Drawing.SystemColors.Control;
            this.propertyGrid.Location = new System.Drawing.Point(600, 22);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(300, 218);
            this.propertyGrid.TabIndex = 7;
            this.propertyGrid.ToolbarVisible = false;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            // 
            // tbAngle
            // 
            this.tbAngle.Location = new System.Drawing.Point(85, 39);
            this.tbAngle.Name = "tbAngle";
            this.tbAngle.Size = new System.Drawing.Size(87, 23);
            this.tbAngle.TabIndex = 3;
            this.tbAngle.UnitConverter = null;
            // 
            // tbDistance
            // 
            this.tbDistance.Location = new System.Drawing.Point(85, 17);
            this.tbDistance.Name = "tbDistance";
            this.tbDistance.Size = new System.Drawing.Size(87, 23);
            this.tbDistance.TabIndex = 2;
            this.tbDistance.UnitConverter = null;
            // 
            // lMethod
            // 
            this.lMethod.AutoSize = true;
            this.lMethod.Location = new System.Drawing.Point(250, 64);
            this.lMethod.Name = "lMethod";
            this.lMethod.Size = new System.Drawing.Size(49, 15);
            this.lMethod.TabIndex = 15;
            this.lMethod.Text = "Method";
            // 
            // cbContactPairMethod
            // 
            this.cbContactPairMethod.FormattingEnabled = true;
            this.cbContactPairMethod.Location = new System.Drawing.Point(305, 61);
            this.cbContactPairMethod.Name = "cbContactPairMethod";
            this.cbContactPairMethod.Size = new System.Drawing.Size(155, 23);
            this.cbContactPairMethod.TabIndex = 14;
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
            this.cmsData.ResumeLayout(false);
            this.gbSearch.ResumeLayout(false);
            this.gbSearch.PerformLayout();
            this.gbPairs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.GroupBox gbSearch;
        private UserControls.TabEnabledPropertyGrid propertyGrid;
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
        private System.Windows.Forms.Label lType;
        private System.Windows.Forms.ComboBox cbType;
        private System.Windows.Forms.Label lAdjustMesh;
        private System.Windows.Forms.ComboBox cbAbjustMesh;
        private System.Windows.Forms.Label lSurfaceinteraction;
        private System.Windows.Forms.ComboBox cbSurfaceInteraction;
        private System.Windows.Forms.ContextMenuStrip cmsData;
        private System.Windows.Forms.ToolStripMenuItem tsmiSwapMasterSlave;
        private System.Windows.Forms.ToolStripMenuItem tsmiMergeByMasterSlave;
        private System.Windows.Forms.Label lMethod;
        private System.Windows.Forms.ComboBox cbContactPairMethod;
    }
}