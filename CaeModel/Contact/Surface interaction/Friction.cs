using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public class Friction : SurfaceInteractionProperty
    {
        // Variables                                                                                                                
        private static string _positive = "The value must be larger than 0.";
        //
        private double _coefficient;
        private double _stickSlope;

        // Properties                                                                                                               
        public double Coefficient 
        { 
            get { return _coefficient; } 
            set { if (value > 0) _coefficient = value; else throw new CaeException(_positive); } 
        }
        public double StickSlope
        {
            get { return _stickSlope; }
            set
            {
                if (double.IsNaN(value) || value > 0) _stickSlope = value;
                else throw new CaeException(_positive);
            }
        }


        // Constructors                                                                                                             
        public Friction()
            :this(0.1)
        {
        }
        public Friction(double coefficient)
            : this(coefficient, double.NaN)
        {
        }
        public Friction(double coefficient, double stickSlope)
        {
            Coefficient = coefficient;
            StickSlope = stickSlope;
        }


        // Methods                                                                                                                  
    }
}
