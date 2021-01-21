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
    class CPropagateFieldOutput : Command
    {
        // Variables                                                                                                                
        private string _stepName;
        private string _fieldOutputName;


        // Constructor                                                                                                              
        public CPropagateFieldOutput(string stepName, string fieldOutputName)
            : base("Propagate field output")
        {
            _stepName = stepName;
            _fieldOutputName = fieldOutputName;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.PropagateFieldOutput(_stepName, _fieldOutputName);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _stepName + ": " + _fieldOutputName;
        }
    }
}
