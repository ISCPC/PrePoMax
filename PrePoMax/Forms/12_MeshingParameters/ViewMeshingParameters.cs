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
    public class ViewMeshingParameters
    {
        // Variables                                                                                                                
        private MeshingParameters _parameters;
        private DynamicCustomTypeDescriptor _dctd = null;


        // Properties                                                                                                               
        [CategoryAttribute("\tSize")]
        [OrderedDisplayName(0, 10, "Max element size")]
        [DescriptionAttribute("The value for the maximum element size.")]
        public double MaxH { get { return _parameters.MaxH; } set { _parameters.MaxH = value; } }

        [CategoryAttribute("\tSize")]
        [OrderedDisplayName(1, 10, "Min element size")]
        [DescriptionAttribute("The value for the minimum element size.")]
        public double MinH { get { return _parameters.MinH; } set { _parameters.MinH = value; } }

        [CategoryAttribute("Mesh optimization")]
        [OrderedDisplayName(2, 10, "Optimize steps 2D")]
        [DescriptionAttribute("Number of optimize steps to use for 2-D mesh optimization.")]
        public int OptimizeSteps2D { get { return _parameters.OptimizeSteps2D; } set { _parameters.OptimizeSteps2D = value; } }

        [CategoryAttribute("Mesh optimization")]
        [OrderedDisplayName(3, 10, "Optimize steps 3D")]
        [DescriptionAttribute("Number of optimize steps to use for 3-D mesh optimization.")]
        public int OptimizeSteps3D { get { return _parameters.OptimizeSteps3D; } set { _parameters.OptimizeSteps3D = value; } }

        [CategoryAttribute("Type")]
        [OrderedDisplayName(4, 10, "Second order")]
        [DescriptionAttribute("Create second order elements.")]
        public bool SecondOrder { get { return _parameters.SecondOrder; } set { _parameters.SecondOrder = value; } }


        // Constructors                                                                                                             
        public ViewMeshingParameters(MeshingParameters parameters)
        {
            _parameters = parameters;
            _dctd = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public MeshingParameters GetBase()
        {
            return _parameters;
        }
    }
}
