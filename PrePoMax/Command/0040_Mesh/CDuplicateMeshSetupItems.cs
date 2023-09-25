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
    class CDuplicateMeshSetupItems : Command
    {
        // Variables                                                                                                                
        private string[] _meshSetupItemNames;


        // Constructor                                                                                                              
        public CDuplicateMeshSetupItems(string[] meshSetupItemNames)
            : base("Duplicate mesh setup item")
        {
            _meshSetupItemNames = meshSetupItemNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.DuplicateMeshSetupItems(_meshSetupItemNames);
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_meshSetupItemNames);
        }
    }
}
