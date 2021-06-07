using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DynamicTypeDescriptor;

namespace PrePoMax
{
    [Serializable]
    public class SpecificHeatDataPoint : TempDataPoint
    {
        // Variables                                                                                                                
        private double _specificHeat;


        // Properties                                                                                                               
        [DisplayName("Specific heat\n[?]")]
        [TypeConverter(typeof(CaeGlobals.StringSpecificHeatFromConverter))]
        public double SpecificHeat { get { return _specificHeat; } set { _specificHeat = value; } }


        // Constructors                                                                                                             
        public SpecificHeatDataPoint()
            :base(0)
        {
            _specificHeat = 0;
        }
        public SpecificHeatDataPoint(double density, double temperature)
            :base(temperature)
        {
            _specificHeat = density;
        }
    }
}


