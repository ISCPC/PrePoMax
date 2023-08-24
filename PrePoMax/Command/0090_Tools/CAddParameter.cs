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
    class CAddParameter : Command
    {
        // Variables                                                                                                                
        private EquationParameter _parameter;


        // Constructor                                                                                                              
        public CAddParameter(EquationParameter parameter)
            : base("Add parameter")
        {
            _parameter = parameter.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.AddParameter(_parameter.DeepClone());
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _parameter.ToString();
        }
    }
}
