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

        private Point _formLocation;
        private List<Form> _allForms;
        private FrmSectionView _frmSectionView;
        private FrmCalculixKeywordEditor _frmCalculixKeywordEditor;
        private FrmSelectEntity _frmSelectEntity;
        private FrmSelectItemSet _frmSelectItemSet;
        private FrmAnalyzeGeometry _frmAnalyzeGeometry;
        private FrmPartProperties _frmPartProperties;
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



        #endregion  ################################################################################################################

        #region Properties #########################################################################################################
        public Controller Controller { get { return _controller; } set { _controller = value; } }
        public ViewGeometryModelResults GetCurrentView()
        {
            return _controller.CurrentView;
        }
        public void SetCurrentView(ViewGeometryModelResults view)
        {
            InvokeIfRequired(() =>
            {
                if (view == ViewGeometryModelResults.Geometry) _modelTree.SetGeometryTab();
                else if (view == ViewGeometryModelResults.Model) _modelTree.SetModelTab();
                else if (view == ViewGeometryModelResults.Results)
                {
                    _modelTree.SetResultsTab();
                    InitializeWidgetPositions();
                }
                else throw new NotSupportedException();

                SetMenuAndToolStripVisibility();

                this.ActiveControl = null;
            });
        }

        #endregion  ################################################################################################################


        // Constructors                                                                                                             
        public FrmMain(string[] args)
        {
            //System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-us");

            // Default culture settings
            //System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;           // This thread
            //System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;   // All feature threads
            //
            //System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
            //System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
       
            InitializeComponent();

            // Initialize               
            _vtk = null;
            _controller = null;
            _modelTree = null;
            _args = args;           
        }


        // Event handling                                                                                                           
        private void FrmMain_Load(object sender, EventArgs e)
        {
            Text = Globals.ProgramName;
            this.TopMost = true;
            splash = new FrmSplash { TopMost = true };
            var task = Task.Run(() => splash.ShowDialog());

            try
            {
                //
                // vtk
                //
                _vtk = new vtkControl.vtkControl();
                panelControl.Parent.Controls.Add(_vtk);
                panelControl.SendToBack();

                // tree
                this._modelTree = new UserControls.ModelTree();
                this._modelTree.Name = "modelTree";
                this.splitContainer1.Panel1.Controls.Add(this._modelTree);
                this._modelTree.Dock = System.Windows.Forms.DockStyle.Fill;
                this._modelTree.TabIndex = 0;

                _modelTree.GeometryMeshResultsEvent += ModelTree_ViewEvent;
                _modelTree.SelectEvent += ModelTree_Select;
                _modelTree.ClearSelectionEvent += Clear3DSelection;
                _modelTree.CreateEvent += ModelTree_CreateEvent;
                _modelTree.EditEvent += ModelTree_Edit;
                _modelTree.HideShowEvent += ModelTree_HideShowEvent;
                _modelTree.SetTransparencyEvent += ModelTree_SetTransparencyEvent;
                _modelTree.ColorContoursVisibilityEvent += ModelTree_ColorContoursVisibilityEvent; 
                _modelTree.MeshingParametersEvent += ModelTree_MeshingParametersEvent;
                _modelTree.CreateMeshEvent += CreateMeshes;
                _modelTree.CopyGeometryToResultsEvent += CopyGeometryPartsToResults;
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

                // controller
                _controller = new PrePoMax.Controller(this);

                // vtk
                _vtk.OnMouseLeftButtonUpSelection += SelectPointOrArea;
                _vtk.Controller_GetNodeActorData = _controller.GetNodeActorData;
                _vtk.Controller_GetCellActorData = _controller.GetCellActorData;
                _vtk.Controller_GetCellFaceActorData = _controller.GetCellFaceActorData;
                _vtk.Controller_GetEdgeActorData = _controller.GetEdgeActorData;
                _vtk.Controller_GetSurfaceEdgesActorData = _controller.GetSurfaceEdgeActorData;
                _vtk.Controller_GetPartActorData = _controller.GetPartActorData;
                _vtk.Controller_GetGeometryActorData = _controller.GetGeometryActorData;

                _vtk.Controller_ActorPicked = SelectBasePart;
                _vtk.Controller_ShowPostSettings = ShowPostSettings;

                // Forms
                _formLocation = new Point(100, 100);
                _allForms = new List<Form>();

                _frmSelectEntity = new FrmSelectEntity(_controller);
                AddFormToAllForms(_frmSelectEntity);

                _frmSelectItemSet = new FrmSelectItemSet(_controller);
                AddFormToAllForms(_frmSelectItemSet);

                _frmSectionView = new FrmSectionView(_controller);
                AddFormToAllForms(_frmSectionView);

                _frmAnalyzeGeometry = new FrmAnalyzeGeometry(_controller);
                AddFormToAllForms(_frmAnalyzeGeometry);

                _frmPartProperties = new FrmPartProperties(_controller);
                AddFormToAllForms(_frmPartProperties);

                _frmTranslate = new FrmTranslate(_controller);
                AddFormToAllForms(_frmTranslate);

                _frmScale = new FrmScale(_controller);
                AddFormToAllForms(_frmScale);

                _frmRotate = new FrmRotate(_controller);
                AddFormToAllForms(_frmRotate);

                _frmNodeSet = new FrmNodeSet(_controller);
                AddFormToAllForms(_frmNodeSet);

                _frmElementSet = new FrmElementSet(_controller);
                AddFormToAllForms(_frmElementSet);

                _frmSurface = new FrmSurface(_controller);                
                AddFormToAllForms(_frmSurface);

                _frmReferencePoint = new FrmReferencePoint(_controller);
                AddFormToAllForms(_frmReferencePoint);

                _frmMaterial = new FrmMaterial(_controller);
                AddFormToAllForms(_frmMaterial);

                _frmSection = new FrmSection(_controller);
                AddFormToAllForms(_frmSection);

                _frmConstraint = new FrmConstraint(_controller);
                AddFormToAllForms(_frmConstraint);

                _frmStep = new FrmStep(_controller);
                AddFormToAllForms(_frmStep);

                _frmHistoryOutput = new FrmHistoryOutput(_controller);
                AddFormToAllForms(_frmHistoryOutput);

                _frmFieldOutput = new FrmFieldOutput(_controller);
                AddFormToAllForms(_frmFieldOutput);

                _frmBoundaryCondition = new FrmBC(_controller);
                AddFormToAllForms(_frmBoundaryCondition);

                _frmLoad = new FrmLoad(_controller);
                AddFormToAllForms(_frmLoad);

                _frmAnalysis = new FrmAnalysis(_controller);
                AddFormToAllForms(_frmAnalysis);

                _frmMonitor = new FrmMonitor();
                _frmMonitor.KillJob += KillAnalysis;
                _frmMonitor.Results += ResultsAnalysis;
                AddFormToAllForms(_frmMonitor);

                _frmSettings = new FrmSettings();
                _frmSettings.UpdateSettings += UpdateSettings;
                AddFormToAllForms(_frmSettings);

                _frmQuery = new FrmQuery();
                _frmQuery.Form_WriteDataToOutput = WriteDataToOutput;
                AddFormToAllForms(_frmQuery);

                _frmAnimation = new FrmAnimation();
                _frmAnimation.Form_ControlsEnable = DisableEnableControlsForAnimation;
                AddFormToAllForms(_frmAnimation);

                _frmHistoryResultsOutput = new FrmHistoryResultsOutput(_controller);
                AddFormToAllForms(_frmHistoryResultsOutput);

                _vtk.Hide();
                _vtk.Enabled = false;
            }
            catch
            {
                // if no error the splash is closed latter
                splash.BeginInvoke((MethodInvoker)delegate () { splash.Close(); });
            }
            finally
            {
                this.TopMost = false;
            }

            if (!System.Diagnostics.Debugger.IsAttached)
            {
                tsmiTest.Visible = false;
            }
        }

       

        private void FrmMain_Shown(object sender, EventArgs e)
        {
            // Set vtk control size
            _vtk.Location = panelControl.Location;
            _vtk.Top += 1;
            _vtk.Left += 1;

            _vtk.Size = panelControl.Size;
            _vtk.Width -= 2;
            _vtk.Height -= 2;

            _vtk.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // Set pass through control for the mouse wheel event
            this.PassThroughControl = _vtk;

            // Timer to delay the rendering of the vtk so that menus get rendered first
            Timer timer = new Timer();
            timer.Interval = 50;
            timer.Tick += new EventHandler(async (object s, EventArgs ea) =>
            {
                timer.Stop();

                _vtk.Show();
                _vtk.Enabled = true;

                // close splash 
                splash.BeginInvoke((MethodInvoker)delegate () { splash.Close(); });

                // at the end when vtk is loaded open the file
                string fileName = null;
                if (_args != null && _args.Length == 1) fileName = _args[0];
                else
                {
                    // chack for open last file
                    GeneralSettings gs = (GeneralSettings)_controller.Settings[Globals.GeneralSettingsName];
                    if (gs.OpenLastFile) fileName = gs.LastFileName;
                }

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

                        _vtk.SetFrontBackView(false, true);
                    }
                    catch (Exception ex)
                    {                        
                        CaeGlobals.ExceptionTools.Show(this, ex);
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

        private void itemForm_VisibleChanged(object sender, EventArgs e)
        {
            Form form = sender as Form;
            int count = 0;

            // one or two forms can be open
            foreach (var oneForm in _allForms) if (oneForm.Visible) count++;

            // Disable model tree mouse and keyboard actions for the form
            bool unactive;
            if (count > 0) unactive = true;
            else unactive = false;
            _modelTree.DisableMouse = unactive;
            menuStripMain.DisableMouseButtons = unactive;
            tsFile.DisableMouseButtons = unactive;

            // this gets also called from item selection form: by angle, by edge ...
            if (form.Visible == false && form.DialogResult == System.Windows.Forms.DialogResult.Cancel)
                UpdateHighlightFromTree();

            if (form.Visible == false) this.Focus();
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

        private void timerOutput_Tick(object sender, EventArgs e)
        {
            tbOutput.Lines = outputLines;
            tbOutput.SelectionStart = tbOutput.Text.Length;
            tbOutput.ScrollToCaret();
            timerOutput.Stop();
        }

        #region ModelTree Events ###################################################################################################
        private void ModelTree_ViewEvent(ViewType viewType)
        {
            try
            {
                CloseAllForms();
                _controller.SelectBy = vtkSelectBy.Off;

                if (viewType == ViewType.Geometry) _controller.CurrentView = ViewGeometryModelResults.Geometry;
                else if (viewType == ViewType.Model) _controller.CurrentView = ViewGeometryModelResults.Model;
                else if (viewType == ViewType.Results) _controller.CurrentView = ViewGeometryModelResults.Results;
                else throw new NotSupportedException();
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
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

        private void ModelTree_CreateEvent(string nodeName, string stepName)
        {
            if (_controller.Model.Mesh != null && _controller.CurrentView == ViewGeometryModelResults.Model)
            {
                if (nodeName == "Node sets") tsmiCreateNodeSet_Click(null, null);
                else if (nodeName == "Element sets") tsmiCreateElementSet_Click(null, null);
                else if (nodeName == "Surfaces") tsmiCreateSurface_Click(null, null);
                else if (nodeName == "Reference points") tsmiCreateRP_Click(null, null);
                else if (nodeName == "Materials") tsmiCreateMaterial_Click(null, null);
                else if (nodeName == "Sections") tsmiCreateSection_Click(null, null);
                else if (nodeName == "Constraints") tsmiCreateConstraint_Click(null, null);
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
                if (namedClass is CaeMesh.GeometryPart) EditGeometryPart((namedClass).Name);
            }
            // Model
            else if (_controller.CurrentView == ViewGeometryModelResults.Model)
            {
                if (namedClass is EmptyNamedClass) // empty named class is used to trasfer the name only
                {
                    if (namedClass.Name == typeof(CaeModel.FeModel).ToString()) tsmiEditCalculiXKeywords_Click(null, null);
                }
                else if (namedClass is CaeMesh.MeshPart) EditModelPart((namedClass).Name);
                else if (namedClass is CaeMesh.FeNodeSet) EditNodeSet((namedClass).Name);
                else if (namedClass is CaeMesh.FeElementSet) EditElementSet((namedClass).Name);
                else if (namedClass is CaeMesh.FeSurface) EditSurface((namedClass).Name);
                else if (namedClass is CaeMesh.FeReferencePoint) EditRP((namedClass).Name);
                else if (namedClass is CaeModel.Material) EditMaterial((namedClass).Name);
                else if (namedClass is CaeModel.Section) EditSection((namedClass).Name);
                else if (namedClass is CaeModel.Constraint) EditConstraint((namedClass).Name);
                else if (namedClass is CaeModel.Step) EditStep((namedClass).Name);
                else if (namedClass is CaeModel.HistoryOutput) EditHistoryOutput(stepName, (namedClass).Name);
                else if (namedClass is CaeModel.FieldOutput) EditFieldOutput(stepName, (namedClass).Name);
                else if (namedClass is CaeModel.BoundaryCondition) EditBoundaryCondition(stepName, (namedClass).Name);
                else if (namedClass is CaeModel.Load) EditLoad(stepName, (namedClass).Name);
                else if (namedClass is CaeJob.AnalysisJob) EditAnalysis((namedClass).Name);
            }
            // Results
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                if (namedClass is CaeMesh.ResultPart || namedClass is CaeMesh.GeometryPart) EditResultPart((namedClass).Name);
                else if (namedClass is CaeResults.HistoryResultData hd) ShowHistoryOutput(hd);
            }
        }

        private void ModelTree_HideShowEvent(NamedClass[] items, HideShowOperation operation, string[] stepNames)
        {
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry)
            {
                HideShowItems<CaeMesh.GeometryPart>(items, operation, HideGeometryParts, ShowGeometryParts, ShowOnlyGeometryParts);
            }
            else if (_controller.CurrentView == ViewGeometryModelResults.Model)
            {
                HideShowItems<CaeMesh.MeshPart>(items, operation, HideModelParts, ShowModelParts, ShowOnlyModelParts);
                HideShowItems<CaeModel.Constraint>(items, operation, HideConstraints, ShowConstraints, ShowOnlyConstraints);
                HideShowStepItems<CaeModel.BoundaryCondition>(items, operation, stepNames, HideBoundaryConditions, 
                                                              ShowBoundaryConditions, ShowOnlyBoundaryConditions);
                HideShowStepItems<CaeModel.Load>(items, operation, stepNames, HideLoads, ShowLoads, ShowOnlyLoads);
            }
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                HideShowItems<CaeMesh.ResultPart>(items, operation, HideResultParts, ShowResultParts, ShowOnlyResultParts);
                HideShowItems<CaeMesh.GeometryPart>(items, operation, HideResultParts, ShowResultParts, ShowOnlyResultParts);
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

        private void ModelTree_MeshingParametersEvent(string[] geometryPartNames)
        {
            GetSetMeshingParameters(geometryPartNames);
        }
      
        private void ModelTree_Delete(NamedClass[] items, string[] stepNames)
        {
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry)
            {
                DeleteItems<CaeMesh.GeometryPart>(items, DeleteGeometryParts);
            }
            else if (_controller.CurrentView == ViewGeometryModelResults.Model)
            {
                DeleteItems<CaeMesh.MeshPart>(items, DeleteModelParts);
                DeleteItems<CaeMesh.FeNodeSet>(items, DeleteNodeSets);
                DeleteItems<CaeMesh.FeElementSet>(items, DeleteElementSets);
                DeleteItems<CaeMesh.FeSurface>(items, DeleteSurfaces);
                DeleteItems<CaeMesh.FeReferencePoint>(items, DeleteRP);
                DeleteItems<CaeModel.Material>(items, DeleteMaterials);
                DeleteItems<CaeModel.Section>(items, DeleteSections);
                DeleteItems<CaeModel.Constraint>(items, DeleteConstraints);
                DeleteItems<CaeModel.Step>(items, DeleteSteps);

                DeleteStepItems<CaeModel.HistoryOutput>(items, stepNames, DeleteHistoryOutputs);
                DeleteStepItems<CaeModel.FieldOutput>(items, stepNames, DeleteFieldOutputs);
                DeleteStepItems<CaeModel.BoundaryCondition>(items, stepNames, DeleteBoundaryConditions);
                DeleteStepItems<CaeModel.Load>(items, stepNames, DeleteLoads);
                DeleteItems<CaeJob.AnalysisJob>(items, DeleteAnalyses);
            }
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                DeleteItems<CaeMesh.ResultPart>(items, DeleteResultParts);
                DeleteItems<CaeMesh.GeometryPart>(items, DeleteResultParts);
            }

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
        private void HideShowItems<T>(NamedClass[] items, HideShowOperation operation, Action<string[]> Hide, Action<string[]> Show, Action<string[]> ShowOnly)
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

        private void DeleteItems<T>(NamedClass[] items, Action<string[]> Delete)
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
        #endregion #################################################################################################################


        // Menus                                                                                                                    
        private void SetMenuAndToolStripVisibility()
        {
            switch (_controller.CurrentView)
            {
                case ViewGeometryModelResults.Geometry:
                    tsmiGeometry.Enabled = true;
                    tsmiMesh.Enabled = false;
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
                    tsmiMesh.Enabled = true;
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

        #region File menu   ########################################################################################################

        private void tsmiNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.ModelChanged &&
                    MessageBox.Show("OK to close current model?", Globals.ProgramName, MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                _controller.New();
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiOpen_Click(object sender, EventArgs e)
        {
            try
            {                
                using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
                {
                    openFileDialog.Filter = "All files|*.pmx;*.frd;*.dat" +
                                            "|PrePoMax files|*.pmx" + 
                                            "|Calculix result files|*.frd" +
                                            "|Calculix dat files|*.dat";

                    openFileDialog.FileName = "";
                    if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        if (_controller.ModelChanged)
                        {
                            if (Path.GetExtension(openFileDialog.FileName).ToLower() == ".pmx")
                            {
                                if (MessageBox.Show("OK to close current model?", 
                                    Globals.ProgramName, 
                                    MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK) return;
                            }
                            else if (Path.GetExtension(openFileDialog.FileName).ToLower() == ".frd" && _controller.Results != null)
                            {
                                if (MessageBox.Show("OK to overwrite current results?",
                                    Globals.ProgramName,
                                    MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK) return;
                            }
                        }

                        OpenAsync(openFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
                _controller.New();
            }
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
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
            finally
            {
                if (stateSet) SetStateReady(Globals.OpeningText);
            }
        }
        private void Open(string fileName, bool resetCamera = true)
        {
            _controller.Open(fileName);

            if (_controller.Results != null)
            {
                // reset the previous step and increment
                SetAllStepAndIncrementIds();
                // set last increment
                SetLastStepAndIncrementIds();   
            }

            if (_controller.CurrentView == ViewGeometryModelResults.Geometry) _controller.DrawGeometry(resetCamera);
            else if (_controller.CurrentView == ViewGeometryModelResults.Model) _controller.DrawMesh(resetCamera);
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                _controller.ViewResultsType = ViewResultsType.ColorContours; // this calls _controller.DrawResults(resetCamera);
                if (resetCamera) tsmiFrontView_Click(null, null);
            }
            else throw new NotSupportedException();
        }
        private async void tsmiImportFile_Click(object sender, EventArgs e)
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
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.ImportingText);
            }
        }
        private async void tsmiSave_Click(object sender, EventArgs e)
        {
            try
            {
                SetStateWorking(Globals.SavingText);
                await Task.Run(() => _controller.Save());
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.SavingAsText);
            }
        }
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                            _controller.ExportToCalculix(saveFileDialog.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.ExportingText);
            }
        }
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

        #endregion  ################################################################################################################

        #region Edit menu  #########################################################################################################

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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
        private void tsmiEditCalculiXKeywords_Click(object sender, EventArgs e)
        {
            try
            {
                if (CheckValiditiy())
                {
                    _frmCalculixKeywordEditor = new FrmCalculixKeywordEditor();
                    _frmCalculixKeywordEditor.Keywords = _controller.GetCalculixModelKeywords();
                    _frmCalculixKeywordEditor.UserKeywords = _controller.GetCalculixUserKeywords();

                    if (_frmCalculixKeywordEditor.Keywords != null)
                    {
                        if (_frmCalculixKeywordEditor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            _controller.SetCalculixUserKeywordsCommand(_frmCalculixKeywordEditor.UserKeywords);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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

        #region View menu   ########################################################################################################

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

        private void tsmiNormalView_Click(object sender, EventArgs e)
        {
            _vtk.SetNormalView(true);
        }
        private void tsmiVerticalView_Click(object sender, EventArgs e)
        {
            _vtk.SetVerticalView(true, true);
        }

        private void tsmiIsometricView_Click(object sender, EventArgs e)
        {
            _vtk.SetIsometricView(true, true);
        }
        private void tsmiZoomToFit_Click(object sender, EventArgs e)
        {
            SetZoomToFit(true);
        }

        private void tsmiShowWireframeEdges_Click(object sender, EventArgs e)
        {
            _vtk.EdgesVisibility = vtkControl.vtkEdgesVisibility.Wireframe;
            if (_controller.Selection != null && _controller.Selection.Nodes.Count > 0)
                _controller.HighlightSelection();
            else if (!_frmSelectItemSet.Visible)    // if everything is deselectd in _frmSelectItemSet do not highlight from tree
                _modelTree.UpdateHighlight();
        }
        private void tsmiShowElementEdges_Click(object sender, EventArgs e)
        {
            _vtk.EdgesVisibility = vtkControl.vtkEdgesVisibility.ElementEdges;
            if (_controller.Selection != null && _controller.Selection.Nodes.Count > 0)
                _controller.HighlightSelection();
            else if (!_frmSelectItemSet.Visible)    // if everything is deselectd in _frmSelectItemSet do not highlight from tree
                _modelTree.UpdateHighlight();
        }
        private void tsmiShowModelEdges_Click(object sender, EventArgs e)
        {
            _vtk.EdgesVisibility = vtkControl.vtkEdgesVisibility.ModelEdges;
            if (_controller.Selection != null && _controller.Selection.Nodes.Count > 0)
                _controller.HighlightSelection();
            else if (!_frmSelectItemSet.Visible)    // if everything is deselectd in _frmSelectItemSet do not highlight from tree
                _modelTree.UpdateHighlight();
        }
        private void tsmiShowNoEdges_Click(object sender, EventArgs e)
        {
            _vtk.EdgesVisibility = vtkControl.vtkEdgesVisibility.NoEdges;
            if (_controller.Selection != null && _controller.Selection.Nodes.Count > 0)
                _controller.HighlightSelection();
            else if (!_frmSelectItemSet.Visible)    // if everything is deselectd in _frmSelectItemSet do not highlight from tree
                _modelTree.UpdateHighlight();
        }

        private void tsmiSectionView_Click(object sender, EventArgs e)
        {
            SinglePointDataEditor.ParentForm = _frmSectionView;
            SinglePointDataEditor.Controller = _controller;

            ShowForm(_frmSectionView, tsmiSectionView.Text, null);
        }

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
                CaeMesh.BasePart[] parts;
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

        #region Geometry part   ####################################################################################################

        private void tsmiEditGeometryPart_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Parts", _controller.GetGeometryParts(), EditGeometryPart);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiHideGeometryPart_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryParts(), HideGeometryParts);
                Clear3DSelection();
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiShowGeometryPart_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryParts(), ShowGeometryParts);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiSetTransparencyForGeometryPart_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryParts(), SetTransparencyForGeometryParts);
                Clear3DSelection();
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiCopyGeometryPartToResults_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryParts(), CopyGeometryPartsToResults);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteGeometryPart_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryParts(), DeleteGeometryParts);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }

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

            using (FrmGetValue frmGetValue = new FrmGetValue(128))
            {
                frmGetValue.NumOfDigits = 0;
                frmGetValue.MinValue = 25;
                frmGetValue.MaxValue = 255;
                SetFormLoaction(frmGetValue);
                frmGetValue.PrepareForm("Set Transparency", "Transparency", "Enter the transparency between 0 and 255.\n" +
                                                                            "(0 - transparent; 255 - opaque)");
                if (frmGetValue.ShowDialog() == DialogResult.OK)
                {
                    _controller.SetTransparencyForGeometryPartsCommand(partNames,(byte)frmGetValue.Value);
                }
                GetFormLoaction(frmGetValue);
            }
        }
        private void ShowOnlyGeometryParts(string[] partNames)
        {
            HashSet<string> allNames = new HashSet<string>(_controller.Model.Geometry.Parts.Keys);
            allNames.ExceptWith(partNames);
            _controller.ShowGeometryPartsCommand(partNames);
            _controller.HideGeometryPartsCommand(allNames.ToArray());
        }
        private void CopyGeometryPartsToResults(string[] partNames)
        {
            _controller.CopyGeometryPartsToResults(partNames);
        }
        private void DeleteGeometryParts(string[] partNames)
        {
            if (MessageBox.Show("OK to delete selected parts?", Globals.ProgramName, MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveGeometryPartsCommand(partNames);
            }
        }

        // Analyze
        private void tsmiGeometryAnalyze_Click(object sender, EventArgs e)
        {
            try
            {
                var parts = _controller.GetGeometryParts();
                if (parts != null && parts.Length > 0 && !_frmAnalyzeGeometry.Visible)
                {
                    CloseAllForms();
                    SetFormLoaction((Form)_frmAnalyzeGeometry);
                    _frmAnalyzeGeometry.Show();
                }
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
            
        }

        #endregion  ################################################################################################################

        #region Meshing ############################################################################################################
        private void tsmiMeshingParameters_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryParts(), GetSetMeshingParameters);
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
                SelectMultipleEntities("Parts", _controller.GetGeometryParts(), CreateMeshes);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
            
        }
        private void GetSetMeshingParameters(string[] partNames)
        {
            try
            {
                CaeMesh.MeshingParameters meshingParameters = GetMeshingParameters(partNames);
                if (meshingParameters != null) // Cancel pressed on Meshing parameters form
                    _controller.SetMeshingParametersCommand(partNames, meshingParameters);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        public CaeMesh.MeshingParameters GetMeshingParameters(string[] partNames)
        {
            double sumMax = 0;
            double sumMin = 0;
            CaeMesh.GeometryPart part;
            CaeMesh.MeshingParameters meshingParameters = null;
            CaeMesh.MeshingParameters defaultMeshingParameters = null;
            foreach (var partName in partNames)
            {
                part = _controller.GetGeometryPart(partName);
                // default parameters
                defaultMeshingParameters = GetDefaultMeshingParameters(partName);
                // first time set the meshing parameters
                if (partName == partNames[0]) meshingParameters = part.MeshingParameters;
                // meshing parameters exist only when all parts have the same meshing parameters
                if (!CaeMesh.MeshingParameters.Equals(meshingParameters, part.MeshingParameters))
                    meshingParameters = null;

                sumMax += defaultMeshingParameters.MaxH;
                sumMin += defaultMeshingParameters.MinH;
            }
            defaultMeshingParameters.MaxH = Math.Round(sumMax / partNames.Length, 2);
            defaultMeshingParameters.MinH = Math.Round(sumMin / partNames.Length, 2);

            // use meshingParameters as default if meshing parameters are not equal
            if (meshingParameters == null) meshingParameters = defaultMeshingParameters;

            return GetMeshingParametersByForm(partNames.ToShortString(), defaultMeshingParameters, meshingParameters);
        }
        public CaeMesh.MeshingParameters GetDefaultMeshingParameters(string partName)
        {
            CaeMesh.GeometryPart part = _controller.GetGeometryPart(partName);

            if (part.CADFileData == null && part.ErrorElementIds != null)
                throw new Exception("The part '" + partName + "' contains errors and can not be meshed.");
            if (!_controller.MeshJobIdle) throw new Exception("The meshing is already in progress.");

            CaeMesh.MeshingParameters defaultMeshingParameters = new CaeMesh.MeshingParameters();
            double factorMax = 20;
            double factorMin = 1000;
            double maxSize = part.BoundingBox.GetDiagonal();
            defaultMeshingParameters.MaxH = CaeGlobals.Tools.RoundToSignificantDigits(maxSize / factorMax, 2);
            defaultMeshingParameters.MinH = CaeGlobals.Tools.RoundToSignificantDigits(maxSize / factorMin, 2);

            return defaultMeshingParameters;
        }
        private CaeMesh.MeshingParameters GetMeshingParametersByForm(string formName, 
                                                                     CaeMesh.MeshingParameters defaultMeshingParameters,
                                                                     CaeMesh.MeshingParameters meshingParameters)
        {
            FrmMeshingParameters frmMeshingParameters = new FrmMeshingParameters();
            frmMeshingParameters.Icon = Icon;
            frmMeshingParameters.PartName = formName;
            frmMeshingParameters.DefaultMeshingParameters = defaultMeshingParameters;
            frmMeshingParameters.MeshingParameters = meshingParameters;
            frmMeshingParameters.Location = new Point(Left + _formLocation.X, Top + _formLocation.Y);

            if (frmMeshingParameters.ShowDialog() == System.Windows.Forms.DialogResult.OK) return frmMeshingParameters.MeshingParameters;
            else return null; // Cancel pressed on Meshing parameters form
        }
        private async void CreateMeshes(string[] partNames)
        {
            try
            {
                SetStateWorking(Globals.MeshingText, true);
                foreach (var partName in partNames)
                {
                    CaeMesh.GeometryPart part = _controller.GetGeometryPart(partName);
                    if (part.MeshingParameters == null) SetDefaultMeshingParameters(partName);

                     await Task.Run(() => _controller.CreateMeshCommand(partName));
                }
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);                
            }
            finally
            {
                SetStateReady(Globals.MeshingText);
            }
        }
        private void SetDefaultMeshingParameters(string partName)
        {
            CaeMesh.MeshingParameters defaultMeshingParameters = GetDefaultMeshingParameters(partName);
            _controller.SetMeshingParametersCommand(new string[] { partName }, defaultMeshingParameters);
        }


        #endregion  ################################################################################################################

        #region Node menu  #########################################################################################################
        private void tsmiRenumberAllNodes_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;

                using (FrmGetValue frmGetValue = new FrmGetValue(1))
                {
                    frmGetValue.NumOfDigits = 0;
                    frmGetValue.MinValue = 1;
                    SetFormLoaction(frmGetValue);
                    frmGetValue.PrepareForm("Renumber Nodes", "Start node id", "Enter the starting node id " +
                                                                               "for the node renumbering.");
                    if (frmGetValue.ShowDialog() == DialogResult.OK)
                    {
                        _controller.RenumberNodesCommand((int)frmGetValue.Value);
                    }
                    GetFormLoaction(frmGetValue);
                }
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiTranslatePart_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), TranslateParts);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiScalePart_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), ScaleParts);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiRotatePart_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), RotateParts);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiMergePart_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), MergeModelParts);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiHidePart_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), HideModelParts);
                Clear3DSelection();
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiShowPart_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), ShowModelParts);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiSetTransparencyForPart_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), SetTransparencyForModelParts);
                Clear3DSelection();
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeletePart_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), DeleteModelParts);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }

        private void EditModelPart(string partName)
        {
            _frmPartProperties.View = ViewGeometryModelResults.Model; 
            ShowForm(_frmPartProperties, "Edit Part", partName);
        }
        private void TranslateParts(string[] partNames)
        {
            SinglePointDataEditor.ParentForm = _frmTranslate;
            SinglePointDataEditor.Controller = _controller;

            _frmTranslate.PartNames = partNames;    // set all part names for translation
            
            ShowForm(_frmTranslate, "Translate parts: " + partNames.ToShortString(), null);
        }
        private void ScaleParts(string[] partNames)
        {
            SinglePointDataEditor.ParentForm = _frmScale;
            SinglePointDataEditor.Controller = _controller;

            _frmScale.PartNames = partNames;    // set all part names for scaling

            ShowForm(_frmScale, "Scale parts: " + partNames.ToShortString(), null);
        }
        private void RotateParts(string[] partNames)
        {
            SinglePointDataEditor.ParentForm = _frmRotate;
            SinglePointDataEditor.Controller = _controller;

            _frmRotate.PartNames = partNames;    // set all part names for rotation

            ShowForm(_frmRotate, "Rotate parts: " + partNames.ToShortString(), null);
        }
        private void MergeModelParts(string[] partNames)
        {
            if (MessageBox.Show("OK to merge selected parts?", Globals.ProgramName, MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
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

            using (FrmGetValue frmGetValue = new FrmGetValue(128))
            {
                frmGetValue.NumOfDigits = 0;
                frmGetValue.MinValue = 25;
                frmGetValue.MaxValue = 255;
                SetFormLoaction(frmGetValue);
                frmGetValue.StartPosition = FormStartPosition.Manual;
                frmGetValue.PrepareForm("Set Transparency", "Transparency", "Enter the transparency between 0 and 255.\n" +
                                                                            "(0 - transparent; 255 - opaque)");
                if (frmGetValue.ShowDialog() == DialogResult.OK)
                {
                    _controller.SetTransparencyForModelPartsCommand(partNames, (byte)frmGetValue.Value);
                }
                GetFormLoaction(frmGetValue);
            }
        }
        private void DeleteModelParts(string[] partNames)
        {
            if (MessageBox.Show("OK to delete selected parts?", Globals.ProgramName, MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
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
                ShowForm(_frmNodeSet, "Edit Node Set", null);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }

        private void EditNodeSet(string nodeSetName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmNodeSet;
            ShowForm(_frmNodeSet, "Edit Node Set", nodeSetName);
        }
        private void DeleteNodeSets(string[] nodeSetNames)
        {
            if (MessageBox.Show("OK to delete selected node sets?", Globals.ProgramName, MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
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
                ShowForm(_frmElementSet, "Edit Element set", null);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }

        private void EditElementSet(string elementSetName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmElementSet;
            ShowForm(_frmElementSet, "Edit Element set", elementSetName);
        }
        private void ConvertElementSetsToMeshParts(string[] elementSetNames)
        {
            _controller.ConvertElementSetsToMeshPartsCommand(elementSetNames);
        }
        private void DeleteElementSets(string[] elementSetNames)
        {
            if (MessageBox.Show("OK to delete selected element sets?", Globals.ProgramName, MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
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
                ShowForm(_frmSurface, "Create Surface", null);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void EditSurface(string surfaceName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmSurface;
            ShowForm(_frmSurface, "Edit Surface", surfaceName);
        }
        private void DeleteSurfaces(string[] surfaceNames)
        {
            if (MessageBox.Show("OK to delete selected surfaces?", Globals.ProgramName, MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
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

                SinglePointDataEditor.ParentForm = _frmReferencePoint;
                SinglePointDataEditor.Controller = _controller;

                ShowForm(_frmReferencePoint, "Create Reference Point", null);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteRP_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Reference points", _controller.GetAllReferencePoints(), DeleteRP);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }

        private void EditRP(string referencePointName)
        {
            ShowForm(_frmReferencePoint, "Edit Reference Point", referencePointName);
        }
        private void DeleteRP(string[] referencePointNames)
        {
            if (MessageBox.Show("OK to delete selected reference points?", Globals.ProgramName, MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveReferencePointsCommand(referencePointNames);
            }
        }

        #endregion  ################################################################################################################

        #region Material menu  #####################################################################################################

        private void tsmiCreateMaterial_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
                ShowForm(_frmMaterial, "Create Material", null);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditMaterial_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Materials", _controller.GetAllMaterials(), EditMaterial);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
           
        }

        private void EditMaterial(string materialName)
        {
            ShowForm(_frmMaterial, "Edit Material", materialName);
        }
        private void DeleteMaterials(string[] materialNames)
        {
            if (MessageBox.Show("OK to delete selected materials?", Globals.ProgramName, MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveMaterialsCommand(materialNames);
            }
        }

        private void tsmiMaterialLibrary_Click(object sender, EventArgs e)
        {
            try
            {
                ShowMaterialLibrary();
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void ShowMaterialLibrary()
        {
            FrmMaterialLibrary fml = new FrmMaterialLibrary(_controller);
            CloseAllForms();
            SetFormLoaction((Form)fml);
            fml.ShowDialog();
        }

        #endregion  ################################################################################################################

        #region Section menu  ######################################################################################################
        private void tsmiCreateSection_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }

        private void EditSection(string sectionName)
        {
            ShowForm(_frmSection, "Edit Section", sectionName);
        }
        private void DeleteSections(string[] sectionNames)
        {
            if (MessageBox.Show("OK to delete selected sections?", Globals.ProgramName, MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
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
                 CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }

        private void EditConstraint(string constraintName)
        {
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
            if (MessageBox.Show("OK to delete selected constraints?", Globals.ProgramName, MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveConstraintsCommand(constraintNames);
            }
        }

        #endregion  ################################################################################################################

        #region Step menu  #########################################################################################################

        private void tsmiCreateStep_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }

        private void EditStep(string stepName)
        {
            ShowForm(_frmStep, "Edit Step", stepName);
        }
        private void DeleteSteps(string[] stepNames)
        {
            if (MessageBox.Show("OK to delete selected steps?", Globals.ProgramName, MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void SelectAndEditHistoryOutput(string stepName)
        {
            SelectOneEntityInStep("History outputs", _controller.GetAllHistoryOutputs(stepName), stepName, EditHistoryOutput);
        }
        private void SelectAndDeleteHistoryOutputs(string stepName)
        {
            SelectMultipleEntitiesInStep("History outputs", _controller.GetAllHistoryOutputs(stepName), stepName, DeleteHistoryOutputs);
        }

        private void CreateHistoryOutput(string stepName)
        {
            if (_controller.Model.Mesh == null) return;
            ShowForm(_frmHistoryOutput, "Create History Output", stepName, null);
        }
        private void EditHistoryOutput(string stepName, string historyOutputName)
        {
            ShowForm(_frmHistoryOutput, "Edit History Output", stepName, historyOutputName);
        }
        private void DeleteHistoryOutputs(string stepName, string[] historyOutputNames)
        {
            if (MessageBox.Show("OK to delete selected history outputs?", Globals.ProgramName, MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
            if (MessageBox.Show("OK to delete selected field outputs?", Globals.ProgramName, MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveFieldOutputsForStepCommand(stepName, fieldOutputNames);
            }
        }

        #endregion  ################################################################################################################

        #region Boundary conditions menu  ##########################################################################################
        private void tsmiCreateBC_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), CreateBoundaryCondition);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }

        private void SelectAndEditBoundaryCondition(string stepName)
        {
            SelectOneEntityInStep("Boundary conditions", _controller.GetStepBoundaryConditions(stepName), stepName, EditBoundaryCondition);
        }
        private void SelectAndHideBoundaryConditions(string stepName)
        {
            SelectMultipleEntitiesInStep("Boundary conditions", _controller.GetStepBoundaryConditions(stepName), stepName, HideBoundaryConditions);
        }
        private void SelectAndShowBoundaryConditions(string stepName)
        {
            SelectMultipleEntitiesInStep("Boundary conditions", _controller.GetStepBoundaryConditions(stepName), stepName, ShowBoundaryConditions);
        }
        private void SelectAndDeleteBoundaryCondition(string stepName)
        {
            SelectMultipleEntitiesInStep("Boundary conditions", _controller.GetStepBoundaryConditions(stepName), stepName, DeleteBoundaryConditions);
        }

        private void CreateBoundaryCondition(string stepName)
        {
            if (_controller.Model.Mesh == null) return;
            ShowForm(_frmBoundaryCondition, "Create Boundary Condition", stepName, null);
        }
        private void EditBoundaryCondition(string stepName, string boundaryConditionName)
        {
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
            HashSet<string> allNames = new HashSet<string>(_controller.Model.StepCollection.GetStep(stepName).BoundaryConditions.Keys);
            allNames.ExceptWith(boundaryConditionNames);
            _controller.ShowBoundaryConditionCommand(stepName, boundaryConditionNames);
            _controller.HideBoundaryConditionCommand(stepName, allNames.ToArray());
        }
        private void DeleteBoundaryConditions(string stepName, string[] boundaryConditionNames)
        {
            if (MessageBox.Show("OK to delete selected boundary conditions?", Globals.ProgramName, MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveBoundaryConditionsCommand(stepName, boundaryConditionNames);
            }
        }

        #endregion  ################################################################################################################

        #region Load menu  #########################################################################################################

        private void tsmiCreateLoad_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), CreateLoad);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
            ShowForm(_frmLoad, "Create Load", stepName, null);
        }
        private void EditLoad(string stepName, string loadName)
        {
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
            if (MessageBox.Show("OK to delete selected loads?", Globals.ProgramName, MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveLoadsCommand(stepName, loadNames);
            }
        }

        #endregion  ################################################################################################################

        #region Settings ###########################################################################################################
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
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
           
        }
        private void ShowPostSettings()
        {
            _frmSettings.SetSettingsToShow(Globals.PostSettingsName);
            tsmiSettings_Click(null, null);
        }

        private void UpdateSettings(Dictionary<string, ISettings> settings)
        {
            _controller.Settings = settings;    // this calls the redraw functions
        }
        #endregion  ################################################################################################################

        #region Query ##############################################################################################################
        private void tsmiQuery_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_frmQuery.Visible)
                {
                    ClearSelection();

                    CloseAllForms();
                    SetFormLoaction(_frmQuery);
                    _frmQuery.PrepareForm(_controller);
                    _frmQuery.Show();
                }
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiRunAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Analyses", _controller.GetAllJobs(), RunAnalysis);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiResultsAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Analyses", _controller.GetAllJobs(), ResultsAnalysis);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
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
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }

        private void EditAnalysis(string jobName)
        {
            ShowForm(_frmAnalysis, "Edit Analysis", jobName);
        }
        private void RunAnalysis(string jobName)
        {
            string[] invalidItems = _controller.CheckAndUpdateValidity();
            if (CheckValiditiy())
            {
                CalculixSettings settings = (CalculixSettings)_controller.Settings[Globals.CalculixSettingsName];

                if (settings.WorkDirectory == null || !Directory.Exists(settings.WorkDirectory))
                    throw new Exception("The work directory of the analysis does not exist.");

                AnalysisJob job = _controller.GetJob(jobName);
                if (job.JobStatus != JobStatus.Running)
                {
                    string inputFileName = Path.Combine(settings.WorkDirectory, jobName + ".inp");
                    if (File.Exists(inputFileName))
                    {
                        if (MessageBox.Show("Overwrite existing analysis files?", "Warning", MessageBoxButtons.OKCancel) != DialogResult.OK) return;
                    }

                    _controller.RunJob(inputFileName, job);
                    MonitorAnalysis(jobName);
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
                _frmMonitor.PrepareForm(_controller.GetJob(jobName));
                _frmMonitor.ShowDialog();
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private async void ResultsAnalysis(string jobName)
        {
            CaeJob.AnalysisJob job = _controller.GetJob(jobName);
            if (job.JobStatus == CaeJob.JobStatus.OK || job.JobStatus == CaeJob.JobStatus.Running)
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
                if (MessageBox.Show("OK to kill selected analysis?", Globals.ProgramName, MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
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
            if (MessageBox.Show("OK to delete selected analyses?", Globals.ProgramName, MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                _controller.RemoveJobsCommand(jobNames);
            }
        }
        public void UpdateAnalysisProgress()
        {
            _frmMonitor.UpdateProgress();
        }

        #endregion  ################################################################################################################

        #region Result part menu  ##################################################################################################

        private void tsmiEditResultPart_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Parts", _controller.GetResultParts(), EditResultPart);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiHideResultPart_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetResultParts(), HideResultParts);
                Clear3DSelection();
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiShowResultPart_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetResultParts(), ShowResultParts);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiSetTransparencyForResultPart_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetResultParts(), SetTransparencyForResultParts);
                Clear3DSelection();
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiColorContoursOff_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetResultParts<CaeMesh.ResultPart>(), ColorContoursOffResultPart);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiColorContoursOn_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetResultParts<CaeMesh.ResultPart>(), ColorContoursOnResultPart);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }

            
        }
        private void tsmiDeleteResultPart_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetResultParts(), DeleteResultParts);
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }

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
            if (_controller.Model.Mesh == null) return;

            using (FrmGetValue frmGetValue = new FrmGetValue(128))
            {
                frmGetValue.NumOfDigits = 0;
                frmGetValue.MinValue = 25;
                frmGetValue.MaxValue = 255;
                SetFormLoaction(frmGetValue);
                frmGetValue.StartPosition = FormStartPosition.Manual;
                frmGetValue.PrepareForm("Set Transparency", "Transparency", "Enter the transparency between 0 and 255.\n" +
                                                                            "(0 - transparent; 255 - opaque)");
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
            if (MessageBox.Show("OK to delete selected parts?", Globals.ProgramName, MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
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

                    string[] columnNames;
                    object[][] rowBasedData;
                    _controller.GetHistoryOutputData(historyData, out columnNames, out rowBasedData);

                    _frmHistoryResultsOutput.SetData(columnNames, rowBasedData);
                    _frmHistoryResultsOutput.Show();
                }
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
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

            string[] preSelectedEntityNames = _modelTree.IntersectSelectionWithList(entities);

            _frmSelectEntity.Location = new Point(Left + _formLocation.X, Top + _formLocation.Y);
            _frmSelectEntity.PrepareForm(title, false, entities, preSelectedEntityNames, null);
            _frmSelectEntity.OneEntitySelected = OperateOnEntity;
            _frmSelectEntity.Show();
        }
        private void SelectOneEntityInStep(string title, NamedClass[] entities, string stepName, Action<string, string> OperateOnEntityInStep)
        {
            if (entities == null || entities.Length == 0) return;

            string[] preSelectedEntityNames = _modelTree.IntersectSelectionWithList(entities);

            _frmSelectEntity.Location = new Point(Left + _formLocation.X, Top + _formLocation.Y);
            _frmSelectEntity.PrepareForm(title, false, entities, preSelectedEntityNames, stepName);
            _frmSelectEntity.OneEntitySelectedInStep = OperateOnEntityInStep;
            _frmSelectEntity.Show();
        }
        private void SelectMultipleEntities(string title, NamedClass[] entities, Action<string[]> OperateOnMultpleEntities)
        {
            if (entities == null || entities.Length == 0) return;

            string[] preSelectedEntityNames = _modelTree.IntersectSelectionWithList(entities);

            _frmSelectEntity.Location = new Point(Left + _formLocation.X, Top + _formLocation.Y);
            _frmSelectEntity.PrepareForm(title, true, entities, preSelectedEntityNames, null);
            _frmSelectEntity.MultipleEntitiesSelected = OperateOnMultpleEntities;
            _frmSelectEntity.Show();
        }
        private void SelectMultipleEntitiesInStep(string title, NamedClass[] entities, string stepName, Action<string, string[]> OperateOnMultpleEntitiesInStep)
        {
            if (entities == null || entities.Length == 0) return;

            string[] preSelectedEntityNames = _modelTree.IntersectSelectionWithList(entities);

            _frmSelectEntity.Location = new Point(Left + _formLocation.X, Top + _formLocation.Y);
            _frmSelectEntity.PrepareForm(title, true, entities, preSelectedEntityNames, stepName);
            _frmSelectEntity.MultipleEntitiesSelectedInStep = OperateOnMultpleEntitiesInStep;
            _frmSelectEntity.Show();
        }

        #endregion  ################################################################################################################

        #region Mouse selection methods  ###########################################################################################

        public void SelectPointOrArea(double[] pickedPoint, double[][] planeParameters, vtkSelectOperation selectOperation)
        {
            _controller.SelectPointOrArea(pickedPoint, planeParameters, selectOperation);
            int[] ids = _controller.GetSelectionIds();

            if (_frmSectionView.Visible) _frmSectionView.PickedIds(ids);
            else if (_frmTranslate.Visible) _frmTranslate.PickedIds(ids);
            else if (_frmScale.Visible) _frmScale.PickedIds(ids);
            else if (_frmRotate.Visible) _frmRotate.PickedIds(ids);
            else if (_frmReferencePoint.Visible) _frmReferencePoint.PickedIds(ids);
            else if (_frmQuery.Visible) _frmQuery.PickedIds(ids);
        }

        public void SetSelectBy(vtkSelectBy selectBy)
        {
            InvokeIfRequired(() => _vtk.SelectBy = selectBy);
        }
        public void SetSelectItem(vtkSelectItem selectItem)
        {
            InvokeIfRequired(() => _vtk.SelectItem = selectItem);
        }

        public void GetGeometryPickProperties(double[] point, out double dist, out int elementId,
                                              out int[] edgeNodeIds, out int[] cellFaceNodeIds)
        {
            _vtk.GetGeometryPickProperties(point, out dist, out elementId, 
                                           out edgeNodeIds, out cellFaceNodeIds);
        }

        public int[] GetNodeIdsFromFrustum(double[][] planeParameters, vtkSelectBy selectBy)
        {
            return _vtk.GetGlobalNodeIdsFromFrustum(planeParameters, selectBy);
        }
        public int[] GetElementIdsFromFrustum(double[][] planeParameters, vtkSelectBy selectBy)
        {
            return _vtk.GetGlobalElementIdsFromFrustum(planeParameters, selectBy);
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
                if (form.PrepareForm(stepName, itemToEditName))
                    form.Show();
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
            if (_allForms != null)
            {
                // first hide the _frmSelectItemSet, since it's hiding enables the form it was called from (like _frmNodeSet...)
                if (_frmSelectItemSet.Visible) _frmSelectItemSet.Hide(DialogResult.Cancel);
                
                foreach (var form in _allForms)
                {
                    if (form.Visible) form.Hide();
                }
            }
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
            _controller.DrawSymbolsForStep = tscbSymbolsForStep.SelectedItem.ToString();
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
                if (_controller.ViewResultsType != ViewResultsType.Undeformed && _controller.GetResultStepIDs().Length > 0 && !_frmAnimation.Visible)
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
            _controller.StopMeshing();
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
                MessageBox.Show(text, "Error", MessageBoxButtons.OK);
                return false;
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

                fileName = Path.GetFileName(_controller.OpenedFileName);
                saveFileDialog.FileName = fileName;

                saveFileDialog.OverwritePrompt = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileName = saveFileDialog.FileName;
                }
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
            string filter = "All supported files|*.stp;*.step;*.igs;*.iges;*.brep;*.stl;*.unv;*.vol;*.inp"
                            + "|Step files|*.stp;*.step"
                            + "|Iges files|*.igs;*.iges"
                            + "|Brep files|*.brep"
                            + "|Stereolitography files|*.stl"
                            + "|Universal files|*.unv"
                            + "|Netgen files|*.vol"
                            + "|Abaqus/Calculix inp files|*.inp";
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
            tscbStepAndIncrement.Items.Clear();
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
            _controller.ClearSelectionHistory();
        }
        public void Clear3DSelection()
        {
            InvokeIfRequired(() => _vtk.ClearSelection());
        }
        public void ClearTreeSelection()
        {
            _modelTree.ClearSelection();
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
            InvokeIfRequired(_vtk.UpdateActorColorContoursVisibility, actorNames, colorContour);
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

        public void InitializeWidgetPositions()
        {
            InvokeIfRequired(_vtk.InitializeWidgetPositions);
        }
        public void SetCoorSysVisibility(bool visibility)
        {
            InvokeIfRequired(_vtk.SetCoorSysVisibility, visibility);
        }
        public void SetScaleWidgetVisibility(bool visibility)
        {
            InvokeIfRequired(_vtk.SetScaleWidgetVisibility, visibility);
        }
        public void SetColorSpectrum(vtkControl.vtkMaxColorSpectrum colorSpectrum)
        {
            InvokeIfRequired(_vtk.SetColorSpectrum, colorSpectrum);
        }
        public void SetScalarBarText(string text)
        {
            InvokeIfRequired(() => _vtk.ScalarBarText = text);
        }
        public void SetShowMinValueLocation(bool show)
        {
            InvokeIfRequired(() => _vtk.ShowMinValueLocation = show);
        }
        public void SetShowMaxValueLocation(bool show)
        {
            InvokeIfRequired(() => _vtk.ShowMaxValueLocation = show);
        }
        public void SetChartNumberFormat(string numberFormat)
        {
            InvokeIfRequired(_vtk.SetChartNumberFormat, numberFormat);
        }
        public void SetStatusBlock(string name, DateTime dateTime, float analysisTime, float scaleFactor, 
                                   vtkControl.DataFieldType fieldType = vtkControl.DataFieldType.Static)
        {
            InvokeIfRequired(_vtk.SetStatusBlock, name, dateTime, analysisTime, scaleFactor, fieldType);
        }
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
        public void SetHighlightColor(Color highlightColor)
        {
            InvokeIfRequired(_vtk.SetHighlightColor, highlightColor);
        }
        public void SetMouseHighlightColor(Color mousehighlightColor)
        {
            InvokeIfRequired(_vtk.SetMouseHighlightColor, mousehighlightColor);
        }

        #endregion  ################################################################################################################

        #region Results  ###########################################################################################################
        // Results
        public void SetFieldData(string name, string component, int stepId, int stepIncrementId)
        {
            CaeResults.FieldData fieldData = new CaeResults.FieldData(name, component, stepId, stepIncrementId);
            CaeResults.FieldData currentData = _controller.CurrentFieldData;

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
                foreach (var entry in allIds)
                {
                    foreach (int incrementId in entry.Value)
                    {
                        tscbStepAndIncrement.Items.Add(entry.Key.ToString() + ", " + incrementId);
                    }
                }
                tscbStepAndIncrement.SelectedIndexChanged += FieldOutput_SelectionChanged;  // reattach event

                // Reselect previous step and increment
                if (prevStepIncrementIds != null)
                {
                    int stepId = int.Parse(prevStepIncrementIds[0]);
                    int incrementId = int.Parse(prevStepIncrementIds[1]);
                    SetStepAndIncrementIds(stepId, incrementId);
                }
                else SetLastStepAndIncrementIds();
            });
        }
        
        public void SetStepAndIncrementIds(int stepId, int incrementId)
        {
            InvokeIfRequired(() =>
            {
                string stepIncrement = stepId + ", " + incrementId;

                // set the combobox
                if (tscbStepAndIncrement.Items.Contains(stepIncrement))
                {
                    tscbStepAndIncrement.SelectedIndexChanged -= FieldOutput_SelectionChanged;
                    tscbStepAndIncrement.SelectedItem = stepIncrement;
                    // set the step and increment if the combobox set was successful
                    CaeResults.FieldData data = _controller.CurrentFieldData;
                    data.StepId = stepId;
                    data.StepIncrementId = incrementId;
                    _controller.CurrentFieldData = data;   // to correctly update the increment time
                    tscbStepAndIncrement.SelectedIndexChanged += FieldOutput_SelectionChanged;
                }
                else SetLastStepAndIncrementIds();
            });
        }
        public void SetLastStepAndIncrementIds()
        {
            string lastStepIncrement = (string)tscbStepAndIncrement.Items[tscbStepAndIncrement.Items.Count - 1];
            string[] tmp = lastStepIncrement.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
            SetStepAndIncrementIds(int.Parse(tmp[0]), int.Parse(tmp[1]));
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
        public void SaveAnimationAsAVI(string fileName, int[] firstLastFrame, int step, int fps, bool scalarRangeFromAllFrames, bool swing, bool encoderOptions)
        {
            InvokeIfRequired(_vtk.SaveAnimationAsAVI, fileName, firstLastFrame, step, fps, scalarRangeFromAllFrames, swing, encoderOptions);
        }
        public void SaveAnimationAsImages(string fileName, int[] firstLastFrame, int step, bool scalarRangeFromAllFrames, bool swing)
        {
            InvokeIfRequired(_vtk.SaveAnimationAsImages, fileName, firstLastFrame, step, scalarRangeFromAllFrames, swing);
        }
        
        #endregion  ################################################################################################################
        
        #region Tree  ##############################################################################################################
        // Tree
        public void RegenerateTree(CaeModel.FeModel model, Dictionary<string, CaeJob.AnalysisJob> jobs,
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

            InvokeIfRequired(_modelTree.AddTreeNode, viewType, item, stepName);
            if (item is CaeModel.Step) UpadteSymbolsForStepList();
        }
        public void UpdateTreeNode(ViewGeometryModelResults view, string oldItemName, NamedClass item, string stepName, bool updateSelection = true)
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
        public void UpdateHighlightFromTree()
        {
            InvokeIfRequired(_modelTree.UpdateHighlight);
        }
        public void SelectBasePart(MouseEventArgs e, Keys modifierKeys, string partName)
        {
            // this is called from _vtk on part selection

            if (!_modelTree.DisableMouse)
            {
                if (partName == null)
                {
                    if (modifierKeys != Keys.Shift && modifierKeys != Keys.Control) _controller.ClearAllSelection();
                }
                else
                {
                    CaeMesh.BasePart part;
                    if (GetCurrentView() == ViewGeometryModelResults.Geometry) part = _controller.GetGeometryPart(partName);
                    else if (GetCurrentView() == ViewGeometryModelResults.Model) part = _controller.GetModelPart(partName);
                    else if (GetCurrentView() == ViewGeometryModelResults.Results) part = _controller.GetResultPart(partName);
                    else throw new NotSupportedException();

                    int numOfSelectedTreeNodes = _modelTree.SelectBasePart(e, modifierKeys, part);

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
        public void InvokeIfRequired<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4)
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
        public void InvokeIfRequired<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 parameter1, T2 parameter2, T3 parameter3,
                                    T4 parameter4, T5 parameter5, T6 parameter6)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate () { action(parameter1, parameter2, parameter3, parameter4, parameter5, parameter6); });
            }
            else
            {
                action(parameter1, parameter2, parameter3, parameter4, parameter5, parameter6);
            }
        }
        public void InvokeIfRequired<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 parameter1, T2 parameter2, T3 parameter3,
                                    T4 parameter4, T5 parameter5, T6 parameter6, T7 parameter7)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate () { action(parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7); });
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
            if (timerTest.Enabled) timerTest.Stop();
            else timerTest.Start();
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

        
    }
}
