using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public class MeshPart : BasePart
    {
        // Variables                                                                                                                
        protected FeElementTypeLinearTria _linearTriaType;
        protected FeElementTypeParabolicTria _parabolicTriaType;
        protected FeElementTypeLinearQuad _linearQuadType;
        protected FeElementTypeParabolicQuad _parabolicQuadType;
        //
        protected FeElementTypeLinearTetra _linearTetraType;
        protected FeElementTypeParabolicTetra _parabolicTetraType;
        protected FeElementTypeLinearWedge _linearWedgeType;
        protected FeElementTypeParabolicWedge _parabolicWedgeType;
        protected FeElementTypeLinearHexa _linearHexaType;
        protected FeElementTypeParabolicHexa _parabolicHexaType;


        // Properties                                                                                                               
        public FeElementTypeLinearTria LinearTriaType { get { return _linearTriaType; } set { _linearTriaType = value; } }
        public FeElementTypeParabolicTria ParabolicTriaType { get { return _parabolicTriaType; } set { _parabolicTriaType = value; } }
        public FeElementTypeLinearQuad LinearQuadType { get { return _linearQuadType; } set { _linearQuadType = value; } }
        public FeElementTypeParabolicQuad ParabolicQuadType { get { return _parabolicQuadType; } set { _parabolicQuadType = value; } }
        //
        public FeElementTypeLinearTetra LinearTetraType { get { return _linearTetraType; } set { _linearTetraType = value; } }
        public FeElementTypeParabolicTetra ParabolicTetraType { get { return _parabolicTetraType; } set { _parabolicTetraType = value; } }
        public FeElementTypeLinearWedge LinearWedgeType { get { return _linearWedgeType; } set { _linearWedgeType = value; } }
        public FeElementTypeParabolicWedge ParabolicWedgeType { get { return _parabolicWedgeType; } set { _parabolicWedgeType = value; } }
        public FeElementTypeLinearHexa LinearHexaType { get { return _linearHexaType; } set { _linearHexaType = value; } }
        public FeElementTypeParabolicHexa ParabolicHexaType { get { return _parabolicHexaType; } set { _parabolicHexaType = value; } }


        // Constructors                                                                                                             
        public MeshPart(string name, int partId, int[] nodeLabels, int[] elementLabels, Type[] elementTypes)
            : base(name, partId, nodeLabels, elementLabels, elementTypes)
        {
            InitializeElementTypes();
        }
        public MeshPart(BasePart part)
            : base(part)
        {
            InitializeElementTypes();
        }
        public MeshPart(MeshPart part)
            : base(part)
        {
            _linearTriaType = part.LinearTriaType;
            _parabolicTriaType = part.ParabolicTriaType;
            _linearQuadType = part.LinearQuadType;
            _parabolicQuadType = part.ParabolicQuadType;
            //
            _linearTetraType = part.LinearTetraType;
            _parabolicTetraType = part.ParabolicTetraType;
            _linearWedgeType = part.LinearWedgeType;
            _parabolicWedgeType = part.ParabolicWedgeType;
            _linearHexaType = part.LinearHexaType;
            _parabolicHexaType = part.ParabolicHexaType;
        }


        // Methods                                                                                                                  
        private void InitializeElementTypes()
        {
            _linearTriaType = FeElementTypeLinearTria.None;
            _parabolicTriaType = FeElementTypeParabolicTria.None;
            _linearQuadType = FeElementTypeLinearQuad.None;
            _parabolicQuadType = FeElementTypeParabolicQuad.None;
            //
            _linearTetraType = FeElementTypeLinearTetra.None;
            _parabolicTetraType = FeElementTypeParabolicTetra.None;
            _linearWedgeType = FeElementTypeLinearWedge.None;
            _parabolicWedgeType = FeElementTypeParabolicWedge.None;
            _linearHexaType = FeElementTypeLinearHexa.None;
            _parabolicHexaType = FeElementTypeParabolicHexa.None;
            //
            HashSet<Type> types = new HashSet<Type>(_elementTypes);
            if (types.Contains(typeof(LinearTriangleElement))) _linearTriaType = FeElementTypeLinearTria.S3;
            if (types.Contains(typeof(ParabolicTriangleElement))) _parabolicTriaType = FeElementTypeParabolicTria.S6;
            if (types.Contains(typeof(LinearQuadrilateralElement))) _linearQuadType = FeElementTypeLinearQuad.S4;
            if (types.Contains(typeof(ParabolicQuadrilateralElement))) _parabolicQuadType = FeElementTypeParabolicQuad.S8;
            //
            if (types.Contains(typeof(LinearTetraElement))) _linearTetraType = FeElementTypeLinearTetra.C3D4;
            if (types.Contains(typeof(ParabolicTetraElement))) _parabolicTetraType = FeElementTypeParabolicTetra.C3D10;
            if (types.Contains(typeof(LinearWedgeElement))) _linearWedgeType = FeElementTypeLinearWedge.C3D6;
            if (types.Contains(typeof(ParabolicWedgeElement))) _parabolicWedgeType = FeElementTypeParabolicWedge.C3D15;
            if (types.Contains(typeof(LinearHexaElement))) _linearHexaType = FeElementTypeLinearHexa.C3D8;
            if (types.Contains(typeof(ParabolicHexaElement))) _parabolicHexaType = FeElementTypeParabolicHexa.C3D20;
        }
        public void AddElementTypes(Type[] elementTypes)
        {
            HashSet<Type> types = new HashSet<Type>(elementTypes);
            //
            if (types.Contains(typeof(LinearTriangleElement)) && _linearTriaType == FeElementTypeLinearTria.None)
                _linearTriaType = FeElementTypeLinearTria.S3;
            if (types.Contains(typeof(ParabolicTriangleElement)) && _parabolicTriaType == FeElementTypeParabolicTria.None)
                _parabolicTriaType = FeElementTypeParabolicTria.S6;
            if (types.Contains(typeof(LinearQuadrilateralElement)) && _linearQuadType == FeElementTypeLinearQuad.None)
                _linearQuadType = FeElementTypeLinearQuad.S4;
            if (types.Contains(typeof(ParabolicQuadrilateralElement)) && _parabolicQuadType == FeElementTypeParabolicQuad.None)
                _parabolicQuadType = FeElementTypeParabolicQuad.S8;
            //
            if (types.Contains(typeof(LinearTetraElement)) && _linearTetraType == FeElementTypeLinearTetra.None)
                _linearTetraType = FeElementTypeLinearTetra.C3D4;
            if (types.Contains(typeof(ParabolicTetraElement)) && _parabolicTetraType == FeElementTypeParabolicTetra.None)
                _parabolicTetraType = FeElementTypeParabolicTetra.C3D10;
            if (types.Contains(typeof(LinearWedgeElement)) && _linearWedgeType == FeElementTypeLinearWedge.None)
                _linearWedgeType = FeElementTypeLinearWedge.C3D6;
            if (types.Contains(typeof(ParabolicWedgeElement)) && _parabolicWedgeType == FeElementTypeParabolicWedge.None)
                _parabolicWedgeType = FeElementTypeParabolicWedge.C3D15;
            if (types.Contains(typeof(LinearHexaElement)) && _linearHexaType == FeElementTypeLinearHexa.None)
                _linearHexaType = FeElementTypeLinearHexa.C3D8;
            if (types.Contains(typeof(ParabolicHexaElement)) && _parabolicHexaType == FeElementTypeParabolicHexa.None)
                _parabolicHexaType = FeElementTypeParabolicHexa.C3D20;
            //
            HashSet<Type> allTypes = new HashSet<Type>();
            if (_elementTypes != null) allTypes.UnionWith(_elementTypes);
            if (elementTypes != null) allTypes.UnionWith(elementTypes);
            _elementTypes = allTypes.ToArray();
        }
        public void CopyActiveElementTypesFrom(MeshPart part)
        {
            if (_linearTriaType != FeElementTypeLinearTria.None) _linearTriaType = part.LinearTriaType;
            if (_parabolicTriaType != FeElementTypeParabolicTria.None) _parabolicTriaType = part.ParabolicTriaType;
            if (_linearQuadType != FeElementTypeLinearQuad.None) _linearQuadType = part.LinearQuadType;
            if (_parabolicQuadType != FeElementTypeParabolicQuad.None) _parabolicQuadType = part.ParabolicQuadType;
            //
            if (_linearTetraType != FeElementTypeLinearTetra.None) _linearTetraType = part.LinearTetraType;
            if (_parabolicTetraType != FeElementTypeParabolicTetra.None) _parabolicTetraType = part.ParabolicTetraType;
            if (_linearWedgeType != FeElementTypeLinearWedge.None) _linearWedgeType = part.LinearWedgeType;
            if (_parabolicWedgeType != FeElementTypeParabolicWedge.None) _parabolicWedgeType = part.ParabolicWedgeType;
            if (_linearHexaType != FeElementTypeLinearHexa.None) _linearHexaType = part.LinearHexaType;
            if (_parabolicHexaType != FeElementTypeParabolicHexa.None) _parabolicHexaType = part._parabolicHexaType;
        }
        public void SetPropertiesFromInpElementTypeName(string[] inpElementTypeNames)
        {
            foreach (var inpElementTypeName in inpElementTypeNames)
            {
                switch (inpElementTypeName)
                {
                    // Triangular
                    case "S3":
                        _linearTriaType = FeElementTypeLinearTria.S3;
                        break;
                    case "STRI65":
                        _parabolicTriaType = FeElementTypeParabolicTria.S6;
                        break;
                    // Quadrilateral
                    case "S4":
                        _linearQuadType = FeElementTypeLinearQuad.S4;
                        break;
                    case "S4R":
                        _linearQuadType = FeElementTypeLinearQuad.S4R;
                        break;
                    case "S8R":
                        _parabolicQuadType = FeElementTypeParabolicQuad.S8R;
                        break;
                    // Tetrahedral
                    case "C3D4":
                        _linearTetraType = FeElementTypeLinearTetra.C3D4;
                        break;
                    case "C3D10":
                        _parabolicTetraType = FeElementTypeParabolicTetra.C3D10;
                        break;
                    // Wedge
                    case "C3D6":
                        _linearWedgeType = FeElementTypeLinearWedge.C3D6;
                        break;
                    case "C3D15":
                        _parabolicWedgeType = FeElementTypeParabolicWedge.C3D15;
                        break;
                    // Hexahedral
                    case "C3D8":
                        _linearHexaType = FeElementTypeLinearHexa.C3D8;
                        break;
                    case "C3D8R":
                        _linearHexaType = FeElementTypeLinearHexa.C3D8R;
                        break;
                    case "C3D8I":
                        _linearHexaType = FeElementTypeLinearHexa.C3D8I;
                        break;
                    case "C3D20":
                        _parabolicHexaType = FeElementTypeParabolicHexa.C3D20;
                        break;
                    case "C3D20R":
                        _parabolicHexaType = FeElementTypeParabolicHexa.C3D20R;
                        break;
                    default:
                        throw new CaeGlobals.CaeException("The inp element type '" + inpElementTypeName + "' is not supported.");
                }
            }
        }
        public override BasePart DeepCopy()
        {
            return new MeshPart(this);
        }
        public override PartProperties GetProperties()
        {
            PartProperties properties = base.GetProperties();
            //
            properties.LinearTriaType = _linearTriaType;
            properties.ParabolicTriaType = _parabolicTriaType;
            properties.LinearQuadType = _linearQuadType;
            properties.ParabolicQuadType = _parabolicQuadType;
            //
            properties.LinearTetraType = _linearTetraType;
            properties.ParabolicTetraType = _parabolicTetraType;
            properties.LinearWedgeType = _linearWedgeType;
            properties.ParabolicWedgeType = _parabolicWedgeType;
            properties.LinearHexaType = _linearHexaType;
            properties.ParabolicHexaType = _parabolicHexaType;

            return properties;
        }
        public override void SetProperties(PartProperties properties)
        {
            base.SetProperties(properties);
            //
            if (_linearTriaType != FeElementTypeLinearTria.None && properties.LinearTriaType != FeElementTypeLinearTria.None)
                _linearTriaType = properties.LinearTriaType;
            if (_parabolicTriaType != FeElementTypeParabolicTria.None && properties.ParabolicTriaType != FeElementTypeParabolicTria.None)
                _parabolicTriaType = properties.ParabolicTriaType;
            if (_linearQuadType != FeElementTypeLinearQuad.None && properties.LinearQuadType != FeElementTypeLinearQuad.None)
                _linearQuadType = properties.LinearQuadType;
            if (_parabolicQuadType != FeElementTypeParabolicQuad.None && properties.ParabolicQuadType != FeElementTypeParabolicQuad.None)
                _parabolicQuadType = properties.ParabolicQuadType;
            //
            if (_linearTetraType != FeElementTypeLinearTetra.None && properties.LinearTetraType != FeElementTypeLinearTetra.None)
                _linearTetraType = properties.LinearTetraType;
            if (_parabolicTetraType != FeElementTypeParabolicTetra.None && properties.ParabolicTetraType != FeElementTypeParabolicTetra.None)
                _parabolicTetraType = properties.ParabolicTetraType;
            if (_linearWedgeType != FeElementTypeLinearWedge.None && properties.LinearWedgeType != FeElementTypeLinearWedge.None)
                _linearWedgeType = properties.LinearWedgeType;
            if (_parabolicWedgeType != FeElementTypeParabolicWedge.None && properties.ParabolicWedgeType != FeElementTypeParabolicWedge.None)
                _parabolicWedgeType = properties.ParabolicWedgeType;
            if (_linearHexaType != FeElementTypeLinearHexa.None && properties.LinearHexaType != FeElementTypeLinearHexa.None)
                _linearHexaType = properties.LinearHexaType;
            if (_parabolicHexaType != FeElementTypeParabolicHexa.None && properties.ParabolicHexaType != FeElementTypeParabolicHexa.None)
                _parabolicHexaType = properties.ParabolicHexaType;
        }
    }
}

