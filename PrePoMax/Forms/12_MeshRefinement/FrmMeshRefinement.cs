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
    class FrmMeshRefinement : UserControls.FrmProperties, IFormBase, IFormItemSetDataParent, IFormHighlight
    {
        // Variables                                                                                                                
        private HashSet<string> _meshRefinementNames;
        private string _meshRefinementToEditName;
        private ViewFeMeshRefinement _viewFeMeshRefinement;
        private List<SelectionNode> _prevSelectionNodes;
        private SelectionNodeIds _selectionNodeIds;
        private Button btnPreview;
        private Controller _controller;
        private int _previewBtnDx;
        private string _defaultName;


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
            _previewBtnDx = btnCancel.Left - btnOK.Right;
            _defaultName = null;
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
            btnPreview.Enabled = true;
        }


        // Overrides                                                                                                                
        protected override void OnApply(bool onOkAddNew)
        {
            _viewFeMeshRefinement = (ViewFeMeshRefinement)propertyGrid.SelectedObject;
            // Check if the name exists
            if ((_meshRefinementToEditName == null && _meshRefinementNames.Contains(MeshRefinement.Name)) ||  // named to existing name
                (MeshRefinement.Name != _meshRefinementToEditName
                 && _meshRefinementNames.Contains(MeshRefinement.Name))) // renamed to existing name
                throw new CaeException("The selected name already exists.");
            //
            if (MeshRefinement.GeometryIds == null || MeshRefinement.GeometryIds.Length == 0)
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
                    // Replace back the ids by the previous selection
                    if (MeshRefinement.CreationData.Nodes[0] is SelectionNodeIds sn && sn.Equals(_selectionNodeIds))
                    {
                        MeshRefinement.CreationData.Nodes.RemoveAt(0);
                        MeshRefinement.CreationData.Nodes.InsertRange(0, _prevSelectionNodes);
                    }
                    //
                    MeshRefinement.Valid = true;
                    _controller.ReplaceMeshRefinementCommand(_meshRefinementToEditName, MeshRefinement);
                }
            }
            // If all is successful close the ItemSetSelectionForm - except for OKAddNew
            if (!onOkAddNew) ItemSetDataEditor.SelectionForm.Hide();
        }
        protected override void OnHideOrClose()
        {
            // Close the ItemSetSelectionForm
            ItemSetDataEditor.SelectionForm.Hide();
            //
            base.OnHideOrClose();
        }
        protected override bool OnPrepareForm(string stepName, string meshRefinementToEditName)
        {
            // To prevent the call to frmMain.itemForm_VisibleChanged when minimized
            this.DialogResult = DialogResult.None;      
            if (meshRefinementToEditName == null)
            {
                btnOkAddNew.Visible = true;
                btnPreview.Location = new System.Drawing.Point(btnOkAddNew.Left - _previewBtnDx - btnPreview.Width, btnOkAddNew.Top);
            }
            else
            {
                btnOkAddNew.Visible = false;
                btnPreview.Location = new System.Drawing.Point(btnOK.Left - _previewBtnDx - btnPreview.Width, btnOkAddNew.Top);
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
            _controller.SetSelectItemToGeometry();
            //
            _meshRefinementNames.UnionWith(_controller.GetMeshRefinementNames());
            _meshRefinementToEditName = meshRefinementToEditName;
            // Create new mesh refinement
            if (_meshRefinementToEditName == null)
            {
                _defaultName = GetMeshRefinementName();
                MeshRefinement = new FeMeshRefinement(_defaultName);
                _controller.Selection.Clear();
            }
            // Edit existing mesh refinement
            else
            {
                MeshRefinement = _controller.GetMeshRefinement(_meshRefinementToEditName);   // to clone
                //
                if (MeshRefinement.GeometryIds != null)
                {
                    // Change node selection history to ids to speed up
                    int[] ids = MeshRefinement.GeometryIds;
                    _selectionNodeIds = new SelectionNodeIds(vtkSelectOperation.None, false, ids, true);
                    _prevSelectionNodes = MeshRefinement.CreationData.Nodes;
                    _controller.CreateNewSelection(MeshRefinement.CreationData.CurrentView, _selectionNodeIds, true);
                    // Copy selection
                    MeshRefinement.CreationData = _controller.Selection.DeepClone();
                }
            }
            //
            propertyGrid.SelectedObject = _viewFeMeshRefinement;
            propertyGrid.Select();
            // Show ItemSetDataForm
            ItemSetDataEditor.SelectionForm.ItemSetData = new ItemSetData(MeshRefinement.GeometryIds);
            ItemSetDataEditor.SelectionForm.ShowIfHidden(this.Owner);
            //
            HighlightMeshRefinement();
            //
            return true;
        }


        // Methods                                                                                                                  
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
                    _controller.SetSelectItemToGeometry();
                    // Surface.CreationData is set to null when the CreatedFrom is changed
                    if (MeshRefinement.CreationData != null)
                    {
                        _controller.Selection = MeshRefinement.CreationData;  // deep copy to not clear
                        _controller.HighlightSelection();
                    }
                }
            }
            catch { }
        }
        //
        public void SelectionChanged(int[] ids)
        {
            if (btnPreview.Enabled)
            {
                MeshRefinement.GeometryIds = ids;
                MeshRefinement.CreationData = _controller.Selection.DeepClone();
                //
                propertyGrid.Refresh();
                //
                _propertyItemChanged = true;
            }
        }

        // IFormHighlight
        public void Highlight()
        {
            HighlightMeshRefinement();
        }

        // IFormItemSetDataParent
        public bool IsSelectionGeometryBased()
        {
            // Prepare ItemSetDataEditor
            if (_meshRefinementToEditName == null) return true;
            return _controller.GetMeshRefinement(_meshRefinementToEditName).CreationData.IsGeometryBased();
        }
        async private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                btnPreview.Enabled = false;
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
            finally
            {
                btnPreview.Enabled = true;
            }
        }

       
    }


}
