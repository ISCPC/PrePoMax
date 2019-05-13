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
    internal class CalDensity : CalculixKeyword
    {
        // Variables                                                                                                                
        private Density _density;


        // Properties                                                                                                               
        public override object BaseItem { get { return _density; } }


        // Events                                                                                                                   


        // Constructor                                                                                                              
        public CalDensity(Density density)
        {
            _density = density;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            return string.Format("*Density{0}", Environment.NewLine);
        }

        public override string GetDataString()
        {
            return string.Format("{0}{1}", _density.Value, Environment.NewLine);
        }
    }
}
