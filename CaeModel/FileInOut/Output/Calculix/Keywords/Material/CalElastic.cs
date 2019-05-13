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


        // Properties                                                                                                               
        public override object BaseItem { get { return _elastic; } }


        // Events                                                                                                                   


        // Constructor                                                                                                              
        public CalElastic(Elastic elastic)
        {
            _elastic = elastic;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            return string.Format("*Elastic{0}", Environment.NewLine);
        }

        public override string GetDataString()
        {
            return string.Format("{0}, {1}{2}", _elastic.YoungsModulus, _elastic.PoissonsRatio, Environment.NewLine);
        }
    }
}
