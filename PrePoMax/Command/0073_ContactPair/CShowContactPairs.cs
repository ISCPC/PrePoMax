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
    class CShowContactPairs : Command
    {
        // Variables                                                                                                                
        private string[] _contactPairNames;


        // Constructor                                                                                                              
        public CShowContactPairs(string[] contactPairNames)
            : base("Show contact pairs")
        {
            _contactPairNames = contactPairNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ShowContactPairs(_contactPairNames);
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_contactPairNames);
        }
    }
}
