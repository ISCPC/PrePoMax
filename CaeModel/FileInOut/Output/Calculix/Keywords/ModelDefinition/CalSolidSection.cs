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
    internal class CalSolidSection : CalculixKeyword
    {
        // Variables                                                                                                                
        private SolidSection _section;
        private string[] _partNames;

        // Constructor                                                                                                              
        public CalSolidSection(SolidSection section, string[] partNames = null)
        {
            _section = section;
            _partNames = partNames;
            if (_partNames != null && partNames.Length == 0) throw new NotSupportedException();
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            string setName = _section.RegionName;
            StringBuilder sb = new StringBuilder();
            //
            if (_partNames != null)
            {
                if (_partNames.Length == 1) setName = _partNames[0];
                else
                {
                    setName = _section.Name;
                    //
                    sb.AppendFormat("*Elset, Elset={0}{1}", setName, Environment.NewLine);
                    for (int i = 0; i < _partNames.Length; i++)
                    {
                        sb.Append(_partNames[i]);
                        if (i < _partNames.Length - 1) sb.Append(',');
                        sb.AppendLine();
                    }
                }
            }
            sb.AppendFormat("*Solid section, Elset={0}, Material={1}{2}", setName, _section.MaterialName,
                            Environment.NewLine);
            return sb.ToString();
        }
        public override string GetDataString()
        {
            if (_section.Type == SolidSectionType.TwoDimensional)
                return string.Format("{0}{1}", _section.Thickness, Environment.NewLine);
            else return "";
        }
    }
}
