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
    internal class CalFilmHeatTransfer : CalculixKeyword
    {
        // Variables                                                                                                                
        private FilmHeatTransfer _filmHeatTransfer;
        private IDictionary<string, FeSurface> _surfaces;


        // Constructor                                                                                                              
        public CalFilmHeatTransfer(IDictionary<string, FeSurface> surfaces, FilmHeatTransfer filmHeatTransfer)
        {
            _surfaces = surfaces;
            _filmHeatTransfer = filmHeatTransfer;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " + _filmHeatTransfer.Name);
            sb.AppendLine("*Film");
            return sb.ToString();
        }
        public override string GetDataString()
        {
            // *Film
            // _obremenitev_el_surf_S3, F3, 20, 0.5
            // _obremenitev_el_surf_S4, F4, 20, 0.5
            // _obremenitev_el_surf_S1, F1, 20, 0.5
            // _obremenitev_el_surf_S2, F2, 20, 0.5
            StringBuilder sb = new StringBuilder();
            FeSurface surface = _surfaces[_filmHeatTransfer.SurfaceName];
            FeFaceName faceName;
            foreach (var entry in surface.ElementFaces)
            {
                faceName = entry.Key;
                sb.AppendFormat("{0}, F{1}, {2}, {3}{4}", entry.Value, faceName.ToString()[1],
                                _filmHeatTransfer.SinkTemperature.ToCalculiX16String(),
                                _filmHeatTransfer.FilmCoefficient.ToCalculiX16String(), Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
