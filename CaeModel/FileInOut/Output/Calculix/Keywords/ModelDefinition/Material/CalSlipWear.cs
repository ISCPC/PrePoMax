using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeMesh;
using CaeGlobals;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    internal class CalSlipWear : CalculixKeyword
    {
        // Variables                                                                                                                
        private SlipWear _slipWear;
        private bool _temperatureDependent;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalSlipWear(SlipWear slipWear, bool temperatureDependent)
        {
            _slipWear = slipWear;
            _temperatureDependent = temperatureDependent;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            return string.Format("*Slip wear{0}", Environment.NewLine);
        }
        public override string GetDataString()
        {
            return string.Format("{0}, {1}{2}", _slipWear.Hardness, _slipWear.WearCoefficient, Environment.NewLine);
        }
    }
}
