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
    public class ElasticWithDensity : MaterialProperty
    {
        // Variables                                                                                                                
        private double _youngsModulus;
        private double _poissonsRatio;
        private double _density;


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


        // Methods                                                                                                                  
    }
}
