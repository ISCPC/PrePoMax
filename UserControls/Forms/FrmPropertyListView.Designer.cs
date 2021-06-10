namespace UserControls
{
    partial class FrmPropertyListView
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("1");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("2");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("3");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("4");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("5");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("6");
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("7");
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem("8");
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem("9");
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem("10");
            this.gbType = new System.Windows.Forms.GroupBox();
            this.lvTypes = new UserControls.ListViewWithSelection();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbProperties.SuspendLayout();
            this.gbType.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbProperties
            // 
            this.gbProperties.Location = new System.Drawing.Point(12, 108);
            this.gbProperties.Size = new System.Drawing.Size(310, 312);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Size = new System.Drawing.Size(298, 284);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(160, 426);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(241, 426);
            // 
            // btnOkAddNew
            // 
            this.btnOkAddNew.Location = new System.Drawing.Point(79, 426);
            // 
            // gbType
            // 
            this.gbType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbType.Controls.Add(this.lvTypes);
            this.gbType.Location = new System.Drawing.Point(12, 12);
            this.gbType.Name = "gbType";
            this.gbType.Size = new System.Drawing.Size(310, 90);
            this.gbType.TabIndex = 15;
            this.gbType.TabStop = false;
            this.gbType.Text = "Type";
            // 
            // lvTypes
            // 
            this.lvTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvTypes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName});
            this.lvTypes.DisableMouse = false;
            this.lvTypes.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lvTypes.FullRowSelect = true;
            this.lvTypes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvTypes.HideSelection = false;
            this.lvTypes.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8,
            listViewItem9,
            listViewItem10});
            this.lvTypes.Location = new System.Drawing.Point(6, 22);
            this.lvTypes.MultiSelect = false;
            this.lvTypes.Name = "lvTypes";
            this.lvTypes.ShowGroups = false;
            this.lvTypes.Size = new System.Drawing.Size(298, 62);
            this.lvTypes.TabIndex = 0;
            this.lvTypes.UseCompatibleStateImageBehavior = false;
            this.lvTypes.View = System.Windows.Forms.View.Details;
            this.lvTypes.SelectedIndexChanged += new System.EventHandler(this.lvTypes_SelectedIndexChanged);
            this.lvTypes.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvTypes_MouseUp);
            // 
            // FrmPropertyListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 461);
            this.Controls.Add(this.gbType);
            this.MinimumSize = new System.Drawing.Size(350, 500);
            this.Name = "FrmPropertyListView";
            this.Text = "FrmPropertyListView";
            this.VisibleChanged += new System.EventHandler(this.FrmPropertyListView_VisibleChanged);
            this.Controls.SetChildIndex(this.gbProperties, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnOkAddNew, 0);
            this.Controls.SetChildIndex(this.gbType, 0);
            this.gbProperties.ResumeLayout(false);
            this.gbType.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.GroupBox gbType;
        protected ListViewWithSelection lvTypes;
        private System.Windows.Forms.ColumnHeader colName;
    }
}