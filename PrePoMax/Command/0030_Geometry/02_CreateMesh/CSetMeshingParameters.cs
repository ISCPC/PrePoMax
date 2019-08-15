using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrePoMax;
using CaeModel;
using CaeMesh;
using CaeGlobals;
using System.Runtime.Serialization;

namespace PrePoMax.Commands
{
    [Serializable]
    class CSetMeshingParameters : Command, ICommandWithDialog, ISerializable
    {
        // Variables                                                                                                                
        private string[] _partNames;
        private MeshingParameters _meshingParameters;


        // Constructors                                                                                                             
        public CSetMeshingParameters(string[] partNames, MeshingParameters meshingParameters)
            : base("Set meshing parameters")
        {
            _partNames = partNames;
            _meshingParameters = meshingParameters.DeepClone();
        }
        public CSetMeshingParameters(SerializationInfo info, StreamingContext context)
            : base("Set meshing parameters")
        {
            string partName = null;         // old serialization parameter
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "Command+_dateCreated":
                        _dateCreated = (DateTime)entry.Value; break;
                    case "_partName":
                        partName = (string)entry.Value; break;
                    case "_partNames":
                        _partNames = (string[])entry.Value; break;
                    case "_meshingParameters":
                        _meshingParameters = (MeshingParameters)entry.Value; break;
                }
            }
            if (_partNames == null && partName != null) _partNames = new string[] { partName };
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            foreach (var partName in _partNames)
            {
                receiver.SetMeshingParameters(partName, _meshingParameters.DeepClone());
            }
            return true;
        }
        public void ExecuteWithDialogs(Controller receiver)
        {
            // first set previously used meshing parameters
            foreach (var partName in _partNames)
            {
                receiver.SetMeshingParameters(partName, _meshingParameters.DeepClone());
            }
            // get new meshing parameters
            MeshingParameters meshingParameters = receiver.GetMeshingParameters(_partNames);
            // if new parameters were defined use them, else use old mesh parameters
            if (meshingParameters != null) _meshingParameters = meshingParameters;
            Execute(receiver);
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_partNames);
        }


        // ISerialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Command+_dateCreated", _dateCreated, typeof(DateTime));
            info.AddValue("_partNames", _partNames, typeof(string[]));
            info.AddValue("_meshingParameters", _meshingParameters, typeof(MeshingParameters));
        }
    }
}
