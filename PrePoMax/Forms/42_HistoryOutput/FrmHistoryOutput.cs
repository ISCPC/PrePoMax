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
    class FrmHistoryOutput : UserControls.FrmPropertyListView, IFormBase
    {
        // Variables                                                                                                                
        private string[] _historyOutputNames;
        private string _historyOutputToEditName;
        private ViewHistoryOutput _viewHistoryOutput;
        private Controller _controller;


        // Properties                                                                                                               
        public HistoryOutput HistoryOutput
        {
            get { return _viewHistoryOutput.GetBase(); }
            set
            {
                if (value is NodalHistoryOutput nho) _viewHistoryOutput = new ViewNodalHistoryOutput(nho.DeepClone());
                else if (value is ElementHistoryOutput eho) _viewHistoryOutput = new ViewElementHistoryOutput(eho.DeepClone());
                else throw new NotImplementedException();
            }
        }


        // Constructors                                                                                                             
        public FrmHistoryOutput(Controller controller)
        {
            InitializeComponent();

            _controller = controller;
            _viewHistoryOutput = null;

            _selectedPropertyGridItemChangedEventActive = true;
        }
        private void InitializeComponent()
        {
            this.gbType.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
           
            // 
            // FrmSection
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

                if (propertyGrid.SelectedObject == null) { }
                else if (propertyGrid.SelectedObject is ViewNodalHistoryOutput nho)
                {
                    if (valueString == nho.NodeSetName) objects = new object[] { nho.NodeSetName };
                    else if (valueString == nho.SurfaceName) objects = new object[] { nho.SurfaceName };
                    else objects = null;
                }
                else if (propertyGrid.SelectedObject is ViewElementHistoryOutput eho)
                {
                    objects = new object[] { eho.ElementSetName };
                }
                else if (propertyGrid.SelectedObject is PrePoMax.Forms.ViewError)
                {
                }
                else throw new NotImplementedException();

                _controller.Highlight3DObjects(objects);
            }
        }
        protected override void Apply()
        {
            _viewHistoryOutput = (ViewHistoryOutput)propertyGrid.SelectedObject;

            if ((_historyOutputToEditName == null && _historyOutputNames.Contains(_viewHistoryOutput.Name)) ||                      // create
                (_viewHistoryOutput.Name != _historyOutputToEditName && _historyOutputNames.Contains(_viewHistoryOutput.Name)))     // edit
                throw new CaeGlobals.CaeException("The selected history output name already exists.");

            if (_historyOutputToEditName == null)
            {
                // Create
                _controller.AddHistoryOutputCommand(_stepName, HistoryOutput);
            }
            else
            {
                // Replace
                if (_propertyItemChanged) _controller.ReplaceHistoryOutputCommand(_stepName, _historyOutputToEditName, HistoryOutput);
            }
        }
        protected override bool OnPrepareForm(string stepName, string historyToOutputToEditName)
        {
            _selectedPropertyGridItemChangedEventActive = false;                             // to prevent clear of the selection

            this.DialogResult = DialogResult.None;      // to prevent the call to frmMain.itemForm_VisibleChanged when minimized
            this.btnOkAddNew.Visible = historyToOutputToEditName == null;

            _propertyItemChanged = false;
            _stepName = null;
            _historyOutputNames = null;
            _historyOutputToEditName = null;
            _viewHistoryOutput = null;
            lvTypes.Items.Clear();
            propertyGrid.SelectedObject = null;

            _stepName = stepName;
            _historyOutputNames = _controller.GetHistoryOutputNamesForStep(_stepName);
            _historyOutputToEditName = historyToOutputToEditName;
            string[] nodeSetNames = _controller.GetUserNodeSetNames();
            string[] elementSetNames = _controller.GetUserElementSetNames();
            string[] surfaceNames = _controller.GetSurfaceNames();
            string[] referencePointNames = _controller.GetReferencePointNames();

            if (_historyOutputNames == null)
                throw new CaeGlobals.CaeException("The history output names must be defined first.");

            PopulateListOfHistoryOutputs(nodeSetNames, elementSetNames, surfaceNames, referencePointNames);

            // Add history outputs
            if (_historyOutputToEditName == null)
            {
                lvTypes.Enabled = true;
                _viewHistoryOutput = null;

                if (lvTypes.Items.Count == 1) lvTypes.Items[0].Selected = true;
            }
            else
            {
                HistoryOutput = _controller.GetHistoryOutput(_stepName, _historyOutputToEditName); // to clone

                // select the appropriate history output in the list view - disable event SelectedIndexChanged
                _lvTypesSelectedIndexChangedEventActive = false;
                if (_viewHistoryOutput is ViewNodalHistoryOutput) lvTypes.Items[0].Selected = true;
                else if (_viewHistoryOutput is ViewElementHistoryOutput) lvTypes.Items[1].Selected = true;
                lvTypes.Enabled = false;
                _lvTypesSelectedIndexChangedEventActive = true;

                if (_viewHistoryOutput is ViewNodalHistoryOutput vnho)
                {
                    // Check for deleted entities
                    if (vnho.RegionType == RegionTypeEnum.NodeSetName.ToFriendlyString())
                        CheckMissingValueRef(ref nodeSetNames, vnho.NodeSetName, s => { vnho.NodeSetName = s; });
                    else if (vnho.RegionType == RegionTypeEnum.SurfaceName.ToFriendlyString())
                        CheckMissingValueRef(ref surfaceNames, vnho.SurfaceName, s => { vnho.SurfaceName = s; });
                    else if (vnho.RegionType == RegionTypeEnum.ReferencePointName.ToFriendlyString())
                        CheckMissingValueRef(ref referencePointNames, vnho.ReferencePointName, s => { vnho.ReferencePointName = s; });
                    else throw new NotSupportedException();

                    vnho.PopululateDropDownLists(nodeSetNames, surfaceNames, referencePointNames);
                }
                else if (_viewHistoryOutput is ViewElementHistoryOutput veho)
                {
                    // Check for deleted entities
                    CheckMissingValueRef(ref elementSetNames, veho.ElementSetName, s => { veho.ElementSetName = s; });
                    veho.PopululateDropDownLists(elementSetNames);
                }

                propertyGrid.SelectedObject = _viewHistoryOutput;
                propertyGrid.Select();

            }
            _selectedPropertyGridItemChangedEventActive = true;

            return true;
        }


        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string sectionToEditName)
        {
            return OnPrepareForm(stepName, sectionToEditName);
        }
        private void PopulateListOfHistoryOutputs(string[] nodeSetNames, string[] elementSetNames, 
                                                  string[] surfaceNames, string[] referencePointNames)
        {
            ListViewItem item;

            // Node
            item = new ListViewItem("Node output");
            if (nodeSetNames.Length + surfaceNames.Length >= 1)
            {
                NodalHistoryOutput nho;
                if (nodeSetNames.Length > 0)
                    nho = new NodalHistoryOutput(GetHistoryOutputName("N"), NodalHistoryVariable.U, nodeSetNames[0], RegionTypeEnum.NodeSetName);
                else
                    nho = new NodalHistoryOutput(GetHistoryOutputName("N"), NodalHistoryVariable.U, surfaceNames[0], RegionTypeEnum.SurfaceName);

                ViewNodalHistoryOutput vnho = new ViewNodalHistoryOutput(nho);
                vnho.PopululateDropDownLists(nodeSetNames, surfaceNames, referencePointNames);
                item.Tag = vnho;
            }
            else item.Tag = new ViewError("There is no node set/surface defined for the history output definition.");
            lvTypes.Items.Add(item);

            // Element
            item = new ListViewItem("Element output");
            if (elementSetNames.Length >= 1)
            {
                ElementHistoryOutput eho;
                eho = new ElementHistoryOutput(GetHistoryOutputName("E"), ElementHistoryVariable.S, elementSetNames[0], RegionTypeEnum.ElementSetName);

                ViewElementHistoryOutput veho = new ViewElementHistoryOutput(eho);
                veho.PopululateDropDownLists(elementSetNames);
                item.Tag = veho;
            }
            else item.Tag = new ViewError("There is no element set defined for the history output definition.");
            lvTypes.Items.Add(item);
        }
        private string GetHistoryOutputName(string prefix)
        {
            return NamedClass.GetNewValueName(_historyOutputNames, prefix + "H-Output-");
        }
    }
}
