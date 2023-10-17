using CaeGlobals;
using CaeMesh.Meshing;
using Octree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public abstract class MeshSetupItem : NamedClass, ISerializable
    {
        // Variables                                                                                                                
        protected int[] _creationIds;               //ISerializable
        protected Selection _creationData;          //ISerializable


        // Properties                                                                                                               
        public int[] CreationIds { get { return _creationIds; } set { _creationIds = value; } }
        public Selection CreationData { get { return _creationData; } set { _creationData = value; } }


        // Constructors                                                                                                             
        public MeshSetupItem(string name)
            : base(name)
        {
            _creationIds = null;
            _creationData = null;
        }
        public MeshSetupItem(MeshSetupItem meshSetupItem)
            : base("tmpName")
        {
            CopyFrom(meshSetupItem);
        }
        public MeshSetupItem(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_creationIds":
                    case "_geometryIds":    // Compatibility for version v1.4.1
                        _creationIds = (int[])entry.Value; break;
                    case "_creationData":
                        _creationData = (Selection)entry.Value; break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        public virtual void Reset()
        {
            // Do not clear the selection
            //_creationIds = null;
            //_creationData = null;
        }
        public void CopyFrom(MeshSetupItem meshSetupItem)
        {
            base.CopyFrom(meshSetupItem);
            //
            _creationIds = meshSetupItem.CreationIds?.ToArray();
            _creationData = meshSetupItem.CreationData?.DeepClone();
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            // Using typeof() works also for null fields
            info.AddValue("_creationIds", _creationIds, typeof(int[]));
            info.AddValue("_creationData", _creationData, typeof(Selection));
        }

    }
}
