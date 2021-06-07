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
    public class PressureOverclosureDataPoint
    {
        // Variables                                                                                                                
        private double _pressure;
        private double _overclosure;


        // Properties                                                                                                               
        [DisplayName("Overclosure\n[?]")]
        [TypeConverter(typeof(CaeGlobals.StringLengthFromConverter))]
        public double Overclosure { get { return _overclosure; } set { _overclosure = value; } }
        //
        [DisplayName("Pressure\n[?]")]
        [TypeConverter(typeof(CaeGlobals.StringPressureFromConverter))]
        public double Pressure { get { return _pressure; } set { _pressure = value; } }


        // Constructors                                                                                                             
        public PressureOverclosureDataPoint()
        {
            _pressure = 0;
            _overclosure = 0;
        }
        public PressureOverclosureDataPoint(double pressure, double overclosure)
        {
            _pressure = pressure;
            _overclosure = overclosure;
        }
    }

}


