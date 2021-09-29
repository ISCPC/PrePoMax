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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.gbSettings = new System.Windows.Forms.GroupBox();
            this.cbAdjust = new System.Windows.Forms.CheckBox();
            this.lGroupBy = new System.Windows.Forms.Label();
            this.cbGroupBy = new System.Windows.Forms.ComboBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.lProperty = new System.Windows.Forms.Label();
            this.tbAngle = new UserControls.UnitAwareTextBox();
            this.lAngle = new System.Windows.Forms.Label();
            this.cbProperty = new System.Windows.Forms.ComboBox();
            this.tbDistance = new UserControls.UnitAwareTextBox();
            this.lDistance = new System.Windows.Forms.Label();
            this.propertyGrid = new UserControls.TabbedPropertyGrid();
            this.gbPairs = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.gbSettings.SuspendLayout();
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
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvData.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvData.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvData.Location = new System.Drawing.Point(7, 22);
            this.dgvData.Name = "dgvData";
            this.dgvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvData.Size = new System.Drawing.Size(525, 199);
            this.dgvData.TabIndex = 1;
            this.dgvData.SelectionChanged += new System.EventHandler(this.dgvData_SelectionChanged);
            // 
            // gbSettings
            // 
            this.gbSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSettings.Controls.Add(this.cbAdjust);
            this.gbSettings.Controls.Add(this.lGroupBy);
            this.gbSettings.Controls.Add(this.cbGroupBy);
            this.gbSettings.Controls.Add(this.btnSearch);
            this.gbSettings.Controls.Add(this.lProperty);
            this.gbSettings.Controls.Add(this.tbAngle);
            this.gbSettings.Controls.Add(this.lAngle);
            this.gbSettings.Controls.Add(this.cbProperty);
            this.gbSettings.Controls.Add(this.tbDistance);
            this.gbSettings.Controls.Add(this.lDistance);
            this.gbSettings.Location = new System.Drawing.Point(12, 12);
            this.gbSettings.Name = "gbSettings";
            this.gbSettings.Size = new System.Drawing.Size(844, 83);
            this.gbSettings.TabIndex = 3;
            this.gbSettings.TabStop = false;
            this.gbSettings.Text = "Settings";
            // 
            // cbAdjust
            // 
            this.cbAdjust.AutoSize = true;
            this.cbAdjust.Location = new System.Drawing.Point(454, 24);
            this.cbAdjust.Name = "cbAdjust";
            this.cbAdjust.Size = new System.Drawing.Size(92, 19);
            this.cbAdjust.TabIndex = 9;
            this.cbAdjust.Text = "Adjust mesh";
            this.cbAdjust.UseVisualStyleBackColor = true;
            // 
            // lGroupBy
            // 
            this.lGroupBy.AutoSize = true;
            this.lGroupBy.Location = new System.Drawing.Point(225, 54);
            this.lGroupBy.Name = "lGroupBy";
            this.lGroupBy.Size = new System.Drawing.Size(56, 15);
            this.lGroupBy.TabIndex = 7;
            this.lGroupBy.Text = "Group by";
            // 
            // cbGroupBy
            // 
            this.cbGroupBy.FormattingEnabled = true;
            this.cbGroupBy.Location = new System.Drawing.Point(287, 51);
            this.cbGroupBy.Name = "cbGroupBy";
            this.cbGroupBy.Size = new System.Drawing.Size(116, 23);
            this.cbGroupBy.TabIndex = 4;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(751, 48);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(87, 27);
            this.btnSearch.TabIndex = 8;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // lProperty
            // 
            this.lProperty.AutoSize = true;
            this.lProperty.Location = new System.Drawing.Point(229, 25);
            this.lProperty.Name = "lProperty";
            this.lProperty.Size = new System.Drawing.Size(52, 15);
            this.lProperty.TabIndex = 5;
            this.lProperty.Text = "Property";
            // 
            // tbAngle
            // 
            this.tbAngle.Location = new System.Drawing.Point(71, 51);
            this.tbAngle.Name = "tbAngle";
            this.tbAngle.Size = new System.Drawing.Size(116, 23);
            this.tbAngle.TabIndex = 2;
            this.tbAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbAngle.UnitConverter = null;
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
            // cbProperty
            // 
            this.cbProperty.FormattingEnabled = true;
            this.cbProperty.Location = new System.Drawing.Point(287, 22);
            this.cbProperty.Name = "cbProperty";
            this.cbProperty.Size = new System.Drawing.Size(116, 23);
            this.cbProperty.TabIndex = 3;
            // 
            // tbDistance
            // 
            this.tbDistance.Location = new System.Drawing.Point(71, 22);
            this.tbDistance.Name = "tbDistance";
            this.tbDistance.Size = new System.Drawing.Size(116, 23);
            this.tbDistance.TabIndex = 1;
            this.tbDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbDistance.UnitConverter = null;
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
            // propertyGrid
            // 
            this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.propertyGrid.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.propertyGrid.LineColor = System.Drawing.SystemColors.Control;
            this.propertyGrid.Location = new System.Drawing.Point(538, 22);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(300, 199);
            this.propertyGrid.TabIndex = 7;
            this.propertyGrid.ToolbarVisible = false;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
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
            this.gbPairs.Size = new System.Drawing.Size(844, 236);
            this.gbPairs.TabIndex = 8;
            this.gbPairs.TabStop = false;
            this.gbPairs.Text = "Contact Pairs";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(763, 343);
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
            this.btnOK.Location = new System.Drawing.Point(670, 343);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(87, 27);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // FrmSearchContactPairs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(872, 382);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.gbPairs);
            this.Controls.Add(this.gbSettings);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(767, 398);
            this.Name = "FrmSearchContactPairs";
            this.Text = "FrmSearchContactPairs";
            this.Load += new System.EventHandler(this.FrmSearchContactPairs_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.gbSettings.ResumeLayout(false);
            this.gbSettings.PerformLayout();
            this.gbPairs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.GroupBox gbSettings;
        private UserControls.TabbedPropertyGrid propertyGrid;
        private System.Windows.Forms.Label lProperty;
        private UserControls.UnitAwareTextBox tbAngle;
        private System.Windows.Forms.Label lAngle;
        private System.Windows.Forms.ComboBox cbProperty;
        private UserControls.UnitAwareTextBox tbDistance;
        private System.Windows.Forms.Label lDistance;
        private System.Windows.Forms.GroupBox gbPairs;
        private System.Windows.Forms.Label lGroupBy;
        private System.Windows.Forms.ComboBox cbGroupBy;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox cbAdjust;
    }
}