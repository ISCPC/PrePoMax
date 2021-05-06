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
    public class BoundaryCondition : NamedClass, IMultiRegion
    {
        // Variables                                                                                                                
        private string _regionName;
        private RegionTypeEnum _regionType;
        private int[] _creationIds;
        private Selection _creationData;
        protected Color _color;


        // Properties                                                                                                               
        public string RegionName { get { return _regionName; } set { _regionName = value; } }
        public RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public int[] CreationIds { get { return _creationIds; } set { _creationIds = value; } }
        public Selection CreationData { get { return _creationData; } set { _creationData = value; } }
        public Color Color
        {
            get
            {
                // Compatibility for version v0.6.0
                if (_color == Color.Empty) _color = Color.Lime;
                //
                return _color;
            }
            set { _color = value; }
        }


        // Constructors                                                                                                             
        public BoundaryCondition(string name, string regionName, RegionTypeEnum regionType)
            : base(name) 
        {
            _regionName = regionName;
            _regionType = regionType;
            _creationIds = null;
            _creationData = null;
            _color = Color.Lime;
        }


        // Methods                                                                                                                  
    }
}
