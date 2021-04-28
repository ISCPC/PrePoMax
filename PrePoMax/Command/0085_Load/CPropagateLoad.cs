using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrePoMax;
using CaeModel;
using CaeGlobals;

namespace PrePoMax.Commands
{
    [Serializable]
    class CPropagateLoad : Command
    {
        // Variables                                                                                                                
        private string _stepName;
        private string _loadName;


        // Constructor                                                                                                              
        public CPropagateLoad(string stepName, string loadName)
            : base("Propagate load")
        {
            _stepName = stepName;
            _loadName = loadName;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.PropagateLoad(_stepName, _loadName);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _stepName + ": " + _loadName;
        }
    }
}
