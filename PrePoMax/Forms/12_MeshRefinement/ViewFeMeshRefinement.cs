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
        [OrderedDisplayName(1, 10, "Mesh size")]
        [DescriptionAttribute("Local size of the mesh.")]
        [TypeConverter(typeof(CaeModel.StringLengthConverter))]
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
