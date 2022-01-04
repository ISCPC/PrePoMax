using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeMesh;
using CaeGlobals;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    internal class CalLinearSpringSection : CalculixKeyword
    {
        // Variables                                                                                                                
        private string _elementSetName;
        private int _direction;
        private double _stiffness;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalLinearSpringSection(string elementSetName, int direction, double stiffness)
        {
            _elementSetName = elementSetName;
            _direction = direction;
            _stiffness = stiffness;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*Spring, Elset={0}{1}", _elementSetName, Environment.NewLine);
            return sb.ToString();
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}{1}", _direction, Environment.NewLine);
            sb.AppendFormat("{0}{1}", _stiffness.ToCalculiX16String(true), Environment.NewLine);
            return sb.ToString();
        }
    }
}
