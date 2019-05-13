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
    public class MomentLoad : Load, IMultiRegion
    {
        // Variables                                                                                                                
        private string _regionName;
        private RegionTypeEnum _regionType;
        private int _nodeId;

        // Properties                                                                                                               
        public string RegionName { get { return _regionName; } set { _regionName = value; } }
        public RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public int NodeId { get { return _nodeId; } set { _nodeId = value; } }
        public double M1 { get; set; }
        public double M2 { get; set; }
        public double M3 { get; set; }
        public double GetDirection(int direction)
        {
            if (direction == 0) return M1;
            else if (direction == 1) return M2;
            else return M3;
        }

        // Constructors                                                                                                             
        public MomentLoad(string name, string regionName, RegionTypeEnum regionType, double m1, double m2, double m3)
            : base(name) 
        {
            _regionName = regionName;
            RegionType = regionType;
            _nodeId = -1;

            M1 = m1;
            M2 = m2;
            M3 = m3;
        }

        public MomentLoad(string name, int nodeId, double m1, double m2, double m3)
            : base(name)
        {
            _regionName = null;
            RegionType = CaeGlobals.RegionTypeEnum.NodeId;
            _nodeId = nodeId;

            M1 = m1;
            M2 = m2;
            M3 = m3;
        }

        // Methods                                                                                                                  
        
    }
}
