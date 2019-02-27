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
    class CAddConstraint : Command
    {
        // Variables                                                                                                                
        private Constraint _constraint;


        // Constructor                                                                                                              
        public CAddConstraint(Constraint constraint)
            : base("Add constraint")
        {
            _constraint = constraint.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.AddConstraint(_constraint.DeepClone());
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _constraint.ToString();
        }
    }
}
