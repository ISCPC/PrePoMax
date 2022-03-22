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
    internal class CalSTLoad : CalculixKeyword
    {
        // Variables                                                                                                                
        private STLoad _load;
        private CLoad[] _cLoads;
        private Dictionary<string, int[]> _referencePointsNodeIds;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalSTLoad(FeModel model, STLoad load, Dictionary<string, int[]> referencePointsNodeIds)
        {
            _cLoads = model.GetNodalLoadsFromSurfaceTraction(load);
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
            if (_cLoads != null)
            {
                foreach (var cLoad in _cLoads)
                {
                    int[] rpNodeIds = null;
                    if (cLoad.RegionType == RegionTypeEnum.ReferencePointName)
                        rpNodeIds = _referencePointsNodeIds[cLoad.RegionName];
                    //
                    List<int> directions = new List<int>();
                    if (cLoad.F1 != 0) directions.Add(1);
                    if (cLoad.F2 != 0) directions.Add(2);
                    if (cLoad.F3 != 0) directions.Add(3);
                    //
                    foreach (var dir in directions)
                    {
                        if (cLoad.RegionType == RegionTypeEnum.NodeId)
                            sb.AppendFormat("{0}, {1}, {2}", cLoad.NodeId, dir,
                                            cLoad.GetDirection(dir - 1).ToCalculiX16String());
                        else if (cLoad.RegionType == RegionTypeEnum.NodeSetName) // node set
                            sb.AppendFormat("{0}, {1}, {2}", cLoad.RegionName, dir,
                                            cLoad.GetDirection(dir - 1).ToCalculiX16String());
                        else if (cLoad.RegionType == RegionTypeEnum.ReferencePointName) // reference point
                            sb.AppendFormat("{0}, {1}, {2}", rpNodeIds[0], dir,
                                            cLoad.GetDirection(dir - 1).ToCalculiX16String());

                        sb.AppendLine();
                    }
                }
            }
            return sb.ToString();
        }
    }
}
