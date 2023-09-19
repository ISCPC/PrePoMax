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
    class CDuplicateBCs : Command
    {
        // Variables                                                                                                                
        private string _stepName;
        private string[] _boundaryConditionNames;


        // Constructor                                                                                                              
        public CDuplicateBCs(string stepName, string[] boundaryConditionNames)
            :base("Duplicate boundary conditions")
        {
            _stepName = stepName;
            _boundaryConditionNames = boundaryConditionNames;
        }
       

        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.DuplicateBoundaryConditions(_stepName, _boundaryConditionNames);
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + _stepName + ": " + GetArrayAsString(_boundaryConditionNames);
        }
    }
}
