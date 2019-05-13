using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;

namespace CaeResults
{
    [Serializable]
    public class HistoryResultSet : NamedClass
    {
        // Variables                                                                                                                
        protected Dictionary<string, HistoryResultField> _fields;


        // Properties                                                                                                               
        public Dictionary<string, HistoryResultField> Fields { get { return _fields; } set { _fields = value; } }


        // Constructor                                                                                                              
        public HistoryResultSet(string name)
            : base()
        {
            _checkName = false;
            _name = name;
            _fields = new Dictionary<string, HistoryResultField>();
        }


        // Static methods                                                                                                           


        // Methods                                                                                                                  
       

    }
}
