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
    public class PlasticDataPoint : TempDataPoint
    {
        // Variables                                                                                                                
        private EquationContainer _strain;
        private EquationContainer _stress;


        // Properties                                                                                                               
        [DisplayName("Yield stress\n[?]")]
        [TypeConverter(typeof(EquationPressureFromConverter))]
        public EquationString StressEq { get { return _stress.Equation; } set { _stress.Equation = value; } }
        //
        [Browsable(false)]
        public EquationContainer Stress { get { return _stress; } set { _stress = value; } }
        //
        [DisplayName("Plastic strain\n[?]")]
        [TypeConverter(typeof(EquationDoubleConverter))]
        public EquationString StrainEq { get { return _strain.Equation; } set { _strain.Equation = value; } }
        //
        [Browsable(false)]
        public EquationContainer Strain { get { return _strain; } set { _strain = value; } }


        // Constructors                                                                                                             
        public PlasticDataPoint()
            :base(0)
        {
            _stress = new EquationContainer(typeof(StringPressureFromConverter), 0);
            _strain = new EquationContainer(typeof(StringDoubleConverter), 0);
        }
        public PlasticDataPoint(EquationContainer stress, EquationContainer strain, EquationContainer temperature)
            :base(temperature)
        {
            _stress = stress;
            _strain = strain;
        }
    }
}
