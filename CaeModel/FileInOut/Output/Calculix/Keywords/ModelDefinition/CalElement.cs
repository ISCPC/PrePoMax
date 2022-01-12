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
    internal class CalElement : CalculixKeyword
    {
        // Variables                                                                                                                
        private string _elementType;
        private string _elementSetName;
        private List<FeElement> _elements;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalElement(string elementType, string elementSetName, List<FeElement> elements)
        {
            _elementType = elementType;
            _elementSetName = elementSetName;
            _elements = elements;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            string elSet = "";
            if (_elementSetName != null && _elementSetName.Length > 0) elSet = ", Elset=" + _elementSetName;
            //
            return string.Format("*Element, Type={0}{1}{2}", _elementType, elSet, Environment.NewLine);
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            int count;
            // Sort
            List<FeElement> sortedElements = _elements.OrderBy(element => element.Id).ToList();
            //
            foreach (FeElement feElement in sortedElements)
            {
                sb.AppendFormat("{0}", feElement.Id);
                count = 1;
                foreach (int nodeId in feElement.NodeIds)
                {
                    count++;
                    if (count == 17)        // 16 entries per line; 17th entry goes in new line
                    {
                        sb.AppendLine();
                        sb.Append(nodeId);
                    }
                    else
                        sb.AppendFormat(", {0}", nodeId);

                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
