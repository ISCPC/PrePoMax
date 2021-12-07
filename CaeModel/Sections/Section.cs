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
    public abstract class Section : NamedClass, IMultiRegion
    {
        // Variables                                                                                                                
        private string _materialName;
        private RegionTypeEnum _regionType;
        private string _regionName;
        private Selection _creationData;
        private int[] _creationIds;
        private bool _twoD;


        // Properties                                                                                                               
        public string MaterialName { get { return _materialName; } set { _materialName = value; } }
        public RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public string RegionName { get { return _regionName; } set { _regionName = value; } }
        public int[] CreationIds { get { return _creationIds; } set { _creationIds = value; } }
        public Selection CreationData { get { return _creationData; } set { _creationData = value; } }
        public bool TwoD { get { return _twoD; } }


        // Constructors                                                                                                             
        public Section(string name, string materialName, string regionName, RegionTypeEnum regionType, bool twoD) 
            : base(name)
        {
            _materialName = materialName;
            _regionName = regionName;
            _regionType = regionType;
            _creationIds = null;
            _creationData = null;
            _twoD = twoD;
        }
    }

}
