using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrePoMax;
using CaeModel;
using CaeMesh;
using CaeGlobals;


namespace PrePoMax.Commands
{
    [Serializable]
    class CReplaceParameter : Command
    {
        // Variables                                                                                                                
        private string _oldParameterName;
        private EquationParameter _newParameter;


        // Constructor                                                                                                              
        public CReplaceParameter(string oldParameterName, EquationParameter newParameter)
            : base("Edit Parameter")
        {
            _oldParameterName = oldParameterName;
            _newParameter = newParameter.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ReplaceParameter(_oldParameterName, _newParameter.DeepClone());
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _oldParameterName + ", " + _newParameter.ToString();
        }
    }
}
