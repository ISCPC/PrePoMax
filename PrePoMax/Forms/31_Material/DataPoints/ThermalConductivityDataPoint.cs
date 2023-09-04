using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DynamicTypeDescriptor;
using CaeGlobals;
using CaeModel;

namespace PrePoMax
{
    [Serializable]
    public class ThermalConductivityDataPoint : TempDataPoint
    {
        // Variables                                                                                                                
        private EquationContainer _thermalConductivity;


        // Properties                                                                                                               
        [DisplayName("Thermal conductivity\n[?]")]
        [TypeConverter(typeof(EquationThermalConductivityFromConverter))]
        public EquationString ThermalConductivityEq
        {
            get { return _thermalConductivity.Equation; }
            set { _thermalConductivity.Equation = value; }
        }
        //
        [Browsable(false)]
        public EquationContainer ThermalConductivity { get { return _thermalConductivity; } set { _thermalConductivity = value; } }


        // Constructors                                                                                                             
        public ThermalConductivityDataPoint()
            : base(0)
        {
            _thermalConductivity = new EquationContainer(typeof(StringThermalConductivityFromConverter), 0);
        }
        public ThermalConductivityDataPoint(EquationContainer thermalConductivity, EquationContainer temperature)
            : base(temperature)
        {
            _thermalConductivity = thermalConductivity;
        }


        // Methods                                                                                                                  
    }
}


