namespace PrePoMax
{
    partial class FrmAnalyzeGeometry
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbCloseEdges = new System.Windows.Forms.CheckBox();
            this.labClosestEdges = new System.Windows.Forms.Label();
            this.tbMinEdgesDistance = new UserControls.UnitAwareTextBox();
            this.cbFacesSmaller = new System.Windows.Forms.CheckBox();
            this.cbEdgesShorter = new System.Windows.Forms.CheckBox();
            this.labSmallestFace = new System.Windows.Forms.Label();
            this.labShortestEdge = new System.Windows.Forms.Label();
            this.tbMinFaceSize = new UserControls.UnitAwareTextBox();
            this.tbMinEdgeLen = new UserControls.UnitAwareTextBox();
            this.btnShow = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.cbCloseEdges);
            this.groupBox3.Controls.Add(this.labClosestEdges);
            this.groupBox3.Controls.Add(this.tbMinEdgesDistance);
            this.groupBox3.Controls.Add(this.cbFacesSmaller);
            this.groupBox3.Controls.Add(this.cbEdgesShorter);
            this.groupBox3.Controls.Add(this.labSmallestFace);
            this.groupBox3.Controls.Add(this.labShortestEdge);
            this.groupBox3.Controls.Add(this.tbMinFaceSize);
            this.groupBox3.Controls.Add(this.tbMinEdgeLen);
            this.groupBox3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(344, 117);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Data";
            // 
            // cbCloseEdges
            // 
            this.cbCloseEdges.AutoSize = true;
            this.cbCloseEdges.Checked = true;
            this.cbCloseEdges.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCloseEdges.Location = new System.Drawing.Point(6, 55);
            this.cbCloseEdges.Name = "cbCloseEdges";
            this.cbCloseEdges.Size = new System.Drawing.Size(118, 19);
            this.cbCloseEdges.TabIndex = 20;
            this.cbCloseEdges.Text = "Edges closer than";
            this.cbCloseEdges.UseVisualStyleBackColor = true;
            // 
            // labClosestEdges
            // 
            this.labClosestEdges.AutoSize = true;
            this.labClosestEdges.Location = new System.Drawing.Point(237, 56);
            this.labClosestEdges.Name = "labClosestEdges";
            this.labClosestEdges.Size = new System.Drawing.Size(104, 15);
            this.labClosestEdges.TabIndex = 21;
            this.labClosestEdges.Text = "Model min: 0.0001";
            // 
            // tbMinEdgesDistance
            // 
            this.tbMinEdgesDistance.Location = new System.Drawing.Point(136, 53);
            this.tbMinEdgesDistance.Name = "tbMinEdgesDistance";
            this.tbMinEdgesDistance.Size = new System.Drawing.Size(75, 23);
            this.tbMinEdgesDistance.TabIndex = 19;
            this.tbMinEdgesDistance.Text = "0.1";
            this.tbMinEdgesDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbMinEdgesDistance.UnitConverter = null;
            // 
            // cbFacesSmaller
            // 
            this.cbFacesSmaller.AutoSize = true;
            this.cbFacesSmaller.Checked = true;
            this.cbFacesSmaller.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbFacesSmaller.Location = new System.Drawing.Point(6, 84);
            this.cbFacesSmaller.Name = "cbFacesSmaller";
            this.cbFacesSmaller.Size = new System.Drawing.Size(123, 19);
            this.cbFacesSmaller.TabIndex = 16;
            this.cbFacesSmaller.Text = "Faces smaller than";
            this.cbFacesSmaller.UseVisualStyleBackColor = true;
            // 
            // cbEdgesShorter
            // 
            this.cbEdgesShorter.AutoSize = true;
            this.cbEdgesShorter.Checked = true;
            this.cbEdgesShorter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbEdgesShorter.Location = new System.Drawing.Point(6, 26);
            this.cbEdgesShorter.Name = "cbEdgesShorter";
            this.cbEdgesShorter.Size = new System.Drawing.Size(124, 19);
            this.cbEdgesShorter.TabIndex = 15;
            this.cbEdgesShorter.Text = "Edges shorter than";
            this.cbEdgesShorter.UseVisualStyleBackColor = true;
            // 
            // labSmallestFace
            // 
            this.labSmallestFace.AutoSize = true;
            this.labSmallestFace.Location = new System.Drawing.Point(237, 85);
            this.labSmallestFace.Name = "labSmallestFace";
            this.labSmallestFace.Size = new System.Drawing.Size(104, 15);
            this.labSmallestFace.TabIndex = 18;
            this.labSmallestFace.Text = "Model min: 0.0001";
            // 
            // labShortestEdge
            // 
            this.labShortestEdge.AutoSize = true;
            this.labShortestEdge.Location = new System.Drawing.Point(237, 27);
            this.labShortestEdge.Name = "labShortestEdge";
            this.labShortestEdge.Size = new System.Drawing.Size(104, 15);
            this.labShortestEdge.TabIndex = 17;
            this.labShortestEdge.Text = "Model min: 0.0001";
            // 
            // tbMinFaceSize
            // 
            this.tbMinFaceSize.Location = new System.Drawing.Point(136, 82);
            this.tbMinFaceSize.Name = "tbMinFaceSize";
            this.tbMinFaceSize.Size = new System.Drawing.Size(75, 23);
            this.tbMinFaceSize.TabIndex = 15;
            this.tbMinFaceSize.Text = "0.1";
            this.tbMinFaceSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbMinFaceSize.UnitConverter = null;
            // 
            // tbMinEdgeLen
            // 
            this.tbMinEdgeLen.Location = new System.Drawing.Point(136, 24);
            this.tbMinEdgeLen.Name = "tbMinEdgeLen";
            this.tbMinEdgeLen.Size = new System.Drawing.Size(75, 23);
            this.tbMinEdgeLen.TabIndex = 10;
            this.tbMinEdgeLen.Text = "0.1";
            this.tbMinEdgeLen.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbMinEdgeLen.UnitConverter = null;
            // 
            // btnShow
            // 
            this.btnShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShow.Location = new System.Drawing.Point(200, 135);
            this.btnShow.Name = "btnShow";
            this.btnShow.Size = new System.Drawing.Size(75, 23);
            this.btnShow.TabIndex = 14;
            this.btnShow.Text = "Show";
            this.btnShow.UseVisualStyleBackColor = true;
            this.btnShow.Click += new System.EventHandler(this.btnShow_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(281, 135);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 14;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FrmAnalyzeGeometry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 170);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnShow);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmAnalyzeGeometry";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Analyze";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmAnalyzeGeometry_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.FrmAnalyzeGeometry_VisibleChanged);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnShow;
        private System.Windows.Forms.Button btnClose;
        private UserControls.UnitAwareTextBox tbMinEdgeLen;
        private System.Windows.Forms.Label labSmallestFace;
        private System.Windows.Forms.Label labShortestEdge;
        private UserControls.UnitAwareTextBox tbMinFaceSize;
        private System.Windows.Forms.CheckBox cbFacesSmaller;
        private System.Windows.Forms.CheckBox cbEdgesShorter;
        private System.Windows.Forms.CheckBox cbCloseEdges;
        private System.Windows.Forms.Label labClosestEdges;
        private UserControls.UnitAwareTextBox tbMinEdgesDistance;
    }
}