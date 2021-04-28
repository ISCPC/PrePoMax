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
    class CRemoveJobs : Command
    {
        // Variables                                                                                                                
        private string[] _jobNames;


        // Constructor                                                                                                              
        public CRemoveJobs(string[] jobNames)
            : base("Remove analyses")
        {
            _jobNames = jobNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.RemoveJobs(_jobNames);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_jobNames);
        }
    }
}
