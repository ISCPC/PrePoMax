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
    internal class CalMomentLoad : CalculixKeyword
    {
        // Variables                                                                                                                
        private MomentLoad _load;
        private Dictionary<string, int[]> _referencePointsNodeIds;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalMomentLoad(MomentLoad load, Dictionary<string, int[]> referencePointsNodeIds)
        {
            _load = load;
            _referencePointsNodeIds = referencePointsNodeIds;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " + _load.Name);
            string amplitude = "";
            if (_load.AmplitudeName != Load.DefaultAmplitudeName) amplitude = ", Amplitude=" + _load.AmplitudeName;
            //
            sb.AppendFormat("*Cload{0}{1}", amplitude, Environment.NewLine);
            //
            return sb.ToString();
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            //
            int[] rpNodeIds = null;
            if (_load.RegionType == RegionTypeEnum.ReferencePointName) rpNodeIds = _referencePointsNodeIds[_load.RegionName];
            //
            List<int> directions = new List<int>();
            if (_load.M1 != 0) directions.Add(4);
            if (_load.M2 != 0) directions.Add(5);
            if (_load.M3 != 0) directions.Add(6);
            //
            foreach (var dir in directions)
            {
                if (_load.RegionType == CaeGlobals.RegionTypeEnum.NodeId)
                    sb.AppendFormat("{0}, {1}, {2}", _load.NodeId, dir, _load.GetDirection(dir - 4).ToCalculiX16String());
                else if (_load.RegionType == CaeGlobals.RegionTypeEnum.NodeSetName) // node set
                    sb.AppendFormat("{0}, {1}, {2}", _load.RegionName, dir, _load.GetDirection(dir - 4).ToCalculiX16String());
                else if (_load.RegionType == CaeGlobals.RegionTypeEnum.ReferencePointName) // reference point
                    sb.AppendFormat("{0}, {1}, {2}", rpNodeIds[1], dir - 3, _load.GetDirection(dir - 4).ToCalculiX16String());
                sb.AppendLine();
            }
            //
            return sb.ToString();
        }
    }
}
