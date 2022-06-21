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
    class CAddStep : Command
    {
        // Variables                                                                                                                
        private Step _step;
        private bool _copyBCsAndLoads;


        // Constructor                                                                                                              
        public CAddStep(Step step, bool copyBCsAndLoads)
            : base("Add step")
        {
            _step = step.DeepClone();
            _copyBCsAndLoads = copyBCsAndLoads;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.AddStep(_step.DeepClone(), _copyBCsAndLoads);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _step.ToString();
        }
    }
}
