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
    class CRemoveContactPairs : Command
    {
        // Variables                                                                                                                
        private string[] _contactPairNames;


        // Constructor                                                                                                              
        public CRemoveContactPairs(string[] contactPairNames)
            : base("Remove contact pairs")
        {
            _contactPairNames = contactPairNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.RemoveContactPairs(_contactPairNames);
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_contactPairNames);
        }
    }
}
