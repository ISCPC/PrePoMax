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
        private double[][] _thermalExpansionTemp;
        private double _zeroTemperature;


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
        public double ZeroTemperature { get { return _zeroTemperature; } set { _zeroTemperature = value; } }


        // Constructors                                                                                                             
        public ThermalExpansion(double[][] thermalExpansionTemp)
        {
            _thermalExpansionTemp = thermalExpansionTemp;
            _zeroTemperature = 20;
        }


        // Methods                                                                                                                  
    }
}
