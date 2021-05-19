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
    internal class CalRadiateFlux : CalculixKeyword
    {
        // Variables                                                                                                                
        private RadiateFlux _flux;
        private IDictionary<string, FeSurface> _surfaces;

        
        // Constructor                                                                                                              
        public CalRadiateFlux(IDictionary<string, FeSurface> surfaces, RadiateFlux flux)
        {
            _surfaces = surfaces;
            _flux = flux;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " +_flux.Name);
            string cavity = "";
            if (_flux.CavityRadiation) cavity = "Cavity=" + _flux.CavityName;
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
            FeSurface surface = _surfaces[_flux.SurfaceName];
            FeFaceName faceName;
            string cavityRadiation = "";
            if (_flux.CavityRadiation) cavityRadiation = "CR";
            //
            foreach (var entry in surface.ElementFaces)
            {
                faceName = entry.Key;
                sb.AppendFormat("{0}, R{1}{2}, {3}, {4}{5}", entry.Value, faceName.ToString()[1], cavityRadiation,
                                _flux.SinkTemperature, _flux.Emissivity, Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
