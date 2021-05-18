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
    internal class CalSTLoad : CalculixKeyword
    {
        // Variables                                                                                                                
        private STLoad _load;
        private CLoad[] _cLoads;
        private Dictionary<string, int[]> _referencePointsNodeIds;


        // Properties                                                                                                               
        public override object GetBase { get { return _load; } }


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
            sb.AppendLine("*Cload");
            return sb.ToString();
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            if (_cLoads != null)
            {
                foreach (var stLoad in _cLoads)
                {
                    int[] rpNodeIds = null;
                    if (stLoad.RegionType == CaeGlobals.RegionTypeEnum.ReferencePointName) rpNodeIds = _referencePointsNodeIds[stLoad.RegionName];
                    //
                    List<int> directions = new List<int>();
                    if (stLoad.F1 != 0) directions.Add(1);
                    if (stLoad.F2 != 0) directions.Add(2);
                    if (stLoad.F3 != 0) directions.Add(3);
                    //
                    foreach (var dir in directions)
                    {
                        if (stLoad.RegionType == CaeGlobals.RegionTypeEnum.NodeId)
                            sb.AppendFormat("{0}, {1}, {2}", stLoad.NodeId, dir, stLoad.GetDirection(dir - 1).ToString());
                        else if (stLoad.RegionType == CaeGlobals.RegionTypeEnum.NodeSetName) // node set
                            sb.AppendFormat("{0}, {1}, {2}", stLoad.RegionName, dir, stLoad.GetDirection(dir - 1).ToString());
                        else if (stLoad.RegionType == CaeGlobals.RegionTypeEnum.ReferencePointName) // reference point
                            sb.AppendFormat("{0}, {1}, {2}", rpNodeIds[0], dir, stLoad.GetDirection(dir - 1).ToString());

                        sb.AppendLine();
                    }
                }
            }
            return sb.ToString();
        }
    }
}
