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
    public class CLoad : Load, IMultiRegion
    {
        // Variables                                                                                                                
        private string _regionName;
        private RegionTypeEnum _regionType;
        private int _nodeId;

        // Properties                                                                                                               
        public string RegionName { get { return _regionName; } set { _regionName = value; } }
        public RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public int NodeId { get { return _nodeId; } set { _nodeId = value; } }
        public double F1 { get; set; }
        public double F2 { get; set; }
        public double F3 { get; set; }
        public double GetDirection(int direction)
        {
            if (direction == 0) return F1;
            else if (direction == 1) return F2;
            else return F3;
        }


        // Constructors                                                                                                             
        public CLoad(string name, string regionName, RegionTypeEnum regionType, double f1, double f2, double f3)
            : base(name) 
        {
            _regionName = regionName;
            RegionType = regionType;
            _nodeId = -1;

            F1 = f1;
            F2 = f2;
            F3 = f3;
        }
        public CLoad(string name, int nodeId, double f1, double f2, double f3)
            : base(name)
        {
            _regionName = null;
            RegionType = CaeGlobals.RegionTypeEnum.NodeId;
            _nodeId = nodeId;

            F1 = f1;
            F2 = f2;
            F3 = f3;
        }


        // Methods                                                                                                                  
        
    }
}
