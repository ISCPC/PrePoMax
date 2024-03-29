﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeModel;
using CaeMesh;
using CaeJob;
using CaeResults;
using System.IO;
using CaeGlobals;
using System.IO.Compression;
using System.Drawing;
using System.ComponentModel;
using System.Management;
using System.Runtime.Serialization;
using System.Reflection.Emit;
using vtkControl;
using System.Runtime.InteropServices;
using System.IO.Ports;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using PrePoMax.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Security.Policy;
using System.Runtime.InteropServices.ComTypes;
using UserControls;
using PrePoMax.Commands;
using System.Windows.Input;
using PrePoMax.Properties;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace PrePoMax
{
    [Serializable]
    public class Controller //: ISerializable
    {
        // Variables                                                                                                                
        [NonSerialized] protected FrmMain _form;
        [NonSerialized] protected SettingsContainer _settings;
        [NonSerialized] protected OrderedDictionary<string, AnalysisJob> _jobs;
        // States
        [NonSerialized] protected bool _modelChanged;
        [NonSerialized] protected bool _savingFile;
        [NonSerialized] protected bool _animating;
        // View
        [NonSerialized] protected ViewGeometryModelResults _currentView;
        [NonSerialized] protected EdgesVisibilitiesCollection _edgesVisibilities;
        [NonSerialized] protected SectionViewsCollection _sectionViews;
        [NonSerialized] protected ExplodedViewsCollection _explodedViews;
        [NonSerialized] protected AnnotateWithColorEnum _annotateWithColor;
        [NonSerialized] protected string _drawSymbolsForStep;
        // Selection
        [NonSerialized] protected vtkSelectBy _selectBy;
        [NonSerialized] protected double _selectAngle;
        [NonSerialized] protected Selection _selection;
        // Annotations
        protected AnnotationContainer _annotations;
        // Results
        [NonSerialized] protected ViewResultsTypeEnum _viewResultsType;
        [NonSerialized] protected FieldData _currentFieldData;
        [NonSerialized] protected TransformationsCollection _transformations;
        // Errors
        [NonSerialized] protected List<string> _errors;
        //
        protected FeModel _model;
        [NonSerialized] protected ExecutableJob _executableJob;
        protected FeResults _results;
        protected ResultsCollection _allResults;
        [NonSerialized] protected FeResults _wearResults;
        // History
        protected CommandsCollection _commands;



        // Properties                                                                                                               
        public FrmMain Form { get { return _form; } }
        public SettingsContainer Settings
        {
            get
            {
                return _settings.Get(_currentView, _currentFieldData);
            }
            set
            {
                try
                {
                    _settings.Set(value, _currentView, _currentFieldData);
                    _settings.SaveToFile();
                    //
                    ApplySettings();
                    // Redraw model with new settings
                    Redraw();
                }
                catch
                { }
            }
        }
        public OrderedDictionary<string, AnalysisJob> Jobs { get { return _jobs; } }
        // States
        public bool ModelInitialized
        {
            get { return _commands != null && _commands.CurrPositionIndex > 0; }
        }
        public bool ResultsInitialized
        {
            get
            {
                return _allResults.Count > 0 && _allResults.CurrentResult != null &&
                       _allResults.CurrentResult.Mesh != null && _allResults.CurrentResult.Mesh.Nodes.Count > 0;
            }
        }
        public bool ContainsComplexResults
        {
            get { return _allResults.ContainsComplexResults(); }
        }
        public bool ModelChanged { get { return _modelChanged; } set { _modelChanged = value; } }
        public bool SavingFile { get { return _savingFile; } }
        public FeModel Model { get { return _model; } }
        public bool ExecutableJobIdle
        {
            get
            {
                if (_executableJob != null && _executableJob.JobStatus == JobStatus.Running) return false;
                else return true;
            }
        }
        // View
        public ViewGeometryModelResults CurrentView
        {
            get { return _currentView; }
            set
            {
                if (_currentView != value)
                {
                    _currentView = value;
                    ClearSelectionHistoryAndCallSelectionChanged(); // the selection nodes are only valid on default mesh
                    _form.SetCurrentView(_currentView);
                    //
                    Redraw();
                }
            }
        }
        public vtkEdgesVisibility CurrentEdgesVisibility
        {
            get { return _edgesVisibilities.GetCurrentEdgesVisibility(); }
            set
            {
                if (_edgesVisibilities.GetCurrentEdgesVisibility() != value)
                {
                    _edgesVisibilities.SetCurrentEdgesVisibility(value);
                    _form.SetCurrentEdgesVisibilities(value);
                }
            }
        }

        // Section view
        public bool IsSectionViewActive()
        {
            return _sectionViews.IsSectionViewActive();
        }
        public Octree.Plane GetSectionViewPlane()
        {
            return _sectionViews.GetCurrentSectionViewPlane();
        }
        // Exploded view
        public bool IsExplodedViewActive()
        {
            return _explodedViews.IsExplodedViewActive();
        }
        public ExplodedViewParameters GetCurrentExplodedViewParameters()
        {
            return _explodedViews.GetCurrentExplodedViewParameters();
        }
        // Annotate
        public AnnotateWithColorEnum AnnotateWithColor
        {
            get { return _annotateWithColor; }
            set
            {
                _annotateWithColor = value;
                //
                if (_annotateWithColor == AnnotateWithColorEnum.None) _form.HideColorBar();
                //
                _form.InitializeColorBarWidgetPosition();
                //
                Redraw();
            }
        }
        // Annotations
        public AnnotationContainer Annotations { get { return _annotations; } }
        // Symbols
        public void DrawSymbolsForStep(string stepName, bool updateHighlight)
        {
            if (stepName != _drawSymbolsForStep)
            {
                _drawSymbolsForStep = stepName;
                // Prevent the symbols from showing up first at: File open -> Regenerate tree
                if (!_form.IsStateOpening()) RedrawSymbols(updateHighlight);
            }
        }
        public string GetDrawSymbolsForStep()
        {
            return _drawSymbolsForStep;
        }
        // Selection
        public vtkSelectItem SelectItem
        {
            get { return _selection.SelectItem; }
            set
            {
                _selection.SelectItem = value;
                _form.SetSelectItem(value);
            }
        }
        public vtkSelectBy SelectBy
        {
            get { return _selectBy; }
            set
            {
                if (value != _selectBy)
                {
                    _selectBy = value;
                    _form.SetSelectBy(_selectBy);
                }
            }
        }
        public double SelectAngle { get { return _selectAngle; } set { _selectAngle = value; } }
        public Selection Selection { get { return _selection; } set { _selection = value; } }
        // Results
        public ResultsCollection AllResults { get { return _allResults; } }
        public FeResults CurrentResult { get { return _allResults.CurrentResult; } }
        public ViewResultsTypeEnum ViewResultsType
        {
            get { return _viewResultsType; }
            set
            {
                _viewResultsType = value;
                // This is used by the model tree to show/hide the Deformed and Color contour context menu lines
                ResultPart.Undeformed = _viewResultsType == ViewResultsTypeEnum.Undeformed;
                //
                if (_allResults.CurrentResult != null && _allResults.CurrentResult.Mesh != null)
                {
                    foreach (var entry in _allResults.CurrentResult.Mesh.Parts)
                    {
                        if (entry.Value is ResultPart resultPart)
                            resultPart.ColorContours = _viewResultsType == ViewResultsTypeEnum.ColorContours;
                    }
                    //
                    DrawResults(false);
                }
            }
        }
        public FieldData CurrentFieldData
        {
            get { return _currentFieldData; }
            set
            {
                _currentFieldData = value;
                _currentFieldData.Time = _allResults.CurrentResult.GetIncrementTime(_currentFieldData.StepId,
                                                                                    _currentFieldData.StepIncrementId);
            }
        }
        public string GetCurrentResultsUnitAbbreviation()
        {
            if (CurrentFieldData.Unit != null && CurrentFieldData.Unit.Length > 0) return CurrentFieldData.Unit;
            else return _allResults.CurrentResult.GetFieldUnitAbbrevation(CurrentFieldData);
        }
        public bool AreTransformationsActive()
        {
            return _transformations.AreTransformationsActive();
        }
        // Errors
        public int GetNumberOfErrors()
        {
            return _errors.Count();
        }
        // Tools
        public string OpenedFileName
        {
            get
            {
                return _settings.General.LastFileName;
            }
            set
            {
                if (_settings != null)
                {
                    if (value != _settings.General.LastFileName)
                    {
                        _settings.General.LastFileName = value;
                        _settings.SaveToFile();
                    }
                    //
                    if (_settings.General.LastFileName != null)
                        _form.SetTitle(Globals.ProgramName + "   " + _settings.General.LastFileName);
                    else _form.SetTitle(Globals.ProgramName);
                }
            }
        }
        public FeMesh DisplayedMesh
        {
            get
            {
                if (_currentView == ViewGeometryModelResults.Geometry) return _model.Geometry;
                else if (_currentView == ViewGeometryModelResults.Model) return _model.Mesh;
                else if (_currentView == ViewGeometryModelResults.Results)
                {
                    if (_allResults.CurrentResult != null) return _allResults.CurrentResult.Mesh;
                    else return null;
                }
                else throw new NotSupportedException();
            }
        }


        // Setters                                                                                                                  
        public void SetSelectByToOff()
        {
            SelectBy = vtkSelectBy.Off;
        }
        public void SetSelectByToDefault()
        {
            SelectBy = vtkSelectBy.Default;
        }
        public void SetSelectBy(vtkSelectBy selectBy)
        {
            SelectBy = selectBy;
        }
        public void SetSelectAngle(double angle)
        {
            SelectAngle = angle;
        }
        public void SetSelectItemToNode()
        {
            SelectItem = vtkSelectItem.Node;
        }
        public void SetSelectItemToElement()
        {
            SelectItem = vtkSelectItem.Element;
        }
        public void SetSelectItemToSurface()
        {
            SelectItem = vtkSelectItem.Surface;
        }
        public void SetSelectItemToPart()
        {
            SelectItem = vtkSelectItem.Part;
        }
        public void SetSelectItemToGeometry()
        {
            SelectItem = vtkSelectItem.Geometry;
        }
        public void SetSelectItemToGeometrySurface()
        {
            SelectItem = vtkSelectItem.GeometrySurface;
        }


        // Constructors                                                                                                             
        public Controller(FrmMain form)
        {
            // Form
            _form = form;
            _form.Controller = this;
            // Jobs
            _jobs = new OrderedDictionary<string, AnalysisJob>("Analysis Jobs", StringComparer.OrdinalIgnoreCase);
            // Edges visibilities
            _edgesVisibilities = new EdgesVisibilitiesCollection(this);
            // Section view
            _sectionViews = new SectionViewsCollection(this);
            // Exploded view
            _explodedViews = new ExplodedViewsCollection(this);
            // Annotations
            _annotations = new AnnotationContainer(this);
            // Selection
            _selection = new Selection();
            // Results
            _allResults = new ResultsCollection();  // must be first
            ViewResultsType = ViewResultsTypeEnum.ColorContours;
            _transformations = new TransformationsCollection(this);
            // Errors - must be here before Clear
            _errors = new List<string>();
            // History
            _commands = new Commands.CommandsCollection(this);
            _commands.WriteOutput = _form.WriteDataToOutput;
            _commands.ModelChanged_ResetJobStatus = ResetAllJobStatus;
            _commands.EnableDisableUndoRedo += _commands_CommandExecuted;
            _commands.OnEnableDisableUndoRedo();
            // Clear
            Clear();
            // Settings - must follow Clear to load the Opened file name
            _settings = new SettingsContainer();
            _settings.LoadFromFile();
            ApplySettings();
        }


        // Static methods                                                                                                           
        public static void PrepareForSaving(Controller controller)
        {
            if (controller != null)
            {
                if (controller.Model != null) FeMesh.PrepareForSaving(controller.Model.Geometry);
                if (controller.Model != null) FeMesh.PrepareForSaving(controller.Model.Mesh);
                if (controller.CurrentResult != null) ResultsCollection.PrepareForSavig(controller._allResults);
            }
        }
        public static void ResetAfterSaving(Controller controller)
        {
            if (controller != null)
            {
                if (controller.Model != null) FeMesh.ResetAfterSaving(controller.Model.Geometry);
                if (controller.Model != null) FeMesh.ResetAfterSaving(controller.Model.Mesh);
                if (controller.CurrentResult != null) ResultsCollection.ResetAfterSavig(controller._allResults);
            }
        }

        //public Controller(SerializationInfo info, StreamingContext context)
        //{

        //}
        //// ISerialization
        //public void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    //// Using typeof() works also for null fields
        //    //info.AddValue("_name", Name, typeof(string));
        //    //info.AddValue("_geometry", _geometry, typeof(FeMesh));
        //    //info.AddValue("_mesh", _mesh, typeof(FeMesh));
        //    //info.AddValue("_materials", _materials, typeof(OrderedDictionary<string, Material>));
        //    //info.AddValue("_sections", _sections, typeof(OrderedDictionary<string, Section>));
        //    //info.AddValue("_constraints", _constraints, typeof(OrderedDictionary<string, Constraint>));
        //    //info.AddValue("_surfaceInteractions", _surfaceInteractions, typeof(OrderedDictionary<string, SurfaceInteraction>));
        //    //info.AddValue("_contactPairs", _contactPairs, typeof(OrderedDictionary<string, ContactPair>));
        //    //info.AddValue("_amplitudes", _amplitudes, typeof(OrderedDictionary<string, Amplitude>));
        //    //info.AddValue("_initialConditions", _initialConditions, typeof(OrderedDictionary<string, InitialCondition>));
        //    //info.AddValue("_stepCollection", _stepCollection, typeof(StepCollection));
        //    //info.AddValue("_calculixUserKeywords", _calculixUserKeywords, typeof(OrderedDictionary<int[], Calculix.CalculixUserKeyword>));
        //    //info.AddValue("_properties", _properties, typeof(ModelProperties));
        //    //info.AddValue("_unitSystem", _unitSystem, typeof(UnitSystem));
        //    //info.AddValue("_hashName", _hashName, typeof(string));
        //}

        #region Commands   #########################################################################################################
        private void _commands_CommandExecuted(string undo, string redo)
        {
            _form.EnableDisableUndoRedo(undo, redo);
        }
        public string GetHistoryFileNameTxt()
        {
            return _commands.GetHistoryFileNameTxt();
        }
        public string GetHistoryFileNameBin()
        {
            return _commands.GetHistoryFileNameBin();
        }
        public void DeleteHistoryFiles()
        {
            if (_commands != null) _commands.DeleteHistoryFiles();
        }

        #endregion #################################################################################################################

        #region Clear   ############################################################################################################
        // COMMANDS ********************************************************************************
        public void ClearCommand()
        {
            Commands.CClear comm = new Commands.CClear();
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public void DeInitialize()
        {
            New();
            //
            _commands.Clear();
            _allResults.Clear();
        }
        public void Clear()
        {
            _form.CloseAllForms();
            _form.SetTitle(Globals.ProgramName);
            OpenedFileName = null;
            _savingFile = false;
            //
            if (_form != null)
            {
                _form.ClearControls();
                _form.SetCurrentView(_currentView);
            }
            //
            ClearModel();
            ClearResults();
            // Selection
            _selection = new Selection();
            SetSelectByToDefault();
            //
            _modelChanged = false;  // must be here since ClearResults can set it to true
        }
        public void ClearModel()
        {
            // Section view
            _sectionViews.ClearModelSectionViews();
            // Exploded view
            _explodedViews.ClearModelExplodedViews();
            //
            _model = new FeModel("Model-1");
            SetNewModelProperties(_model.Properties.ModelSpace, _model.UnitSystem.UnitSystemType);   // update widgets
            //
            _annotateWithColor = AnnotateWithColorEnum.None;
            _drawSymbolsForStep = null;
            _jobs.Clear();
            ClearAllSelection();
            //
            _modelChanged = false;
        }
        public void ClearResults()
        {
            // Section view
            _sectionViews.ClearAllResultsSectionViews();
            // Exploded view
            _explodedViews.ClearAllResultsExplodedViews();
            // Annotations
            _annotations.RemoveAllResultArrowAnnotations();
            //
            if (_allResults.Count > 0)
            {
                _modelChanged = true;
                _allResults.Clear();
            }
            //
            _currentFieldData = null;
            //
            if (_settings != null) _settings.ClearColorSpectrums();
            //
            _form.ClearResults();
            //
            ClearAllSelection();
        }
        public void ClearAllSelection()
        {
            ClearSelectionHistoryAndCallSelectionChanged();
            _form.ClearActiveTreeSelection();
        }
        public void ClearSelectionHistoryAndCallSelectionChanged()
        {
            ClearSelectionHistory();
            //
            _form.SelectionChanged();
        }
        public void ClearSelectionHistory()
        {
            _selection.Clear();
            _form.Clear3DSelection();
        }

        #endregion #################################################################################################################

        // Menus
        #region File menu   ########################################################################################################
        // COMMANDS ********************************************************************************
        public void ImportFileCommand(string fileName, bool onlyMaterials)
        {
            CImportFile comm = new CImportFile(fileName, onlyMaterials);
            _commands.AddAndExecute(comm);
        }
        public void SaveToPmxCommand(string fileName)
        {
            Commands.CSaveToPmx comm = new Commands.CSaveToPmx(fileName);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public void New()
        {
            _currentView = ViewGeometryModelResults.Geometry;
            // Add and execute the clear command
            _commands.Clear();      // also calls _modelChanged = false;
            ClearCommand();         // also calls _modelChanged = false; calls SetNewModelProperties()
            // Annotations
            _annotations = new AnnotationContainer(this);
            //
            _form.UpdateRecentFilesThreadSafe(_settings.General.GetRecentFiles());
        }
        public void Open(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();
            //
            if (extension == ".pmx") OpenPmx(fileName);
            else if (extension == ".pmh") OpenPmh(fileName);
            else if (extension == ".frd") OpenFrd(fileName);
            else if (extension == ".dat") OpenDat(fileName);
            else if (extension == ".foam") OpenFoam(fileName);
            else throw new NotSupportedException();
            // Check validity
            CheckAndUpdateModelValidity();
            // Get first component of the first field for the last increment in the last step
            if (ResultsInitialized)
                CurrentFieldData = _allResults.CurrentResult.GetFirstComponentOfTheFirstFieldAtDefaultIncrement();
            //
            UpdateExplodedView(false);
            // Settings
            AddFileNameToRecentFiles(fileName);  // this redraws the scene
        }
        private void OpenPmx(string fileName)
        {
            Clear();
            //
            OpenedFileName = fileName;
            //
            Controller tmp = null;
            object[] data = null;
            string fileVersion;
            //
            data = TryReadCompressedPmx(fileName, out _model, out _allResults, out fileVersion);
            if (data != null && data.Length == 1 && (string)data[0] == "IncompatibleVersion")
            {
                New();
                return;
            }
            if (data == null) data = TryReadUncompressedPmx(fileName, out _model, out _allResults);
            if (data == null || data.Length < 3)
                throw new Exception("The file cannot be read. It is either corrupt or was created by a previous version.");
            // Get controller
            tmp = (Controller)data[0];
            // Commands
            _commands.EnableDisableUndoRedo -= _commands_CommandExecuted;
            _commands = new Commands.CommandsCollection(this, tmp._commands); // to recreate the history file
            _commands.WriteOutput = _form.WriteDataToOutput;
            _commands.ModelChanged_ResetJobStatus = ResetAllJobStatus;
            _commands.EnableDisableUndoRedo += _commands_CommandExecuted;
            _commands.OnEnableDisableUndoRedo();
            // Annotations
            _annotations = new AnnotationContainer(tmp._annotations, this);
            // Jobs
            _jobs = (OrderedDictionary<string, AnalysisJob>)data[1];
            // Settings
            ApplySettings(); // work folder and executable
            // After settings reset jobs
            ResetAllJobStatus();
            // Determine view
            _currentView = ViewGeometryModelResults.Geometry;
            if (_model != null && _model.Mesh != null && _model.Mesh.Parts.Count > 0)
                _currentView = ViewGeometryModelResults.Model;
            else if (_allResults.Count > 0) _currentView = ViewGeometryModelResults.Results;
            // Set view
            _form.SetCurrentView(_currentView);
            // Regenerate tree
            _form.RegenerateTree(false);
            // Set tree states
            if (data[2] is bool[][] states) _form.SetTreeExpandCollapseState(states);
            //
            //JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            //string json = JsonConvert.SerializeObject(_commands, Formatting.Indented, settings);
            //File.WriteAllText(@"D:\out.txt", json);
        }
        private void OpenPmh(string fileName)
        {
            New();
            //
            _commands.ReadFromFile(fileName);
            //
            Commands.CSaveToPmx lastSave = _commands.GetLastSaveCommnad();
            if (lastSave != null)
            {
                Commands.CommandsCollection prevCommands = new Commands.CommandsCollection(this, _commands);
                _form.Open(lastSave.FileName, Open, true);    // form open redraws the scene
                _commands = new Commands.CommandsCollection(this, prevCommands);
                //
                _commands.ExecuteAllCommandsFromLastSave(lastSave);
            }
            else
            {
                _commands.ExecuteAllCommands();
            }
            // Model changed
            _modelChanged = true;
        }
        private void OpenFrd(string fileName)
        {
            FeResults results;
            bool useWearResults = _wearResults != null;
            bool readDatFile = !useWearResults;
            //
            if (useWearResults)
            {
                results = _wearResults;
                _wearResults = null;
            }
            else results = FrdFileReader.Read(fileName);
            //
            if (results == null || results.Mesh == null)
            {
                MessageBoxes.ShowError("The results file does not exist or is empty.");
                return;
            }
            //
            LoadResults(results, readDatFile);
        }
        private void OpenDat(string fileName, bool redraw = true)
        {
            try
            {
                if (_allResults.CurrentResult == null) _allResults.Add(fileName, new FeResults(fileName));
                // This is also called in AppendResults
                _allResults.CurrentResult.SetHistory(DatFileReader.Read(fileName));
                // Wear
                _allResults.CurrentResult.ComputeWear(_model.StepCollection.GetSlipWearStepIds(),
                                                      _model.GetNodalSlipWearCoefficients(),
                                                      _model.Properties.NumOfSmoothingSteps,
                                                      null);
                //
                if (_allResults.CurrentResult.GetHistory() == null)
                {
                    MessageBoxes.ShowError("The dat file does not exist or is empty.");
                    return;
                }
                else
                {
                    if (redraw)
                    {
                        // Set the view but do not draw
                        _currentView = ViewGeometryModelResults.Results;
                        _form.SetCurrentView(_currentView);
                        // Regenerate tree
                        _form.RegenerateTree();
                    }
                    // Model changed
                    _modelChanged = true;
                }
            }
            // do not throw error in order to open the results
            catch
            {
                MessageBoxes.ShowError($"Could not load {fileName}");
            }
        }
        private void OpenCel(string fileName, bool redraw = true)
        {
            Dictionary<int, FeElement> elements;
            Dictionary<string, FeElementSet> elementSets;
            FileInOut.Input.InpFileReader.ReadCel(fileName, out elements, out elementSets);
            //_results.Mesh.Elements.AddRange(elements);
            //_results.Mesh.ElementSets.AddRange(elementSets);
            //_results.Mesh.CreatePartsFromElementSets(elementSets.Keys.ToArray(),
            //                                         out BasePart[] modifiedParts,
            //                                         out BasePart[] newParts);
            //
            if (elements == null)
            {
                MessageBoxes.ShowError("The cel file does not exist or is empty.");
                return;
            }
            else
            {
                Dictionary<string, FeNodeSet> nodeSets = GetNodeSetsFromCelElements(_allResults.CurrentResult.Mesh.Nodes,
                                                                                    elements,
                                                                                    elementSets);
                _allResults.CurrentResult.Mesh.NodeSets.AddRange(nodeSets);
                //
                if (redraw)
                {
                    // Set the view but do not draw
                    _currentView = ViewGeometryModelResults.Results;
                    _form.SetCurrentView(_currentView);
                    // Regenerate tree
                    _form.RegenerateTree();

                }
                // Model changed
                _modelChanged = true;
            }
        }
        private void OpenNam(string fileName, bool redraw = true)
        {
            string[] nodeSetNames;
            int[][] nodeIds;
            FileInOut.Input.InpFileReader.ReadNam(fileName, out nodeSetNames, out nodeIds);
            //
            if (nodeSetNames == null || nodeSetNames.Length == 0)
            {
                MessageBoxes.ShowError("The file " + fileName + " does not exist or is empty.");
                return;
            }
            else
            {
                string name;
                HashSet<string> existingNames = _allResults.CurrentResult.Mesh.NodeSets.Keys.ToHashSet();
                Dictionary<string, FeNodeSet> nodeSets = new Dictionary<string, FeNodeSet>();
                for (int i = 0; i < nodeSetNames.Length; i++)
                {
                    name = nodeSetNames[i];
                    if (existingNames.Contains(name)) name = _allResults.CurrentResult.Mesh.NodeSets.GetNextNumberedKey(name);
                    //
                    nodeSets.Add(name, new FeNodeSet(name, nodeIds[i]));
                }
                _allResults.CurrentResult.Mesh.NodeSets.AddRange(nodeSets);
                //
                if (redraw)
                {
                    // Set the view but do not draw
                    _currentView = ViewGeometryModelResults.Results;
                    _form.SetCurrentView(_currentView);
                    // Regenerate tree
                    _form.RegenerateTree();

                }
                // Model changed
                _modelChanged = true;
            }
        }
        private void OpenFoam(string fileName)
        {
            FeResults results = OpenFoamFileReader.Read(fileName, _model.UnitSystem.UnitSystemType);
            if (results == null) throw new CaeException("The results file cannot be read.");
            // Load results
            _form.Clear3D();
            ClearResults();
            //
            _allResults.Add(results.FileName, results);
            // Model changed
            _modelChanged = true;
            // Redraw
            // Set the view but do not draw
            _currentView = ViewGeometryModelResults.Results;
            _form.SetCurrentView(_currentView);
            // Regenerate tree
            _form.RegenerateTree();
        }
        private void LoadResults(FeResults results, bool readDatFile)
        {
            // Load results
            _form.Clear3D();
            if (_allResults.Count == 0) ClearResults();
            _allResults.Add(results.FileName, results);
            // Check if the meshes are the same and rename the parts
            if (_model.Mesh != null && _allResults.CurrentResult.Mesh != null &&
                _model.HashName == _allResults.CurrentResult.HashName)
            {
                SuppressExplodedView();
                //
                double similarity = _model.Mesh.IsEqual(_allResults.CurrentResult.Mesh);
                //
                if (similarity > 0)
                {
                    if (similarity < 1)
                    {
                        if (MessageBox.Show("Some node coordinates in the result .frd file are different from " +
                                            "the coordinates in the model mesh." + Environment.NewLine + Environment.NewLine +
                                            "Apply model mesh properties (part names, geometry...) to the result mesh?",
                                            "Warning",
                                            MessageBoxButtons.YesNo,
                                            MessageBoxIcon.Warning) == DialogResult.Yes) similarity = 1;
                    }
                    //
                    if (similarity == 1)
                    {
                        _allResults.CurrentResult.CopyPartsFromMesh(_model.Mesh);
                        _allResults.CurrentResult.CopyMeshItemsFromMesh(_model.Mesh);
                    }
                    else if (similarity == 2)
                    {
                        _allResults.CurrentResult.Mesh.MergePartsBasedOnMesh(_model.Mesh, typeof(ResultPart));
                    }
                }
                //
                ResumeExplodedViews(false); // must be here after the MergePartsBasedOnMesh
            }
            // Model changed
            _modelChanged = true;
            // Open .cel file
            if (_allResults.CurrentResult.FileName != null && _allResults.CurrentResult.FileName.Length > 0)
            {
                string celFileName = Path.GetFileNameWithoutExtension(_allResults.CurrentResult.FileName) + ".cel";
                celFileName = Path.Combine(Path.GetDirectoryName(_allResults.CurrentResult.FileName), celFileName);
                if (File.Exists(celFileName)) OpenCel(celFileName, false);
            }
            // Open .nam file
            if (_allResults.CurrentResult.FileName != null && _allResults.CurrentResult.FileName.Length > 0)
            {
                string namFileName = Path.GetFileNameWithoutExtension(_allResults.CurrentResult.FileName) +
                                     "_WarnNodeMissTiedContact.nam";
                namFileName = Path.Combine(Path.GetDirectoryName(_allResults.CurrentResult.FileName), namFileName);
                if (File.Exists(namFileName)) OpenNam(namFileName, false);
            }
            // Open .dat file
            if (readDatFile)
            {
                string datFileName = Path.GetFileNameWithoutExtension(_allResults.CurrentResult.FileName) + ".dat";
                datFileName = Path.Combine(Path.GetDirectoryName(_allResults.CurrentResult.FileName), datFileName);
                if (File.Exists(datFileName)) OpenDat(datFileName, false);
            }
            // Redraw
            // Set the view but do not draw
            _currentView = ViewGeometryModelResults.Results;
            _form.SetCurrentView(_currentView);
            // Regenerate tree
            _form.RegenerateTree();
        }
        public void AppendResult(string fileName)
        {
            if (_allResults != null && _allResults.Count == 0) Open(fileName);
            else
            {
                FeResults results = FrdFileReader.Read(fileName);
                // Open .dat file
                string datFileName = Path.GetFileNameWithoutExtension(results.FileName) + ".dat";
                datFileName = Path.Combine(Path.GetDirectoryName(results.FileName), datFileName);
                if (File.Exists(datFileName)) results.SetHistory(DatFileReader.Read(datFileName));
                // Check if the meshes are the same and rename the parts
                if (_allResults.CurrentResult.Mesh != null && results.Mesh != null)
                {
                    SuppressExplodedView();
                    //
                    double similarity = _allResults.CurrentResult.Mesh.IsEqual(results.Mesh);
                    //
                    if (similarity > 0)
                    {
                        if (similarity < 1) similarity = 1;
                        //
                        if (similarity == 1)
                        {
                            results.CopyPartsFromMesh(_allResults.CurrentResult.Mesh);
                            //_allResults.CurrentResult.CopyMeshItemsFromMesh(_allResults.CurrentResult.Mesh);
                        }
                        else if (similarity == 2)
                        {
                            results.Mesh.MergePartsBasedOnMesh(_allResults.CurrentResult.Mesh, typeof(ResultPart));
                        }
                        _allResults.CurrentResult.AddResults(results);
                    }
                    // First resume exploded view
                    ResumeExplodedViews(false); // must be here after the MergePartsBasedOnMesh
                    //
                    if (similarity == 0)
                    {
                        // Do not show error!
                        throw new CaeException("The selected result file does not have the same mesh as the current result.");
                    }
                }
                // Model changed
                _modelChanged = true;
                // Redraw
                // Set the view but do not draw
                _currentView = ViewGeometryModelResults.Results;
                _form.SetCurrentView(_currentView);
                // Regenerate tree
                _form.RegenerateTree();
                //
                // Get first component of the first field for the last increment in the last step
                if (ResultsInitialized)
                    CurrentFieldData = _allResults.CurrentResult.GetFirstComponentOfTheFirstFieldAtDefaultIncrement();
            }
        }
        private Dictionary<string, FeNodeSet> GetNodeSetsFromCelElements(Dictionary<int, FeNode> nodes,
                                                                         Dictionary<int, FeElement> elements,
                                                                         Dictionary<string, FeElementSet> elementSets)
        {
            int count = 0;
            HashSet<int> nodeIds = new HashSet<int>();
            Dictionary<string, FeNodeSet> nodeSets = new Dictionary<string, FeNodeSet>();
            //
            foreach (var entry in elementSets)
            {
                nodeIds.Clear();
                foreach (var elementId in entry.Value.Labels) nodeIds.UnionWith(elements[elementId].NodeIds);
                nodeIds.IntersectWith(nodes.Keys);
                nodeSets.Add(entry.Key, new FeNodeSet(entry.Key, nodeIds.ToArray()));
                count += nodeIds.Count();
            }
            //
            if (count == 0) MessageBoxes.ShowWarning("Turn on the 3D output option in the field outputs " +
                                                     "to enable viewing of the contact element nodes.");
            //
            return nodeSets;
        }
        // Read pmx
        private object[] TryReadCompressedPmx(string fileName, out FeModel model, out ResultsCollection allResults,
                                              out string fileVersion)
        {
            model = null;
            allResults = null;
            fileVersion = null;
            bool oldResults = false;
            try
            {
                object[] data = null;
                Controller tmp = null;
                byte[] versionBuffer = new byte[32];
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    fs.Read(versionBuffer, 0, 32);
                    fileVersion = Encoding.ASCII.GetString(versionBuffer).TrimEnd(new char[] { '\0' });
                    //
                    if (fileVersion != Globals.ProgramName)
                    {
                        _form.WriteDataToOutput("Warning: The opened file is from an incompatible version: " + fileVersion);
                        _form.WriteDataToOutput("Some items might not be loaded correctly. Check the model.");
                    }
                    //
                    using (BinaryReader br = new BinaryReader(Decompress(fs)))
                    {
                        data = Tools.LoadDumpFromFile<object[]>(br);
                        tmp = (Controller)data[0];
                        model = tmp._model;
                        // Compatibility v.1.3.5
                        Selection selection;
                        foreach (var entry in model.Geometry.Parts)
                        {
                            if (entry.Value is GeometryPart gp && gp.MeshingParameters != null)
                            {
                                gp.MeshingParameters.FactorMax = MeshingParameters.DefaultFactorMax;
                                gp.MeshingParameters.FactorMin = MeshingParameters.DefaultFactorMin;
                                gp.MeshingParameters.FactorHausdorff = MeshingParameters.DefaultFactorHausdorff;
                                gp.MeshingParameters.SetCheckName(true);
                                string name = model.Geometry.MeshSetupItems.GetNextNumberedKey("Meshing_Parameters");
                                gp.MeshingParameters.Name = name;
                                gp.MeshingParameters.Active = true;
                                gp.MeshingParameters.Visible = true;
                                gp.MeshingParameters.Valid = true;
                                gp.MeshingParameters.Internal = false;
                                //
                                gp.MeshingParameters.CreationIds = new int[] { gp.PartId };
                                selection = new Selection();
                                selection.CurrentView = (int)ViewGeometryModelResults.Geometry;
                                selection.SelectItem = vtkSelectItem.Part;
                                selection.Add(new SelectionNodeIds(vtkSelectOperation.None, false,
                                                                   gp.MeshingParameters.CreationIds));
                                gp.MeshingParameters.CreationData = selection;
                                //
                                model.Geometry.MeshSetupItems.Add(gp.MeshingParameters.Name, gp.MeshingParameters.DeepClone());
                                gp.MeshingParameters = null;
                            }
                        }
                        // Compatibility v.1.3.3
                        if (tmp._allResults == null)
                        {
                            oldResults = true;
                            allResults = new ResultsCollection();
                            if (tmp._results != null) allResults.Add(tmp._results.HashName, tmp._results);
                        }
                        else allResults = tmp._allResults;
                        //
                        string[] versions = fileVersion.Split(new string[] { " ", ".", "v" },
                                                              StringSplitOptions.RemoveEmptyEntries);
                        int major;
                        int minor;
                        int build;
                        int.TryParse(versions[1], out major);
                        int.TryParse(versions[2], out minor);
                        int.TryParse(versions[3], out build);
                        //
                        int version = major * 1_000_000 + minor * 1000 + build;
                        FeModel.ReadFromFile(model, br, version);
                        //
                        if (oldResults) FeResults.ReadFromFile(allResults.CurrentResult, br, version);
                        else if (ResultsCollection.ReadFromFile(allResults, br, version)) ;
                        else
                        {
                            _form.WriteDataToOutput("Warning: There were errors reading the results.");
                            _form.WriteDataToOutput("Some results might not be loaded correctly. Check the results.");
                        }
                    }
                    //
                    model.UpdateMeshPartsElementTypes(true);
                }
                return data;
            }
            catch (Exception ex)
            {
                if (ex.Message == "IncompatibleVersion") return new object[] { ex.Message };
                else return null;
            }
        }
        private object[] TryReadUncompressedPmx(string fileName, out FeModel model, out ResultsCollection allResults)
        {
            allResults = new ResultsCollection();
            //
            try
            {
                object[] data = null;
                Controller tmp = null;
                //
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                using (BinaryReader br = new BinaryReader(fs))
                {
                    data = CaeGlobals.Tools.LoadDumpFromFile<object[]>(br);
                    tmp = (Controller)data[0];
                    model = tmp._model;
                    allResults.Add(tmp._results.FileName, tmp._results);
                    //
                    FeModel.ReadFromFile(model, br, 0_000_000);
                    FeResults.ReadFromFile(allResults.CurrentResult, br, 0_000_000);
                }
                //
                model.UpdateMeshPartsElementTypes(true);
                //
                return data;
            }
            catch
            {
                model = null;
                return null;
            }
        }
        // Import
        public string GetFileNameToImport(bool onlyMaterials)
        {
            return _form.GetFileNameToImport(onlyMaterials);
        }
        public async Task ImportFileAsync(string fileName, bool onlyMaterials)
        {
            await Task.Run(() => ImportFileCommand(fileName, onlyMaterials));
        }
        public void ImportFile(string fileName, bool onlyMaterials)
        {
            if (!File.Exists(fileName)) throw new FileNotFoundException("The file: '" + fileName + "' does not exist.");
            //
            string extension = Path.GetExtension(fileName).ToLower();
            // Import
            if (extension == ".stl")
            {
                string[] addedPartNames = _model.ImportGeometryFromStlFile(fileName);
                if (addedPartNames == null)
                {
                    UserControls.AutoClosingMessageBox.ShowError("There are errors in the imported geometry.", 3000);
                }
                else
                {
                    List<string> largeModels = new List<string>();
                    foreach (var partName in addedPartNames)
                    {
                        if (_model.Geometry.Parts[partName].Labels.Length > 1E5) largeModels.Add(partName);
                    }
                    if (largeModels.Count > 0)
                    {
                        _form.WriteDataToOutput("Feature edge detection was turned off due to a high number of .stl triangles.");
                        _form.WriteDataToOutput("Use the following menu to turn it back on: Geometry -> Find Model Edges by Angle");
                    }
                }
            }
            else if (extension == ".stp" || extension == ".step")
                ImportCADAssemblyFile(fileName, "STEP_ASSEMBLY_SPLIT_TO_COMPOUNDS");
            else if (extension == ".igs" || extension == ".iges")
                ImportCADAssemblyFile(fileName, "IGES_ASSEMBLY_SPLIT_TO_COMPOUNDS");
            else if (extension == ".brep")
                ImportCADAssemblyFile(fileName, "BREP_ASSEMBLY_SPLIT_TO_COMPOUNDS");
            else if (extension == ".vol")
                _model.ImportMeshFromVolFile(fileName);
            else if (extension == ".mesh")
                _model.ImportMeshFromMmgFile(fileName);
            else if (extension == ".inp" && onlyMaterials)
                _errors = _model.ImportMaterialsFromInpFile(fileName, _form.WriteDataToOutput);
            else if (extension == ".inp")
                _errors = _model.ImportModelFromInpFile(fileName, _form.WriteDataToOutput);
            else if (extension == ".unv")
                _model.ImportMeshFromUnvFile(fileName);
            else if (extension == ".obj")
                _model.ImportMeshFromObjFile(fileName);
            else throw new NotSupportedException();
            //
            UpdateAfterImport(extension);
        }
        private void UpdateAfterImport(string extension)
        {
            // Exploded view
            UpdateExplodedView(false);
            // Visualization
            if (extension == ".stl" || extension == ".stp" || extension == ".step" ||
                extension == ".igs" || extension == ".iges" || extension == ".brep")
            {
                _currentView = ViewGeometryModelResults.Geometry;
                _form.SetCurrentView(_currentView);
                DrawGeometry(false);
            }
            else if (extension == ".unv" || extension == ".vol" || extension == ".inp" || extension == ".mesh" ||
                     extension == ".obj")
            {
                // Element types
                _model.UpdateMeshPartsElementTypes(true);
                //
                _currentView = ViewGeometryModelResults.Model;
                _form.SetCurrentView(_currentView);
                DrawModel(false);
            }
            // Regenerate
            _form.RegenerateTree();
            //
            if (extension == ".inp") CheckAndUpdateModelValidity();  // must be here at the last place
        }
        public string[] ImportCADAssemblyFile(string assemblyFileName, string splitCommand)
        {
            string[] filesToImport = SplitAssembly(assemblyFileName, splitCommand);
            string[] addedPartNames;
            List<string> allAddedPartNames = new List<string>();
            //
            if (filesToImport != null)
            {
                foreach (var partFileName in filesToImport)
                {
                    try
                    {
                        if (partFileName.ToLower().Contains("compound"))
                        {
                            ImportBrepCompoundPart(partFileName, null, out string compoundPartName, out addedPartNames);
                        }
                        else
                        {
                            addedPartNames = ImportBrepPartFile(partFileName);
                        }
                        if (addedPartNames != null) allAddedPartNames.AddRange(addedPartNames);
                    }
                    catch (Exception ex)
                    {
                        string[] lines = ex.StackTrace.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                        string error = ex.Message;
                        if (!(ex is CaeException))
                        {
                            for (int i = 0; i < lines.Length; i++)
                            {
                                if (lines[i].Contains("PrePoMax"))
                                {
                                    error += Environment.NewLine + lines[i];
                                    break;
                                }
                            }
                        }
                        _errors.Add("The file " + assemblyFileName + " could not be imported correctly: " +
                                    Environment.NewLine + error);
                    }
                    //
                    if (File.Exists(partFileName)) File.Delete(partFileName);
                }
            }
            //
            return allAddedPartNames.ToArray();
        }
        //
        public string[] SplitAssembly(string assemblyFileName, string splitCommand)
        {
            CalculixSettings settings = _settings.Calculix;
            if (settings.WorkDirectory == null || !Directory.Exists(settings.WorkDirectory))
            {
                MessageBoxes.ShowWorkDirectoryError();
                return null;
            }
            //
            string executable = Application.StartupPath + Globals.NetGenMesher;
            string outFileName = Tools.GetNonExistentRandomFileName(settings.WorkDirectory, ".brep");
            //
            string argument = splitCommand +
                              " \"" + assemblyFileName.ToUTF8() + "\"" +
                              " \"" + outFileName.ToUTF8() + "\"";
            //
            _executableJob = new ExecutableJob("SplitStep", executable, argument, settings.WorkDirectory);
            _executableJob.AppendOutput += executableJob_AppendOutput;
            _executableJob.Submit();
            //
            string brepFile;
            List<string> brepFiles = new List<string>();
            string outFileNameNoExtension = Path.GetFileNameWithoutExtension(outFileName);
            string searchPattern = "*" + outFileNameNoExtension + "*";
            //
            if (_executableJob.JobStatus == JobStatus.OK)
            {
                string[] allFiles = Directory.GetFiles(settings.WorkDirectory, searchPattern);
                foreach (var fileName in allFiles)
                {
                    brepFile = Path.GetFileName(fileName);
                    if (brepFile.StartsWith(outFileNameNoExtension)) brepFiles.Add(fileName);
                }
                //
                return brepFiles.ToArray();
            }
            else return null;
        }
        public void CreateAndImportCompoundPart(string[] partNames, out string compoundPartName, out string[] importedPartNames)
        {
            compoundPartName = null;
            importedPartNames = null;
            //
            GeometryPart part;
            HashSet<PartType> stlPartTypes = new HashSet<PartType>();
            HashSet<PartType> cadPartTypes = new HashSet<PartType>();
            //
            string[] allPartNames = GetMeshablePartNames(partNames);
            foreach (var partName in allPartNames)
            {
                part = (GeometryPart)_model.Geometry.Parts[partName];
                if (part.IsCADPart) cadPartTypes.Add(part.PartType);
                else stlPartTypes.Add(part.PartType);
            }
            if (stlPartTypes.Count + cadPartTypes.Count != 1) throw new NotSupportedException();
            //
            if (stlPartTypes.Count > 0)
            {
                GeometryPart geometryPart;
                string[] mergedPartNames;
                FeMesh mesh = _model.Geometry.DeepCopy();
                mesh.MergeGeometryParts(partNames, out geometryPart, out mergedPartNames);
                // Hide parts
                HideGeometryParts(mergedPartNames);
                // Add parts
                _model.Geometry.AddPartsFromMesh(mesh, new string[] { geometryPart.Name }, null);
                //
                UpdateAfterImport(".stl");
                //
                importedPartNames = new string[] { geometryPart.Name };
            }
            else if (cadPartTypes.Count > 0)
            {
                string[] createdFileNames = CreateBrepCompoundPart(partNames);
                //
                if (createdFileNames.Length == 1)
                {
                    string brepFileName = createdFileNames[0];
                    HideGeometryParts(partNames);
                    ImportBrepCompoundPart(brepFileName, partNames, out compoundPartName, out importedPartNames);
                }
            }
            else throw new NotSupportedException();
        }
        public string[] CreateBrepCompoundPart(string[] partNames)
        {
            string workDirectory = _settings.Calculix.WorkDirectory;
            if (workDirectory == null || !Directory.Exists(workDirectory))
            {
                MessageBoxes.ShowWorkDirectoryError();
                return null;
            }
            //
            string executable = Application.StartupPath + Globals.NetGenMesher;
            string inFileName = Tools.GetNonExistentRandomFileName(workDirectory);
            string[] inFileNames = new string[partNames.Length];
            for (int i = 0; i < partNames.Length; i++)
                inFileNames[i] = inFileName + "_" + (i + 1).ToString().PadLeft(4, '0') + ".brep";
            string brepFileName = Path.Combine(workDirectory, Globals.BrepFileName);
            //
            if (File.Exists(brepFileName)) File.Delete(brepFileName);
            // Write CAD
            for (int i = 0; i < partNames.Length; i++)
                File.WriteAllText(inFileNames[i], ((GeometryPart)_model.Geometry.Parts[partNames[i]]).CADFileData);
            //
            string argument = "BREP_COMPOUND";
            for (int i = 0; i < inFileNames.Length; i++) argument += " \"" + inFileNames[i].ToUTF8() + "\"";
            argument += " \"" + brepFileName.ToUTF8() + "\"";
            //
            _executableJob = new ExecutableJob("CompoundPart", executable, argument, workDirectory);
            _executableJob.AppendOutput += executableJob_AppendOutput;
            _executableJob.Submit();
            //
            for (int i = 0; i < inFileNames.Length; i++)
            {
                if (File.Exists(inFileNames[i])) File.Delete(inFileNames[i]);
            }
            //
            if (_executableJob.JobStatus == JobStatus.OK) return new string[] { brepFileName };
            else return null;
        }
        private void ImportBrepCompoundPart(string brepFileName, string[] createdFromPartNames,
                                            out string compoundPartName, out string[] importedPartNames)
        {
            compoundPartName = _model.Geometry.Parts.GetNextNumberedKey("Compound");
            importedPartNames = ImportCADAssemblyFile(brepFileName, "BREP_ASSEMBLY_SPLIT_TO_PARTS");
            //
            if (importedPartNames.Length == 1)  // only one part was imported - shell compound
            {
                // Rename the part
                PartProperties properties = _model.Geometry.Parts[importedPartNames[0]].GetProperties();
                properties.Name = compoundPartName;
                ReplaceGeometryPartProperties(importedPartNames[0], properties);
            }
            else
            {
                // Create compound part
                CompoundGeometryPart compPart = new CompoundGeometryPart(compoundPartName, createdFromPartNames,
                                                                         importedPartNames);
                for (int i = 0; i < importedPartNames.Length; i++)
                    compPart.BoundingBox.IncludeBox(_model.Geometry.Parts[importedPartNames[i]].BoundingBox);
                compPart.CADFileDataFromFile(brepFileName);
                compPart.PartId = _model.Geometry.GetMaxPartId() + 1;
                _model.Geometry.SetPartColorFromColorTable(compPart);
                _model.Geometry.Parts.Add(compoundPartName, compPart);
            }
            //
            UpdateAfterImport(".brep");
        }
        //
        public string[] ImportBrepPartFile(string brepFileName, bool showError = true)
        {
            CalculixSettings calculixSettings = _settings.Calculix;
            //
            if (calculixSettings.WorkDirectory == null || !Directory.Exists(calculixSettings.WorkDirectory))
            {
                MessageBoxes.ShowWorkDirectoryError();
                return null;
            }
            //
            string executable = Application.StartupPath + Globals.NetGenMesher;
            string visFileName = Path.Combine(calculixSettings.WorkDirectory, Globals.VisFileName);
            //
            if (File.Exists(visFileName)) File.Delete(visFileName);
            //
            string argument = "BREP_VISUALIZATION " +
                              "\"" + brepFileName.ToUTF8() + "\" " +
                              "\"" + visFileName + "\" " +
                              _settings.Graphics.GeometryDeflection.ToString();
            //
            _executableJob = new ExecutableJob("Brep", executable, argument, calculixSettings.WorkDirectory);
            _executableJob.AppendOutput += executableJob_AppendOutput;
            _executableJob.Submit();
            //
            if (_executableJob.JobStatus == JobStatus.OK)
            {
                string[] addedPartNames = _model.ImportGeometryFromBrepFile(visFileName, brepFileName);
                if (addedPartNames.Length == 0)
                {
                    if (showError) MessageBoxes.ShowError("No geometry to import.");
                    return null;
                }
                return addedPartNames;
            }
            else
            {
                if (showError) MessageBoxes.ShowError("Importing brep file failed.");
                return null;
            }
        }
        private void executableJob_AppendOutput(string data)
        {
            _form.WriteDataToOutput(data);
        }
        public void ImportGeneratedMesh(string fileName, BasePart part, bool fromBrep)
        {
            if (!File.Exists(fileName))
                throw new CaeException("The file: '" + fileName + "' does not exist." + Environment.NewLine +
                                       "The reason is a failed mesh generation procedure for part: " + part.Name);
            //
            List<string> partNames = new List<string>();
            if (part is CompoundGeometryPart cgp) { partNames.Add(cgp.Name); partNames.AddRange(cgp.SubPartNames); }
            else partNames.Add(part.Name);
            //
            int[] removedPartIds = RemoveModelParts(partNames.ToArray(), false, true);
            //
            bool convertToSecondOrder = false;
            bool splitCompoundMesh = false;
            bool mergeCompoundParts = false;
            MeshingParameters meshingParameters;
            if (part is GeometryPart gp)
            {
                meshingParameters = GetPartMeshingParameters(gp.Name);
                // Convert mesh to second order
                if (Path.GetExtension(fileName) == ".mesh") convertToSecondOrder = meshingParameters.SecondOrder;   // mmg
                else if (Path.GetExtension(fileName) == ".inp") convertToSecondOrder = false;   // Gmsh already converted it
                else convertToSecondOrder = meshingParameters.SecondOrder && !meshingParameters.MidsideNodesOnGeometry;
                //
                if (convertToSecondOrder) _form.WriteDataToOutput("Converting mesh to second order...");
                // Split compound mesh
                splitCompoundMesh = meshingParameters.SplitCompoundMesh;
                // Merge compound parts
                mergeCompoundParts = meshingParameters.MergeCompoundParts;
            }
            // Import, convert and split mesh
            _model.ImportGeneratedMeshFromMeshFile(fileName, part, convertToSecondOrder, splitCompoundMesh, mergeCompoundParts);
            // Calculate the number of new nodes and elements
            BasePart basePart;
            if (convertToSecondOrder || Path.GetExtension(fileName) == ".inp")
            {
                int numPoints = 0;
                int numElements = 0;
                foreach (var partName in partNames)
                {
                    if (_model.Mesh.Parts.TryGetValue(partName, out basePart))
                    {
                        numPoints += basePart.NodeLabels.Length;
                        numElements += basePart.Labels.Length;
                    }
                }
                _form.WriteDataToOutput("Nodes: " + numPoints);
                _form.WriteDataToOutput("Elements: " + numElements);
            }
            // This is not executed for the first meshing                               
            // For geometry based sets the part id must remain the same after remesh    
            bool renumbered = false;
            if (removedPartIds != null)
            {
                for (int i = 0; i < removedPartIds.Length; i++)
                {
                    if (removedPartIds[i] != -1 && _model.Mesh.Parts.ContainsKey(partNames[i]))    // compound
                    {
                        _model.Mesh.ChangePartId(partNames[i], removedPartIds[i]);
                        renumbered = true;
                        // Set finite element types from previous meshing
                        GetModelPart(partNames[i]).SetElementTypeEnums(GetGeometryPart(partNames[i]).GetElementTypeEnums());
                    }
                }
            }
            // Update finite element types based on model dimensionality
            _model.UpdateMeshPartsElementTypes(false);
            // Shading
            if (fromBrep)
            {
                foreach (var partName in partNames)
                {
                    if (_model.Mesh.Parts.TryGetValue(partName, out basePart)) basePart.SmoothShaded = true;
                }
            }
            // Regenerate and change the DisplayedMesh to Model before updating sets
            _form.Clear3D();
            _currentView = ViewGeometryModelResults.Model;
            _form.SetCurrentView(_currentView);
            // Redraw to be able to update sets based on selection
            FeModelUpdate(UpdateType.DrawModel);
            // At the end update the sets
            if (renumbered)
            {
                // Update sets - must be called with rendering off - SetStateWorking
                UpdateGeometryBasedItems(false);
            }
            // Update the sets and symbols
            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
            // Regenerate tree
            _form.RegenerateTree(true);
        }
        public void ImportGeneratedRemesh(string fileName, int[] elementIds, BasePart part,
                                          bool convertToSecondOrder, Dictionary<int[], FeNode> midNodes,
                                          bool preview)
        {
            if (!File.Exists(fileName))
                throw new CaeException("The file: '" + fileName + "' does not exist." + Environment.NewLine +
                                       "The reason is a failed mesh generation procedure for part: " + part.Name);
            //
            if (preview)
            {
                int id2;
                int[] key;
                FeElement element;
                CompareIntArray comparer = new CompareIntArray();
                HashSet<int[]> edgeKeys = new HashSet<int[]>(comparer);
                List<double[][]> lines = new List<double[][]>();
                double[][] line;
                //
                FeMesh mmgMesh = FileInOut.Input.MmgFileReader.Read(fileName, MeshRepresentation.Mesh);
                //
                foreach (var entry in mmgMesh.Elements)
                {
                    element = entry.Value;
                    if (entry.Value is LinearTriangleElement lte)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            id2 = (i + 1) % 3;
                            key = Tools.GetSortedKey(element.NodeIds[i], element.NodeIds[id2]);
                            if (!edgeKeys.Contains(key))
                            {
                                line = new double[2][];
                                line[0] = mmgMesh.Nodes[element.NodeIds[i]].Coor;
                                line[1] = mmgMesh.Nodes[element.NodeIds[id2]].Coor;
                                lines.Add(line);
                                edgeKeys.Add(key);
                            }
                        }
                    }
                    else throw new NotSupportedException();
                }
                //
                HighlightConnectedEdges(lines.ToArray());
            }
            else
            {
                _model.ImportGeneratedRemeshFromMeshFile(fileName, elementIds, part, convertToSecondOrder, midNodes);
                // Update finite element types based on model dimensionality
                _model.UpdateMeshPartsElementTypes(false);
                // Regenerate and change the DisplayedMesh to Model before updating sets
                _form.Clear3D();
                _currentView = ViewGeometryModelResults.Model;
                _form.SetCurrentView(_currentView);
                // Regenerate tree
                _form.RegenerateTree();
                // Redraw to be able to update sets based on selection
                FeModelUpdate(UpdateType.DrawModel);
                // At the end update the sets
                // Update sets - must be called with rendering off - SetStateWorking
                UpdateGeometryBasedItems(false);
                // Update the sets and symbols
                FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
            }
        }
        private void UpdateGeometryBasedItems(bool feModelUpdate)
        {
            UpdateNodeSetsBasedOnGeometry(feModelUpdate);
            UpdateElementSetsBasedOnGeometry(feModelUpdate);
            UpdateSurfacesBasedOnGeometry(feModelUpdate);
        }
        // Save
        public string GetFileNameToSaveAs()
        {
            return _form.GetFileNameToSaveAs();
        }
        public void Save()
        {
            if (OpenedFileName != null && Path.GetExtension(OpenedFileName) == ".pmx")
            {
                SaveToPmxCommand(OpenedFileName);
            }
            else SaveAs();
        }
        public void SaveAs()
        {
            string fileName = GetFileNameToSaveAs();
            if (fileName != null) SaveToPmxCommand(fileName);
        }
        public void SaveToPmx(string fileName)
        {
            try
            {
                _savingFile = true;
                //
                PrepareForSaving(this);
                bool[][] states = _form.GetTreeExpandCollapseState();
                OpenedFileName = fileName;
                //
                object[] data = new object[] { this, _jobs, states };
                // Use a temporary file to save the data and copy it at the end
                string tmpFileName = Tools.GetNonExistentRandomFileName(Path.GetDirectoryName(fileName), ".tmp");
                //
                SuppressExplodedView();
                //
                using (BinaryWriter bw = new BinaryWriter(new MemoryStream()))
                using (FileStream fs = new FileStream(tmpFileName, FileMode.Create))
                {
                    ResultsCollection allResults = null;
                    bool saveResults = _settings.General.SaveResultsInPmx;
                    // When controller (data[0]) is dumped to stream, the results should be null if selected
                    if (saveResults == false)
                    {
                        allResults = _allResults;
                        _allResults = new ResultsCollection();
                    }
                    // Controller
                    data.DumpToStream(bw);
                    // Model - data is saved inside data[0]._model but without mesh data - speed up
                    FeModel.WriteToFile(_model, bw);
                    // Results - data is saved inside data[0]._results but without mesh data - speed up
                    ResultsCollection.WriteToFile(_allResults, bw);
                    // After dumping restore the results
                    if (saveResults == false)
                    {
                        _allResults = allResults;
                    }
                    //
                    bw.Flush();
                    bw.BaseStream.Position = 0;
                    //
                    byte[] compressedData = Compress(bw.BaseStream);
                    //
                    byte[] version = Encoding.ASCII.GetBytes(Globals.ProgramName);
                    byte[] versionBuffer = new byte[32];
                    version.CopyTo(versionBuffer, 0);
                    //
                    fs.Write(versionBuffer, 0, 32);
                    fs.Write(compressedData, 0, compressedData.Length);
                }
                //
                ResumeExplodedViews(false);
                //
                File.Copy(tmpFileName, fileName, true);
                File.Delete(tmpFileName);
                // Settings
                AddFileNameToRecentFiles(fileName); // this line redraws the scene
                //
                ApplySettings(); // work folder and executable
                //
                _modelChanged = false;
            }
            catch (Exception ex)
            {
                ResumeExplodedViews(true);
                throw ex;
            }
            finally
            {
                ResetAfterSaving(this);
                _savingFile = false;
            }
        }
        // Export
        public void ExportToCalculix(string fileName, Dictionary<int, double[]> deformations = null)
        {
            SuppressExplodedView();
            FileInOut.Output.CalculixFileWriter.Write(fileName, _model, deformations);
            ResumeExplodedViews(false);
            //
            _form.WriteDataToOutput("Model exported to file: " + fileName);
        }
        public void ExportDeformedPartsToCalculix(string[] partNames, string fileName)
        {
            if (_allResults.CurrentResult != null && _allResults.CurrentResult.Mesh != null)
            {
                SuppressExplodedView();
                FeModel newModel = new FeModel("Deformed");
                newModel.Properties.ModelSpace = ModelSpaceEnum.ThreeD;
                newModel.Mesh.AddPartsFromMesh(_allResults.CurrentResult.Mesh, partNames, null, false, false);
                // Change result parts to mesh parts
                OrderedDictionary<string, BasePart> meshParts =
                    new OrderedDictionary<string, BasePart>("Base Parts", StringComparer.OrdinalIgnoreCase);
                MeshPart meshPart;
                foreach (var entry in newModel.Mesh.Parts)
                {
                    meshPart = new MeshPart(entry.Value);
                    meshParts.Add(meshPart.Name, meshPart);
                }
                newModel.Mesh.Parts = meshParts;
                //
                FileInOut.Output.CalculixFileWriter.Write(fileName, newModel);
                ResumeExplodedViews(false);
                //
                _form.WriteDataToOutput("Deformed mesh exported to file: " + fileName);
            }
        }
        public void ExportToAbaqus(string fileName)
        {
            SuppressExplodedView();
            FileInOut.Output.AbaqusFileWriter.Write(fileName, _model);
            ResumeExplodedViews(false);
            //
            _form.WriteDataToOutput("Model exported to file: " + fileName);
        }
        public void ExportCADGeometryPartsAsStep(string[] partNames, string fileName)
        {
            string stepFileName;
            string directory = Path.GetDirectoryName(fileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            GeometryPart part;
            foreach (var partName in partNames)
            {
                part = (GeometryPart)_model.Geometry.Parts[partName];
                if (partNames.Length == 1) stepFileName = fileName;
                else stepFileName = Path.Combine(directory, fileNameWithoutExtension + "_" + partName + extension);
                ExportCADGeometryPartAsStep(part, stepFileName);
                //
                _form.WriteDataToOutput("Part " + partName + " exported to file: " + stepFileName);
            }
        }
        public void ExportCADGeometryPartsAsBrep(string[] partNames, string fileName)
        {
            string brepFileName;
            string directory = Path.GetDirectoryName(fileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            GeometryPart part;
            foreach (var partName in partNames)
            {
                part = (GeometryPart)_model.Geometry.Parts[partName];
                if (partNames.Length == 1) brepFileName = fileName;
                else brepFileName = Path.Combine(directory, fileNameWithoutExtension + "_" + partName + extension);
                File.WriteAllText(brepFileName, part.CADFileData);
                //
                _form.WriteDataToOutput("Part " + partName + " exported to file: " + brepFileName);
            }
        }
        public void ExportCADGeometryPartAsStep(GeometryPart part, string stepFileName)
        {
            CalculixSettings settings = _settings.Calculix;
            if (settings.WorkDirectory == null || !Directory.Exists(settings.WorkDirectory))
            {
                MessageBoxes.ShowWorkDirectoryError();
                return;
            }
            //
            string executable = Application.StartupPath + Globals.NetGenMesher;
            string brepFileName = Path.Combine(settings.WorkDirectory, Globals.BrepFileName);
            //
            if (File.Exists(brepFileName)) File.Delete(brepFileName);
            if (File.Exists(stepFileName)) File.Delete(stepFileName);
            //
            File.WriteAllText(brepFileName, part.CADFileData);
            //
            string argument = "SAVE_BREP_AS_STEP " +
                              "\"" + brepFileName.ToUTF8() + "\" " +
                              "\"" + stepFileName.ToUTF8() + "\"";
            //
            _executableJob = new ExecutableJob(part.Name, executable, argument, settings.WorkDirectory);
            _executableJob.AppendOutput += executableJobMeshing_AppendOutput;
            _executableJob.Submit();
            // Job completed
            if (_executableJob.JobStatus == JobStatus.OK)
                _form.WriteDataToOutput("Part " + part.Name + " exported to file: " + stepFileName);
            else return;
        }
        public void ExportGeometryPartsAsGmshMesh(string fileName)
        {
            SuppressExplodedView();
            //
            FileInOut.Output.GmshMshFileWriter.Write(fileName, _model.Mesh);
            _form.WriteDataToOutput("Mesh exported to file: " + fileName);
            //
            ResumeExplodedViews(false);
        }
        public void ExportGeometryPartsAsMmgMesh(string[] partNames, string fileName)
        {
            string mmgFileName;
            string directory = Path.GetDirectoryName(fileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            GeometryPart part;
            //
            SuppressExplodedView(partNames);
            //
            foreach (var partName in partNames)
            {
                part = (GeometryPart)_model.Geometry.Parts[partName];
                if (partNames.Length == 1) mmgFileName = fileName;
                else mmgFileName = Path.Combine(directory, fileNameWithoutExtension + "_" + partName + extension);
                FileInOut.Output.MmgFileWriter.Write(mmgFileName, part, _model.Geometry, true, false);
                //
                _form.WriteDataToOutput("Part " + partName + " exported to file: " + mmgFileName);
            }
            //
            ResumeExplodedViews(false);
        }
        public void ExportToStl(string[] partNames, string fileName)
        {
            SuppressExplodedView(partNames);
            //
            FeMesh mesh = DisplayedMesh;
            vtkMaxActorData data;
            List<double[][]> stlTriangles = new List<double[][]>();
            //
            for (int i = 0; i < partNames.Length; i++)
            {
                if (_currentView == ViewGeometryModelResults.Geometry)
                {
                    data = GetGeometryPartActorData(mesh, mesh.Parts[partNames[i]], vtkRendererLayer.Base, false, false);
                }
                else if (_currentView == ViewGeometryModelResults.Model)
                {
                    data = GetModelPartActorData(mesh, mesh.Parts[partNames[i]], vtkRendererLayer.Base, null);
                }
                else if (_currentView == ViewGeometryModelResults.Results)
                {
                    data = GetResultPartActorData((ResultPart)mesh.Parts[partNames[i]], _currentFieldData);
                }
                else throw new NotSupportedException();
                //
                stlTriangles.AddRange(data.GetStlTriangles());
            }
            //
            FileInOut.Output.StlFileWriter.Write(fileName, stlTriangles);
            ResumeExplodedViews(false);
            //
            foreach (var partName in partNames)
            {
                _form.WriteDataToOutput("Part " + partName + " exported to file: " + fileName);
            }
        }
        public void ExportGeometryPartsAsStl(string[] partNames, string fileName)
        {
            SuppressExplodedView(partNames);
            FileInOut.Output.StlFileWriter.Write(fileName, _model.Geometry, partNames);
            ResumeExplodedViews(false);
            //
            foreach (var partName in partNames)
            {
                _form.WriteDataToOutput("Part " + partName + " exported to file: " + fileName);
            }
        }
        public string ExportCADPartGeometryToDefaultFile(GeometryPart part)
        {
            CalculixSettings settings = _settings.Calculix;
            if (settings.WorkDirectory == null || !Directory.Exists(settings.WorkDirectory))
            {
                MessageBoxes.ShowWorkDirectoryError();
                return null;
            }
            //
            string brepFileName = Path.Combine(settings.WorkDirectory, Globals.BrepFileName);
            //
            if (File.Exists(brepFileName)) File.Delete(brepFileName);
            //
            SuppressExplodedView(new string[] { part.Name });
            File.WriteAllText(brepFileName, part.CADFileData);
            ResumeExplodedViews(false);
            //
            return brepFileName;
        }
        //
        private static byte[] Compress(Stream input)
        {
            using (var compressStream = new MemoryStream())
            using (var compressor = new DeflateStream(compressStream, CompressionLevel.Fastest))
            {
                input.CopyTo(compressor);
                compressor.Close();
                return compressStream.ToArray();
            }
        }
        private static Stream Decompress(Stream input)
        {
            var output = new MemoryStream();
            //
            using (var decompressor = new DeflateStream(input, CompressionMode.Decompress))
            {
                decompressor.CopyTo(output);
            }
            //
            output.Position = 0;
            return output;
        }
        // Recent
        private void AddFileNameToRecentFiles(string fileName)
        {
            // Settings
            _settings.General.AddRecentFile(fileName);
            _settings.SaveToFile();
            //
            _form.UpdateRecentFilesThreadSafe(_settings.General.GetRecentFiles());
        }
        public void ClearRecentFiles()
        {
            // Settings
            _settings.General.ClearRecentFiles();
            Settings = _settings;   // save to file
            //
            _form.UpdateRecentFilesThreadSafe(_settings.General.GetRecentFiles());
        }

        #endregion ################################################################################################################

        #region Edit menu   ########################################################################################################
        // COMMANDS ********************************************************************************
        public void SetCalculixUserKeywordsCommand(OrderedDictionary<int[],
                                                   FileInOut.Output.Calculix.CalculixUserKeyword> userKeywords)
        {
            Commands.CSetCalculixUserKeywords comm = new Commands.CSetCalculixUserKeywords(userKeywords);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public void UndoHistory()
        {
            string lastFileName = OpenedFileName;
            _commands.Undo();
            OpenedFileName = lastFileName;
        }
        public void RedoHistory()
        {
            _commands.Redo();
        }
        public void RegenerateHistoryCommands(bool showImportDialog = false, bool showMeshDialog = false)
        {
            string lastFileName = OpenedFileName;
            _commands.ExecuteAllCommands(showImportDialog, showMeshDialog);
            OpenedFileName = lastFileName;
        }
        //
        public List<FileInOut.Output.Calculix.CalculixKeyword> GetCalculixModelKeywords()
        {
            if (_model == null)
            {
                MessageBoxes.ShowError("There is no model.");
                return null;
            }
            else return FileInOut.Output.CalculixFileWriter.GetModelKeywords(_model);
        }
        public OrderedDictionary<int[], FileInOut.Output.Calculix.CalculixUserKeyword> GetCalculixUserKeywords()
        {
            if (_model == null)
            {
                MessageBoxes.ShowError("There is no model.");
                return null;
            }
            else return _model.CalculixUserKeywords;
        }
        public void SetCalculixUserKeywords(OrderedDictionary<int[], FileInOut.Output.Calculix.CalculixUserKeyword> userKeywords)
        {
            _model.CalculixUserKeywords = userKeywords;
            _form.SetNumberOfModelUserKeywords(userKeywords.Count);
        }

        #endregion ################################################################################################################

        #region View menu   ########################################################################################################
        // Section view
        public void ApplySectionView(double[] point, double[] normal)
        {
            _sectionViews.SetCurrentSectionViewPlane(point, normal);
            _form.ApplySectionView(point, normal);
        }
        public void UpdateSectionView(double[] point, double[] normal)
        {
            _sectionViews.SetCurrentPointAndNormal(point, normal);
            _form.UpdateSectionView(point, normal);
        }
        public void RemoveSectionView()
        {
            _sectionViews.RemoveCurrentSectionView();
            _form.RemoveSectionView();
        }
        //
        public double[] GetViewPlaneNormal()
        {
            return _form.GetViewPlaneNormal();
        }
        // Exploded view
        public void PreviewExplodedView(ExplodedViewParameters parameters, bool animate,
                                        Dictionary<string, double[]> partOffsets = null,
                                        int timeMs = 500)
        {
            FeMesh mesh = DisplayedMesh;
            if (mesh == null) return;
            //
            string[] partNames = null;
            if (partOffsets == null)
            {
                partOffsets = mesh.GetExplodedViewOffsets((int)parameters.Method, parameters.Center,
                                                          (int)parameters.Direction,
                                                          parameters.ScaleFactor * parameters.Magnification,
                                                          partNames);
            }
            //
            _animating = animate;
            _form.PreviewExplodedView(partOffsets, animate, timeMs);
            _animating = false;
            //
            _form.SetExplodedViewStatus(true);
        }
        public void ApplyExplodedView(ExplodedViewParameters parameters, string[] partNames = null, bool update = true)
        {
            if (parameters.ScaleFactor == -1) return;
            //
            FeMesh mesh = DisplayedMesh;
            if (mesh == null) return;
            //
            _explodedViews.SetCurrentExplodedViewParameters(parameters);
            //
            mesh.RemoveExplodedView();
            //
            Dictionary<string, double[]> partOffsets;
            partOffsets = mesh.GetExplodedViewOffsets((int)parameters.Method, parameters.Center,
                                                      (int)parameters.Direction, parameters.ScaleFactor * parameters.Magnification,
                                                      partNames);
            mesh.ApplyExplodedView(partOffsets);
            //
            _form.SetExplodedViewStatus(true);
            //
            if (update) Redraw();
        }
        public void SuppressExplodedView(string[] partNames = null)
        {
            partNames = GetMeshablePartNames(partNames);
            //
            if (_model.Geometry != null && _explodedViews.IsGeometryExplodedViewActive())
            {
                _model.Geometry.SuppressExplodedView(partNames);
            }
            if (_model.Mesh != null && _explodedViews.IsModelExplodedViewActive())
            {
                _model.Mesh.SuppressExplodedView(partNames);
            }
            FeResults result;
            string[] resultNames = _allResults.GetResultNames();
            for (int i = 0; i < resultNames.Length; i++)
            {
                result = _allResults.GetResult(resultNames[i]);
                if (result != null && result.Mesh != null && _explodedViews.IsResultExplodedViewActive(resultNames[i]))
                {
                    result.Mesh.SuppressExplodedView(partNames);
                }
            }
        }
        public void ResumeExplodedViews(bool update)
        {
            bool updateG = false;
            bool updateM = false;
            bool updateR = false;
            bool updateCR = false;
            //
            if (_model.Geometry != null) updateG = _model.Geometry.ResumeExplodedView();
            if (_model.Mesh != null) updateM = _model.Mesh.ResumeExplodedView();
            //
            FeResults result;
            string[] resultNames = _allResults.GetResultNames();
            for (int i = 0; i < resultNames.Length; i++)
            {
                result = _allResults.GetResult(resultNames[i]);
                if (result != null && result.Mesh != null) updateR = result.Mesh.ResumeExplodedView();
                if (result == _allResults.CurrentResult && updateR) updateCR = true;
            }
            //
            if (update)
            {
                if ((_currentView == ViewGeometryModelResults.Geometry && updateG) ||
                    (_currentView == ViewGeometryModelResults.Model && updateM) ||
                    (_currentView == ViewGeometryModelResults.Results && updateCR))
                {
                    Redraw();
                }
            }
        }
        public void UpdateExplodedView(bool update, string[] partNames = null)
        {
            FeMesh mesh = DisplayedMesh;
            if (mesh == null) return;
            //
            ExplodedViewParameters parameters = _explodedViews.GetCurrentExplodedViewParameters();
            //
            UpdateExplodedView(mesh, parameters, update, partNames);
        }
        public void UpdateExplodedView(FeMesh mesh, ExplodedViewParameters parameters, bool update, string[] partNames = null)
        {
            if (mesh == null) return;
            //
            if (parameters.ScaleFactor != -1)
            {
                mesh.RemoveExplodedView();
                //
                Dictionary<string, double[]> partOffsets =
                    mesh.GetExplodedViewOffsets((int)parameters.Method, parameters.Center,
                                                (int)parameters.Direction,
                                                parameters.ScaleFactor * parameters.Magnification,
                                                partNames);
                mesh.ApplyExplodedView(partOffsets);
                //
                if (update) Redraw();
            }
        }
        private void UpdateCurrentResultExplodedView()
        {
            string currentResultName = _allResults.GetCurrentResultName();
            if (_explodedViews.IsResultExplodedViewActive(currentResultName))
            {
                ExplodedViewParameters parameters = _explodedViews.GetResultExplodedViewParameters(currentResultName);
                if (parameters != null)
                {
                    UpdateExplodedView(_allResults.CurrentResult.Mesh, parameters, false);
                }
            }
        }
        public void TurnExplodedViewOnOff(bool animate, int timeMs = 500)
        {
            // Exit
            if (_animating) return;
            // Suppress section view
            Octree.Plane sectionViewPlane = GetSectionViewPlane();
            if (sectionViewPlane != null) RemoveSectionView();
            // Suppress symbols
            string drawSymbolsForStep = GetDrawSymbolsForStep();
            DrawSymbolsForStep("None", false);
            // Suppress annotations
            _annotations.SuppressCurrentAnnotations();
            // Suppress undeformed results view
            UndeformedModelTypeEnum undeformedType = UndeformedModelTypeEnum.None;
            if (_currentView == ViewGeometryModelResults.Results)
            {
                undeformedType = Settings.Post.UndeformedModelType;
                if (undeformedType != UndeformedModelTypeEnum.None)
                {
                    SetUndeformedModelType(UndeformedModelTypeEnum.None);
                    DrawResults(false);
                }
            }
            // Deactivate exploded view
            if (IsExplodedViewActive())
            {
                ExplodedViewParameters parameters = _explodedViews.GetCurrentExplodedViewParameters().DeepClone();
                Dictionary<string, double[]> partOffsets = RemoveExplodedView(true);   // Highlight
                _form.Clear3DSelection();
                PreviewExplodedView(parameters, false, partOffsets, timeMs);
                parameters.ScaleFactor = 0;
                PreviewExplodedView(parameters, animate, null, timeMs);
                //
                _form.SetExplodedViewStatus(false);
            }
            // Activate exploded view
            else
            {
                FeMesh mesh = DisplayedMesh;
                if (mesh != null && mesh.Parts.Count > 1)
                {
                    _form.Clear3DSelection();
                    ExplodedViewParameters parameters = _explodedViews.GetCurrentExplodedViewParameters().DeepClone();
                    parameters.ScaleFactor = 0.5;
                    PreviewExplodedView(parameters, animate, null, timeMs);
                    ApplyExplodedView(parameters);  // Highlight
                }
            }
            // Resume symbols
            DrawSymbolsForStep(drawSymbolsForStep, false);  // Clears highlight
            // Resume annotations
            _annotations.ResumeCurrentAnnotations();
            // Resume section view
            if (sectionViewPlane != null) ApplySectionView(sectionViewPlane.Point.Coor, sectionViewPlane.Normal.Coor);
            // Resume undeformed results view
            if (undeformedType != UndeformedModelTypeEnum.None)
            {
                SetUndeformedModelType(undeformedType);
                DrawResults(false);
            }
            //
            UpdateHighlight();
            //if (_selection.Nodes.Count > 0) HighlightSelection();
            //else _form.UpdateHighlightFromTree();
        }
        public Dictionary<string, double[]> RemoveExplodedView(bool update, string[] partNames = null)
        {
            Dictionary<string, double[]> partOffsets = new Dictionary<string, double[]>();
            //
            FeMesh mesh = DisplayedMesh;
            if (mesh != null)
            {
                //
                if (partNames == null) partNames = mesh.Parts.Keys.ToArray();
                //
                _form.RemovePreviewedExplodedView(partNames);
                //
                if (_explodedViews.IsExplodedViewActive())
                {
                    _explodedViews.RemoveCurrentExplodedView();
                    //
                    partOffsets = mesh.RemoveExplodedView(partNames);
                    //
                    if (update) Redraw();
                }
            }
            //
            _form.SetExplodedViewStatus(false);
            //
            return partOffsets;
        }

        #endregion ################################################################################################################

        #region Geometry part menu   ###############################################################################################
        // COMMANDS ********************************************************************************
        public void ReplaceGeometryPartPropertiesCommand(string oldPartName, PartProperties newPartProperties)
        {
            Commands.CReplaceGeometryPartProperties comm = new Commands.CReplaceGeometryPartProperties(oldPartName, newPartProperties);
            _commands.AddAndExecute(comm);
        }
        // Sub menu Transform
        public void ScaleGeometryPartsCommand(string[] partNames, double[] scaleCenter, double[] scaleFactors, bool copy)
        {
            Commands.CScaleGeometryParts comm = new Commands.CScaleGeometryParts(partNames, scaleCenter, scaleFactors, copy);
            _commands.AddAndExecute(comm);
        }
        // End Transform
        public void HideGeometryPartsCommand(string[] partNames)
        {
            Commands.CHideGeometryParts comm = new Commands.CHideGeometryParts(partNames);
            _commands.AddAndExecute(comm);
        }
        public void ShowGeometryPartsCommand(string[] partNames)
        {
            Commands.CShowGeometryParts comm = new Commands.CShowGeometryParts(partNames);
            _commands.AddAndExecute(comm);
        }
        public void SetTransparencyForGeometryPartsCommand(string[] partNames, byte alpha)
        {
            Commands.CSetTransparencyForGeometryParts comm = new Commands.CSetTransparencyForGeometryParts(partNames, alpha);
            _commands.AddAndExecute(comm);
        }
        public void RemoveGeometryPartsCommand(string[] partNames)
        {
            Commands.CRemoveGeometryParts comm = new Commands.CRemoveGeometryParts(partNames);
            _commands.AddAndExecute(comm);
        }
        // End Part
        // CAD Part
        public void FlipFaceOrientationsCommand(GeometrySelection geometrySelection)
        {
            Commands.CFlipFaceOrientations comm = new Commands.CFlipFaceOrientations(geometrySelection);
            _commands.AddAndExecute(comm);
        }
        public void SplitAFaceUsingTwoPointsCommand(GeometrySelection surfaceSelection, GeometrySelection verticesSelection)
        {
            Commands.CSplitAFaceUsingTwoPoints comm = new Commands.CSplitAFaceUsingTwoPoints(surfaceSelection, verticesSelection);
            _commands.AddAndExecute(comm);
        }
        // End CAD Part
        // Stl Part
        public void FindEdgesByAngleForGeometryPartsCommand(string[] partNames, double edgeAngle)
        {
            Commands.CFindEdgesByAngleForGeometryParts comm =
                new Commands.CFindEdgesByAngleForGeometryParts(partNames, edgeAngle);
            _commands.AddAndExecute(comm);
        }
        public void FlipStlPartSurfacesNormalCommand(string[] partNames)
        {
            Commands.FlipStlPartSurfacesNormal comm = new Commands.FlipStlPartSurfacesNormal(partNames);
            _commands.AddAndExecute(comm);
        }
        // End Stl Part
        public void CreateAndImportCompoundPartCommand(string[] partNames)
        {
            Commands.CCreateAndImportCompoundPart comm = new Commands.CCreateAndImportCompoundPart(partNames);
            _commands.AddAndExecute(comm);
        }
        public void RegenerateCompoundPartsCommand(string[] compoundPartNames)
        {
            Commands.CRegenerateCompoundParts comm = new Commands.CRegenerateCompoundParts(compoundPartNames);
            _commands.AddAndExecute(comm);
        }
        public void SwapPartGeometriesCommand(string partName1, string partName2)
        {
            Commands.CSwapPartGeometries comm = new Commands.CSwapPartGeometries(partName1, partName2);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public string[] GetGeometryPartNames()
        {
            if (_model.Geometry != null) return _model.Geometry.Parts.Keys.ToArray();
            else return new string[0];
        }
        public bool IsPartCompoundSubPart(string partName)
        {
            return _model.IsPartCompoundSubPart(partName);
        }
        public GeometryPart GetGeometryPart(string partName)
        {
            BasePart part;
            _model.Geometry.Parts.TryGetValue(partName, out part);
            return (GeometryPart)part;
        }
        public GeometryPart[] GetGeometryParts(string[] partNames)
        {
            BasePart part;
            GeometryPart[] parts = new GeometryPart[partNames.Length];
            for (int i = 0; i < partNames.Length; i++)
            {
                _model.Geometry.Parts.TryGetValue(partNames[i], out part);
                parts[i] = (GeometryPart)part;
            }
            return parts;
        }
        public GeometryPart[] GetGeometryPartsForSelection(string[] partNames)
        {
            BasePart part;
            HashSet<GeometryPart> parts = new HashSet<GeometryPart>();  // must be hash set to remove the same compound parts
            //
            if (partNames.Length > 0)
            {
                // Collect all compound parts
                Dictionary<string, string> subPartNameCompoundPartName = new Dictionary<string, string>();
                foreach (var entry in _model.Geometry.Parts)
                {
                    if (entry.Value is CompoundGeometryPart cgp)
                    {
                        foreach (var subPartName in cgp.SubPartNames) subPartNameCompoundPartName.Add(subPartName, entry.Value.Name);
                    }
                }
                // Get a compound part if a sub part was selected
                string partName;
                for (int i = 0; i < partNames.Length; i++)
                {
                    if (!subPartNameCompoundPartName.TryGetValue(partNames[i], out partName)) partName = partNames[i];
                    //
                    if (_model.Geometry.Parts.TryGetValue(partName, out part)) parts.Add((GeometryPart)part);
                }
            }
            return parts.ToArray();
        }
        //
        public GeometryPart[] GetSubParts(string compoundPartName)
        {
            BasePart part;
            GeometryPart[] parts = null;
            _model.Geometry.Parts.TryGetValue(compoundPartName, out part);
            //
            if (part is CompoundGeometryPart cgp)
            {
                parts = new GeometryPart[cgp.SubPartNames.Length];
                for (int i = 0; i < cgp.SubPartNames.Length; i++)
                    parts[i] = (GeometryPart)_model.Geometry.Parts[cgp.SubPartNames[i]];
            }
            //
            return parts;
        }
        public GeometryPart[] GetGeometryParts()
        {
            if (_model == null || _model.Geometry == null) return null;
            //
            int i = 0;
            GeometryPart[] parts = new GeometryPart[_model.Geometry.Parts.Count];
            foreach (var entry in _model.Geometry.Parts) parts[i++] = (GeometryPart)entry.Value;
            return parts;
        }
        public GeometryPart[] GetCADGeometryParts()
        {
            if (_model.Geometry == null) return null;
            //
            List<GeometryPart> parts = new List<GeometryPart>();
            foreach (var entry in _model.Geometry.Parts)
            {
                if (entry.Value is GeometryPart gp && gp.IsCADPart) parts.Add(gp);
            }
            return parts.ToArray();
        }
        public GeometryPart[] GetNonCADGeometryParts()
        {
            if (_model.Geometry == null) return null;
            //
            List<GeometryPart> parts = new List<GeometryPart>();
            foreach (var entry in _model.Geometry.Parts)
            {
                if (entry.Value is GeometryPart gp && !gp.IsCADPart) parts.Add(gp);
            }
            return parts.ToArray();
        }
        public GeometryPart[] GetCompoundParts()
        {
            if (_model.Geometry == null) return null;
            //
            List<CompoundGeometryPart> parts = new List<CompoundGeometryPart>();
            foreach (var entry in _model.Geometry.Parts)
            {
                if (entry.Value is CompoundGeometryPart cgp) parts.Add(cgp);
            }
            return parts.ToArray();
        }
        public GeometryPart[] GetGeometryPartsWithoutSubParts()
        {
            if (_model == null || _model.Geometry == null) return null;
            //
            List<GeometryPart> subParts = new List<GeometryPart>();
            List<GeometryPart> allParts = new List<GeometryPart>();
            foreach (var entry in _model.Geometry.Parts)
            {
                allParts.Add((GeometryPart)entry.Value);
                //
                if (entry.Value is CompoundGeometryPart cgp)
                {
                    for (int i = 0; i < cgp.SubPartNames.Length; i++)
                        subParts.Add((GeometryPart)_model.Geometry.Parts[cgp.SubPartNames[i]]);
                }
            }
            return allParts.Except(subParts).ToArray();
        }
        public string[] GetMeshablePartNames(string[] partNames)
        {
            if (_model.Geometry != null) return _model.Geometry.GetMeshablePartNames(partNames);
            else return null;
        }

        //******************************************************************************************
        // Part
        public void ReplaceGeometryPartProperties(string oldPartName, PartProperties newPartProperties)
        {
            // Replace geometry part
            GeometryPart geomPart = GetGeometryPart(oldPartName);
            geomPart.SetProperties(newPartProperties);
            _model.Geometry.Parts.Replace(oldPartName, geomPart.Name, geomPart);
            // Rename compound sub-part names array
            if (oldPartName != newPartProperties.Name)
            {
                foreach (var entry in _model.Geometry.Parts)
                {
                    if (entry.Value is CompoundGeometryPart cgp)
                    {
                        for (int i = 0; i < cgp.SubPartNames.Length; i++)
                        {
                            if (cgp.SubPartNames[i] == oldPartName)
                            {
                                cgp.SubPartNames[i] = newPartProperties.Name;
                                break;
                            }
                        }
                        if (cgp.CreatedFromPartNames != null)
                        {
                            for (int i = 0; i < cgp.CreatedFromPartNames.Length; i++)
                            {
                                if (cgp.CreatedFromPartNames[i] == oldPartName)
                                {
                                    cgp.CreatedFromPartNames[i] = newPartProperties.Name;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            // Update
            if (!(geomPart is CompoundGeometryPart)) _form.UpdateActor(oldPartName, geomPart.Name, geomPart.Color);
            _form.UpdateTreeNode(ViewGeometryModelResults.Geometry, oldPartName, geomPart, null);
            AnnotateWithColorLegend();
            // Update the mesh part in pair with the geometry part properties
            if (_model.Mesh != null && _model.Mesh.Parts.ContainsKey(oldPartName))
            {
                string newPartName = geomPart.Name;
                MeshPart meshPart = GetModelPart(oldPartName);
                // Color
                meshPart.Color = geomPart.Color;
                // Rename
                if (oldPartName != geomPart.Name)
                {
                    meshPart.Name = newPartName;
                    _model.Mesh.Parts.Replace(oldPartName, meshPart.Name, meshPart);
                    // Update
                    _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldPartName, meshPart, null);
                }
            }
        }
        // Sub menu Transform
        public void ScaleGeometryParts(string[] partNames, double[] scaleCenter, double[] scaleFactors, bool copy)
        {
            if (IsExplodedViewActive()) throw new CaeException("The scaling can only be done when the exploded view is turned off.");
            // Scale
            GeometryPart geometryPart;
            string brepFileName;
            List<string> stlFileNames = new List<string>();
            // Scale CAD models
            foreach (var partName in partNames)
            {
                geometryPart = (GeometryPart)_model.Geometry.Parts[partName];
                if (geometryPart.IsCADPart)
                {
                    brepFileName = ScaleGeometryPart(geometryPart, scaleCenter, scaleFactors);
                    //
                    if (brepFileName != null)
                    {
                        if (copy) ImportBrepPartFile(brepFileName);
                        else ReplacePartGeometryFromFile(geometryPart, brepFileName, true);
                    }
                    else ClearAllSelection();
                }
                else stlFileNames.Add(partName);
            }
            // Scale stl models
            if (stlFileNames.Count > 0)
            {
                string[] scaledPartNames = _model.Geometry.ScaleParts(stlFileNames.ToArray(), scaleCenter, scaleFactors, copy,
                                                                      _model.GetReservedPartNames());
                if (copy)
                {
                    foreach (var scaledPartName in scaledPartNames)
                    {
                        _form.AddTreeNode(ViewGeometryModelResults.Geometry, _model.Geometry.Parts[scaledPartName], null);
                    }
                }
                //
                DrawGeometry(false);
            }
            //
            CheckAndUpdateModelValidity();
        }
        private string ScaleGeometryPart(GeometryPart part, double[] scaleCenter, double[] scaleFactors)
        {
            CalculixSettings settings = _settings.Calculix;
            if (settings.WorkDirectory == null || !Directory.Exists(settings.WorkDirectory))
            {
                MessageBoxes.ShowWorkDirectoryError();
                return null;
            }
            //
            string executable = Application.StartupPath + Globals.NetGenMesher;
            string inputBrepFileName = Path.Combine(settings.WorkDirectory, Globals.BrepFileName);
            string outputBrepFileName = Path.Combine(settings.WorkDirectory, Globals.BrepFileName);
            //
            if (File.Exists(inputBrepFileName)) File.Delete(inputBrepFileName);
            if (File.Exists(outputBrepFileName)) File.Delete(outputBrepFileName);
            //
            File.WriteAllText(inputBrepFileName, part.CADFileData);
            //
            string argument = "BREP_SCALE_GEOMETRY " +
                              "\"" + inputBrepFileName.ToUTF8() + "\" " +
                              "\"" + outputBrepFileName.ToUTF8() + "\" " +
                              scaleCenter[0] + " " + scaleCenter[1] + " " + scaleCenter[2] + " " +
                              scaleFactors[0] + " " + scaleFactors[1] + " " + scaleFactors[2];

            //
            _executableJob = new ExecutableJob(part.Name, executable, argument, settings.WorkDirectory);
            _executableJob.AppendOutput += executableJobMeshing_AppendOutput;
            _executableJob.Submit();
            // Job completed
            CheckAndUpdateModelValidity();
            //
            if (_executableJob.JobStatus == JobStatus.OK) return outputBrepFileName;
            else return null;
        }
        // End Transform
        public void CopyGeometryPartsToResults(string[] partNames)
        {
            HashSet<string> partNamesToCopy = new HashSet<string>(partNames);
            // Find all sub parts to copy except the compound parts
            foreach (var name in partNames)
            {
                if (_model.Geometry.Parts[name] is CompoundGeometryPart cgp)
                {
                    partNamesToCopy.Remove(cgp.Name);
                    partNamesToCopy.UnionWith(cgp.SubPartNames);
                }
            }
            //
            FeResults result = _allResults.CurrentResult;
            if (result != null && result.Mesh != null)
            {
                _model.Geometry.SuppressExplodedView();
                string[] addedPartNames = result.AddPartsFromMesh(_model.Geometry, partNamesToCopy.ToArray());
                _model.Geometry.ResumeExplodedView();
                // Update results exploded view
                UpdateCurrentResultExplodedView();
                //
                if (addedPartNames.Length > 0)
                {
                    _form.RegenerateTree();
                    CurrentView = ViewGeometryModelResults.Results;
                }
            }
            _modelChanged = true;
        }
        public void HideGeometryParts(string[] partNames)
        {
            bool hide;
            BasePart part;
            HashSet<string> partNamesToHide = new HashSet<string>(partNames);
            // Find all sub parts to hide
            foreach (var name in partNames)
            {
                part = _model.Geometry.Parts[name];
                if (part is CompoundGeometryPart cgp) partNamesToHide.UnionWith(cgp.SubPartNames);
            }
            // Hide still visible compound parts with all hidden component parts
            foreach (var entry in _model.Geometry.Parts)
            {
                if (entry.Value is CompoundGeometryPart cgp && cgp.Visible == true)
                {
                    hide = true;
                    for (int i = 0; i < cgp.SubPartNames.Length; i++)
                    {
                        // If sub part is visible and is not about to be hidden
                        if (_model.Geometry.Parts[cgp.SubPartNames[i]].Visible && !partNamesToHide.Contains(cgp.SubPartNames[i]))
                        {
                            hide = false;
                            break;
                        }
                    }
                    if (hide) partNamesToHide.Add(cgp.Name);
                }
            }
            // Perform hide
            foreach (var name in partNamesToHide)
            {
                part = _model.Geometry.Parts[name];
                part.Visible = false;
                _form.UpdateTreeNode(ViewGeometryModelResults.Geometry, name, _model.Geometry.Parts[name], null, false);
            }
            _form.HideActors(partNamesToHide.ToArray(), false);
            // Update
            AnnotateWithColorLegend();
            // Annotations
            _annotations.DrawAnnotations();
        }
        public void ShowGeometryParts(string[] partNames)
        {
            bool show;
            BasePart part;
            HashSet<string> partNamesToShow = new HashSet<string>(partNames);
            // Find all sub parts to show
            foreach (var name in partNames)
            {
                part = _model.Geometry.Parts[name];
                if (part is CompoundGeometryPart cgp) partNamesToShow.UnionWith(cgp.SubPartNames);
            }
            // Show still hidden compound parts with at leas one shown component part
            foreach (var entry in _model.Geometry.Parts)
            {
                if (entry.Value is CompoundGeometryPart cgp && cgp.Visible == false)
                {
                    show = false;
                    for (int i = 0; i < cgp.SubPartNames.Length; i++)
                    {
                        // If sub part is visible or is about to be shown
                        if (_model.Geometry.Parts[cgp.SubPartNames[i]].Visible || partNamesToShow.Contains(cgp.SubPartNames[i]))
                        {
                            show = true;
                            break;
                        }
                    }
                    if (show) partNamesToShow.Add(cgp.Name);
                }
            }
            // Perform show
            foreach (var name in partNamesToShow)
            {
                part = _model.Geometry.Parts[name];
                part.Visible = true;
                _form.UpdateTreeNode(ViewGeometryModelResults.Geometry, name, _model.Geometry.Parts[name], null, false);
            }
            _form.ShowActors(partNamesToShow.ToArray(), false);
            // Update
            AnnotateWithColorLegend();
            // Annotations
            _annotations.DrawAnnotations();
        }
        public void SetTransparencyForGeometryParts(string[] partNames, byte alpha)
        {
            BasePart part;
            HashSet<string> partNamesToSet = new HashSet<string>(partNames);
            // Find all sub parts to set except the compound parts
            foreach (var name in partNames)
            {
                part = _model.Geometry.Parts[name];
                if (part is CompoundGeometryPart cgp)
                {
                    partNamesToSet.Remove(cgp.Name);
                    partNamesToSet.UnionWith(cgp.SubPartNames);
                }
            }
            //
            foreach (var name in partNamesToSet)
            {
                part = _model.Geometry.Parts[name];
                part.Color = Color.FromArgb(alpha, part.Color);
                _form.UpdateActor(name, name, part.Color);
                _form.UpdateTreeNode(ViewGeometryModelResults.Geometry, name, part, null, false);
            }
            // Check and set compound color for the transparency icon
            bool transparent;
            foreach (var entry in _model.Geometry.Parts)
            {
                if (entry.Value is CompoundGeometryPart cgp)
                {
                    transparent = false;
                    foreach (var subPartName in cgp.SubPartNames)
                    {
                        part = _model.Geometry.Parts[subPartName];
                        if (part.Color.A != 255)
                        {
                            transparent = true;
                            break;
                        }
                    }
                    if (transparent == (entry.Value.Color.A == 255))
                    {
                        entry.Value.Color = Color.FromArgb(alpha, entry.Value.Color);
                        _form.UpdateTreeNode(ViewGeometryModelResults.Geometry, entry.Key, entry.Value, null, false);
                    }
                }
            }
        }
        public void RemoveGeometryParts(string[] partNames, bool keepGeometrySelections)
        {
            BasePart part;
            HashSet<string> partNamesToRemove = new HashSet<string>();
            HashSet<string> compoundPartNamesToRemove = new HashSet<string>();
            // Find all sub parts to remove
            foreach (var name in partNames)
            {
                part = _model.Geometry.Parts[name];
                if (part is CompoundGeometryPart cgp)
                {
                    compoundPartNamesToRemove.Add(part.Name);
                    partNamesToRemove.UnionWith(cgp.SubPartNames);
                }
                else partNamesToRemove.Add(part.Name);
            }
            // Use a list fo remove the compound parts as last
            List<string> orderedPartsToRemove = new List<string>(partNamesToRemove);
            orderedPartsToRemove.AddRange(compoundPartNamesToRemove);
            //
            ViewGeometryModelResults view = ViewGeometryModelResults.Geometry;
            // Remove annotations
            _annotations.RemoveCurrentArrowAnnotationsByParts(partNamesToRemove.ToArray(), view);
            //
            string[] removedParts;
            _model.Geometry.RemoveParts(orderedPartsToRemove.ToArray(), out removedParts, keepGeometrySelections);
            //
            foreach (var name in removedParts) _form.RemoveTreeNode<GeometryPart>(view, name, null);
            //
            CheckAndUpdateModelValidity();
            //
            DrawGeometry(false);
        }
        // End Part
        // CAD Part
        // Flip face
        public void FlipFaceOrientations(GeometrySelection geometrySelection)
        {
            if (geometrySelection.CreationData != null)
            {
                // In order for the Regenerate history to work perform the selection
                _selection = geometrySelection.CreationData.DeepClone();
                geometrySelection.GeometryIds = GetSelectionIds();
                _selection.Clear();
            }
            else throw new NotSupportedException("The geometry selection does not contain any selection data.");
            // Flip
            int[] itemTypePartIds;
            GeometryType geomType;
            HashSet<int> faceIds;
            Dictionary<int, HashSet<int>> partIdFaceIds = new Dictionary<int, HashSet<int>>();
            //
            int countSolidFaces = 0;
            foreach (int id in geometrySelection.GeometryIds)
            {
                itemTypePartIds = FeMesh.GetItemTypePartIdsFromGeometryId(id);
                geomType = (GeometryType)itemTypePartIds[1];
                // Surface
                if (geomType == GeometryType.SolidSurface) countSolidFaces++;
                else if (geomType.IsShellSurface())
                {
                    if (partIdFaceIds.TryGetValue(itemTypePartIds[2], out faceIds)) faceIds.Add(itemTypePartIds[0]);
                    else partIdFaceIds.Add(itemTypePartIds[2], new HashSet<int>() { itemTypePartIds[0] });
                }
            }
            //
            if (partIdFaceIds.Keys.Count > 0)
            {
                BasePart part;
                string brepFileName;
                int numOfShellParts = 0;
                foreach (var entry in partIdFaceIds)
                {
                    part = _model.Geometry.GetPartFromId(entry.Key);
                    if (part != null && part is GeometryPart gp && gp.IsCADPart && part.PartType == PartType.Shell)
                    {
                        brepFileName = FlipPartFaceOrientations(gp, entry.Value.ToArray());
                        //
                        if (brepFileName != null) ReplacePartGeometryFromFile(gp, brepFileName, true);
                        else ClearAllSelection();
                        //
                        numOfShellParts++;
                    }
                }
                //
                string warning = "Face orientations on solid parts or non-CAD parts cannot be flipped.";
                if (numOfShellParts <= 0)
                    MessageBoxes.ShowWarning(warning);
                else if (countSolidFaces > 0)
                    MessageBoxes.ShowWarning(warning + Environment.NewLine +
                                             "Only face orientations on CAD shell parts were flipped.");
            }
            //
            CheckAndUpdateModelValidity();
        }
        private string FlipPartFaceOrientations(GeometryPart part, int[] faceIds)
        {
            CalculixSettings settings = _settings.Calculix;
            if (settings.WorkDirectory == null || !Directory.Exists(settings.WorkDirectory))
            {
                MessageBoxes.ShowWorkDirectoryError();
                return null;
            }
            //
            string executable = Application.StartupPath + Globals.NetGenMesher;
            string inputBrepFileName = Path.Combine(settings.WorkDirectory, Globals.BrepFileName);
            string outputBrepFileName = Path.Combine(settings.WorkDirectory, Globals.BrepFileName);
            string faceIdsArgument = "";
            foreach (var id in faceIds) faceIdsArgument += (id + 1) + " ";  // add 1 for the geometry counting
            //
            if (File.Exists(inputBrepFileName)) File.Delete(inputBrepFileName);
            if (File.Exists(outputBrepFileName)) File.Delete(outputBrepFileName);
            //
            File.WriteAllText(inputBrepFileName, part.CADFileData);
            //
            string argument = "BREP_REVERSE_FACES " +
                              "\"" + inputBrepFileName.ToUTF8() + "\" " +
                              "\"" + outputBrepFileName.ToUTF8() + "\" " +
                              faceIdsArgument;
            //
            _executableJob = new ExecutableJob(part.Name, executable, argument, settings.WorkDirectory);
            _executableJob.AppendOutput += executableJobMeshing_AppendOutput;
            _executableJob.Submit();
            // Job completed
            if (_executableJob.JobStatus == JobStatus.OK) return outputBrepFileName;
            else return null;
        }
        private bool ReplacePartGeometryFromFile(GeometryPart part, string fileName, bool keepGeometrySelections)
        {
            int count = 0;
            string[] importedFileNames = null;
            string extension = Path.GetExtension(fileName);
            //
            while (importedFileNames == null && count < 5)  // Check for timeout
            {
                if (extension == ".brep") importedFileNames = ImportBrepPartFile(fileName, false);
                else if (extension == ".stl") importedFileNames = _model.ImportGeometryFromStlFile(fileName);
                else throw new NotSupportedException();
                count++;
            }
            if (importedFileNames == null)
            {
                throw new CaeException("Importing geometry file during the replace of the geometry part failed.");
            }
            else if (importedFileNames.Length == 1)
            {
                //_form.ScreenUpdating = false;
                // Add the imported part to the model tree
                UpdateAfterImport(extension);
                // Copy old part properties to the new part
                GeometryPart newPart = (GeometryPart)_model.Geometry.Parts[importedFileNames[0]];
                newPart.Name = part.Name;
                newPart.Color = part.Color;
                // Switch old and new part in the dictionary
                _model.Geometry.Parts.Replace(part.Name, newPart.Name, newPart);
                part.Name = importedFileNames[0];
                _model.Geometry.Parts.Replace(importedFileNames[0], part.Name, part);
                // Remove old part
                RemoveGeometryParts(new string[] { part.Name }, keepGeometrySelections);
                _model.Geometry.ChangePartId(newPart.Name, part.PartId);
                //
                UpdateMeshSetupItems();
                //
                UpdateAfterImport(extension);
                //
                CheckAndUpdateModelValidity();
                //_form.ScreenUpdating = true;
            }
            else
            {
                UpdateAfterImport(extension);
                ClearAllSelection();
            }
            return true;
        }
        // Split a face using two points
        public void SplitAFaceUsingTwoPoints(GeometrySelection surfaceSelection, GeometrySelection verticesSelection)
        {
            if (surfaceSelection.CreationData != null && verticesSelection.CreationData != null)
            {
                // In order for the Regenerate history to work perform the selection
                _selection = surfaceSelection.CreationData.DeepClone();
                surfaceSelection.GeometryIds = GetSelectionIds();
                _selection.Clear();
                //
                _selection = verticesSelection.CreationData.DeepClone();
                verticesSelection.GeometryIds = GetSelectionIds();
                _selection.Clear();
            }
            else throw new NotSupportedException("The geometry selection does not contain any selection data.");
            //
            if (surfaceSelection.GeometryIds.Length != 1 || verticesSelection.GeometryIds.Length != 2)
                throw new CaeException("The selection does not contain 1 face and 2 vertices.");
            // Split
            int faceId;
            int node1Id;
            int node2Id;
            FeNode node1;
            FeNode node2;
            BasePart part1;
            BasePart part2;
            BasePart facePart;
            int[] itemTypePartIds;
            double[] offset;
            itemTypePartIds = FeMesh.GetItemTypePartIdsFromGeometryId(surfaceSelection.GeometryIds[0]);
            facePart = _model.Geometry.GetPartFromId(itemTypePartIds[2]);
            faceId = itemTypePartIds[0];
            //
            itemTypePartIds = FeMesh.GetItemTypePartIdsFromGeometryId(verticesSelection.GeometryIds[0]);
            part1 = _model.Geometry.GetPartFromId(itemTypePartIds[2]);
            node1Id = part1.Visualization.VertexNodeIds[itemTypePartIds[0]];
            //
            itemTypePartIds = FeMesh.GetItemTypePartIdsFromGeometryId(verticesSelection.GeometryIds[1]);
            part2 = _model.Geometry.GetPartFromId(itemTypePartIds[2]);
            node2Id = part2.Visualization.VertexNodeIds[itemTypePartIds[0]];
            //
            node1 = _model.Geometry.Nodes[node1Id].DeepCopy();
            node2 = _model.Geometry.Nodes[node2Id];
            if (IsExplodedViewActive())
            {
                offset = part1.Offset;
                node1.X -= offset[0];
                node1.Y -= offset[1];
                node1.Z -= offset[2];
                //
                offset = part2.Offset;
                node2.X -= offset[0];
                node2.Y -= offset[1];
                node2.Z -= offset[2];
            }
            //
            if (facePart != null && facePart is GeometryPart gp && gp.IsCADPart)
            {
                string brepFileName = SplitAFaceUsingTwoPoints(gp, faceId, node1, node2);
                //
                if (brepFileName != null) ReplacePartGeometryFromFile(gp, brepFileName, true);
                else ClearAllSelection();
            }
            else MessageBoxes.ShowWarning("Faces on non-CAD parts cannot be split.");
            //
            CheckAndUpdateModelValidity();
        }
        private string SplitAFaceUsingTwoPoints(GeometryPart part, int faceId, FeNode node1, FeNode node2)
        {
            CalculixSettings settings = _settings.Calculix;
            if (settings.WorkDirectory == null || !Directory.Exists(settings.WorkDirectory))
            {
                MessageBoxes.ShowError("The work directory does not exist.");
                return null;
            }
            //
            string executable = Application.StartupPath + Globals.NetGenMesher;
            string inputBrepFileName = Path.Combine(settings.WorkDirectory, Globals.BrepFileName);
            string outputBrepFileName = Path.Combine(settings.WorkDirectory, Globals.BrepFileName);
            //
            if (File.Exists(inputBrepFileName)) File.Delete(inputBrepFileName);
            if (File.Exists(outputBrepFileName)) File.Delete(outputBrepFileName);
            //
            File.WriteAllText(inputBrepFileName, part.CADFileData);
            //
            string argument = "BREP_SPLIT_A_FACE_USING_TWO_POINTS " +
                              "\"" + inputBrepFileName.ToUTF8() + "\" " +
                              "\"" + outputBrepFileName.ToUTF8() + "\" " +
                              (faceId + 1) + " " +
                              node1.X + " " + node1.Y + " " + node1.Z + " " +
                              node2.X + " " + node2.Y + " " + node2.Z;
            //
            _executableJob = new ExecutableJob(part.Name, executable, argument, settings.WorkDirectory);
            _executableJob.AppendOutput += executableJobMeshing_AppendOutput;
            _executableJob.Submit();
            // Job completed
            if (_executableJob.JobStatus == JobStatus.OK) return outputBrepFileName;
            else return null;
        }
        // End CAD Part
        // Stl Part             
        public void FindEdgesByAngleForGeometryParts(string[] partNames, double edgeAngle)
        {
            GeometryPart geometryPart;
            foreach (var partName in partNames)
            {
                geometryPart = (GeometryPart)_model.Geometry.Parts[partName];
                _model.Geometry.ExtractShellPartVisualization(geometryPart, geometryPart.IsCADPart, edgeAngle);
                // Update
                _form.UpdateTreeNode(ViewGeometryModelResults.Geometry, geometryPart.Name, geometryPart, null);
            }
            //
            CheckAndUpdateModelValidity();
            // Draw
            DrawGeometry(false);
        }
        public void FlipStlPartSurfacesNormal(string[] partNames)
        {
            GeometryPart part;
            LinearTriangleElement element;
            foreach (var partName in partNames)
            {
                part = (GeometryPart)_model.Geometry.Parts[partName];
                if (!part.IsCADPart && part.ElementTypes.Length == 1 &&
                    part.ElementTypes[0] == typeof(LinearTriangleElement))
                {
                    foreach (var elementId in part.Labels)
                    {
                        element = (LinearTriangleElement)_model.Geometry.Elements[elementId];
                        element.FlipNormal();
                    }
                    part.Visualization.FlipTriangleNormals();
                }
            }
            //
            CheckAndUpdateModelValidity();
            //
            DrawGeometry(false);
        }
        public void CropGeometryPartWithCylinder(string partName)
        {
            GeometryPart part = (GeometryPart)_model.Geometry.Parts[partName];
            if (part != null)
            {
                CalculixSettings settings = _settings.Calculix;
                string fileName = Path.Combine(settings.WorkDirectory, Globals.StlFileName);
                //
                _form.CropPartWithCylinder(partName, 10, fileName);
                //
                ReplacePartGeometryFromFile(part, fileName, true);
            }
        }
        public void CropGeometryPartWithCube(string partName)
        {
            GeometryPart part = (GeometryPart)_model.Geometry.Parts[partName];
            if (part != null)
            {
                CalculixSettings settings = _settings.Calculix;
                string fileName = Path.Combine(settings.WorkDirectory, Globals.StlFileName);
                //
                _form.CropPartWithCube(partName, 300, fileName);
                //
                ReplacePartGeometryFromFile(part, fileName, true);
            }
        }
        public void SmoothGeometryPart(string partName)
        {
            GeometryPart part = (GeometryPart)_model.Geometry.Parts[partName];
            if (part != null)
            {
                CalculixSettings settings = _settings.Calculix;
                string fileName = Path.Combine(settings.WorkDirectory, Globals.StlFileName);
                //
                _form.SmoothPart(partName, 0, fileName);
                //
                ImportFile(fileName, false);
                //ReplacePartGeometryFromFile(part, fileName);
            }
        }
        // End Stl Part         
        public void SwapGeometryPartsPosition(string partName1, string partName2, out GeometryPart part1, out GeometryPart part2)
        {
            part1 = (GeometryPart)_model.Geometry.Parts[partName1];
            part2 = (GeometryPart)_model.Geometry.Parts[partName2];
            // Swap in dictionary
            string tmpName = _model.Geometry.Parts.GetNextNumberedKey("tmpName");
            BasePart part = new BasePart(tmpName, -1, null, null, null);
            _model.Geometry.Parts.Replace(part1.Name, part.Name, part);
            _model.Geometry.Parts.Replace(part2.Name, part1.Name, part1);
            _model.Geometry.Parts.Replace(part.Name, part2.Name, part2);
            // Swap in tree
            _form.SwapTreeNode(_currentView, partName1, part1, partName2, part2, null);
        }
        public void SwapGeometryPartGeometries(string partName1, string partName2)
        {
            GeometryPart part1;
            GeometryPart part2;
            string tmpName = _model.Geometry.Parts.GetNextNumberedKey("tmpName");
            SwapGeometryPartsPosition(partName1, partName2, out part1, out part2);
            // Swap Ids
            int partId = part1.PartId;
            _model.Geometry.ChangePartId(partName1, part2.PartId);
            _model.Geometry.ChangePartId(partName2, partId);
            // Swap colors
            (part2.Color, part1.Color) = (part1.Color, part2.Color);
            // Swap visibilities
            (part2.Visible, part1.Visible) = (part1.Visible, part2.Visible);
            // Swap names
            part1.Name = partName2;
            part2.Name = partName1;
            _model.Geometry.Parts.Replace(partName1, tmpName, part1);
            _model.Geometry.Parts.Replace(partName2, part2.Name, part2);
            _model.Geometry.Parts.Replace(tmpName, part1.Name, part1);
            // Update colors
            _form.UpdateActor(part1.Name, tmpName, Color.Gray);
            _form.UpdateActor(part2.Name, part1.Name, part1.Color);
            _form.UpdateActor(tmpName, part2.Name, part2.Color);
            // Update visibilities
            if (part1.Visible) _form.ShowActors(new string[] { part1.Name }, false);
            else _form.HideActors(new string[] { part1.Name }, false);
            if (part2.Visible) _form.ShowActors(new string[] { part2.Name }, false);
            else _form.HideActors(new string[] { part2.Name }, false);
            // Update geometry tree
            BasePart part = new BasePart(tmpName, -1, null, null, null);
            _form.UpdateTreeNode(ViewGeometryModelResults.Geometry, partName1, part, null, false);
            _form.UpdateTreeNode(ViewGeometryModelResults.Geometry, partName2, part2, null, false);
            _form.UpdateTreeNode(ViewGeometryModelResults.Geometry, tmpName, part1, null, false);
            //
            UpdateMeshSetupItems(false);
            //
            CheckAndUpdateModelValidity();
            //
            UpdateHighlight();
        }
        public void RegenerateCompoundParts(string[] compoundPartNames)
        {
            if (_model == null || _model.Geometry == null) return;
            //
            bool allPartsExist;
            string importedCompoundPartName;
            string[] importedPartNames;
            PartProperties properties;
            CompoundGeometryPart compoundPart;
            CompoundGeometryPart importedCompoundPart;
            GeometryPart[] subParts;
            GeometryPart[] importedSubParts;
            //
            foreach (var compoundPartName in compoundPartNames)
            {
                compoundPart = (CompoundGeometryPart)_model.Geometry.Parts[compoundPartName];
                if (compoundPart.CreatedFromPartNames != null && compoundPart.CreatedFromPartNames.Length > 1)
                {
                    allPartsExist = true;
                    foreach (var createdFromPartName in compoundPart.CreatedFromPartNames)
                    {
                        if (!_model.Geometry.Parts.ContainsKey(createdFromPartName))
                        {
                            allPartsExist = false;
                            break;
                        }
                    }
                    //
                    if (allPartsExist)
                    {
                        // Create compound part
                        CreateAndImportCompoundPart(compoundPart.CreatedFromPartNames, out importedCompoundPartName,
                                                    out importedPartNames);
                        // Copy sub parts
                        subParts = GetSubParts(compoundPartName);
                        // Get new parts
                        importedCompoundPart = (CompoundGeometryPart)_model.Geometry.Parts[importedCompoundPartName];
                        importedSubParts = GetSubParts(importedCompoundPartName);
                        if (subParts.Length != importedSubParts.Length)
                            throw new CaeException("The regenerated compound part has a different number of sub-parts " +
                                                   "than before the regeneration.");

                        // Swap parts in the tree
                        SwapGeometryPartsPosition(compoundPartName, importedCompoundPartName,
                                                  out GeometryPart p1, out GeometryPart p2);
                        // Delete old compound part
                        RemoveGeometryParts(new string[] { compoundPartName }, true);
                        // Compound                                             
                        // Update visibility
                        importedCompoundPart.Visible = compoundPart.Visible;
                        if (importedCompoundPart.Visible) _form.ShowActors(new string[] { importedCompoundPartName }, false);
                        else _form.HideActors(new string[] { importedCompoundPartName }, false);
                        // Update part id
                        _model.Geometry.ChangePartId(importedCompoundPartName, compoundPart.PartId);
                        // Update properties
                        properties = importedCompoundPart.GetProperties();
                        properties.Name = compoundPartName;
                        properties.Color = compoundPart.Color;
                        ReplaceGeometryPartProperties(importedCompoundPartName, properties);
                        //
                        // Sub parts                                            
                        for (int i = 0; i < importedSubParts.Length; i++)
                        {
                            // Update visibility
                            importedSubParts[i].Visible = subParts[i].Visible;
                            if (importedSubParts[i].Visible) _form.ShowActors(new string[] { importedSubParts[i].Name }, false);
                            else _form.HideActors(new string[] { importedSubParts[i].Name }, false);
                            // Update part id
                            _model.Geometry.ChangePartId(importedSubParts[i].Name, subParts[i].PartId);
                            // Update properties
                            properties = importedSubParts[i].GetProperties();
                            properties.Name = subParts[i].Name;
                            properties.Color = subParts[i].Color;
                            ReplaceGeometryPartProperties(importedSubParts[i].Name, properties);
                        }
                    }
                }
            }
            //
            UpdateMeshSetupItems(false);
            //
            CheckAndUpdateModelValidity();
            //
            _form.SelectBaseParts(compoundPartNames);
        }
        // Analyze geometry
        public double GetShortestEdgeLen(string[] partNames)
        {
            return DisplayedMesh.GetShortestEdgeLen(partNames);
        }
        public void ShowShortEdges(double minEdgeLen, string[] partNames)
        {
            double[][][] edgeNodeCoor = DisplayedMesh.GetShortEdges(minEdgeLen, partNames);
            HighlightConnectedEdges(edgeNodeCoor);
        }
        public double GetClosestUnConnectedEdgesDistance(string[] partNames)
        {
            return DisplayedMesh.GetClosestUnConnectedEdgesDistance(partNames);
        }
        public void ShowCloseUnConnectedEdges(double minDistance, string[] partNames)
        {
            double[][][] edgeNodeCoor = DisplayedMesh.ShowCloseUnConnectedEdges(minDistance, partNames);
            HighlightConnectedEdges(edgeNodeCoor);
        }
        public double GetSmallestFace(string[] partNames)
        {
            return DisplayedMesh.GetSmallestFace(partNames);
        }
        public void ShowSmallFaces(double minFaceArea, string[] partNames)
        {
            //ClearAllSelection();
            FeMesh mesh = DisplayedMesh;
            int[][] cells = mesh.GetSmallestFaces(minFaceArea, partNames);
            HighlightSurface(cells, null, false);
        }
        public void ShowVerticesWithLargeAngle(string[] partNames, double angleDeg)
        {
            double[][] smallAngleVertexCoor = DisplayedMesh.GetVertexCoorWithLargeAngle(partNames, angleDeg);
            HighlightNodes(smallAngleVertexCoor);
        }

        #endregion #################################################################################################################

        #region Mesh   #############################################################################################################
        // COMMANDS ********************************************************************************
        public void CreateMeshCommand(string partName)
        {
            Commands.CCreateMesh comm = new Commands.CCreateMesh(partName);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public MeshingParameters GetPartMeshingParameters(string partName,
                                                          OrderedDictionary<string, MeshSetupItem> meshSetupItems = null)
        {
            HashSet<int> selectedPartIds;
            string[] meshAblePartNames = GetMeshablePartNames(new string[] { partName });
            int[] meshAblePartIds = _model.Geometry.GetPartIdsFomPartNames(meshAblePartNames);
            MeshingParameters meshingParameters = GetPartDefaultMeshingParameters(partName);
            //
            if (meshSetupItems == null) meshSetupItems = _model.Geometry.MeshSetupItems;
            //
            foreach (var entry in meshSetupItems)
            {
                if (entry.Value is MeshingParameters mp && mp.Active && mp.Valid)
                {
                    if (mp.CreationIds != null && mp.CreationIds.Length > 0)
                    {
                        selectedPartIds = new HashSet<int>(FeMesh.GetPartIdsFromGeometryIds(mp.CreationIds));
                        //
                        if (selectedPartIds.Intersect(meshAblePartIds).Count() == meshAblePartIds.Length)
                            meshingParameters = mp.DeepCopy();
                    }
                }
            }
            // Allow a quad dominated mesh only for shells
            if (_model.Geometry.Parts[partName].PartType != PartType.Shell) meshingParameters.QuadDominated = false;
            //
            return meshingParameters;
        }
        public MeshingParameters GetDefaultMeshingParameters(string meshingParametersName)
        {
            MeshingParameters meshingParameters = _settings.Meshing.MeshingParameters.DeepClone();
            meshingParameters.Name = meshingParametersName;
            return meshingParameters;
        }
        public MeshingParameters GetPartDefaultMeshingParameters(string partName)
        {
            BasePart part = GetGeometryPart(partName);
            if (part == null) part = GetModelPart(partName);
            if (part == null) return null;
            //
            if (!ExecutableJobIdle) throw new Exception("The meshing is already in progress.");
            //
            MeshingParameters defaultMeshingParameters = GetDefaultMeshingParameters("Default");
            double factorMax = defaultMeshingParameters.FactorMax;
            double factorMin = defaultMeshingParameters.FactorMin;
            double factorHausdorff = defaultMeshingParameters.FactorHausdorff;
            double diagonal = part.BoundingBox.GetDiagonal();
            //
            if (part.PartType == PartType.Shell && part is GeometryPart gp && !gp.IsCADPart)
                defaultMeshingParameters.UseMmg = true;
            else if (part.PartType == PartType.Shell && part is MeshPart)   // for remeshing
                defaultMeshingParameters.UseMmg = true;
            //
            defaultMeshingParameters.MaxH = CaeGlobals.Tools.RoundToSignificantDigits(diagonal * factorMax, 2);
            defaultMeshingParameters.MinH = CaeGlobals.Tools.RoundToSignificantDigits(diagonal * factorMin, 2);
            defaultMeshingParameters.Hausdorff = CaeGlobals.Tools.RoundToSignificantDigits(diagonal * factorHausdorff, 2);
            //
            return defaultMeshingParameters;
        }
        public MeshingParameters GetPartDefaultMeshingParameters(string[] partNames, bool onlyOneMeshType = true)
        {
            double sumMax = 0;
            double sumMin = 0;
            double sumHausdorff = 0;
            MeshingParameters defaultMeshingParameters = null;
            HashSet<bool> useMmg = new HashSet<bool>();
            foreach (var partName in partNames)
            {
                // Default parameters
                defaultMeshingParameters = GetPartDefaultMeshingParameters(partName);
                // If part is not found return null
                if (defaultMeshingParameters != null)
                {
                    // Check for different types of meshes
                    useMmg.Add(defaultMeshingParameters.UseMmg);
                    //
                    sumMax += defaultMeshingParameters.MaxH;
                    sumMin += defaultMeshingParameters.MinH;
                    sumHausdorff += defaultMeshingParameters.Hausdorff;
                }
                // Part was not found
                else return null;
            }
            //
            defaultMeshingParameters.MaxH = CaeGlobals.Tools.RoundToSignificantDigits(sumMax / partNames.Length, 2);
            defaultMeshingParameters.MinH = CaeGlobals.Tools.RoundToSignificantDigits(sumMin / partNames.Length, 2);
            defaultMeshingParameters.Hausdorff = CaeGlobals.Tools.RoundToSignificantDigits(sumHausdorff / partNames.Length, 2);
            // All parts must be of either netgen type or mmg type
            if (onlyOneMeshType)
            {
                if (useMmg.Count() == 1) return defaultMeshingParameters;
                else return null;
            }
            else return defaultMeshingParameters;
        }
        //
        public void GetMeshItemSizes(string partName, ref GmshData gmshData,
                                     OrderedDictionary<string, MeshSetupItem> meshSetupItems = null)
        {
            HashSet<int> selectedPartIds;
            string[] meshAblePartNames = GetMeshablePartNames(new string[] { partName });
            int[] meshAblePartIds = _model.Geometry.GetPartIdsFomPartNames(meshAblePartNames);
            List<FeMeshRefinement> meshRefinements = new List<FeMeshRefinement>();
            //
            if (meshSetupItems == null) meshSetupItems = _model.Geometry.MeshSetupItems;
            //
            foreach (var entry in meshSetupItems)
            {
                if (entry.Value is FeMeshRefinement mr && mr.Active && mr.Valid)
                {
                    if (mr.CreationIds != null && mr.CreationIds.Length > 0)
                    {
                        selectedPartIds = new HashSet<int>(FeMesh.GetPartIdsFromGeometryIds(mr.CreationIds));
                        //
                        if (selectedPartIds.Intersect(meshAblePartIds).Count() > 0) meshRefinements.Add(mr.DeepCopy());
                    }
                }
            }
            //
            int[] itemTypePartId;
            int itemId;
            GeometryType geometryType;
            int partId;
            BasePart part;
            int edgeId;
            double length;
            int numElements;
            int[] vertexNodeIds;
            HashSet<int> nodeIds = new HashSet<int>();
            gmshData.VertexNodeIdMeshSize = new Dictionary<int, double>();
            gmshData.EdgeIdNumElements = new Dictionary<int, int>();
            foreach (var meshRefinement in meshRefinements)
            {
                nodeIds.Clear();
                //
                foreach (int geometryId in meshRefinement.CreationIds)
                {
                    itemTypePartId = FeMesh.GetItemTypePartIdsFromGeometryId(geometryId);
                    itemId = itemTypePartId[0];
                    geometryType = (GeometryType)itemTypePartId[1];
                    partId = itemTypePartId[2];
                    part = _model.Geometry.GetPartFromId(partId);
                    //
                    if (geometryType == GeometryType.Vertex)
                    {
                        nodeIds.Add(part.Visualization.VertexNodeIds[itemId]);
                    }
                    else if (geometryType.IsEdge())
                    {
                        length = part.Visualization.EdgeLengths[itemId];
                        numElements = (int)Math.Round(length / meshRefinement.MeshSize, 0, MidpointRounding.AwayFromZero);
                        if (numElements < 1) numElements = 1;
                        //
                        gmshData.EdgeIdNumElements[FeMesh.GmshTopologyId(itemId, partId)] = numElements;
                    }
                    else if (geometryType.IsSurface())
                    {
                        for (int i = 0; i < part.Visualization.FaceEdgeIds[itemId].Length; i++)
                        {
                            edgeId = part.Visualization.FaceEdgeIds[itemId][i];
                            length = part.Visualization.EdgeLengths[edgeId];
                            numElements = (int)Math.Round(length / meshRefinement.MeshSize, 0, MidpointRounding.AwayFromZero);
                            if (numElements < 1) numElements = 1;
                            //
                            gmshData.EdgeIdNumElements[FeMesh.GmshTopologyId(edgeId, partId)] = numElements;
                        }
                    }
                    else if (geometryType == GeometryType.Part)
                    {
                        for (int i = 0; i < part.Visualization.FaceCount; i++)
                        {
                            for (int j = 0; j < part.Visualization.FaceEdgeIds[i].Length; j++)
                            {
                                edgeId = part.Visualization.FaceEdgeIds[i][j];
                                length = part.Visualization.EdgeLengths[edgeId];
                                numElements = (int)Math.Round(length / meshRefinement.MeshSize, 0, MidpointRounding.AwayFromZero);
                                if (numElements < 1) numElements = 1;
                                //
                                gmshData.EdgeIdNumElements[FeMesh.GmshTopologyId(edgeId, partId)] = numElements;
                            }
                        }
                    }
                    else throw new NotSupportedException();
                }
                // Add sizes
                foreach (var nodeId in nodeIds) gmshData.VertexNodeIdMeshSize[nodeId] = meshRefinement.MeshSize;
            }
        }
        //
        public bool PreviewEdgeMesh(string partName, MeshSetupItem meshSetupItem)
        {
            GeometryPart part = (GeometryPart)_model.Geometry.Parts[partName];
            //
            OrderedDictionary<string, MeshSetupItem> meshSetupItemsDict = _model.Geometry.MeshSetupItems.DeepClone();
            // If the form to edit mesh setup item is open, overwrite the item
            if (meshSetupItem != null)
            {
                string error = IsMeshSetupItemProperlyDefined(meshSetupItem);
                if (error != null) throw new CaeException(error + " The preview of the mesh failed.");
                //
                if (meshSetupItemsDict.ContainsKey(meshSetupItem.Name)) meshSetupItemsDict[meshSetupItem.Name] = meshSetupItem;
                else meshSetupItemsDict.Add(meshSetupItem.Name, meshSetupItem);
            }
            //
            bool result;
            if (part.IsCADPart) result = PreviewEdgeMeshFromBrep(part, meshSetupItemsDict);
            else result = PreviewEdgeMeshFromStl(part, meshSetupItemsDict);
            // Thicken shell mesh
            if (result)
            {
                MeshSetupItem[] meshSetupItems = GetActiveValidMeshSetupItems<ThickenShellMesh>(part.Name, meshSetupItemsDict);
                if (meshSetupItems != null && meshSetupItems.Length > 0)
                {
                    ThickenShellMesh tsm = (ThickenShellMesh)meshSetupItems[meshSetupItems.Length - 1];
                    result &= PreviewThickenShellMesh(new string[] { partName }, tsm.Thickness, tsm.NumberOfLayers, tsm.Offset);
                }
            }
            return result;
        }
        public bool PreviewEdgeMeshFromStl(GeometryPart part, OrderedDictionary<string, MeshSetupItem> meshSetupItems)
        {
            if (part.PartType == PartType.Shell) return false;  // not supported
            //
            CalculixSettings settings = _settings.Calculix;
            if (settings.WorkDirectory == null || !Directory.Exists(settings.WorkDirectory))
            {
                MessageBoxes.ShowWorkDirectoryError();
                return false;
            }
            //
            string executable = Application.StartupPath + Globals.NetGenMesher;
            string stlFileName = Path.Combine(settings.WorkDirectory, Globals.StlFileName);
            string volFileName = Path.Combine(settings.WorkDirectory, Globals.VolFileName);
            string meshParametersFileName = Path.Combine(settings.WorkDirectory, Globals.MeshParametersFileName);
            string meshRefinementFileName = Path.Combine(settings.WorkDirectory, Globals.MeshRefinementFileName);
            string edgeNodesFileName = Path.Combine(settings.WorkDirectory, Globals.EdgeNodesFileName);
            //
            if (File.Exists(stlFileName)) File.Delete(stlFileName);
            if (File.Exists(volFileName)) File.Delete(volFileName);
            if (File.Exists(meshParametersFileName)) File.Delete(meshParametersFileName);
            if (File.Exists(meshRefinementFileName)) File.Delete(meshRefinementFileName);
            if (File.Exists(edgeNodesFileName)) File.Delete(edgeNodesFileName);
            //
            SuppressExplodedView(new string[] { part.Name });
            FileInOut.Output.StlFileWriter.Write(stlFileName, _model.Geometry, new string[] { part.Name });
            CreateMeshRefinementFile(part, meshRefinementFileName, meshSetupItems);
            MeshingParameters meshingParameters = GetPartMeshingParameters(part.Name, meshSetupItems);
            meshingParameters.WriteToFile(meshParametersFileName, part.BoundingBox.GetDiagonal());
            _model.Geometry.WriteEdgeNodesToFile(part, edgeNodesFileName);
            ResumeExplodedViews(false);
            //
            string argument = "STL_EDGE_MESH " +
                              "\"" + stlFileName + "\" " +
                              "\"" + volFileName + "\" " +
                              "\"" + meshParametersFileName + "\" " +
                              "\"" + meshRefinementFileName + "\" " +
                              "\"" + edgeNodesFileName + "\"";

            _executableJob = new ExecutableJob(part.Name, executable, argument, settings.WorkDirectory);
            _executableJob.AppendOutput += executableJobMeshing_AppendOutput;
            _executableJob.Submit();
            // Job completed
            if (_executableJob.JobStatus == JobStatus.OK)
            {
                ImportGeneratedNodeMesh(volFileName, part, false);
                return true;
            }
            else return false;
        }
        public bool PreviewEdgeMeshFromBrep(GeometryPart part, OrderedDictionary<string, MeshSetupItem> meshSetupItems)
        {
            MeshSetupItem[] gmshSetupItems = GetActiveValidMeshSetupItems<GmshSetupItem>(part.Name, meshSetupItems);
            //
            if (gmshSetupItems.Length > 0) return PreviewEdgeMeshFromBrepGmsh(part, gmshSetupItems, meshSetupItems);
            else return PreviewEdgeMeshFromBrepNetgen(part, meshSetupItems);
        }
        public bool PreviewEdgeMeshFromBrepNetgen(GeometryPart part, OrderedDictionary<string, MeshSetupItem> meshSetupItems)
        {
            CalculixSettings settings = _settings.Calculix;
            if (settings.WorkDirectory == null || !Directory.Exists(settings.WorkDirectory))
            {
                MessageBoxes.ShowWorkDirectoryError();
                return false;
            }
            //
            string executable = Application.StartupPath + Globals.NetGenMesher;
            string brepFileName = Path.Combine(settings.WorkDirectory, Globals.BrepFileName);
            string volFileName = Path.Combine(settings.WorkDirectory, Globals.VolFileName);
            string meshParametersFileName = Path.Combine(settings.WorkDirectory, Globals.MeshParametersFileName);
            string meshRefinementFileName = Path.Combine(settings.WorkDirectory, Globals.MeshRefinementFileName);
            //
            if (File.Exists(brepFileName)) File.Delete(brepFileName);
            if (File.Exists(volFileName)) File.Delete(volFileName);
            if (File.Exists(meshParametersFileName)) File.Delete(meshParametersFileName);
            if (File.Exists(meshRefinementFileName)) File.Delete(meshRefinementFileName);
            //
            SuppressExplodedView(new string[] { part.Name });
            File.WriteAllText(brepFileName, part.CADFileData);
            CreateMeshRefinementFile(part, meshRefinementFileName, meshSetupItems);
            MeshingParameters meshingParameters = GetPartMeshingParameters(part.Name, meshSetupItems);
            meshingParameters.WriteToFile(meshParametersFileName, part.BoundingBox.GetDiagonal());
            ResumeExplodedViews(false);
            //
            string argument = "BREP_EDGE_MESH " +
                              "\"" + brepFileName.ToUTF8() + "\" " +
                              "\"" + volFileName + "\" " +
                              "\"" + meshParametersFileName + "\" " +
                              "\"" + meshRefinementFileName + "\"";
            //
            _executableJob = new ExecutableJob(part.Name, executable, argument, settings.WorkDirectory);
            _executableJob.AppendOutput += executableJobMeshing_AppendOutput;
            _executableJob.Submit();
            // Job completed
            if (_executableJob.JobStatus == JobStatus.OK)
            {
                ImportGeneratedNodeMesh(volFileName, part, false);
                return true;
            }
            else return false;
        }
        public bool PreviewEdgeMeshFromBrepGmsh(GeometryPart part, MeshSetupItem[] gmshSetupItems,
                                                OrderedDictionary<string, MeshSetupItem> meshSetupItems)
        {
            CalculixSettings settings = _settings.Calculix;
            if (settings.WorkDirectory == null || !Directory.Exists(settings.WorkDirectory))
            {
                MessageBoxes.ShowWorkDirectoryError();
                return false;
            }
            //
            string executable = Application.StartupPath + Globals.GmshMesher;
            string brepFileName = Path.Combine(settings.WorkDirectory, Globals.BrepFileName);
            string inpFileName = Path.Combine(settings.WorkDirectory, Globals.InpMeshFileName);
            string gmshDataFileName = Path.Combine(settings.WorkDirectory, Globals.GmshDataFileName);
            //
            if (File.Exists(brepFileName)) File.Delete(brepFileName);
            if (File.Exists(inpFileName)) File.Delete(inpFileName);
            if (File.Exists(gmshDataFileName)) File.Delete(gmshDataFileName);
            //
            SuppressExplodedView(new string[] { part.Name });
            File.WriteAllText(brepFileName, part.CADFileData);
            MeshingParameters partMeshingParameters = GetPartMeshingParameters(part.Name, meshSetupItems);
            GmshData gmshData = new GmshData(brepFileName);
            gmshData.InpFileName = inpFileName;
            gmshData.PartMeshingParameters = partMeshingParameters;
            gmshData.GmshSetupItems = gmshSetupItems;
            gmshData.Preview = true;
            _model.Geometry.GetPartTopologyForGmsh(part.Name, ref gmshData);
            GetMeshItemSizes(part.Name, ref gmshData, meshSetupItems);
            gmshData.WriteToFile(gmshDataFileName);
            ResumeExplodedViews(false);
            //
            string argument = Globals.GmshDataFileName;
            //
            bool jobCompleted;
            if (System.Diagnostics.Debugger.IsAttached)
            {
                GmshBase gmsh = new GmshBase(gmshData, _form.WriteDataToOutput);
                string error = gmsh.CreateMesh();
                jobCompleted = error == null;
            }
            else
            {
                _executableJob = new ExecutableJob(part.Name, executable, argument, settings.WorkDirectory);
                _executableJob.AppendOutput += executableJobMeshing_AppendOutput;
                _executableJob.Submit();
                // Job completed
                jobCompleted = _executableJob.JobStatus == JobStatus.OK;
            }
            //
            if (jobCompleted)
            {
                int id2;
                int[] key;
                FeElement element;
                CompareIntArray comparer = new CompareIntArray();
                HashSet<int[]> edgeKeys = new HashSet<int[]>(comparer);
                List<double[][]> lines = new List<double[][]>();
                double[][] line;
                //
                FileInOut.Input.ElementsToImport elementsToImport;
                bool importEdges = gmshSetupItems.Length == 1 &&
                    (gmshSetupItems[0] is ShellGmsh || gmshSetupItems[0] is ExtrudeMesh || gmshSetupItems[0] is RevolveMesh);
                if (importEdges) elementsToImport = FileInOut.Input.ElementsToImport.Shell;
                else elementsToImport = FileInOut.Input.ElementsToImport.Beam;
                // Mesh
                FeMesh mesh = FileInOut.Input.InpFileReader.ReadMesh(inpFileName, elementsToImport, false);
                // Exploded view
                if (IsExplodedViewActive())
                {
                    double[] offset;
                    if (part is CompoundGeometryPart cgp) offset = _model.Geometry.Parts[cgp.SubPartNames[0]].Offset;
                    else offset = part.Offset;
                    //
                    Dictionary<string, double[]> partOffsets = new Dictionary<string, double[]>();
                    foreach (var entry in mesh.Parts) partOffsets.Add(entry.Key, offset);
                    mesh.ApplyExplodedView(partOffsets);
                }
                // Import edges
                if (importEdges)
                {
                    foreach (var entry in mesh.Elements)
                    {
                        element = entry.Value;
                        int numOfNodes = -1;
                        if (entry.Value is LinearTriangleElement) numOfNodes = 3;
                        else if (entry.Value is LinearQuadrilateralElement) numOfNodes = 4;
                        //
                        if (numOfNodes > 0)
                        {
                            for (int i = 0; i < numOfNodes; i++)
                            {
                                id2 = (i + 1) % numOfNodes;
                                key = Tools.GetSortedKey(element.NodeIds[i], element.NodeIds[id2]);
                                if (!edgeKeys.Contains(key))
                                {
                                    line = new double[2][];
                                    line[0] = mesh.Nodes[element.NodeIds[i]].Coor;
                                    line[1] = mesh.Nodes[element.NodeIds[id2]].Coor;
                                    lines.Add(line);
                                    edgeKeys.Add(key);
                                }
                            }
                        }
                        else throw new NotSupportedException();
                    }
                    //
                    HighlightConnectedEdges(lines.ToArray());
                }
                // Import nodes
                else
                {
                    double[][] nodeCoor = mesh.GetAllNodeCoor();
                    DrawNodes("nodeMesh", nodeCoor, Color.Black, vtkRendererLayer.Selection, -1, true);
                }
                return true;
            }
            else return false;
        }
        public void ImportGeneratedNodeMesh(string fileName, GeometryPart part, bool resetCamera)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException("The file: '" + fileName + "' does not exist." + Environment.NewLine +
                                                "The reason is a failed mesh generation procedure for part: " + part.Name);
            //
            FeMesh mesh = FileInOut.Input.VolFileReader.Read(fileName, FileInOut.Input.ElementsToImport.All, false);
            // Exploded view
            if (IsExplodedViewActive())
            {
                double[] offset;
                if (part is CompoundGeometryPart cgp) offset = _model.Geometry.Parts[cgp.SubPartNames[0]].Offset;
                else offset = part.Offset;
                //
                Dictionary<string, double[]> partOffsets = new Dictionary<string, double[]>();
                foreach (var entry in mesh.Parts) partOffsets.Add(entry.Key, offset);
                mesh.ApplyExplodedView(partOffsets);
            }
            //
            double[][] nodeCoor = mesh.GetAllNodeCoor();
            DrawNodes("nodeMesh", nodeCoor, Color.Black, vtkRendererLayer.Selection, -1, true);
        }
        //
        public bool CreateMesh(string partName)
        {
            GeometryPart part = (GeometryPart)_model.Geometry.Parts[partName];
            //
            bool result;
            if (part.IsCADPart) result = CreateMeshFromBrep(part);
            else result = CreateMeshFromStl(part);
            // Thicken shell mesh
            if (result)
            {
                MeshSetupItem[] meshSetupItems = GetActiveValidMeshSetupItems<ThickenShellMesh>(part.Name);
                if (meshSetupItems != null && meshSetupItems.Length > 0)
                {
                    ThickenShellMesh tsm = (ThickenShellMesh)meshSetupItems[meshSetupItems.Length - 1];
                    result &= ThickenShellMesh(new string[] { partName }, tsm.Thickness, tsm.NumberOfLayers, tsm.Offset);
                }
            }
            return result;
        }
        private bool CreateMeshFromStl(GeometryPart part)
        {
            MeshSetupItem[] meshSetupItems = GetActiveValidMeshSetupItems<GmshSetupItem>(part.Name);
            //
            if (meshSetupItems.Length > 0) return CreateMeshFromStlGmsh(part, meshSetupItems);
            else
            {
                if (part.PartType == PartType.Solid || part.PartType == PartType.SolidAsShell) return CreateMeshFromSolidStl(part);
                else if (part.PartType == PartType.Shell) return CreateMeshFromShellStl(part);
                else throw new NotSupportedException();
            }
        }
        private bool CreateMeshFromStlGmsh(GeometryPart part, MeshSetupItem[] meshSetupItems)
        {
            _form.WriteDataToOutput("");
            //
            CalculixSettings settings = _settings.Calculix;
            if (settings.WorkDirectory == null || !Directory.Exists(settings.WorkDirectory))
            {
                MessageBoxes.ShowWorkDirectoryError();
                return false;
            }
            //
            string executable = Application.StartupPath + Globals.GmshMesher;
            string stlFileName = Path.Combine(settings.WorkDirectory, Globals.StlFileName);
            string inpFileName = Path.Combine(settings.WorkDirectory, Globals.InpMeshFileName);
            string gmshDataFileName = Path.Combine(settings.WorkDirectory, Globals.GmshDataFileName);
            //
            if (File.Exists(stlFileName)) File.Delete(stlFileName);
            if (File.Exists(inpFileName)) File.Delete(inpFileName);
            if (File.Exists(gmshDataFileName)) File.Delete(gmshDataFileName);
            //
            string[] partNames = new string[] { part.Name };
            SuppressExplodedView(partNames);
            FileInOut.Output.StlFileWriter.Write(stlFileName, _model.Geometry, partNames);
            MeshingParameters partMeshingParameters = GetPartMeshingParameters(part.Name);
            GmshData gmshData = new GmshData(stlFileName);
            gmshData.InpFileName = inpFileName;
            gmshData.PartMeshingParameters = partMeshingParameters;
            gmshData.GmshSetupItems = meshSetupItems;
            gmshData.Preview = false;
            _model.Geometry.GetPartTopologyForGmsh(part.Name, ref gmshData);
            GetMeshItemSizes(part.Name, ref gmshData);
            gmshData.WriteToFile(gmshDataFileName);
            ResumeExplodedViews(false);
            //
            string argument = Globals.GmshDataFileName;
            //
            bool jobCompleted;
            if (System.Diagnostics.Debugger.IsAttached)
            {
                GmshBase gmsh = new GmshBase(gmshData, _form.WriteDataToOutput);
                string error = gmsh.CreateMesh();
                jobCompleted = error == null;
            }
            else
            {
                _executableJob = new ExecutableJob(part.Name, executable, argument, settings.WorkDirectory);
                _executableJob.AppendOutput += executableJobMeshing_AppendOutput;
                _executableJob.Submit();
                // Job completed
                jobCompleted = _executableJob.JobStatus == JobStatus.OK;
            }
            //
            if (jobCompleted)
            {
                ImportGeneratedMesh(inpFileName, part, true);
                return true;
            }
            else return false;
        }
        private bool CreateMeshFromSolidStl(GeometryPart part)
        {
            CalculixSettings settings = _settings.Calculix;
            if (settings.WorkDirectory == null || !Directory.Exists(settings.WorkDirectory))
            {
                MessageBoxes.ShowWorkDirectoryError();
                return false;
            }
            //
            string executable = Application.StartupPath + Globals.NetGenMesher;
            string stlFileName = Path.Combine(settings.WorkDirectory, Globals.StlFileName);
            string volFileName = Path.Combine(settings.WorkDirectory, Globals.VolFileName);
            string meshParametersFileName = Path.Combine(settings.WorkDirectory, Globals.MeshParametersFileName);
            string meshRefinementFileName = Path.Combine(settings.WorkDirectory, Globals.MeshRefinementFileName);
            string edgeNodesFileName = Path.Combine(settings.WorkDirectory, Globals.EdgeNodesFileName);
            //
            if (File.Exists(stlFileName)) File.Delete(stlFileName);
            if (File.Exists(volFileName)) File.Delete(volFileName);
            if (File.Exists(meshParametersFileName)) File.Delete(meshParametersFileName);
            if (File.Exists(meshRefinementFileName)) File.Delete(meshRefinementFileName);
            if (File.Exists(edgeNodesFileName)) File.Delete(edgeNodesFileName);
            //
            string[] partNames = new string[] { part.Name };
            SuppressExplodedView(partNames);
            FileInOut.Output.StlFileWriter.Write(stlFileName, _model.Geometry, partNames);
            CreateMeshRefinementFile(part, meshRefinementFileName, null);
            GetPartMeshingParameters(part.Name).WriteToFile(meshParametersFileName, part.BoundingBox.GetDiagonal());
            _model.Geometry.WriteEdgeNodesToFile(part, edgeNodesFileName);
            ResumeExplodedViews(false);
            //
            string argument = "STL_MESH " +
                              "\"" + stlFileName + "\" " +
                              "\"" + volFileName + "\" " +
                              "\"" + meshParametersFileName + "\" " +
                              "\"" + meshRefinementFileName + "\" " +
                              "\"" + edgeNodesFileName + "\"";
            //
            _executableJob = new ExecutableJob(part.Name, executable, argument, settings.WorkDirectory);
            _executableJob.AppendOutput += executableJobMeshing_AppendOutput;
            _executableJob.Submit();
            // Job completed
            if (_executableJob.JobStatus == JobStatus.OK)
            {
                ImportGeneratedMesh(volFileName, part, false);
                return true;
            }
            else throw new CaeException("Mesh generation failed.");
        }
        private bool CreateMeshFromShellStl(GeometryPart part)
        {
            CalculixSettings settings = _settings.Calculix;
            if (settings.WorkDirectory == null || !Directory.Exists(settings.WorkDirectory))
            {
                MessageBoxes.ShowWorkDirectoryError();
                return false;
            }
            //
            string executable = Application.StartupPath + Globals.MmgsMesher;
            string mmgInFileName = Path.Combine(settings.WorkDirectory, Globals.MmgMeshFileName);
            string mmgOutFileName = Path.Combine(settings.WorkDirectory, Path.GetFileNameWithoutExtension(Globals.MmgMeshFileName) +
                                                 ".o" + Path.GetExtension(Globals.MmgMeshFileName));
            string mmgSolFileName = Path.Combine(settings.WorkDirectory,
                                                 Path.GetFileNameWithoutExtension(Globals.MmgMeshFileName) +
                                                 ".sol");
            //
            if (File.Exists(mmgInFileName)) File.Delete(mmgInFileName);
            if (File.Exists(mmgOutFileName)) File.Delete(mmgOutFileName);
            if (File.Exists(mmgSolFileName)) File.Delete(mmgSolFileName);
            //
            SuppressExplodedView(new string[] { part.Name });
            MeshingParameters meshingParameters = GetPartMeshingParameters(part.Name);
            FileInOut.Output.MmgFileWriter.Write(mmgInFileName, part, _model.Geometry, meshingParameters.KeepModelEdges, false);
            ResumeExplodedViews(false);
            //
            System.Diagnostics.PerformanceCounter ramCounter;
            ramCounter = new System.Diagnostics.PerformanceCounter("Memory", "Available MBytes");
            double hausdorff = meshingParameters.Hausdorff;
            if (meshingParameters.RelativeSize) hausdorff = part.BoundingBox.GetDiagonal() * meshingParameters.FactorHausdorff;
            //
            string argument = //"-nr " + 
                              "-m " + ramCounter.NextValue() * 0.9 + " " +
                              "-ar 0.01 " + // this removes curving of the faces
                                            //"-hsiz 0.08 " +  
                              "-hmax " + meshingParameters.MaxH + " " +
                              "-hmin " + meshingParameters.MinH + " " +
                              "-hausd " + hausdorff + " " +
                              "-in \"" + mmgInFileName + "\" " +
                              "-out \"" + mmgOutFileName + "\" ";
            //
            _executableJob = new ExecutableJob(part.Name, executable, argument, settings.WorkDirectory);
            _executableJob.AppendOutput += executableJobMeshing_AppendOutput;
            _executableJob.Submit();
            // Job completed
            if (_executableJob.JobStatus == JobStatus.OK)
            {
                FeMesh mesh = FileInOut.Input.MmgFileReader.Read(mmgOutFileName, MeshRepresentation.Geometry);
                GeometryPart partOut = (GeometryPart)mesh.Parts.First().Value;
                //
                if (File.Exists(mmgInFileName)) File.Delete(mmgInFileName);
                if (File.Exists(mmgOutFileName)) File.Delete(mmgOutFileName);
                if (File.Exists(mmgSolFileName)) File.Delete(mmgSolFileName);
                //
                FileInOut.Output.MmgFileWriter.Write(mmgInFileName, partOut, mesh, meshingParameters.KeepModelEdges, true);
                //
                argument = "-nr " +
                           "-m " + ramCounter.NextValue() * 0.9 + " " +
                           //"-ar 0 " +
                           //"-hsiz 0.08 " +  
                           "-hmax " + meshingParameters.MaxH + " " +
                           "-hmin " + meshingParameters.MinH + " " +
                           "-hausd " + hausdorff + " " +
                           "-in \"" + mmgInFileName + "\" " +
                           "-out \"" + mmgOutFileName + "\" ";
                //
                _executableJob = new ExecutableJob(part.Name, executable, argument, settings.WorkDirectory);
                _executableJob.AppendOutput += executableJobMeshing_AppendOutput;
                _executableJob.Submit();
                //
                if (_executableJob.JobStatus == JobStatus.OK)
                {
                    ImportGeneratedMesh(mmgOutFileName, part, false);
                    return true;
                }
                else throw new CaeException("Mesh generation failed.");
            }
            else throw new CaeException("Mesh generation failed.");
        }
        private bool CreateMeshFromBrep(GeometryPart part)
        {
            MeshSetupItem[] meshSetupItems = GetActiveValidMeshSetupItems<GmshSetupItem>(part.Name);
            //
            if (meshSetupItems.Length > 0) return CreateMeshFromBrepGmsh(part, meshSetupItems);
            else return CreateMeshFromBrepNetgen(part);
        }
        private bool CreateMeshFromBrepNetgen(GeometryPart part)
        {
            CalculixSettings settings = _settings.Calculix;
            if (settings.WorkDirectory == null || !Directory.Exists(settings.WorkDirectory))
            {
                MessageBoxes.ShowWorkDirectoryError();
                return false;
            }
            //
            string executable = Application.StartupPath + Globals.NetGenMesher;
            string brepFileName = Path.Combine(settings.WorkDirectory, Globals.BrepFileName);
            string volFileName = Path.Combine(settings.WorkDirectory, Globals.VolFileName);
            string meshParametersFileName = Path.Combine(settings.WorkDirectory, Globals.MeshParametersFileName);
            string meshRefinementFileName = Path.Combine(settings.WorkDirectory, Globals.MeshRefinementFileName);
            //
            if (File.Exists(brepFileName)) File.Delete(brepFileName);
            if (File.Exists(volFileName)) File.Delete(volFileName);
            if (File.Exists(meshParametersFileName)) File.Delete(meshParametersFileName);
            if (File.Exists(meshRefinementFileName)) File.Delete(meshRefinementFileName);
            //
            SuppressExplodedView(new string[] { part.Name });
            File.WriteAllText(brepFileName, part.CADFileData);
            MeshingParameters meshingParameters = GetPartMeshingParameters(part.Name);
            meshingParameters.WriteToFile(meshParametersFileName, part.BoundingBox.GetDiagonal());
            CreateMeshRefinementFile(part, meshRefinementFileName, null);
            ResumeExplodedViews(false);
            //
            string argument = "BREP_MESH " +
                              "\"" + brepFileName.ToUTF8() + "\" " +
                              "\"" + volFileName + "\" " +
                              "\"" + meshParametersFileName + "\" " +
                              "\"" + meshRefinementFileName + "\"";
            //
            _executableJob = new ExecutableJob(part.Name, executable, argument, settings.WorkDirectory);
            _executableJob.AppendOutput += executableJobMeshing_AppendOutput;
            _executableJob.Submit();
            // Job completed
            if (_executableJob.JobStatus == JobStatus.OK)
            {
                //bool convertToSecondOrder = meshingParameters.SecondOrder && !meshingParameters.MidsideNodesOnGeometry;
                ImportGeneratedMesh(volFileName, part, true);
                return true;
            }
            else throw new CaeException("Mesh generation failed.");
        }
        private bool CreateMeshFromBrepGmsh(GeometryPart part, MeshSetupItem[] meshSetupItems)
        {
            _form.WriteDataToOutput("");
            //
            CalculixSettings settings = _settings.Calculix;
            if (settings.WorkDirectory == null || !Directory.Exists(settings.WorkDirectory))
            {
                MessageBoxes.ShowWorkDirectoryError();
                return false;
            }
            //
            string executable = Application.StartupPath + Globals.GmshMesher;
            string brepFileName = Path.Combine(settings.WorkDirectory, Globals.BrepFileName);
            string inpFileName = Path.Combine(settings.WorkDirectory, Globals.InpMeshFileName);
            string gmshDataFileName = Path.Combine(settings.WorkDirectory, Globals.GmshDataFileName);
            //
            if (File.Exists(brepFileName)) File.Delete(brepFileName);
            if (File.Exists(inpFileName)) File.Delete(inpFileName);
            if (File.Exists(gmshDataFileName)) File.Delete(gmshDataFileName);
            //
            SuppressExplodedView(new string[] { part.Name });
            File.WriteAllText(brepFileName, part.CADFileData);
            MeshingParameters partMeshingParameters = GetPartMeshingParameters(part.Name);
            GmshData gmshData = new GmshData(brepFileName);
            gmshData.InpFileName = inpFileName;
            gmshData.PartMeshingParameters = partMeshingParameters;
            gmshData.GmshSetupItems = meshSetupItems;
            gmshData.Preview = false;
            _model.Geometry.GetPartTopologyForGmsh(part.Name, ref gmshData);
            GetMeshItemSizes(part.Name, ref gmshData);
            gmshData.WriteToFile(gmshDataFileName);
            ResumeExplodedViews(false);
            //
            string argument = Globals.GmshDataFileName;
            //
            string error = null;
            bool jobCompleted;
            bool debuggerAttached = System.Diagnostics.Debugger.IsAttached;
            if (debuggerAttached)
            {
                GmshBase gmsh = new GmshBase(gmshData, _form.WriteDataToOutput);
                error = gmsh.CreateMesh();
                jobCompleted = error == null;
            }
            else
            {
                _executableJob = new ExecutableJob(part.Name, executable, argument, settings.WorkDirectory);
                _executableJob.AppendOutput += executableJobMeshing_AppendOutput;
                _executableJob.Submit();
                // Job completed
                jobCompleted = _executableJob.JobStatus == JobStatus.OK;
            }
            //
            if (jobCompleted)
            {
                ImportGeneratedMesh(inpFileName, part, true);
                return true;
            }
            else
            {
                string message = "Mesh generation failed.";
                if (error != null) message += Environment.NewLine + error;
                throw new CaeException(message);
            }
        }
        private void CreateMeshRefinementFile(GeometryPart part, string fileName,
                                              OrderedDictionary<string, MeshSetupItem> meshSetupItems = null)
        {
            double h;
            int geometryPartId;
            int[] geometryIds;
            List<int> filteredGeometryIds = new List<int>();
            FeMeshRefinement meshRefinement;
            int numPoints = 0;
            int numLines = 0;
            List<double[]> pointsList;
            List<double[][]> linesList;
            Dictionary<double, List<double[]>> allPoints = new Dictionary<double, List<double[]>>();
            Dictionary<double, List<double[][]>> allLines = new Dictionary<double, List<double[][]>>();
            //
            Dictionary<string, FeMeshRefinement> meshRefinements = new Dictionary<string, FeMeshRefinement>();
            if (meshSetupItems == null) meshSetupItems = _model.Geometry.MeshSetupItems;
            foreach (var entry in meshSetupItems)
            {
                if (entry.Value is FeMeshRefinement mr) meshRefinements.Add(mr.Name, mr);
            }
            // Get part ids of the geometry to mesh
            HashSet<int> meshPartIds = new HashSet<int>();
            if (part is CompoundGeometryPart cgp)
            {
                foreach (var partName in cgp.SubPartNames) meshPartIds.Add(_model.Geometry.Parts[partName].PartId);
            }
            else meshPartIds.Add(part.PartId);
            // For each mesh refinement
            MeshingParameters meshingParameters;
            foreach (var entry in meshRefinements)
            {
                meshRefinement = entry.Value;
                filteredGeometryIds.Clear();
                // Export mesh refinement only if it is active
                if (meshRefinement.Active && meshRefinement.Valid)
                {
                    // Get part ids of the mesh refinement
                    geometryIds = meshRefinement.CreationIds;
                    if (geometryIds == null || geometryIds.Length == 0) break;
                    // Filter geometry ids to the part being meshed
                    foreach (var geometryId in geometryIds)
                    {
                        geometryPartId = FeMesh.GetPartIdFromGeometryId(geometryId);
                        if (meshPartIds.Contains(geometryPartId)) filteredGeometryIds.Add(geometryId);
                    }
                    geometryIds = filteredGeometryIds.ToArray();
                    // Export refinement only if it was created for the geometry to mesh
                    if (geometryIds.Length > 0)
                    {
                        meshingParameters = GetPartMeshingParameters(part.Name, meshSetupItems);
                        //
                        if (meshRefinement.MeshSize > meshingParameters.MaxH) h = meshingParameters.MaxH;
                        else if (meshRefinement.MeshSize < meshingParameters.MinH) h = meshingParameters.MinH;
                        else h = meshRefinement.MeshSize;
                        //
                        double[][] points;
                        double[][][] lines;
                        _model.Geometry.GetVertexAndEdgeCoorFromGeometryIds(geometryIds, h, false, out points, out lines);
                        numPoints += points.Length;
                        numLines += lines.Length;
                        //
                        if (allPoints.TryGetValue(h, out pointsList)) pointsList.AddRange(points);
                        else allPoints.Add(h, new List<double[]>(points));
                        //
                        if (allLines.TryGetValue(h, out linesList)) linesList.AddRange(lines);
                        else allLines.Add(h, new List<double[][]>(lines));
                    }
                }
            }
            //
            StringBuilder sb = new StringBuilder();
            //
            sb.AppendLine(numPoints.ToString());  // number of points
            foreach (var entry in allPoints)
            {
                h = entry.Key;
                pointsList = entry.Value;
                foreach (var point in pointsList)
                {
                    sb.AppendFormat("{0} {1} {2} {3} {4}", point[0], point[1], point[2], h, Environment.NewLine);
                }
            }
            sb.AppendLine(numLines.ToString());  // number of lines
            foreach (var entry in allLines)
            {
                h = entry.Key;
                linesList = entry.Value;
                foreach (var line in linesList)
                {
                    sb.AppendFormat("{0} {1} {2} {3} {4} {5} {6} {7}",
                                    line[0][0], line[0][1], line[0][2],
                                    line[1][0], line[1][1], line[1][2],
                                    h, Environment.NewLine);
                }
            }
            //
            File.WriteAllText(fileName, sb.ToString());
        }
        //
        public void StopExecutableJob()
        {
            if (_executableJob != null && _executableJob.JobStatus == JobStatus.Running)
            {
                _executableJob.Kill("Cancel button clicked.");
                _form.SetStateReady(Globals.MeshingText);
            }
        }
        void executableJobMeshing_AppendOutput(string data)
        {
            _form.WriteDataToOutput(data);
        }

        #endregion #################################################################################################################

        #region Mesh Setup Items Menu  #############################################################################################
        public void AddMeshSetupItemCommand(MeshSetupItem meshSetupItem)
        {
            CAddMeshSetupItem comm = new CAddMeshSetupItem(meshSetupItem);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceMeshSetupItemCommand(string oldMeshSetupItemName, MeshSetupItem newMeshSetupItem)
        {
            CReplaceMeshSetupItem comm = new CReplaceMeshSetupItem(oldMeshSetupItemName, newMeshSetupItem);
            _commands.AddAndExecute(comm);
        }
        public void DuplicateMeshSetupItemsCommand(string[] meshSetupItemNames)
        {
            CDuplicateMeshSetupItems comm = new CDuplicateMeshSetupItems(meshSetupItemNames);
            _commands.AddAndExecute(comm);
        }
        public void RemoveMeshSetupItemsCommand(string[] meshSetupItemNames)
        {
            CRemoveMeshSetupItems comm = new CRemoveMeshSetupItems(meshSetupItemNames);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public string[] GetMeshSetupItemNames()
        {
            return _model.Geometry.MeshSetupItems.Keys.ToArray();
        }
        public void AddMeshSetupItem(MeshSetupItem meshSetupItem)
        {
            if (meshSetupItem.CreationData != null)
            {
                // In order for the Regenerate history to work perform the selection
                _selection = meshSetupItem.CreationData.DeepClone();
                meshSetupItem.CreationIds = GetSelectionIds();
                _selection.Clear();
            }
            else throw new NotSupportedException("The mesh setup item does not contain any selection data.");
            //
            _model.Geometry.MeshSetupItems.Add(meshSetupItem.Name, meshSetupItem);
            //
            _form.AddTreeNode(ViewGeometryModelResults.Geometry, meshSetupItem, null);
            //
            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public MeshSetupItem GetMeshSetupItem(string meshSetupItemName)
        {
            return _model.Geometry.MeshSetupItems[meshSetupItemName];
        }
        public MeshSetupItem EditMeshSetupItemByForm(MeshSetupItem meshSetupItem)
        {
            return _form.EditMeshingParametersFromHistory(meshSetupItem);
        }
        public MeshSetupItem[] GetMeshSetupItems()
        {
            if (_model.Geometry != null)
            {
                return _model.Geometry.MeshSetupItems.Values.ToArray();
            }
            else return null;
        }
        private MeshSetupItem[] GetActiveValidMeshSetupItems<T>(string partName,
                                                                  OrderedDictionary<string, MeshSetupItem> meshSetupItems = null)
                                                                  where T : MeshSetupItem
        {
            HashSet<int> selectedPartIds;
            string[] meshablePartNames = GetMeshablePartNames(new string[] { partName });
            int[] meshablePartIds = _model.Geometry.GetPartIdsFomPartNames(meshablePartNames);
            List<MeshSetupItem> meshSetupItemsToCheck = new List<MeshSetupItem>();
            //
            if (meshSetupItems == null) meshSetupItems = _model.Geometry.MeshSetupItems;
            //
            foreach (var entry in meshSetupItems)
            {
                if (entry.Value is T msi && msi.Active && msi.Valid)
                {
                    if (msi.CreationIds != null && msi.CreationIds.Length > 0)
                    {
                        selectedPartIds = new HashSet<int>(FeMesh.GetPartIdsFromGeometryIds(msi.CreationIds));
                        //
                        if (selectedPartIds.Intersect(meshablePartIds).Count() == meshablePartIds.Length)
                            meshSetupItemsToCheck.Add(msi);
                    }
                }
            }
            //
            return meshSetupItemsToCheck.ToArray();
        }
        public void ActivateDeactivateMeshSetupItem(string meshSetupItemName, bool active)
        {
            MeshSetupItem meshSetupItem = _model.Geometry.MeshSetupItems[meshSetupItemName];
            meshSetupItem.Active = active;
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Geometry, meshSetupItemName, meshSetupItem, null);
            //
            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void ReplaceMeshSetupItem(string oldMeshSetupItemName, MeshSetupItem meshSetupItem,
                                         bool updateSelection = true)
        {
            if (meshSetupItem.CreationData != null)
            {
                Selection selection = _selection.DeepClone();
                // In order for the Regenerate history to work perform the selection
                _selection = meshSetupItem.CreationData.DeepClone();
                meshSetupItem.CreationIds = GetSelectionIds();
                _selection = selection;
            }
            else throw new NotSupportedException("The mesh setup item does not contain any selection data.");
            //
            _model.Geometry.MeshSetupItems.Replace(oldMeshSetupItemName, meshSetupItem.Name, meshSetupItem);
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Geometry, oldMeshSetupItemName, meshSetupItem,
                                 null, updateSelection);
            //
            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void DuplicateMeshSetupItems(string[] meshSetupItemNames)
        {
            MeshSetupItem newMeshSetupItem;
            foreach (var name in meshSetupItemNames)
            {
                newMeshSetupItem = _model.Geometry.MeshSetupItems[name].DeepClone();
                newMeshSetupItem.Name = NamedClass.GetNameWithoutLastValue(newMeshSetupItem.Name);
                newMeshSetupItem.Name = _model.Geometry.MeshSetupItems.GetNextNumberedKey(newMeshSetupItem.Name);
                AddMeshSetupItem(newMeshSetupItem);
            }
        }
        public void RemoveMeshSetupItems(string[] meshSetupItemNames)
        {
            foreach (var name in meshSetupItemNames)
            {
                _model.Geometry.MeshSetupItems.Remove(name);
                _form.RemoveTreeNode<MeshSetupItem>(ViewGeometryModelResults.Geometry, name, null);
            }
            //
            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
            //
            ClearAllSelection();
        }
        //
        private void UpdateMeshSetupItems(bool updateSelection = true)
        {
            if (_model != null && _model.Geometry != null)
            {
                // Use array not to throw collection modified exception
                foreach (var meshSetupItem in _model.Geometry.MeshSetupItems.Values.ToArray())
                {
                    if (meshSetupItem.CreationData.IsGeometryBased())
                    {
                        meshSetupItem.Valid = true;
                        ReplaceMeshSetupItem(meshSetupItem.Name, meshSetupItem, updateSelection);
                    }
                }
            }
        }
        //
        public string IsMeshSetupItemProperlyDefined(MeshSetupItem meshSetupItem)
        {
            try
            {
                return _model.IsMeshSetupItemProperlyDefined(meshSetupItem);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion #################################################################################################################

        #region Node menu   ########################################################################################################
        // COMMANDS ********************************************************************************
        public void RenumberNodesCommand(int startNodeId)
        {
            Commands.CRenumberNodes comm = new Commands.CRenumberNodes(startNodeId);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public void RenumberNodes(int startNodeId)
        {
            if (_currentView == ViewGeometryModelResults.Model) _model.Mesh.RenumberNodes(startNodeId);
            else throw new NotSupportedException();
        }
        public int[] GetAllNodeIds()
        {
            if (_currentView == ViewGeometryModelResults.Geometry) return _model.Geometry.Nodes.Keys.ToArray();
            else if (_currentView == ViewGeometryModelResults.Model) return _model.Mesh.Nodes.Keys.ToArray();
            else return _allResults.CurrentResult.Mesh.Nodes.Keys.ToArray();
        }
        public int[] GetVisibleNodeIds()
        {
            if (_currentView == ViewGeometryModelResults.Geometry) return _model.Geometry.GetVisibleNodeIds();
            else if (_currentView == ViewGeometryModelResults.Model) return _model.Mesh.GetVisibleNodeIds();
            else if (_currentView == ViewGeometryModelResults.Results) return _allResults.CurrentResult.Mesh.GetVisibleNodeIds();
            else throw new NotSupportedException();
        }
        public int[] GetAllOuterNodeIds()
        {
            HashSet<int> outerNodes = new HashSet<int>();

            foreach (var entry in DisplayedMesh.Parts)
            {
                foreach (int[] cell in entry.Value.Visualization.Cells)
                {
                    foreach (int nodeId in cell) outerNodes.Add(nodeId);
                }
            }

            return outerNodes.ToArray();
        }
        public FeNode[] GetNodes(int[] nodeIds)
        {
            FeMesh mesh = DisplayedMesh;

            FeNode[] nodes = new FeNode[nodeIds.Length];
            for (int i = 0; i < nodeIds.Length; i++)
            {
                nodes[i] = mesh.Nodes[nodeIds[i]];
            }
            return nodes;
        }
        public FeNode GetNode(int nodeId)
        {
            if (_currentView == ViewGeometryModelResults.Results) return _allResults.CurrentResult.UndeformedNodes[nodeId];
            else return DisplayedMesh.Nodes[nodeId];
        }

        #endregion #################################################################################################################

        #region Element menu   #####################################################################################################
        // COMMANDS ********************************************************************************
        public void RenumberElementsCommand(int startNodeId)
        {
            Commands.CRenumberElements comm = new Commands.CRenumberElements(startNodeId);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public void RenumberElements(int startElementId)
        {
            if (_currentView == ViewGeometryModelResults.Model) _model.Mesh.RenumberElements(startElementId);
            else throw new NotSupportedException();
        }
        public int[] GetAllElementIds()
        {
            if (_currentView == ViewGeometryModelResults.Geometry) return _model.Geometry.Elements.Keys.ToArray();
            else if (_currentView == ViewGeometryModelResults.Model) return _model.Mesh.Elements.Keys.ToArray();
            else if (_currentView == ViewGeometryModelResults.Results)
                return _allResults.CurrentResult.Mesh.Elements.Keys.ToArray();
            else throw new NotSupportedException();
        }
        public int[] GetVisibleElementIds()
        {
            if (_currentView == ViewGeometryModelResults.Geometry) return _model.Geometry.GetVisibleElementIds();
            else if (_currentView == ViewGeometryModelResults.Model) return _model.Mesh.GetVisibleElementIds();
            else if (_currentView == ViewGeometryModelResults.Results) return
                    _allResults.CurrentResult.Mesh.GetVisibleElementIds();
            else throw new NotSupportedException();
        }
        public FeElement GetElement(int elementId)
        {
            return DisplayedMesh.Elements[elementId];
        }
        public string GetElementType(int elementId)
        {
            if (_currentView == ViewGeometryModelResults.Model)
            {
                FeMesh mesh = DisplayedMesh;
                FeElement element = mesh.Elements[elementId];
                MeshPart part = (MeshPart)mesh.GetPartFromId(element.PartId);
                return part.GetElementType(element);
            }
            else return null;
        }
        //
        public bool AreElementsAllSolidElements3D(int[] elementIds)
        {
            foreach (int elementId in elementIds)
            {
                if (!(_model.Mesh.Elements[elementId] is FeElement3D)) return false;
            }
            //
            return true;
        }
        public bool AreElementsAllShellElements(int[] elementIds)
        {
            foreach (int elementId in elementIds)
            {
                if (!(_model.Mesh.Elements[elementId] is FeElement2D)) return false;
            }
            //
            return true;
        }

        #endregion #################################################################################################################

        #region Model menu   #######################################################################################################
        // COMMANDS ********************************************************************************
        public void ReplaceModelPropertiesCommand(string newModelName, ModelProperties newModelProperties)
        {
            CReplaceModelProperties comm = new CReplaceModelProperties(newModelName, newModelProperties);
            _commands.AddAndExecute(comm);
        }
        // Tools
        public void FindEdgesByAngleForModelPartsCommand(string[] partNames, double edgeAngle)
        {
            CFindEdgesByAngleForModelPartsCommand comm = new CFindEdgesByAngleForModelPartsCommand(partNames, edgeAngle);
            _commands.AddAndExecute(comm);
        }
        public void CreateBoundaryLayerCommand(int[] geometryIds, double thickness)
        {
            CCreateBoundaryLayer comm = new CCreateBoundaryLayer(geometryIds, thickness);
            _commands.AddAndExecute(comm);
        }
        public void RemeshElementsCommand(RemeshingParameters remeshingParameters)
        {
            CRemeshElements comm = new CRemeshElements(remeshingParameters);
            _commands.AddAndExecute(comm);
        }
        public void ThickenShellMeshCommand(string[] partNames, double thickness, int numberOfLayers, double offset)
        {
            CThickenShellMesh comm = new CThickenShellMesh(partNames, thickness, numberOfLayers, offset);
            _commands.AddAndExecute(comm);
        }
        public void UpdateNodalCoordinatesFromFileCommand(string fileName)
        {
            CUpdateNodalCoordinatesFromFile comm = new CUpdateNodalCoordinatesFromFile(fileName);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public void ReplaceModelProperties(string newModelName, ModelProperties newModelProperties)
        {
            ModelSpaceEnum prevModelSpace = _model.Properties.ModelSpace;
            ModelSpaceEnum newModelSpace = newModelProperties.ModelSpace;
            bool update = prevModelSpace != newModelSpace;
            //
            _model.Name = newModelName;
            _model.Properties = newModelProperties;
            //
            if (update)
            {
                _model.UpdateMeshPartsElementTypes(true);
                // Check for a change to or from AxiSymmetric
                if (prevModelSpace == ModelSpaceEnum.Axisymmetric || newModelSpace == ModelSpaceEnum.Axisymmetric)
                {
                    if (_model.StepCollection.GetNumberOfCentrifLoads() > 0)
                        FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
                }
            }
        }
        // Tools
        public void FindEdgesByAngleForModelParts(string[] partNames, double edgeAngle)
        {
            MeshPart meshPart;
            foreach (var partName in partNames)
            {
                meshPart = (MeshPart)_model.Mesh.Parts[partName];
                if (meshPart.PartType == PartType.Solid)
                    _model.Mesh.ExtractSolidPartVisualization(meshPart, edgeAngle);
                else if (meshPart.PartType == PartType.Shell)
                    _model.Mesh.ExtractShellPartVisualization(meshPart, false, edgeAngle);
                // Update
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, meshPart.Name, meshPart, null);
            }
            // Update
            FeModelUpdate(UpdateType.DrawModel);
            UpdateGeometryBasedItems(false);
            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void CreateBoundaryLayer(int[] geometryIds, double thickness)
        {
            try
            {
                _form.SetStateWorking("Creating...");
                //
                string[] errors = null;
                if (_model != null)
                    errors = _model.Mesh.CreatePrismaticBoundaryLayer(geometryIds, thickness, false, out FeNode[] inPressedNodes);
                // Redraw the geometry for update of the selection based sets
                FeModelUpdate(UpdateType.DrawModel);
                // Update sets - must be called with rendering off - SetStateWorking
                UpdateNodeSetsBasedOnGeometry(false);
                UpdateElementSetsBasedOnGeometry(false);
                UpdateSurfacesBasedOnGeometry(false);
                // Update the sets and symbols
                FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
                //
                if (errors.Length > 0) throw new CaeException(errors[0]);
            }
            catch (Exception ex)
            {
                throw new CaeException(ex.Message);
            }
            finally
            {
                _form.SetStateReady("Creating...");
            }
        }
        public void PreviewBoundaryLayer(int[] geometryIds, double thickness)
        {
            string[] errors = null;
            FeNode[] inPressedNodes = null;
            if (_model != null)
                errors = _model.Mesh.CreatePrismaticBoundaryLayer(geometryIds, thickness, true, out inPressedNodes);
            //
            if (errors.Length > 0) throw new CaeException(errors[0]);
            //
            if (inPressedNodes != null && inPressedNodes.Length > 0)
            {
                double[][] nodeCoor = new double[inPressedNodes.Length][];
                for (int i = 0; i < nodeCoor.Length; i++)
                {
                    nodeCoor[i] = inPressedNodes[i].Coor;
                }
                HighlightNodes(nodeCoor);
            }
        }
        public bool RemeshElements(RemeshingParameters remeshingParameters, bool preview = false)
        {
            bool result;
            // Create an element set to reselect the selection based items
            string name = GetAllElementSetNames().GetNextNumberedKey(CaeMesh.Globals.InternalName + "_Remeshing");
            //
            FeElementSet elementSet;
            if (remeshingParameters.RegionType == RegionTypeEnum.ElementSetName)
            {
                elementSet = new FeElementSet(_model.Mesh.ElementSets[remeshingParameters.RegionName]);
                elementSet.Name = name;
            }
            else if (remeshingParameters.RegionType == RegionTypeEnum.Selection)
            {
                elementSet = new FeElementSet(name, null);
                elementSet.CreationData = remeshingParameters.CreationData;
                elementSet.CreationIds = remeshingParameters.CreationIds;
            }
            else throw new NotSupportedException();
            //
            elementSet.Internal = true;
            AddElementSet(elementSet);
            //
            result = RemeshShellElements(elementSet, remeshingParameters, preview);
            //
            RemoveElementSets(new string[] { elementSet.Name });
            //
            return result;
        }
        private bool RemeshShellElements(FeElementSet elementSet, RemeshingParameters remeshingParameters, bool preview)
        {
            bool result = true;
            FeElement element;
            List<int> elementIds;
            Dictionary<int, List<int>> partIdElementIds = new Dictionary<int, List<int>>();
            // Collect elements by part id
            foreach (var elementId in elementSet.Labels)
            {
                element = _model.Mesh.Elements[elementId];
                if (element is FeElement2D)
                {
                    if (partIdElementIds.TryGetValue(element.PartId, out elementIds)) elementIds.Add(elementId);
                    else partIdElementIds.Add(element.PartId, new List<int>() { elementId });
                }
            }
            //
            if (partIdElementIds.Count == 0) return false;
            // Remesh
            MeshPart part;
            foreach (var entry in partIdElementIds)
            {
                part = (MeshPart)_model.Mesh.GetPartFromId(entry.Key);
                result &= RemeshShellElementsByPart(part, entry.Value.ToArray(), remeshingParameters, preview);
            }
            return result;
        }
        private bool RemeshShellElementsByPart(MeshPart part, int[] elementIds, RemeshingParameters remeshingParameters,
                                               bool preview)
        {
            CalculixSettings settings = _settings.Calculix;
            if (settings.WorkDirectory == null || !Directory.Exists(settings.WorkDirectory))
            {
                MessageBoxes.ShowWorkDirectoryError();
                return false;
            }
            //
            string executable = Application.StartupPath + Globals.MmgsMesher;
            string mmgInFileName = Path.Combine(settings.WorkDirectory, Globals.MmgMeshFileName);
            string mmgOutFileName = Path.Combine(settings.WorkDirectory, Path.GetFileNameWithoutExtension(Globals.MmgMeshFileName) +
                                                 ".o" + Path.GetExtension(Globals.MmgMeshFileName));
            string mmgSolFileName = Path.Combine(settings.WorkDirectory,
                                                 Path.GetFileNameWithoutExtension(Globals.MmgMeshFileName) +
                                                 ".sol");
            //
            if (File.Exists(mmgInFileName)) File.Delete(mmgInFileName);
            if (File.Exists(mmgOutFileName)) File.Delete(mmgOutFileName);
            if (File.Exists(mmgSolFileName)) File.Delete(mmgSolFileName);
            //
            Dictionary<int[], FeNode> midNodes;
            FileInOut.Output.MmgFileWriter.WriteShellElements(mmgInFileName, elementIds, part, _model.Mesh,
                                                              remeshingParameters.KeepModelEdges, out midNodes);
            //
            System.Diagnostics.PerformanceCounter ramCounter;
            ramCounter = new System.Diagnostics.PerformanceCounter("Memory", "Available MBytes");
            //
            string argument = "-nr " +
                              "-optim " +
                              //"-ar 30 " +
                              "-m " + ramCounter.NextValue() * 0.9 + " " +
                              "-hgrad " + 1 + " " +
                              "-hgradreq " + 1 + " " +
                              "-hmax " + remeshingParameters.MaxH + " " +
                              "-hmin " + remeshingParameters.MinH + " " +
                              "-hausd " + remeshingParameters.Hausdorff + " " +
                              "-in \"" + mmgInFileName + "\" " +
                              "-out \"" + mmgOutFileName + "\" ";
            //
            _executableJob = new ExecutableJob(part.Name, executable, argument, settings.WorkDirectory);
            _executableJob.AppendOutput += executableJobMeshing_AppendOutput;
            _executableJob.Submit();
            // Job completed
            if (_executableJob.JobStatus == JobStatus.OK)
            {
                // Check if all elements are linear or all elements are parabolic
                HashSet<bool> parabolic = new HashSet<bool>();
                foreach (var elementType in part.ElementTypes) parabolic.Add(FeElement.IsParabolic(elementType));
                if (parabolic.Count != 1) throw new NotSupportedException();
                //
                ImportGeneratedRemesh(mmgOutFileName, elementIds, part, parabolic.First(), midNodes, preview);
                return true;
            }
            else throw new CaeException("Mesh generation failed.");
        }
        public bool ThickenShellMesh(string[] partNames, double thickness, int numberOfLayers, double offset)
        {
            try
            {
                _form.SetStateWorking("Creating...");
                //
                string[] errors = null;
                if (_model != null)
                    errors = _model.Mesh.ThickenShellMesh(partNames, thickness, numberOfLayers, offset, false, out _);
                // Update element type icon
                foreach (var partName in partNames)
                {
                    _form.UpdateTreeNode(ViewGeometryModelResults.Model, partName, _model.Mesh.Parts[partName], null);
                }
                // Redraw the geometry for update of the selection based sets
                FeModelUpdate(UpdateType.DrawModel);
                // Update sets - must be called with rendering off - SetStateWorking
                UpdateNodeSetsBasedOnGeometry(false);
                UpdateElementSetsBasedOnGeometry(false);
                UpdateSurfacesBasedOnGeometry(false);
                // Update the sets and symbols
                FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
                //
                if (errors.Length > 0) throw new CaeException(errors[0]);
                //
                return true;
            }
            catch (Exception ex)
            {
                throw new CaeException(ex.Message);
            }
            finally
            {
                _form.SetStateReady("Creating...");
            }
        }
        public bool PreviewThickenShellMesh(string[] partNames, double thickness, int numberOfLayers, double offset)
        {
            string[] errors = null;
            double[][][] connectedEdges = null;
            if (_model != null)
                errors = DisplayedMesh.ThickenShellMesh(partNames, thickness, numberOfLayers, offset, true, out connectedEdges);
            //
            if (errors.Length > 0) throw new CaeException(errors[0]);
            //
            if (connectedEdges != null && connectedEdges.Length > 0) HighlightConnectedEdges(connectedEdges, false);
            //
            return true;
        }
        public void UpdateNodalCoordinatesFromFile(string fileName)
        {
            FeModel newModel = new FeModel("Deformed");
            newModel.Properties.ModelSpace = _model.Properties.ModelSpace;
            newModel.ImportModelFromInpFile(fileName, _form.WriteDataToOutput);
            _model.Mesh.UpdateNodalCoordinatesFromMesh(newModel.Mesh);
            //
            Redraw();
        }

        #endregion #################################################################################################################

        #region Model part menu   ##################################################################################################
        // COMMANDS ********************************************************************************
        public void HideModelPartsCommand(string[] partNames)
        {
            CHideModelParts comm = new CHideModelParts(partNames);
            _commands.AddAndExecute(comm);
        }
        public void ShowModelPartsCommand(string[] partNames)
        {
            CShowModelParts comm = new CShowModelParts(partNames);
            _commands.AddAndExecute(comm);
        }
        public void SetTransparencyForModelPartsCommand(string[] partNames, byte alpha)
        {
            CSetTransparencyForModelParts comm = new CSetTransparencyForModelParts(partNames, alpha);
            _commands.AddAndExecute(comm);
        }
        // Edit
        public void ReplaceModelPartPropertiesCommand(string oldPartName, PartProperties newPartProperties)
        {
            CReplaceModelPart comm = new CReplaceModelPart(oldPartName, newPartProperties);
            _commands.AddAndExecute(comm);
        }
        // Transform
        public void TranslateModelPartsCommand(string[] partNames, double[] translateVector, bool copy)
        {
            CTranslateModelParts comm = new CTranslateModelParts(partNames, translateVector, copy);
            _commands.AddAndExecute(comm);
        }
        public void ScaleModelPartsCommand(string[] partNames, double[] scaleCenter, double[] scaleFactors, bool copy)
        {
            CScaleModelParts comm = new CScaleModelParts(partNames, scaleCenter, scaleFactors, copy);
            _commands.AddAndExecute(comm);
        }
        public void RotateModelPartsCommand(string[] partNames, double[] rotateCenter, double[] rotateAxis, double rotateAngle, bool copy)
        {
            CRotateModelParts comm = new CRotateModelParts(partNames, rotateCenter, rotateAxis, rotateAngle, copy);
            _commands.AddAndExecute(comm);
        }
        //
        public void MergeModelPartsCommand(string[] partNames)
        {
            CMergeModelParts comm = new CMergeModelParts(partNames);
            _commands.AddAndExecute(comm);
        }
        public void RemoveModelPartsCommand(string[] partNames)
        {
            CRemoveModelParts comm = new CRemoveModelParts(partNames);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public string[] GetModelPartNames()
        {
            if (_model.Mesh != null) return _model.Mesh.Parts.Keys.ToArray();
            else return null;
        }
        public MeshPart GetModelPart(string partName)
        {
            return (MeshPart)_model.Mesh.Parts[partName];
        }
        public MeshPart[] GetModelParts(string[] partNames)
        {
            BasePart part;
            MeshPart[] parts = new MeshPart[partNames.Length];
            for (int i = 0; i < partNames.Length; i++)
            {
                _model.Mesh.Parts.TryGetValue(partNames[i], out part);
                parts[i] = (MeshPart)part;
            }
            return parts;
        }
        public MeshPart[] GetModelParts()
        {
            if (_model.Mesh == null) return null;

            int i = 0;
            MeshPart[] parts = new MeshPart[_model.Mesh.Parts.Count];
            foreach (var entry in _model.Mesh.Parts) parts[i++] = (MeshPart)entry.Value;
            return parts;
        }
        public MeshPart[] GetNonCADModelParts()
        {
            if (_model.Mesh == null) return null;
            //
            List<MeshPart> parts = new List<MeshPart>();
            foreach (var entry in _model.Mesh.Parts)
            {
                if (!IsBasePartBasedOnCADGeometry(entry.Value)) parts.Add((MeshPart)entry.Value);
            }
            return parts.ToArray();
        }
        private bool IsBasePartBasedOnCADGeometry(BasePart part)
        {
            BasePart geometryPart;
            if (_model.Geometry == null) return false;
            else _model.Geometry.Parts.TryGetValue(part.Name, out geometryPart);
            //
            if (part is MeshPart mp && mp.CreatedFromBasePart) return false;
            //
            if (part is ResultPart rp && rp.CreatedFromBasePart) return false;
            //
            if (geometryPart == null) return false;
            else if (geometryPart is GeometryPart gp && !gp.IsCADPart) return false;
            //
            return true;
        }
        public string[] GetModelPartNames<T>()
        {
            throw new NotSupportedException("All elements must be checked.");
            //
            List<string> names = new List<string>();
            foreach (var entry in _model.Mesh.Parts)
            {
                if (entry.Value.Labels.Length > 0 && _model.Mesh.Elements[entry.Value.Labels[0]] is T)
                {
                    names.Add(entry.Key);
                }
            }
            return names.ToArray();
        }
        public void HideModelParts(string[] partNames)
        {
            BasePart part;
            foreach (var name in partNames)
            {
                if (_model.Mesh.Parts.TryGetValue(name, out part))
                {
                    part.Visible = false;
                    _form.UpdateTreeNode(ViewGeometryModelResults.Model, name, part, null, false);
                }
            }
            _form.HideActors(partNames, false);
            //
            AnnotateWithColorLegend();
            //
            FeModelUpdate(UpdateType.RedrawSymbols);
            // Annotations
            _annotations.DrawAnnotations();
        }
        public void ShowModelParts(string[] partNames)
        {
            BasePart part;
            foreach (var name in partNames)
            {
                if (_model.Mesh.Parts.TryGetValue(name, out part))
                {
                    part.Visible = true;
                    _form.UpdateTreeNode(ViewGeometryModelResults.Model, name, part, null, false);
                }
            }
            _form.ShowActors(partNames, false);
            //
            AnnotateWithColorLegend();
            //
            FeModelUpdate(UpdateType.RedrawSymbols);
            // Annotations
            _annotations.DrawAnnotations();
        }
        public void SetTransparencyForModelParts(string[] partNames, byte alpha)
        {
            BasePart part;
            foreach (var name in partNames)
            {
                part = _model.Mesh.Parts[name];
                part.Color = Color.FromArgb(alpha, part.Color);
                _form.UpdateActor(name, name, part.Color);
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, name, part, null, false);
            }
        }
        public void ReplaceModelPartProperties(string oldPartName, PartProperties newPartProperties)
        {
            // Replace mesh part
            MeshPart meshPart = GetModelPart(oldPartName);
            meshPart.SetProperties(newPartProperties);
            _model.Mesh.Parts.Replace(oldPartName, meshPart.Name, meshPart);
            // Update
            _form.UpdateActor(oldPartName, meshPart.Name, meshPart.Color);
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldPartName, meshPart, null);
            //
            if (_model.Geometry != null && _model.Geometry.Parts.ContainsKey(oldPartName))
            {
                // Save element type enum
                GeometryPart geomPart = GetGeometryPart(oldPartName);
                geomPart.AddElementTypeEnums(meshPart.GetElementTypeEnums());
                // Recolor the geometry part in pair
                geomPart.Color = meshPart.Color;
                // Rename the geometry part in pair
                if (oldPartName != newPartProperties.Name)
                {
                    string newPartName = meshPart.Name;
                    geomPart.Name = newPartName;
                    _model.Geometry.Parts.Replace(oldPartName, geomPart.Name, geomPart);
                    // Rename compound sub-part names array
                    foreach (var entry in _model.Geometry.Parts)
                    {
                        if (entry.Value is CompoundGeometryPart cgp)
                        {
                            for (int i = 0; i < cgp.SubPartNames.Length; i++)
                            {
                                if (cgp.SubPartNames[i] == oldPartName)
                                {
                                    cgp.SubPartNames[i] = newPartProperties.Name;
                                    break;
                                }
                            }
                            if (cgp.CreatedFromPartNames != null)
                            {
                                for (int i = 0; i < cgp.CreatedFromPartNames.Length; i++)
                                {
                                    if (cgp.CreatedFromPartNames[i] == oldPartName)
                                    {
                                        cgp.CreatedFromPartNames[i] = newPartProperties.Name;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                // Update
                _form.UpdateTreeNode(ViewGeometryModelResults.Geometry, oldPartName, geomPart, null);
            }
            //
            AnnotateWithColorLegend();
            //
            FeModelUpdate(UpdateType.Check);
        }
        // Transform
        public void TranslateModelParts(string[] partNames, double[] translateVector, bool copy)
        {
            if (!copy) ChangeAllSelectionsToIdSelections(partNames);
            //
            string[] translatedPartNames = _model.Mesh.TranslateParts(partNames, translateVector, copy,
                                                                      _model.GetReservedPartNames());
            //
            if (copy)
            {
                foreach (var partName in translatedPartNames)
                {
                    _form.AddTreeNode(ViewGeometryModelResults.Model, _model.Mesh.Parts[partName], null);
                }
            }
            //
            if (IsExplodedViewActive()) UpdateExplodedView(false);
            //
            FeModelUpdate(UpdateType.DrawModel | UpdateType.RedrawSymbols);
        }
        public void ScaleModelParts(string[] partNames, double[] scaleCenter, double[] scaleFactors, bool copy)
        {
            if (IsExplodedViewActive()) throw new CaeException("The scaling can only be done when the exploded view is turned off.");
            //
            if (!copy) ChangeAllSelectionsToIdSelections(partNames);
            //
            string[] scaledPartNames = _model.Mesh.ScaleParts(partNames, scaleCenter, scaleFactors, copy,
                                                              _model.GetReservedPartNames());
            if (copy)
            {
                foreach (var partName in scaledPartNames)
                {
                    _form.AddTreeNode(ViewGeometryModelResults.Model, _model.Mesh.Parts[partName], null);
                }
            }
            //
            if (IsExplodedViewActive()) UpdateExplodedView(false);
            //
            FeModelUpdate(UpdateType.DrawModel | UpdateType.RedrawSymbols);
        }
        public void RotateModelParts(string[] partNames, double[] rotateCenter, double[] rotateAxis, double rotateAngle, bool copy)
        {
            if (!copy) ChangeAllSelectionsToIdSelections(partNames);
            //
            string[] rotatedPartNames = _model.Mesh.RotateParts(partNames, rotateCenter, rotateAxis, rotateAngle, copy,
                                                                _model.GetReservedPartNames());
            if (copy)
            {
                foreach (var partName in rotatedPartNames)
                {
                    _form.AddTreeNode(ViewGeometryModelResults.Model, _model.Mesh.Parts[partName], null);
                }
            }
            //
            if (IsExplodedViewActive()) UpdateExplodedView(false);
            //
            FeModelUpdate(UpdateType.DrawModel | UpdateType.RedrawSymbols);
        }
        //
        public bool AreModelPartsMergeable(string[] partNames)
        {
            return _model.Mesh.ArePartsMergeable(partNames);
        }
        public void MergeModelParts(string[] partNames)
        {
            MeshPart newMeshPart;
            string[] mergedPartNames;
            //
            ExplodedViewParameters parameters = _explodedViews.GetCurrentExplodedViewParameters().DeepClone();
            RemoveExplodedView(false);
            // Remove annotations
            _annotations.RemoveCurrentArrowAnnotationsByParts(partNames, ViewGeometryModelResults.Model);
            //
            _model.Mesh.MergeMeshParts(partNames, out newMeshPart, out mergedPartNames);
            ApplyExplodedView(parameters, null, false);
            //
            if (newMeshPart != null && mergedPartNames != null)
            {
                foreach (var partName in mergedPartNames)
                {
                    _form.RemoveTreeNode<MeshPart>(ViewGeometryModelResults.Model, partName, null);
                }
                //
                _form.AddTreeNode(ViewGeometryModelResults.Model, newMeshPart, null);
                //
                AnnotateWithColorLegend();
                //
                FeModelUpdate(UpdateType.Check | UpdateType.DrawModel | UpdateType.RedrawSymbols);
            }
        }
        public int[] RemoveModelParts(string[] partNames, bool invalidate, bool removeForRemeshing)
        {
            int[] removedPartIds = null;
            if (_model.Mesh != null)
            {
                ViewGeometryModelResults view = ViewGeometryModelResults.Model;
                // Remove annotations
                _annotations.RemoveCurrentArrowAnnotationsByParts(partNames, view);
                //
                string[] removedParts;
                removedPartIds = _model.Mesh.RemoveParts(partNames, out removedParts, removeForRemeshing);
                //
                foreach (var name in removedParts) _form.RemoveTreeNode<MeshPart>(view, name, null);
            }
            //
            UpdateType ut = UpdateType.Check;
            if (invalidate)
            {
                ut |= UpdateType.DrawModel | UpdateType.RedrawSymbols;
                //
                AnnotateWithColorLegend();
            }
            FeModelUpdate(ut);
            //
            return removedPartIds;
        }
        //
        private void ChangeAllSelectionsToIdSelections(string[] partNames)
        {
            MeshPart[] parts = new MeshPart[partNames.Length];
            for (int i = 0; i < partNames.Length; i++)
            {
                parts[i] = _model.Mesh.Parts[partNames[i]] as MeshPart;
            }
            ChangeSelectedNodeSetsToIds(partNames, parts);
            ChangeSelectedElementSetsToIds(partNames, parts);
            ChangeSelectedSurfacesToIds(partNames, parts);
        }

        #endregion #################################################################################################################

        #region Node set   #########################################################################################################
        // COMMANDS ********************************************************************************
        public void AddNodeSetCommand(FeNodeSet nodeSet)
        {
            Commands.CAddNodeSet comm = new Commands.CAddNodeSet(nodeSet);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceNodeSetCommand(string oldNodeSetName, FeNodeSet newNodeSet)
        {
            Commands.CReplaceNodeSet comm = new Commands.CReplaceNodeSet(oldNodeSetName, newNodeSet);
            _commands.AddAndExecute(comm);
        }
        public void DuplicateNodeSetsCommand(string[] nodeSetNames)
        {
            Commands.CDuplicateNodeSets comm = new Commands.CDuplicateNodeSets(nodeSetNames);
            _commands.AddAndExecute(comm);
        }
        public void RemoveNodeSetsCommand(string[] nodeSetNames)
        {
            Commands.CRemoveNodeSets comm = new Commands.CRemoveNodeSets(nodeSetNames);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public string[] GetAllNodeSetNames()
        {
            return _model.Mesh.NodeSets.Keys.ToArray();
        }
        public string[] GetUserNodeSetNames()
        {
            if (_model.Mesh != null)
            {
                List<string> userNodeSetNames = new List<string>();
                foreach (var entry in _model.Mesh.NodeSets)
                {
                    if (!entry.Value.Internal) userNodeSetNames.Add(entry.Key);
                }
                return userNodeSetNames.ToArray();
            }
            else return null;
        }
        public void AddNodeSet(FeNodeSet nodeSet, bool update = true)
        {
            // In order for the Regenerate history to work perform the selection
            if (nodeSet.CreationData != null) ReselectNodeSet(nodeSet);
            //
            _model.Mesh.UpdateNodeSetCenterOfGravity(nodeSet);
            //
            _model.Mesh.NodeSets.Add(nodeSet.Name, nodeSet);
            //
            _form.AddTreeNode(ViewGeometryModelResults.Model, nodeSet, null);
            //
            UpdateSurfacesBasedOnNodeSet(nodeSet.Name);
            UpdateReferencePointsBasedOnNodeSet(nodeSet.Name);
            //
            if (update) FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public FeNodeSet GetNodeSet(string nodeSetName)
        {
            _model.Mesh.NodeSets.TryGetValue(nodeSetName, out FeNodeSet nodeSet);
            return nodeSet;
        }
        public FeNodeSet[] GetUserNodeSets()
        {
            if (_model.Mesh != null)
            {
                List<FeNodeSet> userNodeSets = new List<FeNodeSet>();
                foreach (var entry in _model.Mesh.NodeSets)
                {
                    if (!entry.Value.Internal) userNodeSets.Add(entry.Value);
                }
                return userNodeSets.ToArray();
            }
            else return null;
        }
        public void ReplaceNodeSet(string oldNodeSetName, FeNodeSet nodeSet, bool feModelUpdate)
        {
            // In order for the Regenerate history to work perform the selection
            if (nodeSet.CreationData != null) ReselectNodeSet(nodeSet);
            else throw new NotSupportedException("The node set does not contain any selection data.");
            //
            _model.Mesh.UpdateNodeSetCenterOfGravity(nodeSet);
            //
            _model.Mesh.NodeSets.Replace(oldNodeSetName, nodeSet.Name, nodeSet);
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldNodeSetName, nodeSet, null, feModelUpdate);
            //
            UpdateSurfacesBasedOnNodeSet(nodeSet.Name);
            UpdateReferencePointsBasedOnNodeSet(nodeSet.Name);
            //
            if (feModelUpdate) FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void DuplicateNodeSets(string[] nodeSetNames)
        {
            FeNodeSet newNodeSet;
            foreach (var name in nodeSetNames)
            {
                newNodeSet = _model.Mesh.NodeSets[name].DeepClone();
                newNodeSet.Name = NamedClass.GetNameWithoutLastValue(newNodeSet.Name);
                newNodeSet.Name = GetAllMeshEntityNames().GetNextNumberedKey(newNodeSet.Name);
                AddNodeSet(newNodeSet);
            }
        }
        public void RemoveNodeSets(string[] nodeSetNames)
        {
            FeNodeSet nodeSet;
            foreach (var name in nodeSetNames)
            {
                if (_model.Mesh.NodeSets.TryRemove(name, out nodeSet))
                {
                    if (!nodeSet.Internal) _form.RemoveTreeNode<FeNodeSet>(ViewGeometryModelResults.Model, name, null);
                    UpdateSurfacesBasedOnNodeSet(name);
                    UpdateReferencePointsBasedOnNodeSet(name);
                }
            }
            //
            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void GetNodesCenterOfGravity(FeNodeSet nodeSet)
        {
            _model.Mesh.UpdateNodeSetCenterOfGravity(nodeSet);
        }
        //
        private void ReselectNodeSet(FeNodeSet nodeSet)
        {
            _selection = nodeSet.CreationData.DeepClone();
            //
            if (_selection.SelectItem == vtkSelectItem.Node)
            {
                nodeSet.Labels = GetSelectionIds();
            }
            else if (_selection.SelectItem == vtkSelectItem.Geometry)
            {
                if (_selection.CurrentView == (int)ViewGeometryModelResults.Model)
                {
                    nodeSet.CreationIds = GetSelectionIds();
                    nodeSet.Labels = _model.Mesh.GetIdsFromGeometryIds(nodeSet.CreationIds, vtkSelectItem.Node);
                }
                else throw new NotSupportedException();
            }
            else throw new NotSupportedException();
            //
            _selection.Clear();
        }
        private void UpdateNodeSetsBasedOnGeometry(bool feModelUpdate)
        {
            // Use list not to throw collection modified exception
            List<FeNodeSet> geomNodeSets = new List<FeNodeSet>();
            if (_model != null && _model.Mesh != null)
            {
                foreach (var entry in _model.Mesh.NodeSets)
                {
                    if (entry.Value.CreationData != null && entry.Value.CreationData.IsGeometryBased())
                        geomNodeSets.Add(entry.Value);
                }
                //
                foreach (FeNodeSet nodeSet in geomNodeSets)
                {
                    nodeSet.Valid = true;
                    ReplaceNodeSet(nodeSet.Name, nodeSet, feModelUpdate);
                }
            }
        }
        //
        private void ChangeSelectedNodeSetsToIds(string[] partNames, MeshPart[] parts)
        {
            List<int> geometryIds = new List<int>();
            HashSet<int> nodeIds = new HashSet<int>();
            FeNodeSet nodeSet;
            SelectionNodeIds selectionNodeIds;
            //
            foreach (var entry in _model.Mesh.NodeSets)
            {
                nodeSet = entry.Value;
                if (nodeSet.Internal && nodeSet.CreationData == null) continue; // a surface node set - no need to update
                                                                                // skip last two lines
                if (nodeSet.CreationData != null && nodeSet.CreationData.IsGeometryBased())
                {
                    // Only mouse and geometry ids
                    geometryIds.Clear();
                    foreach (var node in nodeSet.CreationData.Nodes)
                    {
                        if (node is SelectionNodeMouse snm)
                        {
                            _selection.SelectItem = nodeSet.CreationData.SelectItem;
                            geometryIds.AddRange(GetIdsFromSelectionNodeMouse(snm, true));
                        }
                        else if (node is SelectionNodeIds sni) geometryIds.AddRange(sni.ItemIds);
                    }
                    string[] nodeSetPartNames = _model.Mesh.GetPartNamesFromGeometryIds(geometryIds.ToArray());
                    if (partNames.Intersect(nodeSetPartNames).Count() > 0)
                    {
                        selectionNodeIds = new SelectionNodeIds(vtkSelectOperation.None, false, geometryIds.ToArray());
                        selectionNodeIds.GeometryIds = true;
                    }
                    else continue;  // skip last two lines
                }
                else
                {
                    nodeIds.Clear();
                    for (int i = 0; i < parts.Length; i++) nodeIds.UnionWith(parts[i].NodeLabels);
                    if (nodeIds.Intersect(nodeSet.Labels).Count() > 0)
                    {
                        selectionNodeIds = new SelectionNodeIds(vtkSelectOperation.None, false, nodeSet.Labels);
                        if (nodeSet.CreationData == null) nodeSet.CreationData = new Selection();
                    }
                    else continue;  // skip last two lines
                }
                //
                nodeSet.CreationData.Clear();
                nodeSet.CreationData.Add(selectionNodeIds);
            }
        }

        #endregion #################################################################################################################

        #region Element set   ######################################################################################################
        // COMMANDS ********************************************************************************
        public void AddElementSetCommand(FeElementSet elementSet)
        {
            Commands.CAddElementSet comm = new Commands.CAddElementSet(elementSet);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceElementSetCommand(string oldElementSetName, FeElementSet newElementSet)
        {
            Commands.CReplaceElementSet comm = new Commands.CReplaceElementSet(oldElementSetName, newElementSet);
            _commands.AddAndExecute(comm);
        }
        public void DuplicateElementSetsCommand(string[] elementSetNames)
        {
            Commands.CDuplicateElementSets comm = new Commands.CDuplicateElementSets(elementSetNames);
            _commands.AddAndExecute(comm);
        }
        public void ConvertElementSetsToMeshPartsCommand(string[] elementSetNames)
        {
            Commands.CConvertElementSetsToMeshParts comm = new Commands.CConvertElementSetsToMeshParts(elementSetNames);
            _commands.AddAndExecute(comm);
        }
        public void RemoveElementSetsCommand(string[] elementSetNames)
        {
            Commands.CRemoveElementSets comm = new Commands.CRemoveElementSets(elementSetNames);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public string[] GetAllElementSetNames()
        {
            return _model.Mesh.ElementSets.Keys.ToArray();
        }
        public string[] GetUserElementSetNames()
        {
            if (_model.Mesh != null)
            {
                List<string> userElementSetNames = new List<string>();
                foreach (var entry in _model.Mesh.ElementSets)
                {
                    if (!entry.Value.Internal) userElementSetNames.Add(entry.Key);
                }
                return userElementSetNames.ToArray();
            }
            else return null;
        }
        public string[] GetUserElementSetNames<T>()
        {
            throw new NotSupportedException("All elements must be checked.");
            //
            List<string> userElementSetNames = new List<string>();
            foreach (var entry in _model.Mesh.ElementSets)
            {
                if (!entry.Value.Internal && entry.Value.Labels.Length > 0 && _model.Mesh.Elements[entry.Value.Labels[0]] is T)
                    userElementSetNames.Add(entry.Key);
            }
            return userElementSetNames.ToArray();
        }
        public void AddElementSet(FeElementSet elementSet)
        {
            // In order for the Regenerate history to work perform the selection again
            if (elementSet.CreationData != null) ReselectElementSet(elementSet);
            //
            _model.Mesh.ElementSets.Add(elementSet.Name, elementSet);
            //
            if (!elementSet.Internal)   // needed for remeshing
            {
                _form.AddTreeNode(ViewGeometryModelResults.Model, elementSet, null);
                //
                FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
            }
        }
        public FeElementSet GetElementSet(string elementSetName)
        {
            _model.Mesh.ElementSets.TryGetValue(elementSetName, out FeElementSet elementSet);
            return elementSet;
        }
        public FeElementSet[] GetUserElementSets()
        {
            if (_model.Mesh != null)
            {
                List<FeElementSet> userElementSets = new List<FeElementSet>();
                foreach (var entry in _model.Mesh.ElementSets)
                {
                    if (!entry.Value.Internal) userElementSets.Add(entry.Value);
                }
                return userElementSets.ToArray();
            }
            else return null;
        }
        public void ReplaceElementSet(string oldElementSetName, FeElementSet elementSet, bool feModelUpdate)
        {
            // In order for the Regenerate history to work perform the selection again
            if (elementSet.CreationData != null) ReselectElementSet(elementSet);
            else throw new NotSupportedException("The element set does not contain any selection data.");
            //
            _model.Mesh.ElementSets.Replace(oldElementSetName, elementSet.Name, elementSet);
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldElementSetName, elementSet, null, feModelUpdate);
            //
            if (feModelUpdate) FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void DuplicateElementSets(string[] elementSetNames)
        {
            FeElementSet newElementSet;
            foreach (var name in elementSetNames)
            {
                newElementSet = _model.Mesh.ElementSets[name].DeepClone();
                newElementSet.Name = NamedClass.GetNameWithoutLastValue(newElementSet.Name);
                newElementSet.Name = GetAllMeshEntityNames().GetNextNumberedKey(newElementSet.Name);
                AddElementSet(newElementSet);
            }
        }
        public void ConvertElementSetsToMeshParts(string[] elementSetNames)
        {
            BasePart[] modifiedParts;
            BasePart[] newParts;
            //
            ExplodedViewParameters parameters = _explodedViews.GetCurrentExplodedViewParameters().DeepClone();
            RemoveExplodedView(false);  // cannot suppress exploded view since new parts are created
            _model.Mesh.CreatePartsFromElementSets(elementSetNames, out modifiedParts, out newParts);
            ApplyExplodedView(parameters, null, false);
            // Remove annotations
            _annotations.RemoveCurrentArrowAnnotationsByParts(modifiedParts, ViewGeometryModelResults.Model);
            //
            foreach (var part in modifiedParts)
            {
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, part.Name, part, null);
            }
            // Add new parts
            foreach (var part in newParts)
            {
                _form.AddTreeNode(ViewGeometryModelResults.Model, part, null);
            }
            // Remove element sets
            foreach (var elementSetName in elementSetNames)
            {
                _form.RemoveTreeNode<FeElementSet>(ViewGeometryModelResults.Model, elementSetName, null);
            }
            //
            AnnotateWithColorLegend();
            //
            FeModelUpdate(UpdateType.Check | UpdateType.DrawModel | UpdateType.RedrawSymbols);
        }
        public void RemoveElementSets(string[] elementSetNames)
        {
            FeElementSet elementSet;
            bool update = false;    // needed for remeshing
            //
            foreach (var name in elementSetNames)
            {
                if (_model.Mesh.ElementSets.TryRemove(name, out elementSet) && !elementSet.Internal)
                {

                    _form.RemoveTreeNode<FeElementSet>(ViewGeometryModelResults.Model, name, null);
                    update = true;
                }
            }
            //
            if (update) FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        //
        private void ReselectElementSet(FeElementSet elementSet)
        {
            _selection = elementSet.CreationData.DeepClone();
            //
            if (_selection.SelectItem == vtkSelectItem.Element || _selection.SelectItem == vtkSelectItem.Part)
            {
                elementSet.Labels = GetSelectionIds();
            }
            else if (_selection.SelectItem == vtkSelectItem.Geometry)
            {
                if (_selection.CurrentView == (int)ViewGeometryModelResults.Model)
                {
                    elementSet.CreationIds = GetSelectionIds();
                    elementSet.Labels = _model.Mesh.GetIdsFromGeometryIds(elementSet.CreationIds, vtkSelectItem.Element);
                }
                else throw new NotSupportedException();
            }
            else throw new NotSupportedException();
            //
            _selection.Clear();
        }
        private void UpdateElementSetsBasedOnGeometry(bool feModelUpdate)
        {
            // Use list not to throw collection modified exception
            List<FeElementSet> geomElementSets = new List<FeElementSet>();
            if (_model != null && _model.Mesh != null)
            {
                foreach (var entry in _model.Mesh.ElementSets)
                {
                    if (entry.Value.CreationData != null && entry.Value.CreationData.IsGeometryBased())
                        geomElementSets.Add(entry.Value);
                }
                //
                foreach (FeElementSet elementSet in geomElementSets)
                {
                    elementSet.Valid = true;
                    ReplaceElementSet(elementSet.Name, elementSet, feModelUpdate);
                }
            }
        }
        //
        private void ChangeSelectedElementSetsToIds(string[] partNames, MeshPart[] parts)
        {
            List<int> geometryIds = new List<int>();
            HashSet<int> elementIds = new HashSet<int>();
            FeElementSet elementSet;
            SelectionNodeIds selectionNodeIds;
            //
            foreach (var entry in _model.Mesh.ElementSets)
            {
                elementSet = entry.Value;
                if (elementSet.Internal || elementSet.CreationData == null) continue;   // skip last two lines
                //
                if (elementSet.CreationData != null && elementSet.CreationData.IsGeometryBased())
                {
                    // Only mouse and geometry ids
                    geometryIds.Clear();
                    foreach (var node in elementSet.CreationData.Nodes)
                    {
                        if (node is SelectionNodeMouse snm)
                        {
                            _selection.SelectItem = elementSet.CreationData.SelectItem;
                            int[] ids = GetIdsFromSelectionNodeMouse(snm, true);
                            if (ids != null) geometryIds.AddRange(ids);
                        }
                        else if (node is SelectionNodeIds sni) geometryIds.AddRange(sni.ItemIds);
                    }
                    string[] elementSetPartNames = _model.Mesh.GetPartNamesFromGeometryIds(geometryIds.ToArray());
                    if (partNames.Intersect(elementSetPartNames).Count() > 0)
                    {
                        selectionNodeIds = new SelectionNodeIds(vtkSelectOperation.None, false, geometryIds.ToArray());
                        selectionNodeIds.GeometryIds = true;
                    }
                    else continue;  // skip last two lines
                }
                else
                {
                    elementIds.Clear();
                    for (int i = 0; i < parts.Length; i++) elementIds.UnionWith(parts[i].Labels);
                    if (elementIds.Intersect(elementSet.Labels).Count() > 0)
                    {
                        selectionNodeIds = new SelectionNodeIds(vtkSelectOperation.None, false, elementSet.Labels);
                        if (elementSet.CreationData == null) elementSet.CreationData = new Selection();
                    }
                    else continue;  // skip last two lines
                }
                //
                elementSet.CreationData.Clear();
                elementSet.CreationData.Add(selectionNodeIds);
            }
        }

        #endregion #################################################################################################################

        #region Surface menu   #####################################################################################################
        // COMMANDS ********************************************************************************
        public void AddSurfaceCommand(FeSurface surface, bool update = true)
        {
            Commands.CAddSurface comm = new Commands.CAddSurface(surface, update);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceSurfaceCommand(string oldSurfaceName, FeSurface newSurface)
        {
            Commands.CReplaceSurface comm = new Commands.CReplaceSurface(oldSurfaceName, newSurface);
            _commands.AddAndExecute(comm);
        }
        public void DuplicateSurfacesCommand(string[] surfaceNames)
        {
            Commands.CDuplicateSurfaces comm = new Commands.CDuplicateSurfaces(surfaceNames);
            _commands.AddAndExecute(comm);
        }
        public void RemoveSurfacesCommand(string[] surfaceNames)
        {
            Commands.CRemoveSurfaces comm = new Commands.CRemoveSurfaces(surfaceNames);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public string[] GetAllSurfaceNames()
        {
            if (_model.Mesh != null) return _model.Mesh.Surfaces.Keys.ToArray();
            else return null;
        }
        public string[] GetUserSurfaceNames()
        {
            if (_model.Mesh != null)
            {
                List<string> userSurfaceNames = new List<string>();
                foreach (var entry in _model.Mesh.Surfaces)
                {
                    if (!entry.Value.Internal) userSurfaceNames.Add(entry.Key);
                }
                return userSurfaceNames.ToArray();
            }
            else return null;
        }
        public string[] GetUserSurfaceNames(FeSurfaceType surfaceType)
        {
            List<string> surfaceNames = new List<string>();
            if (_model.Mesh != null)
            {
                foreach (var entry in _model.Mesh.Surfaces)
                {
                    if (!entry.Value.Internal && entry.Value.Type == surfaceType) surfaceNames.Add(entry.Key);
                }
                return surfaceNames.ToArray();
            }
            else return null;
        }
        public string[] GetUserSurfaceNames(FeSurfaceType surfaceType, FeSurfaceFaceTypes surfaceFaceTypes)
        {
            List<string> surfaceNames = new List<string>();
            if (_model.Mesh != null)
            {
                foreach (var entry in _model.Mesh.Surfaces)
                {
                    if (!entry.Value.Internal && entry.Value.Type == surfaceType &&
                        entry.Value.SurfaceFaceTypes == surfaceFaceTypes) surfaceNames.Add(entry.Key);
                }
                return surfaceNames.ToArray();
            }
            else return null;
        }
        public void AddSurface(FeSurface surface, bool update = true)
        {
            if (surface.CreatedFrom == FeSurfaceCreatedFrom.Selection)
            {
                // In order for the Regenerate history to work perform the selection
                _selection = surface.CreationData.DeepClone();
                surface.FaceIds = GetSelectionIds();
                _selection.Clear();
            }
            //
            AddSurfaceAndElementFaces(surface);
            //
            _form.AddTreeNode(ViewGeometryModelResults.Model, surface, null);
            //
            UpdateReferencePointsBasedOnSurface(surface.Name);
            //
            if (update) FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public FeSurface GetSurface(string surfaceName)
        {
            return _model.Mesh.Surfaces[surfaceName];
        }
        public FeNodeSet GetSurfaceNodeSet(string surfaceName)
        {
            return _model.Mesh.GetSurfaceNodeSet(surfaceName);
        }
        public FeSurface[] GetUserSurfaces()
        {
            if (_model.Mesh != null)
            {
                List<FeSurface> userSurfaces = new List<FeSurface>();
                foreach (var entry in _model.Mesh.Surfaces)
                {
                    if (!entry.Value.Internal) userSurfaces.Add(entry.Value);
                }
                return userSurfaces.ToArray();
            }
            else return null;
        }
        public void ReplaceSurface(string oldSurfaceName, FeSurface surface, bool feModelUpdate)
        {
            List<string> keys = _model.Mesh.Surfaces.Keys.ToList();     // copy
            RemoveSurfaceAndElementFacesFromModel(new string[] { oldSurfaceName });
            //
            if (surface.CreatedFrom == FeSurfaceCreatedFrom.Selection)
            {
                // In order for the Regenerate history to work perform the selection
                _selection = surface.CreationData.DeepClone();
                surface.FaceIds = GetSelectionIds();
                _selection.Clear();
            }
            //
            AddSurfaceAndElementFaces(surface);
            //
            int index = keys.IndexOf(oldSurfaceName);
            keys.RemoveAt(index);
            keys.Insert(index, surface.Name);
            _model.Mesh.Surfaces.SortKeysAs(keys);
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldSurfaceName, surface, null, feModelUpdate);
            //
            UpdateReferencePointsBasedOnSurface(surface.Name);
            //
            if (feModelUpdate) FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void DuplicateSurfaces(string[] surfaceNames)
        {
            FeSurface newSurface;
            foreach (var name in surfaceNames)
            {
                newSurface = _model.Mesh.Surfaces[name].DeepClone();
                newSurface.Name = NamedClass.GetNameWithoutLastValue(newSurface.Name);
                newSurface.Name = _model.Mesh.Surfaces.GetNextNumberedKey(newSurface.Name);
                if (newSurface.CreationData != null) newSurface.CreatedFrom = FeSurfaceCreatedFrom.Selection;
                AddSurface(newSurface);
            }
        }
        public void RemoveSurfaces(string[] surfaceNames, bool update = true)
        {
            FeSurface[] removedSurfaces = RemoveSurfaceAndElementFacesFromModel(surfaceNames);
            //
            foreach (var surface in removedSurfaces)
            {
                if (!surface.Internal) _form.RemoveTreeNode<FeSurface>(ViewGeometryModelResults.Model, surface.Name, null);
                //
                UpdateReferencePointsBasedOnSurface(surface.Name);
            }
            //
            if (update) FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        private void AddSurfaceAndElementFaces(FeSurface surface)
        {
            _model.Mesh.CreateSurfaceItems(surface);    // create faces or node set
            if (surface.ElementFaces != null)
            {
                foreach (var entry in surface.ElementFaces)
                {
                    // this is now shown - debugging
                    _form.AddTreeNode(ViewGeometryModelResults.Model, _model.Mesh.ElementSets[entry.Value], null);
                }
            }
            _model.Mesh.Surfaces.Add(surface.Name, surface);
        }
        private FeSurface[] RemoveSurfaceAndElementFacesFromModel(string[] surfaceNames)
        {
            return _model.Mesh.RemoveSurfaces(surfaceNames, out _, out _);
        }
        private int[] GetVisibleFaceIds()
        {
            return DisplayedMesh.GetVisibleVisualizationFaceIds();
        }
        // Update
        public void UpdateSurfaceArea(FeSurface surface)
        {
            _model.Mesh.UpdateSurfaceArea(surface);
        }
        private void UpdateSurfacesBasedOnNodeSet(string nodeSetName)
        {
            // use list not to throw collection modified
            List<FeSurface> changedSurfaces = new List<FeSurface>();
            if (_model != null && _model.Mesh != null)
            {
                foreach (var entry in _model.Mesh.Surfaces)
                {
                    if (entry.Value.CreatedFrom == FeSurfaceCreatedFrom.NodeSet &&
                        entry.Value.CreatedFromNodeSetName == nodeSetName)
                    {
                        changedSurfaces.Add(entry.Value);
                    }
                }
                if (changedSurfaces.Count > 0)
                {
                    foreach (FeSurface surface in changedSurfaces) ReplaceSurface(surface.Name, surface, false);
                }
            }
        }
        private void UpdateSurfacesBasedOnGeometry(bool feModelUpdate)
        {
            // Use list not to throw collection modified exception
            List<FeSurface> geomSurfaces = new List<FeSurface>();
            if (_model != null && _model.Mesh != null)
            {
                foreach (var entry in _model.Mesh.Surfaces)
                {
                    if (entry.Value.CreationData != null && entry.Value.CreationData.IsGeometryBased())
                        geomSurfaces.Add(entry.Value);
                }
                //
                foreach (FeSurface surface in geomSurfaces)
                {
                    surface.Valid = true;
                    ReplaceSurface(surface.Name, surface, feModelUpdate);
                }
            }
        }
        //
        private void ChangeSelectedSurfacesToIds(string[] partNames, MeshPart[] parts)
        {
            List<int> geometryIds = new List<int>();
            HashSet<int> nodeIds = new HashSet<int>();
            FeSurface surface;
            SelectionNodeIds selectionNodeIds;
            //
            foreach (var entry in _model.Mesh.Surfaces)
            {
                surface = entry.Value;
                //
                try
                {
                    if (surface.CreationData == null) continue;
                    //
                    if (surface.CreationData.IsGeometryBased())
                    {
                        // Only mouse and geometry ids
                        geometryIds.Clear();
                        foreach (var node in surface.CreationData.Nodes)
                        {
                            if (node is SelectionNodeMouse snm)
                            {
                                _selection.SelectItem = surface.CreationData.SelectItem;
                                geometryIds.AddRange(GetIdsFromSelectionNodeMouse(snm, true));
                            }
                            else if (node is SelectionNodeIds sni) geometryIds.AddRange(sni.ItemIds);
                        }
                        string[] surfacePartNames = _model.Mesh.GetPartNamesFromGeometryIds(geometryIds.ToArray());
                        if (partNames.Intersect(surfacePartNames).Count() > 0)
                        {
                            selectionNodeIds = new SelectionNodeIds(vtkSelectOperation.None, false, geometryIds.ToArray());
                            selectionNodeIds.GeometryIds = true;
                        }
                        else continue;
                    }
                    else
                    {
                        nodeIds.Clear();
                        for (int i = 0; i < parts.Length; i++) nodeIds.UnionWith(parts[i].Labels);
                        if (nodeIds.Intersect(_model.Mesh.NodeSets[surface.NodeSetName].Labels).Count() > 0)
                        {
                            selectionNodeIds = new SelectionNodeIds(vtkSelectOperation.None, false, surface.FaceIds);
                            if (surface.CreationData == null) surface.CreationData = new Selection();
                        }
                        else continue;
                    }
                    //
                    surface.CreationData.Clear();
                    surface.CreationData.Add(selectionNodeIds);
                }
                catch
                {
                    surface.Valid = false;
                }
            }
        }

        #endregion #################################################################################################################

        #region Reference point menu   #############################################################################################
        // COMMANDS ********************************************************************************
        public void AddReferencePointCommand(FeReferencePoint referencePoint)
        {
            Commands.CAddReferencePoint comm = new Commands.CAddReferencePoint(referencePoint);
            _commands.AddAndExecute(comm);
        }
        public void HideReferencePointsCommand(string[] referencePointNames)
        {
            Commands.CHideReferencePoints comm = new Commands.CHideReferencePoints(referencePointNames);
            _commands.AddAndExecute(comm);
        }
        public void ShowReferencePointsCommand(string[] referencePointNames)
        {
            Commands.CShowReferencePoints comm = new Commands.CShowReferencePoints(referencePointNames);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceReferencePointCommand(string oldReferencePointName, FeReferencePoint newReferencePoint)
        {
            Commands.CReplaceReferencePoint comm = new Commands.CReplaceReferencePoint(oldReferencePointName, newReferencePoint);
            _commands.AddAndExecute(comm);
        }
        public void DuplicateReferencePointsCommand(string[] referencePointNames)
        {
            Commands.CDuplicateReferencePoints comm = new Commands.CDuplicateReferencePoints(referencePointNames);
            _commands.AddAndExecute(comm);
        }
        public void RemoveReferencePointsCommand(string[] referencePointNames)
        {
            Commands.CRemoveReferencePoints comm = new Commands.CRemoveReferencePoints(referencePointNames);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public string[] GetReferencePointNames()
        {
            if (_model.Mesh != null) return _model.Mesh.ReferencePoints.Keys.ToArray();
            else return null;
        }
        public void AddReferencePoint(FeReferencePoint referencePoint)
        {
            UpdateReferencePoint(referencePoint);
            //
            _model.Mesh.ReferencePoints.Add(referencePoint.Name, referencePoint);
            //
            _form.AddTreeNode(ViewGeometryModelResults.Model, referencePoint, null);
            //
            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public FeReferencePoint GetReferencePoint(string referencePointName)
        {
            return _model.Mesh.ReferencePoints[referencePointName];
        }
        public FeReferencePoint[] GetAllReferencePoints()
        {
            if (_model.Mesh == null) return null;
            return _model.Mesh.ReferencePoints.Values.ToArray();
        }
        public void HideReferencePoints(string[] referencePointNames)
        {
            foreach (var name in referencePointNames)
            {
                _model.Mesh.ReferencePoints[name].Visible = false;
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, name, _model.Mesh.ReferencePoints[name], null, false);
            }
            //
            FeModelUpdate(UpdateType.RedrawSymbols);
        }
        public void ShowReferencePoints(string[] referencePointNames)
        {
            foreach (var name in referencePointNames)
            {
                _model.Mesh.ReferencePoints[name].Visible = true;
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, name, _model.Mesh.ReferencePoints[name], null, false);
            }
            //
            FeModelUpdate(UpdateType.RedrawSymbols);
        }
        public void ReplaceReferencePoint(string oldReferencePointName, FeReferencePoint newReferencePoint)
        {
            _model.Mesh.ReferencePoints.Replace(oldReferencePointName, newReferencePoint.Name, newReferencePoint);
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldReferencePointName, newReferencePoint, null);
            //
            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void DuplicateReferencePoints(string[] referencePointNames)
        {
            FeReferencePoint newReferencePint;
            foreach (var name in referencePointNames)
            {
                newReferencePint = _model.Mesh.ReferencePoints[name].DeepClone();
                newReferencePint.Name = NamedClass.GetNameWithoutLastValue(newReferencePint.Name);
                newReferencePint.Name = _model.Mesh.ReferencePoints.GetNextNumberedKey(newReferencePint.Name);
                if (newReferencePint.CreationData != null) newReferencePint.RegionType = RegionTypeEnum.Selection;
                AddReferencePoint(newReferencePint);
            }
        }
        public void RemoveReferencePoints(string[] referencePointNames)
        {
            foreach (var name in referencePointNames)
            {
                _model.Mesh.ReferencePoints.Remove(name);
                _form.RemoveTreeNode<FeReferencePoint>(ViewGeometryModelResults.Model, name, null);
            }

            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        //
        public void UpdateReferencePoint(FeReferencePoint referencePoint)
        {
            _model.Mesh.UpdateReferencePoint(referencePoint);
        }
        private void UpdateReferencePointsBasedOnNodeSet(string nodeSetName)
        {
            if (_model != null && _model.Mesh != null)
            {
                foreach (var entry in _model.Mesh.ReferencePoints)
                {
                    if (entry.Value.RegionType == RegionTypeEnum.NodeSetName && entry.Value.RegionName == nodeSetName)
                    {
                        UpdateReferencePoint(entry.Value);
                    }
                }
            }
        }
        private void UpdateReferencePointsBasedOnSurface(string surfaceName)
        {
            if (_model != null && _model.Mesh != null)
            {
                foreach (var entry in _model.Mesh.ReferencePoints)
                {
                    if (entry.Value.RegionType == RegionTypeEnum.SurfaceName && entry.Value.RegionName == surfaceName)
                    {
                        UpdateReferencePoint(entry.Value);
                    }
                }
            }
        }

        #endregion #################################################################################################################

        #region Material menu   ####################################################################################################
        // COMMANDS ********************************************************************************
        public void AddMaterialCommand(Material material)
        {
            Commands.CAddMaterial comm = new Commands.CAddMaterial(material);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceMaterialCommand(string oldMaterialName, Material newMaterial)
        {
            Commands.CReplaceMaterial comm = new Commands.CReplaceMaterial(oldMaterialName, newMaterial);
            _commands.AddAndExecute(comm);
        }
        public void DuplicateMaterialsCommand(string[] materialNames)
        {
            Commands.CDuplicateMaterials comm = new Commands.CDuplicateMaterials(materialNames);
            _commands.AddAndExecute(comm);
        }
        public void RemoveMaterialsCommand(string[] materialNames)
        {
            Commands.CRemoveMaterials comm = new Commands.CRemoveMaterials(materialNames);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public string[] GetMaterialNames()
        {
            return _model.Materials.Keys.ToArray();
        }
        public void AddMaterial(Material material)
        {
            _model.Materials.Add(material.Name, material);
            _form.AddTreeNode(ViewGeometryModelResults.Model, material, null);
            //
            AnnotateWithColorLegend();
            //
            CheckAndUpdateModelValidity();
        }
        public Material GetMaterial(string materialName)
        {
            return _model.Materials[materialName];
        }
        public Material[] GetAllMaterials()
        {
            return _model.Materials.Values.ToArray();
        }
        public void ReplaceMaterial(string oldMaterialName, Material newMaterial)
        {
            _model.Materials.Replace(oldMaterialName, newMaterial.Name, newMaterial);
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldMaterialName, newMaterial, null);
            //
            AnnotateWithColorLegend();
            //
            CheckAndUpdateModelValidity();
        }
        public void DuplicateMaterials(string[] materialNames)
        {
            Material newMaterial;
            foreach (var name in materialNames)
            {
                newMaterial = _model.Materials[name].DeepClone();
                newMaterial.Name = NamedClass.GetNameWithoutLastValue(newMaterial.Name);
                newMaterial.Name = _model.Materials.GetNextNumberedKey(newMaterial.Name);
                AddMaterial(newMaterial);
            }
        }
        public void ExportMaterials(string[] materialNames, string fileName)
        {
            FileInOut.Output.CalculixFileWriter.WriteMaterials(fileName, _model, materialNames);
            //
            _form.WriteDataToOutput("Materials exported to file: " + fileName);
        }
        public void RemoveMaterials(string[] materialNames)
        {
            foreach (var name in materialNames)
            {
                _model.Materials.Remove(name);
                _form.RemoveTreeNode<Material>(ViewGeometryModelResults.Model, name, null);
            }
            //
            AnnotateWithColorLegend();
            //
            CheckAndUpdateModelValidity();
        }
        //
        public string[] GetMaterialLibraryFiles()
        {
            return _settings.General.GetMaterialLibraryFiles();
        }
        public void AddMaterialLibraryFile(string fileNameWithPath)
        {
            _settings.General.AddMaterialLibraryFile(fileNameWithPath);
            _settings.SaveToFile();
        }
        public void RemoveMaterialLibraryFile(string fileNameWithPath)
        {
            _settings.General.RemoveMaterialLibraryFile(fileNameWithPath);
            _settings.SaveToFile();
        }
        public void ClearMaterialLibraryFiles()
        {
            _settings.General.ClearMaterialLibraryFiles();
            _settings.SaveToFile();
        }

        #endregion #################################################################################################################

        #region Section menu   #####################################################################################################
        // COMMANDS ********************************************************************************
        public void AddSectionCommand(Section section)
        {
            CAddSection comm = new CAddSection(section);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceSectionCommand(string oldSectionName, Section newSection)
        {
            CReplaceSection comm = new CReplaceSection(oldSectionName, newSection);
            _commands.AddAndExecute(comm);
        }
        public void DuplicateSectionsCommand(string[] sectionNames)
        {
            CDuplicateSections comm = new CDuplicateSections(sectionNames);
            _commands.AddAndExecute(comm);
        }
        public void RemoveSectionsCommand(string[] sectionNames)
        {
            CRemoveSections comm = new CRemoveSections(sectionNames);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public string[] GetSectionNames()
        {
            return _model.Sections.Keys.ToArray();
        }
        public void AddSection(Section section)
        {
            ConvertSelectionBasedSection(section);
            //
            _model.Sections.Add(section.Name, section);
            _form.AddTreeNode(ViewGeometryModelResults.Model, section, null);
            //
            AnnotateWithColorEnum state = AnnotateWithColorEnum.Materials | AnnotateWithColorEnum.Sections |
                                          AnnotateWithColorEnum.SectionThicknesses;
            if (state.HasFlag(_annotateWithColor)) FeModelUpdate(UpdateType.DrawModel);
            else AnnotateWithColorLegend();
            //
            CheckAndUpdateModelValidity();   // Check the model in both cases: FeModelUpdate and AnnotateWithColorLegend
        }
        public Section GetSection(string sectionName)
        {
            return _model.Sections[sectionName];
        }
        public Section[] GetAllSections()
        {
            return _model.Sections.Values.ToArray();
        }
        public void ReplaceSection(string oldSectionName, Section section)
        {
            DeleteSelectionBasedSectionSets(oldSectionName);
            ConvertSelectionBasedSection(section);
            //
            _model.Sections.Replace(oldSectionName, section.Name, section);
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldSectionName, section, null);
            //
            AnnotateWithColorEnum state = AnnotateWithColorEnum.Materials | AnnotateWithColorEnum.Sections |
                                          AnnotateWithColorEnum.SectionThicknesses;
            if (state.HasFlag(_annotateWithColor)) FeModelUpdate(UpdateType.DrawModel);
            else AnnotateWithColorLegend();
            //
            CheckAndUpdateModelValidity();
        }
        public void DuplicateSections(string[] sectionNames)
        {
            Section newSection;
            foreach (var name in sectionNames)
            {
                newSection = _model.Sections[name].DeepClone();
                newSection.Name = NamedClass.GetNameWithoutLastValue(newSection.Name);
                newSection.Name = _model.Sections.GetNextNumberedKey(newSection.Name);
                if (newSection.CreationData != null) newSection.RegionType = RegionTypeEnum.Selection;
                AddSection(newSection);
            }
        }
        public void RemoveSections(string[] sectionNames)
        {
            foreach (var name in sectionNames)
            {
                DeleteSelectionBasedSectionSets(name);
                _model.Sections.Remove(name);
                _form.RemoveTreeNode<Section>(ViewGeometryModelResults.Model, name, null);
            }
            //
            AnnotateWithColorEnum state = AnnotateWithColorEnum.Materials | AnnotateWithColorEnum.Sections |
                                          AnnotateWithColorEnum.SectionThicknesses;
            if (state.HasFlag(_annotateWithColor)) FeModelUpdate(UpdateType.DrawModel);
            else AnnotateWithColorLegend();
            //
            CheckAndUpdateModelValidity();
        }
        //
        private void ConvertSelectionBasedSection(Section section)
        {
            // Create a named set and convert a selection to a named set
            if (section.RegionType == RegionTypeEnum.Selection)
            {
                string name;
                // Element set output
                if (section is SolidSection || section is ShellSection || section is MembraneSection)
                {
                    name = FeMesh.GetNextFreeSelectionName(_model.Mesh.ElementSets, section.Name);
                    bool createdByPart = section.CreationData != null && (section.CreationData.SelectItem == vtkSelectItem.Part);
                    FeElementSet elementSet = new FeElementSet(name, section.CreationIds, createdByPart);
                    elementSet.CreationData = section.CreationData.DeepClone();
                    elementSet.Internal = true;
                    AddElementSet(elementSet);
                    //
                    section.RegionName = name;
                    section.RegionType = RegionTypeEnum.ElementSetName;
                }
                else throw new NotSupportedException();
            }
            // Clear the creation data if not used
            else
            {
                section.CreationData = null;
                section.CreationIds = null;
            }
        }
        private void DeleteSelectionBasedSectionSets(string oldSectionName)
        {
            // Delete previously created sets
            Section section = GetSection(oldSectionName);
            if (section.CreationData != null && section.RegionName != null)
            {
                if (section is SolidSection || section is ShellSection || section is MembraneSection)
                    RemoveElementSets(new string[] { section.RegionName });
                else throw new NotSupportedException();
            }
        }

        #endregion #################################################################################################################

        #region Constraint menu   ##################################################################################################
        // COMMANDS ********************************************************************************
        public void AddConstraintCommand(Constraint constraint, bool update = true)
        {
            Commands.CAddConstraint comm = new Commands.CAddConstraint(constraint, update);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceConstraintCommand(string oldConstraintName, Constraint newConstraint)
        {
            Commands.CReplaceConstraint comm = new Commands.CReplaceConstraint(oldConstraintName, newConstraint);
            _commands.AddAndExecute(comm);
        }
        public void DuplicateConstraintsCommand(string[] constraintNames)
        {
            Commands.CDuplicateConstraints comm = new Commands.CDuplicateConstraints(constraintNames);
            _commands.AddAndExecute(comm);
        }
        public void SwapMasterSlaveConstraintsCommand(string[] constraintNames)
        {
            Commands.CSwapMasterSlaveConstraints comm = new Commands.CSwapMasterSlaveConstraints(constraintNames);
            _commands.AddAndExecute(comm);
        }
        public void MergeByMasterSlaveConstraintsCommand(string[] constraintNames)
        {
            Commands.CMergeByMasterSlaveConstraints comm = new Commands.CMergeByMasterSlaveConstraints(constraintNames);
            _commands.AddAndExecute(comm);
        }
        public void HideConstraintsCommand(string[] constraintNames)
        {
            Commands.CHideConstraints comm = new Commands.CHideConstraints(constraintNames);
            _commands.AddAndExecute(comm);
        }
        public void ShowConstraintsCommand(string[] constraintNames)
        {
            Commands.CShowConstraints comm = new Commands.CShowConstraints(constraintNames);
            _commands.AddAndExecute(comm);
        }
        public void RemoveConstraintsCommand(string[] constraintNames)
        {
            Commands.CRemoveConstraints comm = new Commands.CRemoveConstraints(constraintNames);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public string[] GetConstraintNames()
        {
            return _model.Constraints.Keys.ToArray();
        }
        public void AddConstraint(Constraint constraint, bool update = true)
        {
            ConvertSelectionBasedConstraint(constraint, update);
            //
            _model.Constraints.Add(constraint.Name, constraint);
            //
            _form.AddTreeNode(ViewGeometryModelResults.Model, constraint, null);
            //
            if (update) FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public Constraint GetConstraint(string constraintName)
        {
            return _model.Constraints[constraintName];
        }
        public Constraint[] GetAllConstraints()
        {
            return _model.Constraints.Values.ToArray();
        }
        public void ReplaceConstraint(string oldConstraintName, Constraint constraint)
        {
            DeleteSelectionBasedConstraintSets(oldConstraintName);
            ConvertSelectionBasedConstraint(constraint);
            //
            _model.Constraints.Replace(oldConstraintName, constraint.Name, constraint);
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldConstraintName, constraint, null);
            //
            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void DuplicateConstraints(string[] constraintNames)
        {
            Constraint newConstraint;
            foreach (var constraintName in constraintNames)
            {
                newConstraint = GetConstraint(constraintName).DeepClone();
                newConstraint.Name = NamedClass.GetNameWithoutLastValue(newConstraint.Name);
                newConstraint.Name = GetConstraintNames().GetNextNumberedKey(newConstraint.Name);
                if (newConstraint.MasterCreationData != null) newConstraint.MasterRegionType = RegionTypeEnum.Selection;
                if (newConstraint.SlaveCreationData != null) newConstraint.SlaveRegionType = RegionTypeEnum.Selection;
                AddConstraint(newConstraint);
            }
        }
        public void SwapMasterSlaveConstraints(string[] constraintNames)
        {
            string newName;
            bool update = false;
            //
            foreach (var name in constraintNames)
            {
                if (_model.Constraints[name] is Tie tie)
                {
                    tie.SwapMasterSlave();
                    newName = tie.Name;
                    //
                    if (newName != name) _model.Constraints.Replace(name, newName, tie);
                    _form.UpdateTreeNode(ViewGeometryModelResults.Model, name, _model.Constraints[newName], null, false);
                    //
                    update = true;
                }
            }
            //
            if (update) FeModelUpdate(UpdateType.RedrawSymbols);
        }
        public void MergeByMasterSlaveConstraints(string[] constraintNames)
        {
            string[] tmp;
            string[] separators = new string[] { CaeMesh.Globals.MasterSlaveSeparator };
            HashSet<string> allNames = new HashSet<string>();
            HashSet<string> masterNames = new HashSet<string>();
            HashSet<string> slaveNames = new HashSet<string>();
            FeSurface masterSurface;
            FeSurface slaveSurface;
            List<Tie> toMerge = new List<Tie>();
            HashSet<FeSurfaceFaceTypes> masterSurfaceTypes = new HashSet<FeSurfaceFaceTypes>();
            HashSet<FeSurfaceFaceTypes> slaveSurfaceTypes = new HashSet<FeSurfaceFaceTypes>();
            //
            foreach (var name in constraintNames)
            {
                // Collect names
                tmp = name.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                if (tmp.Length == 2)
                {
                    masterNames.Add(tmp[0]);
                    slaveNames.Add(tmp[1]);
                }
                // Collect mergeable constraints
                if (_model.Constraints[name] is Tie tie)
                {
                    if (tie.MasterRegionType == RegionTypeEnum.SurfaceName && tie.MasterCreationData != null &&
                        tie.SlaveRegionType == RegionTypeEnum.SurfaceName && tie.SlaveCreationData != null)
                    {
                        if (_model.Mesh.Surfaces.TryGetValue(tie.MasterRegionName, out masterSurface) &&
                            _model.Mesh.Surfaces.TryGetValue(tie.SlaveRegionName, out slaveSurface) &&
                            masterSurface.CreationData != null && masterSurface.CreationData.IsGeometryBased() &&
                            slaveSurface.CreationData != null && slaveSurface.CreationData.IsGeometryBased())
                        {
                            masterSurfaceTypes.Add(masterSurface.SurfaceFaceTypes);
                            slaveSurfaceTypes.Add(slaveSurface.SurfaceFaceTypes);
                            //
                            toMerge.Add(tie);
                        }
                    }
                }
            }
            // Merge
            if (toMerge.Count > 1)
            {
                foreach (var key in _model.Constraints.Keys)
                {
                    tmp = key.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    allNames.UnionWith(tmp);
                }
                //
                if (masterSurfaceTypes.Count == 1 && masterSurfaceTypes.First() != FeSurfaceFaceTypes.Unknown &&
                    slaveSurfaceTypes.Count == 1 && slaveSurfaceTypes.First() != FeSurfaceFaceTypes.Unknown)
                {
                    // Names
                    string name;
                    string masterName;
                    string slaveName;
                    if (masterNames.Count == 1) masterName = masterNames.First();
                    else masterName = allNames.GetNextNumberedKey("Merged");
                    allNames.Add(masterName);
                    //
                    if (slaveNames.Count == 1) slaveName = slaveNames.First();
                    else slaveName = allNames.GetNextNumberedKey("Merged");
                    allNames.Add(slaveName);
                    //
                    name = masterName + CaeMesh.Globals.MasterSlaveSeparator + slaveName;
                    if (_model.Constraints.ContainsKey(name)) name = _model.Constraints.GetNextNumberedKey(name);
                    allNames.Add(name);
                    // New tie
                    bool twoD = _model.Properties.ModelSpace.IsTwoD();
                    Tie firstTie = toMerge.First();
                    Tie newTie = new Tie(name, firstTie.PositionTolerance.Value, firstTie.Adjust, "", RegionTypeEnum.Selection,
                                         "", RegionTypeEnum.Selection, twoD);
                    //
                    newTie.MasterCreationData = new Selection();
                    newTie.MasterCreationData.SelectItem = vtkSelectItem.Surface;
                    newTie.MasterCreationIds = new int[] { 1 };
                    //
                    newTie.SlaveCreationData = new Selection();
                    newTie.SlaveCreationData.SelectItem = vtkSelectItem.Surface;
                    newTie.SlaveCreationIds = new int[] { 1 };
                    // Combine selections
                    List<string> removeNames = new List<string>();
                    foreach (Tie tie in toMerge)
                    {
                        foreach (SelectionNode node in tie.MasterCreationData.Nodes) newTie.MasterCreationData.Add(node, null);
                        foreach (SelectionNode node in tie.SlaveCreationData.Nodes) newTie.SlaveCreationData.Add(node, null);
                        //
                        if (tie != firstTie) removeNames.Add(tie.Name);
                    }
                    // Remove
                    RemoveConstraints(removeNames.ToArray());
                    //
                    ReplaceConstraint(firstTie.Name, newTie); // also updates
                }
            }
            else MessageBoxes.ShowError("The selected constraints are not of the same geometry type.");
        }
        public void HideConstraints(string[] constraintNames)
        {
            foreach (var name in constraintNames)
            {
                _model.Constraints[name].Visible = false;
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, name, _model.Constraints[name], null, false);
            }
            //
            FeModelUpdate(UpdateType.RedrawSymbols);
        }
        public void ShowConstraints(string[] constraintNames)
        {
            foreach (var name in constraintNames)
            {
                _model.Constraints[name].Visible = true;
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, name, _model.Constraints[name], null, false);
            }
            //
            FeModelUpdate(UpdateType.RedrawSymbols);
        }
        public void ActivateDeactivateConstraint(string constraintName, bool active)
        {
            Constraint constraint = _model.Constraints[constraintName];
            constraint.Active = active;
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, constraintName, constraint, null);
            //
            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void RemoveConstraints(string[] constraintNames)
        {
            foreach (var name in constraintNames)
            {
                DeleteSelectionBasedConstraintSets(name);
                _model.Constraints.Remove(name);
                _form.RemoveTreeNode<Constraint>(ViewGeometryModelResults.Model, name, null);
            }
            //
            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        //
        private void ConvertSelectionBasedConstraint(Constraint constraint, bool update = true)
        {
            // Create a named set and convert a selection to a named set
            string name;
            if (constraint is PointSpring ps)
            {
                // Node set
                if (ps.RegionType == RegionTypeEnum.Selection)
                {
                    name = FeMesh.GetNextFreeSelectionName(_model.Mesh.NodeSets, constraint.Name);
                    FeNodeSet nodeSet = new FeNodeSet(name, ps.CreationIds);
                    nodeSet.CreationData = ps.CreationData.DeepClone();
                    nodeSet.Internal = true;
                    AddNodeSet(nodeSet, update);
                    //
                    ps.RegionName = name;
                    ps.RegionType = RegionTypeEnum.NodeSetName;
                }
                // Clear the creation data if not used
                else
                {
                    ps.CreationData = null;
                    ps.CreationIds = null;
                }
            }
            else if (constraint is SurfaceSpring ss)
            {
                // Surface
                if (ss.RegionType == RegionTypeEnum.Selection)
                {
                    name = FeMesh.GetNextFreeSelectionName(_model.Mesh.Surfaces,
                                                           constraint.Name + CaeMesh.Globals.MasterNameSuffix);
                    FeSurface surface = new FeSurface(name, ss.CreationIds, ss.CreationData.DeepClone());
                    surface.Internal = true;
                    AddSurface(surface, update);
                    //
                    ss.RegionName = name;
                    ss.RegionType = RegionTypeEnum.SurfaceName;
                }
                // Clear the creation data if not used
                else
                {
                    ss.CreationData = null;
                    ss.CreationIds = null;
                }
            }
            else if (constraint is CompressionOnly co)
            {
                // Surface
                if (co.RegionType == RegionTypeEnum.Selection)
                {
                    name = FeMesh.GetNextFreeSelectionName(_model.Mesh.Surfaces,
                                                           constraint.Name + CaeMesh.Globals.MasterNameSuffix);
                    FeSurface surface = new FeSurface(name, co.CreationIds, co.CreationData.DeepClone());
                    surface.Internal = true;
                    AddSurface(surface, update);
                    //
                    co.RegionName = name;
                    co.RegionType = RegionTypeEnum.SurfaceName;
                }
                // Clear the creation data if not used
                else
                {
                    co.CreationData = null;
                    co.CreationIds = null;
                }
            }
            else if (constraint is RigidBody rb)
            {
                // Node set
                if (rb.RegionType == RegionTypeEnum.Selection)
                {
                    name = FeMesh.GetNextFreeSelectionName(_model.Mesh.NodeSets, constraint.Name);
                    FeNodeSet nodeSet = new FeNodeSet(name, rb.CreationIds);
                    nodeSet.CreationData = rb.CreationData.DeepClone();
                    nodeSet.Internal = true;
                    AddNodeSet(nodeSet, update);
                    //
                    rb.RegionName = name;
                    rb.RegionType = RegionTypeEnum.NodeSetName;
                }
                // Clear the creation data if not used
                else
                {
                    rb.CreationData = null;
                    rb.CreationIds = null;
                }
            }
            else if (constraint is Tie tie)
            {
                // Master Surface
                if (tie.MasterRegionType == RegionTypeEnum.Selection)
                {
                    name = FeMesh.GetNextFreeSelectionName(_model.Mesh.Surfaces, constraint.Name + CaeMesh.Globals.MasterNameSuffix);
                    FeSurface surface = new FeSurface(name, tie.MasterCreationIds, tie.MasterCreationData.DeepClone());
                    surface.Internal = true;
                    AddSurface(surface, update);
                    //
                    tie.MasterRegionName = name;
                    tie.MasterRegionType = RegionTypeEnum.SurfaceName;
                }
                // Clear the creation data if not used
                else
                {
                    tie.MasterCreationData = null;
                    tie.MasterCreationIds = null;
                }
                // Slave Surface
                if (tie.SlaveRegionType == RegionTypeEnum.Selection)
                {
                    name = FeMesh.GetNextFreeSelectionName(_model.Mesh.Surfaces, constraint.Name + CaeMesh.Globals.SlaveNameSuffix);
                    FeSurface surface = new FeSurface(name, tie.SlaveCreationIds, tie.SlaveCreationData.DeepClone());
                    surface.Internal = true;
                    AddSurface(surface, update);
                    //
                    tie.SlaveRegionName = name;
                    tie.SlaveRegionType = RegionTypeEnum.SurfaceName;
                }
                // Clear the creation data if not used
                else
                {
                    tie.SlaveCreationData = null;
                    tie.SlaveCreationIds = null;
                }
            }
            else throw new NotSupportedException();
        }
        private void DeleteSelectionBasedConstraintSets(string oldConstraintName)
        {
            // Delete previously created sets
            Constraint constraint = GetConstraint(oldConstraintName);
            if (constraint is PointSpring ps && ps.CreationData != null && ps.RegionName != null &&
                ps.RegionType == RegionTypeEnum.NodeSetName)
            {
                RemoveNodeSets(new string[] { ps.RegionName });
            }
            else if (constraint is SurfaceSpring ss)
            {
                if (ss.CreationData != null && ss.RegionName != null)
                    RemoveSurfaces(new string[] { ss.RegionName }, false);
            }
            else if (constraint is RigidBody rb && rb.CreationData != null && rb.RegionName != null)
            {
                RemoveNodeSets(new string[] { rb.RegionName });
            }
            else if (constraint is Tie tie)
            {
                if (tie.MasterCreationData != null && tie.MasterRegionName != null)
                    RemoveSurfaces(new string[] { tie.MasterRegionName }, false);
                if (tie.SlaveCreationData != null && tie.SlaveRegionName != null)
                    RemoveSurfaces(new string[] { tie.SlaveRegionName }, false);
            }
        }
        // Auto create
        public void AutoCreateTiedPairs(List<Forms.SearchContactPair> contactPairs)
        {
            if (contactPairs != null)
            {
                string name;
                Tie tie;
                bool adjust;
                Dictionary<string, int> nameCounter = new Dictionary<string, int>();
                foreach (var contactPair in contactPairs)
                {
                    if (nameCounter.ContainsKey(contactPair.Name)) nameCounter[contactPair.Name]++;
                    else nameCounter.Add(contactPair.Name, 1);
                }
                foreach (var contactPair in contactPairs)
                {
                    if (contactPair.MasterSlaveItem.Unresolved)
                    {
                        name = contactPair.Name;
                        if (nameCounter[name] > 1 || _model.Mesh.Surfaces.ContainsKey(name))
                            name = _model.Mesh.Surfaces.GetNextNumberedKey(name);
                        //
                        FeSurface surface = new FeSurface(name);
                        surface.CreationData = new Selection();
                        surface.CreationData.SelectItem = vtkSelectItem.Surface;
                        surface.CreationData.Add(new SelectionNodeIds(vtkSelectOperation.Add, false,
                                                                      contactPair.MasterSlaveItem.MasterGeometryIds.ToArray(),
                                                                      true));
                        surface.CreationData = GetMouseSelectionFromSelectionNodeIds(surface.CreationData);
                        //
                        AddSurfaceCommand(surface, false);
                    }
                    else
                    {
                        name = contactPair.Name;
                        if (nameCounter[name] > 1 || _model.Constraints.ContainsKey(name))
                            name = _model.Constraints.GetNextNumberedKey(name);
                        //
                        adjust = contactPair.Adjust == Forms.SearchContactPairAdjust.Yes;
                        bool twoD = _model.Properties.ModelSpace.IsTwoD();
                        tie = new Tie(name, contactPair.Distance, adjust, "", RegionTypeEnum.Selection,
                                      "", RegionTypeEnum.Selection, twoD);
                        //
                        tie.MasterCreationData = new Selection();
                        tie.MasterCreationData.SelectItem = vtkSelectItem.Surface;
                        tie.MasterCreationData.EnableShellEdgeFaceSelection = true;
                        tie.MasterCreationData.Add(new SelectionNodeIds(vtkSelectOperation.Add, false,
                                                                        contactPair.MasterSlaveItem.MasterGeometryIds.ToArray(),
                                                                        true));
                        tie.MasterCreationData = GetMouseSelectionFromSelectionNodeIds(tie.MasterCreationData);
                        tie.MasterCreationIds = new int[] { 1 };
                        //
                        tie.SlaveCreationData = new Selection();
                        tie.SlaveCreationData.SelectItem = vtkSelectItem.Surface;
                        tie.SlaveCreationData.EnableShellEdgeFaceSelection = true;
                        tie.SlaveCreationData.Add(new SelectionNodeIds(vtkSelectOperation.Add, false,
                                                                       contactPair.MasterSlaveItem.SlaveGeometryIds.ToArray(),
                                                                       true));
                        tie.SlaveCreationData = GetMouseSelectionFromSelectionNodeIds(tie.SlaveCreationData);
                        tie.SlaveCreationIds = new int[] { 1 };
                        //
                        AddConstraintCommand(tie, false);
                    }
                }
                //
                FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
            }
        }

        #endregion #################################################################################################################

        #region Surface interaction menu   #########################################################################################
        // COMMANDS ********************************************************************************
        public void AddSurfaceInteractionCommand(SurfaceInteraction surfaceInteraction)
        {
            Commands.CAddSurfaceInteraction comm = new Commands.CAddSurfaceInteraction(surfaceInteraction);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceSurfaceInteractionCommand(string oldSurfaceInteractionName, SurfaceInteraction newSurfaceInteraction)
        {
            Commands.CReplaceSurfaceInteraction comm = new Commands.CReplaceSurfaceInteraction(oldSurfaceInteractionName,
                                                                                               newSurfaceInteraction);
            _commands.AddAndExecute(comm);
        }
        public void DuplicateSurfaceInteractionsCommand(string[] surfaceInteractionNames)
        {
            Commands.CDuplicateSurfaceInteractions comm = new Commands.CDuplicateSurfaceInteractions(surfaceInteractionNames);
            _commands.AddAndExecute(comm);
        }
        public void RemoveSurfaceInteractionsCommand(string[] surfaceInteractionNames)
        {
            Commands.CRemoveSurfaceInteractions comm = new Commands.CRemoveSurfaceInteractions(surfaceInteractionNames);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public string[] GetSurfaceInteractionNames()
        {
            return _model.SurfaceInteractions.Keys.ToArray();
        }
        public void AddSurfaceInteraction(SurfaceInteraction surfaceInteraction)
        {
            _model.SurfaceInteractions.Add(surfaceInteraction.Name, surfaceInteraction);
            _form.AddTreeNode(ViewGeometryModelResults.Model, surfaceInteraction, null);
            //
            CheckAndUpdateModelValidity();
        }
        public SurfaceInteraction GetSurfaceInteraction(string surfaceInteractionName)
        {
            return _model.SurfaceInteractions[surfaceInteractionName];
        }
        public SurfaceInteraction[] GetAllSurfaceInteractions()
        {
            return _model.SurfaceInteractions.Values.ToArray();
        }
        public void ReplaceSurfaceInteraction(string oldSurfaceInteractionName, SurfaceInteraction newSurfaceInteraction)
        {
            _model.SurfaceInteractions.Replace(oldSurfaceInteractionName, newSurfaceInteraction.Name, newSurfaceInteraction);
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldSurfaceInteractionName, newSurfaceInteraction, null);
            //
            CheckAndUpdateModelValidity();
        }
        public void DuplicateSurfaceInteractions(string[] surfaceInteractionNames)
        {
            SurfaceInteraction newSurfaceInteraction;
            foreach (var name in surfaceInteractionNames)
            {
                newSurfaceInteraction = _model.SurfaceInteractions[name].DeepClone();
                newSurfaceInteraction.Name = NamedClass.GetNameWithoutLastValue(newSurfaceInteraction.Name);
                newSurfaceInteraction.Name = _model.SurfaceInteractions.GetNextNumberedKey(newSurfaceInteraction.Name);
                AddSurfaceInteraction(newSurfaceInteraction);
            }
        }
        public void RemoveSurfaceInteractions(string[] surfaceInteractionNames)
        {
            foreach (var name in surfaceInteractionNames)
            {
                _model.SurfaceInteractions.Remove(name);
                _form.RemoveTreeNode<SurfaceInteraction>(ViewGeometryModelResults.Model, name, null);
            }
            //
            CheckAndUpdateModelValidity();
        }

        #endregion #################################################################################################################

        #region Contact pair menu   ################################################################################################
        // COMMANDS ********************************************************************************
        public void AddContactPairCommand(ContactPair contactPair, bool update = true)
        {
            Commands.CAddContactPair comm = new Commands.CAddContactPair(contactPair, update);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceContactPairCommand(string oldContactPairName, ContactPair newContactPair)
        {
            Commands.CReplaceContactPair comm = new Commands.CReplaceContactPair(oldContactPairName, newContactPair);
            _commands.AddAndExecute(comm);
        }
        public void DuplicateContactPairsCommand(string[] contactPairNames)
        {
            Commands.CDuplicateContactPairs comm = new Commands.CDuplicateContactPairs(contactPairNames);
            _commands.AddAndExecute(comm);
        }
        public void SwapMasterSlaveContactPairsCommand(string[] contactPairNames)
        {
            Commands.CSwapMasterSlaveContactPairs comm = new Commands.CSwapMasterSlaveContactPairs(contactPairNames);
            _commands.AddAndExecute(comm);
        }
        public void MergeByMasterSlaveContactPairsCommand(string[] contactPairNames)
        {
            Commands.CMergeByMasterSlaveContactPairs comm = new Commands.CMergeByMasterSlaveContactPairs(contactPairNames);
            _commands.AddAndExecute(comm);
        }
        public void HideContactPairsCommand(string[] contactPairNames)
        {
            Commands.CHideContactPairs comm = new Commands.CHideContactPairs(contactPairNames);
            _commands.AddAndExecute(comm);
        }
        public void ShowContactPairsCommand(string[] contactPairNames)
        {
            Commands.CShowContactPairs comm = new Commands.CShowContactPairs(contactPairNames);
            _commands.AddAndExecute(comm);
        }
        public void RemoveContactPairsCommand(string[] contactPairNames)
        {
            Commands.CRemoveContactPairs comm = new Commands.CRemoveContactPairs(contactPairNames);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public string[] GetContactPairNames()
        {
            return _model.ContactPairs.Keys.ToArray();
        }
        public void AddContactPair(ContactPair contactPair, bool update = true)
        {
            ConvertSelectionBasedContactPair(contactPair, update);
            //
            _model.ContactPairs.Add(contactPair.Name, contactPair);
            //
            _form.AddTreeNode(ViewGeometryModelResults.Model, contactPair, null);
            //
            if (update) FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public ContactPair GetContactPair(string contactPairName)
        {
            return _model.ContactPairs[contactPairName];
        }
        public ContactPair[] GetAllContactPairs()
        {
            return _model.ContactPairs.Values.ToArray();
        }
        public void ReplaceContactPair(string oldContactPairName, ContactPair contactPair)
        {
            DeleteSelectionBasedContactPairSets(oldContactPairName);
            ConvertSelectionBasedContactPair(contactPair);
            //
            _model.ContactPairs.Replace(oldContactPairName, contactPair.Name, contactPair);
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldContactPairName, contactPair, null);
            //
            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void DuplicateContactPairs(string[] contactPairNames)
        {
            ContactPair newContactPair;
            foreach (var name in contactPairNames)
            {
                newContactPair = _model.ContactPairs[name].DeepClone();
                newContactPair.Name = NamedClass.GetNameWithoutLastValue(newContactPair.Name);
                newContactPair.Name = _model.ContactPairs.GetNextNumberedKey(newContactPair.Name);
                if (newContactPair.MasterCreationData != null) newContactPair.MasterRegionType = RegionTypeEnum.Selection;
                if (newContactPair.SlaveCreationData != null) newContactPair.SlaveRegionType = RegionTypeEnum.Selection;
                AddContactPair(newContactPair);
            }
        }
        public void SwapMasterSlaveContactPairs(string[] contactPairNames)
        {
            string newName;
            bool update = false;
            ContactPair contactPair;
            //
            foreach (var name in contactPairNames)
            {
                contactPair = _model.ContactPairs[name];
                contactPair.SwapMasterSlave();
                newName = contactPair.Name;
                //
                if (newName != name) _model.ContactPairs.Replace(name, newName, contactPair);
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, name, _model.ContactPairs[newName], null, false);
                //
                update = true;
            }
            //
            if (update) FeModelUpdate(UpdateType.RedrawSymbols);
        }
        public void MergeByMasterSlaveContactPairs(string[] contactPairNames)
        {
            string[] tmp;
            string[] separators = new string[] { CaeMesh.Globals.MasterSlaveSeparator };
            HashSet<string> allNames = new HashSet<string>();
            HashSet<string> masterNames = new HashSet<string>();
            HashSet<string> slaveNames = new HashSet<string>();
            FeSurface masterSurface;
            FeSurface slaveSurface;
            ContactPair contactPair;
            List<ContactPair> toMerge = new List<ContactPair>();
            HashSet<string> surfaceInteractionNames = new HashSet<string>();
            HashSet<ContactPairMethod> contactPairMethods = new HashSet<ContactPairMethod>();
            HashSet<FeSurfaceFaceTypes> masterSurfaceTypes = new HashSet<FeSurfaceFaceTypes>();
            HashSet<FeSurfaceFaceTypes> slaveSurfaceTypes = new HashSet<FeSurfaceFaceTypes>();
            //
            foreach (var name in contactPairNames)
            {
                // Collect names
                tmp = name.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                if (tmp.Length == 2)
                {
                    masterNames.Add(tmp[0]);
                    slaveNames.Add(tmp[1]);
                }
                // Collect mergeable contact parts
                contactPair = _model.ContactPairs[name];
                if (contactPair.MasterRegionType == RegionTypeEnum.SurfaceName && contactPair.MasterCreationData != null &&
                    contactPair.SlaveRegionType == RegionTypeEnum.SurfaceName && contactPair.SlaveCreationData != null)
                {
                    if (_model.Mesh.Surfaces.TryGetValue(contactPair.MasterRegionName, out masterSurface) &&
                        _model.Mesh.Surfaces.TryGetValue(contactPair.SlaveRegionName, out slaveSurface) &&
                        masterSurface.CreationData != null && masterSurface.CreationData.IsGeometryBased() &&
                        slaveSurface.CreationData != null && slaveSurface.CreationData.IsGeometryBased())
                    {
                        masterSurfaceTypes.Add(masterSurface.SurfaceFaceTypes);
                        slaveSurfaceTypes.Add(slaveSurface.SurfaceFaceTypes);
                        //
                        toMerge.Add(contactPair);
                        surfaceInteractionNames.Add(contactPair.SurfaceInteractionName);
                        contactPairMethods.Add(contactPair.Method);
                    }
                }

            }
            // Merge
            if (surfaceInteractionNames.Count != 1)
                MessageBoxes.ShowError("The selected contact pairs do not have the same surface interaction.");
            else if (contactPairMethods.Count != 1)
                MessageBoxes.ShowError("The selected contact pairs do not have the same contact pair method.");
            else if (toMerge.Count > 1)
            {
                foreach (var key in _model.ContactPairs.Keys)
                {
                    tmp = key.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    allNames.UnionWith(tmp);
                }
                //
                if (masterSurfaceTypes.Count == 1 && masterSurfaceTypes.First() != FeSurfaceFaceTypes.Unknown &&
                    slaveSurfaceTypes.Count == 1 && slaveSurfaceTypes.First() != FeSurfaceFaceTypes.Unknown)
                {
                    // Names
                    string name;
                    string masterName;
                    string slaveName;
                    if (masterNames.Count == 1) masterName = masterNames.First();
                    else masterName = allNames.GetNextNumberedKey("Merged");
                    allNames.Add(masterName);
                    //
                    if (slaveNames.Count == 1) slaveName = slaveNames.First();
                    else slaveName = allNames.GetNextNumberedKey("Merged");
                    allNames.Add(slaveName);
                    //
                    name = masterName + CaeMesh.Globals.MasterSlaveSeparator + slaveName;
                    if (_model.ContactPairs.ContainsKey(name)) name = _model.ContactPairs.GetNextNumberedKey(name);
                    allNames.Add(name);
                    // New tie
                    ContactPair firstContactPair = toMerge.First();
                    ContactPair newContactPair = new ContactPair(name, surfaceInteractionNames.First(), contactPairMethods.First(),
                                                                 firstContactPair.SmallSliding, firstContactPair.Adjust,
                                                                 firstContactPair.AdjustmentSize,
                                                                 "", RegionTypeEnum.Selection,
                                                                 "", RegionTypeEnum.Selection);
                    //
                    newContactPair.MasterCreationData = new Selection();
                    newContactPair.MasterCreationData.SelectItem = vtkSelectItem.Surface;
                    newContactPair.MasterCreationIds = new int[] { 1 };
                    //
                    newContactPair.SlaveCreationData = new Selection();
                    newContactPair.SlaveCreationData.SelectItem = vtkSelectItem.Surface;
                    newContactPair.SlaveCreationIds = new int[] { 1 };
                    // Combine selections
                    List<string> removeNames = new List<string>();
                    foreach (ContactPair contactPairToMerge in toMerge)
                    {
                        foreach (SelectionNode node in contactPairToMerge.MasterCreationData.Nodes)
                            newContactPair.MasterCreationData.Add(node, null);
                        foreach (SelectionNode node in contactPairToMerge.SlaveCreationData.Nodes)
                            newContactPair.SlaveCreationData.Add(node, null);
                        //
                        if (contactPairToMerge != firstContactPair) removeNames.Add(contactPairToMerge.Name);
                    }
                    // Remove
                    RemoveContactPairs(removeNames.ToArray());
                    //
                    ReplaceContactPair(firstContactPair.Name, newContactPair); // also updates
                }
            }
            else MessageBoxes.ShowError("The selected contact pairs are not of the same geometry type.");
        }
        public void HideContactPairs(string[] contactPairNames)
        {
            foreach (var name in contactPairNames)
            {
                _model.ContactPairs[name].Visible = false;
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, name, _model.ContactPairs[name], null, false);
            }
            //
            FeModelUpdate(UpdateType.RedrawSymbols);
        }
        public void ShowContactPairs(string[] contactPairNames)
        {
            foreach (var name in contactPairNames)
            {
                _model.ContactPairs[name].Visible = true;
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, name, _model.ContactPairs[name], null, false);
            }
            //
            FeModelUpdate(UpdateType.RedrawSymbols);
        }
        public void ActivateDeactivateContactPair(string contactPairName, bool active)
        {
            ContactPair contactPair = _model.ContactPairs[contactPairName];
            contactPair.Active = active;
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, contactPairName, contactPair, null);
            //
            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void RemoveContactPairs(string[] contactPairNames)
        {
            foreach (var name in contactPairNames)
            {
                DeleteSelectionBasedContactPairSets(name);
                _model.ContactPairs.Remove(name);
                _form.RemoveTreeNode<ContactPair>(ViewGeometryModelResults.Model, name, null);
            }
            //
            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        //
        private void ConvertSelectionBasedContactPair(ContactPair contactPair, bool update = true)
        {
            // Create a named set and convert a selection to a named set
            string name;
            // Master Surface
            if (contactPair.MasterRegionType == RegionTypeEnum.Selection)
            {
                name = FeMesh.GetNextFreeSelectionName(_model.Mesh.Surfaces, contactPair.Name + CaeMesh.Globals.MasterNameSuffix);
                FeSurface surface = new FeSurface(name, contactPair.MasterCreationIds, contactPair.MasterCreationData.DeepClone());
                surface.Internal = true;
                AddSurface(surface, update);
                //
                contactPair.MasterRegionName = name;
                contactPair.MasterRegionType = RegionTypeEnum.SurfaceName;
            }
            // Clear the creation data if not used
            else
            {
                contactPair.MasterCreationData = null;
                contactPair.MasterCreationIds = null;
            }
            // Slave Surface
            if (contactPair.SlaveRegionType == RegionTypeEnum.Selection)
            {
                name = FeMesh.GetNextFreeSelectionName(_model.Mesh.Surfaces, contactPair.Name + CaeMesh.Globals.SlaveNameSuffix);
                FeSurface surface = new FeSurface(name, contactPair.SlaveCreationIds, contactPair.SlaveCreationData.DeepClone());
                surface.Internal = true;
                AddSurface(surface, update);
                //
                contactPair.SlaveRegionName = name;
                contactPair.SlaveRegionType = RegionTypeEnum.SurfaceName;
            }
            // Clear the creation data if not used
            else
            {
                contactPair.SlaveCreationData = null;
                contactPair.SlaveCreationIds = null;
            }

        }
        private void DeleteSelectionBasedContactPairSets(string oldContactPairName)
        {
            // Delete previously created sets
            ContactPair contactPair = GetContactPair(oldContactPairName);
            if (contactPair.MasterCreationData != null && contactPair.MasterRegionName != null)
                RemoveSurfaces(new string[] { contactPair.MasterRegionName }, false);
            if (contactPair.SlaveCreationData != null && contactPair.SlaveRegionName != null)
                RemoveSurfaces(new string[] { contactPair.SlaveRegionName }, false);
        }
        // Auto create
        public void AutoCreateContactPairs(List<SearchContactPair> contactPairs)
        {
            if (contactPairs != null)
            {
                string name;
                bool adjust;
                CaeModel.ContactPairMethod method;
                ContactPair contactPairToAdd;
                Dictionary<string, int> nameCounter = new Dictionary<string, int>();
                foreach (var contactPair in contactPairs)
                {
                    if (nameCounter.ContainsKey(contactPair.Name)) nameCounter[contactPair.Name]++;
                    else nameCounter.Add(contactPair.Name, 1);
                }
                foreach (var contactPair in contactPairs)
                {
                    name = contactPair.Name;
                    if (nameCounter[name] > 1 || _model.ContactPairs.ContainsKey(name))
                        name = _model.ContactPairs.GetNextNumberedKey(name);
                    //
                    adjust = contactPair.Adjust == SearchContactPairAdjust.Yes;
                    if (contactPair.ContactPairMethod == FrmSearchContactPairs.ContactPairMethodNames[0])
                        method = ContactPairMethod.NodeToSurface;
                    else method = ContactPairMethod.SurfaceToSurface;
                    //
                    contactPairToAdd = new ContactPair(name, contactPair.SurfaceInteractionName,
                                                       method, false, adjust, contactPair.Distance,
                                                       "", RegionTypeEnum.Selection, "", RegionTypeEnum.Selection);
                    //
                    contactPairToAdd.MasterCreationData = new Selection();
                    contactPairToAdd.MasterCreationData.SelectItem = vtkSelectItem.Surface;
                    contactPairToAdd.MasterCreationData.EnableShellEdgeFaceSelection = true;
                    contactPairToAdd.MasterCreationData.Add(new SelectionNodeIds(vtkSelectOperation.Add, false,
                                                            contactPair.MasterSlaveItem.MasterGeometryIds.ToArray(),
                                                            true));
                    contactPairToAdd.MasterCreationData = GetMouseSelectionFromSelectionNodeIds(contactPairToAdd.MasterCreationData);
                    contactPairToAdd.MasterCreationIds = new int[] { 1 };
                    //
                    contactPairToAdd.SlaveCreationData = new Selection();
                    contactPairToAdd.SlaveCreationData.SelectItem = vtkSelectItem.Surface;
                    contactPairToAdd.SlaveCreationData.EnableShellEdgeFaceSelection = true;
                    contactPairToAdd.SlaveCreationData.Add(new SelectionNodeIds(vtkSelectOperation.Add, false,
                                                           contactPair.MasterSlaveItem.SlaveGeometryIds.ToArray(),
                                                           true));
                    contactPairToAdd.SlaveCreationData = GetMouseSelectionFromSelectionNodeIds(contactPairToAdd.SlaveCreationData);
                    contactPairToAdd.SlaveCreationIds = new int[] { 1 };
                    //
                    AddContactPairCommand(contactPairToAdd, false);
                }
                //
                FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
            }
        }

        #endregion #################################################################################################################

        #region Amplitude menu   ###################################################################################################
        // COMMANDS ********************************************************************************
        public void AddAmplitudeCommand(Amplitude amplitude)
        {
            Commands.CAddAmplitude comm = new Commands.CAddAmplitude(amplitude);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceAmplitudeCommand(string oldAmplitudeName, Amplitude newAmplitude)
        {
            Commands.CReplaceAmplitude comm = new Commands.CReplaceAmplitude(oldAmplitudeName, newAmplitude);
            _commands.AddAndExecute(comm);
        }
        public void DuplicateAmplitudesCommand(string[] amplitudeNames)
        {
            Commands.CDuplicateAmplitudes comm = new Commands.CDuplicateAmplitudes(amplitudeNames);
            _commands.AddAndExecute(comm);
        }
        public void RemoveAmplitudesCommand(string[] amplitudeNames)
        {
            Commands.CRemoveAmplitudes comm = new Commands.CRemoveAmplitudes(amplitudeNames);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public string[] GetAmplitudeNames()
        {
            return _model.Amplitudes.Keys.ToArray();
        }
        public string[] GetAmplitudeNamesIncludingDefault()
        {
            List<string> names = new List<string>();
            names.Add("Default");
            names.AddRange(_model.Amplitudes.Keys);
            return names.ToArray();
        }
        public void AddAmplitude(Amplitude amplitude)
        {
            _model.Amplitudes.Add(amplitude.Name, amplitude);
            //
            _form.AddTreeNode(ViewGeometryModelResults.Model, amplitude, null);
            //
            FeModelUpdate(UpdateType.Check);
        }
        public Amplitude GetAmplitude(string amplitudeName)
        {
            return _model.Amplitudes[amplitudeName];
        }
        public Amplitude[] GetAllAmplitudes()
        {
            return _model.Amplitudes.Values.ToArray();
        }
        public void ReplaceAmplitude(string oldAmplitudeName, Amplitude amplitude)
        {
            _model.Amplitudes.Replace(oldAmplitudeName, amplitude.Name, amplitude);
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldAmplitudeName, amplitude, null);
            //
            FeModelUpdate(UpdateType.Check);
        }
        public void DuplicateAmplitudes(string[] amplitudeNames)
        {
            Amplitude newAmplitude;
            foreach (var name in amplitudeNames)
            {
                newAmplitude = _model.Amplitudes[name].DeepClone();
                newAmplitude.Name = NamedClass.GetNameWithoutLastValue(newAmplitude.Name);
                newAmplitude.Name = _model.Amplitudes.GetNextNumberedKey(newAmplitude.Name);
                AddAmplitude(newAmplitude);
            }
        }
        public void RemoveAmplitudes(string[] amplitudeNames)
        {
            foreach (var name in amplitudeNames)
            {
                _model.Amplitudes.Remove(name);
                _form.RemoveTreeNode<Amplitude>(ViewGeometryModelResults.Model, name, null);
            }
            //
            FeModelUpdate(UpdateType.Check);
        }

        #endregion #################################################################################################################

        #region Initial condition menu   ###########################################################################################
        // COMMANDS ********************************************************************************
        public void AddInitialConditionCommand(InitialCondition initialCondition)
        {
            Commands.CAddInitialCondition comm = new Commands.CAddInitialCondition(initialCondition);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceInitialConditionCommand(string oldInitialConditionName, InitialCondition newInitialCondition)
        {
            Commands.CReplaceInitialCondition comm = new Commands.CReplaceInitialCondition(oldInitialConditionName,
                                                                                           newInitialCondition);
            _commands.AddAndExecute(comm);
        }
        public void DuplicateInitialConditionsCommand(string[] initialConditionNames)
        {
            Commands.CDuplicateInitialConditions comm = new Commands.CDuplicateInitialConditions(initialConditionNames);
            _commands.AddAndExecute(comm);
        }
        public void RemoveInitialConditionsCommand(string[] initialConditionNames)
        {
            Commands.CRemoveInitialConditions comm = new Commands.CRemoveInitialConditions(initialConditionNames);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public string[] GetInitialConditionNames()
        {
            return _model.InitialConditions.Keys.ToArray();
        }
        public void AddInitialCondition(InitialCondition initialCondition)
        {
            ConvertSelectionBasedInitialCondition(initialCondition);
            //
            _model.InitialConditions.Add(initialCondition.Name, initialCondition);
            //
            _form.AddTreeNode(ViewGeometryModelResults.Model, initialCondition, null);
            //
            FeModelUpdate(UpdateType.Check);
        }
        public InitialCondition GetInitialCondition(string initialConditionName)
        {
            return _model.InitialConditions[initialConditionName];
        }
        public InitialCondition[] GetAllInitialConditions()
        {
            return _model.InitialConditions.Values.ToArray();
        }
        public void PreviewInitialCondition(string initialConditionName)
        {
            InitialCondition initialCondition = GetInitialCondition(initialConditionName);
            if (initialCondition != null)
            {
                FeResults results;
                if (initialCondition is InitialTemperature it)
                {
                    results = it.GetPreview(_model.Mesh, initialConditionName, _model.UnitSystem.UnitSystemType);
                }
                else if (initialCondition is InitialVelocity iv)
                {
                    results = iv.GetPreview(_model.Mesh, initialConditionName, _model.UnitSystem.UnitSystemType);
                }
                else throw new CaeException("It is not possible to preview this initial condition type.");
                //
                SetResults(results);
            }
        }
        public void ReplaceInitialCondition(string oldInitialConditionName, InitialCondition initialCondition)
        {
            DeleteSelectionBasedInitialConditionSets(oldInitialConditionName);
            ConvertSelectionBasedInitialCondition(initialCondition);
            //
            _model.InitialConditions.Replace(oldInitialConditionName, initialCondition.Name, initialCondition);
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldInitialConditionName, initialCondition, null);
            //
            FeModelUpdate(UpdateType.Check);
        }
        public void DuplicateInitialConditions(string[] initialConditionNames)
        {
            InitialCondition newInitialCondition;
            foreach (var name in initialConditionNames)
            {
                newInitialCondition = _model.InitialConditions[name].DeepClone();
                newInitialCondition.Name = NamedClass.GetNameWithoutLastValue(newInitialCondition.Name);
                newInitialCondition.Name = _model.InitialConditions.GetNextNumberedKey(newInitialCondition.Name);
                if (newInitialCondition.CreationData != null) newInitialCondition.RegionType = RegionTypeEnum.Selection;
                AddInitialCondition(newInitialCondition);
            }
        }
        public void ActivateDeactivateInitialCondition(string initialConditionName, bool active)
        {
            InitialCondition initialCondition = _model.InitialConditions[initialConditionName];
            initialCondition.Active = active;
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, initialConditionName, initialCondition, null);
            //
            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void RemoveInitialConditions(string[] initialConditionNames)
        {
            foreach (var name in initialConditionNames)
            {
                DeleteSelectionBasedInitialConditionSets(name);
                _model.InitialConditions.Remove(name);
                _form.RemoveTreeNode<InitialCondition>(ViewGeometryModelResults.Model, name, null);
            }
            //
            FeModelUpdate(UpdateType.Check);
        }
        //
        private void ConvertSelectionBasedInitialCondition(InitialCondition initialCondition)
        {
            // Create a named set and convert a selection to a named set
            if (initialCondition.RegionType == RegionTypeEnum.Selection)
            {
                string name;
                // Initial temperature
                if (initialCondition is InitialTemperature)
                {
                    name = FeMesh.GetNextFreeSelectionName(_model.Mesh.NodeSets, initialCondition.Name);
                    FeNodeSet nodeSet = new FeNodeSet(name, initialCondition.CreationIds);
                    nodeSet.CreationData = initialCondition.CreationData.DeepClone();
                    nodeSet.Internal = true;
                    AddNodeSet(nodeSet);
                    //
                    initialCondition.RegionName = name;
                    initialCondition.RegionType = RegionTypeEnum.NodeSetName;
                }
                // Initial velocity
                else if (initialCondition is InitialVelocity)
                {
                    name = FeMesh.GetNextFreeSelectionName(_model.Mesh.ElementSets, initialCondition.Name);
                    // For
                    FeElementSet elementSet = new FeElementSet(name, initialCondition.CreationIds, true);
                    elementSet.CreationData = initialCondition.CreationData.DeepClone();
                    elementSet.Internal = true;
                    AddElementSet(elementSet);
                    //
                    initialCondition.RegionName = name;
                    initialCondition.RegionType = RegionTypeEnum.ElementSetName;
                }
                else throw new NotSupportedException();
            }
            // Clear the creation data if not used
            else
            {
                initialCondition.CreationData = null;
                initialCondition.CreationIds = null;
            }
        }
        private void DeleteSelectionBasedInitialConditionSets(string oldInitialConditionName)
        {
            // Delete previously created sets
            InitialCondition initialCondition = GetInitialCondition(oldInitialConditionName);
            if (initialCondition.CreationData != null && initialCondition.RegionName != null)
            {
                if (initialCondition is InitialTemperature)
                    RemoveNodeSets(new string[] { initialCondition.RegionName });
                else if (initialCondition is InitialVelocity)
                    RemoveElementSets(new string[] { initialCondition.RegionName });
                else throw new NotSupportedException();
            }
        }

        #endregion #################################################################################################################

        #region Step menu   ########################################################################################################
        // COMMANDS ********************************************************************************
        public void AddStepCommand(Step step, bool copyBCsAndLoads)
        {
            Commands.CAddStep comm = new Commands.CAddStep(step.DeepClone(), copyBCsAndLoads);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceStepCommand(string oldStepName, Step newStep)
        {
            Commands.CReplaceStep comm = new Commands.CReplaceStep(oldStepName, newStep);
            _commands.AddAndExecute(comm);
        }
        public void DuplicateStepsCommand(string[] stepNames)
        {
            Commands.CDuplicateSteps comm = new Commands.CDuplicateSteps(stepNames);
            _commands.AddAndExecute(comm);
        }
        public void RemoveStepsCommand(string[] stepNames)
        {
            Commands.CRemoveSteps comm = new Commands.CRemoveSteps(stepNames);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public string[] GetStepNames()
        {
            return _model.StepCollection.GetStepNames();
        }
        public void AddStep(Step step, bool copyBCsAndLoads)
        {
            // Create the default analysis the first time a step is added
            if (_model.StepCollection.StepsList.Count == 0 && _jobs.Count == 0)
            {
                AnalysisJob job = _form.GetDefaultJob();
                if (job != null) AddJob(job);
            }
            //
            _model.StepCollection.AddStep(step, copyBCsAndLoads);
            _form.AddTreeNode(ViewGeometryModelResults.Model, step, null);
            //
            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public Step GetStep(string stepName)
        {
            return _model.StepCollection.GetStep(stepName);
        }
        public Step[] GetAllSteps()
        {
            return _model.StepCollection.StepsList.ToArray();
        }
        public void ReplaceStep(string oldStepName, Step newStep)
        {
            _model.StepCollection.ReplaceStep(oldStepName, newStep);
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldStepName, newStep, null);
            //
            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void DuplicateSteps(string[] stepNames)
        {
            Step newStep;
            foreach (var stepName in stepNames)
            {
                newStep = GetStep(stepName).DeepClone();
                newStep.Name = NamedClass.GetNameWithoutLastValue(newStep.Name);
                newStep.Name = GetStepNames().GetNextNumberedKey(newStep.Name);
                AddStep(newStep, false);
            }
        }
        public void ActivateDeactivateStep(string stepName, bool active)
        {
            Step step = _model.StepCollection.GetStep(stepName);
            step.Active = active;
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, stepName, step, null);
            //
            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void RemoveSteps(string[] stepNames)
        {
            Step step;
            foreach (var name in stepNames)
            {
                step = _model.StepCollection.GetStep(name);
                RemoveHistoryOutputs(name, step.HistoryOutputs.Keys.ToArray());
                RemoveFieldOutputs(name, step.FieldOutputs.Keys.ToArray());
                RemoveBoundaryConditions(name, step.BoundaryConditions.Keys.ToArray());
                RemoveLoads(name, step.Loads.Keys.ToArray());
                _model.StepCollection.RemoveStep(name);
                _form.RemoveTreeNode<Step>(ViewGeometryModelResults.Model, name, null);
            }
            //
            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }

        #endregion #################################################################################################################

        #region History output menu   ##############################################################################################
        // COMMANDS ********************************************************************************
        public void AddHistoryOutputCommand(string stepName, HistoryOutput historyOutput)
        {
            CAddHistoryOutput comm = new CAddHistoryOutput(stepName, historyOutput);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceHistoryOutputCommand(string stepName, string oldHistoryOutputName, HistoryOutput historyOutput)
        {
            CReplaceHisotryOutput comm = new CReplaceHisotryOutput(stepName, oldHistoryOutputName, historyOutput);
            _commands.AddAndExecute(comm);
        }
        public void DuplicateHistoryOutputsCommand(string stepName, string[] historyOutputNames)
        {
            CDuplicateHistoryOutputs comm = new CDuplicateHistoryOutputs(stepName, historyOutputNames);
            _commands.AddAndExecute(comm);
        }
        public void PropagateHistoryOutputCommand(string stepName, string historyOutputName)
        {
            CPropagateHisotryOutput comm = new CPropagateHisotryOutput(stepName, historyOutputName);
            _commands.AddAndExecute(comm);
        }
        public void RemoveHistoryOutputsCommand(string stepName, string[] historyOutputNames)
        {
            CRemoveHistoryOutputs comm = new CRemoveHistoryOutputs(stepName, historyOutputNames);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public string[] GetHistoryOutputNamesForStep(string stepName)
        {
            return _model.StepCollection.GetStep(stepName).HistoryOutputs.Keys.ToArray();
        }
        public void AddHistoryOutput(string stepName, HistoryOutput historyOutput)
        {
            if (!_model.StepCollection.MultiRegionSelectionExists(stepName, historyOutput))
                ConvertSelectionBasedHistoryOutput(historyOutput);
            //
            _model.StepCollection.AddHistoryOutput(historyOutput, stepName);
            _form.AddTreeNode(ViewGeometryModelResults.Model, historyOutput, stepName);
            //
            CheckAndUpdateModelValidity();
        }
        public HistoryOutput GetHistoryOutput(string stepName, string historyOutputName)
        {
            return _model.StepCollection.GetStep(stepName).HistoryOutputs[historyOutputName]; ;
        }
        public HistoryOutput[] GetAllHistoryOutputs(string stepName)
        {
            return _model.StepCollection.GetStep(stepName).HistoryOutputs.Values.ToArray();
        }
        public void ReplaceHistoryOutput(string stepName, string oldHistoryOutputName, HistoryOutput historyOutput,
                                         bool propagated = false)
        {
            HistoryOutput oldHistoryOutput = GetHistoryOutput(stepName, oldHistoryOutputName);
            // First check for a valid region since MultiRegionChanged changes the region type and region name
            if ((!_model.RegionValid(oldHistoryOutput) || StepCollection.MultiRegionChanged(oldHistoryOutput, historyOutput)) &&
                !propagated)
            {
                DeleteSelectionBasedHistoryOutputSets(stepName, oldHistoryOutputName);
                ConvertSelectionBasedHistoryOutput(historyOutput);
            }
            //
            _model.StepCollection.GetStep(stepName).HistoryOutputs.Replace(oldHistoryOutputName, historyOutput.Name,
                                                                           historyOutput);
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldHistoryOutputName, historyOutput, stepName);
            //
            CheckAndUpdateModelValidity();
        }
        public void DuplicateHistoryOutputs(string stepName, string[] historyOutputNames)
        {
            HistoryOutput newHistoryOutput;
            for (int i = 0; i < historyOutputNames.Length; i++)
            {
                newHistoryOutput = GetHistoryOutput(stepName, historyOutputNames[i]).DeepClone();
                newHistoryOutput.Name = NamedClass.GetNameWithoutLastValue(newHistoryOutput.Name);
                newHistoryOutput.Name =
                    _model.StepCollection.GetStepHistoryOutputNames(stepName).GetNextNumberedKey(newHistoryOutput.Name);
                if (newHistoryOutput.CreationData != null) newHistoryOutput.RegionType = RegionTypeEnum.Selection;
                AddHistoryOutput(stepName, newHistoryOutput);
            }
        }
        public void PropagateHistoryOutput(string stepName, string historyOutputName)
        {
            string[] nextStepNames = _model.StepCollection.GetNextStepNames(stepName);
            HistoryOutput historyOutput = GetHistoryOutput(stepName, historyOutputName).DeepClone();
            Step step;
            foreach (var nextStepName in nextStepNames)
            {
                step = _model.StepCollection.GetStep(nextStepName);
                //
                if (step.HistoryOutputs.ContainsKey(historyOutputName))
                    ReplaceHistoryOutput(nextStepName, historyOutputName, historyOutput, true);
                else
                    AddHistoryOutput(nextStepName, historyOutput);
            }

        }
        public void ActivateDeactivateHistoryOutput(string stepName, string historyOutputName, bool active)
        {
            HistoryOutput historyOutput = _model.StepCollection.GetStep(stepName).HistoryOutputs[historyOutputName];
            historyOutput.Active = active;
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, historyOutputName, historyOutput, stepName);
            //
            CheckAndUpdateModelValidity();
        }
        public void RemoveHistoryOutputs(string stepName, string[] historyOutputNames)
        {
            foreach (var name in historyOutputNames)
            {
                DeleteSelectionBasedHistoryOutputSets(stepName, name);
                _model.StepCollection.GetStep(stepName).HistoryOutputs.Remove(name);
                _form.RemoveTreeNode<HistoryOutput>(ViewGeometryModelResults.Model, name, stepName);
            }
            //
            CheckAndUpdateModelValidity();
        }
        //
        private void ConvertSelectionBasedHistoryOutput(HistoryOutput historyOutput)
        {
            // Create a named set and convert a selection to a named set
            if (historyOutput.RegionType == RegionTypeEnum.Selection)
            {
                string name;
                // Node output
                if (historyOutput is NodalHistoryOutput)
                {
                    name = FeMesh.GetNextFreeSelectionName(_model.Mesh.NodeSets, historyOutput.Name);
                    FeNodeSet nodeSet = new FeNodeSet(name, historyOutput.CreationIds);
                    nodeSet.CreationData = historyOutput.CreationData.DeepClone();
                    nodeSet.Internal = true;
                    AddNodeSet(nodeSet);
                    //
                    historyOutput.RegionName = name;
                    historyOutput.RegionType = RegionTypeEnum.NodeSetName;
                }
                // Element output
                else if (historyOutput is ElementHistoryOutput)
                {
                    name = FeMesh.GetNextFreeSelectionName(_model.Mesh.ElementSets, historyOutput.Name);
                    FeElementSet elementSet = new FeElementSet(name, historyOutput.CreationIds);
                    elementSet.CreationData = historyOutput.CreationData.DeepClone();
                    elementSet.Internal = true;
                    AddElementSet(elementSet);
                    //
                    historyOutput.RegionName = name;
                    historyOutput.RegionType = RegionTypeEnum.ElementSetName;
                }
                else throw new NotSupportedException();
            }
            // Clear the creation data if not used
            else
            {
                historyOutput.CreationData = null;
                historyOutput.CreationIds = null;
            }
        }
        private void DeleteSelectionBasedHistoryOutputSets(string stepName, string oldHistoryOutputName)
        {
            HistoryOutput historyOutput = GetHistoryOutput(stepName, oldHistoryOutputName);
            //
            Dictionary<string, int> regionsCount = _model.StepCollection.GetHistoryOutputRegionsCount();
            // Delete previously created sets
            if (historyOutput.CreationData != null && historyOutput.RegionName != null &&
                regionsCount[historyOutput.RegionName] == 1)
            {
                if (historyOutput is NodalHistoryOutput) RemoveNodeSets(new string[] { historyOutput.RegionName });
                else if (historyOutput is ElementHistoryOutput) RemoveElementSets(new string[] { historyOutput.RegionName });
                else throw new NotSupportedException();
            }
        }
        #endregion #################################################################################################################

        #region Field output menu   ################################################################################################
        // COMMANDS ********************************************************************************
        public void AddFieldOutputCommand(string stepName, FieldOutput fieldOutput)
        {
            CAddFieldOutput comm = new CAddFieldOutput(stepName, fieldOutput);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceFieldOutputCommand(string stepName, string oldFieldOutputName, FieldOutput fieldOutput)
        {
            CReplaceFieldOutput comm = new CReplaceFieldOutput(stepName, oldFieldOutputName, fieldOutput);
            _commands.AddAndExecute(comm);
        }
        public void DuplicateFieldOutputsCommand(string stepName, string[] fieldOutputNames)
        {
            CDuplicateFieldOutputs comm = new CDuplicateFieldOutputs(stepName, fieldOutputNames);
            _commands.AddAndExecute(comm);
        }
        public void PropagateFieldOutputCommand(string stepName, string fieldOutputName)
        {
            CPropagateFieldOutput comm = new CPropagateFieldOutput(stepName, fieldOutputName);
            _commands.AddAndExecute(comm);
        }
        public void RemoveFieldOutputsCommand(string stepName, string[] fieldOutputNames)
        {
            CRemoveFieldOutputs comm = new CRemoveFieldOutputs(stepName, fieldOutputNames);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public string[] GetFieldOutputNamesForStep(string stepName)
        {
            return _model.StepCollection.GetStep(stepName).FieldOutputs.Keys.ToArray();
        }
        public void AddFieldOutput(string stepName, FieldOutput fieldOutput)
        {
            _model.StepCollection.AddFieldOutput(fieldOutput, stepName);
            _form.AddTreeNode(ViewGeometryModelResults.Model, fieldOutput, stepName);

            CheckAndUpdateModelValidity();
        }
        public FieldOutput GetFieldOutput(string stepName, string fieldOutputName)
        {
            return _model.StepCollection.GetStep(stepName).FieldOutputs[fieldOutputName];
        }
        public FieldOutput[] GetAllFieldOutputs(string stepName)
        {
            return _model.StepCollection.GetStep(stepName).FieldOutputs.Values.ToArray();
        }
        public void ReplaceFieldOutput(string stepName, string oldFieldOutputName, FieldOutput fieldOutput)
        {
            _model.StepCollection.GetStep(stepName).FieldOutputs.Replace(oldFieldOutputName, fieldOutput.Name, fieldOutput);
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldFieldOutputName, fieldOutput, stepName);
            //
            CheckAndUpdateModelValidity();
        }
        public void DuplicateFieldOutputs(string stepName, string[] fieldOutputNames)
        {
            FieldOutput newFieldOutput;
            for (int i = 0; i < fieldOutputNames.Length; i++)
            {
                newFieldOutput = GetFieldOutput(stepName, fieldOutputNames[i]).DeepClone();
                newFieldOutput.Name = NamedClass.GetNameWithoutLastValue(newFieldOutput.Name);
                newFieldOutput.Name =
                    _model.StepCollection.GetStepFieldOutputNames(stepName).GetNextNumberedKey(newFieldOutput.Name);
                AddFieldOutput(stepName, newFieldOutput);
            }
        }
        public void PropagateFieldOutput(string stepName, string fieldOutputName)
        {
            string[] nextStepNames = _model.StepCollection.GetNextStepNames(stepName);
            FieldOutput fieldOutput = GetFieldOutput(stepName, fieldOutputName).DeepClone();
            Step step;
            foreach (var nextStepName in nextStepNames)
            {
                step = _model.StepCollection.GetStep(nextStepName);
                //
                if (step.FieldOutputs.ContainsKey(fieldOutputName))
                    ReplaceFieldOutput(nextStepName, fieldOutputName, fieldOutput);
                else
                    AddFieldOutput(nextStepName, fieldOutput);
            }
        }
        public void ActivateDeactivateFieldOutput(string stepName, string fieldOutputName, bool active)
        {
            FieldOutput fieldOutput = _model.StepCollection.GetStep(stepName).FieldOutputs[fieldOutputName];
            fieldOutput.Active = active;
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, fieldOutputName, fieldOutput, stepName);
            //
            CheckAndUpdateModelValidity();
        }
        public void RemoveFieldOutputs(string stepName, string[] fieldOutputNames)
        {
            foreach (var name in fieldOutputNames)
            {
                _model.StepCollection.GetStep(stepName).FieldOutputs.Remove(name);
                _form.RemoveTreeNode<FieldOutput>(ViewGeometryModelResults.Model, name, stepName);
            }

            CheckAndUpdateModelValidity();
        }

        #endregion #################################################################################################################

        #region Boundary condition menu   ##########################################################################################
        // COMMANDS ********************************************************************************
        public void AddBoundaryConditionCommand(string stepName, BoundaryCondition boundaryCondition)
        {
            CAddBC comm = new CAddBC(stepName, boundaryCondition);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceBoundaryConditionCommand(string stepName, string oldBoundaryConditionName,
                                                    BoundaryCondition boundaryCondition)
        {
            CReplaceBC comm = new CReplaceBC(stepName, oldBoundaryConditionName, boundaryCondition);
            _commands.AddAndExecute(comm);
        }
        public void DuplicateBoundaryConditionsCommand(string stepName, string[] boundaryConditionNames)
        {
            CDuplicateBCs comm = new CDuplicateBCs(stepName, boundaryConditionNames);
            _commands.AddAndExecute(comm);
        }
        public void PropagateBoundaryConditionCommand(string stepName, string boundaryConditionName)
        {
            CPropagateBC comm = new CPropagateBC(stepName, boundaryConditionName);
            _commands.AddAndExecute(comm);
        }
        public void HideBoundaryConditionCommand(string stepName, string[] boundaryConditionNames)
        {
            CHideBCs comm = new CHideBCs(stepName, boundaryConditionNames);
            _commands.AddAndExecute(comm);
        }
        public void ShowBoundaryConditionCommand(string stepName, string[] boundaryConditionNames)
        {
            CShowBCs comm = new CShowBCs(stepName, boundaryConditionNames);
            _commands.AddAndExecute(comm);
        }
        public void RemoveBoundaryConditionsCommand(string stepName, string[] boundaryConditionNames)
        {
            CRemoveBCs comm = new CRemoveBCs(stepName, boundaryConditionNames);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public string[] GetAllBoundaryConditionNames()
        {
            return _model.StepCollection.GetAllBoundaryConditionNames();
        }
        public BoundaryCondition GetBoundaryCondition(string stepName, string boundaryConditionName)
        {
            return _model.StepCollection.GetStep(stepName).BoundaryConditions[boundaryConditionName];
        }
        public BoundaryCondition[] GetStepBoundaryConditions(string stepName)
        {
            return _model.StepCollection.GetStep(stepName).BoundaryConditions.Values.ToArray();
        }
        //
        public void AddBoundaryCondition(string stepName, BoundaryCondition boundaryCondition)
        {
            if (!_model.StepCollection.MultiRegionSelectionExists(stepName, boundaryCondition))
                ConvertSelectionBasedBoundaryCondition(boundaryCondition);
            //
            if (_model.StepCollection.GetStep(stepName).AddBoundaryCondition(boundaryCondition))
            {
                _form.AddTreeNode(ViewGeometryModelResults.Model, boundaryCondition, stepName);
                //
                FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
            }
        }
        public void ReplaceBoundaryCondition(string stepName, string oldBoundaryConditionName,
                                             BoundaryCondition boundaryCondition, bool propagated = false)
        {
            BoundaryCondition oldBC = GetBoundaryCondition(stepName, oldBoundaryConditionName);
            // First check for a valid region since MultiRegionChanged changes the region type and region name
            if ((!_model.RegionValid(oldBC) || StepCollection.MultiRegionChanged(oldBC, boundaryCondition)) && !propagated)
            {
                DeleteSelectionBasedBoundaryConditionSets(stepName, oldBoundaryConditionName);
                ConvertSelectionBasedBoundaryCondition(boundaryCondition);
            }
            //
            if (_model.StepCollection.GetStep(stepName).ReplaceBoundaryCondition(oldBoundaryConditionName, boundaryCondition))
            {
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldBoundaryConditionName, boundaryCondition, stepName);
                //
                FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
            }
        }
        public void DuplicateBoundaryConditions(string stepName, string[] boundaryConditionNames)
        {
            BoundaryCondition newBoundaryCondition;
            for (int i = 0; i < boundaryConditionNames.Length; i++)
            {
                newBoundaryCondition = GetBoundaryCondition(stepName, boundaryConditionNames[i]).DeepClone();
                newBoundaryCondition.Name = NamedClass.GetNameWithoutLastValue(newBoundaryCondition.Name);
                newBoundaryCondition.Name =
                    _model.StepCollection.GetStepBoundaryConditionNames(stepName).GetNextNumberedKey(newBoundaryCondition.Name);
                if (newBoundaryCondition.CreationData != null) newBoundaryCondition.RegionType = RegionTypeEnum.Selection;
                AddBoundaryCondition(stepName, newBoundaryCondition);
            }
        }
        public void PropagateBoundaryCondition(string stepName, string boundaryConditionName)
        {
            string[] nextStepNames = _model.StepCollection.GetNextStepNames(stepName);
            BoundaryCondition boundaryCondition = GetBoundaryCondition(stepName, boundaryConditionName);
            BoundaryCondition boundaryConditionClone;
            foreach (var nextStepName in nextStepNames)
            {
                boundaryConditionClone = boundaryCondition.DeepClone();
                if (_model.StepCollection.GetStep(nextStepName).BoundaryConditions.ContainsKey(boundaryConditionName))
                    ReplaceBoundaryCondition(nextStepName, boundaryConditionName, boundaryConditionClone, true);
                else
                    AddBoundaryCondition(nextStepName, boundaryConditionClone);
            }
        }
        public void HideBoundaryConditions(string stepName, string[] boundaryConditionNames)
        {
            foreach (var name in boundaryConditionNames)
            {
                _model.StepCollection.GetStep(stepName).BoundaryConditions[name].Visible = false;
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, name,
                                     _model.StepCollection.GetStep(stepName).BoundaryConditions[name], stepName, false);
            }
            FeModelUpdate(UpdateType.RedrawSymbols);
        }
        public void ShowBoundaryConditions(string stepName, string[] boundaryConditionNames)
        {
            foreach (var name in boundaryConditionNames)
            {
                _model.StepCollection.GetStep(stepName).BoundaryConditions[name].Visible = true;
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, name,
                                     _model.StepCollection.GetStep(stepName).BoundaryConditions[name], stepName, false);
            }
            FeModelUpdate(UpdateType.RedrawSymbols);
        }
        public void ActivateDeactivateBoundaryCondition(string stepName, string boundaryConditionName, bool active)
        {
            BoundaryCondition boundaryCondition =
                _model.StepCollection.GetStep(stepName).BoundaryConditions[boundaryConditionName];
            //
            boundaryCondition.Active = active;
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, boundaryConditionName, boundaryCondition, stepName);
            //
            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void RemoveBoundaryConditions(string stepName, string[] boundaryConditionNames)
        {
            foreach (var name in boundaryConditionNames)
            {
                DeleteSelectionBasedBoundaryConditionSets(stepName, name);
                _model.StepCollection.GetStep(stepName).BoundaryConditions.Remove(name);
                _form.RemoveTreeNode<BoundaryCondition>(ViewGeometryModelResults.Model, name, stepName);
            }
            //
            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        //
        private void ConvertSelectionBasedBoundaryCondition(BoundaryCondition boundaryCondition)
        {
            // Create a named set and convert a selection to a named set
            if (boundaryCondition.RegionType == RegionTypeEnum.Selection)
            {
                string name;
                // Node set
                if (boundaryCondition is FixedBC || boundaryCondition is DisplacementRotation ||
                    boundaryCondition is SubmodelBC || boundaryCondition is TemperatureBC)
                {
                    name = FeMesh.GetNextFreeSelectionName(_model.Mesh.NodeSets, boundaryCondition.Name);
                    FeNodeSet nodeSet = new FeNodeSet(name, boundaryCondition.CreationIds);
                    nodeSet.CreationData = boundaryCondition.CreationData.DeepClone();
                    nodeSet.Internal = true;
                    AddNodeSet(nodeSet);
                    //
                    boundaryCondition.RegionName = name;
                    boundaryCondition.RegionType = RegionTypeEnum.NodeSetName;
                }
                else throw new NotSupportedException();

            }
            // Clear the creation data if not used
            else
            {
                boundaryCondition.CreationData = null;
                boundaryCondition.CreationIds = null;
            }
        }
        private void DeleteSelectionBasedBoundaryConditionSets(string stepName, string oldBoundaryConditionName)
        {
            BoundaryCondition boundaryCondition = GetBoundaryCondition(stepName, oldBoundaryConditionName);
            //
            Dictionary<string, int> regionsCount = _model.StepCollection.GetBoundaryConditionRegionsCount();
            // Delete previously created sets
            if (boundaryCondition.CreationData != null && boundaryCondition.RegionName != null &&
                regionsCount[boundaryCondition.RegionName] == 1)
            {
                if (boundaryCondition is FixedBC || boundaryCondition is DisplacementRotation ||
                    boundaryCondition is SubmodelBC || boundaryCondition is TemperatureBC)
                    RemoveNodeSets(new string[] { boundaryCondition.RegionName });
                else throw new NotSupportedException();
            }
        }

        #endregion #################################################################################################################

        #region Load menu   ########################################################################################################
        // COMMANDS ********************************************************************************
        public void AddLoadCommand(string stepName, Load load)
        {
            Commands.CAddLoad comm = new Commands.CAddLoad(stepName, load);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceLoadCommand(string stepName, string oldLoadName, Load load)
        {
            Commands.CReplaceLoad comm = new Commands.CReplaceLoad(stepName, oldLoadName, load);
            _commands.AddAndExecute(comm);
        }
        public void DuplicateLoadsCommand(string stepName, string[] loadNames)
        {
            Commands.CDuplicateLoads comm = new Commands.CDuplicateLoads(stepName, loadNames);
            _commands.AddAndExecute(comm);
        }
        public void PropagateLoadCommand(string stepName, string loadName)
        {
            Commands.CPropagateLoad comm = new Commands.CPropagateLoad(stepName, loadName);
            _commands.AddAndExecute(comm);
        }
        public void HideLoadsCommand(string stepName, string[] loadNames)
        {
            Commands.CHideLoads comm = new Commands.CHideLoads(stepName, loadNames);
            _commands.AddAndExecute(comm);
        }
        public void ShowLoadsCommand(string stepName, string[] loadNames)
        {
            Commands.CShowLoads comm = new Commands.CShowLoads(stepName, loadNames);
            _commands.AddAndExecute(comm);
        }
        public void RemoveLoadsCommand(string stepName, string[] loadNames)
        {
            Commands.CRemoveLoads comm = new Commands.CRemoveLoads(stepName, loadNames);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public string[] GetAllLoadNames()
        {
            return _model.StepCollection.GetAllLoadNames();
        }
        public Load GetLoad(string stepName, string loadName)
        {
            return _model.StepCollection.GetStep(stepName).Loads[loadName];
        }
        public Load[] GetStepLoads(string stepName)
        {
            return _model.StepCollection.GetStep(stepName).Loads.Values.ToArray();
        }
        //
        public void AddLoad(string stepName, Load load)
        {
            if (!_model.StepCollection.MultiRegionSelectionExists(stepName, load))
                ConvertSelectionBasedLoad(load);
            //
            if (_model.StepCollection.GetStep(stepName).AddLoad(load))
            {
                _form.AddTreeNode(ViewGeometryModelResults.Model, load, stepName);
                //
                FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
            }
        }
        public void ReplaceLoad(string stepName, string oldLoadName, Load load, bool propagated = false)
        {
            Load oldLoad = GetLoad(stepName, oldLoadName);
            // First check for a valid region since MultiRegionChanged changes the region type and region name
            if ((!_model.RegionValid(oldLoad) || StepCollection.MultiRegionChanged(oldLoad, load)) && !propagated)
            {
                DeleteSelectionBasedLoadSets(stepName, oldLoadName);
                ConvertSelectionBasedLoad(load);
            }
            //
            if (_model.StepCollection.GetStep(stepName).ReplaceLoad(oldLoadName, load))
            {
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldLoadName, load, stepName);
                //
                FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
            }
        }
        public void DuplicateLoads(string stepName, string[] loadNames)
        {
            Load newLoad;
            for (int i = 0; i < loadNames.Length; i++)
            {
                newLoad = GetLoad(stepName, loadNames[i]).DeepClone();
                newLoad.Name = NamedClass.GetNameWithoutLastValue(newLoad.Name);
                newLoad.Name = _model.StepCollection.GetStepLoadNames(stepName).GetNextNumberedKey(newLoad.Name);
                if (newLoad.CreationData != null) newLoad.RegionType = RegionTypeEnum.Selection;
                AddLoad(stepName, newLoad);
            }
        }
        public void PropagateLoad(string stepName, string loadName)
        {
            string[] nextStepNames = _model.StepCollection.GetNextStepNames(stepName);
            Load load = GetLoad(stepName, loadName);
            Load loadClone;
            foreach (var nextStepName in nextStepNames)
            {
                loadClone = load.DeepClone();
                if (_model.StepCollection.GetStep(nextStepName).Loads.ContainsKey(loadName))
                    ReplaceLoad(nextStepName, loadName, loadClone, true);
                else
                    AddLoad(nextStepName, loadClone);
            }
        }
        public void PreviewLoad(string stepName, string loadName)
        {
            Load load = GetLoad(stepName, loadName);
            if (load != null)
            {
                FeResults results;
                if (load is DLoad dl)
                {
                    results = dl.GetPreview(_model.Mesh, stepName + "_" + loadName, _model.UnitSystem.UnitSystemType);
                }
                else if (load is ImportedPressure ip)
                {
                    results = ip.GetPreview(_model.Mesh, stepName + "_" + loadName, _model.UnitSystem.UnitSystemType);
                }
                else if (load is HydrostaticPressure hp)
                {
                    results = hp.GetPreview(_model.Mesh, stepName + "_" + loadName, _model.UnitSystem.UnitSystemType);
                }
                else throw new CaeException("It is not possible to preview this load type.");
                //
                SetResults(results);
            }
        }
        public void HideLoads(string stepName, string[] loadNames)
        {
            foreach (var name in loadNames)
            {
                _model.StepCollection.GetStep(stepName).Loads[name].Visible = false;
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, name, _model.StepCollection.GetStep(stepName).Loads[name],
                                     stepName, false);
            }
            FeModelUpdate(UpdateType.RedrawSymbols);
        }
        public void ShowLoads(string stepName, string[] loadNames)
        {
            foreach (var name in loadNames)
            {
                _model.StepCollection.GetStep(stepName).Loads[name].Visible = true;
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, name, _model.StepCollection.GetStep(stepName).Loads[name],
                                     stepName, false);
            }
            FeModelUpdate(UpdateType.RedrawSymbols);
        }
        public void ActivateDeactivateLoad(string stepName, string loadName, bool active)
        {
            Load load = _model.StepCollection.GetStep(stepName).Loads[loadName];
            load.Active = active;
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, loadName, load, stepName);
            //
            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void RemoveLoads(string stepName, string[] loadNames)
        {
            foreach (var name in loadNames)
            {
                DeleteSelectionBasedLoadSets(stepName, name);
                _model.StepCollection.GetStep(stepName).Loads.Remove(name);
                _form.RemoveTreeNode<Load>(ViewGeometryModelResults.Model, name, stepName);
            }
            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        //
        private void ConvertSelectionBasedLoad(Load load)
        {
            // Create a named set and convert a selection to a named set
            if (load.RegionType == RegionTypeEnum.Selection)
            {
                string name;
                // Node set
                if (load is CLoad || load is MomentLoad || load is CFlux)
                {
                    name = FeMesh.GetNextFreeSelectionName(_model.Mesh.NodeSets, load.Name);
                    FeNodeSet nodeSet = new FeNodeSet(name, load.CreationIds);
                    nodeSet.CreationData = load.CreationData.DeepClone();
                    nodeSet.Internal = true;
                    AddNodeSet(nodeSet);
                    //
                    load.RegionName = name;
                    load.RegionType = RegionTypeEnum.NodeSetName;
                }
                // Element set from parts
                else if (load is GravityLoad || load is CentrifLoad || load is BodyFlux)
                {
                    name = FeMesh.GetNextFreeSelectionName(_model.Mesh.ElementSets, load.Name);
                    FeElementSet elementSet = new FeElementSet(name, load.CreationIds, true);
                    elementSet.CreationData = load.CreationData.DeepClone();
                    elementSet.Internal = true;
                    AddElementSet(elementSet);
                    //
                    load.RegionName = name;
                    load.RegionType = RegionTypeEnum.ElementSetName;
                }
                // Surface
                else if (load is DLoad || load is HydrostaticPressure || load is ImportedPressure || load is STLoad ||
                         load is ShellEdgeLoad || load is PreTensionLoad || load is DFlux || load is FilmHeatTransfer ||
                         load is RadiationHeatTransfer)
                {
                    name = FeMesh.GetNextFreeSelectionName(_model.Mesh.Surfaces, load.Name);
                    FeSurface surface = new FeSurface(name, load.CreationIds, load.CreationData.DeepClone());
                    surface.Internal = true;
                    AddSurface(surface);
                    //
                    load.RegionName = name;
                    load.RegionType = RegionTypeEnum.SurfaceName;
                }
                else throw new NotSupportedException();
            }
            // Clear the creation data if not used
            else
            {
                load.CreationData = null;
                load.CreationIds = null;
            }
        }
        private void DeleteSelectionBasedLoadSets(string stepName, string oldLoadName)
        {
            Load load = GetLoad(stepName, oldLoadName);
            //
            Dictionary<string, int> regionsCount = _model.StepCollection.GetLoadRegionsCount();
            // Delete previously created sets
            if (load.CreationData != null && load.RegionName != null && regionsCount[load.RegionName] == 1)
            {
                if (load is CLoad || load is MomentLoad || load is CFlux)
                    RemoveNodeSets(new string[] { load.RegionName });
                else if (load is GravityLoad || load is CentrifLoad || load is BodyFlux)
                    RemoveElementSets(new string[] { load.RegionName });
                else if (load is DLoad || load is HydrostaticPressure || load is ImportedPressure || load is STLoad ||
                         load is ShellEdgeLoad || load is PreTensionLoad || load is DFlux || load is FilmHeatTransfer ||
                         load is RadiationHeatTransfer)
                    RemoveSurfaces(new string[] { load.RegionName }, false);
                else throw new NotSupportedException();
            }
        }

        #endregion #################################################################################################################

        #region Defined field menu   ###############################################################################################
        // COMMANDS ********************************************************************************
        public void AddDefinedFieldCommand(string stepName, DefinedField definedField)
        {
            CAddDefinedField comm = new CAddDefinedField(stepName, definedField);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceDefinedFieldCommand(string stepName, string oldDefinedFieldName, DefinedField definedField)
        {
            CReplaceDefinedField comm = new CReplaceDefinedField(stepName, oldDefinedFieldName, definedField);
            _commands.AddAndExecute(comm);
        }
        public void DuplicateDefinedFieldsForStepCommand(string stepName, string[] definedFieldNames)
        {
            CDuplicateDefinedFields comm = new CDuplicateDefinedFields(stepName, definedFieldNames);
            _commands.AddAndExecute(comm);
        }
        public void PropagateDefinedFieldCommand(string stepName, string definedFieldName)
        {
            CPropagateDefinedField comm = new CPropagateDefinedField(stepName, definedFieldName);
            _commands.AddAndExecute(comm);
        }
        public void RemoveDefinedFieldsForStepCommand(string stepName, string[] definedFieldNames)
        {
            CRemoveDefinedFields comm = new CRemoveDefinedFields(stepName, definedFieldNames);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public string[] GetDefinedFieldNamesForStep(string stepName)
        {
            return _model.StepCollection.GetStep(stepName).DefinedFields.Keys.ToArray();
        }
        public DefinedField GetDefinedField(string stepName, string definedFieldName)
        {
            return _model.StepCollection.GetStep(stepName).DefinedFields[definedFieldName];
        }
        public DefinedField[] GetAllDefinedFields(string stepName)
        {
            return _model.StepCollection.GetStep(stepName).DefinedFields.Values.ToArray();
        }
        //
        public void AddDefinedField(string stepName, DefinedField definedField)
        {
            if (!_model.StepCollection.MultiRegionSelectionExists(stepName, definedField))
                ConvertSelectionBasedDefinedField(definedField);
            //
            _model.StepCollection.AddDefinedField(definedField, stepName);
            _form.AddTreeNode(ViewGeometryModelResults.Model, definedField, stepName);
            //
            CheckAndUpdateModelValidity();
        }
        public void ReplaceDefinedField(string stepName, string oldDefinedFieldName, DefinedField definedField,
                                        bool propagated = false)
        {
            DefinedField oldDefinedField = GetDefinedField(stepName, oldDefinedFieldName);
            // First check for a valid region since MultiRegionChanged changes the region type and region name
            if ((!_model.RegionValid(oldDefinedField) || StepCollection.MultiRegionChanged(oldDefinedField, definedField)) &&
                !propagated)
            {
                DeleteSelectionBasedDefinedFieldSets(stepName, oldDefinedFieldName);
                ConvertSelectionBasedDefinedField(definedField);
            }
            //
            _model.StepCollection.GetStep(stepName).DefinedFields.Replace(oldDefinedFieldName, definedField.Name, definedField);
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldDefinedFieldName, definedField, stepName);
            //
            CheckAndUpdateModelValidity();
        }
        public void DuplicateDefinedFields(string stepName, string[] definedFieldNames)
        {
            DefinedField newDefinedField;
            for (int i = 0; i < definedFieldNames.Length; i++)
            {
                newDefinedField = GetDefinedField(stepName, definedFieldNames[i]).DeepClone();
                newDefinedField.Name = NamedClass.GetNameWithoutLastValue(newDefinedField.Name);
                newDefinedField.Name =
                    _model.StepCollection.GetStepDefinedFieldNames(stepName).GetNextNumberedKey(newDefinedField.Name);
                if (newDefinedField.CreationData != null) newDefinedField.RegionType = RegionTypeEnum.Selection;
                AddDefinedField(stepName, newDefinedField);
            }
        }
        public void PropagateDefinedField(string stepName, string definedFieldName)
        {
            string[] nextStepNames = _model.StepCollection.GetNextStepNames(stepName);
            DefinedField definedField = GetDefinedField(stepName, definedFieldName).DeepClone();
            foreach (var nextStepName in nextStepNames)
            {
                if (_model.StepCollection.GetStep(nextStepName).DefinedFields.ContainsKey(definedFieldName))
                    ReplaceDefinedField(nextStepName, definedFieldName, definedField, true);
                else
                    AddDefinedField(nextStepName, definedField);
            }

        }
        public void PreviewDefinedField(string stepName, string definedFieldName)
        {
            DefinedField definedField = GetDefinedField(stepName, definedFieldName);
            if (definedField != null)
            {
                FeResults results;
                if (definedField is DefinedTemperature dt)
                {
                    results = dt.GetPreview(_model.Mesh, stepName + "_" + definedFieldName, _model.UnitSystem.UnitSystemType);
                }
                else throw new CaeException("It is not possible to preview this defined field type.");
                //
                SetResults(results);
            }
        }
        public void ActivateDeactivateDefinedField(string stepName, string definedFieldName, bool active)
        {
            DefinedField definedField = _model.StepCollection.GetStep(stepName).DefinedFields[definedFieldName];
            definedField.Active = active;
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, definedFieldName, definedField, stepName);
            //
            CheckAndUpdateModelValidity();
        }
        public void RemoveDefinedFields(string stepName, string[] definedFieldNames)
        {
            foreach (var name in definedFieldNames)
            {
                DeleteSelectionBasedDefinedFieldSets(stepName, name);
                _model.StepCollection.GetStep(stepName).DefinedFields.Remove(name);
                _form.RemoveTreeNode<DefinedField>(ViewGeometryModelResults.Model, name, stepName);
            }
            //
            CheckAndUpdateModelValidity();
        }
        //
        private void ConvertSelectionBasedDefinedField(DefinedField definedField)
        {
            // Create a named set and convert a selection to a named set
            if (definedField.RegionType == RegionTypeEnum.Selection)
            {
                string name;
                // Node output
                if (definedField is DefinedTemperature dt)
                {
                    if (dt.Type == DefinedTemperatureTypeEnum.ByValue)
                    {
                        name = FeMesh.GetNextFreeSelectionName(_model.Mesh.NodeSets, definedField.Name);
                        FeNodeSet nodeSet = new FeNodeSet(name, definedField.CreationIds);
                        nodeSet.CreationData = definedField.CreationData.DeepClone();
                        nodeSet.Internal = true;
                        AddNodeSet(nodeSet);
                        //
                        definedField.RegionName = name;
                        definedField.RegionType = RegionTypeEnum.NodeSetName;
                    }
                }
                else throw new NotSupportedException();
            }
            // Clear the creation data if not used
            else
            {
                definedField.CreationData = null;
                definedField.CreationIds = null;
            }
        }
        private void DeleteSelectionBasedDefinedFieldSets(string stepName, string oldDefinedFieldName)
        {
            DefinedField definedField = GetDefinedField(stepName, oldDefinedFieldName);
            //
            Dictionary<string, int> regionsCount = _model.StepCollection.GetDefinedFieldRegionsCount();
            // Delete previously created sets
            if (definedField.CreationData != null && definedField.RegionName != null &&
                regionsCount[definedField.RegionName] == 1)
            {
                if (definedField is DefinedTemperature) RemoveNodeSets(new string[] { definedField.RegionName });
                else throw new NotSupportedException();
            }
        }

        #endregion #################################################################################################################

        #region Settings menu   ####################################################################################################
        // COMMANDS ********************************************************************************
        public void SetNewModelPropertiesCommand(ModelSpaceEnum modelSpace, UnitSystemType unitSystemType)
        {
            Commands.CSetNewModelProperties comm = new Commands.CSetNewModelProperties(modelSpace, unitSystemType);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public void SetNewModelProperties(ModelSpaceEnum modelSpace, UnitSystemType unitSystemType)
        {
            _model.Properties.ModelSpace = modelSpace;
            _model.UnitSystem = new UnitSystem(unitSystemType);
            //
            _form.UpdateUnitSystem(_model.UnitSystem);
        }
        public void SetResultsUnitSystem(UnitSystemType unitSystemType)
        {
            _allResults.CurrentResult.UnitSystem = new UnitSystem(unitSystemType);
            //
            _form.UpdateUnitSystem(_allResults.CurrentResult.UnitSystem);
            //
            SetPostLegendAndStatusBlockSettings();
        }
        public string GetBrepFileName()
        {
            CalculixSettings settings = _settings.Calculix;
            if (settings.WorkDirectory == null || !Directory.Exists(settings.WorkDirectory))
            {
                MessageBoxes.ShowWorkDirectoryError();
                return null;
            }
            return Path.Combine(settings.WorkDirectory, Globals.BrepFileName);
        }
        private void ApplyModelUnitSystem()
        {
            _model.UnitSystem.SetConverterUnits();          // model and results units systems can be different
            _form.UpdateUnitSystem(_model.UnitSystem);      // model and results units systems can be different
        }
        private void ApplyResultsUnitSystem()
        {
            _allResults.CurrentResult.UnitSystem.SetConverterUnits();     // model and results units systems can be different
            _form.UpdateUnitSystem(_allResults.CurrentResult.UnitSystem); // model and results units systems can be different
        }
        //
        public void SetUndeformedModelType(UndeformedModelTypeEnum undeformedModelType)
        {
            _settings.Post.UndeformedModelType = undeformedModelType;
        }
        public UndeformedModelTypeEnum GetUndeformedModelType()
        {
            return _settings.Post.UndeformedModelType;
        }
        //
        public void ApplySettings()
        {
            // Called on property Settings Set when the user changes the setting values

            // General settings
            CaeMesh.Globals.EdgeAngle = _settings.General.EdgeAngle;
            // Graphics settings
            GraphicsSettings gs = _settings.Graphics;
            _form.SetBackground(gs.BackgroundType == BackgroundType.Gradient, gs.TopColor, gs.BottomColor, false);
            _form.SetCoorSysVisibility(gs.CoorSysVisibility);
            _form.SetScaleWidgetVisibility(gs.ScaleWidgetVisibility);
            _form.SetLighting(gs.AmbientComponent, gs.DiffuseComponent, false);
            _form.SetSmoothing(gs.PointSmoothing, gs.LineSmoothing, false);
            // Color settings
            CaeMesh.Globals.ColorTable = _settings.Color.ColorTable;
            // Pre-processing settings
            PreSettings ps = _settings.Pre;
            _form.SetHighlightColor(ps.PrimaryHighlightColor, ps.SecondaryHighlightColor);
            _form.SetMouseHighlightColor(ps.MouseHighlightColor);
            _form.SetDrawSymbolEdges(ps.DrawSymbolEdges);
            //
            _form.DrawColorBarBackground(ps.ColorBarBackgroundType == AnnotationBackgroundType.White);
            _form.DrawColorBarBorder(ps.ColorBarDrawBorder);
            // Job settings
            if (_jobs != null)
            {
                CalculixSettings cs = _settings.Calculix;
                foreach (var entry in _jobs)
                {
                    entry.Value.WorkDirectory = Settings.GetWorkDirectory();
                    entry.Value.Executable = cs.CalculixExe;
                    entry.Value.NumCPUs = cs.NumCPUs;
                    entry.Value.EnvironmentVariables = cs.EnvironmentVariables;
                }
            }
        }

        #endregion #################################################################################################################

        #region Parameters menu   ##################################################################################################
        // COMMANDS ********************************************************************************
        public void AddParameterCommand(EquationParameter parameter)
        {
            Commands.CAddParameter comm = new Commands.CAddParameter(parameter);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceParameterCommand(string oldParameterName, EquationParameter newParameter)
        {
            Commands.CReplaceParameter comm = new Commands.CReplaceParameter(oldParameterName, newParameter);
            _commands.AddAndExecute(comm);
        }
        public void RemoveParametersCommand(string[] parameterNames)
        {
            Commands.CRemoveParameters comm = new Commands.CRemoveParameters(parameterNames);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public void AddParameter(EquationParameter parameter)
        {
            _model.Parameters.Add(parameter.Name, parameter);
            UpdateNCalcParameters();
        }
        public void ReplaceParameter(string oldParameterName, EquationParameter parameter)
        {
            _model.Parameters.Replace(oldParameterName, parameter.Name, parameter);
            UpdateNCalcParameters();
        }
        public void RemoveParameters(string[] parameterNames)
        {
            foreach (var name in parameterNames) _model.Parameters.Remove(name);
            UpdateNCalcParameters();
        }
        //
        public void UpdateNCalcParameters()
        {
            _model.UpdateNCalcParameters();
            FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        #endregion #################################################################################################################

        #region Query menu   #######################################################################################################
        public string GetAnnotationText(string data)
        {
            // This must be in Controller - the _annotation object changes
            return _annotations.GetAnnotationText(data);
        }
        public string GetLengthUnit()
        {
            string unit;
            //
            if (_currentView == ViewGeometryModelResults.Geometry || _currentView == ViewGeometryModelResults.Model)
                unit = _model.UnitSystem.LengthUnitAbbreviation;
            else if (_currentView == ViewGeometryModelResults.Results)
            {
                if (_allResults != null && _allResults.CurrentResult != null)
                    unit = _allResults.CurrentResult.UnitSystem.LengthUnitAbbreviation;
                else
                    unit = _model.UnitSystem.LengthUnitAbbreviation;
            }
            else throw new NotSupportedException();
            //
            return unit;
        }
        public string GetAreaUnit()
        {
            string unit;
            //
            if (_currentView == ViewGeometryModelResults.Geometry || _currentView == ViewGeometryModelResults.Model)
                unit = _model.UnitSystem.AreaUnitAbbreviation;
            else if (_currentView == ViewGeometryModelResults.Results)
                unit = _allResults.CurrentResult.UnitSystem.AreaUnitAbbreviation;
            else throw new NotSupportedException();
            //
            return unit;
        }

        #endregion   ###############################################################################################################

        #region Analysis menu   ####################################################################################################
        // COMMANDS ********************************************************************************
        public void AddJobCommand(AnalysisJob job)
        {
            CAddJob comm = new CAddJob(job);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceJobCommand(string oldJobName, AnalysisJob job)
        {
            CReplaceJob comm = new CReplaceJob(oldJobName, job);
            _commands.AddAndExecute(comm);
        }
        public void DuplicateJobsCommand(string[] jobNames)
        {
            CDuplicateJobs comm = new CDuplicateJobs(jobNames);
            _commands.AddAndExecute(comm);
        }
        public void RemoveJobsCommand(string[] jobNames)
        {
            CRemoveJobs comm = new CRemoveJobs(jobNames);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public string[] GetJobNames()
        {
            return _jobs.Keys.ToArray();
        }
        public void AddJob(AnalysisJob job)
        {
            // Compatibility for version v0.7.0
            if (_jobs.ContainsKey(job.Name)) return;
            //
            _jobs.Add(job.Name, job);
            ApplySettings();
            _form.AddTreeNode(ViewGeometryModelResults.Model, job, null);
        }
        //
        public AnalysisJob GetJob(string jobName)
        {
            return _jobs[jobName];
        }
        public AnalysisJob[] GetAllJobs()
        {
            return _jobs.Values.ToArray();
        }
        public void ReplaceJob(string oldJobName, AnalysisJob job)
        {
            _jobs.Remove(oldJobName);
            _jobs.Add(job.Name, job);
            ApplySettings();
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldJobName, job, null);
        }
        public void DuplicateJobs(string[] jobNames)
        {
            AnalysisJob newAnalysisJob;
            foreach (var name in jobNames)
            {
                newAnalysisJob = _jobs[name].DeepClone();
                newAnalysisJob.Name = NamedClass.GetNameWithoutLastValue(newAnalysisJob.Name);
                newAnalysisJob.Name = _jobs.GetNextNumberedKey(newAnalysisJob.Name);
                AddJob(newAnalysisJob);
            }
        }
        public bool PrepareAndRunJob(string inputFileName, AnalysisJob job, bool onlyCheckModel)
        {
            if (File.Exists(job.Executable))
            {
                if (CheckModelBeforeJobRun() && DeleteFilesBeforeJobRun(inputFileName)) // must be separate due to exception
                {
                    if (onlyCheckModel) _model.StepCollection.SetCheckModel();
                    else _model.StepCollection.SetRunAnalysis();

                    if (_model.Properties.ModelType == ModelType.SlipWearModel)
                    {
                        return RunWearJob(inputFileName, job);
                    }
                    else
                    {
                        return RunJob(inputFileName, job);
                    }
                }
                //
                return false;
            }
            else
            {
                throw new CaeException("The executable file of the analysis does not exists.");
            }
        }
        private bool CheckModelBeforeJobRun()
        {
            // Check for missing section
            string msg;
            int[] unAssignedElementIds = _model.GetSectionAssignments(out _);
            if (unAssignedElementIds.Length != 0)
            {
                string elementSetName = _model.Mesh.ElementSets.GetNextNumberedKey(Globals.MissingSectionName);
                AddElementSetCommand(new FeElementSet(elementSetName, unAssignedElementIds));
                //
                msg = unAssignedElementIds.Length + " finite elements are missing a section assignment. Continue?";
                if (MessageBox.Show(msg, "Warning", MessageBoxButtons.OKCancel,
                                    MessageBoxIcon.Warning) == DialogResult.Cancel) return false;
            }
            // Check for contacts of different type
            if (_model.ContactPairs.Count > 0)
            {
                HashSet<ContactPairMethod> contactPairMethods = new HashSet<ContactPairMethod>();
                foreach (var entry in _model.ContactPairs)
                {
                    if (entry.Value.Active) contactPairMethods.Add(entry.Value.Method);
                }
                if (contactPairMethods.Count > 1)
                {
                    msg = "More than one contact methods is used in the model. Continue?";
                    if (MessageBox.Show(msg, "Warning", MessageBoxButtons.OKCancel,
                                        MessageBoxIcon.Warning) == DialogResult.Cancel) return false;
                }
            }
            // Check for existence of slip wear steps
            int[] slipWearStepIds = _model.StepCollection.GetSlipWearStepIds();
            if (slipWearStepIds.Length > 0 && _model.Properties.ModelType != ModelType.SlipWearModel)
            {
                msg = "Slip wear steps are defined but the model type is not a slip wear model. Continue?";
                if (MessageBox.Show(msg, "Warning", MessageBoxButtons.OKCancel,
                                    MessageBoxIcon.Warning) == DialogResult.Cancel) return false;
            }
            // Check for existence of boundary displacement step
            if (_model.Properties.BdmRemeshing && _model.StepCollection.GetBoundaryDisplacementStep() == null)
            {
                msg = "Mesh smoothing after the slip wear step is turned on but the boundary displacement step " +
                      "is not defined. Continue?";
                if (MessageBox.Show(msg, "Warning", MessageBoxButtons.OKCancel,
                                    MessageBoxIcon.Warning) == DialogResult.Cancel) return false;
            }
            // Check for wear coefficients in a wear analysis
            if (_model.Properties.ModelType == ModelType.SlipWearModel)                
            {
                if (!_model.AreSlipWearCoefficientsDefined(out _))
                {
                    msg = "No slip wear material coefficients are defined. Continue?";
                    if (MessageBox.Show(msg, "Warning", MessageBoxButtons.OKCancel,
                                        MessageBoxIcon.Warning) == DialogResult.Cancel) return false;
                }
                if (!_model.StepCollection.AreContactHistoryOutputsDefined())
                {
                    msg = "Contact history output variables CDIS are not defined for each analysis step. Continue?";
                    if (MessageBox.Show(msg, "Warning", MessageBoxButtons.OKCancel,
                                        MessageBoxIcon.Warning) == DialogResult.Cancel) return false;
                }
            }
            // Check for radiation load without Stefan-Boltzmann and absolute zero constants
            if (_model.StepCollection.IsActiveRadiationLoadDefined())
            {
                msg = "";
                if (!_model.Properties.IsAbsoluteZeroDefined() && !_model.Properties.IsStefanBoltzmannDefined())
                    msg = "A radiation load is used but the absolute zero temperature and the Stefan-Boltzmann constant " +
                           "are not defined. Continue?";
                else if (!_model.Properties.IsAbsoluteZeroDefined())
                    msg = "A radiation load is used but the absolute zero temperature is not defined. Continue?";
                else if (!_model.Properties.IsStefanBoltzmannDefined())
                    msg = "A radiation load is used but the Stefan-Boltzmann constant is not defined. Continue?";
                //
                if (msg.Length > 0)
                {
                    if (MessageBox.Show(msg, "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) ==
                        DialogResult.Cancel) return false;
                }
            }
            return true;
        }
        private bool DeleteFilesBeforeJobRun(string inputFileName)
        {
            // Delete old files
            string directory = Path.GetDirectoryName(inputFileName);
            string inputFileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputFileName);
            string[] files = new string[] { Path.Combine(directory, inputFileNameWithoutExtension + ".inp"),
                                            Path.Combine(directory, inputFileNameWithoutExtension + ".dat"),
                                            Path.Combine(directory, inputFileNameWithoutExtension + ".sta"),
                                            Path.Combine(directory, inputFileNameWithoutExtension + ".cvg"),
                                            Path.Combine(directory, inputFileNameWithoutExtension + ".12d"),
                                            Path.Combine(directory, inputFileNameWithoutExtension + ".cel"), // contact elements
                                            Path.Combine(directory, inputFileNameWithoutExtension +
                                                         "_WarnNodeMissTiedContact.nam"), // missing contact nodes
                                            Path.Combine(directory, "ResultsForLastIterations.frd"),
                                            Path.Combine(directory, inputFileNameWithoutExtension + ".frd")
                                           };
            try
            {
                foreach (var fileName in files) File.Delete(fileName);
                //
                return true;
            }
            catch (Exception ex)
            {
                throw new CaeException(ex.Message);
            }
        }
        private bool RunJob(string inputFileName, AnalysisJob job)
        {
            ExportToCalculix(inputFileName);
            job.JobStatusChanged = JobStatusChanged;
            job.Submit(1, 1);
            //
            return true;
        }
        private bool RunWearJob(string inputFileName, AnalysisJob job)
        {
            // Clear old results
            _wearResults = null;
            //
            job.JobStatusChanged = JobStatusChanged;
            job.PreRun = PreWearRun;
            job.PostRun = PostWearRun;
            job.LastRunCompleted = LastWearRunCompleted;
            //
            int numOfRunSteps = _model.Properties.NumberOfCycles / _model.Properties.CyclesIncrement;
            int numOfRunIncrements = _model.Properties.BdmRemeshing ?  2 : 1;
            //
            job.Submit(numOfRunSteps, numOfRunIncrements);
            //
            return true;
        }
        private void PreWearRun(AnalysisJob job)
        {
            Dictionary<int, double[]> deformations = null;
            if (job.Tag != null) deformations = (Dictionary<int, double[]>)job.Tag;
            //
            DeleteFilesBeforeJobRun(job.InputFileName);
            //
            if (job.CurrentRunIncrement == 1)
            {
                _form.WriteDataToOutput("Starting wear cycle number: " + job.CurrentRunStep * _model.Properties.CyclesIncrement);
                //
                ExportToCalculix(job.InputFileName, deformations);
                //
                //File.Copy(job.InputFileName, Path.Combine(Path.GetDirectoryName(job.InputFileName),
                //    Path.GetFileNameWithoutExtension(job.InputFileName) + "_" + job.CurrentRunStep + ".inp"), true);
            }
            else if (job.CurrentRunIncrement == 2)
            {
                SuppressExplodedView();
                FeModel model = _model.PrepareBdmModel(deformations);
                FileInOut.Output.CalculixFileWriter.Write(job.InputFileName, model, null);
                ResumeExplodedViews(false);
            }
        }
        private void PostWearRun(AnalysisJob job)
        {
            FeResults results;
            Dictionary<int, double[]> deformations;
            //
            if (job.CurrentRunIncrement == 1)
            {
                ReadWearResults(job);
                if (_wearResults != null)
                {
                    deformations = _wearResults.GetGlobalNonZeroVectors(FOFieldNames.WearDepth);
                    job.Tag = deformations;
                }
                else job.Kill("Intermediate wear results do not exist.");
            }
            else if (job.CurrentRunIncrement == 2)
            {
                results = ReadBDMResults(job);
                deformations = results.GetGlobalNonZeroVectors(FOFieldNames.Disp);
                job.Tag = deformations;
            }
        }
        private void LastWearRunCompleted(AnalysisJob job)
        {
            //if (job.JobStatus == JobStatus.OK) DeleteFilesBeforeJobRun(job.InputFileName);
        }
        private void ReadWearResults(AnalysisJob job)
        {
            string resultsFileFrd = Path.Combine(job.WorkDirectory, job.Name + ".frd");
            string resultsFileDat = Path.Combine(job.WorkDirectory, job.Name + ".dat");
            //
            if (File.Exists(resultsFileFrd) && File.Exists(resultsFileDat))
            {
                FeResults results = FrdFileReader.Read(resultsFileFrd);
                //
                if (results == null || results.Mesh == null) job.Kill("Intermediate wear results do not exist.");
                //
                _model.GetMaterialAssignments(out _);
                //
                results.SetHistory(DatFileReader.Read(resultsFileDat));
                //
                int[] slipWearStepIds = _model.StepCollection.GetSlipWearStepIds();
                if (results.ComputeWear(slipWearStepIds, _model.GetNodalSlipWearCoefficients(),
                                        _model.Properties.NumOfSmoothingSteps, null))
                {
                    results.KeepOnlySelectedSlipWearResults(_model.StepCollection.GetStepIdDuration(),
                                                            slipWearStepIds,
                                                            _model.Properties.SlipWearResults);
                    //
                    if (_wearResults == null) _wearResults = results;
                    else _wearResults.AddResults(results);
                }
                else job.Kill("The computation of wear variables failed.");
            }
        }
        private FeResults ReadBDMResults(AnalysisJob job)
        {
            FeResults results = null;
            string resultsFileFrd = Path.Combine(job.WorkDirectory, job.Name + ".frd");
            //
            if (File.Exists(resultsFileFrd))
            {
                //File.Copy(resultsFileFrd,
                //    Path.Combine(Path.GetDirectoryName(resultsFileFrd), Path.GetFileNameWithoutExtension(resultsFileFrd) + "_" +
                //    job.CurrentRunStep + ".frd"), true);
                results = FrdFileReader.Read(resultsFileFrd);
                //
                if (results == null || results.Mesh == null) job.Kill("Intermediate results do not exist.");
            }
            return results;
        }
        private void JobStatusChanged(string jobName, JobStatus jobStatus)
        {
            _form.UpdateAnalysisProgress();
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, jobName, _jobs[jobName], null, true);
        }
        //
        public void KillJob(string jobName)
        {
            _jobs[jobName].Kill(Environment.NewLine + "Kill command sent by user." + Environment.NewLine);
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, jobName, _jobs[jobName], null);
        }
        public void KillAllJobs()
        {
            foreach (var entry in _jobs)
            {
                if (entry.Value.JobStatus == JobStatus.Running) KillJob(entry.Key);
            }
        }
        public void RemoveJobs(string[] jobNames)
        {
            foreach (var name in jobNames)
            {
                _jobs.Remove(name);
                _form.RemoveTreeNode<AnalysisJob>(ViewGeometryModelResults.Model, name, null);
            }
        }

        #endregion #################################################################################################################

        #region Results  ###########################################################################################################
        private void UpdateCurrentFieldData()
        {
            // Check validity of the field data
            _currentFieldData = CurrentResult.GetFieldData(_currentFieldData.Name,
                                                           _currentFieldData.Component,
                                                           _currentFieldData.StepId,
                                                           _currentFieldData.StepIncrementId,
                                                           true);
        }
        public List<Transformation> GetTransformations()
        {
            return _transformations.GetCurrentTransformations();
        }
        public void SetTransformations(List<Transformation> transformations)
        {
            _transformations.SetCurrentTransformations(transformations);
            //
            if (_currentView == ViewGeometryModelResults.Results)
            {
                _form.SetTransformationsStatus(_transformations.AreTransformationsActive());
                DrawResults(false);
            }
        }
        public void RemoveCurrentTransformations(bool update)
        {
            _transformations.RemoveCurrentTransformations();
            //
            if (update && _currentView == ViewGeometryModelResults.Results)
            {
                _form.SetTransformationsStatus(_transformations.AreTransformationsActive());
                DrawResults(false);
            }
        }
        public void SetResults(FeResults results)
        {
            LoadResults(results, false);
            // Check validity
            CheckAndUpdateModelValidity();
            // Get first component of the first field for the last increment in the last step
            if (ResultsInitialized)
                _currentFieldData = _allResults.CurrentResult.GetFirstComponentOfTheFirstFieldAtDefaultIncrement();
        }
        public void RemoveCurrentResult()
        {
            // Edges visibility
            _edgesVisibilities.RemoveCurrentResultEdgesVisibility();
            // Section view
            _sectionViews.RemoveCurrentSectionView();
            // Exploded view
            _explodedViews.RemoveCurrentExplodedView();
            // Annotations
            _annotations.RemoveCurrentResultArrowAnnotations();
            // Transformations
            _transformations.RemoveCurrentTransformations();
            // Results
            _allResults.RemoveCurrentResult();
            //
            _modelChanged = true;
        }

        #endregion #################################################################################################################

        #region Result part menu  ##################################################################################################
        public string[] GetResultPartNames()
        {
            if (_allResults.CurrentResult == null || _allResults.CurrentResult.Mesh == null) return null;
            return _allResults.CurrentResult.Mesh.Parts.Keys.ToArray();
        }
        public BasePart GetResultPart(string partName)
        {
            return _allResults.CurrentResult.Mesh.Parts[partName];
        }
        public BasePart[] GetResultParts(string[] partNames)
        {
            BasePart part;
            BasePart[] parts = new BasePart[partNames.Length];
            for (int i = 0; i < partNames.Length; i++)
            {
                _allResults.CurrentResult.Mesh.Parts.TryGetValue(partNames[i], out part);
                parts[i] = part;
            }
            return parts;
        }
        public BasePart[] GetResultParts()
        {
            if (_allResults.CurrentResult == null || _allResults.CurrentResult.Mesh == null) return null;
            //
            int i = 0;
            BasePart[] parts = new BasePart[_allResults.CurrentResult.Mesh.Parts.Count];
            foreach (var entry in _allResults.CurrentResult.Mesh.Parts) parts[i++] = (BasePart)entry.Value;
            return parts;
        }
        public BasePart[] GetResultParts<T>()
        {
            if (_allResults.CurrentResult == null || _allResults.CurrentResult.Mesh == null) return null;
            //
            List<BasePart> parts = new List<BasePart>();
            foreach (var entry in _allResults.CurrentResult.Mesh.Parts)
            {
                if (entry.Value is ResultPart) parts.Add(entry.Value);
            }
            return parts.ToArray();
        }
        public string[] GetResultPartNames<T>()
        {
            List<string> names = new List<string>();
            foreach (var entry in _allResults.CurrentResult.Mesh.Parts)
            {
                if (entry.Value.Labels.Length > 0 && _allResults.CurrentResult.Mesh.Elements[entry.Value.Labels[0]] is T)
                {
                    names.Add(entry.Key);
                }
            }
            return names.ToArray();
        }
        public void HideResultParts(string[] partNames)
        {
            if (partNames != null)
            {
                foreach (var name in partNames)
                {
                    _allResults.CurrentResult.Mesh.Parts[name].Visible = false;
                    _form.UpdateTreeNode(ViewGeometryModelResults.Results, name,
                                         _allResults.CurrentResult.Mesh.Parts[name], null, false);
                }
                _form.HideActors(partNames, true);
                //
                AnnotateWithColorLegend();
                // Annotations
                _annotations.DrawAnnotations();
            }
        }
        public void ShowResultParts(string[] partNames)
        {
            if (partNames != null)
            {
                foreach (var name in partNames)
                {
                    _allResults.CurrentResult.Mesh.Parts[name].Visible = true;
                    _form.UpdateTreeNode(ViewGeometryModelResults.Results, name, _allResults.CurrentResult.Mesh.Parts[name],
                                         null, false);
                }
                _form.ShowActors(partNames, true);
                //
                AnnotateWithColorLegend();
                // Annotations
                _annotations.DrawAnnotations();
            }
        }
        public void SetTransparencyForResultParts(string[] partNames, byte alpha)
        {
            BasePart part;
            foreach (var name in partNames)
            {
                part = _allResults.CurrentResult.Mesh.Parts[name];
                part.Color = Color.FromArgb(alpha, part.Color);
                _form.UpdateActor(name, name, part.Color);
                _form.UpdateTreeNode(ViewGeometryModelResults.Results, name,
                                     _allResults.CurrentResult.Mesh.Parts[name], null, false);
            }
        }
        public void SetResultPartsColorContoursVisibility(string[] partNames, bool colorContours)
        {
            foreach (var name in partNames)
            {
                if (_allResults.CurrentResult.Mesh.Parts[name] is ResultPart resultPart) resultPart.ColorContours = colorContours;
            }
            _form.UpdateActorColorContoursVisibility(partNames, colorContours);
            UpdateHighlight();
        }
        public void ReplaceResultPartProperties(string oldPartName, PartProperties newPartProperties)
        {
            // Replace result part
            BasePart part = GetResultPart(oldPartName);
            part.SetProperties(newPartProperties);
            _allResults.CurrentResult.Mesh.Parts.Replace(oldPartName, part.Name, part);
            //
            _form.UpdateActor(oldPartName, part.Name, part.Color);
            _form.UpdateTreeNode(ViewGeometryModelResults.Results, oldPartName, part, null);
            //
            AnnotateWithColorLegend();
            //
            FeResultsUpdate(UpdateType.Check);
        }
        public void RemoveResultParts(string[] partNames)
        {
            FeResults result = _allResults.CurrentResult;
            ViewGeometryModelResults view = ViewGeometryModelResults.Results;
            // Remove annotations
            _annotations.RemoveCurrentArrowAnnotationsByParts(partNames, view);
            // Suppress exploded view
            result.Mesh.SuppressExplodedView();
            // Remove
            string[] removedPartNames = result.RemoveParts(partNames);
            // Resume exploded view
            result.Mesh.ResumeExplodedView();  // resume is enough
            //
            foreach (var name in removedPartNames) _form.RemoveTreeNode<BasePart>(view, name, null);
            //
            AnnotateWithColorLegend();
            //
            FeResultsUpdate(UpdateType.Check | UpdateType.DrawResults);
        }
        //
        public bool AreResultPartsMergeable(string[] partNames)
        {
            return _allResults.CurrentResult.Mesh.ArePartsMergeable(partNames);
        }
        public void MergeResultParts(string[] partNames)
        {
            string[] mergedParts;
            ResultPart newResultPart;
            FeResults result = _allResults.CurrentResult;
            ViewGeometryModelResults view = ViewGeometryModelResults.Results;
            // Remove annotations
            _annotations.RemoveCurrentArrowAnnotationsByParts(partNames, view);
            // Remove exploded view
            result.Mesh.RemoveExplodedView();
            // Merge
            result.Mesh.MergeResultParts(partNames, out newResultPart, out mergedParts);
            // Update exploded view
            UpdateCurrentResultExplodedView();
            //
            if (newResultPart != null && mergedParts != null)
            {
                foreach (var partName in mergedParts) _form.RemoveTreeNode<ResultPart>(view, partName, null);
                //
                _form.AddTreeNode(view, newResultPart, null);
                //
                AnnotateWithColorLegend();
                //
                FeResultsUpdate(UpdateType.Check | UpdateType.DrawResults);
            }
        }

        #endregion #################################################################################################################

        #region Result node set  ###################################################################################################
        public string[] GetResultUserNodeSetNames()
        {
            if (_allResults.CurrentResult != null && _allResults.CurrentResult.Mesh != null)
            {
                List<string> userNodeSetNames = new List<string>();
                foreach (var entry in _allResults.CurrentResult.Mesh.NodeSets)
                {
                    if (!entry.Value.Internal) userNodeSetNames.Add(entry.Key);
                }
                return userNodeSetNames.ToArray();
            }
            else return null;
        }

        #endregion #################################################################################################################
        
        #region Result element set  ###################################################################################################
        public string[] GetResultUserElementSetNames()
        {
            if (_allResults.CurrentResult != null && _allResults.CurrentResult.Mesh != null)
            {
                List<string> userElementSetNames = new List<string>();
                foreach (var entry in _allResults.CurrentResult.Mesh.ElementSets)
                {
                    if (!entry.Value.Internal) userElementSetNames.Add(entry.Key);
                }
                return userElementSetNames.ToArray();
            }
            else return null;
        }

        #endregion #################################################################################################################

        #region Result surface  ####################################################################################################
        public string[] GetResultUserSurfaceNames()
        {
            if (_allResults.CurrentResult != null && _allResults.CurrentResult.Mesh != null)
            {
                List<string> userSurfaceNames = new List<string>();
                foreach (var entry in _allResults.CurrentResult.Mesh.Surfaces)
                {
                    if (!entry.Value.Internal) userSurfaceNames.Add(entry.Key);
                }
                return userSurfaceNames.ToArray();
            }
            else return null;
        }

        #endregion #################################################################################################################

        #region Result field output  ###############################################################################################
        public string[] GetResultFieldOutputNames()
        {
            return _allResults.CurrentResult.GetAllFieldNames();
        }
        public string[] GetResultFieldOutputComponents(string fieldOutputName)
        {
            return _allResults.CurrentResult.GetFieldComponentNames(fieldOutputName);
        }
        public int[] GetResultStepIDs()
        {
            if (_allResults.CurrentResult == null || _allResults.CurrentResult.Mesh == null) return new int[0]; // on empty model
            //
            return _allResults.CurrentResult.GetAllStepIds();
        }
        public int[] GetResultStepIncrementIds(int stepId)
        {
            return _allResults.CurrentResult.GetStepIncrementIds(stepId);
        }
        public void AddResultFieldOutput(ResultFieldOutput resultFieldOutput)
        {
            _allResults.CurrentResult.AddResultFieldOutput(resultFieldOutput);
            //
            //_form.RegenerateTree()
            _form.AddTreeNode(ViewGeometryModelResults.Results, resultFieldOutput, null);
            //
            FeResultsUpdate(UpdateType.Check);
        }
        public ResultFieldOutput[] GetResultFieldOutputs()
        {
            return _allResults.CurrentResult.GetResultFieldOutputs();
        }
        public NamedClass[] GetVisibleResultFieldOutputsAsNamedItems()
        {
            return _allResults.CurrentResult.GetVisibleFieldsAsNamedItems();
        }
        public ResultFieldOutput GetResultFieldOutput(string resultFieldOutputName)
        {
            return _allResults.CurrentResult.GetResultFieldOutput(resultFieldOutputName);
        }
        public void ReplaceResultFieldOutput(string oldResultFieldOutputName, ResultFieldOutput resultFieldOutput)
        {
            _allResults.CurrentResult.ReplaceResultFieldOutput(oldResultFieldOutputName, resultFieldOutput);
            //
            _form.UpdateTreeNode(ViewGeometryModelResults.Results, oldResultFieldOutputName, resultFieldOutput, null);
            //
            UpdatePartsScalarFields();
            //
            FeResultsUpdate(UpdateType.Check);
        }
        // Remove
        public void RemoveResultFieldOutputs(string[] fieldOutputNames)
        {
            Dictionary<string, Action<ViewGeometryModelResults, string, string>> nameDeleteAction =
                new Dictionary<string, Action<ViewGeometryModelResults, string, string>>();
            foreach (var name in fieldOutputNames)
            {
                if (_allResults.CurrentResult.ContainsResultFieldOutput(name))
                    nameDeleteAction.Add(name, _form.RemoveTreeNode<ResultFieldOutput>);
                else nameDeleteAction.Add(name, _form.RemoveTreeNode<Field>);
            }
            //
            _allResults.CurrentResult.RemoveResultFieldOutputs(fieldOutputNames);
            _form.ClearActiveTreeSelection();   // prevents errors on _form.RemoveTreeNode
            //
            ViewGeometryModelResults view = ViewGeometryModelResults.Results;
            foreach (var name in fieldOutputNames)
            {
                nameDeleteAction[name](view, name, null);
            }
            //
            if (_allResults.CurrentResult.GetAllComponentNames().Length > 0) _form.SelectFirstComponentOfFirstFieldOutput();
            //
            FeResultsUpdate(UpdateType.Check | UpdateType.DrawResults);
        }
        public void RemoveResultFieldOutputComponents(string fieldOutputName, string[] componentNames)
        {
            _allResults.CurrentResult.RemoveResultFieldOutputComponents(fieldOutputName, componentNames);
            _form.ClearActiveTreeSelection();   // prevents errors on _form.RemoveTreeNode
            //
            ViewGeometryModelResults view = ViewGeometryModelResults.Results;
            foreach (var name in componentNames) _form.RemoveTreeNode<FieldData>(view, name, fieldOutputName);
            //
            if (_allResults.CurrentResult.GetAllComponentNames().Length > 0) _form.SelectFirstComponentOfFirstFieldOutput();
            //
            FeResultsUpdate(UpdateType.Check | UpdateType.DrawResults);
        }
        //
        
        #endregion #################################################################################################################

        #region Result history output  #############################################################################################
        public string[] GetResultHistoryOutputSetNames()
        {
            if (_allResults.CurrentResult != null && _allResults.CurrentResult.GetHistory() != null)
            {
                return _allResults.CurrentResult.GetHistory().Sets.Keys.ToArray();
            }
            else return new string[0];
        }
        public NamedClass[] GetResultHistoryOutputsAsNamedItems()
        {
            return _allResults.CurrentResult.GetHistoryOutputsAsNamedItems();
        }
        public void GetHistoryOutputData(HistoryResultData historyData, out string[] columnNames, out object[][] rowBasedData)
        {
            _allResults.CurrentResult.GetHistoryOutputData(historyData, out columnNames, out rowBasedData);
        }
        //
        public void AddResultHistoryOutput(ResultHistoryOutput resultHistoryOutput)
        {
            HistoryResultSet historyResultSet = _allResults.CurrentResult.AddResultHistoryOutput(resultHistoryOutput);
            //
            _form.AddTreeNode(ViewGeometryModelResults.Results, historyResultSet, null);
        }
        // Remove
        public void RemoveResultHistoryResultSets(string[] historyResultSetNames)
        {
            _allResults.CurrentResult.RemoveResultHistoryResultSets(historyResultSetNames);
            _form.ClearActiveTreeSelection();   // prevents errors on _form.RemoveTreeNode
            //
            ViewGeometryModelResults view = ViewGeometryModelResults.Results;
            foreach (var name in historyResultSetNames) _form.RemoveTreeNode<HistoryResultSet>(view, name, null);
        }
        public void RemoveResultHistoryResultFields(string historyResultSetName, string[] historyResultFieldNames)
        {
            _allResults.CurrentResult.RemoveResultHistoryResultFields(historyResultSetName, historyResultFieldNames);
            _form.ClearActiveTreeSelection();   // prevents errors on _form.RemoveTreeNode
            //
            ViewGeometryModelResults view = ViewGeometryModelResults.Results;
            foreach (var name in historyResultFieldNames)
                _form.RemoveTreeNode<HistoryResultField>(view, name, historyResultSetName);
        }
        public void RemoveResultHistoryResultComponents(string historyResultSetName,
                                                        string historyResultFieldName,
                                                        string[] historyResultComponentNames)
        {
            _allResults.CurrentResult.RemoveResultHistoryResultComponents(historyResultSetName, historyResultFieldName,
                                                         historyResultComponentNames);
            _form.ClearActiveTreeSelection();   // prevents errors on _form.RemoveTreeNode
            //
            ViewGeometryModelResults view = ViewGeometryModelResults.Results;
            foreach (var name in historyResultComponentNames)
                _form.RemoveTreeNode<HistoryResultData>(view, name, historyResultSetName + "@@@" + historyResultFieldName);
        }

        #endregion #################################################################################################################

        #region Activate Deactivate  ###############################################################################################
        // COMMANDS ********************************************************************************
        public void ActivateDeactivateCommand(NamedClass item, bool activate, string stepName)
        {
            Commands.CActivateDeactivate comm = new Commands.CActivateDeactivate(item, activate, stepName);
            _commands.AddAndExecute(comm);
        }
        public void ActivateDeactivateMultipleCommand(NamedClass[] items, bool activate, string[] stepNames)
        {
            Commands.CActivateDeactivateMultilpe comm = new Commands.CActivateDeactivateMultilpe(items, activate, stepNames);
            _commands.AddAndExecute(comm);
        }
        //******************************************************************************************
        public void ActivateDeactivate(NamedClass item, bool activate, string stepName)
        {
            // Do not call the replace command here
            item.Active = activate;
            if (item is MeshSetupItem msi) ActivateDeactivateMeshSetupItem(msi.Name, activate);
            else if (item is Constraint co) ActivateDeactivateConstraint(co.Name, activate);
            else if (item is ContactPair cp) ActivateDeactivateContactPair(cp.Name, activate);
            else if (item is InitialCondition ic) ActivateDeactivateInitialCondition(ic.Name, activate);
            else if (item is Step st) ActivateDeactivateStep(st.Name, activate);
            else if (item is HistoryOutput ho) ActivateDeactivateHistoryOutput(stepName, ho.Name, activate);
            else if (item is FieldOutput fo) ActivateDeactivateFieldOutput(stepName, fo.Name, activate);
            else if (item is BoundaryCondition bc) ActivateDeactivateBoundaryCondition(stepName, bc.Name, activate);
            else if (item is Load lo) ActivateDeactivateLoad(stepName, lo.Name, activate);
            else if (item is DefinedField df) ActivateDeactivateDefinedField(stepName, df.Name, activate);
            else throw new NotImplementedException();
        }
        public void ActivateDeactivateMultiple(NamedClass[] items, bool activate, string[] stepNames)
        {
            int count = 0;
            foreach (var item in items)
            {
                ActivateDeactivate(item, activate, stepNames[count]);
                count++;
            }
        }

        #endregion #################################################################################################################
        
        #region Selection  #########################################################################################################
        public void SetSelectionView(ViewGeometryModelResults selectionView)
        {
            _selection.CurrentView = (int)selectionView;
        }
        public void SetSelectionView(int selectionView)
        {
            _selection.CurrentView = selectionView;
        }
        public static ViewGeometryModelResults GetSelectionView(Selection selection)
        {
            return (ViewGeometryModelResults)selection.CurrentView;
        }
        public void CreateNewSelection(int selectionView, vtkSelectItem selectItem, SelectionNode selectionNode, bool highlight)
        {
            ClearSelectionHistoryAndCallSelectionChanged();
            SetSelectionView(selectionView);
            _selection.SelectItem = selectItem;
            AddSelectionNode(selectionNode, highlight, false);
        }
        // The function called from vtk_control
        public void SelectPointOrArea(double[] pickedPoint, double[] selectionDirection,
                                      double[][] planeParameters, bool completelyInside,
                                      vtkSelectOperation selectOperation, string[] pickedPartNames)
        {
            try
            {
                // Activate user pick
                _form.ActivateUserPick();
                //
                if (_selectBy == vtkSelectBy.Id) return;
                // Empty pick - Clear if no operation is used
                if (pickedPoint == null && planeParameters == null)
                {
                    if (selectOperation == vtkSelectOperation.None) // must be here
                        ClearSelectionHistoryAndCallSelectionChanged();
                }
                else
                {
                    vtkSelectBy selectBy = _selectBy;
                    // Query nodes - more than one selection node is needed - change None to Add
                    if (_selectBy == vtkSelectBy.QueryNode)
                    {
                        selectOperation = vtkSelectOperation.Add;
                    }
                    else
                    {
                        // New pick - Clear history
                        if (selectOperation == vtkSelectOperation.None) ClearSelectionHistoryAndCallSelectionChanged();
                    }
                    // Part ids
                    int[] pickedPartIds = null;
                    double[][] pickedPartOffsets = null;
                    if (pickedPartNames != null && pickedPartNames.Length > 0)
                    {
                        FeMesh mesh = DisplayedMesh;
                        pickedPartIds = mesh.GetPartIdsFomPartNames(pickedPartNames);
                        // Exploded view - save offsets
                        if (IsExplodedViewActive()) pickedPartOffsets = mesh.GetPartOffsetsFromPartNames(pickedPartNames);
                    }
                    //
                    SelectionNode selectionNode = new SelectionNodeMouse(pickedPoint, selectionDirection,
                                                                         planeParameters, completelyInside,
                                                                         selectOperation, pickedPartIds, pickedPartOffsets,
                                                                         _selectBy, _selectAngle);
                    AddSelectionNode(selectionNode, true, false);
                }
            }
            catch { }
            finally
            {
                // Deactivate user pick
                _form.DeactivateUserPick();
            }
        }
        public void AddSelectionNode(SelectionNode node, bool highlight, bool callSelectionChanged)
        {
            // Set the current view for the selection;
            if (_selection.Nodes.Count == 0) SetSelectionView(_currentView);
            // Get selected ids
            int[] ids = GetIdsFromSelectionNode(node, new HashSet<int>());
            int[] afterIds = null;
            FeMesh mesh = DisplayedMesh;
            FeElement element;
            // Check for errors    
            if (node is SelectionNodeIds)
            {
                SelectionNodeIds selectionNodeIds = node as SelectionNodeIds;
                if (!selectionNodeIds.SelectAll)
                {
                    if (_selection.SelectItem == vtkSelectItem.Node)
                    {
                        for (int i = 0; i < ids.Length; i++)
                        {
                            if (!mesh.Nodes.ContainsKey(ids[i]))
                                throw new CaeException("The selected node id does not exist.");
                        }
                    }
                    else if (_selection.SelectItem == vtkSelectItem.Element)
                    {
                        for (int i = 0; i < ids.Length; i++)
                        {
                            if (!mesh.Elements.ContainsKey(ids[i]))
                                throw new CaeException("The selected element id does not exist.");
                        }
                    }
                    else if (_selection.SelectItem == vtkSelectItem.GeometryEdge || _selectBy == vtkSelectBy.QuerySurface)
                    {
                        // Query edge
                        // Query surface 
                        // Both return geometry ids
                    }
                    else if (_selection.SelectItem == vtkSelectItem.Surface)
                    {
                        for (int i = 0; i < ids.Length; i++)
                        {
                            // Check: The selected face id does not exist."
                            mesh.GetCellFromFaceId(ids[i], out _, out _);
                        }
                    }
                    else if (_selection.SelectItem == vtkSelectItem.Geometry ||
                             _selection.SelectItem == vtkSelectItem.GeometrySurface)
                    {
                        // Return geometry ids
                    }
                    else throw new NotSupportedException();
                }
                else ClearSelectionHistoryAndCallSelectionChanged();   // Before adding all clear selection
            }
            // Limit selection to shell edges, parts, geometry type
            int elementId;
            int vtkCellId;
            bool add = true;
            //
            if (_selection.SelectItem == vtkSelectItem.Surface)
            {
                // Limit surface selection to first shell face type
                if (afterIds == null) { _selection.Add(node, ids); afterIds = GetSelectionIds(); _selection.RemoveLast(); }
                HashSet<FeSurfaceFaceTypes> surfaceFaceTypes = mesh.GetSurfaceFaceTypesFromFaceIds(afterIds);
                if (surfaceFaceTypes.Count() > 1) add = false;      // 0 : when subtracting the last item
                // Limit surface selection to shell edge surfaces
                else if (_selection.LimitSelectionToShellEdges)
                {
                    for (int i = 0; i < ids.Length; i++)
                    {
                        mesh.GetElementIdVtkCellIdFromFaceId(ids[i], out elementId, out vtkCellId);
                        element = mesh.Elements[elementId];
                        if (!(element is FeElement2D && vtkCellId >= 2)) { add = false; break; }
                    }
                }
                // Enable selection of shell edge surfaces
                else if (!_selection.EnableShellEdgeFaceSelection)
                {
                    for (int i = 0; i < ids.Length; i++)
                    {
                        mesh.GetElementIdVtkCellIdFromFaceId(ids[i], out elementId, out vtkCellId);
                        element = mesh.Elements[elementId];
                        if (element is FeElement2D && vtkCellId >= 2) { add = false; break; }
                    }
                }
            }
            //
            if (add)
            {
                add = false;
                // Limit selection to first part
                if (_selection.LimitSelectionToFirstPart)
                {
                    if (afterIds == null) { _selection.Add(node, ids); afterIds = GetSelectionIds(); _selection.RemoveLast(); }
                    HashSet<BasePart> parts = mesh.GetPartsFromSelectionIds(afterIds, _selection.SelectItem);
                    if (parts.Count == 1) add = true;
                }
                // Limit selection to first geometry type
                else if (_selection.LimitSelectionToFirstGeometryType)
                {
                    if (afterIds == null) { _selection.Add(node, ids); afterIds = GetSelectionIds(); _selection.RemoveLast(); }
                    HashSet<BasePart> parts = mesh.GetPartsFromSelectionIds(afterIds, _selection.SelectItem);
                    HashSet<PartType> partTypes = new HashSet<PartType>();
                    foreach (var part in parts) partTypes.Add(part.PartType);
                    if (partTypes.Count <= 1) add = true;   // 0 : when subtracting the last item
                }
                else if (_selection.LimitSelectionToFirstMesherType)
                {
                    if (afterIds == null) { _selection.Add(node, ids); afterIds = GetSelectionIds(); _selection.RemoveLast(); }
                    HashSet<BasePart> parts = mesh.GetPartsFromSelectionIds(afterIds, _selection.SelectItem);
                    bool mmg;
                    HashSet<bool> mmgHash = new HashSet<bool>();
                    foreach (var part in parts)
                    {
                        mmg = false;
                        if (part.PartType == PartType.Shell && part is GeometryPart gp && !gp.IsCADPart) mmg = true;
                        else if (part.PartType == PartType.Shell && part is MeshPart) mmg = true; // for remeshing
                        mmgHash.Add(mmg);
                        if (mmgHash.Count > 1) break;
                    }
                    if (mmgHash.Count <= 1) add = true;   // 0 : when subtracting the last item
                }
                else add = true;
            }
            //
            if (add)
            {
                if (_selection.MaxNumberOfIds == 1) _selection.Clear();
                // Add
                _selection.Add(node, ids);
                // Remove the node if the maximum number of items is exceeded
                if (_selection.MaxNumberOfIds > 1)
                {
                    ids = GetSelectionIds();
                    if (ids.Length > _selection.MaxNumberOfIds) _selection.RemoveLast();
                }
            }
            //
            if (callSelectionChanged) _form.SelectionChanged();
            //
            if (highlight) HighlightSelection();
        }
        public void RemoveLastSelectionNode()
        {
            _selection.RemoveLast();
            HighlightSelection();       // one color selection
            //
            _form.SelectionChanged();   // if two color selection is needed it is done from the form 
        }
        //
        public int[] GetSelectionIds()
        {
            // If no nodes are added - return empty
            if (_selection.Nodes.Count == 0) return new int[0];
            // ids for:
            // nodes: global node ids
            // elements: global element ids
            // faces: 10 * global element ids + vtk face ids;   search: (% 10)
            // geometry: itemId * 100000 + typeId * 10000 + partId;
            HashSet<int> selectedIds = new HashSet<int>();
            // Compatibility for version v0.5.2
            if (_selection.CurrentView == -1) SetSelectionView(ViewGeometryModelResults.Model);
            // Copy selection - change of the current view clears the selection history
            Selection selectionCopy = _selection.DeepClone();
            // Set the selection view
            CurrentView = GetSelectionView(selectionCopy);
            // Execute selection
            foreach (SelectionNode node in selectionCopy.Nodes)
                GetIdsFromSelectionNode(node, selectedIds);
            // Return
            int[] sorted = selectedIds.ToArray();
            if (_selectBy != vtkSelectBy.QueryNode) Array.Sort(sorted);   // sorting of the ids breaks the angle query !!!
            return sorted;
        }
        private int[] GetIdsFromSelectionNode(SelectionNode selectionNode, HashSet<int> selectedIds)
        {
            int[] ids;
            //
            if (selectionNode is SelectionNodeInvert selectionNodeInvert)
            {
                ids = GetIdsFromSelectionNodeInvert(selectionNodeInvert, selectedIds);
            }
            else if (selectionNode is SelectionNodeIds selectionNodeIds)
            {
                ids = GetIdsFromSelectionNodeIds(selectionNodeIds);
            }
            else if (selectionNode is SelectionNodeMouse selectionNodeMouse)
            {
                ids = GetIdsFromSelectionNodeMouse(selectionNodeMouse, false);
            }
            else throw new NotSupportedException();
            //
            // Append the new selection ids to the already selected ids
            if (ids != null)
            {
                if (selectionNode.SelectOperation == vtkSelectOperation.None ||
                    selectionNode.SelectOperation == vtkSelectOperation.Invert)
                {
                    selectedIds.Clear();
                    selectedIds.UnionWith(ids);
                }
                else if (selectionNode.SelectOperation == vtkSelectOperation.Add)
                {
                    selectedIds.UnionWith(ids);
                }
                else if (selectionNode.SelectOperation == vtkSelectOperation.Subtract)
                {
                    selectedIds.ExceptWith(ids);
                }
                else if (selectionNode.SelectOperation == vtkSelectOperation.Intersect)
                {
                    selectedIds.IntersectWith(ids);
                }
            }
            //
            return ids;
        }
        private int[] GetIdsFromSelectionNodeInvert(SelectionNodeInvert selectionNodeInvert, HashSet<int> selectedIds)
        {
            HashSet<int> allIds;
            //
            if (_selection.SelectItem == vtkSelectItem.Node)
            {
                allIds = new HashSet<int>(GetVisibleNodeIds());
            }
            else if (_selection.SelectItem == vtkSelectItem.Element)
            {
                allIds = new HashSet<int>(GetVisibleElementIds());
            }
            else if (_selection.SelectItem == vtkSelectItem.Surface)
            {
                allIds = new HashSet<int>(GetVisibleFaceIds());
            }
            else throw new NotSupportedException();
            //
            allIds.ExceptWith(selectedIds);
            //
            return allIds.ToArray();
        }
        private int[] GetIdsFromSelectionNodeIds(SelectionNodeIds selectionNodeIds)
        {
            int[] ids;
            //
            if (selectionNodeIds.SelectAll)
            {
                if (_selection.SelectItem == vtkSelectItem.Node) ids = GetVisibleNodeIds();
                else if (_selection.SelectItem == vtkSelectItem.Element) ids = GetVisibleElementIds();
                else if (_selection.SelectItem == vtkSelectItem.Surface) ids = GetVisibleFaceIds();
                else throw new NotSupportedException();
            }
            else
            {
                if (_selection.SelectItem == vtkSelectItem.Node || _selection.SelectItem == vtkSelectItem.Element ||
                    _selection.SelectItem == vtkSelectItem.GeometryEdge || _selection.SelectItem == vtkSelectItem.Surface ||
                    _selection.SelectItem == vtkSelectItem.Part)
                {
                    if (selectionNodeIds.GeometryIds)
                    {
                        // Change geometry ids to node, cell ids
                        ids = DisplayedMesh.GetIdsFromGeometryIds(selectionNodeIds.ItemIds, _selection.SelectItem);
                    }
                    else
                    {
                        ids = selectionNodeIds.ItemIds.ToArray();
                    }
                }
                else if (_selection.SelectItem == vtkSelectItem.Geometry ||
                         _selection.SelectItem == vtkSelectItem.GeometrySurface)
                {
                    if (selectionNodeIds.GeometryIds)
                    {
                        // Change geometry ids to node, cell ids
                        ids = DisplayedMesh.GetIdsFromGeometryIds(selectionNodeIds.ItemIds, _selection.SelectItem);
                    }
                    else throw new NotSupportedException();
                }
                else throw new NotSupportedException();
            }
            //
            return ids;
        }
        public int[] GetIdsFromSelectionNodeMouse(SelectionNodeMouse selectionNodeMouse, bool keepGeometryIds)
        {
            int[] ids;
            // Get offset
            bool allZero;
            double[][] offsets = GetRelativePartOffsets(selectionNodeMouse, out allZero);
            // Pick a point
            if (selectionNodeMouse.PickedPoint != null)
            {
                // Apply offset
                if (offsets != null && offsets.Length == 1 && !allZero) selectionNodeMouse.AddOffset(offsets[0]);
                // Are node ids already recorded in this session - speed optimization
                if (_selection.TryGetNodeIds(selectionNodeMouse, out ids))
                { }
                else if (selectionNodeMouse.IsGeometryBased)
                {
                    ids = GetIdsAtPointFromGeometrySelection(selectionNodeMouse, keepGeometryIds);
                }
                else if (_selection.SelectItem == vtkSelectItem.None)
                { }
                else if (_selection.SelectItem == vtkSelectItem.Node)
                {
                    ids = GetNodeIdsAtPoint(selectionNodeMouse, out _);
                }
                else if (_selection.SelectItem == vtkSelectItem.Element)
                {
                    ids = GetElementIdsAtPoint(selectionNodeMouse);
                }
                else if (_selection.SelectItem == vtkSelectItem.Surface)
                {
                    ids = GetVisualizationFaceIdsAtPoint(selectionNodeMouse);
                }                
                else if (_selection.SelectItem == vtkSelectItem.Part)
                {
                    ids = GetPartIdAtPoint(selectionNodeMouse);
                }
                else throw new NotSupportedException();
                // Remove offset
                if (offsets != null && offsets.Length == 1 && !allZero) selectionNodeMouse.RemoveOffset(offsets[0]);
            }
            // Pick an area
            else
            {
                string[] partNames = DisplayedMesh.GetPartNamesFromPartIds(selectionNodeMouse.PartIds);
                if (offsets == null || allZero)
                {
                    ids = GetIdsFromSelectionInArea(selectionNodeMouse, partNames, keepGeometryIds);
                }
                else
                {
                    HashSet<int> idsHash = new HashSet<int>();
                    for (int i = 0; i < partNames.Length; i++)
                    {
                        selectionNodeMouse.AddOffset(offsets[i]);
                        ids = GetIdsFromSelectionInArea(selectionNodeMouse, new string[] { partNames[i] }, keepGeometryIds);
                        selectionNodeMouse.RemoveOffset(offsets[i]);
                        //
                        idsHash.UnionWith(ids);
                    }
                    ids = idsHash.ToArray();
                }
            }            
            return ids;
        }
        private int[] GetIdsFromSelectionInArea(SelectionNodeMouse selectionNodeMouse, string[] partNames, bool keepGeometryIds)
        {
            int[] ids;
            //
            // Are node ids already recorded in this session - speed optimization
            if (_selection.TryGetNodeIds(selectionNodeMouse, out ids))
            { }
            else if (selectionNodeMouse.IsGeometryBased) // vtkSelectBy = Geometry, ...
            {
                ids = GetIdsFromFrustumFromGeometrySelection(selectionNodeMouse.PlaneParameters,
                                                             selectionNodeMouse.CompletelyInside,
                                                             partNames, selectionNodeMouse.SelectBy, keepGeometryIds);
            }
            else if (_selection.SelectItem == vtkSelectItem.Node)
            {
                ids = GetNodeIdsFromFrustum(selectionNodeMouse.PlaneParameters, partNames,
                                            selectionNodeMouse.SelectBy);
            }
            else if (_selection.SelectItem == vtkSelectItem.Element)
            {
                ids = GetElementIdsFromFrustum(selectionNodeMouse.PlaneParameters, partNames,
                                               selectionNodeMouse.SelectBy);
            }
            else if (_selection.SelectItem == vtkSelectItem.Surface)
            {
                ids = GetVisualizationFaceIdsFromFrustum(selectionNodeMouse.PlaneParameters, partNames,
                                                         selectionNodeMouse.SelectBy);
            }
            else if (_selection.SelectItem == vtkSelectItem.Part)
            {
                ids = GetPartIdsFromFrustum(selectionNodeMouse.PlaneParameters, partNames,
                                            selectionNodeMouse.SelectBy);
            }
            else throw new NotSupportedException();
            //
            return ids;
        }
        // At point

        private int[] GetIdsAtPointFromGeometrySelection(SelectionNodeMouse selectionNodeMouse, bool keepGeometryIds)
        {
            // Geometry selection - get geometry Ids
            // The first time the selectionNodeMouse.Precision equals -1; if so set the Precision for all future queries
            double precision = _form.GetSelectionPrecision();
            if (selectionNodeMouse.Precision == -1) selectionNodeMouse.Precision = precision;
            //
            double[] pickedPoint = selectionNodeMouse.PickedPoint;
            double[] selectionDirection = selectionNodeMouse.SelectionDirection;
            vtkSelectBy selectBy = selectionNodeMouse.SelectBy;
            double angle = selectionNodeMouse.Angle;
            int selectionOnPartId = selectionNodeMouse.PartIds[0];
            precision = selectionNodeMouse.Precision;
            //
            int[] ids;
            if (selectBy == vtkSelectBy.Geometry)
            {
                ids = new int[] { GetGeometryId(pickedPoint, selectionDirection, selectionOnPartId, precision) };
            }
            else if (selectBy == vtkSelectBy.GeometryVertex)
            {
                int id = GetGeometryVertexId(pickedPoint, selectionOnPartId, precision);
                if (id > 0) ids = new int[] { id };
                else ids = new int[0];
            }
            else if (selectBy == vtkSelectBy.GeometryEdge || selectBy == vtkSelectBy.QueryEdge)
            {
                // GeometryEdge - from form FrmSelectGeometry
                bool shellEdgeFace = false;
                if (DisplayedMesh.GetPartFromId(selectionOnPartId).PartType == PartType.Shell &&
                    _selection.SelectItem == vtkSelectItem.Surface) shellEdgeFace = true;
                //
                ids = GetGeometryEdgeIdsByAngle(pickedPoint, -1, selectionOnPartId, shellEdgeFace);
            }
            else if (selectBy == vtkSelectBy.GeometrySurface || selectBy == vtkSelectBy.QuerySurface)
            {
                // GeometrySurface - from form FrmSelectGeometry
                ids = GetGeometrySurfaceIdsByAngle(pickedPoint, selectionDirection, -1, selectionOnPartId);
            }
            else if (selectBy == vtkSelectBy.GeometryEdgeAngle)
            {
                bool shellEdgeFace = false;
                if (DisplayedMesh.GetPartFromId(selectionOnPartId).PartType == PartType.Shell &&
                    _selection.SelectItem == vtkSelectItem.Surface) shellEdgeFace = true;
                //
                ids = GetGeometryEdgeIdsByAngle(pickedPoint, angle, selectionOnPartId, shellEdgeFace);
            }
            else if (selectBy == vtkSelectBy.GeometrySurfaceAngle)
            {
                ids = GetGeometrySurfaceIdsByAngle(pickedPoint, selectionDirection, angle, selectionOnPartId);
            }
            else if (selectBy == vtkSelectBy.GeometryPart)
            {
                int selectedGeomId = GetGeometryId(pickedPoint, selectionDirection, selectionOnPartId, precision);
                // Convert any geometry id to geometry part id
                ids = new int[] { FeMesh.GetGeometryPartIdFromGeometryId(selectedGeomId) };
            }
            else throw new NotSupportedException();
            // Select all other subparts of a selected subpart
            if (_selection.CurrentView == (int)ViewGeometryModelResults.Geometry && _selection.SelectItem == vtkSelectItem.Part)
            {
                ids = DisplayedMesh.GetGeometryPartIdsForSubPartsFromGeometryIds(ids);
            }
            // Change geometry ids to node, element or cell ids if necessary
            if (!keepGeometryIds) ids = DisplayedMesh.GetIdsFromGeometryIds(ids, _selection.SelectItem);
            return ids;
        }
        private int[] GetNodeIdsAtPoint(SelectionNodeMouse selectionNodeMouse, out int elementId)
        {
            int[] edgeNodeIds;
            int[] cellFaceNodeIds;
            double[] pickedPoint = selectionNodeMouse.PickedPoint;
            vtkSelectBy selectBy = selectionNodeMouse.SelectBy;
            //
            _form.GetGeometryPickProperties(pickedPoint, out elementId, out edgeNodeIds, out cellFaceNodeIds);
            //
            if (selectBy == vtkSelectBy.Node || selectBy == vtkSelectBy.QueryNode)
            {
                int nodeId = DisplayedMesh.GetCellFaceNodeIdClosestToPoint(pickedPoint, cellFaceNodeIds);
                return new int[] { nodeId };
            }
            else if (selectBy == vtkSelectBy.Element || selectBy == vtkSelectBy.QueryElement)
            {
                vtkMaxActorData data = GetCellActorData(new int[] { elementId }, null);
                return data.Geometry.Nodes.Ids;
            }
            else if (selectBy == vtkSelectBy.Edge)
            {
                return DisplayedMesh.GetEdgeNodeIds(elementId, edgeNodeIds);
            }
            else if (selectBy == vtkSelectBy.Surface)
            {
                return DisplayedMesh.GetSurfaceNodeIds(elementId, cellFaceNodeIds);
            }
            else if (selectBy == vtkSelectBy.EdgeAngle)
            {
                return DisplayedMesh.GetEdgeByAngleNodeIds(elementId, edgeNodeIds, selectionNodeMouse.Angle);
            }
            else if (selectBy == vtkSelectBy.SurfaceAngle)
            {
                return DisplayedMesh.GetSurfaceByAngleNodeIds(elementId, cellFaceNodeIds, selectionNodeMouse.Angle);
            }
            else if (selectBy == vtkSelectBy.Part)
            {
                return DisplayedMesh.GetPartNodeIds(elementId);
            }
            else throw new NotSupportedException();
        }
        private int[] GetElementIdsAtPoint(SelectionNodeMouse selectionNodeMouse)
        {
            int elementId;
            int[] edgeNodeIds;
            int[] cellFaceNodeIds;
            double[] pickedPoint = selectionNodeMouse.PickedPoint;
            vtkSelectBy selectBy = selectionNodeMouse.SelectBy;

            _form.GetGeometryPickProperties(pickedPoint, out elementId, out edgeNodeIds, out cellFaceNodeIds);

            if (selectBy == vtkSelectBy.Node)
            {
                int nodeId = DisplayedMesh.GetCellFaceNodeIdClosestToPoint(pickedPoint, cellFaceNodeIds);
                return DisplayedMesh.GetElementIdsFromNodeIds(new int[] { nodeId }, false, false, false);
            }
            else if (selectBy == vtkSelectBy.Element || selectBy == vtkSelectBy.QueryElement)
            {
                return new int[] { elementId };
            }
            else if (selectBy == vtkSelectBy.Edge)
            {
                int[] nodeIds = DisplayedMesh.GetEdgeNodeIds(elementId, edgeNodeIds);
                return DisplayedMesh.GetElementIdsFromNodeIds(nodeIds, true, false, false);
            }
            else if (selectBy == vtkSelectBy.Surface)
            {
                int[] nodeIds = DisplayedMesh.GetSurfaceNodeIds(elementId, cellFaceNodeIds);
                return DisplayedMesh.GetElementIdsFromNodeIds(nodeIds, false, true, false);
            }
            else if (selectBy == vtkSelectBy.EdgeAngle)
            {
                int[] nodeIds = DisplayedMesh.GetEdgeByAngleNodeIds(elementId, edgeNodeIds, selectionNodeMouse.Angle);
                return DisplayedMesh.GetElementIdsFromNodeIds(nodeIds, true, false, false);
            }
            else if (selectBy == vtkSelectBy.SurfaceAngle)
            {
                int[] nodeIds = DisplayedMesh.GetSurfaceByAngleNodeIds(elementId, cellFaceNodeIds, selectionNodeMouse.Angle);
                return DisplayedMesh.GetElementIdsFromNodeIds(nodeIds, false, true, false);
            }
            else if (selectBy == vtkSelectBy.Part)
            {
                return DisplayedMesh.GetPartElementIds(elementId);
            }
            else throw new NotSupportedException();
        }
        private int[] GetVisualizationFaceIdsAtPoint(SelectionNodeMouse selectionNodeMouse)
        {
            // Surface is based on node selection which is converted to face ids
            int elementId;
            int[] elementIds;            
            int[] ids = GetNodeIdsAtPoint(selectionNodeMouse, out elementId);
            bool shellFrontFace = DisplayedMesh.IsShellElementFrontFaceSelected(elementId, selectionNodeMouse.SelectionDirection);
            //
            if (selectionNodeMouse.SelectBy == vtkSelectBy.Node)
            {
                elementIds = DisplayedMesh.GetElementIdsFromNodeIds(ids, false, false, false);
                ids = DisplayedMesh.GetVisualizationFaceIds(ids, elementIds, false, false, shellFrontFace);
            }
            else if (selectionNodeMouse.SelectBy == vtkSelectBy.Element)
            {
                elementIds = DisplayedMesh.GetElementIdsFromNodeIds(ids, false, false, true);
                ids = DisplayedMesh.GetVisualizationFaceIds(ids, elementIds, false, true, shellFrontFace);
            }
            else if (selectionNodeMouse.SelectBy == vtkSelectBy.Part)
            {
                elementIds = DisplayedMesh.GetElementIdsFromNodeIds(ids, false, true, false);
                ids = DisplayedMesh.GetVisualizationFaceIds(ids, elementIds, false, true, shellFrontFace);
            }
            else if (selectionNodeMouse.SelectBy == vtkSelectBy.Edge ||
                     selectionNodeMouse.SelectBy == vtkSelectBy.EdgeAngle)
            {
                elementIds = DisplayedMesh.GetElementIdsFromNodeIds(ids, true, false, false);
                ids = DisplayedMesh.GetVisualizationFaceIds(ids, elementIds, true, false, shellFrontFace);
            }
            else if (selectionNodeMouse.SelectBy == vtkSelectBy.Surface ||
                     selectionNodeMouse.SelectBy == vtkSelectBy.SurfaceAngle)
            {
                elementIds = DisplayedMesh.GetElementIdsFromNodeIds(ids, false, true, false);
                ids = DisplayedMesh.GetVisualizationFaceIds(ids, elementIds, false, true, shellFrontFace);
            }
            return ids;
        }
        private int[] GetPartIdAtPoint(SelectionNodeMouse selectionNodeMouse)
        {
            int elementId;
            double[] pickedPoint = selectionNodeMouse.PickedPoint;
            //
            _form.GetGeometryPickProperties(pickedPoint, out elementId, out _, out _);
            //
            FeElement element;
            if (DisplayedMesh.Elements.TryGetValue(elementId, out element)) return new int[] { element.PartId };
            else return null;
        }
        
        
        // Inside frustum
        private int[] GetIdsFromFrustumFromGeometrySelection(double[][] planeParameters, bool completelyInside,
                                                             string[] selectionPartNames, vtkSelectBy selectBy,
                                                             bool keepGeometryIds)
        {
            int[] nodeIds;
            int[] elementIds;
            FeMesh mesh = DisplayedMesh;
            _form.GetPointAndCellIdsInsideFrustum(planeParameters, selectionPartNames, out nodeIds, out elementIds);
            //
            int[] ids = null;
            if ((elementIds == null || elementIds.Length == 0) && (nodeIds == null || nodeIds.Length == 0)) return ids;
            // Get geometry ids
            ids = mesh.GetGeometryIds(nodeIds, elementIds, completelyInside);
            // Select all other subparts of a selected subpart
            if (_selection.CurrentView == (int)ViewGeometryModelResults.Geometry && _selection.SelectItem == vtkSelectItem.Part)
            {
                ids = DisplayedMesh.GetGeometryPartIdsForSubPartsFromGeometryIds(ids);
            }
            if (keepGeometryIds) return ids;
            // Change geometry ids to node, element, ... ids
            if (selectBy == vtkSelectBy.Geometry)
            {
                if (_selection.SelectItem == vtkSelectItem.Node || _selection.SelectItem == vtkSelectItem.Element ||
                    _selection.SelectItem == vtkSelectItem.Surface || _selection.SelectItem == vtkSelectItem.Part ||
                    _selection.SelectItem == vtkSelectItem.Geometry || _selection.SelectItem == vtkSelectItem.GeometrySurface)
                {
                    ids = mesh.GetIdsFromGeometryIds(ids, _selection.SelectItem);
                }
                else throw new NotSupportedException();
            }
            else if (selectBy == vtkSelectBy.GeometryVertex)
            {
                throw new NotSupportedException();
            }
            else if (selectBy == vtkSelectBy.GeometryEdge || selectBy == vtkSelectBy.QueryEdge)
            {
                throw new NotSupportedException();
            }
            else if (selectBy == vtkSelectBy.GeometrySurface || selectBy == vtkSelectBy.QuerySurface)
            {
                throw new NotSupportedException();
            }
            else if (selectBy == vtkSelectBy.GeometryEdgeAngle)
            {
                throw new NotSupportedException();
            }
            else if (selectBy == vtkSelectBy.GeometrySurfaceAngle)
            {
                throw new NotSupportedException();
            }
            else if (selectBy == vtkSelectBy.GeometryPart)
            {
                if (_selection.SelectItem == vtkSelectItem.Node)
                {
                    string[] partNamesByElementId = mesh.GetPartNamesFromElementIds(elementIds);
                    HashSet<int> partNodeIds = new HashSet<int>();
                    foreach (var partName in partNamesByElementId) partNodeIds.UnionWith(mesh.Parts[partName].NodeLabels);
                    ids = partNodeIds.ToArray();
                }
                else if (_selection.SelectItem == vtkSelectItem.Element)
                {
                    string[] partNamesByElementId = mesh.GetPartNamesFromElementIds(elementIds);
                    HashSet<int> partElementIds = new HashSet<int>();
                    foreach (var partName in partNamesByElementId) partElementIds.UnionWith(mesh.Parts[partName].Labels);
                    return partElementIds.ToArray();
                }
                else if (_selection.SelectItem == vtkSelectItem.Surface)
                {
                    string[] partNamesByElementId = mesh.GetPartNamesFromElementIds(elementIds);
                    HashSet<int> partVisualizationFaceIds = new HashSet<int>();
                    foreach (var partName in partNamesByElementId)
                        partVisualizationFaceIds.UnionWith(mesh.GetVisualizationFaceIds(partName, true));
                    ids = partVisualizationFaceIds.ToArray();
                }
                else if (_selection.SelectItem == vtkSelectItem.Part)
                {
                    return mesh.GetPartIdsFromElementIds(elementIds);
                }
                else if (_selection.SelectItem == vtkSelectItem.Geometry ||
                         _selection.SelectItem == vtkSelectItem.GeometrySurface)
                {
                    for (int i = 0; i < ids.Length; i++) ids[i] = FeMesh.GetGeometryPartIdFromGeometryId(ids[i]);
                }
                else throw new NotSupportedException();
            }
            else throw new NotSupportedException();
            //
            return ids;
        }
        private int[] GetNodeIdsFromFrustum(double[][] planeParameters, string[] selectionPartNames, vtkSelectBy selectBy)
        {
            int[] nodeIds;
            int[] elementIds;
            FeMesh mesh = DisplayedMesh;
            //
            _form.GetPointAndCellIdsInsideFrustum(planeParameters, selectionPartNames, out nodeIds, out elementIds);
            //
            if (selectBy == vtkSelectBy.Node)
            {
                if (nodeIds.Length > 0) return nodeIds;
            }
            else if (selectBy == vtkSelectBy.Element)
            {
                if (nodeIds.Length > 0 && elementIds.Length > 0)
                {
                    // Extract inside cells
                    vtkMaxActorData data = GetCellActorData(elementIds, nodeIds);
                    return data.Geometry.Nodes.Ids;
                }
            }
            else if (selectBy == vtkSelectBy.Part)
            {
                if (elementIds.Length > 0)
                {
                    string[] partNamesByElementId = mesh.GetPartNamesFromElementIds(elementIds);
                    HashSet<int> partNodeIds = new HashSet<int>();
                    foreach (var partName in partNamesByElementId) partNodeIds.UnionWith(mesh.Parts[partName].NodeLabels);
                    return partNodeIds.ToArray();
                }
            }
            else throw new NotSupportedException();
            //
            return new int[0];
        }
        public int[] GetElementIdsFromFrustum(double[][] planeParameters, string[] selectionPartNames, vtkSelectBy selectBy)
        {
            int[] nodeIds;
            int[] elementIds;
            FeMesh mesh = DisplayedMesh;
            //
            _form.GetPointAndCellIdsInsideFrustum(planeParameters, selectionPartNames, out nodeIds, out elementIds);
            //
            if (selectBy == vtkSelectBy.Node)
            {
                if (elementIds.Length > 0) return elementIds;
            }
            else if (selectBy == vtkSelectBy.Element)
            {
                // Extract inside cells
                vtkMaxActorData data = GetCellActorData(elementIds, nodeIds);
                return data.Geometry.Cells.Ids;
            }
            else if (selectBy == vtkSelectBy.Part)
            {
                if (elementIds.Length > 0)
                {
                    string[] partNamesByElementId = mesh.GetPartNamesFromElementIds(elementIds);
                    HashSet<int> partElementIds = new HashSet<int>();
                    foreach (var partName in partNamesByElementId) partElementIds.UnionWith(mesh.Parts[partName].Labels);
                    return partElementIds.ToArray();
                }
            }
            else throw new NotSupportedException();
            //
            return new int[0];
        }
        private int[] GetVisualizationFaceIdsFromFrustum(double[][] planeParameters, string[] selectionPartNames,
                                                         vtkSelectBy selectBy)

        {
            int[] ids;
            int[] nodeIds;
            int[] elementIds;
            // Create surface by area selecting nodes or elements
            if (selectBy == vtkSelectBy.Node || selectBy == vtkSelectBy.Element)
            {
                nodeIds = GetNodeIdsFromFrustum(planeParameters, selectionPartNames, selectBy);
                elementIds = GetElementIdsFromFrustum(planeParameters, selectionPartNames, selectBy);
                int[] frontIds = DisplayedMesh.GetVisualizationFaceIds(nodeIds, elementIds, false, true, false); // select back face
                ids = new int[2 * frontIds.Length];
                // Add front faces to selection
                for (int i = 0; i < frontIds.Length; i++)
                {
                    ids[i] = frontIds[i];                           // back faces: 10 * elementId + 0
                    ids[frontIds.Length + i] = frontIds[i] + 1;     // front faces: 10 * elementId + 1
                }
            }
            else if (selectBy == vtkSelectBy.Part)
            {
                FeMesh mesh = DisplayedMesh;
                _form.GetPointAndCellIdsInsideFrustum(planeParameters, selectionPartNames, out _, out elementIds);
                string[] partNamesByElementId = mesh.GetPartNamesFromElementIds(elementIds);
                HashSet<int> partVisualizationFaceIds = new HashSet<int>();
                foreach (var partName in partNamesByElementId)
                    partVisualizationFaceIds.UnionWith(mesh.GetVisualizationFaceIds(partName, true));
                ids = partVisualizationFaceIds.ToArray();
            }
            else throw new NotSupportedException();
            //
            return ids;
        }
        public int[] GetPartIdsFromFrustum(double[][] planeParameters, string[] selectionPartNames, vtkSelectBy selectBy)
        {
            int[] nodeIds;
            int[] elementIds;
            FeMesh mesh = DisplayedMesh;
            //
            _form.GetPointAndCellIdsInsideFrustum(planeParameters, selectionPartNames, out nodeIds, out elementIds);
            //
            if (selectBy == vtkSelectBy.Node)
            {
                 if (nodeIds.Length > 0) return mesh.GetPartIdsFromNodeIds(nodeIds);
            }
            else if (selectBy == vtkSelectBy.Element)
            {
                if (elementIds.Length > 0) return mesh.GetPartIdsFromElementIds(elementIds);
            }
            else if (selectBy == vtkSelectBy.Part)
            {
                if (nodeIds.Length > 0) return mesh.GetPartIdsFromNodeIds(nodeIds);
                else if (elementIds.Length > 0) return mesh.GetPartIdsFromElementIds(elementIds);
            }
            else throw new NotSupportedException();
            //
            return new int[0];
        }
        public double[][] GetRelativePartOffsets(SelectionNodeMouse selectionNodeMouse, out bool allZero)
        {
            double[][] partOffsets = null;
            // Previous exploded view
            if (selectionNodeMouse.PartOffsets != null && selectionNodeMouse.PartOffsets.Length > 0)
            {
                partOffsets = new double[selectionNodeMouse.PartOffsets.Length][];
                for (int i = 0; i < partOffsets.Length; i++)
                {
                    partOffsets[i] = (-1 * new Vec3D(selectionNodeMouse.PartOffsets[i])).Coor;
                }
            }
            // Current exploded view
            if (IsExplodedViewActive() && selectionNodeMouse.PartIds.Length > 0)
            {
                FeMesh mesh = DisplayedMesh;
                if (partOffsets == null) partOffsets = new double[selectionNodeMouse.PartIds.Length][];
                string[] partNames = mesh.GetPartNamesFromPartIds(selectionNodeMouse.PartIds);
                //
                for (int i = 0; i < partOffsets.Length; i++)
                {
                    if (partOffsets[i] == null) partOffsets[i] = new double[3];
                    partOffsets[i] = (new Vec3D(partOffsets[i]) + new Vec3D(mesh.Parts[partNames[i]].Offset)).Coor;
                }
            }
            //
            allZero = true;
            if (partOffsets != null)
            {
                for (int i = 0; i < partOffsets.Length; i++)
                {
                    if (partOffsets[i][0] != 0 || partOffsets[i][1] != 0 || partOffsets[i][2] != 0)
                    {
                        allZero = false;
                        break;
                    }
                }
            }
            //
            return partOffsets;
        }
        //
        private int GetGeometryId(double[] point, double[] selectionDirection, int selectionOnPartId, double precision)
        {
            int elementId;
            int[] cellFaceNodeIds;
            //
            string[] partNames;
            FeMesh mesh = DisplayedMesh;
            BasePart part = mesh.GetPartFromId(selectionOnPartId);
            if (part != null) partNames = new string[] { part.Name };
            else partNames = null;
            //
            _form.GetGeometryPickProperties(point, out elementId, out _, out cellFaceNodeIds, partNames);
            bool shellFrontFace = mesh.IsShellElementFrontFaceSelected(elementId, selectionDirection);
            return mesh.GetGeometryIdByPrecision(point, elementId, cellFaceNodeIds, shellFrontFace, precision);
        }
        private int GetGeometryVertexId(double[] point, int selectionOnPartId, double precision)
        {
            int elementId;
            int[] cellFaceNodeIds;
            //
            string[] partNames;
            BasePart part = DisplayedMesh.GetPartFromId(selectionOnPartId);
            if (part != null) partNames = new string[] { part.Name };
            else partNames = null;
            //
            _form.GetGeometryPickProperties(point, out elementId, out _, out cellFaceNodeIds, partNames);
            return DisplayedMesh.GetGeometryVertexIdByPrecision(point, elementId, cellFaceNodeIds, precision);
        }
        private int[] GetGeometryEdgeIdsByAngle(double[] point, double angle, int selectionOnPartId, bool shellEdgeFace)
        {
            int elementId;
            int[] edgeNodeIds;
            int[] cellFaceNodeIds;
            //
            string[] partNames;
            FeMesh mesh = DisplayedMesh;
            BasePart part = mesh.GetPartFromId(selectionOnPartId);
            if (part != null) partNames = new string[] { part.Name };
            else partNames = null;
            //
            _form.GetGeometryPickProperties(point, out elementId, out edgeNodeIds, out cellFaceNodeIds, partNames);
            return mesh.GetGeometryEdgeIdsByAngle(point, elementId, edgeNodeIds, cellFaceNodeIds, angle, shellEdgeFace);
        }
        private int[] GetGeometrySurfaceIdsByAngle(double[] point, double[] selectionDirection, double angle,
                                                   int selectionOnPartId)
        {
            int elementId;
            int[] cellFaceNodeIds;
            //
            string[] partNames;
            FeMesh mesh = DisplayedMesh;
            BasePart part = mesh.GetPartFromId(selectionOnPartId);
            if (part != null) partNames = new string[] { part.Name };
            else partNames = null;
            //
            _form.GetGeometryPickProperties(point, out elementId, out _, out cellFaceNodeIds, partNames);
            bool shellFrontFace = mesh.IsShellElementFrontFaceSelected(elementId, selectionDirection);
            return mesh.GetGeometrySurfaceIdsByAngle(elementId, cellFaceNodeIds, shellFrontFace, angle);
        }
        // Get mouse selection from id selection
        private Selection GetMouseSelectionFromSelectionNodeIds(Selection selectionIn)
        {
            if (selectionIn.SelectItem == vtkSelectItem.Surface)
            {
                int elementId;
                int itemId;
                int partId;
                int[] cellIds;
                int[] nodeIds;
                int[] itemTypePartIds;
                bool shellElement;
                double[] coor;
                double[] direction;
                FeNode node;
                FeFaceName faceName;
                vtkSelectBy selectBy;
                GeometryType geomType;
                BasePart part;
                SelectionNodeMouse mouseNode;
                // Copy current selection
                Selection currentSelection = _selection;
                _selection = selectionIn.DeepClone();
                _selection.Clear();
                //
                foreach (var selectionNode in selectionIn.Nodes)
                {
                    if (selectionNode is SelectionNodeIds selectionNodeIds)
                    {
                        if (selectionNodeIds.GeometryIds)
                        {
                            foreach (var geometryId in selectionNodeIds.ItemIds)
                            {
                                coor = new double[3];
                                direction = new double[3];
                                //
                                itemTypePartIds = FeMesh.GetItemTypePartIdsFromGeometryId(geometryId);
                                itemId = itemTypePartIds[0];
                                geomType = (GeometryType)itemTypePartIds[1];
                                partId = itemTypePartIds[2];
                                part = _model.Mesh.GetPartFromId(partId);
                                //
                                if (geomType == GeometryType.SolidSurface)
                                {
                                    cellIds = part.Visualization.CellIdsByFace[itemId];
                                    if (cellIds.Length > 0)
                                    {
                                        nodeIds = part.Visualization.Cells[cellIds[0]];
                                        for (int i = 0; i < nodeIds.Length; i++)
                                        {
                                            node = _model.Mesh.Nodes[nodeIds[i]];
                                            coor[0] += node.X;
                                            coor[1] += node.Y;
                                            coor[2] += node.Z;
                                        }
                                        coor[0] /= nodeIds.Length;
                                        coor[1] /= nodeIds.Length;
                                        coor[2] /= nodeIds.Length;
                                        //
                                        selectBy = vtkSelectBy.GeometrySurface;
                                    }
                                    else throw new NotSupportedException();
                                }
                                else if (geomType == GeometryType.ShellFrontSurface)
                                {
                                    cellIds = part.Visualization.CellIdsByFace[itemId];
                                    if (cellIds.Length > 0)
                                    {
                                        elementId = part.Visualization.CellIds[cellIds[0]];
                                        faceName = FeFaceName.S2;
                                        _model.Mesh.GetElementFaceCenterAndNormal(elementId, faceName, out coor, out direction,
                                                                                  out shellElement);
                                        // Invert direction of the S2 normal
                                        direction[0] *= -1;
                                        direction[1] *= -1;
                                        direction[2] *= -1;
                                        //
                                        selectBy = vtkSelectBy.GeometrySurface;
                                    }
                                    else throw new NotSupportedException();
                                }
                                else if (geomType == GeometryType.ShellBackSurface)
                                {
                                    cellIds = part.Visualization.CellIdsByFace[itemId];
                                    if (cellIds.Length > 0)
                                    {
                                        elementId = part.Visualization.CellIds[cellIds[0]];
                                        faceName = FeFaceName.S1;
                                        _model.Mesh.GetElementFaceCenterAndNormal(elementId, faceName, out coor, out direction,
                                                                                  out shellElement);
                                        // Invert direction of the S1 normal
                                        direction[0] *= -1;
                                        direction[1] *= -1;
                                        direction[2] *= -1;
                                        //
                                        selectBy = vtkSelectBy.GeometrySurface;
                                    }
                                    else throw new NotSupportedException();
                                }
                                else if (geomType == GeometryType.ShellEdgeSurface)
                                {
                                    cellIds = part.Visualization.EdgeCellIdsByEdge[itemId];
                                    if (cellIds.Length > 0)
                                    {
                                        nodeIds = part.Visualization.EdgeCells[cellIds[0]];
                                        for (int i = 0; i < nodeIds.Length; i++)
                                        {
                                            node = _model.Mesh.Nodes[nodeIds[i]];
                                            coor[0] += node.X;
                                            coor[1] += node.Y;
                                            coor[2] += node.Z;
                                        }
                                        coor[0] /= nodeIds.Length;
                                        coor[1] /= nodeIds.Length;
                                        coor[2] /= nodeIds.Length;
                                        //
                                        selectBy = vtkSelectBy.GeometryEdge;
                                    }
                                    else throw new NotSupportedException();
                                }
                                else throw new NotSupportedException();
                                //
                                mouseNode = new SelectionNodeMouse(coor, direction, null, false, vtkSelectOperation.Add,
                                                                   new int[] { partId }, new double[][] { part.Offset },
                                                                   selectBy, -1);
                                //
                                AddSelectionNode(mouseNode, false, false);
                            }
                        }
                    }
                }
                // Restore current selection
                Selection selectionOut = _selection.DeepClone();
                _selection = currentSelection;
                //
                return selectionOut;
            }
            else throw new NotSupportedException();
        }

        #endregion #################################################################################################################

        #region Extraction  ########################################################################################################
        public vtkMaxActorData GetNodeActorData(int[] nodeIds)
        {
            vtkMaxActorData data = new vtkMaxActorData();
            data.Geometry.Nodes.Coor = new double[nodeIds.Length][];
            //
            if (_currentView == ViewGeometryModelResults.Geometry && _model.Geometry != null)
            {
                for (int i = 0; i < nodeIds.Length; i++)
                {
                    data.Geometry.Nodes.Coor[i] = _model.Geometry.Nodes[nodeIds[i]].Coor;
                }
            }
            else if (_currentView == ViewGeometryModelResults.Model && _model.Mesh != null)
            {
                for (int i = 0; i < nodeIds.Length; i++)
                {
                    data.Geometry.Nodes.Coor[i] = _model.Mesh.Nodes[nodeIds[i]].Coor;
                }
            }
            else
            {
                CurrentResult.GetNodesAndValues(_currentFieldData, nodeIds, out data.Geometry.Nodes.Coor,
                                                out data.Geometry.Nodes.Values);
            }
            //
            return data;
        }
        //
        public vtkMaxActorData GetCellActorData(int[] elementIds, int[] nodeIds)
        {
            FeGroup elementSet;

            FeMesh mesh = DisplayedMesh;

            if (nodeIds != null) // keep only elements which are completely inside
            {
                HashSet<int> nodeIdsLookUp = new HashSet<int>(nodeIds);

                List<int> insideElements = new List<int>();
                int[] elementNodeIds;
                bool inside;
                for (int i = 0; i < elementIds.Length; i++)
                {
                    inside = true;
                    elementNodeIds = mesh.Elements[elementIds[i]].NodeIds;
                    for (int j = 0; j < elementNodeIds.Length; j++)
                    {
                        if (!nodeIdsLookUp.Contains(elementNodeIds[j]))
                        {
                            inside = false;
                            break;
                        }
                    }

                    if (inside) insideElements.Add(elementIds[i]);
                }
                elementSet = new FeGroup("tmp", insideElements.ToArray());
            }
            else                // keep all elements
            {
                elementSet = new FeGroup("tmp", elementIds);
            }

            return GetCellActorData(elementSet);

            //bool containsFaces = true;
            //if (nodeIds == null || nodeIds.Length == 0) containsFaces = false;
            //int[] faceIds = GetVisualizationFaceIds(nodeIds, elementIds, false, containsFaces);

            //List<int[]> cells = new List<int[]>();
            //foreach (int faceId in faceIds)
            //{
            //    cells.Add(DisplayedMesh.GetCellFromFaceId(faceId, out FeElement element));
            //}

            //vtkMaxActorData data = new vtkMaxActorData();
            //int[][] freeEdges = DisplayedMesh.GetFreeEdgesFromVisualizationCells(cells.ToArray());

            //DisplayedMesh.GetNodesAndCellsForEdges(freeEdges, out data.Actor.Nodes.Ids, out data.Actor.Nodes.Coor,
            //                                       out data.Actor.Cells.CellNodeIds, out data.Actor.Cells.Types);
            //return data;
        }
        public vtkMaxActorData GetCellActorData(FeGroup elementSet)
        {
            vtkMaxActorData data = new vtkMaxActorData();
            if (_currentView == ViewGeometryModelResults.Geometry && _model.Geometry != null)
            {
                _model.Geometry.GetSetNodesAndCells(elementSet, out data.Geometry.Nodes.Ids, out data.Geometry.Nodes.Coor,
                                                    out data.Geometry.Cells.Ids, out data.Geometry.Cells.CellNodeIds,
                                                    out data.Geometry.Cells.Types);
            }
            else if (_currentView == ViewGeometryModelResults.Model && _model.Mesh != null)
            {
                _model.Mesh.GetSetNodesAndCells(elementSet, out data.Geometry.Nodes.Ids, out data.Geometry.Nodes.Coor,
                                                out data.Geometry.Cells.Ids, out data.Geometry.Cells.CellNodeIds,
                                                out data.Geometry.Cells.Types);
            }
            else if(_currentView == ViewGeometryModelResults.Results && _allResults.CurrentResult.Mesh != null)
            {
                PartExchangeData actorResultData =
                    _allResults.CurrentResult.GetSetNodesCellsAndValues(elementSet, _currentFieldData);
                data = GetVtkData(actorResultData, null, null);
            }
            else throw new NotSupportedException();
            //
            return data;
        }
        public vtkMaxActorData GetCellFaceActorData(int elementId, int[] nodeIds)
        {
            if (elementId < 0) return null;
            // Get all faces containing at least 1 node id
            int[] faceIds = DisplayedMesh.GetVisualizationFaceIds(nodeIds, new int[] { elementId }, false, false, true);
            //
            bool add;
            int[] cell = null;
            FeElement element;
            HashSet<int> hashCell;
            // Find a face containing all node ids
            foreach (int faceId in faceIds)
            {
                cell = DisplayedMesh.GetCellFromFaceId(faceId, out ElementFaceType elementFaceType, out element);
                if (cell.Length < nodeIds.Length) continue;
                //
                hashCell = new HashSet<int>(cell);
                add = true;
                for (int i = 0; i < nodeIds.Length; i++)
                {
                    if (!hashCell.Contains(nodeIds[i]))
                    {
                        add = false;
                        break;
                    }
                }
                if (add) break;
            }
            // Get coordinates
            double[][] nodeCoor = new double[cell.Length][];
            for (int i = 0; i < cell.Length; i++) nodeCoor[i] = DisplayedMesh.Nodes[cell[i]].Coor;
            // Renumber cell node ids
            int[][] cells = new int[1][];
            cells[0] = new int[cell.Length];
            for (int i = 0; i < cell.Length; i++) cells[0][i] = i;
            // Get cell type
            int cellTypes;
            if (cell.Length == 3) cellTypes = (int)vtkCellType.VTK_TRIANGLE;
            else if (cell.Length == 4) cellTypes = (int)vtkCellType.VTK_QUAD;
            else if (cell.Length == 6) cellTypes = (int)vtkCellType.VTK_QUADRATIC_TRIANGLE;
            else if (cell.Length == 8) cellTypes = (int)vtkCellType.VTK_QUADRATIC_QUAD;
            else throw new NotSupportedException();
            //
            vtkMaxActorData data = new vtkMaxActorData();
            data.Geometry.Nodes.Ids = cell;
            data.Geometry.Nodes.Coor = nodeCoor;
            data.Geometry.Cells.CellNodeIds = cells;
            //
            data.Geometry.Cells.Types = new int[] { cellTypes };
            //
            return data;
        }
        public vtkMaxActorData GetEdgeActorData(int elementId, int[] edgeNodeIds)
        {
            vtkMaxActorData data = new vtkMaxActorData();
            int[][] edgeCells = DisplayedMesh.GetEdgeCells(elementId, edgeNodeIds);
            //
            if (edgeCells != null)
            {
                DisplayedMesh.GetNodesAndCellsForEdges(edgeCells, out data.Geometry.Nodes.Ids, out data.Geometry.Nodes.Coor,
                                                       out data.Geometry.Cells.CellNodeIds, out data.Geometry.Cells.Types);
                // Set the name for the probe widget
                data.Name = DisplayedMesh.GetEdgeGeometryIdFromNodeIds(elementId, edgeNodeIds).ToString();
                //
                return data;
            }
            else return null;
        }
        public vtkMaxActorData GetGeometryEdgeActorData(int[] geometryEdgeIds)
        {
            int[][] edgeCells = DisplayedMesh.GetEdgeCellsFromGeometryEdgeIds(geometryEdgeIds);
            if (edgeCells != null)
            {
                vtkMaxActorData data = new vtkMaxActorData();
                DisplayedMesh.GetNodesAndCellsForEdges(edgeCells, out data.Geometry.Nodes.Ids, out data.Geometry.Nodes.Coor,
                                                       out data.Geometry.Cells.CellNodeIds, out data.Geometry.Cells.Types);
                return data;
            }
            else return null;
        }
        public int[][] GetSurfaceCellsByFaceIds(int[] faceIds, out ElementFaceType[] elementFaceTypes)
        {
            int[][] cells = new int[faceIds.Length][];
            elementFaceTypes = new ElementFaceType[faceIds.Length];
            int count = 0;
            ElementFaceType cellType;
            FeMesh mesh = DisplayedMesh;        // it is used in loop
            //
            foreach (int faceId in faceIds)
            {
                cells[count] = mesh.GetCellFromFaceId(faceId, out cellType, out FeElement element);
                elementFaceTypes[count] = cellType;
                count++;
            }
            return cells;
        }
        public vtkMaxActorData GetSurfaceActorDataByNodeIds(int[] nodeIds)
        {
            int[] elementIds = DisplayedMesh.GetElementIdsFromNodeIds(nodeIds, false, true, false);
            int[] faceIds = DisplayedMesh.GetVisualizationFaceIds(nodeIds, elementIds, false, true, true);
            //
            int[][] cells = new int[faceIds.Length][];
            int count = 0;
            foreach (int faceId in faceIds)
            {
                cells[count] = DisplayedMesh.GetCellFromFaceId(faceId, out ElementFaceType elementFaceType, out FeElement element);
                count++;
            }
            //
            vtkMaxActorData data = new vtkMaxActorData();
            int[][] freeEdges = DisplayedMesh.GetFreeEdgesFromVisualizationCells(cells, null);
            //
            DisplayedMesh.GetNodesAndCellsForEdges(freeEdges, out data.Geometry.Nodes.Ids, out data.Geometry.Nodes.Coor,
                                                   out data.Geometry.Cells.CellNodeIds, out data.Geometry.Cells.Types);
            return data;
        }
        public vtkMaxActorData GetSurfaceEdgesActorDataFromElementId(int elementId, int[] cellFaceNodeIds,
                                                                     out string noEdgePartName)
        {
            // From element id and node ids get surface id and from surface id get free edges !!!
            BasePart part;
            int faceId;
            noEdgePartName = null;
            if (DisplayedMesh.GetFaceId(elementId, cellFaceNodeIds, out part, out faceId))
            {
                if (part.Visualization.EdgeCells.Length == 0)
                {
                    noEdgePartName = part.Name;
                    return null;
                }
                else
                {
                    int edgeId;
                    int edgeCellId;
                    List<int[]> edgeCells = new List<int[]>();
                    //
                    for (int i = 0; i < part.Visualization.FaceEdgeIds[faceId].Length; i++)
                    {
                        edgeId = part.Visualization.FaceEdgeIds[faceId][i];
                        for (int j = 0; j < part.Visualization.EdgeCellIdsByEdge[edgeId].Length; j++)
                        {
                            edgeCellId = part.Visualization.EdgeCellIdsByEdge[edgeId][j];
                            edgeCells.Add(part.Visualization.EdgeCells[edgeCellId]);
                        }
                    }
                    //
                    vtkMaxActorData data = new vtkMaxActorData();
                    DisplayedMesh.GetNodesAndCellsForEdges(edgeCells.ToArray(), out data.Geometry.Nodes.Ids, out data.Geometry.Nodes.Coor,
                                                           out data.Geometry.Cells.CellNodeIds, out data.Geometry.Cells.Types);
                    // Name for the probe widget
                    data.Name = FeMesh.GetGeometryId(faceId, (int)GeometryType.SolidSurface, part.PartId).ToString();
                    //
                    return data;
                }
            }
            else return null;
        }
        public vtkMaxActorData GetSurfaceEdgesActorDataFromNodeAndElementIds(int[] nodeIds, int[] elementIds,
                                                                             bool completelyInside,
                                                                             out string[] noEdgePartNames)
        {
            // Called by pick by area
            int itemId;
            int partId;
            int[] itemTypePartIds;
            FeMesh mesh = DisplayedMesh;
            int[] geometryIds = mesh.GetGeometryIds(nodeIds, elementIds, completelyInside);
            //
            BasePart part;
            HashSet<string> noEdgePartNamesHash = new HashSet<string>();
            int edgeId;
            int edgeCellId;
            HashSet<int[]> edgeCells = new HashSet<int[]>();
            HashSet<int> vertexNodeIds = new HashSet<int>();
            //
            foreach (var geometryId in geometryIds)
            {
                itemTypePartIds = FeMesh.GetItemTypePartIdsFromGeometryId(geometryId);
                GeometryType geomType = (GeometryType)itemTypePartIds[1];
                //
                itemId = itemTypePartIds[0];
                partId = itemTypePartIds[2];
                part = mesh.GetPartFromId(partId);
                // Vertex
                if (geomType == GeometryType.Vertex)
                {
                    vertexNodeIds.Add(part.Visualization.VertexNodeIds[itemId]);
                }
                else if (geomType == GeometryType.Edge)
                {
                    for (int j = 0; j < part.Visualization.EdgeCellIdsByEdge[itemId].Length; j++)
                    {
                        edgeCellId = part.Visualization.EdgeCellIdsByEdge[itemId][j];
                        edgeCells.Add(part.Visualization.EdgeCells[edgeCellId]);
                    }
                }
                // Surface - but do not select shell edge surfaces
                else if (geomType.IsSurface()) 
                {
                    if (part.Visualization.EdgeCells.Length == 0) noEdgePartNamesHash.Add(part.Name);
                    else
                    {
                        for (int i = 0; i < part.Visualization.FaceEdgeIds[itemId].Length; i++)
                        {
                            edgeId = part.Visualization.FaceEdgeIds[itemId][i];
                            for (int j = 0; j < part.Visualization.EdgeCellIdsByEdge[edgeId].Length; j++)
                            {
                                edgeCellId = part.Visualization.EdgeCellIdsByEdge[edgeId][j];
                                edgeCells.Add(part.Visualization.EdgeCells[edgeCellId]);
                            }
                        }
                    }
                }                
            }
            //
            vtkMaxActorData data = new vtkMaxActorData();
            // Draw edges
            if (edgeCells.Count > 0)
            {
                DisplayedMesh.GetNodesAndCellsForEdges(edgeCells.ToArray(), out data.Geometry.Nodes.Ids, out data.Geometry.Nodes.Coor,
                                                       out data.Geometry.Cells.CellNodeIds, out data.Geometry.Cells.Types);
            }
            else
            // Draw nodes
            {
                double[][] nodeCoor = DisplayedMesh.GetNodeSetCoor(vertexNodeIds.ToArray(), true);
                data.NodeSize = _settings.Pre.NodeSymbolSize;
                data.Geometry.Nodes.Coor = nodeCoor;
            }
            // Name for the probe widget
            data.Name = geometryIds.ToString();
            //
            noEdgePartNames = noEdgePartNamesHash.ToArray();
            //
            return data;
        }
        //
        public int[][] GetSurfaceCellsByGeometryId(int[] geometrySurfaceIds, out ElementFaceType[] elementFaceTypes)
        {
            if (geometrySurfaceIds.Length != 1) throw new NotSupportedException();
            //
            int[][] cells = DisplayedMesh.GetSurfaceCells(geometrySurfaceIds[0], out elementFaceTypes);
            //
            return cells;
        }
        public vtkMaxActorData GetPartActorData(int[] elementIds)
        {
            FeMesh mesh = DisplayedMesh;
            //
            HashSet<int> partIds = new HashSet<int>();
            for (int i = 0; i < elementIds.Length; i++)
            {
                partIds.Add(mesh.Elements[elementIds[i]].PartId);
            }
            //
            List<int> allElementIds = new List<int>();
            foreach (var entry in mesh.Parts)
            {
                if (partIds.Contains(entry.Value.PartId))
                {
                    allElementIds.AddRange(entry.Value.Labels);
                }
            }
            //
            FeGroup elementSet = new FeGroup("tmp", allElementIds.ToArray());
            //
            return GetCellActorData(elementSet);
        }
        public vtkMaxActorData GetGeometryActorData(double[] point, int elementId, int[] edgeNodeIds, int[] cellFaceNodeIds,
                                                    out string noEdgePartName)
        {
            // Used for mouse move selection
            noEdgePartName = null;
            double precision = _form.GetSelectionPrecision();
            FeMesh mesh = DisplayedMesh;            
            int geomId = mesh.GetGeometryIdByPrecision(point, elementId, cellFaceNodeIds, false, precision);
            int[] itemTypePartIds = FeMesh.GetItemTypePartIdsFromGeometryId(geomId);
            GeometryType geomType = (GeometryType)itemTypePartIds[1];
            //
            if (geomType == GeometryType.Vertex)
            {
                int[] nodeIds = mesh.GetNodeIdsFromGeometryId(geomId);
                return GetNodeActorData(nodeIds);
            }
            else if (geomType.IsEdge())
            {
                return GetGeometryEdgeActorData(new int[] { geomId });
            }
            else if (geomType.IsSurface())
            {
                return GetSurfaceEdgesActorDataFromElementId(elementId, cellFaceNodeIds, out noEdgePartName);
            }
            else throw new NotSupportedException();
        }
        public vtkMaxActorData GetGeometryVertexActorData(double[] point, int elementId,
                                                          int[] edgeNodeIds, int[] cellFaceNodeIds)
        {
            double precision = _form.GetSelectionPrecision();
            FeMesh mesh = DisplayedMesh;
            int geomId = mesh.GetGeometryVertexIdByPrecision(point, elementId, cellFaceNodeIds, precision);
            // If no vertex is selected
            if (geomId < 0) return null;
            else
            {
                int[] itemTypePartIds = FeMesh.GetItemTypePartIdsFromGeometryId(geomId);
                GeometryType geomType = (GeometryType)itemTypePartIds[1];
                //
                if (geomType == GeometryType.Vertex)
                {
                    int[] nodeIds = mesh.GetNodeIdsFromGeometryId(geomId);
                    return GetNodeActorData(nodeIds);
                }
                else throw new NotSupportedException();
            }
        }

        #endregion #################################################################################################################

        public void ResetAllJobStatus()
        {
            foreach (var entry in _jobs)
            {
                entry.Value.ResetJobStatus();
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, entry.Key, entry.Value, null, false);
            }
        }
        public string[] CheckAndUpdateModelValidity()
        {
            // Update user keywords
            if (_model != null && _model.CalculixUserKeywords != null)
            {
                int num = _model.CalculixUserKeywords.Count;
                _model.RemoveLostUserKeywords(_form.SetNumberOfModelUserKeywords);
                int delta = num - _model.CalculixUserKeywords.Count;
                if (delta > 0) MessageBoxes.ShowWarning("Number of removed CalculiX user keywords: " + delta + ".");
            }
            // Tuple<NamedClass, string>   ...   Tuple<invalidItem, stepName>
            List<Tuple<NamedClass, string>> items = new List<Tuple<NamedClass, string>>();
            string[] invalidModelItems = _model.CheckValidity(items);
            foreach (var entry in items)
            {
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, entry.Item1.Name, entry.Item1, entry.Item2, false);
            }
            //
            return invalidModelItems;
        }
        public string[] CheckAndUpdateResultValidity()
        {
            // Tuple<NamedClass, string>   ...   Tuple<invalidItem, stepName>
            List<Tuple<NamedClass, string>> items = new List<Tuple<NamedClass, string>>();
            string[] invalidResultItems = _allResults.CurrentResult.CheckValidity(items);
            foreach (var entry in items)
            {
                _form.UpdateTreeNode(ViewGeometryModelResults.Results, entry.Item1.Name, entry.Item1, entry.Item2, false);
            }
            //
            return invalidResultItems;
        }

        // Common
        public string[] GetAllMeshEntityNames()
        {
            List<string> names = new List<string>();
            if (_model != null)
            {
                names.AddRange(_model.GetAllMeshEntityNames());
            }
            return names.ToArray();
        }
        public double[] GetBoundingBox()
        {
            // xMin, xMax, yMin, yMax, zMin, zMax
            return _form.GetBoundingBox();
        }
        public double[] GetBoundingBoxSize()
        {
            return _form.GetBondingBoxSize();
        }
        public string GetBoundingBoxSizeAsString()
        {
            double[] size = _form.GetBondingBoxSize();
            return string.Format("x: {0}   y: {1}   z: {2}", size[0], size[1], size[2]);
        }
        public void ClearErrors()
        {
            _errors.Clear();
        }
        public int OutputErrors()
        {
            if (_errors.Count > 0)
            {
                _form.WriteDataToOutput("");
                foreach (var line in _errors) _form.WriteDataToOutput("Error: " + line);
            }
            return _errors.Count;
        }

        // Visualize
        public void Redraw(bool resetCamera = false)
        {
            if (_currentView == ViewGeometryModelResults.Geometry) DrawGeometry(resetCamera);
            else if (_currentView == ViewGeometryModelResults.Model) DrawModel(resetCamera);
            else if (_currentView == ViewGeometryModelResults.Results) DrawResults(resetCamera);
            else throw new NotSupportedException();
        }
        //
        #region Draw  ##############################################################################################################
        // Update model
        public void FeModelUpdate(UpdateType updateType)
        {
            // First check the validity to correctly draw the symbols
            if (updateType.HasFlag(UpdateType.Check)) CheckAndUpdateModelValidity();
            if (updateType.HasFlag(UpdateType.DrawModel)) DrawModel(updateType.HasFlag(UpdateType.ResetCamera));
            if (updateType.HasFlag(UpdateType.RedrawSymbols)) RedrawSymbols();
        }
        public void FeResultsUpdate(UpdateType updateType)
        {
            if (updateType.HasFlag(UpdateType.Check)) CheckAndUpdateResultValidity();
            if (updateType.HasFlag(UpdateType.DrawResults)) DrawResults(updateType.HasFlag(UpdateType.ResetCamera));
        }
        private vtkMaxActorRepresentation GetRepresentation(BasePart part)
        {
            if (part.PartType == PartType.Solid) return vtkMaxActorRepresentation.Solid;
            else if (part.PartType == PartType.SolidAsShell) return vtkMaxActorRepresentation.SolidAsShell;
            else if (part.PartType == PartType.Shell) return vtkMaxActorRepresentation.Shell;
            else if (part.PartType == PartType.Wire) return vtkMaxActorRepresentation.Wire;
            else throw new NotSupportedException();
        }
        // Geometry mesh
        public void DrawGeometry(bool resetCamera)
        {
            try
            {
                // Set the current view and call DrawGeometry
                if (_currentView != ViewGeometryModelResults.Geometry) CurrentView = ViewGeometryModelResults.Geometry;
                // Draw geometry
                else
                {
                    _form.Clear3D();    // Removes section cut
                    //
                    if (_model != null)
                    {
                        if (_model.Geometry != null && _model.Geometry.Parts.Count > 0)
                        {
                            ApplyModelUnitSystem();
                            //
                            DrawGeomParts();
                            AnnotateWithColorLegend();
                            _annotations.DrawAnnotations();
                            //
                            Octree.Plane plane = _sectionViews.GetCurrentSectionViewPlane();
                            if (plane != null) ApplySectionView(plane.Point.Coor, plane.Normal.Coor);
                        }
                        UpdateHighlight();
                    }
                    //
                    if (resetCamera) _form.SetFrontBackView(false, true);
                    _form.AdjustCameraDistanceAndClipping();
                }
            }
            catch
            {
                // Do not throw an error - it might cancel a procedure
            }
        }
        public void DrawGeomParts()
        {
            if (_model == null) return;
            //
            vtkRendererLayer layer = vtkRendererLayer.Base;
            List<string> hiddenActors = new List<string>();
            //
            foreach (var entry in _model.Geometry.Parts)
            {
                if (entry.Value is CompoundGeometryPart) continue;
                //
                DrawGeomPart(_model.Geometry, entry.Value, layer, true, true);
                //
                if (!entry.Value.Visible) hiddenActors.Add(entry.Key);
            }
            if (hiddenActors.Count > 0) _form.HideActors(hiddenActors.ToArray(), false);
            //
            _form.AdjustCameraDistanceAndClipping();
        }
        private void DrawGeomPart(FeMesh mesh, BasePart part, vtkRendererLayer layer, bool canHaveElementEdges,
                                  bool pickable)
        {
            vtkMaxActorData data = GetGeometryPartActorData(mesh, part, layer, canHaveElementEdges, pickable);
            //
            if (data != null)
            {
                ApplyLighting(data);
                _form.Add3DCells(data);
            }
        }
        private vtkMaxActorData GetGeometryPartActorData(FeMesh mesh, BasePart part,
                                                         vtkRendererLayer layer,
                                                         bool canHaveElementEdges,
                                                         bool pickable)
        {
            if (part.Labels.Length == 0) return null;
            //
            vtkMaxActorData data = new vtkMaxActorData();
            data.Name = part.Name;
            GetPartColor(part, ref data.Color, ref data.BackfaceColor);
            data.Layer = layer;
            data.CanHaveElementEdges = canHaveElementEdges;
            data.Pickable = pickable;
            data.SmoothShaded = part.SmoothShaded;
            data.ActorRepresentation = GetRepresentation(part);
            // Get all nodes and elements - renumbered
            if (pickable)
            {
                data.CellLocator = new PartExchangeData();
                mesh.GetSetNodesAndCells(part, out data.CellLocator.Nodes.Ids, out data.CellLocator.Nodes.Coor,
                                         out data.CellLocator.Cells.Ids, out data.CellLocator.Cells.CellNodeIds,
                                         out data.CellLocator.Cells.Types);
            }
            // Get only needed nodes and elements - renumbered
            mesh.GetVisualizationNodesAndCells(part, out data.Geometry.Nodes.Ids, out data.Geometry.Nodes.Coor,
                                               out data.Geometry.Cells.Ids, out data.Geometry.Cells.CellNodeIds,
                                               out data.Geometry.Cells.Types);
            // Model edges
            if (part.PartType.HasEdges() && part.Visualization.EdgeCells != null)
            {
                data.ModelEdges = new PartExchangeData();
                mesh.GetNodesAndCellsForModelEdges(part, out data.ModelEdges.Nodes.Ids, out data.ModelEdges.Nodes.Coor,
                                                   out data.ModelEdges.Cells.CellNodeIds, out data.ModelEdges.Cells.Types);
            }
            // Back face
            if (part.PartType == PartType.Shell) data.BackfaceCulling = false;
            //
            data.NodeSize = Globals.BeamNodeSize;
            //
            return data;
        }
        // Model
        public void DrawModel(bool resetCamera)
        {
            bool rendering = _form.RenderingOn;
            //
            try
            {
                // Set the current view and call DrawModel
                if (_currentView != ViewGeometryModelResults.Model) CurrentView = ViewGeometryModelResults.Model;
                // Draw model
                else
                {
                    if (rendering) _form.RenderingOn = false;
                    _form.Clear3D();    // Removes section cut
                    //
                    if (_model != null)
                    {
                        if (_model.Mesh != null && _model.Mesh.Parts.Count > 0)
                        {
                            ApplyModelUnitSystem();
                            // Must be inside to continue screen update
                            try
                            {
                                DrawAllModelParts();
                                AnnotateWithColorLegend();
                                DrawSymbols();
                                _annotations.DrawAnnotations();
                                //
                                Octree.Plane plane = _sectionViews.GetCurrentSectionViewPlane();
                                if (plane != null) ApplySectionView(plane.Point.Coor, plane.Normal.Coor);
                            }
                            catch { }
                        }
                        UpdateHighlight();
                    }
                    //
                    if (resetCamera) _form.SetFrontBackView(false, true);
                    //_form.AdjustCameraDistanceAndClipping();
                }
            }
            catch
            {
                // Do not throw an error - it might cancel a procedure
            }
            finally
            {
                if (rendering) _form.RenderingOn = true;
            }
        }
        public void DrawAllModelParts()
        {
            if (_model == null) return;
            //
            IDictionary<string, BasePart> parts = _model.Mesh.Parts;
            vtkRendererLayer layer = vtkRendererLayer.Base;
            //
            List<string> hiddenActors = new List<string>();
            //
            Dictionary<int, int> elementIdColorId = null;
            if (_annotateWithColor == AnnotateWithColorEnum.Sections)
                _model.GetSectionAssignments(out elementIdColorId);
            else if (_annotateWithColor == AnnotateWithColorEnum.Materials)
                _model.GetMaterialAssignments(out elementIdColorId);
            else if (_annotateWithColor == AnnotateWithColorEnum.SectionThicknesses)
                _model.GetSectionThicknessAssignments(out elementIdColorId);
            //
            foreach (var entry in parts)
            {
                DrawModelPart(_model.Mesh, entry.Value, layer, elementIdColorId);
                //
                if (!entry.Value.Visible) hiddenActors.Add(entry.Key);
            }
            if (hiddenActors.Count > 0) _form.HideActors(hiddenActors.ToArray(), false);
        }
        public void DrawModelPart(FeMesh mesh, BasePart part, vtkRendererLayer layer,
                                  Dictionary<int, int> elementIdColorId = null)
        {
            vtkMaxActorData data = GetModelPartActorData(mesh, part, layer, elementIdColorId);
            //
            if (data != null)
            {
                ApplyLighting(data);
                _form.Add3DCells(data);
            }
        }
        public vtkMaxActorData GetModelPartActorData(FeMesh mesh, BasePart part, vtkRendererLayer layer,
                                                                Dictionary<int, int> elementIdColorId = null)
        {
            if (part is CompoundGeometryPart) return null;
            if (part.Labels.Length == 0) return null;
            // Data
            vtkMaxActorData data = new vtkMaxActorData();
            data.Name = part.Name;
            GetPartColor(part, ref data.Color, ref data.BackfaceColor);
            data.Layer = layer;
            data.CanHaveElementEdges = true;
            data.Pickable = true;
            data.SmoothShaded = part.SmoothShaded;
            data.ActorRepresentation = GetRepresentation(part);
            // Get all nodes and elements for selection - renumbered
            data.CellLocator = new PartExchangeData();
            mesh.GetSetNodesAndCells(part, out data.CellLocator.Nodes.Ids, out data.CellLocator.Nodes.Coor,
                                     out data.CellLocator.Cells.Ids,
                                     out data.CellLocator.Cells.CellNodeIds,
                                     out data.CellLocator.Cells.Types);
            // Get only needed nodes and elements - renumbered
            mesh.GetVisualizationNodesAndCells(part, out data.Geometry.Nodes.Ids, out data.Geometry.Nodes.Coor,
                                               out data.Geometry.Cells.Ids,
                                               out data.Geometry.Cells.CellNodeIds,
                                               out data.Geometry.Cells.Types);
            // Custom coloring of elements
            if (elementIdColorId != null)
            {
                int maxColor = -int.MaxValue;
                float value;
                float[] values = new float[data.Geometry.Cells.Ids.Length];
                ColorSettings colorSettings = _settings.Color;
                for (int i = 0; i < values.Length; i++)
                {
                    value = elementIdColorId[data.Geometry.Cells.Ids[i]] % colorSettings.ColorTable.Length;
                    values[i] = value;
                    if (value > maxColor) maxColor = (int)value;
                }
                data.Geometry.Cells.Values = values;
                //
                maxColor++; // first color is 0 so add +1
                data.ColorTable = new Color[maxColor];
                Array.Copy(colorSettings.ColorTable, data.ColorTable, maxColor);
                //
                //data.ColorTable = colorSettings.ColorTable;
            }
            // Back face
            if (part.PartType == PartType.Shell) data.BackfaceCulling = false;
            // Model edges
            if (part.PartType.HasEdges() && part.Visualization.EdgeCells != null)
            {
                data.ModelEdges = new PartExchangeData();
                mesh.GetNodesAndCellsForModelEdges(part, out data.ModelEdges.Nodes.Ids, out data.ModelEdges.Nodes.Coor,
                                                   out data.ModelEdges.Cells.CellNodeIds, out data.ModelEdges.Cells.Types);
            }
            //
            data.NodeSize = Globals.BeamNodeSize;
            //
            return data;
        }
        
        //
        private void GetPartColor(BasePart part, ref Color color, ref Color backfaceColor)
        {
            color = part.Color;
            // wire part
            foreach (var elType in part.ElementTypes)
            {
                if (elType == typeof(LinearBeamElement) || elType == typeof(ParabolicBeamElement))
                {
                    color = Color.Black;
                    break;
                }
            }
            if (_annotateWithColor == AnnotateWithColorEnum.FaceOrientation)
            {
                if (part.PartType == PartType.Shell)
                {
                    ColorSettings colorSettings = _settings.Color;
                    color = colorSettings.FrontFaceColor;
                    backfaceColor = colorSettings.BackFaceColor;
                }
                else if (part.PartType == PartType.Solid || part.PartType == PartType.SolidAsShell)
                {
                    color = Color.White;
                }
            }
        }
        private void AnnotateWithColorLegend()
        {
            if (_annotateWithColor == AnnotateWithColorEnum.None) return;
            // Clears the contents
            _form.HideColorBar();   
            //
            if (_currentView == ViewGeometryModelResults.Results && _viewResultsType == ViewResultsTypeEnum.ColorContours) return;
            // Face orientation legend
            ColorSettings colorSettings = _settings.Color;
            if (_annotateWithColor == AnnotateWithColorEnum.FaceOrientation)
            {
                _form.SetColorBarColorsAndLabels(new Color[] { colorSettings.FrontFaceColor, colorSettings.BackFaceColor},
                                                               new string[] { "Front face", "Back face" });
            }
            if (_annotateWithColor == AnnotateWithColorEnum.Parts)
            {
                FeMesh mesh = DisplayedMesh;
                List<Color> partColors = new List<Color>();
                List<string> partNames = new List<string>();
                foreach (var entry in mesh.Parts)
                {
                    if (entry.Value.Visible)
                    {
                        if (!(entry.Value is CompoundGeometryPart))
                        {
                            partColors.Add(entry.Value.Color);
                            partNames.Add(entry.Value.Name);
                        }
                    }
                }
                _form.SetColorBarColorsAndLabels(partColors.ToArray(), partNames.ToArray());
            }
            if (_annotateWithColor == AnnotateWithColorEnum.Materials)
            {
                if (_currentView == ViewGeometryModelResults.Model)
                {
                    // Get active materials
                    HashSet<string> activeMaterials = new HashSet<string>();
                    foreach (var entry in _model.Sections) activeMaterials.Add(entry.Value.MaterialName);
                    //
                    List<Color> materialColors = new List<Color>();
                    List<string> materialNames = new List<string>();
                    int count = 0;
                    foreach (var entry in _model.Materials)
                    {
                        if (activeMaterials.Contains(entry.Value.Name))
                        {
                            materialColors.Add(colorSettings.ColorTable[count++]);
                            materialNames.Add(entry.Value.Name);
                        }
                    }
                    _form.SetColorBarColorsAndLabels(materialColors.ToArray(), materialNames.ToArray());
                }
            }
            if (_annotateWithColor == AnnotateWithColorEnum.Sections)
            {
                if (_currentView == ViewGeometryModelResults.Model)
                {
                    List<Color> sectionColors = new List<Color>();
                    List<string> sectionNames = new List<string>();
                    int count = 0;
                    foreach (var entry in _model.Sections)
                    {
                        sectionColors.Add(colorSettings.ColorTable[count++]);
                        sectionNames.Add(entry.Value.Name);
                    }
                    _form.SetColorBarColorsAndLabels(sectionColors.ToArray(), sectionNames.ToArray());
                }
            }
            if (_annotateWithColor == AnnotateWithColorEnum.SectionThicknesses)
            {
                if (_currentView == ViewGeometryModelResults.Model)
                {
                    List<Color> sectionThicknessColors = new List<Color>();
                    HashSet<double> sectionThickness = new HashSet<double>();                    
                    int count = 0;
                    double thickness;
                    foreach (var entry in _model.Sections)
                    {
                        thickness = _model.GetSectionThickness(entry.Value);
                        //
                        if (thickness != -1 && !sectionThickness.Contains(thickness))
                        {
                            sectionThicknessColors.Add(colorSettings.ColorTable[count++]);
                            sectionThickness.Add(thickness);
                        }
                    }
                    // Sort thicknesses
                    double[] sectionThicknessArray = sectionThickness.ToArray();
                    Array.Sort(sectionThicknessArray);
                    // Add unit
                    string[] sectionThicknessNames = new string[sectionThicknessArray.Length];
                    for (int i = 0; i < sectionThicknessArray.Length; i++)
                        sectionThicknessNames[i] = sectionThicknessArray[i] + " " + _model.UnitSystem.LengthUnitAbbreviation;
                    //
                    _form.SetColorBarColorsAndLabels(sectionThicknessColors.ToArray(), sectionThicknessNames);
                }
            }
            if (_annotateWithColor.HasFlag(AnnotateWithColorEnum.ReferencePoints))
            {
                if (_currentView == ViewGeometryModelResults.Model && _drawSymbolsForStep != null && _drawSymbolsForStep != "None")
                {                    List<Color> itemColors = new List<Color>();
                    List<string> itemNames = new List<string>();
                    // Reference points
                    foreach (var entry in _model.Mesh.ReferencePoints)
                    {
                        if (entry.Value.Visible && entry.Value.Active)
                        {
                            itemColors.Add(entry.Value.Color);
                            itemNames.Add(entry.Value.Name);
                        }
                    }
                    _form.SetColorBarColorsAndLabels(itemColors.ToArray(), itemNames.ToArray());
                }
            }
            if (_annotateWithColor.HasFlag(AnnotateWithColorEnum.Constraints))
            {
                if (_currentView == ViewGeometryModelResults.Model && _drawSymbolsForStep != null && _drawSymbolsForStep != "None")
                {
                    List<Color> itemColors = new List<Color>();
                    List<string> itemNames = new List<string>();
                    // Constraints
                    foreach (var entry in _model.Constraints)
                    {
                        if (entry.Value.Visible && entry.Value.Active)
                        {
                            if (entry.Value is PointSpring pts)
                            {
                                itemColors.Add(pts.MasterColor);
                                itemNames.Add(pts.Name);
                            }
                            else if (entry.Value is SurfaceSpring srs)
                            {
                                itemColors.Add(srs.MasterColor);
                                itemNames.Add(srs.Name);
                            }
                            else if (entry.Value is CompressionOnly co)
                            {
                                itemColors.Add(co.MasterColor);
                                itemNames.Add(co.Name);
                            }
                            else if (entry.Value is RigidBody rb)
                            {
                                itemColors.Add(rb.MasterColor);
                                itemNames.Add(rb.Name);
                            }
                            else if (entry.Value is Tie tie)
                            {
                                if (tie.MasterColor != tie.SlaveColor)
                                {
                                    itemColors.Add(tie.MasterColor);
                                    itemNames.Add(tie.Name + " Master");
                                    itemColors.Add(tie.SlaveColor);
                                    itemNames.Add(tie.Name + " Slave");
                                }
                                else
                                {
                                    itemColors.Add(tie.MasterColor);
                                    itemNames.Add(tie.Name);
                                }
                            }
                            else throw new NotSupportedException();
                        }
                    }
                    _form.AddColorBarColorsAndLabels(itemColors.ToArray(), itemNames.ToArray());
                }
            }
            if (_annotateWithColor.HasFlag(AnnotateWithColorEnum.ContactPairs))
            {
                if (_currentView == ViewGeometryModelResults.Model && _drawSymbolsForStep != null && _drawSymbolsForStep != "None")
                {
                    List<Color> itemColors = new List<Color>();
                    List<string> itemNames = new List<string>();
                    // Contact pairs
                    foreach (var entry in _model.ContactPairs)
                    {
                        if (entry.Value.Visible && entry.Value.Active)
                        {
                            if (entry.Value.MasterColor != entry.Value.SlaveColor)
                            {
                                itemColors.Add(entry.Value.MasterColor);
                                itemNames.Add(entry.Value.Name + " Master");
                                itemColors.Add(entry.Value.SlaveColor);
                                itemNames.Add(entry.Value.Name + " Slave");
                            }
                            else
                            {
                                itemColors.Add(entry.Value.MasterColor);
                                itemNames.Add(entry.Value.Name);
                            }
                        }
                    }
                    _form.AddColorBarColorsAndLabels(itemColors.ToArray(), itemNames.ToArray());
                }
            }
            if (_annotateWithColor.HasFlag(AnnotateWithColorEnum.BoundaryConditions))
            {
                if (_currentView == ViewGeometryModelResults.Model && _drawSymbolsForStep != null &&
                    _drawSymbolsForStep != "None" && _drawSymbolsForStep != "Model")
                {
                    List<Color> itemColors = new List<Color>();
                    List<string> itemNames = new List<string>();
                    // Boundary Conditions
                    foreach (var entry in _model.StepCollection.GetStep(_drawSymbolsForStep).BoundaryConditions)
                    {
                        if (entry.Value.Visible && entry.Value.Active)
                        {
                            itemColors.Add(entry.Value.Color);
                            itemNames.Add(entry.Value.Name);
                        }
                    }
                    _form.AddColorBarColorsAndLabels(itemColors.ToArray(), itemNames.ToArray());
                }
            }
            if (_annotateWithColor.HasFlag(AnnotateWithColorEnum.Loads))
            {
                if (_currentView == ViewGeometryModelResults.Model && _drawSymbolsForStep != null &&
                    _drawSymbolsForStep != "None" && _drawSymbolsForStep != "Model")
                {
                    List<Color> itemColors = new List<Color>();
                    List<string> itemNames = new List<string>();
                    // Boundary Conditions
                    foreach (var entry in _model.StepCollection.GetStep(_drawSymbolsForStep).Loads)
                    {
                        if (entry.Value.Visible && entry.Value.Active)
                        {
                            itemColors.Add(entry.Value.Color);
                            itemNames.Add(entry.Value.Name);
                        }
                    }
                    _form.AddColorBarColorsAndLabels(itemColors.ToArray(), itemNames.ToArray());
                }
            }
            // Pre-processing settings
            PreSettings ps = _settings.Pre;
            _form.DrawColorBarBackground(ps.ColorBarBackgroundType == AnnotationBackgroundType.White);
            _form.DrawColorBarBorder(ps.ColorBarDrawBorder);
            //
        }
        // Symbols
        public void DrawSymbols()
        {
            if (_drawSymbolsForStep != null && _drawSymbolsForStep != "None")
            {
                DrawAllReferencePoints();
                DrawAllConstraints();
                DrawAllContactPairs();
                if (_drawSymbolsForStep != "Model")
                {
                    DrawAllBoundaryConditions(_drawSymbolsForStep);
                    DrawAllLoads(_drawSymbolsForStep);
                }
            }
            // Update color legend
            AnnotateWithColorLegend();
            //
            _form.AdjustCameraDistanceAndClipping();
        }
        public void RedrawSymbols(bool updateHighlights = true)
        {
            try
            {
                if (_model != null)
                {
                    if (_currentView == ViewGeometryModelResults.Model &&
                        _model.Mesh != null && _model.Mesh.Parts.Count > 0)
                    {
                        // Clear
                        _form.ClearButKeepParts(_model.Mesh.Parts.Keys.ToArray());
                        //
                        try
                        {
                            // Must be inside to continue screen update
                            if (_currentView != ViewGeometryModelResults.Model) CurrentView = ViewGeometryModelResults.Model;
                            DrawSymbols();
                            //
                            Octree.Plane plane = _sectionViews.GetCurrentSectionViewPlane();
                            if (plane != null)
                            {
                                RemoveSectionView();
                                ApplySectionView(plane.Point.Coor, plane.Normal.Coor);
                            }
                        }
                        catch { }
                        //
                        if (updateHighlights) UpdateHighlight();
                        _form.AdjustCameraDistanceAndClipping();
                    }
                }
            }
            catch
            {
                // do not throw an error - it might cancel a procedure
            }
        }

        // Reference points
        public void DrawAllReferencePoints()
        {
            vtkRendererLayer layer = vtkRendererLayer.Overlay;
            //
            foreach (var entry in _model.Mesh.ReferencePoints)
            {
                DrawReferencePoint(entry.Value, entry.Value.Color, layer);
            }
        }

        public void DrawReferencePoint(FeReferencePoint referencePoint, Color color, vtkRendererLayer layer)
        {
            try
            {
                if (!((referencePoint.Active && referencePoint.Visible && referencePoint.Valid &&
                      !referencePoint.Internal) || layer == vtkRendererLayer.Selection)) return;
                //
                int nodeSize = _settings.Pre.NodeSymbolSize + 5;
                nodeSize = (int)(nodeSize * _settings.Pre.SymbolSize / 50.0);  // 50 is the default size
                //
                Color colorBorder = Color.Black;
                //
                double[][] coor = new double[][] { referencePoint.Coor() };
                //
                //vtkMaxActorData data = new vtkMaxActorData();
                //data.Name = "Reference_point" + Globals.NameSeparator + referencePoint.Name;
                //data.Color = color;
                //data.BackfaceColor = colorBorder;
                //data.Layer = layer;
                //data.Geometry.Nodes.Coor = coor;
                //data.Pickable = true;
                //ApplyLighting(data);
                //_form.AddSphereActor(data, 2 * nodeSize);
                //
                DrawNodes(referencePoint.Name + Globals.NameSeparator + "Border", coor, colorBorder, layer, nodeSize,
                          false, false);
                DrawNodes(referencePoint.Name, coor, color, layer, nodeSize - 3, false, false);
            }
            catch { } // do not show the exception to the user
        }
        // Constraints
        public void DrawAllConstraints()
        {
            int symbolSize = _settings.Pre.SymbolSize;
            int nodeSymbolSize = _settings.Pre.NodeSymbolSize;
            vtkRendererLayer layer = vtkRendererLayer.Base;
            //
            foreach (var entry in _model.Constraints)
            {
                DrawConstraint(entry.Value, entry.Value.MasterColor, entry.Value.SlaveColor, symbolSize,
                               nodeSymbolSize, layer, true);
            }
        }
        public void DrawConstraint(Constraint constraint, Color masterColor, Color slaveColor, int symbolSize,
                                   int nodeSymbolSize, vtkRendererLayer layer, bool onlyVisible)
        {
            try
            {
                if (!((constraint.Active && constraint.Visible && constraint.Valid && !constraint.Internal) ||
                      layer == vtkRendererLayer.Selection)) return;
                //
                double[][] coor = null;
                string prefixName = "CONSTRAINT" + Globals.NameSeparator + constraint.Name;
                vtkRendererLayer symbolLayer = layer == vtkRendererLayer.Selection ? layer : vtkRendererLayer.Overlay;
                //
                int count = 0;
                if (constraint is PointSpring ps)
                {
                    // Node set
                    if (ps.RegionType == RegionTypeEnum.NodeSetName)
                    {
                        if (!_model.Mesh.NodeSets.ContainsKey(ps.RegionName)) return;
                        FeNodeSet nodeSet = _model.Mesh.NodeSets[ps.RegionName];
                        if (nodeSet.Labels.Length < 10)
                        {
                            coor = new double[nodeSet.Labels.Length][];
                            for (int i = 0; i < nodeSet.Labels.Length; i++) coor[i] = _model.Mesh.Nodes[nodeSet.Labels[i]].Coor;
                        }
                        else coor = new double[][] { nodeSet.CenterOfGravity };
                        //
                        count += DrawNodeSet(prefixName, ps.MasterRegionName, masterColor, layer, true, nodeSymbolSize,
                                             false, onlyVisible);
                    }
                    else if (ps.RegionType == RegionTypeEnum.ReferencePointName)
                    {
                        if (!_model.Mesh.ReferencePoints.ContainsKey(ps.RegionName)) return;
                        FeReferencePoint referencePoint = _model.Mesh.ReferencePoints[ps.RegionName];
                        coor = new double[1][];
                        coor[0] = referencePoint.Coor();
                        count++;
                    }
                    else throw new NotSupportedException();
                    // Symbol
                    if (count > 0) DrawSpringSymbols(prefixName, ps, coor, masterColor, symbolSize, symbolLayer);
                }
                else if (constraint is SurfaceSpring ss)
                {
                    // Surface
                    if (ss.RegionType == RegionTypeEnum.SurfaceName)
                    {
                        if (!_model.Mesh.Surfaces.ContainsKey(ss.RegionName)) return;
                        coor = new double[1][];
                        coor[0] = _model.Mesh.GetSurfaceCG(ss.RegionName);
                        //
                        count += DrawSurface(prefixName, ss.RegionName, masterColor, layer, true, false, onlyVisible);
                        if (layer == vtkRendererLayer.Selection)
                            DrawSurfaceEdge(prefixName, ss.RegionName, masterColor, layer, true, false, onlyVisible);
                    }
                    else throw new NotSupportedException();
                    // Symbol
                    if (count > 0) DrawSpringSymbols(prefixName, ss, coor, masterColor, symbolSize, symbolLayer);
                }
                else if (constraint is CompressionOnly co)
                {
                    if (!_model.Mesh.Surfaces.ContainsKey(co.MasterRegionName)) return;
                    //
                    count += DrawSurface(prefixName, co.MasterRegionName, masterColor, layer, true, false, onlyVisible);
                    if (layer == vtkRendererLayer.Selection)
                        DrawSurfaceEdge(prefixName, co.MasterRegionName, masterColor, layer, true, false, onlyVisible);
                    //
                    if (count > 0)
                    {
                        // 2D
                        //if (co.TwoD)
                        //    DrawShellEdgeLoadSymbols(prefixName, dLoad.SurfaceName, dLoad.Magnitude.Value,
                        //                             color, symbolSize, layer);
                        // 3D
                        //else 
                            DrawCompressionOnlySymbols(prefixName, co, masterColor, symbolSize, layer);
                    }
                }
                else if (constraint is RigidBody rb)
                {
                    // Master
                    // Only draw reference point during highlight
                    if (layer == vtkRendererLayer.Selection)
                    {
                        if (!_model.Mesh.ReferencePoints.ContainsKey(rb.ReferencePointName)) return;
                        else DrawReferencePoint(_model.Mesh.ReferencePoints[rb.ReferencePointName], masterColor, layer);
                    }
                    // Slave
                    if (rb.RegionType == RegionTypeEnum.NodeSetName)
                        count += DrawNodeSet(prefixName, rb.RegionName, masterColor, layer, true, nodeSymbolSize,
                                             true, onlyVisible);
                    else if (rb.RegionType == RegionTypeEnum.SurfaceName)
                    {
                        count += DrawSurface(prefixName, rb.RegionName, masterColor, layer, true, true, onlyVisible);
                        if (layer == vtkRendererLayer.Selection)
                            DrawSurfaceEdge(prefixName, rb.RegionName, masterColor, layer, true, true, onlyVisible);
                    }
                    else throw new NotSupportedException();
                    // Symbol
                    if (count > 0)
                    {
                        DrawRigidBodySymbol(rb, masterColor, symbolLayer, onlyVisible);
                    }
                }
                else if (constraint is Tie tie)
                {
                    // Master
                    count += DrawSurface(prefixName, tie.MasterRegionName, masterColor, layer, true, false, onlyVisible);
                    if (layer == vtkRendererLayer.Selection)
                        DrawSurfaceEdge(prefixName, tie.MasterRegionName, masterColor, layer, true, false, onlyVisible);
                    // Slave
                    count += DrawSurface(prefixName, tie.SlaveRegionName, slaveColor, layer, true, true, onlyVisible);
                    if (layer == vtkRendererLayer.Selection)
                        DrawSurfaceEdge(prefixName, tie.SlaveRegionName, slaveColor, layer, true, true, onlyVisible);
                }
                else throw new NotSupportedException();
            }
            catch { } // do not show the exception to the user
        }
        public void DrawRigidBodySymbol(RigidBody rigidBody, Color color, vtkRendererLayer layer,
                                        bool onlyVisible)
        {
            int[][] cells;
            int[] cellsTypes;
            double[][] nodeCoor;
            double[][] distributedNodeCoor;
            bool canHaveEdges = false;
            //
            if (!GetReferencePointNames().Contains(rigidBody.ReferencePointName)) return;
            // Node set
            string nodeSetName;
            if (rigidBody.RegionType == RegionTypeEnum.NodeSetName) nodeSetName = rigidBody.RegionName;
            else if (rigidBody.RegionType == RegionTypeEnum.SurfaceName) nodeSetName = _model.Mesh.Surfaces[rigidBody.RegionName].NodeSetName;
            else throw new NotSupportedException();

            if (nodeSetName !=null && _model.Mesh.NodeSets.ContainsKey(nodeSetName))
            {
                FeNodeSet nodeSet = _model.Mesh.NodeSets[nodeSetName];
                if (nodeSet.Labels.Length == 0) return;     // after remeshing this is 0 before the node set update
                // All nodes
                nodeCoor = _model.Mesh.GetNodeSetCoor(nodeSet.Labels, onlyVisible);
                // If all nodes are hidden
                if (nodeCoor == null || nodeCoor.Length == 0) return;
                // Ids go from 0 to Length
                int[] distributedIds = GetSpatiallyEquallyDistributedCoor(nodeCoor, 3);
                // Distributed nodes
                distributedNodeCoor = new double[distributedIds.Length][];
                for (int i = 0; i < distributedIds.Length; i++) distributedNodeCoor[i] = nodeCoor[distributedIds[i]];
                // Create wire elements
                // Distributed coor +1 for reference point
                nodeCoor = new double[distributedIds.Length + 1][];
                nodeCoor[0] = GetReferencePoint(rigidBody.ReferencePointName).Coor();
                for (int i = 0; i < distributedIds.Length; i++) nodeCoor[i + 1] = distributedNodeCoor[i];
                //
                cells = new int[distributedIds.Length][];
                cellsTypes = new int[distributedIds.Length];
                LinearBeamElement element = new LinearBeamElement(0, null);
                for (int i = 0; i < distributedIds.Length; i++)
                {
                    cells[i] = new int[] { 0, i + 1 };
                    cellsTypes[i] = element.GetVtkCellType();
                }
                //
                if (cells.Length > 0)
                {
                    vtkMaxActorData data = new vtkMaxActorData();
                    data.Name = rigidBody.Name + "_lines";
                    data.Color = color;
                    data.Layer = layer;
                    data.CanHaveElementEdges = canHaveEdges;
                    data.Pickable = false;
                    data.Geometry.Nodes.Ids = null;
                    data.Geometry.Nodes.Coor = nodeCoor.ToArray();
                    data.Geometry.Cells.CellNodeIds = cells;
                    data.Geometry.Cells.Types = cellsTypes;
                    ApplyLighting(data);
                    _form.Add3DCells(data);
                }
            }
        }
        public void DrawSpringSymbols(string prefixName, SpringConstraint spring, double[][] symbolCoor,
                                      Color color, int symbolSize, vtkRendererLayer layer)
        {
            // Spring
            List<double[]> allCoor = new List<double[]>();
            List<double[]> allNormals = new List<double[]>();
            HashSet<int> directions = new HashSet<int>(spring.GetSpringDirections());
            //
            if (directions.Contains(1))
            {
                double[] normalX = new double[] { 1, 0, 0 };
                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normalX);
                }
            }
            if (directions.Contains(2))
            {
                double[] normalY = new double[] { 0, 1, 0 };
                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normalY);
                }
            }
            if (directions.Contains(3))
            {
                double[] normalZ = new double[] { 0, 0, 1 };
                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normalZ);
                }
            }
            if (allCoor.Count > 0)
            {
                vtkMaxActorData data = new vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Geometry.Nodes.Coor = allCoor.ToArray();
                data.Geometry.Nodes.Normals = allNormals.ToArray();
                ApplyLighting(data);
                _form.AddOrientedSpringActor(data, symbolSize);
            }
        }
        public void DrawCompressionOnlySymbols(string prefixName, CompressionOnly compressionOnly, Color color, int symbolSize,
                                               vtkRendererLayer layer)
        {
            FeSurface surface = _model.Mesh.Surfaces[compressionOnly.MasterRegionName];
            //
            List<int> allElementIds = new List<int>();
            List<FeFaceName> allElementFaceNames = new List<FeFaceName>();
            List<double[]> allCoor = new List<double[]>();
            double[] faceCenter;
            FeElementSet elementSet;
            foreach (var entry in surface.ElementFaces)     // entry:  S3; elementSetName
            {
                elementSet = _model.Mesh.ElementSets[entry.Value];
                foreach (var elementId in elementSet.Labels)
                {
                    allElementIds.Add(elementId);
                    allElementFaceNames.Add(entry.Key);
                    _model.Mesh.GetElementFaceCenter(elementId, entry.Key, out faceCenter);
                    allCoor.Add(faceCenter);
                }
            }
            //
            int[] distributedElementIds = GetSpatiallyEquallyDistributedCoor(allCoor.ToArray(), 6);
            // Front shell face which is a S2 POS face works in the same way as a solid face
            // Back shell face which is a S1 NEG must be inverted
            int id;
            double[] faceNormal;
            bool shellEdgeFace;
            bool shellElement;
            double[][] distributedCoor = new double[distributedElementIds.Length][];
            double[][] distributedLoadNormals = new double[distributedElementIds.Length][];
            for (int i = 0; i < distributedElementIds.Length; i++)
            {
                id = distributedElementIds[i];
                _model.Mesh.GetElementFaceCenterAndNormal(allElementIds[id], allElementFaceNames[id], out faceCenter,
                                                          out faceNormal, out shellElement);
                // Invert normal
                shellEdgeFace = shellElement && allElementFaceNames[id] != FeFaceName.S1 &&
                                allElementFaceNames[id] != FeFaceName.S2;
                if (shellElement && ! shellEdgeFace)
                {
                    faceNormal[0] *= -1;
                    faceNormal[1] *= -1;
                    faceNormal[2] *= -1;
                }
                //
                distributedCoor[i] = faceCenter;
                distributedLoadNormals[i] = faceNormal;
            }
            // Cones
            if (allCoor.Count > 0)
            {
                vtkMaxActorData data = new vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Geometry.Nodes.Coor = distributedCoor.ToArray();
                data.Geometry.Nodes.Normals = distributedLoadNormals.ToArray();
                data.SectionViewPossible = false;
                ApplyLighting(data);
                bool translate = true;
                _form.AddOrientedDisplacementConstraintActor(data, symbolSize);
            }
        }
        // Contact pairs
        public void DrawAllContactPairs()
        {
            vtkRendererLayer layer = vtkRendererLayer.Base;
            //
            foreach (var entry in _model.ContactPairs)
            {
                DrawContactPair(entry.Value, entry.Value.MasterColor, entry.Value.SlaveColor, layer, true);
            }
        }
        public void DrawContactPair(ContactPair contactPair, Color masterColor, Color slaveColor, vtkRendererLayer layer,
                                    bool onlyVisible)
        {
            try
            {
                if (!((contactPair.Active && contactPair.Visible && contactPair.Valid && !contactPair.Internal) ||
                      layer == vtkRendererLayer.Selection)) return;
                //
                string prefixName = "CONTACT_PAIR" + Globals.NameSeparator + contactPair.Name;
                // Master
                DrawSurfaceWithEdge(prefixName, contactPair.MasterRegionName, masterColor, layer, true, false, onlyVisible);
                // Slave
                DrawSurfaceWithEdge(prefixName, contactPair.SlaveRegionName, slaveColor, layer, true, true, onlyVisible);
            }
            catch { } // do not show the exception to the user
        }

        // BCs
        public void DrawAllBoundaryConditions(string stepName)
        {
            int symbolSize = _settings.Pre.SymbolSize;
            int nodeSymbolSize = _settings.Pre.NodeSymbolSize;
            vtkRendererLayer layer = vtkRendererLayer.Base;
            //
            foreach (var step in _model.StepCollection.StepsList)
            {
                if (step.Name == stepName)
                {
                    foreach (var entry in step.BoundaryConditions)
                    {
                        DrawBoundaryCondition(step.Name, entry.Value, entry.Value.Color, symbolSize, nodeSymbolSize, layer, true);
                    }
                    break;
                }
            }
        }
        public void DrawBoundaryCondition(string stepName, BoundaryCondition boundaryCondition, Color color, 
                                          int symbolSize, int nodeSymbolSize, vtkRendererLayer layer, bool onlyVisible)
        {
            try
            {
                if (!((boundaryCondition.Active && boundaryCondition.Visible && boundaryCondition.Valid &&
                      !boundaryCondition.Internal) || layer == vtkRendererLayer.Selection)) return;
                //
                double[][] coor = null;
                string prefixName = stepName + Globals.NameSeparator + "BC" + Globals.NameSeparator + boundaryCondition.Name;
                vtkRendererLayer symbolLayer = vtkRendererLayer.Overlay;
                if (layer == vtkRendererLayer.Selection) symbolLayer = vtkRendererLayer.Selection;
                //
                int count = 0;
                if (boundaryCondition is DisplacementRotation || boundaryCondition is FixedBC)
                {
                    if (boundaryCondition.RegionType == RegionTypeEnum.NodeSetName)
                    {
                        if (!_model.Mesh.NodeSets.ContainsKey(boundaryCondition.RegionName)) return;
                        FeNodeSet nodeSet = _model.Mesh.NodeSets[boundaryCondition.RegionName];
                        coor = new double[1][];
                        coor[0] = nodeSet.CenterOfGravity;
                        //
                        count += DrawNodeSet(prefixName, nodeSet.Name, color, layer, true, nodeSymbolSize, false, onlyVisible);
                    }
                    else if (boundaryCondition.RegionType == RegionTypeEnum.SurfaceName)
                    {
                        if (!_model.Mesh.Surfaces.ContainsKey(boundaryCondition.RegionName)) return;
                        FeSurface surface = _model.Mesh.Surfaces[boundaryCondition.RegionName];
                        coor = new double[1][];
                        coor[0] = _model.Mesh.NodeSets[surface.NodeSetName].CenterOfGravity;
                        //
                        count += DrawSurface(prefixName, surface.Name, color, layer, true, false, onlyVisible);
                        if (layer == vtkRendererLayer.Selection)
                            DrawSurfaceEdge(prefixName, surface.Name, color, layer, true, false, onlyVisible);
                    }
                    else if (boundaryCondition.RegionType == RegionTypeEnum.ReferencePointName)
                    {
                        if (!_model.Mesh.ReferencePoints.ContainsKey(boundaryCondition.RegionName)) return;
                        FeReferencePoint referencePoint = _model.Mesh.ReferencePoints[boundaryCondition.RegionName];
                        coor = new double[1][];
                        coor[0] = referencePoint.Coor();
                        count++;
                    }
                    else throw new NotSupportedException();
                    if (count > 0)
                    {
                        if (boundaryCondition is FixedBC fix)
                            DrawFixedBCSymbols(prefixName, coor, color, symbolSize, symbolLayer, boundaryCondition.TwoD);
                        else if (boundaryCondition is DisplacementRotation dispRot)
                            DrawDisplacementRotationSymbols(prefixName, dispRot, coor, color, symbolSize, symbolLayer);
                    }
                }
                else if (boundaryCondition is SubmodelBC submodel)
                {
                    if (submodel.RegionType == RegionTypeEnum.NodeSetName)
                    {
                        if (!_model.Mesh.NodeSets.ContainsKey(submodel.RegionName)) return;
                        FeNodeSet nodeSet = _model.Mesh.NodeSets[submodel.RegionName];
                        coor = new double[1][];
                        coor[0] = nodeSet.CenterOfGravity;
                        //
                        count += DrawNodeSet(prefixName, nodeSet.Name, color, layer, true, nodeSymbolSize, false, onlyVisible);
                    }
                    else if (submodel.RegionType == RegionTypeEnum.SurfaceName)
                    {
                        if (!_model.Mesh.Surfaces.ContainsKey(submodel.RegionName)) return;
                        FeSurface surface = _model.Mesh.Surfaces[submodel.RegionName];
                        coor = new double[1][];
                        coor[0] = _model.Mesh.NodeSets[surface.NodeSetName].CenterOfGravity;
                        //
                        count += DrawSurface(prefixName, surface.Name, color, layer, true, false, onlyVisible);
                        if (layer == vtkRendererLayer.Selection)
                            DrawSurfaceEdge(prefixName, surface.Name, color, layer, true, false, onlyVisible);
                    }
                    else throw new NotSupportedException();
                    if (count > 0) DrawSubmodelBCSymbols(prefixName, submodel, coor, color, symbolSize, symbolLayer);
                }
                else if (boundaryCondition is TemperatureBC temperature)
                {
                    if (temperature.RegionType == RegionTypeEnum.NodeSetName)
                    {
                        if (!_model.Mesh.NodeSets.ContainsKey(temperature.RegionName)) return;
                        FeNodeSet nodeSet = _model.Mesh.NodeSets[temperature.RegionName];
                        coor = new double[1][];
                        coor[0] = nodeSet.CenterOfGravity;
                        //
                        count += DrawNodeSet(prefixName, nodeSet.Name, color, layer, false, nodeSymbolSize, false, onlyVisible);
                    }
                    else if (temperature.RegionType == RegionTypeEnum.SurfaceName)
                    {
                        if (!_model.Mesh.Surfaces.ContainsKey(temperature.RegionName)) return;
                        FeSurface surface = _model.Mesh.Surfaces[temperature.RegionName];
                        coor = new double[1][];
                        coor[0] = _model.Mesh.NodeSets[surface.NodeSetName].CenterOfGravity;
                        //
                        count += DrawSurface(prefixName, surface.Name, color, layer, true, false, onlyVisible);
                        if (layer == vtkRendererLayer.Selection)
                            DrawSurfaceEdge(prefixName, surface.Name, color, layer, true, false, onlyVisible);
                    }
                    else throw new NotSupportedException();
                    if (count > 0) DrawTemperatureBCSymbols(prefixName, temperature, coor, color, symbolSize, layer);
                }
            }
            catch { } // do not show the exception to the user
        }
        public void DrawFixedBCSymbols(string prefixName, double[][] symbolCoor, Color color,
                                       int symbolSize, vtkRendererLayer layer, bool twoD)
        {
            vtkMaxActorData data;
            List<double[]> allCoor = new List<double[]>();
            List<double[]> allNormals = new List<double[]>();
            //
            double[] normalX = new double[] { 1, 0, 0 };
            double[] normalY = new double[] { 0, 1, 0 };
            double[] normalZ = new double[] { 0, 0, 1 };
            // Cones
            for (int i = 0; i < symbolCoor.Length; i++)
            {
                allCoor.Add(symbolCoor[i]);
                allNormals.Add(normalX);
            }
            //
            for (int i = 0; i < symbolCoor.Length; i++)
            {
                allCoor.Add(symbolCoor[i]);
                allNormals.Add(normalY);
            }
            //
            if (!twoD)
            {
                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normalZ);
                }
            }
            //
            data = new vtkMaxActorData();
            data.Name = prefixName;
            data.Color = color;
            data.Layer = layer;
            data.Geometry.Nodes.Coor = allCoor.ToArray();
            data.Geometry.Nodes.Normals = allNormals.ToArray();
            ApplyLighting(data);
            _form.AddOrientedDisplacementConstraintActor(data, symbolSize);
            // Cylinders
            allCoor.Clear();
            allNormals.Clear();
            //
            if (!twoD)
            {
                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normalX);
                }
                //
                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normalY);
                }
            }
            //
            for (int i = 0; i < symbolCoor.Length; i++)
            {
                allCoor.Add(symbolCoor[i]);
                allNormals.Add(normalZ);
            }
            //
            data = new vtkMaxActorData();
            data.Name = prefixName;
            data.Color = color;
            data.Layer = layer;
            data.Geometry.Nodes.Coor = allCoor.ToArray();
            data.Geometry.Nodes.Normals = allNormals.ToArray();
            ApplyLighting(data);
            _form.AddOrientedRotationalConstraintActor(data, symbolSize);
        }
        public void DrawDisplacementRotationSymbols(string prefixName, DisplacementRotation dispRot, double[][] symbolCoor,
                                                    Color color, int symbolSize, vtkRendererLayer layer)
        {
            // Cones
            List<double[]> allCoor = new List<double[]>();
            List<double[]> allNormals = new List<double[]>();
            if (dispRot.GetDofType(1) == DOFType.Zero || dispRot.GetDofType(1) == DOFType.Fixed)
            {
                double[] normalX = new double[] { 1, 0, 0 };
                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normalX);
                }
            }
            if (dispRot.GetDofType(2) == DOFType.Zero || dispRot.GetDofType(2) == DOFType.Fixed)
            {
                double[] normalY = new double[] { 0, 1, 0 };
                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normalY);
                }
            }
            if (dispRot.GetDofType(3) == DOFType.Zero || dispRot.GetDofType(3) == DOFType.Fixed)
            {
                double[] normalZ = new double[] { 0, 0, 1 };
                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normalZ);
                }
            }
            if (allCoor.Count > 0)
            {
                vtkMaxActorData data = new vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Geometry.Nodes.Coor = allCoor.ToArray();
                data.Geometry.Nodes.Normals = allNormals.ToArray();
                ApplyLighting(data);
                _form.AddOrientedDisplacementConstraintActor(data, symbolSize);
            }
            // Cylinders
            allCoor.Clear();
            allNormals.Clear();
            if (dispRot.GetDofType(4) == DOFType.Zero || dispRot.GetDofType(4) == DOFType.Fixed)
            {
                double[] normalX = new double[] { 1, 0, 0 };
                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normalX);
                }
            }
            if (dispRot.GetDofType(5) == DOFType.Zero || dispRot.GetDofType(5) == DOFType.Fixed)
            {
                double[] normalY = new double[] { 0, 1, 0 };
                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normalY);
                }
            }
            if (dispRot.GetDofType(6) == DOFType.Zero || dispRot.GetDofType(6) == DOFType.Fixed)
            {
                double[] normalZ = new double[] { 0, 0, 1 };
                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normalZ);
                }
            }
            if (allCoor.Count > 0)
            {
                vtkMaxActorData data = new vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Geometry.Nodes.Coor = allCoor.ToArray();
                data.Geometry.Nodes.Normals = allNormals.ToArray();
                ApplyLighting(data);
                _form.AddOrientedRotationalConstraintActor(data, symbolSize);
            }
            //                                                                                                                      
            // Arrows
            allCoor.Clear();
            allNormals.Clear();
            //
            if (dispRot.GetDofType(1) == DOFType.Prescribed ||
                dispRot.GetDofType(2) == DOFType.Prescribed ||
                dispRot.GetDofType(3) == DOFType.Prescribed)
            {
                double[] normal = new double[3];
                if (dispRot.GetDofType(1) == DOFType.Prescribed) normal[0] = dispRot.U1.Value;
                if (dispRot.GetDofType(2) == DOFType.Prescribed) normal[1] = dispRot.U2.Value;
                if (dispRot.GetDofType(3) == DOFType.Prescribed) normal[2] = dispRot.U3.Value;
                //
                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normal);
                }
            }
            if (allCoor.Count > 0)
            {
                vtkMaxActorData data = new vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Geometry.Nodes.Coor = allCoor.ToArray();
                data.Geometry.Nodes.Normals = allNormals.ToArray();
                ApplyLighting(data);
                _form.AddOrientedArrowsActor(data, symbolSize);
            }
            // Double arrows
            allCoor.Clear();
            allNormals.Clear();
            if (dispRot.GetDofType(4) == DOFType.Prescribed ||
                dispRot.GetDofType(5) == DOFType.Prescribed ||
                dispRot.GetDofType(6) == DOFType.Prescribed)
            {
                double[] normal = new double[3];
                if (dispRot.GetDofType(4) == DOFType.Prescribed) normal[0] = dispRot.UR1.Value;
                if (dispRot.GetDofType(5) == DOFType.Prescribed) normal[1] = dispRot.UR2.Value;
                if (dispRot.GetDofType(6) == DOFType.Prescribed) normal[2] = dispRot.UR3.Value;
                //
                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normal);
                }
            }
            if (allCoor.Count > 0)
            {
                vtkMaxActorData data = new vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Geometry.Nodes.Coor = allCoor.ToArray();
                data.Geometry.Nodes.Normals = allNormals.ToArray();
                ApplyLighting(data);
                _form.AddOrientedDoubleArrowsActor(data, symbolSize);
            }
        }
        public void DrawSubmodelBCSymbols(string prefixName, SubmodelBC submodel, double[][] symbolCoor, Color color,
                                          int symbolSize, vtkRendererLayer layer)
        {
            // Cones
            List<double[]> allCoor = new List<double[]>();
            List<double[]> allNormals = new List<double[]>();
            if (submodel.U1)
            {
                double[] normalX = new double[] { 1, 0, 0 };
                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normalX);
                }
            }
            if (submodel.U2)
            {
                double[] normalY = new double[] { 0, 1, 0 };
                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normalY);
                }
            }
            if (submodel.U3)
            {
                double[] normalZ = new double[] { 0, 0, 1 };
                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normalZ);
                }
            }
            if (allCoor.Count > 0)
            {
                vtkMaxActorData data = new vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Geometry.Nodes.Coor = allCoor.ToArray();
                data.Geometry.Nodes.Normals = allNormals.ToArray();
                ApplyLighting(data);
                _form.AddOrientedDisplacementConstraintActor(data, symbolSize);
            }
            // Cylinders
            allCoor.Clear();
            allNormals.Clear();
            if (submodel.UR1)
            {
                double[] normalX = new double[] { 1, 0, 0 };
                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normalX);
                }
            }
            if (submodel.UR2)
            {
                double[] normalY = new double[] { 0, 1, 0 };
                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normalY);
                }
            }
            if (submodel.UR3)
            {
                double[] normalZ = new double[] { 0, 0, 1 };
                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normalZ);
                }
            }
            if (allCoor.Count > 0)
            {
                vtkMaxActorData data = new vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Geometry.Nodes.Coor = allCoor.ToArray();
                data.Geometry.Nodes.Normals = allNormals.ToArray();
                ApplyLighting(data);
                _form.AddOrientedRotationalConstraintActor(data, symbolSize);
            }
        }
        public void DrawTemperatureBCSymbols(string prefixName, TemperatureBC temperature, double[][] symbolCoor, Color color,
                                             int symbolSize, vtkRendererLayer layer)
        {
            FeSurface surface;
            if (temperature.RegionType == RegionTypeEnum.NodeSetName)
            {
                string name = Model.Mesh.Surfaces.GetNextNumberedKey("Thermo");
                surface = new FeSurface(name, temperature.RegionName);
                surface.Internal = true;
                _model.Mesh.CreateSurfaceItems(surface);
                _model.Mesh.Surfaces.Add(surface.Name, surface);    // Must add here for the remove to work properly 
                //
                if (surface.ElementFaces == null) // after meshing/update the node set is not yet updated
                {
                    RemoveSurfaceAndElementFacesFromModel(new string[] { surface.Name });
                    return;
                }
            }
            else if (temperature.RegionType == RegionTypeEnum.SurfaceName)
            {
                surface = _model.Mesh.Surfaces[temperature.RegionName];
            }
            else throw new NotSupportedException();
            //
            List<int> allElementIds = new List<int>();
            List<FeFaceName> allElementFaceNames = new List<FeFaceName>();
            List<double[]> allCoor = new List<double[]>();
            double[] faceCenter;
            FeElementSet elementSet;
            foreach (var entry in surface.ElementFaces)     // entry:  S3; elementSetName
            {
                elementSet = _model.Mesh.ElementSets[entry.Value];
                foreach (var elementId in elementSet.Labels)
                {
                    allElementIds.Add(elementId);
                    allElementFaceNames.Add(entry.Key);
                    _model.Mesh.GetElementFaceCenter(elementId, entry.Key, out faceCenter);
                    allCoor.Add(faceCenter);
                }
            }
            // Remove created surface
            if (temperature.RegionType == RegionTypeEnum.NodeSetName)
            {
                RemoveSurfaceAndElementFacesFromModel(new string[] { surface.Name });
            }
            //
            int[] distributedElementIds = GetSpatiallyEquallyDistributedCoor(allCoor.ToArray(), 3);
            // Front shell face which is a S2 POS face works in the same way as a solid face
            // Back shell face which is a S1 NEG must be inverted
            int id;
            double[] faceNormal;
            bool shellElement;
            bool shellEdge;
            double[][] distributedCoor = new double[distributedElementIds.Length][];
            double[][] distributedLoadNormals = new double[distributedElementIds.Length][];
            for (int i = 0; i < distributedElementIds.Length; i++)
            {
                id = distributedElementIds[i];
                _model.Mesh.GetElementFaceCenterAndNormal(allElementIds[id], allElementFaceNames[id], out faceCenter,
                                                          out faceNormal, out shellElement);
                //
                shellEdge = shellElement && allElementFaceNames[id] != FeFaceName.S1 && allElementFaceNames[id] != FeFaceName.S2;
                if (!shellElement || shellEdge)
                {
                    faceNormal[0] *= -1;
                    faceNormal[1] *= -1;
                    faceNormal[2] *= -1;
                }
                //
                distributedCoor[i] = faceCenter;
                distributedLoadNormals[i] = faceNormal;
            }
            // Thermos
            if (allCoor.Count > 0)
            {
                vtkMaxActorData data = new vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Geometry.Nodes.Coor = distributedCoor.ToArray();
                data.Geometry.Nodes.Normals = distributedLoadNormals.ToArray();
                data.SectionViewPossible = false;
                ApplyLighting(data);
                bool translate = false;
                _form.AddOrientedThermosActor(data, symbolSize, translate);
            }
            return;
        }
        // Loads
        private void DrawAllLoads(string stepName)
        {
            int symbolSize = _settings.Pre.SymbolSize;
            int nodeSymbolSize = _settings.Pre.NodeSymbolSize;
            vtkRendererLayer layer = vtkRendererLayer.Base;
            //
            foreach (var step in _model.StepCollection.StepsList)
            {
                if (step.Name == stepName)
                {
                    foreach (var entry in step.Loads)
                    {
                        DrawLoad(step.Name, entry.Value, entry.Value.Color, symbolSize, nodeSymbolSize, layer, true);
                    }
                    break;
                }
            }
        }
        public void DrawLoad(string stepName, Load load, Color color, int symbolSize, int nodeSymbolSize,
                             vtkRendererLayer layer, bool onlyVisible)
        {
            try
            {
                if (!((load.Active && load.Visible && load.Valid && !load.Internal) || layer == vtkRendererLayer.Selection))
                    return;
                //
                double[][] coor = null;
                string prefixName = stepName + Globals.NameSeparator + "LOAD" + Globals.NameSeparator + load.Name;
                vtkRendererLayer symbolLayer = layer == vtkRendererLayer.Selection ? layer : vtkRendererLayer.Overlay;
                //
                int count = 0;
                if (load is CLoad cLoad)
                {
                    if (cLoad.RegionType == RegionTypeEnum.NodeSetName)
                    {
                        if (!_model.Mesh.NodeSets.ContainsKey(cLoad.RegionName)) return;
                        FeNodeSet nodeSet = _model.Mesh.NodeSets[cLoad.RegionName];
                        if (nodeSet.Labels.Length < 10)
                        { 
                            coor = new double[nodeSet.Labels.Length][];
                            for (int i = 0; i < nodeSet.Labels.Length; i++) coor[i] = _model.Mesh.Nodes[nodeSet.Labels[i]].Coor;
                        }
                        else coor = new double[][] { nodeSet.CenterOfGravity };
                        //
                        count += DrawNodeSet(prefixName, nodeSet.Name, color, layer, true, nodeSymbolSize, false, onlyVisible);
                    }
                    else if (cLoad.RegionType == RegionTypeEnum.ReferencePointName)
                    {
                        if (!_model.Mesh.ReferencePoints.ContainsKey(cLoad.RegionName)) return;
                        FeReferencePoint referencePoint = _model.Mesh.ReferencePoints[cLoad.RegionName];
                        coor = new double[1][];
                        coor[0] = referencePoint.Coor();
                        count++;
                    }
                    else throw new NotSupportedException();
                    if (count > 0) DrawCLoadSymbols(prefixName, cLoad, coor, color, symbolSize, symbolLayer);
                }
                else if (load is MomentLoad momentLoad)
                {
                    if (momentLoad.RegionType == RegionTypeEnum.NodeSetName)
                    {
                        if (!_model.Mesh.NodeSets.ContainsKey(momentLoad.RegionName)) return;
                        FeNodeSet nodeSet = _model.Mesh.NodeSets[momentLoad.RegionName];
                        coor = new double[nodeSet.Labels.Length][];
                        for (int i = 0; i < nodeSet.Labels.Length; i++) coor[i] = _model.Mesh.Nodes[nodeSet.Labels[i]].Coor;
                        //
                        count += DrawNodeSet(prefixName, nodeSet.Name, color, layer, true, nodeSymbolSize, false, onlyVisible);
                    }
                    else if (momentLoad.RegionType == RegionTypeEnum.ReferencePointName)
                    {
                        if (!_model.Mesh.ReferencePoints.ContainsKey(momentLoad.RegionName)) return;
                        FeReferencePoint referencePoint = _model.Mesh.ReferencePoints[momentLoad.RegionName];
                        coor = new double[1][];
                        coor[0] = referencePoint.Coor();
                        count++;
                    }
                    else throw new NotSupportedException();
                    if (count > 0) DrawMomentLoadSymbols(prefixName, momentLoad, coor, color, symbolSize, symbolLayer);
                }
                else if (load is DLoad dLoad)
                {
                    if (!_model.Mesh.Surfaces.ContainsKey(dLoad.SurfaceName)) return;
                    //
                    count += DrawSurface(prefixName, dLoad.SurfaceName, color, layer, true, false, onlyVisible);
                    if (layer == vtkRendererLayer.Selection)
                        DrawSurfaceEdge(prefixName, dLoad.SurfaceName, color, layer, true, false, onlyVisible);
                    //
                    if (count > 0)
                    {
                        // 2D
                        if (dLoad.TwoD)
                            DrawShellEdgeLoadSymbols(prefixName, dLoad.SurfaceName, dLoad.Magnitude.Value,
                                                     color, symbolSize, layer);
                        // 3D
                        else DrawDLoadSymbols(prefixName, dLoad, color, symbolSize, layer);
                    }
                }
                else if (load is HydrostaticPressure hpLoad)
                {
                    if (!_model.Mesh.Surfaces.ContainsKey(hpLoad.SurfaceName)) return;
                    //
                    count += DrawSurface(prefixName, hpLoad.SurfaceName, color, layer, true, false, onlyVisible);
                    if (layer == vtkRendererLayer.Selection)
                        DrawSurfaceEdge(prefixName, hpLoad.SurfaceName, color, layer, true, false, onlyVisible);
                    //
                    if (count > 0)
                    {
                        // 2D and 3D
                        DrawHydrostaticPressureLoadSymbols(prefixName, hpLoad, color, symbolSize, layer);
                    }
                }
                else if (load is ImportedPressure ipLoad)
                {
                    if (!_model.Mesh.Surfaces.ContainsKey(ipLoad.SurfaceName)) return;
                    //
                    count += DrawSurface(prefixName, ipLoad.SurfaceName, color, layer, true, false, onlyVisible);
                    if (layer == vtkRendererLayer.Selection)
                        DrawSurfaceEdge(prefixName, ipLoad.SurfaceName, color, layer, true, false, onlyVisible);
                    //
                    if (count > 0)
                    {
                        // 2D
                        if (ipLoad.TwoD)
                            DrawShellEdgeLoadSymbols(prefixName, ipLoad.SurfaceName, ipLoad.MagnitudeFactor.Value,
                                                     color, symbolSize, layer);
                        // 3D
                        else DrawImportedPressureLoadSymbols(prefixName, ipLoad, color, symbolSize, layer);
                    }
                }
                else if (load is STLoad stLoad)
                {
                    if (!_model.Mesh.Surfaces.ContainsKey(stLoad.SurfaceName)) return;
                    coor = new double[1][];
                    coor[0] = _model.Mesh.GetSurfaceCG(stLoad.SurfaceName);
                    //
                    count += DrawSurface(prefixName, stLoad.SurfaceName, color, layer, true, false, onlyVisible);
                    if (layer == vtkRendererLayer.Selection)
                        DrawSurfaceEdge(prefixName, stLoad.SurfaceName, color, layer, true, false, onlyVisible);
                    //
                    if (count > 0) DrawSTLoadSymbols(prefixName, stLoad, coor, color, symbolSize, symbolLayer);
                }
                else if (load is ShellEdgeLoad shellEdgeLoad)
                {
                    if (!_model.Mesh.Surfaces.ContainsKey(shellEdgeLoad.SurfaceName)) return;
                    //
                    count += DrawSurface(prefixName, shellEdgeLoad.SurfaceName, color, layer, true, false, onlyVisible);
                    //
                    if (count > 0) DrawShellEdgeLoadSymbols(prefixName, shellEdgeLoad.SurfaceName, shellEdgeLoad.Magnitude.Value,
                                                            color, symbolSize, layer);
                }
                else if (load is GravityLoad gLoad)
                {
                    string[] partNames = null;
                    FeElementSet elementSet = null;
                    if (layer == vtkRendererLayer.Selection)
                    {
                        if (gLoad.RegionType == RegionTypeEnum.PartName)
                            count += HighlightModelParts(new string[] { gLoad.RegionName });
                        else if (gLoad.RegionType == RegionTypeEnum.ElementSetName)
                        {
                            if (!_model.Mesh.ElementSets.ContainsKey(gLoad.RegionName)) return;
                            elementSet = _model.Mesh.ElementSets[gLoad.RegionName];
                            count += HighlightElementSets(new string[] { elementSet.Name });
                        }
                        else throw new NotSupportedException();
                    }
                    FeNodeSet nodeSet = null;
                    if (gLoad.RegionType == RegionTypeEnum.ElementSetName)
                    {
                        if (!_model.Mesh.ElementSets.ContainsKey(gLoad.RegionName)) return;
                        elementSet = _model.Mesh.ElementSets[gLoad.RegionName];
                        if (elementSet != null && elementSet.CreatedFromParts)
                        {
                            partNames = _model.Mesh.GetPartNamesFromPartIds(elementSet.Labels);
                            if (partNames != null) nodeSet = _model.Mesh.GetNodeSetFromPartNames(partNames, onlyVisible);
                            else throw new NotSupportedException();
                        }
                    }
                    if (nodeSet == null) nodeSet = _model.Mesh.GetNodeSetFromPartOrElementSetName(gLoad.RegionName, false);
                    //
                    if (nodeSet.Labels.Length > 0)
                        DrawGravityLoadSymbol(prefixName, gLoad, nodeSet.CenterOfGravity, color, symbolSize, symbolLayer);
                }
                else if (load is CentrifLoad cfLoad)
                {
                    FeElementSet elementSet;
                    if (layer == vtkRendererLayer.Selection)
                    {
                        if (cfLoad.RegionType == RegionTypeEnum.PartName)
                            count +=  HighlightModelParts(new string[] { cfLoad.RegionName });
                        else if (cfLoad.RegionType == RegionTypeEnum.ElementSetName)
                        {
                            if (!_model.Mesh.ElementSets.ContainsKey(cfLoad.RegionName)) return;
                            elementSet = _model.Mesh.ElementSets[cfLoad.RegionName];
                            count += HighlightElementSets(new string[] { elementSet.Name });
                        }
                        else throw new NotSupportedException();
                    }
                    else
                    {
                        if (cfLoad.RegionType == RegionTypeEnum.PartName && _model.Mesh.Parts[cfLoad.RegionName].Visible) count++;
                        else if (cfLoad.RegionType == RegionTypeEnum.ElementSetName)
                        {
                            if (!_model.Mesh.ElementSets.ContainsKey(cfLoad.RegionName)) return;
                            elementSet = _model.Mesh.ElementSets[cfLoad.RegionName];
                            if (elementSet.CreatedFromParts)
                            {
                                string[] partNames = _model.Mesh.GetPartNamesFromPartIds(elementSet.Labels);
                                foreach (var partName in partNames) if (_model.Mesh.Parts[partName].Visible) { count++; break; }
                            }
                            else
                            {
                                Dictionary<int, bool> partVisibilities = new Dictionary<int, bool>();
                                foreach (var part in _model.Mesh.Parts) partVisibilities.Add(part.Value.PartId, part.Value.Visible);
                                FeElement element;
                                foreach (var elementId in elementSet.Labels)
                                {
                                    element = _model.Mesh.Elements[elementId];
                                    if (partVisibilities[element.PartId]) { count++; break; }
                                }
                            }
                        }
                    }
                    if (count > 0) DrawCentrifLoadSymbol(prefixName, cfLoad, color, symbolSize, symbolLayer);
                }
                else if (load is PreTensionLoad ptLoad)
                {
                    if (!_model.Mesh.Surfaces.ContainsKey(ptLoad.SurfaceName)) return;
                    coor = new double[2][];
                    coor[0] = _model.Mesh.GetSurfaceCG(ptLoad.SurfaceName);
                    coor[1] = coor[0];
                    //
                    count += DrawSurface(prefixName, ptLoad.SurfaceName, color, layer, true, false, onlyVisible);
                    if (layer == vtkRendererLayer.Selection)
                        DrawSurfaceEdge(prefixName, ptLoad.SurfaceName, color, layer, true, false, onlyVisible);
                    //
                    if (count > 0) DrawPreTensionLoadSymbols(prefixName, ptLoad, coor, color, symbolSize, symbolLayer);
                }
                // Thermal
                else if (load is CFlux cFlux)
                {
                    FeNodeSet nodeSet;
                    if (cFlux.RegionType == RegionTypeEnum.NodeSetName)
                    {
                        if (!_model.Mesh.NodeSets.ContainsKey(cFlux.RegionName)) return;
                        nodeSet = _model.Mesh.NodeSets[cFlux.RegionName];
                        coor = new double[nodeSet.Labels.Length][];
                        for (int i = 0; i < nodeSet.Labels.Length; i++) coor[i] = _model.Mesh.Nodes[nodeSet.Labels[i]].Coor;
                        //
                        count += DrawNodeSet(prefixName, nodeSet.Name, color, layer, true, nodeSymbolSize, false, onlyVisible);
                    }
                    else throw new NotSupportedException();
                    if (count > 0) DrawCFluxSymbols(prefixName, cFlux, coor, color, symbolSize, symbolLayer);
                }
                else if (load is DFlux dFlux)
                {
                    if (!_model.Mesh.Surfaces.ContainsKey(dFlux.SurfaceName)) return;
                    //
                    count += DrawSurface(prefixName, dFlux.SurfaceName, color, layer, true, false, onlyVisible);
                    if (layer == vtkRendererLayer.Selection)
                        DrawSurfaceEdge(prefixName, dFlux.SurfaceName, color, layer, true, false, onlyVisible);
                    if (count > 0) DrawDFluxSymbols(prefixName, dFlux, color, symbolSize, layer);
                }
                else if (load is BodyFlux bFlux)
                {
                    string[] partNames = null;
                    FeElementSet elementSet = null;
                    if (layer == vtkRendererLayer.Selection)
                    {
                        if (bFlux.RegionType == RegionTypeEnum.PartName)
                            count += HighlightModelParts(new string[] { bFlux.RegionName });
                        else if (bFlux.RegionType == RegionTypeEnum.ElementSetName)
                        {
                            if (!_model.Mesh.ElementSets.ContainsKey(bFlux.RegionName)) return;
                            elementSet = _model.Mesh.ElementSets[bFlux.RegionName];
                            count += HighlightElementSets(new string[] { elementSet.Name });
                        }
                        else throw new NotSupportedException();
                    }
                    FeNodeSet nodeSet = null;
                    if (bFlux.RegionType == RegionTypeEnum.ElementSetName)
                    {
                        if (!_model.Mesh.ElementSets.ContainsKey(bFlux.RegionName)) return;
                        elementSet = _model.Mesh.ElementSets[bFlux.RegionName];
                        if (elementSet != null && elementSet.CreatedFromParts)
                        {
                            partNames = _model.Mesh.GetPartNamesFromPartIds(elementSet.Labels);
                            if (partNames != null) nodeSet = _model.Mesh.GetNodeSetFromPartNames(partNames, onlyVisible);
                            else throw new NotSupportedException();
                        }
                    }
                    if (nodeSet == null) nodeSet = _model.Mesh.GetNodeSetFromPartOrElementSetName(bFlux.RegionName, false);
                    //
                    if (nodeSet.Labels.Length > 0)
                        DrawBodyFluxSymbol(prefixName, bFlux, nodeSet.CenterOfGravity, color, symbolSize, symbolLayer);
                }
                else if (load is FilmHeatTransfer filmHeatTransfer)
                {
                    if (!_model.Mesh.Surfaces.ContainsKey(filmHeatTransfer.SurfaceName)) return;
                    //
                    count += DrawSurface(prefixName, filmHeatTransfer.SurfaceName, color, layer, true, false, onlyVisible);
                    if (layer == vtkRendererLayer.Selection)
                        DrawSurfaceEdge(prefixName, filmHeatTransfer.SurfaceName, color, layer, true, false, onlyVisible);
                    if (count > 0) DrawFilmSymbols(prefixName, filmHeatTransfer, color, symbolSize, layer);
                }
                else if (load is RadiationHeatTransfer radiationHeatTransfer)
                {
                    if (!_model.Mesh.Surfaces.ContainsKey(radiationHeatTransfer.SurfaceName)) return;
                    //
                    count += DrawSurface(prefixName, radiationHeatTransfer.SurfaceName, color, layer, true, false, onlyVisible);
                    if (layer == vtkRendererLayer.Selection)
                        DrawSurfaceEdge(prefixName, radiationHeatTransfer.SurfaceName, color, layer, true, false, onlyVisible);
                    if (count > 0) DrawRadiateSymbols(prefixName, radiationHeatTransfer, color, symbolSize, layer);
                }
                else throw new NotSupportedException();
            }
            catch { }
        }
        public void DrawCLoadSymbols(string prefixName, CLoad cLoad, double[][] symbolCoor, Color color,
                                    int symbolSize, vtkRendererLayer layer)
        {
            // Arrows
            List<double[]> allLoadNormals = new List<double[]>();
            double[] normal = new double[] { cLoad.F1.Value, cLoad.F2.Value, cLoad.F3.Value };
            for (int i = 0; i < symbolCoor.GetLength(0); i++)
            {
                allLoadNormals.Add(normal);
            }
            //
            if (symbolCoor.GetLength(0) > 0)
            {
                vtkMaxActorData data = new vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Geometry.Nodes.Coor = symbolCoor.ToArray();
                data.Geometry.Nodes.Normals = allLoadNormals.ToArray();
                ApplyLighting(data);
                _form.AddOrientedArrowsActor(data, symbolSize);
            }
        }
        public void DrawMomentLoadSymbols(string prefixName, MomentLoad momentLoad, double[][] symbolCoor, 
                                          Color color, int symbolSize, vtkRendererLayer layer)
        {
            // Arrows
            List<double[]> allLoadNormals = new List<double[]>();
            double[] normal = new double[] { momentLoad.M1.Value, momentLoad.M2.Value, momentLoad.M3.Value };
            for (int i = 0; i < symbolCoor.GetLength(0); i++)
            {
                allLoadNormals.Add(normal);
            }
            //
            if (symbolCoor.GetLength(0) > 0)
            {
                vtkMaxActorData data = new vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Geometry.Nodes.Coor = symbolCoor.ToArray();
                data.Geometry.Nodes.Normals = allLoadNormals.ToArray();
                ApplyLighting(data);
                _form.AddOrientedDoubleArrowsActor(data, symbolSize);
            }
        }
        public void DrawDLoadSymbols(string prefixName, DLoad dLoad, Color color, int symbolSize,
                                     vtkRendererLayer layer)
        {
            FeSurface surface = _model.Mesh.Surfaces[dLoad.SurfaceName];
            //
            List<int> allElementIds = new List<int>();
            List<FeFaceName> allElementFaceNames = new List<FeFaceName>();
            List<double[]> allCoor = new List<double[]>();
            double[] faceCenter;
            FeElementSet elementSet;
            foreach (var entry in surface.ElementFaces)     // entry:  S3; elementSetName
            {
                elementSet = _model.Mesh.ElementSets[entry.Value];
                foreach (var elementId in elementSet.Labels)
                {
                    allElementIds.Add(elementId);
                    allElementFaceNames.Add(entry.Key);
                    _model.Mesh.GetElementFaceCenter(elementId, entry.Key, out faceCenter);
                    allCoor.Add(faceCenter);
                }
            }
            //
            int[] distributedElementIds = GetSpatiallyEquallyDistributedCoor(allCoor.ToArray(), 6);
            // Front shell face which is a S2 POS face works in the same way as a solid face
            // Back shell face which is a S1 NEG must be inverted
            int id;
            double[] faceNormal;
            bool shellElement;
            double[][] distributedCoor = new double[distributedElementIds.Length][];
            double[][] distributedLoadNormals = new double[distributedElementIds.Length][];
            for (int i = 0; i < distributedElementIds.Length; i++)
            {
                id = distributedElementIds[i];
                _model.Mesh.GetElementFaceCenterAndNormal(allElementIds[id], allElementFaceNames[id], out faceCenter,
                                                          out faceNormal, out shellElement);
                //
                if ((dLoad.Magnitude.Value < 0) != shellElement) // if both are equal no need to reverse the direction
                {
                    faceNormal[0] *= -1;
                    faceNormal[1] *= -1;
                    faceNormal[2] *= -1;
                }
                //
                distributedCoor[i] = faceCenter;
                distributedLoadNormals[i] = faceNormal;
            }
            // Arrows
            if (allCoor.Count > 0)
            {
                vtkMaxActorData data = new vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Geometry.Nodes.Coor = distributedCoor.ToArray();
                data.Geometry.Nodes.Normals = distributedLoadNormals.ToArray();
                data.SectionViewPossible = false;
                ApplyLighting(data);
                bool translate = dLoad.Magnitude.Value > 0;
                _form.AddOrientedArrowsActor(data, symbolSize, translate);
            }
        }
        public void DrawHydrostaticPressureLoadSymbols(string prefixName, HydrostaticPressure hpLoad, Color color, int symbolSize,
                                                       vtkRendererLayer layer)
        {
            FeSurface surface = _model.Mesh.Surfaces[hpLoad.SurfaceName];
            //
            List<int> allElementIds = new List<int>();
            List<FeFaceName> allElementFaceNames = new List<FeFaceName>();
            List<double[]> allCoor = new List<double[]>();
            double[] faceCenter;
            FeElementSet elementSet;
            foreach (var entry in surface.ElementFaces)     // entry:  S3; elementSetName
            {
                elementSet = _model.Mesh.ElementSets[entry.Value];
                foreach (var elementId in elementSet.Labels)
                {
                    allElementIds.Add(elementId);
                    allElementFaceNames.Add(entry.Key);
                    _model.Mesh.GetElementFaceCenter(elementId, entry.Key, out faceCenter);
                    allCoor.Add(faceCenter);
                }
            }
            //
            int[] distributedElementIds = GetSpatiallyEquallyDistributedCoor(allCoor.ToArray(), 6);
            // Front shell face which is a S2 POS face works in the same way as a solid face
            // Back shell face which is a S1 NEG must be inverted
            int id;
            bool shellElement;
            double[] faceNormal;
            double maxPressure = 0;
            double[] pressures = new double[distributedElementIds.Length];
            double[][] distributedCoor = new double[distributedElementIds.Length][];
            double[][] distributedLoadNormals = new double[distributedElementIds.Length][];
            for (int i = 0; i < distributedElementIds.Length; i++)
            {
                id = distributedElementIds[i];
                _model.Mesh.GetElementFaceCenterAndNormal(allElementIds[id], allElementFaceNames[id], out faceCenter,
                                                          out faceNormal, out shellElement);
                // Pressure
                pressures[i] = hpLoad.GetPressureForPoint(faceCenter);
                if (Math.Abs(pressures[i]) > maxPressure) maxPressure = Math.Abs(pressures[i]);
                //
                if ((hpLoad.TwoD && pressures[i] < 0) ||    // only 2d edges can be selected, 3d edges cannot be selected
                    (!hpLoad.TwoD && (pressures[i] < 0) != shellElement))   // if both are equal no need to reverse the direction
                {
                    faceNormal[0] *= -1;
                    faceNormal[1] *= -1;
                    faceNormal[2] *= -1;
                }
                //
                distributedCoor[i] = faceCenter;
                distributedLoadNormals[i] = faceNormal;
            }
            // Arrows
            vtkMaxActorData data;
            for (int i = 0; i < distributedElementIds.Length; i++)
            {
                data = new vtkMaxActorData();
                data.Name = prefixName + "_" + i.ToString();
                data.Color = color;
                data.Layer = layer;
                data.Geometry.Nodes.Coor = new double[][] { distributedCoor[i] };
                data.Geometry.Nodes.Normals = new double[][] { distributedLoadNormals[i] };
                data.SectionViewPossible = false;
                ApplyLighting(data);
                bool translate = pressures[i] > 0;
                _form.AddOrientedArrowsActor(data, symbolSize, translate, Math.Abs(pressures[i]) / maxPressure);
            }
        }
        public void DrawImportedPressureLoadSymbols(string prefixName, ImportedPressure ipLoad, Color color, int symbolSize,
                                                    vtkRendererLayer layer)
        {
            if (!ipLoad.IsInitialized()) ipLoad.ImportPressure(_model.UnitSystem.UnitSystemType);
            //
            FeSurface surface = _model.Mesh.Surfaces[ipLoad.SurfaceName];
            //
            List<int> allElementIds = new List<int>();
            List<FeFaceName> allElementFaceNames = new List<FeFaceName>();
            List<double[]> allCoor = new List<double[]>();
            double[] faceCenter;
            FeElementSet elementSet;
            foreach (var entry in surface.ElementFaces)     // entry:  S3; elementSetName
            {
                elementSet = _model.Mesh.ElementSets[entry.Value];
                foreach (var elementId in elementSet.Labels)
                {
                    allElementIds.Add(elementId);
                    allElementFaceNames.Add(entry.Key);
                    _model.Mesh.GetElementFaceCenter(elementId, entry.Key, out faceCenter);
                    allCoor.Add(faceCenter);
                }
            }
            //
            int[] distributedElementIds = GetSpatiallyEquallyDistributedCoor(allCoor.ToArray(), 6);
            // Front shell face which is a S2 POS face works in the same way as a solid face
            // Back shell face which is a S1 NEG must be inverted
            int id;
            bool shellElement;
            double[] faceNormal;
            double maxPressure = 0;
            double[] pressures = new double[distributedElementIds.Length];
            double[][] distributedCoor = new double[distributedElementIds.Length][];
            double[][] distributedLoadNormals = new double[distributedElementIds.Length][];
            for (int i = 0; i < distributedElementIds.Length; i++)
            {
                id = distributedElementIds[i];
                _model.Mesh.GetElementFaceCenterAndNormal(allElementIds[id], allElementFaceNames[id], out faceCenter,
                                                          out faceNormal, out shellElement);
                // Pressure
                pressures[i] = ipLoad.GetPressureForPoint(faceCenter);
                if (Math.Abs(pressures[i]) > maxPressure) maxPressure = Math.Abs(pressures[i]);
                //
                if ((ipLoad.TwoD && pressures[i] < 0) ||    // only 2d edges can be selected, 3d edges cannot be selected
                    (!ipLoad.TwoD && (pressures[i] < 0) != shellElement))   // if both are equal no need to reverse the direction
                {
                    faceNormal[0] *= -1;
                    faceNormal[1] *= -1;
                    faceNormal[2] *= -1;
                }
                //
                distributedCoor[i] = faceCenter;
                distributedLoadNormals[i] = faceNormal;
            }
            // Arrows
            vtkMaxActorData data;
            for (int i = 0; i < distributedElementIds.Length; i++)
            {
                data = new vtkMaxActorData();
                data.Name = prefixName + "_" + i.ToString();
                data.Color = color;
                data.Layer = layer;
                data.Geometry.Nodes.Coor = new double[][] { distributedCoor[i] };
                data.Geometry.Nodes.Normals = new double[][] { distributedLoadNormals[i] };
                data.SectionViewPossible = false;
                ApplyLighting(data);
                bool translate = pressures[i] > 0;
                _form.AddOrientedArrowsActor(data, symbolSize, translate, Math.Abs(pressures[i]) / maxPressure);
            }
        }
        public void DrawSTLoadSymbols(string prefixName, STLoad stLoad, double[][] symbolCoor, Color color,
                                      int symbolSize, vtkRendererLayer layer)
        {
            // Arrows
            List<double[]> allLoadNormals = new List<double[]>();
            double[] normal = new double[] { stLoad.F1.Value, stLoad.F2.Value, stLoad.F3.Value };
            for (int i = 0; i < symbolCoor.GetLength(0); i++)
            {
                allLoadNormals.Add(normal);
            }
            //
            if (symbolCoor.GetLength(0) > 0)
            {
                vtkMaxActorData data = new vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Geometry.Nodes.Coor = symbolCoor.ToArray();
                data.Geometry.Nodes.Normals = allLoadNormals.ToArray();
                ApplyLighting(data);
                _form.AddOrientedArrowsActor(data, symbolSize);
            }
        }
        public void DrawShellEdgeLoadSymbols(string prefixName, string surfaceName, double magnitude, Color color,
                                             int symbolSize, vtkRendererLayer layer)
        {
            FeSurface surface = _model.Mesh.Surfaces[surfaceName];
            //
            List<int> allElementIds = new List<int>();
            List<FeFaceName> allElementFaceNames = new List<FeFaceName>();
            List<double[]> allCoor = new List<double[]>();
            double[] faceCenter;
            FeElementSet elementSet;
            foreach (var entry in surface.ElementFaces)     // entry:  S3; elementSetName
            {
                elementSet = _model.Mesh.ElementSets[entry.Value];
                foreach (var elementId in elementSet.Labels)
                {
                    allElementIds.Add(elementId);
                    allElementFaceNames.Add(entry.Key);
                    _model.Mesh.GetElementFaceCenter(elementId, entry.Key, out faceCenter);
                    allCoor.Add(faceCenter);
                }
            }
            //
            int[] distributedElementIds = GetSpatiallyEquallyDistributedCoor(allCoor.ToArray(), 6);
            //
            int id;
            double[] faceNormal;
            List<double[]> distributedCoor = new List<double[]>();
            List<double[]> distributedLoadNormals = new List<double[]>();
            for (int i = 0; i < distributedElementIds.Length; i++)
            {
                id = distributedElementIds[i];
                _model.Mesh.GetElementFaceCenterAndNormal(allElementIds[id], allElementFaceNames[id], out faceCenter,
                                                          out faceNormal, out _);
                if (magnitude < 0)
                {
                    faceNormal[0] *= -1;
                    faceNormal[1] *= -1;
                    faceNormal[2] *= -1;
                }
                //
                distributedCoor.Add(faceCenter);
                distributedLoadNormals.Add(faceNormal);
            }
            // Arrows
            if (allCoor.Count > 0)
            {
                vtkMaxActorData data = new vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Geometry.Nodes.Coor = distributedCoor.ToArray();
                data.Geometry.Nodes.Normals = distributedLoadNormals.ToArray();
                data.SectionViewPossible = false;
                ApplyLighting(data);
                bool translate = magnitude > 0;
                _form.AddOrientedArrowsActor(data, symbolSize, translate);
            }
        }
        public void DrawGravityLoadSymbol(string prefixName, GravityLoad gLoad, double[] symbolCoor, Color color, 
                                          int symbolSize, vtkRendererLayer layer)
        {
            // Arrows
            double[] normal = new double[] { gLoad.F1.Value, gLoad.F2.Value, gLoad.F3.Value };
            //
            vtkMaxActorData data = new vtkMaxActorData();
            data.Name = prefixName;
            data.Color = color;
            data.Layer = layer;
            data.Geometry.Nodes.Coor = new double[][] { symbolCoor };
            data.Geometry.Nodes.Normals = new double[][] { normal };
            ApplyLighting(data);
            _form.AddOrientedArrowsActor(data, symbolSize);
            _form.AddSphereActor(data, symbolSize);
        }
        public void DrawCentrifLoadSymbol(string prefixName, CentrifLoad cfLoad, Color color, int symbolSize, 
                                          vtkRendererLayer layer)
        {
            // Arrows
            double[] normal = new double[] { cfLoad.N1.Value, cfLoad.N2.Value, cfLoad.N3.Value };
            //
            vtkMaxActorData data = new vtkMaxActorData();
            data.Name = prefixName;
            data.Color = color;
            data.Layer = layer;
            data.Geometry.Nodes.Coor = new double[][] { new double[] { cfLoad.X.Value, cfLoad.Y.Value, cfLoad.Z.Value } };
            data.Geometry.Nodes.Normals = new double[][] { normal };
            ApplyLighting(data);
            _form.AddOrientedDoubleArrowsActor(data, symbolSize);
            _form.AddSphereActor(data, symbolSize);
        }
        public void DrawPreTensionLoadSymbols(string prefixName, PreTensionLoad ptLoad, double[][] symbolCoor, Color color,
                                              int symbolSize, vtkRendererLayer layer)
        {
            // Arrows
            List<double[]> allLoadNormals = new List<double[]>();
            double[] normal;
            if (ptLoad.AutoComputeDirection) normal = _model.Mesh.GetSurfaceNormal(ptLoad.SurfaceName);
            else normal = new double[] { ptLoad.X.Value, ptLoad.Y.Value, ptLoad.Z.Value };
            //
            allLoadNormals.Add(normal);
            allLoadNormals.Add(new double[] { -normal[0], -normal[1], -normal[2] });
            //
            if (symbolCoor.GetLength(0) > 0)
            {
                vtkMaxActorData data = new vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Geometry.Nodes.Coor = symbolCoor.ToArray();
                data.Geometry.Nodes.Normals = allLoadNormals.ToArray();
                ApplyLighting(data);
                _form.AddOrientedArrowsActor(data, symbolSize);
            }
        }
        public void DrawCFluxSymbols(string prefixName, CFlux cFlux, double[][] symbolCoor, Color color,
                                     int symbolSize, vtkRendererLayer layer)
        {
            // Flux symbols
            if (symbolCoor.Length > 0)
            {
                double[][] normals = new double[symbolCoor.Length][];
                for (int i = 0; i < symbolCoor.Length; i++) normals[i] = new double[] { 1, 0, 0};
                //
                vtkMaxActorData data = new vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Geometry.Nodes.Coor = symbolCoor.ToArray();
                data.Geometry.Nodes.Normals = normals.ToArray();
                data.SectionViewPossible = false;
                ApplyLighting(data);
                bool translate = false;
                _form.AddOrientedFluxActor(data, symbolSize, true, translate);
            }
            return;
        }
        public void DrawCFluxSymbols_(string prefixName, CFlux cFlux, int[] nodeIds, double[][] symbolCoor, Color color,
                                     int symbolSize, vtkRendererLayer layer)
        {
            // Flux symbols
            if (symbolCoor.Length > 0)
            {
                double[][] normals = _model.Mesh.GetNodeNormals(nodeIds);
                //
                vtkMaxActorData data = new vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Geometry.Nodes.Coor = symbolCoor.ToArray();
                data.Geometry.Nodes.Normals = normals.ToArray();
                data.SectionViewPossible = false;
                ApplyLighting(data);
                _form.AddOrientedArrowsActor(data, symbolSize);
            }
            return;
        }
        public void DrawDFluxSymbols(string prefixName, DFlux dFlux, Color color, int symbolSize,
                                     vtkRendererLayer layer)
        {
            FeSurface surface = _model.Mesh.Surfaces[dFlux.SurfaceName];
            //
            List<int> allElementIds = new List<int>();
            List<FeFaceName> allElementFaceNames = new List<FeFaceName>();
            List<double[]> allCoor = new List<double[]>();
            double[] faceCenter;
            FeElementSet elementSet;
            foreach (var entry in surface.ElementFaces)     // entry:  S3; elementSetName
            {
                elementSet = _model.Mesh.ElementSets[entry.Value];
                foreach (var elementId in elementSet.Labels)
                {
                    allElementIds.Add(elementId);
                    allElementFaceNames.Add(entry.Key);
                    _model.Mesh.GetElementFaceCenter(elementId, entry.Key, out faceCenter);
                    allCoor.Add(faceCenter);
                }
            }
            //
            int[] distributedElementIds = GetSpatiallyEquallyDistributedCoor(allCoor.ToArray(), 6);
            // Front shell face which is a S2 POS face works in the same way as a solid face
            // Back shell face which is a S1 NEG must be inverted
            int id;
            double[] faceNormal;
            bool shellElement;
            double[][] distributedCoor = new double[distributedElementIds.Length][];
            double[][] distributedLoadNormals = new double[distributedElementIds.Length][];
            for (int i = 0; i < distributedElementIds.Length; i++)
            {
                id = distributedElementIds[i];
                _model.Mesh.GetElementFaceCenterAndNormal(allElementIds[id], allElementFaceNames[id], out faceCenter,
                                                          out faceNormal, out shellElement);
                //
                if ((surface.SurfaceFaceTypes == FeSurfaceFaceTypes.ShellEdgeFaces && dFlux.Magnitude.Value < 0) ||
                    (surface.SurfaceFaceTypes != FeSurfaceFaceTypes.ShellEdgeFaces && (dFlux.Magnitude.Value < 0) != shellElement))
                {
                    faceNormal[0] *= -1;
                    faceNormal[1] *= -1;
                    faceNormal[2] *= -1;
                }
                //
                distributedCoor[i] = faceCenter;
                distributedLoadNormals[i] = faceNormal;
            }
            // Arrows
            if (allCoor.Count > 0)
            {
                vtkMaxActorData data = new vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Geometry.Nodes.Coor = distributedCoor.ToArray();
                data.Geometry.Nodes.Normals = distributedLoadNormals.ToArray();
                data.SectionViewPossible = false;
                ApplyLighting(data);
                bool translate = dFlux.Magnitude.Value > 0;
                _form.AddOrientedArrowsActor(data, symbolSize, translate);
            }
        }
        public void DrawBodyFluxSymbol(string prefixName, BodyFlux bFlux, double[] symbolCoor, Color color,
                                      int symbolSize, vtkRendererLayer layer)
        {
            double[][] normals = new double[][] { new double[] { 1, 0, 0 } };
            //
            vtkMaxActorData data = new vtkMaxActorData();
            data.Name = prefixName;
            data.Color = color;
            data.Layer = layer;
            data.Geometry.Nodes.Coor = new double[][] { symbolCoor };
            data.Geometry.Nodes.Normals = normals;
            ApplyLighting(data);            
            _form.AddSphereActor(data, symbolSize * 0.8);
            //
            data.Geometry.Nodes.Coor = new double[6][];
            for (int i = 0; i < 6; i++)
            {
                data.Geometry.Nodes.Coor[i] = symbolCoor;
            }
            normals = new double[][] {
                new double[] { 1, 0, 0 },
                new double[] { -1, 0, 0 },
                new double[] { 0, 1, 0 },
                new double[] { 0, -1, 0 },
                new double[] { 0, 0, 1 },
                new double[] { 0, 0, -1 },
            };
            data.Geometry.Nodes.Normals = normals;
            _form.AddOrientedArrowsActor(data, symbolSize * 0.5);

        }
        public void DrawFilmSymbols(string prefixName, FilmHeatTransfer filmHeatTransfer, Color color, int symbolSize,
                                    vtkRendererLayer layer)
        {
            FeSurface surface = _model.Mesh.Surfaces[filmHeatTransfer.SurfaceName];
            //
            List<int> allElementIds = new List<int>();
            List<FeFaceName> allElementFaceNames = new List<FeFaceName>();
            List<double[]> allCoor = new List<double[]>();
            double[] faceCenter;
            FeElementSet elementSet;
            foreach (var entry in surface.ElementFaces)     // entry:  S3; elementSetName
            {
                elementSet = _model.Mesh.ElementSets[entry.Value];
                foreach (var elementId in elementSet.Labels)
                {
                    allElementIds.Add(elementId);
                    allElementFaceNames.Add(entry.Key);
                    _model.Mesh.GetElementFaceCenter(elementId, entry.Key, out faceCenter);
                    allCoor.Add(faceCenter);
                }
            }
            //
            int[] distributedElementIds = GetSpatiallyEquallyDistributedCoor(allCoor.ToArray(), 3);
            // Front shell face which is a S2 POS face works in the same way as a solid face
            // Back shell face which is a S1 NEG must be inverted
            int id;
            double[] faceNormal;
            bool shellElement;
            double[][] distributedCoor = new double[distributedElementIds.Length][];
            double[][] distributedLoadNormals = new double[distributedElementIds.Length][];
            for (int i = 0; i < distributedElementIds.Length; i++)
            {
                id = distributedElementIds[i];
                _model.Mesh.GetElementFaceCenterAndNormal(allElementIds[id], allElementFaceNames[id], out faceCenter,
                                                          out faceNormal, out shellElement);
                //
                if (surface.SurfaceFaceTypes == FeSurfaceFaceTypes.ShellEdgeFaces || !shellElement)
                {
                    faceNormal[0] *= -1;
                    faceNormal[1] *= -1;
                    faceNormal[2] *= -1;
                }
                //
                distributedCoor[i] = faceCenter;
                distributedLoadNormals[i] = faceNormal;
            }
            // Flux symbol
            if (allCoor.Count > 0)
            {
                vtkMaxActorData data = new vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Geometry.Nodes.Coor = distributedCoor.ToArray();
                data.Geometry.Nodes.Normals = distributedLoadNormals.ToArray();
                data.SectionViewPossible = false;
                ApplyLighting(data);
                bool translate = false;
                _form.AddOrientedFluxActor(data, symbolSize, false, translate);
            }
        }
        public void DrawRadiateSymbols(string prefixName, RadiationHeatTransfer radiationHeatTransfer, Color color, int symbolSize,
                                       vtkRendererLayer layer)
        {
            FeSurface surface = _model.Mesh.Surfaces[radiationHeatTransfer.SurfaceName];
            //
            List<int> allElementIds = new List<int>();
            List<FeFaceName> allElementFaceNames = new List<FeFaceName>();
            List<double[]> allCoor = new List<double[]>();
            double[] faceCenter;
            FeElementSet elementSet;
            foreach (var entry in surface.ElementFaces)     // entry:  S3; elementSetName
            {
                elementSet = _model.Mesh.ElementSets[entry.Value];
                foreach (var elementId in elementSet.Labels)
                {
                    allElementIds.Add(elementId);
                    allElementFaceNames.Add(entry.Key);
                    _model.Mesh.GetElementFaceCenter(elementId, entry.Key, out faceCenter);
                    allCoor.Add(faceCenter);
                }
            }
            //
            int[] distributedElementIds = GetSpatiallyEquallyDistributedCoor(allCoor.ToArray(), 3);
            // Front shell face which is a S2 POS face works in the same way as a solid face
            // Back shell face which is a S1 NEG must be inverted
            int id;
            double[] faceNormal;
            bool shellElement;
            double[][] distributedCoor = new double[distributedElementIds.Length][];
            double[][] distributedLoadNormals = new double[distributedElementIds.Length][];
            for (int i = 0; i < distributedElementIds.Length; i++)
            {
                id = distributedElementIds[i];
                _model.Mesh.GetElementFaceCenterAndNormal(allElementIds[id], allElementFaceNames[id], out faceCenter,
                                                          out faceNormal, out shellElement);
                //
                if (surface.SurfaceFaceTypes == FeSurfaceFaceTypes.ShellEdgeFaces || !shellElement)
                {
                    faceNormal[0] *= -1;
                    faceNormal[1] *= -1;
                    faceNormal[2] *= -1;
                }
                //
                distributedCoor[i] = faceCenter;
                distributedLoadNormals[i] = faceNormal;
            }
            // Flux symbol
            if (allCoor.Count > 0)
            {
                vtkMaxActorData data = new vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Geometry.Nodes.Coor = distributedCoor.ToArray();
                data.Geometry.Nodes.Normals = distributedLoadNormals.ToArray();
                data.SectionViewPossible = false;
                ApplyLighting(data);
                bool translate = false;
                _form.AddOrientedFluxActor(data, symbolSize, false, translate);
            }
        }
        public int[] GetSpatiallyEquallyDistributedCoor(double[][] coor, int maxN)
        {
            // Divide space into boxes and then find the coor closest to the box center
            if (coor.Length <= 0) return null;
            // Bounding box
            BoundingBox box = new BoundingBox();
            box.IncludeCoors(coor);
            //
            double max = Math.Max(box.MaxX - box.MinX, box.MaxY - box.MinY);
            max = Math.Max(max, box.MaxZ - box.MinZ);
            double maxDelta = max / maxN;
            //
            double n;
            int[] xyz = new int[3];
            //
            if (maxDelta < 1) maxDelta = 1;
            //
            n = (box.MaxX - box.MinX) / maxDelta;
            if (n < 1E-2) xyz[0] = 1;      // tiny
            else if (n < 2) xyz[0] = 2;    // small
            else xyz[0] = (int)n;          // normal
            //
            n = (box.MaxY - box.MinY) / maxDelta;
            if (n < 1E-2) xyz[1] = 1;      // tiny
            else if (n < 2) xyz[1] = 2;    // small
            else xyz[1] = (int)n;          // normal
            //
            n = (box.MaxZ - box.MinZ) / maxDelta;
            if (n < 1E-2) xyz[2] = 1;      // tiny
            else if (n < 2) xyz[2] = 2;    // small
            else xyz[2] = (int)n;          // normal
            //
            if (maxN == 1)
            {
                xyz[0] = 1;
                xyz[1] = 1;
                xyz[2] = 1;
            }
            //
            return GetSpatiallyEquallyDistributedCoor(coor, box, xyz);
        }
        private int[] GetSpatiallyEquallyDistributedCoor(double[][] coor, BoundingBox box, int[] n)
        {
            // Divide space into boxes and then find the coor closest to the box center
            if (coor.Length <= 0) return null;
            // Divide space into hexahedrons
            int nX = n[0];
            int nY = n[1];
            int nZ = n[2];
            //
            double deltaX = 1;
            double deltaY = 1;
            double deltaZ = 1;
            // Interval from 0...2 has 2 segments; value 2 is out of it
            if (box.MaxX - box.MinX != 0) deltaX = ((box.MaxX - box.MinX) / nX) * 1.01;    
            if (box.MaxY - box.MinY != 0) deltaY = ((box.MaxY - box.MinY) / nY) * 1.01;
            if (box.MaxZ - box.MinZ != 0) deltaZ = ((box.MaxZ - box.MinZ) / nZ) * 1.01;
            box.MinX -= deltaX * 0.005;
            box.MinY -= deltaY * 0.005;
            box.MinZ -= deltaZ * 0.005;
            //
            List<int>[][][] spatialIds = new List<int>[nX][][];
            for (int i = 0; i < nX; i++)
            {
                spatialIds[i] = new List<int>[nY][];
                for (int j = 0; j < nY; j++)
                {
                    spatialIds[i][j] = new List<int>[nZ];
                }
            }
            // Fill space hexahedrons
            int idX;
            int idY;
            int idZ;
            for (int i = 0; i < coor.GetLength(0); i++)
            {
                idX = (int)Math.Floor((coor[i][0] - box.MinX) / deltaX);
                idY = (int)Math.Floor((coor[i][1] - box.MinY) / deltaY);
                idZ = (int)Math.Floor((coor[i][2] - box.MinZ) / deltaZ);
                if (spatialIds[idX][idY][idZ] == null) spatialIds[idX][idY][idZ] = new List<int>();
                spatialIds[idX][idY][idZ].Add(i);
            }
            //
            double[] center = new double[3];
            List<int> centerIds = new List<int>();
            for (int i = 0; i < nX; i++)
            {
                for (int j = 0; j < nY; j++)
                {
                    for (int k = 0; k < nZ; k++)
                    {
                        if (spatialIds[i][j][k] != null)
                        {
                            center[0] = box.MinX + (i + 0.5) * deltaX;
                            center[1] = box.MinY + (j + 0.5) * deltaY;
                            center[2] = box.MinZ + (k + 0.5) * deltaZ;
                            //
                            centerIds.Add(FindClosestIdFromIds(spatialIds[i][j][k].ToArray(), center, coor));
                        }
                    }
                }
            }
            //
            return centerIds.ToArray();
        }
        private int FindClosestIdFromIds(int[] ids, double[] center, double[][] coor)
        {
            int minId = -1;
            double minDistance = double.MaxValue;
            int id;
            double d;
            for (int i = 0; i < ids.Length; i++)
            {
                id = ids[i];
                d = Math.Pow(center[0] - coor[id][0], 2) +
                             Math.Pow(center[1] - coor[id][1], 2) +
                             Math.Pow(center[2] - coor[id][2], 2);
                //
                if (d < minDistance)
                {
                    minDistance = d;
                    minId = id;
                }
            }
            return minId;
        }
        // Geometry
        public int DrawNodes(string prefixName, int[] nodeIds, Color color, vtkRendererLayer layer,
                             int nodeSize = -1, bool onlyVisible = false, bool useSecondaryHighlightColor = false)
        {
            double[][] nodeCoor = DisplayedMesh.GetNodeSetCoor(nodeIds, onlyVisible);
            DrawNodes(prefixName, nodeCoor, color, layer, nodeSize, false, useSecondaryHighlightColor);
            return nodeCoor.Length;
        }
        public void DrawNodes(string prefixName, double[][] nodeCoor, Color color, vtkRendererLayer layer,
                              int nodeSize = -1, bool drawOnGeometry = false, bool useSecondaryHighlightColor = false)
        {
            if (nodeSize == -1) nodeSize = _settings.Pre.NodeSymbolSize;
            //
            vtkMaxActorData data = new vtkMaxActorData();
            data.Name = prefixName + Globals.NameSeparator + "nodes";
            data.NodeSize = nodeSize;
            data.Color = color;
            data.Layer = layer;
            data.DrawOnGeometry = drawOnGeometry;
            data.Geometry.Nodes.Coor = nodeCoor;
            data.UseSecondaryHighightColor = useSecondaryHighlightColor;
            //
            ApplyLighting(data);
            _form.Add3DNodes(data);
        }
        public int DrawNodeSet(string prefixName, string nodeSetName, Color color, 
                               vtkRendererLayer layer, bool backfaceCulling = true, int nodeSize = -1,
                               bool useSecondaryHighlightColor = false, bool onlyVisible = false)
        {            
            if (nodeSetName != null)
            {
                FeMesh mesh = DisplayedMesh;
                //
                if (mesh.NodeSets.ContainsKey(nodeSetName))
                {
                    FeNodeSet nodeSet = mesh.NodeSets[nodeSetName];
                    //
                    if (nodeSize == -1) nodeSize = _settings.Pre.NodeSymbolSize;
                    // Draw node set as geometry
                    if (nodeSet.CreationData != null && nodeSet.CreationData.SelectItem == vtkSelectItem.Geometry)
                    {
                        int[] ids = nodeSet.CreationIds;
                        //
                        if (ids == null || ids.Length == 0) return 0;
                        //
                        nodeSize = (int)Math.Max(1.5 * nodeSize, nodeSize + 3);
                        return DrawItemsByGeometryIds(ids, prefixName, nodeSetName, color, layer, nodeSize, backfaceCulling,
                                                      useSecondaryHighlightColor, onlyVisible);
                    }
                    // Draw node set as single nodes
                    else
                    {
                        double[][] nodeCoor = mesh.GetNodeSetCoor(nodeSet.Labels, onlyVisible);
                        //
                        DrawNodes(prefixName + Globals.NameSeparator + nodeSetName, nodeCoor, color,
                                  layer, nodeSize, false, useSecondaryHighlightColor);
                        return nodeCoor.Length;
                    }
                }
            }
            return 0;
        }
        private int DrawElements(string prefixName, int[] elementIds, Color color,
                                 vtkRendererLayer layer)
        {
            int[] nodeIds;
            double[][] nodeCoor;
            int[] cellIds;
            int[][] cells;
            int[] cellTypes;
            bool canHaveEdges = true;            
            vtkMaxActorData data;
            //
            FeMesh mesh = DisplayedMesh;
            BasePart[] parts = mesh.CreateBasePartsByTypeFromElementIds(elementIds);
            //
            int drawnCells = 0;
            foreach (BasePart part in parts)
            {
                mesh.GetVisualizationNodesAndCells(part, out nodeIds, out nodeCoor, out cellIds, out cells, out cellTypes);
                //
                data = new vtkMaxActorData();
                data.Name = prefixName + Globals.NameSeparator + "elements";
                data.Color = color;
                data.Layer = layer;
                if (part.PartType == PartType.Shell) data.BackfaceCulling = false;
                else data.BackfaceCulling = true;
                //
                data.CanHaveElementEdges = canHaveEdges;
                data.Geometry.Nodes.Ids = null;
                data.Geometry.Nodes.Coor = nodeCoor;
                data.Geometry.Cells.CellNodeIds = cells;
                data.Geometry.Cells.Types = cellTypes;
                //
                ApplyLighting(data);
                _form.Add3DCells(data);
                //
                drawnCells += cellIds.Length;
            }
            //
            return drawnCells;
        }
        private int DrawElementSet(string prefixName, FeElementSet elementSet, Color color,
                                   vtkRendererLayer layer, bool backfaceCulling = true)
        {
            int count = 0;
            if (elementSet.CreatedFromParts) count += HighlightModelParts(_model.Mesh.GetPartNamesFromPartIds(elementSet.Labels));
            else if (elementSet.CreationData != null && elementSet.CreationData.SelectItem == vtkSelectItem.Geometry)
            {
                // In order for the Regenerate history to work perform the selection
                int[] ids = elementSet.CreationIds;
                //
                if (ids == null || ids.Length == 0) return 0;
                //
                bool useSecondaryHighlightColor = false;
                bool onlyVisible = false;
                count += DrawItemsByGeometryIds(ids, prefixName, elementSet.Name, color, layer, 5, backfaceCulling,
                                                useSecondaryHighlightColor, onlyVisible);
            }
            else
            {
                count += DrawElements(prefixName, elementSet.Labels, color, layer);
            }
            //
            return count;
        }
        public int DrawSurfaceWithEdge(string prefixName, string surfaceName, Color color,
                                       vtkRendererLayer layer, bool backfaceCulling = true,
                                       bool useSecondaryHighlightColor = false, bool onlyVisible = false)
        {
            int count = DrawSurface(prefixName, surfaceName, color, layer, backfaceCulling, useSecondaryHighlightColor, onlyVisible);
            if (layer == vtkRendererLayer.Selection)
                DrawSurfaceEdge(prefixName, surfaceName, color, layer, backfaceCulling, useSecondaryHighlightColor, onlyVisible);
            return count;
        }
        public int DrawSurface(string prefixName, string surfaceName, Color color,
                               vtkRendererLayer layer, bool backfaceCulling = true,
                               bool useSecondaryHighlightColor = false, bool onlyVisible = false)
        {
            FeSurface s;
            FeMesh mesh = DisplayedMesh;
            if (mesh.Surfaces.TryGetValue(surfaceName, out s) && s.Active && s.Visible && s.Valid)
            {
                if (s.Type == FeSurfaceType.Element && s.ElementFaces != null)
                {
                    if (s.SurfaceFaceTypes == FeSurfaceFaceTypes.ShellEdgeFaces)
                    {
                        return DrawSurfaceEdge(prefixName, surfaceName, color, layer, backfaceCulling,
                                               useSecondaryHighlightColor, onlyVisible);
                    }
                    else
                    {
                        vtkMaxActorData data = new vtkMaxActorData();
                        mesh.GetSurfaceGeometry(surfaceName, out data.Geometry.Nodes.Coor, out data.Geometry.Cells.CellNodeIds,
                                                out data.Geometry.Cells.Types, onlyVisible);
                        //
                        data.Name = prefixName + Globals.NameSeparator + surfaceName;
                        data.Color = color;
                        data.Layer = layer;
                        data.CanHaveElementEdges = true;
                        data.BackfaceCulling = backfaceCulling;
                        data.DrawOnGeometry = true;
                        data.UseSecondaryHighightColor = useSecondaryHighlightColor;
                        //
                        ApplyLighting(data);
                        _form.Add3DCells(data);
                        //
                        return data.Geometry.Cells.CellNodeIds.Length;
                    }
                }
                else if (s.Type == FeSurfaceType.Node && Model.Mesh.NodeSets.TryGetValue(s.NodeSetName, out _))
                {
                    return DrawNodeSet(prefixName + Globals.NameSeparator + surfaceName, s.NodeSetName, color, layer,
                                       true, -1, useSecondaryHighlightColor, onlyVisible);
                }
            }
            return 0;
        }

        public void DrawSurface(string prefixName, int[][] cells, Color color,
                                vtkRendererLayer layer, bool backfaceCulling = true,
                                bool useSecondaryHighlightColor = false, bool drawEdges = false)
        {
            FeMesh mesh = DisplayedMesh;
            // Copy
            int[][] cellsCopy = new int[cells.Length][];
            for (int i = 0; i < cells.Length; i++) cellsCopy[i] = cells[i].ToArray();
            // Faces
            vtkMaxActorData data = new vtkMaxActorData();
            data.Name = prefixName + Globals.NameSeparator + "draw_surface";
            data.Color = color;
            data.Layer = layer;
            data.CanHaveElementEdges = true;
            data.BackfaceCulling = backfaceCulling;
            data.DrawOnGeometry = true;
            data.UseSecondaryHighightColor = useSecondaryHighlightColor;
            data.Geometry.Cells.CellNodeIds = cells;
            mesh.GetSurfaceGeometry(cells, out data.Geometry.Nodes.Ids, out data.Geometry.Nodes.Coor, out data.Geometry.Cells.Types);
            //
            ApplyLighting(data);
            _form.Add3DCells(data);
            //
            if (!drawEdges) return;
            // Edges
            cells = mesh.GetFreeEdgesFromVisualizationCells(cellsCopy, null);
            //
            data = new vtkMaxActorData();
            data.Name = prefixName + Globals.NameSeparator + "draw_surface_edges";
            data.Color = color;
            data.Layer = layer;
            data.CanHaveElementEdges = true;
            data.BackfaceCulling = backfaceCulling;
            data.UseSecondaryHighightColor = useSecondaryHighlightColor;
            data.Geometry.Cells.CellNodeIds = cells;
            mesh.GetSurfaceEdgesGeometry(cells, out data.Geometry.Nodes.Ids, out data.Geometry.Nodes.Coor,
                                         out data.Geometry.Cells.Types);
            //
            ApplyLighting(data);
            _form.Add3DCells(data);
        }
        public int DrawSurfaceEdge(string prefixName, string surfaceName, Color color,
                                    vtkRendererLayer layer, bool backfaceCulling = true,
                                    bool useSecondaryHighlightColor = false, bool onlyVisible = false)
        {
            FeSurface s;
            if (_model.Mesh.Surfaces.TryGetValue(surfaceName, out s) && s.Active && s.Visible && s.Valid)
            {
                if (s.Type == FeSurfaceType.Element && s.ElementFaces != null)
                {
                    vtkMaxActorData data = new vtkMaxActorData();
                    data.Name = prefixName + Globals.NameSeparator + surfaceName + "_edge";
                    data.LineWidth = 2;
                    data.Color = color;
                    data.Layer = layer;
                    data.CanHaveElementEdges = true;
                    data.BackfaceCulling = backfaceCulling;
                    data.UseSecondaryHighightColor = useSecondaryHighlightColor;
                    _model.Mesh.GetSurfaceEdgesGeometry(surfaceName, out data.Geometry.Nodes.Coor,
                                                        out data.Geometry.Cells.CellNodeIds,
                                                        out data.Geometry.Cells.Types, onlyVisible);
                    //
                    ApplyLighting(data);
                    _form.Add3DCells(data);
                    //
                    return data.Geometry.Cells.CellNodeIds.Length;
                }
                else if (s.Type == FeSurfaceType.Node && Model.Mesh.NodeSets.TryGetValue(s.NodeSetName, out _))
                {
                    //DrawNodeSet(prefixName + Globals.NameSeparator + surfaceName, s.NodeSetName, color, layer);
                }
            }
            return 0;
        }
        // Draw geometry ids
        public void DrawEdgesByGeometryEdgeIds(string prefixName, int[] ids, Color color,
                                               vtkRendererLayer layer, int nodeSize = -1,
                                               bool useSecondaryHighlightColor = false)
        {
            if (nodeSize == -1) nodeSize = _settings.Pre.NodeSymbolSize;
            // QueryEdge from frmQuery
            vtkMaxActorData data = GetGeometryEdgeActorData(ids);
            data.Name = prefixName + Globals.NameSeparator + "edges";
            data.NodeSize = nodeSize;
            data.LineWidth = 2;
            data.Color = color;
            data.Layer = layer;
            data.DrawOnGeometry = layer != vtkRendererLayer.Selection;
            data.UseSecondaryHighightColor = useSecondaryHighlightColor;
            ApplyLighting(data);
            _form.Add3DCells(data);
        }
        public void DrawItemsBySurfaceIds(string prefixName, int[] ids, Color color,
                                          vtkRendererLayer layer, bool backfaceCulling = true,
                                          bool useSecondaryHighlightColor = false, bool drawSurfaceEdges = false)
        {
            int[][] cells;
            // Highlight surface: QuerySurface from frmQuery
            if (ids.Length == 1 && DisplayedMesh.IsThisIdGeometryId(ids[0]))
                cells = GetSurfaceCellsByGeometryId(ids, out _);
            else cells = GetSurfaceCellsByFaceIds(ids, out _);
            //
            DrawSurface(prefixName, cells, color, layer, backfaceCulling, useSecondaryHighlightColor, drawSurfaceEdges);
        }
        private int DrawItemsByGeometryIds(int[] ids, string prefixName, string itemName, Color color,
                                           vtkRendererLayer layer, int nodeSize = -1, bool backfaceCulling = true,
                                           bool useSecondaryHighlightColor = false, bool onlyVisible = false)
        {
            List<int> nodeIdsList = new List<int>();
            List<int> edgeIdsList = new List<int>();
            List<int> surfaceIdsList = new List<int>();
            List<int> partIdsList = new List<int>();
            int[] itemTypePartIds;
            FeMesh mesh = DisplayedMesh;
            foreach (var id in ids)
            {
                itemTypePartIds = FeMesh.GetItemTypePartIdsFromGeometryId(id);
                if (mesh.GetPartFromId(itemTypePartIds[2]) is BasePart bp && bp != null && bp.Visible)
                {
                    GeometryType geomType = (GeometryType)itemTypePartIds[1];
                    if (geomType == GeometryType.Vertex) nodeIdsList.Add(id);
                    else if (geomType.IsEdge()) edgeIdsList.Add(id);
                    else if (geomType.IsSurface()) surfaceIdsList.Add(id);
                    else if (geomType == GeometryType.Part) partIdsList.Add(id);
                    else throw new NotSupportedException();
                }
            }
            //
            int[] nodeIds = mesh.GetIdsFromGeometryIds(nodeIdsList.ToArray(), vtkSelectItem.Node, onlyVisible);
            int[] edgeIds = mesh.GetIdsFromGeometryIds(edgeIdsList.ToArray(), vtkSelectItem.GeometryEdge, onlyVisible);
            int[] surfaceFaceIds = mesh.GetIdsFromGeometryIds(surfaceIdsList.ToArray(), vtkSelectItem.Surface, onlyVisible);
            int[] partFaceIds = mesh.GetIdsFromGeometryIds(partIdsList.ToArray(), vtkSelectItem.Surface, onlyVisible);
            //
            string name = prefixName + Globals.NameSeparator + itemName;
            bool selection = layer == vtkRendererLayer.Selection;
            bool drawSurfaceEdges = selection;
            if (nodeIds.Length > 0)
            {
                // Black border
                int nodeSizeBB = nodeSize == -1 ? _settings.Pre.NodeSymbolSize + 2 : nodeSize + 2;
                if (!selection) DrawNodes(name + "black", nodeIds, Color.Black, layer, nodeSizeBB + 2);
                //
                DrawNodes(name, nodeIds, color, layer, nodeSize, onlyVisible, useSecondaryHighlightColor);
            }
            if (edgeIds.Length > 0) DrawEdgesByGeometryEdgeIds(name, edgeIds, color, layer, nodeSize, useSecondaryHighlightColor);
            if (surfaceFaceIds.Length > 0) DrawItemsBySurfaceIds(name, surfaceFaceIds, color, layer, backfaceCulling,
                                                                 useSecondaryHighlightColor, drawSurfaceEdges);
            if (partFaceIds.Length > 0) DrawItemsBySurfaceIds(name, partFaceIds, color, layer, backfaceCulling,
                                                             useSecondaryHighlightColor, drawSurfaceEdges);
            //
            return nodeIds.Length + edgeIds.Length + surfaceFaceIds.Length + partFaceIds.Length;
        }
        // Tools
        public void DrawArrowSymbols(string prefixName, double[][] symbolCoor, double[][] symbolNormals, Color color,
                                     int symbolSize, vtkRendererLayer layer, bool useSecondaryHighlightColor = false,
                                     bool doubleArrow = false)
        {
            // Arrows
            if (symbolCoor.Length > 0)
            {
                vtkMaxActorData data = new vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Geometry.Nodes.Coor = symbolCoor.ToArray();
                data.Geometry.Nodes.Normals = symbolNormals.ToArray();
                data.UseSecondaryHighightColor = useSecondaryHighlightColor;
                //
                ApplyLighting(data);
                if (doubleArrow) _form.AddOrientedDoubleArrowsActor(data, symbolSize);
                else _form.AddOrientedArrowsActor(data, symbolSize);
            }
        }

        private void ReduceCoor(ref double[][] coor, int numberOfPoints)
        {
            if (coor.Length > numberOfPoints)
            {
                List<int> allIds = new List<int>();
                for (int i = 0; i < coor.Length; i++) allIds.Add(i);
                //
                int id;
                Random rand = new Random();
                double[][] newCoor = new double[numberOfPoints][];
                for (int i = 0; i < numberOfPoints; i++)
                {
                    id = rand.Next(0, allIds.Count() - 1);
                    //
                    newCoor[i] = coor[id];
                    allIds.RemoveAt(id);
                }
                coor = newCoor;
            }
        }
        // Apply settings
        private void ApplyLighting(vtkMaxActorData data)
        {
            data.Ambient = _settings.Graphics.AmbientComponent;
            data.Diffuse = _settings.Graphics.DiffuseComponent;
        }


        #endregion #################################################################################################################

        #region Highlight  #########################################################################################################
        public void UpdateHighlight()
        {            
            _form.UpdateHighlight();
        }
        public void Highlight3DObjects(object[] obj, bool clear = true)
        {
            Highlight3DObjects(_currentView, obj, clear);
        }
        public void Highlight3DObjects(ViewGeometryModelResults view, object[] obj, bool clear)
        {
            if (clear) _form.Clear3DSelection();       // must be here: clears the highlight in the results
            //
            if (obj != null)
            {
                foreach (var item in obj) Highlight3DObject(view, item);
                //
                _form.AdjustCameraDistanceAndClipping();
            }
        }
        private void Highlight3DObject(ViewGeometryModelResults view, object obj)
        {
            try
            {
                if (view == ViewGeometryModelResults.Geometry)
                {
                    if (obj is GeometryPart gp)
                    {
                        HighlightGeometryParts(new string[] { gp.Name });
                    }
                    else if (obj is MeshSetupItem msi)
                    {
                        HighlightMeshSetupItems(new string[] { msi.Name });
                    }
                }
                else if (view == ViewGeometryModelResults.Model)
                {
                    if (obj is string name)
                    {
                        if (_model.Mesh.NodeSets.ContainsKey(name))
                            Highlight3DObject(view, _model.Mesh.NodeSets[name]);
                        else if (_model.Mesh.ElementSets.ContainsKey(name))
                            Highlight3DObject(view, _model.Mesh.ElementSets[name]);
                        else if (_model.Mesh.Parts.ContainsKey(name))
                            Highlight3DObject(view, _model.Mesh.Parts[name]);
                        else if (_model.Mesh.Surfaces.ContainsKey(name))
                            Highlight3DObject(view, _model.Mesh.Surfaces[name]);
                        else if (_model.Mesh.ReferencePoints.ContainsKey(name))
                            Highlight3DObject(view, _model.Mesh.ReferencePoints[name]);
                    }
                    else if (obj is MeshPart mp)
                    {
                        HighlightModelParts(new string[] { mp.Name });
                    }
                    else if (obj is FeNodeSet ns)
                    {
                        HighlightNodeSets(new string[] { ns.Name });
                    }
                    else if (obj is FeElementSet es)
                    {
                        HighlightElementSets(new string[] { es.Name });
                    }
                    else if (obj is FeSurface s)
                    {
                        HighlightSurfaces(new string[] { s.Name });
                    }
                    else if (obj is FeReferencePoint rp)
                    {
                        HighlightReferencePoints(new string[] { rp.Name });
                    }
                    else if (obj is Section sec)
                    {
                        if (sec.RegionType == RegionTypeEnum.PartName) HighlightModelParts(new string[] { sec.RegionName });
                        else if (sec.RegionType == RegionTypeEnum.ElementSetName)
                        {
                            bool backfaceCulling = sec is SolidSection;
                            HighlightElementSets(new string[] { sec.RegionName }, backfaceCulling);
                        }
                        else throw new NotSupportedException();
                    }
                    else if (obj is Constraint c)
                    {
                        HighlightConstraints(new string[] { c.Name });
                    }
                    else if (obj is ContactPair cp)
                    {
                        HighlightContactPairs(new string[] { cp.Name });
                    }
                    else if (obj is InitialCondition ic)
                    {
                        if (ic.RegionType == RegionTypeEnum.PartName)
                            HighlightModelParts(new string[] { ic.RegionName });
                        else if (ic.RegionType == RegionTypeEnum.NodeSetName)
                            HighlightNodeSets(new string[] { ic.RegionName });
                        else if (ic.RegionType == RegionTypeEnum.ElementSetName)
                            HighlightElementSets(new string[] { ic.RegionName });
                        else if (ic.RegionType == RegionTypeEnum.SurfaceName)
                            HighlightSurfaces(new string[] { ic.RegionName });
                        else if (ic.RegionType == RegionTypeEnum.Selection) { }
                        else throw new NotSupportedException();
                    }
                    else if (obj is HistoryOutput ho)
                    {
                        if (ho.RegionType == RegionTypeEnum.NodeSetName)
                            HighlightNodeSets(new string[] { ho.RegionName });
                        else if (ho.RegionType == RegionTypeEnum.ElementSetName)
                            HighlightElementSets(new string[] { ho.RegionName });
                        else if (ho.RegionType == RegionTypeEnum.SurfaceName)
                            HighlightSurfaces(new string[] { ho.RegionName });
                        else if (ho.RegionType == RegionTypeEnum.ReferencePointName)
                            HighlightReferencePoints(new string[] { ho.RegionName });
                        else if (ho.RegionType == RegionTypeEnum.ContactPair)
                            HighlightContactPairs(new string[] { ho.RegionName });
                        else if (ho.RegionType == RegionTypeEnum.Selection) { }
                        else throw new NotSupportedException();
                    }
                    else if (obj is BoundaryCondition bc)
                    {
                        HighlightBoundaryCondition(bc);
                    }
                    else if (obj is Load l)
                    {
                        HighlightLoad(l);
                    }
                    else if (obj is DefinedField df)
                    {
                        if (df is DefinedTemperature dt && dt.Type == DefinedTemperatureTypeEnum.FromFile) { }
                        else if (df.RegionType == RegionTypeEnum.NodeSetName) HighlightNodeSets(new string[] { df.RegionName });
                        else if (df.RegionType == RegionTypeEnum.SurfaceName) HighlightSurfaces(new string[] { df.RegionName });
                        else if (df.RegionType == RegionTypeEnum.Selection) { }
                        else throw new NotSupportedException();
                    }
                }
                else if (view == ViewGeometryModelResults.Results)
                {
                    if (obj is string name)
                    {
                        if (_allResults.CurrentResult.Mesh.NodeSets.ContainsKey(name))
                            Highlight3DObject(view, _allResults.CurrentResult.Mesh.NodeSets[name]);
                        else if (_allResults.CurrentResult.Mesh.ElementSets.ContainsKey(name))
                            Highlight3DObject(view, _allResults.CurrentResult.Mesh.ElementSets[name]);
                        //else if (_results.Mesh.Parts.ContainsKey(name))
                        //    Highlight3DObject(view, _results.Mesh.Parts[name]);
                        else if (_allResults.CurrentResult.Mesh.Surfaces.ContainsKey(name))
                            Highlight3DObject(view, _allResults.CurrentResult.Mesh.Surfaces[name]);
                        //else if (_results.Mesh.ReferencePoints.ContainsKey(name))
                        //    Highlight3DObject(view, _results.Mesh.ReferencePoints[name]);
                    }
                    else if (obj is ResultPart || obj is GeometryPart)
                    {
                        HighlightResultParts(new string[] { ((BasePart)obj).Name });
                    }
                    else if (obj is FeNodeSet ns)
                    {
                        HighlightNodeSets(new string[] { ns.Name });
                    }
                    else if (obj is FeElementSet es)
                    {
                        HighlightElementSets(new string[] { es.Name });
                    }
                    else if (obj is FeSurface s)
                    {
                        HighlightSurfaces(new string[] { s.Name });
                    }
                }
            }
            catch { }
        }
        public void HighlightGeometryParts(string[] partsToSelect)
        {
            HashSet<string> partNamesToSelect = new HashSet<string>(partsToSelect);
            // Find all sub parts to select except the compound parts
            foreach (var name in partsToSelect)
            {
                if (_model.Geometry.Parts.ContainsKey(name) && _model.Geometry.Parts[name] is CompoundGeometryPart cgp)
                {
                    partNamesToSelect.Remove(cgp.Name);
                    partNamesToSelect.UnionWith(cgp.SubPartNames);
                }
            }
            //
            GeometryPart[] parts = GetGeometryParts();
            Color color = Settings.Pre.PrimaryHighlightColor;
            vtkRendererLayer layer = vtkRendererLayer.Selection;
            //
            bool solidError;
            bool shellError;
            HashSet<int> edgeCellIds = new HashSet<int>();
            HashSet<int> nodeIds = new HashSet<int>();
            List<int[]> edgeCells = new List<int[]>();
            foreach (var part in parts)
            {
                if (partNamesToSelect.Contains(part.Name) && _form.ContainsActor(part.Name))
                {
                    solidError = (part.PartType == PartType.Solid || part.PartType == PartType.SolidAsShell) && part.HasFreeEdges;
                    shellError = part.PartType == PartType.Shell && part.HasErrors;
                    //
                    if (solidError || shellError)
                    {
                        // Error                                            
                        edgeCellIds.Clear();
                        if (solidError)
                        {
                            if (part.ErrorEdgeCellIds != null) edgeCellIds.UnionWith(part.ErrorEdgeCellIds);
                            if (part.FreeEdgeCellIds != null) edgeCellIds.UnionWith(part.FreeEdgeCellIds);
                        }
                        else if (shellError)
                        {
                            if (part.ErrorEdgeCellIds != null) edgeCellIds.UnionWith(part.ErrorEdgeCellIds);
                        }
                        //
                        edgeCells.Clear();
                        foreach (var elementId in edgeCellIds) edgeCells.Add(part.Visualization.EdgeCells[elementId]);
                        //
                        vtkMaxActorData data = new vtkMaxActorData();
                        DisplayedMesh.GetNodesAndCellsForEdges(edgeCells.ToArray(), out data.Geometry.Nodes.Ids,
                                                               out data.Geometry.Nodes.Coor,
                                                               out data.Geometry.Cells.CellNodeIds,
                                                               out data.Geometry.Cells.Types);
                        // Data
                        data.Name = part.Name + "_ErrorEdgeElements";
                        data.Color = color;
                        data.Layer = layer;
                        data.CanHaveElementEdges = true;
                        data.BackfaceCulling = true;
                        data.UseSecondaryHighightColor = false;
                        //
                        ApplyLighting(data);
                        _form.Add3DCells(data);
                        // Nodes                
                        nodeIds.Clear();
                        if (solidError)
                        {
                            if (part.ErrorNodeIds != null) nodeIds.UnionWith(part.ErrorNodeIds);
                            if (part.FreeNodeIds != null) nodeIds.UnionWith(part.FreeNodeIds);
                        }
                        else if (shellError)
                        {
                            if (part.ErrorNodeIds != null) nodeIds.UnionWith(part.ErrorNodeIds);
                        }
                        DrawNodes(part.Name, nodeIds.ToArray(), color, layer);
                        // Free                                             
                        if (shellError)
                        {
                            edgeCellIds.Clear();
                            if (part.FreeEdgeCellIds != null) edgeCellIds.UnionWith(part.FreeEdgeCellIds);
                            if (part.ErrorEdgeCellIds != null) edgeCellIds.ExceptWith(part.ErrorEdgeCellIds);
                            //
                            edgeCells.Clear();
                            foreach (var elementId in edgeCellIds) edgeCells.Add(part.Visualization.EdgeCells[elementId]);
                            //
                            data = new vtkMaxActorData();
                            DisplayedMesh.GetNodesAndCellsForEdges(edgeCells.ToArray(), out data.Geometry.Nodes.Ids,
                                                                   out data.Geometry.Nodes.Coor,
                                                                   out data.Geometry.Cells.CellNodeIds,
                                                                   out data.Geometry.Cells.Types);
                            // Data
                            data.Name = part.Name + "_ErrorEdgeElements";
                            data.Color = color;
                            data.Layer = layer;
                            data.CanHaveElementEdges = true;
                            data.BackfaceCulling = true;
                            data.UseSecondaryHighightColor = true;
                            //
                            ApplyLighting(data);
                            _form.Add3DCells(data);
                            // Nodes                
                            nodeIds.Clear();
                            if (part.FreeNodeIds != null) nodeIds.UnionWith(part.FreeNodeIds);
                            if (part.ErrorNodeIds != null) nodeIds.ExceptWith(part.ErrorNodeIds);
                            DrawNodes(part.Name, nodeIds.ToArray(), color, layer, -1, false, true);
                        }
                    }
                    else
                    {
                        _form.HighlightActor(part.Name);
                    }
                }
            }
        }
        public int HighlightModelParts(string[] partsToSelect)
        {
            MeshPart[] parts = GetModelParts();
            //
            int count = 0;
            foreach (var part in parts)
            {
                if (partsToSelect.Contains(part.Name))
                {
                    if (_form.ContainsActor(part.Name))
                    {
                        _form.HighlightActor(part.Name);
                        count++;
                    }
                }
            }
            return count;
        }
        public void HighlightResultParts(string[] partsToSelect)
        {
            BasePart[] parts = GetResultParts();
            //
            foreach (var part in parts)
            {
                //if (part.Visible && partsToSelect.Contains(part.Name))
                if (partsToSelect.Contains(part.Name))
                {
                    if (_form.ContainsActor(part.Name)) _form.HighlightActor(part.Name);
                }
            }
        }
        //
        public void HighlightMeshSetupItems(string[] meshSetupItemNamesToSelect)
        {
            MeshSetupItem meshSetupItem;
            //
            foreach (var name in meshSetupItemNamesToSelect)
            {
                meshSetupItem = _model.Geometry.MeshSetupItems[name];
                HighlightMeshSetupItem(meshSetupItem);
            }
        }
        public void HighlightMeshSetupItem(MeshSetupItem meshSetupItem, bool highlightNodes = true,
                                           bool useSecondaryHighlightColor = false)
        {
            if (meshSetupItem is MeshingParameters || meshSetupItem is ShellGmsh || meshSetupItem is ThickenShellMesh ||
                meshSetupItem is TetrahedralGmsh || meshSetupItem is TransfiniteMesh)
                HighlightMeshSetupItemParts(meshSetupItem, useSecondaryHighlightColor);
            else if (meshSetupItem is FeMeshRefinement mr) HighlightMeshRefinement(mr, highlightNodes, useSecondaryHighlightColor);
            else if (meshSetupItem is ExtrudeMesh em) HighlightExtrudeMesh(em, useSecondaryHighlightColor);
            else if (meshSetupItem is RevolveMesh rm) HighlightRevolveMesh(rm, useSecondaryHighlightColor);
            else throw new NotSupportedException();
        }
        public void HighlightMeshSetupItemParts(MeshSetupItem meshSetupItem, bool useSecondaryHighlightColor = false)
        {
            HashSet<int> selectedPartIds = new HashSet<int>(meshSetupItem.CreationIds);
            if (selectedPartIds.Count > 0)
            {
                string[] partNames = _model.Geometry.GetPartNamesFromPartIds(selectedPartIds.ToArray());
                HighlightGeometryParts(partNames);
            }
        }
        public void HighlightMeshRefinement(FeMeshRefinement meshRefinement, bool highlightNodes = true, 
                                            bool useSecondaryHighlightColor = false)
        {
            int[] ids;
            int[] itemTypePartIds;
            double[][] coor;
            double meshSize;
            bool backfaceCulling;
            GeometryPart part;
            FeMesh mesh = DisplayedMesh;
            MeshingParameters meshingParameters;
            //
            ids = meshRefinement.CreationIds;
            if (ids.Length == 0) return;
            // The selection is limited to one part
            itemTypePartIds = FeMesh.GetItemTypePartIdsFromGeometryId(ids[0]);
            part = (GeometryPart)mesh.GetPartFromId(itemTypePartIds[2]);
            if (part == null) return;
            //
            meshingParameters = GetPartMeshingParameters(part.Name);
            //
            meshSize = meshRefinement.MeshSize;
            //if (meshRefinement.MeshSize > meshingParameters.MaxH) meshSize = meshingParameters.MaxH;
            //else if (meshRefinement.MeshSize < meshingParameters.MinH) meshSize = meshingParameters.MinH;
            //
            mesh.GetMeshRefinementCoor(ids, meshSize, out coor);
            if (highlightNodes) HighlightNodes(coor, useSecondaryHighlightColor);
            //
            backfaceCulling = part.PartType != PartType.Shell;
            //
            HighlightItemsByGeometryIds(ids, backfaceCulling, useSecondaryHighlightColor);
        }
        public void HighlightShellInMeshSetupItem(MeshSetupItem meshSetupItem, bool useSecondaryHighlightColor = false)
        {
            int[] ids = meshSetupItem.CreationIds;
            if (ids.Length == 0) return;
            // The selection is limited to one part
            int partId = FeMesh.GetPartIdFromGeometryId(ids[0]);
            BasePart part = (GeometryPart)_model.Geometry.GetPartFromId(partId);
            if (part == null) return;
            //
            bool backfaceCulling = part.PartType != PartType.Shell;
            //
            HighlightItemsByGeometryIds(ids, backfaceCulling, false);
        }
        public void HighlightExtrudeMesh(ExtrudeMesh extrudeMesh, bool useSecondaryHighlightColor = false)
        {
            HighlightShellInMeshSetupItem(extrudeMesh, useSecondaryHighlightColor);
            //
            string prefixName = extrudeMesh.Name + Globals.NameSeparator + "Arrows";
            if (extrudeMesh.ExtrudeCenter != null && extrudeMesh.Direction != null)
            {
                int symbolSize = 2 * _settings.Pre.SymbolSize;
                DrawArrowSymbols(prefixName, new double[][] { extrudeMesh.ExtrudeCenter },
                                 new double[][] { extrudeMesh.Direction }, Color.Empty, symbolSize,
                                 vtkRendererLayer.Selection, !useSecondaryHighlightColor);
            }
        }
        public void HighlightRevolveMesh(RevolveMesh revolveMesh, bool useSecondaryHighlightColor = false)
        {
            HighlightShellInMeshSetupItem(revolveMesh, useSecondaryHighlightColor);
            //
            string prefixName = revolveMesh.Name + Globals.NameSeparator + "Arrows";
            if (revolveMesh.AxisCenter != null && revolveMesh.AxisDirection != null)
            {
                int symbolSize = 2 * _settings.Pre.SymbolSize;
                DrawArrowSymbols(prefixName, new double[][] { revolveMesh.AxisCenter },
                                 new double[][] { revolveMesh.AxisDirection }, Color.Empty, symbolSize,
                                 vtkRendererLayer.Selection, !useSecondaryHighlightColor, true);
            }
        }
        //
        public void HighlightNodes(double[][] nodeCoor, bool useSecondaryHighlightColor = false)
        {
            int nodeSize = _settings.Pre.NodeSymbolSize;
            Color color = Color.Red;
            vtkRendererLayer layer = vtkRendererLayer.Selection;
            DrawNodes("Highlight", nodeCoor, color, layer, nodeSize, false, useSecondaryHighlightColor);
        }
        public void HighlightNodeSets(string[] nodeSetsToSelect, bool useSecondaryHighlightColor = false)
        {
            Color color = Color.Red;
            vtkRendererLayer layer = vtkRendererLayer.Selection;
            int nodeSize = _settings.Pre.NodeSymbolSize;
            foreach (var nodeSetName in nodeSetsToSelect)
            {
                DrawNodeSet("Highlight", nodeSetName, color, layer, true, nodeSize, useSecondaryHighlightColor);
            }
        }
        //
        public void HighlightNode(int nodeId)
        {
            DrawNodes("Highlight", new int[] { nodeId }, Color.Red, vtkRendererLayer.Selection);
        }
        public void HighlightElement(int elementId)
        {
            DrawElements("Highlight", new int[] { elementId }, Color.Red, vtkRendererLayer.Selection);
        }        
        public int HighlightElementSets(string[] elementSetsToSelect, bool backfaceCulling = true)
        {
            int count = 0;
            FeElementSet elementSet;
            OrderedDictionary<string, FeElementSet> elementSets = null;
            foreach (var elementSetName in elementSetsToSelect)
            {
                if (_currentView == ViewGeometryModelResults.Model)
                    elementSets = _model.Mesh.ElementSets;
                else if (_currentView == ViewGeometryModelResults.Results)
                    elementSets = _allResults.CurrentResult.Mesh.ElementSets;
                //
                if (elementSets != null && elementSets.TryGetValue(elementSetName, out elementSet))
                {
                    count += DrawElementSet("Highlight", elementSet, Color.Red, vtkRendererLayer.Selection, backfaceCulling);
                    // Draw nodes
                    if (elementSet.Name.StartsWith(Globals.MissingSectionName))
                    {
                        HashSet<int> nodeIds = new HashSet<int>();
                        foreach (var elementId in elementSets[elementSetName].Labels)
                        {
                            nodeIds.UnionWith(_model.Mesh.Elements[elementId].NodeIds);
                        }
                        DrawNodes("Highlight", nodeIds.ToArray(), Color.Red, vtkRendererLayer.Selection);
                    }
                }
                
            }
            return count;
        }
        
        public void HighlightSurface(int[][] cells, ElementFaceType[] elementFaceTypes, bool useSecondaryHighlightColor)
        {
            FeMesh mesh = DisplayedMesh;
            Color color = Color.Red;
            vtkRendererLayer layer = vtkRendererLayer.Selection;
            // Copy
            int[][] cellsCopy = new int[cells.Length][];
            for (int i = 0; i < cells.Length; i++) cellsCopy[i] = cells[i].ToArray();
            // Faces
            vtkMaxActorData data = new vtkMaxActorData();
            data.Name = "highlight_surface_by_cells";
            data.Color = color;
            data.Layer = layer;
            data.CanHaveElementEdges = true;
            data.BackfaceCulling = true;
            data.DrawOnGeometry = true;
            data.UseSecondaryHighightColor = useSecondaryHighlightColor;
            data.Geometry.Cells.CellNodeIds = cells;
            mesh.GetSurfaceGeometry(cells, out data.Geometry.Nodes.Ids, out data.Geometry.Nodes.Coor, out data.Geometry.Cells.Types);
            //
            ApplyLighting(data);
            _form.Add3DCells(data);
            // Edges
            cells = mesh.GetFreeEdgesFromVisualizationCells(cellsCopy, elementFaceTypes);
            //
            data = new vtkMaxActorData();
            data.Name = "highlight_surface_edges_by_cells";
            data.Color = color;
            data.Layer = layer;
            data.CanHaveElementEdges = true;
            data.BackfaceCulling = true;
            data.UseSecondaryHighightColor = useSecondaryHighlightColor;
            data.Geometry.Cells.CellNodeIds = cells;
            mesh.GetSurfaceEdgesGeometry(cells, out data.Geometry.Nodes.Ids, out data.Geometry.Nodes.Coor, 
                                         out data.Geometry.Cells.Types);
            //
            ApplyLighting(data);
            _form.Add3DCells(data);
        }
        public void HighlightSurfaces(string[] surfacesToSelect, bool useSecondaryHighlightColor = false)
        {
            Color color = Color.Red;
            vtkRendererLayer layer = vtkRendererLayer.Selection;
            //
            foreach (var surfaceName in surfacesToSelect)
            {
                DrawSurface("Highlight-Surface", surfaceName, color, layer, true, useSecondaryHighlightColor);
                DrawSurfaceEdge("Highlight-SurfaceEdges", surfaceName, color, layer, true, useSecondaryHighlightColor);
            }
        }
        public void HighlightReferencePoints(string[] referencePointsToSelect)
        {
            Color color = Color.Red;
            vtkRendererLayer layer = vtkRendererLayer.Selection;
            //
            FeReferencePoint rp;
            foreach (var name in referencePointsToSelect)
            {
                if (_model.Mesh.ReferencePoints.TryGetValue(name, out rp))  DrawReferencePoint(rp, color, layer);
            }
        }
        public void HighlightConstraints(string[] constraintsToSelect)
        {
            Constraint constraint;
            int symbolSize = _settings.Pre.SymbolSize;
            int nodeSize = _settings.Pre.NodeSymbolSize;
            //
            foreach (var constraintName in constraintsToSelect)
            {
                constraint = _model.Constraints[constraintName];
                //
                if (constraint is PointSpring || constraint is SurfaceSpring || constraint is CompressionOnly ||
                    constraint is RigidBody || constraint is Tie)
                {
                    DrawConstraint(constraint, Color.Red, Color.Red, symbolSize, nodeSize, vtkRendererLayer.Selection, false);
                }
                else throw new NotSupportedException();
            }
        }
        public void HighlightContactPairs(string[] contactPairsToSelect)
        {
            ContactPair contactPair;
            foreach (var contactPairName in contactPairsToSelect)
            {
                if (_model.ContactPairs.TryGetValue(contactPairName, out contactPair))
                {
                    DrawContactPair(contactPair, Color.Red, Color.Red, vtkRendererLayer.Selection, false);
                }
            }
        }
        public void HighlightBoundaryCondition(BoundaryCondition boundaryCondition)
        {
            Step step = _model.StepCollection.GetBoundaryConditionStep(boundaryCondition);
            if (step != null) _form.SelectOneStepInSymbolsForStepList(step.Name);
            //
            int symbolSize = _settings.Pre.SymbolSize;
            int nodeSymbolSize = 2 * _settings.Pre.NodeSymbolSize;
            DrawBoundaryCondition("Step-Highlight", boundaryCondition, Color.Red, symbolSize,
                                  nodeSymbolSize, vtkRendererLayer.Selection, false);
        }
        public void HighlightLoad(Load load)
        {
            Step step = _model.StepCollection.GetLoadStep(load);
            if (step != null) _form.SelectOneStepInSymbolsForStepList(step.Name);
            //
            int symbolSize = _settings.Pre.SymbolSize;
            int nodeSymbolSize = 2 * _settings.Pre.NodeSymbolSize;
            DrawLoad("Highlight", load, Color.Red, symbolSize, nodeSymbolSize, vtkRendererLayer.Selection, false);
        }
        public void HighlightConnectedLines(double[][] lineNodeCoor)
        {
            // Create wire elements
            Color color = Color.Red;
            vtkRendererLayer layer = vtkRendererLayer.Selection;
            //
            LinearBeamElement element = new LinearBeamElement(0, new int[] { 0, 1 });
            //
            int[][] cells = new int[lineNodeCoor.GetLength(0) - 1][];
            int[] cellsTypes = new int[cells.GetLength(0)];
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                cells[i] = new int[] { i, i + 1 };
                cellsTypes[i] = element.GetVtkCellType();
            }
            //
            vtkMaxActorData data = new vtkMaxActorData();
            data.Color = color;
            data.Layer = layer;
            data.Pickable = false;
            data.Geometry.Nodes.Ids = null;
            data.Geometry.Nodes.Coor = lineNodeCoor.ToArray();
            data.Geometry.Cells.CellNodeIds = cells;
            data.Geometry.Cells.Types = cellsTypes;
            //
            ApplyLighting(data);
            _form.Add3DCells(data);
            //
            double[][] nodeCoor = new double[2][];
            nodeCoor[0] = lineNodeCoor[0];
            nodeCoor[1] = lineNodeCoor[lineNodeCoor.Length - 1];
            //
            //DrawNodes("short_edges", nodeCoor, color, layer, nodeSize);
        }
        public void HighlightConnectedEdges(double[][][] lineNodeCoor, bool drawNodes = true)
        {
            // Using HighlightConnectedLines is slow since invalidate is called each time
            // Create wire elements
            vtkRendererLayer layer = vtkRendererLayer.Selection;
            //
            int elementVtkCellType = new LinearBeamElement(0, new int[] { 0, 1 }).GetVtkCellType();
            //
            int n = 0;
            for (int i = 0; i < lineNodeCoor.Length; i++) n += lineNodeCoor[i].Length - 1;
            //
            int[][] cells = new int[n][];
            int[] cellsTypes = new int[cells.GetLength(0)];
            List<double[]> nodeCoor = new List<double[]>();
            //
            int countCells = 0;
            int countNodeIds = 0;
            for (int i = 0; i < lineNodeCoor.Length; i++)                       // lines
            {
                for (int j = 0; j < lineNodeCoor[i].Length - 1; j++)            // cells
                {
                    cells[countCells] = new int[] { countNodeIds, countNodeIds + 1 };
                    cellsTypes[countCells] = elementVtkCellType;
                    countCells++;
                    countNodeIds++;
                }
                countNodeIds++;                                                 // next line
                nodeCoor.AddRange(lineNodeCoor[i]);
            }
            //
            vtkMaxActorData data = new vtkMaxActorData();
            data.Layer = layer;
            data.Pickable = false;
            data.Geometry.Nodes.Ids = null;
            data.Geometry.Nodes.Coor = nodeCoor.ToArray();
            data.Geometry.Cells.CellNodeIds = cells;
            data.Geometry.Cells.Types = cellsTypes;
            //
            ApplyLighting(data);
            _form.Add3DCells(data);
            //
            nodeCoor.Clear();
            //
            if (drawNodes)
            {
                for (int i = 0; i < lineNodeCoor.Length; i++)                   // lines
                {
                    nodeCoor.Add(lineNodeCoor[i][0]);
                    nodeCoor.Add(lineNodeCoor[i][lineNodeCoor[i].Length - 1]);
                }
                HighlightNodes(nodeCoor.ToArray());
            }
        }
        //
        public void HighlightSelection(bool clear = true, bool backfaceCulling = true, bool useSecondaryHighlightColor = false)
        {
            if (clear) _form.Clear3DSelection();
            int[] ids = GetSelectionIds();
            if (ids.Length == 0) return;
            //
            if (_selection.SelectItem == vtkSelectItem.Node)
                HighlightItemsByNodeIds(ids, useSecondaryHighlightColor);
            else if (_selection.SelectItem == vtkSelectItem.Element)
                HighlightItemsByElementIds(ids);
            else if (_selection.SelectItem == vtkSelectItem.GeometryEdge)   // QueryEdge
                HighlightItemsByGeometryEdgeIds(ids, useSecondaryHighlightColor);
            else if (_selection.SelectItem == vtkSelectItem.Surface)
                HighlightItemsBySurfaceIds(ids, useSecondaryHighlightColor);
            else if (_selection.SelectItem == vtkSelectItem.Geometry ||
                     _selection.SelectItem == vtkSelectItem.GeometrySurface)
                HighlightItemsByGeometryIds(ids, backfaceCulling, useSecondaryHighlightColor);
            else if (_selection.SelectItem == vtkSelectItem.Part)
            {
                string[] partNames = DisplayedMesh.GetPartNamesFromPartIds(ids);
                //
                if (_currentView == ViewGeometryModelResults.Geometry) HighlightGeometryParts(partNames);
                else if (_currentView == ViewGeometryModelResults.Model) HighlightModelParts(partNames);
                return;
            }
            else throw new NotSupportedException();
        }
        private void HighlightItemsByNodeIds(int[] ids, bool useSecondaryHighlightColor)
        {
            vtkMaxActorData data = GetNodeActorData(ids);
            data.NodeSize = _settings.Pre.NodeSymbolSize;
            data.Layer = vtkRendererLayer.Selection;
            data.UseSecondaryHighightColor = useSecondaryHighlightColor;
            ApplyLighting(data);
            _form.Add3DNodes(data);
        }
        private void HighlightItemsByElementIds(int[] ids)
        {
            DrawElements("Highlight", ids, Color.Red, vtkRendererLayer.Selection);
        }
        public void HighlightItemsByGeometryEdgeIds(int[] ids, bool useSecondaryHighlightColor)   
        {
            // QueryEdge from frmQuery
            vtkMaxActorData data = GetGeometryEdgeActorData(ids);
            data.UseSecondaryHighightColor = useSecondaryHighlightColor;
            HighlightActorData(data);
        }
        public void HighlightItemsBySurfaceIds(int[] ids, bool useSecondaryHighlightColor)
        {
            int[][] cells;
            ElementFaceType[] elementFaceTypes;
            // QuerySurface from frmQuery
            if (ids.Length == 1 && DisplayedMesh.IsThisIdGeometryId(ids[0]))
                cells = GetSurfaceCellsByGeometryId(ids, out elementFaceTypes);
            else cells = GetSurfaceCellsByFaceIds(ids, out elementFaceTypes);
            //
            HighlightSurface(cells, elementFaceTypes, useSecondaryHighlightColor);
        }
        private void HighlightItemsByGeometryIds(int[] ids, bool backfaceCulling, bool useSecondaryHighlightColor)
        {
            int nodeSize = _settings.Pre.NodeSymbolSize + 2;
            DrawItemsByGeometryIds(ids, "highlight", "items", Color.Empty, vtkRendererLayer.Selection, nodeSize,
                                   backfaceCulling, useSecondaryHighlightColor);
        }
        public void HighlightActorData(vtkMaxActorData aData)
        {
            aData.Layer = vtkRendererLayer.Selection;
            aData.CanHaveElementEdges = false;
            ApplyLighting(aData);
            _form.Add3DCells(aData);
        }
        #endregion #################################################################################################################

        #region Results  ###########################################################################################################
        public void DrawResults(bool resetCamera)
        {
            bool rendering = _form.RenderingOn;
            try
            {
                // Set the current view and call DrawResults
                if (_currentView != ViewGeometryModelResults.Results) CurrentView = ViewGeometryModelResults.Results;
                // Draw results
                else
                {
                    if (rendering) _form.RenderingOn = false;
                    _form.Clear3D();    // Removes section cut
                    //
                    if (_allResults.CurrentResult == null || _allResults.CurrentResult.Mesh == null) return;
                    if (_allResults.CurrentResult.GetAllComponentNames().Length == 0)
                        _viewResultsType = ViewResultsTypeEnum.Undeformed;
                    //
                    ApplyResultsUnitSystem();
                    // Settings - must be here before drawing parts to correctly set the numer of colors
                    SetPostLegendAndStatusBlockSettings();
                    AnnotateWithColorLegend();
                    //
                    float scale = GetScale();
                    SetStatusBlock(scale);
                    //
                    _allResults.CurrentResult.SetMeshDeformation(scale, _currentFieldData.StepId,
                                                                 _currentFieldData.StepIncrementId);
                    DrawAllResultParts(_currentFieldData, _settings.Post.UndeformedModelType,
                                       _settings.Post.UndeformedModelColor);
                    // Transformation
                    ApplyTransformation();
                    // Annotations
                    _annotations.DrawAnnotations();
                    // Section view
                    Octree.Plane plane = _sectionViews.GetCurrentSectionViewPlane();
                    if (plane != null) ApplySectionView(plane.Point.Coor, plane.Normal.Coor);
                    //
                    UpdateHighlight();
                    //
                    if (resetCamera) _form.SetFrontBackView(true, true); // animation:true is here to correctly draw max/min widgets 
                    //
                    _form.UpdateScalarsAndCameraAndRedraw();
                }
            }
            catch
            {
                // Do not throw an error - it might cancel a procedure
            }
            finally
            {
                if (rendering) _form.RenderingOn = true;
            }
        }
        private void DrawAllResultParts(FieldData fieldData, UndeformedModelTypeEnum undeformedModelType,
                                        Color undeformedModelColor)
        {
            vtkRendererLayer layer = vtkRendererLayer.Base;
            List<string> hiddenActors = new List<string>();
            //
            _form.InitializeResultWidgetPositions(); // reset the widget position after setting the status block content
            //
            foreach (var entry in _allResults.CurrentResult.Mesh.Parts)
            {
                if (entry.Value is ResultPart resultPart)
                {
                    if (_viewResultsType == ViewResultsTypeEnum.Undeformed)
                    {
                        // Undeformed view
                        DrawModelPart(_allResults.CurrentResult.Mesh, resultPart, layer);
                    }
                    else
                    {
                        // Undeformed copy
                        if (undeformedModelType != UndeformedModelTypeEnum.None)
                            DrawUndeformedPartCopy(resultPart, undeformedModelType, undeformedModelColor, layer);
                        // Deformed
                        DrawResultPart(resultPart, fieldData, false);
                    }
                }
                // Draw geometry parts copied to the results
                else if (entry.Value is GeometryPart)
                {
                    // Pickable for the Section view to work
                    DrawGeomPart(_allResults.CurrentResult.Mesh, entry.Value, layer, false, true);
                }
                //
                if (!entry.Value.Visible) hiddenActors.Add(entry.Key);
            }
            if (hiddenActors.Count > 0) _form.HideActors(hiddenActors.ToArray(), true);
        }
        private void DrawResultPart(ResultPart part, FieldData fieldData, bool update)
        {
            vtkMaxActorData data = GetResultPartActorData(part, fieldData);
            //
            if (data != null)
            {
                ApplyLighting(data);
                _form.AddScalarFieldOn3DCells(data, update);
            }
        }
        private vtkMaxActorData GetResultPartActorData(ResultPart part, FieldData fieldData)
        {
            if (part.Labels.Length == 0) return null;
            // Get visualization nodes and renumbered elements           
            PartExchangeData actorResultData = _allResults.CurrentResult.GetVisualizationNodesCellsAndValues(part, fieldData);
            // Model edges
            PartExchangeData modelEdgesResultData = null;
            if (part.PartType.HasEdges() && part.Visualization.EdgeCells != null)
            {
                modelEdgesResultData = _allResults.CurrentResult.GetEdgesNodesAndCells(part, fieldData);
            }
            // Get all needed nodes and elements - renumbered               
            PartExchangeData locatorResultData = _allResults.CurrentResult.GetSetNodesCellsAndValues(part, fieldData);
            //
            vtkMaxActorData data = GetVtkData(actorResultData, modelEdgesResultData, locatorResultData);
            data.Name = part.Name;
            GetPartColor(part, ref data.Color, ref data.BackfaceColor);
            data.ColorContours = part.ColorContours;
            data.CanHaveElementEdges = true;
            data.Pickable = true;
            data.SmoothShaded = part.SmoothShaded;
            data.ActorRepresentation = GetRepresentation(part);
            data.NodeSize = Globals.BeamNodeSize;
            // Back face                                                    
            if (part.PartType == PartType.Shell) data.BackfaceCulling = false;
            //
            return data;
        }
        // Animation
        public bool DrawScaleFactorAnimation(int numFrames)
        {
            _form.Clear3D();
            //
            if (_allResults.CurrentResult == null || _allResults.CurrentResult.Mesh == null) return false;
            if (_allResults.CurrentResult.GetAllComponentNames().Length == 0) _viewResultsType = ViewResultsTypeEnum.Undeformed;
            //
            ApplyResultsUnitSystem();
            // Settings - must be here before drawing parts to correctly set the numer of colors
            float scale = GetScale();
            SetPostLegendAndStatusBlockSettings();
            SetStatusBlock(scale);
            //
            vtkMaxActorData data;
            vtkRendererLayer layer = vtkRendererLayer.Base;
            //
            bool result = true;
            PostSettings postSettings = _settings.Post;
            List<string> hiddenActors = new List<string>();
            double[] allFramesScalarRange = new double[] { double.MaxValue, -double.MaxValue };
            foreach (var entry in _allResults.CurrentResult.Mesh.Parts)
            {
                if (entry.Value is ResultPart resultPart)
                {
                    // Undeformed
                    if (postSettings.UndeformedModelType != UndeformedModelTypeEnum.None)
                        DrawUndeformedPartCopy(resultPart, postSettings.UndeformedModelType,
                                               postSettings.UndeformedModelColor, layer);
                    // Deformed
                    data = GetScaleFactorAnimationDataFromPart(resultPart, _currentFieldData, scale, numFrames);
                    // Min max
                    if (entry.Value.Visible)
                    {
                        foreach (NodesExchangeData nData in data.Geometry.ExtremeNodesAnimation)
                        {
                            if (nData != null)
                            {
                                if (nData.Values[0] < allFramesScalarRange[0]) allFramesScalarRange[0] = nData.Values[0];
                                if (nData.Values[1] > allFramesScalarRange[1]) allFramesScalarRange[1] = nData.Values[1];
                            }
                        }
                    }
                    //
                    ApplyLighting(data);
                    result = _form.AddAnimatedScalarFieldOn3DCells(data);                    
                    if (result == false) {_form.Clear3D(); return false;}
                }
                else if (entry.Value is GeometryPart)
                {
                    // For the Section view to work: pickable = true 
                    DrawGeomPart(_allResults.CurrentResult.Mesh, entry.Value, layer, false, true);
                }
                if (!entry.Value.Visible) hiddenActors.Add(entry.Key);
            }
            if (hiddenActors.Count > 0) _form.HideActors(hiddenActors.ToArray(), true);
            // Transformation
            ApplyTransformation();
            // Annotations
            _annotations.DrawAnnotations(true);
            // Section view
            Octree.Plane plane = _sectionViews.GetCurrentSectionViewPlane();
            if (plane != null) ApplySectionView(plane.Point.Coor, plane.Normal.Coor);
            // Animation field data
            float[] time = new float[numFrames];
            int[] stepId = new int[numFrames];
            int[] stepIncrementId = new int[numFrames];
            float[] animationScale = new float[numFrames];
            float ratio = 1f / (numFrames - 1);
            for (int i = 0; i < numFrames; i++)
            {
                time[i] = _currentFieldData.Time;
                stepId[i] = _currentFieldData.StepId;
                stepIncrementId[i] = _currentFieldData.StepIncrementId;
                animationScale[i] = i * ratio;
            }
            //
             _form.SetAnimationFrameData(time, stepId, stepIncrementId, animationScale, allFramesScalarRange,
                                         vtkMaxAnimationType.ScaleFactor);
            //
            return result;
        }
        public bool DrawTimeIncrementAnimation(out int numFrames)
        {
            _form.Clear3D();
            //
            numFrames = -1;
            if (_allResults.CurrentResult == null || _allResults.CurrentResult.Mesh == null) return false;
            if (_allResults.CurrentResult.GetAllComponentNames().Length == 0) _viewResultsType = ViewResultsTypeEnum.Undeformed;
            //
            ApplyResultsUnitSystem();
            // Settings - must be here before drawing parts to correctly set the numer of colors
            float scale = GetScaleForAllStepsAndIncrements();
            SetPostLegendAndStatusBlockSettings();
            SetStatusBlock(scale);
            //
            vtkMaxActorData data = null;
            vtkRendererLayer layer = vtkRendererLayer.Base;
            //
            bool result = true;
            PostSettings postSettings = _settings.Post;
            List<string> hiddenActors = new List<string>();
            double[] allFramesScalarRange = new double[] { double.MaxValue, -double.MaxValue };
            foreach (var entry in _allResults.CurrentResult.Mesh.Parts)
            {
                if (entry.Value is ResultPart resultPart)
                {
                    // Undeformed shape
                    if (postSettings.UndeformedModelType != UndeformedModelTypeEnum.None)
                        DrawUndeformedPartCopy(resultPart, postSettings.UndeformedModelType,
                                               postSettings.UndeformedModelColor, layer);
                    // Results
                    data = GetTimeIncrementAnimationDataFromPart(resultPart, _currentFieldData, scale);
                    // Min max
                    if (entry.Value.Visible)
                    {
                        foreach (NodesExchangeData nData in data.Geometry.ExtremeNodesAnimation)
                        {
                            if (nData.Values[0] < allFramesScalarRange[0]) allFramesScalarRange[0] = nData.Values[0];
                            if (nData.Values[1] > allFramesScalarRange[1]) allFramesScalarRange[1] = nData.Values[1];
                        }
                    }
                    //
                    ApplyLighting(data);
                    result = _form.AddAnimatedScalarFieldOn3DCells(data);
                    if (result == false) { _form.Clear3D(); return false; }
                }
                else if (entry.Value is GeometryPart)
                {
                    // For the Section view to work: pickable = true 
                    DrawGeomPart(_allResults.CurrentResult.Mesh, entry.Value, layer, false, true);
                }
                if (!entry.Value.Visible) hiddenActors.Add(entry.Key);
            }
            if (hiddenActors.Count > 0) _form.HideActors(hiddenActors.ToArray(), true);
            // Transformation
            ApplyTransformation();
            // Annotations
            _annotations.DrawAnnotations(true);
            // Section view
            Octree.Plane plane = _sectionViews.GetCurrentSectionViewPlane();
            if (plane != null) ApplySectionView(plane.Point.Coor, plane.Normal.Coor);
            // Animation field data
            var existingIncrements =
                _allResults.CurrentResult.GetExistingIncrementIds(_currentFieldData.Name, _currentFieldData.Component);
            List<float> time = new List<float>();
            List<int> stepId = new List<int>();
            List<int> stepIncrementId = new List<int>();
            List<float> animationScale = new List<float>();
            foreach (var entry in existingIncrements)
            {
                for (int i = 0; i < entry.Value.Length; i++)
                {
                    time.Add(_allResults.CurrentResult.GetIncrementTime(entry.Key, entry.Value[i]));
                    stepId.Add(entry.Key);
                    stepIncrementId.Add(entry.Value[i]);
                    animationScale.Add(-1);
                }
            }
            _form.SetAnimationFrameData(time.ToArray(), stepId.ToArray(), stepIncrementId.ToArray(), animationScale.ToArray(),
                                        allFramesScalarRange, vtkMaxAnimationType.TimeIncrements);
            //
            numFrames = data.Geometry.NodesAnimation.Length;
            //
            return result;
        }
        public bool DrawHarmonicAnimation(int numFrames)
        {
            if (_allResults.CurrentResult == null || _allResults.CurrentResult.Mesh == null) return false;
            if (_allResults.CurrentResult.GetAllComponentNames().Length == 0) _viewResultsType = ViewResultsTypeEnum.Undeformed;
            //
            float scale = GetScale();
            // Prepare data - first prepare data, than clear the vtk data
            float delta = 360f / numFrames; // skip the final angle which is the same as the first angle
            float[] angles = new float[numFrames];
            for (int i = 0; i < numFrames; i++) angles[i] = i * delta;
            //
            vtkMaxActorData frameData;
            vtkMaxActorData existingData;
            Dictionary<ResultPart, vtkMaxActorData> partData = new Dictionary<ResultPart, vtkMaxActorData>();
            for (int i = 0; i < angles.Length; i++)
            {
                CurrentResult.SetComplexResultTypeAndAngle(ComplexResultTypeEnum.RealAtAngle, angles[i], _currentFieldData);
                //
                _allResults.CurrentResult.SetMeshDeformation(scale, _currentFieldData.StepId,
                                                             _currentFieldData.StepIncrementId);
                //
                foreach (var entry in _allResults.CurrentResult.Mesh.Parts)
                {
                    if (entry.Value is ResultPart resultPart)
                    {
                        frameData = GetResultPartActorData(resultPart, _currentFieldData);
                        //
                        if (i == 0)
                        {
                            frameData.Geometry.NodesAnimation = new NodesExchangeData[numFrames];
                            frameData.Geometry.ExtremeNodesAnimation = new NodesExchangeData[numFrames];
                            frameData.CellLocator.NodesAnimation = new NodesExchangeData[numFrames];
                            frameData.ModelEdges.NodesAnimation = new NodesExchangeData[numFrames];
                            //
                            frameData.Geometry.NodesAnimation[i] = frameData.Geometry.Nodes;
                            frameData.Geometry.ExtremeNodesAnimation[i] = frameData.Geometry.ExtremeNodes;
                            frameData.CellLocator.NodesAnimation[i] = frameData.CellLocator.Nodes;
                            frameData.ModelEdges.NodesAnimation[i] = frameData.ModelEdges.Nodes;
                            //
                            partData.Add(resultPart, frameData);
                        }
                        else
                        {
                            if (partData.TryGetValue(resultPart, out existingData))
                            {
                                existingData.Geometry.NodesAnimation[i] = frameData.Geometry.Nodes;
                                existingData.Geometry.ExtremeNodesAnimation[i] = frameData.Geometry.ExtremeNodes;
                                existingData.CellLocator.NodesAnimation[i] = frameData.CellLocator.Nodes;
                                existingData.ModelEdges.NodesAnimation[i] = frameData.ModelEdges.Nodes;
                            }
                        }
                    }
                }
            }
            // Start drawing
            _form.Clear3D();
            //
            ApplyResultsUnitSystem();
            // Settings - must be here before drawing parts to correctly set the numer of colors
            SetPostLegendAndStatusBlockSettings();
            SetStatusBlock(scale);
            //
            vtkMaxActorData data;
            vtkRendererLayer layer = vtkRendererLayer.Base;
            //
            bool result = true;
            PostSettings postSettings = _settings.Post;
            List<string> hiddenActors = new List<string>();
            double[] allFramesScalarRange = new double[] { double.MaxValue, -double.MaxValue };
            //
            foreach (var entry in _allResults.CurrentResult.Mesh.Parts)
            {
                if (entry.Value is ResultPart resultPart)
                {
                    // Undeformed
                    if (postSettings.UndeformedModelType != UndeformedModelTypeEnum.None)
                        DrawUndeformedPartCopy(resultPart, postSettings.UndeformedModelType,
                                               postSettings.UndeformedModelColor, layer);
                    // Deformed
                    data = partData[resultPart];
                    // Min max
                    if (entry.Value.Visible)
                    {
                        foreach (NodesExchangeData nData in data.Geometry.ExtremeNodesAnimation)
                        {
                            if (nData != null)
                            {
                                if (nData.Values[0] < allFramesScalarRange[0]) allFramesScalarRange[0] = nData.Values[0];
                                if (nData.Values[1] > allFramesScalarRange[1]) allFramesScalarRange[1] = nData.Values[1];
                            }
                        }
                    }
                    //
                    ApplyLighting(data);
                    result = _form.AddAnimatedScalarFieldOn3DCells(data);
                    if (result == false) { _form.Clear3D(); return false; }
                }
                else if (entry.Value is GeometryPart)
                {
                    // For the Section view to work: pickable = true 
                    DrawGeomPart(_allResults.CurrentResult.Mesh, entry.Value, layer, false, true);
                }
                if (!entry.Value.Visible) hiddenActors.Add(entry.Key);
            }
            if (hiddenActors.Count > 0) _form.HideActors(hiddenActors.ToArray(), true);
            // Transformation
            ApplyTransformation();
            // Annotations
            _annotations.DrawAnnotations(true);
            // Section view
            Octree.Plane plane = _sectionViews.GetCurrentSectionViewPlane();
            if (plane != null) ApplySectionView(plane.Point.Coor, plane.Normal.Coor);
            // Animation field data
            float[] time = new float[numFrames];
            int[] stepId = new int[numFrames];
            int[] stepIncrementId = new int[numFrames];
            float[] animationScale = angles;
            for (int i = 0; i < numFrames; i++)
            {
                time[i] = _currentFieldData.Time;
                stepId[i] = _currentFieldData.StepId;
                stepIncrementId[i] = _currentFieldData.StepIncrementId;
            }
            //
            _form.SetAnimationFrameData(time, stepId, stepIncrementId, animationScale, allFramesScalarRange,
                                        vtkMaxAnimationType.Harmonic);
            //
            return result;
        }
        private vtkMaxActorData GetScaleFactorAnimationDataFromPart(ResultPart part, FieldData fieldData,
                                                                    float scale, int numFrames)
        {
            // Get visualization nodes and renumbered elements
            PartExchangeData modelResultData;
            PartExchangeData modelEdgesResultData;
            PartExchangeData locatorResultData;
            _allResults.CurrentResult.GetScaleFactorAnimationData(part, fieldData, scale, numFrames,
                                                                  out modelResultData, out modelEdgesResultData,
                                                                  out locatorResultData);
            //
            vtkMaxActorData data = GetVtkData(modelResultData, modelEdgesResultData, locatorResultData);
            data.Name = part.Name;
            GetPartColor(part, ref data.Color, ref data.BackfaceColor);
            data.ColorContours = part.ColorContours;
            data.CanHaveElementEdges = true;
            data.Pickable = false;
            data.SmoothShaded = part.SmoothShaded;
            data.ActorRepresentation = GetRepresentation(part);
            data.NodeSize = Globals.BeamNodeSize;
            // Back face
            if (part.PartType == PartType.Shell) data.BackfaceCulling = false;
            //
            return data;
        }
        private vtkMaxActorData GetTimeIncrementAnimationDataFromPart(ResultPart part, FieldData fieldData,
                                                                      float scale)
        {
            // Get visualization nodes and renumbered elements
            PartExchangeData modelResultData;
            PartExchangeData modelEdgesResultData;
            PartExchangeData locatorResultData;
            _allResults.CurrentResult.GetTimeIncrementAnimationData(part, fieldData, scale,
                                                                    out modelResultData, out modelEdgesResultData,
                                                                    out locatorResultData);
            //
            vtkMaxActorData data = GetVtkData(modelResultData, modelEdgesResultData, locatorResultData);
            data.Name = part.Name;
            GetPartColor(part, ref data.Color, ref data.BackfaceColor);
            data.ColorContours = part.ColorContours;
            data.CanHaveElementEdges = true;
            data.Pickable = false;
            data.SmoothShaded = part.SmoothShaded;
            data.ActorRepresentation = GetRepresentation(part);
            data.NodeSize = Globals.BeamNodeSize;
            // Back face
            if (part.PartType == PartType.Shell) data.BackfaceCulling = false;
            //
            return data;
        }
        // Common
        private void SetPostLegendAndStatusBlockSettings()
        {
            if (_allResults.CurrentResult != null)
            {
                _allResults.CurrentResult.DeformationFieldOutputName = _form.GetDeformationVariable();
                _allResults.CurrentResult.SetComplexResultTypeAndAngle(_form.GetComplexResultType(),
                                                                       (float)_form.GetComplexAngleDeg());
                UpdateCurrentFieldData();
            }
            //
            if (_viewResultsType == ViewResultsTypeEnum.ColorContours)
            {
                LegendSettings legendSettings = Settings.Legend;    // use Settings property to account for the results view
                StatusBlockSettings statusBlockSettings = _settings.StatusBlock;
                // Legend settings
                _form.SetScalarBarColorSpectrum(legendSettings.ColorSpectrum);
                string complexComponent;
                Field field = _allResults.CurrentResult.GetField(_currentFieldData);
                if (field != null && field.Complex)
                {
                    ComplexResultTypeEnum resultType = _form.GetComplexResultType();
                    complexComponent = resultType.GetDisplayedName();
                    if (resultType == ComplexResultTypeEnum.RealAtAngle)
                    {
                        complexComponent += " " + _form.GetComplexAngleDeg() + " " +
                                            StringAngleDegConverter.GetUnitAbbreviation();
                    }
                }
                else complexComponent = null;
                //
                _form.SetScalarBarText(_currentFieldData.Name, _currentFieldData.Component,
                                       GetCurrentResultsUnitAbbreviation(),
                                       complexComponent,
                                       legendSettings.ColorSpectrum.MinMaxType.ToString());
                //
                _form.SetScalarBarNumberFormat(legendSettings.GetColorChartNumberFormat());
                _form.DrawLegendBackground(legendSettings.BackgroundType == AnnotationBackgroundType.White);
                _form.DrawLegendBorder(legendSettings.DrawBorder);
                // Status block
                _form.SetStatusBlockVisibility(statusBlockSettings.Visible);
                _form.DrawStatusBlockBackground(statusBlockSettings.BackgroundType == AnnotationBackgroundType.White);
                _form.DrawStatusBlockBorder(statusBlockSettings.DrawBorder);
                // Limits
                //_form.SetShowMinValueLocation(postSettings.ShowMinValueLocation);
                //_form.SetShowMaxValueLocation(postSettings.ShowMaxValueLocation);
            }
        }
        private void SetStatusBlock(float scale)
        {
            string unit;
            if (_currentFieldData.StepType == StepTypeEnum.Static)
                unit = _allResults.CurrentResult.UnitSystem.TimeUnitAbbreviation;
            else if (_currentFieldData.StepType == StepTypeEnum.Frequency)
                unit = _allResults.CurrentResult.UnitSystem.FrequencyUnitAbbreviation;
            else if (_currentFieldData.StepType == StepTypeEnum.FrequencySensitivity)
                unit = _allResults.CurrentResult.UnitSystem.FrequencyUnitAbbreviation;
            else if (_currentFieldData.StepType == StepTypeEnum.Buckling)
                unit = "";
            else if (_currentFieldData.StepType == StepTypeEnum.SteadyStateDynamics)
                unit = _allResults.CurrentResult.UnitSystem.FrequencyUnitAbbreviation;
            else if (_currentFieldData.StepType == StepTypeEnum.LastIterations)
                unit = _allResults.CurrentResult.UnitSystem.TimeUnitAbbreviation;
            else throw new NotSupportedException();
            // Deformation variable
            string deformationVariable = _form.GetDeformationVariable();
            //
            vtkMaxFieldDataType fieldType = ConvertStepType(_currentFieldData);
            //
            int stepNumber = _currentFieldData.StepId;
            int incrementNumber = _currentFieldData.StepIncrementId;
            //
            _form.SetStatusBlock(Path.GetFileName(_allResults.CurrentResult.FileName), _allResults.CurrentResult.DateTime,
                                 _currentFieldData.Time, unit, deformationVariable, scale, fieldType, stepNumber, incrementNumber);
        }
        private vtkMaxFieldDataType ConvertStepType(FieldData fieldData)
        {
            vtkMaxFieldDataType fieldType;
            if (fieldData.StepType == StepTypeEnum.Static) fieldType = vtkMaxFieldDataType.Static;
            else if (fieldData.StepType == StepTypeEnum.Frequency) fieldType = vtkMaxFieldDataType.Frequency;
            else if (fieldData.StepType == StepTypeEnum.FrequencySensitivity) fieldType = vtkMaxFieldDataType.FrequencySensitivity;
            else if (fieldData.StepType == StepTypeEnum.Buckling) fieldType = vtkMaxFieldDataType.Buckling;
            else if (fieldData.StepType == StepTypeEnum.SteadyStateDynamics) fieldType = vtkMaxFieldDataType.SteadyStateDynamic;
            else if (fieldData.StepType == StepTypeEnum.LastIterations) fieldType = vtkMaxFieldDataType.LastIterations;
            else throw new NotSupportedException();
            return fieldType;
        }
        public void UpdatePartsScalarFields()
        {
            if (_allResults.CurrentResult == null || _allResults.CurrentResult.Mesh == null) return;
            // Settings                                                              
            SetPostLegendAndStatusBlockSettings();
            //
            Octree.Plane plane = _sectionViews.GetCurrentSectionViewPlane();
            if (plane != null) RemoveSectionView();
            //
            foreach (var entry in _allResults.CurrentResult.Mesh.Parts)
            {
                if (entry.Value is ResultPart)
                {
                    // Get all needed nodes and elements - renumbered
                    PartExchangeData locatorResultData =
                        _allResults.CurrentResult.GetSetNodesCellsAndValues(entry.Value, _currentFieldData);
                    // Get visualization nodes and renumbered elements - to scale min nad max nodes coor
                    PartExchangeData actorResultData =
                        _allResults.CurrentResult.GetVisualizationNodesCellsAndValues(entry.Value, _currentFieldData);
                    //
                    _form.UpdateActorSurfaceScalarField(entry.Key, actorResultData.Nodes.Values, actorResultData.ExtremeNodes,
                                                        locatorResultData.Nodes.Values, false);
                }
            }
            // Annotations
            _annotations.DrawAnnotations();
            //
            if (plane != null) ApplySectionView(plane.Point.Coor, plane.Normal.Coor);
            //
            _form.UpdateScalarsAndRedraw();
        }
        public void DrawUndeformedPartCopy(BasePart part, UndeformedModelTypeEnum undeformedModelType,
                                           Color color, vtkRendererLayer layer)
        {
            vtkMaxActorData data;
            data = new vtkMaxActorData();
            data.Name = part.Name + "_undeformed";
            data.Color = color;
            data.Layer = layer;
            data.CanHaveElementEdges = false;
            data.SmoothShaded = part.SmoothShaded;
            //
            if (undeformedModelType == UndeformedModelTypeEnum.WireframeBody)
            {
                if (data.Color.A == 255) data.Color = Color.FromArgb(254, data.Color);
                _allResults.CurrentResult.GetUndeformedModelEdges(part, out data.Geometry.Nodes.Coor,
                                                                  out data.Geometry.Cells.CellNodeIds,
                                                                  out data.Geometry.Cells.Types);
            }
            else if(undeformedModelType == UndeformedModelTypeEnum.SolidBody)
            {
                _allResults.CurrentResult.GetUndeformedNodesAndCells(part, out data.Geometry.Nodes.Coor,
                                                                     out data.Geometry.Cells.CellNodeIds,
                                                                     out data.Geometry.Cells.Types);
            }
            //
            ApplyLighting(data);
            _form.Add3DCells(data);
        }        
        private void ApplyTransformation()
        {
            List<Transformation> transformations = _transformations.GetCurrentTransformations();
            if (transformations != null && transformations.Count >= 1)
            {
                foreach (var transformation in transformations)
                {
                    if (transformation is Symmetry sym)
                    {
                        _form.AddSymmetry((int)sym.SymmetryPlane, sym.PointCoor);
                    }
                    else if (transformation is LinearPattern lp)
                    {
                        _form.AddLinearPattern(lp.Displacement, lp.NumberOfItems);
                    }
                    else if (transformation is CircularPattern cp)
                    {
                        _form.AddCircularPattern(cp.AxisFirstPoint, cp.AxisNormal, cp.Angle, cp.NumberOfItems);
                    }
                    else throw new NotSupportedException();
                }
                _form.ApplyTransformations();
            }
        }
        //
        private vtkMaxActorData GetVtkData(PartExchangeData actorData, PartExchangeData modelEdgesData,
                                           PartExchangeData locatorData)
        {
            vtkMaxActorData vtkData = new vtkMaxActorData();
            
            vtkData.Geometry = actorData;
            vtkData.ModelEdges = modelEdgesData;
            vtkData.CellLocator = locatorData;

            return vtkData;
        }
        // Scale 
        public FeNode GetScaledNode(float scale, int nodeId)
        {
            return GetScaledNode(FOFieldNames.Default, scale, nodeId);
        }
        public FeNode GetScaledNode(string deformationFieldOutputName, float scale, int nodeId)
        {
            if (_currentView == ViewGeometryModelResults.Results)
            {
                FeNode node = _allResults.CurrentResult.UndeformedNodes[nodeId];
                double[][] coor = new double[][] { node.Coor };
                _allResults.CurrentResult.ScaleNodeCoordinates(deformationFieldOutputName, scale, _currentFieldData.StepId,
                                                               _currentFieldData.StepIncrementId,
                                                               new int[] { nodeId }, ref coor);
                node.X = coor[0][0];
                node.Y = coor[0][1];
                node.Z = coor[0][2];
                // Exploded view
                if (IsExplodedViewActive())
                {
                    int[] partIds = _allResults.CurrentResult.Mesh.GetPartIdsFromNodeIds(new int[] { nodeId });
                    if (partIds != null && partIds.Length == 1)
                    {
                        BasePart part = _allResults.CurrentResult.Mesh.GetPartFromId(partIds[0]);
                        node.X += part.Offset[0];
                        node.Y += part.Offset[1];
                        node.Z += part.Offset[2];
                    }
                }
                //
                return node;
            }
            return new FeNode();
        }
        public FeNode[] GetScaledNodes(float scale, int[] nodeIds)
        {
            return GetScaledNodes(FOFieldNames.Default, scale, nodeIds);
        }
        public FeNode[] GetScaledNodes(string deformationFieldOutputName, float scale, int[] nodeIds)
        {
            if (_currentView == ViewGeometryModelResults.Results)
            {
                double[][] coor = new double[nodeIds.Length][];
                for (int i = 0; i < nodeIds.Length; i++) coor[i] = _allResults.CurrentResult.UndeformedNodes[nodeIds[i]].Coor;
                //
                _allResults.CurrentResult.ScaleNodeCoordinates(deformationFieldOutputName, scale, _currentFieldData.StepId,
                                                               _currentFieldData.StepIncrementId, nodeIds, ref coor);
                //
                FeNode[] nodes = new FeNode[nodeIds.Length];
                Dictionary<int, int> globalToLocalNodeIds = new Dictionary<int, int>();
                for (int i = 0; i < nodes.Length; i++)
                {
                    nodes[i] = new FeNode(nodeIds[i], coor[i]);
                    globalToLocalNodeIds.Add(nodeIds[i], i);
                }
                // Exploded view
                if (IsExplodedViewActive())
                {
                    HashSet<int> nodeIdsHash = new HashSet<int>(nodeIds);
                    HashSet<int> commonNodes;
                    foreach (var entry in _allResults.CurrentResult.Mesh.Parts)
                    {
                        commonNodes = new HashSet<int>(nodeIdsHash.Intersect(entry.Value.NodeLabels));
                        if (commonNodes.Count() > 0)
                        {
                            foreach (var nodeId in commonNodes)
                            {
                                nodes[globalToLocalNodeIds[nodeId]].X += entry.Value.Offset[0];
                                nodes[globalToLocalNodeIds[nodeId]].Y += entry.Value.Offset[1];
                                nodes[globalToLocalNodeIds[nodeId]].Z += entry.Value.Offset[2];
                            }
                            nodeIdsHash.ExceptWith(commonNodes);
                        }
                        if (nodeIdsHash.Count() == 0) break;
                    }
                }
                //
                return nodes;
            }
            return new FeNode[0];
        }
        public float GetNodalValue(int nodeId)
        {
            float[] values = _allResults.CurrentResult.GetValues(_currentFieldData, new int[] { nodeId });
            if (values == null) return 0;
            else return values[0];
        }
        #endregion #################################################################################################################

        // Tools
        public float GetScale()
        {
            if (_allResults != null && _allResults.CurrentResult != null)
            {
                float maxDisplacement = _allResults.CurrentResult.GetMaxDeformation(_currentFieldData.StepId,
                                                                                    _currentFieldData.StepIncrementId);
                return GetScale(maxDisplacement);
            }
            else return 1;
        }
        public float GetScaleForAllStepsAndIncrements()
        {
            float maxDisplacement = _allResults.CurrentResult.GetMaxDeformation();
            return GetScale(maxDisplacement);
        }
        private float GetScale(float maxDisplacement)
        {
            if (_viewResultsType == ViewResultsTypeEnum.Undeformed) return 0;
            //
            DeformationScaleFactorTypeEnum scaleFactorType = _form.GetDeformationType();
            if (scaleFactorType == DeformationScaleFactorTypeEnum.Undeformed) return 0;
            else if (scaleFactorType == DeformationScaleFactorTypeEnum.TrueScale) return 1;
            else
            {
                float scale = 1;
                float automaticScale = scaleFactorType.GetAutomaticFactor();
                // Automatic
                if (_allResults.CurrentResult != null && _allResults.CurrentResult.Mesh != null && automaticScale != -1)
                {
                    float size;
                    // 2D
                    if (_allResults.CurrentResult.Mesh.BoundingBox.Is2D())
                        size = (float)_allResults.CurrentResult.Mesh.GetBoundingBoxAreaAsSquareSide();
                    // 3D
                    else size = (float)_allResults.CurrentResult.Mesh.GetBoundingBoxVolumeAsCubeSide();
                    //
                    if (maxDisplacement == -float.MaxValue) scale = 0;  // the displacement filed does not exist
                    else if (maxDisplacement != 0) scale = automaticScale * (size * 0.25f / maxDisplacement);
                    // Round
                    scale = (float)Tools.RoundToSignificantDigits(scale, 2);
                }
                // User defined
                else scale = _form.GetDeformationFactor();
                //
                return scale;
            }
        }
        //
        public void TestCreateSurface()
        {
            int surfaceId = 1;
            int surfaceType = (int)GeometryType.SolidSurface;
            int partId = 1;
            int geometryId = FeMesh.GetGeometryId(surfaceId, surfaceType, + partId);
            int[] faceIds = _model.Mesh.GetIdsFromGeometryIds(new int[] { geometryId }, vtkSelectItem.Surface);
            //
            FeSurface surface = new FeSurface(_model.Mesh.Surfaces.GetNextNumberedKey("UserSurface"));
            surface.CreatedFrom = FeSurfaceCreatedFrom.Faces;
            surface.FaceIds = faceIds;
            AddSurface(surface);
        }
    }
















}

