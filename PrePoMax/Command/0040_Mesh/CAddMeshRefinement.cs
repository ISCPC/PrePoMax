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
    class CAddMeshRefinement : Command
    {
        // Variables                                                                                                                
        private FeMeshRefinement _meshRefinement;


        // Constructor                                                                                                              
        public CAddMeshRefinement(FeMeshRefinement meshRefinement)
            : base("Add mesh refinement")
        {
            _meshRefinement = meshRefinement.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.AddMeshRefinement(_meshRefinement.DeepClone());
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + _meshRefinement.ToString();
        }
    }
}
