using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeGlobals;
using System.Windows.Forms;

namespace PrePoMax.Forms
{
    class FrmConstraint : UserControls.FrmPropertyListView, IFormBase
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
                SetSelectItem();
                //
                ShowHideSelectionForm();
                //
                propertyGrid.SelectedObject = itemTag;
                propertyGrid.Select();
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
            if (property == nameof(_viewConstraint.RegionType)) ShowHideSelectionForm();
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
            if ((_constraintToEditName == null && 
                 _constraintNames.Contains(_viewConstraint.Name)) ||    // named to existing name
                (_viewConstraint.Name != _constraintToEditName &&
                 _constraintNames.Contains(_viewConstraint.Name)))      // renamed to existing name
                throw new CaeException("The selected constraint name already exists.");
            //
            if (_viewConstraint is ViewTie vt)
            {
                // Check for errors with constructor
                var tie = new Tie(vt.Name, vt.SlaveSurfaceName, vt.MasterSurfaceName);
            }
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
                // Get and convert a converted load back to selection
                Constraint = _controller.GetConstraint(_constraintToEditName);    // to clone
                if (Constraint is RigidBody rb && rb.CreationData != null) rb.RegionType = RegionTypeEnum.Selection;
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
                    if (vrb.RegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                    else if (vrb.RegionType == RegionTypeEnum.NodeSetName.ToFriendlyString())
                        CheckMissingValueRef(ref nodeSetNames, vrb.NodeSetName, s => { vrb.NodeSetName = s; });
                    else if (vrb.RegionType == RegionTypeEnum.SurfaceName.ToFriendlyString())
                        CheckMissingValueRef(ref surfaceNames, vrb.SurfaceName, s => { vrb.SurfaceName = s; });
                    else throw new NotSupportedException();
                    //
                    vrb.PopululateDropDownLists(referencePointNames, nodeSetNames, surfaceNames);
                    // Context menu strip
                    propertyGrid.ContextMenuStrip = null;
                }
                else if (_viewConstraint is ViewTie vt)
                {
                    CheckMissingValueRef(ref surfaceNames, vt.SlaveSurfaceName, s => { vt.SlaveSurfaceName = s; });
                    CheckMissingValueRef(ref surfaceNames, vt.MasterSurfaceName, s => { vt.MasterSurfaceName = s; });
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
            // Populate list view                                                                               
            ListViewItem item;
            ViewTie vt = null;
            // Rigid body
            item = new ListViewItem("Rigid body");
            if (referencePointNames.Length > 0)
            {
                RigidBody rb = new RigidBody(GetConstraintName("Rigid-body-"), referencePointNames[0], "", RegionTypeEnum.Selection);
                ViewRigidBody vrb = new ViewRigidBody(rb);
                vrb.PopululateDropDownLists(referencePointNames, nodeSetNames, surfaceNames);
                item.Tag = vrb;
            }
            else item.Tag = new ViewError("There is no reference point defined for the rigid body constraint definition.");
            lvTypes.Items.Add(item);
            // Tie
            item = new ListViewItem("Tie");
            if (surfaceNames.Length >= 2)
            {
                vt = new ViewTie(new Tie(GetConstraintName("Tie-"), surfaceNames[0], surfaceNames[1]));
                vt.PopululateDropDownLists(surfaceNames);
                item.Tag = vt;
            }
            else item.Tag = new ViewError("At least two surfaces are needed for the tie constraint definition.");
            lvTypes.Items.Add(item);
        }
        private string GetConstraintName(string namePrefix)
        {
            return NamedClass.GetNewValueName(_constraintNames, namePrefix);
        }
        private void tsmiSwapMasterSlave_Click(object sender, EventArgs e)
        {
            if (propertyGrid.SelectedObject is ViewTie vt)
            {
                string tmp = vt.SlaveSurfaceName;
                vt.SlaveSurfaceName = vt.MasterSurfaceName;
                vt.MasterSurfaceName = tmp;
                propertyGrid.Refresh();

                OnPropertyGridSelectedGridItemChanged();    // highlight
                _propertyItemChanged = true;
            }
        }
        private void HighlightConstraint()
        {
            try
            {
                if (propertyGrid.SelectedGridItem == null) return;
                //
                _controller.ClearSelectionHistory();
                //
                if (_viewConstraint == null) { }
                else if (Constraint is RigidBody rb)
                {
                    _controller.Highlight3DObjects(new object[] { rb.ReferencePointName });
                    if (rb.RegionType == RegionTypeEnum.NodeSetName ||
                        rb.RegionType == RegionTypeEnum.SurfaceName)
                    {
                        _controller.Highlight3DObjects(new object[] { rb.RegionName }, false);
                    }
                    else if (rb.RegionType == RegionTypeEnum.Selection)
                    {
                        SetSelectItem();
                        //
                        if (rb.CreationData != null)
                        {
                            _controller.Selection = rb.CreationData.DeepClone();
                            _controller.HighlightSelection(false);
                        }
                    }
                    else throw new NotImplementedException();
                }
                else if (Constraint is Tie tie)
                {
                    _controller.Highlight3DObjects(new object[] { tie.SlaveSurfaceName });
                    _controller.Highlight3DObjects(new object[] { tie.MasterSurfaceName }, false);
                }
                else throw new NotSupportedException();
            }
            catch { }
        }
        private void ShowHideSelectionForm()
        {
            if (Constraint != null && Constraint is RigidBody rb && rb.RegionType == RegionTypeEnum.Selection)
                ItemSetDataEditor.SelectionForm.ShowIfHidden(this.Owner);
            else
                ItemSetDataEditor.SelectionForm.Hide();
        }
        private void SetSelectItem()
        {
            if (Constraint is null) { }
            else if (Constraint is RigidBody) _controller.SetSelectItemToSurface();
            else if (Constraint is Tie) _controller.SetSelectItemToSurface();
        }
        //
        public void SelectionChanged(int[] ids)
        {
            if (Constraint != null)
            {
                if (Constraint is RigidBody rb)
                {
                    if (rb.RegionType == RegionTypeEnum.Selection)
                    {
                        rb.CreationIds = ids;
                        rb.CreationData = _controller.Selection.DeepClone();
                        //
                        propertyGrid.Refresh();
                        //
                        _propertyItemChanged = true;
                    }
                }
                else if (Constraint is Tie tie) { }
                else throw new NotSupportedException();
            }
        }
    }
}
