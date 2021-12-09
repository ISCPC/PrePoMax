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
    internal class CalMembraneSection : CalculixKeyword
    {
        // Variables                                                                                                                
        private MembraneSection _section;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalMembraneSection(MembraneSection section)
        {
            _section = section;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*Membrane section, Elset={0}, Material={1}", _section.RegionName, _section.MaterialName);
            if (!double.IsNaN(_section.Offset)) sb.AppendFormat(", Offset={0}", _section.Offset.ToCalculiX16String());
            sb.AppendLine();
            //
            return sb.ToString();
        }
        public override string GetDataString()
        {
            return string.Format("{0}{1}", _section.Thickness.ToCalculiX16String(), Environment.NewLine);
        }
    }
}
