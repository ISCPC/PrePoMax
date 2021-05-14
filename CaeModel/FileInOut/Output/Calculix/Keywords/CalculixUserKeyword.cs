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
    public class CalculixUserKeyword : CalculixKeyword
    {
        // Variables                                                                                                                
        private string _data;
        private object _parent;
        private object _previousKeyword;


        // Properties                                                                                                               
        public string Data { get { return _data; } set { _data = value; } }
        public object Parent { get { return _parent; } set { _parent = value; } }
        public object PreviousKeyword { get { return _previousKeyword; } set { _previousKeyword = value; } }
        public override object GetBase { get { return this; } }


        // Constructor                                                                                                              
        public CalculixUserKeyword(string data)
        {
            _data = data;
            _parent = null;
            _previousKeyword = null;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            if (_data != null && _data.Length > 0) return string.Format("{0}{1}", _data, Environment.NewLine);
            else return "";
        }
        public override string GetDataString()
        {
            return "";
        }
    }
}
