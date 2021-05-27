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
    internal class CalDFlux : CalculixKeyword
    {
        // Variables                                                                                                                
        private DFlux _flux;
        private IDictionary<string, FeSurface> _surfaces;

        
        // Constructor                                                                                                              
        public CalDFlux(IDictionary<string, FeSurface> surfaces, DFlux flux)
        {
            _surfaces = surfaces;
            _flux = flux;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " + _flux.Name);
            sb.AppendLine("*Dflux");
            return sb.ToString();
        }
        public override string GetDataString()
        {
            //*Dflux
            //Surface-1, S, 10
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}, S, {1}{2}", _flux.SurfaceName, _flux.Magnitude, Environment.NewLine);
            return sb.ToString();
        }
    }
}
