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
    public class Tie : Constraint
    {
        // Variables                                                                                                                
        private static string _positive = "The value must be larger than 0.";
        //
        private double _positionTolerance;
        private bool _adjust;


        // Properties                                                                                                               
        public double PositionTolerance
        {
            get { return _positionTolerance; }
            set { if (double.IsNaN(value) || value > 0) _positionTolerance = value; else throw new CaeException(_positive); }
        }
        public bool Adjust { get { return _adjust; } set { _adjust = value; } }       


        // Constructors                                                                                                             
        public Tie(string name, string masterSurfaceName, RegionTypeEnum masterRegionType,
                   string slaveSurfaceName, RegionTypeEnum slaveRegionType)
           : this(name, double.NaN, true, masterSurfaceName, masterRegionType, slaveSurfaceName, slaveRegionType)
        {
        }
        public Tie(string name, double positionTolerance, bool adjust, string masterSurfaceName, RegionTypeEnum masterRegionType,
                   string slaveSurfaceName, RegionTypeEnum slaveRegionType)
            : base(name, masterSurfaceName, masterRegionType, slaveSurfaceName, slaveRegionType)
        {
            if (masterRegionType == RegionTypeEnum.SurfaceName && slaveRegionType == RegionTypeEnum.SurfaceName &&
                slaveSurfaceName == masterSurfaceName) throw new CaeException("Master and slave surface names must be different.");
            //
            PositionTolerance = positionTolerance;
            _adjust = adjust;
        }


        // Methods                                                                                                                  
        public void SwapMasterSlave()
        {
            if (_name.Contains(Globals.MasterSlaveSeparator))
            {
                string[] tmp = _name.Split(new string[] { Globals.MasterSlaveSeparator }, StringSplitOptions.None);
                if (tmp.Length == 2) _name = tmp[1] + Globals.MasterSlaveSeparator + tmp[0];
            }
            //
            RegionTypeEnum tmpRegionType = MasterRegionType;
            MasterRegionType = SlaveRegionType;
            SlaveRegionType = tmpRegionType;
            //
            string tmpSurfaceName = MasterRegionName;
            MasterRegionName = SlaveRegionName;
            SlaveRegionName = tmpSurfaceName;
            //
            int[] tmpCreationIds = MasterCreationIds;
            MasterCreationIds = SlaveCreationIds;
            SlaveCreationIds = tmpCreationIds;
            //
            Selection tmpCreationData = MasterCreationData;
            MasterCreationData = SlaveCreationData;
            SlaveCreationData = tmpCreationData;
        }
        

    }
}
