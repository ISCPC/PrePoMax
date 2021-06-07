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
    public class DensityDataPoint : TempDataPoint
    {
        // Variables                                                                                                                
        private double _density;


        // Properties                                                                                                               
        [DisplayName("Density\n[?]")]
        [TypeConverter(typeof(CaeGlobals.StringDensityFromConverter))]
        public double Density { get { return _density; } set { _density = value; } }


        // Constructors                                                                                                             
        public DensityDataPoint()
            :base(0)
        {
            _density = 0;
        }
        public DensityDataPoint(double density, double temperature)
            :base(temperature)
        {
            _density = density;
        }
    }
}


