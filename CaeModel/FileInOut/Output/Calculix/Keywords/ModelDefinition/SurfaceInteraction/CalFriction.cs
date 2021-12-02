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
    internal class CalFriction : CalculixKeyword
    {
        // Variables                                                                                                                
        private Friction _friction;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalFriction(Friction friction)
        {
            _friction = friction;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            return string.Format("*Friction{0}", Environment.NewLine);
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(_friction.Coefficient.ToCalculiX16String());
            if (!double.IsNaN(_friction.StickSlope)) sb.AppendFormat(", {0}", _friction.StickSlope.ToCalculiX16String());
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
