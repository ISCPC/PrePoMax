using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using System.IO;
using CaeJob;

namespace PrePoMax
{
    [Serializable]
    public class CalculixSettings : PrePoMax.Settings.ViewSettings, ISettings, Settings.IReset
    {
        // Variables                                                                                                                
        private string _workDirectory;
        private string _executable;
        private int _numCPUs;
        private List<EnvironmentVariable> _environmentVariables;


        // Properties                                                                                                               
        [CategoryAttribute("Calculix")]
        [OrderedDisplayName(0, 10, "Work directory")]
        [DescriptionAttribute("Select the work directory.")]
        [EditorAttribute(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string WorkDirectory 
        { 
            get { return Tools.GetGlobalPath(_workDirectory); }
            set 
            {
                string path = Tools.GetGlobalPath(value);
                if (!Directory.Exists(path))
                    throw new Exception("The selected work directory does not exist.");
                _workDirectory = Tools.GetLocalPath(path);
            } 
        }

        [CategoryAttribute("Calculix")]
        [OrderedDisplayName(1, 10, "Executable")]
        [DescriptionAttribute("Select the calculix executable file (ccx.exe).")]
        [EditorAttribute(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string CalculixExe 
        {
            get { return Tools.GetGlobalPath(_executable); }
            set
            {
                string path = Tools.GetGlobalPath(value);
                if (!File.Exists(path))
                    throw new Exception("The selected calculix executable does not exist.");
                if (Path.GetExtension(path) != ".exe" && Path.GetExtension(path) != ".bat" && Path.GetExtension(path) != ".cmd")
                    throw new Exception("The selected executable's file extension is not '.exe', '.bat' or '.cmd'.");
                _executable = Tools.GetLocalPath(path);
            }
        }

        [CategoryAttribute("Parallelization")]
        [OrderedDisplayName(0, 10, "Number of processors")]
        [DescriptionAttribute("Set the number of processors for the executable to use (OMP_NUM_THREADS = n).")]
        public int NumCPUs
        {
            get { return _numCPUs; }
            set
            {
                _numCPUs = value;
                if (_numCPUs < 1) _numCPUs = 1;
            }
        }

        [CategoryAttribute("Parallelization")]
        [OrderedDisplayName(1, 10, "Environment variables")]
        [DescriptionAttribute("Add additional environment variables needed for the executable to run.")]
        [Editor(typeof(Forms.EnvVarsUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public List<EnvironmentVariable> EnvironmentVariables { get { return _environmentVariables; } set { _environmentVariables = value; } }



        // Constructors                                                                                                             
        public CalculixSettings()
        {
            Reset();
        }


        // Methods                                                                                                                  
        public override ISettings GetBase()
        {
            return this;
        }
        public void CheckValues()
        {
        }
        public void Reset()
        {
            _workDirectory = null;
            _executable = null;
            _numCPUs = 1;
            _environmentVariables = null;
        }
    }
}
