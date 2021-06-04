using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using System.Drawing.Design;

namespace PrePoMax.Settings
{
    [Serializable]
    public class ViewCalculixSettings : IViewSettings, IReset
    {
        // Variables                                                                                                                
        private CalculixSettings _calculixSettings;
        private DynamicCustomTypeDescriptor _dctd = null;


        // Properties                                                                                                               
        [CategoryAttribute("Calculix")]
        [OrderedDisplayName(0, 10, "Work directory")]
        [DescriptionAttribute("Select the work directory.")]
        [EditorAttribute(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(UITypeEditor))]
        public string WorkDirectory
        {
            get { return _calculixSettings.WorkDirectory; }
            set { _calculixSettings.WorkDirectory = value; }
        }
        //
        [CategoryAttribute("Calculix")]
        [OrderedDisplayName(1, 10, "Use .pmx folder as work directory")]
        [DescriptionAttribute("Select yes to use .pmx file folder as a work directory.")]
        public bool UsePmxFolderAsWorkDirectory
        {
            get { return _calculixSettings.UsePmxFolderAsWorkDirectory; }
            set { _calculixSettings.UsePmxFolderAsWorkDirectory = value; }
        }
        //
        [CategoryAttribute("Calculix")]
        [OrderedDisplayName(2, 10, "Executable")]
        [DescriptionAttribute("Select the calculix executable file (ccx.exe).")]
        [EditorAttribute(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(UITypeEditor))]
        public string CalculixExe
        {
            get { return _calculixSettings.CalculixExe; }
            set { _calculixSettings.CalculixExe = value; }
        }
        //
        [CategoryAttribute("Calculix")]
        [OrderedDisplayName(3, 10, "Default solver")]
        [DescriptionAttribute("Select the default matrix solver type.")]
        public CaeModel.SolverTypeEnum DefaultSolverType
        {
            get { return _calculixSettings.DefaultSolverType; }
            set { _calculixSettings.DefaultSolverType = value; }
        }
        //
        [CategoryAttribute("Parallelization")]
        [OrderedDisplayName(0, 10, "Number of processors")]
        [DescriptionAttribute("Set the number of processors for the executable to use (OMP_NUM_THREADS = n).")]
        public int NumCPUs
        {
            get { return _calculixSettings.NumCPUs; }
            set { _calculixSettings.NumCPUs = value; }
        }
        //
        [CategoryAttribute("Parallelization")]
        [OrderedDisplayName(1, 10, "Environment variables")]
        [DescriptionAttribute("Add additional environment variables needed for the executable to run.")]
        [Editor(typeof(Forms.EnvVarsUIEditor), typeof(UITypeEditor))]
        public List<CaeJob.EnvironmentVariable> EnvironmentVariables
        {
            get { return _calculixSettings.EnvironmentVariables; }
            set { _calculixSettings.EnvironmentVariables = value; }
        }


        // Constructors                                                                                                             
        public ViewCalculixSettings(CalculixSettings calculixSettings)
        {
            _calculixSettings = calculixSettings;
            _dctd = ProviderInstaller.Install(this);
            // Now lets display Yes/No instead of True/False
            _dctd.RenameBooleanPropertyToYesNo(nameof(UsePmxFolderAsWorkDirectory));
        }

        // Methods                                                                                                                  
        public ISettings GetBase()
        {
            return _calculixSettings;
        }
        public void Reset()
        {
            _calculixSettings.Reset();
        }
    }

}
