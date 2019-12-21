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
    class CRemoveHistoryOutputs : Command
    {
        // Variables                                                                                                                
        private string _stepName;
        private string[] _historyOutputNames;

        // Constructor                                                                                                              
        public CRemoveHistoryOutputs(string stepName, string[] historyOutputNames)
            :base("Remove history outputs")

        {
            _stepName = stepName;
            _historyOutputNames = historyOutputNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.RemoveHistoryOutputs(_stepName, _historyOutputNames);
            return true;
        }

        public override string GetCommandString()
        {

            return base.GetCommandString() + _stepName + ": " + GetArrayAsString(_historyOutputNames);
        }
    }
}
