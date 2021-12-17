﻿using System;
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


        // Constructor                                                                                                              
        public CalNodeFile(NodalFieldOutput nodalFieldOutput)
        {
            _nodalFieldOutput = nodalFieldOutput;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            string frequency = _nodalFieldOutput.Frequency > 1 ? ", Frequency=" + _nodalFieldOutput.Frequency : "";
            string lastIterations = _nodalFieldOutput.LastIterations ? ", Last iterations" : "";
            //
            return string.Format("*Node file{0}{1}{2}", frequency, lastIterations, Environment.NewLine);
        }
        public override string GetDataString()
        {
            return string.Format("{0}{1}", _nodalFieldOutput.Variables.ToString(), Environment.NewLine);
        }
    }
}
