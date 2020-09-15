namespace PrePoMax.Forms
{
    partial class FrmMonitor
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
            this.btnClose = new System.Windows.Forms.Button();
            this.tbOutput = new UserControls.AutoScrollTextBox();
            this.btnKill = new System.Windows.Forms.Button();
            this.btnResults = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpOutput = new System.Windows.Forms.TabPage();
            this.tpStatus = new System.Windows.Forms.TabPage();
            this.tbStatus = new UserControls.AutoScrollTextBox();
            this.tpCovergence = new System.Windows.Forms.TabPage();
            this.tbConvergence = new UserControls.AutoScrollTextBox();
            this.pbAnalysisStatus = new System.Windows.Forms.ProgressBar();
            this.labAnalysisStatus = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tpOutput.SuspendLayout();
            this.tpStatus.SuspendLayout();
            this.tpCovergence.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnClose.Location = new System.Drawing.Point(635, 372);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(87, 27);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // tbOutput
            // 
            this.tbOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbOutput.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbOutput.Location = new System.Drawing.Point(6, 6);
            this.tbOutput.Multiline = true;
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbOutput.Size = new System.Drawing.Size(690, 314);
            this.tbOutput.TabIndex = 0;
            // 
            // btnKill
            // 
            this.btnKill.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnKill.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnKill.Location = new System.Drawing.Point(12, 372);
            this.btnKill.Name = "btnKill";
            this.btnKill.Size = new System.Drawing.Size(87, 27);
            this.btnKill.TabIndex = 4;
            this.btnKill.Text = "Kill";
            this.btnKill.UseVisualStyleBackColor = true;
            this.btnKill.Click += new System.EventHandler(this.btnKill_Click);
            // 
            // btnResults
            // 
            this.btnResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResults.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnResults.Location = new System.Drawing.Point(542, 372);
            this.btnResults.Name = "btnResults";
            this.btnResults.Size = new System.Drawing.Size(87, 27);
            this.btnResults.TabIndex = 5;
            this.btnResults.Text = "Results";
            this.btnResults.UseVisualStyleBackColor = true;
            this.btnResults.Click += new System.EventHandler(this.btnResults_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tpOutput);
            this.tabControl1.Controls.Add(this.tpStatus);
            this.tabControl1.Controls.Add(this.tpCovergence);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(710, 354);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tpOutput
            // 
            this.tpOutput.Controls.Add(this.tbOutput);
            this.tpOutput.Location = new System.Drawing.Point(4, 24);
            this.tpOutput.Name = "tpOutput";
            this.tpOutput.Padding = new System.Windows.Forms.Padding(3);
            this.tpOutput.Size = new System.Drawing.Size(702, 326);
            this.tpOutput.TabIndex = 0;
            this.tpOutput.Text = "Output";
            this.tpOutput.UseVisualStyleBackColor = true;
            // 
            // tpStatus
            // 
            this.tpStatus.Controls.Add(this.tbStatus);
            this.tpStatus.Location = new System.Drawing.Point(4, 24);
            this.tpStatus.Name = "tpStatus";
            this.tpStatus.Padding = new System.Windows.Forms.Padding(3);
            this.tpStatus.Size = new System.Drawing.Size(702, 326);
            this.tpStatus.TabIndex = 1;
            this.tpStatus.Text = "Status";
            this.tpStatus.UseVisualStyleBackColor = true;
            // 
            // tbStatus
            // 
            this.tbStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbStatus.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbStatus.Location = new System.Drawing.Point(6, 6);
            this.tbStatus.Multiline = true;
            this.tbStatus.Name = "tbStatus";
            this.tbStatus.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbStatus.Size = new System.Drawing.Size(690, 314);
            this.tbStatus.TabIndex = 1;
            // 
            // tpCovergence
            // 
            this.tpCovergence.Controls.Add(this.tbConvergence);
            this.tpCovergence.Location = new System.Drawing.Point(4, 24);
            this.tpCovergence.Name = "tpCovergence";
            this.tpCovergence.Size = new System.Drawing.Size(702, 326);
            this.tpCovergence.TabIndex = 2;
            this.tpCovergence.Text = "Convergence";
            this.tpCovergence.UseVisualStyleBackColor = true;
            // 
            // tbConvergence
            // 
            this.tbConvergence.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbConvergence.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbConvergence.Location = new System.Drawing.Point(6, 6);
            this.tbConvergence.Multiline = true;
            this.tbConvergence.Name = "tbConvergence";
            this.tbConvergence.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbConvergence.Size = new System.Drawing.Size(690, 314);
            this.tbConvergence.TabIndex = 2;
            // 
            // pbAnalysisStatus
            // 
            this.pbAnalysisStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbAnalysisStatus.Location = new System.Drawing.Point(105, 373);
            this.pbAnalysisStatus.MarqueeAnimationSpeed = 40;
            this.pbAnalysisStatus.Name = "pbAnalysisStatus";
            this.pbAnalysisStatus.Size = new System.Drawing.Size(296, 25);
            this.pbAnalysisStatus.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbAnalysisStatus.TabIndex = 6;
            // 
            // labAnalysisStatus
            // 
            this.labAnalysisStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labAnalysisStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labAnalysisStatus.Image = global::PrePoMax.Properties.Resources.Running;
            this.labAnalysisStatus.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labAnalysisStatus.Location = new System.Drawing.Point(407, 373);
            this.labAnalysisStatus.Name = "labAnalysisStatus";
            this.labAnalysisStatus.Size = new System.Drawing.Size(129, 25);
            this.labAnalysisStatus.TabIndex = 7;
            this.labAnalysisStatus.Text = "      Running";
            this.labAnalysisStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labAnalysisStatus.UseMnemonic = false;
            // 
            // FrmMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(734, 411);
            this.Controls.Add(this.labAnalysisStatus);
            this.Controls.Add(this.pbAnalysisStatus);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnResults);
            this.Controls.Add(this.btnKill);
            this.Controls.Add(this.btnClose);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(750, 450);
            this.Name = "FrmMonitor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Monitor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMonitor_FormClosing);
            this.Shown += new System.EventHandler(this.FrmMonitor_Shown);
            this.tabControl1.ResumeLayout(false);
            this.tpOutput.ResumeLayout(false);
            this.tpOutput.PerformLayout();
            this.tpStatus.ResumeLayout(false);
            this.tpStatus.PerformLayout();
            this.tpCovergence.ResumeLayout(false);
            this.tpCovergence.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private UserControls.AutoScrollTextBox tbOutput;
        private System.Windows.Forms.Button btnKill;
        private System.Windows.Forms.Button btnResults;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpOutput;
        private System.Windows.Forms.TabPage tpStatus;
        private UserControls.AutoScrollTextBox tbStatus;
        private System.Windows.Forms.ProgressBar pbAnalysisStatus;
        private System.Windows.Forms.Label labAnalysisStatus;
        private System.Windows.Forms.TabPage tpCovergence;
        private UserControls.AutoScrollTextBox tbConvergence;
    }
}