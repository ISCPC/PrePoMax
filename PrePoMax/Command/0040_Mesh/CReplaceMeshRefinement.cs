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
    class CReplaceMeshRefinement : Command
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
            receiver.ReplaceMeshRefinement(_oldMeshRefinementName, _newMeshRefinement.DeepClone());
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + _oldMeshRefinementName + ", " + _newMeshRefinement.ToString();
        }
    }
}
