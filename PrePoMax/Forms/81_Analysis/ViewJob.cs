using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Management;
using System.ComponentModel;
using CaeJob;
using CaeGlobals;
using System.Drawing.Design;

namespace PrePoMax.Forms
{
    [Serializable]
    public class ViewJob
    {
        // Variables                                                                                                                
        protected AnalysisJob _job;
        
        
        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the analysis.")]
        public string Name { get { return _job.Name; } set { _job.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Executable")]
        [DescriptionAttribute("Calculix executable file (ccx.exe).")]
        public string Executable
        {
            get { return _job.Executable; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Executable arguments")]
        [DescriptionAttribute("Addtional Calculix arguments. Change this value only if you want to run the solver in a different way.")]
        public string Argument
        {
            get { return _job.Argument; }
            set { _job.Argument = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(3, 10, "Work directory")]
        [DescriptionAttribute("Work directory.")]
        public string WorkDirectory { get { return _job.WorkDirectory; } }


        // Constructor                                                                                                              
        public ViewJob(AnalysisJob job)
        {
            _job = job;
        }


        // Methods                                                                                                                  
        public AnalysisJob GetBase()
        {
            return _job;
        }
    }
}
