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
    internal class CalLinearSpringSection : CalculixKeyword
    {
        // Variables                                                                                                                
        private LinearSpringSectionData _linearSpringSectionData;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalLinearSpringSection(LinearSpringSectionData linearSpringSectionData)
        {
            _linearSpringSectionData = linearSpringSectionData;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*Spring, Elset={0}{1}", _linearSpringSectionData.RegionName, Environment.NewLine);
            return sb.ToString();
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}{1}", _linearSpringSectionData.Direction, Environment.NewLine);
            sb.AppendFormat("{0}{1}", _linearSpringSectionData.Stiffness.ToCalculiX16String(true), Environment.NewLine);
            return sb.ToString();
        }
    }
}
