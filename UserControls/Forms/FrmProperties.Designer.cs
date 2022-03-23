namespace UserControls
{
    partial class FrmProperties
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
            this.gbProperties = new System.Windows.Forms.GroupBox();
            this.propertyGrid = new UserControls.TabEnabledPropertyGrid();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOkAddNew = new System.Windows.Forms.Button();
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbProperties
            // 
            this.gbProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbProperties.Controls.Add(this.propertyGrid);
            this.gbProperties.Location = new System.Drawing.Point(12, 6);
            this.gbProperties.Name = "gbProperties";
            this.gbProperties.Size = new System.Drawing.Size(310, 364);
            this.gbProperties.TabIndex = 0;
            this.gbProperties.TabStop = false;
            this.gbProperties.Text = "Properties";
            // 
            // propertyGrid
            // 
            this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.propertyGrid.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.propertyGrid.LineColor = System.Drawing.SystemColors.Control;
            this.propertyGrid.Location = new System.Drawing.Point(6, 22);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(298, 336);
            this.propertyGrid.TabIndex = 3;
            this.propertyGrid.ToolbarVisible = false;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            this.propertyGrid.SelectedGridItemChanged += new System.Windows.Forms.SelectedGridItemChangedEventHandler(this.propertyGrid_SelectedGridItemChanged);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(160, 376);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 13;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(241, 376);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOkAddNew
            // 
            this.btnOkAddNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOkAddNew.Location = new System.Drawing.Point(79, 376);
            this.btnOkAddNew.Name = "btnOkAddNew";
            this.btnOkAddNew.Size = new System.Drawing.Size(75, 23);
            this.btnOkAddNew.TabIndex = 14;
            this.btnOkAddNew.Text = "OK - New";
            this.btnOkAddNew.UseVisualStyleBackColor = true;
            this.btnOkAddNew.Click += new System.EventHandler(this.btnOKAddNew_Click);
            // 
            // FrmProperties
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(334, 411);
            this.Controls.Add(this.btnOkAddNew);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.gbProperties);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(350, 450);
            this.Name = "FrmProperties";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FrmProperties";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmProperties_FormClosing);
            this.EnabledChanged += new System.EventHandler(this.FrmProperties_EnabledChanged);
            this.VisibleChanged += new System.EventHandler(this.FrmProperties_VisibleChanged);
            this.gbProperties.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.GroupBox gbProperties;
        protected UserControls.TabEnabledPropertyGrid propertyGrid;
        protected System.Windows.Forms.Button btnOK;
        protected System.Windows.Forms.Button btnCancel;
        protected System.Windows.Forms.Button btnOkAddNew;
    }
}