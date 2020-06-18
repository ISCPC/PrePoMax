using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using CaeGlobals;
using CaeModel;
using CaeMesh;
using CaeJob;
using CaeResults;

namespace UserControls
{
    public enum ViewType
    {
        Geometry,
        Model,
        Results
    }
    public struct ContextMenuFields
    {
        public int Create;
        public int Edit;
        public int Duplicate;
        public int Hide;
        public int Show;
        public int Transparency;
        public int Deformed;
        public int ColorContours;
        public int CompoundPart;
        public int MeshingParameters;
        public int PreviewEdgeMesh;
        public int CreateMesh;
        public int CopyPartToGeometry;
        public int EditCalculixKeywords;
        public int MergePart;
        public int ConvertToPart;
        public int MaterialLibrary;
        public int Run;
        public int Monitor;
        public int Results;
        public int Kill;
        public int Activate;
        public int Deactivate;
        public int Expand;
        public int Colapse;
        public int Delete;
    }
    public enum HideShowOperation
    {
        Hide,
        Show,
        ShowOnly
    }


    public partial class ModelTree : UserControl
    {
        // Variables                                                                                                                
        private bool _screenUpdating;
        private bool _doubleClick;
        private int _numUserKeywords;
        private const int WM_MOUSEMOVE = 0x0200;
        private bool _disableMouse;
        private bool _disableSelectionsChanged;
        // Geometry
        private TreeNode _geomParts;
        private TreeNode _meshRefinements;
        // Model
        private TreeNode _model;
        private TreeNode _meshParts;
        private TreeNode _resultParts;
        private TreeNode _nodeSets;
        private TreeNode _elementSets;
        private TreeNode _surfaces;
        private TreeNode _referencePoints;
        private TreeNode _materials;
        private TreeNode _sections;
        private TreeNode _constraints;
        private TreeNode _surfaceInteractions;
        private TreeNode _contactPairs;
        private TreeNode _steps;
        private TreeNode _analyses;
        // Results
        private TreeNode _resultFieldOutputs;
        private TreeNode _resultHistoryOutputs;
        //
        private string _geomPartsName;
        private string _meshRefinementsName;
        //
        private string _modelName;
        private string _meshPartsName;
        private string _resultPartsName;
        private string _nodeSetsName;
        private string _elementSetsName;
        private string _surfacesName;
        private string _referencePointsName;
        private string _materialsName;
        private string _sectionsName;
        private string _constraintsName;
        private string _surfaceInteractionsName;
        private string _contactPairsName;
        private string _stepsName;
        private string _historyOutputsName;
        private string _fieldOutputsName;
        private string _boundaryConditionsName;
        private string _loadsName;
        private string _analysesName;
        
        
        // Properties                                                                                                               
        public bool ScreenUpdating 
        { 
            get { return _screenUpdating; } 
            set
            {
                _screenUpdating = value;
            }
        }
        public bool DisableMouse
        {
            get { return _disableMouse; }
            set
            {
                _disableMouse = value;
                cltvGeometry.DisableMouse = value;
                cltvModel.DisableMouse = value;
                cltvResults.DisableMouse = value;
            }
        }
        public string[] IntersectSelectionWithList(NamedClass[] list)
        {
            List<string> selected = new List<string>();

            CodersLabTreeView tree = GetActiveTree();
            foreach (TreeNode node in tree.SelectedNodes)
            {
                if (node.Tag != null && list.Contains(node.Tag))
                    selected.Add(node.Text);
            }

            return selected.ToArray();
        }


        // Events                                                                                                                   
        public event Action<ViewType> GeometryMeshResultsEvent;
        public event Action<NamedClass[]> SelectEvent;
        public event Action ClearSelectionEvent;

        public event Action<string, string> CreateEvent;
        public event Action<NamedClass[], string[]> DuplicateEvent;
        public event Action<NamedClass, string> EditEvent;
        public event Action<NamedClass[], HideShowOperation, string[]> HideShowEvent;
        public event Action<string[]> SetTransparencyEvent;
        public event Action<NamedClass[], bool> ColorContoursVisibilityEvent;
        public event Action<string[]> CreateCompoundPart;
        public event Action<string[]> MeshingParametersEvent;
        public event Action<string[]> PreviewEdgeMesh;
        public event Action<string[]> CreateMeshEvent;
        public event Action<string[]> CopyGeometryToResultsEvent;
        public event Action EditCalculixKeywords;
        public event Action<string[]> MergeParts;
        public event Action<string[]> ConvertElementSetsToMeshParts;
        public event Action MaterialLibrary;
        public event Action<string> RunEvent;
        public event Action<string> MonitorEvent;
        public event Action<string> ResultsEvent;
        public event Action<string> KillEvent;
        public event Action<NamedClass[], bool, string[]> ActivateDeactivateEvent;
        public event Action<NamedClass[], string[]> DeleteEvent;

        public event Action<string[]> FieldDataSelectEvent;
        public event Action RenderingOn;
        public event Action RenderingOff;



        // Constructors                                                                                                             
        public ModelTree()
        {
            InitializeComponent();

            _geomPartsName = "Parts";
            _meshRefinementsName = "Mesh refinements";
            //
            _modelName = "Model";
            _meshPartsName = "Parts";
            _resultPartsName = "Parts";
            _nodeSetsName = "Node sets";
            _elementSetsName = "Element sets";
            _surfacesName = "Surfaces";
            _referencePointsName = "Reference points";
            _materialsName = "Materials";
            _sectionsName = "Sections";
            _constraintsName = "Constraints";
            _surfaceInteractionsName = "Surface interactions";
            _contactPairsName = "Contact pairs";
            _stepsName = "Steps";
            _boundaryConditionsName = "BCs";
            _loadsName = "Loads";
            _analysesName = "Analyses";
            // Results
            _fieldOutputsName = "Field outputs";
            _historyOutputsName = "History outputs";
            //
            _geomParts = cltvGeometry.Nodes.Find(_geomPartsName, true)[0];
            _meshRefinements = cltvGeometry.Nodes.Find(_meshRefinementsName, true)[0];
            //
            _model = cltvModel.Nodes.Find(_modelName, true)[0];
            _meshParts = cltvModel.Nodes.Find(_meshPartsName, true)[0];
            _resultParts = cltvResults.Nodes.Find(_resultPartsName, true)[0];
            _nodeSets = cltvModel.Nodes.Find(_nodeSetsName, true)[0];
            _elementSets = cltvModel.Nodes.Find(_elementSetsName, true)[0];
            _surfaces = cltvModel.Nodes.Find(_surfacesName, true)[0];
            _referencePoints = cltvModel.Nodes.Find(_referencePointsName, true)[0];
            _materials = cltvModel.Nodes.Find(_materialsName, true)[0];
            _sections = cltvModel.Nodes.Find(_sectionsName, true)[0];
            _constraints = cltvModel.Nodes.Find(_constraintsName, true)[0];
            _surfaceInteractions = cltvModel.Nodes.Find(_surfaceInteractionsName, true)[0];
            _contactPairs = cltvModel.Nodes.Find(_contactPairsName, true)[0];
            _steps = cltvModel.Nodes.Find(_stepsName, true)[0];
            _analyses = cltvModel.Nodes.Find(_analysesName, true)[0];
            _resultFieldOutputs = cltvResults.Nodes.Find(_fieldOutputsName, true)[0];
            _resultHistoryOutputs = cltvResults.Nodes.Find(_historyOutputsName, true)[0];

            // add NamedClasses to static items
            _model.Tag = new EmptyNamedClass(typeof(CaeModel.FeModel).ToString());
            // Geometry icons
            _geomParts.StateImageKey = "GeomPart";
            // Model icons
            cltvModel.Nodes.Find("Mesh", true)[0].StateImageKey = "Mesh";
            _meshParts.StateImageKey = "BasePart";
            _nodeSets.StateImageKey = "Node_set";
            _elementSets.StateImageKey = "Element_set";
            _surfaces.StateImageKey = "Surface";
            _referencePoints.StateImageKey = "Reference_point";
            _materials.StateImageKey = "Material";
            _sections.StateImageKey = "Section";
            _constraints.StateImageKey = "Constraints";
            _surfaceInteractions.StateImageKey = "SurfaceInteractions";
            _contactPairs.StateImageKey = "ContactPairs";
            _steps.StateImageKey = "Step";
            _analyses.StateImageKey = "Bc";
            // Results icons
            cltvResults.Nodes.Find("Mesh", true)[0].StateImageKey = "Mesh";
            _resultParts.StateImageKey = "BasePart";
            _resultFieldOutputs.StateImageKey = "Field_output";
            _resultHistoryOutputs.StateImageKey = "History_output";

            _doubleClick = false;
            _screenUpdating = true;

            cltvGeometry.Nodes[0].ExpandAll();
            cltvModel.Nodes[0].ExpandAll();
            cltvResults.Nodes[0].ExpandAll();
            cltvResults.Nodes[1].ExpandAll();

            //cltvModel.SelectionBackColor = Color.White;
            //cltvModel.ForeColor = Color.Black;
        }


        // Event hadlers                                                                                                            
        private void tcGeometryModelResults_Deselecting(object sender, TabControlCancelEventArgs e)
        {
            e.Cancel = _disableMouse;
        }

        #region Geometry-Model-Results
        private ViewType GetViewType()
        {
            if (tcGeometryModelResults.SelectedTab == tpGeometry) return ViewType.Geometry;
            else if (tcGeometryModelResults.SelectedTab == tpModel) return ViewType.Model;
            else if (tcGeometryModelResults.SelectedTab == tpResults) return ViewType.Results;
            else throw new NotSupportedException();
        }
        private void tcGeometryModelResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            GeometryMeshResultsEvent?.Invoke(GetViewType());
        }
        public void SetGeometryTab()
        {
            tcGeometryModelResults.SelectedIndexChanged -= tcGeometryModelResults_SelectedIndexChanged;
            bool prevMouseState = _disableMouse;
            _disableMouse = false;
            tcGeometryModelResults.SelectedTab = tpGeometry;
            _disableMouse = prevMouseState;
            tcGeometryModelResults.SelectedIndexChanged += tcGeometryModelResults_SelectedIndexChanged;
        }
        public void SetModelTab()
        {
            tcGeometryModelResults.SelectedIndexChanged -= tcGeometryModelResults_SelectedIndexChanged;
            bool prevMouseState = _disableMouse;
            _disableMouse = false;
            tcGeometryModelResults.SelectedTab = tpModel;
            _disableMouse = prevMouseState;
            tcGeometryModelResults.SelectedIndexChanged += tcGeometryModelResults_SelectedIndexChanged;
        }
        public void SetResultsTab()
        {
            tcGeometryModelResults.SelectedIndexChanged -= tcGeometryModelResults_SelectedIndexChanged;
            bool prevMouseState = _disableMouse;
            _disableMouse = false;
            tcGeometryModelResults.SelectedTab = tpResults;
            _disableMouse = prevMouseState;
            tcGeometryModelResults.SelectedIndexChanged += tcGeometryModelResults_SelectedIndexChanged;
        }
        #endregion

        #region Tree event handlers
        private void PrepareToolStripItem(CodersLabTreeView tree)
        {
            int n = tree.SelectedNodes.Count;
            //
            ContextMenuFields menuFields = new ContextMenuFields();
            foreach (TreeNode node in tree.SelectedNodes)
            {
                AppendMenuFields(node, ref menuFields);
            }
            bool oneAboveVisible = false;
            // Visibility                                                                                           
            bool visible;
            // Create
            visible = menuFields.Create == n;
            tsmiCreate.Visible = visible;
            oneAboveVisible |= visible;
            // Edit
            visible = menuFields.Edit == n;
            tsmiEdit.Visible = visible;
            oneAboveVisible |= visible;
            // Duplicate
            visible = menuFields.Duplicate == n;
            tsmiDuplicate.Visible = visible;
            oneAboveVisible |= visible;
            //Geometry                                              
            visible = menuFields.CompoundPart == n && n > 1;
            tsmiSpaceCompoundPart.Visible = false;
            tsmiCompoundPart.Visible = visible;
            oneAboveVisible |= visible;
            //Mesh                                                  
            visible = menuFields.MeshingParameters == n;
            tsmiSpaceMesh.Visible = visible && oneAboveVisible;
            tsmiMeshingParameters.Visible = visible;
            tsmiPreviewEdgeMesh.Visible = visible;
            tsmiCreateMesh.Visible = visible;
            // Copy part                                            
            tsmiSpaceCopyPart.Visible = visible;
            tsmiCopyGeometryToResults.Visible = visible;
            oneAboveVisible |= visible;
            // Edit Calculix Keywords                               
            visible = menuFields.EditCalculixKeywords == n && n > 0;
            tsmiSpaceEditCalculiXKeywords.Visible = visible && oneAboveVisible;
            tsmiEditCalculiXKeywords.Visible = visible;
            oneAboveVisible |= visible;
            // Merge mesh parts                                     
            visible = menuFields.MergePart == n && n > 1;
            tsmiSpaceMergeParts.Visible = false;
            tsmiMergeParts.Visible = visible;
            oneAboveVisible |= visible;
            // Convert element set                                  
            visible = menuFields.ConvertToPart == n;
            //tsmiSpaceConvertToPart.Visible = visible;
            tsmiSpaceConvertToPart.Visible = false;
            tsmiConvertToPart.Visible = visible;
            oneAboveVisible |= visible;
            // Hide/Show/                                           
            visible = menuFields.Hide + menuFields.Show == n;
            tsmiSpaceHideShow.Visible = visible && oneAboveVisible;
            tsmiHide.Visible = visible;
            tsmiShow.Visible = visible;
            tsmiShowOnly.Visible = visible;
            // Transparency                                         
            tsmiSetTransparency.Visible = menuFields.Transparency == n;
            oneAboveVisible |= visible; // Hide/Show
            // Deformed/Color contours                              
            visible = menuFields.Deformed == n;
            tsmiSpaceColorContours.Visible = visible && oneAboveVisible;
            tsmiColorContoursOff.Visible = visible;
            tsmiColorContoursOn.Visible = visible;
            oneAboveVisible |= visible;
            // Material library                                     
            visible = menuFields.MaterialLibrary == n && n > 0;
            tsmiSpaceMaterialLibrary.Visible = visible && oneAboveVisible;
            tsmiMaterialLibrary.Visible = visible;
            oneAboveVisible |= visible;
            // Analysis                                             
            visible = menuFields.Run == n;
            tsmiSpaceAnalysis.Visible = visible && oneAboveVisible;
            tsmiRun.Visible = visible;
            tsmiMonitor.Visible = visible;
            tsmiResults.Visible = visible;
            tsmiKill.Visible = visible;
            oneAboveVisible |= visible;
            // Activate/Deactivate                                  
            visible = menuFields.Activate + menuFields.Deactivate == n;
            tsmiSpaceActive.Visible = visible && oneAboveVisible;
            tsmiActivate.Visible = visible;
            tsmiDeactivate.Visible = visible;
            oneAboveVisible |= visible;
            //Expand / Collapse                                     
            tsmiSpaceExpandColapse.Visible = oneAboveVisible;
            tsmiExpandAll.Visible = true;
            tsmiCollapseAll.Visible = true;
            oneAboveVisible = true;
            // Delete                                               
            visible = menuFields.Delete == n;
            tsmiSpaceDelete.Visible = visible && oneAboveVisible;
            tsmiDelete.Visible = visible;
            oneAboveVisible |= visible;

            // Enabled                                                                                              
            bool enabled;
            // Create
            enabled = menuFields.Create == 1;
            tsmiCreate.Enabled = enabled;
            // Edit
            enabled = menuFields.Edit == 1;
            tsmiEdit.Enabled = enabled;
            // Hide/Show
            tsmiHide.Enabled = true;
            tsmiShow.Enabled = true;
            tsmiShowOnly.Enabled = true;
            // Deformed/Color contours
            tsmiColorContoursOff.Enabled = true;
            tsmiColorContoursOn.Enabled = true;
            // Mesh
            tsmiMeshingParameters.Enabled = true;
            tsmiCreateMesh.Enabled = true;
            // Copy part
            tsmiCopyGeometryToResults.Enabled = true;
            // Material library
            tsmiMaterialLibrary.Enabled = true;
            // Analysis
            enabled = menuFields.Run == 1;
            tsmiRun.Enabled = enabled;
            tsmiMonitor.Enabled = enabled;
            tsmiResults.Enabled = enabled;
            tsmiKill.Enabled = enabled;
            // Activate/Deactivate 
            tsmiActivate.Enabled = true;
            tsmiDeactivate.Enabled = true;
            // Delete
            tsmiDelete.Enabled = true;
        }
        //
        private void AppendMenuFields(TreeNode node, ref ContextMenuFields menuFields)
        {
            // Check if selected node is Model
            // Edit Calculix Keywords
            if (node == _model)
            {
                menuFields.Edit++;
                menuFields.EditCalculixKeywords++;
                return;
            }
            //
            NamedClass item = (NamedClass)node.Tag;
            bool subPart = node.Parent != null && node.Parent.Tag is CompoundGeometryPart;
            // Create
            if (CanCreate(node)) menuFields.Create++;
            // Edit
            if (item != null && item.Visible && item.Active) menuFields.Edit++;
            //Duplicate
            if (item != null && CanDuplicate(node)) menuFields.Duplicate++;
            // Hide/Show
            if (item != null && CanHide(item))
            {
                if (item.Visible) menuFields.Hide++;
                else menuFields.Show++;
            }
            //Transparency
            if (item != null && item is BasePart)
            {
                menuFields.Transparency++;
            }
            // Deformed/Color contours
            if (item != null && item is ResultPart)
            {
                if (!ResultPart.Undeformed)
                {
                    menuFields.Deformed++;
                    menuFields.ColorContours++;
                }
            }
            // Geometry part - Geometry
            if (item != null && item is GeometryPart && GetActiveTree() == cltvGeometry)
            {
                menuFields.CompoundPart++;
            }
            // Geometry part - Mesh
            if (item != null && item is GeometryPart && GetActiveTree() == cltvGeometry)
            {
                if (!subPart)
                {
                    menuFields.MeshingParameters++;
                    menuFields.PreviewEdgeMesh++;
                    menuFields.CreateMesh++;
                }
                menuFields.CopyPartToGeometry++;
            }
            // Merge mesh parts
            if (item != null && item is MeshPart && GetActiveTree() == cltvModel)
            {
                menuFields.MergePart++;
            }
            // Convert element set to part
            if (item != null && item is FeElementSet && GetActiveTree() == cltvModel)
            {
                menuFields.ConvertToPart++;
            }
            // Material library
            if (node == _materials)
            {
                menuFields.MaterialLibrary++;
            }
            // Analysis
            if (node.Parent == _analyses)
            {
                menuFields.Run++;
                menuFields.Monitor++;
                menuFields.Results++;
                menuFields.Kill++;
            }
            // Activate/Deactivate 
            if (item != null && CanDeactivate(node))
            {
                if (item.Active) menuFields.Deactivate++;
                else menuFields.Activate++;
            }
            // Delete
            if (item != null && !subPart)
            {
                menuFields.Delete++;
            }
        }
        //
        private void cltv_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                CodersLabTreeView tree = (CodersLabTreeView)sender;
                //
                if (ModifierKeys != Keys.Shift && ModifierKeys != Keys.Control && e.Clicks > 1) _doubleClick = true;
                else _doubleClick = false;
                //
                TreeNode node = tree.GetNodeAt(e.Location);
                if (node == null)
                {
                    tree.SelectedNodes.Clear();
                    ClearSelectionEvent();
                }
                //else
                //{
                //    if (e.Button == MouseButtons.Left)
                //    {
                //        if (!_doubleClick && tree.SelectedNode != null && node != null && tree.SelectedNode == node)
                //        {
                //            UpdateHighlight();
                //        }
                //    }
                //}
            }
            catch
            { }
        }
        private void cltv_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                CodersLabTreeView tree = (CodersLabTreeView)sender;

                if (e.Button == MouseButtons.Right)
                {
                    if (tree.SelectedNodes.Count > 0)
                    {
                        PrepareToolStripItem(tree);
                        cmsTree.Show(tree, e.Location);
                    }
                }
            }
            catch
            { }
        }
        private void cltv_SelectionsChanged(object sender, EventArgs e)
        {
            if (_disableSelectionsChanged) return;

            // this function is also called with sender as null parameter
            CodersLabTreeView tree = GetActiveTree();

            TreeNode node;
            List<NamedClass> items = new List<NamedClass>();

            if (!_doubleClick && tree.SelectedNodes.Count > 0)
            {
                // Select field data
                if (tree.SelectedNodes.Count == 1)
                {
                    node = tree.SelectedNodes[0];

                    if (node.Tag is FieldData)          // Results
                    {
                        SelectEvent?.Invoke(null);      // clear selection
                        FieldDataSelectEvent?.Invoke(new string[] { node.Parent.Name, node.Name });
                        ActiveControl = cltvResults;    // this is for the arrow keys to work on the results tree
                        return;
                    }
                }

                // Select
                foreach (TreeNode selectedNode in tree.SelectedNodes)
                {
                    if (selectedNode.Tag == null) continue;

                    items.Add((NamedClass)selectedNode.Tag);
                }
                SelectEvent?.Invoke(items.ToArray());
            }
            else if (tree.SelectedNodes.Count == 0) ClearSelectionEvent();

            return;
        }
        private void cltv_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            CodersLabTreeView tree = (CodersLabTreeView)sender;

            if (tree.SelectedNode == null || ModifierKeys == Keys.Shift || ModifierKeys == Keys.Control) return;

            if (tree.SelectedNode == tree.HitTest(e.Location).Node)
            {
                if (tree.SelectedNode.Tag == null) tsmiCreate_Click(null, null);
                else
                {
                    if ((tree.SelectedNode.Tag as NamedClass).Visible) tsmiEdit_Click(null, null);
                }
            }
            _doubleClick = false;
        }
        private void cltv_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (_doubleClick == true && e.Action == TreeViewAction.Expand)
            {
                e.Cancel = true;
            }
        }
        private void cltv_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (_doubleClick == true && e.Action == TreeViewAction.Collapse)
            {
                e.Cancel = true;
            }
        }
        private void cltv_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            CodersLabTreeView tree = (CodersLabTreeView)sender;
            foreach (TreeNode node in tree.Nodes) SetAllNodesStatusIcons(node);
        }
        private void cltv_AfterExpand(object sender, TreeViewEventArgs e)
        {
            CodersLabTreeView tree = (CodersLabTreeView)sender;
            foreach (TreeNode node in tree.Nodes) SetAllNodesStatusIcons(node);
            
        }
        private void cltv_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                tsmiDelete_Click(null, null);
            }
            else if (e.KeyCode == Keys.Space)
            {
                tsmiInvertHideShow_Click(null, null);
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                tsmiEdit_Click(null, null);
            }
        }

        #endregion

        #region Tree context menu
        private void tsmiCreate_Click(object sender, EventArgs e)
        {
            try
            {
                CodersLabTreeView tree = GetActiveTree();
                if (tree.SelectedNodes.Count != 1) return;

                TreeNode selectedNode = tree.SelectedNodes[0];

                if (selectedNode.Tag == null)
                {
                    if (CanCreate(selectedNode))
                    {
                        string stepName = null;
                        if (selectedNode.Parent != null && selectedNode.Parent.Tag is Step)
                            stepName = selectedNode.Parent.Text;

                        CreateEvent?.Invoke(selectedNode.Name, stepName);
                    }
                }
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDuplicate_Click(object sender, EventArgs e)
        {
            try
            {
                List<NamedClass> items = new List<NamedClass>();
                string stepName;
                List<string> stepNames = new List<string>();
                //
                foreach (TreeNode selectedNode in GetActiveTree().SelectedNodes)
                {
                    if (selectedNode.Tag == null) continue;
                    //
                    items.Add((NamedClass)selectedNode.Tag);
                    stepName = null;
                    if (selectedNode.Parent != null && selectedNode.Parent.Parent != null && selectedNode.Parent.Parent.Tag is Step)
                        stepName = selectedNode.Parent.Parent.Text;
                    stepNames.Add(stepName);
                }
                //
                if (items.Count > 0)
                {
                    RenderingOff?.Invoke();
                    DuplicateEvent?.Invoke(items.ToArray(), stepNames.ToArray());
                    RenderingOn?.Invoke();
                }
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEdit_Click(object sender, EventArgs e)
        {
            try
            {
                CodersLabTreeView tree = GetActiveTree();
                if (tree.SelectedNodes.Count != 1) return;
                //
                TreeNode selectedNode = tree.SelectedNodes[0];
                //
                if (selectedNode.Tag == null) return;
                //
                string stepName = null;
                if (selectedNode.Parent != null && selectedNode.Parent.Parent != null && selectedNode.Parent.Parent.Tag is Step)
                    stepName = selectedNode.Parent.Parent.Text;
                //
                EditEvent?.Invoke((NamedClass)selectedNode.Tag, stepName);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        // Visibility
        private void tsmiHideShow_Click(object sender, EventArgs e)
        {
            try
            {
                List<NamedClass> items = new List<NamedClass>();
                string stepName;
                List<string> stepNames = new List<string>();


                foreach (TreeNode selectedNode in GetActiveTree().SelectedNodes)
                {
                    if (selectedNode.Tag == null) continue;

                    if (CanHide(selectedNode.Tag))
                    {
                        items.Add((NamedClass)selectedNode.Tag);
                        stepName = null;
                        if (selectedNode.Parent != null && selectedNode.Parent.Parent != null && selectedNode.Parent.Parent.Tag is Step)
                            stepName = selectedNode.Parent.Parent.Text;
                        stepNames.Add(stepName);
                    }
                }

                if (items.Count > 0)
                {
                    RenderingOff?.Invoke();
                    HideShowOperation operation;
                    if (sender == tsmiHide) operation = HideShowOperation.Hide;
                    else if (sender == tsmiShow) operation = HideShowOperation.Show;
                    else if (sender == tsmiShowOnly) operation = HideShowOperation.ShowOnly;
                    else throw new NotSupportedException();

                    HideShowEvent?.Invoke(items.ToArray(), operation, stepNames.ToArray());
                    RenderingOn?.Invoke();
                }
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiInvertHideShow_Click(object sender, EventArgs e)
        {
            try
            {
                List<NamedClass> items = new List<NamedClass>();
                string stepName;
                List<string> stepNames = new List<string>();

                foreach (TreeNode selectedNode in GetActiveTree().SelectedNodes)
                {
                    if (selectedNode.Tag == null) continue;

                    if (CanHide(selectedNode.Tag))
                    {
                        items.Add((NamedClass)selectedNode.Tag);
                        stepName = null;
                        if (selectedNode.Parent != null && selectedNode.Parent.Parent != null && selectedNode.Parent.Parent.Tag is Step)
                            stepName = selectedNode.Parent.Parent.Text;
                        stepNames.Add(stepName);
                    }
                }

                if (items.Count > 0)
                {
                    RenderingOff?.Invoke();
                    List<NamedClass> toShow = new List<NamedClass>();
                    List<NamedClass> toHide = new List<NamedClass>();

                    foreach (var item in items)
                    {
                        if (item.Visible) toHide.Add(item);
                        else toShow.Add(item);
                    }

                    if (toShow.Count > 0) HideShowEvent?.Invoke(toShow.ToArray(), HideShowOperation.Show, stepNames.ToArray());
                    if (toHide.Count > 0) HideShowEvent?.Invoke(toHide.ToArray(), HideShowOperation.Hide, stepNames.ToArray());
                    RenderingOn?.Invoke();
                }
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiSetTransparency_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> parts = new List<string>();

                foreach (TreeNode selectedNode in GetActiveTree().SelectedNodes)
                {
                    if (selectedNode.Tag == null) continue;
                    if (selectedNode.Tag is BasePart part) parts.Add(part.Name);
                }

                if (parts.Count > 0) SetTransparencyEvent?.Invoke(parts.ToArray());
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiResultColorContoutsVisibility_Click(object sender, EventArgs e)
        {
            try
            {
                List<NamedClass> items = new List<NamedClass>();

                foreach (TreeNode selectedNode in GetActiveTree().SelectedNodes)
                {
                    if (selectedNode.Tag == null) continue;

                    if (selectedNode.Tag is ResultPart)
                    {
                        items.Add((NamedClass)selectedNode.Tag);
                    }
                }

                if (items.Count > 0)
                {
                    bool colorContours = (sender == tsmiColorContoursOn);
                    ColorContoursVisibilityEvent?.Invoke(items.ToArray(), colorContours);
                }
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        // Geometry - compound
        private void tsmiCompoundPart_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> names = new List<string>();
                foreach (TreeNode node in cltvGeometry.SelectedNodes)
                {
                    if (node.Tag != null) names.Add(((NamedClass)node.Tag).Name);
                }
                if (names.Count > 0) CreateCompoundPart?.Invoke(names.ToArray());
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        // Meshing
        private void tsmiMeshingParameters_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> names = new List<string>();
                foreach (TreeNode node in cltvGeometry.SelectedNodes)
                {
                    if (node.Tag != null) names.Add(((NamedClass)node.Tag).Name);
                }
                if (names.Count > 0) MeshingParametersEvent?.Invoke(names.ToArray());
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiPreviewEdgeMesh_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> names = new List<string>();
                foreach (TreeNode node in cltvGeometry.SelectedNodes)
                {
                    if (node.Tag != null) names.Add(((NamedClass)node.Tag).Name);
                }
                if (names.Count > 0) PreviewEdgeMesh?.Invoke(names.ToArray());
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }            
        }
        private void tsmiCreateMesh_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> names = new List<string>();
                foreach (TreeNode node in cltvGeometry.SelectedNodes)
                {
                    if (node.Tag != null) names.Add(((NamedClass)node.Tag).Name);
                }
                if (names.Count > 0) CreateMeshEvent?.Invoke(names.ToArray());
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        // Geometry - copy
        private void tsmiCopyGeometryPartToResults_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> names = new List<string>();
                foreach (TreeNode node in cltvGeometry.SelectedNodes)
                {
                    if (node.Tag != null) names.Add(((NamedClass)node.Tag).Name);
                }
                if (names.Count > 0) CopyGeometryToResultsEvent?.Invoke(names.ToArray());
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        // Model
        private void tsmiEditCalculiXKeywords_Click(object sender, EventArgs e)
        {
            try
            {
                CodersLabTreeView tree = GetActiveTree();
                if (tree.SelectedNodes.Count != 1) return;
                //
                TreeNode selectedNode = tree.SelectedNodes[0];
                if (selectedNode != _model) return;
                //
                EditCalculixKeywords?.Invoke();
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiMergeParts_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> names = new List<string>();
                foreach (TreeNode node in cltvModel.SelectedNodes)
                {
                    if (node.Tag != null) names.Add(((NamedClass)node.Tag).Name);
                }
                if (names.Count > 0) MergeParts?.Invoke(names.ToArray());
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiConvertToPart_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> names = new List<string>();
                foreach (TreeNode node in cltvModel.SelectedNodes)
                {
                    if (node.Tag != null) names.Add(((NamedClass)node.Tag).Name);
                }
                if (names.Count > 0) ConvertElementSetsToMeshParts?.Invoke(names.ToArray());
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        // Material
        private void tsmiMaterialLibrary_Click(object sender, EventArgs e)
        {
            try
            {
                CodersLabTreeView tree = GetActiveTree();
                if (tree.SelectedNodes.Count != 1) return;
                //
                TreeNode selectedNode = tree.SelectedNodes[0];
                if (selectedNode.Tag != null) return;
                //
                MaterialLibrary?.Invoke();
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        // Analysis
        private void tsmiRun_Click(object sender, EventArgs e)
        {
            try
            {
                CodersLabTreeView tree = GetActiveTree();
                if (tree.SelectedNodes.Count != 1) return;

                TreeNode selectedNode = tree.SelectedNodes[0];

                if (selectedNode.Tag == null) return;

                RunEvent?.Invoke(selectedNode.Name);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiMonitor_Click(object sender, EventArgs e)
        {
            try
            {
                CodersLabTreeView tree = GetActiveTree();
                if (tree.SelectedNodes.Count != 1) return;

                TreeNode selectedNode = tree.SelectedNodes[0];

                if (selectedNode.Tag == null) return;

                MonitorEvent?.Invoke(selectedNode.Name);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiResults_Click(object sender, EventArgs e)
        {
            try
            {
                CodersLabTreeView tree = GetActiveTree();
                if (tree.SelectedNodes.Count != 1) return;

                TreeNode selectedNode = tree.SelectedNodes[0];

                if (selectedNode.Tag == null) return;

                ResultsEvent?.Invoke(selectedNode.Name);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiKill_Click(object sender, EventArgs e)
        {
            try
            {
                CodersLabTreeView tree = GetActiveTree();
                if (tree.SelectedNodes.Count != 1) return;

                TreeNode selectedNode = tree.SelectedNodes[0];

                if (selectedNode.Tag == null) return;

                KillEvent?.Invoke(selectedNode.Name);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        // Activate/Deactivate
        private void ActivateDeactivate_Click(object sender, EventArgs e)
        {
            try
            {
                List<NamedClass> items = new List<NamedClass>();
                string stepName;
                List<string> stepNames = new List<string>();
                CodersLabTreeView tree = GetActiveTree();
                //
                foreach (TreeNode selectedNode in GetActiveTree().SelectedNodes)
                {
                    if (selectedNode.Tag == null) continue;

                    if (CanDeactivate(selectedNode))
                    {
                        items.Add((NamedClass)selectedNode.Tag);
                        stepName = null;
                        if (tree.SelectedNode.Parent != null && tree.SelectedNode.Parent.Parent != null && tree.SelectedNode.Parent.Parent.Tag is Step)
                            stepName = tree.SelectedNode.Parent.Parent.Text;
                        stepNames.Add(stepName);
                    }
                }

                RenderingOff?.Invoke();
                bool activate = (sender == tsmiActivate);
                ActivateDeactivateEvent?.Invoke(items.ToArray(), activate, stepNames.ToArray());
                RenderingOn?.Invoke();
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        // Expand/Collapse
        private void tsmiExpandAll_Click(object sender, EventArgs e)
        {
            foreach (TreeNode selectedNode in GetActiveTree().SelectedNodes)
            {
                selectedNode.ExpandAll();
            }
        }
        private void tsmiCollapseAll_Click(object sender, EventArgs e)
        {
            foreach (TreeNode selectedNode in GetActiveTree().SelectedNodes)
            {
                selectedNode.Collapse();
            }
        }
        // Delete
        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            try
            {
                List<NamedClass> items = new List<NamedClass>();
                string stepName;
                List<string> stepNames = new List<string>();
                //
                foreach (TreeNode selectedNode in GetActiveTree().SelectedNodes)
                {
                    if (selectedNode.Tag == null) continue;
                    //
                    items.Add((NamedClass)selectedNode.Tag);
                    stepName = null;
                    if (selectedNode.Parent != null && selectedNode.Parent.Parent != null && selectedNode.Parent.Parent.Tag is Step)
                        stepName = selectedNode.Parent.Parent.Text;
                    stepNames.Add(stepName);
                }
                //
                if (items.Count > 0)
                {
                    RenderingOff?.Invoke();
                    DeleteEvent?.Invoke(items.ToArray(), stepNames.ToArray());
                    RenderingOn?.Invoke();
                }
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        #endregion


        // Methods                                                                                                                  
        protected override void WndProc(ref Message m)
        {
            //
            // 0200        512     WM_MOUSEFIRST
            // 0200        512     WM_MOUSEMOVE
            // 0201        513     WM_LBUTTONDOWN
            // 0202        514     WM_LBUTTONUP
            // 0203        515     WM_LBUTTONDBLCLK
            // 0204        516     WM_RBUTTONDOWN
            // 0205        517     WM_RBUTTONUP
            // 0206        518     WM_RBUTTONDBLCLK
            // 0207        519     WM_MBUTTONDOWN
            // 0208        520     WM_MBUTTONUP
            // 0209        521     WM_MBUTTONDBLCLK
            // 0209        521     WM_MOUSELAST
            // 020a        522     WM_MOUSEWHEEL
            //

            // Eat left and right mouse clicks
            if (_disableMouse && (m.Msg >= 513 && m.Msg <= 522))
            {
                // eat message
            }
            else
            {
                // Eat button highlighting if the form is not in focus
                if (m.Msg == WM_MOUSEMOVE)
                {
                    if (!this.TopLevelControl.ContainsFocus)
                    {
                        // eat message
                    }
                    else
                    {
                        // handle messages normally
                        base.WndProc(ref m);
                    }
                }
                else
                {
                    // handle messages normally
                    base.WndProc(ref m);
                }
            }
        }
        // Clear                                                                                                                    
        public void Clear()
        {
            _geomParts.Nodes.Clear();
            _meshRefinements.Nodes.Clear();
            //
            _meshParts.Nodes.Clear();
            _nodeSets.Nodes.Clear();
            _elementSets.Nodes.Clear();
            _surfaces.Nodes.Clear();
            _referencePoints.Nodes.Clear();
            _materials.Nodes.Clear();
            _sections.Nodes.Clear();
            _constraints.Nodes.Clear();
            _surfaceInteractions.Nodes.Clear();
            _contactPairs.Nodes.Clear();
            _steps.Nodes.Clear();
            _analyses.Nodes.Clear();
            //
            SetNumberOfUserKeywords(0);
            _geomParts.Text = _geomPartsName;
            _meshRefinements.Text = _meshRefinementsName;
            //
            _meshParts.Text = _meshPartsName;
            _nodeSets.Text = _nodeSetsName;
            _elementSets.Text = _elementSetsName;
            _surfaces.Text = _surfacesName;
            _referencePoints.Text = _referencePointsName;
            _materials.Text = _materialsName;
            _sections.Text = _sectionsName;
            _constraints.Text = _constraintsName;
            _surfaceInteractions.Text = _surfaceInteractionsName;
            _contactPairs.Text = _contactPairsName;
            _steps.Text = _stepsName;
            _analyses.Text = _analysesName;
            //
            cltvGeometry.SelectedNodes.Clear();
            cltvModel.SelectedNodes.Clear();
            //
            ClearResults(); //calls cltvResults.SelectedNodes.Clear();
        }
        public void ClearActiveTreeSelection()
        {
            GetActiveTree().SelectedNodes.Clear();
        }
        public void ClearTreeSelection(ViewType view)
        {
            CodersLabTreeView tree = null;
            if (view == ViewType.Geometry) tree = cltvGeometry;
            else if (view == ViewType.Model) tree = cltvModel;
            else if (view == ViewType.Results) tree = cltvResults;
            else throw new NotSupportedException();
            tree.SelectedNodes.Clear();
        }
        public void ClearResults()
        {
            _resultParts.Nodes.Clear();
            _resultParts.Text = _resultPartsName;
            //
            _resultFieldOutputs.Nodes.Clear();
            _resultFieldOutputs.Text = _fieldOutputsName;
            //
            _resultHistoryOutputs.Nodes.Clear();
            _resultHistoryOutputs.Text = _historyOutputsName;
            //
            cltvResults.SelectedNodes.Clear();
        }
        //
        public void UpdateHighlight()
        {
            cltv_SelectionsChanged(null, null);
        }
        public int SelectBasePart(MouseEventArgs e, Keys modifierKeys, BasePart part)
        {
            try
            {
                _disableSelectionsChanged = true;

                CodersLabTreeView tree = GetActiveTree();
                TreeNode baseNode = tree.Nodes[0];
                TreeNode[] tmp = baseNode.Nodes.Find(part.Name, true);

                if (tmp.Length > 1)
                {
                    foreach (var treeNode in tmp)
                    {
                        if (treeNode.Tag.GetType() == part.GetType())
                        {
                            baseNode = treeNode;
                            break;
                        }
                    }
                }
                else
                {
                    if (tmp.Length > 0) baseNode = tmp[0];
                    else return -1;
                }
                //
                baseNode.Parent.Expand();   // expand if called as function
                //
                if (e.Button == MouseButtons.Left)
                {
                    if (modifierKeys == Keys.Shift && modifierKeys == Keys.Control) { }
                    else if (modifierKeys == Keys.Shift) tree.SelectedNodes.Add(baseNode);
                    else if (modifierKeys == Keys.Control)
                    {
                        if (tree.SelectedNodes.Contains(baseNode))
                            tree.SelectedNodes.Remove(baseNode);
                        else
                            tree.SelectedNodes.Add(baseNode);
                    }
                    else
                    {
                        // this is without modifier keys - a new selection
                        tree.SelectedNodes.Clear();
                        tree.SelectedNodes.Add(baseNode);
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (!tree.SelectedNodes.Contains(baseNode))
                    {
                        tree.SelectedNodes.Clear();
                        tree.SelectedNodes.Add(baseNode);
                    }
                }

                _disableSelectionsChanged = false;
                UpdateHighlight();

                return tree.SelectedNodes.Count;
            }
            catch { return -1; }
            finally { _disableSelectionsChanged = false; }
        }
        
        // Regenerate tree                                                                                                          
        public void RegenerateTree(FeModel model, IDictionary<string, AnalysisJob> jobs, FeResults results,
                                   HistoryResults history)
        {
            if (!_screenUpdating) return;
            //
            try
            {
                cltvGeometry.BeginUpdate();
                cltvModel.BeginUpdate();
                cltvResults.BeginUpdate();
                Dictionary<CodersLabTreeView, string[]> selectedNodePaths = GetSelectedNodePaths();
                Clear();
                if (model != null)
                {
                    // User keywords
                    if (model.CalculixUserKeywords != null) SetNumberOfUserKeywords(model.CalculixUserKeywords.Count);
                    // Geom Parts
                    if (model.Geometry != null)
                    {
                        //AddObjectsToNode<string, CaeMesh.BasePart>(_geomPartsName, _geomParts, model.Geometry.Parts);
                        AddGeometryParts(model.Geometry.Parts);
                        _geomParts.Expand();
                        AddObjectsToNode<string, CaeMesh.FeMeshRefinement>(_meshRefinementsName, _meshRefinements, 
                                                                           model.Geometry.MeshRefinements);
                        _meshRefinements.Expand();
                    }
                    //
                    if (model.Mesh != null)
                    {
                        // Mesh Parts
                        AddObjectsToNode<string, CaeMesh.BasePart>(_meshPartsName, _meshParts, model.Mesh.Parts);
                        // Node sets
                        AddObjectsToNode<string, CaeMesh.FeNodeSet>(_nodeSetsName, _nodeSets, model.Mesh.NodeSets);
                        // Element sets
                        AddObjectsToNode<string, CaeMesh.FeElementSet>(_elementSetsName, _elementSets, model.Mesh.ElementSets);
                        // Surfaces
                        AddObjectsToNode<string, CaeMesh.FeSurface>(_surfacesName, _surfaces, model.Mesh.Surfaces);
                        // Reference points
                        AddObjectsToNode<string, CaeMesh.FeReferencePoint>(_referencePointsName, _referencePoints,
                                                                           model.Mesh.ReferencePoints);
                    }
                    // Materials
                    AddObjectsToNode<string, Material>(_materialsName, _materials, model.Materials);
                    // Sections
                    AddObjectsToNode<string, Section>(_sectionsName, _sections, model.Sections);
                    // Constraints
                    AddObjectsToNode<string, Constraint>(_constraintsName, _constraints, model.Constraints);
                    // Surface interactions
                    AddObjectsToNode<string, SurfaceInteraction>(_surfaceInteractionsName, _surfaceInteractions,
                                                                 model.SurfaceInteractions);
                    // Contact pairs
                    AddObjectsToNode<string, ContactPair>(_contactPairsName, _contactPairs, model.ContactPairs);
                    // Steps
                    AddSteps(model.StepCollection.StepsList);
                    // Analyses
                    AddObjectsToNode<string, AnalysisJob>(_analysesName, _analyses, jobs);
                    _analyses.ExpandAll();
                    if (results != null)
                    {
                        if (results.Mesh != null)
                        {
                            // Results parts
                            AddObjectsToNode<string, CaeMesh.BasePart>(_resultPartsName, _resultParts, results.Mesh.Parts);
                            // Field outputs
                            string[] fieldNames;
                            string[][] allComponents;
                            //
                            fieldNames = results.GetAllFieldNames();
                            //
                            allComponents = new string[fieldNames.Length][];
                            for (int i = 0; i < fieldNames.Length; i++)
                            {
                                allComponents[i] = results.GetComponentNames(fieldNames[i]);
                            }
                            SetFieldOutputAndComponentNames(fieldNames, allComponents);
                            //SelectFirstComponentOfFirstFieldOutput();
                        }
                    }
                    if (history != null) SetHistoryOutputNames(history);
                }
                //
                SelectNodesByPath(selectedNodePaths);
                //
                cltvGeometry.EndUpdate();
                cltvModel.EndUpdate();
                cltvResults.EndUpdate();
            }
            catch { }
        }
        private Dictionary<CodersLabTreeView, string[]> GetSelectedNodePaths()
        {
            string splitter = "\\";
            string path;
            TreeNode node;
            Dictionary<CodersLabTreeView, string[]> selectedNodePaths = new Dictionary<CodersLabTreeView, string[]>();
            List<string> nodePaths = new List<string>();
            CodersLabTreeView[] trees = new CodersLabTreeView[] { cltvGeometry, cltvModel, cltvResults };
            //
            for (int i = 0; i < trees.Length; i++)
            {
                if (trees[i].SelectedNodes.Count > 0)
                {
                    nodePaths.Clear();
                    foreach (TreeNode selectedNode in trees[i].SelectedNodes)
                    {
                        path = "";
                        node = selectedNode;
                        while (node != null)
                        {
                            path = node.Name + splitter + path;
                            node = node.Parent;
                        }
                        nodePaths.Add(path);
                    }
                    selectedNodePaths.Add(trees[i], nodePaths.ToArray());
                }
            }
            return selectedNodePaths;
        }
        private void SelectNodesByPath(Dictionary<CodersLabTreeView, string[]> selectedNodeNames)
        {
            string[] splitter = new string[] { "\\" };
            string[] tmp;
            TreeNode node;
            foreach (var entry in selectedNodeNames)
            {
                node = null;
                for (int i = 0; i < entry.Value.Length; i++)
                {
                    tmp = entry.Value[i].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                    node = entry.Key.Nodes[tmp[0]];
                    for (int j = 1; j < tmp.Length; j++)
                    {
                        if (node != null) node = node.Nodes[tmp[j]];
                    }
                    if (node != null) entry.Key.SelectedNodes.Add(node);
                }
            }
        }
        public void AddTreeNode(ViewType view, NamedClass item, string stepName)
        {
            if (!_screenUpdating) return;
            if (item.Internal) return;
            //
            TreeNode node;
            TreeNode parent;
            TreeNode[] tmp;
            //
            if (item is FeMeshRefinement)
            {
                node = _meshRefinements.Nodes.Add(item.Name);
                node.Name = node.Text;
                node.Tag = item;
                parent = _meshRefinements;
            }
            else if (item is MeshPart)
            {
                node = _meshParts.Nodes.Add(item.Name);
                node.Name = node.Text;
                node.Tag = item;
                parent = _meshParts;
            }
            else if (item is FeNodeSet)
            {
                node = _nodeSets.Nodes.Add(item.Name);
                node.Name = node.Text;
                node.Tag = item;
                parent = _nodeSets;
            }
            else if (item is FeElementSet)
            {
                node = _elementSets.Nodes.Add(item.Name);
                node.Name = node.Text;
                node.Tag = item;
                parent = _elementSets;
            }
            else if (item is FeElementSet)
            {
                node = _elementSets.Nodes.Add(item.Name);
                node.Name = node.Text;
                node.Tag = item;
                parent = _elementSets;
            }
            else if (item is FeSurface)
            {
                node = _surfaces.Nodes.Add(item.Name);
                node.Name = node.Text;
                node.Tag = item;
                parent = _surfaces;
            }
            else if (item is FeReferencePoint)
            {
                node = _referencePoints.Nodes.Add(item.Name);
                node.Name = node.Text;
                node.Tag = item;
                parent = _referencePoints;
            }
            else if (item is Material)
            {
                node = _materials.Nodes.Add(item.Name);
                node.Name = node.Text;
                node.Tag = item;
                parent = _materials;
            }
            else if (item is Section)
            {
                node = _sections.Nodes.Add(item.Name);
                node.Name = node.Text;
                node.Tag = item;
                parent = _sections;
            }
            else if (item is Constraint)
            {
                node = _constraints.Nodes.Add(item.Name);
                node.Name = node.Text;
                node.Tag = item;
                parent = _constraints;
            }
            else if (item is SurfaceInteraction)
            {
                node = _surfaceInteractions.Nodes.Add(item.Name);
                node.Name = node.Text;
                node.Tag = item;
                parent = _surfaceInteractions;
            }
            else if (item is ContactPair)
            {
                node = _contactPairs.Nodes.Add(item.Name);
                node.Name = node.Text;
                node.Tag = item;
                parent = _contactPairs;
            }
            else if (item is Step)
            {
                node = AddStep((Step)item);
                parent = _steps;
            }
            else if (item is HistoryOutput && stepName != null)
            {
                tmp = _steps.Nodes.Find(stepName, true);
                if (tmp.Length > 1) throw new Exception("Adding operation failed. More than one step named: " + stepName);
                //
                tmp = tmp[0].Nodes.Find(_historyOutputsName, true);
                if (tmp.Length > 1) throw new Exception("Adding operation failed. There is no history output node to add to.");
                //
                node = tmp[0].Nodes.Add(item.Name);
                node.Name = node.Text;
                node.Tag = item;
                parent = tmp[0];
            }
            else if (item is FieldOutput && stepName != null)
            {
                tmp = _steps.Nodes.Find(stepName, true);
                if (tmp.Length > 1) throw new Exception("Adding operation failed. More than one step named: " + stepName);
                //
                tmp = tmp[0].Nodes.Find(_fieldOutputsName, true);
                if (tmp.Length > 1) throw new Exception("Adding operation failed. There is no field output node to add to.");
                //
                node = tmp[0].Nodes.Add(item.Name);
                node.Name = node.Text;
                node.Tag = item;
                parent = tmp[0];
            }
            else if (item is BoundaryCondition && stepName != null)
            {
                tmp = _steps.Nodes.Find(stepName, true);
                if (tmp.Length > 1) throw new Exception("Adding operation failed. More than one step named: " + stepName);
                //
                tmp = tmp[0].Nodes.Find(_boundaryConditionsName, true);
                if (tmp.Length > 1) throw new Exception("Adding operation failed. There is no bounday condition node to add to.");
                //
                node = tmp[0].Nodes.Add(item.Name);
                node.Name = node.Text;
                node.Tag = item;
                parent = tmp[0];
            }
            else if (item is Load && stepName != null)
            {
                tmp = _steps.Nodes.Find(stepName, true);
                if (tmp.Length > 1) throw new Exception("Adding operation failed. More than one step named: " + stepName);
                //
                tmp = tmp[0].Nodes.Find(_loadsName, true);
                if (tmp.Length > 1) throw new Exception("Adding operation failed. There is no load node to add to.");
                //
                node = tmp[0].Nodes.Add(item.Name);
                node.Name = node.Text;
                node.Tag = item;
                parent = tmp[0];
            }
            else if (item is AnalysisJob)
            {
                node = _analyses.Nodes.Add(item.Name);
                node.Name = node.Text;
                node.Tag = item;
                parent = _analyses;
            }           
            else throw new NotImplementedException();
            //
            parent.Text = parent.Name;
            parent.Expand();
            if (parent.Nodes.Count > 0) parent.Text += " (" + parent.Nodes.Count + ")";
            //
            SetNodeStatus(node);
            //
            GetActiveTree().SelectedNode = node;
        }
        public void UpdateTreeNode(ViewType view, string oldItemName, NamedClass item, string stepName, bool updateSelection)
        {
            if (!_screenUpdating) return;   // must be here; when _screenUpdating = false the function add tree node is not working
            //
            CodersLabTreeView tree = null;
            if (view == ViewType.Geometry) tree = cltvGeometry;
            else if (view == ViewType.Model) tree = cltvModel;
            else if (view == ViewType.Results) tree = cltvResults;
            else throw new NotSupportedException();
            //
            TreeNode baseNode = null;
            if (item is AnalysisJob) baseNode = _analyses;
            else if (item is FeMeshRefinement) baseNode = _meshRefinements;
            else baseNode = tree.Nodes[0];

            TreeNode[] tmp;
            if (stepName != null)
            {
                tmp = _steps.Nodes.Find(stepName, true);
                if (tmp.Length > 1) throw new Exception("Tree update failed. More than one step named: " + stepName);
                baseNode = tmp[0];
            }

            bool nodeFound;
            tmp = baseNode.Nodes.Find(oldItemName, true);
            if (tmp.Length > 1)
            {
                nodeFound = false;
                foreach (var treeNode in tmp)
                {
                    if (treeNode.Tag.GetType() == item.GetType())
                    {
                        baseNode = treeNode;
                        nodeFound = true;
                        break;
                    }
                }
                if (!nodeFound) throw new Exception("Tree update failed. The item name to edit was not found: " + item.Name);
            }
            else
            {
                if (tmp.Length > 0) baseNode = tmp[0];
                else return;
            }

            baseNode.Text = item.Name;
            baseNode.Name = item.Name;
            baseNode.Tag = item;

            SetNodeStatus(baseNode);

            // update selection
            if (updateSelection)
            {
                if (tree != null && tree.SelectedNodes.Contains(baseNode)) UpdateHighlight();    // update only once
                else baseNode.TreeView.SelectedNode = baseNode; // for job the tree is null
            }
        }
        public void RemoveTreeNode<T>(ViewType view, string nodeName, string stepName) where T : NamedClass
        {
            if (!_screenUpdating) return;
            //
            CodersLabTreeView tree = null;      // the tree depends on the view
            if (view == ViewType.Geometry) tree = cltvGeometry;
            else if (view == ViewType.Model) tree = cltvModel;
            else if (view == ViewType.Results) tree = cltvResults;
            else throw new NotSupportedException();
            //
            TreeNode baseNode = null;
            if (typeof(T) == typeof(AnalysisJob)) baseNode = _analyses;
            else if (typeof(T) == typeof(FeMeshRefinement)) baseNode = _meshRefinements;
            else baseNode = tree.Nodes[0];
            //
            TreeNode[] tmp;
            if (stepName != null)
            {
                tmp = _steps.Nodes.Find(stepName, true);
                if (tmp.Length > 1) throw new Exception("Tree update failed. More than one step named: " + stepName);
                baseNode = tmp[0];
            }
            //
            tmp = baseNode.Nodes.Find(nodeName, true);
            int count = 0;
            //
            for (int i = 0; i < tmp.Length; i++)
            {
                if (tmp[i].Tag is T)
                {
                    tmp[0] = tmp[i];
                    count++;
                }
            }
            //
            if (count > 1) throw new Exception("Tree update failed. More than one tree node named: " + nodeName);
            if (count < 1) throw new Exception("Tree update failed. There is no tree node named: " + nodeName);
            //
            TreeNode parent = tmp[0].Parent;
            //
            tmp[0].Remove();
            tree.SelectedNodes.Remove(tmp[0]);
            //
            parent.Text = parent.Name;
            if (parent.Nodes.Count > 0) parent.Text += " (" + parent.Nodes.Count + ")";
        }
        private void AddObjectsToNode<Tkey, Tval>(string initialNodeName, TreeNode node, IDictionary<Tkey, Tval> dictionary,
                                                  bool countNodes = true)
        {
            TreeNode nodeToAdd;
            //
            var list = dictionary.Keys.ToList();
            //
            foreach (var key in list)
            {
                if (dictionary[key] is NamedClass nc && nc.Internal) continue;
                //
                nodeToAdd = node.Nodes.Add(key.ToString());
                nodeToAdd.Name = nodeToAdd.Text;
                nodeToAdd.Tag = dictionary[key];
                SetNodeStatus(nodeToAdd);
            }
            //
            if (countNodes && node.Nodes.Count > 0) node.Text = initialNodeName + " (" + node.Nodes.Count.ToString() + ")";
            else node.Text = initialNodeName;
        }
        private void AddGeometryParts(IDictionary<string, BasePart> parts)
        {
            HashSet<string> dependentPartNames = new HashSet<string>();
            foreach (var entry in parts)
            {
                if (entry.Value is CompoundGeometryPart cgp)
                {
                    dependentPartNames.UnionWith(cgp.SubPartNames);
                }
            }
            //
            BasePart part;
            BasePart subPart;
            TreeNode nodeToAdd;
            TreeNode subNodeToAdd;
            foreach (var entry in parts)
            {
                part = entry.Value;                
                if (!dependentPartNames.Contains(part.Name))
                {
                    // Independent parts
                    nodeToAdd = _geomParts.Nodes.Add(part.Name);
                    nodeToAdd.Name = nodeToAdd.Text;
                    nodeToAdd.Tag = part;
                    SetNodeStatus(nodeToAdd);
                    //Compound parts
                    if (part is CompoundGeometryPart cgp)
                    {
                        for (int i = 0; i < cgp.SubPartNames.Length; i++)
                        {
                            subPart = parts[cgp.SubPartNames[i]];
                            subNodeToAdd = nodeToAdd.Nodes.Add(subPart.Name);
                            subNodeToAdd.Name = subNodeToAdd.Text;
                            subNodeToAdd.Tag = subPart;
                            SetNodeStatus(subNodeToAdd);
                        }
                        //
                        nodeToAdd.Expand();
                    }
                }
            }
            //
            if (_geomParts.Nodes.Count > 0) _geomParts.Text = _geomPartsName + " (" + _geomParts.Nodes.Count.ToString() + ")";
            else _geomParts.Text = _geomPartsName;
        }
        private void AddSteps(List<Step> steps)
        {
            foreach (var step in steps) AddStep(step);

            if (_steps.Nodes.Count > 0) _steps.Text = _stepsName + " (" + _steps.Nodes.Count.ToString() + ")";
            else _steps.Text = _stepsName;

            _steps.Expand();
        }
        private TreeNode AddStep(Step step)
        {
            TreeNode stepNode = _steps.Nodes.Add(step.Name);
            stepNode.Name = stepNode.Text;
            stepNode.Tag = step;
            SetNodeStatus(stepNode);

            TreeNode tmp;
            // HistoryOutputs
            tmp = stepNode.Nodes.Add(_historyOutputsName);
            tmp.Name = tmp.Text;
            AddObjectsToNode<string, HistoryOutput>(_historyOutputsName, tmp, step.HistoryOutputs);
            SetNodeImage(tmp, "History_output.ico");

            // FieldOutputs
            tmp = stepNode.Nodes.Add(_fieldOutputsName);
            tmp.Name = tmp.Text;
            AddObjectsToNode<string, FieldOutput>(_fieldOutputsName, tmp, step.FieldOutputs);
            SetNodeImage(tmp, "Field_output.ico");

            // BoundaryConditions
            tmp = stepNode.Nodes.Add(_boundaryConditionsName);
            tmp.Name = tmp.Text;
            AddObjectsToNode<string, BoundaryCondition>(_boundaryConditionsName, tmp, step.BoundaryConditions);
            SetNodeImage(tmp, "Bc.ico");
            
            // Loads
            tmp = stepNode.Nodes.Add(_loadsName);
            tmp.Name = tmp.Text;
            AddObjectsToNode<string, Load>(_loadsName, tmp, step.Loads);
            SetNodeImage(tmp, "Load.ico");

            stepNode.Expand();

            return stepNode;
        }

        // Node status                                                                                                              
        private void SetAllNodesStatusIcons(TreeNode node)
        {
            if (node.ImageKey == "Dots.ico" || node.ImageKey == "Dots_t.ico")
            {
                if (node.Nodes.Count == 0 || !node.IsExpanded) SetNodeImage(node, "Dots.ico");
                else SetNodeImage(node, "Dots_t.ico");
            }

            foreach (TreeNode child in node.Nodes)
            {
                SetAllNodesStatusIcons(child);
            }
        }
        private void SetNodeStatus(TreeNode node)
        {
            if (node != null && node.Tag != null)
            {
                CodersLabTreeView cltv = (CodersLabTreeView)node.TreeView;
                NamedClass item = (NamedClass)node.Tag;
                if (item.Active)
                {
                    if (node.Tag is AnalysisJob)
                    {
                        AnalysisJob job = node.Tag as AnalysisJob;
                        if (job.JobStatus == JobStatus.OK) SetNodeImage(node, "OK.ico");
                        else if (job.JobStatus == JobStatus.Running) SetNodeImage(node, "Running.ico");
                        else if (job.JobStatus == JobStatus.FailedWithResults) SetNodeImage(node, "Warning.ico");
                        else SetNodeImage(node, "NoResult.ico");
                    }
                    else
                    {
                        // Hide
                        if (CanHide(item) && !item.Visible)
                        {
                            cltv.SetNodeForeColor(node, Color.Gray);
                            SetNodeImage(node, "Hide.ico");
                        }
                        // Show
                        else
                        {
                            cltv.SetNodeForeColor(node, Color.Black);
                            //
                            if (node.Nodes.Count == 0 || !node.IsExpanded) SetNodeImage(node, "Dots.ico");
                            else SetNodeImage(node, "Dots_t.ico");
                            //
                            if (item is GeometryPart && ((GeometryPart)item).ErrorElementIds != null)
                            {
                                SetNodeImage(node, "Warning.ico");
                            }
                        }
                    }
                }
                else
                {
                    cltv.SetNodeForeColor(node, Color.Gray);
                    SetNodeImage(node, "Unactive.ico");
                }
                //
                if (!item.Valid)
                {
                    cltv.SetNodeForeColor(node, cltv.HighlightForeErrorColor);
                    node.Parent.Expand();
                }
            }
        }
        private void SetNodeImage(TreeNode node, string imageName)
        {
            if (node.ImageKey != imageName)
            {
                node.ImageKey = imageName;
                node.SelectedImageKey = imageName;
            }
        }

        // Results                                                                                                                  
        public void SetFieldOutputAndComponentNames(string[] fieldNames, string[][] components)
        {
            TreeNode node;
            TreeNode child;
            for (int i = 0; i < fieldNames.Length; i++)
            {
                node = _resultFieldOutputs.Nodes.Add(fieldNames[i]);
                node.Name = node.Text;
                SetNodeImage(node, "Dots.ico");
                //
                for (int j = 0; j < components[i].Length; j++)
                {
                    child = node.Nodes.Add(components[i][j]);
                    child.Name = child.Text;
                    child.Tag = new CaeResults.FieldData(child.Name);
                    SetNodeImage(child, "Dots.ico");
                }
                //
                if (i <= 1) node.Expand();
            }
            //
            _resultFieldOutputs.Expand();
            //
            int n = _resultFieldOutputs.Nodes.Count;
            if (n > 0) _resultFieldOutputs.Text = _fieldOutputsName + " (" + n + ")";
        }
        public void SelectFirstComponentOfFirstFieldOutput()
        {
            if (_resultFieldOutputs.Nodes.Count > 0)
            {
                foreach (TreeNode node in _resultFieldOutputs.Nodes)
                {
                    if (node.Nodes.Count > 0)
                    {
                        cltvResults.SelectedNode = node.Nodes[0];
                        return;
                    }
                }
            }
        }
        public void SetHistoryOutputNames(HistoryResults historyOutput)
        {
            TreeNode node1;
            TreeNode node2;
            TreeNode node3;
            foreach (var setEntry in historyOutput.Sets)
            {
                node1 = _resultHistoryOutputs.Nodes.Add(setEntry.Key);
                node1.Name = node1.Text;
                SetNodeImage(node1, "Dots.ico");
                //
                foreach (var fieldEntry in setEntry.Value.Fields)
                {
                    node2 = node1.Nodes.Add(fieldEntry.Key);
                    node2.Name = node2.Text;
                    SetNodeImage(node2, "Dots.ico");
                    //
                    foreach (var componentEntry in fieldEntry.Value.Components)
                    {
                        node3 = node2.Nodes.Add(componentEntry.Key);
                        node3.Name = node3.Text;
                        SetNodeImage(node3, "Dots.ico");
                        node3.Tag = new HistoryResultData(setEntry.Key, fieldEntry.Key, componentEntry.Key);
                    }
                }
            }
            //
            _resultHistoryOutputs.Expand();
            //
            int n = _resultHistoryOutputs.Nodes.Count;
            if (n > 0) _resultHistoryOutputs.Text = _historyOutputsName + " (" + n + ")";
        }
       

        // Expand/Collapse                                                                                                          
        private CodersLabTreeView GetActiveTree()
        {
            if (tcGeometryModelResults.TabPages.Count <= 0) return null;
            //
            if (tcGeometryModelResults.SelectedTab == tpGeometry) return cltvGeometry;
            else if (tcGeometryModelResults.SelectedTab == tpModel) return cltvModel;
            else if (tcGeometryModelResults.SelectedTab == tpResults) return cltvResults;
            else throw new NotSupportedException();
        }
        public bool[] GetTreeExpandCollapseState()
        {
            List<bool> states = new List<bool>();
            foreach (TreeNode node in cltvModel.Nodes)
            {
                GetNodeExpandCollapseState(node, states);
            }
           
            return states.ToArray();
        }
        private void GetNodeExpandCollapseState(TreeNode node, List<bool> states)
        {
            states.Add(node.IsExpanded);
            foreach (TreeNode child in node.Nodes)
            {
                GetNodeExpandCollapseState(child, states);
            }
        }
        public void SetTreeExpandCollapseState(bool[] states)
        {
            try
            {
                int count = 0;
                cltvModel.BeginUpdate();
                foreach (TreeNode node in cltvModel.Nodes)
                {
                    SetNodeExpandCollapseState(node, states, ref count);
                }
                cltvModel.EndUpdate();
            }
            catch (Exception)
            {
            }
           
        }
        private void SetNodeExpandCollapseState(TreeNode node, bool[] states, ref int count)
        {
            if (states[count]) node.Expand();
            else node.Collapse();
            count++;

            foreach (TreeNode child in node.Nodes)
            {
                SetNodeExpandCollapseState(child, states, ref count);
            }
        }

        //                                                                                                                          
        private bool CanCreate(TreeNode node)
        {
            if (node.Name == _meshRefinementsName) return true;
            else if (node.Name == _nodeSetsName) return true;
            else if (node.Name == _elementSetsName) return true;
            else if (node.Name == _surfacesName) return true;
            else if (node.Name == _referencePointsName) return true;
            else if (node.Name == _materialsName) return true;
            else if (node.Name == _sectionsName) return true;
            else if (node.Name == _constraintsName) return true;
            else if (node.Name == _surfaceInteractionsName) return true;
            else if (node.Name == _contactPairsName) return true;
            else if (node.Name == _stepsName) return true;
            else if (node.Name == _historyOutputsName) return true;
            else if (node.Name == _fieldOutputsName) return true;
            else if (node.Name == _boundaryConditionsName) return true;
            else if (node.Name == _loadsName) return true;
            else if (node.Name == _analysesName) return true;
            else return false;
        }
        private bool CanDuplicate(TreeNode node)
        {
            if (node.Tag is Material) return true;
            else if (node.Tag is SurfaceInteraction) return true;
            else if (node.Tag is Step) return true;
            else return false;
        }
        private bool CanDeactivate(TreeNode node)
        {
            if (node.Tag is FeMeshRefinement) return true;
            else if (node.Tag is Constraint) return true;
            else if (node.Tag is ContactPair) return true;
            else if (node.Tag is Step) return true;
            else if (node.Tag is HistoryOutput) return true;
            else if (node.Tag is FieldOutput) return true;
            else if (node.Tag is BoundaryCondition) return true;
            else if (node.Tag is Load) return true;
            else return false;
        }
        private bool CanHide(object item)
        {
            if (item is BasePart) return true;
            else if (item is Constraint) return true;
            else if (item is ContactPair) return true;
            else if (item is BoundaryCondition) return true;
            else if (item is Load) return true;
            else return false;
        }

        //                                                                                                                          
        public void SetNumberOfUserKeywords(int numOfUserKeywords)
        {
            _numUserKeywords = numOfUserKeywords;
            cltvModel.Nodes[0].Text = "Model";
            if (_numUserKeywords > 0) cltvModel.Nodes[0].Text += " (User keywords: " + _numUserKeywords + ")";
        }

        //                                                                                                                          
        public void ShowContextMenu(Control control, int x, int y)
        {
            CodersLabTreeView tree = GetActiveTree();
            if (tree.SelectedNodes.Count > 0)
            {
                PrepareToolStripItem(tree);
                //Expand / Collapse
                tsmiSpaceExpandColapse.Visible = false;
                tsmiExpandAll.Visible = false;
                tsmiCollapseAll.Visible = false;
                //
                cmsTree.Show(control, x, y);
            }
        }

       
    }
}
