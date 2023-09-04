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
    public class DensityDataPoint : TempDataPoint
    {
        // Variables                                                                                                                
        private EquationContainer _density;


        // Properties                                                                                                               
        [DisplayName("Density\n[?]")]
        [TypeConverter(typeof(EquationDensityFromConverter))]
        public EquationString DensityEq { get { return _density.Equation; } set { _density.Equation = value; } }
        //
        [Browsable(false)]
        public EquationContainer Density { get { return _density; } set { _density = value; } }


        // Constructors                                                                                                             
        public DensityDataPoint()
            :base(0)
        {
            _density = new EquationContainer(typeof(StringDensityFromConverter), 0);
        }
        public DensityDataPoint(EquationContainer density, EquationContainer temperature)
            :base(temperature)
        {
            _density = density;
        }
    }
}


