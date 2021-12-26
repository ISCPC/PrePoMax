using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public class BodyFlux : Load
    {
        // Variables                                                                                                                
        private string _regionName;
        private RegionTypeEnum _regionType;
        private double _magnitude;


        // Properties                                                                                                               
        public override string RegionName { get { return _regionName; } set { _regionName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public double Magnitude { get { return _magnitude; } set { _magnitude = value; } }
        

        // Constructors                                                                                                             
        public BodyFlux(string name, string regionName, RegionTypeEnum regionType, double magnitude, bool twoD)
            : base(name, twoD) 
        {
            _regionName = regionName;
            _regionType = regionType;
            _magnitude = magnitude;
        }


        // Methods                                                                                                                  
    }
}
