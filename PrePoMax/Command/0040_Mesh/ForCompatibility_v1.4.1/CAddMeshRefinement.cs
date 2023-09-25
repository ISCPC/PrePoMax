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
    class CAddMeshRefinement : Command, ICommandWithDialog
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
            receiver.AddMeshSetupItem(_meshRefinement.DeepClone());
            return true;
        }
        // ICommandWithDialog
        public bool ExecuteWithDialog(Controller receiver)
        {
            _meshRefinement = (FeMeshRefinement)receiver.EditMeshSetupItemByForm(_meshRefinement.DeepClone());
            return Execute(receiver);
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + _meshRefinement.ToString();
        }
    }
}
