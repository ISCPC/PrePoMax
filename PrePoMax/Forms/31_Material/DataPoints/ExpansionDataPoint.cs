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
        private double _expansion;


        // Properties                                                                                                               
        [DisplayName("Expansion\n[?]")]
        [TypeConverter(typeof(CaeGlobals.StringExpansionFromConverter))]
        public double Expansion { get { return _expansion; } set { _expansion = value; } }


        // Constructors                                                                                                             
        public ExpansionDataPoint()
            :base(0)
        {
            _expansion = 0;
        }
        public ExpansionDataPoint(double expansion, double temperature)
            :base(temperature)
        {
            _expansion = expansion;
        }
    }
}


