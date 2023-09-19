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
    class CDuplicateDefinedFields : Command
    {
        // Variables                                                                                                                
        private string _stepName;
        private string[] _definedFieldNames;


        // Constructor                                                                                                              
        public CDuplicateDefinedFields(string stepName, string[] definedFieldNames)
            :base("Duplicate defined fields")
        {
            _stepName = stepName;
            _definedFieldNames = definedFieldNames;
        }
       

        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.DuplicateDefinedFields(_stepName, _definedFieldNames);
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + _stepName + ": " + GetArrayAsString(_definedFieldNames);
        }
    }
}
