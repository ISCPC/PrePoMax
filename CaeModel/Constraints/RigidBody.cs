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
    public class RigidBody : Constraint, IMultiRegion
    {
        // Variables                                                                                                                
        private string _referencePointName;
        private RegionTypeEnum _regionType;
        private string _regionName;


        // Properties                                                                                                               
        public string RegionName { get { return _regionName; } set { _regionName = value; } }
        public RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public string ReferencePointName { get { return _referencePointName; } set { _referencePointName = value; } }


        // Constructors                                                                                                             
        public RigidBody(string name, string referencePointName, string regionName, RegionTypeEnum regionType)
            : base(name)
        {
            _referencePointName = referencePointName;
            _regionName = regionName;
            _regionType = regionType;
        }


        // Methods                                                                                                                  
    }
}
