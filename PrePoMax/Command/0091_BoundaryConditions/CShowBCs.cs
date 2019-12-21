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
    class CShowBCs : Command
    {
        // Variables                                                                                                                
        private string _stepName;
        private string[] _boundaryConditionNames;


        // Constructor                                                                                                              
        public CShowBCs(string stepName, string[] boundaryConditionNames)
            : base("Show boundary conditions")
        {
            _stepName = stepName; 
            _boundaryConditionNames = boundaryConditionNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ShowBoundaryConditions(_stepName, _boundaryConditionNames);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _stepName + ": " + GetArrayAsString(_boundaryConditionNames);
        }
    }
}
