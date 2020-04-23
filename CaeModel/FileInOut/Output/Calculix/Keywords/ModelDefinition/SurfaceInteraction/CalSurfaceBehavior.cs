using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeMesh;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    internal class CalSurfaceBehavior : CalculixKeyword
    {
        // Variables                                                                                                                
        private SurfaceBehavior _surfaceBehavior;


        // Constructor                                                                                                              
        public CalSurfaceBehavior(SurfaceBehavior surfaceBehavior)
        {
            _surfaceBehavior = surfaceBehavior;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            string pressureOverclosureType = ", Pressure-overclosure=" + _surfaceBehavior.PressureOverclosureType.ToString();
            return string.Format("*Surface behavior{0}{1}", pressureOverclosureType, Environment.NewLine);
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            if (_surfaceBehavior.PressureOverclosureType == PressureOverclosureEnum.Hard)
            { }
            else if (_surfaceBehavior.PressureOverclosureType == PressureOverclosureEnum.Linear)
            {
                sb.AppendFormat("{0}, {1}, {2}{3}", _surfaceBehavior.K, _surfaceBehavior.Sinf, _surfaceBehavior.C0,
                    Environment.NewLine);
            }
            else if (_surfaceBehavior.PressureOverclosureType == PressureOverclosureEnum.Exponential)
            {
                sb.AppendFormat("{0}, {1}{2}", _surfaceBehavior.C0, _surfaceBehavior.P0, Environment.NewLine);
            }
            else if (_surfaceBehavior.PressureOverclosureType == PressureOverclosureEnum.Tabular)
            {
                for (int i = 0; i < _surfaceBehavior.PressureOverclosure.Length; i++)
                {
                    sb.AppendFormat("{0}, {1}{2}", _surfaceBehavior.PressureOverclosure[i][0],
                                                   _surfaceBehavior.PressureOverclosure[i][1],
                                                   Environment.NewLine);
                }
            }
            else if (_surfaceBehavior.PressureOverclosureType == PressureOverclosureEnum.Tied)
            {
                sb.AppendFormat("{0}{1}", _surfaceBehavior.K, Environment.NewLine);
            }
            else throw new NotSupportedException();
            //
            return sb.ToString();
        }
    }
}
