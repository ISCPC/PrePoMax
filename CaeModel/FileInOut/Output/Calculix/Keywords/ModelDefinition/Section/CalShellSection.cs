﻿using System;
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
    internal class CalShellSection : CalculixKeyword
    {
        // Variables                                                                                                                
        private ShellSection _section;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalShellSection(ShellSection section)
        {
            _section = section;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*Shell section, Elset={0}, Material={1}", _section.RegionName, _section.MaterialName);
            if (!double.IsNaN(_section.Offset.Value)) sb.AppendFormat(", Offset={0}", _section.Offset.Value.ToCalculiX16String());
            sb.AppendLine();
            //
            return sb.ToString();
        }
        public override string GetDataString()
        {
            return string.Format("{0}{1}", _section.Thickness.Value.ToCalculiX16String(), Environment.NewLine);
        }
    }
}
