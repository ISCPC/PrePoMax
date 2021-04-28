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
    class CPropagateDefinedField : Command
    {
        // Variables                                                                                                                
        private string _stepName;
        private string _definedFieldName;


        // Constructor                                                                                                              
        public CPropagateDefinedField(string stepName, string definedFieldName)
            : base("Propagate defined field")
        {
            _stepName = stepName;
            _definedFieldName = definedFieldName;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.PropagateDefinedField(_stepName, _definedFieldName);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _stepName + ": " + _definedFieldName;
        }
    }
}
