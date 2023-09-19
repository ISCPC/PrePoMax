using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrePoMax;
using CaeModel;
using CaeMesh;
using CaeGlobals;
using System.Runtime.Serialization;

namespace PrePoMax.Commands
{
    [Serializable]
    class CDuplicateHistoryOutputs : Command
    {
        // Variables                                                                                                                
        private string _stepName;
        private string[] _historyOutputNames;


        // Constructor                                                                                                              
        public CDuplicateHistoryOutputs(string stepName, string[] historyOutputNames)
            :base("Duplicate history outputs")
        {
            _stepName = stepName;
            _historyOutputNames = historyOutputNames;
        }
       

        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.DuplicateHistoryOutputs(_stepName, _historyOutputNames);
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + _stepName + ": " + GetArrayAsString(_historyOutputNames);
        }
    }
}
