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
    public class RigidBody : Constraint
    {
        // Variables                                                                                                                


        // Properties                                                                                                               
        public string ReferencePointName { get { return MasterRegionName; } set { MasterRegionName = value; } }
        public string RegionName { get { return SlaveRegionName; } set { SlaveRegionName = value; } }
        public RegionTypeEnum RegionType { get { return SlaveRegionType; } set { SlaveRegionType = value; } }
        //
        public int[] CreationIds { get { return SlaveCreationIds; } set { SlaveCreationIds = value; } }
        public Selection CreationData { get { return SlaveCreationData; } set { SlaveCreationData = value; } }


        // Constructors                                                                                                             
        public RigidBody(string name, string referencePointName, string regionName, RegionTypeEnum regionType)
            : base(name, referencePointName, RegionTypeEnum.ReferencePointName, regionName, regionType)
        {
        }


        // Methods                                                                                                                  
    }
}
