using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using CaeMesh.Meshing;
using DynamicTypeDescriptor;

namespace CaeMesh
{
    [Serializable]
    public class ExtrudeMesh : MeshSetupItem, ISerializable
    {
        // Variables                                                                                                                
        private GmshAlgorithmMesh2DEnum _algorithmMesh2D;               // ISerializable
        private GmshAlgorithmRecombineEnum _algorithmRecombine;         // ISerializable
        private int _numberOfLayers;                                    // ISerializable


        // Properties                                                                                                               
        public GmshAlgorithmMesh2DEnum AlgorithmMesh2D { get { return _algorithmMesh2D; } set { _algorithmMesh2D = value; } }
        public GmshAlgorithmRecombineEnum AlgorithmRecombine
        {
            get { return _algorithmRecombine; }
            set { _algorithmRecombine = value; }
        }
        public int NumberOfLayers
        {
            get { return _numberOfLayers; }
            set
            {
                _numberOfLayers = value;
                if (_numberOfLayers < 1) _numberOfLayers = 1;
            }
        }


        // Constructors                                                                                                             
        public ExtrudeMesh(string name)
            : base(name)
        {
            Reset();
        }
        public ExtrudeMesh(ExtrudeMesh extrudeMesh)
            : base("tmpName")
        {
            CopyFrom(extrudeMesh);
        }
        public ExtrudeMesh(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_algorithmMesh2D":
                        _algorithmMesh2D = (GmshAlgorithmMesh2DEnum)entry.Value; break;
                    case "_algorithmRecombine":
                        _algorithmRecombine = (GmshAlgorithmRecombineEnum)entry.Value; break;
                    case "_numberOfLayers":
                        _numberOfLayers = (int)entry.Value; break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        public void Reset()
        {
            _algorithmMesh2D = GmshAlgorithmMesh2DEnum.Automatic;
            _algorithmRecombine = GmshAlgorithmRecombineEnum.None;
            _numberOfLayers = 1;
        }
        public void CopyFrom(ExtrudeMesh extrudeMesh)
        {
            base.CopyFrom(extrudeMesh);
            //
            _algorithmMesh2D = extrudeMesh._algorithmMesh2D;
            _algorithmRecombine = extrudeMesh._algorithmRecombine;
            _numberOfLayers = extrudeMesh._numberOfLayers;
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            // Using typeof() works also for null fields
            info.AddValue("_algorithmMesh2D", _algorithmMesh2D, typeof(GmshAlgorithmMesh2DEnum));
            info.AddValue("_algorithmRecombine", _algorithmRecombine, typeof(GmshAlgorithmRecombineEnum));
            info.AddValue("_numberOfLayers", _numberOfLayers, typeof(int));
        }
    }
}
