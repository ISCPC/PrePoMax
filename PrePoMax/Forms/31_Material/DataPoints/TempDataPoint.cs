using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace PrePoMax
{
    [Serializable]
    public class TempDataPoint
    {
        // Variables                                                                                                                
        private double _temperature;


        // Properties                                                                                                               
        [DisplayName("Temperature\n[?]")]
        [TypeConverter(typeof(CaeGlobals.StringTemperatureFromConverter))]
        public double Temperature { get { return _temperature; } set { _temperature = value; } }


        // Constructors                                                                                                             
        public TempDataPoint()
        {
            _temperature = 0;
        }
        public TempDataPoint(double temperature)
        {
            _temperature = temperature;
        }
    }
}
