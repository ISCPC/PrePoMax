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
    public class Tie : Constraint, IMasterSlaveMultiRegion
    {
        // Variables                                                                                                                
        private RegionTypeEnum _slaveRegionType;
        private string _slaveSurfaceName;
        private RegionTypeEnum _masterRegionType;
        private string _masterSurfaceName;
        private double _positionTolerance;
        private bool _adjust;
        private int[] _slaveCreationIds;
        private Selection _slaveCreationData;
        private int[] _masterCreationIds;
        private Selection _masterCreationData;


        // Properties                                                                                                               
        public RegionTypeEnum MasterRegionType { get { return _masterRegionType; } set { _masterRegionType = value; } }
        public string MasterRegionName { get { return _masterSurfaceName; } set { _masterSurfaceName = value; } }
        public RegionTypeEnum SlaveRegionType { get { return _slaveRegionType; } set { _slaveRegionType = value; } }
        public string SlaveRegionName { get { return _slaveSurfaceName; } set { _slaveSurfaceName = value; } }
        public double PositionTolerance { get { return _positionTolerance; } set { _positionTolerance = value; } }
        public bool Adjust { get { return _adjust; } set { _adjust = value; } }
        //
        public int[] SlaveCreationIds { get { return _slaveCreationIds; } set { _slaveCreationIds = value; } }
        public Selection SlaveCreationData { get { return _slaveCreationData; } set { _slaveCreationData = value; } }
        public int[] MasterCreationIds { get { return _masterCreationIds; } set { _masterCreationIds = value; } }
        public Selection MasterCreationData { get { return _masterCreationData; } set { _masterCreationData = value; } }


        // Constructors                                                                                                             
        public Tie(string name, string masterSurfaceName, RegionTypeEnum masterRegionType,
                   string slaveSurfaceName, RegionTypeEnum slaveRegionType)
           : this(name, masterSurfaceName, masterRegionType, slaveSurfaceName, slaveRegionType,  0, true)
        {
        }
        public Tie(string name, string masterSurfaceName, RegionTypeEnum masterRegionType, 
                   string slaveSurfaceName, RegionTypeEnum slaveRegionType,
                   double positionTolerance, bool adjust)
            : base(name)
        {
            if (masterRegionType == RegionTypeEnum.SurfaceName && slaveRegionType == RegionTypeEnum.SurfaceName &&
                slaveSurfaceName == masterSurfaceName) throw new CaeException("The master and slave surface names must be different.");
            //
            _masterRegionType = masterRegionType;
            _masterSurfaceName = masterSurfaceName;
            _slaveRegionType = slaveRegionType;
            _slaveSurfaceName = slaveSurfaceName;
            _positionTolerance = positionTolerance;
            _adjust = adjust;
            //
            _slaveCreationIds = null;
            _slaveCreationData = null;
            _masterCreationIds = null;
            _masterCreationData = null;
        }


        // Methods                                                                                                                  


    }
}
