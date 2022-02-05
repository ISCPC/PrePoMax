using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.ComponentModel;
using DynamicTypeDescriptor;

namespace PrePoMax.Forms
{
    [Serializable]
    public class ViewFeMeshRefinement
    {
        // Variables                                                                                                                
        private FeMeshRefinement _meshRefinement;
        private DynamicCustomTypeDescriptor _dctd = null;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the mesh refinement.")]
        public string Name { get { return _meshRefinement.Name; } set { _meshRefinement.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Element size")]
        [DescriptionAttribute("Element size can only be used to reduce the local element size, but the global limit of " +
                              "the minimum element size is kept.")]
        [TypeConverter(typeof(StringLengthConverter))]
        public double MeshSize { get { return _meshRefinement.MeshSize; } set { _meshRefinement.MeshSize = value; } }


        // Constructors                                                                                                             
        public ViewFeMeshRefinement(FeMeshRefinement meshRefinement)
        {
            _meshRefinement = meshRefinement;                               // 1 command
            _dctd = ProviderInstaller.Install(this);                        // 2 command
        }


        // Methods                                                                                                                  
        public FeMeshRefinement GetBase()
        {
            return _meshRefinement;
        }
    }
}
