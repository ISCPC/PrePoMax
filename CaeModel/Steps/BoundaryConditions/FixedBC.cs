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
    public class FixedBC : BoundaryCondition
    {
        // Variables                                                                                                                
        private RegionTypeEnum _regionType;
        private string _regionName;


        // Properties                                                                                                               
        public override string RegionName { get { return _regionName; } set { _regionName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }


        // Constructors                                                                                                             
        public FixedBC(string name, string regionName, RegionTypeEnum regionType)
            : base(name) 
        {
            _regionName = regionName;
            _regionType = regionType;
        }


        // Methods                                                                                                                  
        public int[] GetConstrainedDirections()
        {
            return new int[] { 1, 2, 3, 4, 5, 6 };
        }
    }
}
