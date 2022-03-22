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
    internal class CalRadiationHeatTransfer : CalculixKeyword
    {
        // Variables                                                                                                                
        private RadiationHeatTransfer _radiationHeatTransfer;
        private FeSurface _surface;

        
        // Constructor                                                                                                              
        public CalRadiationHeatTransfer(RadiationHeatTransfer radiationHeatTransfer, FeSurface surface)
        {
            _surface = surface;
            _radiationHeatTransfer = radiationHeatTransfer;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " + _radiationHeatTransfer.Name);
            string amplitude = "";
            if (_radiationHeatTransfer.AmplitudeName != Load.DefaultAmplitudeName)
                amplitude = ", Amplitude=" + _radiationHeatTransfer.AmplitudeName;
            string cavity = "";
            if (_radiationHeatTransfer.CavityRadiation) cavity = "Cavity=" + _radiationHeatTransfer.CavityName;
            //
            sb.AppendFormat("*Radiate{0}{1}{2}", amplitude, cavity, Environment.NewLine);
            //
            return sb.ToString();
        }
        public override string GetDataString()
        {
            //*Radiate
            //_obremenitev_el_surf_S3, R3, -273.15, 0.5
            //_obremenitev_el_surf_S2, R2, -273.15, 0.5
            //
            //_obremenitev_el_surf_S3, R3CR, -273.15, 0.5
            //_obremenitev_el_surf_S2, R2CR, -273.15, 0.5
            StringBuilder sb = new StringBuilder();
            FeFaceName faceName;
            string faceKey = "";
            string cavityRadiation = "";
            if (_radiationHeatTransfer.CavityRadiation) cavityRadiation = "CR";
            //
            foreach (var entry in _surface.ElementFaces)
            {
                faceName = entry.Key;
                if (_radiationHeatTransfer.TwoD)
                {
                    if (faceName == FeFaceName.S1) faceKey = "RN";
                    else if (faceName == FeFaceName.S2) faceKey = "RP";
                    else if (faceName == FeFaceName.S3) faceKey = "R1";
                    else if (faceName == FeFaceName.S4) faceKey = "R2";
                    else if (faceName == FeFaceName.S5) faceKey = "R3";
                    else if (faceName == FeFaceName.S6) faceKey = "R4";
                }
                else
                {
                    faceKey = "R" + faceName.ToString()[1];
                }
                //
                sb.AppendFormat("{0}, {1}{2}, {3}, {4}{5}", entry.Value, faceKey, cavityRadiation,
                                _radiationHeatTransfer.SinkTemperature.ToCalculiX16String(),
                                _radiationHeatTransfer.Emissivity.ToCalculiX16String(), Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
