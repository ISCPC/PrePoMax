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
        private bool _update;


        // Constructor                                                                                                              
        public CAddConstraint(Constraint constraint, bool update)
            : base("Add constraint")
        {
            _constraint = constraint.DeepClone();
            _update = update;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.AddConstraint(_constraint.DeepClone(), _update);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _constraint.ToString();
        }
    }
}
