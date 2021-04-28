using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.ComponentModel;
using DynamicTypeDescriptor;
using System.Drawing.Design;

namespace PrePoMax.Forms
{
    [Serializable]
    public class ViewPartProperties
    {
        // Variables                                                                                                                      
        private PartProperties _partProperties;
        private DynamicCustomTypeDescriptor _dctd = null;


        // Properties                                                                                                               
        [Category("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [Description("Name of the part.")]
        [Id(1, 1)]
        public string Name { get { return _partProperties.Name; } set { _partProperties.Name = value; } }
        //
        [Category("Data")]
        [OrderedDisplayName(1, 10, "Part type")]
        [Description("Part type.")]
        [Id(2, 1)]
        public PartType PartType { get { return _partProperties.PartType; } }
        //
        [Category("Mesh")]
        [OrderedDisplayName(0, 10, "Number of elements")]
        [Description("Number of elements.")]
        [Id(1, 2)]
        public int NumberOfElements { get { return _partProperties.NumberOfElements; } }
        //
        [Category("Mesh")]
        [OrderedDisplayName(1, 10, "Number of nodes")]
        [Description("Number of nodes.")]
        [Id(2, 2)]
        public int NumberOfNodes { get { return _partProperties.NumberOfNodes; } }
        //
        [Category("Element type")]
        [OrderedDisplayName(0, 10, "Linear tria type")]
        [Description("Select the type of the linear triangular elements.")]
        [Id(1, 3)]
        public FeElementTypeLinearTria LinearTriaType
        {
            get { return _partProperties.LinearTriaType; }
            set { _partProperties.LinearTriaType = value; }
        }
        //
        [Category("Element type")]
        [OrderedDisplayName(1, 10, "Parabolic tria type")]
        [Description("Select the type of the parabolic triangular elements.")]
        [Id(2, 3)]
        public FeElementTypeParabolicTria ParabolicTriaType
        {
            get { return _partProperties.ParabolicTriaType; }
            set { _partProperties.ParabolicTriaType = value; }
        }
        //
        [Category("Element type")]
        [OrderedDisplayName(2, 10, "Linear quad type")]
        [Description("Select the type of the linear quadrilateral elements.")]
        [Id(3, 3)]
        public FeElementTypeLinearQuad LinearQuadType
        {
            get { return _partProperties.LinearQuadType; }
            set { _partProperties.LinearQuadType = value; }
        }
        //
        [Category("Element type")]
        [OrderedDisplayName(3, 10, "Parabolic quad type")]
        [Description("Select the type of the parabolic quadrilateral elements.")]
        [Id(4, 3)]
        public FeElementTypeParabolicQuad ParabolicQuadType
        {
            get { return _partProperties.ParabolicQuadType; }
            set { _partProperties.ParabolicQuadType = value; }
        }
        //
        [Category("Element type")]
        [OrderedDisplayName(4, 10, "Linear tetra type")]
        [Description("Select the type of the linear tetrahedron elements.")]
        [Id(5, 3)]
        public FeElementTypeLinearTetra LinearTetraType { get { return _partProperties.LinearTetraType; } set { _partProperties.LinearTetraType = value; } }
        //
        [Category("Element type")]
        [OrderedDisplayName(5, 10, "Parabolic tetra type")]
        [Description("Select the type of the parabolic tetrahedron elements.")]
        [Id(6, 3)]
        public FeElementTypeParabolicTetra ParabolicTetraType { get { return _partProperties.ParabolicTetraType; } set { _partProperties.ParabolicTetraType = value; } }
        //
        [Category("Element type")]
        [OrderedDisplayName(6, 10, "Linear wedge type")]
        [Description("Select the type of the linear wedge elements.")]
        [Id(7, 3)]
        public FeElementTypeLinearWedge LinearWedgeType { get { return _partProperties.LinearWedgeType; } set { _partProperties.LinearWedgeType = value; } }
        //
        [Category("Element type")]
        [OrderedDisplayName(7, 10, "Parabolic wedge type")]
        [Description("Select the type of the parabolic wedge elements.")]
        [Id(8, 3)]
        public FeElementTypeParabolicWedge ParabolicWedgeType { get { return _partProperties.ParabolicWedgeType; } set { _partProperties.ParabolicWedgeType = value; } }
        //
        [Category("Element type")]
        [OrderedDisplayName(8, 10, "Linear hexa type")]
        [Description("Select the type of the linear hexahedron elements.")]
        [Id(9, 3)]
        public FeElementTypeLinearHexa LinearHexaType { get { return _partProperties.LinearHexaType; } set { _partProperties.LinearHexaType = value; } }
        //
        [Category("Element type")]
        [OrderedDisplayName(9, 10, "Parabolic hexa type")]
        [Description("Select the type of the parabolic hexahedron elements.")]
        [Id(10, 3)]
        public FeElementTypeParabolicHexa ParabolicHexaType { get { return _partProperties.ParabolicHexaType; } set { _partProperties.ParabolicHexaType = value; } }
        //
        [Category("Appearance")]
        [OrderedDisplayName(0, 10, "Part color")]
        [Description("Select part color.")]
        [Editor(typeof(UserControls.ColorEditorEx), typeof(UITypeEditor))]
        [Id(1, 4)]
        public System.Drawing.Color Color
        {
            get { return _partProperties.Color; }
            set
            {
                _partProperties.Color = System.Drawing.Color.FromArgb(Math.Max((byte)25, value.A), value);
            }
        }


        // Constructors                                                                                                             
        public ViewPartProperties(PartProperties properties, ViewGeometryModelResults currentView)
        {
            _partProperties = properties;
            _dctd = ProviderInstaller.Install(this);
            _dctd.CategorySortOrder = CustomSortOrder.AscendingById;
            _dctd.PropertySortOrder = CustomSortOrder.AscendingById;
            //
            CustomPropertyDescriptor cpd = null;
            //
            if (currentView == ViewGeometryModelResults.Results)
            {
                cpd = _dctd.GetProperty(nameof(NumberOfElements));
                cpd.SetIsBrowsable(true);
                cpd = _dctd.GetProperty(nameof(NumberOfNodes));
                cpd.SetIsBrowsable(true);
                //
                cpd = _dctd.GetProperty(nameof(LinearTriaType));
                cpd.SetIsBrowsable(false);
                cpd = _dctd.GetProperty(nameof(ParabolicTriaType));
                cpd.SetIsBrowsable(false);
                cpd = _dctd.GetProperty(nameof(LinearQuadType));
                cpd.SetIsBrowsable(false);
                cpd = _dctd.GetProperty(nameof(ParabolicQuadType));
                cpd.SetIsBrowsable(false);
                //
                cpd = _dctd.GetProperty(nameof(LinearTetraType));
                cpd.SetIsBrowsable(false);
                cpd = _dctd.GetProperty(nameof(ParabolicTetraType));
                cpd.SetIsBrowsable(false);
                cpd = _dctd.GetProperty(nameof(LinearWedgeType));
                cpd.SetIsBrowsable(false);
                cpd = _dctd.GetProperty(nameof(ParabolicWedgeType));
                cpd.SetIsBrowsable(false);
                cpd = _dctd.GetProperty(nameof(LinearHexaType));
                cpd.SetIsBrowsable(false);
                cpd = _dctd.GetProperty(nameof(ParabolicHexaType));
                cpd.SetIsBrowsable(false);
            }
            else if (currentView == ViewGeometryModelResults.Geometry)
            {
                cpd = _dctd.GetProperty(nameof(NumberOfElements));
                cpd.SetIsBrowsable(false);
                cpd = _dctd.GetProperty(nameof(NumberOfNodes));
                cpd.SetIsBrowsable(false);
                //
                cpd = _dctd.GetProperty(nameof(LinearTriaType));
                cpd.SetIsBrowsable(false);
                cpd = _dctd.GetProperty(nameof(ParabolicTriaType));
                cpd.SetIsBrowsable(false);
                cpd = _dctd.GetProperty(nameof(LinearQuadType));
                cpd.SetIsBrowsable(false);
                cpd = _dctd.GetProperty(nameof(ParabolicQuadType));
                cpd.SetIsBrowsable(false);
                //
                cpd = _dctd.GetProperty(nameof(LinearTetraType));
                cpd.SetIsBrowsable(false);
                cpd = _dctd.GetProperty(nameof(ParabolicTetraType));
                cpd.SetIsBrowsable(false);
                cpd = _dctd.GetProperty(nameof(LinearWedgeType));
                cpd.SetIsBrowsable(false);
                cpd = _dctd.GetProperty(nameof(ParabolicWedgeType));
                cpd.SetIsBrowsable(false);
                cpd = _dctd.GetProperty(nameof(LinearHexaType));
                cpd.SetIsBrowsable(false);
                cpd = _dctd.GetProperty(nameof(ParabolicHexaType));
                cpd.SetIsBrowsable(false);
                //
                if (PartType == PartType.Compound)
                {
                    cpd = _dctd.GetProperty(nameof(Color));
                    cpd.SetIsBrowsable(false);
                }
            }
            else //currentView == ViewGeometryMeshResults.Mesh
            {
                cpd = _dctd.GetProperty(nameof(NumberOfElements));
                cpd.SetIsBrowsable(true);
                cpd = _dctd.GetProperty(nameof(NumberOfNodes));
                cpd.SetIsBrowsable(true);
                //
                cpd = _dctd.GetProperty(nameof(LinearTriaType));
                cpd.SetIsBrowsable(_partProperties.LinearTriaType != FeElementTypeLinearTria.None);
                cpd = _dctd.GetProperty(nameof(ParabolicTriaType));
                cpd.SetIsBrowsable(_partProperties.ParabolicTriaType != FeElementTypeParabolicTria.None);
                cpd = _dctd.GetProperty(nameof(LinearQuadType));
                cpd.SetIsBrowsable(_partProperties.LinearQuadType != FeElementTypeLinearQuad.None);
                cpd = _dctd.GetProperty(nameof(ParabolicQuadType));
                cpd.SetIsBrowsable(_partProperties.ParabolicQuadType != FeElementTypeParabolicQuad.None);
                //
                cpd = _dctd.GetProperty(nameof(LinearTetraType));
                cpd.SetIsBrowsable(_partProperties.LinearTetraType != FeElementTypeLinearTetra.None);
                cpd = _dctd.GetProperty(nameof(ParabolicTetraType));
                cpd.SetIsBrowsable(_partProperties.ParabolicTetraType != FeElementTypeParabolicTetra.None);
                cpd = _dctd.GetProperty(nameof(LinearWedgeType));
                cpd.SetIsBrowsable(_partProperties.LinearWedgeType != FeElementTypeLinearWedge.None);
                cpd = _dctd.GetProperty(nameof(ParabolicWedgeType));
                cpd.SetIsBrowsable(_partProperties.ParabolicWedgeType != FeElementTypeParabolicWedge.None);
                cpd = _dctd.GetProperty(nameof(LinearHexaType));
                cpd.SetIsBrowsable(_partProperties.LinearHexaType != FeElementTypeLinearHexa.None);
                cpd = _dctd.GetProperty(nameof(ParabolicHexaType));
                cpd.SetIsBrowsable(_partProperties.ParabolicHexaType != FeElementTypeParabolicHexa.None);
            }
        }


        // Methods                                                                                                                  
        public PartProperties GetBase()
        {
            return _partProperties;
        }
       

    }
}
