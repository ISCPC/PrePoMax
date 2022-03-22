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
        private FeSurface _surface;


        // Constructor                                                                                                              
        public CalFilmHeatTransfer(FilmHeatTransfer filmHeatTransfer, FeSurface surface)
        {
            _filmHeatTransfer = filmHeatTransfer;
            _surface = surface;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " + _filmHeatTransfer.Name);
            string amplitude = "";
            if (_filmHeatTransfer.AmplitudeName != Load.DefaultAmplitudeName)
                amplitude = ", Amplitude=" + _filmHeatTransfer.AmplitudeName;
            //
            sb.AppendFormat("*Film{0}{1}", amplitude, Environment.NewLine);
            //
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
            FeFaceName faceName;
            string faceKey = "";
            foreach (var entry in _surface.ElementFaces)
            {
                faceName = entry.Key;
                if (_filmHeatTransfer.TwoD)
                {
                    if (faceName == FeFaceName.S1) faceKey = "FN";
                    else if (faceName == FeFaceName.S2) faceKey = "FP";
                    else if (faceName == FeFaceName.S3) faceKey = "F1";
                    else if (faceName == FeFaceName.S4) faceKey = "F2";
                    else if (faceName == FeFaceName.S5) faceKey = "F3";
                    else if (faceName == FeFaceName.S6) faceKey = "F4";
                }
                else
                {
                    faceKey = "F" + faceName.ToString()[1];
                }
                //
                sb.AppendFormat("{0}, {1}, {2}, {3}{4}", entry.Value, faceKey,
                                _filmHeatTransfer.SinkTemperature.ToCalculiX16String(),
                                _filmHeatTransfer.FilmCoefficient.ToCalculiX16String(), Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
