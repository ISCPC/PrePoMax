
namespace UserControls
{
    partial class SearchTextBox
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
            this.lBorder = new System.Windows.Forms.Label();
            this.pBackground = new System.Windows.Forms.Panel();
            this.btnClear = new System.Windows.Forms.Button();
            this.lIcon = new System.Windows.Forms.Label();
            this.tbSearchBox = new UserControls.ResizableTextBox();
            this.pBackground.SuspendLayout();
            this.SuspendLayout();
            // 
            // lBorder
            // 
            this.lBorder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lBorder.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.lBorder.Location = new System.Drawing.Point(0, 0);
            this.lBorder.Name = "lBorder";
            this.lBorder.Size = new System.Drawing.Size(200, 22);
            this.lBorder.TabIndex = 9;
            // 
            // pBackground
            // 
            this.pBackground.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pBackground.BackColor = System.Drawing.SystemColors.Window;
            this.pBackground.Controls.Add(this.btnClear);
            this.pBackground.Controls.Add(this.lIcon);
            this.pBackground.Controls.Add(this.tbSearchBox);
            this.pBackground.Location = new System.Drawing.Point(1, 1);
            this.pBackground.Name = "pBackground";
            this.pBackground.Size = new System.Drawing.Size(198, 20);
            this.pBackground.TabIndex = 10;
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Image = global::UserControls.Properties.Resources.Remove;
            this.btnClear.Location = new System.Drawing.Point(176, -2);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(24, 24);
            this.btnClear.TabIndex = 11;
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Visible = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lIcon
            // 
            this.lIcon.BackColor = System.Drawing.SystemColors.Window;
            this.lIcon.Image = global::UserControls.Properties.Resources.Search;
            this.lIcon.Location = new System.Drawing.Point(-1, 0);
            this.lIcon.Name = "lIcon";
            this.lIcon.Size = new System.Drawing.Size(20, 20);
            this.lIcon.TabIndex = 9;
            // 
            // tbSearchBox
            // 
            this.tbSearchBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSearchBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbSearchBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSearchBox.Location = new System.Drawing.Point(21, 2);
            this.tbSearchBox.Name = "tbSearchBox";
            this.tbSearchBox.Size = new System.Drawing.Size(149, 18);
            this.tbSearchBox.TabIndex = 10;
            this.tbSearchBox.EnabledChanged += new System.EventHandler(this.tbSearchBox_EnabledChanged);
            this.tbSearchBox.TextChanged += new System.EventHandler(this.tbSearchBox_TextChanged);
            // 
            // SearchTextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pBackground);
            this.Controls.Add(this.lBorder);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Name = "SearchTextBox";
            this.Size = new System.Drawing.Size(200, 22);
            this.pBackground.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lBorder;
        private System.Windows.Forms.Panel pBackground;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lIcon;
        private ResizableTextBox tbSearchBox;
    }
}
