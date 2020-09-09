using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using DynamicTypeDescriptor;

namespace CaeMesh
{
    [Serializable]
    public class GeometrySelection : NamedClass
    {
        // Variables                                                                                                                
        private int[] _geometryIds;
        private Selection _creationData;
        

        // Properties                                                                                                               
        public int[] GeometryIds { get { return _geometryIds; } set { _geometryIds = value; } }
        public Selection CreationData { get { return _creationData; } set { _creationData = value; } }


        // Constructors                                                                                                             
        public GeometrySelection(string name)
            :base(name)
        {
            Clear();
        }
        public GeometrySelection(string name, int[] geometryIds, Selection creationDataClone)
           : this(name)
        {
            _geometryIds = geometryIds;
            _creationData = creationDataClone;
        }
        public GeometrySelection(GeometrySelection geometrySelection)
           : this(geometrySelection.Name)
        {
            _geometryIds = geometrySelection.GeometryIds != null ? geometrySelection.GeometryIds.ToArray() : null;
            _creationData = geometrySelection.CreationData != null ? geometrySelection.CreationData.DeepClone() : null;
        }


        // Methods                                                                                                                  
        public void Clear()
        {
            _geometryIds = null;
            _creationData = null;
        }
    }
}
