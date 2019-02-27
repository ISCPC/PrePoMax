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


        // Properties                                                                                                               
        public string MaterialName { get { return _materialName; } set { _materialName = value; } }
        public string RegionName { get { return _regionName; } set { _regionName = value; } }
        public RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        


        // Constructors                                                                                                             
        public Section(string name, string materialName, string regionName, RegionTypeEnum regionType) 
            : base(name)
        {
            _materialName = materialName;
            _regionName = regionName;
            _regionType = regionType;
        }
    }

}
