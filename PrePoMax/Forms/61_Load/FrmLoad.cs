using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeModel;
using CaeGlobals;

namespace PrePoMax.Forms
{
    class FrmLoad : UserControls.FrmPropertyListView, IFormBase
    {
        // Variables                                                                                                                
        private string[] _loadNames;
        private string _loadToEditName;
        private ViewLoad _viewLoad;
        private Controller _controller;


        // Properties                                                                                                               
        // SetLoad and GetLoad to distinguish from Load event
        public Load GetLoad 
        {
            get { return _viewLoad.GetBase(); }
        }
        public Load SetLoad
        {
            set
            {
                var clone = value.DeepClone();
                if (clone is DLoad dl) _viewLoad = new ViewDLoad(dl);
                else if (clone is CLoad cl) _viewLoad = new ViewCLoad(cl);
                else if (clone is MomentLoad ml) _viewLoad = new ViewMomentLoad(ml);
                else if (clone is STLoad stl) _viewLoad = new ViewSTLoad(stl);
                else if (clone is GravityLoad gl) _viewLoad = new ViewGravityLoad(gl);
                else if (clone is CentrifLoad cfl) _viewLoad = new ViewCentrifLoad(cfl);
                else if (clone is PreTensionLoad ptl) _viewLoad = new ViewPreTensionLoad(ptl);
                else throw new NotImplementedException();
            }
        }


        // Constructors                                                                                                             
        public FrmLoad(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewLoad = null;
            //
            _selectedPropertyGridItemChangedEventActive = true;
            //
            this.Height = 600;
        }
        private void InitializeComponent()
        {
            this.gbType.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbType
            // 
            this.gbType.Size = new System.Drawing.Size(310, 161);
            // 
            // lvTypes
            // 
            this.lvTypes.Size = new System.Drawing.Size(298, 133);
            // 
            // gbProperties
            // 
            this.gbProperties.Location = new System.Drawing.Point(12, 179);
            this.gbProperties.Size = new System.Drawing.Size(310, 291);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Size = new System.Drawing.Size(298, 263);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(160, 476);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(241, 476);
            // 
            // btnOkAddNew
            // 
            this.btnOkAddNew.Location = new System.Drawing.Point(79, 476);
            // 
            // FrmLoad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 511);
            this.Name = "FrmLoad";
            this.Text = "Edit Load";
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
            object value = propertyGrid.SelectedGridItem.Value;
            if (value != null)
            {
                string valueString = value.ToString();
                object[] objects = null;
                //
                if (propertyGrid.SelectedObject == null) { }
                else if (propertyGrid.SelectedObject is ViewCLoad vcl)
                {
                    if (valueString == vcl.NodeSetName) objects = new object[] { vcl.NodeSetName };
                    else if (valueString == vcl.ReferencePointName) objects = new object[] { vcl.ReferencePointName };
                    else objects = null;
                }
                else if (propertyGrid.SelectedObject is ViewMomentLoad vml)
                {
                    if (valueString == vml.ReferencePointName) objects = new object[] { vml.ReferencePointName };
                    else objects = null;
                }
                else if (propertyGrid.SelectedObject is ViewDLoad vdl)
                {
                    if (valueString == vdl.SurfaceName) objects = new object[] { vdl.SurfaceName };
                    else objects = null;
                }
                else if (propertyGrid.SelectedObject is ViewSTLoad vstl)
                {
                    if (valueString == vstl.SurfaceName) objects = new object[] { vstl.SurfaceName };
                    else objects = null;
                }
                else if (propertyGrid.SelectedObject is ViewGravityLoad vgl)
                {
                    if (valueString == vgl.PartName) objects = new object[] { vgl.PartName };
                    else if (valueString == vgl.ElementSetName) objects = new object[] { vgl.ElementSetName };
                    else objects = null;
                }
                else if (propertyGrid.SelectedObject is ViewCentrifLoad vcfl)
                {
                    if (valueString == vcfl.PartName) objects = new object[] { vcfl.PartName };
                    else if (valueString == vcfl.ElementSetName) objects = new object[] { vcfl.ElementSetName };
                    else objects = null;
                }
                else if (propertyGrid.SelectedObject is ViewPreTensionLoad vptl)
                {
                    if (valueString == vptl.SurfaceName) objects = new object[] { vptl.SurfaceName };
                    else objects = null;
                }
                else if (propertyGrid.SelectedObject is PrePoMax.Forms.ViewError)
                {
                }
                else throw new NotImplementedException();
                //
                _controller.Highlight3DObjects(objects);
            }
        }
        protected override void Apply()
        {
            if (propertyGrid.SelectedObject == null) throw new CaeException("No item selected.");
            //
            _viewLoad = (ViewLoad)propertyGrid.SelectedObject;
            //
            if ((_loadToEditName == null && _loadNames.Contains(_viewLoad.Name)) ||                 // named to existing name
                (_viewLoad.Name != _loadToEditName && _loadNames.Contains(_viewLoad.Name)))         // renamed to existing name
                throw new CaeException("The selected load name already exists.");
            //
            // check for 0 values
            if (_viewLoad is ViewCLoad vcl)
            {
                if (vcl.F1 == 0 && vcl.F2 == 0 && vcl.F3 == 0)
                    throw new CaeException("At least one load component must not be equal to 0.");
            }
            else if (_viewLoad is ViewMomentLoad vml)
            {
                if (vml.F1 == 0 && vml.F2 == 0 && vml.F3 == 0)
                    throw new CaeException("At least one load component must not be equal to 0.");
            }
            else if (_viewLoad is ViewDLoad vdl)
            {
                if (vdl.Magnitude == 0)
                    throw new CaeException("The load magnitude must not be equal to 0.");
            }
            else if (_viewLoad is ViewSTLoad vstl)
            {
                if (vstl.F1 == 0 && vstl.F2 == 0 && vstl.F3 == 0)
                    throw new CaeException("At least one load component must not be equal to 0.");
            }
            else if (_viewLoad is ViewGravityLoad vgl)
            {
                if (vgl.F1 == 0 && vgl.F2 == 0 && vgl.F3 == 0)
                    throw new CaeException("At least one load component must not be equal to 0.");
            }
            else if (_viewLoad is ViewCentrifLoad vcfl)
            {
                if (vcfl.N1 == 0 && vcfl.N2 == 0 && vcfl.N3 == 0)
                    throw new CaeException("At least one axis direction component must not be equal to 0.");
                if (vcfl.RotationalSpeed2 == 0)
                    throw new CaeException("Rotational speed square must not be equal to 0.");
            }
            else if (_viewLoad is ViewPreTensionLoad vptl)
            {
                if (vptl.X == 0 && vptl.Y == 0 && vptl.Z == 0)
                    throw new CaeException("At least one force direction component must not be equal to 0.");
                if (vptl.ForceMagnitude == 0)
                    throw new CaeException("Force magnitude must not be equal to 0.");
            }
            //
            if (_loadToEditName == null)
            {
                // Create
                _controller.AddLoadCommand(_stepName, GetLoad);
            }
            else
            {
                // Replace
                if (_propertyItemChanged) _controller.ReplaceLoadCommand(_stepName, _loadToEditName, GetLoad);
            }
        }
        protected override bool OnPrepareForm(string stepName, string loadToEditName)
        {
            // To prevent clear of the selection
            _selectedPropertyGridItemChangedEventActive = false;                             
            // To prevent the call to frmMain.itemForm_VisibleChanged when minimized
            this.DialogResult = DialogResult.None;      
            this.btnOkAddNew.Visible = loadToEditName == null;
            // Clear
            _propertyItemChanged = false;
            _stepName = null;
            _loadNames = null;
            _loadToEditName = null;
            _viewLoad = null;
            lvTypes.Items.Clear();
            propertyGrid.SelectedObject = null;
            //
            _stepName = stepName;
            _loadNames = _controller.GetLoadNames(stepName);
            _loadToEditName = loadToEditName;
            //
            if (!CheckIfStepSupportsLoads()) return false;
            //
            string[] partNames = _controller.GetModelPartNames();
            string[] nodeSetNames = _controller.GetUserNodeSetNames();
            string[] elementSetNames = _controller.GetUserElementSetNames();
            string[] surfaceNames = _controller.GetSurfaceNames();
            string[] elementBasedSurfaceNames = _controller.GetElementBasedSurfaceNames();
            string[] referencePointNames = _controller.GetReferencePointNames();
            if (partNames == null) partNames = new string[0];
            if (nodeSetNames == null) nodeSetNames = new string[0];
            if (elementSetNames == null) elementSetNames = new string[0];
            if (surfaceNames == null) surfaceNames = new string[0];
            if (elementBasedSurfaceNames == null) elementBasedSurfaceNames = new string[0];
            if (referencePointNames == null) referencePointNames = new string[0];
            //
            if (_loadNames == null)
                throw new CaeException("The load names must be defined first.");
            //
            PopulateListOfLoads(partNames, nodeSetNames, elementSetNames, referencePointNames,
                                surfaceNames, elementBasedSurfaceNames);
            // Add loads                                                                                                            
            if (_loadToEditName == null)
            {
                if (nodeSetNames.Length + referencePointNames.Length + surfaceNames.Length == 0)
                    throw new CaeException("There is no node set/reference point/surface defined to which a load could be applied.");
                //
                lvTypes.Enabled = true;
                //
                _viewLoad = null;
            }
            else
            {
                SetLoad = _controller.GetLoad(stepName, _loadToEditName); // to clone
                // Select the appropriate load in the list view - disable event SelectedIndexChanged
                if (_viewLoad is ViewCLoad) lvTypes.Items[0].Selected = true;
                else if (_viewLoad is ViewMomentLoad) lvTypes.Items[1].Selected = true;
                else if (_viewLoad is ViewDLoad) lvTypes.Items[2].Selected = true;
                else if (_viewLoad is ViewSTLoad) lvTypes.Items[3].Selected = true;
                else if (_viewLoad is ViewGravityLoad) lvTypes.Items[4].Selected = true;
                else if (_viewLoad is ViewCentrifLoad) lvTypes.Items[5].Selected = true;
                else if (_viewLoad is ViewPreTensionLoad) lvTypes.Items[6].Selected = true;
                else throw new Exception();
                //
                lvTypes.Enabled = false;
                //
                if (_viewLoad is ViewCLoad vcl)
                {
                    // Check for deleted regions
                    if (vcl.RegionType == RegionTypeEnum.NodeSetName.ToFriendlyString())
                        CheckMissingValueRef(ref nodeSetNames, vcl.NodeSetName, s => { vcl.NodeSetName = s; });
                    else if (vcl.RegionType == RegionTypeEnum.ReferencePointName.ToFriendlyString())
                        CheckMissingValueRef(ref referencePointNames, vcl.ReferencePointName, s => { vcl.ReferencePointName = s; });
                    else throw new NotSupportedException();
                    //
                    vcl.PopululateDropDownLists(nodeSetNames, referencePointNames);
                }
                else if (_viewLoad is ViewMomentLoad vml)
                {
                    // Check for deleted regions
                    if (vml.RegionType == RegionTypeEnum.NodeSetName.ToFriendlyString())
                        CheckMissingValueRef(ref nodeSetNames, vml.NodeSetName, s => { vml.NodeSetName = s; });
                    else if (vml.RegionType == RegionTypeEnum.ReferencePointName.ToFriendlyString())
                        CheckMissingValueRef(ref referencePointNames, vml.ReferencePointName, s => { vml.ReferencePointName = s; });
                    else throw new NotSupportedException();
                    //
                    vml.PopululateDropDownLists(nodeSetNames, referencePointNames);
                }
                else if (_viewLoad is ViewDLoad vdl)
                {
                    // Check for deleted regions
                    CheckMissingValueRef(ref surfaceNames, vdl.SurfaceName, s => { vdl.SurfaceName = s; });
                    //
                    vdl.PopululateDropDownLists(surfaceNames);
                }
                else if (_viewLoad is ViewSTLoad vstl)
                {
                    // Check for deleted regions
                    CheckMissingValueRef(ref surfaceNames, vstl.SurfaceName, s => { vstl.SurfaceName = s; });
                    //
                    vstl.PopululateDropDownLists(surfaceNames);
                }
                else if (_viewLoad is ViewGravityLoad vgl)
                {
                    // Check for deleted regions
                    if (vgl.RegionType == RegionTypeEnum.PartName.ToFriendlyString())
                        CheckMissingValueRef(ref partNames, vgl.PartName, s => { vgl.PartName = s; });
                    else if (vgl.RegionType == RegionTypeEnum.ElementSetName.ToFriendlyString())
                        CheckMissingValueRef(ref elementSetNames, vgl.ElementSetName, s => { vgl.ElementSetName = s; });
                    else throw new NotSupportedException();
                    //
                    vgl.PopululateDropDownLists(partNames, elementSetNames);
                }
                else if (_viewLoad is ViewCentrifLoad vcfl)
                {
                    // Check for deleted regions
                    if (vcfl.RegionType == RegionTypeEnum.PartName.ToFriendlyString())
                        CheckMissingValueRef(ref partNames, vcfl.PartName, s => { vcfl.PartName = s; });
                    else if (vcfl.RegionType == RegionTypeEnum.ElementSetName.ToFriendlyString())
                        CheckMissingValueRef(ref elementSetNames, vcfl.ElementSetName, s => { vcfl.ElementSetName = s; });
                    else throw new NotSupportedException();
                    //
                    vcfl.PopululateDropDownLists(partNames, elementSetNames);
                }
                else if (_viewLoad is ViewPreTensionLoad vptl)
                {
                    // Check for deleted regions
                    CheckMissingValueRef(ref elementBasedSurfaceNames, vptl.SurfaceName, s => { vptl.SurfaceName = s; });
                    //
                    vptl.PopululateDropDownLists(elementBasedSurfaceNames);
                }
                else throw new NotSupportedException();
                //
                propertyGrid.SelectedObject = _viewLoad;
                propertyGrid.Select();
            }
            _selectedPropertyGridItemChangedEventActive = true;
            //
            return true;
        }


        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string loadToEditName)
        {
            return OnPrepareForm(stepName, loadToEditName);
        }
        private void PopulateListOfLoads(string[] partNames, string[] nodeSetNames, string[] elementSetNames, 
                                         string[] referencePointNames, string[] surfaceNames, string[] elementBasedSurfaceNames)
        {
            // Populate list view                                                                               
            ListViewItem item;
            ViewCLoad vcl = null;
            ViewMomentLoad vml = null;
            ViewDLoad vdl;
            ViewSTLoad vstl;
            ViewGravityLoad vgl = null;
            ViewCentrifLoad vcfl = null;
            ViewPreTensionLoad vptl = null;
            // Concentrated force -  node set, reference points
            item = new ListViewItem("Concentrated force");
            if (nodeSetNames.Length > 0 || referencePointNames.Length > 0)
            {
                if (nodeSetNames.Length > 0) vcl = new ViewCLoad(new CLoad(GetLoadName(), nodeSetNames[0], RegionTypeEnum.NodeSetName, 0, 0, 0));
                else if (referencePointNames.Length > 0) vcl = new ViewCLoad(new CLoad(GetLoadName(), referencePointNames[0], RegionTypeEnum.ReferencePointName, 0, 0, 0));
                vcl.PopululateDropDownLists(nodeSetNames, referencePointNames);
                item.Tag = vcl;
            }
            else item.Tag = new ViewError("There is no node set/reference point defined to which a load could be applied.");
            lvTypes.Items.Add(item);
            // Moment
            item = new ListViewItem("Moment");
            if (nodeSetNames.Length > 0 || referencePointNames.Length > 0)
            {
                if (nodeSetNames.Length > 0) vml = new ViewMomentLoad(new MomentLoad(GetLoadName(), nodeSetNames[0], RegionTypeEnum.NodeSetName, 0, 0, 0));
                else if (referencePointNames.Length > 0) vml = new ViewMomentLoad(new MomentLoad(GetLoadName(), referencePointNames[0], RegionTypeEnum.ReferencePointName, 0, 0, 0));
                vml.PopululateDropDownLists(nodeSetNames, referencePointNames);
                item.Tag = vml;
            }
            else item.Tag = new ViewError("There is no node set/reference point defined to which a load could be applied.");
            lvTypes.Items.Add(item);
            // Pressure
            item = new ListViewItem("Pressure");
            if (surfaceNames.Length > 0)
            {
                vdl = new ViewDLoad(new DLoad(GetLoadName(), surfaceNames[0], 0));
                vdl.PopululateDropDownLists(surfaceNames);
                item.Tag = vdl;
            }
            else item.Tag = new ViewError("There is no surface defined to which a load could be applied.");
            lvTypes.Items.Add(item);
            // Surface traction
            item = new ListViewItem("Surface traction");
            if (surfaceNames.Length > 0)
            {
                vstl = new ViewSTLoad(new STLoad(GetLoadName(), surfaceNames[0], 0, 0, 0));
                vstl.PopululateDropDownLists(surfaceNames);
                item.Tag = vstl;
            }
            else item.Tag = new ViewError("There is no surface defined to which a load could be applied.");
            lvTypes.Items.Add(item);
            // Gravity load -  part, element sets
            item = new ListViewItem("Gravity");
            if (partNames.Length > 0 || elementSetNames.Length > 0)
            {
                if (partNames.Length > 0) vgl = new ViewGravityLoad(new GravityLoad(GetLoadName(), partNames[0], RegionTypeEnum.PartName));
                else if (elementSetNames.Length > 0) vgl = new ViewGravityLoad(new GravityLoad(GetLoadName(), elementSetNames[0], RegionTypeEnum.ElementSetName));
                vgl.PopululateDropDownLists(partNames, elementSetNames);
                item.Tag = vgl;
            }
            else item.Tag = new ViewError("There is no part/element set defined to which a load could be applied.");
            lvTypes.Items.Add(item);
            // Centrifugal load -  part, element sets
            item = new ListViewItem("Centrifugal load");
            if (partNames.Length > 0 || elementSetNames.Length > 0)
            {
                if (partNames.Length > 0) vcfl = new ViewCentrifLoad(new CentrifLoad(GetLoadName(), partNames[0], RegionTypeEnum.PartName));
                else if (elementSetNames.Length > 0) vcfl = new ViewCentrifLoad(new CentrifLoad(GetLoadName(), elementSetNames[0], RegionTypeEnum.ElementSetName));
                vcfl.PopululateDropDownLists(partNames, elementSetNames);
                item.Tag = vcfl;
            }
            else item.Tag = new ViewError("There is no part/element set defined to which a load could be applied.");
            lvTypes.Items.Add(item);
            // Pre-tension load
            item = new ListViewItem("Pre-tension");
            if (elementBasedSurfaceNames.Length > 0)
            {
                vptl = new ViewPreTensionLoad(new PreTensionLoad(GetLoadName(), elementBasedSurfaceNames[0], 0, 0, 0, 0));
                vptl.PopululateDropDownLists(elementBasedSurfaceNames);
                item.Tag = vptl;
            }
            else item.Tag = new ViewError("There is no element based surface defined to which a load could be applied.");
            lvTypes.Items.Add(item);
        }
        private string GetLoadName()
        {
            int max = 0;
            int tmp;
            string[] parts;
            foreach (var loadName in _loadNames)
            {
                parts = loadName.Split('-');
                if (int.TryParse(parts.Last(), out tmp))
                {
                    if (tmp > max) max = tmp;
                }
            }
            max++;

            return "Load-" + max.ToString();
        }
        //
        private bool CheckIfStepSupportsLoads()
        {
            Step step = _controller.GetStep(_stepName);
            if (step.SupportsLoads) return true;
            else
            {
                MessageBox.Show("The selected step does not support loads.", "Warning");
                return false;
            }
        }
    }
}
