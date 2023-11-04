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
    public class GapSection : Section, ISerializable
    {
        // Variables                                                                                                                
        private double _clearance;                          //ISerializable
        private double[] _direction;                        //ISerializable
        private double _springStiffness;                    //ISerializable
        private double _tensileForceAtNegativeInfinity;     //ISerializable


        // Properties                                                                                                               
        public double Clearance { get { return _clearance; } set { _clearance = value; } }
        public double[] Direction { get { return _direction; } set { _direction = value; } }


        // Constructors                                                                                                             
        public GapSection(string name, string elementSetName, double clearance, double[] direction, bool twoD)
            : base(name, null, elementSetName, RegionTypeEnum.ElementSetName, 1, twoD)
        {
            _clearance = clearance;
            _direction = direction;
        }
        public GapSection(SerializationInfo info, StreamingContext context)
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
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        public override void CheckEquations()
        {
            base.CheckEquations();
            //
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_clearance", _clearance, typeof(double));
            info.AddValue("_direction", _direction, typeof(double[]));
        }
    }
}
