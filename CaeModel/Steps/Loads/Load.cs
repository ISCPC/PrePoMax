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
    public abstract class Load : NamedClass, IMultiRegion
    {
        // Variables                                                                                                                
        private int[] _creationIds;
        private Selection _creationData;


        // Properties                                                                                                               
        public virtual string RegionName { get; set; }
        public virtual RegionTypeEnum RegionType { get; set; }
        public int[] CreationIds { get { return _creationIds; } set { _creationIds = value; } }
        public Selection CreationData { get { return _creationData; } set { _creationData = value; } }


        // Constructors                                                                                                             
        public Load(string name)
            : base(name) 
        {
            _creationIds = null;
            _creationData = null;
        }


        // Methods                                                                                                                  
    }
}
