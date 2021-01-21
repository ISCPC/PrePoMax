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
    class CPropagateHisotryOutput : Command
    {
        // Variables                                                                                                                
        private string _stepName;
        private string _historyOutputName;


        // Constructor                                                                                                              
        public CPropagateHisotryOutput(string stepName, string historyOutputName)
            : base("Propagate history output")
        {
            _stepName = stepName;
            _historyOutputName = historyOutputName;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.PropagateHistoryOutput(_stepName, _historyOutputName);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _stepName + ": " + _historyOutputName;
        }
    }
}
