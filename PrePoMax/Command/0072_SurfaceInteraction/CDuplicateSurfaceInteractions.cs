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
    class CDuplicateSurfaceInteractions : Command
    {
        // Variables                                                                                                                
        private string[] _surfaceInteractionNames;


        // Constructor                                                                                                              
        public CDuplicateSurfaceInteractions(string[] surfaceInteractionNames)
            : base("Duplicate surface interactions")
        {
            _surfaceInteractionNames = surfaceInteractionNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.DuplicateSurfaceInteractions(_surfaceInteractionNames);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_surfaceInteractionNames);
        }
    }
}
