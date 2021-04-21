using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public class Expansion : MaterialProperty
    {
        // Variables                                                                                                                
        private static string _positive = "The value must be larger than 0.";
        //
        private double[][] _expansionTemp;


        // Properties                                                                                                               
        public double[][] ExpansionTemp
        {
            get { return _expansionTemp; }
            set
            {
                _expansionTemp = value;
                if (_expansionTemp != null)
                {
                    for (int i = 0; i < _expansionTemp.Length; i++)
                    {
                        if (_expansionTemp[i][0] <= 0) throw new CaeException(_positive);
                    }
                }
            }
        }


        // Constructors                                                                                                             
        public Expansion(double[][] expansionTemp)
        {
            _expansionTemp = expansionTemp;
        }


        // Methods                                                                                                                  
    }
}
