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
    internal class CalGapConductance : CalculixKeyword
    {
        // Variables                                                                                                                
        private GapConductance _gapConductance;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalGapConductance(GapConductance gapConductance)
        {
            _gapConductance = gapConductance;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            return string.Format("*Gap conductance{0}", Environment.NewLine);
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            if (_gapConductance.GapConductanceType == GapConductanceEnum.Constant)
            {
                sb.AppendFormat("{0}{1}", _gapConductance.ConductnancePressureTemp[0][0].ToCalculiX16String(), Environment.NewLine);
            }
            else if (_gapConductance.GapConductanceType == GapConductanceEnum.Tabular)
            {
                for (int i = 0; i < _gapConductance.ConductnancePressureTemp.Length; i++)
                {
                    sb.AppendFormat("{0}, {1}, {2}{3}", _gapConductance.ConductnancePressureTemp[i][0].ToCalculiX16String(),
                                                        _gapConductance.ConductnancePressureTemp[i][1].ToCalculiX16String(),
                                                        _gapConductance.ConductnancePressureTemp[i][2].ToCalculiX16String(),
                                                        Environment.NewLine);
                }
            }
            else throw new NotSupportedException();
            //
            return sb.ToString();
        }
    }
}
