namespace UserControls
{
    partial class AdvisorControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labTitle = new System.Windows.Forms.Label();
            this.lnklabPrevious = new System.Windows.Forms.LinkLabel();
            this.lnklabNext = new System.Windows.Forms.LinkLabel();
            this.btnClose = new System.Windows.Forms.Button();
            this.panLine = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panBackground = new System.Windows.Forms.Panel();
            this.panContents = new UserControls.DoubleBufferedPanel();
            this.linkLabel6 = new System.Windows.Forms.LinkLabel();
            this.linkLabel5 = new System.Windows.Forms.LinkLabel();
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
            this.panPage = new UserControls.DoubleBufferedPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.panBackground.SuspendLayout();
            this.panContents.SuspendLayout();
            this.panPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // labTitle
            // 
            this.labTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labTitle.BackColor = System.Drawing.SystemColors.Control;
            this.labTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.labTitle.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labTitle.Location = new System.Drawing.Point(4, 124);
            this.labTitle.Name = "labTitle";
            this.labTitle.Size = new System.Drawing.Size(222, 25);
            this.labTitle.TabIndex = 5;
            this.labTitle.Text = "Title";
            this.labTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labTitle.UseCompatibleTextRendering = true;
            // 
            // lnklabPrevious
            // 
            this.lnklabPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnklabPrevious.AutoSize = true;
            this.lnklabPrevious.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lnklabPrevious.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lnklabPrevious.Location = new System.Drawing.Point(9, 379);
            this.lnklabPrevious.Name = "lnklabPrevious";
            this.lnklabPrevious.Size = new System.Drawing.Size(52, 15);
            this.lnklabPrevious.TabIndex = 15;
            this.lnklabPrevious.TabStop = true;
            this.lnklabPrevious.Text = "Previous";
            this.lnklabPrevious.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnklabPrevious_LinkClicked);
            // 
            // lnklabNext
            // 
            this.lnklabNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lnklabNext.AutoSize = true;
            this.lnklabNext.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lnklabNext.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lnklabNext.Location = new System.Drawing.Point(187, 379);
            this.lnklabNext.Name = "lnklabNext";
            this.lnklabNext.Size = new System.Drawing.Size(32, 15);
            this.lnklabNext.TabIndex = 16;
            this.lnklabNext.TabStop = true;
            this.lnklabNext.Text = "Next";
            this.lnklabNext.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnklabNext_LinkClicked);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(112)))), ((int)(((byte)(122)))));
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(17)))), ((int)(((byte)(35)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Image = global::UserControls.Properties.Resources.Close;
            this.btnClose.Location = new System.Drawing.Point(201, 5);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(25, 25);
            this.btnClose.TabIndex = 6;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            this.btnClose.MouseEnter += new System.EventHandler(this.btnClose_MouseEnter);
            this.btnClose.MouseLeave += new System.EventHandler(this.btnClose_MouseLeave);
            // 
            // panLine
            // 
            this.panLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panLine.BackColor = System.Drawing.SystemColors.Control;
            this.panLine.Location = new System.Drawing.Point(5, 351);
            this.panLine.Name = "panLine";
            this.panLine.Size = new System.Drawing.Size(220, 25);
            this.panLine.TabIndex = 18;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.BackColor = System.Drawing.SystemColors.Control;
            this.label3.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(4, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(222, 25);
            this.label3.TabIndex = 19;
            this.label3.Text = "Contents";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label3.UseCompatibleTextRendering = true;
            // 
            // panBackground
            // 
            this.panBackground.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panBackground.Controls.Add(this.panContents);
            this.panBackground.Controls.Add(this.lnklabPrevious);
            this.panBackground.Controls.Add(this.panLine);
            this.panBackground.Controls.Add(this.labTitle);
            this.panBackground.Controls.Add(this.panPage);
            this.panBackground.Controls.Add(this.lnklabNext);
            this.panBackground.Controls.Add(this.btnClose);
            this.panBackground.Controls.Add(this.label3);
            this.panBackground.Location = new System.Drawing.Point(0, 0);
            this.panBackground.Name = "panBackground";
            this.panBackground.Size = new System.Drawing.Size(233, 403);
            this.panBackground.TabIndex = 20;
            // 
            // panContents
            // 
            this.panContents.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panContents.Controls.Add(this.linkLabel6);
            this.panContents.Controls.Add(this.linkLabel5);
            this.panContents.Controls.Add(this.linkLabel4);
            this.panContents.Location = new System.Drawing.Point(5, 33);
            this.panContents.Name = "panContents";
            this.panContents.Size = new System.Drawing.Size(221, 88);
            this.panContents.TabIndex = 18;
            // 
            // linkLabel6
            // 
            this.linkLabel6.AutoSize = true;
            this.linkLabel6.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.linkLabel6.Location = new System.Drawing.Point(4, 64);
            this.linkLabel6.Name = "linkLabel6";
            this.linkLabel6.Size = new System.Drawing.Size(60, 15);
            this.linkLabel6.TabIndex = 4;
            this.linkLabel6.TabStop = true;
            this.linkLabel6.Text = "linkLabel6";
            // 
            // linkLabel5
            // 
            this.linkLabel5.AutoSize = true;
            this.linkLabel5.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.linkLabel5.Location = new System.Drawing.Point(4, 35);
            this.linkLabel5.Name = "linkLabel5";
            this.linkLabel5.Size = new System.Drawing.Size(60, 15);
            this.linkLabel5.TabIndex = 3;
            this.linkLabel5.TabStop = true;
            this.linkLabel5.Text = "linkLabel5";
            // 
            // linkLabel4
            // 
            this.linkLabel4.AutoSize = true;
            this.linkLabel4.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.linkLabel4.Location = new System.Drawing.Point(4, 9);
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.Size = new System.Drawing.Size(60, 15);
            this.linkLabel4.TabIndex = 2;
            this.linkLabel4.TabStop = true;
            this.linkLabel4.Text = "linkLabel4";
            // 
            // panPage
            // 
            this.panPage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panPage.Controls.Add(this.label2);
            this.panPage.Controls.Add(this.label1);
            this.panPage.Controls.Add(this.linkLabel1);
            this.panPage.Controls.Add(this.linkLabel2);
            this.panPage.Controls.Add(this.linkLabel3);
            this.panPage.Location = new System.Drawing.Point(6, 152);
            this.panPage.Name = "panPage";
            this.panPage.Size = new System.Drawing.Size(220, 193);
            this.panPage.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label2.Location = new System.Drawing.Point(-1, 147);
            this.label2.MaximumSize = new System.Drawing.Size(220, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(214, 37);
            this.label2.TabIndex = 17;
            this.label2.Text = "Affronting everything discretion men now own did. Still round match we to. ";
            this.label2.UseCompatibleTextRendering = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.Location = new System.Drawing.Point(-1, 15);
            this.label1.MaximumSize = new System.Drawing.Size(220, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(214, 37);
            this.label1.TabIndex = 2;
            this.label1.Text = "Affronting everything discretion men now own did. Still round match we to. ";
            this.label1.UseCompatibleTextRendering = true;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.linkLabel1.Location = new System.Drawing.Point(3, 63);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(60, 15);
            this.linkLabel1.TabIndex = 1;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "linkLabel1";
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.linkLabel2.Location = new System.Drawing.Point(3, 88);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(60, 15);
            this.linkLabel2.TabIndex = 3;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "linkLabel2";
            // 
            // linkLabel3
            // 
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.linkLabel3.Location = new System.Drawing.Point(3, 116);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(60, 15);
            this.linkLabel3.TabIndex = 4;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "linkLabel3";
            // 
            // AdvisorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.panBackground);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "AdvisorControl";
            this.Size = new System.Drawing.Size(233, 462);
            this.Resize += new System.EventHandler(this.AdvisorControl_Resize);
            this.panBackground.ResumeLayout(false);
            this.panBackground.PerformLayout();
            this.panContents.ResumeLayout(false);
            this.panContents.PerformLayout();
            this.panPage.ResumeLayout(false);
            this.panPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.Label labTitle;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.LinkLabel lnklabPrevious;
        private System.Windows.Forms.LinkLabel lnklabNext;
        private DoubleBufferedPanel panPage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panLine;
        private System.Windows.Forms.Label label3;
        private DoubleBufferedPanel panContents;
        private System.Windows.Forms.LinkLabel linkLabel6;
        private System.Windows.Forms.LinkLabel linkLabel5;
        private System.Windows.Forms.LinkLabel linkLabel4;
        private System.Windows.Forms.Panel panBackground;
    }
}
