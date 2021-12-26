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
    class CRenumberNodes : Command
    {
        // Variables                                                                                                                
        private int _startNodeId;


        // Constructor                                                                                                              
        public CRenumberNodes(int startNodeId)
            : base("Renumber nodes")
        {
            _startNodeId = startNodeId;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.RenumberNodes(_startNodeId);
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + _startNodeId;
        }
    }
}
