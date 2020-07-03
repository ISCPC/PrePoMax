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
    public class HistoryResultEntries : NamedClass
    {
        // Variables                                                                                                                
        private List<double> _time;
        private List<double> _values;
        private bool _local;


        // Properties                                                                                                               
        public List<double> Time { get { return _time; } set { _time = value; } }
        public List<double> Values { get { return _values; } set { _values = value; } }
        public bool Local { get { return _local; } set { _local = value; } }


        // Constructor                                                                                                              
        public HistoryResultEntries(string name, bool local)
            : base()
        {
            _checkName = false;
            _name = name;
            _time = new List<double>();
            _values = new List<double>();
            _local = local;
        }


        // Static methods                                                                                                           


        // Methods                                                                                                                  


    }
}
