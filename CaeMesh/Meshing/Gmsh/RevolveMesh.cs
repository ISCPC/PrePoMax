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
    public class RevolveMesh : GmshSetupItem, ISerializable
    {
        // Variables                                                                                                                
        private double[] _axisCenter;                                   // ISerializable
        private double[] _axisDirection;                                // ISerializable
        private double _angleDeg;                                       // ISerializable
        private double _middleR;                                        // ISerializable


        // Properties                                                                                                               
        public double[] AxisCenter { get { return _axisCenter; } set { _axisCenter = value; } }
        public double[] AxisDirection { get { return _axisDirection; } set { _axisDirection = value; } }
        public double AngleDeg { get { return _angleDeg; } set { _angleDeg = value; } }
        public double MiddleR { get { return _middleR; } set { _middleR = value; } }


        // Constructors                                                                                                             
        public RevolveMesh(string name)
            : base(name)
        {
            Reset();
        }
        public RevolveMesh(RevolveMesh revolveMesh)
            : base("tmpName")
        {
            CopyFrom(revolveMesh);
        }
        public RevolveMesh(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_axisCenter":
                        _axisCenter = (double[])entry.Value; break;
                    case "_axisDirection":
                        _axisDirection = (double[])entry.Value; break;
                    case "_angleDeg":
                        _angleDeg = (double)entry.Value; break;
                    case "_middleR":
                        _middleR = (double)entry.Value; break;
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
            _axisCenter = null;
            _axisDirection = null;
            _angleDeg = -1;
            _middleR = -1;
        }
        public void CopyFrom(RevolveMesh revolveMesh)
        {
            base.CopyFrom(revolveMesh);
            //
            _axisCenter = revolveMesh._axisCenter.ToArray();
            _axisDirection = revolveMesh._axisDirection.ToArray();
            _angleDeg = revolveMesh._angleDeg;
            _middleR = revolveMesh._middleR;
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            // Using typeof() works also for null fields
            info.AddValue("_axisCenter", _axisCenter, typeof(double[]));
            info.AddValue("_axisDirection", _axisDirection, typeof(double[]));
            info.AddValue("_angleDeg", _angleDeg, typeof(double));
            info.AddValue("_middleR", _middleR, typeof(double));
        }
    }
}
