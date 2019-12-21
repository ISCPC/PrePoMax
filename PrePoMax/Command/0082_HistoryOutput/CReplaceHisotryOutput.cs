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
    class CReplaceHisotryOutput : Command
    {
        // Variables                                                                                                                
        private string _stepName;
        private string _oldHistoryOutputName;
        private HistoryOutput _newHistoryOutput;


        // Constructor                                                                                                              
        public CReplaceHisotryOutput(string stepName, string oldHistoryOutputName, HistoryOutput newHistoryOutput)
            : base("Edit history output")
        {
            _stepName = stepName;
            _oldHistoryOutputName = oldHistoryOutputName;
            _newHistoryOutput = newHistoryOutput.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ReplaceHistoryOutput(_stepName, _oldHistoryOutputName, _newHistoryOutput.DeepClone());
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _stepName + ": " + _oldHistoryOutputName + ", " + _newHistoryOutput.ToString();
        }
    }
}
