namespace UserControls
{
    partial class AnnotationEditor
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
            this.pBorder = new System.Windows.Forms.Panel();
            this.wertbData = new UserControls.AnnotationEditorRichTextBox();
            this.pBorder.SuspendLayout();
            this.SuspendLayout();
            // 
            // pBorder
            // 
            this.pBorder.BackColor = System.Drawing.SystemColors.Highlight;
            this.pBorder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pBorder.Controls.Add(this.wertbData);
            this.pBorder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pBorder.Location = new System.Drawing.Point(0, 0);
            this.pBorder.Name = "pBorder";
            this.pBorder.Size = new System.Drawing.Size(139, 69);
            this.pBorder.TabIndex = 0;
            // 
            // wertbData
            // 
            this.wertbData.Beep = false;
            this.wertbData.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.wertbData.DetectUrls = false;
            this.wertbData.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.wertbData.Location = new System.Drawing.Point(2, 2);
            this.wertbData.MaxSize = new System.Drawing.Size(0, 0);
            this.wertbData.MinSize = new System.Drawing.Size(0, 0);
            this.wertbData.Name = "wertbData";
            this.wertbData.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.wertbData.Size = new System.Drawing.Size(124, 43);
            this.wertbData.TabIndex = 6;
            this.wertbData.Text = "Edit widgets";
            this.wertbData.WordWrap = false;
            this.wertbData.SizeChanged += new System.EventHandler(this.wertbData_SizeChanged);
            // 
            // WidgetEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pBorder);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "WidgetEditor";
            this.Size = new System.Drawing.Size(139, 69);
            this.pBorder.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pBorder;
        private AnnotationEditorRichTextBox wertbData;
    }
}
