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
    internal class CalFixedBC : CalculixKeyword
    {
        // Variables                                                                                                                
        private FixedBC _fixedBC;
        private Dictionary<string, int[]> _referencePointsNodeIds;
        private string _nodeSetNameOfSurface;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalFixedBC(FixedBC fixedBC, Dictionary<string, int[]> referencePointsNodeIds,
                          string nodeSetNameOfSurface)
        {
            _fixedBC = fixedBC;
            _referencePointsNodeIds = referencePointsNodeIds;
            _nodeSetNameOfSurface = nodeSetNameOfSurface;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " + _fixedBC.Name);
            sb.AppendLine("*Boundary");
            return sb.ToString();
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            // *Boundary
            // 6975, 1, 1, 0        node id, start DOF, end DOF, value
            int[] directions = _fixedBC.GetConstrainedDirections();
            // Node set
            if (_fixedBC.RegionType == CaeGlobals.RegionTypeEnum.NodeSetName)
            {
                if (_fixedBC.TwoD)
                {
                    sb.AppendFormat("{0}, 1, 2, 0", _fixedBC.RegionName); sb.AppendLine();
                    sb.AppendFormat("{0}, 6, 6, 0", _fixedBC.RegionName);
                }
                else
                {
                    sb.AppendFormat("{0}, 1, 6, 0", _fixedBC.RegionName);
                }
                sb.AppendLine();
            }
            // Surface
            else if (_fixedBC.RegionType == CaeGlobals.RegionTypeEnum.SurfaceName)
            {
                if (_nodeSetNameOfSurface == null) throw new ArgumentException();
                if (_fixedBC.TwoD)
                {
                    sb.AppendFormat("{0}, 1, 2, 0", _nodeSetNameOfSurface); sb.AppendLine();
                    sb.AppendFormat("{0}, 6, 6, 0", _nodeSetNameOfSurface);
                }
                else
                {
                    sb.AppendFormat("{0}, 1, 6, 0", _nodeSetNameOfSurface);
                }
                sb.AppendLine();
            }
            // Reference point
            else if (_fixedBC.RegionType == CaeGlobals.RegionTypeEnum.ReferencePointName)
            {
                int[] rpNodeIds = _referencePointsNodeIds[_fixedBC.RegionName];
                //
                if (_fixedBC.TwoD)
                {
                    sb.AppendFormat("{0}, 1, 2, 0", rpNodeIds[0]); sb.AppendLine();
                    sb.AppendFormat("{0}, 3, 3, 0", rpNodeIds[1]); sb.AppendLine();
                }
                else
                {
                    sb.AppendFormat("{0}, 1, 3, 0", rpNodeIds[0]); sb.AppendLine();
                    sb.AppendFormat("{0}, 1, 3, 0", rpNodeIds[1]); sb.AppendLine();
                }
            }
            else throw new NotSupportedException();
            //
            return sb.ToString();
        }
    }
}
