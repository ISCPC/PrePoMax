using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeModel
{
    [Serializable]
    public class Density : MaterialProperty
    {
        // Variables                                                                                                                
        

        // Properties                                                                                                               
        public double Value { get; set; }

        // Constructors                                                                                                             
        public Density(double density)
        {
            Value = density;
        }

        // Methods                                                                                                                  
    }
}
