using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using CaeModel;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    internal class CalHeading : CalculixKeyword
    {
        // Variables                                                                                                                
        string _modelName;
        string _hashName;
        UnitSystemType _unitSystemType;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalHeading(string modelName, string hashName, UnitSystemType unitSystemType)
        {
            _modelName = modelName;
            _hashName = hashName;
            _unitSystemType = unitSystemType;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            return string.Format("*Heading{0}", Environment.NewLine);
        }
        public override string GetDataString()
        {
            return string.Format("Hash: {0}, Date: {1}, Unit system: {2}{3}",
                                 _hashName,
                                 DateTime.Now.ToShortDateString(), _unitSystemType,
                                 Environment.NewLine);
        }
    }
}
