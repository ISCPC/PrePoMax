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
    public class SpecificHeatDataPoint : TempDataPoint
    {
        // Variables                                                                                                                
        private EquationContainer _specificHeat;


        // Properties                                                                                                               
        [DisplayName("Specific heat\n[?]")]
        [TypeConverter(typeof(EquationSpecificHeatFromConverter))]
        public EquationString SpecificHeatEq { get { return _specificHeat.Equation; } set { _specificHeat.Equation = value; } }
        //
        [Browsable(false)]
        public EquationContainer SpecificHeat { get { return _specificHeat; } set { _specificHeat = value; } }



        // Constructors                                                                                                             
        public SpecificHeatDataPoint()
            :base(0)
        {
            _specificHeat = new EquationContainer(typeof(StringSpecificHeatFromConverter), 0);
        }
        public SpecificHeatDataPoint(EquationContainer density, EquationContainer temperature)
            :base(temperature)
        {
            _specificHeat = density;
        }


        // Methods                                                                                                                  
        
    }
}


