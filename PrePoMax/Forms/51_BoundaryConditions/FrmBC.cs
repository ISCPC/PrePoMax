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
    class FrmBC : UserControls.FrmPropertyListView, IFormBase
    {
        // Variables                                                                                                                
        private string[] _boundaryConditionNames;
        private string _boundaryConditionToEditName;
        private ViewBoundaryCondition _viewBc;
        private Controller _controller;
        

        // Properties                                                                                                               
        public BoundaryCondition BoundaryCondition
        {
            get { return _viewBc != null ? _viewBc.GetBase() : null; }
            set
            {
                if (value is DisplacementRotation) _viewBc = new ViewDisplacementRotation((DisplacementRotation)value.DeepClone());
                else if (value is SubmodelBC) _viewBc = new ViewSubmodel((SubmodelBC)value.DeepClone());
                else throw new NotImplementedException();
            }
        }


        // Constructors                                                                                                             
        public FrmBC(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewBc = null;
            //
            _selectedPropertyGridItemChangedEventActive = true;
        }
        private void InitializeComponent()
        {
            this.gbType.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbProperties
            // 
            this.gbProperties.Size = new System.Drawing.Size(310, 343);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Size = new System.Drawing.Size(298, 315);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(160, 457);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(241, 457);
            // 
            // btnOkAddNew
            // 
            this.btnOkAddNew.Location = new System.Drawing.Point(79, 457);
            // 
            // FrmBC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 492);
            this.Name = "FrmBC";
            this.Text = "Edit Boundary Condition";
            this.gbType.ResumeLayout(false);
            this.gbProperties.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Overrides                                                                                                                
        protected override void OnListViewTypeSelectedIndexChanged()
        {
            if (lvTypes.Enabled && lvTypes.SelectedItems != null && lvTypes.SelectedItems.Count > 0)
            {
                object itemTag = lvTypes.SelectedItems[0].Tag;
                if (itemTag is ViewError) _viewBc = null;
                else if (itemTag is ViewDisplacementRotation vdr) _viewBc = vdr;
                else if (itemTag is ViewSubmodel vsm) _viewBc = vsm;
                else throw new NotImplementedException();
                //
                SetSelectItem();
                //
                ShowHideSelectionForm();
                //
                propertyGrid.SelectedObject = itemTag;
                propertyGrid.Select();
                //
                HighlightBoundaryCondition();
            }
        }
        protected override void OnPropertyGridPropertyValueChanged()
        {
            string property = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
            //
            if (property == nameof(_viewBc.RegionType))
            {
                ShowHideSelectionForm();
                //
                HighlightBoundaryCondition();
            }
            else if (_viewBc is ViewDisplacementRotation vdr &&
                     (property == nameof(vdr.NodeSetName) ||
                      property == nameof(vdr.ReferencePointName) ||
                      property == nameof(vdr.SurfaceName)))
            {
                HighlightBoundaryCondition();
            }
            else if (_viewBc is ViewSubmodel vsm && 
                    (property == nameof(vsm.NodeSetName) ||
                     property == nameof(vsm.SurfaceName)))
            {
                HighlightBoundaryCondition();
            }
            //
            base.OnPropertyGridPropertyValueChanged();
        }
        protected override void OnApply(bool onOkAddNew)
        {
            if (propertyGrid.SelectedObject is ViewError ve) throw new CaeGlobals.CaeException(ve.Message);
            //
            _viewBc = (ViewBoundaryCondition)propertyGrid.SelectedObject;
            //
            if (BoundaryCondition == null) throw new CaeGlobals.CaeException("No boundary condition was selected.");
            //
            if (BoundaryCondition.RegionType == RegionTypeEnum.Selection &&
                (BoundaryCondition.CreationIds == null || BoundaryCondition.CreationIds.Length == 0))
                throw new CaeException("The boundary condition must contain at least one item.");
            //
            if ((_boundaryConditionToEditName == null && 
                 _boundaryConditionNames.Contains(BoundaryCondition.Name)) ||   // named to existing name
                (BoundaryCondition.Name != _boundaryConditionToEditName && 
                 _boundaryConditionNames.Contains(BoundaryCondition.Name)))     // renamed to existing name
                throw new CaeException("The selected boundary condition name already exists.");
            //
            if (BoundaryCondition is DisplacementRotation dr)
            {
                if (!dr.IsProperlyDefined(out string error)) throw new CaeException(error);
            }
            else if (BoundaryCondition is SubmodelBC sm)
            {
                if (sm.GetConstrainedDirections().Length == 0)
                    throw new CaeException("At least one degree of freedom must be defined for the boundary condition.");
            }
            // Create
            if (_boundaryConditionToEditName == null)
            {
                _controller.AddBoundaryConditionCommand(_stepName, BoundaryCondition);
            }
            // Replace
            else if (_propertyItemChanged)
            {
                _controller.ReplaceBoundaryConditionCommand(_stepName, _boundaryConditionToEditName, BoundaryCondition);
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
        protected override bool OnPrepareForm(string stepName, string boundaryConditionToEditName)
        {
            // To prevent clear of the selection
            _selectedPropertyGridItemChangedEventActive = false;
            // To prevent the call to frmMain.itemForm_VisibleChanged when minimized
            this.DialogResult = DialogResult.None;
            this.btnOkAddNew.Visible = boundaryConditionToEditName == null;
            //
            _propertyItemChanged = false;
            _stepName = null;
            _boundaryConditionNames = null;
            _boundaryConditionToEditName = null;
            _viewBc = null;
            lvTypes.Items.Clear();
            propertyGrid.SelectedObject = null;
            //
            _stepName = stepName;
            _boundaryConditionNames = _controller.GetAllBoundaryConditionNames();
            _boundaryConditionToEditName = boundaryConditionToEditName;
            //
            string[] nodeSetNames = _controller.GetUserNodeSetNames();
            string[] surfaceNames = _controller.GetUserSurfaceNames();
            string[] referencePointNames = _controller.GetReferencePointNames();
            if (nodeSetNames == null) nodeSetNames = new string[0];
            if (surfaceNames == null) surfaceNames = new string[0];
            if (referencePointNames == null) referencePointNames = new string[0];
            //
            if (_boundaryConditionNames == null)
                throw new CaeGlobals.CaeException("The boundary condition names must be defined first.");
            // Populate list view
            PopulateListOfBCs(nodeSetNames, surfaceNames, referencePointNames);
            // Create new boundary condition
            if (_boundaryConditionToEditName == null)
            {
                lvTypes.Enabled = true;
                _viewBc = null;
            }
            // Edit existing boundary condition
            else
            {
                // Get and convert a converted load back to selection
                BoundaryCondition = _controller.GetBoundaryCondition(stepName, _boundaryConditionToEditName); // to clone
                if (BoundaryCondition.CreationData != null) BoundaryCondition.RegionType = RegionTypeEnum.Selection;
                // Select the appropriate boundary condition in the list view - disable event SelectedIndexChanged
                _lvTypesSelectedIndexChangedEventActive = false;
                if (_viewBc is ViewDisplacementRotation) lvTypes.Items[0].Selected = true;
                else if (_viewBc is ViewSubmodel) lvTypes.Items[1].Selected = true;
                else throw new NotSupportedException();
                //
                lvTypes.Enabled = false;
                _lvTypesSelectedIndexChangedEventActive = true;
                //
                if (_viewBc is ViewDisplacementRotation vdr)
                {
                    // Check for deleted entities
                    if (vdr.RegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                    else if (vdr.RegionType == RegionTypeEnum.NodeSetName.ToFriendlyString())
                        CheckMissingValueRef(ref nodeSetNames, vdr.NodeSetName, s => { vdr.NodeSetName = s; });
                    else if (vdr.RegionType == RegionTypeEnum.SurfaceName.ToFriendlyString())
                        CheckMissingValueRef(ref surfaceNames, vdr.SurfaceName, s => { vdr.SurfaceName = s; });
                    else if (vdr.RegionType == RegionTypeEnum.ReferencePointName.ToFriendlyString())
                        CheckMissingValueRef(ref referencePointNames, vdr.ReferencePointName, s => { vdr.ReferencePointName = s; });
                    else throw new NotSupportedException();
                    //
                    vdr.PopululateDropDownLists(nodeSetNames, surfaceNames, referencePointNames);
                }
                else if (_viewBc is ViewSubmodel vsm)
                {
                    // Check for deleted entities
                    if (vsm.RegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                    else if (vsm.RegionType == RegionTypeEnum.NodeSetName.ToFriendlyString())
                        CheckMissingValueRef(ref nodeSetNames, vsm.NodeSetName, s => { vsm.NodeSetName = s; });
                    else if (vsm.RegionType == RegionTypeEnum.SurfaceName.ToFriendlyString())
                        CheckMissingValueRef(ref surfaceNames, vsm.SurfaceName, s => { vsm.SurfaceName = s; });
                    else throw new NotSupportedException();
                    //
                    vsm.PopululateDropDownLists(nodeSetNames, surfaceNames);
                }
                else throw new NotSupportedException();
                //
                propertyGrid.SelectedObject = _viewBc;
                propertyGrid.Select();
            }
            _selectedPropertyGridItemChangedEventActive = true;
            //
            SetSelectItem();
            //
            ShowHideSelectionForm();
            //
            HighlightBoundaryCondition(); // must be here if called from the menu
            //
            return true;
        }


        // Methods                                                                                                                  
        private void PopulateListOfBCs(string[] nodeSetNames, string[] surfaceNames, string[] referencePointNames)
        {
            ListViewItem item;
            // Displacement/Rotation"
            item = new ListViewItem("Displacement/Rotation");
            DisplacementRotation dr = new DisplacementRotation(GetBoundaryConditionName(), "", RegionTypeEnum.Selection);
            ViewDisplacementRotation vdr = new ViewDisplacementRotation(dr);
            vdr.PopululateDropDownLists(nodeSetNames, surfaceNames, referencePointNames);
            item.Tag = vdr;
            lvTypes.Items.Add(item);
            // Submodel
            item = new ListViewItem("Submodel");
            SubmodelBC sm = new SubmodelBC(GetBoundaryConditionName(), "", RegionTypeEnum.Selection);
            ViewSubmodel vsm = new ViewSubmodel(sm);
            vsm.PopululateDropDownLists(nodeSetNames, surfaceNames);
            item.Tag = vsm;
            lvTypes.Items.Add(item);
        }
        private string GetBoundaryConditionName()
        {
            return NamedClass.GetNewValueName(_boundaryConditionNames, "BC-");
        }
        private void HighlightBoundaryCondition()
        {
            try
            {
                _controller.ClearSelectionHistoryAndSelectionChanged();
                //
                if (_viewBc == null) { }
                else if (BoundaryCondition is DisplacementRotation || BoundaryCondition is SubmodelBC)
                {
                    if (BoundaryCondition.RegionType == RegionTypeEnum.NodeSetName ||
                        BoundaryCondition.RegionType == RegionTypeEnum.ReferencePointName ||
                        BoundaryCondition.RegionType == RegionTypeEnum.SurfaceName)
                    {
                        _controller.Highlight3DObjects(new object[] { BoundaryCondition.RegionName });
                    }
                    else if (BoundaryCondition.RegionType == RegionTypeEnum.Selection)
                    {
                        SetSelectItem();
                        //
                        if (BoundaryCondition.CreationData != null)
                        {
                            _controller.Selection = BoundaryCondition.CreationData.DeepClone();
                            _controller.HighlightSelection();
                        }
                    }
                    else throw new NotImplementedException();
                }
                else throw new NotSupportedException();
            }
            catch { }
        }
        private void ShowHideSelectionForm()
        {
            if (BoundaryCondition != null && BoundaryCondition.RegionType == RegionTypeEnum.Selection)
                ItemSetDataEditor.SelectionForm.ShowIfHidden(this.Owner);
            else
                ItemSetDataEditor.SelectionForm.Hide();
        }
        private void SetSelectItem()
        {
            if (BoundaryCondition is null) { }
            else if (BoundaryCondition is DisplacementRotation) _controller.SetSelectItemToSurface();
            else if (BoundaryCondition is SubmodelBC) _controller.SetSelectItemToSurface();
        }
        //
        public void SelectionChanged(int[] ids)
        {
            if (BoundaryCondition != null && BoundaryCondition.RegionType == RegionTypeEnum.Selection)
            {
                if (BoundaryCondition is DisplacementRotation || BoundaryCondition is SubmodelBC)
                {
                    BoundaryCondition.CreationIds = ids;
                    BoundaryCondition.CreationData = _controller.Selection.DeepClone();
                    //
                    propertyGrid.Refresh();
                    //
                    _propertyItemChanged = true;
                }
                else throw new NotSupportedException();
            }
        }
    }
}
