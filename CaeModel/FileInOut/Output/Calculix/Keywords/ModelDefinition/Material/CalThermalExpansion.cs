﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeMesh;

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
            return string.Format("*Expansion{0}", Environment.NewLine);
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            double[][] data = _thermalExpansion.ThermalExpansionTemp;
            for (int i = 0; i < data.Length; i++)
            {
                if (_temperatureDependent)
                    sb.AppendFormat("{0}, {1}{2}", data[i][0], data[i][1], Environment.NewLine);
                else
                {
                    sb.AppendFormat("{0}{1}", data[i][0], Environment.NewLine);
                    break;
                }
            }
            return sb.ToString();
        }
    }
}