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


        // Properties                                                                                                               
        public override object BaseItem { get { return _section; } }


        // Events                                                                                                                   


        // Constructor                                                                                                              
        public CalSolidSection(SolidSection section)
        {
            _section = section;
            _active = section.Active;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            return string.Format("*Solid section, Elset={0}, Material={1}{2}", _section.RegionName, _section.MaterialName, Environment.NewLine);
        }

        public override string GetDataString()
        {
            if (_section.Type == SolidSectionType.TwoDimensional) return string.Format("{0}{1}", _section.Thickness, Environment.NewLine);
            return "";
        }
    }
}
