namespace PrePoMax
{
    partial class FrmSelectItemSet
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAddId = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.rbSurface = new System.Windows.Forms.RadioButton();
            this.rbEdgeAngle = new System.Windows.Forms.RadioButton();
            this.btnSubtractId = new System.Windows.Forms.Button();
            this.rbId = new System.Windows.Forms.RadioButton();
            this.rbSurfaceAngle = new System.Windows.Forms.RadioButton();
            this.rbPart = new System.Windows.Forms.RadioButton();
            this.rbElement = new System.Windows.Forms.RadioButton();
            this.rbNode = new System.Windows.Forms.RadioButton();
            this.btnClearSelection = new System.Windows.Forms.Button();
            this.btnInvertSelection = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnUndoSelection = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.rbEdge = new System.Windows.Forms.RadioButton();
            this.tbEdgeAngle = new UserControls.NumericTextBox();
            this.tbId = new UserControls.NumericTextBox();
            this.tbSurfaceAngle = new UserControls.NumericTextBox();
            this.groupBox5.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.Location = new System.Drawing.Point(204, 298);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 27);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAddId
            // 
            this.btnAddId.Enabled = false;
            this.btnAddId.Location = new System.Drawing.Point(60, 222);
            this.btnAddId.Name = "btnAddId";
            this.btnAddId.Size = new System.Drawing.Size(49, 23);
            this.btnAddId.TabIndex = 2;
            this.btnAddId.Text = "Add";
            this.btnAddId.UseVisualStyleBackColor = true;
            this.btnAddId.Click += new System.EventHandler(this.btnAddId_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox5.Controls.Add(this.rbEdge);
            this.groupBox5.Controls.Add(this.rbSurface);
            this.groupBox5.Controls.Add(this.tbEdgeAngle);
            this.groupBox5.Controls.Add(this.rbEdgeAngle);
            this.groupBox5.Controls.Add(this.btnSubtractId);
            this.groupBox5.Controls.Add(this.btnAddId);
            this.groupBox5.Controls.Add(this.rbId);
            this.groupBox5.Controls.Add(this.tbId);
            this.groupBox5.Controls.Add(this.tbSurfaceAngle);
            this.groupBox5.Controls.Add(this.rbSurfaceAngle);
            this.groupBox5.Controls.Add(this.rbPart);
            this.groupBox5.Controls.Add(this.rbElement);
            this.groupBox5.Controls.Add(this.rbNode);
            this.groupBox5.Location = new System.Drawing.Point(6, 22);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(181, 252);
            this.groupBox5.TabIndex = 8;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Select by";
            // 
            // rbSurface
            // 
            this.rbSurface.AutoSize = true;
            this.rbSurface.Location = new System.Drawing.Point(6, 97);
            this.rbSurface.Name = "rbSurface";
            this.rbSurface.Size = new System.Drawing.Size(64, 19);
            this.rbSurface.TabIndex = 11;
            this.rbSurface.Text = "Surface";
            this.rbSurface.UseVisualStyleBackColor = true;
            this.rbSurface.CheckedChanged += new System.EventHandler(this.rbSelectBy_CheckedChanged);
            // 
            // rbEdgeAngle
            // 
            this.rbEdgeAngle.AutoSize = true;
            this.rbEdgeAngle.Location = new System.Drawing.Point(7, 147);
            this.rbEdgeAngle.Name = "rbEdgeAngle";
            this.rbEdgeAngle.Size = new System.Drawing.Size(114, 19);
            this.rbEdgeAngle.TabIndex = 9;
            this.rbEdgeAngle.Text = "Edge angle [deg]";
            this.rbEdgeAngle.UseVisualStyleBackColor = true;
            this.rbEdgeAngle.CheckedChanged += new System.EventHandler(this.rbSelectBy_CheckedChanged);
            // 
            // btnSubtractId
            // 
            this.btnSubtractId.Enabled = false;
            this.btnSubtractId.Location = new System.Drawing.Point(115, 222);
            this.btnSubtractId.Name = "btnSubtractId";
            this.btnSubtractId.Size = new System.Drawing.Size(61, 23);
            this.btnSubtractId.TabIndex = 8;
            this.btnSubtractId.Text = "Subtract";
            this.btnSubtractId.UseVisualStyleBackColor = true;
            this.btnSubtractId.Click += new System.EventHandler(this.btnSubtractId_Click);
            // 
            // rbId
            // 
            this.rbId.AutoSize = true;
            this.rbId.Location = new System.Drawing.Point(7, 197);
            this.rbId.Name = "rbId";
            this.rbId.Size = new System.Drawing.Size(36, 19);
            this.rbId.TabIndex = 7;
            this.rbId.Text = "ID";
            this.rbId.UseVisualStyleBackColor = true;
            this.rbId.CheckedChanged += new System.EventHandler(this.rbSelectBy_CheckedChanged);
            // 
            // rbSurfaceAngle
            // 
            this.rbSurfaceAngle.AutoSize = true;
            this.rbSurfaceAngle.Location = new System.Drawing.Point(7, 172);
            this.rbSurfaceAngle.Name = "rbSurfaceAngle";
            this.rbSurfaceAngle.Size = new System.Drawing.Size(112, 19);
            this.rbSurfaceAngle.TabIndex = 4;
            this.rbSurfaceAngle.Text = "Face angle [deg]";
            this.rbSurfaceAngle.UseVisualStyleBackColor = true;
            this.rbSurfaceAngle.CheckedChanged += new System.EventHandler(this.rbSelectBy_CheckedChanged);
            // 
            // rbPart
            // 
            this.rbPart.AutoSize = true;
            this.rbPart.Location = new System.Drawing.Point(7, 122);
            this.rbPart.Name = "rbPart";
            this.rbPart.Size = new System.Drawing.Size(46, 19);
            this.rbPart.TabIndex = 3;
            this.rbPart.Text = "Part";
            this.rbPart.UseVisualStyleBackColor = true;
            this.rbPart.CheckedChanged += new System.EventHandler(this.rbSelectBy_CheckedChanged);
            // 
            // rbElement
            // 
            this.rbElement.AutoSize = true;
            this.rbElement.Location = new System.Drawing.Point(6, 47);
            this.rbElement.Name = "rbElement";
            this.rbElement.Size = new System.Drawing.Size(68, 19);
            this.rbElement.TabIndex = 2;
            this.rbElement.Text = "Element";
            this.rbElement.UseVisualStyleBackColor = true;
            this.rbElement.CheckedChanged += new System.EventHandler(this.rbSelectBy_CheckedChanged);
            // 
            // rbNode
            // 
            this.rbNode.AutoSize = true;
            this.rbNode.Checked = true;
            this.rbNode.Location = new System.Drawing.Point(7, 22);
            this.rbNode.Name = "rbNode";
            this.rbNode.Size = new System.Drawing.Size(54, 19);
            this.rbNode.TabIndex = 1;
            this.rbNode.TabStop = true;
            this.rbNode.Text = "Node";
            this.rbNode.UseVisualStyleBackColor = true;
            this.rbNode.CheckedChanged += new System.EventHandler(this.rbSelectBy_CheckedChanged);
            // 
            // btnClearSelection
            // 
            this.btnClearSelection.Location = new System.Drawing.Point(192, 119);
            this.btnClearSelection.Name = "btnClearSelection";
            this.btnClearSelection.Size = new System.Drawing.Size(87, 24);
            this.btnClearSelection.TabIndex = 9;
            this.btnClearSelection.Text = "Clear";
            this.btnClearSelection.UseVisualStyleBackColor = true;
            this.btnClearSelection.Click += new System.EventHandler(this.btnClearSelection_Click);
            // 
            // btnInvertSelection
            // 
            this.btnInvertSelection.Location = new System.Drawing.Point(192, 59);
            this.btnInvertSelection.Name = "btnInvertSelection";
            this.btnInvertSelection.Size = new System.Drawing.Size(87, 24);
            this.btnInvertSelection.TabIndex = 10;
            this.btnInvertSelection.Text = "Invert";
            this.btnInvertSelection.UseVisualStyleBackColor = true;
            this.btnInvertSelection.Click += new System.EventHandler(this.btnInvertSelection_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(192, 29);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(87, 24);
            this.btnSelectAll.TabIndex = 11;
            this.btnSelectAll.Text = "All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.btnUndoSelection);
            this.groupBox3.Controls.Add(this.btnSelectAll);
            this.groupBox3.Controls.Add(this.btnInvertSelection);
            this.groupBox3.Controls.Add(this.btnClearSelection);
            this.groupBox3.Controls.Add(this.groupBox5);
            this.groupBox3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(285, 280);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Data";
            // 
            // btnUndoSelection
            // 
            this.btnUndoSelection.Location = new System.Drawing.Point(192, 89);
            this.btnUndoSelection.Name = "btnUndoSelection";
            this.btnUndoSelection.Size = new System.Drawing.Size(87, 24);
            this.btnUndoSelection.TabIndex = 12;
            this.btnUndoSelection.Text = "Undo";
            this.btnUndoSelection.UseVisualStyleBackColor = true;
            this.btnUndoSelection.Click += new System.EventHandler(this.btnUndoSelection_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOK.Location = new System.Drawing.Point(106, 298);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(87, 27);
            this.btnOK.TabIndex = 13;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // rbEdge
            // 
            this.rbEdge.AutoSize = true;
            this.rbEdge.Location = new System.Drawing.Point(6, 72);
            this.rbEdge.Name = "rbEdge";
            this.rbEdge.Size = new System.Drawing.Size(51, 19);
            this.rbEdge.TabIndex = 12;
            this.rbEdge.Text = "Edge";
            this.rbEdge.UseVisualStyleBackColor = true;
            this.rbEdge.CheckedChanged += new System.EventHandler(this.rbSelectBy_CheckedChanged);
            // 
            // tbEdgeAngle
            // 
            this.tbEdgeAngle.Enabled = false;
            this.tbEdgeAngle.Location = new System.Drawing.Point(121, 146);
            this.tbEdgeAngle.Name = "tbEdgeAngle";
            this.tbEdgeAngle.Size = new System.Drawing.Size(55, 23);
            this.tbEdgeAngle.TabIndex = 10;
            this.tbEdgeAngle.Text = "10";
            this.tbEdgeAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbEdgeAngle.TextChanged += new System.EventHandler(this.tbEdgeAngle_TextChanged);
            // 
            // tbId
            // 
            this.tbId.Enabled = false;
            this.tbId.Location = new System.Drawing.Point(60, 196);
            this.tbId.Name = "tbId";
            this.tbId.Size = new System.Drawing.Size(116, 23);
            this.tbId.TabIndex = 1;
            this.tbId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbSurfaceAngle
            // 
            this.tbSurfaceAngle.Enabled = false;
            this.tbSurfaceAngle.Location = new System.Drawing.Point(121, 172);
            this.tbSurfaceAngle.Name = "tbSurfaceAngle";
            this.tbSurfaceAngle.Size = new System.Drawing.Size(55, 23);
            this.tbSurfaceAngle.TabIndex = 5;
            this.tbSurfaceAngle.Text = "10";
            this.tbSurfaceAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbSurfaceAngle.TextChanged += new System.EventHandler(this.tbSurfaceAngle_TextChanged);
            // 
            // FrmSelectItemSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(311, 331);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSelectItemSet";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Set selection";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmSelectItemSet_FormClosing);
            this.Shown += new System.EventHandler(this.FrmSelectItemSet_Shown);
            this.VisibleChanged += new System.EventHandler(this.FrmSelectItemSet_VisibleChanged);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAddId;
        private UserControls.NumericTextBox tbId;
        private System.Windows.Forms.GroupBox groupBox5;
        private UserControls.NumericTextBox tbSurfaceAngle;
        private System.Windows.Forms.RadioButton rbSurfaceAngle;
        private System.Windows.Forms.RadioButton rbPart;
        private System.Windows.Forms.RadioButton rbElement;
        private System.Windows.Forms.RadioButton rbNode;
        private System.Windows.Forms.Button btnClearSelection;
        private System.Windows.Forms.Button btnInvertSelection;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnUndoSelection;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.RadioButton rbId;
        private System.Windows.Forms.Button btnSubtractId;
        private UserControls.NumericTextBox tbEdgeAngle;
        private System.Windows.Forms.RadioButton rbEdgeAngle;
        private System.Windows.Forms.RadioButton rbSurface;
        private System.Windows.Forms.RadioButton rbEdge;
    }
}