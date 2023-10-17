using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using DynamicTypeDescriptor;

namespace CaeMesh
{
    [Serializable]
    public class FeMeshRefinement : MeshSetupItem, ISerializable
    {
        // Variables                                                                                                                
        private double _meshSize;           //ISerializable


        // Properties                                                                                                               
        public double MeshSize 
        {
            get { return _meshSize; } 
            set
            {
                _meshSize = value;
                if (_meshSize < 1E-10) _meshSize = 1E-10;
            } 
        }


        // Constructors                                                                                                             
        public FeMeshRefinement(string name)
            : base(name)
        {
            Reset();
        }
        public FeMeshRefinement(FeMeshRefinement meshRefinement)
            : base("tmpName")
        {
            CopyFrom(meshRefinement);
        }
        public FeMeshRefinement(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_meshSize":
                        _meshSize = (double)entry.Value; break;
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
            _meshSize = 1;
        }
        public void CopyFrom(FeMeshRefinement meshRefinement)
        {
            base.CopyFrom(meshRefinement);
            //
            _meshSize = meshRefinement.MeshSize;
        }
        public FeMeshRefinement DeepCopy()
        {
            return new FeMeshRefinement(this);
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            // Using typeof() works also for null fields
            info.AddValue("_meshSize", _meshSize, typeof(double));
        }
    }
}
