using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeModel
{
    [Serializable]
    public class Plastic : MaterialProperty
    {
        // Variables                                                                                                                
        public double[][] StressStrain { get; set; }


        // Constructors                                                                                                             
        public Plastic(double[][] stressStrain)
        {
            StressStrain = stressStrain;
        }

        // Methods                                                                                                                  
    }
}
