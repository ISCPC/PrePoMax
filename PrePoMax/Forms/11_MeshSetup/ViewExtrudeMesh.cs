using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.ComponentModel;
using DynamicTypeDescriptor;
using CaeMesh.Meshing;

namespace PrePoMax.Forms
{
    [Serializable]
    public class ViewExtrudeMesh : ViewMeshSetupItem
    {
        // Variables                                                                                                                
        private ExtrudeMesh _extrudeMesh;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the extrude mesh item.")]
        public override string Name { get { return _extrudeMesh.Name; } set { _extrudeMesh.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "2D meshing algorithm")]
        [DescriptionAttribute("Select the algorithm for meshing the surfaces before extrusion.")]
        public GmshAlgorithmMesh2DEnum AlgorithmMesh2D
        {
            get { return _extrudeMesh.AlgorithmMesh2D; }
            set { _extrudeMesh.AlgorithmMesh2D = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Recombine algorithm")]
        [DescriptionAttribute("Select the algorithm for recombination of triangles into quads.")]
        public GmshAlgorithmRecombineEnum AlgorithmRecombine
        {
            get { return _extrudeMesh.AlgorithmRecombine; }
            set { _extrudeMesh.AlgorithmRecombine = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(3, 10, "Number of layers")]
        [DescriptionAttribute("Enter the number of layers of the extruded mesh.")]
        public int NumberOfLayers { get { return _extrudeMesh.NumberOfLayers; } set { _extrudeMesh.NumberOfLayers = value; } }


        // Constructors                                                                                                             
        public ViewExtrudeMesh(ExtrudeMesh extrudeMesh)
        {
            _extrudeMesh = extrudeMesh;                             // 1 command
            _dctd = ProviderInstaller.Install(this);                // 2 command
        }


        // Methods                                                                                                                  
        public override MeshSetupItem GetBase()
        {
            return _extrudeMesh;
        }
    }
}
