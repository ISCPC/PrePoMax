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
            get { return _viewBc.GetBase(); }
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

            _controller = controller;
            _viewBc = null;

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
            this.gbProperties.Size = new System.Drawing.Size(310, 322);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Size = new System.Drawing.Size(298, 294);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(160, 436);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(241, 436);
            // 
            // btnOkAddNew
            // 
            this.btnOkAddNew.Location = new System.Drawing.Point(79, 436);
            // 
            // FrmBC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 471);
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
                propertyGrid.SelectedObject = lvTypes.SelectedItems[0].Tag;
                propertyGrid.Select();
            }
        }
        protected override void OnPropertyGridSelectedGridItemChanged()
        {
            // Highlight
            object value = propertyGrid.SelectedGridItem.Value;
            if (value != null)
            {
                string valueString = value.ToString();

                if (propertyGrid.SelectedObject == null) { }
                else if (propertyGrid.SelectedObject is ViewDisplacementRotation)
                {
                    object[] objects;
                    ViewDisplacementRotation vdr = propertyGrid.SelectedObject as ViewDisplacementRotation;
                    if (valueString == vdr.NodeSetName) objects = new object[] { vdr.NodeSetName };
                    else if (valueString == vdr.SurfaceName) objects = new object[] { vdr.SurfaceName };
                    else if (valueString == vdr.ReferencePointName) objects = new object[] { vdr.ReferencePointName };
                    else objects = null;

                    _controller.Highlight3DObjects(objects);
                }
                else if (propertyGrid.SelectedObject is ViewSubmodel)
                {
                    object[] objects;
                    ViewSubmodel vsm = propertyGrid.SelectedObject as ViewSubmodel;
                    if (valueString == vsm.NodeSetName) objects = new object[] { vsm.NodeSetName };
                    else if (valueString == vsm.SurfaceName) objects = new object[] { vsm.SurfaceName };
                    else objects = null;

                    _controller.Highlight3DObjects(objects);
                }
                else throw new NotImplementedException();
            }
        }
        protected override void Apply()
        {
            _viewBc = (ViewBoundaryCondition)propertyGrid.SelectedObject;

            if ((_boundaryConditionToEditName == null && _boundaryConditionNames.Contains(_viewBc.Name)) ||             // named to existing name
                (_viewBc.Name != _boundaryConditionToEditName && _boundaryConditionNames.Contains(_viewBc.Name)))       // renamed to existing name
                throw new CaeException("The selected boundary condition name already exists.");

            if (_viewBc is ViewDisplacementRotation vdr)
            {
                if (!((DisplacementRotation)vdr.GetBase()).IsProperlyDefined(out string error))
                    throw new CaeException(error);
            }
            else if (_viewBc is ViewSubmodel vsm)
            {
                if (((SubmodelBC)vsm.GetBase()).GetConstrainedDirections().Length == 0)
                    throw new CaeException("At least one degree of freedom must be defined for the boundary condition.");
            }

            if (_boundaryConditionToEditName == null)
            {
                // Create
                _controller.AddBoundaryConditionCommand(_stepName, BoundaryCondition);
            }
            else
            {
                // Replace
                if (_propertyItemChanged)
                    _controller.ReplaceBoundaryConditionCommand(_stepName, _boundaryConditionToEditName, BoundaryCondition);
            }
        }
        protected override bool OnPrepareForm(string stepName, string boundaryConditionToEditName)
        {
            _selectedPropertyGridItemChangedEventActive = false;                             // to prevent clear of the selection

            this.DialogResult = DialogResult.None;      // to prevent the call to frmMain.itemForm_VisibleChanged when minimized
            //this.btnOkAddNew.Visible = boundaryConditionToEditName == null;

            _propertyItemChanged = false;
            _stepName = null;
            _boundaryConditionNames = null;
            _boundaryConditionToEditName = null;
            _viewBc = null;
            lvTypes.Items.Clear();
            propertyGrid.SelectedObject = null;

            _stepName = stepName;
            _boundaryConditionNames = _controller.GetBoundaryConditionNames(stepName);
            _boundaryConditionToEditName = boundaryConditionToEditName;

            string[] nodeSetNames = _controller.GetUserNodeSetNames();
            string[] surfaceNames = _controller.GetSurfaceNames();
            string[] referencePointNames = _controller.GetReferencePointNames();
            if (nodeSetNames == null) nodeSetNames = new string[0];
            if (surfaceNames == null) surfaceNames = new string[0];
            if (referencePointNames == null) referencePointNames = new string[0];

            if (_boundaryConditionNames == null)
                throw new CaeGlobals.CaeException("The boundary condition names must be defined first.");

            // populate list view
            PopulateListOfBCs(nodeSetNames, surfaceNames, referencePointNames);

            if (_boundaryConditionToEditName == null)
            {
                if (nodeSetNames.Length + surfaceNames.Length + referencePointNames.Length == 0)
                    throw new CaeGlobals.CaeException("There is no node set/surface/reference point defined to which a boundary condition could be applied.");

                lvTypes.Enabled = true;
                _viewBc = null;

                if (lvTypes.Items.Count == 1) lvTypes.Items[0].Selected = true;
            }
            else
            {
                BoundaryCondition = _controller.GetBoundaryCondition(stepName, _boundaryConditionToEditName); // to clone and set viewBC

                // select the appropriate boundary condition in the list view - disable event SelectedIndexChanged
                _lvTypesSelectedIndexChangedEventActive = false;
                if (_viewBc is ViewDisplacementRotation) lvTypes.Items[0].Selected = true;
                else if (_viewBc is ViewSubmodel) lvTypes.Items[1].Selected = true;
                lvTypes.Enabled = false;
                _lvTypesSelectedIndexChangedEventActive = true;

                if (_viewBc is ViewDisplacementRotation vdr)
                {
                    // Check for deleted regions
                    if (vdr.RegionType == RegionTypeEnum.NodeSetName.ToFriendlyString())
                        CheckMissingValueRef(ref nodeSetNames, vdr.NodeSetName, s => { vdr.NodeSetName = s; });
                    else if (vdr.RegionType == RegionTypeEnum.SurfaceName.ToFriendlyString())
                        CheckMissingValueRef(ref surfaceNames, vdr.SurfaceName, s => { vdr.SurfaceName = s; });
                    else if (vdr.RegionType == RegionTypeEnum.ReferencePointName.ToFriendlyString())
                        CheckMissingValueRef(ref referencePointNames, vdr.ReferencePointName, s => { vdr.ReferencePointName = s; });
                    else throw new NotSupportedException();

                    vdr.PopululateDropDownLists(nodeSetNames, surfaceNames, referencePointNames);
                }
                else if (_viewBc is ViewSubmodel vsm)
                {
                    // Check for deleted regions
                    if (vsm.RegionType == RegionTypeEnum.NodeSetName.ToFriendlyString())
                        CheckMissingValueRef(ref nodeSetNames, vsm.NodeSetName, s => { vsm.NodeSetName = s; });
                    else if (vsm.RegionType == RegionTypeEnum.SurfaceName.ToFriendlyString())
                        CheckMissingValueRef(ref surfaceNames, vsm.SurfaceName, s => { vsm.SurfaceName = s; });
                    else throw new NotSupportedException();

                    vsm.PopululateDropDownLists(nodeSetNames, surfaceNames);
                }
                else throw new NotSupportedException();

                propertyGrid.SelectedObject = _viewBc;
                propertyGrid.Select();
            }
            _selectedPropertyGridItemChangedEventActive = true;

            return true;
        }


        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string boundaryConditionToEditName)
        {
            return OnPrepareForm(stepName, boundaryConditionToEditName);
        }
        private void PopulateListOfBCs(string[] nodeSetNames, string[] surfaceNames, string[] referencePointNames)
        {
            ListViewItem item = new ListViewItem("Displacement/Rotation");
            ViewDisplacementRotation vdr = null;

            if (nodeSetNames.Length > 0)
                vdr = new ViewDisplacementRotation(new DisplacementRotation(GetBoundaryConditionName(), nodeSetNames[0], RegionTypeEnum.NodeSetName));
            else if (surfaceNames.Length > 0)
                vdr = new ViewDisplacementRotation(new DisplacementRotation(GetBoundaryConditionName(), surfaceNames[0], RegionTypeEnum.SurfaceName));
            else if (referencePointNames.Length > 0)
                vdr = new ViewDisplacementRotation(new DisplacementRotation(GetBoundaryConditionName(), referencePointNames[0], RegionTypeEnum.ReferencePointName));

            if (vdr != null)
            {
                vdr.PopululateDropDownLists(nodeSetNames, surfaceNames, referencePointNames);
                item.Tag = vdr;
            }
            else item.Tag = new ViewError("There is no node set/surface/reference point defined to which a boundary condition could be applied.");

            lvTypes.Items.Add(item);

            item = new ListViewItem("Submodel");
            ViewSubmodel vsm = null;

            if (nodeSetNames.Length > 0)
                vsm = new ViewSubmodel(new SubmodelBC(GetBoundaryConditionName(), nodeSetNames[0], RegionTypeEnum.NodeSetName));
            else if (surfaceNames.Length > 0)
                vsm = new ViewSubmodel(new SubmodelBC(GetBoundaryConditionName(), surfaceNames[0], RegionTypeEnum.SurfaceName));

            if (vsm != null)
            {
                vsm.PopululateDropDownLists(nodeSetNames, surfaceNames);
                item.Tag = vsm;
            }
            else item.Tag = new ViewError("There is no node set/surface defined to which a boundary condition could be applied.");

            lvTypes.Items.Add(item);
        }
        private string GetBoundaryConditionName()
        {
            return NamedClass.GetNewValueName(_boundaryConditionNames, "BC-");
        }
    }
}
