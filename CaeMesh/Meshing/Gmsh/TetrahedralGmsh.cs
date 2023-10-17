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
    public class TetrahedralGmsh : GmshSetupItem, ISerializable
    {
        // Variables                                                                                                                


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public TetrahedralGmsh(string name)
            : base(name)
        {
            Reset();
        }
        public TetrahedralGmsh(TetrahedralGmsh tetrahedronGmsh)
            : base("tmpName")
        {
            CopyFrom(tetrahedronGmsh);
        }
        public TetrahedralGmsh(SerializationInfo info, StreamingContext context)
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
            //
            Transfinite = false;
        }
        public void CopyFrom(TetrahedralGmsh tetrahedronGmsh)
        {
            base.CopyFrom(tetrahedronGmsh);
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            // Using typeof() works also for null fields
        }
    }
}
