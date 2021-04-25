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
    class CRemoveInitialConditions : Command
    {
        // Variables                                                                                                                
        private string[] _initialConditionNames;


        // Constructor                                                                                                              
        public CRemoveInitialConditions(string[] initialConditionNames)
            : base("Remove initial conditions")
        {
            _initialConditionNames = initialConditionNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.RemoveInitialConditions(_initialConditionNames);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_initialConditionNames);
        }
    }
}
