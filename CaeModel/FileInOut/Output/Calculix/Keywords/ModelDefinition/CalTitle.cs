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
    public class CalTitle : CalculixKeyword
    {
        // Variables                                                                                                                
        private string _title;
        private string _data;


        // Properties                                                                                                               
        public string Title { get { return _title; } }


        // Constructor                                                                                                              
        public CalTitle(string title, string data)
        {
            _title = title;
            _data = data;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("************************************************************");
            sb.AppendLine("**");
            sb.AppendLine(("** " + _title + " ").PadRight(60, '+'));
            sb.AppendLine("**");
            //sb.AppendLine("************************************************************");
            return sb.ToString();
        }
        public override string GetDataString()
        {
            if (_data != null && _data.Length > 0) return string.Format("{0}{1}", _data, Environment.NewLine);
            else return "";
        }
    }
}
