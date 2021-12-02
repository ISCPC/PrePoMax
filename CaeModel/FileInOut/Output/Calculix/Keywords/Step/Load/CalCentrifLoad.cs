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
    internal class CalCentrifLoad : CalculixKeyword
    {
        // Variables                                                                                                                
        private CentrifLoad _load;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalCentrifLoad(CentrifLoad load)
        {
            _load = load;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " + _load.Name);
            sb.AppendLine("*Dload");
            return sb.ToString();
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            //
            double n1, n2, n3;
            double len = Math.Sqrt(_load.N1 * _load.N1 + _load.N2 * _load.N2 + _load.N3 * _load.N3);
            if (len != 0)
            {
                n1 = _load.N1 / len;
                n2 = _load.N2 / len;
                n3 = _load.N3 / len;
            }
            else n1 = n2 = n3 = 0;
            //
            sb.AppendFormat("{0}, CENTRIF, {1}, {2}, {3}, {4}, {5}, {6}, {7}", _load.RegionName,
                            _load.RotationalSpeed2.ToCalculiX16String(), 
                            _load.X.ToCalculiX16String(), _load.Y.ToCalculiX16String(), _load.Z.ToCalculiX16String(),
                            n1.ToCalculiX16String(), n2.ToCalculiX16String(), n3.ToCalculiX16String());
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
