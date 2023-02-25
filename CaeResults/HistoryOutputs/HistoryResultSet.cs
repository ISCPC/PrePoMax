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
        protected bool _harmonic;
        protected Dictionary<string, HistoryResultField> _fields;


        // Properties                                                                                                               
        public bool Harmonic { get { return _harmonic; } set { _harmonic = value; } }
        public Dictionary<string, HistoryResultField> Fields { get { return _fields; } set { _fields = value; } }


        // Constructor                                                                                                              
        public HistoryResultSet(string name)
            : base()
        {
            _checkName = false;
            _name = name;
            _harmonic = false;
            _fields = new Dictionary<string, HistoryResultField>();
        }


        // Static methods                                                                                                           


        // Methods                                                                                                                  
       

    }
}
