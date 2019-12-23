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
            this.gbFEMesh = new System.Windows.Forms.GroupBox();
            this.rbEdge = new System.Windows.Forms.RadioButton();
            this.rbSurface = new System.Windows.Forms.RadioButton();
            this.tbEdgeAngle = new UserControls.NumericTextBox();
            this.rbEdgeAngle = new System.Windows.Forms.RadioButton();
            this.btnSubtractId = new System.Windows.Forms.Button();
            this.rbId = new System.Windows.Forms.RadioButton();
            this.tbId = new UserControls.NumericTextBox();
            this.tbSurfaceAngle = new UserControls.NumericTextBox();
            this.rbSurfaceAngle = new System.Windows.Forms.RadioButton();
            this.rbPart = new System.Windows.Forms.RadioButton();
            this.rbElement = new System.Windows.Forms.RadioButton();
            this.rbNode = new System.Windows.Forms.RadioButton();
            this.rbGeometry = new System.Windows.Forms.RadioButton();
            this.btnClearSelection = new System.Windows.Forms.Button();
            this.btnInvertSelection = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.gbGeometry = new System.Windows.Forms.GroupBox();
            this.tbGeometrySurfaceAngle = new UserControls.NumericTextBox();
            this.rbGeometrySurfaceAngle = new System.Windows.Forms.RadioButton();
            this.tbGeometryEdgeAngle = new UserControls.NumericTextBox();
            this.rbGeometryEdgeAngle = new System.Windows.Forms.RadioButton();
            this.btnUndoSelection = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnMoreLess = new System.Windows.Forms.Button();
            this.gbFEMesh.SuspendLayout();
            this.gbGeometry.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(82, 227);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(60, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAddId
            // 
            this.btnAddId.Enabled = false;
            this.btnAddId.Location = new System.Drawing.Point(76, 209);
            this.btnAddId.Name = "btnAddId";
            this.btnAddId.Size = new System.Drawing.Size(49, 23);
            this.btnAddId.TabIndex = 2;
            this.btnAddId.Text = "Add";
            this.btnAddId.UseVisualStyleBackColor = true;
            this.btnAddId.Click += new System.EventHandler(this.btnAddId_Click);
            // 
            // gbFEMesh
            // 
            this.gbFEMesh.Controls.Add(this.rbEdge);
            this.gbFEMesh.Controls.Add(this.rbSurface);
            this.gbFEMesh.Controls.Add(this.tbEdgeAngle);
            this.gbFEMesh.Controls.Add(this.rbEdgeAngle);
            this.gbFEMesh.Controls.Add(this.btnSubtractId);
            this.gbFEMesh.Controls.Add(this.btnAddId);
            this.gbFEMesh.Controls.Add(this.rbId);
            this.gbFEMesh.Controls.Add(this.tbId);
            this.gbFEMesh.Controls.Add(this.tbSurfaceAngle);
            this.gbFEMesh.Controls.Add(this.rbSurfaceAngle);
            this.gbFEMesh.Controls.Add(this.rbPart);
            this.gbFEMesh.Controls.Add(this.rbElement);
            this.gbFEMesh.Controls.Add(this.rbNode);
            this.gbFEMesh.Location = new System.Drawing.Point(222, 12);
            this.gbFEMesh.Name = "gbFEMesh";
            this.gbFEMesh.Size = new System.Drawing.Size(196, 238);
            this.gbFEMesh.TabIndex = 8;
            this.gbFEMesh.TabStop = false;
            this.gbFEMesh.Text = "FE mesh based selection";
            // 
            // rbEdge
            // 
            this.rbEdge.AutoSize = true;
            this.rbEdge.Location = new System.Drawing.Point(6, 66);
            this.rbEdge.Name = "rbEdge";
            this.rbEdge.Size = new System.Drawing.Size(51, 19);
            this.rbEdge.TabIndex = 12;
            this.rbEdge.Text = "Edge";
            this.rbEdge.UseVisualStyleBackColor = true;
            this.rbEdge.CheckedChanged += new System.EventHandler(this.rbSelectBy_CheckedChanged);
            // 
            // rbSurface
            // 
            this.rbSurface.AutoSize = true;
            this.rbSurface.Location = new System.Drawing.Point(6, 90);
            this.rbSurface.Name = "rbSurface";
            this.rbSurface.Size = new System.Drawing.Size(64, 19);
            this.rbSurface.TabIndex = 11;
            this.rbSurface.Text = "Surface";
            this.rbSurface.UseVisualStyleBackColor = true;
            this.rbSurface.CheckedChanged += new System.EventHandler(this.rbSelectBy_CheckedChanged);
            // 
            // tbEdgeAngle
            // 
            this.tbEdgeAngle.Enabled = false;
            this.tbEdgeAngle.Location = new System.Drawing.Point(154, 137);
            this.tbEdgeAngle.Name = "tbEdgeAngle";
            this.tbEdgeAngle.Size = new System.Drawing.Size(36, 23);
            this.tbEdgeAngle.TabIndex = 10;
            this.tbEdgeAngle.Text = "10";
            this.tbEdgeAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbEdgeAngle.TextChanged += new System.EventHandler(this.tbEdgeAngle_TextChanged);
            // 
            // rbEdgeAngle
            // 
            this.rbEdgeAngle.AutoSize = true;
            this.rbEdgeAngle.Location = new System.Drawing.Point(6, 138);
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
            this.btnSubtractId.Location = new System.Drawing.Point(131, 209);
            this.btnSubtractId.Name = "btnSubtractId";
            this.btnSubtractId.Size = new System.Drawing.Size(60, 23);
            this.btnSubtractId.TabIndex = 8;
            this.btnSubtractId.Text = "Subtract";
            this.btnSubtractId.UseVisualStyleBackColor = true;
            this.btnSubtractId.Click += new System.EventHandler(this.btnSubtractId_Click);
            // 
            // rbId
            // 
            this.rbId.AutoSize = true;
            this.rbId.Location = new System.Drawing.Point(6, 186);
            this.rbId.Name = "rbId";
            this.rbId.Size = new System.Drawing.Size(36, 19);
            this.rbId.TabIndex = 7;
            this.rbId.Text = "ID";
            this.rbId.UseVisualStyleBackColor = true;
            this.rbId.CheckedChanged += new System.EventHandler(this.rbSelectBy_CheckedChanged);
            // 
            // tbId
            // 
            this.tbId.Enabled = false;
            this.tbId.Location = new System.Drawing.Point(77, 185);
            this.tbId.Name = "tbId";
            this.tbId.Size = new System.Drawing.Size(113, 23);
            this.tbId.TabIndex = 1;
            this.tbId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbSurfaceAngle
            // 
            this.tbSurfaceAngle.Enabled = false;
            this.tbSurfaceAngle.Location = new System.Drawing.Point(154, 161);
            this.tbSurfaceAngle.Name = "tbSurfaceAngle";
            this.tbSurfaceAngle.Size = new System.Drawing.Size(36, 23);
            this.tbSurfaceAngle.TabIndex = 5;
            this.tbSurfaceAngle.Text = "10";
            this.tbSurfaceAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbSurfaceAngle.TextChanged += new System.EventHandler(this.tbSurfaceAngle_TextChanged);
            // 
            // rbSurfaceAngle
            // 
            this.rbSurfaceAngle.AutoSize = true;
            this.rbSurfaceAngle.Location = new System.Drawing.Point(6, 162);
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
            this.rbPart.Location = new System.Drawing.Point(6, 114);
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
            this.rbElement.Location = new System.Drawing.Point(6, 42);
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
            this.rbNode.Location = new System.Drawing.Point(6, 18);
            this.rbNode.Name = "rbNode";
            this.rbNode.Size = new System.Drawing.Size(54, 19);
            this.rbNode.TabIndex = 1;
            this.rbNode.Text = "Node";
            this.rbNode.UseVisualStyleBackColor = true;
            this.rbNode.CheckedChanged += new System.EventHandler(this.rbSelectBy_CheckedChanged);
            // 
            // rbGeometry
            // 
            this.rbGeometry.AutoSize = true;
            this.rbGeometry.Checked = true;
            this.rbGeometry.Location = new System.Drawing.Point(6, 18);
            this.rbGeometry.Name = "rbGeometry";
            this.rbGeometry.Size = new System.Drawing.Size(172, 19);
            this.rbGeometry.TabIndex = 13;
            this.rbGeometry.TabStop = true;
            this.rbGeometry.Text = "Surfaces, edges and vertices";
            this.rbGeometry.UseVisualStyleBackColor = true;
            this.rbGeometry.CheckedChanged += new System.EventHandler(this.rbSelectBy_CheckedChanged);
            // 
            // btnClearSelection
            // 
            this.btnClearSelection.Location = new System.Drawing.Point(148, 142);
            this.btnClearSelection.Name = "btnClearSelection";
            this.btnClearSelection.Size = new System.Drawing.Size(60, 23);
            this.btnClearSelection.TabIndex = 9;
            this.btnClearSelection.Text = "Clear";
            this.btnClearSelection.UseVisualStyleBackColor = true;
            this.btnClearSelection.Click += new System.EventHandler(this.btnClearSelection_Click);
            // 
            // btnInvertSelection
            // 
            this.btnInvertSelection.Enabled = false;
            this.btnInvertSelection.Location = new System.Drawing.Point(424, 57);
            this.btnInvertSelection.Name = "btnInvertSelection";
            this.btnInvertSelection.Size = new System.Drawing.Size(60, 23);
            this.btnInvertSelection.TabIndex = 10;
            this.btnInvertSelection.Text = "Invert";
            this.btnInvertSelection.UseVisualStyleBackColor = true;
            this.btnInvertSelection.Click += new System.EventHandler(this.btnInvertSelection_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Enabled = false;
            this.btnSelectAll.Location = new System.Drawing.Point(424, 28);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(60, 23);
            this.btnSelectAll.TabIndex = 11;
            this.btnSelectAll.Text = "All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // gbGeometry
            // 
            this.gbGeometry.Controls.Add(this.tbGeometrySurfaceAngle);
            this.gbGeometry.Controls.Add(this.rbGeometrySurfaceAngle);
            this.gbGeometry.Controls.Add(this.tbGeometryEdgeAngle);
            this.gbGeometry.Controls.Add(this.rbGeometryEdgeAngle);
            this.gbGeometry.Controls.Add(this.rbGeometry);
            this.gbGeometry.Location = new System.Drawing.Point(12, 12);
            this.gbGeometry.Name = "gbGeometry";
            this.gbGeometry.Size = new System.Drawing.Size(196, 95);
            this.gbGeometry.TabIndex = 13;
            this.gbGeometry.TabStop = false;
            this.gbGeometry.Text = "Geometry based selection";
            // 
            // tbGeometrySurfaceAngle
            // 
            this.tbGeometrySurfaceAngle.Enabled = false;
            this.tbGeometrySurfaceAngle.Location = new System.Drawing.Point(154, 65);
            this.tbGeometrySurfaceAngle.Name = "tbGeometrySurfaceAngle";
            this.tbGeometrySurfaceAngle.Size = new System.Drawing.Size(36, 23);
            this.tbGeometrySurfaceAngle.TabIndex = 17;
            this.tbGeometrySurfaceAngle.Text = "10";
            this.tbGeometrySurfaceAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbGeometrySurfaceAngle.TextChanged += new System.EventHandler(this.tbGeometrySurfaceAngle_TextChanged);
            // 
            // rbGeometrySurfaceAngle
            // 
            this.rbGeometrySurfaceAngle.AutoSize = true;
            this.rbGeometrySurfaceAngle.Location = new System.Drawing.Point(6, 66);
            this.rbGeometrySurfaceAngle.Name = "rbGeometrySurfaceAngle";
            this.rbGeometrySurfaceAngle.Size = new System.Drawing.Size(127, 19);
            this.rbGeometrySurfaceAngle.TabIndex = 16;
            this.rbGeometrySurfaceAngle.Text = "Surface angle [deg]";
            this.rbGeometrySurfaceAngle.UseVisualStyleBackColor = true;
            this.rbGeometrySurfaceAngle.CheckedChanged += new System.EventHandler(this.rbSelectBy_CheckedChanged);
            // 
            // tbGeometryEdgeAngle
            // 
            this.tbGeometryEdgeAngle.Enabled = false;
            this.tbGeometryEdgeAngle.Location = new System.Drawing.Point(154, 41);
            this.tbGeometryEdgeAngle.Name = "tbGeometryEdgeAngle";
            this.tbGeometryEdgeAngle.Size = new System.Drawing.Size(36, 23);
            this.tbGeometryEdgeAngle.TabIndex = 15;
            this.tbGeometryEdgeAngle.Text = "10";
            this.tbGeometryEdgeAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbGeometryEdgeAngle.TextChanged += new System.EventHandler(this.tbGeometryEdgeAngle_TextChanged);
            // 
            // rbGeometryEdgeAngle
            // 
            this.rbGeometryEdgeAngle.AutoSize = true;
            this.rbGeometryEdgeAngle.Location = new System.Drawing.Point(6, 42);
            this.rbGeometryEdgeAngle.Name = "rbGeometryEdgeAngle";
            this.rbGeometryEdgeAngle.Size = new System.Drawing.Size(114, 19);
            this.rbGeometryEdgeAngle.TabIndex = 14;
            this.rbGeometryEdgeAngle.Text = "Edge angle [deg]";
            this.rbGeometryEdgeAngle.UseVisualStyleBackColor = true;
            this.rbGeometryEdgeAngle.CheckedChanged += new System.EventHandler(this.rbSelectBy_CheckedChanged);
            // 
            // btnUndoSelection
            // 
            this.btnUndoSelection.Location = new System.Drawing.Point(148, 113);
            this.btnUndoSelection.Name = "btnUndoSelection";
            this.btnUndoSelection.Size = new System.Drawing.Size(60, 23);
            this.btnUndoSelection.TabIndex = 12;
            this.btnUndoSelection.Text = "Undo";
            this.btnUndoSelection.UseVisualStyleBackColor = true;
            this.btnUndoSelection.Click += new System.EventHandler(this.btnUndoSelection_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(16, 227);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(60, 23);
            this.btnOK.TabIndex = 13;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnMoreLess
            // 
            this.btnMoreLess.Location = new System.Drawing.Point(148, 227);
            this.btnMoreLess.Name = "btnMoreLess";
            this.btnMoreLess.Size = new System.Drawing.Size(60, 23);
            this.btnMoreLess.TabIndex = 14;
            this.btnMoreLess.Text = "Less";
            this.btnMoreLess.UseVisualStyleBackColor = true;
            this.btnMoreLess.Click += new System.EventHandler(this.btnMoreLess_Click);
            // 
            // FrmSelectItemSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 261);
            this.Controls.Add(this.btnUndoSelection);
            this.Controls.Add(this.btnSelectAll);
            this.Controls.Add(this.btnInvertSelection);
            this.Controls.Add(this.gbGeometry);
            this.Controls.Add(this.btnMoreLess);
            this.Controls.Add(this.btnClearSelection);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.gbFEMesh);
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
            this.VisibleChanged += new System.EventHandler(this.FrmSelectItemSet_VisibleChanged);
            this.gbFEMesh.ResumeLayout(false);
            this.gbFEMesh.PerformLayout();
            this.gbGeometry.ResumeLayout(false);
            this.gbGeometry.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAddId;
        private UserControls.NumericTextBox tbId;
        private System.Windows.Forms.GroupBox gbFEMesh;
        private UserControls.NumericTextBox tbSurfaceAngle;
        private System.Windows.Forms.RadioButton rbSurfaceAngle;
        private System.Windows.Forms.RadioButton rbPart;
        private System.Windows.Forms.RadioButton rbElement;
        private System.Windows.Forms.RadioButton rbNode;
        private System.Windows.Forms.Button btnClearSelection;
        private System.Windows.Forms.Button btnInvertSelection;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnUndoSelection;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.RadioButton rbId;
        private System.Windows.Forms.Button btnSubtractId;
        private UserControls.NumericTextBox tbEdgeAngle;
        private System.Windows.Forms.RadioButton rbEdgeAngle;
        private System.Windows.Forms.RadioButton rbSurface;
        private System.Windows.Forms.RadioButton rbEdge;
        private System.Windows.Forms.RadioButton rbGeometry;
        private System.Windows.Forms.GroupBox gbGeometry;
        private UserControls.NumericTextBox tbGeometrySurfaceAngle;
        private System.Windows.Forms.RadioButton rbGeometrySurfaceAngle;
        private UserControls.NumericTextBox tbGeometryEdgeAngle;
        private System.Windows.Forms.RadioButton rbGeometryEdgeAngle;
        private System.Windows.Forms.Button btnMoreLess;
    }
}