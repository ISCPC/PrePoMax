using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeResults
{
    [Serializable]
    public class HistoryResultData : CaeGlobals.NamedClass
    {
        // Variables                                                                                                                
        public string SetName;
        public string FieldName;
        public string ComponentName;



        // Constructors                                                                                                              
        public HistoryResultData(string setName, string fieldName, string componentName)
            :base()
        {
            _checkName = false;
            _name = setName + "_" + fieldName + "_" + componentName;
            SetName = setName;
            FieldName = fieldName;
            ComponentName = componentName;
        }


        // Static methods                                                                                                           


        // Methods                                                                                                                  
    }
}
