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
    internal class CalNodeSet : CalculixKeyword
    {
        // Variables                                                                                                                
        private FeNodeSet _nodeSet;


        // Constructor                                                                                                              
        public CalNodeSet(FeNodeSet nodeSet)
        {
            _nodeSet = nodeSet;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            return string.Format("*Nset, Nset={0}{1}", _nodeSet.Name, Environment.NewLine);
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            foreach (var nodeId in _nodeSet.Labels)
            {
                sb.Append(nodeId);
                if (count < _nodeSet.Labels.Length - 1)
                    sb.Append(", ");
                if (++count % 16 == 0) sb.AppendLine();
            }
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
