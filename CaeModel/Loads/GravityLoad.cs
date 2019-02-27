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
    public class GravityLoad : Load, IMultiRegion
    {
        // Variables                                                                                                                
        private string _regionName;
        private RegionTypeEnum _regionType;


        // Properties                                                                                                               
        public string RegionName { get { return _regionName; } set { _regionName = value; } }
        public RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public double F1 { get; set; }
        public double F2 { get; set; }
        public double F3 { get; set; }


        // Constructors                                                                                                             
        public GravityLoad(string name, string regionName, RegionTypeEnum regionType, double f1, double f2, double f3)
            : base(name) 
        {
            _regionName = regionName;
            RegionType = regionType;

            F1 = f1;
            F2 = f2;
            F3 = f3;
        }
        public GravityLoad(string name, string regionName, RegionTypeEnum regionType)
            : this(name, regionName, regionType, 0, 0, 0)
        {
        }
        
        
        // Methods                                                                                                                  
    }
}
