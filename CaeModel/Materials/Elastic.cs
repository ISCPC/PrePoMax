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
    public class Elastic : MaterialProperty, ISerializable
    {
        // Variables                                                                                                                
        [NonSerialized]
        private static string _positive = "The value must be larger than 0.";
        //
        private double _youngsModulus;
        private double _poissonsRatio;

        // Properties                                                                                                               
        public double YoungsModulus 
        { 
            get { return _youngsModulus; }
            set { if (value > 0) _youngsModulus = value; else throw new CaeException(_positive); } 
        }
        public double PoissonsRatio
        {
            get { return _poissonsRatio; }
            set { _poissonsRatio = value; }
        }


        // Constructors                                                                                                             
        public Elastic(double youngsModulus, double poissonsRatio)
        {
            // The constructor must wotk with E = 0
            _youngsModulus = youngsModulus;
            // Use the method to perform any checks necessary
            PoissonsRatio = poissonsRatio;
        }
        public Elastic(SerializationInfo info, StreamingContext context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_youngsModulus":
                    case "<YoungsModulus>k__BackingField":
                        _youngsModulus = (double)entry.Value; break;
                    case "_poissonsRatio":
                    case "<PoissonsRatio>k__BackingField":
                        _poissonsRatio = (double)entry.Value; break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }


        // Methods                                                                                                                  

        // ISerialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // using typeof() works also for null fields
            info.AddValue("_youngsModulus", _youngsModulus, typeof(double));
            info.AddValue("_poissonsRatio", _poissonsRatio, typeof(double));
        }
    }
}
