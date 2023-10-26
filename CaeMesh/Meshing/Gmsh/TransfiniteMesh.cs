using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using CaeMesh.Meshing;
using DynamicTypeDescriptor;
using GmshCommon;

namespace CaeMesh
{
    [Serializable]
    public class TransfiniteMesh : GmshSetupItem, ISerializable
    {
        // Variables                                                                                                                


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public TransfiniteMesh(string name)
            : base(name)
        {
            Reset();
        }
        public TransfiniteMesh(TransfiniteMesh transfiniteMesh)
            : base("tmpName")
        {
            CopyFrom(transfiniteMesh);
        }
        public TransfiniteMesh(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        public override void Reset()
        {
            base.Reset();
            AlgorithmRecombine = GmshAlgorithmRecombineEnum.Simple;
        }
        public void CopyFrom(TransfiniteMesh transfiniteMesh)
        {
            base.CopyFrom(transfiniteMesh);
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            // Using typeof() works also for null fields
        }
    }
}
