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
    class CReplaceStep : Command
    {
        // Variables                                                                                                                
        private string _oldStepName;
        private Step _newStep;

        // Constructor                                                                                                              
        public CReplaceStep(string oldStepName, Step newStep)
            : base("Edit step")
        {
            _oldStepName = oldStepName;
            _newStep = newStep.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ReplaceStep(_oldStepName, _newStep.DeepClone());
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _oldStepName + ", " + _newStep.ToString();
        }
    }
}
