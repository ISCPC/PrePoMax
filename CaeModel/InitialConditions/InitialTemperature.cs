using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using System.ComponentModel;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public class InitialTemperature : InitialCondition
    {
        // Variables                                                                                                                
        private double _temperature;


        // Properties                                                                                                               
        public double Temperature { get { return _temperature; } set { _temperature = value; } }


        // Constructors                                                                                                             
        public InitialTemperature(string name, string regionName, RegionTypeEnum regionType)
            : base(name, regionName, regionType)
        {
        }


        // Methods                                                                                                                  
    }
}
