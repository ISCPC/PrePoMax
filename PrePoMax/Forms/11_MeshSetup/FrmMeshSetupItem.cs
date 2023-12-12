using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeGlobals;
using CaeMesh;
using System.Windows.Forms;


namespace PrePoMax.Forms
{
    class FrmMeshSetupItem : UserControls.FrmPropertyListView, IFormBase, IFormItemSetDataParent, IFormHighlight
    {
        // Variables                                                                                                                
        private string[] _meshSetupItemNames;
        private string _meshSetupItemToEditName;
        private ViewMeshSetupItem _viewMeshSetupItem;
        private Controller _controller;
        private bool _meshingParametersChanged;
        //
        private ContextMenuStrip cmsPropertyGrid;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem tsmiResetAll;
        private ToolTip ttText;
        private Button btnPreview;


        // Callbacks                                                                                                                
        public Func<string[], MeshSetupItem, Task> PreviewEdgeMeshAsync;



        // Properties                                                                                                               
        public MeshSetupItem MeshSetupItem
        {
            get { return _viewMeshSetupItem != null ? _viewMeshSetupItem.GetBase() : null; }
            set
            {
                if (value is MeshingParameters mp)
                {
                    bool advancedView = mp.AdvancedView;
                    if (_viewMeshSetupItem != null && _viewMeshSetupItem is ViewMeshingParameters vmp)
                        advancedView = vmp.AdvancedView;
                    _viewMeshSetupItem = new ViewMeshingParameters(mp.DeepClone(), advancedView);
                }
                else if (value is FeMeshRefinement mr) _viewMeshSetupItem = new ViewFeMeshRefinement(mr.DeepClone());
                else if (value is ShellGmsh sg) _viewMeshSetupItem = new ViewShellGmsh(sg.DeepClone());
                else if (value is TetrahedralGmsh tg) _viewMeshSetupItem = new ViewTetrahedralGmsh(tg.DeepClone());
                else if (value is TransfiniteMesh tm) _viewMeshSetupItem = new ViewTransfiniteMesh(tm.DeepClone());
                else if (value is ExtrudeMesh em) _viewMeshSetupItem = new ViewExtrudeMesh(em.DeepClone());
                else if (value is RevolveMesh rm) _viewMeshSetupItem = new ViewRevolveMesh(rm.DeepClone());
                else throw new NotImplementedException("MeshSetupItemTypeException");
            }
        }


        // Constructors                                                                                                             
        public FrmMeshSetupItem(Controller controller)
            : base(1.72)
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewMeshSetupItem = null;
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cmsPropertyGrid = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiResetAll = new System.Windows.Forms.ToolStripMenuItem();
            this.btnPreview = new System.Windows.Forms.Button();
            this.ttText = new System.Windows.Forms.ToolTip(this.components);
            this.gbType.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.cmsPropertyGrid.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbType
            // 
            this.gbType.Size = new System.Drawing.Size(340, 101);
            // 
            // lvTypes
            // 
            this.lvTypes.Size = new System.Drawing.Size(328, 73);
            // 
            // gbProperties
            // 
            this.gbProperties.Size = new System.Drawing.Size(340, 310);
            // 
            // propertyGrid
            // 
            this.propertyGrid.ContextMenuStrip = this.cmsPropertyGrid;
            this.propertyGrid.Size = new System.Drawing.Size(328, 282);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(190, 436);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(271, 436);
            // 
            // btnOkAddNew
            // 
            this.btnOkAddNew.Location = new System.Drawing.Point(109, 436);
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
            this.btnPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPreview.Image = global::PrePoMax.Properties.Resources.Show;
            this.btnPreview.Location = new System.Drawing.Point(75, 436);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(28, 23);
            this.btnPreview.TabIndex = 17;
            this.ttText.SetToolTip(this.btnPreview, "Preview");
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // FrmMeshSetupItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(364, 471);
            this.Controls.Add(this.btnPreview);
            this.MinimumSize = new System.Drawing.Size(380, 510);
            this.Name = "FrmMeshSetupItem";
            this.Text = "Edit Mesh Setup Item";
            this.VisibleChanged += new System.EventHandler(this.FrmMeshSetupItem_VisibleChanged);
            this.Controls.SetChildIndex(this.gbProperties, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnOkAddNew, 0);
            this.Controls.SetChildIndex(this.gbType, 0);
            this.Controls.SetChildIndex(this.btnPreview, 0);
            this.gbType.ResumeLayout(false);
            this.gbProperties.ResumeLayout(false);
            this.cmsPropertyGrid.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Event handlers                                                                                                           
        private void FrmMeshSetupItem_VisibleChanged(object sender, EventArgs e)
        {
            _controller.Selection.LimitSelectionToFirstMesherType = Visible;
            //
            if (Visible) ShowHideSelectionForm();
        }
        private void tsmiResetAll_Click(object sender, EventArgs e)
        {
            if (MeshSetupItem is MeshingParameters mp)
            {
                mp.CopyFrom(GetDefaultMeshingParameters(MeshSetupItem.CreationIds));
                _meshingParametersChanged = false;
            }
            else if (MeshSetupItem is FeMeshRefinement mr) mr.Reset();
            else if (MeshSetupItem is ShellGmsh sg) sg.Reset();
            else if (MeshSetupItem is TetrahedralGmsh tg) tg.Reset();
            else if (MeshSetupItem is TransfiniteMesh tm) tm.Reset();
            else if (MeshSetupItem is ExtrudeMesh em) em.Reset();
            else if (MeshSetupItem is RevolveMesh rm) rm.Reset();
            else throw new NotSupportedException("MeshSetupItemTypeException");
            //
            _propertyItemChanged = true;
            //
            propertyGrid.Refresh();
        }
        async private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                Enabled = false;
                ItemSetDataEditor.SelectionForm.Enabled = false;
                //
                string[] partNames;
                if (MeshSetupItem is MeshingParameters || MeshSetupItem is TetrahedralGmsh || MeshSetupItem is TransfiniteMesh)
                    partNames = _controller.DisplayedMesh.GetPartNamesFromPartIds(MeshSetupItem.CreationIds);
                else if (MeshSetupItem is ShellGmsh || MeshSetupItem is FeMeshRefinement || MeshSetupItem is ExtrudeMesh ||
                         MeshSetupItem is RevolveMesh)
                    partNames = _controller.DisplayedMesh.GetPartNamesFromGeometryIds(MeshSetupItem.CreationIds);
                else throw new NotSupportedException("MeshSetupItemTypeException");
                //
                if (partNames != null && partNames.Length > 0)
                {
                    HighlightMeshSetupItem(false);
                    //
                    await PreviewEdgeMeshAsync?.Invoke(partNames, MeshSetupItem);
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                Enabled = true;
                ItemSetDataEditor.SelectionForm.Enabled = true;
            }
        }


        // Overrides                                                                                                                
        protected override void OnListViewTypeSelectedIndexChanged()
        {
            if (lvTypes.SelectedItems != null && lvTypes.SelectedItems.Count > 0)
            {
                object itemTag = lvTypes.SelectedItems[0].Tag;
                if (itemTag is ViewMeshingParameters vmp) _viewMeshSetupItem = vmp;
                else if (itemTag is ViewFeMeshRefinement vmr) _viewMeshSetupItem = vmr;
                else if (itemTag is ViewShellGmsh vsg) _viewMeshSetupItem = vsg;
                else if (itemTag is ViewTetrahedralGmsh vtg) _viewMeshSetupItem = vtg;
                else if (itemTag is ViewTransfiniteMesh vtm) _viewMeshSetupItem = vtm;
                else if (itemTag is ViewExtrudeMesh vem) _viewMeshSetupItem = vem;
                else if (itemTag is ViewRevolveMesh vrm) _viewMeshSetupItem = vrm;
                else throw new NotImplementedException("MeshSetupItemTypeException");
                //
                ShowHideSelectionForm();
                //
                propertyGrid.SelectedObject = _viewMeshSetupItem;
                propertyGrid.Select();
                //
                HighlightMeshSetupItem();
            }
        }
        protected override void OnPropertyGridPropertyValueChanged()
        {
            string property = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
            //
            if (_viewMeshSetupItem is ViewMeshingParameters vmp &&
                property != nameof(vmp.Name) && property != nameof(vmp.AdvancedView))
            {
                _meshingParametersChanged = true;
            }
            HighlightMeshSetupItem();
            //
            base.OnPropertyGridPropertyValueChanged();
        }
        protected override void OnApply(bool onOkAddNew)
        {
            _viewMeshSetupItem = (ViewMeshSetupItem)propertyGrid.SelectedObject;
            //
            if (_viewMeshSetupItem == null) throw new CaeException("No mesh setup item was selected.");
            // Check if the name exists
            CheckName(_meshSetupItemToEditName, MeshSetupItem.Name, _meshSetupItemNames, "mesh setup item");
            //
            if (MeshSetupItem.CreationIds == null || MeshSetupItem.CreationIds.Length == 0)
                throw new CaeException("The mesh setup item selection must contain at least one item.");
            //
            string error = _controller.IsMeshSetupItemProperlyDefined(MeshSetupItem);
            if (error != null) throw new CaeException(error);
            // When opened from regenerate
            if (this.Modal) { }
            // When opened normally
            else
            {
                // Create
                if (_meshSetupItemToEditName == null)
                {
                    _controller.AddMeshSetupItemCommand(MeshSetupItem);
                }
                // Replace
                else if (_propertyItemChanged)
                {
                    _controller.ReplaceMeshSetupItemCommand(_meshSetupItemToEditName, MeshSetupItem);
                }
            }
            // If all is successful close the ItemSetSelectionForm - except for OKAddNew
            if (!onOkAddNew)
            {
                DialogResult = DialogResult.OK;
                ItemSetDataEditor.SelectionForm.Hide();
            }
        }
        protected override void OnHideOrClose()
        {
            // Close the ItemSetSelectionForm
            ItemSetDataEditor.SelectionForm.Hide();
            //
            base.OnHideOrClose();
        }
        protected override bool OnPrepareForm(string stepName, string meshSetupItemToEditName)
        {
            DialogResult = DialogResult.None;
            //
            int dx = btnCancel.Left - btnOK.Right;
            if (meshSetupItemToEditName == null)
            {
                btnOkAddNew.Visible = true;
                btnPreview.Visible = true;
                btnPreview.Location = new System.Drawing.Point(btnOkAddNew.Left - dx - btnPreview.Width, btnOkAddNew.Top);
            }
            else
            {
                btnOkAddNew.Visible = false;
                btnPreview.Visible = true;
                btnPreview.Location = new System.Drawing.Point(btnOK.Left - dx - btnPreview.Width, btnOkAddNew.Top);
            }
            //
            _propertyItemChanged = false;
            _meshSetupItemNames = null;
            _meshSetupItemToEditName = null;
            _viewMeshSetupItem = null;
            lvTypes.Items.Clear();
            propertyGrid.SelectedObject = null;
            _meshingParametersChanged = false;
            _controller.ClearAllSelection();
            //
            _meshSetupItemNames = _controller.GetMeshSetupItemNames();
            _meshSetupItemToEditName = meshSetupItemToEditName;
            _meshingParametersChanged = _meshSetupItemToEditName != null;
            //
            if (_meshSetupItemNames == null)
                throw new CaeException("The mesh setup item names must be defined first.");
            //
            PopulateListOfMeshSetupItems();
            // Create new mesh setup item
            if (_meshSetupItemToEditName == null)
            {
                lvTypes.Enabled = true;
                _viewMeshSetupItem = null;
            }
            // Edit existing mesh setup item
            else
            {
                MeshSetupItem = _controller.GetMeshSetupItem(_meshSetupItemToEditName);   // to clone
                // Check validity
                if (!MeshSetupItem.Valid)
                {
                    MeshSetupItem.CreationData = null;
                    MeshSetupItem.CreationIds = null;
                    MeshSetupItem.Valid = true;
                }
                int selectedId;
                if (_viewMeshSetupItem is ViewMeshingParameters) selectedId = 0;
                else if (_viewMeshSetupItem is ViewFeMeshRefinement) selectedId = 1;
                else if (_viewMeshSetupItem is ViewShellGmsh) selectedId = 2;
                else if (_viewMeshSetupItem is ViewTetrahedralGmsh) selectedId = 3;
                else if (_viewMeshSetupItem is ViewTransfiniteMesh) selectedId = 4;
                else if (_viewMeshSetupItem is ViewExtrudeMesh) selectedId = 5;
                else if (_viewMeshSetupItem is ViewRevolveMesh) selectedId = 6;
                else throw new NotSupportedException("MeshSetupItemTypeException");
                //
                lvTypes.Items[selectedId].Tag = _viewMeshSetupItem;
                _preselectIndex = selectedId;
            }

            //
            ShowHideSelectionForm();
            //
            return true;
        }


        // Methods                                                                                                                  
        private MeshingParameters GetDefaultMeshingParameters(int[] ids)
        {
            MeshingParameters meshingParameters = _controller.Settings.Meshing.MeshingParameters;
            //
            if (MeshSetupItem is MeshingParameters mp)
            {
                meshingParameters.Name = mp.Name;
                //
                if (ids != null && ids.Length > 0)
                {
                    string[] partNames = _controller.Model.Geometry.GetPartNamesFromPartIds(ids);
                    MeshingParameters defaultMeshingParameters = _controller.GetPartDefaultMeshingParameters(partNames);
                    //
                    if (defaultMeshingParameters != null)
                    {
                        defaultMeshingParameters.Name = mp.Name;
                        meshingParameters = defaultMeshingParameters;
                    }
                }
                // Copy relative
                meshingParameters.RelativeSize = mp.RelativeSize;
                // Copy creation data
                meshingParameters.CreationIds = mp.CreationIds;
                meshingParameters.CreationData = mp.CreationData;
            }
            return meshingParameters;
        }
        public void SetMeshSetupItem(MeshSetupItem meshSetupItem)
        {
            int selectedId;
            _meshSetupItemToEditName = meshSetupItem.Name;  // the OnApply checks this name
            //
            if (meshSetupItem is MeshingParameters mp)
            {
                selectedId = 0;
                _viewMeshSetupItem = new ViewMeshingParameters(mp.DeepClone());
            }
            else if (meshSetupItem is FeMeshRefinement mr)
            {
                selectedId = 1;
                _viewMeshSetupItem = new ViewFeMeshRefinement(mr.DeepClone());
            }
            else if (meshSetupItem is ShellGmsh sg)
            {
                selectedId = 2;
                _viewMeshSetupItem = new ViewShellGmsh(sg.DeepClone());
            }
            else if (meshSetupItem is TetrahedralGmsh tg)
            {
                selectedId = 3;
                _viewMeshSetupItem = new ViewTetrahedralGmsh(tg.DeepClone());
            }
            else if (meshSetupItem is TransfiniteMesh tm)
            {
                selectedId = 4;
                _viewMeshSetupItem = new ViewTransfiniteMesh(tm.DeepClone());
            }
            else if (meshSetupItem is ExtrudeMesh em)
            {
                selectedId = 5;
                _viewMeshSetupItem = new ViewExtrudeMesh(em.DeepClone());
            }
            else if (meshSetupItem is RevolveMesh rm)
            {
                selectedId = 6;
                _viewMeshSetupItem = new ViewRevolveMesh(rm.DeepClone());
            }
            else throw new NotSupportedException("MeshSetupItemTypeException");
            //
            _viewMeshSetupItem.HideName();
            btnOkAddNew.Visible = false;
            btnPreview.Visible = false;
            //
            lvTypes.Items[selectedId].Tag = _viewMeshSetupItem;
            //
            PreselectListViewItem(selectedId);
            //
            ItemSetDataEditor.SelectionForm.Hide();
        }
        //
        private void PopulateListOfMeshSetupItems()
        {
            bool twoD = _controller.Model.Properties.ModelSpace.IsTwoD();
            //
            ListViewItem item;
            // Meshing parameters
            item = new ListViewItem("Meshing Parameters");
            MeshingParameters mp = _controller.GetDefaultMeshingParameters(GetMeshSetupItemName("Meshing_Parameters"));
            ViewMeshingParameters vmp = new ViewMeshingParameters(mp);
            item.Tag = vmp;
            lvTypes.Items.Add(item);
            // Mesh refinement
            item = new ListViewItem("Mesh Refinement");
            FeMeshRefinement mr = new FeMeshRefinement(GetMeshSetupItemName("Mesh_Refinement"));
            ViewFeMeshRefinement vmr = new ViewFeMeshRefinement(mr);
            item.Tag = vmr;
            lvTypes.Items.Add(item);
            // Shell gmsh
            item = new ListViewItem("Shell Gmsh");
            ShellGmsh sg = new ShellGmsh(GetMeshSetupItemName("Shell_Gmsh"));
            ViewShellGmsh vsg = new ViewShellGmsh(sg);
            item.Tag = vsg;
            lvTypes.Items.Add(item);
            // Tetrahedral gmsh
            item = new ListViewItem("Tetrahedral Gmsh");
            TetrahedralGmsh tg = new TetrahedralGmsh(GetMeshSetupItemName("Tetrahedral_Gmsh"));
            ViewTetrahedralGmsh vtg = new ViewTetrahedralGmsh(tg);
            item.Tag = vtg;
            lvTypes.Items.Add(item);
            //
            // Transfinite mesh
            item = new ListViewItem("Transfinite Mesh");
            TransfiniteMesh tm = new TransfiniteMesh(GetMeshSetupItemName("Transfinite_Mesh"));
            ViewTransfiniteMesh vtm = new ViewTransfiniteMesh(tm);
            item.Tag = vtm;
            lvTypes.Items.Add(item);
            //
            if (!twoD)
            {
                // Extrude mesh
                item = new ListViewItem("Extrude Mesh");
                ExtrudeMesh em = new ExtrudeMesh(GetMeshSetupItemName("Extrude_Mesh"));
                ViewExtrudeMesh vem = new ViewExtrudeMesh(em);
                item.Tag = vem;
                lvTypes.Items.Add(item);
                // Revolve mesh
                item = new ListViewItem("Revolve Mesh");
                RevolveMesh rm = new RevolveMesh(GetMeshSetupItemName("Revolve_Mesh"));
                ViewRevolveMesh vrm = new ViewRevolveMesh(rm);
                item.Tag = vrm;
                lvTypes.Items.Add(item);
            }
        }
        private string GetMeshSetupItemName(string name)
        {
            return _meshSetupItemNames.GetNextNumberedKey(name);
        }
        private void HighlightMeshSetupItem(bool highlightNodes = true)
        {
            try
            {
                // This calls selection which might change the view which calls clear
                // which calls SelectionChanged which changes the MeshSetupItem !!!
                if (Modal) return;
                //
                _controller.ClearSelectionHistory();
                //
                if (_viewMeshSetupItem == null) { }
                else
                {
                    if (MeshSetupItem.CreationData != null && MeshSetupItem.CreationIds != null &&
                        MeshSetupItem.CreationIds.Length > 0)
                    {
                        try
                        {
                            _controller.HighlightMeshSetupItem(MeshSetupItem, highlightNodes);
                        }
                        catch { }
                        // Set previous selection to continue it
                        _controller.Selection = MeshSetupItem.CreationData.DeepClone();
                    }
                }
            }
            catch { }
        }
        private void ShowHideSelectionForm()
        {
            // When opened from regenerate
            if (!Visible || this.Modal) ItemSetDataEditor.SelectionForm.Hide();
            // When opened normally
            else
            {
                if (MeshSetupItem != null)
                {
                    ItemSetDataEditor.SelectionForm.ItemSetData = new ItemSetData(MeshSetupItem.CreationIds);
                    ItemSetDataEditor.SelectionForm.ShowIfHidden(this.Owner);
                }
                else ItemSetDataEditor.SelectionForm.Hide();
            }
            //
            SetSelectItem();
        }
        private void SetSelectItem()
        {
            if (MeshSetupItem != null)
            {
                if (MeshSetupItem is MeshingParameters) _controller.SetSelectItemToPart();
                else if (MeshSetupItem is FeMeshRefinement) _controller.SetSelectItemToGeometry();
                else if (MeshSetupItem is ShellGmsh) _controller.SetSelectItemToPart();
                else if (MeshSetupItem is TetrahedralGmsh) _controller.SetSelectItemToPart();
                else if (MeshSetupItem is TransfiniteMesh) _controller.SetSelectItemToPart();
                else if (MeshSetupItem is ExtrudeMesh) _controller.SetSelectItemToGeometrySurface();
                else if (MeshSetupItem is RevolveMesh) _controller.SetSelectItemToGeometrySurface();
                else throw new NotSupportedException("MeshSetupItemTypeException");
            }
            else _controller.SetSelectByToOff();
        }
        //
        public void SelectionChanged(int[] ids)
        {
            if (MeshSetupItem != null)
            {
                if (MeshSetupItem is MeshingParameters || MeshSetupItem is FeMeshRefinement ||
                    MeshSetupItem is ShellGmsh || MeshSetupItem is TetrahedralGmsh ||
                    MeshSetupItem is TransfiniteMesh || MeshSetupItem is ExtrudeMesh || MeshSetupItem is RevolveMesh)
                {
                    if (MeshSetupItem is MeshingParameters mp && !_meshingParametersChanged)
                    {
                        mp.CopyFrom(GetDefaultMeshingParameters(ids));
                    }
                    MeshSetupItem.CreationIds = ids;
                    MeshSetupItem.CreationData = _controller.Selection.DeepClone();
                    //
                    propertyGrid.Refresh();
                    //
                    _propertyItemChanged = true;
                    //
                    if (MeshSetupItem is ExtrudeMesh || MeshSetupItem is RevolveMesh)
                    {
                        _controller.IsMeshSetupItemProperlyDefined(MeshSetupItem);
                    }
                    //
                    if (ids.Length != 0) HighlightMeshSetupItem(); // this will redraw the selection with correct backfaceCulling
                }
                else throw new NotSupportedException("MeshSetupItemTypeException");
            }
        }

        // IFormHighlight
        public void Highlight()
        {
            HighlightMeshSetupItem();
        }

        // IFormItemSetDataParent
        public bool IsSelectionGeometryBased()
        {
            // Prepare ItemSetDataEditor - prepare Geometry or Mesh based selection
            // All meshing parameters are geometry based
            return true;
        }
    }
}
