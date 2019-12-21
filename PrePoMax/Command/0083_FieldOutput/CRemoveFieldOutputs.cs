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
    class CRemoveFieldOutputs : Command
    {
        // Variables                                                                                                                
        private string _stepName;
        private string[] _fieldOutputNames;

        // Constructor                                                                                                              
        public CRemoveFieldOutputs(string stepName, string[] fieldOutputNames)
            :base("Remove field outputs")

        {
            _stepName = stepName;
            _fieldOutputNames = fieldOutputNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.RemoveFieldOutputs(_stepName, _fieldOutputNames);
            return true;
        }

        public override string GetCommandString()
        {

            return base.GetCommandString() + _stepName + ": " + GetArrayAsString(_fieldOutputNames);
        }
    }
}
