using System;
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

namespace PrePoMax
{
    [Serializable]
    public class Controller
    {
        // Variables                                                                                                                
        [NonSerialized]
        protected FrmMain _form;
        [NonSerialized]
        protected Dictionary<string, ISettings> _settings;
        //
        protected bool _modelChanged;
        protected bool _savingFile;
        protected FeModel _model;
        protected Dictionary<string, AnalysisJob> _jobs;
        protected NetgenJob _netgenJob;
        protected FeResults _results;
        protected HistoryResults _history;
        // View
        protected ViewGeometryModelResults _currentView;
        protected string _drawSymbolsForStep;
        // Selection
        protected vtkSelectBy _selectBy;
        protected double _selectAngle;
        protected Selection _selection;
        // Results
        protected ViewResultsType _viewResultsType;
        protected FieldData _currentFieldData;
        // History
        protected Commands.CommandsCollection _commands;


        // Properties                                                                                                               
        public Dictionary<string, ISettings> Settings
        {
            get { return _settings; }
            set
            {
                try
                {
                    _settings = value;
                    _settings.DumpToFile(Path.Combine(System.Windows.Forms.Application.StartupPath, Globals.SettingsFileName));

                    ApplySettings();

                    // Redraw model with new settings
                    if (_currentView == ViewGeometryModelResults.Geometry) DrawGeometry(false);
                    else if (_currentView == ViewGeometryModelResults.Model) DrawMesh(false);
                    else DrawResults(false);
                }
                catch
                { }
            }
        }
        //
        public bool ModelChanged { get { return _modelChanged; } set { _modelChanged = value; } }
        public bool SavingFile { get { return _savingFile; } }
        public FeModel Model { get { return _model; } }
        public Dictionary<string, AnalysisJob> Jobs { get { return _jobs; } }
        public bool MeshJobIdle
        {
            get
            {
                if (_netgenJob != null && _netgenJob.JobStatus == JobStatus.Running) return false;
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
                    _form.SetCurrentView(_currentView);
                    if (_currentView == ViewGeometryModelResults.Geometry) DrawGeometry(false);
                    else if (_currentView == ViewGeometryModelResults.Model) DrawMesh(false);
                    else if (_currentView == ViewGeometryModelResults.Results) DrawResults(false);
                    else throw new NotSupportedException();
                }
            }
        }
        public string DrawSymbolsForStep
        {
            get { return _drawSymbolsForStep; }
            set
            {
                if (value != _drawSymbolsForStep)
                {
                    _drawSymbolsForStep = value;
                    RedrawSymbols();
                }
            }
        }
        // Selection
        public vtkSelectItem SelectItem 
        {
            get { return _selection.SelectItem; }
            set
            {
                //if (_selection.SelectItem != value) - default value was not set correctly
                {
                    _selection.SelectItem = value;
                    _form.SetSelectItem(value);
                }
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
        public FeResults Results { get { return _results; } }
        public HistoryResults History { get { return _history; } }
        public ViewResultsType ViewResultsType
        {
            get { return _viewResultsType; }
            set
            {
                //if (_currentView == ViewGeometryModelResults.Results)
                {
                    _viewResultsType = value;

                    // this is used by the model tree to show hide the Deformed and Color contour context menu lines
                    ResultPart.Undeformed = _viewResultsType == ViewResultsType.Undeformed;

                    if (_results != null && _results.Mesh != null)
                    {
                        foreach (var entry in _results.Mesh.Parts)
                        {
                            if (entry.Value is ResultPart resultPart) resultPart.ColorContours = _viewResultsType == ViewResultsType.ColorContours;
                        }

                        DrawResults(false);
                    }
                }
            }
        }
        public FieldData CurrentFieldData
        {
            get { return _currentFieldData; }
            set
            {
                _currentFieldData = value;
                _currentFieldData.Time = _results.GetIncrementTime(_currentFieldData.Name, _currentFieldData.StepId, _currentFieldData.StepIncrementId);
            }
        }
        // History
        public string GetHistoryFileName()
        {
            return _commands.HistoryFileNameTxt;
        }
        //
        public String OpenedFileName
        {
            get
            {
                return ((GeneralSettings)_settings[Globals.GeneralSettingsName]).LastFileName;
            }
            set
            {
                if (_settings != null)
                {
                    GeneralSettings gs = (GeneralSettings)_settings[Globals.GeneralSettingsName];
                    if (value != gs.LastFileName)
                    {
                        gs.LastFileName = value;
                        _settings.DumpToFile(Path.Combine(System.Windows.Forms.Application.StartupPath, Globals.SettingsFileName));
                    }

                    if (gs.LastFileName != null) _form.SetTitle(Globals.ProgramName + "   " + gs.LastFileName);
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
                else if (_currentView == ViewGeometryModelResults.Results) return _results.Mesh;
                else throw new NotSupportedException();
            }
        }


        // Setters                                                                                                                  
        public void SetSelectionOff()
        {
            SelectBy = vtkSelectBy.Off;
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

        public void SetSelectBy(vtkSelectBy selectBy)
        {
            SelectBy = selectBy;
        }
        public void SetSelectAngle(double angle)
        {
            SelectAngle = angle;
        }


        // Constructors                                                                                                             
        public Controller(FrmMain form)
        {
            _form = form;
            _form.Controller = this;
            _commands = new Commands.CommandsCollection(this);
            _commands.WriteOutput = _form.WriteDataToOutput;
            _commands.ModelChanged_ResetJobStatus = ResetAllJobStatus;
            _commands.EnableDisableUndoRedo += _commands_CommandExecuted;
            _commands.OnEnableDisableUndoRedo();

            _jobs = new Dictionary<string, AnalysisJob>();
            _selection = new Selection();

            Clear();

            try
            {
                string fileName = Path.Combine(System.Windows.Forms.Application.StartupPath, Globals.SettingsFileName);
                if (File.Exists(fileName))
                {
                    var t = Task.Run(() => _settings = CaeGlobals.Tools.LoadDumpFromFile<Dictionary<string, ISettings>>(fileName));
                    t.Wait();
                }
                else PrepareSettings();
            }
            catch
            {
                PrepareSettings();
            }
            
            ApplySettings();

            ViewResultsType = ViewResultsType.ColorContours;
        }

        void _commands_CommandExecuted(string undo, string redo)
        {
            _form.EnableDisableUndoRedo(undo, redo);
        }
        private void PrepareSettings()
        {
            _settings = new Dictionary<string, ISettings>();
            _settings.Add(Globals.GeneralSettingsName, new GeneralSettings());
            _settings.Add(Globals.GraphicsSettingsName, new GraphicsSettings());
            _settings.Add(Globals.PreSettingsName, new PreSettings());
            _settings.Add(Globals.CalculixSettingsName, new CalculixSettings());
            _settings.Add(Globals.PostSettingsName, new PostSettings());
        }

        #region Clear   ############################################################################################################
        // COMMANDS ********************************************************************************
        public void ClearCommand()
        {
            Commands.CClear comm = new Commands.CClear();
            _commands.AddAndExecute(comm);
        }

        //******************************************************************************************
        public void Clear()
        {
            _form.SetTitle(Globals.ProgramName);
            OpenedFileName = null;
            _savingFile = false;
            //_currentView = ViewGeometryModelResults.Geometry;

            if (_form != null)
            {
                _form.ClearControls();
                _form.SetCurrentView(_currentView);
            }

            ClearModel();
            ClearResults();
        }
        public void ClearModel()
        {
            _model = new FeModel("Model-1");
            _drawSymbolsForStep = null;
            _jobs.Clear();
            ClearAllSelection();
            
            _modelChanged = false;
        }
        public void ClearResults()
        {
            if (_results != null || _history != null)
            {
                _modelChanged = true;
                _results = null;
                _history = null;
            }
            _currentFieldData = null;
            ClearAllSelection();
        }
        public void ClearSelectionHistory()
        {
            _selection.Clear();
            _form.Clear3DSelection();            
        }
        public void ClearAllSelection()
        {
            _selection.Clear();
            _form.Clear3DSelection();
            _form.ClearTreeSelection();
        }
        #endregion ################################################################################################################

        // Menus
        #region File menu   ########################################################################################################
        // COMMANDS ********************************************************************************
        public void ImportFileCommand(string fileName)
        {
            Commands.CImportFile comm = new Commands.CImportFile(fileName);
            _commands.AddAndExecute(comm);
        }

        //******************************************************************************************
        
        public void New()
        {
            // Add and execute the clear command
            _commands.Clear();      // also calls _modelChanged = false;
            ClearCommand();         // also calls _modelChanged = false;

            //OpenedFileName = null;
        }
        public void Open(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();

            if (extension == ".pmx") OpenPmx(fileName);
            else if (extension == ".frd") OpenFrd(fileName);
            else if (extension == ".dat") OpenDat(fileName);
            else throw new NotSupportedException();
            
            // Get first component of the first field for the last increment in the last step
            if (_results != null) _currentFieldData = _results.GetFirstFieldAtLastIncrement();
        }
        private void OpenPmx(string fileName)
        {
            Clear();

            OpenedFileName = fileName;

            Controller tmp = null;
            object[] data = null;

            data = TryReadCompressedPmx(fileName, out _model, out _results);
            if (data != null && data.Length == 1 && (string)data[0] == "UncompatibleVersion") return;
            if (data == null) data = TryReadUncompressedPmx(fileName, out _model, out _results);
            if (data == null || data.Length < 3)
                throw new Exception("The file can not be read. It is either corrupt or was created by a previous version.");

            tmp = (Controller)data[0];

            _history = tmp.History;

            _commands.EnableDisableUndoRedo -= _commands_CommandExecuted;
            _commands = new Commands.CommandsCollection(this, tmp._commands); // to recreate the history file
            _commands.WriteOutput = _form.WriteDataToOutput;
            _commands.ModelChanged_ResetJobStatus = ResetAllJobStatus;
            _commands.EnableDisableUndoRedo += _commands_CommandExecuted;
            _commands.OnEnableDisableUndoRedo();

            _jobs = (Dictionary<string, AnalysisJob>)data[1];

            ApplySettings(); // work folder and executable
            ResetAllJobStatus();

            _currentView = ViewGeometryModelResults.Geometry;
            if (_model != null && _model.Mesh != null) _currentView = ViewGeometryModelResults.Model;
            else if (_results != null) _currentView = ViewGeometryModelResults.Results;

            _form.RegenerateTree(_model, _jobs, _results, _history);

            //_form.SetTreeExpandCollapseState((bool[])data[2]);

            _form.SetCurrentView(_currentView); // at the end
        }
        private void OpenFrd(string fileName)
        {
            ClearResults();
            _results = CaeResults.FrdFileReader.Read(fileName);

            if (_results == null)
            {
                MessageBox.Show("The results file does not exist or is empty.", "Error");
                return;
            }
            else
            {
                _form.Clear3D();

                // check if the meshes are the same and rename the parts
                if (_model.Mesh != null && _results.Mesh != null && _model.Mesh.IsEqual(_results.Mesh))
                {
                    //_results.Mesh.SetPartPropertiesFromMesh(_model.Mesh);
                    //_results.Mesh = .CopyPartsFromMesh(_model.Mesh);
                    _results.CopyPartsFromMesh(_model.Mesh);
                }

                _currentView = ViewGeometryModelResults.Results;     // do not draw
                _form.SetCurrentView(_currentView);

                _form.RegenerateTree(_model, _jobs, _results, _history);

                _modelChanged = true;
            }

            string datFileName = Path.GetFileNameWithoutExtension(fileName) + ".dat";
            datFileName = Path.Combine(Path.GetDirectoryName(fileName), datFileName);
            if (File.Exists(datFileName)) OpenDat(datFileName);
        }

        private void OpenDat(string fileName)
        {
            _history = CaeResults.DatFileReader.Read(fileName);

            if (_history == null)
            {
                MessageBox.Show("The dat file does not exist or is empty.", "Error");
                return;
            }
            else
            {
                _form.RegenerateTree(_model, _jobs, _results, _history);

                _modelChanged = true;
            }
        }

        private object[] TryReadCompressedPmx(string fileName, out FeModel model, out FeResults results)
        {
            try
            {
                string fileVersion;
                object[] data = null;
                Controller tmp = null;
                byte[] versionBuffer = new byte[32];
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    fs.Read(versionBuffer, 0, 32);
                    fileVersion = Encoding.ASCII.GetString(versionBuffer).TrimEnd(new char[] { '\0' });

                    if (fileVersion != Globals.ProgramName)
                    {
                        MessageBox.Show("The selected file is from an uncompatible version: " + fileVersion, "Error", MessageBoxButtons.OK);
                        throw new Exception("UncompatibleVersion");
                    }

                    using (BinaryReader br = new BinaryReader(Decompress(fs)))
                    {
                        data = CaeGlobals.Tools.LoadDumpFromFile<object[]>(br);
                        tmp = (Controller)data[0];
                        model = tmp._model;
                        results = tmp._results;

                        FeModel.ReadFromFile(model, br);
                        FeResults.ReadFromFile(results, br);
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                model = null;
                results = null;
                if (ex.Message == "UncompatibleVersion") return new object[] { ex.Message };
                else return null;
            }
        }
        private object[] TryReadUncompressedPmx(string fileName, out FeModel model, out FeResults results)
        {
            try
            {
                object[] data = null;
                Controller tmp = null;

                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                using (BinaryReader br = new BinaryReader(fs))
                {
                    data = CaeGlobals.Tools.LoadDumpFromFile<object[]>(br);
                    tmp = (Controller)data[0];
                    model = tmp._model;
                    results = tmp._results;

                    FeModel.ReadFromFile(model, br);
                    FeResults.ReadFromFile(results, br);
                }
                return data;
            }
            catch
            {
                model = null;
                results = null;
                return null;
            }
        }

        public string GetFileNameToImport()
        {
            return _form.GetFileNameToImport();
        }
        public async Task ImportFileAsync(string fileName)
        {
           //     Clear(); // clear all
           //     // Add the Clear() to the command queue and execute
           //     // to clear the model on regenerate; false to not clear the commands or regenerate
           //     ClearCommand(false);
           
            await Task.Run(() => ImportFileCommand(fileName));
        }
        public void ImportFile(string fileName)
        {
            if (!File.Exists(fileName)) throw new FileNotFoundException("The file: '" + fileName + "' does not exist.");

            string extension = Path.GetExtension(fileName).ToLower();

            // Import
            if (extension == ".stl")
            {
                if (!_model.ImportGeometryFromStlFile(fileName) || _model.Geometry.ManifoldGeometry)
                {
                    string message = "There are errors in the imported geometry.";
                    if (_model.Geometry.ManifoldGeometry) message += Environment.NewLine + "The geometry is manifold.";
                    UserControls.AutoClosingMessageBox.Show(message, "Error", 3000);
                }
            }
            else if (extension == ".stp" || extension == ".step")
                ImportStepAssemblyFile(fileName);
            else if (extension == ".igs" || extension == ".iges")
                ImportIgesAssemblyFile(fileName);
            else if (extension == ".unv")
                _model.ImportMeshFromUnvFile(fileName);
            else if (extension == ".vol")
                _model.ImportMeshFromVolFile(fileName);
            else if (extension == ".inp")
            {
                List<string> errors = _model.ImportMeshFromInpFile(fileName, _form.WriteDataToOutput);
                if (errors.Count > 0)
                {
                    _form.WriteDataToOutput("");
                    foreach (var line in errors) _form.WriteDataToOutput("Error: " + line);
                    string message = "There were " + errors.Count + " errors while importing the file.";
                    _form.WriteDataToOutput(message);
                    UserControls.AutoClosingMessageBox.Show(message, "Error", 3000);
                }
                CheckAndUpdateValidity();
            }

            // Visualization
            if (extension == ".stl" || extension == ".stp" || extension == ".step" || extension == ".igs" || extension == ".iges")
            {
                _currentView = ViewGeometryModelResults.Geometry;
                _form.SetCurrentView(_currentView);
                DrawGeometry(false);
            }
            else if (extension == ".unv" || extension == ".vol" || extension == ".inp")
            {
                _currentView = ViewGeometryModelResults.Model;
                _form.SetCurrentView(_currentView);
                DrawMesh(false);
            }

            // Regenerate
            _form.RegenerateTree(_model, _jobs, _results, _history);
        }
        public void ImportStepAssemblyFile(string fileName)
        {
            string[] filesToImport = SplitStepAssemblyToBrepParts(fileName);

            if (filesToImport != null)
            {
                foreach (var stepPartFileName in filesToImport)
                {
                    ImportBrepPartFile(stepPartFileName);

                    if (File.Exists(stepPartFileName)) File.Delete(stepPartFileName);
                }
            }
        }
        public void ImportIgesAssemblyFile(string fileName)
        {
            string[] filesToImport = SplitIgesAssemblyToBrepParts(fileName);

            if (filesToImport != null)
            {
                foreach (var stepPartFileName in filesToImport)
                {
                    ImportBrepPartFile(stepPartFileName);

                    if (File.Exists(stepPartFileName)) File.Delete(stepPartFileName);
                }
            }
        }
        public bool ImportBrepPartFile(string brepFileName)
        {
            CalculixSettings calculixSettings = (CalculixSettings)Settings[Globals.CalculixSettingsName];
            GraphicsSettings graphicsSettings = (GraphicsSettings)Settings[Globals.GraphicsSettingsName];

            if (calculixSettings.WorkDirectory == null || !Directory.Exists(calculixSettings.WorkDirectory))
            {
                MessageBox.Show("The work directory does not exist.", "Error", MessageBoxButtons.OK);
                return false;
            }

            string executable = Application.StartupPath + Globals.NetGenMesher;
            string visFileName = Path.Combine(calculixSettings.WorkDirectory, Globals.VisFileName);

            if (File.Exists(visFileName)) File.Delete(visFileName);

            string argument = "BREP_VISUALIZATION " +
                              "\"" + brepFileName + "\" " +
                              "\"" + visFileName + "\" " +
                              graphicsSettings.GeometryDeflection.ToString();

            _netgenJob = new NetgenJob("Brep", executable, argument, calculixSettings.WorkDirectory);
            _netgenJob.AppendOutput += netgenJobStepImport_AppendOutput;
            //_form.SetStateWorking("Importing step...", true);
            _netgenJob.Submit();

            if (_netgenJob.JobStatus == JobStatus.OK)
            {
                //_form.SetStateReady("Importing step...");

                string error = _model.ImportGeometryFromBrepFile(visFileName, brepFileName);

                if (error != null)
                {
                    MessageBox.Show(error, "Error", MessageBoxButtons.OK);
                    return false;
                }
                return true;
            }
            else
            {
                MessageBox.Show("Importing step file failed.", "Error", MessageBoxButtons.OK);
                return false;
            }
        }
        public string[] SplitStepAssemblyToBrepParts(string stepFileName)
        {
            CalculixSettings settings = (CalculixSettings)Settings[Globals.CalculixSettingsName];
            if (settings.WorkDirectory == null || !Directory.Exists(settings.WorkDirectory))
            {
                MessageBox.Show("The work directory does not exist.", "Error", MessageBoxButtons.OK);
                return null;
            }

            string executable = Application.StartupPath + Globals.NetGenMesher;
            string outFileName = GetFreeRandomFileName(settings.WorkDirectory, ".brep");

            string argument = "STEP_ASSEMBLY_SPLIT " +
                              "\"" + stepFileName + "\" " +
                              "\"" + outFileName + "\"";

            _netgenJob = new NetgenJob("SplitStep", executable, argument, settings.WorkDirectory);
            _netgenJob.AppendOutput += netgenJobStepImport_AppendOutput;
            //_form.SetStateWorking("Importing step...", true);
            _netgenJob.Submit();

            string brepFile;
            List<string> brepFiles = new List<string>();
            string outFileNameNoExtension = Path.GetFileNameWithoutExtension(outFileName);

            if (_netgenJob.JobStatus == JobStatus.OK)
            {
                //_form.SetStateReady("Importing step...");

                string[] allFiles = Directory.GetFiles(settings.WorkDirectory);
                foreach (var fileName in allFiles)
                {
                    brepFile = Path.GetFileName(fileName);
                    if (brepFile.StartsWith(outFileNameNoExtension)) brepFiles.Add(fileName);
                }

                return brepFiles.ToArray();
            }
            else return null;
        }
        public string[] SplitIgesAssemblyToBrepParts(string igesFileName)
        {
            CalculixSettings settings = (CalculixSettings)Settings[Globals.CalculixSettingsName];
            if (settings.WorkDirectory == null || !Directory.Exists(settings.WorkDirectory))
            {
                MessageBox.Show("The work directory does not exist.", "Error", MessageBoxButtons.OK);
                return null;
            }

            string executable = Application.StartupPath + Globals.NetGenMesher;
            string outFileName = GetFreeRandomFileName(settings.WorkDirectory, ".brep");

            string argument = "IGES_ASSEMBLY_SPLIT " +
                              "\"" + igesFileName + "\" " +
                              "\"" + outFileName + "\"";

            _netgenJob = new NetgenJob("SplitIges", executable, argument, settings.WorkDirectory);
            _netgenJob.AppendOutput += netgenJobStepImport_AppendOutput;
            _netgenJob.Submit();

            string brepFile;
            List<string> brepFiles = new List<string>();
            string outFileNameNoExtension = Path.GetFileNameWithoutExtension(outFileName);

            if (_netgenJob.JobStatus == JobStatus.OK)
            {
                string[] allFiles = Directory.GetFiles(settings.WorkDirectory);
                foreach (var fileName in allFiles)
                {
                    brepFile = Path.GetFileName(fileName);
                    if (brepFile.StartsWith(outFileNameNoExtension)) brepFiles.Add(fileName);
                }

                return brepFiles.ToArray();
            }
            else return null;
        }
        private string GetFreeRandomFileName(string path, string extension)
        {
            string hash;
            bool repeate;
            string[] allFiles = Directory.GetFiles(path);

            do
            {
                hash = GetRandomString(8);

                repeate = false;
                foreach (var fileName in allFiles)
                {
                    if (fileName.StartsWith(hash))
                    {
                        repeate = true;
                        break;
                    }
                }
            }
            while (repeate);

            return Path.Combine(path, Path.ChangeExtension(hash, extension));
        }
        private string GetRandomString(int len)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[len];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            return new String(stringChars);
        }
        void netgenJobStepImport_AppendOutput(string data)
        {
            _form.WriteDataToOutput(data);
        }
        public void ImportGeneratedMesh(string fileName, string partName, bool resetCamera, bool fromBrep, 
                                        bool convertToSecondOrder)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException("The file: '" + fileName + "' does not exist." + Environment.NewLine +
                                                "The reason is a failed mesh generation procedure for part: " + partName);

            int[] removedPartId = RemoveModelParts(new string[] { partName }, false, true);

            _model.ImportGeneratedMeshFromVolFile(fileName, partName, convertToSecondOrder);
          
            // Regenerate and change the DisplayedMesh to Model before updating sets
            _form.Clear3D();
            _currentView = ViewGeometryModelResults.Model;
            _form.SetCurrentView(_currentView);

            // For geometry based sets the part id must remain the same after remesh
            if (removedPartId != null && removedPartId[0] != -1)                
            {
                if (_model.Mesh.Parts[partName].PartId != removedPartId[0])
                    _model.Mesh.RenumberPart(partName, removedPartId[0]);
                
                // Update sets
                UpdateSurfacesBasedOnGeometry();
            }

            // Shading
            if (fromBrep) _model.Mesh.Parts[partName].SmoothShaded = true;

            // Tree
            //Update(UpdateType.Check);
            _form.RegenerateTree(_model, _jobs, _results, _history);
            
            DrawMesh(resetCamera); // also draws symbols
        }

        public string GetFileNameToSaveAs()
        {
            return _form.GetFileNameToSaveAs();
        }
        public void Save()
        {
            if (OpenedFileName != null && Path.GetExtension(OpenedFileName) == ".pmx")
            {
                SaveToFile(OpenedFileName);
            }
            else SaveAs();
        }
        public void SaveAs()
        {
            string fileName = GetFileNameToSaveAs();
            if (fileName != null) SaveToFile(fileName);
        }
        public void SaveToFile(string fileName)
        {
            try
            {
                _savingFile = true;

                bool[] states = _form.GetTreeExpandCollapseState();
                OpenedFileName = fileName;
                object[] data = new object[] { this, _jobs, states };

                using (BinaryWriter bw = new BinaryWriter(new MemoryStream()))
                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
                    FeResults results = null;
                    HistoryResults history = null;
                    bool saveResults = ((GeneralSettings)Settings[Globals.GeneralSettingsName]).SaveResultsInPmx;

                    // when controller (data[0]) is dumped to stream, the results should be null if selected
                    if (saveResults == false)
                    {
                        results = _results;
                        _results = null;
                        history = _history;
                        _history = null;
                    }

                    // Controller
                    data.DumpToStream(bw);
                    // Model - data is saved inside data[0]._model but without mesh data - speed up
                    FeModel.WriteToFile(_model, bw);
                    // Results - data is saved inside data[0]._results but without mesh data - speed up
                    FeResults.WriteToFile(_results, bw);

                    // after dumping restore the results
                    if (saveResults == false)
                    {
                        _results = results;
                        _history = history;
                    }

                    bw.Flush();
                    bw.BaseStream.Position = 0;

                    byte[] compressedData = Compress(bw.BaseStream);

                    byte[] version = Encoding.ASCII.GetBytes(Globals.ProgramName);
                    byte[] versionBuffer = new byte[32];
                    version.CopyTo(versionBuffer, 0);

                    fs.Write(versionBuffer, 0, 32);
                    fs.Write(compressedData, 0, compressedData.Length);
                }

                _modelChanged = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _savingFile = false;
            }
        }
        public void ExportToCalculix(string fileName)
        {
            FileInOut.Output.CalculixFileWriter.Write(fileName, _model);
        }
        public void ExportToAbaqus(string fileName)
        {
            FileInOut.Output.AbaqusFileWriter.Write(fileName, _model);
        }

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

            using (var decompressor = new DeflateStream(input, CompressionMode.Decompress))
            {
                decompressor.CopyTo(output);
            }

            output.Position = 0;
            return output;
        }
        #endregion ################################################################################################################

        #region Edit menu   ########################################################################################################
        // COMMANDS ********************************************************************************
        public void SetCalculixUserKeywordsCommand(Dictionary<int[], FileInOut.Output.Calculix.CalculixUserKeyword> userKeywords)
        {
            Commands.CSetCalculixUserKeywords comm = new Commands.CSetCalculixUserKeywords(userKeywords);
            _commands.AddAndExecute(comm);
        }

        public void UndoHistory()
        {
            _commands.Undo();
        }
        public void RedoHistory()
        {
            _commands.Redo();
        }
        public void RegenerateHistoryCommands()
        {
            string lastFileName = OpenedFileName;
            RegenerateHistoryCommandsWithDialogs(false, false);
            OpenedFileName = lastFileName;
        }
        public void RegenerateHistoryCommandsWithDialogs(bool showImportDialog, bool showMeshParametersDialog)
        {
            //ViewGeometryMeshResults view = _currentView;
            string lastFileName = OpenedFileName;
            _commands.ExecuteAllCommands(showImportDialog, showMeshParametersDialog);
            OpenedFileName = lastFileName;
            //CurrentView = view;
        }

        public List<FileInOut.Output.Calculix.CalculixKeyword> GetCalculixModelKeywords()
        {
            if (_model == null)
            {
                MessageBox.Show("There is no model.", "Error", System.Windows.Forms.MessageBoxButtons.OK);
                return null;
            }
            else return FileInOut.Output.CalculixFileWriter.GetModelKeywords(_model);
        }
        public Dictionary<int[], FileInOut.Output.Calculix.CalculixUserKeyword> GetCalculixUserKeywords()
        {
            if (_model == null)
            {
                MessageBox.Show("There is no model.", "Error", System.Windows.Forms.MessageBoxButtons.OK);
                return null;
            }
            else return _model.CalculixUserKeywords;
        }
        public void SetCalculixUserKeywords(Dictionary<int[], FileInOut.Output.Calculix.CalculixUserKeyword> userKeywords)
        {
            _model.CalculixUserKeywords = userKeywords;
            _form.SetNumberOfModelUserKeywords(userKeywords.Count);
        }

        #endregion ################################################################################################################

        #region Geometry part menu   ###############################################################################################
        // COMMANDS ********************************************************************************
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
        public void ReplaceGeometryPartPropertiesCommand(string oldPartName, PartProperties newPartProperties)
        {
            Commands.CReplaceGeometryPartProperties comm = new Commands.CReplaceGeometryPartProperties(oldPartName, newPartProperties);
            _commands.AddAndExecute(comm);
        }
        public void RemoveGeometryPartsCommand(string[] partNames)
        {
            Commands.CRemoveGeometryParts comm = new Commands.CRemoveGeometryParts(partNames);
            _commands.AddAndExecute(comm);
        }

        //******************************************************************************************

        public string[] GetGeometryPartNames()
        {
            return _model.Geometry.Parts.Keys.ToArray();
        }
        public GeometryPart GetGeometryPart(string partName)
        {
            return (GeometryPart)_model.Geometry.Parts[partName];
        }
        public GeometryPart[] GetGeometryParts()
        {
            if (_model.Geometry == null) return null;

            int i = 0;
            GeometryPart[] parts = new GeometryPart[_model.Geometry.Parts.Count];
            foreach (var entry in _model.Geometry.Parts) parts[i++] = (GeometryPart)entry.Value;
            return parts;
        }
        public string[] GetGeometryPartNames<T>()
        {
            List<string> names = new List<string>();
            foreach (var entry in _model.Geometry.Parts)
            {
                if (entry.Value.Labels.Length > 0 && _model.Geometry.Elements[entry.Value.Labels[0]] is T)
                {
                    names.Add(entry.Key);
                }
            }
            return names.ToArray();
        }
        public void HideGeometryParts(string[] partNames)
        {
            foreach (var name in partNames)
            {
                _model.Geometry.Parts[name].Visible = false;
                _form.UpdateTreeNode(ViewGeometryModelResults.Geometry, name, _model.Geometry.Parts[name], null);
            }
            _form.HideActors(partNames, false);
        }
        public void ShowGeometryParts(string[] partNames)
        {
            foreach (var name in partNames)
            {
                _model.Geometry.Parts[name].Visible = true;
                _form.UpdateTreeNode(ViewGeometryModelResults.Geometry, name, _model.Geometry.Parts[name], null);
            }
            _form.ShowActors(partNames, false);
        }
        public void ReplaceGeometryPartProperties(string oldPartName, PartProperties newPartProperties)
        {
            // Replace geometry part
            GeometryPart geomPart = GetGeometryPart(oldPartName);
            _model.Geometry.Parts.Remove(oldPartName);
            geomPart.SetProperties(newPartProperties);
            _model.Geometry.Parts.Add(geomPart.Name, geomPart);
            _form.UpdateActor(oldPartName, geomPart.Name, geomPart.Color);
            _form.UpdateTreeNode(ViewGeometryModelResults.Geometry, oldPartName, geomPart, null);

            // Rename the mesh part in pair with geometry part
            if (oldPartName != geomPart.Name && _model.Mesh != null && _model.Mesh.Parts.ContainsKey(oldPartName))
            {
                string newPartName = geomPart.Name;
                MeshPart meshPart = GetModelPart(oldPartName);
                meshPart.Name = newPartName;
                _model.Mesh.Parts.Remove(oldPartName);
                _model.Mesh.Parts.Add(meshPart.Name, meshPart);
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldPartName, meshPart, null);
            }
        }
        public void CopyGeometryPartsToResults(string[] partNames)
        {
            if (_results != null && _results.Mesh != null)
            {
                string[] addedPartNames = _results.Mesh.AddPartsFromMesh(_model.Geometry, partNames);
                if (addedPartNames.Length > 0)
                {
                    _form.RegenerateTree(_model, _jobs, _results, _history);
                    CurrentView = ViewGeometryModelResults.Results;
                }
            }
            _modelChanged = true;
        }
        public void RemoveGeometryParts(string[] partNames)
        {
            string[] removedParts;
            _model.Geometry.RemoveParts(partNames, out removedParts, false);

            ViewGeometryModelResults view = ViewGeometryModelResults.Geometry;
            foreach (var name in removedParts) _form.RemoveTreeNode<GeometryPart>(view, name, null);
            
            DrawGeometry(false);
        }

        // Analyze geometry ***********************************************************************

        public double GetShortestEdgeLen()
        {
            return DisplayedMesh.GetShortestEdgeLen();
        }
        public void ShowShortEdges(double minEdgeLen)
        {
            //ClearAllSelection();
            double[][][] edgeNodeCoor = DisplayedMesh.GetShortEdges(minEdgeLen);
            HighlightConnectedEdges(edgeNodeCoor, 7);
        }
        public double GetSmallestFace()
        {
            return DisplayedMesh.GetSmallestFace();
        }
        public void ShowSmallFaces(double minFaceArea)
        {
            //ClearAllSelection();
            FeMesh mesh = DisplayedMesh;
            int[][] cells = mesh.GetSmallestFaces(minFaceArea);
            HighlightSurface(cells);
        }

        #endregion #################################################################################################################

        #region Meshing   ##########################################################################################################
        // COMMANDS ********************************************************************************
        public void SetMeshingParametersCommand(string partName, MeshingParameters meshingParameters)
        {
            Commands.CSetMeshingParameters comm = new Commands.CSetMeshingParameters(partName, meshingParameters);
            _commands.AddAndExecute(comm);
        }
        public void CreateMeshCommand(string partName)
        {
            Commands.CCreateMesh comm = new Commands.CCreateMesh(partName);
            _commands.AddAndExecute(comm);
        }

        //******************************************************************************************
        public MeshingParameters GetDefaultMeshingParameters(string partName)
        {
            return _form.GetDefaultMeshingParameters(partName);
        }
        public MeshingParameters GetMeshingParameters(string partName)
        {
            return _form.GetMeshingParameters(partName);
        }
        public void SetMeshingParameters(string partName, MeshingParameters meshingParameters)
        {
            ((GeometryPart)_model.Geometry.Parts[partName]).MeshingParameters = meshingParameters;
        }
        public bool CreateMesh(string partName)
        {
            GeometryPart part = (GeometryPart)_model.Geometry.Parts[partName];

            if (part.CADFileData == null) return CreateMeshFromStl(part);
            else return CreateMeshFromBrep(part);
        }
        public bool CreateMeshFromStl(GeometryPart part)
        {
            CalculixSettings settings = (CalculixSettings)Settings[Globals.CalculixSettingsName];
            if (settings.WorkDirectory == null || !Directory.Exists(settings.WorkDirectory))
            {
                MessageBox.Show("The work directory does not exist.", "Error", MessageBoxButtons.OK);
                return false;
            }

            string executable = Application.StartupPath + Globals.NetGenMesher;
            string stlFileName = Path.Combine(settings.WorkDirectory, Globals.StlFileName);
            string volFileName = Path.Combine(settings.WorkDirectory, Globals.VolFileName);
            string parametersFileName = Path.Combine(settings.WorkDirectory, Globals.ParametersFileName);
            string edgeNodesFileName = Path.Combine(settings.WorkDirectory, Globals.EdgeNodesFileName);

            if (File.Exists(stlFileName)) File.Delete(stlFileName);
            if (File.Exists(volFileName)) File.Delete(volFileName);
            if (File.Exists(parametersFileName)) File.Delete(parametersFileName);
            if (File.Exists(edgeNodesFileName)) File.Delete(edgeNodesFileName);

            FileInOut.Output.StlFileWriter.Write(stlFileName, _model.Geometry, part.Name);
            part.MeshingParameters.WriteToFile(parametersFileName);
            _model.Geometry.WriteEdgeNodesToFile(part, edgeNodesFileName);

            string argument = "STL_MESH " +
                              "\"" + stlFileName + "\" " +
                              "\"" + volFileName + "\" " +
                              "\"" + parametersFileName + "\" " +
                              "\"" + edgeNodesFileName + "\"";

            _netgenJob = new NetgenJob(part.Name, executable, argument, settings.WorkDirectory);
            _netgenJob.AppendOutput += netgenJobMeshing_AppendOutput;
            _form.SetStateWorking("Meshing...", true);
            _netgenJob.Submit();

            // Job completed
            if (_netgenJob.JobStatus == JobStatus.OK)
            {
                _form.SetStateReady("Meshing...");
                bool convertToSecondOrder = part.MeshingParameters.SecondOrder && !part.MeshingParameters.MidsideNodesOnGeometry;
                ImportGeneratedMesh(volFileName, part.Name, false, false, convertToSecondOrder);

                return true;
            }
            else return false;
        }
        public bool CreateMeshFromBrep(GeometryPart part)
        {
            CalculixSettings settings = (CalculixSettings)Settings[Globals.CalculixSettingsName];
            if (settings.WorkDirectory == null || !Directory.Exists(settings.WorkDirectory))
            {
                MessageBox.Show("The work directory does not exist.", "Error", MessageBoxButtons.OK);
                return false;
            }

            string executable = Application.StartupPath + Globals.NetGenMesher;
            string stepFileName = Path.Combine(settings.WorkDirectory, Globals.StepFileName);
            string volFileName = Path.Combine(settings.WorkDirectory, Globals.VolFileName);
            string parametersFileName = Path.Combine(settings.WorkDirectory, Globals.ParametersFileName);
            string edgeNodesFileName = Path.Combine(settings.WorkDirectory, Globals.EdgeNodesFileName);

            if (File.Exists(stepFileName)) File.Delete(stepFileName);
            if (File.Exists(volFileName)) File.Delete(volFileName);
            if (File.Exists(parametersFileName)) File.Delete(parametersFileName);
            if (File.Exists(edgeNodesFileName)) File.Delete(edgeNodesFileName);

            File.WriteAllText(stepFileName, part.CADFileData);
            part.MeshingParameters.WriteToFile(parametersFileName);

            string argument = "BREP_MESH " +
                              "\"" + stepFileName + "\" " +
                              "\"" + volFileName + "\" " +
                              "\"" + parametersFileName + "\"";

            _netgenJob = new NetgenJob(part.Name, executable, argument, settings.WorkDirectory);
            _netgenJob.AppendOutput += netgenJobMeshing_AppendOutput;
            _form.SetStateWorking("Meshing...", true);
            _netgenJob.Submit();

            // Job completed
            if (_netgenJob.JobStatus == JobStatus.OK)
            {
                _form.SetStateReady("Meshing...");
                bool convertToSecondOrder = part.MeshingParameters.SecondOrder && !part.MeshingParameters.MidsideNodesOnGeometry;
                ImportGeneratedMesh(volFileName, part.Name, false, true, convertToSecondOrder);

                return true;
            }
            else return false;
        }
        public void StopMeshing()
        {
            if (_netgenJob != null && _netgenJob.JobStatus == JobStatus.Running)
            {
                _netgenJob.Kill("Cancel button clicked.");
                _form.SetStateReady("Meshing...");
            }
        }
        void netgenJobMeshing_AppendOutput(string data)
        {
            _form.WriteDataToOutput(data);
        }
        
        #endregion #################################################################################################################

        #region Node menu   ########################################################################################################
        // COMMANDS ********************************************************************************
        public void RenumberNodesCommand(int startNodeId)
        {
            Commands.CRenumberNodes comm = new Commands.CRenumberNodes(startNodeId, _currentView);
            _commands.AddAndExecute(comm);
        }
        
        //******************************************************************************************
        public void RenumberNodes(int startNodeId)
        {
            if (_currentView == ViewGeometryModelResults.Geometry) _model.Geometry.RenumberNodes(startNodeId);
            else if (_currentView == ViewGeometryModelResults.Model) _model.Mesh.RenumberNodes(startNodeId);
        }
        public int[] GetAllNodeIds()
        {
            if (_currentView == ViewGeometryModelResults.Geometry) return _model.Geometry.Nodes.Keys.ToArray();
            else if (_currentView == ViewGeometryModelResults.Model) return _model.Mesh.Nodes.Keys.ToArray();
            else return _results.Mesh.Nodes.Keys.ToArray();
        }
        public int[] GetVisibleNodeIds()
        {
            if (_currentView == ViewGeometryModelResults.Geometry) return _model.Geometry.GetVisibleNodeIds();
            else if (_currentView == ViewGeometryModelResults.Model) return _model.Mesh.GetVisibleNodeIds();
            else if (_currentView == ViewGeometryModelResults.Results) return _results.Mesh.GetVisibleNodeIds();
            else throw new NotSupportedException();
        }
        public int[] GetAllOutterNodeIds()
        {
            HashSet<int> outterNodes = new HashSet<int>();

            foreach (var entry in DisplayedMesh.Parts)
            {
                foreach (int[] cell in entry.Value.Visualization.Cells)
                {
                    foreach (int nodeId in cell) outterNodes.Add(nodeId);
                }
            }

            return outterNodes.ToArray();
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
            return DisplayedMesh.Nodes[nodeId];
        }        

        #endregion #################################################################################################################

        #region Element menu   #####################################################################################################
        
        public int[] GetAllElementIds()
        {
            if (_currentView == ViewGeometryModelResults.Geometry) return _model.Geometry.Elements.Keys.ToArray();
            else if (_currentView == ViewGeometryModelResults.Model) return _model.Mesh.Elements.Keys.ToArray();
            else if (_currentView == ViewGeometryModelResults.Results) return _results.Mesh.Elements.Keys.ToArray();
            else throw new NotSupportedException();
        }
        public int[] GetVisibleElementIds()
        {
            if (_currentView == ViewGeometryModelResults.Geometry) return _model.Geometry.GetVisibleElementIds();
            else if (_currentView == ViewGeometryModelResults.Model) return _model.Mesh.GetVisibleElementIds();
            else if (_currentView == ViewGeometryModelResults.Results) return _results.Mesh.GetVisibleElementIds();
            else throw new NotSupportedException();
        }

        #endregion #################################################################################################################

        #region Mesh part menu   ###################################################################################################
        // COMMANDS ********************************************************************************
        public void HideModelPartsCommand(string[] partNames)
        {
            Commands.CHideModelParts comm = new Commands.CHideModelParts(partNames);
            _commands.AddAndExecute(comm);
        }
        public void ShowModelPartsCommand(string[] partNames)
        {
            Commands.CShowModelParts comm = new Commands.CShowModelParts(partNames);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceModelPartPropertiesCommand(string oldPartName, PartProperties newPartProperties)
        {
            Commands.CReplaceModelPart comm = new Commands.CReplaceModelPart(oldPartName, newPartProperties);
            _commands.AddAndExecute(comm);
        }
        public void TranlsateModelPartsCommand(string[] partNames, double[] translateVector, bool copy)
        {
            Commands.CTranslateModelParts comm = new Commands.CTranslateModelParts(partNames, translateVector, copy);
            _commands.AddAndExecute(comm);
        }
        public void MergeModelPartsCommand(string[] partNames)
        {
            Commands.CMergeModelParts comm = new Commands.CMergeModelParts(partNames);
            _commands.AddAndExecute(comm);
        }
        public void RemoveModelPartsCommand(string[] partNames)
        {
            Commands.CRemoveModelParts comm = new Commands.CRemoveModelParts(partNames);
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
        public MeshPart[] GetModelParts()
        {
            if (_model.Mesh == null) return null;

            int i = 0;
            MeshPart[] parts = new MeshPart[_model.Mesh.Parts.Count];
            foreach (var entry in _model.Mesh.Parts) parts[i++] = (MeshPart)entry.Value;
            return parts;
        }
        public string[] GetModelPartNames<T>()
        {
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
                    _form.UpdateTreeNode(ViewGeometryModelResults.Model, name, part, null);
                }
            }
            _form.HideActors(partNames, false);
        }
        public void ShowModelParts(string[] partNames)
        {
            BasePart part;
            foreach (var name in partNames)
            {
                if (_model.Mesh.Parts.TryGetValue(name, out part))
                {
                    part.Visible = true;
                    _form.UpdateTreeNode(ViewGeometryModelResults.Model, name, part, null);
                }
            }
            _form.ShowActors(partNames, false);
        }
        public void ReplaceModelPartProperties(string oldPartName, PartProperties newPartProperties)
        {
            // Replace mesh part
            MeshPart meshPart = GetModelPart(oldPartName);
            meshPart.SetProperties(newPartProperties);
            _model.Mesh.Parts.Remove(oldPartName);
            _model.Mesh.Parts.Add(meshPart.Name, meshPart);
            _form.UpdateActor(oldPartName, meshPart.Name, meshPart.Color);
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldPartName, meshPart, null);

            // Rename the geometric part in pair
            if (oldPartName != meshPart.Name && _model.Geometry != null && _model.Geometry.Parts.ContainsKey(oldPartName))
            {
                string newPartName = meshPart.Name;
                GeometryPart geomPart = GetGeometryPart(oldPartName);
                geomPart.Name = newPartName;
                _model.Geometry.Parts.Remove(oldPartName);
                _model.Geometry.Parts.Add(geomPart.Name, geomPart);
                _form.UpdateTreeNode(ViewGeometryModelResults.Geometry, oldPartName, geomPart, null);
            }

            CheckAndUpdateValidity();
        }
        public void TranslateModelParts(string[] partNames, double[] translateVector, bool copy)
        {
            string[] translatedPartNames = _model.Mesh.TranslateParts(partNames, translateVector, copy);
            if (copy)
            {
                foreach (var partName in translatedPartNames)
                {
                    _form.AddTreeNode(ViewGeometryModelResults.Model, _model.Mesh.Parts[partName], null);
                }
            }
            Update(UpdateType.DrawMesh | UpdateType.RedrawSymbols);
        }
        public void MergeModelParts(string[] partNames)
        {
            ViewGeometryModelResults view = ViewGeometryModelResults.Model;
            MeshPart newMeshPart;
            string[] mergedParts;

            _model.Mesh.MergeMeshParts(partNames, out newMeshPart, out mergedParts);

            if (newMeshPart != null && mergedParts != null)
            {
                foreach (var partName in mergedParts)
                {
                    _form.RemoveTreeNode<MeshPart>(view, partName, null);
                }

                _form.AddTreeNode(ViewGeometryModelResults.Model, newMeshPart, null);

                Update(UpdateType.Check | UpdateType.DrawMesh | UpdateType.RedrawSymbols);
            }
        }
        public int[] RemoveModelParts(string[] partNames, bool invalidate, bool removeForRemeshing)
        {
            int[] removedPartIds = null;
            if (_model.Mesh != null)
            {
                string[] removedParts;
                removedPartIds = _model.Mesh.RemoveParts(partNames, out removedParts, removeForRemeshing);

                ViewGeometryModelResults view = ViewGeometryModelResults.Model;
                foreach (var name in removedParts) _form.RemoveTreeNode<MeshPart>(view, name, null);
            }

            UpdateType ut = UpdateType.Check;
            if (invalidate) ut |= UpdateType.DrawMesh | UpdateType.RedrawSymbols;
            Update(ut);

            return removedPartIds;
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
        public void AddNodeSet(FeNodeSet nodeSet)
        {
            if (nodeSet.CreationData != null && nodeSet.CreationData is Selection)
            {
                _selection = nodeSet.CreationData.DeepClone() as Selection;  // in order for the Regenerate history to work
                nodeSet.Labels = GetSelectionIds();
                _selection.Clear();
                _model.Mesh.UpdateNodeSetCenterOfGravity(nodeSet);
                _model.Mesh.NodeSets.Add(nodeSet.Name, nodeSet);
                _form.AddTreeNode(ViewGeometryModelResults.Model, nodeSet, null);
            }
            else throw new NotSupportedException("The node set does not contain any selection data.");

            UpdateSurfacesBasedOnNodeSet(nodeSet.Name);
            UpdateReferencePointsDependentOnNodeSet(nodeSet.Name);

            // check - if a missing item was added
            Update(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public FeNodeSet GetNodeSet(string nodeSetName)
        {
            return _model.Mesh.NodeSets[nodeSetName];
        }
        public FeNodeSet[] GetAllNodeSets()
        {
            if (_model.Mesh == null) return null;
            return _model.Mesh.NodeSets.Values.ToArray();
        }
        public void ReplaceNodeSet(string oldNodeSetName, FeNodeSet nodeSet)
        {
            _model.Mesh.NodeSets.Remove(oldNodeSetName);

            if (nodeSet.CreationData != null && nodeSet.CreationData is Selection)
            {
                _selection = nodeSet.CreationData.DeepClone() as Selection;  // in order for the Regenerate history to work
                nodeSet.Labels = GetSelectionIds();
                _selection.Clear();
                _model.Mesh.UpdateNodeSetCenterOfGravity(nodeSet);
                _model.Mesh.NodeSets.Add(nodeSet.Name, nodeSet);
            }
            else throw new NotSupportedException("The node set does not contain any selection data.");

            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldNodeSetName, nodeSet, null);
            UpdateSurfacesBasedOnNodeSet(nodeSet.Name);
            UpdateReferencePointsDependentOnNodeSet(nodeSet.Name);

            Update(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void RemoveNodeSets(string[] nodeSetNames)
        {
            foreach (var name in nodeSetNames)
            {
                _model.Mesh.NodeSets.Remove(name);
                _form.RemoveTreeNode<FeNodeSet>(ViewGeometryModelResults.Model, name, null);
                UpdateSurfacesBasedOnNodeSet(name);
                UpdateReferencePointsDependentOnNodeSet(name);
            }

            Update(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void GetNodesCenterOfGravity(FeNodeSet nodeSet)
        {
            _model.Mesh.UpdateNodeSetCenterOfGravity(nodeSet);
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
            List<string> userElementSetNames = new List<string>();
            foreach (var entry in _model.Mesh.ElementSets)
            {
                if (!entry.Value.Internal && entry.Value.Labels.Length > 0 && _model.Mesh.Elements[entry.Value.Labels[0]] is T)
                    userElementSetNames.Add(entry.Key);
            }
            return userElementSetNames.ToArray();
        }
        public void AddelementSet(FeElementSet elementSet)
        {
            if (elementSet.CreationData != null && elementSet.CreationData is Selection)
            {
                _selection = elementSet.CreationData.DeepClone() as Selection;  // in order for the Regenerate history to work
                elementSet.Labels = GetSelectionIds();
                _selection.Clear();
                _model.Mesh.ElementSets.Add(elementSet.Name, elementSet);
                _form.AddTreeNode(ViewGeometryModelResults.Model, elementSet, null);
            }
            else throw new NotSupportedException("The element set does not contain any selection data.");

            Update(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public FeElementSet GetElementSet(string elementSetName)
        {
            return _model.Mesh.ElementSets[elementSetName];
        }
        public FeElementSet[] GetAllelementSets()
        {
            if (_model.Mesh == null) return null;
            return _model.Mesh.ElementSets.Values.ToArray();
        }
        public void ReplaceElementSet(string oldElementSetName, FeElementSet elementSet)
        {
            _model.Mesh.ElementSets.Remove(oldElementSetName);

            if (elementSet.CreationData != null && elementSet.CreationData is Selection)
            {
                _selection = elementSet.CreationData.DeepClone() as Selection;  // in order for the Regenerate history to work
                elementSet.Labels = GetSelectionIds();
                _selection.Clear();
                _model.Mesh.ElementSets.Add(elementSet.Name, elementSet);
            }
            else throw new NotSupportedException("The element set does not contain any selection data.");

            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldElementSetName, elementSet, null);

            Update(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void ConvertElementSetsToMeshParts(string[] elementSetNames)
        {
            BasePart[] modifiedParts;
            BasePart[] newParts;

            _model.Mesh.CreateMeshPartsFromElementSets(elementSetNames, out modifiedParts, out newParts);
            foreach (var part in modifiedParts)
            {
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, part.Name, part, null);
            }

            // add new parts and remove element sets
            foreach (var part in newParts)
            {
                _form.AddTreeNode(ViewGeometryModelResults.Model, part, null);
                _form.RemoveTreeNode<FeElementSet>(ViewGeometryModelResults.Model, part.Name, null);
            }

            Update(UpdateType.Check | UpdateType.DrawMesh | UpdateType.RedrawSymbols);
        }
        public void RemoveElementSets(string[] elementSetNames)
        {
            foreach (var name in elementSetNames)
            {
                _model.Mesh.ElementSets.Remove(name);
                _form.RemoveTreeNode<FeElementSet>(ViewGeometryModelResults.Model, name, null);
            }

            Update(UpdateType.Check | UpdateType.RedrawSymbols);
        }

        #endregion #################################################################################################################

        #region Surface menu   #####################################################################################################
        // COMMANDS ********************************************************************************
        public void AddSurfaceCommand(FeSurface surface)
        {
            Commands.CAddSurface comm = new Commands.CAddSurface(surface);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceSurfaceCommand(string oldSurfaceName, FeSurface newSurface)
        {
            Commands.CReplaceSurface comm = new Commands.CReplaceSurface(oldSurfaceName, newSurface);
            _commands.AddAndExecute(comm);
        }
        public void RemoveSurfacesCommand(string[] surfaceNames)
        {
            Commands.CRemoveSurfaces comm = new Commands.CRemoveSurfaces(surfaceNames);
            _commands.AddAndExecute(comm);
        }

        //******************************************************************************************

        public string[] GetSurfaceNames()
        {
            if (_model.Mesh != null) return _model.Mesh.Surfaces.Keys.ToArray();
            else return null;
        }
        public void AddSurface(FeSurface surface)
        {
            if (surface.CreatedFrom == FeSurfaceCreatedFrom.Selection)
            {
                _selection = surface.CreationData.DeepClone() as Selection;  // in order for the Regenerate history to work
                surface.FaceIds = GetSelectionIds();
                _selection.Clear();
            }
            
            AddSurfaceAndElementFaces(surface);

            _form.AddTreeNode(ViewGeometryModelResults.Model, surface, null);

            Update(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public FeSurface GetSurface(string surfaceName)
        {
            return _model.Mesh.Surfaces[surfaceName];
        }
        public FeSurface[] GetAllSurfaces()
        {
            if (_model.Mesh == null) return null;
            return _model.Mesh.Surfaces.Values.ToArray();
        }
        public void ReplaceSurface(string oldSurfaceName, FeSurface newSurface)
        {
            RemoveSurfaceAndElementFacesFromModel(new string[] { oldSurfaceName });

            newSurface.Valid = true;    // added for the update geometry based surfaces to work after remeshing
            if (newSurface.CreatedFrom == FeSurfaceCreatedFrom.Selection)
            {
                _selection = newSurface.CreationData.DeepClone() as Selection;  // in order for the Regenerate history to work
                newSurface.FaceIds = GetSelectionIds();
                _selection.Clear();
            }

            AddSurfaceAndElementFaces(newSurface);

            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldSurfaceName, newSurface, null);

            Update(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void RemoveSurfaces(string[] surfaceNames)
        {
            RemoveSurfaceAndElementFacesFromModel(surfaceNames);
            
            foreach (var name in surfaceNames) _form.RemoveTreeNode<FeSurface>(ViewGeometryModelResults.Model, name, null);

            Update(UpdateType.Check | UpdateType.RedrawSymbols);
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
        private void RemoveSurfaceAndElementFacesFromModel(string[] surfaceNames)
        {
            string[] removedNodeSets;
            string[] removedElementSets;
            _model.Mesh.RemoveSurfaces(surfaceNames, out removedNodeSets, out removedElementSets);

            //foreach (var nodeSetName in removedNodeSets) _form.RemoveTreeNode<FeNodeSet>(ViewGeometryModelResults.Model, nodeSetName, null);
            //foreach (var elementSetName in removedElementSets) _form.RemoveTreeNode<FeElementSet>(ViewGeometryModelResults.Model, elementSetName, null);
        }
        private int[] GetVisibleFaceIds()
        {
            return DisplayedMesh.GetVisibleVisualizationFaceIds();
        }

         
        public void UpdateSurfaceArea(FeSurface surface)
        {
            _model.Mesh.UpdateSurfaceArea(surface);
        }
        private void UpdateSurfacesBasedOnNodeSet(string nodeSetName)
        {
            // use list not to throw collection moddified
            List<CaeMesh.FeSurface> changedSurfaces = new List<FeSurface>();
            if (_model != null && _model.Mesh != null)
            {
                foreach (var entry in _model.Mesh.Surfaces)
                {
                    if (entry.Value.CreatedFrom == FeSurfaceCreatedFrom.NodeSet && entry.Value.CreatedFromNodeSetName == nodeSetName)
                    {
                        changedSurfaces.Add(entry.Value);
                    }
                }
                if (changedSurfaces.Count > 0)
                {
                    foreach (FeSurface surface in changedSurfaces) ReplaceSurface(surface.Name, surface);
                }
            }
        }
        private void UpdateSurfacesBasedOnGeometry()
        {
            // use list not to throw collection moddified
            List<CaeMesh.FeSurface> geomSurfaces = new List<FeSurface>();
            if (_model != null && _model.Mesh != null)
            {
                foreach (var entry in _model.Mesh.Surfaces)
                {
                    if (entry.Value.CreationData.IsGeometryBased()) geomSurfaces.Add(entry.Value);
                }
                if (geomSurfaces.Count > 0)
                {
                    foreach (FeSurface surface in geomSurfaces) ReplaceSurface(surface.Name, surface);
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
        public void ReplaceReferencePointCommand(string oldReferencePointName, FeReferencePoint newReferencePoint)
        {
            Commands.CReplaceReferencePoint comm = new Commands.CReplaceReferencePoint(oldReferencePointName, newReferencePoint);
            _commands.AddAndExecute(comm);
        }
        public void RemoveReferencePointsCommand(string[] referencePointNames)
        {
            Commands.CRemoveReferencePoints comm = new Commands.CRemoveReferencePoints(referencePointNames);
            _commands.AddAndExecute(comm);
        }

        ////******************************************************************************************

        public string[] GetReferencePointNames()
        {
            if (_model.Mesh != null) return _model.Mesh.ReferencePoints.Keys.ToArray();
            else return null;
        }
        public void AddReferencePoint(FeReferencePoint referencePoint)
        {
            if (referencePoint.CreatedFrom != FeReferencePointCreatedFrom.Coordinates)
                referencePoint.UpdateCoordinates(_model.Mesh.NodeSets[referencePoint.NodeSetName]);
            _model.Mesh.ReferencePoints.Add(referencePoint.Name, referencePoint);

            _form.AddTreeNode(ViewGeometryModelResults.Model, referencePoint, null);

            Update(UpdateType.Check | UpdateType.RedrawSymbols);
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
        public void ReplaceReferencePoint(string oldReferencePointName, FeReferencePoint newReferencePoint)
        {
            _model.Mesh.ReferencePoints.Remove(oldReferencePointName);
            _model.Mesh.ReferencePoints.Add(newReferencePoint.Name, newReferencePoint);

            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldReferencePointName, newReferencePoint, null);

            Update(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void RemoveReferencePoints(string[] referencePointNames)
        {
            foreach (var name in referencePointNames)
            {
                _model.Mesh.ReferencePoints.Remove(name);
                _form.RemoveTreeNode<FeReferencePoint>(ViewGeometryModelResults.Model, name, null);
            }

            Update(UpdateType.Check | UpdateType.RedrawSymbols);
        }

        private void UpdateReferencePointsDependentOnNodeSet(string nodeSetName)
        {
            FeNodeSet ns;
            if (_model != null && _model.Mesh != null)
            {
                foreach (var entry in _model.Mesh.ReferencePoints)
                {
                    if (entry.Value.CreatedFrom != FeReferencePointCreatedFrom.Coordinates && entry.Value.NodeSetName == nodeSetName)
                    {
                        if (_model.Mesh.NodeSets.TryGetValue(nodeSetName, out ns)) entry.Value.UpdateCoordinates(ns);
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

            CheckAndUpdateValidity();
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
            _model.Materials.Remove(oldMaterialName);
            _model.Materials.Add(newMaterial.Name, newMaterial);
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldMaterialName, newMaterial, null);

            CheckAndUpdateValidity();
        }
        public void RemoveMaterials(string[] materialNames)
        {
            foreach (var name in materialNames)
            {
                _model.Materials.Remove(name);
                _form.RemoveTreeNode<Material>(ViewGeometryModelResults.Model, name, null);
            }

            CheckAndUpdateValidity();
        }

        #endregion #################################################################################################################

        #region Section menu   #####################################################################################################
        // COMMANDS ********************************************************************************
        public void AddSectionCommand(Section section)
        {
            Commands.CAddSection comm = new Commands.CAddSection(section);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceSectionCommand(string oldSectionName, Section newSection)
        {
            Commands.CReplaceSection comm = new Commands.CReplaceSection(oldSectionName, newSection);
            _commands.AddAndExecute(comm);
        }
        public void RemoveSectionsCommand(string[] sectionNames)
        {
            Commands.CRemoveSections comm = new Commands.CRemoveSections(sectionNames);
            _commands.AddAndExecute(comm);
        }

        //******************************************************************************************

        public string[] GetSectionNames()
        {
            return _model.Sections.Keys.ToArray();
        }
        public void AddSection(Section section)
        {
            _model.Sections.Add(section.Name, section);
            _form.AddTreeNode(ViewGeometryModelResults.Model, section, null);

            CheckAndUpdateValidity();
        }
        public Section GetSection(string sectionName)
        {
            return _model.Sections[sectionName];
        }
        public Section[] GetAllSections()
        {
            return _model.Sections.Values.ToArray();
        }
        public void ReplaceSection(string oldSectionName, Section newSection)
        {
            _model.Sections.Remove(oldSectionName);
            _model.Sections.Add(newSection.Name, newSection);
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldSectionName, newSection, null);

            CheckAndUpdateValidity();
        }
        public void RemoveSections(string[] sectionNames)
        {
            foreach (var name in sectionNames)
            {
                _model.Sections.Remove(name);
                _form.RemoveTreeNode<Section>(ViewGeometryModelResults.Model, name, null);
            }

            CheckAndUpdateValidity();
        }

        #endregion #################################################################################################################

        #region Constraints menu   #################################################################################################
        // COMMANDS ********************************************************************************
        public void AddConstraintCommand(Constraint constraint)
        {
            Commands.CAddConstraint comm = new Commands.CAddConstraint(constraint);
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
        public void ReplaceConstraintCommand(string oldConstraintName, Constraint newConstraint)
        {
            Commands.CReplaceConstraint comm = new Commands.CReplaceConstraint(oldConstraintName, newConstraint);
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
        public void AddConstraint(Constraint constraint)
        {
            _model.Constraints.Add(constraint.Name, constraint);

            _form.AddTreeNode(ViewGeometryModelResults.Model, constraint, null);

            Update(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public Constraint GetConstraint(string constraintName)
        {
            return _model.Constraints[constraintName];
        }
        public Constraint[] GetAllConstraints()
        {
            return _model.Constraints.Values.ToArray();
        }
        public void HideConstraints(string[] constraintNames)
        {
            foreach (var name in constraintNames)
            {
                _model.Constraints[name].Visible = false;
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, name, _model.Constraints[name], null);
            }
            //_form.HideActors(constraintNames);
            Update(UpdateType.RedrawSymbols);
        }
        public void ShowConstraints(string[] constraintNames)
        {
            foreach (var name in constraintNames)
            {
                _model.Constraints[name].Visible = true;
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, name, _model.Constraints[name], null);
            }
            //_form.ShowActors(constraintNames);
            Update(UpdateType.RedrawSymbols);
        }
        public void ReplaceConstraint(string oldConstraintName, Constraint newConstraint)
        {
            _model.Constraints.Remove(oldConstraintName);
            _model.Constraints.Add(newConstraint.Name, newConstraint);

            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldConstraintName, newConstraint, null);

            Update(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void RemoveConstraints(string[] constraintNames)
        {
            foreach (var name in constraintNames)
            {
                _model.Constraints.Remove(name);
                _form.RemoveTreeNode<Constraint>(ViewGeometryModelResults.Model, name, null);
            }

            Update(UpdateType.Check | UpdateType.RedrawSymbols);
        }

        #endregion #################################################################################################################

        #region Step menu   ########################################################################################################
        // COMMANDS ********************************************************************************
        public void AddStepCommand(Step step)
        {
            Commands.CAddStep comm = new Commands.CAddStep(step);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceStepCommand(string oldStepName, Step newStep)
        {
            Commands.CReplaceStep comm = new Commands.CReplaceStep(oldStepName, newStep);
            _commands.AddAndExecute(comm);
        }
        public void RemoveStepsCommnad(string[] stepNames)
        {
            Commands.CRemoveSteps comm = new Commands.CRemoveSteps(stepNames);
            _commands.AddAndExecute(comm);
        }

        //******************************************************************************************

        public string[] GetStepNames()
        {
            return _model.StepCollection.GetStepNames();
        }
        public void AddStep(Step step)
        {
            _model.StepCollection.AddStep(step);
            _form.AddTreeNode(ViewGeometryModelResults.Model, step, null);

            Update(UpdateType.Check | UpdateType.RedrawSymbols);
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

            Update(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void RemoveSteps(string[] stepNames)
        {
            foreach (var name in stepNames)
            {
                _model.StepCollection.RemoveStep(name);
                _form.RemoveTreeNode<Step>(ViewGeometryModelResults.Model, name, null);
            }

            Update(UpdateType.Check | UpdateType.RedrawSymbols);
        }

        #endregion #################################################################################################################

        #region History output menu   ##############################################################################################
        // COMMANDS ********************************************************************************
        public void AddHistoryOutputCommand(string stepName, HistoryOutput historyOutput)
        {
            Commands.CAddHistoryOutput comm = new Commands.CAddHistoryOutput(stepName, historyOutput);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceHistoryOutputCommand(string stepName, string oldHistoryOutputName, HistoryOutput historyOutput)
        {
            Commands.CReplaceHisotryOutput comm = new Commands.CReplaceHisotryOutput(stepName, oldHistoryOutputName, historyOutput);
            _commands.AddAndExecute(comm);
        }
        public void RemoveHistoryOutputsForStepCommand(string stepName, string[] historyOutputNames)
        {
            Commands.CRemoveHistoryOutputs comm = new Commands.CRemoveHistoryOutputs(stepName, historyOutputNames);
            _commands.AddAndExecute(comm);
        }

        //******************************************************************************************

        public string[] GetHistoryOutputNames()
        {
            return _model.StepCollection.GetHistoryOutputNames();
        }
        public string[] GetHistoryOutputNamesForStep(string stepName)
        {
            return _model.StepCollection.GetStep(stepName).HistoryOutputs.Keys.ToArray();
        }
        public void AddHistoryOutput(string stepName, HistoryOutput historyOutput)
        {
            _model.StepCollection.GetStep(stepName).HistoryOutputs.Add(historyOutput.Name, historyOutput);
            _form.AddTreeNode(ViewGeometryModelResults.Results, historyOutput, stepName);

            CheckAndUpdateValidity();
        }
        public HistoryOutput GetHistoryOutput(string stepName, string historyOutputName)
        {
            return _model.StepCollection.GetStep(stepName).HistoryOutputs[historyOutputName];
        }
        public HistoryOutput[] GetAllHistoryOutputs(string stepName)
        {
            return _model.StepCollection.GetStep(stepName).HistoryOutputs.Values.ToArray();
        }
        public void ReplaceHistoryOutput(string stepName, string oldHistoryOutputName, HistoryOutput historyOutput)
        {
            _model.StepCollection.GetStep(stepName).HistoryOutputs.Remove(oldHistoryOutputName);
            _model.StepCollection.GetStep(stepName).HistoryOutputs.Add(historyOutput.Name, historyOutput);
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldHistoryOutputName, historyOutput, stepName);

            CheckAndUpdateValidity();
        }
        public void RemoveHistoryOutputs(string stepName, string[] historyOutputNames)
        {
            foreach (var name in historyOutputNames)
            {
                _model.StepCollection.GetStep(stepName).HistoryOutputs.Remove(name);
                _form.RemoveTreeNode<HistoryOutput>(ViewGeometryModelResults.Model, name, stepName);
            }

            CheckAndUpdateValidity();
        }

        #endregion #################################################################################################################

        #region Field output menu   ################################################################################################
        // COMMANDS ********************************************************************************
        public void AddFieldOutputCommand(string stepName, FieldOutput fieldOutput)
        {
            Commands.CAddFieldOutput comm = new Commands.CAddFieldOutput(stepName, fieldOutput);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceFieldOutputCommand(string stepName, string oldFieldOutputName, FieldOutput fieldOutput)
        {
            Commands.CReplaceFieldOutput comm = new Commands.CReplaceFieldOutput(stepName, oldFieldOutputName, fieldOutput);
            _commands.AddAndExecute(comm);
        }
        public void RemoveFieldOutputsForStepCommand(string stepName, string[] fieldOutputNames)
        {
            Commands.CRemoveFieldOutputs comm = new Commands.CRemoveFieldOutputs(stepName, fieldOutputNames);
            _commands.AddAndExecute(comm);
        }

        //******************************************************************************************

        public string[] GetFieldOutputNames()
        {
            return _model.StepCollection.GetFieldOutputNames();
        }
        public string[] GetFieldOutputNamesForStep(string stepName)
        {
            return _model.StepCollection.GetStep(stepName).FieldOutputs.Keys.ToArray();
        }
        public void AddFieldOutput(string stepName, FieldOutput fieldOutput)
        {
            _model.StepCollection.GetStep(stepName).FieldOutputs.Add(fieldOutput.Name, fieldOutput);
            _form.AddTreeNode(ViewGeometryModelResults.Results, fieldOutput, stepName);

            CheckAndUpdateValidity();
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
            _model.StepCollection.GetStep(stepName).FieldOutputs.Remove(oldFieldOutputName);
            _model.StepCollection.GetStep(stepName).FieldOutputs.Add(fieldOutput.Name, fieldOutput);
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldFieldOutputName, fieldOutput, stepName);

            CheckAndUpdateValidity();
        }
        public void RemoveFieldOutputs(string stepName, string[] fieldOutputNames)
        {
            foreach (var name in fieldOutputNames)
            {
                _model.StepCollection.GetStep(stepName).FieldOutputs.Remove(name);
                _form.RemoveTreeNode<FieldOutput>(ViewGeometryModelResults.Model, name, stepName);
            }

            CheckAndUpdateValidity();
        }

        #endregion #################################################################################################################

        #region Boundary condition menu   ##########################################################################################
        // COMMANDS ********************************************************************************
        public void AddBoundaryConditionCommand(string stepName, BoundaryCondition boundaryCondition)
        {
            Commands.CAddBC comm = new Commands.CAddBC(stepName, boundaryCondition);
            _commands.AddAndExecute(comm);
        }
        public void HideBoundaryConditionCommand(string stepName, string[] boundaryConditionNames)
        {
            Commands.CHideBCs comm = new Commands.CHideBCs(stepName, boundaryConditionNames);
            _commands.AddAndExecute(comm);
        }
        public void ShowBoundaryConditionCommand(string stepName, string[] boundaryConditionNames)
        {
            Commands.CShowBCs comm = new Commands.CShowBCs(stepName, boundaryConditionNames);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceBoundaryConditionCommand(string stepName, string oldBoundaryConditionName, BoundaryCondition boundaryCondition)
        {
            Commands.CReplaceBC comm = new Commands.CReplaceBC(stepName, oldBoundaryConditionName, boundaryCondition);
            _commands.AddAndExecute(comm);
        }
        public void RemoveBoundaryConditionsCommand(string stepName, string[] boundaryConditionNames)
        {
            Commands.CRemoveBCs comm = new Commands.CRemoveBCs(stepName, boundaryConditionNames);
            _commands.AddAndExecute(comm);
        }

        //******************************************************************************************

        public string[] GetBoundaryConditionNames(string stepName)
        {
            return _model.StepCollection.GetStep(stepName).BoundaryConditions.Keys.ToArray();
        }
        public void AddBoundaryCondition(string stepName, BoundaryCondition boundaryCondition)
        {
            _model.StepCollection.AddBoundaryCondition(boundaryCondition, stepName);

            _form.AddTreeNode(ViewGeometryModelResults.Model, boundaryCondition, stepName);

            Update(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public BoundaryCondition GetBoundaryCondition(string stepName, string boundaryConditionName)
        {
            return _model.StepCollection.GetStep(stepName).BoundaryConditions[boundaryConditionName];
        }
        public BoundaryCondition[] GetStepBoundaryConditions(string stepName)
        {
            return _model.StepCollection.GetStep(stepName).BoundaryConditions.Values.ToArray();
        }

        public void HideBoundaryConditions(string stepName, string[] boundaryConditionNames)
        {
            foreach (var name in boundaryConditionNames)
            {
                _model.StepCollection.GetStep(stepName).BoundaryConditions[name].Visible = false;
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, name, _model.StepCollection.GetStep(stepName).BoundaryConditions[name], stepName);
            }
            Update(UpdateType.RedrawSymbols);
        }
        public void ShowBoundaryConditions(string stepName, string[] boundaryConditionNames)
        {
            foreach (var name in boundaryConditionNames)
            {
                _model.StepCollection.GetStep(stepName).BoundaryConditions[name].Visible = true;
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, name, _model.StepCollection.GetStep(stepName).BoundaryConditions[name], stepName);
            }
            Update(UpdateType.RedrawSymbols);
        }
        public void ReplaceBoundaryCondition(string stepName, string oldBoundaryConditionName, BoundaryCondition boundaryCondition)
        {
            _model.StepCollection.GetStep(stepName).BoundaryConditions.Remove(oldBoundaryConditionName);
            _model.StepCollection.GetStep(stepName).BoundaryConditions.Add(boundaryCondition.Name, boundaryCondition);

            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldBoundaryConditionName, boundaryCondition, stepName);

            Update(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void RemoveBoundaryConditions(string stepName, string[] boundaryConditionNames)
        {
            foreach (var name in boundaryConditionNames)
            {
                _model.StepCollection.GetStep(stepName).BoundaryConditions.Remove(name);
                _form.RemoveTreeNode<BoundaryCondition>(ViewGeometryModelResults.Model, name, stepName);
            }

            Update(UpdateType.Check | UpdateType.RedrawSymbols);
        }

        #endregion #################################################################################################################

        #region Load menu   ########################################################################################################
        // COMMANDS ********************************************************************************
        public void AddLoadCommand(string stepName, Load load)
        {
            Commands.CAddLoad comm = new Commands.CAddLoad(stepName, load);
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
        public void ReplaceLoadCommand(string stepName, string oldLoadName, Load load)
        {
            Commands.CReplaceLoad comm = new Commands.CReplaceLoad(stepName, oldLoadName, load);
            _commands.AddAndExecute(comm);
        }
        public void RemoveLoadsCommand(string stepName, string[] loadNames)
        {
            Commands.CRemoveLoads comm = new Commands.CRemoveLoads(stepName, loadNames);
            _commands.AddAndExecute(comm);
        }

        //******************************************************************************************

        public string[] GetLoadNames(string stepName)
        {
            return _model.StepCollection.GetStep(stepName).Loads.Keys.ToArray();
        }
        public void AddLoad(string stepName, Load load)
        {
            _model.StepCollection.GetStep(stepName).Loads.Add(load.Name, load);

            _form.AddTreeNode(ViewGeometryModelResults.Model, load, stepName);

            Update(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public Load GetLoad(string stepName, string loadName)
        {
            return _model.StepCollection.GetStep(stepName).Loads[loadName];
        }
        public Load[] GetStepLoads(string stepName)
        {
            return _model.StepCollection.GetStep(stepName).Loads.Values.ToArray();
        }
        public void HideLoads(string stepName, string[] loadNames)
        {
            foreach (var name in loadNames)
            {
                _model.StepCollection.GetStep(stepName).Loads[name].Visible = false;
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, name, _model.StepCollection.GetStep(stepName).Loads[name], stepName);
            }
            Update(UpdateType.RedrawSymbols);
        }
        public void ShowLoads(string stepName, string[] loadNames)
        {
            foreach (var name in loadNames)
            {
                _model.StepCollection.GetStep(stepName).Loads[name].Visible = true;
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, name, _model.StepCollection.GetStep(stepName).Loads[name], stepName);
            }
            Update(UpdateType.RedrawSymbols);
        }
        public void ReplaceLoad(string stepName, string oldLoadName, Load load)
        {
            _model.StepCollection.GetStep(stepName).Loads.Remove(oldLoadName);
            _model.StepCollection.GetStep(stepName).Loads.Add(load.Name, load);

            _form.UpdateTreeNode(ViewGeometryModelResults.Model, oldLoadName, load, stepName);

            Update(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void RemoveLoads(string stepName, string[] loadNames)
        {
            foreach (var name in loadNames)
            {
                _model.StepCollection.GetStep(stepName).Loads.Remove(name);
                _form.RemoveTreeNode<Load>(ViewGeometryModelResults.Model, name, stepName);
            }
            Update(UpdateType.Check | UpdateType.RedrawSymbols);
        }

        #endregion #################################################################################################################

        #region Settings menu   ####################################################################################################

        public void ApplySettings()
        {
            // Graphics settings
            GraphicsSettings graphicsSettings = (GraphicsSettings)_settings[Globals.GraphicsSettingsName];
            _form.SetBackground(graphicsSettings.BackgroundType == BackgroundType.Gradient, graphicsSettings.TopColor, graphicsSettings.BottomColor, false);
            _form.SetCoorSysVisibility(graphicsSettings.CoorSysVisibility);
            _form.SetScaleWidgetVisibility(graphicsSettings.ScaleWidgetVisibility);
            _form.SetLighting(graphicsSettings.AmbientComponent, graphicsSettings.DiffuseComponent, false);
            _form.SetSmoothing(graphicsSettings.PointSmoothing, graphicsSettings.LineSmoothing, false);

            // Pre-processing settings
            PreSettings preSettings = (PreSettings)_settings[Globals.PreSettingsName];
            _form.SetHighlightColor(preSettings.HighlightColor);
            _form.SetMouseHighlightColor(preSettings.MouseHighlightColor);

            // Job settings
            if (_jobs != null)
            {
                CalculixSettings settings = (CalculixSettings)_settings[Globals.CalculixSettingsName];
                foreach (var entry in _jobs)
                {
                    entry.Value.WorkDirectory = settings.WorkDirectory;
                    entry.Value.Executable = settings.CalculixExe;
                    entry.Value.NumCPUs = settings.NumCPUs;
                    entry.Value.EnvironmentVariables = settings.EnvironmentVariables;
                }
            }
        }

        #endregion #################################################################################################################

        #region Analysis menu   ####################################################################################################
        // COMMANDS ********************************************************************************
        public void AddJobCommand(AnalysisJob job)
        {
            Commands.CAddJob comm = new Commands.CAddJob(job);
            _commands.AddAndExecute(comm);
        }
        public void ReplaceJobCommand(string oldJobName, AnalysisJob job)
        {
            Commands.CReplaceJob comm = new Commands.CReplaceJob(oldJobName, job);
            _commands.AddAndExecute(comm);
        }
        public void RemoveJobsCommand(string[] jobNames)
        {
            Commands.CRemoveJobs comm = new Commands.CRemoveJobs(jobNames);
            _commands.AddAndExecute(comm);
        }

        //******************************************************************************************

        public string[] GetJobNames()
        {
            return _jobs.Keys.ToArray();
        }
        public void AddJob(AnalysisJob job)
        {           
            _jobs.Add(job.Name, job);
            ApplySettings();
            _form.AddTreeNode(ViewGeometryModelResults.Model, job, null);
        }

      
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
        public void RunJob(string inputFileName, AnalysisJob job)
        {
            if (File.Exists(job.Executable))
            {
                string directory = Path.GetDirectoryName(inputFileName);
                string searchPattern = Path.GetFileNameWithoutExtension(inputFileName) + ".*";
                string[] files = Directory.GetFiles(directory, searchPattern);
                try
                {
                    foreach (var fileName in files) File.Delete(fileName);
                }
                catch (Exception ex)
                {
                    throw new CaeGlobals.CaeException(ex.Message);
                }
                
                ExportToCalculix(inputFileName);
                job.JobStatusChanged = JobStatusChanged;
                job.Submit();
            }
            else
            {
                throw new CaeException("The executable file of the analysis does not exists.");
            }
        }
        private void JobStatusChanged(string jobName, JobStatus jobStatus)
        {
            _form.UpdateAnalysisProgress();
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, jobName, _jobs[jobName], null, true);
        }

        public void KillJob(string jobName)
        {
            _jobs[jobName].Kill(Environment.NewLine + "Kill command sent by user." + Environment.NewLine);
            _form.UpdateTreeNode(ViewGeometryModelResults.Model, jobName, _jobs[jobName], null);
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

        #region Result Part menu  ##################################################################################################
        public string[] GetResultPartNames()
        {
            return _results.Mesh.Parts.Keys.ToArray();
        }
        public BasePart GetResultPart(string partName)
        {
            return _results.Mesh.Parts[partName];
        }
        public BasePart[] GetResultParts()
        {
            int i = 0;
            BasePart[] parts = new BasePart[_results.Mesh.Parts.Count];
            foreach (var entry in _results.Mesh.Parts) parts[i++] = (BasePart)entry.Value;
            return parts;
        }
        public BasePart[] GetResultParts<T>()
        {
            List<BasePart> parts = new List<BasePart>();
            foreach (var entry in _results.Mesh.Parts)
            {
                if (entry.Value is ResultPart) parts.Add(entry.Value);
            }
            return parts.ToArray();
        }
        public string[] GetResultPartNames<T>()
        {
            List<string> names = new List<string>();
            foreach (var entry in _results.Mesh.Parts)
            {
                if (entry.Value.Labels.Length > 0 && _results.Mesh.Elements[entry.Value.Labels[0]] is T)
                {
                    names.Add(entry.Key);
                }
            }
            return names.ToArray();
        }
        public void HideResultParts(string[] partNames)
        {
            foreach (var name in partNames)
            {
                _results.Mesh.Parts[name].Visible = false;
                _form.UpdateTreeNode(ViewGeometryModelResults.Results, name, _results.Mesh.Parts[name], null);
            }
            _form.HideActors(partNames, true);
        }
        public void ShowResultParts(string[] partNames)
        {
            foreach (var name in partNames)
            {
                _results.Mesh.Parts[name].Visible = true;
                _form.UpdateTreeNode(ViewGeometryModelResults.Results, name, _results.Mesh.Parts[name], null);
            }
            _form.ShowActors(partNames, true);
        }
        public void SetResultPartsColorContoursVisibility(string[] partNames, bool colorContours)
        {
            foreach (var name in partNames)
            {
                if (_results.Mesh.Parts[name] is ResultPart resultPart) resultPart.ColorContours = colorContours;
            }
            _form.UpdateActorColorContoursVisibility(partNames, colorContours);
            UpdateHighlightFromTree();
        }
        public void ReplaceResultPartProperties(string oldPartName, PartProperties newPartProperties)
        {
            // Replace result part
            BasePart part = GetResultPart(oldPartName);
            part.SetProperties(newPartProperties);
            _results.Mesh.Parts.Remove(oldPartName);
            _results.Mesh.Parts.Add(part.Name, part);
            _form.UpdateActor(oldPartName, part.Name, part.Color);
            _form.UpdateTreeNode(ViewGeometryModelResults.Results, oldPartName, part, null);
        }
        public void RemoveResultParts(string[] partNames)
        {
            string[] removedParts;
            _results.Mesh.RemoveParts(partNames, out removedParts, false);

            ViewGeometryModelResults view = ViewGeometryModelResults.Results;
            foreach (var name in removedParts) _form.RemoveTreeNode<BasePart>(view, name, null);

            DrawResults(false);
        }

        #endregion #################################################################################################################

        #region Results  ###########################################################################################################
        public string[] GetResultFieldOutputNames()
        {
            return _results.GetAllFieldNames();
        }
        public string[] GetResultFieldOutputComponents(string fieldOutputName)
        {
            return _results.GetComponentNames(fieldOutputName);
        }
        public int[] GetResultStepIDs()
        {
            return _results.GetAllStepIds();
        }
        public int[] GetResultStepIncrementIds(int stepId)
        {
            return _results.GetIncrementIds(stepId);
        }
        public Dictionary<int, int[]> GetResultExistingIncrementIds(string name, string component)
        {
            return _results.GetExistingIncrementIds(name, component);
        }

        public void GetHistoryOutputData(HistoryResultData historyData, out string[] columnNames, out object[][] rowBasedData)
        {
            HistoryResultSet set = _history.Sets[historyData.SetName];
            HistoryResultField field = set.Fields[historyData.FieldName];
            HistoryResultComponent component = field.Components[historyData.ComponentName];

            int numCol = component.Entries.Count + 1; // +1 for the time column
            int numRow = component.Entries.First().Value.Time.Count;
            columnNames = new string[numCol];
            rowBasedData = new object[numRow][];
            
            // Create rows
            for (int i = 0; i < numRow; i++) rowBasedData[i] = new object[numCol];

            // Add time column
            int col = 0;
            int row = 0;
            columnNames[col] = "Time";
            foreach (var time in component.Entries.First().Value.Time)
            {
                rowBasedData[row][0] = time;
                row++;
            }

            // Add data column
            col = 1;
            foreach (var entry in component.Entries)
            {
                columnNames[col] = entry.Key;

                row = 0;
                foreach (var value in entry.Value.Values)
                {
                    rowBasedData[row][col] = value;
                    row++;
                }
                col++;
            }


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
            item.Active = activate;
            if (item is HistoryOutput)
            {
                ReplaceHistoryOutput(stepName, item.Name, ((HistoryOutput)item));
            }
            else if (item is FieldOutput)
            {
                ReplaceFieldOutput(stepName, item.Name, ((FieldOutput)item));
            }
            else if (item is BoundaryCondition)
            {
                ReplaceBoundaryCondition(stepName, item.Name, ((BoundaryCondition)item));
            }
            else if (item is Load)
            {
                ReplaceLoad(stepName, item.Name, ((Load)item));
            }
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


        // Select
        #region Selection  #########################################################################################################
        // the function called from vtk_control
        public void SelectPointOrArea(double[] pickedPoint, double[][] planeParameters, vtkSelectOperation selectOperation)
        {
            try
            {
                if (_selectBy == vtkSelectBy.Id) return;

                if (pickedPoint == null && planeParameters == null) ClearSelectionHistory();   // empty pick - clear
                else
                {
                    vtkSelectBy selectBy = _selectBy;

                    // Querry
                    if (_selectBy == vtkSelectBy.QueryNode || 
                        _selectBy == vtkSelectBy.QueryElement ||
                        _selectBy == vtkSelectBy.QueryPart)
                    {
                        selectOperation = vtkSelectOperation.Add;
                    }
                    else // Set creation
                    {
                        // new pick - clear
                        if (selectOperation == vtkSelectOperation.None) ClearSelectionHistory(); 
                    }

                    SelectionNode selectionNode;
                    if (_selectBy == vtkSelectBy.Geometry)
                    {
                        // Get selected geometry ID
                        int id = GetGeometryId(pickedPoint);
                        selectionNode = new SelectionNodeIds(selectOperation, false, new int[] { id })
                                                            { GeometryIds = true};
                    }
                    else
                    {
                        // Mouse selection
                        selectionNode = new SelectionNodeMouse(pickedPoint, planeParameters, selectOperation, _selectBy, _selectAngle);
                    }

                    AddSelectionNode(selectionNode, true);
                }
            }
            catch
            {
            }
        }

        public void AddSelectionNode(SelectionNode node, bool highlight)
        {
            // check for errors    
            int[] ids = GetIdsFromSelectionNode(node, new HashSet<int>());
            if (node is SelectionNodeIds)
            {
                SelectionNodeIds selectionNodeIds = node as SelectionNodeIds;
                if (!selectionNodeIds.SelectAll)
                {
                    FeMesh mesh = DisplayedMesh;
                    
                    if (_selection.SelectItem == vtkSelectItem.Node)
                    {
                        for (int i = 0; i < ids.Length; i++)
                        {
                            if (!mesh.Nodes.ContainsKey(ids[i])) throw new CaeGlobals.CaeException("The selected node id does not exist.");
                        }
                    }
                    else if (_selection.SelectItem == vtkSelectItem.Element)
                    {
                        for (int i = 0; i < ids.Length; i++)
                        {
                            if (!mesh.Elements.ContainsKey(ids[i])) throw new CaeGlobals.CaeException("The selected element id does not exist.");
                        }
                    }
                    else if (_selection.SelectItem == vtkSelectItem.Surface)
                    {
                        for (int i = 0; i < ids.Length; i++)
                        {
                            mesh.GetCellFromFaceId(ids[i], out FeElement element); // Check: The selected face id does not exist."
                        }
                    }
                    else throw new NotSupportedException();
                }
                else ClearSelectionHistory();   // Before adding all clear selection
            }

            _selection.Add(node);
            if (highlight) HighlightSelection();
        }
        public void AddSelectionNodes(List<SelectionNode> nodes, bool highlight)
        {
            foreach (var node in nodes)
            {
                AddSelectionNode(node, false);
            }
            if (highlight) HighlightSelection();
        }
        public void RemoveLastSelectionNode(bool highlight)
        {
            _selection.RemoveLast();
            if (highlight) HighlightSelection();
        }
     
        public int[] GetSelectionIds()
        {
            // ids for:
            // nodes: global node ids
            // elements: global element ids
            // faces: 10 * global element ids + vtk face ids;   search: (% 10)
            HashSet<int> selectedIds = new HashSet<int>();
            int[] ids;
            
            foreach (SelectionNode node in _selection.Nodes)
            {
                ids = GetIdsFromSelectionNode(node, selectedIds);
                if (ids == null) continue;

                if (node.SelectOperation == vtkSelectOperation.None || node.SelectOperation == vtkSelectOperation.Invert)
                {
                    selectedIds.Clear();
                    selectedIds.UnionWith(ids);
                }
                else if (node.SelectOperation == vtkSelectOperation.Add)
                {
                    selectedIds.UnionWith(ids);
                }
                else if (node.SelectOperation == vtkSelectOperation.Subtract)
                {
                    selectedIds.ExceptWith(ids);
                }
                else if (node.SelectOperation == vtkSelectOperation.Intersect)
                {
                    selectedIds.IntersectWith(ids);
                }
            }

            ids = selectedIds.ToArray();

            return ids;
        }
        private int[] GetIdsFromSelectionNode(SelectionNode selectionNode, HashSet<int> selectedIds)
        {
            int[] ids = null;

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
                ids = GetIdsFromSelectionNodeMouse(selectionNodeMouse);
            }

            return ids;
        }
        private int[] GetIdsFromSelectionNodeInvert(SelectionNodeInvert selectionNodeInvert, HashSet<int> selectedIds)
        {
            HashSet<int> allIds = null;

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

            allIds.ExceptWith(selectedIds);

            return allIds.ToArray();
        }
        private int[] GetIdsFromSelectionNodeIds(SelectionNodeIds selectionNodeIds)
        {
            int[] ids = null;

            if (selectionNodeIds.SelectAll)
            {
                if (_selection.SelectItem == vtkSelectItem.Node) ids = GetVisibleNodeIds();
                else if (_selection.SelectItem == vtkSelectItem.Element) ids = GetVisibleElementIds();
                else if (_selection.SelectItem == vtkSelectItem.Surface) ids = GetVisibleFaceIds();
                else throw new NotSupportedException();
            }
            else
            {
                if (_selection.SelectItem == vtkSelectItem.Node || _selection.SelectItem == vtkSelectItem.Element
                    || _selection.SelectItem == vtkSelectItem.Surface)
                {
                    if (selectionNodeIds.GeometryIds)
                    {
                        ids = GetIdsFromGeometryId(selectionNodeIds.ItemIds[0], _selection.SelectItem);
                    }
                    else
                    {
                        ids = selectionNodeIds.ItemIds.ToArray();
                    }
                }
                else throw new NotSupportedException();
            }

            return ids;
        }
        private int[] GetIdsFromSelectionNodeMouse(SelectionNodeMouse selectionNodeMouse)
        {
            int[] ids = null;

            if (selectionNodeMouse.PickedPoint != null) // pick a 3D point
            {
                if (_selection.SelectItem == vtkSelectItem.None) 
                {}
                else if (_selection.SelectItem == vtkSelectItem.Node)
                {
                    ids = _form.GetNodeIdsAtPoint(selectionNodeMouse.PickedPoint, selectionNodeMouse.SelectBy, selectionNodeMouse.Angle);
                }
                else if (_selection.SelectItem == vtkSelectItem.Element)
                {
                    ids = _form.GetElementIdsAtPoint(selectionNodeMouse.PickedPoint, selectionNodeMouse.SelectBy, selectionNodeMouse.Angle);
                }
                else if (_selection.SelectItem == vtkSelectItem.Surface)
                {
                    // surface is based on node selection and then it is converted to face ids
                    ids = GetVisualizationFaceIdsFromPoint(selectionNodeMouse);
                }
                else if (_selection.SelectItem == vtkSelectItem.Part) 
                {
                    string partName = _form.GetPartNameAtPoint(selectionNodeMouse.PickedPoint);
                    ids = new int[] { DisplayedMesh.Parts[partName].PartId };
                }
                else throw new NotSupportedException();
            }
            else    // pick an area
            {
                if (_selection.SelectItem == vtkSelectItem.Node)
                {
                    ids = _form.GetNodeIdsFromFrustum(selectionNodeMouse.PlaneParameters, selectionNodeMouse.SelectBy);
                }
                else if (_selection.SelectItem == vtkSelectItem.Element)
                {
                    ids = _form.GetElementIdsFromFrustum(selectionNodeMouse.PlaneParameters, selectionNodeMouse.SelectBy);
                }
                else if (_selection.SelectItem == vtkSelectItem.Surface)
                {
                    ids = GetVisualizationFaceIdsFromArea(selectionNodeMouse);
                }
                else throw new NotSupportedException();
            }

            return ids;
        }

        private int[] GetVisualizationFaceIdsFromPoint(SelectionNodeMouse selectionNodeMouse)
        {
            int[] elementIds;
            int[] ids = _form.GetNodeIdsAtPoint(selectionNodeMouse.PickedPoint, selectionNodeMouse.SelectBy, selectionNodeMouse.Angle);

            if (selectionNodeMouse.SelectBy == vtkSelectBy.Node)
            {
                elementIds = GetElementIdsFromNodeIds(ids, false, false, false);
                ids = GetVisualizationFaceIds(ids, elementIds, false, false);
            }
            else if (selectionNodeMouse.SelectBy == vtkSelectBy.Element)
            {
                elementIds = GetElementIdsFromNodeIds(ids, false, false, true);
                ids = GetVisualizationFaceIds(ids, elementIds, false, true);
            }
            else if (selectionNodeMouse.SelectBy == vtkSelectBy.Part)
            {
                elementIds = GetElementIdsFromNodeIds(ids, false, true, false);
                ids = GetVisualizationFaceIds(ids, elementIds, false, true);
            }
            else if (selectionNodeMouse.SelectBy == vtkSelectBy.Edge ||
                     selectionNodeMouse.SelectBy == vtkSelectBy.EdgeAngle)
            {
                elementIds = GetElementIdsFromNodeIds(ids, true, false, false);
                ids = GetVisualizationFaceIds(ids, elementIds, true, false);
            }
            else if (selectionNodeMouse.SelectBy == vtkSelectBy.Surface ||
                     selectionNodeMouse.SelectBy == vtkSelectBy.SurfaceAngle)
            {
                elementIds = GetElementIdsFromNodeIds(ids, false, true, false);
                ids = GetVisualizationFaceIds(ids, elementIds, false, true);
            }
            return ids;
        }
        private int[] GetVisualizationFaceIdsFromArea(SelectionNodeMouse selectionNodeMouse)
        {
            int[] ids = null;
            // create surface by area selecting nodes or elements
            ids = _form.GetElementIdsFromFrustum(selectionNodeMouse.PlaneParameters, selectionNodeMouse.SelectBy);
            ids = DisplayedMesh.GetVisualizationFaceIds(null, ids, false, false);
            return ids;
        }


        #endregion #################################################################################################################

        #region Extraction  ########################################################################################################
        public vtkControl.vtkMaxActorData GetNodeActorData(int[] nodeIds)
        {
            vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
            data.Actor.Nodes.Coor = new double[nodeIds.Length][];

            if (_currentView == ViewGeometryModelResults.Geometry && _model.Geometry != null)
            {
                for (int i = 0; i < nodeIds.Length; i++)
                {
                    data.Actor.Nodes.Coor[i] = _model.Geometry.Nodes[nodeIds[i]].Coor;
                }
            }
            else if (_currentView == ViewGeometryModelResults.Model && _model.Mesh != null)
            {
                for (int i = 0; i < nodeIds.Length; i++)
                {
                    data.Actor.Nodes.Coor[i] = _model.Mesh.Nodes[nodeIds[i]].Coor;
                }
            }
            else
            {
                float scale = GetScale();
                Results.GetScaledNodesAndValues(_currentFieldData, scale, nodeIds, out data.Actor.Nodes.Coor, out data.Actor.Nodes.Values);
            }

            return data;
        }
        public vtkControl.vtkMaxActorData GetCellActorData(int[] elementIds, int[] nodeIds)
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

            //vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
            //int[][] freeEdges = DisplayedMesh.GetFreeEdgesFromVisualizationCells(cells.ToArray());

            //DisplayedMesh.GetNodesAndCellsForEdges(freeEdges, out data.Actor.Nodes.Ids, out data.Actor.Nodes.Coor,
            //                                       out data.Actor.Cells.CellNodeIds, out data.Actor.Cells.Types);
            //return data;
        }
        public vtkControl.vtkMaxActorData GetCellActorData(FeGroup elementSet)
        {
            vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
            if (_currentView == ViewGeometryModelResults.Geometry && _model.Geometry != null)
            {
                _model.Geometry.GetAllNodesAndCells(elementSet, out data.Actor.Nodes.Ids, out data.Actor.Nodes.Coor, out data.Actor.Cells.Ids,
                                                    out data.Actor.Cells.CellNodeIds, out data.Actor.Cells.Types);
            }
            else if (_currentView == ViewGeometryModelResults.Model && _model.Mesh != null)
            {
                _model.Mesh.GetAllNodesAndCells(elementSet, out data.Actor.Nodes.Ids, out data.Actor.Nodes.Coor, out data.Actor.Cells.Ids,
                                                out data.Actor.Cells.CellNodeIds, out data.Actor.Cells.Types);
            }
            else
            {
                float scale = GetScale();
                PartExchangeData actorResultData = _results.GetAllScaledNodesCellsAndValues(elementSet, _currentFieldData, scale);
                data = GetVtkData(actorResultData, null, null);
            }

            return data;
        }
        public vtkControl.vtkMaxActorData GetCellFaceActorData(int elementId, int[] nodeIds)
        {
            // get all faces containing at least 1 node id
            int[] faceIds = GetVisualizationFaceIds(nodeIds, new int[] { elementId }, false, false);

            bool add;
            int[] cell = null;
            FeElement element = null;
            HashSet<int> hashCell;

            // find a face containing all node ids
            foreach (int faceId in faceIds)
            {
                cell = DisplayedMesh.GetCellFromFaceId(faceId, out element);
                if (cell.Length < nodeIds.Length) continue;

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
            
            // get coordinates
            double[][] nodeCoor = new double[cell.Length][];
            for (int i = 0; i < cell.Length; i++) nodeCoor[i] = DisplayedMesh.Nodes[cell[i]].Coor;
            // renumber cell node ids
            int[][] cells = new int[1][];
            cells[0] = new int[cell.Length];
            for (int i = 0; i < cell.Length; i++) cells[0][i] = i;
            // get cell type
            int cellTypes;
            if (cell.Length == 3) cellTypes = (int)vtkCellType.VTK_TRIANGLE;
            else if (cell.Length == 4) cellTypes = (int)vtkCellType.VTK_QUAD;
            else if (cell.Length == 6) cellTypes = (int)vtkCellType.VTK_QUADRATIC_TRIANGLE;
            else if (cell.Length == 8) cellTypes = (int)vtkCellType.VTK_QUADRATIC_QUAD;
            else throw new NotSupportedException();

            vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
            data.Actor.Nodes.Ids = cell;
            data.Actor.Nodes.Coor = nodeCoor;
            data.Actor.Cells.CellNodeIds = cells;
            
            data.Actor.Cells.Types = new int[] { cellTypes };
            return data;
        }
        public vtkControl.vtkMaxActorData GetEdgeActorData(int elementId, int[] edgeNodeIds)
        {
            int[][] edgeCells = DisplayedMesh.GetEdgeCells(elementId, edgeNodeIds);
            if (edgeCells != null)
            {
                vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
                DisplayedMesh.GetNodesAndCellsForEdges(edgeCells, out data.Actor.Nodes.Ids, out data.Actor.Nodes.Coor,
                                                       out data.Actor.Cells.CellNodeIds, out data.Actor.Cells.Types);
                return data;
            }
            else return null;
        }
        public vtkControl.vtkMaxActorData GetFaceActorDataByFaceIds(int[] faceIds)
        {
            int[][] cells = new int[faceIds.Length][];
            int count = 0;
            FeMesh mesh = DisplayedMesh;        // it is used in loop
            foreach (int faceId in faceIds)
            {
                cells[count++] = mesh.GetCellFromFaceId(faceId, out FeElement element);
            }

            vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
            data.Actor.Cells.CellNodeIds = cells;
            DisplayedMesh.GetSurfaceGeometry(cells, out data.Actor.Nodes.Coor, out data.Actor.Cells.Types);

            return data;
        }
        public vtkControl.vtkMaxActorData GetSurfaceActorDataByNodeIds(int[] nodeIds)
        {
            int[] elementIds = GetElementIdsFromNodeIds(nodeIds, false, true, false);
            int[] faceIds = GetVisualizationFaceIds(nodeIds, elementIds, false, true);

            int[][] cells = new int[faceIds.Length][];
            int count = 0;
            foreach (int faceId in faceIds)
            {
                cells[count] = DisplayedMesh.GetCellFromFaceId(faceId, out FeElement element);
                count++;
            }

            vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
            int[][] freeEdges = DisplayedMesh.GetFreeEdgesFromVisualizationCells(cells);

            DisplayedMesh.GetNodesAndCellsForEdges(freeEdges, out data.Actor.Nodes.Ids, out data.Actor.Nodes.Coor,
                                                   out data.Actor.Cells.CellNodeIds, out data.Actor.Cells.Types);
            return data;
        }
        public vtkControl.vtkMaxActorData GetSurfaceEdgeActorData(int elementId, int[] cellFaceNodeIds)
        {
            // from element id and node ids get surface id and from surface id get free edges !!!
            BasePart part;
            int faceId;
            if (DisplayedMesh.GetFaceId(elementId, cellFaceNodeIds, out part, out faceId))
            {
                List<int[]> edgeCells = new List<int[]>();
                int edgeId;
                int edgeCellId;
                for (int i = 0; i < part.Visualization.FaceEdgeIds[faceId].Length; i++)
                {
                    edgeId = part.Visualization.FaceEdgeIds[faceId][i];
                    for (int j = 0; j < part.Visualization.EdgeCellIdsByEdge[edgeId].Length; j++)
                    {
                        edgeCellId = part.Visualization.EdgeCellIdsByEdge[edgeId][j];
                        edgeCells.Add(part.Visualization.EdgeCells[edgeCellId]);
                    }
                }

                vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
                DisplayedMesh.GetNodesAndCellsForEdges(edgeCells.ToArray(), out data.Actor.Nodes.Ids, out data.Actor.Nodes.Coor,
                                                       out data.Actor.Cells.CellNodeIds, out data.Actor.Cells.Types);
                return data;
            }
            else return null;
        }
        public vtkControl.vtkMaxActorData GetPartActorData(int[] elementIds)
        {
            FeMesh mesh = DisplayedMesh;

            HashSet<int> partIds = new HashSet<int>();
            for (int i = 0; i < elementIds.Length; i++)
            {
                partIds.Add(mesh.Elements[elementIds[i]].PartId);
            }

            List<int> allElementIds = new List<int>();
            foreach (var entry in mesh.Parts)
            {
                if (partIds.Contains(entry.Value.PartId))
                {
                    allElementIds.AddRange(entry.Value.Labels);
                }
            }

            FeGroup elementSet = new FeGroup("tmp", allElementIds.ToArray());

            return GetCellActorData(elementSet);
        }
        public vtkControl.vtkMaxActorData GetGeometryActorData(double[] point, double dist, int elementId, int[] edgeNodeIds, int[] cellFaceNodeIds)
        {
            int geomId = DisplayedMesh.GetGeometryId(point, dist, elementId, edgeNodeIds, cellFaceNodeIds);
            int typeId = (geomId / 10000) % 10;
            int itemId = geomId / 100000;

            if (typeId == 1)
            {
                int[] nodeIds = DisplayedMesh.GetNodeIdsFromGeometryId(geomId);
                return GetNodeActorData(nodeIds);
            }
            else if (typeId == 2) return GetEdgeActorData(elementId, edgeNodeIds);
            else if (typeId == 3) return GetSurfaceEdgeActorData(elementId, cellFaceNodeIds);
            else throw new NotSupportedException();
        }

        public int[] GetPartNodeIds(int elementId)
        {
            return DisplayedMesh.GetPartNodeIds(elementId);
        }
        public int[] GetPartElementIds(int elementId)
        {
            return DisplayedMesh.GetPartElementIds(elementId);
        }
        public int[] GetEdgeNodeIds(int elementId, int[] edgeGlobalNodeIds)
        {
            return DisplayedMesh.GetEdgeNodeIds(elementId, edgeGlobalNodeIds);
        }
        public int[] GetSurfaceNodeIds(int elementId, int[] cellFaceGlobalNodeIds)
        {
            return DisplayedMesh.GetSurfaceNodeIds(elementId, cellFaceGlobalNodeIds);
        }
        public int[] GetEdgeByAngleNodeIds(int elementId, int[] edgeGlobalNodeIds, double angle)
        {
            return DisplayedMesh.GetEdgeByAngleNodeIds(elementId, edgeGlobalNodeIds, angle);
        }
        public int[] GetSurfaceByAngleNodeIds(int elementId, int[] cellFaceGlobalNodeIds, double angle)
        {
            return DisplayedMesh.GetSurfaceByAngleNodeIds(elementId, cellFaceGlobalNodeIds, angle);
        }
        
        private int GetGeometryId(double[] point)
        {
            double dist;
            int elementId;
            int[] edgeNodeIds;
            int[] cellFaceNodeIds;

            _form.GetGeometryPickProperties(point, out dist, out elementId,
                                            out edgeNodeIds, out cellFaceNodeIds);
            int id = DisplayedMesh.GetGeometryId(point, dist, elementId, 
                                                 edgeNodeIds, cellFaceNodeIds);

            return id;
        }

        private int[] GetIdsFromGeometryId(int geometryId, vtkSelectItem selectItem)
        {
            int typeId = (geometryId / 10000) % 10;
            int itemId = geometryId / 100000;

            int[] nodeIds = DisplayedMesh.GetNodeIdsFromGeometryId(geometryId);

            if (selectItem == vtkSelectItem.Node)
            {
                return nodeIds;
            }
            else if (selectItem == vtkSelectItem.Element)
            {
                return GetElementIdsFromNodeIds(nodeIds, false, false, false);
            }
            else if (selectItem == vtkSelectItem.Surface)
            {
                int[] elementsIds = GetElementIdsFromNodeIds(nodeIds, false, true, false);
                return GetVisualizationFaceIds(nodeIds, elementsIds, false, true);
            }
            else throw new NotSupportedException();
        }
        
        public int[] GetElementIdsFromNodeIds(int[] nodeIds, bool containsEdge, bool containsFace, bool containsElement)
        {
            return DisplayedMesh.GetElementIdsFromNodeIds(nodeIds, containsEdge, containsFace, containsElement);
        }
        public int[] GetVisualizationFaceIds(int[] nodeIds, int[] elementIds, bool containsEdge, bool containsFace)
        {
            return DisplayedMesh.GetVisualizationFaceIds(nodeIds, elementIds, containsEdge, containsFace);
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
        public string[] CheckAndUpdateValidity()
        {
            // Update user keywords
            if (_model != null && _model.CalculixUserKeywords != null)
            {
                int num = _model.CalculixUserKeywords.Count;
                _model.RemoveLostUserKeywords(_form.SetNumberOfModelUserKeywords);
                int delta = num - _model.CalculixUserKeywords.Count;
                if (delta > 0) MessageBox.Show("Number of removed CalculiX user keywords: " + delta, "Warning", MessageBoxButtons.OK);
            }
            // Tuple<NamedClass, string>   ...   Tuple<invalidItem, stepName>
            List<Tuple<NamedClass, string>> items = new List<Tuple<NamedClass, string>>();
            string[] invalidItems = _model.CheckValidity(items);
            foreach (var entry in items)
            {
                _form.UpdateTreeNode(ViewGeometryModelResults.Model, entry.Item1.Name, entry.Item1, entry.Item2, false);
            }
            return invalidItems;
        }

        // Common
        public string[] GetAllMeshEntityNames()
        {
            List<string> names = new List<string>();
            names.AddRange(_model.Mesh.Parts.Keys);
            names.AddRange(_model.Mesh.NodeSets.Keys);
            names.AddRange(_model.Mesh.ElementSets.Keys);
            names.AddRange(_model.Mesh.Surfaces.Keys);
            names.AddRange(_model.Mesh.ReferencePoints.Keys);
            return names.ToArray();
        }
        public double[] GetBoundingBox()
        {
            // xmin, xmax, ymin, ymax, zmin, zmax
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


        // Results                                          
        
        public FeNode GetScaledNode(float scale, int nodeId)
        {
            if (_currentView == ViewGeometryModelResults.Results)
            {
                FeNode node = _results.Mesh.Nodes[nodeId].DeepClone();
                double[][] coor = new double[][] { node.Coor };
                _results.ScaleNodeCoordinates(scale, _currentFieldData.StepId, _currentFieldData.StepIncrementId, new int[] { nodeId }, ref coor);
                node.X = coor[0][0];
                node.Y = coor[0][1];
                node.Z = coor[0][2];
                return node;
            }
            return new FeNode();
        }
        public float GetNodalValue(int nodeId)
        {
            float[] values = _results.GetValues(_currentFieldData, new int[] { nodeId });
            return values[0];
        }

        // Visualize
        #region Draw  ##############################################################################################################
        // Update model stat
        public void Update(UpdateType updateType)
        {
            if (updateType.HasFlag(UpdateType.Check)) CheckAndUpdateValidity(); // first check the validity to correctly draw the symbols
            if (updateType.HasFlag(UpdateType.DrawMesh)) DrawMesh(updateType.HasFlag(UpdateType.ResetCamera));
            if (updateType.HasFlag(UpdateType.RedrawSymbols)) RedrawSymbols();
        }
        // Geometry mesh
        public void DrawGeometry(bool resetCamera)
        {
            try
            {
                _form.Clear3D();

                if (_model != null)
                {
                    if (_model.Geometry != null && _model.Geometry.Parts.Count > 0)
                    {
                        CurrentView = ViewGeometryModelResults.Geometry;
                        DrawAllGeomParts();
                    }
                    UpdateHighlightFromTree();
                }

                if (resetCamera) _form.SetFrontBackView(false, true);
                _form.AdjustCameraDistanceAndClipping();
            }
            catch
            {
                // do not throw an error - it might cancel a procedure
            }
        }
        public void DrawAllGeomParts()
        {
            if (_model == null) return;

            vtkControl.vtkRendererLayer layer = vtkControl.vtkRendererLayer.Base;
            List<string> hiddenActors = new List<string>();

            foreach (var entry in _model.Geometry.Parts)
            {
                DrawGeomPart(_model.Geometry, entry.Value, layer, true, true);
                if (!entry.Value.Visible) hiddenActors.Add(entry.Key);
            }
            if (hiddenActors.Count > 0) _form.HideActors(hiddenActors.ToArray(), false);
        }
        private void DrawGeomPart(FeMesh mesh, BasePart part, vtkControl.vtkRendererLayer layer, bool canHaveElementEdges, bool pickable)
        {
            System.Drawing.Color color = part.Color;
            foreach (var elType in part.ElementTypes)
            {
                if (elType == typeof(LinearBeamElement) || elType == typeof(ParabolicBeamElement)) color = System.Drawing.Color.Black;
            }

            vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
            data.Name = part.Name;
            data.Color = color;
            data.Layer = layer;
            data.CanHaveElementEdges = canHaveElementEdges;
            data.Pickable = pickable;
            data.SmoothShaded = part.SmoothShaded;

            // get all nodes and elements - renumbered
            if (pickable)
            {
                data.CellLocator = new PartExchangeData();
                mesh.GetAllNodesAndCells(part, out data.CellLocator.Nodes.Ids, out data.CellLocator.Nodes.Coor, out data.CellLocator.Cells.Ids,
                                         out data.CellLocator.Cells.CellNodeIds, out data.CellLocator.Cells.Types);
            }

            // get only needed nodes and elements - renumbered
            mesh.GetVisualizationNodesAndCells(part, out data.Actor.Nodes.Ids, out data.Actor.Nodes.Coor, out data.Actor.Cells.Ids,
                                        out data.Actor.Cells.CellNodeIds, out data.Actor.Cells.Types);

            // model edges
            if ((part.PartType == PartType.Solid || part.PartType == PartType.Shell) && part.Visualization.EdgeCells != null)
            {
                data.ModelEdges = new PartExchangeData();
                mesh.GetNodesAndCellsForModelEdges(part, out data.ModelEdges.Nodes.Ids, out data.ModelEdges.Nodes.Coor, out data.ModelEdges.Cells.CellNodeIds,
                                                   out data.ModelEdges.Cells.Types);
            }

            ApplyLighting(data);
            _form.Add3DCells(data);
        }

        // Mesh
        public void DrawMesh(bool resetCamera)
        {
            try
            {
                _form.Clear3D();
                if (_model != null)
                {
                    if (_model.Mesh != null && _model.Mesh.Parts.Count > 0)
                    {
                        try // must be inside to continue screen update
                        {
                            if (_currentView != ViewGeometryModelResults.Model) CurrentView = ViewGeometryModelResults.Model;
                            DrawAllMeshParts();
                            DrawSymbols();
                        }
                        catch { }
                    }
                    UpdateHighlightFromTree();
                }

                if (resetCamera) _form.SetFrontBackView(false, true);
                _form.AdjustCameraDistanceAndClipping();
            }
            catch
            {
                // do not throw an error - it might cancel a procedure
            }
        }
        public void DrawAllMeshParts()
        {
            if (_model == null) return;

            Dictionary<string, BasePart> parts = _model.Mesh.Parts;
            vtkControl.vtkRendererLayer layer = vtkControl.vtkRendererLayer.Base;
            
            List<string> hiddenActors = new List<string>();

            foreach (var entry in parts)
            {
                DrawMeshPart(_model.Mesh, entry.Value, layer);

                if (!entry.Value.Visible) hiddenActors.Add(entry.Key);
            }
            if (hiddenActors.Count > 0) _form.HideActors(hiddenActors.ToArray(), false);
        }
        public void DrawMeshPart(FeMesh mesh, BasePart part, vtkControl.vtkRendererLayer layer)
        {
            vtkControl.vtkMaxActorData data;
            System.Drawing.Color color = part.Color;
            foreach (var elType in part.ElementTypes)
            {
                if (elType == typeof(LinearBeamElement) || elType == typeof(ParabolicBeamElement)) color = System.Drawing.Color.Black;
            }

            data = new vtkControl.vtkMaxActorData();
            data.Name = part.Name;
            data.Color = color;
            data.Layer = layer;
            data.CanHaveElementEdges = true;
            data.Pickable = true;
            data.SmoothShaded = part.SmoothShaded;

            // get all nodes and elements for selection - renumbered
            data.CellLocator = new PartExchangeData();
            mesh.GetAllNodesAndCells(part, out data.CellLocator.Nodes.Ids, out data.CellLocator.Nodes.Coor, out data.CellLocator.Cells.Ids, 
                                     out data.CellLocator.Cells.CellNodeIds, out data.CellLocator.Cells.Types);



            // get only needed nodes and elements - renumbered
            mesh.GetVisualizationNodesAndCells(part, out data.Actor.Nodes.Ids, out data.Actor.Nodes.Coor, out data.Actor.Cells.Ids, 
                                        out data.Actor.Cells.CellNodeIds, out data.Actor.Cells.Types);
            
            // model edges
            if (((part.PartType == PartType.Solid || part.PartType == PartType.Shell) && part.Visualization.EdgeCells != null) ||
                  part.PartType == PartType.Wire)
            {
                data.ModelEdges = new PartExchangeData();
                mesh.GetNodesAndCellsForModelEdges(part, out data.ModelEdges.Nodes.Ids, out data.ModelEdges.Nodes.Coor, 
                                                   out data.ModelEdges.Cells.CellNodeIds, out data.ModelEdges.Cells.Types);
            }

            ApplyLighting(data);
            _form.Add3DCells(data);
        }

        // Symbols
        public void DrawSymbols()
        {
            if (_drawSymbolsForStep != null && _drawSymbolsForStep != "None")
            {

                DrawAllReferencePoints();
                DrawAllConstraints();
                if (_drawSymbolsForStep != "Model")
                {
                    DrawAllBoundaryConditions(_drawSymbolsForStep);
                    DrawAllLoads(_drawSymbolsForStep);
                }
            }
        }
        public void RedrawSymbols()
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

                        try // must be inside to continue screen update
                        {
                            if (_currentView != ViewGeometryModelResults.Model) CurrentView = ViewGeometryModelResults.Model;
                            DrawSymbols();
                        }
                        catch { }

                        UpdateHighlightFromTree();
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
            System.Drawing.Color color = System.Drawing.Color.FromArgb(0, 255, 255, 0);     // yellow
            vtkControl.vtkRendererLayer layer = vtkControl.vtkRendererLayer.Overlay;

            foreach (var entry in _model.Mesh.ReferencePoints)
            {
                DrawReferencePoint(entry.Key, color, layer);
            }
        }
        public void DrawReferencePoint(string referencePointName, System.Drawing.Color color, vtkControl.vtkRendererLayer layer, int nodeSize = 10)
        {
            try
            {
                FeReferencePoint rp;
                if (_model.Mesh.ReferencePoints.TryGetValue(referencePointName, out rp) && rp.Active && rp.Visible && rp.Valid)
                {
                    System.Drawing.Color colorBorder = System.Drawing.Color.Black;

                    vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
                    data.NodeSize = nodeSize;
                    data.Color = colorBorder;
                    data.Layer = layer;
                    data.Actor.Nodes.Coor = new double[][] { rp.Coor() };
                    ApplyLighting(data);
                    _form.Add3DNodes(data);

                    data.NodeSize = nodeSize - 2;
                    data.Color = color;
                    ApplyLighting(data);
                    _form.Add3DNodes(data);
                }
            }
            catch { } // do not show the exception to the user
        }

        // Constraints
        public void DrawAllConstraints()
        {
            PreSettings preSettings = (PreSettings)_settings[Globals.PreSettingsName];
            System.Drawing.Color color = preSettings.ConstraintSymbolColor;
            int nodeSymbolSize = preSettings.NodeSymbolSize;
            vtkControl.vtkRendererLayer layer = vtkControl.vtkRendererLayer.Base;

            foreach (var entry in _model.Constraints)
            {
                DrawConstraint(entry.Value, color, nodeSymbolSize, layer);
            }
        }
        public void DrawConstraint(Constraint constraint, System.Drawing.Color color, int nodeSymbolSize, vtkControl.vtkRendererLayer layer)
        {
            try
            {
                if (!(constraint.Active && constraint.Visible && constraint.Valid)) return;

                string prefixName = "CONSTRAINT" + Globals.NameSeparator + constraint.Name;
                vtkControl.vtkRendererLayer symbolLayer = layer == vtkControl.vtkRendererLayer.Selection ? layer : vtkControl.vtkRendererLayer.Overlay;
                string nodeSetName;

                if (constraint is RigidBody rb)
                {
                    if (!_model.Mesh.ReferencePoints.ContainsKey(rb.ReferencePointName)) return;

                    if (rb.RegionType == RegionTypeEnum.NodeSetName) nodeSetName = rb.RegionName;
                    else if (rb.RegionType == RegionTypeEnum.SurfaceName)
                    {
                        if (!_model.Mesh.Surfaces.ContainsKey(rb.RegionName)) return;
                        nodeSetName = _model.Mesh.Surfaces[rb.RegionName].NodeSetName;
                    }
                    else throw new NotSupportedException();
                    DrawNodeSet(prefixName, nodeSetName, color, layer, nodeSymbolSize);
                    DrawRigidBodySymbol(rb, color, symbolLayer);
                }
                else if (constraint is Tie t)
                {
                    if (!_model.Mesh.Surfaces.ContainsKey(t.SlaveSurfaceName) || !_model.Mesh.Surfaces.ContainsKey(t.MasterSurfaceName)) return;

                    nodeSetName = _model.Mesh.Surfaces[t.SlaveSurfaceName].NodeSetName;
                    DrawNodeSet(prefixName + Globals.NameSeparator + "Slave", nodeSetName, color, layer, nodeSymbolSize);

                    //nodeSetName = _model.Mesh.Surfaces[t.MasterSurfaceName].NodeSetName;
                    //DrawNodeSet(prefixName + Globals.NameSeparator + "Master", nodeSetName, color, layer, nodeSymbolSize);
                    DrawSurface(prefixName, t.MasterSurfaceName, color, layer, true);
                }
                else throw new NotSupportedException();
            }
            catch { } // do not show the exception to the user
        }
        public void DrawRigidBodySymbol(RigidBody rigidBody, System.Drawing.Color color, vtkControl.vtkRendererLayer layer)
        {
            int[][] cells;
            int[] cellsTypes;
            double[][] nodeCoor;
            double[][] distributedNodeCoor;
            bool canHaveEdges = false;

            if (!GetReferencePointNames().Contains(rigidBody.ReferencePointName)) return;

            // node set
            string nodeSetName;
            if (rigidBody.RegionType == RegionTypeEnum.NodeSetName) nodeSetName = rigidBody.RegionName;
            else if (rigidBody.RegionType == RegionTypeEnum.SurfaceName) nodeSetName = _model.Mesh.Surfaces[rigidBody.RegionName].NodeSetName;
            else throw new NotSupportedException();

            if (_model.Mesh.NodeSets.ContainsKey(nodeSetName))
            {
                FeNodeSet nodeSet = _model.Mesh.NodeSets[nodeSetName];
                // all nodes
                nodeCoor = _model.Mesh.GetNodeSetCoor(nodeSet.Labels);
                // ids go from 0 to Length
                int[] distributedIds = GetSpatiallyEquallyDistributedCoor(nodeCoor, 3);
                // distributed nodes
                distributedNodeCoor = new double[distributedIds.Length][];
                for (int i = 0; i < distributedIds.Length; i++) distributedNodeCoor[i] = nodeCoor[distributedIds[i]];

                // create wire elements
                // distributed coor +1 for reference point
                nodeCoor = new double[distributedIds.Length + 1][];
                nodeCoor[0] = GetReferencePoint(rigidBody.ReferencePointName).Coor();
                for (int i = 0; i < distributedIds.Length; i++) nodeCoor[i + 1] = distributedNodeCoor[i];

                cells = new int[distributedIds.Length][];
                cellsTypes = new int[distributedIds.Length];
                LinearBeamElement element = new LinearBeamElement(0, null);
                for (int i = 0; i < distributedIds.Length; i++)
                {
                    cells[i] = new int[] { 0, i + 1 };
                    cellsTypes[i] = element.GetVtkCellType();
                }

                if (cells.Length > 0)
                {
                    vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
                    data.Name = rigidBody.Name + "_lines";
                    data.Color = color;
                    data.Layer = layer;
                    data.CanHaveElementEdges = canHaveEdges;
                    data.Pickable = false;
                    data.Actor.Nodes.Ids = null;
                    data.Actor.Nodes.Coor = nodeCoor.ToArray();
                    data.Actor.Cells.CellNodeIds = cells;
                    data.Actor.Cells.Types = cellsTypes;
                    ApplyLighting(data);
                    _form.Add3DCells(data);
                }
            }
        }
        
        // BCs
        public void DrawAllBoundaryConditions(string stepName)
        {
            PreSettings preSettings = (PreSettings)_settings[Globals.PreSettingsName];
            System.Drawing.Color color = preSettings.BoundaryConditionSymbolColor;
            int symbolSize = preSettings.SymbolSize;
            int nodeSymbolSize = preSettings.NodeSymbolSize;
            vtkControl.vtkRendererLayer layer = vtkControl.vtkRendererLayer.Base;

            foreach (var step in _model.StepCollection.StepsList)
            {
                if (step.Name == stepName)
                {
                    foreach (var bcEntry in step.BoundaryConditions)
                    {
                        DrawBoundaryCondition(step.Name, bcEntry.Value, color, symbolSize, nodeSymbolSize, layer);
                    }
                    break;
                }
            }
        }
        public void DrawBoundaryCondition(string stepName, BoundaryCondition boundaryCondition, System.Drawing.Color color, int symbolSize, 
                                          int nodeSymbolSize, vtkControl.vtkRendererLayer layer)
        {
            try
            {
                if (!(boundaryCondition.Active && boundaryCondition.Visible && boundaryCondition.Valid)) return;

                double[][] coor = null;
                string prefixName = stepName + Globals.NameSeparator + "BC" + Globals.NameSeparator + boundaryCondition.Name;
                vtkControl.vtkRendererLayer symbolLayer = layer == vtkControl.vtkRendererLayer.Selection ? layer : vtkControl.vtkRendererLayer.Overlay;

                if (boundaryCondition is DisplacementRotation)
                {
                    DisplacementRotation dispRot = boundaryCondition as DisplacementRotation;
               
                    if (dispRot.RegionType == RegionTypeEnum.NodeSetName)
                    {
                        if (!_model.Mesh.NodeSets.ContainsKey(dispRot.RegionName)) return;
                        FeNodeSet nodeSet = _model.Mesh.NodeSets[dispRot.RegionName];
                        coor = new double[1][];
                        coor[0] = nodeSet.CenterOfGravity;

                        DrawNodeSet(prefixName, nodeSet.Name, color, layer, nodeSymbolSize);
                    }
                    else if (dispRot.RegionType == RegionTypeEnum.SurfaceName)
                    {
                        if (!_model.Mesh.Surfaces.ContainsKey(dispRot.RegionName)) return;
                        FeSurface surface = _model.Mesh.Surfaces[dispRot.RegionName];
                        coor = new double[1][];
                        coor[0] = _model.Mesh.NodeSets[surface.NodeSetName].CenterOfGravity;

                        //DrawNodeSet(prefixName, surface.NodeSetName, color, layer, nodeSymbolSize);
                        DrawSurface(prefixName, surface.Name, color, layer, true);
                    }
                    else if (dispRot.RegionType == RegionTypeEnum.ReferencePointName)
                    {
                        if (!_model.Mesh.ReferencePoints.ContainsKey(dispRot.RegionName)) return;
                        FeReferencePoint referencePoint = _model.Mesh.ReferencePoints[dispRot.RegionName];
                        coor = new double[1][];
                        coor[0] = referencePoint.Coor();
                    }
                    else throw new NotSupportedException();
                    DrawDisplacementRotationSymbols(prefixName, dispRot, coor, color, symbolSize, symbolLayer);
                }
            }
            catch { } // do not show the exception to the user
        }
        public void DrawDisplacementRotationSymbols(string prefixName, DisplacementRotation dispRot, double[][] symbolCoor, System.Drawing.Color color,
                                                    int symbolSize, vtkControl.vtkRendererLayer layer)
        {
            if (!dispRot.Visible) return;

            // cones
            List<double[]> allCoor = new List<double[]>();
            List<double[]> allNormals = new List<double[]>();
            if (dispRot.U1 == 0)
            {
                double[] normalX = new double[] { 1, 0, 0 };
                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normalX);
                }
            }
            if (dispRot.U2 == 0)
            {
                double[] normalY = new double[] { 0, 1, 0 };
                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normalY);
                }
            }
            if (dispRot.U3 == 0)
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
                vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Actor.Nodes.Coor = allCoor.ToArray();
                data.Actor.Nodes.Normals = allNormals.ToArray();
                ApplyLighting(data);
                _form.AddOrientedDisplacementConstraintActor(data, symbolSize);                
            }

            // cylinders
            allCoor.Clear();
            allNormals.Clear();
            if (dispRot.UR1 == 0)
            {
                double[] normalX = new double[] { 1, 0, 0 };
                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normalX);
                }
            }
            if (dispRot.UR2 == 0)
            {
                double[] normalY = new double[] { 0, 1, 0 };
                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normalY);
                }
            }
            if (dispRot.UR3 == 0)
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
                vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Actor.Nodes.Coor = allCoor.ToArray();
                data.Actor.Nodes.Normals = allNormals.ToArray();
                ApplyLighting(data);
                _form.AddOrientedRotationalConstraintActor(data, symbolSize);
            }

            // arrows
            allCoor.Clear();
            allNormals.Clear();

            if ((!double.IsNaN(dispRot.U1) && dispRot.U1 != 0) || (!double.IsNaN(dispRot.U2) && dispRot.U2 != 0) || (!double.IsNaN(dispRot.U3) && dispRot.U3 != 0))
            {
                double[] normal = new double[3];
                if (!double.IsNaN(dispRot.U1) && dispRot.U1 != 0) normal[0] = dispRot.U1;
                if (!double.IsNaN(dispRot.U2) && dispRot.U2 != 0) normal[1] = dispRot.U2;
                if (!double.IsNaN(dispRot.U3) && dispRot.U3 != 0) normal[2] = dispRot.U3;

                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normal);
                }
            }
            if (allCoor.Count > 0)
            {
                vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Actor.Nodes.Coor = allCoor.ToArray();
                data.Actor.Nodes.Normals = allNormals.ToArray();
                ApplyLighting(data);
                _form.AddOrientedArrowsActor(data, symbolSize);
            }

            // double arrows
            allCoor.Clear();
            allNormals.Clear();
            if ((!double.IsNaN(dispRot.UR1) && dispRot.UR1 != 0) || (!double.IsNaN(dispRot.UR2) && dispRot.UR2 != 0) || (!double.IsNaN(dispRot.UR3) && dispRot.UR3 != 0))
            {
                double[] normal = new double[3];
                if (!double.IsNaN(dispRot.UR1) && dispRot.UR1 != 0) normal[0] = dispRot.UR1;
                if (!double.IsNaN(dispRot.UR2) && dispRot.UR2 != 0) normal[1] = dispRot.UR2;
                if (!double.IsNaN(dispRot.UR3) && dispRot.UR3 != 0) normal[2] = dispRot.UR3;

                for (int i = 0; i < symbolCoor.Length; i++)
                {
                    allCoor.Add(symbolCoor[i]);
                    allNormals.Add(normal);
                }
            }
            if (allCoor.Count > 0)
            {
                vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Actor.Nodes.Coor = allCoor.ToArray();
                data.Actor.Nodes.Normals = allNormals.ToArray();
                ApplyLighting(data);
                _form.AddOrientedDoubleArrowsActor(data, symbolSize);
            }
        }
        // Loads
        private void DrawAllLoads(string stepName)
        {
            PreSettings preSettings = (PreSettings)_settings[Globals.PreSettingsName];
            int symbolSize = preSettings.SymbolSize;
            int nodeSymbolSize = preSettings.NodeSymbolSize;
            System.Drawing.Color color = preSettings.LoadSymbolColor;
            vtkControl.vtkRendererLayer layer = vtkControl.vtkRendererLayer.Base;

            foreach (var step in _model.StepCollection.StepsList)
            {
                if (step.Name == stepName)
                {
                    foreach (var loadEntry in step.Loads)
                    {
                        DrawLoad(step.Name, loadEntry.Value, color, symbolSize, nodeSymbolSize, layer);
                    }
                    break;
                }
            }
        }
        public void DrawLoad(string stepName, Load load, System.Drawing.Color color, int symbolSize, int nodeSymbolSize, vtkControl.vtkRendererLayer layer)
        {
            try
            {
                if (!(load.Active && load.Visible && load.Valid)) return;

                double[][] coor = null;
                string prefixName = stepName + Globals.NameSeparator + "LOAD" + Globals.NameSeparator + load.Name;
                vtkControl.vtkRendererLayer symbolLayer = layer == vtkControl.vtkRendererLayer.Selection ? layer : vtkControl.vtkRendererLayer.Overlay;

                if (load is CLoad cLoad)
                {
                    if (cLoad.RegionType == RegionTypeEnum.NodeSetName)
                    {
                        if (!_model.Mesh.NodeSets.ContainsKey(cLoad.RegionName)) return;
                        FeNodeSet nodeSet = _model.Mesh.NodeSets[cLoad.RegionName];
                        coor = new double[nodeSet.Labels.Length][];
                        for (int i = 0; i < nodeSet.Labels.Length; i++) coor[i] = _model.Mesh.Nodes[nodeSet.Labels[i]].Coor;

                        DrawNodeSet(prefixName, nodeSet.Name, color, layer, nodeSymbolSize);
                    }
                    else if (cLoad.RegionType == RegionTypeEnum.ReferencePointName)
                    {
                        if (!_model.Mesh.ReferencePoints.ContainsKey(cLoad.RegionName)) return;
                        FeReferencePoint referencePoint = _model.Mesh.ReferencePoints[cLoad.RegionName];
                        coor = new double[1][];
                        coor[0] = referencePoint.Coor();
                    }
                    else throw new NotSupportedException();
                    DrawCLoadSymbols(prefixName, cLoad, coor, color, symbolSize, symbolLayer);
                }
                else if (load is MomentLoad momentLoad)
                {
                    if (momentLoad.RegionType == RegionTypeEnum.NodeSetName)
                    {
                        if (!_model.Mesh.NodeSets.ContainsKey(momentLoad.RegionName)) return;
                        FeNodeSet nodeSet = _model.Mesh.NodeSets[momentLoad.RegionName];
                        coor = new double[nodeSet.Labels.Length][];
                        for (int i = 0; i < nodeSet.Labels.Length; i++) coor[i] = _model.Mesh.Nodes[nodeSet.Labels[i]].Coor;

                        DrawNodeSet(prefixName, nodeSet.Name, color, layer, nodeSymbolSize);
                    }
                    else if (momentLoad.RegionType == RegionTypeEnum.ReferencePointName)
                    {
                        if (!_model.Mesh.ReferencePoints.ContainsKey(momentLoad.RegionName)) return;
                        FeReferencePoint referencePoint = _model.Mesh.ReferencePoints[momentLoad.RegionName];
                        coor = new double[1][];
                        coor[0] = referencePoint.Coor();
                    }
                    else throw new NotSupportedException();
                    DrawMomentLoadSymbols(prefixName, momentLoad, coor, color, symbolSize, symbolLayer);
                }
                else if (load is STLoad stLoad)
                {
                    if (!_model.Mesh.Surfaces.ContainsKey(stLoad.SurfaceName)) return;
                    FeNodeSet nodeSet = _model.Mesh.NodeSets[_model.Mesh.Surfaces[stLoad.SurfaceName].NodeSetName];
                    coor = new double[1][];
                    coor[0] = nodeSet.CenterOfGravity;

                    //DrawNodeSet(prefixName, nodeSet.Name, color, layer, nodeSymbolSize);
                    DrawSurface(prefixName, stLoad.SurfaceName, color, layer, true);
                    DrawSTLoadSymbols(prefixName, stLoad, coor, color, symbolSize, symbolLayer);
                }
                else if (load is DLoad dLoad)
                {
                    if (!_model.Mesh.Surfaces.ContainsKey(dLoad.SurfaceName)) return;

                    DrawSurface(prefixName, dLoad.SurfaceName, color, layer, true);
                    DrawDLoadSymbols(prefixName, dLoad, color, symbolSize, layer);
                }
                else if (load is GravityLoad gLoad)
                {
                    if (layer == vtkControl.vtkRendererLayer.Selection)
                    {
                        if (gLoad.RegionType == RegionTypeEnum.PartName) HighlightModelParts(new string[] { gLoad.RegionName });
                        else if (gLoad.RegionType == RegionTypeEnum.ElementSetName) HighlightElementSet(_model.Mesh.ElementSets[gLoad.RegionName], _model.Mesh);
                    }
                    FeNodeSet nodeSet = _model.Mesh.GetNodeSetFromPartOrElementSet(gLoad.RegionName);
                    DrawGravityLoadSymbol(prefixName, gLoad, nodeSet.CenterOfGravity, color, symbolSize, symbolLayer);
                }
                else if (load is CentrifLoad cfLoad)
                {
                    if (layer == vtkControl.vtkRendererLayer.Selection)
                    {
                        if (cfLoad.RegionType == RegionTypeEnum.PartName) HighlightModelParts(new string[] { cfLoad.RegionName });
                        else if (cfLoad.RegionType == RegionTypeEnum.ElementSetName) HighlightElementSet(_model.Mesh.ElementSets[cfLoad.RegionName], _model.Mesh);
                    }
                    DrawCentrifLoadSymbol(prefixName, cfLoad, color, symbolSize, symbolLayer);
                }
                else throw new NotSupportedException();
            }
            catch { }           
        }
        public void DrawCLoadSymbols(string prefixName, CLoad cLoad, double[][] symbolCoor, System.Drawing.Color color, int symbolSize, 
                                     vtkControl.vtkRendererLayer layer)
        {
            if (!cLoad.Visible) return;

            // arrows
            List<double[]> allLoadNormals = new List<double[]>();
            double[] normal = new double[] { cLoad.F1, cLoad.F2, cLoad.F3 };
            for (int i = 0; i < symbolCoor.GetLength(0); i++)
            {
                allLoadNormals.Add(normal);
            }

            if (symbolCoor.GetLength(0) > 0)
            {
                vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Actor.Nodes.Coor = symbolCoor.ToArray();
                data.Actor.Nodes.Normals = allLoadNormals.ToArray();
                ApplyLighting(data);
                _form.AddOrientedArrowsActor(data, symbolSize);
            }
        }
        public void DrawMomentLoadSymbols(string prefixName, MomentLoad momentLoad, double[][] symbolCoor, System.Drawing.Color color, int symbolSize, 
                                          vtkControl.vtkRendererLayer layer)
        {
            if (!momentLoad.Visible) return;
            
            // arrows
            List<double[]> allLoadNormals = new List<double[]>();
            double[] normal = new double[] { momentLoad.M1, momentLoad.M2, momentLoad.M3 };
            for (int i = 0; i < symbolCoor.GetLength(0); i++)
            {
                allLoadNormals.Add(normal);
            }

            if (symbolCoor.GetLength(0) > 0)
            {
                vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Actor.Nodes.Coor = symbolCoor.ToArray();
                data.Actor.Nodes.Normals = allLoadNormals.ToArray();
                ApplyLighting(data);
                _form.AddOrientedDoubleArrowsActor(data, symbolSize);
            }
        }
        public void DrawSTLoadSymbols(string prefixName, STLoad stLoad, double[][] symbolCoor, System.Drawing.Color color, int symbolSize,
                                      vtkControl.vtkRendererLayer layer)
        {
            if (!stLoad.Visible) return;

            // arrows
            List<double[]> allLoadNormals = new List<double[]>();
            double[] normal = new double[] { stLoad.F1, stLoad.F2, stLoad.F3 };
            for (int i = 0; i < symbolCoor.GetLength(0); i++)
            {
                allLoadNormals.Add(normal);
            }

            if (symbolCoor.GetLength(0) > 0)
            {
                vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Actor.Nodes.Coor = symbolCoor.ToArray();
                data.Actor.Nodes.Normals = allLoadNormals.ToArray();
                ApplyLighting(data);
                _form.AddOrientedArrowsActor(data, symbolSize);
            }
        }
        public void DrawDLoadSymbols(string prefixName, DLoad dLoad, System.Drawing.Color color, int symbolSize, vtkControl.vtkRendererLayer layer)
        {
            FeSurface surface = _model.Mesh.Surfaces[dLoad.SurfaceName];

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

            if (!dLoad.Visible) return;
            
            int[] distributedElementIds = GetSpatiallyEquallyDistributedCoor(allCoor.ToArray(), 4);

            int id;
            double[] faceNormal;
            List<double[]> distributedCoor = new List<double[]>();
            List<double[]> distributedLoadNormals = new List<double[]>();
            for (int i = 0; i < distributedElementIds.Length; i++)
            {
                id = distributedElementIds[i];
                _model.Mesh.GetElementFaceCenterAndNormal(allElementIds[id], allElementFaceNames[id], out faceCenter, out faceNormal);
                if (dLoad.Magnitude < 0)
                {
                    faceNormal[0] *= -1;
                    faceNormal[1] *= -1;
                    faceNormal[2] *= -1;
                }
                distributedCoor.Add(faceCenter);
                distributedLoadNormals.Add(faceNormal);
            }

            // arrows
            if (allCoor.Count > 0)
            {
                vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
                data.Name = prefixName;
                data.Color = color;
                data.Layer = layer;
                data.Actor.Nodes.Coor = distributedCoor.ToArray();
                data.Actor.Nodes.Normals = distributedLoadNormals.ToArray();
                ApplyLighting(data);
                _form.AddOrientedArrowsActor(data, symbolSize, dLoad.Magnitude > 0);
            }
        }
        public void DrawGravityLoadSymbol(string prefixName, GravityLoad gLoad, double[] symbolCoor, System.Drawing.Color color, int symbolSize, vtkControl.vtkRendererLayer layer)
        {
            if (!gLoad.Visible) return;

            // arrows
            double[] normal = new double[] { gLoad.F1, gLoad.F2, gLoad.F3 };
           
            vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
            data.Name = prefixName;
            data.Color = color;
            data.Layer = layer;
            data.Actor.Nodes.Coor = new double[][] { symbolCoor };
            data.Actor.Nodes.Normals = new double[][] { normal };
            ApplyLighting(data);
            _form.AddOrientedArrowsActor(data, symbolSize);
            _form.AddSphereActor(data, symbolSize);
        }
        public void DrawCentrifLoadSymbol(string prefixName, CentrifLoad cfLoad, System.Drawing.Color color, int symbolSize, vtkControl.vtkRendererLayer layer)
        {
            if (!cfLoad.Visible) return;

            // arrows
            double[] normal = new double[] { cfLoad.N1, cfLoad.N2, cfLoad.N3 };

            vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
            data.Name = prefixName;
            data.Color = color;
            data.Layer = layer;
            data.Actor.Nodes.Coor = new double[][] { new double[] { cfLoad.X, cfLoad.Y, cfLoad.Z } };
            data.Actor.Nodes.Normals = new double[][] { normal };
            ApplyLighting(data);
            _form.AddOrientedDoubleArrowsActor(data, symbolSize);
            _form.AddSphereActor(data, symbolSize);
        }
        private int[] GetSpatiallyEquallyDistributedCoor(double[][] coor, int n)
        {
            // divide space into boxes and then find the coor closest to the box center
            if (coor.GetLength(0) <= 0) return null;
            if (coor[0].Length != 3) return null;

            // bounding box
            double minX = coor[0][0];
            double maxX = coor[0][0];
            double minY = coor[0][1];
            double maxY = coor[0][1];
            double minZ = coor[0][2];
            double maxZ = coor[0][2];

            for (int i = 0; i < coor.GetLength(0); i++)
            {
                if (coor[i][0] > maxX) maxX = coor[i][0];
                else if (coor[i][0] < minX) minX = coor[i][0];

                if (coor[i][1] > maxY) maxY = coor[i][1];
                else if (coor[i][1] < minY) minY = coor[i][1];
                
                if (coor[i][2] > maxZ) maxZ = coor[i][2];
                else if (coor[i][2] < minZ) minZ = coor[i][2];
            }

            double max = Math.Max(maxX - minX, maxY - minY);
            max = Math.Max(max, maxZ - minZ);
            double delta = max / n;

            // divide space into hexahedrons
            int nX = 1;
            int nY = 1;
            int nZ = 1;
            if (maxX - minX != 0) nX = (int)Math.Ceiling((maxX - minX) / delta);
            if (maxY - minY != 0) nY = (int)Math.Ceiling((maxY - minY) / delta);
            if (maxZ - minZ != 0) nZ = (int)Math.Ceiling((maxZ - minZ) / delta);

            double deltaX = 1;
            double deltaY = 1;
            double deltaZ = 1;
            if (maxX - minX != 0) deltaX = ((maxX - minX) / nX) * 1.01;    // interval from 0...2 has 2 segments; value 2 is out of it
            if (maxY - minY != 0) deltaY = ((maxY - minY) / nY) * 1.01;
            if (maxZ - minZ != 0) deltaZ = ((maxZ - minZ) / nZ) * 1.01;
            minX -= deltaX * 0.005;
            minY -= deltaY * 0.005;
            minZ -= deltaZ * 0.005;

            List<int>[][][] spatialIds = new List<int>[nX][][];
            for (int i = 0; i < nX; i++)
            {
                spatialIds[i] = new List<int>[nY][];
                for (int j = 0; j < nY; j++)
                {
                    spatialIds[i][j] = new List<int>[nZ];
                }
            }

            // fill space hexahedrons
            int idX;
            int idY;
            int idZ;
            for (int i = 0; i < coor.GetLength(0); i++)
            {
                idX = (int)Math.Floor((coor[i][0] - minX) / deltaX);
                idY = (int)Math.Floor((coor[i][1] - minY) / deltaY);
                idZ = (int)Math.Floor((coor[i][2] - minZ) / deltaZ);
                if (spatialIds[idX][idY][idZ] == null) spatialIds[idX][idY][idZ] = new List<int>();
                spatialIds[idX][idY][idZ].Add(i);
            }

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
                            center[0] = minX + (i + 0.5) * deltaX;
                            center[1] = minY + (j + 0.5) * deltaY;
                            center[2] = minZ + (k + 0.5) * deltaZ;

                            centerIds.Add(FindClosestIdFromIds(spatialIds[i][j][k].ToArray(), center, coor));
                            //centerIds.Add(spatialIds[i][j][k].First());
                        }
                    }
                }
            }

            return centerIds.ToArray();
        }
        private int FindClosestIdFromIds(int[] ids, double[] center, double[][] coor)
        {
            int minId = -1;
            double minDist = double.MaxValue;
            int id;
            double d;
            for (int i = 0; i < ids.Length; i++)
            {
                id = ids[i];
                d = Math.Pow(center[0] - coor[id][0], 2) + Math.Pow(center[1] - coor[id][1], 2) + Math.Pow(center[2] - coor[id][2], 2);

                if (d < minDist)
                {
                    minDist = d;
                    minId = id;
                }
            }
            return minId;
        }

        // Geometry
        public void DrawNodes(string prefixName, double[][] nodeCoor, System.Drawing.Color color, vtkControl.vtkRendererLayer layer, int nodeSize = 5)
        {
            vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
            data.Name = prefixName + Globals.NameSeparator + "nodes";
            data.NodeSize = nodeSize;
            data.Color = color;
            data.Layer = layer;
            data.Actor.Nodes.Coor = nodeCoor;
            ApplyLighting(data);
            _form.Add3DNodes(data);
        }
        public void DrawNodes(string prefixName, int[] nodeIds, System.Drawing.Color color, vtkControl.vtkRendererLayer layer, int nodeSize = 5)
        {
            DrawNodes(prefixName, _model.Geometry.GetNodeSetCoor(nodeIds), color, layer, nodeSize);
        }
        public void DrawNodeSet(string prefixName, string nodeSetName, System.Drawing.Color color, vtkControl.vtkRendererLayer layer, int nodeSize = 5)
        {
            if (_model.Mesh.NodeSets.ContainsKey(nodeSetName))
            {
                FeNodeSet nodeSet = _model.Mesh.NodeSets[nodeSetName];
                double[][] nodeCoor = _model.Mesh.GetNodeSetCoor(nodeSet.Labels);

                vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
                data.Name = prefixName + Globals.NameSeparator + nodeSetName;
                data.NodeSize = nodeSize;
                data.Color = color;
                data.Layer = layer;
                data.Actor.Nodes.Coor = nodeCoor;
                ApplyLighting(data);
                _form.Add3DNodes(data);
            }
        }
        public void DrawSurface(string prefixName, string surfaceName, System.Drawing.Color color, vtkControl.vtkRendererLayer layer, bool backfaceCulling = true)
        {            
            FeSurface s;
            FeNodeSet ns;
            if (_model.Mesh.Surfaces.TryGetValue(surfaceName, out s) && s.Active && s.Visible && s.Valid)
            {
                if (s.Type == FeSurfaceType.Element && s.ElementFaces != null)
                {
                    vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
                    data.Name = prefixName + Globals.NameSeparator + surfaceName;
                    data.Color = color;
                    data.Layer = layer;
                    data.CanHaveElementEdges = true;
                    data.BackfaceCulling = backfaceCulling;
                    _model.Mesh.GetSurfaceGeometry(surfaceName, out data.Actor.Nodes.Coor, out data.Actor.Cells.CellNodeIds, out data.Actor.Cells.Types);

                    ApplyLighting(data);
                    _form.Add3DCells(data);
                }
                else if (s.Type == FeSurfaceType.Node && Model.Mesh.NodeSets.TryGetValue(s.NodeSetName, out ns))
                {
                    DrawNodeSet(prefixName + Globals.NameSeparator + surfaceName, s.NodeSetName, color, layer);
                }
            }
        }
        public void DrawSurfaceEdge(string prefixName, string surfaceName, System.Drawing.Color color, vtkControl.vtkRendererLayer layer, bool backfaceCulling = true)
        {
            FeSurface s;
            FeNodeSet ns;
            if (_model.Mesh.Surfaces.TryGetValue(surfaceName, out s) && s.Active && s.Visible && s.Valid)
            {
                if (s.Type == FeSurfaceType.Element && s.ElementFaces != null)
                {
                    vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
                    data.Name = prefixName + Globals.NameSeparator + surfaceName;
                    data.Color = color;
                    data.Layer = layer;
                    data.CanHaveElementEdges = true;
                    data.BackfaceCulling = backfaceCulling;
                    _model.Mesh.GetSurfaceEdgesGeometry(surfaceName, out data.Actor.Nodes.Coor, out data.Actor.Cells.CellNodeIds, out data.Actor.Cells.Types);

                    ApplyLighting(data);
                    _form.Add3DCells(data);
                }
                else if (s.Type == FeSurfaceType.Node && Model.Mesh.NodeSets.TryGetValue(s.NodeSetName, out ns))
                {
                    DrawNodeSet(prefixName + Globals.NameSeparator + surfaceName, s.NodeSetName, color, layer);
                }
            }
        }

        // Apply settings
        private void ApplyLighting(vtkControl.vtkMaxActorData data)
        {
            GraphicsSettings graphicsSettings = (GraphicsSettings)_settings[Globals.GraphicsSettingsName];
            data.Ambient = graphicsSettings.AmbientComponent;
            data.Diffuse = graphicsSettings.DiffuseComponent;
        }


        #endregion #################################################################################################################

        #region Highlight  #########################################################################################################
        public void UpdateHighlightFromTree()
        {
            _form.Clear3DSelection();
            _form.UpdateHighlightFromTree();
        }
        public void Highlight3DObjects(object[] obj)
        {
            Highlight3DObjects(CurrentView, obj);
        }
        public void Highlight3DObjects(ViewGeometryModelResults view, object[] obj)
        {
            _form.Clear3DSelection();       // must be here: clears the highlight in the results

            if (obj != null)
            {
                foreach (var item in obj)
                {
                    Highlight3DObject(view, item);
                }

                _form.AdjustCameraDistanceAndClipping();
            }
        }
        private void Highlight3DObject(ViewGeometryModelResults view, object obj)
        {
            try
            {
                if (view == ViewGeometryModelResults.Geometry)
                {
                    if (obj is CaeMesh.GeometryPart)
                    {
                        HighlightGeometryParts(new string[] { ((CaeMesh.BasePart)obj).Name });
                    }
                }
                else if (view == ViewGeometryModelResults.Model)
                {
                    if (obj is string name)
                    {
                        if (_model.Mesh.NodeSets.ContainsKey(name)) Highlight3DObject(view, _model.Mesh.NodeSets[name]);
                        else if (_model.Mesh.ElementSets.ContainsKey(name)) Highlight3DObject(view, _model.Mesh.ElementSets[name]);
                        else if (_model.Mesh.Parts.ContainsKey(name)) Highlight3DObject(view, _model.Mesh.Parts[name]);
                        else if (_model.Mesh.Surfaces.ContainsKey(name)) Highlight3DObject(view, _model.Mesh.Surfaces[name]);
                        else if (_model.Mesh.ReferencePoints.ContainsKey(name)) Highlight3DObject(view, _model.Mesh.ReferencePoints[name]);
                    }
                    else if (obj is CaeMesh.MeshPart)
                    {
                        HighlightModelParts(new string[] { ((CaeMesh.BasePart)obj).Name });
                    }                   
                    else if (obj is CaeMesh.FeNodeSet)
                    {
                        HighlightNodeSets(new string[] { ((CaeMesh.FeNodeSet)obj).Name });
                    }
                    else if (obj is CaeMesh.FeElementSet)
                    {
                        HighlightElementSets(new string[] { ((CaeMesh.FeElementSet)obj).Name });
                    }
                    else if (obj is CaeMesh.FeSurface)
                    {
                        HighlightSurfaces(new string[] { ((CaeMesh.FeSurface)obj).Name });
                    }
                    else if (obj is CaeMesh.FeReferencePoint)
                    {
                        HighlightReferencePoints(new string[] { ((CaeMesh.FeReferencePoint)obj).Name });
                    }
                    else if (obj is CaeModel.SolidSection ss)
                    {
                        if (ss.RegionType == RegionTypeEnum.PartName) HighlightModelParts(new string[] { ss.RegionName });
                        else if (ss.RegionType == RegionTypeEnum.ElementSetName) HighlightElementSets(new string[] { ss.RegionName });
                        else throw new NotSupportedException();
                    }
                    else if (obj is CaeModel.Constraint)
                    {
                        HighlightConstraints(new string[] { ((CaeModel.Constraint)obj).Name });
                    }
                    else if (obj is CaeModel.HistoryOutput ho)
                    {
                        if (ho.RegionType == RegionTypeEnum.NodeSetName) HighlightNodeSets(new string[] { ho.RegionName });
                        else if (ho.RegionType == RegionTypeEnum.ElementSetName) HighlightElementSets(new string[] { ho.RegionName });
                        else if (ho.RegionType == RegionTypeEnum.SurfaceName) HighlightSurfaces(new string[] { ho.RegionName });
                        else throw new NotSupportedException();
                    }
                    else if (obj is CaeModel.DisplacementRotation)
                    {
                        HighlightBoundaryCondition(obj as BoundaryCondition);
                    }
                    else if (obj is CaeModel.Load)
                    {
                        HighlightLoad(obj as Load);
                    }
                }
                else if (view == ViewGeometryModelResults.Results)
                {
                    if (obj is CaeMesh.ResultPart || obj is CaeMesh.GeometryPart)
                    {
                        HighlightResultParts(new string[] { ((CaeMesh.BasePart)obj).Name });
                    }
                }
                
            }
            catch { }
        }
        public void HighlightGeometryParts(string[] partsToSelect)
        {
            GeometryPart[] parts = GetGeometryParts();
            System.Drawing.Color color = System.Drawing.Color.Red;
            vtkControl.vtkRendererLayer layer = vtkControl.vtkRendererLayer.Selection;

            foreach (var part in parts)
            {
                if (part.Visible && partsToSelect.Contains(part.Name) && _form.ContainsActor(part.Name))
                {
                    if (part.ErrorElementIds != null)
                    {
                        FeElementSet errorElemetSet = new FeElementSet("Error_elements", part.ErrorElementIds);
                        HighlightElementSet(errorElemetSet, _model.Geometry);
                        DrawNodes(part.Name, part.ErrorNodeIds, color, layer);
                    }
                    else _form.HighlightActor(part.Name);
                }
            }
        }
        public void HighlightModelParts(string[] partsToSelect)
        {
            MeshPart[] parts = GetModelParts();
            System.Drawing.Color color = System.Drawing.Color.Red;

            foreach (var part in parts)
            {
                if (part.Visible && partsToSelect.Contains(part.Name))
                {
                    if (_form.ContainsActor(part.Name)) _form.HighlightActor(part.Name);
                }
            }
        }
        public void HighlightResultParts(string[] partsToSelect)
        {
            BasePart[] parts = GetResultParts();
            System.Drawing.Color color = System.Drawing.Color.Red;

            foreach (var part in parts)
            {
                if (part.Visible && partsToSelect.Contains(part.Name))
                {
                    if (_form.ContainsActor(part.Name)) _form.HighlightActor(part.Name);
                }
            }
        }
        public void HighlightNodeSets(string[] nodeSetsToSelect)
        {
            Dictionary<string, FeNodeSet> nodeSets = _model.Mesh.NodeSets;
            System.Drawing.Color color = System.Drawing.Color.Red;
            vtkControl.vtkRendererLayer layer = vtkControl.vtkRendererLayer.Selection;
            int nodeSize = 1; // size <= 1 gets overwritten in vtkControl for the highlights in selection layer
            foreach (var nodeSetName in nodeSetsToSelect)
            {
                DrawNodeSet("Highlight", nodeSetName, color, layer, nodeSize);
            }
        }
        
        public void HighlightElement(int elementId)
        {
            vtkControl.vtkMaxActorData data = GetCellActorData(new int[] {elementId}, null);
            data.Actor.Nodes.Values = null; // to draw in highlight color
            data.Layer = vtkControl.vtkRendererLayer.Selection;
            data.CanHaveElementEdges = true;
            ApplyLighting(data);
            _form.Add3DCells(data);
        }
        private void HighlightElements(int[] elementIds, FeMesh mesh)
        {
            int[] nodeIds;
            double[][] nodeCoor;
            int[] cellIds;
            int[][] cells;
            int[] cellTypes;
            System.Drawing.Color color = System.Drawing.Color.Red;
            vtkControl.vtkRendererLayer layer = vtkControl.vtkRendererLayer.Selection;
            bool canHaveEdges = true;
            vtkControl.vtkMaxActorData data;

            // get only needed nodes and renumbered elements
            // mesh.GetAllNodesAndCells(elementSet, out nodeIds, out nodeCoor, out cellIds, out cells, out cellTypes);

            BasePart part = mesh.CreateBasePartFromElementIds(elementIds);
            mesh.GetVisualizationNodesAndCells(part, out nodeIds, out nodeCoor, out cellIds, out cells, out cellTypes);

            data = new vtkControl.vtkMaxActorData();
            data.Color = color;
            data.Layer = layer;
            data.CanHaveElementEdges = canHaveEdges;
            data.Actor.Nodes.Ids = null;
            data.Actor.Nodes.Coor = nodeCoor;
            data.Actor.Cells.CellNodeIds = cells;
            data.Actor.Cells.Types = cellTypes;

            ApplyLighting(data);
            _form.Add3DCells(data);
        }
        public void HighlightElementSets(string[] elementSetsToSelect)
        {
            foreach (var elementSetName in elementSetsToSelect)
            {
                if (_model.Mesh.ElementSets.ContainsKey(elementSetName))
                    HighlightElementSet(_model.Mesh.ElementSets[elementSetName], _model.Mesh);
            }
        }
        private void HighlightElementSet(FeElementSet elementSet, FeMesh mesh)
        {
            HighlightElements(elementSet.Labels, mesh);
        }
        public void HighlightSurface(int[][] cells)
        {
            FeMesh mesh = DisplayedMesh;
            System.Drawing.Color color = System.Drawing.Color.Red;
            vtkControl.vtkRendererLayer layer = vtkControl.vtkRendererLayer.Selection;

            // copy
            int[][] cellsCopy = new int[cells.Length][];
            for (int i = 0; i < cells.Length; i++) cellsCopy[i] = cells[i].ToArray();

            // faces
            vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
            data.Name = "highlight_surface_by_cells";
            data.Color = color;
            data.Layer = layer;
            data.CanHaveElementEdges = true;
            data.BackfaceCulling = true;
            data.Actor.Cells.CellNodeIds = cells;
            mesh.GetSurfaceGeometry(cells, out data.Actor.Nodes.Coor, out data.Actor.Cells.Types);

            ApplyLighting(data);
            _form.Add3DCells(data);

            //edges
            cells = mesh.GetFreeEdgesFromVisualizationCells(cellsCopy);

            data = new vtkControl.vtkMaxActorData();
            data.Name = "highlight_surface_edges_by_cells";
            data.Color = color;
            data.Layer = layer;
            data.CanHaveElementEdges = true;
            data.BackfaceCulling = true;
            data.Actor.Cells.CellNodeIds = cells;
            mesh.GetSurfaceEdgesGeometry(cells, out data.Actor.Nodes.Coor, out data.Actor.Cells.Types);

            ApplyLighting(data);
            _form.Add3DCells(data);

        }
        public void HighlightSurfaces(string[] surfacesToSelect)
        {
            System.Drawing.Color color = System.Drawing.Color.Red;
            vtkControl.vtkRendererLayer layer = vtkControl.vtkRendererLayer.Selection;

            foreach (var surfaceName in surfacesToSelect)
            {
                DrawSurface("Highlight-Surface", surfaceName, color, layer);
                DrawSurfaceEdge("Highlight-SurfaceEdges", surfaceName, color, layer);
            }
        }
        public void HighlightReferencePoints(string[] referencePointsToSelect)
        {
            System.Drawing.Color color = System.Drawing.Color.Red;
            vtkControl.vtkRendererLayer layer = vtkControl.vtkRendererLayer.Selection;
            vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();

            foreach (var name in referencePointsToSelect)
            {
                DrawReferencePoint(name, color, layer);
            }
        }
        public void HighlightConstraints(string[] constraintsToSelect)
        {
            Constraint constraint;
            foreach (var constraintName in constraintsToSelect)
            {
                constraint = _model.Constraints[constraintName];

                if (!constraint.Visible) return;

                if (constraint is RigidBody rb)
                {
                    DrawConstraint(constraint, System.Drawing.Color.Red, 4, vtkControl.vtkRendererLayer.Selection);
                    HighlightReferencePoints(new string[] { rb.ReferencePointName });
                }
                else if (constraint is Tie t)
                {
                    DrawConstraint(constraint, System.Drawing.Color.Red, 4, vtkControl.vtkRendererLayer.Selection);
                }
                else throw new NotSupportedException();
            }
        }
        public void HighlightBoundaryCondition(BoundaryCondition boundaryCondition)
        {
            PreSettings preSettings = (PreSettings)_settings[Globals.PreSettingsName];
            int symbolSize = preSettings.SymbolSize;
            int nodeSymbolSize = preSettings.NodeSymbolSize;
            DrawBoundaryCondition("Step-Highlight", boundaryCondition, System.Drawing.Color.Red, symbolSize, nodeSymbolSize, vtkControl.vtkRendererLayer.Selection);
        }
        public void HighlightLoad(Load load)
        {
            PreSettings preSettings = (PreSettings)_settings[Globals.PreSettingsName];
            int symbolSize = preSettings.SymbolSize;
            int nodeSymbolSize = preSettings.NodeSymbolSize;
            DrawLoad("Highlight", load, System.Drawing.Color.Red, symbolSize, nodeSymbolSize, vtkControl.vtkRendererLayer.Selection);
        }
        public void HighlightConnectedLines(double[][] lineNodeCoor, int nodeSize = 5)
        {
            // create wire elements
            System.Drawing.Color color = System.Drawing.Color.Red;
            vtkControl.vtkRendererLayer layer = vtkControl.vtkRendererLayer.Selection;

            LinearBeamElement element = new LinearBeamElement(0, new int[] { 0, 1 });

            int[][] cells = new int[lineNodeCoor.GetLength(0) - 1][];
            int[] cellsTypes = new int[cells.GetLength(0)];
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                cells[i] = new int[] { i, i + 1 };
                cellsTypes[i] = element.GetVtkCellType();
            }

            vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
            data.Color = color;
            data.Layer = layer;
            data.Pickable = false;
            data.Actor.Nodes.Ids = null;
            data.Actor.Nodes.Coor = lineNodeCoor.ToArray();
            data.Actor.Cells.CellNodeIds = cells;
            data.Actor.Cells.Types = cellsTypes;

            ApplyLighting(data);
            _form.Add3DCells(data);

            double[][] nodeCoor = new double[2][];
            nodeCoor[0] = lineNodeCoor[0];
            nodeCoor[1] = lineNodeCoor[lineNodeCoor.Length - 1];

            //DrawNodes("short_edges", nodeCoor, color, layer, nodeSize);
        }
        public void HighlightConnectedEdges(double[][][] lineNodeCoor, int nodeSize = 5)
        {   
            // using HighlightConnectedLines is slow since invalidate is called each time

            // create wire elements
            vtkControl.vtkRendererLayer layer = vtkControl.vtkRendererLayer.Selection;

            int elementVtkCellType = new LinearBeamElement(0, new int[] { 0, 1 }).GetVtkCellType();

            int n = 0;
            for (int i = 0; i < lineNodeCoor.Length; i++) n += lineNodeCoor[i].Length - 1;
            
            int[][] cells = new int[n][];
            int[] cellsTypes = new int[cells.GetLength(0)];
            List<double[]> nodeCoor = new List<double[]>();

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

            vtkControl.vtkMaxActorData data = new vtkControl.vtkMaxActorData();
            data.Layer = layer;
            data.Pickable = false;
            data.Actor.Nodes.Ids = null;
            data.Actor.Nodes.Coor = nodeCoor.ToArray();
            data.Actor.Cells.CellNodeIds = cells;
            data.Actor.Cells.Types = cellsTypes;

            ApplyLighting(data);
            _form.Add3DCells(data);

            nodeCoor.Clear();

            for (int i = 0; i < lineNodeCoor.Length; i++)                       // lines
            {
                nodeCoor.Add(lineNodeCoor[i][0]);
                nodeCoor.Add(lineNodeCoor[i][lineNodeCoor[i].Length - 1]);
            }
            DrawNodes("short_edges", nodeCoor.ToArray(), System.Drawing.Color.Empty, layer, nodeSize);
        }

        public void HighlightSelection()
        {
            _form.Clear3DSelection();
            int[] ids = GetSelectionIds();
            if (ids.Length == 0) return;

            if (_selection.SelectItem == vtkSelectItem.Node)
            {
                vtkControl.vtkMaxActorData data = GetNodeActorData(ids);
                data.Layer = vtkControl.vtkRendererLayer.Selection;
                ApplyLighting(data);
                _form.Add3DNodes(data);
            }
            else if (_selection.SelectItem == vtkSelectItem.Element)
            {
                HighlightElements(ids, DisplayedMesh);
            }
            else if (_selection.SelectItem == vtkSelectItem.Surface)
            {
                vtkControl.vtkMaxActorData data = GetFaceActorDataByFaceIds(ids);
                data.Layer = vtkControl.vtkRendererLayer.Selection;
                data.CanHaveElementEdges = true;
                ApplyLighting(data);
                _form.Add3DCells(data);
            }
            else if (_selection.SelectItem == vtkSelectItem.Part)
            {
                foreach (var entry in DisplayedMesh.Parts)
                {
                    if (entry.Value.PartId == ids[0])
                    {
                        if (entry.Value is GeometryPart) HighlightGeometryParts(new string[] { entry.Key });
                        else if (entry.Value is MeshPart) HighlightModelParts(new string[] { entry.Key });
                        return;
                    }
                }
            }
            else throw new NotSupportedException();
        }
        #endregion #################################################################################################################

        #region Results  ###########################################################################################################
        public void DrawResults(bool resetCamera)
        {
            _form.Clear3D();

            if (_results == null) return;

            // Settings                                                              
            // must be here before drawing parts to correctly set the numer of colors
            PostSettings postSettings = (PostSettings)_settings[Globals.PostSettingsName];
            _form.SetColorSpectrum(postSettings.ColorSpectrum);
            _form.SetScalarBarText(_currentFieldData.Name + ": " + _currentFieldData.Component + Environment.NewLine 
                                   + postSettings.ColorSpectrum.MinMaxType.ToString());
            _form.SetShowMinValueLocation(postSettings.ShowMinValueLocation);
            _form.SetShowMaxValueLocation(postSettings.ShowMaxValueLocation);
            _form.SetChartNumberFormat(postSettings.GetColorChartNumberFormat());

            float scale = GetScale();
            DrawResult(_currentFieldData, scale, postSettings.DrawUndeformedModel, postSettings.UndeformedModelColor);

            if (resetCamera) _form.SetFrontBackView(true, true); // animation:true is here to correctly draw max/min widgets 
            _form.AdjustCameraDistanceAndClipping();
        }
        private void DrawResult(FieldData fieldData, float scale, bool drawUndeformedModel, System.Drawing.Color undeformedModelColor)
        {
            vtkControl.vtkMaxActorData data;
            vtkControl.vtkRendererLayer layer = vtkControl.vtkRendererLayer.Base;
            List<string> hiddenActors = new List<string>();

            vtkControl.DataFieldType fieldType = ConvertStepType(fieldData);
            _form.SetStatusBlock(Path.GetFileName(_results.FileName), _results.DateTime, fieldData.Time, scale, fieldType);
            _form.InitializeWidgetPositions(); // reset the widget position after setting the status block content

            // udeformed shape
            foreach (var entry in _results.Mesh.Parts)
            {
                if (entry.Value is ResultPart resultPart)
                {
                    if (_viewResultsType == ViewResultsType.Undeformed)
                    {
                        DrawMeshPart(_results.Mesh, resultPart, layer);
                    }
                    else
                    {
                        if (drawUndeformedModel) DrawUndeformedPartCopy(resultPart, undeformedModelColor, layer);

                        data = GetVtkMaxActorDataFromPart(resultPart, fieldData, scale);
                        ApplyLighting(data);
                        _form.AddScalarFieldOn3DCells(data);
                    }
                }
                else if (entry.Value is GeometryPart)
                {
                    DrawGeomPart(_results.Mesh, entry.Value, layer, false, true);
                }

                if (!entry.Value.Visible) hiddenActors.Add(entry.Key);
            }
            if (hiddenActors.Count > 0) _form.HideActors(hiddenActors.ToArray(), true);
        }
        private vtkControl.vtkMaxActorData GetVtkMaxActorDataFromPart(ResultPart part, FieldData fieldData, float scale)
        {
            // get visualization nodes and renumbered elements           
            PartExchangeData actorResultData = _results.GetVisualizationScaledNodesCellsAndValues(part, fieldData, scale);

            // model edges
            PartExchangeData modelEdgesResultData = null;
            if (_results.Mesh.Elements[part.Labels[0]] is FeElement3D && part.Visualization.EdgeCells != null)
            {
                modelEdgesResultData = _results.GetScaledEdgesNodesAndCells(part, fieldData, scale);
            }

            // get all needed nodes and elements - renumbered
            PartExchangeData locatorResultData = _results.GetAllScaledNodesCellsAndValues(part, fieldData, scale);

            vtkControl.vtkMaxActorData data = GetVtkData(actorResultData, modelEdgesResultData, locatorResultData);
            data.Name = part.Name;
            data.Color = part.Color;
            data.ColorContours = part.ColorContours;
            data.CanHaveElementEdges = true;
            data.Pickable = true;
            data.SmoothShaded = part.SmoothShaded;
            
            return data;
        }
        // Animation
        public bool DrawScaleFactorAnimation(int numFrames)
        {
            _form.Clear3D();

            if (_results == null) return false;

            // Settings                                                              
            // must be here before drawing parts to correctly set the numer of colors
            PostSettings postSettings = (PostSettings)_settings[Globals.PostSettingsName];
            _form.SetColorSpectrum(postSettings.ColorSpectrum);
            _form.SetScalarBarText(_currentFieldData.Name + ": " + _currentFieldData.Component + Environment.NewLine 
                                   + postSettings.ColorSpectrum.MinMaxType.ToString());
            _form.SetShowMinValueLocation(postSettings.ShowMinValueLocation);
            _form.SetShowMaxValueLocation(postSettings.ShowMaxValueLocation);
            _form.SetChartNumberFormat(postSettings.GetColorChartNumberFormat());

            float scale = GetScale();
            vtkControl.vtkMaxActorData data;
            vtkControl.vtkRendererLayer layer = vtkControl.vtkRendererLayer.Base;

            vtkControl.DataFieldType fieldType = ConvertStepType(_currentFieldData);
            _form.SetStatusBlock(Path.GetFileName(_results.FileName), _results.DateTime, _currentFieldData.Time, scale, fieldType);

            bool result = true;
            List<string> hiddenActors = new List<string>();
            double[] allFramesScalarRange = new double[] { double.MaxValue, -double.MaxValue };
            foreach (var entry in _results.Mesh.Parts)
            {
                if (entry.Value is CaeMesh.ResultPart resultPart)
                {
                    // udeformed shape
                    if (postSettings.DrawUndeformedModel) DrawUndeformedPartCopy(resultPart, postSettings.UndeformedModelColor, layer);

                    // results
                    data = GetScaleFactorAnimationDataFromPart(resultPart, _currentFieldData, scale, numFrames);
                    foreach (NodesExchangeData nData in data.Actor.ExtremeNodesAnimation)
                    {
                        if (nData.Values[0] < allFramesScalarRange[0]) allFramesScalarRange[0] = nData.Values[0];
                        if (nData.Values[1] > allFramesScalarRange[1]) allFramesScalarRange[1] = nData.Values[1];
                    }
                    //data = GetVtkMaxActorDataFromPart(resultPart, _currentFieldData, scale);
                    ApplyLighting(data);
                    result = _form.AddAnimatedScalarFieldOn3DCells(data);                    
                    if (result == false) {_form.Clear3D(); return false;}
                }
                else if (entry.Value is CaeMesh.GeometryPart)
                {
                    DrawGeomPart(_results.Mesh, entry.Value, layer, false, false);
                }
                if (!entry.Value.Visible) hiddenActors.Add(entry.Key);
            }
            if (hiddenActors.Count > 0) _form.HideActors(hiddenActors.ToArray(), true);

            // animation field data
            float[] time = new float[numFrames];
            float[] animationScale = new float[numFrames];
            float ratio = 1f / (numFrames - 1);
            for (int i = 0; i < numFrames; i++)
            {
                time[i] = _currentFieldData.Time;
                animationScale[i] = i * ratio;
            }

             _form.SetAnimationFrameData(time, animationScale, allFramesScalarRange);

            return result;
        }
        public bool DrawTimeIncrementAnimation(out int numFrames)
        {
            _form.Clear3D();

            numFrames = -1;
            if (_results == null) return false;

            // Settings                                                              
            // must be here before drawing parts to correctly set the numer of colors
            PostSettings postSettings = (PostSettings)_settings[Globals.PostSettingsName];
            _form.SetColorSpectrum(postSettings.ColorSpectrum);
            _form.SetScalarBarText(_currentFieldData.Name + ": " + _currentFieldData.Component + Environment.NewLine 
                                   + postSettings.ColorSpectrum.MinMaxType.ToString());
            _form.SetShowMinValueLocation(postSettings.ShowMinValueLocation);
            _form.SetShowMaxValueLocation(postSettings.ShowMaxValueLocation);
            _form.SetChartNumberFormat(postSettings.GetColorChartNumberFormat());

            float scale = GetScaleForAllStepsAndIncrements();
            vtkControl.vtkMaxActorData data = null;
            vtkControl.vtkRendererLayer layer = vtkControl.vtkRendererLayer.Base;

            vtkControl.DataFieldType fieldType = ConvertStepType(_currentFieldData);
            _form.SetStatusBlock(Path.GetFileName(_results.FileName), _results.DateTime, _currentFieldData.Time, scale, fieldType);

            bool result = true;
            List<string> hiddenActors = new List<string>();
            double[] allFramesScalarRange = new double[] { double.MaxValue, -double.MaxValue };
            foreach (var entry in _results.Mesh.Parts)
            {
                if (entry.Value is CaeMesh.ResultPart resultPart)
                {
                    // udeformed shape
                    if (postSettings.DrawUndeformedModel) DrawUndeformedPartCopy(resultPart, postSettings.UndeformedModelColor, layer);

                    // results
                    data = GetTimeIncrementAnimationDataFromPart(resultPart, _currentFieldData, scale);
                    foreach (NodesExchangeData nData in data.Actor.ExtremeNodesAnimation)
                    {
                        if (nData.Values[0] < allFramesScalarRange[0]) allFramesScalarRange[0] = nData.Values[0];
                        if (nData.Values[1] > allFramesScalarRange[1]) allFramesScalarRange[1] = nData.Values[1];
                    }
                    ApplyLighting(data);
                    result = _form.AddAnimatedScalarFieldOn3DCells(data);
                    if (result == false) { _form.Clear3D(); return false; }
                }
                else if (entry.Value is CaeMesh.GeometryPart)
                {
                    DrawGeomPart(_results.Mesh, entry.Value, layer, false, false);
                }
                if (!entry.Value.Visible) hiddenActors.Add(entry.Key);
            }
            if (hiddenActors.Count > 0) _form.HideActors(hiddenActors.ToArray(), true);

            // animation field data
            var existingIncrements = _results.GetExistingIncrementIds(_currentFieldData.Name, _currentFieldData.Component);
            List<float> time = new List<float>();
            List<float> animationScale = new List<float>();
            foreach (var entry in existingIncrements)
            {
                for (int i = 0; i < entry.Value.Length; i++)
                {
                    time.Add(_results.GetIncrementTime(_currentFieldData.Name, entry.Key, entry.Value[i]));
                    animationScale.Add(-1);
                }
            }
            _form.SetAnimationFrameData(time.ToArray(), animationScale.ToArray(), allFramesScalarRange);

            numFrames = data.Actor.NodesAnimation.Length;

            return result;
        }        
        private vtkControl.vtkMaxActorData GetScaleFactorAnimationDataFromPart(ResultPart part, FieldData fieldData, float scale, int numFrames)
        {
            // get visualization nodes and renumbered elements
            PartExchangeData actorResultData = _results.GetScaleFactorAnimationDataVisualizationNodesCellsAndValues(part, fieldData, scale, numFrames);
            
            // model edges
            PartExchangeData modelEdgesResultData = null;
            if (_results.Mesh.Elements[part.Labels[0]] is FeElement3D && part.Visualization.EdgeCells != null)
            {
                modelEdgesResultData = _results.GetScaleFactorAnimationDataEdgesNodesAndCells(part, fieldData, scale, numFrames);
            }

            vtkControl.vtkMaxActorData data = GetVtkData(actorResultData, modelEdgesResultData, null);
            data.Name = part.Name;
            data.Color = part.Color;
            data.ColorContours = part.ColorContours;
            data.CanHaveElementEdges = true;
            data.Pickable = false;
            data.SmoothShaded = part.SmoothShaded;
            return data;
        }
        private vtkControl.vtkMaxActorData GetTimeIncrementAnimationDataFromPart(ResultPart part, FieldData fieldData, float scale)
        {
            // get visualization nodes and renumbered elements
            PartExchangeData actorResultData = _results.GetTimeIncrementAnimationDataVisualizationNodesCellsAndValues(part, fieldData, scale);

            // model edges
            PartExchangeData modelEdgesResultData = null;
            if (_results.Mesh.Elements[part.Labels[0]] is FeElement3D && part.Visualization.EdgeCells != null)
            {
                modelEdgesResultData = _results.GetTimeIncrementAnimationDataVisualizationEdgesNodesAndCells(part, fieldData, scale);
            }

            vtkControl.vtkMaxActorData data = GetVtkData(actorResultData, modelEdgesResultData, null);
            data.Name = part.Name;
            data.Color = part.Color;
            data.ColorContours = part.ColorContours;
            data.CanHaveElementEdges = true;
            data.Pickable = false;
            data.SmoothShaded = part.SmoothShaded;
            return data;
        }
        // Common
        private vtkControl.DataFieldType ConvertStepType(FieldData fieldData)
        {
            vtkControl.DataFieldType fieldType;
            if (fieldData.Type == StepType.Static) fieldType = vtkControl.DataFieldType.Static;
            else if (fieldData.Type == StepType.Frequency) fieldType = vtkControl.DataFieldType.Frequency;
            else if (fieldData.Type == StepType.Buckling) fieldType = vtkControl.DataFieldType.Buckling;
            else throw new NotSupportedException();
            return fieldType;
        }
        public void UpdatePartsScalarFields()
        {
            if (_results == null) return;

            PostSettings settings = (PostSettings)_settings[Globals.PostSettingsName];

            // Settings                                                              
            _form.SetScalarBarText(_currentFieldData.Name + ": " + _currentFieldData.Component + Environment.NewLine 
                                   + settings.ColorSpectrum.MinMaxType.ToString());

            float scale = GetScale();
            foreach (var entry in _results.Mesh.Parts)
            {
                if (entry.Value is ResultPart)
                {
                    // get visualization nodes and renumbered elements
                    PartExchangeData actorResultData = _results.GetVisualizationScaledNodesCellsAndValues(entry.Value, _currentFieldData, scale);  // to scale min nad max nodes coor
                    _form.UpdateActorScalarField(entry.Key, actorResultData.Nodes.Values, actorResultData.ExtremeNodes);
                }
            }
        }
        public void DrawUndeformedPartCopy(BasePart part, System.Drawing.Color color, vtkControl.vtkRendererLayer layer)
        {
            vtkControl.vtkMaxActorData data;
            data = new vtkControl.vtkMaxActorData();
            data.Name = part.Name + "_undeformed";
            data.Color = color;
            data.Layer = layer;
            data.CanHaveElementEdges = false;
            data.SmoothShaded = part.SmoothShaded;
            _results.GetUndeformedNodesAndCells(part, out data.Actor.Nodes.Coor, out data.Actor.Cells.CellNodeIds, out data.Actor.Cells.Types);
            ApplyLighting(data);
            _form.Add3DCells(data);
        }

        private vtkControl.vtkMaxActorData GetVtkData(PartExchangeData actorData, PartExchangeData modelEdgesData, PartExchangeData locatorData)
        {
            vtkControl.vtkMaxActorData vtkData = new vtkControl.vtkMaxActorData();
            
            vtkData.Actor = actorData;
            vtkData.ModelEdges = modelEdgesData;
            vtkData.CellLocator = locatorData;

            return vtkData;
        }
        #endregion #################################################################################################################


        // Tools
        public float GetScale()
        {
            if (_viewResultsType == ViewResultsType.Undeformed) return 0;

            float scale = 1;
            PostSettings settings = (PostSettings)_settings[Globals.PostSettingsName];

            if (settings.DeformationScaleFactorType == DeformationScaleFactorType.Automatic)
            {
                float size = (float)_results.Mesh.GetBoundingBoxVolumeAsCubeSide();
                float maxDisp = _results.GetMaxDisplacement(_currentFieldData.StepId, _currentFieldData.StepIncrementId);
                if (maxDisp != 0) scale = size * 0.25f / maxDisp;
            }
            else scale = (float)settings.DeformationScaleFactorValue;
            return scale;
        }

        public float GetScaleForAllStepsAndIncrements()
        {
            if (_viewResultsType == ViewResultsType.Undeformed) return 0;

            float scale = 1;
            PostSettings settings = (PostSettings)_settings[Globals.PostSettingsName];

            if (settings.DeformationScaleFactorType == DeformationScaleFactorType.Automatic)
            {
                float size = (float)_results.Mesh.GetBoundingBoxVolumeAsCubeSide();
                float maxDisp = _results.GetMaxDisplacement();
                if (maxDisp != 0) scale = size * 0.25f / maxDisp;
            }
            else scale = (float)settings.DeformationScaleFactorValue;
            return scale;
        }
    }
















}
