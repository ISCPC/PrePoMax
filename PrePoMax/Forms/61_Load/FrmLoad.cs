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
    class FrmLoad : UserControls.FrmPropertyListView, IFormBase, IFormHighlight
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
            this.Height = 637;
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
            this.gbProperties.Size = new System.Drawing.Size(310, 334);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Size = new System.Drawing.Size(298, 306);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(160, 519);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(241, 519);
            // 
            // btnOkAddNew
            // 
            this.btnOkAddNew.Location = new System.Drawing.Point(79, 519);
            // 
            // FrmLoad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 554);
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
                object itemTag = lvTypes.SelectedItems[0].Tag;
                if (itemTag is ViewError) _viewLoad = null;
                else if (itemTag is ViewCLoad vcl) _viewLoad = vcl;
                else if (itemTag is ViewMomentLoad vml) _viewLoad = vml;
                else if (itemTag is ViewDLoad vdl) _viewLoad = vdl;
                else if (itemTag is ViewSTLoad vstl) _viewLoad = vstl;
                else if (itemTag is ViewGravityLoad vgl) _viewLoad = vgl;
                else if (itemTag is ViewCentrifLoad vcfl) _viewLoad = vcfl;
                else if (itemTag is ViewPreTensionLoad vprl) _viewLoad = vprl;
                else throw new NotImplementedException();
                //
                SetSelectItem();
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
            //
            base.OnPropertyGridPropertyValueChanged();
        }
        protected override void OnApply(bool onOkAddNew)
        {
            if (propertyGrid.SelectedObject is ViewError ve) throw new CaeGlobals.CaeException(ve.Message);
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
                    throw new CaeException("The pressure magnitude must not be equal to 0.");
            }
            else if (FELoad is STLoad stl)
            {
                if (stl.F1 == 0 && stl.F2 == 0 && stl.F3 == 0)
                    throw new CaeException("At least one surface traction load component must not be equal to 0.");
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
                    throw new CaeException("Rotational speed square must not be equal to 0.");
            }
            else if (FELoad is PreTensionLoad ptl)
            {
                if (!ptl.AutoComputeDirection && ptl.X == 0 && ptl.Y == 0 && ptl.Z == 0)
                    throw new CaeException("At least one pre-tension direction component must not be equal to 0.");
                if (ptl.Type == PreTensionLoadType.Force && ptl.Magnitude == 0)
                    throw new CaeException("Pre-tension magnitude must not be equal to 0.");
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
            // Convert the load from internal to show it
            LoadInternal(false);
            //
            base.OnHideOrClose();
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
            _loadNames = _controller.GetAllLoadNames();
            _loadToEditName = loadToEditName;
            //
            if (!CheckIfStepSupportsLoads()) return false;
            //
            string[] partNames = _controller.GetModelPartNames();
            string[] nodeSetNames = _controller.GetUserNodeSetNames();
            string[] elementSetNames = _controller.GetUserElementSetNames();
            string[] surfaceNames = _controller.GetUserSurfaceNames();
            string[] elementBasedSurfaceNames = _controller.GetUserElementBasedSurfaceNames();
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
            // Create new load
            if (_loadToEditName == null)
            {
                lvTypes.Enabled = true;
                _viewLoad = null;
            }
            else
            {
                // Get and convert a converted load back to selection
                FELoad = _controller.GetLoad(stepName, _loadToEditName); // to clone
                // Convert the load to internal to hide it
                LoadInternal(true);
                //
                if (FELoad.CreationData != null) FELoad.RegionType = RegionTypeEnum.Selection;
                // Select the appropriate load in the list view - disable event SelectedIndexChanged
                _lvTypesSelectedIndexChangedEventActive = false;
                if (_viewLoad is ViewCLoad) lvTypes.Items[0].Selected = true;
                else if (_viewLoad is ViewMomentLoad) lvTypes.Items[1].Selected = true;
                else if (_viewLoad is ViewDLoad) lvTypes.Items[2].Selected = true;
                else if (_viewLoad is ViewSTLoad) lvTypes.Items[3].Selected = true;
                else if (_viewLoad is ViewGravityLoad) lvTypes.Items[4].Selected = true;
                else if (_viewLoad is ViewCentrifLoad) lvTypes.Items[5].Selected = true;
                else if (_viewLoad is ViewPreTensionLoad) lvTypes.Items[6].Selected = true;
                else throw new NotSupportedException();
                //
                lvTypes.Enabled = false;
                _lvTypesSelectedIndexChangedEventActive = true;
                //
                if (_viewLoad is ViewCLoad vcl)
                {
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
                    // Check for deleted regions
                    if (vdl.RegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                    else if (vdl.RegionType == RegionTypeEnum.SurfaceName.ToFriendlyString())
                        CheckMissingValueRef(ref surfaceNames, vdl.SurfaceName, s => { vdl.SurfaceName = s; });
                    else throw new NotSupportedException();
                    //
                    vdl.PopululateDropDownLists(surfaceNames);
                }
                else if (_viewLoad is ViewSTLoad vstl)
                {
                    // Check for deleted regions
                    if (vstl.RegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                    else if (vstl.RegionType == RegionTypeEnum.SurfaceName.ToFriendlyString())
                        CheckMissingValueRef(ref surfaceNames, vstl.SurfaceName, s => { vstl.SurfaceName = s; });
                    else throw new NotSupportedException();
                    //
                    vstl.PopululateDropDownLists(surfaceNames);
                }
                else if (_viewLoad is ViewGravityLoad vgl)
                {
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
                    // Check for deleted regions
                    if (vptl.RegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                    else if (vptl.RegionType == RegionTypeEnum.SurfaceName.ToFriendlyString())
                        CheckMissingValueRef(ref elementBasedSurfaceNames, vptl.SurfaceName, s => { vptl.SurfaceName = s; });
                    else throw new NotSupportedException();
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
            SetSelectItem();
            //
            ShowHideSelectionForm();
            //
            HighlightLoad(); // must be here if called from the menu
            //
            return true;
        }


        // Methods                                                                                                                  
        private void PopulateListOfLoads(string[] partNames, string[] nodeSetNames, string[] elementSetNames, 
                                         string[] referencePointNames, string[] surfaceNames, string[] elementBasedSurfaceNames)
        {
            // Populate list view                                                                               
            ListViewItem item;
            string name;
            string loadName;
            // Concentrated force -  node set, reference points
            name = "Concentrated force";
            loadName = GetLoadName(name);
            item = new ListViewItem(name);
            ViewCLoad vcl = new ViewCLoad(new CLoad(loadName, "", RegionTypeEnum.Selection, 0, 0, 0));
            vcl.PopululateDropDownLists(nodeSetNames, referencePointNames);
            item.Tag = vcl;
            lvTypes.Items.Add(item);
            // Moment
            name = "Moment";
            loadName = GetLoadName(name);
            item = new ListViewItem(name);
            ViewMomentLoad vml = new ViewMomentLoad(new MomentLoad(loadName, "", RegionTypeEnum.Selection, 0, 0, 0));
            vml.PopululateDropDownLists(nodeSetNames, referencePointNames);
            item.Tag = vml;
            lvTypes.Items.Add(item);
            // Pressure
            name = "Pressure";
            loadName = GetLoadName(name);
            item = new ListViewItem(name);
            ViewDLoad vdl = new ViewDLoad(new DLoad(loadName, "", RegionTypeEnum.Selection, 0));
            vdl.PopululateDropDownLists(surfaceNames);
            item.Tag = vdl;
            lvTypes.Items.Add(item);
            // Surface traction
            name = "Surface traction";
            loadName = GetLoadName(name);
            item = new ListViewItem(name);
            ViewSTLoad vstl = new ViewSTLoad(new STLoad(loadName, "", RegionTypeEnum.Selection, 0, 0, 0));
            vstl.PopululateDropDownLists(surfaceNames);
            item.Tag = vstl;
            lvTypes.Items.Add(item);
            // Gravity load -  part, element sets
            name = "Gravity";
            loadName = GetLoadName(name);
            item = new ListViewItem(name);
            ViewGravityLoad vgl = new ViewGravityLoad(new GravityLoad(loadName, "", RegionTypeEnum.Selection));
            vgl.PopululateDropDownLists(partNames, elementSetNames);
            item.Tag = vgl;
            lvTypes.Items.Add(item);
            // Centrifugal load -  part, element sets
            name = "Centrifugal load";
            loadName = GetLoadName(name);
            item = new ListViewItem(name);
            ViewCentrifLoad vcfl = new ViewCentrifLoad(new CentrifLoad(loadName, "", RegionTypeEnum.Selection));
            vcfl.PopululateDropDownLists(partNames, elementSetNames);
            item.Tag = vcfl;
            lvTypes.Items.Add(item);
            // Pre-tension load
            name = "Pre-tension";
            loadName = GetLoadName(name);
            item = new ListViewItem(name);
            ViewPreTensionLoad vptl = new ViewPreTensionLoad(new PreTensionLoad(loadName, "", RegionTypeEnum.Selection, 0));
            vptl.PopululateDropDownLists(elementBasedSurfaceNames);
            item.Tag = vptl;
            lvTypes.Items.Add(item);
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
        private void HighlightLoad()
        {
            try
            {
                if (_viewLoad == null) { }
                else if (FELoad is CLoad || FELoad is MomentLoad || FELoad is DLoad || FELoad is STLoad 
                         || FELoad is GravityLoad || FELoad is CentrifLoad || FELoad is PreTensionLoad)
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
        }
        private void SetSelectItem()
        {
            if (FELoad is null) { }
            else if (FELoad is CLoad) _controller.SetSelectItemToNode();
            else if (FELoad is MomentLoad) _controller.SetSelectItemToNode();
            else if (FELoad is DLoad) _controller.SetSelectItemToSurface();
            else if (FELoad is STLoad) _controller.SetSelectItemToSurface();
            else if (FELoad is GravityLoad) _controller.SetSelectItemToPart();
            else if (FELoad is CentrifLoad) _controller.SetSelectItemToPart();
            else if (FELoad is PreTensionLoad) _controller.SetSelectItemToSurface();
        }
        private void LoadInternal(bool toInternal)
        {
            if (_stepName != null && _loadToEditName != null)
            {
                // Convert the load from/to internal to hide/show it
                _controller.GetLoad(_stepName, _loadToEditName).Internal = toInternal;
                _controller.Update(UpdateType.RedrawSymbols);
            }
        }
        //
        public void SelectionChanged(int[] ids)
        {
            if (FELoad != null && FELoad.RegionType == RegionTypeEnum.Selection)
            {
                if (FELoad is CLoad || FELoad is MomentLoad || FELoad is DLoad || FELoad is STLoad
                    || FELoad is GravityLoad || FELoad is CentrifLoad || FELoad is PreTensionLoad)
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
    }
}
