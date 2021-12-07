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
    public class CFlux : Load
    {
        // Variables                                                                                                                
        private string _regionName;
        private RegionTypeEnum _regionType;


        // Properties                                                                                                               
        public override string RegionName { get { return _regionName; } set { _regionName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public bool AddFlux { get; set; }
        public double Magnitude { get; set; }


        // Constructors                                                                                                             
        public CFlux(string name, string regionName, RegionTypeEnum regionType, double magnitude, bool twoD)
            : base(name, twoD) 
        {
            _regionName = regionName;
            RegionType = regionType;
            //
            AddFlux = false;
            Magnitude = magnitude;
        }


        // Methods                                                                                                                  
        
    }
}
