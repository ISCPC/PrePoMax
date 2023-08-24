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
    class CRemoveParameters : Command
    {
        // Variables                                                                                                                
        private string[] _parameterNames;


        // Constructor                                                                                                              
        public CRemoveParameters(string[] parameterNames)
            : base("Remove parameters")
        {
            _parameterNames = parameterNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.RemoveParameters(_parameterNames);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_parameterNames);
        }
    }
}
