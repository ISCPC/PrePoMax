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
    class CRemoveDefinedFields : Command
    {
        // Variables                                                                                                                
        private string _stepName;
        private string[] _definedFieldNames;

        // Constructor                                                                                                              
        public CRemoveDefinedFields(string stepName, string[] definedFieldNames)
            :base("Remove defined field")

        {
            _stepName = stepName;
            _definedFieldNames = definedFieldNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.RemoveDefinedFields(_stepName, _definedFieldNames);
            return true;
        }

        public override string GetCommandString()
        {

            return base.GetCommandString() + _stepName + ": " + GetArrayAsString(_definedFieldNames);
        }
    }
}
