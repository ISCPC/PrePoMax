using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public class ThermalExpansion : MaterialProperty
    {
        // Variables                                                                                                                
        private static string _positive = "The value must be larger than 0.";
        //
        private double[][] _thermalExpansionTemp;


        // Properties                                                                                                               
        public double[][] ThermalExpansionTemp
        {
            get { return _thermalExpansionTemp; }
            set
            {
                _thermalExpansionTemp = value;
                if (_thermalExpansionTemp != null)
                {
                    for (int i = 0; i < _thermalExpansionTemp.Length; i++)
                    {
                        if (_thermalExpansionTemp[i][0] <= 0) throw new CaeException(_positive);
                    }
                }
            }
        }


        // Constructors                                                                                                             
        public ThermalExpansion(double[][] thermalExpansionTemp)
        {
            _thermalExpansionTemp = thermalExpansionTemp;
        }


        // Methods                                                                                                                  
    }
}
