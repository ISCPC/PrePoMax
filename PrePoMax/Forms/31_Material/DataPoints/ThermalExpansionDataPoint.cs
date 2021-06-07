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
    public class ThermalExpansionDataPoint : TempDataPoint
    {
        // Variables                                                                                                                
        private double _thermalExpansion;


        // Properties                                                                                                               
        [DisplayName("Thermal expansion\n[?]")]
        [TypeConverter(typeof(CaeGlobals.StringThermalExpansionFromConverter))]
        public double ThermalExpansion { get { return _thermalExpansion; } set { _thermalExpansion = value; } }


        // Constructors                                                                                                             
        public ThermalExpansionDataPoint()
            :base(0)
        {
            _thermalExpansion = 0;
        }
        public ThermalExpansionDataPoint(double thermalExpansion, double temperature)
            :base(temperature)
        {
            _thermalExpansion = thermalExpansion;
        }
    }
}


