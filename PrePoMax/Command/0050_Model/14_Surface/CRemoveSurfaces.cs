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
    class CRemoveSurfaces : Command
    {
        // Variables                                                                                                                
        private string[] _surfaceNames;


        // Constructor                                                                                                              
        public CRemoveSurfaces(string[] surfaceNames)
            : base("Remove surfaces")
        {
            _surfaceNames = surfaceNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.RemoveSurfaces(_surfaceNames);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_surfaceNames);
        }
    }
}
