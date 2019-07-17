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
        CustomPropertyDescriptor cpd = null;
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

        [CategoryAttribute("\tSize")]
        [OrderedDisplayName(2, 10, "Fineness")]
        [DescriptionAttribute("The value of the mesh fineness (0 => coarse; 1 => fine).")]
        public double Fineness { get { return _parameters.Fineness; } set { _parameters.Fineness = value; } }

        [CategoryAttribute("\tSize")]
        [OrderedDisplayName(3, 10, "Grading")]
        [DescriptionAttribute("The value of the mesh grading (0 => uniform mesh; 1 => aggressive local grading).")]
        public double Grading { get { return _parameters.Grading; } set { _parameters.Grading = value; } }

        [CategoryAttribute("\tSize")]
        [OrderedDisplayName(4, 10, "Elements per edge")]
        [DescriptionAttribute("Number of elements to generate per edge of the geometry.")]
        public double Elementsperedge { get { return _parameters.Elementsperedge; } set { _parameters.Elementsperedge = value; } }

        [CategoryAttribute("\tSize")]
        [OrderedDisplayName(5, 10, "Elements per curve")]
        [DescriptionAttribute("Number of elements to generate per curvature radius.")]
        public double Elementspercurve { get { return _parameters.Elementspercurve; } set { _parameters.Elementspercurve = value; } }

        [CategoryAttribute("Mesh optimization")]
        [OrderedDisplayName(0, 10, "Optimize steps 2D")]
        [DescriptionAttribute("Number of optimize steps to use for 2-D mesh optimization.")]
        public int OptimizeSteps2D { get { return _parameters.OptimizeSteps2D; } set { _parameters.OptimizeSteps2D = value; } }


        [CategoryAttribute("Mesh optimization")]
        [OrderedDisplayName(1, 10, "Optimize steps 3D")]
        [DescriptionAttribute("Number of optimize steps to use for 3-D mesh optimization.")]
        public int OptimizeSteps3D { get { return _parameters.OptimizeSteps3D; } set { _parameters.OptimizeSteps3D = value; } }

        [CategoryAttribute("Type")]
        [OrderedDisplayName(0, 10, "Second order")]
        [DescriptionAttribute("Create second order elements.")]
        public bool SecondOrder
        {
            get{return _parameters.SecondOrder;}
            set
            {
                _parameters.SecondOrder = value;

                cpd = _dctd.GetProperty("MidsideNodesOnGeometry");
                cpd.SetIsBrowsable(value);
            }
        }

        [CategoryAttribute("Type")]
        [OrderedDisplayName(1, 10, "Midside nodes on geometry")]
        [DescriptionAttribute("Create midside nodes on geometry.")]
        public bool MidsideNodesOnGeometry { get { return _parameters.MidsideNodesOnGeometry; } set { _parameters.MidsideNodesOnGeometry = value; } }


        // Constructors                                                                                                             
        public ViewMeshingParameters(MeshingParameters parameters)
        {
            _parameters = parameters;
            _dctd = ProviderInstaller.Install(this);

            SecondOrder = parameters.SecondOrder;       // to show/hide the MediumNodesOnGeometry property

            // now lets display Yes/No instead of True/False
            cpd = _dctd.GetProperty("SecondOrder");
            foreach (StandardValueAttribute sva in cpd.StatandardValues)
            {
                if ((bool)sva.Value == true) sva.DisplayName = "Yes";
                else sva.DisplayName = "No";
            }

            // now lets display Yes/No instead of True/False
            cpd = _dctd.GetProperty("MidsideNodesOnGeometry");
            foreach (StandardValueAttribute sva in cpd.StatandardValues)
            {
                if ((bool)sva.Value == true) sva.DisplayName = "Yes";
                else sva.DisplayName = "No";
            }
        }


        // Methods                                                                                                                  
        public MeshingParameters GetBase()
        {
            return _parameters;
        }
    }
}
