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
    class CReplaceDefinedField : Command
    {
        // Variables                                                                                                                
        private string _stepName;
        private string _oldDefinedFieldName;
        private DefinedField _newDefinedField;


        // Constructor                                                                                                              
        public CReplaceDefinedField(string stepName, string oldDefinedFieldName, DefinedField newDefinedField)
            : base("Edit defined field")
        {
            _stepName = stepName;
            _oldDefinedFieldName = oldDefinedFieldName;
            _newDefinedField = newDefinedField.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ReplaceDefinedField(_stepName, _oldDefinedFieldName, _newDefinedField.DeepClone());
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _stepName + ": " + _oldDefinedFieldName + ", " + _newDefinedField.ToString();
        }
    }
}
