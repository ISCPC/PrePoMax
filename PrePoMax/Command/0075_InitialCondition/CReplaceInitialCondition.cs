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
    class CReplaceInitialCondition : Command
    {
        // Variables                                                                                                                
        private string _oldInitialConditionName;
        private InitialCondition _newInitialCondition;


        // Constructor                                                                                                              
        public CReplaceInitialCondition(string oldInitialConditionName, InitialCondition newInitialCondition)
            : base("Edit initial condition")
        {
            _oldInitialConditionName = oldInitialConditionName;
            _newInitialCondition = newInitialCondition.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ReplaceInitialCondition(_oldInitialConditionName, _newInitialCondition.DeepClone());
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + _oldInitialConditionName + ", " + _newInitialCondition.ToString();
        }
    }
}
