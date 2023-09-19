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
    class CDuplicateMeshRefinements : Command
    {
        // Variables                                                                                                                
        private string[] _meshRefinementNames;


        // Constructor                                                                                                              
        public CDuplicateMeshRefinements(string[] meshRefinementNames)
            : base("Duplicate mesh refinements")
        {
            _meshRefinementNames = meshRefinementNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.DuplicateMeshRefinements(_meshRefinementNames);
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_meshRefinementNames);
        }
    }
}
