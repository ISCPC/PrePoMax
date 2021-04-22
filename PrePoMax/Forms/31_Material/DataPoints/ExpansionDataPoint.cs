using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DynamicTypeDescriptor;

namespace CaeModel
{
    [Serializable]
    public class ExpansionDataPoint : TempDataPoint
    {
        // Variables                                                                                                                
        private double _thermalExpansion;


        // Properties                                                                                                               
        [DisplayName("Thermal Expansion\n[?]")]
        [TypeConverter(typeof(CaeGlobals.StringThermalExpansionFromConverter))]
        public double ThermalExpansion { get { return _thermalExpansion; } set { _thermalExpansion = value; } }


        // Constructors                                                                                                             
        public ExpansionDataPoint()
            :base(0)
        {
            _thermalExpansion = 0;
        }
        public ExpansionDataPoint(double thermalExpansion, double temperature)
            :base(temperature)
        {
            _thermalExpansion = thermalExpansion;
        }
    }
}


