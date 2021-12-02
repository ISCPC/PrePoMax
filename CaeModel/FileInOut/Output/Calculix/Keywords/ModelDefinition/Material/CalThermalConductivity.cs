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
    internal class CalThermalConductivity : CalculixKeyword
    {
        // Variables                                                                                                                
        private ThermalConductivity _thermalConductivity;
        private bool _temperatureDependent;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalThermalConductivity(ThermalConductivity thermalConductivity, bool temperatureDependent)
        {
            _thermalConductivity = thermalConductivity;
            _temperatureDependent = temperatureDependent;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            return string.Format("*Conductivity{0}", Environment.NewLine);
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            double[][] data = _thermalConductivity.ThermalConductivityTemp;
            for (int i = 0; i < data.Length; i++)
            {
                if (_temperatureDependent)
                    sb.AppendFormat("{0}, {1}{2}", data[i][0].ToCalculiX16String(), data[i][1].ToCalculiX16String(),
                                    Environment.NewLine);
                else
                {
                    sb.AppendFormat("{0}{1}", data[i][0].ToCalculiX16String(), Environment.NewLine);
                    break;
                }
            }
            return sb.ToString();
        }
    }
}
