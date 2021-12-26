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
    internal class CalSolidSection : CalculixKeyword
    {
        // Variables                                                                                                                
        private SolidSection _section;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalSolidSection(SolidSection section)
        {
            _section = section;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*Solid section, Elset={0}, Material={1}{2}", _section.RegionName,
                            _section.MaterialName, Environment.NewLine);
            return sb.ToString();
        }
        public override string GetDataString()
        {
            if (_section.TwoD) return string.Format("{0}{1}", _section.Thickness.ToCalculiX16String(), Environment.NewLine);
            else return "";
        }
    }
}
