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
    class CAddNodeSet : Command
    {
        // Variables                                                                                                                
        private FeNodeSet _nodeSet;


        // Constructor                                                                                                              
        public CAddNodeSet(FeNodeSet nodeSet)
            : base("Add node set")
        {
            _nodeSet = nodeSet.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.AddNodeSet(_nodeSet.DeepClone());
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _nodeSet.ToString();
        }
    }
}
