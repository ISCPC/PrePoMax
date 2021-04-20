using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeModel
{
    [Serializable]
    public enum PlasticHardening
    {
        Isotropic,
        Kinematic,
        Combined
    }

    [Serializable]
    public class Plastic : MaterialProperty
    {
        // Variables                                                                                                                
        public double[][] StressStrainTemp { get; set; }
        public PlasticHardening Hardening { get; set; }        


        // Constructors                                                                                                             
        public Plastic(double[][] stressStrainTemp)
        {
            StressStrainTemp = stressStrainTemp;
            Hardening = PlasticHardening.Isotropic;
        }

        // Methods                                                                                                                  
    }
}
