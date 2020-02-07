using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    internal class CalHeading : CalculixKeyword
    {
        // Variables                                                                                                                
        string _modelName;

        
        // Constructor                                                                                                              
        public CalHeading(string modelName)
        {
            _modelName = modelName;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            return string.Format("*Heading{0}", Environment.NewLine);
        }
        public override string GetDataString()
        {
            return string.Format("Model: {0},     Date: {1}" + Environment.NewLine, _modelName, DateTime.Now.ToShortDateString());
        }
    }
}
