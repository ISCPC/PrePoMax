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
    class FrmMeshRefinement : UserControls.FrmProperties, IFormBase, IFormItemSetDataParent
    {
        // Variables                                                                                                                
        private HashSet<string> _meshRefinementNames;
        private string _meshRefinementToEditName;
        private ViewFeMeshRefinement _viewFeMeshRefinement;
        private List<SelectionNode> _prevSelectionNodes;
        private SelectionNodeIds _selectionNodeIds;
        private Button btnPreview;
        private Controller _controller;
        private int _previewDx;


        // Properties                                                                                                               
        public FeMeshRefinement MeshRefinement
        {
            get { return _viewFeMeshRefinement.GetBase(); }
            set { _viewFeMeshRefinement = new ViewFeMeshRefinement(value.DeepClone()); }
        }


        // Constructors                                                                                                             
        public FrmMeshRefinement(Controller controller) 
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewFeMeshRefinement = null;
            _meshRefinementNames = new HashSet<string>();
            _previewDx = btnCancel.Left - btnOK.Right;
            //
            SelectionClear = _controller.Selection.Clear;
        }
        private void InitializeComponent()
        {
            this.btnPreview = new System.Windows.Forms.Button();
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbProperties
            // 
            this.gbProperties.Size = new System.Drawing.Size(329, 364);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Size = new System.Drawing.Size(317, 336);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(179, 376);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(260, 376);
            // 
            // btnOkAddNew
            // 
            this.btnOkAddNew.Location = new System.Drawing.Point(98, 376);
            // 
            // btnPreview
            // 
            this.btnPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPreview.Location = new System.Drawing.Point(17, 376);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 23);
            this.btnPreview.TabIndex = 17;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // FrmMeshRefinement
            // 
            this.ClientSize = new System.Drawing.Size(353, 411);
            this.Controls.Add(this.btnPreview);
            this.MaximumSize = new System.Drawing.Size(369, 450);
            this.MinimumSize = new System.Drawing.Size(369, 450);
            this.Name = "FrmMeshRefinement";
            this.Text = "Edit Mesh Refinement";
            this.VisibleChanged += new System.EventHandler(this.FrmMeshRefinement_VisibleChanged);
            this.Controls.SetChildIndex(this.gbProperties, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnOkAddNew, 0);
            this.Controls.SetChildIndex(this.btnPreview, 0);
            this.gbProperties.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Event handlers                                                                                                           
        private void FrmMeshRefinement_VisibleChanged(object sender, EventArgs e)
        {
            // Limit selection to the first selected part
            _controller.Selection.LimitSelectionToFirstPart = Visible;
        }


        // Overrides                                                                                                                
        protected override void OnPropertyGridPropertyValueChanged()
        {
            //string value = (string)propertyGrid.SelectedGridItem.Value.ToString();
            //HighlightMeshRefinement()
            base.OnPropertyGridPropertyValueChanged();
        }
        protected override void Apply()
        {
            _viewFeMeshRefinement = (ViewFeMeshRefinement)propertyGrid.SelectedObject;
            //
            if ((_meshRefinementToEditName == null && _meshRefinementNames.Contains(_viewFeMeshRefinement.Name)) ||                    // named to existing name
                (_viewFeMeshRefinement.Name != _meshRefinementToEditName && _meshRefinementNames.Contains(_viewFeMeshRefinement.Name)))// renamed to existing name
                throw new CaeException("The selected name already exists.");
            //
            if (_viewFeMeshRefinement.ItemSetData.ItemIds == null || _viewFeMeshRefinement.ItemSetData.ItemIds.Length == 0)
                throw new CaeException("The mesh refinement must contain at least one item.");
            //
            if (_meshRefinementToEditName == null)
            {
                // Create
                _controller.AddMeshRefinementCommand(MeshRefinement);
            }
            else
            {
                // Replace
                if (_propertyItemChanged || !MeshRefinement.Valid)
                {
                    // replace back the ids by the previous selection
                    Selection selection = MeshRefinement.CreationData;
                    if (selection.Nodes[0] is SelectionNodeIds sn && sn.Equals(_selectionNodeIds))
                    {
                        selection.Nodes.RemoveAt(0);
                        selection.Nodes.InsertRange(0, _prevSelectionNodes);
                    }
                    //
                    MeshRefinement.Valid = true;
                    _controller.ReplaceMeshRefinementCommand(_meshRefinementToEditName, MeshRefinement);
                }
            }
            _controller.Selection.Clear();
        }
        protected override bool OnPrepareForm(string stepName, string meshRefinementToEditName)
        {
            this.DialogResult = DialogResult.None;      // to prevent the call to frmMain.itemForm_VisibleChanged when minimized
            if (meshRefinementToEditName == null)
            {
                btnOkAddNew.Visible = true;
                btnPreview.Location = new System.Drawing.Point(btnOkAddNew.Left - _previewDx - btnPreview.Width, btnOkAddNew.Top);
            }
            else
            {
                btnOkAddNew.Visible = false;
                btnPreview.Location = new System.Drawing.Point(btnOK.Left - _previewDx - btnPreview.Width, btnOkAddNew.Top);
            }
            //
            _propertyItemChanged = false;
            _meshRefinementNames.Clear();
            _meshRefinementToEditName = null;
            _viewFeMeshRefinement = null;
            propertyGrid.SelectedObject = null;
            _prevSelectionNodes = null;
            _selectionNodeIds = null;
            //
            _controller.SetSelectItemToSurface();
            //
            _meshRefinementNames.UnionWith(_controller.GetMeshRefinementNames());
            _meshRefinementToEditName = meshRefinementToEditName;
            //
            if (_meshRefinementToEditName == null)
            {
                MeshRefinement = new FeMeshRefinement(GetMeshRefinementName());
                _controller.Selection.Clear();
            }
            else
            {
                MeshRefinement = _controller.GetMeshRefinement(_meshRefinementToEditName);   // to clone
                //
                if (MeshRefinement.GeometryIds != null)
                {
                    // change node selection history to ids to speed up
                    int[] ids = MeshRefinement.GeometryIds;
                    _selectionNodeIds = new SelectionNodeIds(vtkSelectOperation.None, false, ids);
                    _selectionNodeIds.GeometryIds = true;
                    _prevSelectionNodes = MeshRefinement.CreationData.Nodes;
                    _controller.ClearSelectionHistory();
                    _controller.SelectItem = vtkSelectItem.Geometry;
                    _controller.AddSelectionNode(_selectionNodeIds, true);
                    MeshRefinement.CreationData = _controller.Selection.DeepClone();
                }
            }
            //
            propertyGrid.SelectedObject = _viewFeMeshRefinement;
            propertyGrid.Select();
            // add surface selection data to selection history
            HighlightMeshRefinement();
            //
            return true;
        }
        protected override void OnEnabledChanged()
        {
            // form is Enabled On and Off by the itemSetForm
            if (this.Enabled)
            {
                if (this.DialogResult == System.Windows.Forms.DialogResult.OK)              // the FrmItemSet was closed with OK
                {
                    MeshRefinement.CreationData = _controller.Selection.DeepClone();
                    _propertyItemChanged = true;
                }
                else if (this.DialogResult == System.Windows.Forms.DialogResult.Cancel)     // the FrmItemSet was closed with Cancel
                {
                    if (MeshRefinement.CreationData != null)
                    {
                        _controller.Selection.CopySelectonData(MeshRefinement.CreationData);
                        HighlightMeshRefinement();
                    }
                }
            }
            else
            {
                // When the itemSetForm is shown, reset the highlight (after Preview Edge Mesh)
                HighlightMeshRefinement();
            }
            this.DialogResult = System.Windows.Forms.DialogResult.None;
        }


        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string meshRefinementToEditName)
        {
            return OnPrepareForm(stepName, meshRefinementToEditName);
        }
        private string GetMeshRefinementName()
        {
            return NamedClass.GetNewValueName(_meshRefinementNames, "Mesh_Refinement-");
        }
        private void HighlightMeshRefinement()
        {
            try
            {
                if (_controller != null)
                {
                    _controller.ClearSelectionHistory();
                    //
                    _controller.Selection.SelectItem = vtkSelectItem.Geometry;
                    // Surface.CreationData is set to null when the CreatedFrom is changed
                    if (MeshRefinement.CreationData != null)
                        _controller.Selection.CopySelectonData(MeshRefinement.CreationData);  // deep copy to not clear
                    //
                    _controller.HighlightSelection();
                }
            }
            catch { }
        }


        // IFormItemSetDataParent
        public bool IsSelectionGeometryBased()
        {
            // prepare ItemSetDataEditor
            if (_meshRefinementToEditName == null) return true;
            return _controller.GetMeshRefinement(_meshRefinementToEditName).CreationData.IsGeometryBased();
        }
        async private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                FeMeshRefinement meshRefinement = MeshRefinement.DeepClone();
                string[] partNames = _controller.GetPartNamesFromMeshRefinement(meshRefinement);
                if (partNames != null && partNames.Length > 0)
                {
                    HighlightMeshRefinement();
                    //Set the name to the prev meshRefinement name
                    if (_meshRefinementToEditName != null) meshRefinement.Name = _meshRefinementToEditName;
                    //
                    foreach (var partName in partNames)
                    {
                        await Task.Run(() => _controller.PreviewEdgeMesh(partName, null, meshRefinement));
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
