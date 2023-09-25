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
    class CReplaceMeshRefinement : Command, ICommandWithDialog
    {
        // Variables                                                                                                                
        private string _oldMeshRefinementName;
        private FeMeshRefinement _newMeshRefinement;


        // Constructor                                                                                                              
        public CReplaceMeshRefinement(string oldMeshRefinementName, FeMeshRefinement newMeshRefinement)
            : base("Edit mesh refinement")
        {
            _oldMeshRefinementName = oldMeshRefinementName;
            _newMeshRefinement = newMeshRefinement.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ReplaceMeshSetupItem(_oldMeshRefinementName, _newMeshRefinement.DeepClone());
            return true;
        }
        // ICommandWithDialog
        public bool ExecuteWithDialog(Controller receiver)
        {
            _newMeshRefinement = (FeMeshRefinement)receiver.EditMeshSetupItemByForm(_newMeshRefinement.DeepClone());
            return Execute(receiver);
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + _oldMeshRefinementName + ", " + _newMeshRefinement.ToString();
        }
    }
}
