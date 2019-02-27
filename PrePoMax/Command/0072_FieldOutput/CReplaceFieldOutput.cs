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
    class CReplaceFieldOutput : Command
    {
        // Variables                                                                                                                
        private string _stepName;
        private string _oldFieldOutputName;
        private FieldOutput _newFieldOutput;


        // Constructor                                                                                                              
        public CReplaceFieldOutput(string stepName, string oldFieldOutputName, FieldOutput newFieldOutput)
            : base("Edit field output")
        {
            _stepName = stepName;
            _oldFieldOutputName = oldFieldOutputName;
            _newFieldOutput = newFieldOutput.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ReplaceFieldOutput(_stepName, _oldFieldOutputName, _newFieldOutput.DeepClone());
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _stepName + ": " + _oldFieldOutputName + ", " + _newFieldOutput.ToString();
        }
    }
}
