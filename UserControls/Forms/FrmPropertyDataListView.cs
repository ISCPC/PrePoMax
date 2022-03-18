using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserControls.Forms
{
    class FrmPropertyDataListView : FrmPropertyListView
    {
        private System.Windows.Forms.TabControl tcProperties;
        private System.Windows.Forms.TabPage tpProperties;
        private TabbedPropertyGrid tabbedPropertyGrid1;
        private System.Windows.Forms.TabPage tpDataPoints;
        private DataGridViewCopyPaste dgvData;
        private System.ComponentModel.IContainer components;

        // Constructors                                                                                                             
        public FrmPropertyDataListView()
            : this(2.0)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelRatio">Larger value means wider second column. Default = 2.0</param>
        public FrmPropertyDataListView(double labelRatio)
            : base(labelRatio)
        {
            InitializeComponent();
            //
            gbProperties.Controls.Remove(propertyGrid);
            Controls.Remove(gbProperties);
            //
            _preselectIndex = -1;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tcProperties = new System.Windows.Forms.TabControl();
            this.tpProperties = new System.Windows.Forms.TabPage();
            this.tabbedPropertyGrid1 = new UserControls.TabbedPropertyGrid();
            this.tpDataPoints = new System.Windows.Forms.TabPage();
            this.dgvData = new UserControls.DataGridViewCopyPaste();
            this.gbType.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.tcProperties.SuspendLayout();
            this.tpProperties.SuspendLayout();
            this.tpDataPoints.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.SuspendLayout();
            // 
            // tcProperties
            // 
            this.tcProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcProperties.Controls.Add(this.tpProperties);
            this.tcProperties.Controls.Add(this.tpDataPoints);
            this.tcProperties.Location = new System.Drawing.Point(50, 200);
            this.tcProperties.Margin = new System.Windows.Forms.Padding(0);
            this.tcProperties.Name = "tcProperties";
            this.tcProperties.SelectedIndex = 0;
            this.tcProperties.Size = new System.Drawing.Size(240, 200);
            this.tcProperties.TabIndex = 16;
            // 
            // tpProperties
            // 
            this.tpProperties.BackColor = System.Drawing.SystemColors.Control;
            this.tpProperties.Controls.Add(this.tabbedPropertyGrid1);
            this.tpProperties.Location = new System.Drawing.Point(4, 24);
            this.tpProperties.Name = "tpProperties";
            this.tpProperties.Padding = new System.Windows.Forms.Padding(3);
            this.tpProperties.Size = new System.Drawing.Size(232, 237);
            this.tpProperties.TabIndex = 0;
            this.tpProperties.Text = "Properties";
            // 
            // tabbedPropertyGrid1
            // 
            this.tabbedPropertyGrid1.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.tabbedPropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabbedPropertyGrid1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tabbedPropertyGrid1.LineColor = System.Drawing.SystemColors.Control;
            this.tabbedPropertyGrid1.Location = new System.Drawing.Point(3, 3);
            this.tabbedPropertyGrid1.Name = "tabbedPropertyGrid1";
            this.tabbedPropertyGrid1.ReadOnly = false;
            this.tabbedPropertyGrid1.Size = new System.Drawing.Size(226, 231);
            this.tabbedPropertyGrid1.TabIndex = 6;
            this.tabbedPropertyGrid1.ToolbarVisible = false;
            // 
            // tpDataPoints
            // 
            this.tpDataPoints.BackColor = System.Drawing.SystemColors.Control;
            this.tpDataPoints.Controls.Add(this.dgvData);
            this.tpDataPoints.Location = new System.Drawing.Point(4, 24);
            this.tpDataPoints.Name = "tpDataPoints";
            this.tpDataPoints.Padding = new System.Windows.Forms.Padding(3);
            this.tpDataPoints.Size = new System.Drawing.Size(435, 237);
            this.tpDataPoints.TabIndex = 1;
            this.tpDataPoints.Text = "Data points";
            // 
            // dgvData
            // 
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvData.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvData.EnableCutMenu = true;
            this.dgvData.EnablePasteMenu = true;
            this.dgvData.Location = new System.Drawing.Point(3, 3);
            this.dgvData.Name = "dgvData";
            this.dgvData.Size = new System.Drawing.Size(429, 231);
            this.dgvData.StartPlotAtZero = false;
            this.dgvData.TabIndex = 0;
            this.dgvData.XColIndex = 0;
            // 
            // FrmPropertyDataListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 461);
            this.Controls.Add(this.tcProperties);
            this.Name = "FrmPropertyDataListView";
            this.Controls.SetChildIndex(this.gbProperties, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnOkAddNew, 0);
            this.Controls.SetChildIndex(this.gbType, 0);
            this.Controls.SetChildIndex(this.tcProperties, 0);
            this.gbType.ResumeLayout(false);
            this.gbProperties.ResumeLayout(false);
            this.tcProperties.ResumeLayout(false);
            this.tpProperties.ResumeLayout(false);
            this.tpDataPoints.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
