using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CaeGlobals;

namespace PrePoMax.Commands
{
    [Serializable]
    public class CommandsCollection
    {
        
        // Variables                                                                                                                
        protected bool _showDialogs;
        protected int _currPositionIndex;
        protected Controller _controller;
        protected List<Command> _commands;
        protected List<string> _history;
        protected string _historyFileNameTxt;
        protected string _historyFileNameBin;
        protected ViewGeometryModelResults _previousView;


        // Properties                                                                                                               
        public int Count { get { return _commands.Count(); } }
        public int CurrPositionIndex { get { return _currPositionIndex; } }


        // Callbacks                                                                                                                
        [NonSerialized] public Action<string> WriteOutput;
        [NonSerialized] public Action ModelChanged_ResetJobStatus;

        // Events                                                                                                                   
        public event Action<string, string> EnableDisableUndoRedo;


        // Constructor                                                                                                              
        public CommandsCollection(Controller controller)
        {
            _controller = controller;
            _currPositionIndex = -1;
            _commands = new List<Command>();
            _history = new List<string>();
            _historyFileNameTxt = Path.Combine(System.Windows.Forms.Application.StartupPath, Globals.HistoryFileName + ".txt");
            _historyFileNameBin = Path.Combine(System.Windows.Forms.Application.StartupPath, Globals.HistoryFileName + ".pmxh");
            _previousView = ViewGeometryModelResults.Geometry;
            //
            WriteToFile();
        }
        public CommandsCollection(Controller controller, CommandsCollection commandsCollection)
            :this(controller)
        {
            _currPositionIndex = commandsCollection._currPositionIndex;
            _commands = commandsCollection._commands;
            _history = commandsCollection._history;
            _previousView = commandsCollection._previousView;
            //
            WriteToFile();
        }


        // Methods                                                                                                                  
        public void AddAndExecute(Command command)
        {
            ExecuteCommand(command, true);
        }
        private void AddCommand(Command command)
        {
            // Remove old commands
            if (_currPositionIndex < _commands.Count - 1)
                _commands.RemoveRange(_currPositionIndex + 1, _commands.Count - _currPositionIndex - 1);
            //
            _commands.Add(command);
        }
        private void AddToHistory(Command command)
        {
            if (command is CClear) return;
            //
            string data = command.GetCommandString();
            //
            _history.Add(data);
            _controller.ModelChanged = true;
            //
            ModelChanged_ResetJobStatus?.Invoke();
        }
        private void ExecuteCommand(Command command, bool addCommand)
        {
            // Write to form
            WriteToOutput(command);
            // First execute to check for errors
            if (command.Execute(_controller))
            {
                // Add command
                if (addCommand) AddCommand(command);
                // Add history
                AddToHistory(command);
                // Write to file
                WriteToFile();
                //
                _currPositionIndex++;
                //
                OnEnableDisableUndoRedo();
            }
        }
        public void ExecuteAllCommands()
        {
            ExecuteAllCommands(false, false);
        }
        public void ExecuteAllCommands(bool showImportDialog, bool showMeshParametersDialog)
        {
            int count = 0;
            bool showDialogs = showImportDialog || showMeshParametersDialog;
            _history.Clear();
            List<string> errors = new List<string>();
            //
            foreach (Command command in _commands)
            {
                if (count++ <= _currPositionIndex)
                {                    
                    // Write to form
                    WriteToOutput(command);
                    // Try
                    try
                    {
                        // Skip save
                        if (command is CSaveToPmx) { }
                        else 
                        {

                            // Execute with dialog
                            if (showDialogs && command is ICommandWithDialog icwd &&
                                ((showImportDialog && command is CImportFile) ||
                                 (showMeshParametersDialog && command is CSetMeshingParameters)))
                            {
                                icwd.ExecuteWithDialogs(_controller);
                            }
                            // Execute without dialog
                            else command.Execute(_controller);
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add(command.Name + ": " + ex.Message);
                    }
                    // Add history
                    AddToHistory(command);
                }
                else break;
            }
            // Report Errors
            if (errors.Count != 0)
            {
                WriteOutput?.Invoke("");
                WriteOutput?.Invoke("****   Exceptions   ****");                
                foreach (var error in errors)
                {
                    WriteOutput?.Invoke(error);
                }
                WriteOutput?.Invoke("****   Number of exceptions: " + errors.Count + "   ****");
            }
            // Write to file
            WriteToFile();
            //
            OnEnableDisableUndoRedo();
        }
        // Clear
        public void Clear()
        {
            _currPositionIndex = -1;
            _commands.Clear();
            _history.Clear();
            _previousView = ViewGeometryModelResults.Geometry;

            // write to file
            WriteToFile();

            OnEnableDisableUndoRedo();
            
            ModelChanged_ResetJobStatus?.Invoke();
        }
        // Undo / Redo
        public void Undo()
        {
            if (IsUndoPossible)
            {
                _currPositionIndex--;
                ExecuteAllCommands();   // also rewrites history
                //
                OnEnableDisableUndoRedo();
            }
        }
        public void Redo()
        {
            if (IsRedoPossible)
            {
                //_currPositionIndex++;
                ExecuteCommand(_commands[_currPositionIndex + 1], false);  // also rewrites history
            }
        }
        public void OnEnableDisableUndoRedo()
        {
            string undo = null;
            string redo = null;
            //
            if (IsUndoPossible) undo = _commands[_currPositionIndex].Name;
            if (IsRedoPossible) redo = _commands[_currPositionIndex + 1].Name;
            //
            if (EnableDisableUndoRedo != null) EnableDisableUndoRedo(undo, redo);
        }
        private bool IsUndoPossible
        {
            get { return _currPositionIndex > -1; }
        }
        private bool IsRedoPossible
        {
            get { return _currPositionIndex < _commands.Count - 1; }
        }
        // Write
        private void WriteToOutput(Command command)
        {
            if (command is CClear) return;
            string data = command.GetCommandString();
            if (data.Length > 20) data = data.Substring(20);    // Remove date and time for the write to form
            WriteOutput?.Invoke(data);
        }
        private void WriteToFile()
        {
            if (_commands.Count > 1)
            {
                // write to files
                File.WriteAllLines(_historyFileNameTxt, _history.ToArray());
                _commands.DumpToFile(_historyFileNameBin);
            }
        }
        public void ReadFromFile(string fileName)
        {
            _commands = CaeGlobals.Tools.LoadDumpFromFile<List<Command>>(fileName);
            _currPositionIndex = _commands.Count - 1;
        }
        // History files
        public string GetHistoryFileNameTxt()
        {
            return _historyFileNameTxt;
        }
        public string GetHistoryFileNameBin()
        {
            return _historyFileNameBin;
        }
        public void DeleteHistoryFiles()
        {
            if (File.Exists(_historyFileNameTxt)) File.Delete(_historyFileNameTxt);
            if (File.Exists(_historyFileNameBin)) File.Delete(_historyFileNameBin);
        }
    }
}
