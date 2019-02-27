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
    class CAddJob : Command
    {
        // Variables                                                                                                                
        private AnalysisJob _job;


        // Constructor                                                                                                              
        public CAddJob(AnalysisJob job)
            : base("Add analysis")
        {
            _job = job.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.AddJob(_job.DeepClone());
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _job.ToString();
        }
    }
}
