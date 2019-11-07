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
    internal class CalDLoad : CalculixKeyword
    {
        // Variables                                                                                                                
        private DLoad _load;
        private IDictionary<string, FeSurface> _surfaces;

        // Properties                                                                                                               
        public override object BaseItem { get { return _load; } }


        // Events                                                                                                                   


        // Constructor                                                                                                              
        public CalDLoad(IDictionary<string, FeSurface> surfaces, DLoad load)
        {
            _surfaces = surfaces;
            _load = load;
            _active = load.Active;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " +_load.Name);
            sb.AppendLine("*Dload");
            return sb.ToString();
        }

        public override string GetDataString()
        {
            //*Dload
            //_obremenitev_el_surf_S3, P3, 1
            //_obremenitev_el_surf_S4, P4, 1
            //_obremenitev_el_surf_S1, P1, 1
            //_obremenitev_el_surf_S2, P2, 1

            StringBuilder sb = new StringBuilder();
            FeSurface surface = _surfaces[_load.SurfaceName];
            foreach (var entry in surface.ElementFaces)
            {
                sb.AppendFormat("{0}, P{1}, {2}", entry.Value, entry.Key.ToString()[1], _load.Magnitude).AppendLine();
            }
            return sb.ToString();
        }
    }
}
