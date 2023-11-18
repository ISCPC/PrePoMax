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
    internal class CalEquation : CalculixKeyword
    {
        // Variables                                                                                                                
        private double[] _parameters;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalEquation(double[] parameters)
        {
            if (parameters.Length % 3 != 0) throw new NotSupportedException();
            _parameters = parameters;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            //*EQUATION
            //3
            //3,2,2.3,28,1,4.05,17,1,-8.22
            return "*Equation" + Environment.NewLine;
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine((_parameters.Length / 3).ToString());
            sb.Append(_parameters[0]);
            for (int i = 1; i < _parameters.Length; i++)
            {
                sb.Append(", ");
                if (i % 12 == 0) sb.AppendLine();
                sb.Append(_parameters[i].ToString());
            }
            //
            if (_parameters.Length % 12 != 0) sb.AppendLine();
            //
            return sb.ToString();
        }
    }
}