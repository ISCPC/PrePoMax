using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeGlobals;
using System.Windows.Forms;
using System.Drawing;

namespace PrePoMax.Forms
{
    class FrmConstraint : UserControls.FrmPropertyListView, IFormBase, IFormItemSetDataParent, IFormHighlight
    {
        // Variables                                                                                                                
        private string[] _constraintNames;
        private string _constraintToEditName;
        private ViewConstraint _viewConstraint;
        private ContextMenuStrip cmsPropertyGrid;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem tsmiSwapMasterSlave;
        private Controller _controller;


        // Properties                                                                                                               
        public Constraint Constraint
        {
            get { return _viewConstraint != null ? _viewConstraint.GetBase() : null; }
            set
            {
                var clone = value.DeepClone();
                if (value is RigidBody) _viewConstraint = new ViewRigidBody((RigidBody)clone);
                else if (value is Tie) _viewConstraint = new ViewTie((Tie)clone);
                else throw new NotImplementedException();
            }
        }


        // Constructors                                                                                                             
        public FrmConstraint(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewConstraint = null;
            //
            _selectedPropertyGridItemChangedEventActive = true;
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cmsPropertyGrid = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSwapMasterSlave = new System.Windows.Forms.ToolStripMenuItem();
            this.gbType.SuspendLayout();
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
            this.tsmiSwapMasterSlave});
            this.cmsPropertyGrid.Name = "cmsPropertyGrid";
            this.cmsPropertyGrid.Size = new System.Drawing.Size(173, 26);
            // 
            // tsmiSwapMasterSlave
            // 
            this.tsmiSwapMasterSlave.Name = "tsmiSwapMasterSlave";
            this.tsmiSwapMasterSlave.Size = new System.Drawing.Size(172, 22);
            this.tsmiSwapMasterSlave.Text = "Swap master/slave";
            this.tsmiSwapMasterSlave.Click += new System.EventHandler(this.tsmiSwapMasterSlave_Click);
            // 
            // FrmConstraint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 461);
            this.Name = "FrmConstraint";
            this.Text = "Edit Constraint";
            this.Controls.SetChildIndex(this.gbProperties, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnOkAddNew, 0);
            this.Controls.SetChildIndex(this.gbType, 0);
            this.gbType.ResumeLayout(false);
            this.gbProperties.ResumeLayout(false);
            this.cmsPropertyGrid.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Overrides                                                                                                                
        protected override void OnListViewTypeSelectedIndexChanged()
        {
            if (lvTypes.Enabled && lvTypes.SelectedItems != null && lvTypes.SelectedItems.Count > 0)
            {
                object itemTag = lvTypes.SelectedItems[0].Tag;
                if (itemTag is ViewError) _viewConstraint = null;
                else if (itemTag is ViewRigidBody vrd) _viewConstraint = vrd;
                else if (itemTag is ViewTie vt) _viewConstraint = vt;
                else throw new NotImplementedException();
                //
                propertyGrid.SelectedObject = itemTag;
                propertyGrid.Select();
                //
                SetSelectItem();
                //
                ShowHideSelectionForm();
                //
                HighlightConstraint();
                // Context menu
                if (propertyGrid.SelectedObject is ViewRigidBody vrb) propertyGrid.ContextMenuStrip = null;
                else if (propertyGrid.SelectedObject is ViewTie vt) propertyGrid.ContextMenuStrip = cmsPropertyGrid;
            }
        }
        protected override void OnPropertyGridSelectedGridItemChanged()
        {
            if (propertyGrid.SelectedGridItem.PropertyDescriptor == null) return;
            //
            string property = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
            //
            if (_viewConstraint is ViewRigidBody vrb && property == nameof(vrb.SlaveRegionType)) ShowHideSelectionForm();
            else if (_viewConstraint is ViewTie vtie)
            {
                ShowHideSelectionForm();
            }
            //
            HighlightConstraint();
            //
            base.OnPropertyGridSelectedGridItemChanged();
        }
        protected override void OnApply(bool onOkAddNew)
        {
            if (propertyGrid.SelectedObject is ViewError ve) throw new CaeGlobals.CaeException(ve.Message);
            //
            _viewConstraint = (ViewConstraint)propertyGrid.SelectedObject;
            //
            if (Constraint == null) throw new CaeException("No constraint was selected.");
            //
            if (Constraint is RigidBody rb && rb.RegionType == RegionTypeEnum.Selection &&
                (rb.CreationIds == null || rb.CreationIds.Length == 0))
                throw new CaeException("The rigid body region must contain at least one item.");
            //
            if (Constraint is Tie tie)
            {
                if (tie.MasterRegionType == RegionTypeEnum.Selection &&
                    (tie.MasterCreationIds == null || tie.MasterCreationIds.Length == 0))
                    throw new CaeException("The tie master region must contain at least one item.");
                //
                if (tie.SlaveRegionType == RegionTypeEnum.Selection &&
                    (tie.SlaveCreationIds == null || tie.SlaveCreationIds.Length == 0))
                    throw new CaeException("The tie slave region must contain at least one item.");
                // Check for errors with constructor
                var tmp = new Tie(tie.Name, tie.MasterRegionName, tie.MasterRegionType, tie.SlaveRegionName, tie.SlaveRegionType);
            }
            //
            if ((_constraintToEditName == null && _constraintNames.Contains(Constraint.Name)) ||            // named to existing name
                (Constraint.Name != _constraintToEditName && _constraintNames.Contains(Constraint.Name)))   // renamed to existing name
                throw new CaeException("The selected constraint name already exists.");
            // Create
            if (_constraintToEditName == null)
            {
                _controller.AddConstraintCommand(Constraint);
            }
            // Replace
            else if (_propertyItemChanged)
            {
                _controller.ReplaceConstraintCommand(_constraintToEditName, Constraint);
            }
            // Convert the constraint from internal to show it
            else
            {
                ConstraintInternal(false);
            }
            // If all is successful close the ItemSetSelectionForm - except for OKAddNew
            if (!onOkAddNew) ItemSetDataEditor.SelectionForm.Hide();
        }
        protected override void OnHideOrClose()
        {
            // Close the ItemSetSelectionForm
            ItemSetDataEditor.SelectionForm.Hide();
            // Convert the constraint from internal to show it
            ConstraintInternal(false);
            //
            base.OnHideOrClose();
        }
        protected override bool OnPrepareForm(string stepName, string constraintToEditName)
        {
            // To prevent clear of the selection
            _selectedPropertyGridItemChangedEventActive = false;
            // To prevent the call to frmMain.itemForm_VisibleChanged when minimized
            this.DialogResult = DialogResult.None;
            this.btnOkAddNew.Visible = constraintToEditName == null;
            //
            _propertyItemChanged = false;
            _constraintNames = null;
            _constraintToEditName = null;
            _viewConstraint = null;
            lvTypes.Items.Clear();
            propertyGrid.SelectedObject = null;
            //
            _constraintNames = _controller.GetConstraintNames();
            _constraintToEditName = constraintToEditName;
            //
            string[] referencePointNames = _controller.GetReferencePointNames();
            string[] nodeSetNames = _controller.GetUserNodeSetNames();
            string[] surfaceNames = _controller.GetUserSurfaceNames();
            if (referencePointNames == null) referencePointNames = new string[0];
            if (nodeSetNames == null) nodeSetNames = new string[0];
            if (surfaceNames == null) surfaceNames = new string[0];
            //
            if (_constraintNames == null)
                throw new CaeException("The constraint names must be defined first.");
            // Populate list view
            PopulateListOfConstraints(referencePointNames, nodeSetNames, surfaceNames);
            // Create new constraint
            if (_constraintToEditName == null)
            {
                lvTypes.Enabled = true;
                _viewConstraint = null;
            }
            // Edit existing constraint
            else
            {
                // Get and convert a converted constraint back to selection
                Constraint = _controller.GetConstraint(_constraintToEditName); // to clone
                // Convert the constraint to internal to hide it
                ConstraintInternal(true);
                //
                if (Constraint is RigidBody rb && rb.CreationData != null) rb.RegionType = RegionTypeEnum.Selection;
                else if (Constraint is Tie tie)
                {
                    if (tie.MasterCreationData != null) tie.MasterRegionType = RegionTypeEnum.Selection;
                    if (tie.SlaveCreationData != null) tie.SlaveRegionType = RegionTypeEnum.Selection;
                }
                // Select the appropriate constraint in the list view - disable event SelectedIndexChanged
                _lvTypesSelectedIndexChangedEventActive = false;
                if (_viewConstraint is ViewRigidBody) lvTypes.Items[0].Selected = true;
                else if (_viewConstraint is ViewTie) lvTypes.Items[1].Selected = true;
                lvTypes.Enabled = false;
                _lvTypesSelectedIndexChangedEventActive = true;
                //
                if (_viewConstraint is ViewRigidBody vrb)
                {
                    CheckMissingValueRef(ref referencePointNames, vrb.ReferencePointName, s => { vrb.ReferencePointName = s; });
                    //
                    if (vrb.SlaveRegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                    else if (vrb.SlaveRegionType == RegionTypeEnum.NodeSetName.ToFriendlyString())
                        CheckMissingValueRef(ref nodeSetNames, vrb.NodeSetName, s => { vrb.NodeSetName = s; });
                    else if (vrb.SlaveRegionType == RegionTypeEnum.SurfaceName.ToFriendlyString())
                        CheckMissingValueRef(ref surfaceNames, vrb.SurfaceName, s => { vrb.SurfaceName = s; });
                    else throw new NotSupportedException();
                    //
                    vrb.PopululateDropDownLists(referencePointNames, nodeSetNames, surfaceNames);
                    // Context menu strip
                    propertyGrid.ContextMenuStrip = null;
                }
                else if (_viewConstraint is ViewTie vt)
                {
                    // Master
                    if (vt.MasterRegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                    else CheckMissingValueRef(ref surfaceNames, vt.MasterSurfaceName, s => { vt.MasterSurfaceName = s; });
                    // Slave
                    if (vt.SlaveRegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                    else CheckMissingValueRef(ref surfaceNames, vt.SlaveSurfaceName, s => { vt.SlaveSurfaceName = s; });
                    //
                    vt.PopululateDropDownLists(surfaceNames);
                    // Context menu strip
                    propertyGrid.ContextMenuStrip = cmsPropertyGrid;
                }
                else throw new NotSupportedException();
                //
                propertyGrid.SelectedObject = _viewConstraint;
                propertyGrid.Select();
            }
            _selectedPropertyGridItemChangedEventActive = true;
            //
            SetSelectItem();
            //
            ShowHideSelectionForm();
            //
            HighlightConstraint(); // must be here if called from the menu
            //
            return true;
        }
        

        // Methods                                                                                                                  
        private void PopulateListOfConstraints(string[] referencePointNames, string[] nodeSetNames, string[] surfaceNames)
        {
            Color color = _controller.Settings.Pre.ConstraintSymbolColor;
            // Populate list view                                                                               
            ListViewItem item;
            // Rigid body   
            item = new ListViewItem("Rigid body");
            if (referencePointNames.Length > 0)
            {
                RigidBody rb = new RigidBody(GetConstraintName("Rigid-body-"), referencePointNames[0], "", RegionTypeEnum.Selection);
                ViewRigidBody vrb = new ViewRigidBody(rb);
                vrb.PopululateDropDownLists(referencePointNames, nodeSetNames, surfaceNames);
                vrb.Color = color;
                item.Tag = vrb;
            }
            else item.Tag = new ViewError("There is no reference point defined for the rigid body constraint definition.");
            lvTypes.Items.Add(item);
            // Tie          
            item = new ListViewItem("Tie");
            Tie tie = new Tie(GetConstraintName("Tie-"), "", RegionTypeEnum.Selection, "", RegionTypeEnum.Selection);
            ViewTie vt = new ViewTie(tie);
            vt.PopululateDropDownLists(surfaceNames);
            item.Tag = vt;
            vt.MasterColor = color;
            vt.SlaveColor = color;
            lvTypes.Items.Add(item);
        }
        private string GetConstraintName(string namePrefix)
        {
            return NamedClass.GetNewValueName(_constraintNames, namePrefix);
        }
        private void tsmiSwapMasterSlave_Click(object sender, EventArgs e)
        {
            //if (propertyGrid.SelectedObject is ViewTie vt)
            //{
            //    string tmp = vt.SlaveRegionName;
            //    vt.SlaveRegionName = vt.MasterRegionName;
            //    vt.MasterRegionName = tmp;
            //    //
            //    propertyGrid.Refresh();
            //    //
            //    OnPropertyGridSelectedGridItemChanged();    // highlight
            //    _propertyItemChanged = true;
            //}
        }
        private void HighlightConstraint()
        {
            try
            {
                if (propertyGrid.SelectedGridItem == null || propertyGrid.SelectedGridItem.PropertyDescriptor == null) return;
                //
                string property = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
                //
                _controller.ClearSelectionHistory();
                //
                if (_viewConstraint == null) { }
                else if (Constraint is RigidBody rb)
                {
                    // Master
                    _controller.Highlight3DObjects(new object[] { rb.ReferencePointName });
                    // Slave
                    HighlightRegion(rb.SlaveRegionType, rb.SlaveRegionName, rb.CreationData, false, true);                  // slave
                }
                else if (Constraint is Tie tie)
                {
                    if (property == nameof(ViewTie.MasterRegionType))
                    {
                        HighlightRegion(tie.MasterRegionType, tie.MasterRegionName, tie.MasterCreationData, true, false);   // master
                    }
                    else if(property == nameof(ViewTie.SlaveRegionType))
                    {
                        HighlightRegion(tie.SlaveRegionType, tie.SlaveRegionName, tie.SlaveCreationData, true, true);       // slave
                    }
                    else
                    {
                        HighlightRegion(tie.MasterRegionType, tie.MasterRegionName, tie.MasterCreationData, true, false);   // master
                        HighlightRegion(tie.SlaveRegionType, tie.SlaveRegionName, tie.SlaveCreationData, false, true);      // slave
                    }
                }
                else throw new NotSupportedException();
            }
            catch { }
        }
        private void HighlightRegion(RegionTypeEnum regionType, string regionName, Selection creationData,
                                     bool clear, bool useSecondaryHighlightColor)
        {
            if (regionType == RegionTypeEnum.NodeSetName) _controller.HighlightNodeSets(new string[] { regionName }, useSecondaryHighlightColor);
            else if (regionType == RegionTypeEnum.SurfaceName) _controller.HighlightSurfaces(new string[] { regionName }, useSecondaryHighlightColor);
            else if (regionType == RegionTypeEnum.Selection)
            {
                SetSelectItem();
                //
                if (creationData != null)
                {
                    _controller.Selection = creationData.DeepClone();
                    _controller.HighlightSelection(clear, useSecondaryHighlightColor);
                }
            }
        }
        private void ShowHideSelectionForm()
        {
            //
            if (propertyGrid.SelectedGridItem == null || propertyGrid.SelectedGridItem.PropertyDescriptor == null) return;
            //
            string property = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
            //
            if (Constraint != null)
            {
                if (Constraint is RigidBody rb && rb.RegionType == RegionTypeEnum.Selection)
                    ItemSetDataEditor.SelectionForm.ShowIfHidden(this.Owner);
                else if (Constraint is Tie tie)
                {
                    if (property == nameof(ViewTie.MasterRegionType) &&  tie.MasterRegionType == RegionTypeEnum.Selection)
                        ItemSetDataEditor.SelectionForm.ShowIfHidden(this.Owner);
                    else if (property == nameof(ViewTie.SlaveRegionType) && tie.SlaveRegionType == RegionTypeEnum.Selection)
                        ItemSetDataEditor.SelectionForm.ShowIfHidden(this.Owner);
                    else ItemSetDataEditor.SelectionForm.Hide();
                }
                else ItemSetDataEditor.SelectionForm.Hide();
            }
            else ItemSetDataEditor.SelectionForm.Hide();
        }
        private void SetSelectItem()
        {
            if (Constraint == null) { }
            else if (Constraint is RigidBody) _controller.SetSelectItemToGeometry();
            else if (Constraint is Tie) _controller.SetSelectItemToSurface();
            else throw new NotSupportedException();
        }
        private void ConstraintInternal(bool toInternal)
        {
            if (_constraintToEditName != null)
            {
                // Convert the constraint from/to internal to hide/show it
                _controller.GetConstraint(_constraintToEditName).Internal = toInternal;
                _controller.Update(UpdateType.RedrawSymbols);
            }
        }
        //
        public void SelectionChanged(int[] ids)
        {
            if (propertyGrid.SelectedGridItem == null || propertyGrid.SelectedGridItem.PropertyDescriptor == null) return;
            //
            string property = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
            //
            bool changed = false;
            if (Constraint != null)
            {
                if (Constraint is RigidBody rb)
                {
                    if (rb.RegionType == RegionTypeEnum.Selection)
                    {
                        rb.CreationIds = ids;
                        rb.CreationData = _controller.Selection.DeepClone();
                        changed = true;
                    }
                }
                else if (Constraint is Tie tie) 
                {
                    if (property == nameof(ViewTie.MasterRegionType) && tie.MasterRegionType == RegionTypeEnum.Selection)
                    {
                        tie.MasterCreationIds = ids;
                        tie.MasterCreationData = _controller.Selection.DeepClone();
                        changed = true;
                    }
                    else if (property == nameof(ViewTie.SlaveRegionType) && tie.SlaveRegionType == RegionTypeEnum.Selection)
                    {
                        tie.SlaveCreationIds = ids;
                        tie.SlaveCreationData = _controller.Selection.DeepClone();
                        changed = true;
                    }
                }
                else throw new NotSupportedException();
                //
                if (changed)
                {
                    propertyGrid.Refresh();
                    //
                    _propertyItemChanged = true;
                }
            }
        }
        
        // IFormHighlight
        public void Highlight()
        {
            HighlightConstraint();
        }

        // IFormItemSetDataParent
        public bool IsSelectionGeometryBased()
        {
            // Prepare ItemSetDataEditor - prepare Geometry or Mesh based selection
            if (propertyGrid.SelectedGridItem == null || propertyGrid.SelectedGridItem.PropertyDescriptor == null) return true;
            //
            string property = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
            //
            if (Constraint != null)
            {
                if (Constraint is RigidBody rb)
                {
                    if (rb.CreationData != null) return rb.CreationData.IsGeometryBased();
                    else return true;
                }
                else if (Constraint is Tie tie)
                {
                    if (property == nameof(ViewTie.MasterRegionType) && tie.MasterRegionType == RegionTypeEnum.Selection)
                    {
                        if (tie.MasterCreationData != null) return tie.MasterCreationData.IsGeometryBased();
                        else return true;
                    }
                    else if (property == nameof(ViewTie.SlaveRegionType) && tie.SlaveRegionType == RegionTypeEnum.Selection)
                    {
                        if (tie.SlaveCreationData != null) return tie.SlaveCreationData.IsGeometryBased();
                        else return true;
                    }
                }
                else throw new NotSupportedException();
            }
            return true;
        }
    }
}
