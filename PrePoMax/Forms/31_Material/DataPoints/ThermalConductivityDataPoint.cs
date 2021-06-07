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
    public class ThermalConductivityDataPoint : TempDataPoint
    {
        // Variables                                                                                                                
        private double _thermalConductivity;


        // Properties                                                                                                               
        [DisplayName("Thermal conductivity\n[?]")]
        [TypeConverter(typeof(CaeGlobals.StringThermalConductivityFromConverter))]
        public double ThermalConductivity { get { return _thermalConductivity; } set { _thermalConductivity = value; } }


        // Constructors                                                                                                             
        public ThermalConductivityDataPoint()
            :base(0)
        {
            _thermalConductivity = 0;
        }
        public ThermalConductivityDataPoint(double thermalConductivity, double temperature)
            :base(temperature)
        {
            _thermalConductivity = thermalConductivity;
        }
    }
}


