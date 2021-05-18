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
    public class CalDeactivated : CalculixKeyword
    {
        // Variables                                                                                                                
        private string _name;


        // Properties                                                                                                               
        public string Name { get { return _name; } }


        // Constructor                                                                                                              
        public CalDeactivated(string name)
        {
            _name = name;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(("** Name: " + _name + ": Deactivated"));
            return sb.ToString();
        }
        public override string GetDataString()
        {
            return "";
        }
    }
}
