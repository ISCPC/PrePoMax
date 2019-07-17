using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeMesh;
using CaeGlobals;

namespace PrePoMax.Forms
{
    class FrmSurface : UserControls.FrmProperties, IFormBase
    {
        // Variables                                                                                                                
        private HashSet<string> _allExistingNames;
        private string _surfaceToEditName;
        private ViewFeSurface _viewSurface;
        private List<SelectionNode> _prevSelectionNodes;
        private SelectionNodeIds _selectionNodeIds;
        private Controller _controller;


        // Properties                                                                                                               
        public FeSurface Surface
        {
            get { return _viewSurface.GetBase(); }
            set { _viewSurface = new ViewFeSurface(value.DeepClone()); }
        }


        // Constructors                                                                                                             
        public FrmSurface(Controller controller) 
        {
            InitializeComponent();

            _controller = controller;
            _viewSurface = null;
            _allExistingNames = new HashSet<string>();

            SelectionClear = _controller.Selection.Clear;
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
            // FrmSurface
            // 
            this.ClientSize = new System.Drawing.Size(334, 411);
            this.Name = "FrmSurface";
            this.Text = "Edit Surface";
            this.gbProperties.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Overrides                                                                                                                
        protected override void OnPropertyGridPropertyValueChanged()
        {
            string value = (string)propertyGrid.SelectedGridItem.Value.ToString();

            if (value == FeSurfaceCreatedFrom.Selection.ToString()) HighlightSurface();
            else if (value == FeSurfaceCreatedFrom.NodeSet.ToString()) HighlightSurface();
            else if (value == _viewSurface.NodeSetName) HighlightSurface();

            base.OnPropertyGridPropertyValueChanged();
        }
        protected override void Apply()
        {
            _viewSurface = (ViewFeSurface)propertyGrid.SelectedObject;

            if ((_surfaceToEditName == null && _allExistingNames.Contains(_viewSurface.Name)) ||                // named to existing name
                (_viewSurface.Name != _surfaceToEditName && _allExistingNames.Contains(_viewSurface.Name)))     // renamed to existing name
                throw new CaeException("The selected name already exists.");

            if (_viewSurface.CreateSurfaceFrom == FeSurfaceCreatedFrom.Selection &&
                (_viewSurface.ItemSetData.ItemIds == null || _viewSurface.ItemSetData.ItemIds.Length == 0))
                throw new CaeException("The surface must contain at least one item.");

            if (_surfaceToEditName == null)
            {
                // Create
                _controller.AddSurfaceCommand(Surface);
            }
            else
            {
                // Replace
                if (_propertyItemChanged || !Surface.Valid)
                {
                    if (Surface.CreatedFrom == FeSurfaceCreatedFrom.Selection)
                    {
                        // replace back the ids by the previous selection
                        Selection selection = (Selection)Surface.CreationData;
                        if (selection.Nodes[0] is SelectionNodeIds sn && sn.Equals(_selectionNodeIds))
                        {
                            selection.Nodes.RemoveAt(0);
                            selection.Nodes.InsertRange(0, _prevSelectionNodes);
                        }
                    }

                    Surface.Valid = true;
                    _controller.ReplaceSurfaceCommand(_surfaceToEditName, Surface);
                }
                //else _controller.UpdateHighlightFromTree();
            }
            _controller.Selection.Clear();
        }
        protected override bool OnPrepareForm(string stepName, string surfaceToEditName)
        {
            this.DialogResult = DialogResult.None;      // to prevent the call to frmMain.itemForm_VisibleChanged when minimized
            this.btnOkAddNew.Visible = surfaceToEditName == null;

            _propertyItemChanged = false;
            _allExistingNames.Clear();
            _surfaceToEditName = null;
            _viewSurface = null;
            propertyGrid.SelectedObject = null;
            _prevSelectionNodes = null;
            _selectionNodeIds = null;

            _controller.SetSelectItemToSurface();

            _allExistingNames.UnionWith(_controller.GetAllMeshEntityNames());
            _surfaceToEditName = surfaceToEditName;

            string[] nodeSetNames = _controller.GetUserNodeSetNames();

            if (_surfaceToEditName == null)
            {
                Surface = new FeSurface(GetSurfaceName());
                _controller.Selection.Clear();
            }
            else
            {
                Surface = _controller.GetSurface(_surfaceToEditName);   // to clone

                if (Surface.FaceIds != null)
                {
                    // change node selection history to ids to speed up
                    int[] ids = Surface.FaceIds;
                    _selectionNodeIds = new SelectionNodeIds(vtkSelectOperation.None, false, ids);
                    // if the base surface was remeshed the creation data does not exist any more - recreate it by ids
                    //if (Surface.CreationData != null) _prevSelectionNodes = ((Selection)Surface.CreationData).Nodes;
                    //else _prevSelectionNodes = new List<SelectionNode>() { _selectionNodeIds };
                    //
                    _prevSelectionNodes = ((Selection)Surface.CreationData).Nodes;
                    _controller.ClearSelectionHistory();
                    _controller.AddSelectionNode(_selectionNodeIds, true);
                    Surface.CreationData = _controller.Selection.DeepClone();
                }

                if (Surface.CreatedFrom == FeSurfaceCreatedFrom.NodeSet)
                    CheckMissingValueRef(ref nodeSetNames, _viewSurface.NodeSetName, s => { _viewSurface.NodeSetName = s; });
            }

            _viewSurface.PopululateDropDownList(nodeSetNames);

            propertyGrid.SelectedObject = _viewSurface;
            propertyGrid.Select();

            // add surface selection data to selection history
            HighlightSurface();

            return true;
        }
        protected override void OnEnabledChanged()
        {
            // form is Enabled On and Off by the itemSetForm
            if (this.Enabled)
            {
                if (this.DialogResult == System.Windows.Forms.DialogResult.OK)              // the FrmItemSet was closed with OK
                {
                    Surface.CreationData = _controller.Selection.DeepClone();
                    _propertyItemChanged = true;
                    UpdateSurfaceArea();
                }
                else if (this.DialogResult == System.Windows.Forms.DialogResult.Cancel)     // the FrmItemSet was closed with Cancel
                {
                    if (Surface.CreationData != null)
                    {
                        _controller.Selection.CopySelectonData(Surface.CreationData as Selection);
                        HighlightSurface();
                    }
                }
            }
            this.DialogResult = System.Windows.Forms.DialogResult.None;
        }


        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string surfaceToEditName)
        {
            return OnPrepareForm(stepName, surfaceToEditName);
        }
        private string GetSurfaceName()
        {
            return NamedClass.GetNewValueName(_allExistingNames.ToArray(), "Surface-");
        }
        private void HighlightSurface()
        {
            try
            {
                UpdateSurfaceArea();

                if (_controller != null)
                {
                    _controller.ClearSelectionHistory();
                    if (Surface.CreatedFrom == FeSurfaceCreatedFrom.Selection)
                    {
                        _controller.Selection.SelectItem = vtkSelectItem.Surface;
                        // Surface.CreationData is set to null when the CreatedFrom is changed
                        if (Surface.CreationData != null) _controller.Selection.CopySelectonData(Surface.CreationData as Selection);  // deep copy to not clear
                    }
                    else if (Surface.CreatedFrom == FeSurfaceCreatedFrom.NodeSet && Surface.CreatedFromNodeSetName != null)
                    {
                        _controller.ClearSelectionHistory();
                        _controller.Selection.SelectItem = vtkSelectItem.Node;
                        if (_controller.GetUserNodeSetNames().Contains(Surface.CreatedFromNodeSetName))
                        {
                            int[] ids = _controller.GetNodeSet(Surface.CreatedFromNodeSetName).Labels;
                            SelectionNodeIds selectionNode = new SelectionNodeIds(vtkSelectOperation.None, false, ids);
                            _controller.AddSelectionNode(selectionNode, true);
                        }
                    }
                    _controller.HighlightSelection();
                }
            }
            catch { }
        }
        private void UpdateSurfaceArea()
        {
            try
            {
                // update area
                if (Surface.CreatedFrom == FeSurfaceCreatedFrom.Selection && Surface.CreationData != null)
                    _controller.UpdateSurfaceArea(Surface);
                else if (Surface.CreatedFrom == FeSurfaceCreatedFrom.NodeSet && Surface.CreatedFromNodeSetName != null
                         && _controller.GetUserNodeSetNames().Contains(Surface.CreatedFromNodeSetName))
                    _controller.UpdateSurfaceArea(Surface);
                else Surface.Area = 0;

                propertyGrid.Refresh();
            }
            catch { }
        }
    }


}
