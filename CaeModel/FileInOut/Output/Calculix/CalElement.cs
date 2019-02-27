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
        private MeshPart _part;


        // Properties                                                                                                               
        public override object BaseItem { get { return _part; } }


        // Events                                                                                                                   


        // Constructor                                                                                                              
        public CalElement(string elementType, string elementSetName, List<FeElement> elements, MeshPart part)
        {
            _elementType = elementType;
            _elementSetName = elementSetName;
            _elements = elements;
            _part = part;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            return string.Format("*Element, Type={0}, Elset={1}{2}", _elementType, _elementSetName, Environment.NewLine);
        }

        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            int count;
            foreach (FeElement feElement in _elements)
            {
                sb.AppendFormat("{0}", feElement.Id);
                count = 1;
                foreach (int nodeId in feElement.NodeIDs)
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
