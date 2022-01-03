using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.Drawing;

namespace CaeModel
{
    [Serializable]
    public abstract class Constraint : NamedClass, IMasterSlaveMultiRegion
    {
        // Variables                                                                                                                
        private string _masterRegionName;
        private RegionTypeEnum _masterRegionType;
        private string _slaveRegionName;
        private RegionTypeEnum _slaveRegionType;
        //
        private int[] _masterCreationIds;
        private Selection _masterCreationData;
        private int[] _slaveCreationIds;
        private Selection _slaveCreationData;
        //
        protected Color _masterColor;
        protected Color _slaveColor;


        // Properties                                                                                                               
        public string MasterRegionName { get { return _masterRegionName; } set { _masterRegionName = value; } }
        public RegionTypeEnum MasterRegionType { get { return _masterRegionType; } set { _masterRegionType = value; } }
        public string SlaveRegionName { get { return _slaveRegionName; } set { _slaveRegionName = value; } }
        public RegionTypeEnum SlaveRegionType { get { return _slaveRegionType; } set { _slaveRegionType = value; } }
        //
        public int[] MasterCreationIds { get { return _masterCreationIds; } set { _masterCreationIds = value; } }
        public Selection MasterCreationData { get { return _masterCreationData; } set { _masterCreationData = value; } }
        public int[] SlaveCreationIds { get { return _slaveCreationIds; } set { _slaveCreationIds = value; } }
        public Selection SlaveCreationData { get { return _slaveCreationData; } set { _slaveCreationData = value; } }
        //
        public Color MasterColor { get { return _masterColor; } set { _masterColor = value; } }
        public Color SlaveColor { get { return _slaveColor; } set { _slaveColor = value; } }


        // Constructors                                                                                                             
        public Constraint(string name, string masterRegionName, RegionTypeEnum masterRegionType,
                          string slaveRegionName, RegionTypeEnum slaveRegionType)
            : base(name) 
        {
            _masterRegionName = masterRegionName;
            _masterRegionType = masterRegionType;
            _slaveRegionName = slaveRegionName;
            _slaveRegionType = slaveRegionType;
            //
            _masterCreationIds = null;
            _masterCreationData = null;
            _slaveCreationIds = null;
            _slaveCreationData = null;
            //
            _masterColor = Color.Yellow;
            _slaveColor = Color.Yellow;
        }


        // Methods                                                                                                                  

    }
}
