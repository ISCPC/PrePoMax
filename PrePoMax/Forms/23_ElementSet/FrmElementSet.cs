using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.Windows.Forms;

namespace PrePoMax.Forms
{
    class FrmElementSet : UserControls.FrmProperties, IFormBase
    {
        // Variables                                                                                                                
        private HashSet<string> _allExistingNames;
        private string _elementSetToEditName;
        private ViewElementSet _viewElementSet;
        private Selection _selection;
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
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(160, 376);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(241, 376);
            // 
            // btnOkAddNew
            // 
            this.btnOkAddNew.Location = new System.Drawing.Point(65, 376);
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
                    ElementSet.Valid = true;
                    _controller.ReplaceElementSetCommand(_elementSetToEditName, ElementSet);
                }
            }
            _selection.Clear();
        }
        protected override void OnPrepareForm(string stepName, string elementSetToEditName)
        {
            this.DialogResult = DialogResult.None;      // to prevent the call to frmMain.itemForm_VisibleChanged when minimized
            this.btnOkAddNew.Visible = elementSetToEditName == null;

            _controller.SetSelectItemToElement();

            _propertyItemChanged = false;
            _allExistingNames.Clear();
            _elementSetToEditName = null;
            _viewElementSet = null;
            propertyGrid.SelectedObject = null;
            _selection = null;

            _allExistingNames.UnionWith(_controller.GetAllMeshEntityNames());
            _elementSetToEditName = elementSetToEditName;

            if (_elementSetToEditName == null)
            {
                ElementSet = new FeElementSet(GetElementSetName(), null);
                _controller.Selection.Clear();
            }
            else
            {
                ElementSet = _controller.GetElementSet(_elementSetToEditName); // to clone
                int[] ids = ElementSet.Labels;
                SelectionNodeIds selectionNode = new SelectionNodeIds(vtkControl.vtkSelectOperation.None, false, ids);
                _controller.ClearSelectionHistory();
                _controller.AddSelectionNode(selectionNode, true);
                ElementSet.CreationData = _controller.Selection.DeepClone();
            }
            _selection = _controller.Selection;

            propertyGrid.SelectedObject = _viewElementSet;
            propertyGrid.Select();

            _controller.HighlightSelection();
        }
        protected override void OnEnabledChanged()
        {
            // form is Enabled On and Off by the itemSetForm
            if (this.Enabled)
            {
                if (this.DialogResult == System.Windows.Forms.DialogResult.OK)              // the FrmItemSet was closed with OK
                {
                    ElementSet.CreationData = _selection.DeepClone();
                    _propertyItemChanged = true;
                }
                else if (this.DialogResult == System.Windows.Forms.DialogResult.Cancel)     // the FrmItemSet was closed with Cancel
                {
                    if (ElementSet.CreationData != null)
                    {
                        _selection.CopySelectonData(ElementSet.CreationData as Selection);
                        _controller.HighlightSelection();
                    }
                }
            }
            this.DialogResult = System.Windows.Forms.DialogResult.None;
        }


        // Methods                                                                                                                  
        public void PrepareForm(string stepName, string elementSetToEditName)
        {
            OnPrepareForm(stepName, elementSetToEditName);
        }
        private string GetElementSetName()
        {
            return NamedClass.GetNewValueName(_allExistingNames.ToArray(), "ElementSet-");
        }
    }
}
