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
    class CRemoveNodeSets : Command
    {
        // Variables                                                                                                                
        private string[] _nodeSetNames;


        // Constructor                                                                                                              
        public CRemoveNodeSets(string[] nodeSetNames)
            : base("Remove node sets")
        {
            _nodeSetNames = nodeSetNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.RemoveNodeSets(_nodeSetNames);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_nodeSetNames);
        }
    }
}
