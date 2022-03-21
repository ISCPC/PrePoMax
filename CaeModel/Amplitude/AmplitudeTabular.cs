using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using System.Runtime.Serialization;

namespace CaeModel
{
    [Serializable]
    public class AmplitudeTabular : Amplitude
    {
        // Variables                                                                                                                
        


        // Properties                                                                                                               
        


        // Constructors                                                                                                             
        public AmplitudeTabular(string name, double[][] timeAmplitude)
            : base(name)
        {
            _timeAmplitude = timeAmplitude;
        }


        // Methods                                                                                                                  
    }
}
