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
    class FrmNodeSet : UserControls.FrmProperties, IFormBase, IFormItemSetDataParent, IFormHighlight
    {
        // Variables                                                                                                                
        private HashSet<string> _allExistingNames;
        private string _nodeSetToEditName;
        private ViewNodeSet _viewNodeSet;
        private List<SelectionNode> _prevSelectionNodes;
        private SelectionNodeIds _selectionNodeIds;
        private Controller _controller;


        // Properties                                                                                                               
        public FeNodeSet NodeSet
        {
            get { return _viewNodeSet.GetBase(); }
            set { _viewNodeSet = new ViewNodeSet(this, value.DeepClone()); }
        }


        // Constructors                                                                                                             
        public FrmNodeSet(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewNodeSet = null;
            _allExistingNames = new HashSet<string>();
        }
        private void InitializeComponent()
        {
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // FrmNodeSet
            // 
            this.ClientSize = new System.Drawing.Size(334, 411);
            this.Name = "FrmNodeSet";
            this.Text = "Edit Node Set";
            this.gbProperties.ResumeLayout(false);
            this.ResumeLayout(false);
        }


        // Overrides                                                                                                                
        protected override void OnApply(bool onOkAddNew)
        {
            _viewNodeSet = (ViewNodeSet)propertyGrid.SelectedObject;
            //
            if ((_nodeSetToEditName == null && _allExistingNames.Contains(_viewNodeSet.Name)) ||                // named to existing name
                (_viewNodeSet.Name != _nodeSetToEditName && _allExistingNames.Contains(_viewNodeSet.Name)))     // renamed to existing name
                throw new CaeGlobals.CaeException("The selected name already exists.");
            //
            if (NodeSet.Labels == null || NodeSet.Labels.Length <= 0) throw new CaeGlobals.CaeException("The node set must contain at least one item.");
            //
            if (_nodeSetToEditName == null)
            {
                // Create
                _controller.AddNodeSetCommand(NodeSet);
            }
            else
            {
                // Replace
                if (_propertyItemChanged || !NodeSet.Valid)
                {
                    // replace the ids by the previous selection
                    Selection selection = NodeSet.CreationData;
                    if (selection.Nodes[0] is SelectionNodeIds sn && sn.Equals(_selectionNodeIds))
                    {
                        selection.Nodes.RemoveAt(0);
                        selection.Nodes.InsertRange(0, _prevSelectionNodes);
                    }
                    //
                    NodeSet.Valid = true;
                    _controller.ReplaceNodeSetCommand(_nodeSetToEditName, NodeSet);
                }
            }
            // If all is successful close the ItemSetSelectionForm
            ItemSetDataEditor.SelectionForm.Hide();
        }
        protected override void OnHideOrClose()
        {
            // Close the ItemSetSelectionForm
            ItemSetDataEditor.SelectionForm.Hide();
            //
            base.OnHideOrClose();
        }
        protected override bool OnPrepareForm(string stepName, string nodeSetToEditName)
        {
            this.DialogResult = DialogResult.None;      // to prevent the call to frmMain.itemForm_VisibleChanged when minimized
            this.btnOkAddNew.Visible = nodeSetToEditName == null;
            //
            _controller.SetSelectItemToNode();
            //
            _propertyItemChanged = false;
            _allExistingNames.Clear();
            _nodeSetToEditName = null;
            _viewNodeSet = null;
            propertyGrid.SelectedObject = null;
            _prevSelectionNodes = null;
            _selectionNodeIds = null;
            //
            _allExistingNames.UnionWith(_controller.GetAllMeshEntityNames());
            _nodeSetToEditName = nodeSetToEditName;
            // Create new nodeset
            if (_nodeSetToEditName == null)
            {
                NodeSet = new FeNodeSet(GetNodeSetName(), null);
                _controller.Selection.Clear();
            }
            // Edit existing nodeset
            else
            {
                NodeSet = _controller.GetNodeSet(_nodeSetToEditName);   // to clone
                // Change node selection history to ids to speed up
                int[] ids = NodeSet.Labels;                             
                _selectionNodeIds = new SelectionNodeIds(vtkSelectOperation.None, false, ids);
                _prevSelectionNodes = NodeSet.CreationData.Nodes;
                _controller.CreateNewSelection(NodeSet.CreationData.CurrentView, _selectionNodeIds, true);
                NodeSet.CreationData = _controller.Selection.DeepClone();
            }
            //
            propertyGrid.SelectedObject = _viewNodeSet;
            propertyGrid.Select();
            // Show ItemSetDataForm
            ItemSetDataEditor.SelectionForm.ItemSetData = new ItemSetData(NodeSet.Labels);
            ItemSetDataEditor.SelectionForm.ShowIfHidden(this.Owner);
            //
            _controller.HighlightSelection();
            //
            return true;
        }


        // Methods                                                                                                                  
        private string GetNodeSetName()
        {
            return NamedClass.GetNewValueName(_allExistingNames, "Node_Set-");
        }
        private void HighlightNodeSet()
        {
            try
            {
                if (NodeSet.CreationData != null)
                {
                    _controller.Selection = NodeSet.CreationData.DeepClone();
                    _controller.HighlightSelection();
                }
            }
            catch { }
        }
        //
        public void SelectionChanged(int[] ids)
        {
            NodeSet.Labels = ids;
            NodeSet.CreationData = _controller.Selection.DeepClone();
            _controller.GetNodesCenterOfGravity(NodeSet);
            //
            propertyGrid.Refresh();
            //
            _propertyItemChanged = true;
        }

        // IFormHighlight
        public void Highlight()
        {
            HighlightNodeSet();
        }

        // IFormItemSetDataParent
        public bool IsSelectionGeometryBased()
        {
            // Prepare ItemSetDataEditor - prepare Geometry or Mesh based selection
            if (_nodeSetToEditName == null) return true;
            return _controller.GetNodeSet(_nodeSetToEditName).CreationData.IsGeometryBased(); // NodeSet was modified for speed up
        }

    }
}
