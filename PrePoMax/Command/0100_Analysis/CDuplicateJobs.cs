using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrePoMax;
using CaeModel;
using CaeMesh;
using CaeGlobals;


namespace PrePoMax.Commands
{
    [Serializable]
    class CDuplicateJobs : Command
    {
        // Variables                                                                                                                
        private string[] _jobNames;


        // Constructor                                                                                                              
        public CDuplicateJobs(string[] jobNames)
            : base("Duplicate jobs")
        {
            _jobNames = jobNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.DuplicateJobs(_jobNames);
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_jobNames);
        }
    }
}
