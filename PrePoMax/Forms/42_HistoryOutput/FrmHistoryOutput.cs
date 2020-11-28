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
    class FrmHistoryOutput : UserControls.FrmPropertyListView, IFormBase, IFormItemSetDataParent, IFormHighlight
    {
        // Variables                                                                                                                
        private string[] _historyOutputNames;
        private string _historyOutputToEditName;
        private ViewHistoryOutput _viewHistoryOutput;
        private Controller _controller;


        // Properties                                                                                                               
        public HistoryOutput HistoryOutput
        {
            get { return _viewHistoryOutput != null ? _viewHistoryOutput.GetBase() : null; }
            set
            {
                if (value is NodalHistoryOutput nho) _viewHistoryOutput = new ViewNodalHistoryOutput(nho.DeepClone());
                else if (value is ElementHistoryOutput eho) _viewHistoryOutput = new ViewElementHistoryOutput(eho.DeepClone());
                else if (value is ContactHistoryOutput cho) _viewHistoryOutput = new ViewContactHistoryOutput(cho.DeepClone());
                else throw new NotImplementedException();
            }
        }


        // Constructors                                                                                                             
        public FrmHistoryOutput(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewHistoryOutput = null;
            //
            _selectedPropertyGridItemChangedEventActive = true;
        }
        private void InitializeComponent()
        {
            this.gbType.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvTypes
            // 
            this.lvTypes.Size = new System.Drawing.Size(298, 69);
            // 
            // gbProperties
            // 
            this.gbProperties.Location = new System.Drawing.Point(12, 109);
            this.gbProperties.Size = new System.Drawing.Size(310, 311);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Size = new System.Drawing.Size(298, 283);
            // 
            // FrmHistoryOutput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 461);
            this.Name = "FrmHistoryOutput";
            this.Text = "Edit History Output";
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
                if (itemTag is ViewError)  _viewHistoryOutput = null;
                else if (itemTag is ViewNodalHistoryOutput vnho) _viewHistoryOutput = vnho;
                else if (itemTag is ViewElementHistoryOutput veho) _viewHistoryOutput = veho;
                else if (itemTag is ViewContactHistoryOutput vcho) _viewHistoryOutput = vcho;
                else throw new NotImplementedException();
                //
                ShowHideSelectionForm();
                //
                propertyGrid.SelectedObject = itemTag;
                propertyGrid.Select();
                //
                HighlightHistoryOutput();
            }
        }
        protected override void OnPropertyGridPropertyValueChanged()
        {
            string property = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
            //
            if (property == nameof(_viewHistoryOutput.RegionType))
            {
                ShowHideSelectionForm();
                //
                HighlightHistoryOutput();
            }
            else if (_viewHistoryOutput is ViewNodalHistoryOutput vnho &&
                     (property == nameof(vnho.NodeSetName) ||
                      property == nameof(vnho.ReferencePointName) ||
                      property == nameof(vnho.SurfaceName)))
            {
                HighlightHistoryOutput();
            }
            else if (_viewHistoryOutput is ViewElementHistoryOutput veho &&
                    property == nameof(veho.ElementSetName))
            {
                HighlightHistoryOutput();
            }
            else if (_viewHistoryOutput is ViewContactHistoryOutput vcho &&
                    property == nameof(vcho.ContactPairName))
            {
                HighlightHistoryOutput();
            }
            //
            base.OnPropertyGridPropertyValueChanged();
        }
        protected override void OnApply(bool onOkAddNew)
        {
            if (propertyGrid.SelectedObject is ViewError ve) throw new CaeGlobals.CaeException(ve.Message);
            //
            _viewHistoryOutput = (ViewHistoryOutput)propertyGrid.SelectedObject;
            //
            if (HistoryOutput == null) throw new CaeGlobals.CaeException("No history output was selected.");
            //
            if (HistoryOutput.RegionType == RegionTypeEnum.Selection &&
                (HistoryOutput.CreationIds == null || HistoryOutput.CreationIds.Length == 0))
                throw new CaeException("The history output must contain at least one item.");
            //
            if ((_historyOutputToEditName == null &&
                 _historyOutputNames.Contains(HistoryOutput.Name)) ||   // named to existing name
                (HistoryOutput.Name != _historyOutputToEditName &&
                 _historyOutputNames.Contains(HistoryOutput.Name)))     // renamed to existing name
                throw new CaeGlobals.CaeException("The selected history output name already exists.");            
            // Create
            if (_historyOutputToEditName == null)
            {
                _controller.AddHistoryOutputCommand(_stepName, HistoryOutput);
            }
            // Replace
            else if (_propertyItemChanged)
            {
                _controller.ReplaceHistoryOutputCommand(_stepName, _historyOutputToEditName, HistoryOutput);
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
        protected override bool OnPrepareForm(string stepName, string historyToOutputToEditName)
        {
            // To prevent clear of the selection
            _selectedPropertyGridItemChangedEventActive = false;
            // To prevent the call to frmMain.itemForm_VisibleChanged when minimized
            this.DialogResult = DialogResult.None;      
            this.btnOkAddNew.Visible = historyToOutputToEditName == null;
            //
            _propertyItemChanged = false;
            _stepName = null;
            _historyOutputNames = null;
            _historyOutputToEditName = null;
            _viewHistoryOutput = null;
            lvTypes.Items.Clear();
            propertyGrid.SelectedObject = null;
            //
            _stepName = stepName;
            _historyOutputNames = _controller.GetHistoryOutputNamesForStep(_stepName);
            _historyOutputToEditName = historyToOutputToEditName;
            string[] nodeSetNames = _controller.GetUserNodeSetNames();
            string[] elementSetNames = _controller.GetUserElementSetNames();
            string[] surfaceNames = _controller.GetUserSurfaceNames();
            string[] referencePointNames = _controller.GetReferencePointNames();
            string[] contactPairNames = _controller.GetContactPairNames();
            //
            if (_historyOutputNames == null)
                throw new CaeGlobals.CaeException("The history output names must be defined first.");
            // Populate list view
            PopulateListOfHistoryOutputs(nodeSetNames, elementSetNames, surfaceNames, referencePointNames, contactPairNames);
            // Create new history output
            if (_historyOutputToEditName == null)
            {
                lvTypes.Enabled = true;
                _viewHistoryOutput = null;
            }
            else
            // Edit existing history output
            {
                // Get and convert a converted load back to selection
                HistoryOutput = _controller.GetHistoryOutput(_stepName, _historyOutputToEditName); // to clone
                if (HistoryOutput.CreationData != null) HistoryOutput.RegionType = RegionTypeEnum.Selection;
                // Select the appropriate history output in the list view - disable event SelectedIndexChanged
                _lvTypesSelectedIndexChangedEventActive = false;
                if (_viewHistoryOutput is ViewNodalHistoryOutput) lvTypes.Items[0].Selected = true;
                else if (_viewHistoryOutput is ViewElementHistoryOutput) lvTypes.Items[1].Selected = true;
                else if (_viewHistoryOutput is ViewContactHistoryOutput) lvTypes.Items[2].Selected = true;
                else throw new NotSupportedException();
                //
                lvTypes.Enabled = false;
                _lvTypesSelectedIndexChangedEventActive = true;
                //
                if (_viewHistoryOutput is ViewNodalHistoryOutput vnho)
                {
                    // Check for deleted entities
                    if (vnho.RegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                    else if (vnho.RegionType == RegionTypeEnum.NodeSetName.ToFriendlyString())
                        CheckMissingValueRef(ref nodeSetNames, vnho.NodeSetName, s => { vnho.NodeSetName = s; });
                    else if (vnho.RegionType == RegionTypeEnum.SurfaceName.ToFriendlyString())
                        CheckMissingValueRef(ref surfaceNames, vnho.SurfaceName, s => { vnho.SurfaceName = s; });
                    else if (vnho.RegionType == RegionTypeEnum.ReferencePointName.ToFriendlyString())
                        CheckMissingValueRef(ref referencePointNames, vnho.ReferencePointName, s => { vnho.ReferencePointName = s; });
                    else throw new NotSupportedException();
                    //
                    vnho.PopululateDropDownLists(nodeSetNames, surfaceNames, referencePointNames);
                }
                else if (_viewHistoryOutput is ViewElementHistoryOutput veho)
                {
                    // Check for deleted entities
                    if (veho.RegionType == RegionTypeEnum.Selection.ToFriendlyString()) { }
                    else if (veho.RegionType == RegionTypeEnum.ElementSetName.ToFriendlyString())
                        CheckMissingValueRef(ref elementSetNames, veho.ElementSetName, s => { veho.ElementSetName = s; });
                    else throw new NotSupportedException();
                    //
                    veho.PopululateDropDownLists(elementSetNames);
                }

                else if (_viewHistoryOutput is ViewContactHistoryOutput vcho)
                {
                    // Check for deleted entities
                    CheckMissingValueRef(ref contactPairNames, vcho.ContactPairName, s => { vcho.ContactPairName = s; });
                    //
                    vcho.PopululateDropDownLists(contactPairNames);
                }
                else throw new NotSupportedException();
                //
                propertyGrid.SelectedObject = _viewHistoryOutput;
                propertyGrid.Select();
            }
            _selectedPropertyGridItemChangedEventActive = true;
            //
            ShowHideSelectionForm();
            //
            HighlightHistoryOutput(); // must be here if called from the menu
            //
            return true;
        }


        // Methods                                                                                                                  
        private void PopulateListOfHistoryOutputs(string[] nodeSetNames, string[] elementSetNames, 
                                                  string[] surfaceNames, string[] referencePointNames, string[] contactPairNames)
        {
            ListViewItem item;
            // Node output
            item = new ListViewItem("Node output");
            NodalHistoryOutput nho = new NodalHistoryOutput(GetHistoryOutputName("N"), NodalHistoryVariable.U,
                                                            "", RegionTypeEnum.Selection);
            ViewNodalHistoryOutput vnho = new ViewNodalHistoryOutput(nho);
            vnho.PopululateDropDownLists(nodeSetNames, surfaceNames, referencePointNames);
            item.Tag = vnho;
            lvTypes.Items.Add(item);
            // Element output
            item = new ListViewItem("Element output");
            ElementHistoryOutput eho = new ElementHistoryOutput(GetHistoryOutputName("E"), ElementHistoryVariable.S,
                                                                "", RegionTypeEnum.Selection);
            ViewElementHistoryOutput veho = new ViewElementHistoryOutput(eho);
            veho.PopululateDropDownLists(elementSetNames);
            item.Tag = veho;
            lvTypes.Items.Add(item);
            // Contact output
            item = new ListViewItem("Contact output");
            if (contactPairNames.Length > 0)
            {
                ContactHistoryOutput cho = new ContactHistoryOutput(GetHistoryOutputName("C"),
                                                                    //ContactHistoryVariable.CDIS |
                                                                    //ContactHistoryVariable.CSTR |
                                                                    ContactHistoryVariable.CF,
                                                                    contactPairNames[0]);
                ViewContactHistoryOutput vcho = new ViewContactHistoryOutput(cho);
                vcho.PopululateDropDownLists(contactPairNames);
                item.Tag = vcho;
            }
            else item.Tag = new ViewError("There is no contact pair defined for the history output definition.");
            //
            lvTypes.Items.Add(item);
        }
        private string GetHistoryOutputName(string prefix)
        {
            return NamedClass.GetNewValueName(_historyOutputNames, prefix + "H_output-");
        }
        private void HighlightHistoryOutput()
        {
            try
            {
                _controller.ClearSelectionHistory();
                //
                if (_viewHistoryOutput == null) { }
                else if (HistoryOutput is NodalHistoryOutput || HistoryOutput is ElementHistoryOutput)
                {
                    if (HistoryOutput.RegionType == RegionTypeEnum.NodeSetName ||
                        HistoryOutput.RegionType == RegionTypeEnum.ReferencePointName ||
                        HistoryOutput.RegionType == RegionTypeEnum.SurfaceName ||
                        HistoryOutput.RegionType == RegionTypeEnum.ElementSetName)
                    {
                        _controller.Highlight3DObjects(new object[] { HistoryOutput.RegionName });
                    }
                    else if (HistoryOutput.RegionType == RegionTypeEnum.Selection)
                    {
                        SetSelectItem();
                        //
                        if (HistoryOutput.CreationData != null)
                        {
                            _controller.Selection = HistoryOutput.CreationData.DeepClone();
                            _controller.HighlightSelection();
                        }
                    }
                    else throw new NotImplementedException();
                }
                else if (HistoryOutput is ContactHistoryOutput)
                {
                    _controller.HighlightContactPairs(new string[] { HistoryOutput.RegionName });
                }
                else throw new NotSupportedException();
            }
            catch { }
        }
        private void ShowHideSelectionForm()
        {
            if (HistoryOutput != null && HistoryOutput.RegionType == RegionTypeEnum.Selection)
                ItemSetDataEditor.SelectionForm.ShowIfHidden(this.Owner);
            else
                ItemSetDataEditor.SelectionForm.Hide();
            //
            SetSelectItem();
        }
        private void SetSelectItem()
        {
            if (HistoryOutput != null && HistoryOutput.RegionType == RegionTypeEnum.Selection)
            {
                if (HistoryOutput is null) { }
                else if (HistoryOutput is NodalHistoryOutput) _controller.SetSelectItemToNode();
                else if (HistoryOutput is ElementHistoryOutput) _controller.SetSelectItemToElement();
                else if (HistoryOutput is ContactHistoryOutput) _controller.SelectItem = vtkSelectItem.None;
            }
            else _controller.SetSelectByToOff();
        }
        //
        public void SelectionChanged(int[] ids)
        {
            if (HistoryOutput != null && HistoryOutput.RegionType == RegionTypeEnum.Selection)
            {
                if (HistoryOutput is NodalHistoryOutput || HistoryOutput is ElementHistoryOutput)
                {
                    HistoryOutput.CreationIds = ids;
                    HistoryOutput.CreationData = _controller.Selection.DeepClone();
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
            HighlightHistoryOutput();
        }

        // IFormItemSetDataParent
        public bool IsSelectionGeometryBased()
        {
            // Prepare ItemSetDataEditor - prepare Geometry or Mesh based selection
            if (HistoryOutput == null || HistoryOutput.CreationData == null) return true;
            return HistoryOutput.CreationData.IsGeometryBased();
        }
    }
}
