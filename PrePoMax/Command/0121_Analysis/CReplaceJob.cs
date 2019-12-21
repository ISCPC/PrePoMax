using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrePoMax;
using CaeModel;
using CaeMesh;
using CaeGlobals;
using CaeJob;

namespace PrePoMax.Commands
{
    [Serializable]
    class CReplaceJob : Command
    {
        // Variables                                                                                                                
        private string _oldJobName;
        private AnalysisJob _newJob;

        // Constructor                                                                                                              
        public CReplaceJob(string oldJobName, AnalysisJob newJob)
            : base("Edit analysis")
        {
            _oldJobName = oldJobName;
            _newJob = newJob.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ReplaceJob(_oldJobName, _newJob.DeepClone());
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _oldJobName + ", " + _newJob.ToString();
        }
    }
}
