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
    internal class CalDLoad : CalculixKeyword
    {
        // Variables                                                                                                                
        private DLoad _load;
        private IDictionary<string, FeSurface> _surfaces;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalDLoad(IDictionary<string, FeSurface> surfaces, DLoad load)
        {
            _surfaces = surfaces;
            _load = load;
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
            // *Dload
            // _obremenitev_el_surf_S3, P3, 1
            // _obremenitev_el_surf_S4, P4, 1
            // _obremenitev_el_surf_S1, P1, 1
            // _obremenitev_el_surf_S2, P2, 1
            StringBuilder sb = new StringBuilder();
            FeSurface surface = _surfaces[_load.SurfaceName];
            FeFaceName faceName;
            double magnitude;
            foreach (var entry in surface.ElementFaces)
            {
                faceName = entry.Key;
                if (surface.SurfaceFaceTypes == FeSurfaceFaceTypes.ShellFaces && faceName == FeFaceName.S2)
                    magnitude = -_load.Magnitude;
                else
                    magnitude = _load.Magnitude;
                //
                sb.AppendFormat("{0}, P{1}, {2}", entry.Value, faceName.ToString()[1], magnitude.ToCalculiX16String()).AppendLine();
            }
            return sb.ToString();
        }
    }
}
