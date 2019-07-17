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
    internal class CalElFile : CalculixKeyword
    {
        // Variables                                                                                                                
        private ElementFieldOutput _elementFieldOutput;


        // Properties                                                                                                               
        public override object BaseItem { get { return _elementFieldOutput; } }


        // Events                                                                                                                   


        // Constructor                                                                                                              
        public CalElFile(ElementFieldOutput elementFieldOutput)
        {
            _elementFieldOutput = elementFieldOutput;
            _active = elementFieldOutput.Active;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            string frequency = _elementFieldOutput.Frequency > 1 ? ", Frequency=" + _elementFieldOutput.Frequency : "";

            return string.Format("*El file{0}{1}", frequency, Environment.NewLine);
        }

        public override string GetDataString()
        {
            return string.Format("{0}{1}", _elementFieldOutput.Variables.ToString(), Environment.NewLine);
        }
    }
}
