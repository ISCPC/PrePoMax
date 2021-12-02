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
    internal class CalTie : CalculixKeyword
    {
        // Variables                                                                                                                
        private Tie _tie;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalTie(Tie tie)
        {
            _tie = tie;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            //*TIE, NAME=TIE-1, POSITION TOLERANCE=0, ADJUST=NO
            //slaveSurfaceName, masterSurfaceName
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("*Tie, Name={0}", _tie.Name );
            if (!double.IsNaN(_tie.PositionTolerance)) sb.AppendFormat(", Position tolerance={0}",
                                                                       _tie.PositionTolerance.ToCalculiX16String());
            if (!_tie.Adjust) sb.AppendFormat(", Adjust=No");
            sb.AppendLine();
            return sb.ToString();
        }
        public override string GetDataString()
        {
            return string.Format("{0}, {1}{2}", _tie.SlaveRegionName, _tie.MasterRegionName, Environment.NewLine);
        }
    }
}
