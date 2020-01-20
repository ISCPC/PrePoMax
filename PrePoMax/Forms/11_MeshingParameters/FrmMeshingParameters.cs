﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;

namespace PrePoMax.Forms
{
    class FrmMeshingParameters : UserControls.FrmProperties, IFormBase
    {
        // Variables                                                                                                                
        private Controller _controller;
        private string[] _partNames;
        private MeshingParameters _defaultMeshingParameters;
        private System.Windows.Forms.ContextMenuStrip cmsPropertyGrid;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.ToolStripMenuItem tsmiResetAll;
        private System.Windows.Forms.Button btnPreview;
        private ViewMeshingParameters _viewMeshingParameters;


        // Callbacks                                                                                                                
        public Action UpdateHighlightFromTree;


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
        public string[] PartNames
        {
            get { return _partNames; }
            set
            {
                _partNames = value;
                Text = "Edit Meshing Parameters: " + _partNames.ToShortString();
            }
        }


        // Constructors                                                                                                             
        public FrmMeshingParameters(Controller controller) 
            : base(1.5)
        {
            InitializeComponent();
            //
            _controller = controller;
            _partNames = null;
            _defaultMeshingParameters = null;
            _viewMeshingParameters = null;
            //
            btnOkAddNew.Visible = false;
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cmsPropertyGrid = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiResetAll = new System.Windows.Forms.ToolStripMenuItem();
            this.btnPreview = new System.Windows.Forms.Button();
            this.gbProperties.SuspendLayout();
            this.cmsPropertyGrid.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyGrid
            // 
            this.propertyGrid.ContextMenuStrip = this.cmsPropertyGrid;
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
            // btnPreview
            // 
            this.btnPreview.Location = new System.Drawing.Point(79, 376);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 23);
            this.btnPreview.TabIndex = 16;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // FrmMeshingParameters
            // 
            this.ClientSize = new System.Drawing.Size(334, 411);
            this.Controls.Add(this.btnPreview);
            this.Name = "FrmMeshingParameters";
            this.Text = "Edit Meshing Parameters";
            this.VisibleChanged += new System.EventHandler(this.FrmMeshingParameters_VisibleChanged);
            this.Controls.SetChildIndex(this.gbProperties, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnOkAddNew, 0);
            this.Controls.SetChildIndex(this.btnPreview, 0);
            this.gbProperties.ResumeLayout(false);
            this.cmsPropertyGrid.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Event handlers                                                                                                           
        private void FrmMeshingParameters_VisibleChanged(object sender, EventArgs e)
        {
            UpdateHighlightFromTree?.Invoke();
            btnPreview.Visible = Visible && !Modal;
        }

        // Overrides                                                                                                                
        protected override void Apply()
        {
            // OK
            _viewMeshingParameters = (ViewMeshingParameters)propertyGrid.SelectedObject;
            //
            if (_viewMeshingParameters.GetBase() != null && !Modal)
            {
                _controller.SetMeshingParametersCommand(_partNames, _viewMeshingParameters.GetBase());
            }
        }


        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string partToEditName)
        {
            return OnPrepareForm(stepName, partToEditName);
        }
        private void tsmiResetAll_Click(object sender, EventArgs e)
        {
            MeshingParameters = _defaultMeshingParameters;
        }
        async private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                MeshingParameters parameters = ((ViewMeshingParameters)(propertyGrid.SelectedObject)).GetBase();
                if (_partNames != null && _partNames.Length > 0)
                {
                    UpdateHighlightFromTree?.Invoke();
                    foreach (var partName in _partNames)
                    {
                        await Task.Run(() => _controller.PreviewEdgeMesh(partName, parameters, null));
                    }
                }
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }

       
    }
}