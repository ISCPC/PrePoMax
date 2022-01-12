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
        private string[] _partNames;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalElementSet(FeGroup elementSet)
            : this(elementSet, null)
        {
        }
        public CalElementSet(FeGroup elementSet, FeModel model)
        {
            _elementSet = elementSet;
            if (model != null && _elementSet is FeElementSet es && es.CreatedFromParts)
                _partNames = model.Mesh.GetPartNamesByIds(es.Labels);
            else _partNames = null;
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
            if (_partNames != null)
            {
                foreach (var partName in _partNames)
                {
                    sb.Append(partName);
                    if (count < _partNames.Length - 1)
                    {
                        sb.Append(", ");
                        if (++count % 1 == 0) sb.AppendLine();
                    }
                }
            }
            else
            {
                int[] sorted = _elementSet.Labels.ToArray();
                Array.Sort(sorted);
                //
                foreach (var elementId in sorted)
                {
                    sb.Append(elementId);
                    if (count < sorted.Length - 1)
                    {
                        sb.Append(", ");
                        if (++count % 16 == 0) sb.AppendLine();
                    }
                }
            }
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
