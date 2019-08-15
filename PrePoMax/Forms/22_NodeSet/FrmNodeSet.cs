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
    class FrmNodeSet : UserControls.FrmProperties, IFormBase, IFormItemSetDataParent
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
        protected override void Apply()
        {
            _viewNodeSet = (ViewNodeSet)propertyGrid.SelectedObject;

            if ((_nodeSetToEditName == null && _allExistingNames.Contains(_viewNodeSet.Name)) ||                // named to existing name
                (_viewNodeSet.Name != _nodeSetToEditName && _allExistingNames.Contains(_viewNodeSet.Name)))     // renamed to existing name
                throw new CaeGlobals.CaeException("The selected name already exists.");

            if (NodeSet.Labels == null || NodeSet.Labels.Length <= 0) throw new CaeGlobals.CaeException("The node set must contain at least one item.");

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

                    NodeSet.Valid = true;
                    _controller.ReplaceNodeSetCommand(_nodeSetToEditName, NodeSet);
                }
            }
        }
        protected override bool OnPrepareForm(string stepName, string nodeSetToEditName)
        {
            this.DialogResult = DialogResult.None;      // to prevent the call to frmMain.itemForm_VisibleChanged when minimized
            this.btnOkAddNew.Visible = nodeSetToEditName == null;

            _controller.SetSelectItemToNode();

            _propertyItemChanged = false;
            _allExistingNames.Clear();
            _nodeSetToEditName = null;
            _viewNodeSet = null;
            propertyGrid.SelectedObject = null;
            _prevSelectionNodes = null;
            _selectionNodeIds = null;

            _allExistingNames.UnionWith(_controller.GetAllMeshEntityNames());
            _nodeSetToEditName = nodeSetToEditName;

            if (_nodeSetToEditName == null)
            {
                NodeSet = new FeNodeSet(GetNodeSetName(), null);
                _controller.Selection.Clear();
            }
            else
            {
                NodeSet = _controller.GetNodeSet(_nodeSetToEditName);   // to clone

                int[] ids = NodeSet.Labels;                             // change node selection history to ids to speed up
                _selectionNodeIds = new SelectionNodeIds(vtkSelectOperation.None, false, ids);
                _prevSelectionNodes = NodeSet.CreationData.Nodes;
                _controller.ClearSelectionHistory();
                _controller.AddSelectionNode(_selectionNodeIds, true);
                NodeSet.CreationData = _controller.Selection.DeepClone();
            }

            propertyGrid.SelectedObject = _viewNodeSet;
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
                    NodeSet.CreationData = _controller.Selection.DeepClone();
                    _controller.GetNodesCenterOfGravity(NodeSet);
                    _propertyItemChanged = true;
                }
                else if (this.DialogResult == System.Windows.Forms.DialogResult.Cancel)     // the FrmItemSet was closed with Cancel
                {
                    if (NodeSet.CreationData != null)
                    {
                        _controller.Selection.CopySelectonData(NodeSet.CreationData);
                        _controller.HighlightSelection();
                    }
                }
            }
            this.DialogResult = System.Windows.Forms.DialogResult.None;
        }


        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string nodeSetToEditName)
        {
            return OnPrepareForm(stepName, nodeSetToEditName);
        }
        private string GetNodeSetName()
        {
            return NamedClass.GetNewValueName(_allExistingNames.ToArray(), "NodeSet-");
        }


        // IFormItemSetDataParent
        public bool IsSelectionGeometryBased()
        {
            // prepare ItemSetDataEditor
            if (_nodeSetToEditName == null) return true;
            return _controller.GetNodeSet(_nodeSetToEditName).CreationData.IsGeometryBased();
        }

    }
}
