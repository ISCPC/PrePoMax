using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public class Elastic : MaterialProperty
    {
        // Variables                                                                                                                
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
            _youngsModulus = youngsModulus;
            PoissonsRatio = poissonsRatio;
        }

        // Methods                                                                                                                  
    }
}
