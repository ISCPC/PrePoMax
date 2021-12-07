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
    internal class CalShellEdgeLoad : CalculixKeyword
    {
        // Variables                                                                                                                
        private ShellEdgeLoad _load;
        private IDictionary<string, FeSurface> _surfaces;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalShellEdgeLoad(IDictionary<string, FeSurface> surfaces, ShellEdgeLoad load)
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
            //*Dload
            //_obremenitev_el_surf_S3, EDNOR1, 1
            //_obremenitev_el_surf_S4, EDNOR2, 1
            StringBuilder sb = new StringBuilder();
            FeSurface surface = _surfaces[_load.SurfaceName];
            FeFaceName faceName;
            int faceId;
            double magnitude;
            //
            foreach (var entry in surface.ElementFaces)
            {
                faceName = entry.Key;
                if (faceName == FeFaceName.S1 || faceName == FeFaceName.S2) throw new NotSupportedException();
                faceId = int.Parse(faceName.ToString().Substring(1)) - 2;
                magnitude = _load.Magnitude;
                //
                sb.AppendFormat("{0}, EDNOR{1}, {2}", entry.Value, faceId, magnitude.ToCalculiX16String()).AppendLine();
            }
            return sb.ToString();
        }
    }
}
