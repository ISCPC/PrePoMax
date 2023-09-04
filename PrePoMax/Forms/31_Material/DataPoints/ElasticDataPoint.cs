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
    public class ElasticDataPoint : TempDataPoint
    {
        // Variables                                                                                                                
        private EquationContainer _youngsModulus;
        private EquationContainer _poissonsRatio;


        // Properties                                                                                                               
        [DisplayName("Youngs modulus\n[?]")]
        [TypeConverter(typeof(EquationPressureFromConverter))]
        public EquationString YoungsModulusEq { get { return _youngsModulus.Equation; } set { _youngsModulus.Equation = value; } }
        //
        [Browsable(false)]
        public EquationContainer YoungsModulus { get { return _youngsModulus; } set { _youngsModulus = value; } }
        //
        [DisplayName("Poissons ratio\n[?]")]
        [TypeConverter(typeof(EquationDoubleConverter))]
        public EquationString PoissonsRatioEq { get { return _poissonsRatio.Equation; } set { _poissonsRatio.Equation = value; } }
        //
        [Browsable(false)]
        public EquationContainer PoissonsRatio { get { return _poissonsRatio; } set { _poissonsRatio = value; } }


        // Constructors                                                                                                             
        public ElasticDataPoint()
            :base(0)
        {
            _youngsModulus = new EquationContainer(typeof(StringPressureFromConverter), 0);
            _poissonsRatio = new EquationContainer(typeof(StringDoubleConverter), 0);
        }
        public ElasticDataPoint(EquationContainer youngsModulus, EquationContainer poissonsRatio, EquationContainer temperature)
            :base(temperature)
        {
            _youngsModulus = youngsModulus;
            _poissonsRatio = poissonsRatio;
        }
    }
}
