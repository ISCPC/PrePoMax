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
        private static string _positive = "The value must be larger than 0.";
        //
        private double _positionTolerance;
        private bool _adjust;
        //
        private RegionTypeEnum _masterRegionType;
        private string _masterSurfaceName;
        private RegionTypeEnum _slaveRegionType;
        private string _slaveSurfaceName;
        //
        private int[] _masterCreationIds;
        private Selection _masterCreationData;
        private int[] _slaveCreationIds;
        private Selection _slaveCreationData;


        // Properties                                                                                                               
        public double PositionTolerance
        {
            get { return _positionTolerance; }
            set { if (double.IsNaN(value) || value > 0) _positionTolerance = value; else throw new CaeException(_positive); }
        }
        public bool Adjust { get { return _adjust; } set { _adjust = value; } }
        //
        public RegionTypeEnum MasterRegionType { get { return _masterRegionType; } set { _masterRegionType = value; } }
        public string MasterRegionName { get { return _masterSurfaceName; } set { _masterSurfaceName = value; } }
        public RegionTypeEnum SlaveRegionType { get { return _slaveRegionType; } set { _slaveRegionType = value; } }
        public string SlaveRegionName { get { return _slaveSurfaceName; } set { _slaveSurfaceName = value; } }
        //
        public int[] MasterCreationIds { get { return _masterCreationIds; } set { _masterCreationIds = value; } }
        public Selection MasterCreationData { get { return _masterCreationData; } set { _masterCreationData = value; } }
        public int[] SlaveCreationIds { get { return _slaveCreationIds; } set { _slaveCreationIds = value; } }
        public Selection SlaveCreationData { get { return _slaveCreationData; } set { _slaveCreationData = value; } }


        // Constructors                                                                                                             
        public Tie(string name, string masterSurfaceName, RegionTypeEnum masterRegionType,
                   string slaveSurfaceName, RegionTypeEnum slaveRegionType)
           : this(name, double.NaN, true, masterSurfaceName, masterRegionType, slaveSurfaceName, slaveRegionType)
        {
        }
        public Tie(string name, double positionTolerance, bool adjust, string masterSurfaceName, RegionTypeEnum masterRegionType,
                   string slaveSurfaceName, RegionTypeEnum slaveRegionType)
            : base(name)
        {
            if (masterRegionType == RegionTypeEnum.SurfaceName && slaveRegionType == RegionTypeEnum.SurfaceName &&
                slaveSurfaceName == masterSurfaceName) throw new CaeException("The master and slave surface names must be different.");
            //
            PositionTolerance = positionTolerance;
            _adjust = adjust;
            //
            _masterRegionType = masterRegionType;
            _masterSurfaceName = masterSurfaceName;
            _slaveRegionType = slaveRegionType;
            _slaveSurfaceName = slaveSurfaceName;
            //
            _masterCreationIds = null;
            _masterCreationData = null;
            _slaveCreationIds = null;
            _slaveCreationData = null;
        }


        // Methods                                                                                                                  
        public void SwapMasterSlave()
        {
            if (_name.Contains("_to_"))
            {
                string[] tmp = _name.Split(new string[] { "_to_" }, StringSplitOptions.None);
                if (tmp.Length == 2) _name = tmp[1] + "_to_" + tmp[0];
            }
            //
            RegionTypeEnum tmpRegionType = _masterRegionType;
            _masterRegionType = _slaveRegionType;
            _slaveRegionType = tmpRegionType;
            //
            string tmpSurfaceName = _masterSurfaceName;
            _masterSurfaceName = _slaveSurfaceName;
            _slaveSurfaceName = tmpSurfaceName;
            //
            int[] tmpCreationIds = _masterCreationIds;
            _masterCreationIds = _slaveCreationIds;
            _slaveCreationIds = tmpCreationIds;
            //
            Selection tmpCreationData = _masterCreationData;
            _masterCreationData = _slaveCreationData;
            _slaveCreationData = tmpCreationData;
        }

    }
}
