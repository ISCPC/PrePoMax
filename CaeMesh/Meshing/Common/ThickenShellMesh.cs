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
    public class ThickenShellMesh : MeshSetupItem, ISerializable
    {
        // Variables                                                                                                                
        private string[] _partNames;        //ISerializable
        private double _thickness;          //ISerializable
        private int _numberOfLayers;        //ISerializable
        private double _offset;             //ISerializable


        // Properties                                                                                                               
        public string[] PartNames { get { return _partNames; } set { _partNames = value; } }
        public double Thickness
        {
            get { return _thickness; }
            set
            {
                if (value < 0) throw new CaeException("Thickness must be larger than 0.");
                _thickness = value;
            }
        }
        public int NumberOfLayers
        {
            get { return _numberOfLayers; }
            set
            {
                if (value < 0) throw new CaeException("Number of layers must be larger than 0.");
                _numberOfLayers = value;
            }
        }
        public double Offset { get { return _offset; } set { _offset = value; } }


        // Constructors                                                                                                             
        public ThickenShellMesh(string name)
            : base(name)
        {
            Reset();
        }
        public ThickenShellMesh(ThickenShellMesh thickenShellMesh)
            : base("tmpName")
        {
            CopyFrom(thickenShellMesh);
        }
        public ThickenShellMesh(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_partNames":
                        _partNames = (string[])entry.Value; break;
                    case "_thickness":
                        _thickness = (double)entry.Value; break;
                    case "_numberOfLayers":
                        _numberOfLayers = (int)entry.Value; break;
                    case "_offset":
                        _offset = (double)entry.Value; break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        public override void Reset()
        {
            _partNames = null;
            _thickness = 1;
            _numberOfLayers = 1;
            _offset = 0;
        }
        public void CopyFrom(ThickenShellMesh thickenShellMesh)
        {
           base.CopyFrom(thickenShellMesh);
            //
            _partNames = thickenShellMesh.PartNames != null ? thickenShellMesh.PartNames.ToArray() : null;
            _thickness = thickenShellMesh.Thickness;
            _numberOfLayers = thickenShellMesh.NumberOfLayers;
            _offset = thickenShellMesh.Offset;
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            // Using typeof() works also for null fields
            info.AddValue("_partNames", _partNames, typeof(string[]));
            info.AddValue("_thickness", _thickness, typeof(double));
            info.AddValue("_numberOfLayers", _numberOfLayers, typeof(int));
            info.AddValue("_offset", _offset, typeof(double));
        }

    }
}
