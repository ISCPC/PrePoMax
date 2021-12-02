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
            string fileData = "";
            if (_definedTemperature.Type == DefinedTemperatureTypeEnum.FromFile)
                fileData = string.Format(", File={0}, Step={1}", _definedTemperature.FileName, _definedTemperature.StepNumber);
            //
            sb.AppendLine("** Name: " + _definedTemperature.Name);
            sb.AppendFormat("*Temperature{0}{1}", fileData, Environment.NewLine);
            return sb.ToString();
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            if (_definedTemperature.Type == DefinedTemperatureTypeEnum.ByValue)
            {
                sb.AppendFormat("{0}, {1}{2}", _regionName, _definedTemperature.Temperature.ToCalculiX16String(),
                                Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
