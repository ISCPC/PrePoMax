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
    class CDuplicateMeshingParameters : Command
    {
        // Variables                                                                                                                
        private string[] _meshingParameterNames;


        // Constructor                                                                                                              
        public CDuplicateMeshingParameters(string[] meshingParameterNames)
            : base("Duplicate meshing parameters")
        {
            _meshingParameterNames = meshingParameterNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.DuplicateMeshingParameters(_meshingParameterNames);
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_meshingParameterNames);
        }
    }
}
