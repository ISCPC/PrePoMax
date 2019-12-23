using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using System.Windows.Forms;
using CaeGlobals;

namespace PrePoMax.Forms
{
    class FrmElementSet : UserControls.FrmProperties, IFormBase, IFormItemSetDataParent
    {
        // Variables                                                                                                                
        private HashSet<string> _allExistingNames;
        private string _elementSetToEditName;
        private ViewElementSet _viewElementSet;
        private List<SelectionNode> _prevSelectionNodes;
        private SelectionNodeIds _selectionNodeIds;
        private Controller _controller;


        // Properties                                                                                                               
        public FeElementSet ElementSet
        {
            get { return _viewElementSet.GetBase(); }
            set { _viewElementSet = new ViewElementSet(this, value.DeepClone()); }
        }


        // Constructors                                                                                                             
        public FrmElementSet(Controller controller)
        {
            InitializeComponent();

            _controller = controller;
            _viewElementSet = null;
            _allExistingNames = new HashSet<string>();
        }
        private void InitializeComponent()
        {
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // FrmElementSet
            // 
            this.ClientSize = new System.Drawing.Size(334, 411);
            this.Name = "FrmElementSet";
            this.Text = "Edit Element Set";
            this.gbProperties.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Overrides                                                                                                                
        protected override void Apply()
        {
            _viewElementSet = (ViewElementSet)propertyGrid.SelectedObject;            

            if ((_elementSetToEditName == null && _allExistingNames.Contains(_viewElementSet.Name)) ||                   // named to existing name
                (_viewElementSet.Name != _elementSetToEditName && _allExistingNames.Contains(_viewElementSet.Name)))     // renamed to existing name
                throw new CaeException("The selected name already exists.");

            if (ElementSet.Labels == null || ElementSet.Labels.Length <= 0)
                throw new CaeException("The element set must contain at least one item.");

            if (_elementSetToEditName == null)
            {
                // Create
                _controller.AddElementSetCommand(ElementSet);
            }
            else
            {
                // Replace
                if (_propertyItemChanged || !ElementSet.Valid)
                {
                    // replace the ids by the previous selection
                    Selection selection = ElementSet.CreationData;
                    if (selection.Nodes[0] is SelectionNodeIds sn && sn.Equals(_selectionNodeIds))
                    {
                        selection.Nodes.RemoveAt(0);
                        selection.Nodes.InsertRange(0, _prevSelectionNodes);
                    }

                    ElementSet.Valid = true;
                    _controller.ReplaceElementSetCommand(_elementSetToEditName, ElementSet);
                }
            }
        }
        protected override bool OnPrepareForm(string stepName, string elementSetToEditName)
        {
            this.DialogResult = DialogResult.None;      // to prevent the call to frmMain.itemForm_VisibleChanged when minimized
            this.btnOkAddNew.Visible = elementSetToEditName == null;

            _controller.SetSelectItemToElement();

            _propertyItemChanged = false;
            _allExistingNames.Clear();
            _elementSetToEditName = null;
            _viewElementSet = null;
            propertyGrid.SelectedObject = null;
            _prevSelectionNodes = null;
            _selectionNodeIds = null;

            _allExistingNames.UnionWith(_controller.GetAllMeshEntityNames());
            _elementSetToEditName = elementSetToEditName;

            if (_elementSetToEditName == null)
            {
                ElementSet = new FeElementSet(GetElementSetName(), null);
                _controller.Selection.Clear();
            }
            else
            {
                ElementSet = _controller.GetElementSet(_elementSetToEditName);  // to clone

                int[] ids = ElementSet.Labels;                                  // change node selection history to ids to speed up
                _selectionNodeIds = new SelectionNodeIds(vtkSelectOperation.None, false, ids);
                _prevSelectionNodes = ElementSet.CreationData.Nodes;
                _controller.ClearSelectionHistory();
                _controller.AddSelectionNode(_selectionNodeIds, true);
                ElementSet.CreationData = _controller.Selection.DeepClone();
            }

            propertyGrid.SelectedObject = _viewElementSet;
            propertyGrid.Select();

            _controller.HighlightSelection();

            return true;
        }
        protected override void OnEnabledChanged()
        {
            // form is Enabled On and Off by the itemSetForm
            if (this.Enabled)
            {
                if (this.DialogResult == System.Windows.Forms.DialogResult.OK)              // the FrmItemSet was closed with OK
                {
                    ElementSet.CreationData = _controller.Selection.DeepClone();
                    _propertyItemChanged = true;
                }
                else if (this.DialogResult == System.Windows.Forms.DialogResult.Cancel)     // the FrmItemSet was closed with Cancel
                {
                    if (ElementSet.CreationData != null)
                    {
                        _controller.Selection.CopySelectonData(ElementSet.CreationData);
                        _controller.HighlightSelection();
                    }
                }
            }
            this.DialogResult = System.Windows.Forms.DialogResult.None;
        }


        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string elementSetToEditName)
        {
            return OnPrepareForm(stepName, elementSetToEditName);
        }
        private string GetElementSetName()
        {
            return NamedClass.GetNewValueName(_allExistingNames.ToArray(), "Element_Set-");
        }


        // IFormItemSetDataParent
        public bool IsSelectionGeometryBased()
        {
            // prepare ItemSetDataEditor
            if (_elementSetToEditName == null) return true;
            return _controller.GetElementSet(_elementSetToEditName).CreationData.IsGeometryBased();
        }
    }
}
