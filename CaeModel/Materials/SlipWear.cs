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
    public class SlipWear : MaterialProperty
    {
        // Variables                                                                                                                
        private double _hardness;
        private double _wearCoefficient;


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


        // Methods                                                                                                                  
    }
}
