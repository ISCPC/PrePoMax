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
    internal class CalElementSet : CalculixKeyword
    {
        // Variables                                                                                                                
        private FeGroup _elementSet;


        // Constructor                                                                                                              
        public CalElementSet(FeGroup elementSet)
        {
            _elementSet = elementSet;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            return string.Format("*Elset, Elset={0}{1}", _elementSet.Name, Environment.NewLine);
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            foreach (var elementId in _elementSet.Labels)
            {
                sb.Append(elementId);
                if (count < _elementSet.Labels.Length - 1)
                    sb.Append(", ");
                if (++count % 16 == 0) sb.AppendLine();
            }
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
