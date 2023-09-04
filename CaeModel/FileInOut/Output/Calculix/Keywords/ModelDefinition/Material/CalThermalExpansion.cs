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
    internal class CalThermalExpansion : CalculixKeyword
    {
        // Variables                                                                                                                
        private ThermalExpansion _thermalExpansion;
        private bool _temperatureDependent;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalThermalExpansion(ThermalExpansion thermalExpansion, bool temperatureDependent)
        {
            _thermalExpansion = thermalExpansion;
            _temperatureDependent = temperatureDependent;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            string zeroTemperature = "";
            if (_thermalExpansion.ZeroTemperature.Value != 0)
                zeroTemperature = ", Zero=" + _thermalExpansion.ZeroTemperature.Value.ToCalculiX16String();
            return string.Format("*Expansion{0}{1}", zeroTemperature, Environment.NewLine);
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            EquationContainer[][] data = _thermalExpansion.ThermalExpansionTemp;
            for (int i = 0; i < data.Length; i++)
            {
                if (_temperatureDependent)
                    sb.AppendFormat("{0}, {1}{2}", data[i][0].Value.ToCalculiX16String(), data[i][1].Value.ToCalculiX16String(),
                                    Environment.NewLine);
                else
                {
                    sb.AppendFormat("{0}{1}", data[i][0].Value.ToCalculiX16String(), Environment.NewLine);
                    break;
                }
            }
            return sb.ToString();
        }
    }
}
