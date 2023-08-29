using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using System.Runtime.Serialization;

namespace CaeModel
{
    [Serializable]
    public class SlipWear : MaterialProperty, ISerializable
    {
        // Variables                                                                                                                
        private double _hardness;               //ISerializable
        private double _wearCoefficient;        //ISerializable


        // Properties                                                                                                               
        public double Hardness
        {
            get { return _hardness; }
            set
            {
                if (value <= 0) throw new CaeException(_positive);
                _hardness = value;
            }
        }
        public double WearCoefficient
        {
            get { return _wearCoefficient; }
            set
            {
                if (value <= 0) throw new CaeException(_positive);
                _wearCoefficient = value;
            }
        }


        // Constructors                                                                                                             
        public SlipWear(double hardness, double wearCoefficient)
        {
            // The constructor must wotk with H = 0; K = 0
            _hardness = hardness;
            _wearCoefficient = wearCoefficient;
        }
        public SlipWear(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_hardness":
                        _hardness = (double)entry.Value; break;
                    case "_wearCoefficient":
                        _wearCoefficient = (double)entry.Value; break;
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
            info.AddValue("_hardness", _hardness, typeof(double));
            info.AddValue("_wearCoefficient", _wearCoefficient, typeof(double));
        }
    }
}
