namespace PrePoMax.Forms
{
    partial class FrmMaterialLibrary
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Materials");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMaterialLibrary));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnDeleteFromModel = new System.Windows.Forms.Button();
            this.lvModelMaterials = new UserControls.ListViewWithSelection();
            this.btnRename = new System.Windows.Forms.Button();
            this.tbCategoryName = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnDeleteFromLibrary = new System.Windows.Forms.Button();
            this.btnAddCategory = new System.Windows.Forms.Button();
            this.btvLibrary = new UserControls.BufferedTreeView();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCopyToLibrary = new System.Windows.Forms.Button();
            this.btnCopyToModel = new System.Windows.Forms.Button();
            this.ttText = new System.Windows.Forms.ToolTip(this.components);
            this.cbPreview = new System.Windows.Forms.CheckBox();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnDeleteFromModel);
            this.groupBox2.Controls.Add(this.lvModelMaterials);
            this.groupBox2.Location = new System.Drawing.Point(330, 14);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(205, 427);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "FE Model materials";
            // 
            // btnDeleteFromModel
            // 
            this.btnDeleteFromModel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteFromModel.Location = new System.Drawing.Point(124, 22);
            this.btnDeleteFromModel.Name = "btnDeleteFromModel";
            this.btnDeleteFromModel.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteFromModel.TabIndex = 10;
            this.btnDeleteFromModel.Text = "Delete";
            this.btnDeleteFromModel.UseVisualStyleBackColor = true;
            this.btnDeleteFromModel.Click += new System.EventHandler(this.btnDeleteFromModel_Click);
            // 
            // lvModelMaterials
            // 
            this.lvModelMaterials.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvModelMaterials.DisableMouse = false;
            this.lvModelMaterials.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lvModelMaterials.FullRowSelect = true;
            this.lvModelMaterials.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvModelMaterials.HideSelection = false;
            this.lvModelMaterials.Location = new System.Drawing.Point(7, 51);
            this.lvModelMaterials.MultiSelect = false;
            this.lvModelMaterials.Name = "lvModelMaterials";
            this.lvModelMaterials.ShowGroups = false;
            this.lvModelMaterials.Size = new System.Drawing.Size(192, 370);
            this.lvModelMaterials.TabIndex = 1;
            this.lvModelMaterials.UseCompatibleStateImageBehavior = false;
            this.lvModelMaterials.View = System.Windows.Forms.View.List;
            this.lvModelMaterials.Enter += new System.EventHandler(this.lvModelMaterials_Enter);
            this.lvModelMaterials.Leave += new System.EventHandler(this.lvModelMaterials_Leave);
            this.lvModelMaterials.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lvModelMaterials_MouseDown);
            this.lvModelMaterials.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvModelMaterials_MouseUp);
            // 
            // btnRename
            // 
            this.btnRename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRename.Location = new System.Drawing.Point(199, 428);
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new System.Drawing.Size(75, 23);
            this.btnRename.TabIndex = 0;
            this.btnRename.Text = "Rename";
            this.btnRename.UseVisualStyleBackColor = true;
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
            // 
            // tbCategoryName
            // 
            this.tbCategoryName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCategoryName.Location = new System.Drawing.Point(6, 428);
            this.tbCategoryName.Name = "tbCategoryName";
            this.tbCategoryName.Size = new System.Drawing.Size(187, 23);
            this.tbCategoryName.TabIndex = 2;
            this.tbCategoryName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbCategoryName_KeyDown);
            this.tbCategoryName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbCategoryName_KeyUp);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.btnDeleteFromLibrary);
            this.groupBox3.Controls.Add(this.btnAddCategory);
            this.groupBox3.Controls.Add(this.btvLibrary);
            this.groupBox3.Controls.Add(this.tbCategoryName);
            this.groupBox3.Controls.Add(this.btnRename);
            this.groupBox3.Location = new System.Drawing.Point(14, 14);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(280, 456);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Library materials";
            // 
            // btnDeleteFromLibrary
            // 
            this.btnDeleteFromLibrary.Location = new System.Drawing.Point(199, 22);
            this.btnDeleteFromLibrary.Name = "btnDeleteFromLibrary";
            this.btnDeleteFromLibrary.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteFromLibrary.TabIndex = 9;
            this.btnDeleteFromLibrary.Text = "Delete";
            this.btnDeleteFromLibrary.UseVisualStyleBackColor = true;
            this.btnDeleteFromLibrary.Click += new System.EventHandler(this.btnDeleteFromLibrary_Click);
            // 
            // btnAddCategory
            // 
            this.btnAddCategory.Location = new System.Drawing.Point(6, 22);
            this.btnAddCategory.Name = "btnAddCategory";
            this.btnAddCategory.Size = new System.Drawing.Size(87, 23);
            this.btnAddCategory.TabIndex = 5;
            this.btnAddCategory.Text = "Add category";
            this.btnAddCategory.UseVisualStyleBackColor = true;
            this.btnAddCategory.Click += new System.EventHandler(this.btnAddCategory_Click);
            // 
            // btvLibrary
            // 
            this.btvLibrary.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btvLibrary.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.btvLibrary.HighLightDeselectedColor = System.Drawing.SystemColors.Highlight;
            this.btvLibrary.HighLightDeselectedTextColor = System.Drawing.SystemColors.ScrollBar;
            this.btvLibrary.HighlightErrorColor = System.Drawing.Color.Red;
            this.btvLibrary.HighLightSelectedColor = System.Drawing.SystemColors.Highlight;
            this.btvLibrary.HighLightSelectedTextColor = System.Drawing.SystemColors.HighlightText;
            this.btvLibrary.Location = new System.Drawing.Point(6, 51);
            this.btvLibrary.Name = "btvLibrary";
            treeNode1.Name = "Materials";
            treeNode1.Text = "Materials";
            this.btvLibrary.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.btvLibrary.Size = new System.Drawing.Size(268, 370);
            this.btvLibrary.TabIndex = 4;
            this.btvLibrary.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.btvLibrary_AfterSelect);
            this.btvLibrary.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.btvLibrary_MouseDoubleClick);
            this.btvLibrary.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btvLibrary_MouseDown);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnOK.Location = new System.Drawing.Point(373, 476);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 12;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnCancel.Location = new System.Drawing.Point(454, 476);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnSave.Location = new System.Drawing.Point(20, 476);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 13;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCopyToLibrary
            // 
            this.btnCopyToLibrary.Image = global::PrePoMax.Properties.Resources.Left_arrow;
            this.btnCopyToLibrary.Location = new System.Drawing.Point(300, 95);
            this.btnCopyToLibrary.Name = "btnCopyToLibrary";
            this.btnCopyToLibrary.Size = new System.Drawing.Size(24, 24);
            this.btnCopyToLibrary.TabIndex = 10;
            this.ttText.SetToolTip(this.btnCopyToLibrary, "Copy the selected material model from the FE model to the selected library catego" +
        "ry");
            this.btnCopyToLibrary.UseVisualStyleBackColor = true;
            this.btnCopyToLibrary.Click += new System.EventHandler(this.btnCopyToLibrary_Click);
            // 
            // btnCopyToModel
            // 
            this.btnCopyToModel.Image = global::PrePoMax.Properties.Resources.Right_arrow;
            this.btnCopyToModel.Location = new System.Drawing.Point(300, 65);
            this.btnCopyToModel.Name = "btnCopyToModel";
            this.btnCopyToModel.Size = new System.Drawing.Size(24, 24);
            this.btnCopyToModel.TabIndex = 8;
            this.ttText.SetToolTip(this.btnCopyToModel, "Copy the selected material model from the library to the FE model");
            this.btnCopyToModel.UseVisualStyleBackColor = true;
            this.btnCopyToModel.Click += new System.EventHandler(this.btnCopyToModel_Click);
            // 
            // cbPreview
            // 
            this.cbPreview.AutoSize = true;
            this.cbPreview.Location = new System.Drawing.Point(337, 444);
            this.cbPreview.Name = "cbPreview";
            this.cbPreview.Size = new System.Drawing.Size(169, 19);
            this.cbPreview.TabIndex = 14;
            this.cbPreview.Text = "Preview material properties";
            this.cbPreview.UseVisualStyleBackColor = true;
            this.cbPreview.CheckedChanged += new System.EventHandler(this.cbPreview_CheckedChanged);
            // 
            // FrmMaterialLibrary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(554, 511);
            this.Controls.Add(this.cbPreview);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCopyToLibrary);
            this.Controls.Add(this.btnCopyToModel);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(540, 550);
            this.Name = "FrmMaterialLibrary";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Material Library";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMaterialLibrary_FormClosing);
            this.Load += new System.EventHandler(this.FrmMaterialLibrary_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnRename;
        private System.Windows.Forms.TextBox tbCategoryName;
        private System.Windows.Forms.GroupBox groupBox3;
        private UserControls.BufferedTreeView btvLibrary;
        private System.Windows.Forms.Button btnAddCategory;
        private System.Windows.Forms.Button btnCopyToLibrary;
        private System.Windows.Forms.Button btnCopyToModel;
        private System.Windows.Forms.Button btnDeleteFromLibrary;
        private UserControls.ListViewWithSelection lvModelMaterials;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDeleteFromModel;
        private System.Windows.Forms.ToolTip ttText;
        private System.Windows.Forms.CheckBox cbPreview;
    }
}