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
    class CPropagateBC : Command
    {
        // Variables                                                                                                                
        private string _stepName;
        private string _boundaryConditionName;


        // Constructor                                                                                                              
        public CPropagateBC(string stepName, string boundaryConditionName)
            : base("Propagate BC")
        {
            _stepName = stepName;
            _boundaryConditionName = boundaryConditionName;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.PropagateBoundaryCondition(_stepName, _boundaryConditionName);
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + _stepName + ": " + _boundaryConditionName;
        }
    }
}
