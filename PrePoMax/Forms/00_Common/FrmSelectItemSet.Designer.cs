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
            this.tbEdgeAngle = new UserControls.UnitAwareTextBox();
            this.rbEdgeAngle = new System.Windows.Forms.RadioButton();
            this.btnRemoveId = new System.Windows.Forms.Button();
            this.rbId = new System.Windows.Forms.RadioButton();
            this.tbId = new UserControls.NumericTextBox();
            this.tbSurfaceAngle = new UserControls.UnitAwareTextBox();
            this.rbSurfaceAngle = new System.Windows.Forms.RadioButton();
            this.rbPart = new System.Windows.Forms.RadioButton();
            this.rbElement = new System.Windows.Forms.RadioButton();
            this.rbNode = new System.Windows.Forms.RadioButton();
            this.rbGeometry = new System.Windows.Forms.RadioButton();
            this.btnClearSelection = new System.Windows.Forms.Button();
            this.btnInvertSelection = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.gbGeometry = new System.Windows.Forms.GroupBox();
            this.tbGeometrySurfaceAngle = new UserControls.UnitAwareTextBox();
            this.rbGeometrySurfaceAngle = new System.Windows.Forms.RadioButton();
            this.tbGeometryEdgeAngle = new UserControls.UnitAwareTextBox();
            this.rbGeometryEdgeAngle = new System.Windows.Forms.RadioButton();
            this.btnUndoSelection = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnMoreLess = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.rbGeometryPart = new System.Windows.Forms.RadioButton();
            this.gbFEMesh.SuspendLayout();
            this.gbGeometry.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(12, 198);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(60, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAddId
            // 
            this.btnAddId.Enabled = false;
            this.btnAddId.Location = new System.Drawing.Point(29, 209);
            this.btnAddId.Name = "btnAddId";
            this.btnAddId.Size = new System.Drawing.Size(60, 23);
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
            this.gbFEMesh.Controls.Add(this.btnRemoveId);
            this.gbFEMesh.Controls.Add(this.btnAddId);
            this.gbFEMesh.Controls.Add(this.rbId);
            this.gbFEMesh.Controls.Add(this.tbId);
            this.gbFEMesh.Controls.Add(this.tbSurfaceAngle);
            this.gbFEMesh.Controls.Add(this.rbSurfaceAngle);
            this.gbFEMesh.Controls.Add(this.rbPart);
            this.gbFEMesh.Controls.Add(this.rbElement);
            this.gbFEMesh.Controls.Add(this.rbNode);
            this.gbFEMesh.Location = new System.Drawing.Point(191, 12);
            this.gbFEMesh.Name = "gbFEMesh";
            this.gbFEMesh.Size = new System.Drawing.Size(161, 238);
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
            this.tbEdgeAngle.Location = new System.Drawing.Point(96, 137);
            this.tbEdgeAngle.Name = "tbEdgeAngle";
            this.tbEdgeAngle.Size = new System.Drawing.Size(58, 23);
            this.tbEdgeAngle.TabIndex = 10;
            this.tbEdgeAngle.Text = "30";
            this.tbEdgeAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbEdgeAngle.UnitConverter = null;
            this.tbEdgeAngle.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbAngle_KeyUp);
            // 
            // rbEdgeAngle
            // 
            this.rbEdgeAngle.AutoSize = true;
            this.rbEdgeAngle.Location = new System.Drawing.Point(6, 138);
            this.rbEdgeAngle.Name = "rbEdgeAngle";
            this.rbEdgeAngle.Size = new System.Drawing.Size(83, 19);
            this.rbEdgeAngle.TabIndex = 9;
            this.rbEdgeAngle.Text = "Edge angle";
            this.rbEdgeAngle.UseVisualStyleBackColor = true;
            this.rbEdgeAngle.CheckedChanged += new System.EventHandler(this.rbSelectBy_CheckedChanged);
            // 
            // btnRemoveId
            // 
            this.btnRemoveId.Enabled = false;
            this.btnRemoveId.Location = new System.Drawing.Point(95, 209);
            this.btnRemoveId.Name = "btnRemoveId";
            this.btnRemoveId.Size = new System.Drawing.Size(60, 23);
            this.btnRemoveId.TabIndex = 8;
            this.btnRemoveId.Text = "Remove";
            this.btnRemoveId.UseVisualStyleBackColor = true;
            this.btnRemoveId.Click += new System.EventHandler(this.btnRemoveId_Click);
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
            this.tbId.Location = new System.Drawing.Point(96, 185);
            this.tbId.Name = "tbId";
            this.tbId.Size = new System.Drawing.Size(58, 23);
            this.tbId.TabIndex = 1;
            this.tbId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbId.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbId_KeyDown);
            // 
            // tbSurfaceAngle
            // 
            this.tbSurfaceAngle.Enabled = false;
            this.tbSurfaceAngle.Location = new System.Drawing.Point(96, 161);
            this.tbSurfaceAngle.Name = "tbSurfaceAngle";
            this.tbSurfaceAngle.Size = new System.Drawing.Size(58, 23);
            this.tbSurfaceAngle.TabIndex = 5;
            this.tbSurfaceAngle.Text = "30";
            this.tbSurfaceAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbSurfaceAngle.UnitConverter = null;
            this.tbSurfaceAngle.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbAngle_KeyUp);
            // 
            // rbSurfaceAngle
            // 
            this.rbSurfaceAngle.AutoSize = true;
            this.rbSurfaceAngle.Location = new System.Drawing.Point(6, 162);
            this.rbSurfaceAngle.Name = "rbSurfaceAngle";
            this.rbSurfaceAngle.Size = new System.Drawing.Size(81, 19);
            this.rbSurfaceAngle.TabIndex = 4;
            this.rbSurfaceAngle.Text = "Face angle";
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
            this.btnClearSelection.Location = new System.Drawing.Point(119, 197);
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
            this.btnInvertSelection.Location = new System.Drawing.Point(358, 57);
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
            this.btnSelectAll.Location = new System.Drawing.Point(358, 28);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(60, 23);
            this.btnSelectAll.TabIndex = 11;
            this.btnSelectAll.Text = "All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // gbGeometry
            // 
            this.gbGeometry.Controls.Add(this.rbGeometryPart);
            this.gbGeometry.Controls.Add(this.tbGeometrySurfaceAngle);
            this.gbGeometry.Controls.Add(this.rbGeometrySurfaceAngle);
            this.gbGeometry.Controls.Add(this.tbGeometryEdgeAngle);
            this.gbGeometry.Controls.Add(this.rbGeometryEdgeAngle);
            this.gbGeometry.Controls.Add(this.rbGeometry);
            this.gbGeometry.Location = new System.Drawing.Point(6, 12);
            this.gbGeometry.Name = "gbGeometry";
            this.gbGeometry.Size = new System.Drawing.Size(179, 120);
            this.gbGeometry.TabIndex = 13;
            this.gbGeometry.TabStop = false;
            this.gbGeometry.Text = "Geometry based selection";
            // 
            // tbGeometrySurfaceAngle
            // 
            this.tbGeometrySurfaceAngle.Enabled = false;
            this.tbGeometrySurfaceAngle.Location = new System.Drawing.Point(115, 91);
            this.tbGeometrySurfaceAngle.Name = "tbGeometrySurfaceAngle";
            this.tbGeometrySurfaceAngle.Size = new System.Drawing.Size(58, 23);
            this.tbGeometrySurfaceAngle.TabIndex = 17;
            this.tbGeometrySurfaceAngle.Text = "30";
            this.tbGeometrySurfaceAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbGeometrySurfaceAngle.UnitConverter = null;
            this.tbGeometrySurfaceAngle.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbAngle_KeyUp);
            // 
            // rbGeometrySurfaceAngle
            // 
            this.rbGeometrySurfaceAngle.AutoSize = true;
            this.rbGeometrySurfaceAngle.Location = new System.Drawing.Point(6, 91);
            this.rbGeometrySurfaceAngle.Name = "rbGeometrySurfaceAngle";
            this.rbGeometrySurfaceAngle.Size = new System.Drawing.Size(96, 19);
            this.rbGeometrySurfaceAngle.TabIndex = 16;
            this.rbGeometrySurfaceAngle.Text = "Surface angle";
            this.rbGeometrySurfaceAngle.UseVisualStyleBackColor = true;
            this.rbGeometrySurfaceAngle.CheckedChanged += new System.EventHandler(this.rbSelectBy_CheckedChanged);
            // 
            // tbGeometryEdgeAngle
            // 
            this.tbGeometryEdgeAngle.Enabled = false;
            this.tbGeometryEdgeAngle.Location = new System.Drawing.Point(115, 66);
            this.tbGeometryEdgeAngle.Name = "tbGeometryEdgeAngle";
            this.tbGeometryEdgeAngle.Size = new System.Drawing.Size(58, 23);
            this.tbGeometryEdgeAngle.TabIndex = 15;
            this.tbGeometryEdgeAngle.Text = "30";
            this.tbGeometryEdgeAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbGeometryEdgeAngle.UnitConverter = null;
            this.tbGeometryEdgeAngle.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbAngle_KeyUp);
            // 
            // rbGeometryEdgeAngle
            // 
            this.rbGeometryEdgeAngle.AutoSize = true;
            this.rbGeometryEdgeAngle.Location = new System.Drawing.Point(6, 67);
            this.rbGeometryEdgeAngle.Name = "rbGeometryEdgeAngle";
            this.rbGeometryEdgeAngle.Size = new System.Drawing.Size(83, 19);
            this.rbGeometryEdgeAngle.TabIndex = 14;
            this.rbGeometryEdgeAngle.Text = "Edge angle";
            this.rbGeometryEdgeAngle.UseVisualStyleBackColor = true;
            this.rbGeometryEdgeAngle.CheckedChanged += new System.EventHandler(this.rbSelectBy_CheckedChanged);
            // 
            // btnUndoSelection
            // 
            this.btnUndoSelection.Location = new System.Drawing.Point(119, 168);
            this.btnUndoSelection.Name = "btnUndoSelection";
            this.btnUndoSelection.Size = new System.Drawing.Size(60, 23);
            this.btnUndoSelection.TabIndex = 12;
            this.btnUndoSelection.Text = "Undo";
            this.btnUndoSelection.UseVisualStyleBackColor = true;
            this.btnUndoSelection.Click += new System.EventHandler(this.btnUndoSelection_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(12, 227);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(60, 23);
            this.btnOK.TabIndex = 13;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Visible = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnMoreLess
            // 
            this.btnMoreLess.Location = new System.Drawing.Point(119, 226);
            this.btnMoreLess.Name = "btnMoreLess";
            this.btnMoreLess.Size = new System.Drawing.Size(60, 23);
            this.btnMoreLess.TabIndex = 14;
            this.btnMoreLess.Text = "Less";
            this.btnMoreLess.UseVisualStyleBackColor = true;
            this.btnMoreLess.Click += new System.EventHandler(this.btnMoreLess_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.Location = new System.Drawing.Point(9, 135);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 30);
            this.label1.TabIndex = 15;
            this.label1.Text = "SHIFT - add items\r\nCTRL - remove items\r\n";
            // 
            // rbGeometryPart
            // 
            this.rbGeometryPart.AutoSize = true;
            this.rbGeometryPart.Location = new System.Drawing.Point(6, 42);
            this.rbGeometryPart.Name = "rbGeometryPart";
            this.rbGeometryPart.Size = new System.Drawing.Size(46, 19);
            this.rbGeometryPart.TabIndex = 18;
            this.rbGeometryPart.Text = "Part";
            this.rbGeometryPart.UseVisualStyleBackColor = true;
            this.rbGeometryPart.CheckedChanged += new System.EventHandler(this.rbSelectBy_CheckedChanged);
            // 
            // FrmSelectItemSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 261);
            this.Controls.Add(this.btnUndoSelection);
            this.Controls.Add(this.btnSelectAll);
            this.Controls.Add(this.btnInvertSelection);
            this.Controls.Add(this.gbGeometry);
            this.Controls.Add(this.btnMoreLess);
            this.Controls.Add(this.btnClearSelection);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.gbFEMesh);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label1);
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
            this.Move += new System.EventHandler(this.FrmSelectItemSet_Move);
            this.gbFEMesh.ResumeLayout(false);
            this.gbFEMesh.PerformLayout();
            this.gbGeometry.ResumeLayout(false);
            this.gbGeometry.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAddId;
        private UserControls.NumericTextBox tbId;
        private System.Windows.Forms.GroupBox gbFEMesh;
        private UserControls.UnitAwareTextBox tbSurfaceAngle;
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
        private System.Windows.Forms.Button btnRemoveId;
        private UserControls.UnitAwareTextBox tbEdgeAngle;
        private System.Windows.Forms.RadioButton rbEdgeAngle;
        private System.Windows.Forms.RadioButton rbSurface;
        private System.Windows.Forms.RadioButton rbEdge;
        private System.Windows.Forms.RadioButton rbGeometry;
        private System.Windows.Forms.GroupBox gbGeometry;
        private UserControls.UnitAwareTextBox tbGeometrySurfaceAngle;
        private System.Windows.Forms.RadioButton rbGeometrySurfaceAngle;
        private UserControls.UnitAwareTextBox tbGeometryEdgeAngle;
        private System.Windows.Forms.RadioButton rbGeometryEdgeAngle;
        private System.Windows.Forms.Button btnMoreLess;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbGeometryPart;
    }
}