using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
    [Serializable]
    public class RemeshingParameters : MeshingParameters, IMultiRegion
    {
        // Variables                                                                                                                
        private RegionTypeEnum _regionType;
        string _regionName;
        private Selection _creationData;
        private int[] _creationIds;


        // Properties                                                                                                               
        public RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public string RegionName { get { return _regionName; } set { _regionName = value; } }
        public Selection CreationData { get { return _creationData; } set { _creationData = value; } }
        public int[] CreationIds { get { return _creationIds; } set { _creationIds = value; } }


        // Constructors                                                                                                             
        public RemeshingParameters(string regionName, RegionTypeEnum regionType)
        {
            UseMmg = true;
            //
            _regionName = regionName;
            _regionType = regionType;
            _creationData = null;
            _creationIds = null;
        }
        public RemeshingParameters(string regionName, RegionTypeEnum regionType, MeshingParameters meshingParameters)
            : base(meshingParameters)
        {
            UseMmg = true;
            //
            _regionName = regionName;
            _regionType = regionType;
            _creationData = null;
            _creationIds = null;
        }


        // Methods                                                                                                                  
    }
}
