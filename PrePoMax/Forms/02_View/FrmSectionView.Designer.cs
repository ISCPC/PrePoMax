namespace PrePoMax.Forms
{
    partial class FrmSectionView
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.hsbPosition = new System.Windows.Forms.HScrollBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.propertyGrid = new UserControls.TabEnabledPropertyGrid();
            this.cmsNormal = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.xNormalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yNormalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zDirectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.reverseDirectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCancel = new System.Windows.Forms.Button();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.btnOK = new System.Windows.Forms.Button();
            this.btnDisable = new System.Windows.Forms.Button();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.cmsNormal.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.groupBox1);
            this.groupBox3.Controls.Add(this.propertyGrid);
            this.groupBox3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(310, 358);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Data";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.hsbPosition);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Location = new System.Drawing.Point(6, 302);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(298, 50);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Position";
            // 
            // hsbPosition
            // 
            this.hsbPosition.LargeChange = 1;
            this.hsbPosition.Location = new System.Drawing.Point(6, 23);
            this.hsbPosition.Maximum = 1000;
            this.hsbPosition.Name = "hsbPosition";
            this.hsbPosition.Size = new System.Drawing.Size(286, 18);
            this.hsbPosition.TabIndex = 0;
            this.hsbPosition.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hsbPosition_Scroll);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(5, 22);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(288, 20);
            this.panel1.TabIndex = 7;
            // 
            // propertyGrid
            // 
            this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid.ContextMenuStrip = this.cmsNormal;
            this.propertyGrid.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.propertyGrid.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.propertyGrid.LineColor = System.Drawing.SystemColors.Control;
            this.propertyGrid.Location = new System.Drawing.Point(6, 22);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(298, 274);
            this.propertyGrid.TabIndex = 16;
            this.propertyGrid.ToolbarVisible = false;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            // 
            // cmsNormal
            // 
            this.cmsNormal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetToolStripMenuItem,
            this.toolStripMenuItem1,
            this.xNormalToolStripMenuItem,
            this.yNormalToolStripMenuItem,
            this.zDirectionToolStripMenuItem,
            this.toolStripMenuItem2,
            this.reverseDirectionToolStripMenuItem});
            this.cmsNormal.Name = "cmsNormal";
            this.cmsNormal.Size = new System.Drawing.Size(206, 126);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.resetToolStripMenuItem.Text = "Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(202, 6);
            // 
            // xNormalToolStripMenuItem
            // 
            this.xNormalToolStripMenuItem.Name = "xNormalToolStripMenuItem";
            this.xNormalToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.xNormalToolStripMenuItem.Text = "Normal direction: X";
            this.xNormalToolStripMenuItem.Click += new System.EventHandler(this.xDirectionToolStripMenuItem_Click);
            // 
            // yNormalToolStripMenuItem
            // 
            this.yNormalToolStripMenuItem.Name = "yNormalToolStripMenuItem";
            this.yNormalToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.yNormalToolStripMenuItem.Text = "Normal direction: Y";
            this.yNormalToolStripMenuItem.Click += new System.EventHandler(this.yDirectionToolStripMenuItem_Click);
            // 
            // zDirectionToolStripMenuItem
            // 
            this.zDirectionToolStripMenuItem.Name = "zDirectionToolStripMenuItem";
            this.zDirectionToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.zDirectionToolStripMenuItem.Text = "Normal direction: Z";
            this.zDirectionToolStripMenuItem.Click += new System.EventHandler(this.zDirectionToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(202, 6);
            // 
            // reverseDirectionToolStripMenuItem
            // 
            this.reverseDirectionToolStripMenuItem.Name = "reverseDirectionToolStripMenuItem";
            this.reverseDirectionToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.reverseDirectionToolStripMenuItem.Text = "Reverse normal direction";
            this.reverseDirectionToolStripMenuItem.Click += new System.EventHandler(this.reverseDirectionToolStripMenuItem_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(247, 376);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // timerUpdate
            // 
            this.timerUpdate.Interval = 20;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOK.Location = new System.Drawing.Point(85, 376);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 15;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnDisable
            // 
            this.btnDisable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDisable.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDisable.Location = new System.Drawing.Point(166, 376);
            this.btnDisable.Name = "btnDisable";
            this.btnDisable.Size = new System.Drawing.Size(75, 23);
            this.btnDisable.TabIndex = 16;
            this.btnDisable.Text = "Disable";
            this.btnDisable.UseVisualStyleBackColor = true;
            this.btnDisable.Click += new System.EventHandler(this.btnDisable_Click);
            // 
            // FrmSectionView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(334, 411);
            this.Controls.Add(this.btnDisable);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox3);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(350, 450);
            this.Name = "FrmSectionView";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Section View";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmSectionView_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.FrmSectionView_VisibleChanged);
            this.groupBox3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.cmsNormal.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.HScrollBar hsbPosition;
        private System.Windows.Forms.Timer timerUpdate;
        private System.Windows.Forms.Button btnOK;
        protected UserControls.TabEnabledPropertyGrid propertyGrid;
        private System.Windows.Forms.ContextMenuStrip cmsNormal;
        private System.Windows.Forms.ToolStripMenuItem xNormalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem yNormalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zDirectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem reverseDirectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnDisable;
        private System.Windows.Forms.Panel panel1;
    }
}