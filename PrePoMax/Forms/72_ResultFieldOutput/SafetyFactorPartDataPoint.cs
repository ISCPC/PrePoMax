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
    public class SafetyFactorPartDataPoint
    {
        // Variables                                                                                                                
        private string _partName;
        private double _safetyLimit;


        // Properties                                                                                                               
        [DisplayName("Part")]
        public string PartName { get { return _partName; } set { _partName = value; } }
        //
        [DisplayName("Safety Limit")]
        [TypeConverter(typeof(StringDoubleConverter))]
        public double SafetyLimit { get { return _safetyLimit; } set { _safetyLimit = value; } }


        // Constructors                                                                                                             
        public SafetyFactorPartDataPoint()
            : this(null, 0)
        {
        }
        public SafetyFactorPartDataPoint(string partName, double safetyLimit)
        {
            _partName = partName;
            _safetyLimit = safetyLimit;
        }
    }
}
