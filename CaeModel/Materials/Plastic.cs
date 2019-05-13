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
        public double[][] StressStrain { get; set; }

        public PlasticHardening Hardening { get; set; }


        // Constructors                                                                                                             
        public Plastic(double[][] stressStrain)
        {
            StressStrain = stressStrain;
            Hardening = PlasticHardening.Isotropic;
        }

        // Methods                                                                                                                  
    }
}
