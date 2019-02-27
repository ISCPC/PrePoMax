using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;

namespace PrePoMax.Forms
{
    class FrmMeshingParameters : UserControls.FrmProperties
    {
        // Variables                                                                                                                
        private string _partName;
        private MeshingParameters _defaultMeshingParameters;
        private System.Windows.Forms.ContextMenuStrip cmsPropertyGrid;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.ToolStripMenuItem tsmiResetAll;
        private ViewMeshingParameters _viewMeshingParameters;


        // Properties                                                                                                               
        public MeshingParameters MeshingParameters
        {
            get { return _viewMeshingParameters.GetBase(); }
            set
            {
                _viewMeshingParameters = new ViewMeshingParameters(value.DeepClone());
                propertyGrid.SelectedObject = _viewMeshingParameters;
                propertyGrid.Select();
            }
        }
        public MeshingParameters DefaultMeshingParameters { set { _defaultMeshingParameters = value; } }
        public string PartName
        {
            get { return _partName; }
            set
            {
                _partName = value;
                Text = "Edit Meshing Parameters: " + _partName;
            }
        }


        // Constructors                                                                                                             
        public FrmMeshingParameters()
        {
            InitializeComponent();

            _partName = null;
            _defaultMeshingParameters = null;
            _viewMeshingParameters = null;

            _hideOnClose = false;
            btnOkAddNew.Visible = false;
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cmsPropertyGrid = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiResetAll = new System.Windows.Forms.ToolStripMenuItem();
            this.gbProperties.SuspendLayout();
            this.cmsPropertyGrid.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyGrid
            // 
            this.propertyGrid.ContextMenuStrip = this.cmsPropertyGrid;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(160, 376);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(241, 376);
            // 
            // btnOkAddNew
            // 
            this.btnOkAddNew.Location = new System.Drawing.Point(65, 376);
            // 
            // cmsPropertyGrid
            // 
            this.cmsPropertyGrid.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiResetAll});
            this.cmsPropertyGrid.Name = "cmsPropertyGrid";
            this.cmsPropertyGrid.Size = new System.Drawing.Size(118, 26);
            // 
            // tsmiResetAll
            // 
            this.tsmiResetAll.Name = "tsmiResetAll";
            this.tsmiResetAll.Size = new System.Drawing.Size(117, 22);
            this.tsmiResetAll.Text = "Reset all";
            this.tsmiResetAll.Click += new System.EventHandler(this.tsmiResetAll_Click);
            // 
            // FrmMeshingParameters
            // 
            this.ClientSize = new System.Drawing.Size(334, 411);
            this.Name = "FrmMeshingParameters";
            this.Text = "Edit Meshing Parameters";
            this.Controls.SetChildIndex(this.gbProperties, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnOkAddNew, 0);
            this.gbProperties.ResumeLayout(false);
            this.cmsPropertyGrid.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Overrides                                                                                                                
        protected override void Apply()
        {
            _viewMeshingParameters = (ViewMeshingParameters)propertyGrid.SelectedObject;

        }


        // Methods                                                                                                                  
        private void tsmiResetAll_Click(object sender, EventArgs e)
        {
            MeshingParameters = _defaultMeshingParameters;
        }

       

        
    }
}
