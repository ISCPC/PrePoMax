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
    class CRemoveMeshSetupItems : Command
    {
        // Variables                                                                                                                
        private string[] _meshSetupItemNames;


        // Constructor                                                                                                              
        public CRemoveMeshSetupItems(string[] meshSetupItemNames)
            : base("Remove mesh setup item")
        {
            _meshSetupItemNames = meshSetupItemNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.RemoveMeshSetupItems(_meshSetupItemNames);
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_meshSetupItemNames);
        }
    }
}
