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
    internal class CalBodyFlux : CalculixKeyword
    {
        // Variables                                                                                                                
        private BodyFlux _flux;

        
        // Constructor                                                                                                              
        public CalBodyFlux(BodyFlux flux)
        {
            _flux = flux;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " + _flux.Name);
            string amplitude = "";
            if (_flux.AmplitudeName != Load.DefaultAmplitudeName) amplitude = ", Amplitude=" + _flux.AmplitudeName;
            //
            sb.AppendFormat("*Dflux{0}{1}", amplitude, Environment.NewLine);
            //
            return sb.ToString();
        }
        public override string GetDataString()
        {
            //*Dflux
            //ElementSet-1, BF, 10
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}, BF, {1}{2}", _flux.RegionName, _flux.Magnitude.ToCalculiX16String(), Environment.NewLine);
            return sb.ToString();
        }
    }
}
