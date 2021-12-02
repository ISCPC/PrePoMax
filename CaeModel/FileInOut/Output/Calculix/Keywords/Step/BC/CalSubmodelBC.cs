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
    internal class CalSubmodelBC : CalculixKeyword
    {
        // Variables                                                                                                                
        private SubmodelBC _submodel;
        private string _surfaceNodeSetName;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalSubmodelBC(SubmodelBC submodel, string surfaceNodeSetName)
        {
            _submodel = submodel;
            _surfaceNodeSetName = surfaceNodeSetName;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " + _submodel.Name);
            sb.AppendLine("*Boundary, Submodel, Step=" + _submodel.StepNumber);
            return sb.ToString();
        }
        public override string GetDataString()
        {
            // *Boundary, Submodel, Step=1
            // nodeSet-1, 1, 1        node set id, start DOF, end DOF
            StringBuilder sb = new StringBuilder();
            int[] directions = _submodel.GetConstrainedDirections();
            // Node set
            if (_submodel.RegionType == CaeGlobals.RegionTypeEnum.NodeSetName)
            {
                for (int i = 0; i < directions.Length; i++)
                {
                    sb.AppendFormat("{0}, {1}, {2}", _submodel.RegionName, directions[i], directions[i]);
                    sb.AppendLine();
                }
            }
            // Surface
            else if (_submodel.RegionType == CaeGlobals.RegionTypeEnum.SurfaceName)
            {
                if (_surfaceNodeSetName == null) throw new ArgumentException();
                for (int i = 0; i < directions.Length; i++)
                {
                    sb.AppendFormat("{0}, {1}, {2}", _surfaceNodeSetName, directions[i], directions[i]);
                    sb.AppendLine();
                }
            }
            else throw new NotSupportedException();
            //
            return sb.ToString();
        }
    }
}
