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
    public class HistoryResultField : NamedClass
    {
        // Variables                                                                                                                
        protected Dictionary<string, HistoryResultComponent> _components;


        // Properties                                                                                                               
        public Dictionary<string, HistoryResultComponent> Components { get { return _components; } set { _components = value; } }
        

        // Constructor                                                                                                              
        public HistoryResultField(string name)
            : base()
        {
            _checkName = false;
            _name = name;
            _components = new Dictionary<string, HistoryResultComponent>();
        }


        // Static methods                                                                                                           


        // Methods                                                                                                                  
       

    }
}
