using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public class SpecificHeat : MaterialProperty
    {
        // Variables                                                                                                                
        private static string _positive = "The value must be larger than 0.";
        //
        private double[][] _specificHeatTemp;


        // Properties                                                                                                               
        public double[][] SpecificHeatTemp
        {
            get { return _specificHeatTemp; }
            set
            {
                _specificHeatTemp = value;
                if (_specificHeatTemp != null)
                {
                    for (int i = 0; i < _specificHeatTemp.Length; i++)
                    {
                        if (_specificHeatTemp[i][0] <= 0) throw new CaeException(_positive);
                    }
                }
            }
        }


        // Constructors                                                                                                             
        public SpecificHeat(double[][] specificHeatTemp)
        {
            _specificHeatTemp = specificHeatTemp;
        }


        // Methods                                                                                                                  
    }
}
