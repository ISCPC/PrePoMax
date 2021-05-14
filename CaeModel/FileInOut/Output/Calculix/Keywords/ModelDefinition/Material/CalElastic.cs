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
    internal class CalElastic : CalculixKeyword
    {
        // Variables                                                                                                                
        private Elastic _elastic;
        private bool _temperatureDependent;


        // Properties                                                                                                               
        public override object GetBase { get { return _elastic; } }


        // Constructor                                                                                                              
        public CalElastic(Elastic elastic, bool temperatureDependent)
        {
            _elastic = elastic;
            _temperatureDependent = temperatureDependent;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            return string.Format("*Elastic{0}", Environment.NewLine);
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            double[][] data = _elastic.YoungsPoissonsTemp;
            //
            for (int i = 0; i < data.Length; i++)
            {
                if (_temperatureDependent)
                    sb.AppendFormat("{0}, {1}, {2}{3}", data[i][0], data[i][1], data[i][2], Environment.NewLine);
                else
                {
                    sb.AppendFormat("{0}, {1}{2}", data[i][0], data[i][1], Environment.NewLine);
                    break;
                }
            }
            return sb.ToString();
        }
    }
}
