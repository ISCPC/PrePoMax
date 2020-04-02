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
    class FrmSurface : UserControls.FrmProperties, IFormBase, IFormItemSetDataParent
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
            //
            if (value == FeSurfaceCreatedFrom.Selection.ToString())
            {
                ItemSetDataEditor.SelectionForm.ShowIfHidden(this.Owner);
                HighlightSurface();
            }
            else if (value == FeSurfaceCreatedFrom.NodeSet.ToString())
            {
                ItemSetDataEditor.SelectionForm.Hide();
                HighlightSurface();
            }
            else if (value == _viewSurface.NodeSetName) HighlightSurface();
            //
            base.OnPropertyGridPropertyValueChanged();
        }
        protected override void OnApply(bool onOkAddNew)
        {
            _viewSurface = (ViewFeSurface)propertyGrid.SelectedObject;
            //
            if (_viewSurface == null) throw new CaeGlobals.CaeException("No surface was selected.");
            // Check if the name exists
            if ((_surfaceToEditName == null && _allExistingNames.Contains(Surface.Name)) ||       // named to existing name
                (Surface.Name != _surfaceToEditName && _allExistingNames.Contains(Surface.Name))) // renamed to existing name
                throw new CaeException("The selected name already exists.");
            //
            if (Surface.CreatedFrom == FeSurfaceCreatedFrom.Selection && (Surface.FaceIds == null || Surface.FaceIds.Length == 0))
                throw new CaeException("The surface must contain at least one item.");
            //
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
                        // Replace back the ids by the previous selection
                        if (Surface.CreationData.Nodes[0] is SelectionNodeIds sn && sn.Equals(_selectionNodeIds))
                        {
                            Surface.CreationData.Nodes.RemoveAt(0);
                            Surface.CreationData.Nodes.InsertRange(0, _prevSelectionNodes);
                        }
                    }
                    //
                    Surface.Valid = true;
                    _controller.ReplaceSurfaceCommand(_surfaceToEditName, Surface);
                }
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
        protected override bool OnPrepareForm(string stepName, string surfaceToEditName)
        {
            // To prevent the call to frmMain.itemForm_VisibleChanged when minimized
            this.DialogResult = DialogResult.None;
            this.btnOkAddNew.Visible = surfaceToEditName == null;
            //
            _propertyItemChanged = false;
            _allExistingNames.Clear();
            _surfaceToEditName = null;
            _viewSurface = null;
            propertyGrid.SelectedObject = null;
            _prevSelectionNodes = null;
            _selectionNodeIds = null;
            //
            _controller.SetSelectItemToSurface();
            //
            _allExistingNames.UnionWith(_controller.GetAllMeshEntityNames());
            _surfaceToEditName = surfaceToEditName;
            //
            string[] nodeSetNames = _controller.GetUserNodeSetNames();
            // Create new surface
            if (_surfaceToEditName == null)
            {
                Surface = new FeSurface(GetSurfaceName());
                _controller.Selection.Clear();                
            }
            // Edit existing surface
            else
            {
                Surface = _controller.GetSurface(_surfaceToEditName);   // to clone
                //
                if (Surface.FaceIds != null)
                {
                    // Change node selection history to ids to speed up
                    int[] ids = Surface.FaceIds;
                    _selectionNodeIds = new SelectionNodeIds(vtkSelectOperation.None, false, ids);
                    _prevSelectionNodes = Surface.CreationData.Nodes;
                    _controller.CreateNewSelection(Surface.CreationData.CurrentView, _selectionNodeIds, true);
                    Surface.CreationData = _controller.Selection.DeepClone();
                }
                //
                if (Surface.CreatedFrom == FeSurfaceCreatedFrom.Selection) { }
                else if (Surface.CreatedFrom == FeSurfaceCreatedFrom.NodeSet)
                    CheckMissingValueRef(ref nodeSetNames, _viewSurface.NodeSetName, s => { _viewSurface.NodeSetName = s; });
                else throw new NotSupportedException();
            }
            //
            _viewSurface.PopululateDropDownList(nodeSetNames);
            //
            propertyGrid.SelectedObject = _viewSurface;
            propertyGrid.Select();
            // Show ItemSetDataForm
            if (Surface.CreatedFrom == FeSurfaceCreatedFrom.Selection)
            {
                ItemSetDataEditor.SelectionForm.ItemSetData = new ItemSetData(Surface.FaceIds);
                ItemSetDataEditor.SelectionForm.ShowIfHidden(this.Owner);
            }
            //
            HighlightSurface();
            //
            return true;
        }


        // Methods                                                                                                                  
        private string GetSurfaceName()
        {
            return NamedClass.GetNewValueName(_allExistingNames, "Surface-");
        }
        private void HighlightSurface()
        {
            try
            {
                UpdateSurfaceArea();
                //
                if (_controller != null)
                {
                    _controller.ClearSelectionHistory();
                    //
                    if (Surface.CreatedFrom == FeSurfaceCreatedFrom.Selection)
                    {
                        // Surface.CreationData is set to null when the CreatedFrom is changed
                        if (Surface.CreationData != null)
                            _controller.Selection.CopySelectonData(Surface.CreationData); // deep copy to not clear
                    }
                    else if (Surface.CreatedFrom == FeSurfaceCreatedFrom.NodeSet && Surface.CreatedFromNodeSetName != null)
                    {
                        _controller.SetSelectItemToNode();
                        //
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
            finally 
            {
                _controller.SetSelectItemToSurface();
            }
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
                //
                propertyGrid.Refresh();
            }
            catch { }
        }
        //
        public void SelectionChanged(int[] ids)
        {
            if (Surface.CreatedFrom == FeSurfaceCreatedFrom.Selection)
            {
                Surface.FaceIds = ids;
                Surface.CreationData = _controller.Selection.DeepClone();
                UpdateSurfaceArea();
                //
                propertyGrid.Refresh();
                //
                _propertyItemChanged = true;
            }
        }


        // IFormItemSetDataParent
        public bool IsSelectionGeometryBased()
        {
            // prepare ItemSetDataEditor
            if (_surfaceToEditName == null) return true;
            FeSurface surface = _controller.GetSurface(_surfaceToEditName);
            if (surface == null || surface.CreationData == null) return true;
            return surface.CreationData.IsGeometryBased();
        }
    }


}
