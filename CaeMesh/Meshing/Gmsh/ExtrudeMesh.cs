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
    public class ExtrudeMesh : GmshSetupItem, ISerializable
    {
        // Variables                                                                                                                
        private double[] _direction;                                    // ISerializable
        private double[] _extrudeCenter;                                // ISerializable


        // Properties                                                                                                               
        public double[] Direction { get { return _direction; } set { _direction = value; } }
        public double[] ExtrudeCenter { get { return _extrudeCenter; } set { _extrudeCenter = value; } }


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
                    case "_direction":
                        _direction = (double[])entry.Value; break;
                    case "_extrudeCenter":
                        _extrudeCenter = (double[])entry.Value; break;
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
            _direction = null;
            _extrudeCenter = null;
        }
        public void CopyFrom(ExtrudeMesh extrudeMesh)
        {
            base.CopyFrom(extrudeMesh);
            //
            _direction = extrudeMesh._direction.ToArray();
            _extrudeCenter = extrudeMesh._extrudeCenter.ToArray();
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            // Using typeof() works also for null fields
            info.AddValue("_direction", _direction, typeof(double[]));
            info.AddValue("_extrudeCenter", _extrudeCenter, typeof(double[]));
        }
    }
}
