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
    class CReplaceMeshSetupItem : Command, ICommandWithDialog
    {
        // Variables                                                                                                                
        private string _oldMeshSetupItemName;
        private MeshSetupItem _meshSetupItem;


        // Constructor                                                                                                              
        public CReplaceMeshSetupItem(string oldMeshSetupItemName, MeshSetupItem newMeshSetupItem)
            : base("Edit mesh setup item")
        {
            _oldMeshSetupItemName = oldMeshSetupItemName;
            _meshSetupItem = newMeshSetupItem.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ReplaceMeshSetupItem(_oldMeshSetupItemName, _meshSetupItem.DeepClone());
            return true;
        }
        // ICommandWithDialog
        public bool ExecuteWithDialog(Controller receiver)
        {
            if (_meshSetupItem is MeshingParameters mp)
                _meshSetupItem = (MeshingParameters)receiver.EditMeshSetupItemByForm(mp.DeepClone());
            else if (_meshSetupItem is FeMeshRefinement mr)
                _meshSetupItem = (FeMeshRefinement)receiver.EditMeshSetupItemByForm(mr.DeepClone());
            else throw new NotSupportedException("MeshSetupItemTypeException");
            //
            return Execute(receiver);
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + _oldMeshSetupItemName + ", " + _meshSetupItem.ToString();
        }
    }
}
