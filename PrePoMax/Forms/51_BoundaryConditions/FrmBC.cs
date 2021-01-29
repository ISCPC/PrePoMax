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
    class FrmBC : UserControls.FrmPropertyListView, IFormBase, IFormItemSetDataParent, IFormHighlight
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
                if (value is FixedBC fix) _viewBc = new ViewFixedBC(fix.DeepClone());
                else if (value is DisplacementRotation dr) _viewBc = new ViewDisplacementRotation(dr.DeepClone());
                else if (value is SubmodelBC sm) _viewBc = new ViewSubmodelBC(sm.DeepClone());
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
        }
        private void InitializeComponent()
        {
            this.gbType.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbType
            // 
            this.gbType.Size = new System.Drawing.Size(310, 100);
            // 
            // lvTypes
            // 
            this.lvTypes.Size = new System.Drawing.Size(298, 72);
            // 
            // gbProperties
            // 
            this.gbProperties.Location = new System.Drawing.Point(12, 118);
            this.gbProperties.Size = new System.Drawing.Size(310, 402);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Size = new System.Drawing.Size(298, 374);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(160, 526);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(241, 526);
            // 
            // btnOkAddNew
            // 
            this.btnOkAddNew.Location = new System.Drawing.Point(79, 526);
            // 
            // FrmBC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 561);
            this.Name = "FrmBC";
            this.Text = "Edit Boundary Condition";
            this.gbType.ResumeLayout(false);
            this.gbProperties.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Overrides                                                                                                                
        protected override void OnListViewTypeSelectedIndexChanged()
        {            
            if (lvTypes.SelectedItems != null && lvTypes.SelectedItems.Count > 0)
            {
                object itemTag = lvTypes.SelectedItems[0].Tag;
                if (itemTag is ViewError) _viewBc = null;
                else if (itemTag is ViewFixedBC fix) _viewBc = fix;
                else if (itemTag is ViewDisplacementRotation vdr) _viewBc = vdr;
                else if (itemTag is ViewSubmodelBC vsm) _viewBc = vsm;
                else throw new NotImplementedException();
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
            else if (_viewBc is ViewFixedBC fix &&
                     (property == nameof(fix.NodeSetName) ||
                      property == nameof(fix.ReferencePointName) ||
                      property == nameof(fix.SurfaceName)))
            {
                HighlightBoundaryCondition();
            }
            else if (_viewBc is ViewDisplacementRotation vdr &&
                     (property == nameof(vdr.NodeSetName) ||
                      property == nameof(vdr.ReferencePointName) ||
                      property == nameof(vdr.SurfaceName)))
            {
                HighlightBoundaryCondition();
            }
            else if (_viewBc is ViewSubmodelBC vsm && 
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
                _boundaryConditionToEditName = null; // prevents the execution of toInternal in OnHideOrClose
            }
            // Convert the boundary condition from internal to show it
            else
            {
                BoundaryConditionInternal(false);
            }
            // If all is successful close the ItemSetSelectionForm - except for OKAddNew
            if (!onOkAddNew) ItemSetDataEditor.SelectionForm.Hide();
        }
        protected override void OnHideOrClose()
        {
            // Close the ItemSetSelectionForm
            ItemSetDataEditor.SelectionForm.Hide();
            // Convert the boundary condition from internal to show it
            BoundaryConditionInternal(false);
            //
            base.OnHideOrClose();
        }
        protected override bool OnPrepareForm(string stepName, string boundaryConditionToEditName)
        {
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
                throw new CaeException("The boundary condition names must be defined first.");
            // Populate list view
            PopulateListOfBCs(nodeSetNames, surfaceNames, referencePointNames);
            // Create new boundary condition
            if (_boundaryConditionToEditName == null)
            {
                lvTypes.Enabled = true;
                _viewBc = null;
                //
                HighlightBoundaryCondition(); // must be here if called from the menu
            }
            // Edit existing boundary condition
            else
            {
                // Get and convert a converted load back to selection
                BoundaryCondition = _controller.GetBoundaryCondition(stepName, _boundaryConditionToEditName); // to clone
                if (BoundaryCondition.CreationData != null) BoundaryCondition.RegionType = RegionTypeEnum.Selection;
                // Convert the boundary condition to internal to hide it
                BoundaryConditionInternal(true);
                //
                int selectedId;
                if (_viewBc is ViewFixedBC fix)
                {
                    selectedId = 0;
                    // Check for deleted entities
                    if (fix.RegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                    else if (fix.RegionType == RegionTypeEnum.NodeSetName.ToFriendlyString())
                        CheckMissingValueRef(ref nodeSetNames, fix.NodeSetName, s => { fix.NodeSetName = s; });
                    else if (fix.RegionType == RegionTypeEnum.SurfaceName.ToFriendlyString())
                        CheckMissingValueRef(ref surfaceNames, fix.SurfaceName, s => { fix.SurfaceName = s; });
                    else if (fix.RegionType == RegionTypeEnum.ReferencePointName.ToFriendlyString())
                        CheckMissingValueRef(ref referencePointNames, fix.ReferencePointName, s => { fix.ReferencePointName = s; });
                    else throw new NotSupportedException();
                    //
                    fix.PopululateDropDownLists(nodeSetNames, surfaceNames, referencePointNames);
                }
                else if (_viewBc is ViewDisplacementRotation vdr)
                {
                    selectedId = 1;
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
                else if (_viewBc is ViewSubmodelBC vsm)
                {
                    selectedId = 2;
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
                lvTypes.Items[selectedId].Tag = _viewBc;
                _preselectIndex = selectedId;
            }
            ShowHideSelectionForm();
            //
            return true;
        }


        // Methods                                                                                                                  
        private void PopulateListOfBCs(string[] nodeSetNames, string[] surfaceNames, string[] referencePointNames)
        {
            System.Drawing.Color color = _controller.Settings.Pre.BoundaryConditionSymbolColor;
            ListViewItem item;
            // Fixed
            item = new ListViewItem("Fixed");
            FixedBC fix = new FixedBC(GetBoundaryConditionName("Fixed"), "", RegionTypeEnum.Selection);
            ViewFixedBC vfix = new ViewFixedBC(fix);
            vfix.PopululateDropDownLists(nodeSetNames, surfaceNames, referencePointNames);
            vfix.Color = color;
            item.Tag = vfix;
            lvTypes.Items.Add(item);
            // Displacement/Rotation
            item = new ListViewItem("Displacement/Rotation");
            DisplacementRotation dr = new DisplacementRotation(GetBoundaryConditionName("Displacement_rotation"), 
                                                               "", RegionTypeEnum.Selection);
            ViewDisplacementRotation vdr = new ViewDisplacementRotation(dr);
            vdr.PopululateDropDownLists(nodeSetNames, surfaceNames, referencePointNames);
            vdr.Color = color;
            item.Tag = vdr;
            lvTypes.Items.Add(item);
            // Submodel
            item = new ListViewItem("Submodel");
            SubmodelBC sm = new SubmodelBC(GetBoundaryConditionName("Submodel"), "", RegionTypeEnum.Selection);
            ViewSubmodelBC vsm = new ViewSubmodelBC(sm);
            vsm.PopululateDropDownLists(nodeSetNames, surfaceNames);
            vsm.Color = color;
            item.Tag = vsm;
            lvTypes.Items.Add(item);
        }
        private string GetBoundaryConditionName(string baseName)
        {
            return NamedClass.GetNewValueName(_boundaryConditionNames, baseName + "-");
        }
        private void HighlightBoundaryCondition()
        {
            try
            {
                _controller.ClearSelectionHistory();
                //
                if (_viewBc == null) { }
                else if (BoundaryCondition is FixedBC || BoundaryCondition is DisplacementRotation ||
                         BoundaryCondition is SubmodelBC)
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
            //
            SetSelectItem();
        }
        private void SetSelectItem()
        {
            if (BoundaryCondition != null && BoundaryCondition.RegionType == RegionTypeEnum.Selection)
            {
                if (BoundaryCondition is null) { }
                else if (BoundaryCondition is FixedBC) _controller.SetSelectItemToGeometry();
                else if (BoundaryCondition is DisplacementRotation) _controller.SetSelectItemToGeometry();
                else if (BoundaryCondition is SubmodelBC) _controller.SetSelectItemToGeometry();
                else throw new NotSupportedException();
            }
            else _controller.SetSelectByToOff();
        }
        private void BoundaryConditionInternal(bool toInternal)
        {
            if (_stepName != null && _boundaryConditionToEditName != null)
            {
                // Convert the boundary condition from/to internal to hide/show it
                _controller.GetBoundaryCondition(_stepName, _boundaryConditionToEditName).Internal = toInternal;
                _controller.FeModelUpdate(UpdateType.RedrawSymbols);
            }
        }
        //
        public void SelectionChanged(int[] ids)
        {
            if (BoundaryCondition != null && BoundaryCondition.RegionType == RegionTypeEnum.Selection)
            {
                if (BoundaryCondition is FixedBC || BoundaryCondition is DisplacementRotation ||
                    BoundaryCondition is SubmodelBC)
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

        // IFormHighlight
        public void Highlight()
        {
            HighlightBoundaryCondition();
        }

        public bool IsSelectionGeometryBased()
        {
            // Prepare ItemSetDataEditor - prepare Geometry or Mesh based selection
            return true;
        }
    }
}
