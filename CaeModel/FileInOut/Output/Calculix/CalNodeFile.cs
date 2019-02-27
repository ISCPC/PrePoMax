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
    internal class CalNodeFile : CalculixKeyword
    {
        // Variables                                                                                                                
        private NodalFieldOutput _nodalFieldOutput;


        // Properties                                                                                                               
        public override object BaseItem { get { return _nodalFieldOutput; } }


        // Events                                                                                                                   


        // Constructor                                                                                                              
        public CalNodeFile(NodalFieldOutput nodalFieldOutput)
        {
            _nodalFieldOutput = nodalFieldOutput;
            _active = nodalFieldOutput.Active;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            return string.Format("*Node file{0}", Environment.NewLine);
        }

        public override string GetDataString()
        {
            return string.Format("{0}{1}", _nodalFieldOutput.Variables.ToString(), Environment.NewLine);
        }
    }
}
