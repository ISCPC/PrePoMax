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
    internal class CalDefinedTemperature : CalculixKeyword
    {
        // Variables                                                                                                                
        private string _regionName;
        private DefinedTemperature _definedTemperature;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalDefinedTemperature(FeModel model, DefinedTemperature definedTemperature)
        {
            _definedTemperature = definedTemperature;
            //
            _regionName = "";
            if (_definedTemperature.RegionType == CaeGlobals.RegionTypeEnum.NodeSetName)
                _regionName += _definedTemperature.RegionName;
            else if (_definedTemperature.RegionType == CaeGlobals.RegionTypeEnum.SurfaceName)
                _regionName += model.Mesh.Surfaces[_definedTemperature.RegionName].NodeSetName;
            else if (_definedTemperature.RegionType == CaeGlobals.RegionTypeEnum.Selection)
            { }
            else throw new NotSupportedException();
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " + _definedTemperature.Name);
            sb.AppendLine("*Temperature");
            return sb.ToString();
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}, {1}{2}", _regionName, _definedTemperature.Temperature, Environment.NewLine);
            return sb.ToString();
        }
    }
}
