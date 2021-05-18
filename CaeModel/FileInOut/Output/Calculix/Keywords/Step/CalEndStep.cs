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
    internal class CalEndStep : CalculixKeyword
    {
        // Variables                                                                                                                


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalEndStep()
        {
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            return string.Format("*End step{0}", Environment.NewLine);
        }
        public override string GetDataString()
        {
            return "";
        }
    }
}
