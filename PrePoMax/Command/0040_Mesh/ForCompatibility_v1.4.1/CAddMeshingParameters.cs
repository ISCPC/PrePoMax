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
    class CAddMeshingParameters : Command, ICommandWithDialog
    {
        // Variables                                                                                                                
        private MeshingParameters _meshingParameters;


        // Constructor                                                                                                              
        public CAddMeshingParameters(MeshingParameters meshingParameters)
            : base("Add meshing parameters")
        {
            _meshingParameters = meshingParameters.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.AddMeshSetupItem(_meshingParameters.DeepClone());
            return true;
        }
        // ICommandWithDialog
        public bool ExecuteWithDialog(Controller receiver)
        {
            _meshingParameters = (MeshingParameters)receiver.EditMeshSetupItemByForm(_meshingParameters.DeepClone());
            return Execute(receiver);
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + _meshingParameters.ToString();
        }
    }
}
