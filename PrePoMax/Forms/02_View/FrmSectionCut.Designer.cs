namespace PrePoMax
{
    partial class FrmSectionCut
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
            this.lPosition = new System.Windows.Forms.Label();
            this.rbZ = new System.Windows.Forms.RadioButton();
            this.rbY = new System.Windows.Forms.RadioButton();
            this.rbX = new System.Windows.Forms.RadioButton();
            this.ntbPosition = new UserControls.NumericTextBox();
            this.hsbPosition = new System.Windows.Forms.HScrollBar();
            this.btnClose = new System.Windows.Forms.Button();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.cbReverse = new System.Windows.Forms.CheckBox();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.cbReverse);
            this.groupBox3.Controls.Add(this.lPosition);
            this.groupBox3.Controls.Add(this.rbZ);
            this.groupBox3.Controls.Add(this.rbY);
            this.groupBox3.Controls.Add(this.rbX);
            this.groupBox3.Controls.Add(this.ntbPosition);
            this.groupBox3.Controls.Add(this.hsbPosition);
            this.groupBox3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(330, 73);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Data";
            // 
            // lPosition
            // 
            this.lPosition.AutoSize = true;
            this.lPosition.Location = new System.Drawing.Point(252, 24);
            this.lPosition.Name = "lPosition";
            this.lPosition.Size = new System.Drawing.Size(50, 15);
            this.lPosition.TabIndex = 6;
            this.lPosition.Text = "Position";
            // 
            // rbZ
            // 
            this.rbZ.AutoSize = true;
            this.rbZ.Location = new System.Drawing.Point(86, 22);
            this.rbZ.Name = "rbZ";
            this.rbZ.Size = new System.Drawing.Size(32, 19);
            this.rbZ.TabIndex = 5;
            this.rbZ.Text = "Z";
            this.rbZ.UseVisualStyleBackColor = true;
            this.rbZ.CheckedChanged += new System.EventHandler(this.rbAxis_CheckedChanged);
            // 
            // rbY
            // 
            this.rbY.AutoSize = true;
            this.rbY.Location = new System.Drawing.Point(47, 22);
            this.rbY.Name = "rbY";
            this.rbY.Size = new System.Drawing.Size(32, 19);
            this.rbY.TabIndex = 4;
            this.rbY.Text = "Y";
            this.rbY.UseVisualStyleBackColor = true;
            this.rbY.CheckedChanged += new System.EventHandler(this.rbAxis_CheckedChanged);
            // 
            // rbX
            // 
            this.rbX.AutoSize = true;
            this.rbX.Checked = true;
            this.rbX.Location = new System.Drawing.Point(9, 22);
            this.rbX.Name = "rbX";
            this.rbX.Size = new System.Drawing.Size(32, 19);
            this.rbX.TabIndex = 3;
            this.rbX.TabStop = true;
            this.rbX.Text = "X";
            this.rbX.UseVisualStyleBackColor = true;
            this.rbX.CheckedChanged += new System.EventHandler(this.rbAxis_CheckedChanged);
            // 
            // ntbPosition
            // 
            this.ntbPosition.Location = new System.Drawing.Point(255, 44);
            this.ntbPosition.Name = "ntbPosition";
            this.ntbPosition.Size = new System.Drawing.Size(69, 23);
            this.ntbPosition.TabIndex = 2;
            // 
            // hsbPosition
            // 
            this.hsbPosition.LargeChange = 1;
            this.hsbPosition.Location = new System.Drawing.Point(9, 47);
            this.hsbPosition.Maximum = 1000;
            this.hsbPosition.Name = "hsbPosition";
            this.hsbPosition.Size = new System.Drawing.Size(237, 18);
            this.hsbPosition.TabIndex = 0;
            this.hsbPosition.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hsbPosition_Scroll);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(267, 91);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 14;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // timerUpdate
            // 
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // cbReverse
            // 
            this.cbReverse.AutoSize = true;
            this.cbReverse.Location = new System.Drawing.Point(133, 23);
            this.cbReverse.Name = "cbReverse";
            this.cbReverse.Size = new System.Drawing.Size(107, 19);
            this.cbReverse.TabIndex = 7;
            this.cbReverse.Text = "Reverse normal";
            this.cbReverse.UseVisualStyleBackColor = true;
            this.cbReverse.CheckedChanged += new System.EventHandler(this.cbReverse_CheckedChanged);
            // 
            // FrmSectionCut
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 126);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.groupBox3);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSectionCut";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Section Cut";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmSectionCut_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.FrmSectionCut_VisibleChanged);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnClose;
        private UserControls.NumericTextBox ntbPosition;
        private System.Windows.Forms.HScrollBar hsbPosition;
        private System.Windows.Forms.Timer timerUpdate;
        private System.Windows.Forms.Label lPosition;
        private System.Windows.Forms.RadioButton rbZ;
        private System.Windows.Forms.RadioButton rbY;
        private System.Windows.Forms.RadioButton rbX;
        private System.Windows.Forms.CheckBox cbReverse;
    }
}