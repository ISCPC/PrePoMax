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
    public class FeMeshRefinement : NamedClass
    {
        // Variables                                                                                                                
        private int[] _geometryIds;
        private Selection _creationData;
        private double _meshSize;
        

        // Properties                                                                                                               
        public int[] GeometryIds { get { return _geometryIds; } set { _geometryIds = value; } }
        public Selection CreationData { get { return _creationData; } set { _creationData = value; } }
        public double MeshSize 
        {
            get { return _meshSize; } 
            set
            {
                _meshSize = value;
                if (_meshSize < 1E-10) _meshSize = 1E-10;
            } 
        }


        // Constructors                                                                                                             
        public FeMeshRefinement(string name)
            :base(name)
        {
            Clear();
        }
        public FeMeshRefinement(string name, int[] geometryIds, Selection creationDataClone)
           : this(name)
        {
            _geometryIds = geometryIds;
            _creationData = creationDataClone;
        }
        public FeMeshRefinement(FeMeshRefinement meshRefinement)
           : this(meshRefinement.Name)
        {
            _geometryIds = meshRefinement.GeometryIds != null ? meshRefinement.GeometryIds.ToArray() : null;
            _creationData = meshRefinement.CreationData != null ? meshRefinement.CreationData.DeepClone() : null;
            _meshSize = meshRefinement.MeshSize;
        }


        // Methods                                                                                                                  
        private void Clear()
        {
            _geometryIds = null;
            _creationData = null;
            _meshSize = 1;
        }
    }
}
