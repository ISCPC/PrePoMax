using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using PrePoMax.Forms;
using CaeGlobals;
using UserControls;
using CaeJob;
using System.Reflection;
using CaeModel;
using CaeMesh;

namespace PrePoMax
{
    public enum Cell3D
    {
        Wire,
        Shell,
        Solid
    }

    public enum FormMode
    {
        Create,
        Edit
    }

    public partial class FrmMain : MainMouseWheelManagedForm
    {
        // Variables                                                                                                                
        #region Variables ##########################################################################################################

        FrmSplash splash;

        private vtkControl.vtkControl _vtk;
        private UserControls.ModelTree _modelTree;
        private Controller _controller;
        private string[] _args;
        private string[] outputLines;
        private Dictionary<ViewGeometryModelResults, Action<object, EventArgs>> _edgeVisibilities; // save display style
        private AdvisorControl _advisorControl;
        //
        private Point _formLocation;
        private List<Form> _allForms;
        private FrmSectionView _frmSectionView;        
        private FrmSelectEntity _frmSelectEntity;
        private FrmSelectGeometry _frmSelectGeometry;
        private FrmSelectItemSet _frmSelectItemSet;
        private FrmUnitSystem _frmUnitSystem;
        private FrmAnalyzeGeometry _frmAnalyzeGeometry;
        private FrmMeshingParameters _frmMeshingParameters;
        private FrmMeshRefinement _frmMeshRefinement;
        private FrmModelProperties _frmModelProperties;
        private FrmCalculixKeywordEditor _frmCalculixKeywordEditor;
        private FrmPartProperties _frmPartProperties;
        private FrmBoundaryLayer _frmBoundaryLayer;
        private FrmRemeshingParameters _frmRemeshingParameters;
        private FrmTranslate _frmTranslate;
        private FrmScale _frmScale;
        private FrmRotate _frmRotate;
        private FrmNodeSet _frmNodeSet;
        private FrmElementSet _frmElementSet;
        private FrmSurface _frmSurface;
        private FrmReferencePoint _frmReferencePoint;
        private FrmMaterial _frmMaterial;
        private FrmSection _frmSection;
        private FrmConstraint _frmConstraint;
        private FrmSurfaceInteraction _frmSurfaceInteraction;
        private FrmContactPair _frmContactPair;
        private FrmStep _frmStep;
        private FrmHistoryOutput _frmHistoryOutput;
        private FrmFieldOutput _frmFieldOutput;
        private FrmBC _frmBoundaryCondition;
        private FrmLoad _frmLoad;
        private FrmSettings _frmSettings;
        private FrmQuery _frmQuery;
        private FrmAnalysis _frmAnalysis;
        private FrmMonitor _frmMonitor;
        private FrmAnimation _frmAnimation;
        private FrmHistoryResultsOutput _frmHistoryResultsOutput;
        private FrmTransformation _frmTransformation;



        #endregion  ################################################################################################################

        #region Properties #########################################################################################################
        public Controller Controller { get { return _controller; } set { _controller = value; } }
        public ViewGeometryModelResults GetCurrentView()
        {
            return _controller.CurrentView;
        }
        public void SetCurrentView(ViewGeometryModelResults view)
        {
            // This gets called from: _controller.CurrentView
            InvokeIfRequired(() =>
            {
                if (view == ViewGeometryModelResults.Geometry)
                {
                    _modelTree.SetGeometryTab();
                    if (_controller.Model != null) UpdateUnitSystem(_controller.Model.UnitSystem);
                }
                else if (view == ViewGeometryModelResults.Model)
                {
                    _modelTree.SetModelTab();
                    if (_controller.Model != null) UpdateUnitSystem(_controller.Model.UnitSystem);
                }
                else if (view == ViewGeometryModelResults.Results)
                {
                    _modelTree.SetResultsTab();
                    InitializeResultWidgetPositions();
                    if (_controller.Results != null) UpdateUnitSystem(_controller.Results.UnitSystem);
                }
                else throw new NotSupportedException();
                //
                if (_advisorControl != null)
                {
                    ViewType viewType;
                    if (view == ViewGeometryModelResults.Geometry) viewType = ViewType.Geometry;
                    else if (view == ViewGeometryModelResults.Model) viewType = ViewType.Model;
                    else if (view == ViewGeometryModelResults.Results) viewType = ViewType.Results;
                    else throw new NotSupportedException();
                    //
                    _advisorControl.PrepareControls(viewType);
                }
                //
                SetMenuAndToolStripVisibility();
                //
                _edgeVisibilities[_controller.CurrentView](null, null);
                //
                this.ActiveControl = null;
            });
        }
        public bool ScreenUpdating { get { return _modelTree.ScreenUpdating; } set { _modelTree.ScreenUpdating = value; } }

        #endregion  ################################################################################################################


        // Constructors                                                                                                             
        public FrmMain(string[] args)
        {
            // Initialize               
            InitializeComponent();
            //SettingsContainer settings = new SettingsContainer();
            //settings.LoadFromFile();
            //if (settings.General.Maximized)
            //{
            //    Rectangle resolution = Screen.FromControl(this).Bounds;
            //    this.Location = new Point(0, 0);
            //    this.Size = new Size(resolution.Width, resolution.Height);
            //    this.WindowState = FormWindowState.Maximized;
            //}
            _vtk = null;
            _controller = null;
            _modelTree = null;
            _args = args;
            _edgeVisibilities = new Dictionary<ViewGeometryModelResults, Action<object, EventArgs>>();
            _edgeVisibilities.Add(ViewGeometryModelResults.Geometry, tsmiShowModelEdges_Click);
            _edgeVisibilities.Add(ViewGeometryModelResults.Model, tsmiShowElementEdges_Click);
            _edgeVisibilities.Add(ViewGeometryModelResults.Results, tsmiShowElementEdges_Click);
        }


        // Event handling                                                                                                           
        private void FrmMain_Load(object sender, EventArgs e)
        {
            //StringEnergyConverter.SetUnit = "in·lb";
            ////
            //StringEnergyPerVolumeConverter converter = new StringEnergyPerVolumeConverter();
            //StringEnergyPerVolumeConverter.SetEnergyUnit = "in·lb";
            //StringEnergyPerVolumeConverter.SetVolumeUnit = "in³";
            //double v1 = (double)converter.ConvertFromString("8.5 in·lb/in³");

            Text = Globals.ProgramName;
            this.TopMost = true;
            splash = new FrmSplash { TopMost = true };
            var task = Task.Run(() => splash.ShowDialog());            
            //
            try
            {
                // Vtk
                _vtk = new vtkControl.vtkControl();
                panelControl.Parent.Controls.Add(_vtk);
                panelControl.SendToBack();
                // Menu
                tsmiColorAnnotations.DropDown.Closing += DropDown_Closing;
                // Tree
                this._modelTree = new UserControls.ModelTree();
                this._modelTree.Name = "modelTree";
                this.splitContainer1.Panel1.Controls.Add(this._modelTree);
                this._modelTree.Dock = System.Windows.Forms.DockStyle.Fill;
                this._modelTree.TabIndex = 0;
                //
                _modelTree.GeometryMeshResultsEvent += ModelTree_ViewEvent;
                _modelTree.SelectEvent += ModelTree_Select;
                _modelTree.ClearSelectionEvent += Clear3DSelection;
                _modelTree.CreateEvent += ModelTree_CreateEvent;
                _modelTree.DuplicateEvent += ModelTree_DuplicateEvent;
                _modelTree.EditEvent += ModelTree_Edit;
                _modelTree.HideShowEvent += ModelTree_HideShowEvent;
                _modelTree.SetTransparencyEvent += ModelTree_SetTransparencyEvent;
                _modelTree.ColorContoursVisibilityEvent += ModelTree_ColorContoursVisibilityEvent;
                _modelTree.CreateCompoundPart += CreateAndImportCompoundPart;
                _modelTree.MeshingParametersEvent += GetSetMeshingParameters;
                _modelTree.PreviewEdgeMesh += PreviewEdgeMeshes;
                _modelTree.CreateMeshEvent += CreatePartMeshes;
                _modelTree.CopyGeometryToResultsEvent += CopyGeometryPartsToResults;
                _modelTree.EditCalculixKeywords += EditEditCalculiXKeywords;
                _modelTree.MergeParts += MergeModelParts;
                _modelTree.ConvertElementSetsToMeshParts += ConvertElementSetsToMeshParts;
                _modelTree.MaterialLibrary += ShowMaterialLibrary;
                _modelTree.RunEvent += RunAnalysis;
                _modelTree.MonitorEvent += MonitorAnalysis;
                _modelTree.ResultsEvent += ResultsAnalysis;
                _modelTree.KillEvent += KillAnalysis;
                _modelTree.ActivateDeactivateEvent += ModelTree_ActivateDeactivateEvent;
                _modelTree.DeleteEvent += ModelTree_Delete;
                _modelTree.FieldDataSelectEvent += ModelTree_FieldDataSelectEvent;
                _modelTree.RenderingOff += () => _vtk.RenderingOn = false;
                _modelTree.RenderingOn += () => _vtk.RenderingOn = true;
                // Strip menus
                tsFile.Location = new Point(0, 0);
                tsViews.Location = new Point(tsFile.Left + tsFile.Width, 0);
                tsResults.Location = new Point(tsViews.Left + tsViews.Width, 0);
                tscbSymbolsForStep.SelectedIndexChanged += tscbSymbolsForStep_SelectedIndexChanged;
                // Controller
                _controller = new PrePoMax.Controller(this);
                // Vtk
                _vtk.OnMouseLeftButtonUpSelection += SelectPointOrArea;
                _vtk.KeyPressEvent += Vtk_KeyPressEvent;
                _vtk.Controller_GetNodeActorData = _controller.GetNodeActorData;
                _vtk.Controller_GetCellActorData = _controller.GetCellActorData;
                _vtk.Controller_GetCellFaceActorData = _controller.GetCellFaceActorData;
                _vtk.Controller_GetEdgeActorData = _controller.GetEdgeActorData;
                _vtk.Controller_GetSurfaceEdgesActorDataFromElementId = _controller.GetSurfaceEdgeActorDataFromElementId;
                _vtk.Controller_GetSurfaceEdgesActorDataFromNodeAndElementIds = _controller.GetSurfaceEdgeActorDataFromNodeAndElementIds;
                _vtk.Controller_GetPartActorData = _controller.GetPartActorData;
                _vtk.Controller_GetGeometryActorData = _controller.GetGeometryActorData;
                _vtk.Controller_GetGeometryVertexActorData = _controller.GetGeometryVertexActorData;
                _vtk.Controller_ActorsPicked = SelectBaseParts;
                _vtk.Controller_ShowLegendSettings = ShowLegendSettings;
                _vtk.Controller_ShowStatusBlockSettings = ShowStatusBlockSettings;
                // Forms
                _formLocation = new Point(100, 100);
                _allForms = new List<Form>();
                //
                _frmSelectEntity = new FrmSelectEntity(_controller);
                AddFormToAllForms(_frmSelectEntity);
                //
                _frmSelectGeometry = new FrmSelectGeometry(_controller);
                AddFormToAllForms(_frmSelectGeometry);
                //
                _frmSelectItemSet = new FrmSelectItemSet(_controller);
                AddFormToAllForms(_frmSelectItemSet);
                //
                _frmSectionView = new FrmSectionView(_controller);
                AddFormToAllForms(_frmSectionView);
                //
                _frmUnitSystem = new FrmUnitSystem(_controller);
                AddFormToAllForms(_frmUnitSystem);
                //
                _frmAnalyzeGeometry = new FrmAnalyzeGeometry(_controller);
                AddFormToAllForms(_frmAnalyzeGeometry);
                //
                _frmMeshingParameters = new FrmMeshingParameters(_controller);
                _frmMeshingParameters.UpdateHighlightFromTree = UpdateHighlightFromTree;
                AddFormToAllForms(_frmMeshingParameters);
                //
                _frmMeshRefinement = new FrmMeshRefinement(_controller);
                AddFormToAllForms(_frmMeshRefinement);
                //
                _frmModelProperties = new FrmModelProperties(_controller);
                AddFormToAllForms(_frmModelProperties);
                //
                _frmPartProperties = new FrmPartProperties(_controller);
                AddFormToAllForms(_frmPartProperties);
                //
                _frmBoundaryLayer = new FrmBoundaryLayer(_controller);
                AddFormToAllForms(_frmBoundaryLayer);
                //
                _frmRemeshingParameters = new FrmRemeshingParameters(_controller);
                AddFormToAllForms(_frmRemeshingParameters);
                //
                _frmTranslate = new FrmTranslate(_controller);
                AddFormToAllForms(_frmTranslate);
                //
                _frmScale = new FrmScale(_controller);
                AddFormToAllForms(_frmScale);
                //
                _frmRotate = new FrmRotate(_controller);
                AddFormToAllForms(_frmRotate);
                //
                _frmNodeSet = new FrmNodeSet(_controller);
                AddFormToAllForms(_frmNodeSet);
                //
                _frmElementSet = new FrmElementSet(_controller);
                AddFormToAllForms(_frmElementSet);
                //
                _frmSurface = new FrmSurface(_controller);                
                AddFormToAllForms(_frmSurface);
                //
                _frmReferencePoint = new FrmReferencePoint(_controller);
                AddFormToAllForms(_frmReferencePoint);
                //
                _frmMaterial = new FrmMaterial(_controller);
                AddFormToAllForms(_frmMaterial);
                //
                _frmSection = new FrmSection(_controller);
                AddFormToAllForms(_frmSection);
                //
                _frmConstraint = new FrmConstraint(_controller);
                AddFormToAllForms(_frmConstraint);
                //
                _frmSurfaceInteraction = new FrmSurfaceInteraction(_controller);
                AddFormToAllForms(_frmSurfaceInteraction);
                //
                _frmContactPair = new FrmContactPair(_controller);
                AddFormToAllForms(_frmContactPair);
                //
                _frmStep = new FrmStep(_controller);
                AddFormToAllForms(_frmStep);
                //
                _frmHistoryOutput = new FrmHistoryOutput(_controller);
                AddFormToAllForms(_frmHistoryOutput);
                //
                _frmFieldOutput = new FrmFieldOutput(_controller);
                AddFormToAllForms(_frmFieldOutput);
                //
                _frmBoundaryCondition = new FrmBC(_controller);
                AddFormToAllForms(_frmBoundaryCondition);
                //
                _frmLoad = new FrmLoad(_controller);
                AddFormToAllForms(_frmLoad);
                //
                _frmAnalysis = new FrmAnalysis(_controller);
                AddFormToAllForms(_frmAnalysis);
                //
                _frmMonitor = new FrmMonitor(_controller);
                _frmMonitor.KillJob += KillAnalysis;
                _frmMonitor.Results += ResultsAnalysis;
                AddFormToAllForms(_frmMonitor);
                //
                _frmSettings = new FrmSettings();
                _frmSettings.UpdateSettings += UpdateSettings;
                AddFormToAllForms(_frmSettings);
                //
                _frmQuery = new FrmQuery();
                _frmQuery.Form_WriteDataToOutput = WriteDataToOutput;
                AddFormToAllForms(_frmQuery);
                //
                _frmAnimation = new FrmAnimation();
                _frmAnimation.Form_ControlsEnable = DisableEnableControlsForAnimation;
                AddFormToAllForms(_frmAnimation);
                //
                _frmHistoryResultsOutput = new FrmHistoryResultsOutput(_controller);
                AddFormToAllForms(_frmHistoryResultsOutput);
                //
                _frmTransformation = new FrmTransformation(_controller);
                AddFormToAllForms(_frmTransformation);
                //
                _vtk.Hide();
                _vtk.Enabled = false;
            }
            catch
            {
                // If no error the splash is closed latter
                splash.BeginInvoke((MethodInvoker)delegate () { splash.Close(); });
            }
            finally
            {
                this.TopMost = false;
            }
            //
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                tsmiTest.Visible = false;
                tsmiCropWithCylinder.Visible = false;
            }
        }
        //
        private void FrmMain_Shown(object sender, EventArgs e)
        {
            // Set vtk control size
            UpdateVtkControlSize();
            //
            _vtk.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            // Set pass through control for the mouse wheel event
            this.PassThroughControl = _vtk;
            // Timer to delay the rendering of the vtk so that menus get rendered first
            Timer timer = new Timer();
            timer.Interval = 50;
            timer.Tick += new EventHandler(async (object s, EventArgs ea) =>
            {
                timer.Stop();
                // Set form size
                _controller.Settings.General.ApplyFormSize(this);
                //
                _vtk.Show();
                _vtk.Enabled = true;
                // Close splash 
                splash.BeginInvoke((MethodInvoker)delegate () { splash.Close(); });
                // At the end when vtk is loaded open the file
                string fileName = null;
                if (_args != null && _args.Length == 1) fileName = _args[0];
                else
                {
                    // Check for open last file
                    if (_controller.Settings.General.OpenLastFile) fileName = _controller.OpenedFileName;
                }
                //
                if (File.Exists(fileName))
                {
                    try
                    {
                        SetStateWorking(Globals.OpeningText);
                        string extension = Path.GetExtension(fileName).ToLower();
                        if (extension == ".pmx" || extension == ".frd")
                            await Task.Run(() => Open(fileName));
                        else if (extension == ".stl" || extension == ".unv" || extension == ".vol" || extension == ".inp")
                        {
                            await _controller.ImportFileAsync(fileName);
                            _controller.OpenedFileName = null; // otherwise the previous OpenedFileName gets overwriten on Save
                        }
                        else MessageBox.Show("The file name extension is not supported.", "Error", MessageBoxButtons.OK);
                        //
                        _vtk.SetFrontBackView(false, true);
                    }
                    catch (Exception ex)
                    {                        
                        ExceptionTools.Show(this, ex);
                        _controller.New();
                    }
                    finally
                    {
                        SetStateReady(Globals.OpeningText);                        
                    }
                }
                else
                {
                    // New file
                    _controller.New();
                }
            });
            timer.Start();
        }
        private void FrmMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized && _frmAnimation.Visible) _frmAnimation.UpdateAnimation();            
        }
        private async void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                DialogResult response = DialogResult.None;
                // Save form size and location
                if (_controller != null)
                {
                    _controller.Settings.General.SaveFormSize(this);
                    _controller.Settings = _controller.Settings;    // to save the settings
                }
                //
                if (tsslState.Text != Globals.ReadyText)
                {
                    response = MessageBox.Show("There is a task running. Close anyway?", "Warning", MessageBoxButtons.YesNo);
                    if (response == DialogResult.No) e.Cancel = true;
                    else if (response == DialogResult.Yes && _controller.SavingFile)
                    {
                        while (_controller.SavingFile) System.Threading.Thread.Sleep(100);
                    }
                }
                else if (_controller.ModelChanged)
                {
                    response = MessageBox.Show("Save file before closing?", "Warning", MessageBoxButtons.YesNoCancel);
                    if (response == DialogResult.Yes)
                    {
                        e.Cancel = true;                                // stop the form from closing before saving
                        await Task.Run(() => _controller.Save());       // save                        
                        this.Close();                                   // close the control
                    }
                    else if (response == DialogResult.Cancel) e.Cancel = true;
                }
            }
            catch
            { }
        }
        private void FrmMain_Move(object sender, EventArgs e)
        {
            if (_allForms == null) return;

            foreach (Form form in _allForms)
            {
                if (form.Visible)
                {
                    //form.Location = new Point(Left + _formLocation.X, Top + _formLocation.Y);
                }
            }
        }
        private void UpdateVtkControlSize()
        {
            // Update vtk control size
            _vtk.Location = panelControl.Location;
            _vtk.Top += 1;
            _vtk.Left += 1;
            //
            _vtk.Size = panelControl.Size;
            _vtk.Width -= 2;
            _vtk.Height -= 2;
        }
        //
        private void itemForm_VisibleChanged(object sender, EventArgs e)
        {
            Form form = sender as Form;
            int count = 0;
            // One or two forms can be open
            foreach (var aForm in _allForms)
            {
                // Do not count the Query form
                if (aForm.Visible 
                    //&& !(aForm is FrmQuery)
                    ) count++;
            }
            // Disable model tree mouse and keyboard actions for the form
            bool unactive;
            if (count > 0) unactive = true;
            else unactive = false;
            //
            _modelTree.DisableMouse = unactive;
            menuStripMain.DisableMouseButtons = unactive;
            tsFile.DisableMouseButtons = unactive;
            // This gets also called from item selection form: by angle, by edge ...
            if (form.Visible == false)
            {
                UpdateHighlightFromTree();
                GetFormLoaction(form);
                //
                _controller.SetSelectByToDefault();
                //
                this.Focus();
            }
        }
        private void itemForm_Move(object sender, EventArgs e)
        {
            Form form = sender as Form;
            //Size screenSize =  Screen.GetWorkingArea(form).Size;
            //if (form.Left < 0) form.Left = 0;
            //else if (form.Left + form.Width > screenSize.Width) form.Left = screenSize.Width - form.Width;
            //if (form.Top < 0) form.Top = 0;
            //else if (form.Top + form.Height > screenSize.Height) form.Top = screenSize.Height - form.Height;
            GetFormLoaction(form);
        }
        //
        private void Vtk_KeyPressEvent(sbyte key)
        {
            if (key == 27) CloseAllForms(); // = Esc
        }
        //
        private void timerOutput_Tick(object sender, EventArgs e)
        {
            tbOutput.Lines = outputLines;
            tbOutput.SelectionStart = tbOutput.Text.Length;
            tbOutput.ScrollToCaret();
            timerOutput.Stop();
        }

        #region ModelTree Events ###################################################################################################
        //
        internal void ModelTree_ViewEvent(ViewType viewType)
        {
            try
            {
                if ((viewType == ViewType.Geometry && GetCurrentView() == ViewGeometryModelResults.Geometry) ||
                    (viewType == ViewType.Model && GetCurrentView() == ViewGeometryModelResults.Model) ||
                    (viewType == ViewType.Results && GetCurrentView() == ViewGeometryModelResults.Results)) return;
                //
                CloseAllForms();
                _controller.SelectBy = vtkSelectBy.Default;
                //
                if (viewType == ViewType.Geometry)_controller.CurrentView = ViewGeometryModelResults.Geometry;
                else if (viewType == ViewType.Model) _controller.CurrentView = ViewGeometryModelResults.Model;
                else if (viewType == ViewType.Results) _controller.CurrentView = ViewGeometryModelResults.Results;
                else throw new NotSupportedException();
                //
                if (_advisorControl != null) _advisorControl.PrepareControls(viewType);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void ModelTree_Select(NamedClass[] items)
        {
            try
            {
                _controller.Highlight3DObjects(items);
            }
            catch
            {
            }
        }       
        //
        private void ModelTree_CreateEvent(string nodeName, string stepName)
        {
            if (_controller.Model.Geometry != null && _controller.CurrentView == ViewGeometryModelResults.Geometry)
            {
                if (nodeName == "Mesh refinements") tsmiCreateMeshRefinement_Click(null, null);
            }
            else if (_controller.Model.Mesh != null && _controller.CurrentView == ViewGeometryModelResults.Model)
            {
                if (nodeName == "Node sets") tsmiCreateNodeSet_Click(null, null);
                else if (nodeName == "Element sets") tsmiCreateElementSet_Click(null, null);
                else if (nodeName == "Surfaces") tsmiCreateSurface_Click(null, null);
                else if (nodeName == "Reference points") tsmiCreateRP_Click(null, null);
                else if (nodeName == "Materials") tsmiCreateMaterial_Click(null, null);
                else if (nodeName == "Sections") tsmiCreateSection_Click(null, null);
                else if (nodeName == "Constraints") tsmiCreateConstraint_Click(null, null);
                else if (nodeName == "Surface interactions") tsmiCreateSurfaceInteraction_Click(null, null);
                else if (nodeName == "Contact pairs") tsmiCreateContactPair_Click(null, null);
                else if (nodeName == "Steps") tsmiCreateStep_Click(null, null);
                else if (nodeName == "History outputs" && stepName != null) CreateHistoryOutput(stepName);
                else if (nodeName == "Field outputs" && stepName != null) CreateFieldOutput(stepName);
                else if (nodeName == "BCs" && stepName != null) CreateBoundaryCondition(stepName);
                else if (nodeName == "Loads" && stepName != null) CreateLoad(stepName);
                else if (nodeName == "Analyses") tsmiCreateAnalysis_Click(null, null);
            }
        }
        private void ModelTree_Edit(NamedClass namedClass, string stepName)
        {
            // Geometry
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry)
            {
                if (namedClass is GeometryPart) EditGeometryPart(namedClass.Name);
                else if (namedClass is FeMeshRefinement) EditMeshRefinement(namedClass.Name);
            }
            // Model
            else if (_controller.CurrentView == ViewGeometryModelResults.Model)
            {
                if (namedClass is EmptyNamedClass) // empty named class is used to trasfer the name only
                {
                    if (namedClass.Name == typeof(FeModel).ToString()) tsmiEditModel_Click(null, null);
                }
                else if (namedClass is MeshPart) EditModelPart(namedClass.Name);
                else if (namedClass is FeNodeSet) EditNodeSet(namedClass.Name);
                else if (namedClass is FeElementSet) EditElementSet(namedClass.Name);
                else if (namedClass is FeSurface) EditSurface(namedClass.Name);
                else if (namedClass is FeReferencePoint) EditRP(namedClass.Name);
                else if (namedClass is Material) EditMaterial(namedClass.Name);
                else if (namedClass is Section) EditSection(namedClass.Name);
                else if (namedClass is CaeModel.Constraint) EditConstraint(namedClass.Name);
                else if (namedClass is SurfaceInteraction) EditSurfaceInteraction(namedClass.Name);
                else if (namedClass is ContactPair) EditContactPair(namedClass.Name);
                else if (namedClass is Step) EditStep(namedClass.Name);
                else if (namedClass is HistoryOutput) EditHistoryOutput(stepName, namedClass.Name);
                else if (namedClass is FieldOutput) EditFieldOutput(stepName, namedClass.Name);
                else if (namedClass is BoundaryCondition) EditBoundaryCondition(stepName, namedClass.Name);
                else if (namedClass is Load) EditLoad(stepName, namedClass.Name);
                else if (namedClass is AnalysisJob) EditAnalysis(namedClass.Name);
            }
            // Results
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                if (namedClass is ResultPart || namedClass is GeometryPart) EditResultPart(namedClass.Name);
                else if (namedClass is CaeResults.HistoryResultData hd) ShowHistoryOutput(hd);
            }
        }
        private void ModelTree_DuplicateEvent(NamedClass[] items, string[] stepNames)
        {
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry)
            {
            }
            else if (_controller.CurrentView == ViewGeometryModelResults.Model)
            {
                ApplyActionOnItems<FeNodeSet>(items, DuplicateNodeSets);
                ApplyActionOnItems<FeElementSet>(items, DuplicateElementSets);
                ApplyActionOnItems<Material>(items, DuplicateMaterials);
                ApplyActionOnItems<SurfaceInteraction>(items, DuplicateSurfaceInteractions);
                //
                ApplyActionOnItems<Step>(items, DuplicateSteps);
            }
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
            }
        }
        //
        private void ModelTree_HideShowEvent(NamedClass[] items, HideShowOperation operation, string[] stepNames)
        {
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry)
            {
                HideShowItems<GeometryPart>(items, operation, HideGeometryParts, ShowGeometryParts, ShowOnlyGeometryParts);
            }
            else if (_controller.CurrentView == ViewGeometryModelResults.Model)
            {
                HideShowItems<MeshPart>(items, operation, HideModelParts, ShowModelParts, ShowOnlyModelParts);
                HideShowItems<FeReferencePoint>(items, operation, HideRPs, ShowRPs, ShowOnlyRPs);
                HideShowItems<CaeModel.Constraint>(items, operation, HideConstraints, ShowConstraints, ShowOnlyConstraints);
                HideShowItems<ContactPair>(items, operation, HideContactPairs, ShowContactPairs, ShowOnlyContactPairs);
                HideShowStepItems<BoundaryCondition>(items, operation, stepNames, HideBoundaryConditions, 
                                                              ShowBoundaryConditions, ShowOnlyBoundaryConditions);
                HideShowStepItems<Load>(items, operation, stepNames, HideLoads, ShowLoads, ShowOnlyLoads);
            }
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                HideShowItems<ResultPart>(items, operation, HideResultParts, ShowResultParts, ShowOnlyResultParts);
                HideShowItems<GeometryPart>(items, operation, HideResultParts, ShowResultParts, ShowOnlyResultParts);
            }
        }
        private void ModelTree_SetTransparencyEvent(string[] partNames)
        {
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry) SetTransparencyForGeometryParts(partNames);
            else if (_controller.CurrentView == ViewGeometryModelResults.Model) SetTransparencyForModelParts(partNames);
            else if (_controller.CurrentView == ViewGeometryModelResults.Results) SetTransparencyForResultParts(partNames);
        }
        private void ModelTree_ColorContoursVisibilityEvent(NamedClass[] items, bool colorContours)
        {
            List<string> names = new List<string>();
            for (int i = 0; i < items.Length; i++)
            {
                names.Add(items[i].Name);
            }

            if (names.Count > 0)
            {
                if (colorContours) ColorContoursOnResultPart(names.ToArray());
                else ColorContoursOffResultPart(names.ToArray());
            }
        }
        //
        private void ModelTree_Delete(NamedClass[] items, string[] stepNames)
        {
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry)
            {
                ApplyActionOnItems<FeMeshRefinement>(items, DeleteMeshRefinements);
                // At last delete the parts
                ApplyActionOnItems<GeometryPart>(items, DeleteGeometryParts);
            }
            else if (_controller.CurrentView == ViewGeometryModelResults.Model)
            {
                ApplyActionOnItems<FeNodeSet>(items, DeleteNodeSets);
                ApplyActionOnItems<FeElementSet>(items, DeleteElementSets);
                ApplyActionOnItems<FeSurface>(items, DeleteSurfaces);
                ApplyActionOnItems<FeReferencePoint>(items, DeleteRPs);
                ApplyActionOnItems<Material>(items, DeleteMaterials);
                ApplyActionOnItems<Section>(items, DeleteSections);
                ApplyActionOnItems<CaeModel.Constraint>(items, DeleteConstraints);
                ApplyActionOnItems<SurfaceInteraction>(items, DeleteSurfaceInteractions);
                ApplyActionOnItems<ContactPair>(items, DeleteContactPairs);
                //
                DeleteStepItems<HistoryOutput>(items, stepNames, DeleteHistoryOutputs);
                DeleteStepItems<FieldOutput>(items, stepNames, DeleteFieldOutputs);
                DeleteStepItems<BoundaryCondition>(items, stepNames, DeleteBoundaryConditions);
                DeleteStepItems<Load>(items, stepNames, DeleteLoads);
                ApplyActionOnItems<Step>(items, DeleteSteps);
                //
                ApplyActionOnItems<AnalysisJob>(items, DeleteAnalyses);
                // At last delete the parts
                ApplyActionOnItems<MeshPart>(items, DeleteModelParts);
            }
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                ApplyActionOnItems<ResultPart>(items, DeleteResultParts);
                ApplyActionOnItems<GeometryPart>(items, DeleteResultParts);
            }
            //
            ClearSelection();
        }
        private void ModelTree_ActivateDeactivateEvent(NamedClass[] items, bool activate, string[] stepNames)
        {
            _controller.ActivateDeactivateMultipleCommand(items, activate, stepNames);
        }
        private void ModelTree_FieldDataSelectEvent(string[] obj)
        {
            try
            {
                SetFieldData(obj[0], obj[1], GetCurrentFieldOutputStepId(), GetCurrentFieldOutputStepIncrementId());
            }
            catch { }
        }
        //                                                                                                                          
        private void HideShowItems<T>(NamedClass[] items, HideShowOperation operation, Action<string[]> Hide, 
                                      Action<string[]> Show, Action<string[]> ShowOnly)
        {
            List<string> names = new List<string>();
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] is T) names.Add(items[i].Name);
            }
            if (names.Count > 0)
            {
                if (operation == HideShowOperation.Hide) Hide(names.ToArray());
                else if (operation == HideShowOperation.Show) Show(names.ToArray());
                else if (operation == HideShowOperation.ShowOnly) ShowOnly(names.ToArray());
                else throw new NotSupportedException();
            }
        }
        private void HideShowStepItems<T>(NamedClass[] items, HideShowOperation operation, string[] stepNames, 
                                          Action<string, string[]> Hide,
                                          Action<string, string[]> Show, 
                                          Action<string, string[]> ShowOnly)
        {
            Dictionary<string, List<string>> stepItems = new Dictionary<string, List<string>>();
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] is T)
                {
                    if (stepItems.ContainsKey(stepNames[i])) stepItems[stepNames[i]].Add(items[i].Name);
                    else stepItems.Add(stepNames[i], new List<string>() { items[i].Name });
                }
            }
            if (stepItems.Count > 0)
            {
                foreach (var entry in stepItems)
                {
                    if (operation == HideShowOperation.Hide) Hide(entry.Key, entry.Value.ToArray());
                    else if (operation == HideShowOperation.Show) Show(entry.Key, entry.Value.ToArray());
                    else if (operation == HideShowOperation.ShowOnly) ShowOnly(entry.Key, entry.Value.ToArray());
                    else throw new NotSupportedException();
                }
            }
        }
        private void ApplyActionOnItems<T>(NamedClass[] items, Action<string[]> Delete)
        {
            List<string> names = new List<string>();
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] is T) names.Add(items[i].Name);
            }
            if (names.Count > 0) Delete(names.ToArray());
        }
        private void DeleteStepItems<T>(NamedClass[] items, string[] stepNames, Action<string, string[]> Delete)
        {
            Dictionary<string, List<string>> stepItems = new Dictionary<string, List<string>>();
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] is T)
                {
                    if (stepItems.ContainsKey(stepNames[i])) stepItems[stepNames[i]].Add(items[i].Name);
                    else stepItems.Add(stepNames[i], new List<string>() { items[i].Name });
                }
            }
            if (stepItems.Count > 0)
            {
                foreach (var entry in stepItems)
                {
                    Delete(entry.Key, entry.Value.ToArray());
                }
            }
        }
        //                                                                                                                          
        
        #endregion #################################################################################################################


        // Menus                                                                                                                    
        private void SetMenuAndToolStripVisibility()
        {
            switch (_controller.CurrentView)
            {
                case ViewGeometryModelResults.Geometry:
                    tsmiGeometry.Enabled = true;
                    tsmiMesh.Enabled = true;
                    tsmiModel.Enabled = false;
                    tsmiProperty.Enabled = false;
                    tsmiInteraction.Enabled = false;
                    tsmiStepMenu.Enabled = false;
                    tsmiBCs.Enabled = false;
                    tsmiLoad.Enabled = false;
                    tsmiAnalysis.Enabled = false;
                    tsmiResults.Enabled = false;

                    tsmiDividerView4.Visible = false;
                    tsmiResultsUndeformed.Visible = false;
                    tsmiResultsDeformed.Visible = false;
                    tsmiResultsColorContours.Visible = false;

                    // toolStrip
                    toolStripViewSeparator4.Visible = false;
                    tslSymbols.Visible = false;
                    tscbSymbolsForStep.Visible = false;

                    tsResults.Visible = false;
                    break;
                case ViewGeometryModelResults.Model:
                    tsmiGeometry.Enabled = false;
                    tsmiMesh.Enabled = false;
                    tsmiModel.Enabled = true;
                    tsmiProperty.Enabled = true;
                    tsmiInteraction.Enabled = true;
                    tsmiStepMenu.Enabled = true;
                    tsmiBCs.Enabled = true;
                    tsmiLoad.Enabled = true;
                    tsmiAnalysis.Enabled = true;
                    tsmiResults.Enabled = false;

                    tsmiDividerView4.Visible = false;
                    tsmiResultsUndeformed.Visible = false;
                    tsmiResultsDeformed.Visible = false;
                    tsmiResultsColorContours.Visible = false;

                    // toolStrip
                    toolStripViewSeparator4.Visible = true;
                    tslSymbols.Visible = true;
                    tscbSymbolsForStep.Visible = true;

                    tsResults.Visible = false;
                    break;
                case ViewGeometryModelResults.Results:
                    tsmiGeometry.Enabled = false;
                    tsmiMesh.Enabled = false;
                    tsmiModel.Enabled = false;
                    tsmiProperty.Enabled = false;
                    tsmiInteraction.Enabled = false;
                    tsmiStepMenu.Enabled = false;
                    tsmiBCs.Enabled = false;
                    tsmiLoad.Enabled = false;
                    tsmiAnalysis.Enabled = false;
                    tsmiResults.Enabled = true;

                    tsmiDividerView4.Visible = true;
                    tsmiResultsUndeformed.Visible = true;
                    tsmiResultsDeformed.Visible = true;
                    tsmiResultsColorContours.Visible = true;

                    // toolStrip
                    toolStripViewSeparator4.Visible = false;
                    tslSymbols.Visible = false;
                    tscbSymbolsForStep.Visible = false;

                    tsResults.Visible = true;
                    break;
                default:
                    break;
            }
        }

        #region File menu ##########################################################################################################

        internal void tsmiNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.ModelChanged && MessageBox.Show("OK to close current model?", Globals.ProgramName,
                    MessageBoxButtons.OKCancel) != DialogResult.OK) return;
                //
                _controller.New();
                //
                SelectModelUnitSystem();
                // No need for ModelChanged
                _controller.ModelChanged = false;
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiOpen_Click(object sender, EventArgs e)
        {
            try
            {                
                using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
                {
                    if (!System.Diagnostics.Debugger.IsAttached)
                    {
                        openFileDialog.Filter = "All files|*.pmx;*.frd;*.dat" +
                                                "|PrePoMax files|*.pmx" +
                                                "|Calculix result files|*.frd" +
                                                "|Calculix dat files|*.dat";
                    }
                    else
                    {
                        openFileDialog.Filter = "All files|*.pmx;*.frd;*.dat" +
                                                "|PrePoMax files|*.pmx" +
                                                "|Calculix result files|*.frd";
                    }
                    //
                    openFileDialog.FileName = "";
                    if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        if (CheckBeforeOpen(openFileDialog.FileName)) OpenAsync(openFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
                _controller.New();
            }
        }
        private bool CheckBeforeOpen(string fileName)
        {
            if (!File.Exists(fileName)) return false;
            //
            if (_controller.ModelChanged)
            {
                if (Path.GetExtension(fileName).ToLower() == ".pmx")
                {
                    if (MessageBox.Show("OK to close current model?",
                        Globals.ProgramName,
                        MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK) return false;
                }
                else if (Path.GetExtension(fileName).ToLower() == ".frd" && _controller.Results != null)
                {
                    if (MessageBox.Show("OK to overwrite current results?",
                        Globals.ProgramName,
                        MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK) return false;
                }
            }
            return true;
        }
        private async void OpenAsync(string fileName, bool resetCamera = true, Action callback = null)
        {
            bool stateSet = false;
            try
            {
                if (SetStateWorking(Globals.OpeningText))
                {
                    stateSet = true;
                    await Task.Run(() => Open(fileName, resetCamera));
                    callback?.Invoke();
                }
                else MessageBox.Show("Another task is already running.");
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                if (stateSet) SetStateReady(Globals.OpeningText);
            }
        }
        private void Open(string fileName, bool resetCamera = true)
        {
            _controller.Open(fileName);
            //
            if (_controller.Results != null)
            {
                // Reset the previous step and increment
                SetAllStepAndIncrementIds();
                // Set last increment
                SetDefaultStepAndIncrementIds();
                // Show the selection in the results tree
                InvokeIfRequired(_modelTree.SelectFirstComponentOfFirstFieldOutput);
            }
            //
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry) _controller.DrawGeometry(resetCamera);
            else if (_controller.CurrentView == ViewGeometryModelResults.Model) _controller.DrawModel(resetCamera);
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                // Set the representation which also calls Draw
                _controller.ViewResultsType = ViewResultsType.ColorContours;  // Draw
                //
                if (resetCamera) tsmiFrontView_Click(null, null);
            }
            else throw new NotSupportedException();
        }
        internal async void tsmiImportFile_Click(object sender, EventArgs e)
        {
            try
            {
                string[] files = GetFileNamesToImport();
                if (files != null && files.Length > 0)
                {
                    SetStateWorking(Globals.ImportingText);
                    foreach (var file in files)
                    {
                        await _controller.ImportFileAsync(file);
                    }
                    SetFrontBackView(true, true);   // animate must be true in order for the scale bar to work correctly
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.ImportingText);
            }
        }
        internal async void tsmiSave_Click(object sender, EventArgs e)
        {
            try
            {
                SetStateWorking(Globals.SavingText);
                await Task.Run(() => _controller.Save());
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.SavingText);
            }
        }
        private async void tsmiSaveAs_Click(object sender, EventArgs e)
        {
            try
            {
                SetStateWorking(Globals.SavingAsText);
                await Task.Run(() => _controller.SaveAs());
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.SavingAsText);
            }
        }
        //
        private void tsmiExportToCalculix_Click(object sender, EventArgs e)
        {
            try
            {
                if (CheckValiditiy())
                {
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "Calculix files | *.inp";
                        if (_controller.OpenedFileName != null)
                            saveFileDialog.FileName = Path.GetFileNameWithoutExtension(_controller.OpenedFileName) + ".inp";
                        if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            // the filter adds the extension to the file name
                            SetStateWorking(Globals.ExportingText);
                            _controller.ExportToCalculix(saveFileDialog.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.ExportingText);
            }
        }
        private void tsmiExportToAbaqus_Click(object sender, EventArgs e)
        {
            try
            {
                if (CheckValiditiy())
                {
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "Abaqus files | *.inp";
                        if (_controller.OpenedFileName != null)
                            saveFileDialog.FileName = Path.GetFileNameWithoutExtension(_controller.OpenedFileName) + ".inp";
                        if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            // the filter adds the extension to the file name
                            SetStateWorking(Globals.ExportingText);
                            //_controller.ExportToCalculix(saveFileDialog.FileName);
                            _controller.ExportToAbaqus(saveFileDialog.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.ExportingText);
            }
        }
        private void tsmiExportToStep_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Geometry != null && _controller.Model.Geometry.Parts != null)
                {
                    SelectMultipleEntities("Parts", _controller.GetCADGeometryParts(), SaveCADPartsAsStep);
                }
                else throw new CaeException("No geometry to export.");
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiExportToBrep_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Geometry != null && _controller.Model.Geometry.Parts != null)
                {
                    SelectMultipleEntities("Parts", _controller.GetCADGeometryParts(), SaveCADPartsAsBrep);
                }
                else throw new CaeException("No geometry to export.");
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiExportToMmgMesh_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Geometry != null && _controller.Model.Geometry.Parts != null)
                {
                    SelectMultipleEntities("Parts", _controller.GetGeometryParts(), SavePartsAsMmgMesh);
                }
                else throw new CaeException("No geometry to export.");
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiExportToStereolitography_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Geometry != null && _controller.Model.Geometry.Parts != null)
                {
                    SelectMultipleEntities("Parts", _controller.GetGeometryParts(), SavePartsAsStl);
                }
                else throw new CaeException("No geometry to export.");
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private async void SaveCADPartsAsStep(string[] partNames)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Step files | *.stp";
                    if (_controller.OpenedFileName != null)
                        saveFileDialog.FileName = Path.GetFileNameWithoutExtension(_controller.OpenedFileName) + ".stp";
                    //
                    if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        // The filter adds the extension to the file name
                        SetStateWorking(Globals.ExportingText);
                        //
                        await Task.Run(() => _controller.ExportCADGeometryPartsAsStep(partNames, saveFileDialog.FileName));
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.ExportingText);
            }
        }
        private async void SaveCADPartsAsBrep(string[] partNames)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Brep files | *.brep";
                    if (_controller.OpenedFileName != null)
                        saveFileDialog.FileName = Path.GetFileNameWithoutExtension(_controller.OpenedFileName) + ".brep";
                    //
                    if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        // The filter adds the extension to the file name
                        SetStateWorking(Globals.ExportingText);
                        //
                        await Task.Run(() => _controller.ExportCADGeometryPartsAsBrep(partNames, saveFileDialog.FileName));
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.ExportingText);
            }
        }
        private async void SavePartsAsMmgMesh(string[] partNames)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Mmg files | *.mesh";
                    if (_controller.OpenedFileName != null)
                        saveFileDialog.FileName = Path.GetFileNameWithoutExtension(_controller.OpenedFileName) + ".mesh";
                    //
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // The filter adds the extension to the file name
                        SetStateWorking(Globals.ExportingText);
                        //
                        await Task.Run(() => _controller.ExportGeometryPartsAsMmgMesh(partNames, saveFileDialog.FileName));
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.ExportingText);
            }
        }
        private async void SavePartsAsStl(string[] partNames)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Stereolitography files | *.stl";
                    if (_controller.OpenedFileName != null)
                        saveFileDialog.FileName = Path.GetFileNameWithoutExtension(_controller.OpenedFileName) + ".stl";
                    //
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // The filter adds the extension to the file name
                        SetStateWorking(Globals.ExportingText);
                        //
                        await Task.Run(() => _controller.ExportGeometryPartsAsStl(partNames, saveFileDialog.FileName));
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.ExportingText);
            }
        }
        //
        private void tsmiCloseResults_Click(object sender, EventArgs e)
        {
            _controller.ClearResults();

            ClearResults();
            if (_controller.CurrentView == ViewGeometryModelResults.Results) Clear3D();

            _modelTree.ClearResults();
        }
        private void tsmiExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //Recent
        public void UpdateRecentFilesThreadSafe(string[] fileNames)
        {
            InvokeIfRequired(UpdateRecentFiles, fileNames);
        }
        public void UpdateRecentFiles(string[] fileNames)
        {
            try
            {
                if (fileNames != null)
                {
                    tsmiOpenRecent.DropDownItems.Clear();
                    //
                    ToolStripMenuItem menuItem;
                    foreach (var fileName in fileNames)
                    {
                        menuItem = new ToolStripMenuItem(fileName);
                        menuItem.Click += tsmiRecentFile_Click;
                        tsmiOpenRecent.DropDownItems.Add(menuItem);
                    }
                    if (fileNames.Length > 0)
                    {
                        ToolStripSeparator separator = new ToolStripSeparator();
                        tsmiOpenRecent.DropDownItems.Add(separator);
                    }
                    menuItem = new ToolStripMenuItem("Clear Recent Files");
                    menuItem.Click += tsmiClearRecentFiles_Click;
                    tsmiOpenRecent.DropDownItems.Add(menuItem);
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiRecentFile_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = ((ToolStripMenuItem)sender).Text;
                if (CheckBeforeOpen(fileName)) OpenAsync(fileName);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiClearRecentFiles_Click(object sender, EventArgs e)
        {
            try
            {
                _controller.ClearRecentFiles();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }

        #endregion  ################################################################################################################

        #region Edit menu ##########################################################################################################

        private async void tsmiUndo_Click(object sender, EventArgs e)
        {
            try
            {
                SetStateWorking(Globals.UndoingText);
                _modelTree.ScreenUpdating = false;

                await Task.Run(() => _controller.UndoHistory());
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.UndoingText);
                _modelTree.ScreenUpdating = true;
                _modelTree.RegenerateTree(_controller.Model, _controller.Jobs, _controller.Results, _controller.History);
            }
        }
        private void tsmiRedo_Click(object sender, EventArgs e)
        {
            try
            {
                _controller.RedoHistory();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiViewHistory_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(_controller.GetHistoryFileName());
        }
        private async void tsmiRegenerate_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllForms();
                Clear3D();
                Application.DoEvents();
                SetStateWorking(Globals.RegeneratingText);
                _modelTree.ScreenUpdating = false;
                await Task.Run(() => _controller.RegenerateHistoryCommands());
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.RegeneratingText);
                _modelTree.ScreenUpdating = true;
                _modelTree.RegenerateTree(_controller.Model, _controller.Jobs, _controller.Results, _controller.History);
            }
        }
        private void tsmiRegenerteUsingOtherFiles_Click(object sender, EventArgs e)
        {
            RegenerateWithDialogs(true, false);
        }
        private void tsmiRegenerateForRemeshing_Click(object sender, EventArgs e)
        {
            RegenerateWithDialogs(false, true);
        }        
        private async void RegenerateWithDialogs(bool showImportDialog, bool showMeshParametersDialog)
        {
            try
            {
                CloseAllForms();
                Clear3D();
                Application.DoEvents();
                SetStateWorking(Globals.RegeneratingText);
                _modelTree.ScreenUpdating = false;
                await Task.Run(() => _controller.RegenerateHistoryCommandsWithDialogs(showImportDialog, showMeshParametersDialog));
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.RegeneratingText);
                _modelTree.ScreenUpdating = true;
                _modelTree.RegenerateTree(_controller.Model, _controller.Jobs, _controller.Results, _controller.History);
            }
        }

        public void EnableDisableUndoRedo(string undo, string redo)
        {
            InvokeIfRequired(EnableDisable, undo, redo);
        }
        private void EnableDisable(string undo, string redo)
        {
            if (undo == null)
            {
                tsmiUndo.Text = "Undo";
                tsmiUndo.Enabled = false;
            }
            else
            {
                tsmiUndo.Text = "Undo - " + undo;
                tsmiUndo.Enabled = true;
            }

            if (redo == null)
            {
                tsmiRedo.Text = "Redo";
                tsmiRedo.Enabled = false;
            }
            else
            {
                tsmiRedo.Text = "Redo - " + redo;
                tsmiRedo.Enabled = true;
            }
        }

        #endregion  ################################################################################################################

        #region View menu ##########################################################################################################

        private void tsmiFrontView_Click(object sender, EventArgs e)
        {
            _vtk.SetFrontBackView(true, true);
        }
        private void tsmiBackView_Click(object sender, EventArgs e)
        {
            _vtk.SetFrontBackView(true, false);
        }
        private void tsmiTopView_Click(object sender, EventArgs e)
        {
            _vtk.SetTopBottomView(true, true);
        }
        private void tsmiBottomView_Click(object sender, EventArgs e)
        {
            _vtk.SetTopBottomView(true, false);
        }
        private void tsmiLeftView_Click(object sender, EventArgs e)
        {
            _vtk.SetLeftRightView(true, true);
        }
        private void tsmiRightView_Click(object sender, EventArgs e)
        {
            _vtk.SetLeftRightView(true, false);
        }
        //
        private void tsmiNormalView_Click(object sender, EventArgs e)
        {
            _vtk.SetNormalView(true);
        }
        private void tsmiVerticalView_Click(object sender, EventArgs e)
        {
            _vtk.SetVerticalView(true, true);
        }
        //
        private void tsmiIsometricView_Click(object sender, EventArgs e)
        {
            _vtk.SetIsometricView(true, true);
        }
        private void tsmiZoomToFit_Click(object sender, EventArgs e)
        {
            SetZoomToFit(true);
        }
        //
        private void tsmiShowWireframeEdges_Click(object sender, EventArgs e)
        {
            try
            {
                _edgeVisibilities[_controller.CurrentView] = tsmiShowWireframeEdges_Click;
                //
                SetEdgesVisibility(vtkControl.vtkEdgesVisibility.Wireframe);
            }
            catch { }
        }
        private void tsmiShowElementEdges_Click(object sender, EventArgs e)
        {
            try
            {
                _edgeVisibilities[_controller.CurrentView] = tsmiShowElementEdges_Click;
                //
                SetEdgesVisibility(vtkControl.vtkEdgesVisibility.ElementEdges);
            }
            catch { }
        }
        private void tsmiShowModelEdges_Click(object sender, EventArgs e)
        {
            try
            {
                _edgeVisibilities[_controller.CurrentView] = tsmiShowModelEdges_Click;
                //
                SetEdgesVisibility(vtkControl.vtkEdgesVisibility.ModelEdges);
            }
            catch { }
        }
        private void tsmiShowNoEdges_Click(object sender, EventArgs e)
        {
            try
            {
                _edgeVisibilities[_controller.CurrentView] = tsmiShowNoEdges_Click;
                //
                SetEdgesVisibility(vtkControl.vtkEdgesVisibility.NoEdges);
            }
            catch { }
        }
        private void SetEdgesVisibility(vtkControl.vtkEdgesVisibility edgesVisibility)
        {
            _vtk.EdgesVisibility = edgesVisibility;
            //
            UpdateHighlight();

            //if (_controller.Selection != null && _controller.Selection.Nodes.Count > 0)
            //    _controller.HighlightSelection();
            //else if (_frmSelectItemSet != null && !_frmSelectItemSet.Visible)   // null for the initiation
            //    // if everything is deselectd in _frmSelectItemSet do not highlight from tree
            //    _modelTree.UpdateHighlight();
        }
        //
        private void tsmiSectionView_Click(object sender, EventArgs e)
        {
            SinglePointDataEditor.ParentForm = _frmSectionView;
            SinglePointDataEditor.Controller = _controller;
            //
            ShowForm(_frmSectionView, tsmiSectionView.Text, null);
        }
        // Hide/Show
        private void tsmiHideAllParts_Click(object sender, EventArgs e)
        {
            try
            {
                string[] partNames;
                if (_controller.CurrentView == ViewGeometryModelResults.Geometry)
                {
                    partNames = _controller.GetGeometryPartNames();
                    _controller.HideGeometryPartsCommand(partNames);
                }
                else if (_controller.CurrentView == ViewGeometryModelResults.Model)
                {
                    partNames = _controller.GetModelPartNames();
                    _controller.HideModelPartsCommand(partNames);
                }
                else if (_controller.CurrentView == ViewGeometryModelResults.Results)
                {
                    partNames = _controller.GetResultPartNames();
                    _controller.HideResultParts(partNames);
                }
            }
            catch { }
        }
        private void tsmiShowAllParts_Click(object sender, EventArgs e)
        {
            try
            {
                string[] partNames;
                if (_controller.CurrentView == ViewGeometryModelResults.Geometry)
                {
                    partNames = _controller.GetGeometryPartNames();
                    _controller.ShowGeometryPartsCommand(partNames);
                }
                else if (_controller.CurrentView == ViewGeometryModelResults.Model)
                {
                    partNames = _controller.GetModelPartNames();
                    _controller.ShowModelPartsCommand(partNames);
                }
                else if (_controller.CurrentView == ViewGeometryModelResults.Results)
                {
                    partNames = _controller.GetResultPartNames();
                    _controller.ShowResultParts(partNames);
                }
            }
            catch { }
        }
        private void tsmiInvertVisibleParts_Click(object sender, EventArgs e)
        {
            try
            {
                BasePart[] parts;
                List<string> partNamesToHide = new List<string>();
                List<string> partNamesToShow = new List<string>();

                if (_controller.CurrentView == ViewGeometryModelResults.Geometry)
                {
                    parts = _controller.GetGeometryParts();
                    foreach (var part in parts)
                    {
                        if (part.Visible) partNamesToHide.Add(part.Name);
                        else partNamesToShow.Add(part.Name);
                    }
                    if (partNamesToHide.Count > 0) _controller.HideGeometryPartsCommand(partNamesToHide.ToArray());
                    if (partNamesToShow.Count > 0) _controller.ShowGeometryPartsCommand(partNamesToShow.ToArray());
                }
                else if (_controller.CurrentView == ViewGeometryModelResults.Model)
                {
                    parts = _controller.GetModelParts();
                    foreach (var part in parts)
                    {
                        if (part.Visible) partNamesToHide.Add(part.Name);
                        else partNamesToShow.Add(part.Name);
                    }
                    if (partNamesToHide.Count > 0) _controller.HideModelPartsCommand(partNamesToHide.ToArray());
                    if (partNamesToShow.Count > 0) _controller.ShowModelPartsCommand(partNamesToShow.ToArray());
                }
                else if (_controller.CurrentView == ViewGeometryModelResults.Results)
                {
                    parts = _controller.GetResultParts();
                    foreach (var part in parts)
                    {
                        if (part.Visible) partNamesToHide.Add(part.Name);
                        else partNamesToShow.Add(part.Name);
                    }
                    if (partNamesToHide.Count > 0) _controller.HideResultParts(partNamesToHide.ToArray());
                    if (partNamesToShow.Count > 0) _controller.ShowResultParts(partNamesToShow.ToArray());
                }
            }
            catch { }
        }
        // Annotate
        private void DropDown_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                e.Cancel = true;
        }
        private void tsmiAnnotateFaceOrientations_Click(object sender, EventArgs e)
        {
            _controller.AnnotateWithColor = ChangeAnnotationStatus(sender);
        }
        private void tsmiAnnotateParts_Click(object sender, EventArgs e)
        {
            _controller.AnnotateWithColor = ChangeAnnotationStatus(sender);
        }
        private void tsmiAnnotateMaterials_Click(object sender, EventArgs e)
        {
            _controller.AnnotateWithColor = ChangeAnnotationStatus(sender);
        }
        private void tsmiAnnotateSections_Click(object sender, EventArgs e)
        {
            _controller.AnnotateWithColor = ChangeAnnotationStatus(sender);
        }
        private void tsmiAnnotateSectionThicknesses_Click(object sender, EventArgs e)
        {
            _controller.AnnotateWithColor = ChangeAnnotationStatus(sender);
        }
        private void tsmiAnnotateAllSymbols_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem tsmi)
            {
                // Turn off
                if (tsmi.Checked)
                {
                    ClearAnnotationStatus();  // first check and then clear
                    _controller.AnnotateWithColor = AnnotateWithColorEnum.None;
                }
                // Turn on
                else
                {
                    ClearAnnotationStatus();
                    //
                    tsmi.Checked = true;
                    tsmiAnnotateReferencePoints.Checked = true;
                    tsmiAnnotateConstraints.Checked = true;
                    tsmiAnnotateContactPairs.Checked = true;
                    tsmiAnnotateBCs.Checked = true;
                    tsmiAnnotateLoads.Checked = true;
                    //
                    _controller.AnnotateWithColor = AnnotateWithColorEnum.ReferencePoints |
                                                    AnnotateWithColorEnum.Constraints |
                                                    AnnotateWithColorEnum.ContactPairs |
                                                    AnnotateWithColorEnum.BoundaryConditions |
                                                    AnnotateWithColorEnum.Loads;
                }
            }
        }
        private void tsmiAnnotateReferencePoints_Click(object sender, EventArgs e)
        {
            _controller.AnnotateWithColor = ChangeAnnotationStatus(sender);
        }
        private void tsmiAnnotateConstraints_Click(object sender, EventArgs e)
        {
            _controller.AnnotateWithColor = ChangeAnnotationStatus(sender);
        }
        private void tsmiAnnotateContactPairs_Click(object sender, EventArgs e)
        {
            _controller.AnnotateWithColor = ChangeAnnotationStatus(sender);
        }
        private void tsmiAnnotateBCs_Click(object sender, EventArgs e)
        {
            _controller.AnnotateWithColor = ChangeAnnotationStatus(sender);
        }
        private void tsmiAnnotateLoads_Click(object sender, EventArgs e)
        {
            _controller.AnnotateWithColor = ChangeAnnotationStatus(sender);
        }
        //
        private AnnotateWithColorEnum ChangeAnnotationStatus(object sender)
        {
            if (sender is ToolStripMenuItem tsmi)
            {
                // Turn off
                if (tsmi.Checked) tsmi.Checked = false;
                // Turn on
                else
                {
                    // Only one possibility - Face orientations or parts
                    if (tsmi == tsmiAnnotateFaceOrientations || tsmi == tsmiAnnotateParts ||
                        tsmi == tsmiAnnotateMaterials || tsmi == tsmiAnnotateSections ||  
                        tsmi == tsmiAnnotateSectionThicknesses)
                    {
                        ClearAnnotationStatus();
                        tsmi.Checked = true;
                    }
                    // Symbols
                    else
                    {
                        tsmiAnnotateFaceOrientations.Checked = false;
                        tsmiAnnotateParts.Checked = false;
                        tsmiAnnotateMaterials.Checked = false;
                        tsmiAnnotateSections.Checked = false;
                        tsmiAnnotateSectionThicknesses.Checked = false;
                        //
                        tsmi.Checked = true;
                    }
                }
                //
                if (tsmiAnnotateFaceOrientations.Checked) return AnnotateWithColorEnum.FaceOrientation;
                else if (tsmiAnnotateParts.Checked) return AnnotateWithColorEnum.Parts;
                else if (tsmiAnnotateMaterials.Checked) return AnnotateWithColorEnum.Materials;
                else if (tsmiAnnotateSections.Checked) return AnnotateWithColorEnum.Sections;
                else if (tsmiAnnotateSectionThicknesses.Checked) return AnnotateWithColorEnum.SectionThicknesses;
                else
                {
                    AnnotateWithColorEnum status = AnnotateWithColorEnum.None;
                    if (tsmiAnnotateReferencePoints.Checked) status |= AnnotateWithColorEnum.ReferencePoints;
                    if (tsmiAnnotateConstraints.Checked) status |= AnnotateWithColorEnum.Constraints;
                    if (tsmiAnnotateContactPairs.Checked) status |= AnnotateWithColorEnum.ContactPairs;
                    if (tsmiAnnotateBCs.Checked) status |= AnnotateWithColorEnum.BoundaryConditions;
                    if (tsmiAnnotateLoads.Checked) status |= AnnotateWithColorEnum.Loads;
                    //
                    tsmiAnnotateAllSymbols.Checked = status.HasFlag(AnnotateWithColorEnum.ReferencePoints |
                                                                    AnnotateWithColorEnum.Constraints |
                                                                    AnnotateWithColorEnum.ContactPairs |
                                                                    AnnotateWithColorEnum.BoundaryConditions |
                                                                    AnnotateWithColorEnum.Loads);
                    //
                    return status;
                }
            }
            return AnnotateWithColorEnum.None;
        }
        //
        private void tsmiResultsUndeformed_Click(object sender, EventArgs e)
        {
            if (_frmAnimation.Visible) _frmAnimation.Hide();
            _controller.ViewResultsType = ViewResultsType.Undeformed;
        }
        private void tsmiResultsDeformed_Click(object sender, EventArgs e)
        {
            if (_frmAnimation.Visible) _frmAnimation.Hide();
            _controller.ViewResultsType = ViewResultsType.Deformed;
        }
        private void tsmiResultsColorContours_Click(object sender, EventArgs e)
        {
            if (_frmAnimation.Visible) _frmAnimation.Hide();
            _controller.ViewResultsType = ViewResultsType.ColorContours;
        }

        #endregion

        #region Geometry ###########################################################################################################
        
        //Geometry part
        internal void tsmiEditGeometryPart_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Parts", _controller.GetGeometryParts(), EditGeometryPart);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiHideGeometryParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryParts(), HideGeometryParts);
                Clear3DSelection();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiShowGeometryParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryParts(), ShowGeometryParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiShowOnlyGeometryParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryParts(), ShowOnlyGeometryParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiSetTransparencyForGeometryParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryParts(), SetTransparencyForGeometryParts);
                Clear3DSelection();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiCopyGeometryPartsToResults_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryParts(), CopyGeometryPartsToResults);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteGeometryParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryPartsWithoutSubParts(), DeleteGeometryParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void EditGeometryPart(string partName)
        {
            _frmPartProperties.View = ViewGeometryModelResults.Geometry;
            ShowForm(_frmPartProperties, "Edit Part", partName);
        }
        private void HideGeometryParts(string[] partNames)
        {
            _controller.HideGeometryPartsCommand(partNames);            
        }
        private void ShowGeometryParts(string[] partNames)
        {
            _controller.ShowGeometryPartsCommand(partNames);
        }
        private void SetTransparencyForGeometryParts(string[] partNames)
        {
            if (_controller.Model.Geometry == null) return;
            //
            using (FrmGetValue frmGetValue = new FrmGetValue())
            {
                frmGetValue.NumOfDigits = 0;
                frmGetValue.MinValue = 25;
                frmGetValue.MaxValue = 255;
                SetFormLoaction(frmGetValue);
                OrderedDictionary<string, double> presetValues = new OrderedDictionary<string, double>();
                presetValues.Add("Semi-transparent", 128);
                presetValues.Add("Opaque", 255);
                string desc = "Enter the transparency between 0 and 255.\n" + "(0 - transparent; 255 - opaque)";
                frmGetValue.PrepareForm("Set Transparency: " + partNames.ToShortString(), "Transparency",  desc, 128, presetValues);
                if (frmGetValue.ShowDialog() == DialogResult.OK)
                {
                    _controller.SetTransparencyForGeometryPartsCommand(partNames,(byte)frmGetValue.Value);
                }
                GetFormLoaction(frmGetValue);
            }
        }
        private void ShowOnlyGeometryParts(string[] partNames)
        {
            // If sub part is selected add the whole compound part
            HashSet<string> partsToShow = new HashSet<string>(partNames);
            foreach (var entry in _controller.Model.Geometry.Parts)
            {
                if (entry.Value is CompoundGeometryPart cgp)
                {
                    if (partNames.Contains(cgp.Name))
                    {
                        partsToShow.Add(cgp.Name);
                        partsToShow.UnionWith(cgp.SubPartNames);
                    }
                }
            }
            //
            HashSet<string> allNames = new HashSet<string>(_controller.Model.Geometry.Parts.Keys);
            allNames.ExceptWith(partsToShow);
            _controller.HideGeometryPartsCommand(allNames.ToArray());
            _controller.ShowGeometryPartsCommand(partsToShow.ToArray());
        }
        private void CopyGeometryPartsToResults(string[] partNames)
        {
            CloseAllForms();
            _controller.CopyGeometryPartsToResults(partNames);
        }
        private void DeleteGeometryParts(string[] partNames)
        {
            if (MessageBox.Show("OK to delete selected parts?" + Environment.NewLine + partNames.ToRows(),
                                Globals.ProgramName, MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveGeometryPartsCommand(partNames);
            }
        }
        //
        private void tsmiCreateAndImportCompoundPart_Click(object sender, EventArgs e)
        {
            try
            {
                Clear3DSelection();
                SelectMultipleEntities("Parts", _controller.GetCADGeometryParts(), CreateAndImportCompoundPart, 2);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private async void CreateAndImportCompoundPart(string[] partNames)
        {
            try
            {
                SetStateWorking(Globals.CreatingCompound, true);
                //
                GeometryPart part;
                foreach (var partName in partNames)
                {
                    part = (GeometryPart)_controller.Model.Geometry.Parts[partName];
                    if (part.CADFileData == null)
                        throw new CaeException("Compound part can only be made from CAD based geometry parts.");
                }
                //
                await Task.Run(() => _controller.CreateAndImportCompoundPartCommand(partNames));
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.CreatingCompound);
            }
        }
        // 
        private void tsmiGeometryAnalyze_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryParts(), AnalyzeGeometry);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }

        }
        private void AnalyzeGeometry(string[] partNames)
        {
            if (!_frmAnalyzeGeometry.Visible)
            {
                CloseAllForms();
                SetFormLoaction((Form)_frmAnalyzeGeometry);
                _frmAnalyzeGeometry.PartNamesToAnalyze = partNames;
                _frmAnalyzeGeometry.Show();
            }
        }
        //
        private async void tsmiFlipFaceNormal_Click(object sender, EventArgs e)
        {
            try
            {
                // Must be outside the await part otherwise couses screen flickering
                AnnotateWithColorEnum _prevShowWithColor = _controller.AnnotateWithColor;
                _controller.AnnotateWithColor = AnnotateWithColorEnum.FaceOrientation;
                //
                await Task.Run(() =>
                {
                    _frmSelectGeometry.HideFormOnOK = false;
                    _frmSelectGeometry.SelectionFilter = SelectGeometryEnum.Surface;
                    _frmSelectGeometry.OnOKCallback = FlipFaces;
                    //
                    InvokeIfRequired(() => { ShowForm(_frmSelectGeometry, "Select faces to flip", null); });
                    while (_frmSelectGeometry.Visible) System.Threading.Thread.Sleep(100);
                });
                //
                _controller.AnnotateWithColor = _prevShowWithColor;
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.FlippingNormals);
            }
        }
        private void FlipFaces(GeometrySelection geometrySelection)
        {
            SetStateWorking(Globals.FlippingNormals);
            _controller.FlipFaceOrientationsCommand(geometrySelection);
            SetStateReady(Globals.FlippingNormals);
        }
        //
        private async void tsmiSplitAFaceUsingTwoPoints_Click(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    GeometrySelection surfaceSelection;
                    GeometrySelection verticesSelection;
                    while (true)
                    {
                        // Get a surface to split
                        _frmSelectGeometry.MaxNumberOfSelectedItems = 1;
                        _frmSelectGeometry.SelectionFilter = SelectGeometryEnum.Surface;
                        //
                        InvokeIfRequired(() => { ShowForm(_frmSelectGeometry, "Select a face to split", null); });
                        while (_frmSelectGeometry.Visible) System.Threading.Thread.Sleep(100);
                        //
                        if (_frmSelectGeometry.DialogResult == DialogResult.OK)
                        {
                            surfaceSelection = _frmSelectGeometry.GeometrySelection.DeepClone();
                            // Get two vertices to split the surface
                            _frmSelectGeometry.MaxNumberOfSelectedItems = 2;
                            _frmSelectGeometry.SelectionFilter = SelectGeometryEnum.Vertex;
                            //
                            InvokeIfRequired(() => { ShowForm(_frmSelectGeometry, "Select splitting vertices", null); });
                            while (_frmSelectGeometry.Visible) System.Threading.Thread.Sleep(100);
                            //
                            if (_frmSelectGeometry.DialogResult == DialogResult.OK)
                            {
                                verticesSelection = _frmSelectGeometry.GeometrySelection.DeepClone();
                                //
                                SetStateWorking(Globals.SplittingFaces);
                                _controller.SplitAFaceUsingTwoPointsCommand(surfaceSelection, verticesSelection);
                                SetStateReady(Globals.SplittingFaces);
                            }
                            else break;
                        }
                        else break;
                    }
                });
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.SplittingFaces);
            }
        }
        //
        private void tsmiCropWithCylinder_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Parts", _controller.GetGeometryParts(), _controller.CropGeometryPartWithCylinder);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void tsmiFindEdgesByAngleForGeometryParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetNonCADGeometryParts(), FindEdgesByAngleForGeometryParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void FindEdgesByAngleForGeometryParts(string[] partNames)
        {
            using (FrmGetValue frmGetValue = new FrmGetValue())
            {
                SetUpFrmGetValueForEdgeAngle(frmGetValue, partNames);
                //
                if (frmGetValue.ShowDialog() == DialogResult.OK)
                {
                    _controller.FindEdgesByAngleForGeometryPartsCommand(partNames, frmGetValue.Value);
                }
                GetFormLoaction(frmGetValue);
            }
        }
        private void SetUpFrmGetValueForEdgeAngle(FrmGetValue frmGetValue, string[] partNames)
        {
            frmGetValue.NumOfDigits = 2;
            frmGetValue.MinValue = 0;
            frmGetValue.MaxValue = 90;
            SetFormLoaction(frmGetValue);
            OrderedDictionary<string, double> presetValues = new OrderedDictionary<string, double>();
            presetValues.Add("Default", CaeMesh.Globals.EdgeAngle);
            string desc = "Enter the face angle for model edges detection.";
            frmGetValue.PrepareForm("Find model edges: " + partNames.ToShortString(), "Angle", desc,
                                    CaeMesh.Globals.EdgeAngle, presetValues, new StringAngleDegConverter());
        }

        #endregion  ################################################################################################################

        #region Mesh ###############################################################################################################
        internal void tsmiMeshingParameters_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryPartsWithoutSubParts(), GetSetMeshingParameters);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiPreviewEdgeMesh_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryPartsWithoutSubParts(), PreviewEdgeMeshes);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiCreateMeshRefinement_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Geometry == null) return;
                // Data editor
                ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
                ItemSetDataEditor.ParentForm = _frmMeshRefinement;
                _frmSelectItemSet.SetOnlyGeometrySelection(true);
                ShowForm(_frmMeshRefinement, "Create Mesh Refinement", null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditMeshRefinement_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Mesh Refinements", _controller.GetMeshRefinements(), EditMeshRefinement);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteMeshRefinement_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Mesh Refinements", _controller.GetMeshRefinements(), DeleteMeshRefinements);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        internal void tsmiCreateMesh_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryPartsWithoutSubParts(), CreatePartMeshes);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }            
        }
        
        //
        private void GetSetMeshingParameters(string[] partNames)
        {
            try
            {
                GetMeshingParameters(partNames, false);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        public MeshingParameters GetMeshingParameters(string[] partNames, bool formModal)
        {
            BasePart part;
            MeshingParameters meshingParameters = null;
            MeshingParameters defaultMeshingParameters = GetDefaultMeshingParameters(partNames);
            if (defaultMeshingParameters != null)
            {
                foreach (var partName in partNames)
                {
                    part = _controller.GetGeometryPart(partName);
                    // First time set the meshing parameters
                    if (partName == partNames[0] && part is GeometryPart gp1)
                        meshingParameters = gp1.MeshingParameters;
                    // Meshing parameters exist only when all parts have the same meshing parameters
                    if (!(part is GeometryPart gp2) || !MeshingParameters.Equals(meshingParameters, gp2.MeshingParameters))
                        meshingParameters = null;
                }
                // Use meshingParameters as default if meshing parameters are not equal
                if (meshingParameters == null) meshingParameters = defaultMeshingParameters;
            }
            else return null;
            //
            MeshingParameters parameters = null;
            //
            InvokeIfRequired(() =>
            {
                parameters = GetMeshingParametersForm(partNames, defaultMeshingParameters, meshingParameters, formModal);
            });
            return parameters;
        }
        public MeshingParameters GetDefaultMeshingParameters(string[] partNames)
        {
            double sumMax = 0;
            double sumMin = 0;
            double sumHausDorff = 0;
            MeshingParameters defaultMeshingParameters = null;
            HashSet<bool> useMmg = new HashSet<bool>();
            foreach (var partName in partNames)
            {
                // Default parameters
                defaultMeshingParameters = GetDefaultMeshingParameters(partName);
                // If part is not found return null
                if (defaultMeshingParameters != null)
                {
                    // Check for different types of meshes
                    useMmg.Add(defaultMeshingParameters.UseMmg);
                    //
                    sumMax += defaultMeshingParameters.MaxH;
                    sumMin += defaultMeshingParameters.MinH;
                    sumHausDorff += defaultMeshingParameters.Hausdorff;
                }
                // Part was not found
                else return null;
            }
            // All parts must be of either netgen type or mmg type
            if (useMmg.Count() == 1)            
            {
                defaultMeshingParameters.MaxH = CaeGlobals.Tools.RoundToSignificantDigits(sumMax / partNames.Length, 2);
                defaultMeshingParameters.MinH = CaeGlobals.Tools.RoundToSignificantDigits(sumMin / partNames.Length, 2);
                defaultMeshingParameters.Hausdorff = CaeGlobals.Tools.RoundToSignificantDigits(sumHausDorff / partNames.Length, 2);
                //
                return defaultMeshingParameters;
            }
            else return null;
        }
        public MeshingParameters GetDefaultMeshingParameters(string partName)
        {
            BasePart part = _controller.GetGeometryPart(partName);
            if (part == null) part = _controller.GetModelPart(partName);
            if (part == null) return null;
            //
            if (!_controller.MeshJobIdle) throw new Exception("The meshing is already in progress.");
            //
            MeshingParameters defaultMeshingParameters = new MeshingParameters();
            double factorMax = 20;
            double factorMin = 1000;
            double factorHausdorff = 500;
            double maxSize = part.BoundingBox.GetDiagonal();
            //
            if (part.PartType == PartType.Shell && part is GeometryPart gp && gp.CADFileData == null)
                defaultMeshingParameters.UseMmg = true;
            else if (part.PartType == PartType.Shell && part is MeshPart)   // for remeshing
                defaultMeshingParameters.UseMmg = true;
            //
            defaultMeshingParameters.MaxH = CaeGlobals.Tools.RoundToSignificantDigits(maxSize / factorMax, 2);
            defaultMeshingParameters.MinH = CaeGlobals.Tools.RoundToSignificantDigits(maxSize / factorMin, 2);
            defaultMeshingParameters.Hausdorff = CaeGlobals.Tools.RoundToSignificantDigits(maxSize / factorHausdorff, 2);
            //
            return defaultMeshingParameters;
        }
        public MeshingParameters GetMeshingParametersForm(string[] partNames,
                                                                  MeshingParameters defaultMeshingParameters,
                                                                  MeshingParameters meshingParameters,
                                                                  bool formModal)
        {
            // - show form
            CloseAllForms();
            SetFormLoaction(_frmMeshingParameters);
            _frmMeshingParameters.PartNames = partNames;
            _frmMeshingParameters.DefaultMeshingParameters = defaultMeshingParameters;
            _frmMeshingParameters.MeshingParameters = meshingParameters;
            //
            if (formModal)
            {
                if (_frmMeshingParameters.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    return _frmMeshingParameters.MeshingParameters;
                else return null; // Cancel pressed on Meshing parameters form
            }
            else
            {
                _frmMeshingParameters.Show();
                return null;
            }
        }
        public void SetDefaultMeshingParameters(string partName)
        {
            MeshingParameters defaultMeshingParameters = GetDefaultMeshingParameters(partName);
            _controller.SetMeshingParametersCommand(new string[] { partName }, defaultMeshingParameters);
        }
        private async void PreviewEdgeMeshes(string[] partNames)
        {
            try
            {
                foreach (var partName in partNames)
                {
                    await Task.Run(() => _controller.PreviewEdgeMesh(partName, null, null));
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void EditMeshRefinement(string meshRefinementName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmMeshRefinement;
            _frmSelectItemSet.SetOnlyGeometrySelection(true);
            ShowForm(_frmMeshRefinement, "Edit Mesh Refinement", meshRefinementName);
        }
        private void DeleteMeshRefinements(string[] meshRefinementNames)
        {
            if (MessageBox.Show("OK to delete selected mesh refinements?" + Environment.NewLine + meshRefinementNames.ToRows(),
                                Globals.ProgramName,
                                MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveMeshRefinementsCommand(meshRefinementNames);
            }
        }
        private async void CreatePartMeshes(string[] partNames)
        {
            try
            {
                SetStateWorking(Globals.MeshingText, true);
                MouseEventArgs e = new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0);
                Keys modifierKeys = Keys.Control;
                _modelTree.ClearTreeSelection(ViewType.Model);
                //
                foreach (var partName in partNames)
                {
                    GeometryPart part = _controller.GetGeometryPart(partName);
                    if (part.MeshingParameters == null) SetDefaultMeshingParameters(partName);
                    //
                    CloseAllForms();
                    await Task.Run(() => _controller.CreateMeshCommand(partName));
                    //
                    _modelTree.SelectBasePart(e, modifierKeys, part);
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
                WriteDataToOutput("");
                WriteDataToOutput("Mesh generation failed. Check the geometry and adjust the meshing parameters.");
            }
            finally
            {
                SetStateReady(Globals.MeshingText);
            }
        }
        // Advisor
        internal void CreateDefaultMesh(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryPartsWithoutSubParts(), CreateDefaultMeshes);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        internal void CreateUserDefinedMesh(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryPartsWithoutSubParts(), CreateUserDefinedMeshes);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void CreateDefaultMeshes(string[] partNames)
        {
            foreach (var partName in partNames) SetDefaultMeshingParameters(partName);
            CreatePartMeshes(partNames);
        }
        private async void CreateUserDefinedMeshes(string[] partNames)
        {
            await Task.Run(() =>
            {
                GetSetMeshingParameters(partNames);
                while (_frmMeshingParameters.Visible)
                {
                    System.Threading.Thread.Sleep(100);
                }
            });
            if (_frmMeshingParameters.DialogResult == DialogResult.OK) CreatePartMeshes(partNames);
        }

        #endregion  ################################################################################################################

        #region Model edit  ########################################################################################################

        private void tsmiEditModel_Click(object sender, EventArgs e)
        {
            try
            {
                EditModel();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditCalculiXKeywords_Click(object sender, EventArgs e)
        {
            try
            {
                EditEditCalculiXKeywords();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        // Tools
        private void tsmiCreateBoundaryLayer_Click(object sender, EventArgs e)
        {
            try
            {
                CreateBoundaryLayer();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiFindEdgesByAngleForModelParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetNonCADModelParts(), FindEdgesByAngleForModelParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiRemeshElements_Click(object sender, EventArgs e)
        {
            try
            {
                RemeshElements();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //                                                                                                                          
        private void EditModel()
        {
            ShowForm(_frmModelProperties, "Edit Model", null);
        }
        private void EditEditCalculiXKeywords()
        {
            // This is also called from the model tree - needs try, catch
            try
            {
                if (CheckValiditiy())
                {
                    _frmCalculixKeywordEditor = new FrmCalculixKeywordEditor();
                    _frmCalculixKeywordEditor.Keywords = _controller.GetCalculixModelKeywords();
                    _frmCalculixKeywordEditor.UserKeywords = _controller.GetCalculixUserKeywords();
                    //
                    if (_frmCalculixKeywordEditor.Keywords != null)
                    {
                        _frmCalculixKeywordEditor.PrepareForm(); // must be here to check for errors
                        if (_frmCalculixKeywordEditor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            _controller.SetCalculixUserKeywordsCommand(_frmCalculixKeywordEditor.UserKeywords);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        // Tools
        private void CreateBoundaryLayer()
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmBoundaryLayer;
            _frmSelectItemSet.SetOnlyGeometrySelection(true);
            ShowForm(_frmBoundaryLayer, "Create Boundary Layer", null);
        }
        private void FindEdgesByAngleForModelParts(string[] partNames)
        {
            using (FrmGetValue frmGetValue = new FrmGetValue())
            {
                SetUpFrmGetValueForEdgeAngle(frmGetValue, partNames);
                //
                if (frmGetValue.ShowDialog() == DialogResult.OK)
                {
                    _controller.FindEdgesByAngleForModelPartsCommand(partNames, frmGetValue.Value);
                }
                GetFormLoaction(frmGetValue);
            }
        }
        private void RemeshElements()
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmRemeshingParameters;
            _frmSelectItemSet.SetOnlyGeometrySelection(false);
            ShowForm(_frmRemeshingParameters, "Remeshing Parameters", null);
        }

        #endregion  ################################################################################################################

        #region Node menu  #########################################################################################################
        private void tsmiRenumberAllNodes_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;

                using (FrmGetValue frmGetValue = new FrmGetValue())
                {
                    frmGetValue.NumOfDigits = 0;
                    frmGetValue.MinValue = 1;
                    SetFormLoaction(frmGetValue);
                    string desc = "Enter the starting node id for the node renumbering.";
                    frmGetValue.PrepareForm("Renumber Nodes", "Start node id", desc, 1, null);
                    if (frmGetValue.ShowDialog() == DialogResult.OK)
                    {
                        _controller.RenumberNodesCommand((int)frmGetValue.Value);
                    }
                    GetFormLoaction(frmGetValue);
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }

        #endregion  ################################################################################################################

        #region Model part menu  ###################################################################################################
        private void tsmiEditPart_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Parts", _controller.GetModelParts(), EditModelPart);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        // Transform
        private void tsmiTranslateParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), TranslateParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiScaleParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), ScaleParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiRotateParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), RotateParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void tsmiMergeParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), MergeModelParts, 2);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiHideParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), HideModelParts);
                Clear3DSelection();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiShowParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), ShowModelParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiShowOnlyParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), ShowOnlyModelParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiSetTransparencyForParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), SetTransparencyForModelParts);
                Clear3DSelection();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), DeleteModelParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //                                                                                                                          
        private void EditModelPart(string partName)
        {
            _frmPartProperties.View = ViewGeometryModelResults.Model; 
            ShowForm(_frmPartProperties, "Edit Part", partName);
        }
        // Transform
        private void TranslateParts(string[] partNames)
        {
            SinglePointDataEditor.ParentForm = _frmTranslate;
            SinglePointDataEditor.Controller = _controller;
            // Set all part names for translation
            _frmTranslate.PartNames = partNames;    
            //
            ShowForm(_frmTranslate, "Translate parts: " + partNames.ToShortString(), null);
        }
        private void ScaleParts(string[] partNames)
        {
            SinglePointDataEditor.ParentForm = _frmScale;
            SinglePointDataEditor.Controller = _controller;
            // Set all part names for scaling
            _frmScale.PartNames = partNames;    
            //
            ShowForm(_frmScale, "Scale parts: " + partNames.ToShortString(), null);
        }
        private void RotateParts(string[] partNames)
        {
            SinglePointDataEditor.ParentForm = _frmRotate;
            SinglePointDataEditor.Controller = _controller;
            // Set all part names for rotation
            _frmRotate.PartNames = partNames;    
            //
            ShowForm(_frmRotate, "Rotate parts: " + partNames.ToShortString(), null);
        }
        //
        private void MergeModelParts(string[] partNames)
        {
            if (MessageBox.Show("OK to merge selected parts?",
                                Globals.ProgramName,
                                MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.MergeModelPartsCommand(partNames);
            }
        }
        private void HideModelParts(string[] partNames)
        {
            _controller.HideModelPartsCommand(partNames);
        }
        private void ShowModelParts(string[] partNames)
        {
            _controller.ShowModelPartsCommand(partNames);
        }
        private void ShowOnlyModelParts(string[] partNames)
        {
            HashSet<string> allNames = new HashSet<string>(_controller.Model.Mesh.Parts.Keys);
            allNames.ExceptWith(partNames);
            _controller.ShowModelPartsCommand(partNames);
            _controller.HideModelPartsCommand(allNames.ToArray());
        }
        private void SetTransparencyForModelParts(string[] partNames)
        {
            if (_controller.Model.Mesh == null) return;
            //
            using (FrmGetValue frmGetValue = new FrmGetValue())
            {
                frmGetValue.NumOfDigits = 0;
                frmGetValue.MinValue = 25;
                frmGetValue.MaxValue = 255;
                SetFormLoaction(frmGetValue);
                OrderedDictionary<string, double> presetValues = new OrderedDictionary<string, double>();
                presetValues.Add("Semi-transparent", 128);
                presetValues.Add("Opaque", 255);
                string desc = "Enter the transparency between 0 and 255.\n" + "(0 - transparent; 255 - opaque)";
                frmGetValue.PrepareForm("Set Transparency: " + partNames.ToShortString(), "Transparency", desc, 128, presetValues);
                if (frmGetValue.ShowDialog() == DialogResult.OK)
                {
                    _controller.SetTransparencyForModelPartsCommand(partNames, (byte)frmGetValue.Value);
                }
                GetFormLoaction(frmGetValue);
            }
        }
        private void DeleteModelParts(string[] partNames)
        {
            if (MessageBox.Show("OK to delete selected parts?" + Environment.NewLine + partNames.ToRows(),
                                Globals.ProgramName,
                                MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveModelPartsCommand(partNames);
            }
        }
        
        #endregion  ################################################################################################################

        #region Node set menu  #####################################################################################################

        private void tsmiCreateNodeSet_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
                // Data editor
                ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
                ItemSetDataEditor.ParentForm = _frmNodeSet;
                _frmSelectItemSet.SetOnlyGeometrySelection(false);
                ShowForm(_frmNodeSet, "Create Node Set", null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditNodeSet_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Node Sets", _controller.GetUserNodeSets(), EditNodeSet);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDuplicateNodeSet_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Node Sets", _controller.GetUserNodeSets(), DuplicateNodeSets);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteNodeSet_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Node Sets", _controller.GetUserNodeSets(), DeleteNodeSets);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void EditNodeSet(string nodeSetName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmNodeSet;
            _frmSelectItemSet.SetOnlyGeometrySelection(false);
            ShowForm(_frmNodeSet, "Edit Node Set", nodeSetName);
        }
        private void DuplicateNodeSets(string[] nodeSetNames)
        {
            _controller.DuplicateNodeSetsCommand(nodeSetNames);
        }
        private void DeleteNodeSets(string[] nodeSetNames)
        {
            if (MessageBox.Show("OK to delete selected node sets?" + Environment.NewLine + nodeSetNames.ToRows(),
                                Globals.ProgramName,
                                MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveNodeSetsCommand(nodeSetNames);
            }
        }

        #endregion  ################################################################################################################

        #region Element set menu  ##################################################################################################
        
        private void tsmiCreateElementSet_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
                // Data editor
                ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
                ItemSetDataEditor.ParentForm = _frmElementSet;
                _frmSelectItemSet.SetOnlyGeometrySelection(false);
                ShowForm(_frmElementSet, "Create Element Set", null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditElementSet_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Element Sets", _controller.GetUserElementSets(), EditElementSet);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDuplicateElementSet_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Element Sets", _controller.GetUserElementSets(), DuplicateElementSets);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiConvertElementSetsToMeshParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Element Sets", _controller.GetUserElementSets(), ConvertElementSetsToMeshParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteElementSet_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Element Sets", _controller.GetUserElementSets(), DeleteElementSets);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void EditElementSet(string elementSetName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmElementSet;
            _frmSelectItemSet.SetOnlyGeometrySelection(false);
            ShowForm(_frmElementSet, "Edit Element Set", elementSetName);
        }
        private void DuplicateElementSets(string[] elementSetNames)
        {
            _controller.DuplicateElementSetsCommand(elementSetNames);
        }
        private void ConvertElementSetsToMeshParts(string[] elementSetNames)
        {
            _controller.ConvertElementSetsToMeshPartsCommand(elementSetNames);
        }
        private void DeleteElementSets(string[] elementSetNames)
        {
            if (MessageBox.Show("OK to delete selected element sets?" + Environment.NewLine + elementSetNames.ToRows(),
                                Globals.ProgramName,
                                MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveElementSetsCommand(elementSetNames);
            }
        }

        #endregion  ################################################################################################################

        #region Surface menu  ######################################################################################################

        private void tsmiCreateSurface_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
                // Data editor
                ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
                ItemSetDataEditor.ParentForm = _frmSurface;
                _frmSelectItemSet.SetOnlyGeometrySelection(false);
                ShowForm(_frmSurface, "Create Surface", null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditSurface_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Surfaces", _controller.GetAllSurfaces(), EditSurface);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteSurface_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Surfaces", _controller.GetAllSurfaces(), DeleteSurfaces);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void EditSurface(string surfaceName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmSurface;
            _frmSelectItemSet.SetOnlyGeometrySelection(false);
            ShowForm(_frmSurface, "Edit Surface", surfaceName);
        }
        private void DeleteSurfaces(string[] surfaceNames)
        {
            if (MessageBox.Show("OK to delete selected surfaces?" + Environment.NewLine + surfaceNames.ToRows(),
                                Globals.ProgramName,
                                MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveSurfacesCommand(surfaceNames);
            }
        }

        #endregion  ################################################################################################################

        #region Reference point  ###################################################################################################
       
        private void tsmiCreateRP_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
                //
                ShowForm(_frmReferencePoint, "Create Reference Point", null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditRP_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Reference points", _controller.GetAllReferencePoints(), EditRP);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteRP_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Reference points", _controller.GetAllReferencePoints(), DeleteRPs);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }

        private void EditRP(string referencePointName)
        {
            ShowForm(_frmReferencePoint, "Edit Reference Point", referencePointName);
        }
        private void HideRPs(string[] constraintNames)
        {
            _controller.HideReferencePointsCommand(constraintNames);
        }
        private void ShowRPs(string[] constraintNames)
        {
            _controller.ShowReferencePointsCommand(constraintNames);
        }
        private void ShowOnlyRPs(string[] referencePointNames)
        {
            HashSet<string> allNames = new HashSet<string>(_controller.Model.Mesh.ReferencePoints.Keys);
            allNames.ExceptWith(referencePointNames);
            _controller.ShowReferencePointsCommand(referencePointNames);
            _controller.HideReferencePointsCommand(allNames.ToArray());
        }
        private void DeleteRPs(string[] referencePointNames)
        {
            if (MessageBox.Show("OK to delete selected reference points?" + Environment.NewLine + referencePointNames.ToRows(),
                                Globals.ProgramName,
                                MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveReferencePointsCommand(referencePointNames);
            }
        }

        #endregion  ################################################################################################################

        #region Material menu  #####################################################################################################

        internal void tsmiCreateMaterial_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
                //
                ShowForm(_frmMaterial, "Create Material", null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        internal void CreateSimpleMaterial(object sender, EventArgs e)
        {
            _frmMaterial.UseSimpleEditor = true;
            tsmiCreateMaterial_Click(sender, e);
        }
        private void tsmiEditMaterial_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Materials", _controller.GetAllMaterials(), EditMaterial);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDuplicateMaterial_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Materials", _controller.GetAllMaterials(), DuplicateMaterials);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteMaterial_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Materials", _controller.GetAllMaterials(), DeleteMaterials);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
           
        }
        //
        private void EditMaterial(string materialName)
        {
            ShowForm(_frmMaterial, "Edit Material", materialName);
        }
        private void DuplicateMaterials(string[] materialNames)
        {
            _controller.DuplicateMaterialsCommand(materialNames);
        }
        private void DeleteMaterials(string[] materialNames)
        {
            if (MessageBox.Show("OK to delete selected materials?" + Environment.NewLine + materialNames.ToRows(),
                                Globals.ProgramName,
                                MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveMaterialsCommand(materialNames);
            }
        }
        //
        internal void tsmiMaterialLibrary_Click(object sender, EventArgs e)
        {
            try
            {
                ShowMaterialLibrary();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void ShowMaterialLibrary()
        {
            if (_controller.Model.Mesh != null)
            {
                FrmMaterialLibrary fml = new FrmMaterialLibrary(_controller);
                CloseAllForms();
                SetFormLoaction((Form)fml);
                fml.ShowDialog();
            }
        }

        #endregion  ################################################################################################################

        #region Section menu  ######################################################################################################
        internal void tsmiCreateSection_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
                // Data editor
                ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
                ItemSetDataEditor.ParentForm = _frmSection;
                _frmSelectItemSet.SetOnlyGeometrySelection(true);
                ShowForm(_frmSection, "Create Section", null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditSection_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Sections", _controller.GetAllSections(), EditSection);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Sections", _controller.GetAllSections(), DeleteSections);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void EditSection(string sectionName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmSection;
            _frmSelectItemSet.SetOnlyGeometrySelection(true);
            ShowForm(_frmSection, "Edit Section", sectionName);
        }
        private void DeleteSections(string[] sectionNames)
        {
            if (MessageBox.Show("OK to delete selected sections?" + Environment.NewLine + sectionNames.ToRows(),
                                Globals.ProgramName,
                                MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveSectionsCommand(sectionNames);
            }
        }

        #endregion  ################################################################################################################

        #region Constraint menu  ###################################################################################################
        private void tsmiCreateConstraint_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
                // Data editor
                ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
                ItemSetDataEditor.ParentForm = _frmConstraint;
                _frmSelectItemSet.SetOnlyGeometrySelection(false);
                ShowForm(_frmConstraint, "Create Constraint", null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditConstraint_Click(object sender, EventArgs e)
        {
             try
             {
                 SelectOneEntity("Constraints", _controller.GetAllConstraints(), EditConstraint);
             }
             catch (Exception ex)
             {
                 ExceptionTools.Show(this, ex);
             }
        }
        private void tsmiHideConstraint_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Constaints", _controller.GetAllConstraints(), HideConstraints);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiShowConstraint_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Constaints", _controller.GetAllConstraints(), ShowConstraints);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteConstraint_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Constraints", _controller.GetAllConstraints(), DeleteConstraints);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }

        private void EditConstraint(string constraintName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmConstraint;
            _frmSelectItemSet.SetOnlyGeometrySelection(false);
            ShowForm(_frmConstraint, "Edit Constraint", constraintName);
        }
        private void HideConstraints(string[] constraintNames)
        {
            _controller.HideConstraintsCommand(constraintNames);
        }
        private void ShowConstraints(string[] constraintNames)
        {
            _controller.ShowConstraintsCommand(constraintNames);
        }
        private void ShowOnlyConstraints(string[] constraintNames)
        {
            HashSet<string> allNames = new HashSet<string>(_controller.Model.Constraints.Keys);
            allNames.ExceptWith(constraintNames);
            _controller.ShowConstraintsCommand(constraintNames);
            _controller.HideConstraintsCommand(allNames.ToArray());
        }
        private void DeleteConstraints(string[] constraintNames)
        {
            if (MessageBox.Show("OK to delete selected constraints?" + Environment.NewLine + constraintNames.ToRows(),
                                Globals.ProgramName,
                                MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveConstraintsCommand(constraintNames);
            }
        }

        #endregion  ################################################################################################################

        #region Surface interaction menu  ##########################################################################################

        private void tsmiCreateSurfaceInteraction_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
                ShowForm(_frmSurfaceInteraction, "Create surface interaction", null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditSurfaceInteraction_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Surface interactions", _controller.GetAllSurfaceInteractions(), EditSurfaceInteraction);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDuplicateSurfaceInteraction_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Surface interactions", _controller.GetAllSurfaceInteractions(),
                                       DuplicateSurfaceInteractions);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteSurfaceInteraction_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Surface interactions", _controller.GetAllSurfaceInteractions(), DeleteSurfaceInteractions);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }

        }
        //
        private void EditSurfaceInteraction(string surfaceInteractionName)
        {
            ShowForm(_frmSurfaceInteraction, "Edit surface interaction", surfaceInteractionName);
        }
        private void DuplicateSurfaceInteractions(string[] surfaceInteractionNames)
        {
            _controller.DuplicateSurfaceInteractionsCommand(surfaceInteractionNames);
        }
        private void DeleteSurfaceInteractions(string[] surfaceInteractionNames)
        {
            if (MessageBox.Show("OK to delete selected surface interactions?" + Environment.NewLine +
                                surfaceInteractionNames.ToRows(),
                                Globals.ProgramName,
                                MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveSurfaceInteractionsCommand(surfaceInteractionNames);
            }
        }



        #endregion  ################################################################################################################

        #region Contact pair menu  #################################################################################################
        private void tsmiCreateContactPair_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
                // Data editor
                ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
                ItemSetDataEditor.ParentForm = _frmContactPair;
                _frmSelectItemSet.SetOnlyGeometrySelection(false);
                ShowForm(_frmContactPair, "Create Contact Pair", null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditContactPair_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Contact pairs", _controller.GetAllContactPairs(), EditContactPair);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiHideContactPair_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Contact pairs", _controller.GetAllContactPairs(), HideContactPairs);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiShowContactPair_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Contact pairs", _controller.GetAllContactPairs(), ShowContactPairs);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteContactPair_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Contact pairs", _controller.GetAllContactPairs(), DeleteContactPairs);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void EditContactPair(string contactPairName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmContactPair;
            _frmSelectItemSet.SetOnlyGeometrySelection(false);
            ShowForm(_frmContactPair, "Edit Contact Pair", contactPairName);
        }
        private void HideContactPairs(string[] contactPairNames)
        {
            _controller.HideContactPairsCommand(contactPairNames);
        }
        private void ShowContactPairs(string[] contactPairNames)
        {
            _controller.ShowContactPairsCommand(contactPairNames);
        }
        private void ShowOnlyContactPairs(string[] contactPairNames)
        {
            HashSet<string> allNames = new HashSet<string>(_controller.Model.ContactPairs.Keys);
            allNames.ExceptWith(contactPairNames);
            _controller.ShowContactPairsCommand(contactPairNames);
            _controller.HideContactPairsCommand(allNames.ToArray());
        }
        private void DeleteContactPairs(string[] contactPairNames)
        {
            if (MessageBox.Show("OK to delete selected contact pairs?" + Environment.NewLine + contactPairNames.ToRows(),
                                Globals.ProgramName,
                                MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveContactPairsCommand(contactPairNames);
            }
        }
        #endregion  ################################################################################################################

        #region Step menu  #########################################################################################################

        internal void tsmiCreateStep_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
                //
                int selectedIndex = -1;
                if (e is EventArgs<int> ea) selectedIndex = ea.Value;
                _frmStep.PreselectListViewItem(selectedIndex);
                //
                ShowForm(_frmStep, "Create Step", null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditStep_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), EditStep);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDuplicateStep_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Steps", _controller.GetAllSteps(), DuplicateSteps);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteStep_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Steps", _controller.GetAllSteps(), DeleteSteps);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }

        private void EditStep(string stepName)
        {
            ShowForm(_frmStep, "Edit Step", stepName);
        }
        private void DuplicateSteps(string[] stepNames)
        {
            _controller.DuplicateStepsCommnad(stepNames);
        }
        private void DeleteSteps(string[] stepNames)
        {
            if (MessageBox.Show("OK to delete selected steps?" + Environment.NewLine + stepNames.ToRows(),
                                Globals.ProgramName,
                                MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveStepsCommnad(stepNames);
            }
        }

        #endregion  ################################################################################################################

        #region History output menu  ###############################################################################################
        private void tsmiCreateHistoryOutput_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), CreateHistoryOutput);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditHistoryOutput_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndEditHistoryOutput);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteHistoryOutput_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndDeleteHistoryOutputs);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void SelectAndEditHistoryOutput(string stepName)
        {
            SelectOneEntityInStep("History outputs", _controller.GetAllHistoryOutputs(stepName), stepName, EditHistoryOutput);
        }
        private void SelectAndDeleteHistoryOutputs(string stepName)
        {
            SelectMultipleEntitiesInStep("History outputs", _controller.GetAllHistoryOutputs(stepName),
                                         stepName, DeleteHistoryOutputs);
        }
        //
        private void CreateHistoryOutput(string stepName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmHistoryOutput;
            _frmSelectItemSet.SetOnlyGeometrySelection(false);
            if (_controller.Model.Mesh == null) return;
            ShowForm(_frmHistoryOutput, "Create History Output", stepName, null);
        }
        private void EditHistoryOutput(string stepName, string historyOutputName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmHistoryOutput;
            _frmSelectItemSet.SetOnlyGeometrySelection(false);
            ShowForm(_frmHistoryOutput, "Edit History Output", stepName, historyOutputName);
        }
        private void DeleteHistoryOutputs(string stepName, string[] historyOutputNames)
        {
            if (MessageBox.Show("OK to delete selected history outputs from step " + stepName + "?" + Environment.NewLine +
                                historyOutputNames.ToRows(),
                                Globals.ProgramName,
                                MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveHistoryOutputsForStepCommand(stepName, historyOutputNames);
            }
        }
        #endregion  ################################################################################################################

        #region Field output menu  #################################################################################################
        private void tsmiCreateFieldOutput_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), CreateFieldOutput);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditFieldOutput_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndEditFieldOutput);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteFieldOutput_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndDeleteFieldOutputs);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }

        private void SelectAndEditFieldOutput(string stepName)
        {
            SelectOneEntityInStep("Field outputs", _controller.GetAllFieldOutputs(stepName), stepName, EditFieldOutput);
        }
        private void SelectAndDeleteFieldOutputs(string stepName)
        {
            SelectMultipleEntitiesInStep("Field outputs", _controller.GetAllFieldOutputs(stepName), stepName, DeleteFieldOutputs);
        }

        private void CreateFieldOutput(string stepName)
        {
            if (_controller.Model.Mesh == null) return;
            ShowForm(_frmFieldOutput, "Create Field Output", stepName, null);
        }
        private void EditFieldOutput(string stepName, string fieldOutputName)
        {
            ShowForm(_frmFieldOutput, "Edit Field Output", stepName, fieldOutputName);
        }
        private void DeleteFieldOutputs(string stepName, string[] fieldOutputNames)
        {
            if (MessageBox.Show("OK to delete selected field outputs from step " + stepName + "?" + Environment.NewLine +
                                fieldOutputNames.ToRows(),
                                Globals.ProgramName,
                                MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveFieldOutputsForStepCommand(stepName, fieldOutputNames);
            }
        }

        #endregion  ################################################################################################################

        #region Boundary conditions menu  ##########################################################################################
        internal void tsmiCreateBC_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedIndex = -1;
                if (e is EventArgs<int> ea) selectedIndex = ea.Value;
                _frmBoundaryCondition.PreselectListViewItem(selectedIndex);
                //
                SelectOneEntity("Steps", _controller.GetAllSteps(), CreateBoundaryCondition);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditBC_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndEditBoundaryCondition);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiHideBC_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndHideBoundaryConditions);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiShowBC_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndShowBoundaryConditions);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteBC_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndDeleteBoundaryCondition);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void SelectAndEditBoundaryCondition(string stepName)
        {
            SelectOneEntityInStep("Boundary conditions", _controller.GetStepBoundaryConditions(stepName), stepName,
                                  EditBoundaryCondition);
        }
        private void SelectAndHideBoundaryConditions(string stepName)
        {
            SelectMultipleEntitiesInStep("Boundary conditions", _controller.GetStepBoundaryConditions(stepName),
                                         stepName, HideBoundaryConditions);
        }
        private void SelectAndShowBoundaryConditions(string stepName)
        {
            SelectMultipleEntitiesInStep("Boundary conditions", _controller.GetStepBoundaryConditions(stepName),
                                         stepName, ShowBoundaryConditions);
        }
        private void SelectAndDeleteBoundaryCondition(string stepName)
        {
            SelectMultipleEntitiesInStep("Boundary conditions", _controller.GetStepBoundaryConditions(stepName),
                                         stepName, DeleteBoundaryConditions);
        }
        //
        private void CreateBoundaryCondition(string stepName)
        {
            if (_controller.Model.Mesh == null) return;
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmBoundaryCondition;
            _frmSelectItemSet.SetOnlyGeometrySelection(true);
            ShowForm(_frmBoundaryCondition, "Create Boundary Condition", stepName, null);
        }
        private void EditBoundaryCondition(string stepName, string boundaryConditionName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmBoundaryCondition;
            _frmSelectItemSet.SetOnlyGeometrySelection(true);
            ShowForm(_frmBoundaryCondition, "Edit Boundary Condition", stepName, boundaryConditionName);
        }
        private void HideBoundaryConditions(string stepName, string[] boundaryConditionNames)
        {
            _controller.HideBoundaryConditionCommand(stepName, boundaryConditionNames);
        }
        private void ShowBoundaryConditions(string stepName, string[] boundaryConditionNames)
        {
            _controller.ShowBoundaryConditionCommand(stepName, boundaryConditionNames);
        }
        private void ShowOnlyBoundaryConditions(string stepName, string[] boundaryConditionNames)
        {
            HashSet<string> allNames =
                new HashSet<string>(_controller.Model.StepCollection.GetStep(stepName).BoundaryConditions.Keys);
            allNames.ExceptWith(boundaryConditionNames);
            _controller.ShowBoundaryConditionCommand(stepName, boundaryConditionNames);
            _controller.HideBoundaryConditionCommand(stepName, allNames.ToArray());
        }
        private void DeleteBoundaryConditions(string stepName, string[] boundaryConditionNames)
        {
            if (MessageBox.Show("OK to delete selected boundary conditions from step " + stepName + "?" + Environment.NewLine +
                                boundaryConditionNames.ToRows(),
                                Globals.ProgramName,
                                MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveBoundaryConditionsCommand(stepName, boundaryConditionNames);
            }
        }

        #endregion  ################################################################################################################

        #region Load menu  #########################################################################################################

        internal void tsmiCreateLoad_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedIndex = -1;
                if (e is EventArgs<int> ea) selectedIndex = ea.Value;
                _frmLoad.PreselectListViewItem(selectedIndex);
                //
                SelectOneEntity("Steps", _controller.GetAllSteps(), CreateLoad);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditLoad_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndEditLoad);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiHideLoad_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndHideLoads);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiShowLoad_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndShowLoads);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteLoad_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndDeleteLoad);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }

        private void SelectAndEditLoad(string stepName)
        {
            SelectOneEntityInStep("Loads", _controller.GetStepLoads(stepName), stepName, EditLoad);
        }
        private void SelectAndHideLoads(string stepName)
        {
            SelectMultipleEntitiesInStep("Loads", _controller.GetStepLoads(stepName), stepName, HideLoads);
        }
        private void SelectAndShowLoads(string stepName)
        {
            SelectMultipleEntitiesInStep("Loads", _controller.GetStepLoads(stepName), stepName, ShowLoads);
        }
        private void SelectAndDeleteLoad(string stepName)
        {
            SelectMultipleEntitiesInStep("Loads", _controller.GetStepLoads(stepName), stepName, DeleteLoads);
        }

        private void CreateLoad(string stepName)
        {
            if (_controller.Model.Mesh == null) return;
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmLoad;
            _frmSelectItemSet.SetOnlyGeometrySelection(false);
            ShowForm(_frmLoad, "Create Load", stepName, null);
        }
        private void EditLoad(string stepName, string loadName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmLoad;
            _frmSelectItemSet.SetOnlyGeometrySelection(false);
            ShowForm(_frmLoad, "Edit Load", stepName, loadName);
        }
        private void HideLoads(string stepName, string[] loadNames)
        {
            _controller.HideLoadsCommand(stepName, loadNames);
        }
        private void ShowLoads(string stepName, string[] loadNames)
        {
            _controller.ShowLoadsCommand(stepName, loadNames);
        }
        private void ShowOnlyLoads(string stepName, string[] loadNames)
        {
            HashSet<string> allNames = new HashSet<string>(_controller.Model.StepCollection.GetStep(stepName).Loads.Keys);
            allNames.ExceptWith(loadNames);
            _controller.ShowLoadsCommand(stepName, loadNames);
            _controller.HideLoadsCommand(stepName, allNames.ToArray());
        }
        private void DeleteLoads(string stepName, string[] loadNames)
        {
            if (MessageBox.Show("OK to delete selected loads from step " + stepName + "?" + Environment.NewLine +
                                loadNames.ToRows(),
                                Globals.ProgramName,
                                MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveLoadsCommand(stepName, loadNames);
            }
        }

        #endregion  ################################################################################################################

        #region Tools ##############################################################################################################
        private void tsmiSettings_Click(object sender, EventArgs e)
        {
            try
            {
                //string settingsName = Globals.GraphicsSettingsName;
                //if (_controller.CurrentView == ViewGeometryMeshResults.Results) settingsName = Globals.PostSettingsName;

                if (!_frmSettings.Visible)
                {
                    CloseAllForms();
                    SetFormLoaction(_frmSettings);
                    _frmSettings.PrepareForm(_controller);
                    _frmSettings.Show();
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
           
        }
        private void tsmiQuery_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_frmQuery.Visible)
                {
                    ClearSelection();
                    //
                    CloseAllForms();
                    SetFormLoaction(_frmQuery);
                    _frmQuery.PrepareForm(_controller);
                    _frmQuery.Show();
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void ShowLegendSettings()
        {
            _frmSettings.SetSettingsToShow(Globals.LegendSettingsName);
            tsmiSettings_Click(null, null);
        }
        private void ShowStatusBlockSettings()
        {
            _frmSettings.SetSettingsToShow(Globals.StatusBlockSettingsName);
            tsmiSettings_Click(null, null);
        }
        //
        private void UpdateSettings(Dictionary<string, ISettings> items)
        {
            _controller.Settings = new SettingsContainer(items);
        }

        // Unit system
        private void tsslUnitSystem_Click(object sender, EventArgs e)
        {
            try
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    if (GetCurrentView() == ViewGeometryModelResults.Geometry ||
                    GetCurrentView() == ViewGeometryModelResults.Model) SelectModelUnitSystem();
                    else SelectResultsUnitSystem();
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        public void SelectModelUnitSystem()
        {
            try
            {
                // Disable unit system selection during regenerate - check that the state is ready
                if (tsslState.Text != Globals.RegeneratingText)
                {
                    UnitSystemType unitSystemType = _controller.Settings.General.UnitSystemType;
                    if (unitSystemType == UnitSystemType.Undefined)
                    {
                        InvokeIfRequired(ShowForm, _frmUnitSystem, "Select Unit System", "Geometry & Model");
                    }
                    else
                    {
                        _controller.SetModelUnitSystemCommand(unitSystemType);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        public void SelectResultsUnitSystem()
        {
            try
            {
                // Disable unit system selection during regenerate - check that the state is ready
                if (tsslState.Text != Globals.RegeneratingText)
                    InvokeIfRequired(ShowForm, _frmUnitSystem, "Select Unit System", "Results");
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        public void UpdateUnitSystem(UnitSystem unitSystem)
        {
            tsslUnitSystem.Text = "Unit system: " + unitSystem.UnitSystemType.GetDescription();
            //
            SetScaleWidgetUnit(unitSystem);
        }
        #endregion  ################################################################################################################

        #region Analysis menu  #####################################################################################################

        private void tsmiCreateAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
                ShowForm(_frmAnalysis, "Create Analysis", null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Analyses", _controller.GetAllJobs(), EditAnalysis);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        internal void tsmiRunAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Analyses", _controller.GetAllJobs(), RunAnalysis);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiMonitorAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Analyses", _controller.GetAllJobs(), MonitorAnalysis);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        internal void tsmiResultsAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Analyses", _controller.GetAllJobs(), ResultsAnalysis);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiKillAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Analyses", _controller.GetAllJobs(), KillAnalysis);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Analyses", _controller.GetAllJobs(), DeleteAnalyses);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void EditAnalysis(string jobName)
        {
            ShowForm(_frmAnalysis, "Edit Analysis", jobName);
        }
        private void RunAnalysis(string jobName)
        {
            // Check validity
            if (CheckValiditiy())
            {
                string workDirectory = _controller.Settings.GetWorkDirectory();
                //
                if (workDirectory == null || !Directory.Exists(workDirectory))
                    throw new Exception("The work directory of the analysis does not exist.");
                //
                AnalysisJob job = _controller.GetJob(jobName);
                if (job.JobStatus != JobStatus.Running)
                {
                    string inputFileName = Path.Combine(workDirectory, jobName + ".inp");
                    if (File.Exists(inputFileName))
                    {
                        if (MessageBox.Show("Overwrite existing analysis files?",
                                            "Warning",
                                            MessageBoxButtons.OKCancel) != DialogResult.OK) return;
                    }
                    //
                    if (_controller.RunJob(inputFileName, job)) MonitorAnalysis(jobName);
                }
                else MessageBox.Show("The analysis is already running or in queue.", "Error", MessageBoxButtons.OK);
            }
        }
        private void MonitorAnalysis(string jobName)
        {
            try
            {
                CloseAllForms();
                SetFormLoaction(_frmMonitor);
                _frmMonitor.PrepareForm(jobName);
                _frmMonitor.ShowDialog(this);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private async void ResultsAnalysis(string jobName)
        {
            AnalysisJob job = _controller.GetJob(jobName);
            if (job.JobStatus == JobStatus.OK || job.JobStatus == JobStatus.Running ||
                job.JobStatus == JobStatus.FailedWithResults)
            {
                //string resultsFile = Path.GetFileNameWithoutExtension(job.Name) + ".frd";
                string resultsFile = job.Name + ".frd";

                OpenAsync(Path.Combine(job.WorkDirectory, resultsFile), false,
                    () => { if (_controller.Results != null) _frmMonitor.DialogResult = DialogResult.OK; }); // this hides the dialog

                //if (_controller.Results != null) _frmMonitor.DialogResult = DialogResult.OK; // this hides the dialog
                //_frmMonitor.Hide();
            }
            else
            {
                MessageBox.Show("The analysis did not complete.", "Error", MessageBoxButtons.OK);
            }
        }
        private void KillAnalysis(string jobName)
        {
            if (_controller.GetJob(jobName).JobStatus == JobStatus.Running)
            {
                if (MessageBox.Show("OK to kill selected analysis?",
                                    Globals.ProgramName,
                                    MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    _controller.KillJob(jobName);
                }
            }
            else
            {
                MessageBox.Show("The analysis is not running.", "Error", MessageBoxButtons.OK);
            }
        }
        private void DeleteAnalyses(string[] jobNames)
        {
            if (MessageBox.Show("OK to delete selected analyses?" + Environment.NewLine + jobNames.ToRows(),
                                Globals.ProgramName,
                                MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveJobsCommand(jobNames);
            }
        }
        public void UpdateAnalysisProgress()
        {
            _frmMonitor.UpdateProgress();
        }
        //
        
        public AnalysisJob GetDefaultJob()
        {
            try
            {
                List<AnalysisJob> analysisJob = new List<AnalysisJob>();
                InvokeIfRequired(GetDefaultJob, analysisJob);
                return analysisJob[0];
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
                return null;
            }
        }
        private void GetDefaultJob(List<AnalysisJob> defaultJob)
        {
            _frmAnalysis.PrepareForm(null, null);
            defaultJob.Add(_frmAnalysis.Job);
        }

        #endregion  ################################################################################################################

        #region Result part menu  ##################################################################################################

        private void tsmiEditResultParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Parts", _controller.GetResultParts(), EditResultPart);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiHideResultParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetResultParts(), HideResultParts);
                Clear3DSelection();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiShowResultParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetResultParts(), ShowResultParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiShowOnlyResultParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetResultParts(), ShowOnlyResultParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiSetTransparencyForResultParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetResultParts(), SetTransparencyForResultParts);
                Clear3DSelection();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiColorContoursOff_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetResultParts<ResultPart>(), ColorContoursOffResultPart);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiColorContoursOn_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetResultParts<ResultPart>(), ColorContoursOnResultPart);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }

            
        }
        private void tsmiDeleteResultParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetResultParts(), DeleteResultParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void tsmiTransformation_Click(object sender, EventArgs e)
        {
            try
            {
                SinglePointDataEditor.ParentForm = _frmTransformation;
                SinglePointDataEditor.Controller = _controller;
                //
                ShowForm(_frmTransformation, "Create Transformation", null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void EditResultPart(string partName)
        {
            _frmPartProperties.View = ViewGeometryModelResults.Results;
            ShowForm(_frmPartProperties, "Edit Part", partName);
        }
        private void HideResultParts(string[] partNames)
        {
            _controller.HideResultParts(partNames);
        }
        private void ShowResultParts(string[] partNames)
        {
            _controller.ShowResultParts(partNames);
        }
        private void ShowOnlyResultParts(string[] partNames)
        {
            HashSet<string> allNames = new HashSet<string>(_controller.Results.Mesh.Parts.Keys);
            allNames.ExceptWith(partNames);
            _controller.ShowResultParts(partNames);
            _controller.HideResultParts(allNames.ToArray());
        }
        private void SetTransparencyForResultParts(string[] partNames)
        {
            if (_controller.Results == null || _controller.Results.Mesh == null) return;
            //
            using (FrmGetValue frmGetValue = new FrmGetValue())
            {
                frmGetValue.NumOfDigits = 0;
                frmGetValue.MinValue = 25;
                frmGetValue.MaxValue = 255;
                SetFormLoaction(frmGetValue);
                OrderedDictionary<string, double> presetValues = new OrderedDictionary<string, double>();
                presetValues.Add("Semi-transparent", 128);
                presetValues.Add("Opaque", 255);
                string desc = "Enter the transparency between 0 and 255.\n" + "(0 - transparent; 255 - opaque)";
                frmGetValue.PrepareForm("Set Transparency: " + partNames.ToShortString(), "Transparency", desc, 128, presetValues);
                if (frmGetValue.ShowDialog() == DialogResult.OK)
                {
                    _controller.SetTransparencyForResultParts(partNames, (byte)frmGetValue.Value);
                }
                GetFormLoaction(frmGetValue);
            }
        }
        private void ColorContoursOffResultPart(string[] partNames)
        {
            _controller.SetResultPartsColorContoursVisibility(partNames, false);
        }
        private void ColorContoursOnResultPart(string[] partNames)
        {
            _controller.SetResultPartsColorContoursVisibility(partNames, true);
        }
        private void DeleteResultParts(string[] partNames)
        {
            if (MessageBox.Show("OK to delete selected parts?" + Environment.NewLine + partNames.ToRows(),
                                Globals.ProgramName,
                                MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveResultParts(partNames);
            }
        }

        #endregion  ################################################################################################################

        #region Result  ############################################################################################################

        public void ShowHistoryOutput(CaeResults.HistoryResultData historyData)
        {
            try
            {
                if (!_frmHistoryResultsOutput.Visible)
                {
                    CloseAllForms();
                    SetFormLoaction((Form)_frmHistoryResultsOutput);
                    //
                    string[] columnNames;
                    object[][] rowBasedData;
                    _controller.GetHistoryOutputData(historyData, out columnNames, out rowBasedData);
                    //
                    _frmHistoryResultsOutput.SetData(columnNames, rowBasedData);
                    _frmHistoryResultsOutput.Show();
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
       
        #endregion  ################################################################################################################

        #region Help menu  #########################################################################################################
        private void tsmiHomePage_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Globals.HomePage);
        }
        private void tsmiAbout_Click(object sender, EventArgs e)
        {
            FrmSplash frmSplash = new FrmSplash();
            frmSplash.ShowHelp = true;
            frmSplash.ShowDialog();
        }
        #endregion  ################################################################################################################

        #region Selection methods  #################################################################################################

        private void SelectOneEntity(string title, NamedClass[] entities, Action<string> OperateOnEntity)
        {
            if (entities == null || entities.Length == 0) return;
            // Only one entity exists
            if (entities.Length == 1)
            {
                OperateOnEntity(entities[0].Name);
            }
            // Multiple entities exists
            else
            {
                string[] preSelectedEntityNames = _modelTree.IntersectSelectionWithList(entities);
                //
                _frmSelectEntity.Location = new Point(Left + _formLocation.X, Top + _formLocation.Y);
                _frmSelectEntity.PrepareForm(title, false, entities, preSelectedEntityNames, null);
                _frmSelectEntity.OneEntitySelected = OperateOnEntity;
                _frmSelectEntity.Show();
            }
        }
        private void SelectOneEntityInStep(string title, NamedClass[] entities, string stepName,
                                           Action<string, string> OperateOnEntityInStep)
        {
            if (entities == null || entities.Length == 0) return;
            // Only one entity exists
            if (entities.Length == 1)
            {
                OperateOnEntityInStep(stepName, entities[0].Name);
            }
            // Multiple entities exists
            else
            {
                string[] preSelectedEntityNames = _modelTree.IntersectSelectionWithList(entities);
                //
                _frmSelectEntity.Location = new Point(Left + _formLocation.X, Top + _formLocation.Y);
                _frmSelectEntity.PrepareForm(title, false, entities, preSelectedEntityNames, stepName);
                _frmSelectEntity.OneEntitySelectedInStep = OperateOnEntityInStep;
                _frmSelectEntity.Show();
            }
        }
        private void SelectMultipleEntities(string title, NamedClass[] entities, Action<string[]> OperateOnMultpleEntities,
                                            int minNumberOfEntities = 1)
        {
            if (entities == null || entities.Length == 0) return;
            // Only one entity exists
            if (entities.Length == 1)
            {
                if (minNumberOfEntities == 1) OperateOnMultpleEntities(entities.GetNames());
            }
            // Multiple entities exists
            else
            {
                string[] preSelectedEntityNames = _modelTree.IntersectSelectionWithList(entities);
                //
                _frmSelectEntity.Location = new Point(Left + _formLocation.X, Top + _formLocation.Y);
                _frmSelectEntity.PrepareForm(title, true, entities, preSelectedEntityNames, null);
                _frmSelectEntity.MultipleEntitiesSelected = OperateOnMultpleEntities;
                _frmSelectEntity.MinNumberOfEntities = minNumberOfEntities;
                _frmSelectEntity.Show();
            }
        }
        private void SelectMultipleEntitiesInStep(string title, NamedClass[] entities, string stepName,
                                                  Action<string, string[]> OperateOnMultpleEntitiesInStep)
        {
            if (entities == null || entities.Length == 0) return;
            // Only one entity exists
            if (entities.Length == 1)
            {
                OperateOnMultpleEntitiesInStep(stepName, entities.GetNames());
            }
            // Multiple entities exists
            else
            {
                string[] preSelectedEntityNames = _modelTree.IntersectSelectionWithList(entities);
                //
                _frmSelectEntity.Location = new Point(Left + _formLocation.X, Top + _formLocation.Y);
                _frmSelectEntity.PrepareForm(title, true, entities, preSelectedEntityNames, stepName);
                _frmSelectEntity.MultipleEntitiesSelectedInStep = OperateOnMultpleEntitiesInStep;
                _frmSelectEntity.Show();
            }
        }

        #endregion  ################################################################################################################

        #region Mouse selection methods  ###########################################################################################

        public void SelectPointOrArea(double[] pickedPoint, double[] selectionDirection, double[][] planeParameters,
                                      vtkSelectOperation selectOperation, string[] pickedPartNames)
        {
            _controller.SelectPointOrArea(pickedPoint, selectionDirection, planeParameters, selectOperation, pickedPartNames);
            //
            int[] ids = _controller.GetSelectionIds();
            // Must be here since it calls Clear which calls SelectionChanged
            if (_frmSectionView != null && _frmSectionView.Visible) _frmSectionView.PickedIds(ids);
            if (_frmTranslate != null && _frmTranslate.Visible) _frmTranslate.PickedIds(ids);
            if (_frmScale != null && _frmScale.Visible) _frmScale.PickedIds(ids);
            if (_frmRotate != null && _frmRotate.Visible) _frmRotate.PickedIds(ids);
            if (_frmReferencePoint != null && _frmReferencePoint.Visible) _frmReferencePoint.PickedIds(ids);
            if (_frmQuery != null && _frmQuery.Visible) _frmQuery.PickedIds(ids);
            if (_frmTransformation != null && _frmTransformation.Visible) _frmTransformation.PickedIds(ids);
            //
            SelectionChanged(ids);
        }
        public void SelectionChanged(int[] ids = null)
        {
            if (ids == null) ids = _controller.GetSelectionIds();
            //
            if (_frmMeshRefinement != null && _frmMeshRefinement.Visible) _frmMeshRefinement.SelectionChanged(ids);
            if (_frmSelectGeometry != null && _frmSelectGeometry.Visible) _frmSelectGeometry.SelectionChanged(ids);
            //
            if (_frmBoundaryLayer != null && _frmBoundaryLayer.Visible) _frmBoundaryLayer.SelectionChanged(ids);
            if (_frmRemeshingParameters != null && _frmRemeshingParameters.Visible) _frmRemeshingParameters.SelectionChanged(ids);
            if (_frmNodeSet != null && _frmNodeSet.Visible) _frmNodeSet.SelectionChanged(ids);
            if (_frmElementSet != null && _frmElementSet.Visible) _frmElementSet.SelectionChanged(ids);
            if (_frmSurface != null && _frmSurface.Visible) _frmSurface.SelectionChanged(ids);
            if (_frmSection != null && _frmSection.Visible) _frmSection.SelectionChanged(ids);
            if (_frmConstraint != null && _frmConstraint.Visible) _frmConstraint.SelectionChanged(ids);
            if (_frmContactPair != null && _frmContactPair.Visible) _frmContactPair.SelectionChanged(ids);
            //
            if (_frmHistoryOutput != null && _frmHistoryOutput.Visible) _frmHistoryOutput.SelectionChanged(ids);
            if (_frmBoundaryCondition != null && _frmBoundaryCondition.Visible) _frmBoundaryCondition.SelectionChanged(ids);
            if (_frmLoad != null && _frmLoad.Visible) _frmLoad.SelectionChanged(ids);
        }
        public void SetSelectBy(vtkSelectBy selectBy)
        {
            InvokeIfRequired(() => _vtk.SelectBy = selectBy);
        }
        public void SetSelectItem(vtkSelectItem selectItem)
        {
            InvokeIfRequired(() => _vtk.SelectItem = selectItem);
        }
        public void GetGeometryPickProperties(double[] point, out int elementId, out int[] edgeNodeIds,
                                              out int[] cellFaceNodeIds, string[] selectionPartNames = null)
        {
            elementId = -1;
            edgeNodeIds = null;
            cellFaceNodeIds = null;
            try
            {
                if (selectionPartNames != null) _vtk.SetSelectableActorsFilter(selectionPartNames);
                _vtk.GetGeometryPickProperties(point, out elementId, out edgeNodeIds, out cellFaceNodeIds);
            }
            catch { }
            finally
            {
                if (selectionPartNames != null) _vtk.SetSelectableActorsFilter(null);
            }
        }
        public double GetSelectionPrecision()
        {
            return _vtk.GetSelectionPrecision();
        }
        public void GetPointAndCellIdsInsideFrustum(double[][] planeParameters, out int[] pointIds, out int[] cellIds,
                                                    string[] selectionPartNames = null)
        {
            cellIds = null;
            pointIds = null;
            try
            {
                if (selectionPartNames != null) _vtk.SetSelectableActorsFilter(selectionPartNames);
                _vtk.GetPointAndCellIdsInsideFrustum(planeParameters, out pointIds, out cellIds);
            }
            catch { }
            finally
            {
                if (selectionPartNames != null) _vtk.SetSelectableActorsFilter(null);
            }
        }

        #endregion  ################################################################################################################

        private void AddFormToAllForms(Form form)
        {
            form.StartPosition = FormStartPosition.Manual;
            form.Icon = Icon;
            form.Owner = this;
            form.Move += itemForm_Move;
            form.VisibleChanged += itemForm_VisibleChanged;
            _allForms.Add(form);
        }
        private void ShowForm(IFormBase form, string text, string itemToEditName)
        {
            ShowForm(form, text, null, itemToEditName);
        }
        private void ShowForm(IFormBase form, string text, string stepName, string itemToEditName)
        {
            if (!form.Visible)
            {
                CloseAllForms();
                SetFormLoaction((Form)form);
                form.Text = text;
                if (itemToEditName != null) form.Text += ": " + itemToEditName;
                if (form.PrepareForm(stepName, itemToEditName)) form.Show();
            }
        }
        private void SetFormLoaction(Form form)
        {            
            form.Location = new Point(Left + _formLocation.X, Top + _formLocation.Y);
        }
        private void GetFormLoaction(Form form)
        {
            _formLocation.X = form.Location.X - Left;
            _formLocation.Y = form.Location.Y - Top;
        }
        public void CloseAllForms()
        {
            InvokeIfRequired(() =>
            {
               if (_allForms != null)
               {
                    // first hide the _frmSelectItemSet, since it's hiding enables the form it was called from (like _frmNodeSet...)
                    if (_frmSelectItemSet.Visible) _frmSelectItemSet.Hide(DialogResult.Cancel);

                   foreach (var form in _allForms)
                   {
                       if (form.Visible) form.Hide();
                   }
               }
           });
        }

        // Toolbars                                                                                                                 
        #region File toolbar #######################################################################################################
        private void tsbNew_Click(object sender, EventArgs e)
        {
            tsmiNew_Click(null, null);
        }
        private void tsbImport_Click(object sender, EventArgs e)
        {
            tsmiImportFile_Click(null, null);
        }
        private void tsbOpen_Click(object sender, EventArgs e)
        {
            tsmiOpen_Click(null, null);
        }
        private void tsbSave_Click(object sender, EventArgs e)
        {
            tsmiSave_Click(null, null);
        }
        #endregion  ################################################################################################################

        #region View toolbar  ######################################################################################################
        private void tsbFrontView_Click(object sender, EventArgs e)
        {
            tsmiFrontView_Click(null, null);
        }
        private void tsbBackView_Click(object sender, EventArgs e)
        {
            tsmiBackView_Click(null, null);
        }
        private void tsbTopView_Click(object sender, EventArgs e)
        {
            tsmiTopView_Click(null, null);
        }
        private void tsbBottomView_Click(object sender, EventArgs e)
        {
            tsmiBottomView_Click(null, null);
        }
        private void tsbLeftView_Click(object sender, EventArgs e)
        {
            tsmiLeftView_Click(null, null);
        }
        private void tsbRightView_Click(object sender, EventArgs e)
        {
            tsmiRightView_Click(null, null);
        }
        
        private void tsbNormalView_Click(object sender, EventArgs e)
        {
            tsmiNormalView_Click(null, null);
        }
        private void tsbVerticalView_Click(object sender, EventArgs e)
        {
            tsmiVerticalView_Click(null, null);
        }

        private void tsbIsometric_Click(object sender, EventArgs e)
        {
            tsmiIsometricView_Click(null, null);
        }
        
        private void tsbZoomToFit_Click(object sender, EventArgs e)
        {
            tsmiZoomToFit_Click(null, null);
        }

        private void tsbShowWireframeEdges_Click(object sender, EventArgs e)
        {
            tsmiShowWireframeEdges_Click(null, null);
        }
        private void tsbShowElementEdges_Click(object sender, EventArgs e)
        {
            tsmiShowElementEdges_Click(null, null);
        }
        private void tsbShowModelEdges_Click(object sender, EventArgs e)
        {
            tsmiShowModelEdges_Click(null, null);
        }
        private void tsbShowNoEdges_Click(object sender, EventArgs e)
        {
            tsmiShowNoEdges_Click(null, null);
        }

        private void tsbSectionView_Click(object sender, EventArgs e)
        {
            tsmiSectionView_Click(null, null);
        }

        private void tsbHideAllParts_Click(object sender, EventArgs e)
        {
            tsmiHideAllParts_Click(sender, e);
        }
        private void tsbShowAllParts_Click(object sender, EventArgs e)
        {
            tsmiShowAllParts_Click(sender, e);
        }
        private void tsbInvertVisibleParts_Click(object sender, EventArgs e)
        {
            tsmiInvertVisibleParts_Click(sender, e);
        }

        private void tscbSymbolsForStep_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            // If BC or load is selected it will reset the step - clear selection
            _controller.ClearAllSelection();
            //
            _controller.DrawSymbolsForStep(tscbSymbolsForStep.SelectedItem.ToString(), false);  // no need to highlight after clear
        }
        public void UpadteSymbolsForStepList()
        {
            InvokeIfRequired(() =>
            {
                ClearSymbolsDropDown();
                tscbSymbolsForStep.Items.AddRange(_controller.GetStepNames());
                tscbSymbolsForStep.SelectedIndex = tscbSymbolsForStep.Items.Count - 1;
            });
        }
        public void UpadteOneStepInSymbolsForStepList(string oldStepName, string newStepName)
        {
            InvokeIfRequired(() =>
            {
                int index = -1;
                for (int i = 0; i < tscbSymbolsForStep.Items.Count; i++)
                {
                    if (tscbSymbolsForStep.Items[i].ToString() == oldStepName)
                    {
                        index = i;
                        break;
                    }
                }
                if (index != -1)
                {
                    tscbSymbolsForStep.Items[index] = newStepName;
                }
            });
        }
        public void RemoveOneStepInSymbolsForStepList(string stepName)
        {
            InvokeIfRequired(() =>
            {
                int selectedIndex = tscbSymbolsForStep.SelectedIndex;
                //
                int index = -1;
                for (int i = 0; i < tscbSymbolsForStep.Items.Count; i++)
                {
                    if (tscbSymbolsForStep.Items[i].ToString() == stepName)
                    {
                        index = i;
                        break;
                    }
                }
                if (index != -1)
                {
                    tscbSymbolsForStep.Items.RemoveAt(index);
                    if (index == selectedIndex) tscbSymbolsForStep.SelectedIndex = tscbSymbolsForStep.Items.Count - 1;
                }
            });
        }
        public void SelectOneStepInSymbolsForStepList(string stepName)
        {
            InvokeIfRequired(() =>
            {
                int index = -1;
                for (int i = 0; i < tscbSymbolsForStep.Items.Count; i++)
                {
                    if (tscbSymbolsForStep.Items[i].ToString() == stepName)
                    {
                        index = i;
                        break;
                    }
                }
                if (index != -1)
                {
                    tscbSymbolsForStep.SelectedIndexChanged -= tscbSymbolsForStep_SelectedIndexChanged;
                    tscbSymbolsForStep.SelectedIndex = index;
                    tscbSymbolsForStep.SelectedIndexChanged += tscbSymbolsForStep_SelectedIndexChanged;
                    //
                    _controller.DrawSymbolsForStep(tscbSymbolsForStep.SelectedItem.ToString(), false); // do not highlight!
                }
            });
        }


        #endregion  ################################################################################################################

        #region Results field toolbar  #############################################################################################

        private void tsbResultsUndeformed_Click(object sender, EventArgs e)
        {
            tsmiResultsUndeformed_Click(null, null);
        }
        private void tsbResultsDeformed_Click(object sender, EventArgs e)
        {
            tsmiResultsDeformed_Click(null, null);
        }
        private void tsbResultsColorContours_Click(object sender, EventArgs e)
        {
            tsmiResultsColorContours_Click(null, null);
        }
        private void tsbTransformation_Click(object sender, EventArgs e)
        {
            tsmiTransformation_Click(null, null);
        }
        //
        private void FieldOutput_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                CaeResults.FieldData current = _controller.CurrentFieldData;
                SetFieldData(current.Name, current.Component, GetCurrentFieldOutputStepId(), GetCurrentFieldOutputStepIncrementId());
            }
            catch
            { }
        }
        private void tsbPreviousStepIncrement_Click(object sender, EventArgs e)
        {
            try
            {
                if (tscbStepAndIncrement.Enabled && tscbStepAndIncrement.SelectedIndex > 0) 
                    tscbStepAndIncrement.SelectedIndex--;
            }
            catch
            { }
        }
        private void tsbNextStepIncrement_Click(object sender, EventArgs e)
        {
            try
            {
                if (tscbStepAndIncrement.Enabled && tscbStepAndIncrement.SelectedIndex < tscbStepAndIncrement.Items.Count - 1) 
                    tscbStepAndIncrement.SelectedIndex++;
            }
            catch
            { }
        }
        private void tsbFirstStepIncrement_Click(object sender, EventArgs e)
        {
            try
            {
                if (tscbStepAndIncrement.Enabled) tscbStepAndIncrement.SelectedIndex = 0;
            }
            catch
            { }
        }
        private void tsbLastStepIncrement_Click(object sender, EventArgs e)
        {
            try
            {
                if (tscbStepAndIncrement.Enabled) tscbStepAndIncrement.SelectedIndex = tscbStepAndIncrement.Items.Count - 1;
            }
            catch
            { }
        }
        private void tsbAnimate_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.ViewResultsType != ViewResultsType.Undeformed &&
                    _controller.GetResultStepIDs().Length > 0 && !_frmAnimation.Visible)
                {
                    CloseAllForms();
                    DisableEnableControlsForAnimation(false);
                    SetFormLoaction(_frmAnimation);
                    _frmAnimation.PrepareForm(this, _controller);
                    if (_frmAnimation.DialogResult == DialogResult.Abort)
                        DisableEnableControlsForAnimation(true);
                    else
                        _frmAnimation.Show();
                }
            }
            catch
            { }
        }

        private void DisableEnableControlsForAnimation(bool enable)
        {
            // _modelTree.DisableMouse = !enable; this is done in the itemForm_VisibleChanged
            menuStripMain.DisableMouseButtons = !enable;
            tsFile.DisableMouseButtons = !enable;
            tsResults.DisableMouseButtons = !enable;
            tscbStepAndIncrement.Enabled = enable;      // must be here despite the tsResults.DisableMouseButtons = !enable;

            tsbShowAllParts.Enabled = enable;
            tsbHideAllParts.Enabled = enable;
            tsbInvertVisibleParts.Enabled = enable;
        }


        #endregion  ################################################################################################################

        #region Status strip  ######################################################################################################
        public void SetState(string text, bool working)
        {
            InvokeIfRequired(() =>
            {
                tsslState.Text = text;

                if (working) tspbProgress.Style = ProgressBarStyle.Marquee;
                else tspbProgress.Style = ProgressBarStyle.Blocks;

                _vtk.RenderingOn = !working;
                _vtk.Enabled = !working;
                _modelTree.DisableMouse = working;
                menuStripMain.DisableMouseButtons = working;
                tsFile.DisableMouseButtons = working;
                tsViews.DisableMouseButtons = working;
                tsResults.DisableMouseButtons = working;

                //this.DisableAllMouseEvents = working;
            });
        }
        public void SetStateReady(string currentText)
        {
            if (tsslState.Text == currentText) // check that the same command is being canceled
            {
                SetState(Globals.ReadyText, false);
                tsslCancel.Visible = false;
            }
        }
        public bool SetStateWorking(string text, bool showCancelButton = false)
        {
            if (tsslState.Text == Globals.ReadyText) // check that the state is ready
            {
                SetState(text, true);
                tsslCancel.Visible = showCancelButton;
                return true;
            }
            return false;
        }

        private void tsslCancel_MouseDown(object sender, MouseEventArgs e)
        {
            tsslCancel.BorderStyle = Border3DStyle.Sunken;
        }
        private void tsslCancel_MouseUp(object sender, MouseEventArgs e)
        {
            tsslCancel.BorderStyle = Border3DStyle.Raised;
        }
        private void tsslCancel_MouseLeave(object sender, EventArgs e)
        {
            tsslCancel.BorderStyle = Border3DStyle.Raised;
        }
        private void tsslCancel_Click(object sender, EventArgs e)
        {
            _controller.StopNetGenJob();
        }
        #endregion  ################################################################################################################


        // Methods                                                                                                                  
        #region Methods
        public void SetTitle(string title)
        {
            InvokeIfRequired(() => this.Text = title);
        }
        private bool CheckValiditiy()
        {
            string[] invalidItems = _controller.CheckAndUpdateValidity();
            if (invalidItems.Length > 0)
            {
                string text = "The model contains active invlaid items:" + Environment.NewLine;
                foreach (var item in invalidItems) text += Environment.NewLine + item;
                text += Environment.NewLine + Environment.NewLine + "Continue?";
                return MessageBox.Show(text, "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes;
            }
            return true;
        }

        public double[] GetBoundingBox()
        {
            // xmin, xmax, ymin, ymax, zmin, zmax
            return _vtk.GetBoundingBox();
        }
        public double[] GetBondingBoxSize()
        {
            return _vtk.GetBoundingBoxSize();
        }

        public string GetFileNameToImport()
        {
            string fileName = null;
            InvokeIfRequired(() =>
            {
                openFileDialog.Filter = GetFileImportFilter();
                openFileDialog.FileName = "";
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileName = openFileDialog.FileName;
                }
            });
            return fileName;
        }
        public string GetFileNameToSaveAs()
        {
            string fileName = null;
            InvokeIfRequired(() =>
            {
                saveFileDialog.Filter = "PrePoMax files | *.pmx";
                //
                fileName = Path.GetFileName(_controller.OpenedFileName);
                saveFileDialog.FileName = fileName;
                //
                saveFileDialog.OverwritePrompt = true;
                //
                if (saveFileDialog.ShowDialog() == DialogResult.OK) fileName = saveFileDialog.FileName;
                else fileName = null;
            });
            return fileName;
        }
        public string[] GetFileNamesToImport()
        {
            string[] fileNames = null;
            InvokeIfRequired(() =>
            {
                // create new dialog to enable multiFilter
                using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
                {
                    openFileDialog.Multiselect = true;
                    openFileDialog.Filter = GetFileImportFilter();
                    openFileDialog.FileName = "";
                    if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        fileNames = openFileDialog.FileNames;
                    }
                }
            });
            return fileNames;
        }
        private string GetFileImportFilter()
        {
            string filter = "All supported files|*.stp;*.step;*.igs;*.iges;*.brep;*.stl;*.unv;*.vol;*.inp;*.mesh"
                            + "|Step files|*.stp;*.step"
                            + "|Iges files|*.igs;*.iges"
                            + "|Brep files|*.brep"
                            + "|Stereolitography files|*.stl"
                            + "|Universal files|*.unv"
                            + "|Netgen files|*.vol"
                            + "|Abaqus/Calculix inp files|*.inp"
                            + "|Mmg mesh files|*.mesh";
            return filter;
        }

        #region Clear  #############################################################################################################
        public void ClearControls()
        {
            InvokeIfRequired(() =>
            {
                _vtk.Clear();
                _modelTree.Clear();
                outputLines = new string[0];
                tbOutput.Text = "";
                ClearResults();
                ClearAnnotationStatus();
            });
        }
        private void ClearSymbolsDropDown()
        {
            tscbSymbolsForStep.Items.Clear();
            tscbSymbolsForStep.Items.Add("None");
            tscbSymbolsForStep.Items.Add("Model");
            tscbSymbolsForStep.SelectedIndex = 1;
        }
        public void ClearResults()
        {
            InvokeIfRequired(() =>
            {
                tscbStepAndIncrement.Items.Clear();
                _modelTree.ClearResults();
            });
        }
        public void Clear3D()
        {
            InvokeIfRequired(_vtk.Clear);
        }
        public void ClearButKeepParts(string[] partNames)
        {
            InvokeIfRequired(_vtk.ClearButKeepParts, partNames);
        }
        public void ClearSelection()
        {
            _controller.ClearSelectionHistoryAndCallSelectionChanged();
        }
        public void Clear3DSelection()
        {
            InvokeIfRequired(() => _vtk.ClearSelection());
        }
        public void ClearActiveTreeSelection()
        {
            InvokeIfRequired(_modelTree.ClearActiveTreeSelection);
        }
        private void ClearAnnotationStatus()
        {
            tsmiAnnotateFaceOrientations.Checked = false;
            tsmiAnnotateParts.Checked = false;
            tsmiAnnotateMaterials.Checked = false;
            tsmiAnnotateSections.Checked = false;
            tsmiAnnotateSectionThicknesses.Checked = false;
            tsmiAnnotateReferencePoints.Checked = false;
            tsmiAnnotateConstraints.Checked = false;
            tsmiAnnotateContactPairs.Checked = false;
            tsmiAnnotateBCs.Checked = false;
            tsmiAnnotateLoads.Checked = false;
            tsmiAnnotateAllSymbols.Checked = false;
            //
            HideColorBar();
        }

        #endregion  ################################################################################################################

        #region vtkControl  ########################################################################################################
        // vtkControl
        public void SetFrontBackView(bool animate, bool front)
        {
            InvokeIfRequired(_vtk.SetFrontBackView, animate, front);
        }
        public void SetZoomToFit(bool animate)
        {
            InvokeIfRequired(_vtk.SetZoomToFit, animate);
        }
        public double[] GetViewPlaneNormal()
        {
            if (this.InvokeRequired)
            {
                return (double[])this.Invoke((MethodInvoker)delegate () { _vtk.GetViewPlaneNormal(); });
            }
            else
            {
                return _vtk.GetViewPlaneNormal();
            }
        }
        public void AdjustCameraDistanceAndClipping()
        {
            InvokeIfRequired(_vtk.AdjustCameraDistanceAndClipping);
        }
        // Section view
        public void ApplySectionView(double[] point, double[] normal)
        {
            InvokeIfRequired(_vtk.ApplySectionView, point, normal);
        }
        public void UpdateSectionView(double[] point, double[] normal)
        {
            InvokeIfRequired(_vtk.UpdateSectionView, point, normal);
        }
        public void RemoveSectionView()
        {
            InvokeIfRequired(_vtk.RemoveSectionView);
        }
        // Transforms
        
        public void AddSymetry(int symetryPlane, double[] symetryPoint)
        {
            InvokeIfRequired(_vtk.AddSymetry, symetryPlane, symetryPoint);
        }
        public void AddLinearPattern(double[] displacement, int numOfItems)
        {
            InvokeIfRequired(_vtk.AddLinearPattern, displacement, numOfItems);
        }
        public void AddCircularPattern(double[] axisPoint, double[] axisNormal, double angle, int numOfItems)
        {
            InvokeIfRequired(_vtk.AddCircularPattern, axisPoint, axisNormal, angle, numOfItems);
        }
        public void ApplyTransforms()
        {
            InvokeIfRequired(_vtk.ApplyTransforms);
        }
        //
        public void Add3DNodes(vtkControl.vtkMaxActorData actorData)
        {
            InvokeIfRequired(_vtk.AddPoints, actorData);
        }
        public void Add3DCells(vtkControl.vtkMaxActorData cellData)
        {
            InvokeIfRequired(_vtk.AddCells, cellData);
        }
        public void AddScalarFieldOn3DCells(vtkControl.vtkMaxActorData actorData)
        {
            InvokeIfRequired(_vtk.AddScalarFieldOnCells, actorData);
        }
        public bool AddAnimatedScalarFieldOn3DCells(vtkControl.vtkMaxActorData actorData)
        {
            return _vtk.AddAnimatedScalarFieldOnCells(actorData);
        }
        public void UpdateActorSurfaceScalarField(string actorName, float[] values, NodesExchangeData extremeNodes,
                                                  float[] frustumCellLocatorValues)
        {
            InvokeIfRequired(_vtk.UpdateActorScalarField, actorName, values, extremeNodes, frustumCellLocatorValues);
        }
        public void UpdateActorColorContoursVisibility(string[] actorNames, bool colorContour)
        {
            InvokeIfRequired(_vtk.UpdateActorsColorContoursVisibility, actorNames, colorContour);
        }
        public void AddSphereActor(vtkControl.vtkMaxActorData actorData, double symbolSize)
        {
            InvokeIfRequired(_vtk.AddSphereActor, actorData, symbolSize);
        }
        public void AddOrientedDisplacementConstraintActor(vtkControl.vtkMaxActorData actorData, double symbolSize)
        {
            InvokeIfRequired(_vtk.AddOrientedDisplacementConstraintActor, actorData, symbolSize);
        }
        public void AddOrientedRotationalConstraintActor(vtkControl.vtkMaxActorData actorData, double symbolSize)
        {
            InvokeIfRequired(_vtk.AddOrientedRotationalConstraintActor, actorData, symbolSize);
        }
        public void AddOrientedArrowsActor(vtkControl.vtkMaxActorData actorData, double symbolSize, bool invert = false)
        {
            InvokeIfRequired(_vtk.AddOrientedArrowsActor, actorData, symbolSize, invert);
        }
        public void AddOrientedDoubleArrowsActor(vtkControl.vtkMaxActorData actorData, double symbolSize)
        {
            InvokeIfRequired(_vtk.AddOrientedDoubleArrowsActor, actorData, symbolSize);
        }


        public bool ContainsActor(string actorName)
        {
            return _vtk.ContainsActor(actorName);
        }
        public void HighlightActor(string actorName)
        {
            InvokeIfRequired(_vtk.HighlightActor, actorName);
        }
        public void UpdateActor(string oldName, string newName, Color newColor)
        {
            InvokeIfRequired(_vtk.UpdateActor, oldName, newName, newColor);
        }
        public void HideActors(string[] actorNames, bool updateColorContours)
        {
            InvokeIfRequired(_vtk.HideActors, actorNames, updateColorContours);
        }
        public void ShowActors(string[] actorNames, bool updateColorContours)
        {
            InvokeIfRequired(_vtk.ShowActors, actorNames, updateColorContours);
        }
        // Settings                                             
        public void SetCoorSysVisibility(bool visibility)
        {
            InvokeIfRequired(_vtk.SetCoorSysVisibility, visibility);
        }
        // Scale bar
        public void SetScaleWidgetVisibility(bool visibility)
        {
            InvokeIfRequired(_vtk.SetScaleWidgetVisibility, visibility);
        }
        private void SetScaleWidgetUnit(UnitSystem unitSystem)
        {
            string unit = "";
            if (unitSystem.UnitSystemType != UnitSystemType.Undefined) unit = unitSystem.LengthUnitAbbreviation;
            //
            InvokeIfRequired(_vtk.SetScaleWidgetUnit, unit);
        }
        // Scalar bar
        public void InitializeResultWidgetPositions()
        {
            InvokeIfRequired(_vtk.InitializeResultWidgetPositions);
        }
        public void SetScalarBarColorSpectrum(vtkControl.vtkMaxColorSpectrum colorSpectrum)
        {
            InvokeIfRequired(_vtk.SetScalarBarColorSpectrum, colorSpectrum);
        }
        public void SetScalarBarNumberFormat(string numberFormat)
        {
            InvokeIfRequired(_vtk.SetScalarBarNumberFormat, numberFormat);
        }
        public void SetScalarBarText(string fieldName, string componentName, string unitAbbreviation, string minMaxType)
        {
            InvokeIfRequired(_vtk.SetScalarBarText, fieldName, componentName, unitAbbreviation, minMaxType);
        }
        public void DrawLegendBackground(bool drawBackground)
        {
            InvokeIfRequired(_vtk.DrawScalarBarBackground, drawBackground);
        }
        public void DrawLegendBorder(bool drawBorder)
        {
            InvokeIfRequired(_vtk.DrawScalarBarBorder, drawBorder);
        }
        // Color bar
        public void InitializeColorBarWidgetPosition()
        {
            InvokeIfRequired(_vtk.InitializeColorBarWidgetPosition);
        }
        public void SetColorBarColorsAndLabels(Color[] colors, string[] labels)
        {
            InvokeIfRequired(_vtk.SetColorBarColorsAndLabels, colors, labels);
        }
        public void AddColorBarColorsAndLabels(Color[] colors, string[] labels)
        {
            InvokeIfRequired(_vtk.AddColorBarColorsAndLabels, colors, labels);
        }
        public void DrawColorBarBackground(bool drawBackground)
        {
            InvokeIfRequired(_vtk.DrawColorBarBackground, drawBackground);
        }
        public void DrawColorBarBorder(bool drawBorder)
        {
            InvokeIfRequired(_vtk.DrawColorBarBorder, drawBorder);
        }
        public void HideColorBar()
        {
            InvokeIfRequired(_vtk.HideColorBar);
        }
        // Status bar
        public void DrawStatusBlockBackground(bool drawBackground)
        {
            InvokeIfRequired(_vtk.DrawStatusBlockBackground, drawBackground);
        }
        public void DrawStatusBlockBorder(bool drawBorder)
        {
            InvokeIfRequired(_vtk.DrawStatusBlockBorder, drawBorder);
        }
        public void SetStatusBlock(string name, DateTime dateTime, float analysisTime, string unit,
                                   float scaleFactor, vtkControl.DataFieldType fieldType, int modeNumber)
        {
            InvokeIfRequired(_vtk.SetStatusBlock, name, dateTime, analysisTime, unit, scaleFactor,
                             fieldType, modeNumber);
        }
        // General
        public void SetBackground(bool gradient, Color topColor, Color bottomColor, bool redraw)
        {
            InvokeIfRequired(_vtk.SetBackground, gradient, topColor, bottomColor, redraw);
        }
        public void SetLighting(double ambient, double diffuse, bool redraw)
        {
            InvokeIfRequired(_vtk.SetLighting, ambient, diffuse, redraw);
        }
        public void SetSmoothing(bool pointSmoothing, bool lineSmoothing, bool redraw)
        {
            InvokeIfRequired(_vtk.SetSmoothing, pointSmoothing, lineSmoothing, redraw);
        }
        // Highlight
        public void SetHighlightColor(Color primaryHighlightColor, Color secundaryHighlightColor)
        {
            InvokeIfRequired(_vtk.SetHighlightColor, primaryHighlightColor, secundaryHighlightColor);
        }
        public void SetMouseHighlightColor(Color mousehighlightColor)
        {
            InvokeIfRequired(_vtk.SetMouseHighlightColor, mousehighlightColor);
        }
        // Symbols
        public void SetDrawSymbolEdges(bool drawSilhouettes)
        {
            InvokeIfRequired(_vtk.SetDrawSymbolEdges, drawSilhouettes);
        }
        // Min / Max
        public void SetShowMinValueLocation(bool show)
        {
            InvokeIfRequired(() => _vtk.ShowMinValueLocation = show);
        }
        public void SetShowMaxValueLocation(bool show)
        {
            InvokeIfRequired(() => _vtk.ShowMaxValueLocation = show);
        }
        //
        public void CropPartWithCylinder(string partName, double r, string fileName)
        {
            InvokeIfRequired(_vtk.CropPartWithCylinder, partName, r, fileName);
        }

        #endregion  ################################################################################################################

        #region Results  ###########################################################################################################
        // Results
        public void SetFieldData(string name, string component, int stepId, int stepIncrementId)
        {
            CaeResults.FieldData fieldData = new CaeResults.FieldData(name, component, stepId, stepIncrementId);
            CaeResults.FieldData currentData = _controller.CurrentFieldData;
            // In case the currentData is null exit
            if (currentData == null) return;
            //
            if (!fieldData.Equals(currentData)) // update results only if field data changed
            {
                // stop and update animation data only if field data changed
                if (_frmAnimation.Visible) _frmAnimation.Hide();

                if (fieldData.Name == currentData.Name && fieldData.Component == currentData.Component)
                {
                    // the step id or increment id changed                                              

                    // find the choosen data; also contains info about type of step ...
                    fieldData = _controller.Results.GetFieldData(fieldData.Name,
                                                                 fieldData.Component,
                                                                 fieldData.StepId,
                                                                 fieldData.StepIncrementId);
                    // update controller field data
                    _controller.CurrentFieldData = fieldData;
                    // draw deformation or field data
                    if (_controller.ViewResultsType != ViewResultsType.Undeformed) _controller.DrawResults(false);
                }
                else
                {
                    // field of field component changed                                                 

                    // update controller field data; this is used for the SetStepAndIncrementIds to detect missing ids
                    _controller.CurrentFieldData = fieldData;
                    // find all step and step increments
                    SetAllStepAndIncrementIds();
                    // find the existing choosen data; also contains info about type of step ...
                    fieldData = _controller.Results.GetFieldData(fieldData.Name,
                                                                 fieldData.Component,
                                                                 fieldData.StepId,
                                                                 fieldData.StepIncrementId);
                    // update controller field data
                    _controller.CurrentFieldData = fieldData;
                    // draw field data
                    if (_controller.ViewResultsType == ViewResultsType.ColorContours) _controller.UpdatePartsScalarFields();
                }
                // Move focus from step and step increment dropdown menus
                this.ActiveControl = null;
            }
        }
        public void SetAllStepAndIncrementIds()
        {
            InvokeIfRequired(() =>
            {
                // Save current step and increment id
                string currentStepIncrement = (string)tscbStepAndIncrement.SelectedItem;
                string[] prevStepIncrementIds = null;
                if (currentStepIncrement != null)
                    prevStepIncrementIds = currentStepIncrement.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                // Set all increments
                tscbStepAndIncrement.SelectedIndexChanged -= FieldOutput_SelectionChanged;  // detach event
                tscbStepAndIncrement.Items.Clear();
                Dictionary<int, int[]> allIds = _controller.GetResultExistingIncrementIds(_controller.CurrentFieldData.Name,
                                                                                          _controller.CurrentFieldData.Component);
                int lastStepId = 1;
                int lastIncrementId = 0;
                foreach (var entry in allIds)
                {
                    foreach (int incrementId in entry.Value)
                    {
                        tscbStepAndIncrement.Items.Add(entry.Key.ToString() + ", " + incrementId);
                        lastIncrementId = incrementId;
                    }
                    lastStepId = entry.Key;
                }
                tscbStepAndIncrement.SelectedIndexChanged += FieldOutput_SelectionChanged;  // reattach event
                // Reselect previous step and increment
                if (prevStepIncrementIds != null)
                {
                    int stepId = Math.Min(int.Parse(prevStepIncrementIds[0]), lastStepId);
                    int incrementId = Math.Min(int.Parse(prevStepIncrementIds[1]), lastIncrementId);
                    SetStepAndIncrementIds(stepId, incrementId);
                }
                else SetDefaultStepAndIncrementIds();
            });
        }
        
        public void SetStepAndIncrementIds(int stepId, int incrementId)
        {
            InvokeIfRequired(() =>
            {
                string stepIncrement = stepId + ", " + incrementId;
                // Set the combobox
                if (tscbStepAndIncrement.Items.Contains(stepIncrement))
                {
                    tscbStepAndIncrement.SelectedIndexChanged -= FieldOutput_SelectionChanged;
                    tscbStepAndIncrement.SelectedItem = stepIncrement;
                    // Set the step and increment if the combobox set was successful
                    CaeResults.FieldData data = _controller.CurrentFieldData;
                    data.StepId = stepId;
                    data.StepIncrementId = incrementId;
                    _controller.CurrentFieldData = data;   // to correctly update the increment time
                    tscbStepAndIncrement.SelectedIndexChanged += FieldOutput_SelectionChanged;
                }
                else SetDefaultStepAndIncrementIds();
            });
        }
        public void SetDefaultStepAndIncrementIds()
        {
            string[] tmp;
            CaeResults.FieldData fieldData = _controller.CurrentFieldData;
            SetStepAndIncrementIds(fieldData.StepId, fieldData.StepIncrementId);
            return;

            if (_controller.CurrentFieldData.Type == CaeResults.StepType.Frequency)
            {
                string firstStepIncrement = (string)tscbStepAndIncrement.Items[tscbStepAndIncrement.Items.Count - 1];
            }
            else
            {
                string lastStepIncrement = (string)tscbStepAndIncrement.Items[tscbStepAndIncrement.Items.Count - 1];
                tmp = lastStepIncrement.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                
            }
        }
        public int GetCurrentFieldOutputStepId()
        {
            string selectedStepIncrement = (string)tscbStepAndIncrement.SelectedItem;
            string[] tmp = selectedStepIncrement.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
            return int.Parse(tmp[0]);
        }
        public int GetCurrentFieldOutputStepIncrementId()
        {
            string selectedStepIncrement = (string)tscbStepAndIncrement.SelectedItem;
            string[] tmp = selectedStepIncrement.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
            return int.Parse(tmp[1]);
        }
        public void SetAnimationAcceleration(bool animationAcceleration)
        {
            InvokeIfRequired(_vtk.SetAnimationAcceleration, animationAcceleration);
        }
        public void SetAnimationFrameData(float[] time, float[] scale, double[] allFramesScalarRange)
        {
            InvokeIfRequired(_vtk.SetAnimationFrameData, time, scale, allFramesScalarRange);
        }
        public void SetAnimationFrame(int frameNum, bool scalarRangeFromAllFrames)
        {
            InvokeIfRequired(_vtk.SetAnimationFrame, frameNum, scalarRangeFromAllFrames);
        }
        public void SaveAnimationAsAVI(string fileName, int[] firstLastFrame, int step, int fps, bool scalarRangeFromAllFrames,
                                       bool swing, bool encoderOptions)
        {
            InvokeIfRequired(_vtk.SaveAnimationAsAVI, fileName, firstLastFrame, step, fps, scalarRangeFromAllFrames,
                             swing, encoderOptions);
        }
        public void SaveAnimationAsImages(string fileName, int[] firstLastFrame, int step, bool scalarRangeFromAllFrames, bool swing)
        {
            InvokeIfRequired(_vtk.SaveAnimationAsImages, fileName, firstLastFrame, step, scalarRangeFromAllFrames, swing);
        }
        
        #endregion  ################################################################################################################
        
        #region Tree  ##############################################################################################################
        // Tree
        public void RegenerateTree(CaeModel.FeModel model, OrderedDictionary<string, CaeJob.AnalysisJob> jobs,
                                   CaeResults.FeResults results, CaeResults.HistoryResults history)
        {
            InvokeIfRequired(_modelTree.RegenerateTree, model, jobs, results, history);
            InvokeIfRequired(UpadteSymbolsForStepList);
        }
        public void AddTreeNode(ViewGeometryModelResults view, NamedClass item, string stepName)
        {
            ViewType viewType;
            if (view == ViewGeometryModelResults.Geometry) viewType = ViewType.Geometry;
            else if (view == ViewGeometryModelResults.Model) viewType = ViewType.Model;
            else if (view == ViewGeometryModelResults.Results) viewType = ViewType.Results;
            else throw new NotSupportedException();
            //
            InvokeIfRequired(_modelTree.AddTreeNode, viewType, item, stepName);
            if (item is Step) UpadteSymbolsForStepList();
        }
        public void UpdateTreeNode(ViewGeometryModelResults view, string oldItemName, NamedClass item, string stepName,
                                   bool updateSelection = true)
        {
            ViewType viewType;
            if (view == ViewGeometryModelResults.Geometry) viewType = ViewType.Geometry;
            else if (view == ViewGeometryModelResults.Model) viewType = ViewType.Model;
            else if (view == ViewGeometryModelResults.Results) viewType = ViewType.Results;
            else throw new NotSupportedException();

            InvokeIfRequired(_modelTree.UpdateTreeNode, viewType, oldItemName, item, stepName, updateSelection);
            if (item is CaeModel.Step) UpadteOneStepInSymbolsForStepList(oldItemName, item.Name);
        }
        public void RemoveTreeNode<T>(ViewGeometryModelResults view, string nodeName, string stepName) where T : NamedClass
        {
            ViewType viewType;
            if (view == ViewGeometryModelResults.Geometry) viewType = ViewType.Geometry;
            else if (view == ViewGeometryModelResults.Model) viewType = ViewType.Model;
            else if (view == ViewGeometryModelResults.Results) viewType = ViewType.Results;
            else throw new NotSupportedException();
            //
            InvokeIfRequired(_modelTree.RemoveTreeNode<T>, viewType, nodeName, stepName);
            if (typeof(T) == typeof(CaeModel.Step)) RemoveOneStepInSymbolsForStepList(nodeName);
        }
        public bool[] GetTreeExpandCollapseState()
        {
            if (this.InvokeRequired)
            {
                return (bool[])this.Invoke((MethodInvoker)delegate () { _modelTree.GetTreeExpandCollapseState(); });
            }
            else
            {
                return _modelTree.GetTreeExpandCollapseState();
            }
        }
        public void SetTreeExpandCollapseState(bool[] states)
        {
            InvokeIfRequired(_modelTree.SetTreeExpandCollapseState, states);
        }
        public void UpdateHighlight()
        {
            if (_allForms == null) return;
            //
            List<IFormHighlight> highlightForms = new List<IFormHighlight>();
            foreach (var aForm in _allForms)
            {
                // Do not count the Query form
                if (aForm.Visible && (aForm is IFormHighlight ihf)) highlightForms.Add(ihf);
            }
            if (highlightForms.Count == 0) UpdateHighlightFromTree();
            else if (highlightForms.Count == 1) highlightForms[0].Highlight();
            else throw new NotSupportedException();
        }
        public void UpdateHighlightFromTree()
        {
            InvokeIfRequired(_modelTree.UpdateHighlight);
        }
        public void SelectBaseParts(MouseEventArgs e, Keys modifierKeys, string[] partNames)
        {
            // This is called from _vtk on part selection
            //if (!_modelTree.DisableMouse)
            {
                if ((partNames != null && partNames.Length == 0) || 
                    (partNames != null && partNames.Length > 0 && partNames[0] == null))
                {
                    if (modifierKeys != Keys.Shift && modifierKeys != Keys.Control) _controller.ClearAllSelection();
                }
                else
                {
                    int count = 0;
                    BasePart part;
                    int numOfSelectedTreeNodes = 0;
                    //
                    foreach (var partName in partNames)
                    {
                        if (GetCurrentView() == ViewGeometryModelResults.Geometry) part = _controller.GetGeometryPart(partName);
                        else if (GetCurrentView() == ViewGeometryModelResults.Model) part = _controller.GetModelPart(partName);
                        else if (GetCurrentView() == ViewGeometryModelResults.Results) part = _controller.GetResultPart(partName);
                        else throw new NotSupportedException();
                        //
                        numOfSelectedTreeNodes = _modelTree.SelectBasePart(e, modifierKeys, part);
                        count++;
                        //
                        if (count == 1 && modifierKeys == Keys.None) modifierKeys |= Keys.Shift;
                    }
                    //
                    if (numOfSelectedTreeNodes > 0 && e.Button == MouseButtons.Right)
                    {
                        _modelTree.ShowContextMenu(_vtk, e.X, _vtk.Height - e.Y);
                    }
                }
            }
        }

        public void SetNumberOfModelUserKeywords(int numOfUserKeywords)
        {
            InvokeIfRequired(_modelTree.SetNumberOfUserKeywords, numOfUserKeywords);
        }
        
        #endregion  ################################################################################################################

        // Output
        public void WriteDataToOutput(string data)
        {
            if (data == null) return;
            // 20 chars is an empty line with date
            if (data.Length == 0 && (outputLines.Length > 0 && outputLines.Last().Length == 20)) return;

            InvokeIfRequired(() =>
            {
                data = data.Replace("\r\n", "\n");
                data = data.Replace('\r', '\n');
                string[] lines = data.Split('\n');

                foreach (var line in lines) WriteLineToOutputWithDate(line);

                timerOutput.Start();
            });
            
            
        }
        private void WriteLineToOutputWithDate(string data)
        {
            int numColDate = 20;
            int numCol = 150 + numColDate;
            int numRows = 100;      // number of displayed lines

            List<string> lines = new List<string>(outputLines);
            List<string> wrappedLines = new List<string>();

            while (numColDate + data.Length > numCol)
            {
                wrappedLines.Add(data.Substring(0, numCol - numColDate) + "...");
                data = data.Substring(numCol - numColDate);
            }
            wrappedLines.Add(data);

            foreach (var wrappedLine in wrappedLines)
            {
                lines.Add(DateTime.Now.ToString("MM/dd/yy HH:mm:ss").PadRight(numColDate) + wrappedLine);
            }

            int firstLine = Math.Max(0, lines.Count - numRows);
            int numLines = Math.Min(lines.Count, numRows);

            outputLines = new string[numLines];
            Array.Copy(lines.ToArray(), firstLine, outputLines, 0, numLines);
        }

        #region Invoke  ############################################################################################################
        // Invoke
        public void InvokeIfRequired(Action action)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { action(); });
            }
            else
            {
                action();
            }
        }
        public void InvokeIfRequired<T>(Action<T> action, T parameter)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { action(parameter); });
            }
            else
            {
                action(parameter);
            }
        }
        public void InvokeIfRequired<T1,T2>(Action<T1, T2> action, T1 parameter1, T2 parameter2)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { action(parameter1, parameter2); });
            }
            else
            {
                action(parameter1, parameter2);
            }
        }
        public void InvokeIfRequired<T1, T2, T3>(Action<T1, T2, T3> action, T1 parameter1, T2 parameter2, T3 parameter3)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { action(parameter1, parameter2, parameter3); });
            }
            else
            {
                action(parameter1, parameter2, parameter3);
            }
        }
        public void InvokeIfRequired<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 parameter1, T2 parameter2, T3 parameter3, 
                                                     T4 parameter4)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { action(parameter1, parameter2, parameter3, parameter4); });
            }
            else
            {
                action(parameter1, parameter2, parameter3, parameter4);
            }
        }
        public void InvokeIfRequired<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 parameter1, T2 parameter2, T3 parameter3,
                                    T4 parameter4, T5 parameter5)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { action(parameter1, parameter2, parameter3, parameter4, parameter5); });
            }
            else
            {
                action(parameter1, parameter2, parameter3, parameter4, parameter5);
            }
        }
        public void InvokeIfRequired<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 parameter1, T2 parameter2, 
                                     T3 parameter3, T4 parameter4, T5 parameter5, T6 parameter6)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate () { action(parameter1, parameter2, parameter3, parameter4, 
                                                         parameter5, parameter6); });
            }
            else
            {
                action(parameter1, parameter2, parameter3, parameter4, parameter5, parameter6);
            }
        }
        public void InvokeIfRequired<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 parameter1, 
                                     T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5, T6 parameter6, T7 parameter7)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate () { action(parameter1, parameter2, parameter3, parameter4, parameter5,
                                                         parameter6, parameter7); });
            }
            else
            {
                action(parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7);
            }
        }



































        #endregion  ################################################################################################################

        #endregion

        private void tsmiTest_Click(object sender, EventArgs e)
        {
            //if (timerTest.Enabled) timerTest.Stop();
            //else timerTest.Start();
            _vtk.SwithchLights();
        }

        private void timerTest_Tick(object sender, EventArgs e)
        {
            try
            {
                timerTest.Interval = 10;
                //timerTest.Stop();

                string[] names = new string[] { "STRESS", "DISP" };
                string[] components = new string[] { "SZZ", "D2" };

                CaeResults.FieldData currentData = _controller.CurrentFieldData;
                int i = 0;
                if (currentData.Component == components[i]) i++;

                int len = names.Length;

                SetFieldData(names[i % len], components[i % len], 1, 1);
            }
            catch
            {}
        }

        internal void tsmiAdvisor_Click(object sender, EventArgs e)
        {
            // Change the wizzard check state
            tsmiAdvisor.Checked = !tsmiAdvisor.Checked;
            // Add wizard panel
            if (tsmiAdvisor.Checked == true)
            {
                Control parent = panelControl.Parent;
                if (parent == splitContainer2.Panel1)
                {
                    // First remove the vtk comtrol and panel border
                    parent.Controls.Remove(_vtk);
                    parent.Controls.Remove(panelControl);
                    // Split container
                    SplitContainer splitContainer = new SplitContainer();
                    splitContainer.FixedPanel = FixedPanel.Panel2;
                    splitContainer.Dock = DockStyle.Fill;
                    parent.Controls.Add(splitContainer);
                    // Set the Panel 2 size - min 100 max 300
                    splitContainer.SplitterDistance = Math.Max(100, Math.Max(parent.Width - 300, (int)(parent.Width * 0.8)));
                    // Panel 1 - LEFT
                    splitContainer.Panel1.Controls.Add(_vtk);
                    splitContainer.Panel1.Controls.Add(panelControl);
                    panelControl.SendToBack();
                    // Update vtk control size
                    UpdateVtkControlSize();
                    // Panel 2 - RIGHT
                    _advisorControl = AdvisorCreator.CreateControl(this);
                    splitContainer.Panel2.Controls.Add(_advisorControl);
                    _advisorControl.Dock = DockStyle.Fill;
                    _advisorControl.UpdateDesign();
                    _advisorControl.AutoScroll = true;
                }
            }
            // Remove wizard panel
            else
            {
                Control parent = panelControl.Parent;
                if (parent is SplitterPanel && parent != splitContainer2.Panel1)
                {
                    // First remove the vtk comtrol and panel border
                    parent.Controls.Remove(_vtk);
                    parent.Controls.Remove(panelControl);
                    // Remove added split container
                    splitContainer2.Panel1.Controls.Clear();
                    _advisorControl = null;
                    // Add controls back
                    splitContainer2.Panel1.Controls.Add(panelControl);
                    splitContainer2.Panel1.Controls.Add(_vtk);
                    panelControl.SendToBack();
                    // Update vtk control size
                    UpdateVtkControlSize();
                }
            }
        }

        
    }
}
