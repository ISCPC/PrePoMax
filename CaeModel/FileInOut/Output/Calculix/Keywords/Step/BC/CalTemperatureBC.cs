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
    internal class CalTemperatureBC : CalculixKeyword
    {
        // Variables                                                                                                                
        private TemperatureBC _temperatureBC;
        private string _nodeSetNameOfSurface;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalTemperatureBC(TemperatureBC temperatureBC, string nodeSetNameOfSurface)
        {
            _temperatureBC = temperatureBC;
            _nodeSetNameOfSurface = nodeSetNameOfSurface;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " + _temperatureBC.Name);
            string amplitude = "";
            if (_temperatureBC.AmplitudeName != BoundaryCondition.DefaultAmplitudeName)
                amplitude = ", Amplitude=" + _temperatureBC.AmplitudeName;
            //
            sb.AppendFormat("*Boundary{0}{1}", amplitude, Environment.NewLine);
            //
            return sb.ToString();
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            // *Boundary
            // 6975, 11, 11, 100        node id, start DOF, end DOF, value
            // Node set
            string regionName;
            if (_temperatureBC.RegionType == RegionTypeEnum.NodeSetName)
            {
                regionName = _temperatureBC.RegionName;
            }
            // Surface
            else if (_temperatureBC.RegionType == RegionTypeEnum.SurfaceName)
            {
                if (_nodeSetNameOfSurface == null) throw new ArgumentException();
                regionName = _nodeSetNameOfSurface;
            }
            else throw new NotSupportedException();
            //
            sb.AppendFormat("{0}, 11, 11, {1}{2}", regionName, _temperatureBC.Temperature.ToCalculiX16String(),
                            Environment.NewLine);
            //
            return sb.ToString();
        }
    }
}
