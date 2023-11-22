using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public class GapSectionData : SectionData, ISerializable
    {
        // Static Variables                                                                                                         
        public static readonly double InitialSpringStiffness = 1E12;
        public static readonly double InitialTensileForceAtNegativeInfinity = 1E-3;


        // Variables                                                                                                                
        private double _clearance;                          //ISerializable
        private double[] _direction;                        //ISerializable
        private double _springStiffness;                    //ISerializable
        private double _tensileForceAtNegativeInfinity;     //ISerializable


        // Properties                                                                                                               
        public double Clearance { get { return _clearance; } set { _clearance = value; } }
        public double[] Direction { get { return _direction; } set { _direction = value; } }
        public double SpringStiffness { get { return _springStiffness; } set { _springStiffness = value; } }
        public double TensileForceAtNegativeInfinity
        {
            get { return _tensileForceAtNegativeInfinity; }
            set { _tensileForceAtNegativeInfinity = value; }
        }


        // Constructors                                                                                                             
        public GapSectionData(string name, string elementSetName, double clearance, double[] direction,
                          double springStiffness, double tensileForceAtNegativeInfinity)
            : base(name, null, elementSetName, RegionTypeEnum.ElementSetName, 1)
        {
            _clearance = clearance;
            _direction = direction;
            _springStiffness = springStiffness;
            _tensileForceAtNegativeInfinity = tensileForceAtNegativeInfinity;
        }
        public GapSectionData(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_clearance":
                        _clearance = (double)entry.Value;
                        break;
                    case "_direction":
                        _direction = (double[])entry.Value;
                        break;
                    case "_springStiffness":
                        _springStiffness = (double)entry.Value;
                        break;
                    case "_tensileForceAtNegativeInfinity":
                        _tensileForceAtNegativeInfinity = (double)entry.Value;
                        break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
       

        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_clearance", _clearance, typeof(double));
            info.AddValue("_direction", _direction, typeof(double[]));
            info.AddValue("_springStiffness", _springStiffness, typeof(double));
            info.AddValue("_tensileForceAtNegativeInfinity", _tensileForceAtNegativeInfinity, typeof(double));
        }
    }
}
