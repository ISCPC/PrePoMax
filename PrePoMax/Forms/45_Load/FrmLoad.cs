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
    class FrmLoad : UserControls.FrmPropertyListView, IFormBase, IFormItemSetDataParent, IFormHighlight
    {
        // Variables                                                                                                                
        private string[] _loadNames;
        private string _loadToEditName;
        private ViewLoad _viewLoad;
        private Controller _controller;


        // Properties                                                                                                               
        // SetLoad and GetLoad to distinguish from Load event
        public Load FELoad 
        {
            get { return _viewLoad != null ? _viewLoad.GetBase() : null; }
            set
            {
                var clone = value.DeepClone();
                if (clone is CLoad cl) _viewLoad = new ViewCLoad(cl);
                else if (clone is MomentLoad ml) _viewLoad = new ViewMomentLoad(ml);
                else if (clone is DLoad dl) _viewLoad = new ViewDLoad(dl);
                else if (clone is STLoad stl) _viewLoad = new ViewSTLoad(stl);
                else if (clone is ShellEdgeLoad sel) _viewLoad = new ViewShellEdgeLoad(sel);
                else if (clone is GravityLoad gl) _viewLoad = new ViewGravityLoad(gl);
                else if (clone is CentrifLoad cfl) _viewLoad = new ViewCentrifLoad(cfl);
                else if (clone is PreTensionLoad ptl) _viewLoad = new ViewPreTensionLoad(ptl);
                else if (clone is RadiateLoad rl) _viewLoad = new ViewRadiateLoad(rl);
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
            this.Height = 560 + 3 * 19;
        }
        private void InitializeComponent()
        {
            this.gbType.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbType
            // 
            this.gbType.Size = new System.Drawing.Size(310, 115);
            // 
            // lvTypes
            // 
            this.lvTypes.Size = new System.Drawing.Size(298, 87);
            // 
            // gbProperties
            // 
            this.gbProperties.Location = new System.Drawing.Point(12, 133);
            this.gbProperties.Size = new System.Drawing.Size(310, 360);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Size = new System.Drawing.Size(298, 332);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(160, 499);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(241, 499);
            // 
            // btnOkAddNew
            // 
            this.btnOkAddNew.Location = new System.Drawing.Point(79, 499);
            // 
            // FrmLoad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 534);
            this.Name = "FrmLoad";
            this.Text = "Edit Load";
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
                _controller.Selection.LimitSelectionToFirstGeometryType = false;
                _controller.Selection.LimitSelectionToShellEdges = false;
                //
                if (itemTag is ViewError) _viewLoad = null;
                else if (itemTag is ViewCLoad vcl) _viewLoad = vcl;
                else if (itemTag is ViewMomentLoad vml) _viewLoad = vml;
                else if (itemTag is ViewDLoad vdl)  // in order for S1, S2,... to include the same element types
                {
                    _viewLoad = vdl;
                    _controller.Selection.LimitSelectionToFirstGeometryType = true;
                }
                else if (itemTag is ViewSTLoad vstl)
                {
                    _viewLoad = vstl;
                    _controller.Selection.EnableShellEdgeFaceSelection = true;
                }
                else if (itemTag is ViewShellEdgeLoad vsel)
                {
                    _viewLoad = vsel;
                    _controller.Selection.LimitSelectionToShellEdges = true;
                }
                else if (itemTag is ViewGravityLoad vgl) _viewLoad = vgl;
                else if (itemTag is ViewCentrifLoad vcfl) _viewLoad = vcfl;
                else if (itemTag is ViewPreTensionLoad vprl) _viewLoad = vprl;
                else if (itemTag is ViewRadiateLoad rl)  // in order for S1, S2,... to include the same element types
                {
                    _viewLoad = rl;
                    _controller.Selection.LimitSelectionToFirstGeometryType = true;
                }
                else throw new NotImplementedException();
                //
                ShowHideSelectionForm();
                //
                propertyGrid.SelectedObject = itemTag;
                propertyGrid.Select();
                //
                HighlightLoad();
            }
        }
        protected override void OnPropertyGridPropertyValueChanged()
        {
            string property = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
            //
            if (property == nameof(_viewLoad.RegionType))
            {
                ShowHideSelectionForm();
                //
                HighlightLoad();
            }
            else if (_viewLoad is ViewCLoad vcl &&
                     (property == nameof(vcl.NodeSetName) || property == nameof(vcl.ReferencePointName)))
            {
                HighlightLoad();
            }
            else if (_viewLoad is ViewMomentLoad vml &&
                     (property == nameof(vml.NodeSetName) || property == nameof(vml.ReferencePointName)))
            {
                HighlightLoad();
            }
            else if (_viewLoad is ViewDLoad vdl && property == nameof(vdl.SurfaceName))
            {
                HighlightLoad();
            }
            else if (_viewLoad is ViewSTLoad vstl && property == nameof(vstl.SurfaceName))
            {
                HighlightLoad();
            }
            else if (_viewLoad is ViewShellEdgeLoad vsel && property == nameof(vstl.SurfaceName))
            {
                HighlightLoad();
            }
            else if (_viewLoad is ViewGravityLoad vgl &&
                     (property == nameof(vgl.PartName) || property == nameof(vgl.ElementSetName)))
            {
                HighlightLoad();
            }
            else if (_viewLoad is ViewCentrifLoad vcfl &&
                     (property == nameof(vcfl.PartName) || property == nameof(vcfl.ElementSetName)))
            {
                HighlightLoad();
            }
            else if (_viewLoad is ViewPreTensionLoad vptl && property == nameof(vptl.SurfaceName))
            {
                HighlightLoad();
            }
            else if (_viewLoad is ViewRadiateLoad vrl && property == nameof(vrl.SurfaceName))
            {
                HighlightLoad();
            }
            //
            base.OnPropertyGridPropertyValueChanged();
        }
        protected override void OnApply(bool onOkAddNew)
        {
            if (propertyGrid.SelectedObject is ViewError ve) throw new CaeException(ve.Message);
            //
            _viewLoad = (ViewLoad)propertyGrid.SelectedObject;
            //
            if (_viewLoad == null) throw new CaeException("No load selected.");
            //
            if ((_loadToEditName == null && _loadNames.Contains(FELoad.Name)) ||        // named to existing name
                (FELoad.Name != _loadToEditName && _loadNames.Contains(FELoad.Name)))   // renamed to existing name
                throw new CaeException("The selected load name already exists.");
            //
            if (FELoad.RegionType == RegionTypeEnum.Selection &&
                (FELoad.CreationIds == null || FELoad.CreationIds.Length == 0))
                throw new CaeException("The load must contain at least one item.");
            //
            // check for 0 values
            if (FELoad is CLoad cl)
            {
                if (cl.F1 == 0 && cl.F2 == 0 && cl.F3 == 0)
                    throw new CaeException("At least one force component must not be equal to 0.");
            }
            else if (FELoad is MomentLoad ml)
            {
                if (ml.M1 == 0 && ml.M2 == 0 && ml.M3 == 0)
                    throw new CaeException("At least one moment component must not be equal to 0.");
            }
            else if (FELoad is DLoad dl)
            {
                if (dl.Magnitude == 0)
                    throw new CaeException("The pressure load magnitude must not be equal to 0.");
            }
            else if (FELoad is STLoad stl)
            {
                if (stl.F1 == 0 && stl.F2 == 0 && stl.F3 == 0)
                    throw new CaeException("At least one surface traction load component must not be equal to 0.");
            }
            else if (FELoad is ShellEdgeLoad sel)
            {
                if (sel.Magnitude == 0)
                    throw new CaeException("The shell edge load magnitude must not be equal to 0.");
            }
            else if (FELoad is GravityLoad gl)
            {
                if (gl.F1 == 0 && gl.F2 == 0 && gl.F3 == 0)
                    throw new CaeException("At least one gravity load component must not be equal to 0.");
            }
            else if (FELoad is CentrifLoad cfl)
            {
                if (cfl.N1 == 0 && cfl.N2 == 0 && cfl.N3 == 0)
                    throw new CaeException("At least one axis direction component must not be equal to 0.");
                if (cfl.RotationalSpeed2 == 0)
                    throw new CaeException("Rotational speed must not be equal to 0.");
            }
            else if (FELoad is PreTensionLoad ptl)
            {
                if (!ptl.AutoComputeDirection && ptl.X == 0 && ptl.Y == 0 && ptl.Z == 0)
                    throw new CaeException("At least one pre-tension direction component must not be equal to 0.");
                if (ptl.Type == PreTensionLoadType.Force && ptl.Magnitude == 0)
                    throw new CaeException("Pre-tension magnitude must not be equal to 0.");
            }
            else if (FELoad is RadiateLoad rl)
            {
                if (rl.CavityRadiation && (rl.CavityName == null || rl.CavityName == ""))
                    throw new CaeException("For cavity radiation a cavity name must be specified.");
            }
            // Create
            if (_loadToEditName == null)
            {
                _controller.AddLoadCommand(_stepName, FELoad);
            }
            // Replace
            else if (_propertyItemChanged)
            {
                _controller.ReplaceLoadCommand(_stepName, _loadToEditName, FELoad);
                _loadToEditName = null; // prevents the execution of toInternal in OnHideOrClose
            }
            // Convert the load from internal to show it
            else
            {
                LoadInternal(false);
            }
            // If all is successful close the ItemSetSelectionForm - except for OKAddNew
            if (!onOkAddNew) ItemSetDataEditor.SelectionForm.Hide();
        }
        protected override void OnHideOrClose()
        {
            // Close the ItemSetSelectionForm
            ItemSetDataEditor.SelectionForm.Hide();
            // Deactivate selection limits
            _controller.Selection.LimitSelectionToFirstGeometryType = false;
            _controller.Selection.EnableShellEdgeFaceSelection = false;
            _controller.Selection.LimitSelectionToShellEdges = false;
            // Convert the load from internal to show it
            LoadInternal(false);
            //
            base.OnHideOrClose();
        }
        protected override bool OnPrepareForm(string stepName, string loadToEditName)
        {
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
            _loadNames = _controller.GetAllLoadNames();
            _loadToEditName = loadToEditName;
            //
            string[] partNames = _controller.GetModelPartNames();
            string[] nodeSetNames = _controller.GetUserNodeSetNames();
            string[] elementSetNames = _controller.GetUserElementSetNames();
            string[] elementBasedSurfaceNames = _controller.GetUserSurfaceNames(CaeMesh.FeSurfaceType.Element);
            string[] shellEdgeSurfaceNames = _controller.GetUserSurfaceNames(CaeMesh.FeSurfaceType.Element,
                                                                             CaeMesh.FeSurfaceFaceTypes.ShellEdgeFaces);
            string[] solidFaceSurfaceNames = _controller.GetUserSurfaceNames(CaeMesh.FeSurfaceType.Element,
                                                                             CaeMesh.FeSurfaceFaceTypes.SolidFaces);
            string[] referencePointNames = _controller.GetReferencePointNames();
            if (partNames == null) partNames = new string[0];
            if (nodeSetNames == null) nodeSetNames = new string[0];
            if (elementSetNames == null) elementSetNames = new string[0];
            if (elementBasedSurfaceNames == null) elementBasedSurfaceNames = new string[0];
            if (shellEdgeSurfaceNames == null) shellEdgeSurfaceNames = new string[0];
            if (referencePointNames == null) referencePointNames = new string[0];
            //
            if (_loadNames == null) throw new CaeException("The load names must be defined first.");
            //
            string[] noEdgeSurfaceNames = elementBasedSurfaceNames.Except(shellEdgeSurfaceNames).ToArray();
            PopulateListOfLoads(partNames, nodeSetNames, elementSetNames, referencePointNames,
                                elementBasedSurfaceNames, solidFaceSurfaceNames, noEdgeSurfaceNames, shellEdgeSurfaceNames);
            // Check if this step supports any loads
            if (lvTypes.Items.Count == 0) return false;
            // Create new load
            if (_loadToEditName == null)
            {
                lvTypes.Enabled = true;
                _viewLoad = null;
                //
                HighlightLoad(); // must be here if called from the menu
            }
            else
            {
                // Get and convert a converted load back to selection
                FELoad = _controller.GetLoad(stepName, _loadToEditName); // to clone
                if (FELoad.CreationData != null) FELoad.RegionType = RegionTypeEnum.Selection;
                // Convert the load to internal to hide it
                LoadInternal(true);
                //
                int selectedId;
                if (_viewLoad is ViewCLoad vcl)
                {
                    selectedId = lvTypes.FindItemWithText("Concentrated force").Index;
                    // Check for deleted regions
                    if (vcl.RegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                    else if (vcl.RegionType == RegionTypeEnum.NodeSetName.ToFriendlyString())
                        CheckMissingValueRef(ref nodeSetNames, vcl.NodeSetName, s => { vcl.NodeSetName = s; });
                    else if (vcl.RegionType == RegionTypeEnum.ReferencePointName.ToFriendlyString())
                        CheckMissingValueRef(ref referencePointNames, vcl.ReferencePointName, s => { vcl.ReferencePointName = s; });
                    else throw new NotSupportedException();
                    //
                    vcl.PopululateDropDownLists(nodeSetNames, referencePointNames);
                }
                else if (_viewLoad is ViewMomentLoad vml)
                {
                    selectedId = lvTypes.FindItemWithText("Moment").Index;
                    // Check for deleted regions
                    if (vml.RegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                    else if (vml.RegionType == RegionTypeEnum.NodeSetName.ToFriendlyString())
                        CheckMissingValueRef(ref nodeSetNames, vml.NodeSetName, s => { vml.NodeSetName = s; });
                    else if (vml.RegionType == RegionTypeEnum.ReferencePointName.ToFriendlyString())
                        CheckMissingValueRef(ref referencePointNames, vml.ReferencePointName, s => { vml.ReferencePointName = s; });
                    else throw new NotSupportedException();
                    //
                    vml.PopululateDropDownLists(nodeSetNames, referencePointNames);
                }
                else if (_viewLoad is ViewDLoad vdl)
                {
                    selectedId = lvTypes.FindItemWithText("Pressure").Index;
                    // Check for deleted regions
                    if (vdl.RegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                    else if (vdl.RegionType == RegionTypeEnum.SurfaceName.ToFriendlyString())
                        CheckMissingValueRef(ref noEdgeSurfaceNames, vdl.SurfaceName, s => { vdl.SurfaceName = s; });
                    else throw new NotSupportedException();
                    //
                    vdl.PopululateDropDownLists(noEdgeSurfaceNames);
                }
                else if (_viewLoad is ViewSTLoad vstl)
                {
                    selectedId = lvTypes.FindItemWithText("Surface traction").Index;
                    // Check for deleted regions
                    if (vstl.RegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                    else if (vstl.RegionType == RegionTypeEnum.SurfaceName.ToFriendlyString())
                        CheckMissingValueRef(ref elementBasedSurfaceNames, vstl.SurfaceName, s => { vstl.SurfaceName = s; });
                    else throw new NotSupportedException();
                    //
                    vstl.PopululateDropDownLists(elementBasedSurfaceNames);
                }
                else if (_viewLoad is ViewShellEdgeLoad vsel)
                {
                    selectedId = lvTypes.FindItemWithText("Normal shell edge load").Index;
                    // Check for deleted regions
                    if (vsel.RegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                    else if (vsel.RegionType == RegionTypeEnum.SurfaceName.ToFriendlyString())
                        CheckMissingValueRef(ref shellEdgeSurfaceNames, vsel.SurfaceName, s => { vsel.SurfaceName = s; });
                    else throw new NotSupportedException();
                    //
                    vsel.PopululateDropDownLists(shellEdgeSurfaceNames);
                }
                else if (_viewLoad is ViewGravityLoad vgl)
                {
                    selectedId = lvTypes.FindItemWithText("Gravity").Index;
                    // Check for deleted regions
                    if (vgl.RegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                    else if (vgl.RegionType == RegionTypeEnum.PartName.ToFriendlyString())
                        CheckMissingValueRef(ref partNames, vgl.PartName, s => { vgl.PartName = s; });
                    else if (vgl.RegionType == RegionTypeEnum.ElementSetName.ToFriendlyString())
                        CheckMissingValueRef(ref elementSetNames, vgl.ElementSetName, s => { vgl.ElementSetName = s; });
                    else throw new NotSupportedException();
                    //
                    vgl.PopululateDropDownLists(partNames, elementSetNames);
                }
                else if (_viewLoad is ViewCentrifLoad vcfl)
                {
                    selectedId = lvTypes.FindItemWithText("Centrifugal load").Index;
                    // Check for deleted regions
                    if (vcfl.RegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                    else if (vcfl.RegionType == RegionTypeEnum.PartName.ToFriendlyString())
                        CheckMissingValueRef(ref partNames, vcfl.PartName, s => { vcfl.PartName = s; });
                    else if (vcfl.RegionType == RegionTypeEnum.ElementSetName.ToFriendlyString())
                        CheckMissingValueRef(ref elementSetNames, vcfl.ElementSetName, s => { vcfl.ElementSetName = s; });
                    else throw new NotSupportedException();
                    //
                    vcfl.PopululateDropDownLists(partNames, elementSetNames);
                }
                else if (_viewLoad is ViewPreTensionLoad vptl)
                {
                    selectedId = lvTypes.FindItemWithText("Pre-tension").Index;
                    // Check for deleted regions
                    if (vptl.RegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                    else if (vptl.RegionType == RegionTypeEnum.SurfaceName.ToFriendlyString())
                        CheckMissingValueRef(ref elementBasedSurfaceNames, vptl.SurfaceName, s => { vptl.SurfaceName = s; });
                    else throw new NotSupportedException();
                    //
                    vptl.PopululateDropDownLists(solidFaceSurfaceNames);
                }
                else if (_viewLoad is ViewRadiateLoad vrl)
                {
                    selectedId = lvTypes.FindItemWithText("Radiate").Index;
                    // Check for deleted regions
                    if (vrl.RegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                    else if (vrl.RegionType == RegionTypeEnum.SurfaceName.ToFriendlyString())
                        CheckMissingValueRef(ref noEdgeSurfaceNames, vrl.SurfaceName, s => { vrl.SurfaceName = s; });
                    else throw new NotSupportedException();
                    //
                    vrl.PopululateDropDownLists(noEdgeSurfaceNames);
                }
                else throw new NotSupportedException();
                //
                lvTypes.Items[selectedId].Tag = _viewLoad;
                _preselectIndex = selectedId;
            }
            ShowHideSelectionForm();
            //
            return true;
        }


        // Methods                                                                                                                  
        private void PopulateListOfLoads(string[] partNames, string[] nodeSetNames, string[] elementSetNames, 
                                         string[] referencePointNames, string[] elementBasedSurfaceNames,
                                         string[] solidFaceSurfaceNames, string[] noEdgeSurfaceNames,
                                         string[] shellEdgeSurfaceNames)
        {
            Step step = _controller.GetStep(_stepName);
            System.Drawing.Color color = _controller.Settings.Pre.LoadSymbolColor;
            // Populate list view
            ListViewItem item;
            string name;
            string loadName;
            // Concentrated force -  node set, reference points
            name = "Concentrated force";
            loadName = GetLoadName(name);
            item = new ListViewItem(name);
            CLoad cLoad = new CLoad(loadName, "", RegionTypeEnum.Selection, 0, 0, 0);
            if (step.IsLoadSupported(cLoad))
            {
                ViewCLoad vcl = new ViewCLoad(cLoad);
                vcl.PopululateDropDownLists(nodeSetNames, referencePointNames);
                vcl.Color = color;
                item.Tag = vcl;
                lvTypes.Items.Add(item);
            }
            // Moment
            name = "Moment";
            loadName = GetLoadName(name);
            item = new ListViewItem(name);
            MomentLoad momentLoad = new MomentLoad(loadName, "", RegionTypeEnum.Selection, 0, 0, 0);
            if (step.IsLoadSupported(cLoad))
            {
                ViewMomentLoad vml = new ViewMomentLoad(momentLoad);
                vml.PopululateDropDownLists(nodeSetNames, referencePointNames);
                vml.Color = color;
                item.Tag = vml;
                lvTypes.Items.Add(item);
            }
            // Pressure
            name = "Pressure";
            loadName = GetLoadName(name);
            item = new ListViewItem(name);
            DLoad dLoad = new DLoad(loadName, "", RegionTypeEnum.Selection, 0);
            if (step.IsLoadSupported(dLoad))
            {
                ViewDLoad vdl = new ViewDLoad(dLoad);
                vdl.PopululateDropDownLists(noEdgeSurfaceNames);
                vdl.Color = color;
                item.Tag = vdl;
                lvTypes.Items.Add(item);
            }
            // Surface traction
            name = "Surface traction";
            loadName = GetLoadName(name);
            item = new ListViewItem(name);
            STLoad sTLoad = new STLoad(loadName, "", RegionTypeEnum.Selection, 0, 0, 0);
            if (step.IsLoadSupported(sTLoad))
            {
                ViewSTLoad vstl = new ViewSTLoad(sTLoad);
                vstl.PopululateDropDownLists(elementBasedSurfaceNames);
                vstl.Color = color;
                item.Tag = vstl;
                lvTypes.Items.Add(item);
            }
            // Shell edge load
            name = "Normal shell edge load";
            loadName = GetLoadName(name);
            item = new ListViewItem(name);
            ShellEdgeLoad shellEdgeLoad = new ShellEdgeLoad(loadName, "", RegionTypeEnum.Selection, 0);
            if (step.IsLoadSupported(shellEdgeLoad))
            {
                ViewShellEdgeLoad vsel = new ViewShellEdgeLoad(shellEdgeLoad);
                vsel.PopululateDropDownLists(shellEdgeSurfaceNames);
                vsel.Color = color;
                item.Tag = vsel;
                lvTypes.Items.Add(item);
            }
            // Gravity load -  part, element sets
            name = "Gravity";
            loadName = GetLoadName(name);
            item = new ListViewItem(name);
            GravityLoad gravityLoad = new GravityLoad(loadName, "", RegionTypeEnum.Selection);
            if (step.IsLoadSupported(gravityLoad))
            {
                ViewGravityLoad vgl = new ViewGravityLoad(gravityLoad);
                vgl.PopululateDropDownLists(partNames, elementSetNames);
                vgl.Color = color;
                item.Tag = vgl;
                lvTypes.Items.Add(item);
            }
            // Centrifugal load -  part, element sets
            name = "Centrifugal load";
            loadName = GetLoadName(name);
            item = new ListViewItem(name);
            CentrifLoad centrifLoad = new CentrifLoad(loadName, "", RegionTypeEnum.Selection);
            ViewCentrifLoad vcfl = new ViewCentrifLoad(centrifLoad);
            if (step.IsLoadSupported(gravityLoad))
            {
                vcfl.PopululateDropDownLists(partNames, elementSetNames);
                vcfl.Color = color;
                item.Tag = vcfl;
                lvTypes.Items.Add(item);
            }
            // Pre-tension load
            name = "Pre-tension";
            loadName = GetLoadName(name);
            item = new ListViewItem(name);
            PreTensionLoad preTensionLoad = new PreTensionLoad(loadName, "", RegionTypeEnum.Selection, 0);
            if (step.IsLoadSupported(preTensionLoad))
            {
                ViewPreTensionLoad vptl = new ViewPreTensionLoad(preTensionLoad);
                vptl.PopululateDropDownLists(solidFaceSurfaceNames);
                vptl.Color = color;
                item.Tag = vptl;
                lvTypes.Items.Add(item);
            }
            // Radiate load
            name = "Radiate";
            loadName = GetLoadName(name);
            item = new ListViewItem(name);
            RadiateLoad radiateLoad = new RadiateLoad(loadName, "", RegionTypeEnum.Selection, 0, 0.5);
            if (step.IsLoadSupported(radiateLoad))
            {
                ViewRadiateLoad vrl = new ViewRadiateLoad(radiateLoad);
                vrl.PopululateDropDownLists(noEdgeSurfaceNames);
                vrl.Color = color;
                item.Tag = vrl;
                lvTypes.Items.Add(item);
            }
        }
        private string GetLoadName(string name)
        {
            if (name == null || name == "") name = "Load";
            name = name.Replace(' ', '_');
            if (name[name.Length - 1] != '-') name += '-';
            name = NamedClass.GetNewValueName(_loadNames, name);
            //
            return name;
        }
        private void HighlightLoad()
        {
            try
            {
                _controller.ClearSelectionHistory();
                //
                if (_viewLoad == null) { }
                else if (FELoad is CLoad || FELoad is MomentLoad || FELoad is DLoad || FELoad is STLoad ||
                         FELoad is ShellEdgeLoad || FELoad is GravityLoad || FELoad is CentrifLoad ||
                         FELoad is PreTensionLoad || FELoad is RadiateLoad)
                {
                    if (FELoad.RegionType == RegionTypeEnum.NodeSetName ||
                        FELoad.RegionType == RegionTypeEnum.ReferencePointName ||
                        FELoad.RegionType == RegionTypeEnum.SurfaceName ||
                        FELoad.RegionType == RegionTypeEnum.PartName ||
                        FELoad.RegionType == RegionTypeEnum.ElementSetName)
                    {
                        _controller.Highlight3DObjects(new object[] { FELoad.RegionName });
                    }
                    else if (FELoad.RegionType == RegionTypeEnum.Selection)
                    {
                        SetSelectItem();
                        //
                        if (FELoad.CreationData != null)
                        {
                            _controller.Selection = FELoad.CreationData.DeepClone();
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
            if (FELoad != null && FELoad.RegionType == RegionTypeEnum.Selection)
                ItemSetDataEditor.SelectionForm.ShowIfHidden(this.Owner);
            else
                ItemSetDataEditor.SelectionForm.Hide();
            //
            SetSelectItem();
        }
        private void SetSelectItem()
        {
            if (FELoad != null && FELoad.RegionType == RegionTypeEnum.Selection)
            {
                if (FELoad is null) { }
                else if (FELoad is CLoad) _controller.SetSelectItemToNode();
                else if (FELoad is MomentLoad) _controller.SetSelectItemToNode();
                else if (FELoad is DLoad) _controller.SetSelectItemToSurface();
                else if (FELoad is STLoad) _controller.SetSelectItemToSurface();
                else if (FELoad is ShellEdgeLoad) _controller.SetSelectItemToSurface();
                else if (FELoad is GravityLoad) _controller.SetSelectItemToPart();
                else if (FELoad is CentrifLoad) _controller.SetSelectItemToPart();
                else if (FELoad is PreTensionLoad) _controller.SetSelectItemToSurface();
                else if (FELoad is RadiateLoad) _controller.SetSelectItemToSurface();
                else throw new NotSupportedException();
            }
            else _controller.SetSelectByToOff();
        }
        private void LoadInternal(bool toInternal)
        {
            if (_stepName != null && _loadToEditName != null)
            {
                // Convert the load from/to internal to hide/show it
                _controller.GetLoad(_stepName, _loadToEditName).Internal = toInternal;
                _controller.FeModelUpdate(UpdateType.RedrawSymbols);
            }
        }
        //
        public void SelectionChanged(int[] ids)
        {
            if (FELoad != null && FELoad.RegionType == RegionTypeEnum.Selection)
            {
                if (FELoad is CLoad || FELoad is MomentLoad || FELoad is DLoad || FELoad is STLoad || FELoad is ShellEdgeLoad ||
                    FELoad is GravityLoad || FELoad is CentrifLoad || FELoad is PreTensionLoad || FELoad is RadiateLoad)
                {
                    FELoad.CreationIds = ids;
                    FELoad.CreationData = _controller.Selection.DeepClone();
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
            HighlightLoad();
        }

        // IFormItemSetDataParent
        public bool IsSelectionGeometryBased()
        {
            // Prepare ItemSetDataEditor - prepare Geometry or Mesh based selection
            if (FELoad == null || FELoad.CreationData == null) return true;   // element set based section
            return FELoad.CreationData.IsGeometryBased();
        }
    }
}
