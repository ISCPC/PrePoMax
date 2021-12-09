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
            int faceId;
            double magnitude;
            int delta = _load.TwoD ? 2 : 0;
            foreach (var entry in surface.ElementFaces)
            {
                faceName = entry.Key;
                if (_load.TwoD && (faceName == FeFaceName.S1 || faceName == FeFaceName.S2)) throw new NotSupportedException();
                faceId = int.Parse(faceName.ToString().Substring(1)) - delta;
                magnitude = _load.Magnitude;
                if (surface.SurfaceFaceTypes == FeSurfaceFaceTypes.ShellFaces && faceName == FeFaceName.S2) magnitude *= -1;
                //
                sb.AppendFormat("{0}, P{1}, {2}", entry.Value, faceId, magnitude.ToCalculiX16String()).AppendLine();
            }
            return sb.ToString();
        }
    }
}
