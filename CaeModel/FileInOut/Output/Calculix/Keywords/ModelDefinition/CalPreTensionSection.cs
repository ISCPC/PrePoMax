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
    internal class CalPreTensionSection : CalculixKeyword
    {
        // Variables                                                                                                                
        private string _surfaceName;
        private int _nodeId;
        double _x;
        double _y;
        double _z;


        // Constructor                                                                                                              
        public CalPreTensionSection(string surfaceName, int nodeId, double x, double y, double z)
        {
            _surfaceName = surfaceName;
            _nodeId = nodeId;
            _x = x;
            _y = y;
            _z = z;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            return string.Format("*Pre-tension section, Surface={0}, Node={1}{2}", _surfaceName, _nodeId, Environment.NewLine);
        }
        public override string GetDataString()
        {
            return string.Format("{0}, {1}, {2}{3}", _x, _y, _z, Environment.NewLine);
        }
    }
}
