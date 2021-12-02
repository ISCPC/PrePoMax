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
        private IDictionary<string, FeSurface> _surfaces;

        
        // Constructor                                                                                                              
        public CalRadiationHeatTransfer(IDictionary<string, FeSurface> surfaces, RadiationHeatTransfer radiationHeatTransfer)
        {
            _surfaces = surfaces;
            _radiationHeatTransfer = radiationHeatTransfer;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " + _radiationHeatTransfer.Name);
            string cavity = "";
            if (_radiationHeatTransfer.CavityRadiation) cavity = "Cavity=" + _radiationHeatTransfer.CavityName;
            sb.AppendLine("*Radiate" + cavity);
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
            FeSurface surface = _surfaces[_radiationHeatTransfer.SurfaceName];
            FeFaceName faceName;
            string cavityRadiation = "";
            if (_radiationHeatTransfer.CavityRadiation) cavityRadiation = "CR";
            //
            foreach (var entry in surface.ElementFaces)
            {
                faceName = entry.Key;
                sb.AppendFormat("{0}, R{1}{2}, {3}, {4}{5}", entry.Value, faceName.ToString()[1], cavityRadiation,
                                _radiationHeatTransfer.SinkTemperature.ToCalculiX16String(),
                                _radiationHeatTransfer.Emissivity.ToCalculiX16String(), Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
