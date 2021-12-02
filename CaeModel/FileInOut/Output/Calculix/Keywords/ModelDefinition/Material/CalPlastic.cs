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
    internal class CalPlastic : CalculixKeyword
    {
        // Variables                                                                                                                
        private Plastic _plastic;
        private bool _temperatureDependent;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalPlastic(Plastic plastic, bool temperatureDependent)
        {
            _plastic = plastic;
            _temperatureDependent = temperatureDependent;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            string hardening = "";
            if (_plastic.Hardening != PlasticHardening.Isotropic) hardening = ", Hardening=" + _plastic.Hardening;
            return string.Format("*Plastic{0}{1}", hardening, Environment.NewLine);
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            double[][] data = _plastic.StressStrainTemp;
            for (int i = 0; i < data.Length; i++)
            {
                if (_temperatureDependent)
                    sb.AppendFormat("{0}, {1}, {2}{3}", data[i][0].ToCalculiX16String(), data[i][1].ToCalculiX16String(),
                                    data[i][2].ToCalculiX16String(), Environment.NewLine);
                else
                    sb.AppendFormat("{0}, {1}{2}", data[i][0].ToCalculiX16String(), data[i][1].ToCalculiX16String(),
                                    Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
