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
    class CRemoveBCs : Command
    {
        // Variables                                                                                                                
        private string _stepName;
        private string[] _boundaryConditionNames;

        // Constructor                                                                                                              
        public CRemoveBCs(string stepName, string[] boundaryConditionNames)
            :base("Remove BCs")
        {
            _stepName = stepName;
            _boundaryConditionNames = boundaryConditionNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.RemoveBoundaryConditions(_stepName, _boundaryConditionNames);
            return true;
        }

        public override string GetCommandString()
        {

            return base.GetCommandString() + _stepName + ": " + GetArrayAsString(_boundaryConditionNames);
        }
    }
}
