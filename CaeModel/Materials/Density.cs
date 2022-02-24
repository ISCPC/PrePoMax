using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public class Density : MaterialProperty
    {
        // Variables                                                                                                                
        private double[][] _densityTemp;


        // Properties                                                                                                               
        public double[][] DensityTemp
        {
            get { return _densityTemp; }
            set
            {
                _densityTemp = value;
                if (_densityTemp != null)
                {
                    for (int i = 0; i < _densityTemp.Length; i++)
                    {
                        if (_densityTemp[i][0] <= 0) throw new CaeException(_positive);
                    }
                }
            }
        }


        // Constructors                                                                                                             
        public Density(double[][] densityTemp)
        {
            _densityTemp = densityTemp;
        }


        // Methods                                                                                                                  
    }
}
