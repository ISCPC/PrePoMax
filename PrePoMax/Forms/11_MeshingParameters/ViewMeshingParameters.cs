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
        private CustomPropertyDescriptor cpd = null;
        private DynamicCustomTypeDescriptor _dctd = null;


        // Properties                                                                                                               
        [CategoryAttribute("Mesh size")]
        [OrderedDisplayNameAttribute(1, 10, "Max element size")]
        [DescriptionAttribute("The value for the maximum element size.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(1, 1)]
        public double MaxH { get { return _parameters.MaxH; } set { _parameters.MaxH = value; } }
        //
        [CategoryAttribute("Mesh size")]
        [OrderedDisplayNameAttribute(2, 10, "Min element size")]
        [DescriptionAttribute("The value for the minimum element size.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(1, 1)]
        public double MinH { get { return _parameters.MinH; } set { _parameters.MinH = value; } }
        //
        [CategoryAttribute("Mesh size")]
        [OrderedDisplayNameAttribute(3, 10, "Grading")]
        [DescriptionAttribute("The value of the mesh grading (0 => uniform mesh; 1 => aggressive local grading).")]
        [Id(1, 1)]
        public double Grading { get { return _parameters.Grading; } set { _parameters.Grading = value; } }
        //
        [CategoryAttribute("Mesh size")]
        [OrderedDisplayNameAttribute(4, 10, "Elements per edge")]
        [DescriptionAttribute("Number of elements to generate per edge of the geometry.")]
        [Id(1, 1)]
        public double Elementsperedge { get { return _parameters.Elementsperedge; } set { _parameters.Elementsperedge = value; } }
        //
        [CategoryAttribute("Mesh size")]
        [OrderedDisplayNameAttribute(5, 10, "Elements per curvature")]
        [DescriptionAttribute("Number of elements to generate per curvature radius.")]
        [Id(1, 1)]
        public double Elementspercurve { get { return _parameters.Elementspercurve; } set { _parameters.Elementspercurve = value; } }
        //
        //
        [CategoryAttribute("Mesh optimization")]
        [OrderedDisplayNameAttribute(1, 10, "Optimize steps 2D")]
        [DescriptionAttribute("Number of optimize steps to use for 2-D mesh optimization.")]
        [Id(1, 2)]
        public int OptimizeSteps2D { get { return _parameters.OptimizeSteps2D; } set { _parameters.OptimizeSteps2D = value; } }
        //
        [CategoryAttribute("Mesh optimization")]
        [OrderedDisplayNameAttribute(2, 10, "Optimize steps 3D")]
        [DescriptionAttribute("Number of optimize steps to use for 3-D mesh optimization.")]
        [Id(1, 2)]
        public int OptimizeSteps3D { get { return _parameters.OptimizeSteps3D; } set { _parameters.OptimizeSteps3D = value; } }
        //
        //
        [CategoryAttribute("Mesh type")]
        [OrderedDisplayNameAttribute(1, 10, "Second order")]
        [DescriptionAttribute("Create second order elements.")]
        [Id(1, 3)]
        public bool SecondOrder
        {
            get{return _parameters.SecondOrder;}
            set
            {
                _parameters.SecondOrder = value;
                //
                cpd = _dctd.GetProperty("MidsideNodesOnGeometry");
                cpd.SetIsBrowsable(value);
                _dctd.PropertySortOrder = CustomSortOrder.AscendingById;
            }
        }
        //
        [CategoryAttribute("Mesh type")]
        [OrderedDisplayNameAttribute(2, 10, "Midside nodes on geometry")]
        [DescriptionAttribute("Create midside nodes on geometry.")]
        [Id(1, 3)]
        public bool MidsideNodesOnGeometry { get { return _parameters.MidsideNodesOnGeometry; } set { _parameters.MidsideNodesOnGeometry = value; } }
        //
        //
        [CategoryAttribute("Mesh operations")]
        [OrderedDisplayNameAttribute(1, 10, "Split compound mesh")]
        [DescriptionAttribute("Split compound part mesh to unconnected part meshes.")]
        [Id(1, 4)]
        public bool SplitCompoundMesh { get { return _parameters.SplitCompoundMesh; } set { _parameters.SplitCompoundMesh = value; } }


        // Constructors                                                                                                             
        public ViewMeshingParameters(MeshingParameters parameters, string lengthUnit)
        {
            _parameters = parameters;
            _dctd = ProviderInstaller.Install(this);
            // Category sorting
            _dctd.CategorySortOrder = CustomSortOrder.AscendingById;
            _dctd.PropertySortOrder = CustomSortOrder.AscendingById;    // seems not to work
            // To show/hide the MediumNodesOnGeometry property
            SecondOrder = parameters.SecondOrder;
            // Now lets display Yes/No instead of True/False
            _dctd.RenameBooleanPropertyToYesNo("SecondOrder");
            _dctd.RenameBooleanPropertyToYesNo("MidsideNodesOnGeometry");
            _dctd.RenameBooleanPropertyToYesNo("SplitCompoundMesh");
            //
            StringLengthConverter.SetUnit = lengthUnit;
        }


        // Methods                                                                                                                  
        public MeshingParameters GetBase()
        {
            return _parameters;
        }
    }
}
