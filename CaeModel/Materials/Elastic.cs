using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeModel
{
    [Serializable]
    public class Elastic : MaterialProperty
    {
        // Variables                                                                                                                
        public double YoungsModulus { get; set; }
        public double PoissonsRatio { get; set; }


        // Constructors                                                                                                             
        public Elastic(double youngsModulus, double poissonsRatio)
        {
            YoungsModulus = youngsModulus;
            PoissonsRatio = poissonsRatio;
        }

        // Methods                                                                                                                  
    }
}
