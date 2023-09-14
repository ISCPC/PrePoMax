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
    class CDuplicateMaterials : Command
    {
        // Variables                                                                                                                
        private string[] _materialNames;


        // Constructor                                                                                                              
        public CDuplicateMaterials(string[] materialNames)
            : base("Duplicate materials")
        {
            _materialNames = materialNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.DuplicateMaterials(_materialNames);
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_materialNames);
        }
    }
}
