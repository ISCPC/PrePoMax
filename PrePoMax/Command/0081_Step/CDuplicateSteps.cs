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
    class CDuplicateSteps : Command
    {
        // Variables                                                                                                                
        private string[] _stepNames;


        // Constructor                                                                                                              
        public CDuplicateSteps(string[] stepNames)
            : base("Duplicate steps")
        {
            _stepNames = stepNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.DuplicateSteps(_stepNames);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_stepNames);
        }
    }
}
