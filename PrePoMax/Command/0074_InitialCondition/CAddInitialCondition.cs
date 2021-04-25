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
    class CAddInitialCondition : Command
    {
        // Variables                                                                                                                
        private InitialCondition _initialCondition;


        // Constructor                                                                                                              
        public CAddInitialCondition(InitialCondition initialCondition)
            : base("Add initial condition")
        {
            _initialCondition = initialCondition.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.AddInitialCondition(_initialCondition.DeepClone());
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _initialCondition.ToString();
        }
    }
}
