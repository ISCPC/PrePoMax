using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DynamicTypeDescriptor;
using CaeGlobals;

namespace PrePoMax
{
    [Serializable]
    public class ThermalExpansionDataPoint : TempDataPoint
    {
        // Variables                                                                                                                
        private EquationContainer _thermalExpansion;


        // Properties                                                                                                               
        [DisplayName("Thermal expansion\n[?]")]
        [TypeConverter(typeof(EquationThermalExpansionFromConverter))]
        public EquationString ThermalExpansionEq
        {
            get { return _thermalExpansion.Equation; }
            set { _thermalExpansion.Equation = value; }
        }
        //
        [Browsable(false)]
        public EquationContainer ThermalExpansion { get { return _thermalExpansion; } set { _thermalExpansion = value; } }


        // Constructors                                                                                                             
        public ThermalExpansionDataPoint()
            :base(0)
        {
            _thermalExpansion = new EquationContainer(typeof(StringThermalExpansionFromConverter), 0);
        }
        public ThermalExpansionDataPoint(EquationContainer thermalExpansion, EquationContainer temperature)
            :base(temperature)
        {
            _thermalExpansion = thermalExpansion;
        }


        // Methods                                                                                                                  
       
    }
}


