using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public class Density : MaterialProperty
    {
        // Variables                                                                                                                
        private static string _positive = "The value must be larger than 0.";
        private double _value;

        // Properties                                                                                                               
        public double Value 
        {
            get { return _value; }
            //set { if (value > 0) _value = value; else throw new CaeException(_positive); } 
            set { _value = value; }
        }

        // Constructors                                                                                                             
        public Density(double density)
        {
            Value = density;
        }

        // Methods                                                                                                                  
    }
}
