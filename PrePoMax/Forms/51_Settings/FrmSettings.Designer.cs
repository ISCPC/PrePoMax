namespace PrePoMax.Forms
{
    partial class FrmSettings
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("1");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("2");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("3");
            this.gbProperties = new System.Windows.Forms.GroupBox();
            this.lvSettings = new UserControls.ListViewWithSelection();
            this.propertyGrid = new UserControls.TabEnabledPropertyGrid();
            this.cmsPropertyGrid = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiResetAll = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbProperties.SuspendLayout();
            this.cmsPropertyGrid.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbProperties
            // 
            this.gbProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbProperties.Controls.Add(this.lvSettings);
            this.gbProperties.Controls.Add(this.propertyGrid);
            this.gbProperties.Location = new System.Drawing.Point(12, 12);
            this.gbProperties.Name = "gbProperties";
            this.gbProperties.Size = new System.Drawing.Size(710, 408);
            this.gbProperties.TabIndex = 0;
            this.gbProperties.TabStop = false;
            this.gbProperties.Text = "Data";
            // 
            // lvSettings
            // 
            this.lvSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lvSettings.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName});
            this.lvSettings.DisableMouse = false;
            this.lvSettings.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lvSettings.FullRowSelect = true;
            this.lvSettings.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvSettings.HideSelection = false;
            this.lvSettings.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3});
            this.lvSettings.Location = new System.Drawing.Point(6, 22);
            this.lvSettings.MultiSelect = false;
            this.lvSettings.Name = "lvSettings";
            this.lvSettings.ShowGroups = false;
            this.lvSettings.Size = new System.Drawing.Size(175, 380);
            this.lvSettings.TabIndex = 4;
            this.lvSettings.UseCompatibleStateImageBehavior = false;
            this.lvSettings.View = System.Windows.Forms.View.Details;
            this.lvSettings.SelectedIndexChanged += new System.EventHandler(this.lvSettings_SelectedIndexChanged);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid.ContextMenuStrip = this.cmsPropertyGrid;
            this.propertyGrid.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.propertyGrid.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.propertyGrid.LineColor = System.Drawing.SystemColors.Control;
            this.propertyGrid.Location = new System.Drawing.Point(187, 22);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(517, 380);
            this.propertyGrid.TabIndex = 3;
            this.propertyGrid.ToolbarVisible = false;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            // 
            // cmsPropertyGrid
            // 
            this.cmsPropertyGrid.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiResetAll});
            this.cmsPropertyGrid.Name = "cmsPropertyGrid";
            this.cmsPropertyGrid.Size = new System.Drawing.Size(118, 26);
            this.cmsPropertyGrid.Opening += new System.ComponentModel.CancelEventHandler(this.cmsPropertyGrid_Opening);
            // 
            // tsmiResetAll
            // 
            this.tsmiResetAll.Name = "tsmiResetAll";
            this.tsmiResetAll.Size = new System.Drawing.Size(117, 22);
            this.tsmiResetAll.Text = "Reset all";
            this.tsmiResetAll.Click += new System.EventHandler(this.tsmiResetAll_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(485, 426);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 14;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(647, 426);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(566, 426);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 13;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // FrmSettings
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(734, 461);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.gbProperties);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(750, 500);
            this.Name = "FrmSettings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Edit Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmSettings_FormClosing);
            this.gbProperties.ResumeLayout(false);
            this.cmsPropertyGrid.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbProperties;
        private UserControls.TabEnabledPropertyGrid propertyGrid;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private UserControls.ListViewWithSelection lvSettings;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.ContextMenuStrip cmsPropertyGrid;
        private System.Windows.Forms.ToolStripMenuItem tsmiResetAll;
        private System.Windows.Forms.ColumnHeader colName;
    }
}