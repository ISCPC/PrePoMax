using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeMesh;
using CaeGlobals;

namespace PrePoMax.Forms
{
    class FrmBoundaryLayer : UserControls.FrmProperties, IFormBase, IFormItemSetDataParent
    {
        // Variables                                                                                                                
        private ViewBoundaryLayer _viewBoundaryLayer;
        private Controller _controller;


        // Constructors                                                                                                             
        public FrmBoundaryLayer(Controller controller) 
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewBoundaryLayer = null;
            //
            SelectionClear = _controller.Selection.Clear;
        }
        private void InitializeComponent()
        {
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbProperties
            // 
            this.gbProperties.Size = new System.Drawing.Size(310, 264);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Size = new System.Drawing.Size(298, 236);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(160, 276);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(241, 276);
            // 
            // btnOkAddNew
            // 
            this.btnOkAddNew.Location = new System.Drawing.Point(79, 276);
            // 
            // FrmBoundaryLayer
            // 
            this.ClientSize = new System.Drawing.Size(334, 311);
            this.MaximumSize = new System.Drawing.Size(350, 350);
            this.MinimumSize = new System.Drawing.Size(350, 350);
            this.Name = "FrmBoundaryLayer";
            this.Text = "Create Boundary Layer";
            this.gbProperties.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Event handlers                                                                                                           


        // Overrides                                                                                                                
        protected override void OnPropertyGridPropertyValueChanged()
        {
            base.OnPropertyGridPropertyValueChanged();
        }
        protected override void Apply(bool onOkAddNew)
        {
            _viewBoundaryLayer = (ViewBoundaryLayer)propertyGrid.SelectedObject;
            
            if (_viewBoundaryLayer.ItemSetData.ItemIds == null || _viewBoundaryLayer.GetGeometryIds().Length == 0)
                throw new CaeException("The boundary layer must contain at least one item.");
            // Create
            _controller.CreateBoundaryLayerCommand(_viewBoundaryLayer.GetGeometryIds(), _viewBoundaryLayer.Thickness);
            //
            _controller.Selection.Clear();
        }
        protected override bool OnPrepareForm(string stepName, string meshRefinementToEditName)
        {
            this.DialogResult = DialogResult.None;      // to prevent the call to frmMain.itemForm_VisibleChanged when minimized
            //
            _propertyItemChanged = false;
            _viewBoundaryLayer = null;
            propertyGrid.SelectedObject = null;
            //
            _controller.SetSelectItemToGeometry();
            //
            _viewBoundaryLayer = new ViewBoundaryLayer();
            _controller.Selection.Clear();
            //
            propertyGrid.SelectedObject = _viewBoundaryLayer;
            propertyGrid.Select();
            //
            return true;
        }
        protected override void OnEnabledChanged()
        {
            // Form is Enabled On and Off by the itemSetForm
            if (this.Enabled)
            {
                // The FrmItemSet was closed with OK
                if (this.DialogResult == System.Windows.Forms.DialogResult.OK)   
                {
                    _viewBoundaryLayer.CreationData = _controller.Selection.DeepClone();
                    _propertyItemChanged = true;
                }
                // The FrmItemSet was closed with Cancel
                else if (this.DialogResult == System.Windows.Forms.DialogResult.Cancel)
                {
                    if (_viewBoundaryLayer.CreationData != null)
                    {
                        _controller.Selection.CopySelectonData(_viewBoundaryLayer.CreationData);
                    }
                }
            }
            // When the itemSetForm is shown, reset the highlight (after Preview Edge Mesh)
            HighlightSurface();
            this.DialogResult = System.Windows.Forms.DialogResult.None;
        }


        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string meshRefinementToEditName)
        {
            return OnPrepareForm(stepName, meshRefinementToEditName);
        }


        // IFormItemSetDataParent
        public bool IsSelectionGeometryBased()
        {
            return true;
        }
        
        private void HighlightSurface()
        {
            try
            {
                if (_controller != null)
                {
                    _controller.ClearSelectionHistory();
                    _controller.SetSelectItemToGeometry();
                    // Surface.CreationData is set to null when the CreatedFrom is changed
                    if (_viewBoundaryLayer.CreationData != null)
                        _controller.Selection.CopySelectonData(_viewBoundaryLayer.CreationData.DeepClone()); // Deep copy to not clear
                    _controller.HighlightSelection();
                }
            }
            catch { }
        }

    }


}
