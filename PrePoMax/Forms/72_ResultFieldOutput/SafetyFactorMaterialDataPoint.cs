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
    public class SafetyFactorElementSetDataPoint
    {
        // Variables                                                                                                                
        private string _elementSetName;
        private double _safetyLimit;


        // Properties                                                                                                               
        [DisplayName("Element Set")]
        public string ElementSetName { get { return _elementSetName; } set { _elementSetName = value; } }
        //
        [DisplayName("Safety Limit")]
        [TypeConverter(typeof(StringDoubleConverter))]
        public double SafetyLimit { get { return _safetyLimit; } set { _safetyLimit = value; } }


        // Constructors                                                                                                             
        public SafetyFactorElementSetDataPoint()
            : this(null, 0)
        {
        }
        public SafetyFactorElementSetDataPoint(string elementSetName, double safetyLimit)
        {
            _elementSetName = elementSetName;
            _safetyLimit = safetyLimit;
        }
    }
}
