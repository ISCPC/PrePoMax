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
    internal class CalCFlux : CalculixKeyword
    {
        // Variables                                                                                                                
        private CFlux _flux;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalCFlux(CFlux flux)
        {
            _flux = flux;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " + _flux.Name);
            string add = "";
            if (_flux.AddFlux) add = ", Add";
            sb.AppendLine("*Cflux" + add);
            return sb.ToString();
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            //
            sb.AppendFormat("{0}, 11, {1}{2}", _flux.RegionName, _flux.Magnitude.ToCalculiX16String(), Environment.NewLine);
            //
            return sb.ToString();
        }
    }
}
