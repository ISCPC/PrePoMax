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
    class CDuplicateNodeSets : Command
    {
        // Variables                                                                                                                
        private string[] _nodeSetNames;


        // Constructor                                                                                                              
        public CDuplicateNodeSets(string[] nodeSetNames)
            : base("Duplicate node set")
        {
            _nodeSetNames = nodeSetNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.DuplicateNodeSets(_nodeSetNames);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_nodeSetNames);
        }
    }
}
