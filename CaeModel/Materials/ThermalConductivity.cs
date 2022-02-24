using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public class ThermalConductivity : MaterialProperty
    {
        // Variables                                                                                                                
        private double[][] _thermalConductivityTemp;


        // Properties                                                                                                               
        public double[][] ThermalConductivityTemp
        {
            get { return _thermalConductivityTemp; }
            set
            {
                _thermalConductivityTemp = value;
                if (_thermalConductivityTemp != null)
                {
                    for (int i = 0; i < _thermalConductivityTemp.Length; i++)
                    {
                        if (_thermalConductivityTemp[i][0] <= 0) throw new CaeException(_positive);
                    }
                }
            }
        }


        // Constructors                                                                                                             
        public ThermalConductivity(double[][] thermalConductivityTemp)
        {
            _thermalConductivityTemp = thermalConductivityTemp;
        }


        // Methods                                                                                                                  
    }
}
