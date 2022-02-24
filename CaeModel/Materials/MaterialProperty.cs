using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;

namespace CaeModel
{
    [Serializable]
    public abstract class MaterialProperty
    {
        // Variables                                                                                                                
        [NonSerialized]
        protected const string _positive = "The value must be larger than 0.";


        // Properties                                                                                                               


        // Constructors                                                                                                             


        // Methods                                                                                                                  
    }
}
