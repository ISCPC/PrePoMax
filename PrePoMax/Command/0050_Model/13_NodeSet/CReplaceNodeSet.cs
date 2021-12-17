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
    class CReplaceNodeSet : Command
    {
        // Variables                                                                                                                
        private string _oldNodeSetName;
        private FeNodeSet _newNodeSet;

        // Constructor                                                                                                              
        public CReplaceNodeSet(string oldNodeSetName, FeNodeSet newNodeSet)
            : base("Edit node set")
        {
            _oldNodeSetName = oldNodeSetName;
            _newNodeSet = newNodeSet.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ReplaceNodeSet(_oldNodeSetName, _newNodeSet.DeepClone(), true);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _oldNodeSetName + ", " + _newNodeSet.ToString();
        }
    }
}
