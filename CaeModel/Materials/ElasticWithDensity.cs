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
    public class ElasticWithDensity : MaterialProperty, ISerializable
    {
        // Variables                                                                                                                
        private double _youngsModulus;          //ISerializable
        private double _poissonsRatio;          //ISerializable
        private double _density;                //ISerializable


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
        public double Density
        {
            get { return _density; }
            set { if (value > 0) _density = value; else throw new CaeException(_positive); }
        }


        // Constructors                                                                                                             
        public ElasticWithDensity(double youngsModulus, double poissonsRatio, double density)
        {
            // The constructor must wotk with E = 0
            _youngsModulus = youngsModulus;
            // Use the method to perform any checks necessary
            PoissonsRatio = poissonsRatio;
            // The constructor must wotk with rho = 0
            _density = density;
        }
        public ElasticWithDensity(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_youngsModulus":
                        _youngsModulus = (double)entry.Value; break;
                    case "_poissonsRatio":
                        _poissonsRatio = (double)entry.Value; break;
                    case "_density":
                        _density = (double)entry.Value; break;
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
            info.AddValue("_youngsModulus", _youngsModulus, typeof(double));
            info.AddValue("_poissonsRatio", _poissonsRatio, typeof(double));
            info.AddValue("_density", _density, typeof(double));
        }
    }
}
