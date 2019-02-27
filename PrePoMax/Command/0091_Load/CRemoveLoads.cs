using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrePoMax;
using CaeModel;

namespace PrePoMax.Commands
{
    [Serializable]
    class CRemoveLoads : Command
    {
        // Variables                                                                                                                
        private string _stepName;
        private string[] _loadNames;

        // Constructor                                                                                                              
        public CRemoveLoads(string stepName, string[] loadNames)
            : base("Remove loads")
        {
            _stepName = stepName;
            _loadNames = loadNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.RemoveLoads(_stepName, _loadNames);
            return true;
        }

        public override string GetCommandString()
        {

            return base.GetCommandString() + _stepName + ": " + GetArrayAsString(_loadNames);
        }
    }
}
