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
    internal class CalFrequencyStep : CalculixKeyword
    {
        // Variables                                                                                                                
        private FrequencyStep _step;


        // Properties                                                                                                               
        public override object BaseItem { get { return _step; } }


        // Events                                                                                                                   


        // Constructor                                                                                                              
        public CalFrequencyStep(FrequencyStep step)
        {
            _step = step;
            _active = step.Active;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            return string.Format("*Frequency{0}", Environment.NewLine);
        }

        public override string GetDataString()
        {
            return string.Format("{0}{1}", _step.NumOfFrequencies, Environment.NewLine);
        }
    }
}
