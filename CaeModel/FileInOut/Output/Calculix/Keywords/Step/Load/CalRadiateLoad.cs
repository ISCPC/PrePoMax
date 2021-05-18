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
    internal class CalRadiateLoad : CalculixKeyword
    {
        // Variables                                                                                                                
        private RadiateLoad _load;
        private IDictionary<string, FeSurface> _surfaces;

        
        // Constructor                                                                                                              
        public CalRadiateLoad(IDictionary<string, FeSurface> surfaces, RadiateLoad load)
        {
            _surfaces = surfaces;
            _load = load;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " +_load.Name);
            string cavity = "";
            if (_load.CavityRadiation) cavity = "Cavity=" + _load.CavityName;
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
            FeSurface surface = _surfaces[_load.SurfaceName];
            FeFaceName faceName;
            string cavityRadiation = "";
            if (_load.CavityRadiation) cavityRadiation = "CR";
            //
            foreach (var entry in surface.ElementFaces)
            {
                faceName = entry.Key;
                sb.AppendFormat("{0}, R{1}{2}, {3}, {4}{5}", entry.Value, faceName.ToString()[1], cavityRadiation,
                                _load.SinkTemperature, _load.Emissivity, Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
