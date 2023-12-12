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
        protected FeElementTypeLinearBeam _linearBeamType;
        protected FeElementTypeParabolicBeam _parabolicBeamType;
        //
        protected FeElementTypeLinearTria _linearTriaType;
        protected FeElementTypeParabolicTria _parabolicTriaType;
        protected FeElementTypeLinearQuad _linearQuadType;
        protected FeElementTypeParabolicQuad _parabolicQuadType;
        //
        protected FeElementTypeLinearTetra _linearTetraType;
        protected FeElementTypeParabolicTetra _parabolicTetraType;
        protected FeElementTypeLinearPyramid _linearPyramidType;
        protected FeElementTypeParabolicPyramid _parabolicPyramidType;
        protected FeElementTypeLinearWedge _linearWedgeType;
        protected FeElementTypeParabolicWedge _parabolicWedgeType;
        protected FeElementTypeLinearHexa _linearHexaType;
        protected FeElementTypeParabolicHexa _parabolicHexaType;
        //
        protected bool _createdFromBasePart;


        // Properties                                                                                                               
        public FeElementTypeLinearBeam LinearBeamType { get { return _linearBeamType; } set { _linearBeamType = value; } }
        public FeElementTypeParabolicBeam ParabolicBeamType
        {
            get { return _parabolicBeamType; }
            set { _parabolicBeamType = value; }
        }
        //
        public FeElementTypeLinearTria LinearTriaType { get { return _linearTriaType; } set { _linearTriaType = value; } }
        public FeElementTypeParabolicTria ParabolicTriaType
        {
            get { return _parabolicTriaType; }
            set { _parabolicTriaType = value; }
        }
        public FeElementTypeLinearQuad LinearQuadType { get { return _linearQuadType; } set { _linearQuadType = value; } }
        public FeElementTypeParabolicQuad ParabolicQuadType
        {
            get { return _parabolicQuadType; }
            set { _parabolicQuadType = value; }
        }
        //
        public FeElementTypeLinearTetra LinearTetraType { get { return _linearTetraType; } set { _linearTetraType = value; } }
        public FeElementTypeParabolicTetra ParabolicTetraType
        {
            get { return _parabolicTetraType; }
            set { _parabolicTetraType = value; }
        }

        public FeElementTypeLinearPyramid LinearPyramidType { get { return _linearPyramidType; } set { _linearPyramidType = value; } }
        public FeElementTypeParabolicPyramid ParabolicPyramidType
        {
            get { return _parabolicPyramidType; }
            set { _parabolicPyramidType = value; }
        }

        public FeElementTypeLinearWedge LinearWedgeType { get { return _linearWedgeType; } set { _linearWedgeType = value; } }
        public FeElementTypeParabolicWedge ParabolicWedgeType
        {
            get { return _parabolicWedgeType; }
            set { _parabolicWedgeType = value; }
        }
        public FeElementTypeLinearHexa LinearHexaType { get { return _linearHexaType; } set { _linearHexaType = value; } }
        public FeElementTypeParabolicHexa ParabolicHexaType
        {
            get { return _parabolicHexaType; }
            set { _parabolicHexaType = value; }
        }
        //
        public bool CreatedFromBasePart { get { return _createdFromBasePart; } }


        // Constructors                                                                                                             
        public MeshPart(string name, int partId, int[] nodeLabels, int[] elementLabels, Type[] elementTypes)
            : base(name, partId, nodeLabels, elementLabels, elementTypes)
        {
            InitializeElementTypes();
            //
            _createdFromBasePart = false;
        }
        public MeshPart(BasePart part)
            : base(part)
        {
            InitializeElementTypes();
            //
            _createdFromBasePart = true;
        }
        public MeshPart(MeshPart part)
            : base(part)
        {
            _linearBeamType = part.LinearBeamType;
            _parabolicBeamType = part.ParabolicBeamType;
            //
            _linearTriaType = part.LinearTriaType;
            _parabolicTriaType = part.ParabolicTriaType;
            _linearQuadType = part.LinearQuadType;
            _parabolicQuadType = part.ParabolicQuadType;
            //
            _linearTetraType = part.LinearTetraType;
            _parabolicTetraType = part.ParabolicTetraType;
            _linearPyramidType = part.LinearPyramidType;
            _parabolicPyramidType = part.ParabolicPyramidType;
            _linearWedgeType = part.LinearWedgeType;
            _parabolicWedgeType = part.ParabolicWedgeType;
            _linearHexaType = part.LinearHexaType;
            _parabolicHexaType = part.ParabolicHexaType;
            //
            _createdFromBasePart = part.CreatedFromBasePart;
        }


        // Methods                                                                                                                  
        private void InitializeElementTypes()
        {
            _linearBeamType = FeElementTypeLinearBeam.None;
            _parabolicBeamType = FeElementTypeParabolicBeam.None;
            //
            _linearTriaType = FeElementTypeLinearTria.None;
            _parabolicTriaType = FeElementTypeParabolicTria.None;
            _linearQuadType = FeElementTypeLinearQuad.None;
            _parabolicQuadType = FeElementTypeParabolicQuad.None;
            //
            _linearTetraType = FeElementTypeLinearTetra.None;
            _parabolicTetraType = FeElementTypeParabolicTetra.None;
            _linearPyramidType = FeElementTypeLinearPyramid.None;
            _parabolicPyramidType = FeElementTypeParabolicPyramid.None;
            _linearWedgeType = FeElementTypeLinearWedge.None;
            _parabolicWedgeType = FeElementTypeParabolicWedge.None;
            _linearHexaType = FeElementTypeLinearHexa.None;
            _parabolicHexaType = FeElementTypeParabolicHexa.None;
            //
            HashSet<Type> types = new HashSet<Type>(_elementTypes);
            // Default values
            if (types.Contains(typeof(LinearBeamElement))) _linearBeamType = FeElementTypeLinearBeam.B31;
            if (types.Contains(typeof(ParabolicBeamElement))) _parabolicBeamType = FeElementTypeParabolicBeam.B32;
            //
            if (types.Contains(typeof(LinearTriangleElement))) _linearTriaType = FeElementTypeLinearTria.S3;
            if (types.Contains(typeof(ParabolicTriangleElement))) _parabolicTriaType = FeElementTypeParabolicTria.S6;
            if (types.Contains(typeof(LinearQuadrilateralElement))) _linearQuadType = FeElementTypeLinearQuad.S4;
            if (types.Contains(typeof(ParabolicQuadrilateralElement))) _parabolicQuadType = FeElementTypeParabolicQuad.S8;
            //
            if (types.Contains(typeof(LinearTetraElement))) _linearTetraType = FeElementTypeLinearTetra.C3D4;
            if (types.Contains(typeof(ParabolicTetraElement))) _parabolicTetraType = FeElementTypeParabolicTetra.C3D10;
            if (types.Contains(typeof(LinearPyramidElement))) _linearPyramidType = FeElementTypeLinearPyramid.C3D5;
            if (types.Contains(typeof(ParabolicPyramidElement))) _parabolicPyramidType = FeElementTypeParabolicPyramid.C3D13;
            if (types.Contains(typeof(LinearWedgeElement))) _linearWedgeType = FeElementTypeLinearWedge.C3D6;
            if (types.Contains(typeof(ParabolicWedgeElement))) _parabolicWedgeType = FeElementTypeParabolicWedge.C3D15;
            if (types.Contains(typeof(LinearHexaElement))) _linearHexaType = FeElementTypeLinearHexa.C3D8;
            if (types.Contains(typeof(ParabolicHexaElement))) _parabolicHexaType = FeElementTypeParabolicHexa.C3D20;
        }
        public void AddElementTypes(Type[] elementTypes)
        {
            HashSet<Type> types = new HashSet<Type>(elementTypes);
            //
            if (types.Contains(typeof(LinearBeamElement)) && _linearBeamType == FeElementTypeLinearBeam.None)
                _linearBeamType = FeElementTypeLinearBeam.B31;
            if (types.Contains(typeof(ParabolicBeamElement)) && _parabolicBeamType == FeElementTypeParabolicBeam.None)
                _parabolicBeamType = FeElementTypeParabolicBeam.B32;
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
            if (types.Contains(typeof(LinearPyramidElement)) && _linearPyramidType == FeElementTypeLinearPyramid.None)
                _linearPyramidType = FeElementTypeLinearPyramid.C3D5;
            if (types.Contains(typeof(ParabolicPyramidElement)) && _parabolicPyramidType == FeElementTypeParabolicPyramid.None)
                _parabolicPyramidType = FeElementTypeParabolicPyramid.C3D13;
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
        //
        public void CopyActiveElementTypesFrom(MeshPart part)
        {
            if (_linearBeamType != FeElementTypeLinearBeam.None) _linearBeamType = part.LinearBeamType;
            if (_parabolicBeamType != FeElementTypeParabolicBeam.None) _parabolicBeamType = part.ParabolicBeamType;
            //
            if (_linearTriaType != FeElementTypeLinearTria.None) _linearTriaType = part.LinearTriaType;
            if (_parabolicTriaType != FeElementTypeParabolicTria.None) _parabolicTriaType = part.ParabolicTriaType;
            if (_linearQuadType != FeElementTypeLinearQuad.None) _linearQuadType = part.LinearQuadType;
            if (_parabolicQuadType != FeElementTypeParabolicQuad.None) _parabolicQuadType = part.ParabolicQuadType;
            //
            if (_linearTetraType != FeElementTypeLinearTetra.None) _linearTetraType = part.LinearTetraType;
            if (_parabolicTetraType != FeElementTypeParabolicTetra.None) _parabolicTetraType = part.ParabolicTetraType;
            if (_linearPyramidType != FeElementTypeLinearPyramid.None) _linearPyramidType = part.LinearPyramidType;
            if (_parabolicPyramidType != FeElementTypeParabolicPyramid.None) _parabolicPyramidType = part.ParabolicPyramidType;
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
                    // Beam
                    case "T3D2":
                        if (_linearBeamType != FeElementTypeLinearBeam.None)
                            _linearBeamType = FeElementTypeLinearBeam.T3D2; break;
                    case "B31":
                        if (_linearBeamType != FeElementTypeLinearBeam.None)
                            _linearBeamType = FeElementTypeLinearBeam.B31; break;
                    case "B31R":
                        if (_linearBeamType != FeElementTypeLinearBeam.None)
                            _linearBeamType = FeElementTypeLinearBeam.B31R; break;
                    case "T2D2":
                        if (_linearBeamType != FeElementTypeLinearBeam.None)
                            _linearBeamType = FeElementTypeLinearBeam.T2D2; break;
                    case "B21":
                        if (_linearBeamType != FeElementTypeLinearBeam.None)
                            _linearBeamType = FeElementTypeLinearBeam.B21; break;
                    //
                    case "T2D3":
                    case "T3D3":    // Abaqus
                        if (_parabolicBeamType != FeElementTypeParabolicBeam.None)
                            _parabolicBeamType = FeElementTypeParabolicBeam.T2D3; break;
                    case "B32":
                        if (_parabolicBeamType != FeElementTypeParabolicBeam.None)
                            _parabolicBeamType = FeElementTypeParabolicBeam.B32; break;
                    case "B32R":
                        if (_parabolicBeamType != FeElementTypeParabolicBeam.None)
                            _parabolicBeamType = FeElementTypeParabolicBeam.B32R; break;
                    // Triangular
                    case "S3":
                    case "S3R":
                        if (_linearTriaType != FeElementTypeLinearTria.None)
                            _linearTriaType = FeElementTypeLinearTria.S3; break;
                    case "M3D3":
                        if (_linearTriaType != FeElementTypeLinearTria.None)
                            _linearTriaType = FeElementTypeLinearTria.M3D3; break;
                    case "CPS3":
                        if (_linearTriaType != FeElementTypeLinearTria.None)
                            _linearTriaType = FeElementTypeLinearTria.CPS3; break;
                    case "CPE3":
                        if (_linearTriaType != FeElementTypeLinearTria.None)
                            _linearTriaType = FeElementTypeLinearTria.CPE3; break;
                    case "CAX3":
                        if (_linearTriaType != FeElementTypeLinearTria.None)
                            _linearTriaType = FeElementTypeLinearTria.CAX3; break;
                    //
                    case "S6":
                    case "STRI65":  // Abaqus
                        if (_parabolicTriaType != FeElementTypeParabolicTria.None)
                            _parabolicTriaType = FeElementTypeParabolicTria.S6; break;
                    case "M3D6":
                        if (_parabolicTriaType != FeElementTypeParabolicTria.None)
                            _parabolicTriaType = FeElementTypeParabolicTria.M3D6; break;
                    case "CPS6":
                        if (_parabolicTriaType != FeElementTypeParabolicTria.None)
                            _parabolicTriaType = FeElementTypeParabolicTria.CPS6; break;
                    case "CPE6":
                        if (_parabolicTriaType != FeElementTypeParabolicTria.None)
                            _parabolicTriaType = FeElementTypeParabolicTria.CPE6; break;
                    case "CAX6":
                        if (_parabolicTriaType != FeElementTypeParabolicTria.None)
                            _parabolicTriaType = FeElementTypeParabolicTria.CAX6; break;
                    // Quadrilateral
                    case "S4":
                        if (_linearQuadType != FeElementTypeLinearQuad.None)
                            _linearQuadType = FeElementTypeLinearQuad.S4; break;
                    case "S4R":
                        if (_linearQuadType != FeElementTypeLinearQuad.None)
                            _linearQuadType = FeElementTypeLinearQuad.S4R; break;
                    case "M3D4":
                        if (_linearQuadType != FeElementTypeLinearQuad.None)
                            _linearQuadType = FeElementTypeLinearQuad.M3D4; break;
                    case "M3D4R":
                        if (_linearQuadType != FeElementTypeLinearQuad.None)
                            _linearQuadType = FeElementTypeLinearQuad.M3D4R; break;
                    case "CPS4":
                        if (_linearQuadType != FeElementTypeLinearQuad.None)
                            _linearQuadType = FeElementTypeLinearQuad.CPS4; break;
                    case "CPS4R":
                        if (_linearQuadType != FeElementTypeLinearQuad.None)
                            _linearQuadType = FeElementTypeLinearQuad.CPS4R; break;
                    case "CPE4":
                        if (_linearQuadType != FeElementTypeLinearQuad.None)
                            _linearQuadType = FeElementTypeLinearQuad.CPE4; break;
                    case "CPE4R":
                        if (_linearQuadType != FeElementTypeLinearQuad.None)
                            _linearQuadType = FeElementTypeLinearQuad.CPE4R; break;
                    case "CAX4":
                        if (_linearQuadType != FeElementTypeLinearQuad.None)
                            _linearQuadType = FeElementTypeLinearQuad.CAX4; break;
                    case "CAX4R":
                        if (_linearQuadType != FeElementTypeLinearQuad.None)
                            _linearQuadType = FeElementTypeLinearQuad.CAX4R; break;
                    //
                    case "S8":
                        if (_parabolicQuadType != FeElementTypeParabolicQuad.None)
                            _parabolicQuadType = FeElementTypeParabolicQuad.S8; break;
                    case "S8R":
                        if (_parabolicQuadType != FeElementTypeParabolicQuad.None)
                            _parabolicQuadType = FeElementTypeParabolicQuad.S8R; break;
                    case "M3D8":
                        if (_parabolicQuadType != FeElementTypeParabolicQuad.None)
                            _parabolicQuadType = FeElementTypeParabolicQuad.M3D8; break;
                    case "M3D8R":
                        if (_parabolicQuadType != FeElementTypeParabolicQuad.None)
                            _parabolicQuadType = FeElementTypeParabolicQuad.M3D8R; break;
                    case "CPS8":
                        if (_parabolicQuadType != FeElementTypeParabolicQuad.None)
                            _parabolicQuadType = FeElementTypeParabolicQuad.CPS8; break;
                    case "CPS8R":
                        if (_parabolicQuadType != FeElementTypeParabolicQuad.None)
                            _parabolicQuadType = FeElementTypeParabolicQuad.CPS8R; break;
                    case "CPE8":
                        if (_parabolicQuadType != FeElementTypeParabolicQuad.None)
                            _parabolicQuadType = FeElementTypeParabolicQuad.CPE8; break;
                    case "CPE8R":
                        if (_parabolicQuadType != FeElementTypeParabolicQuad.None)
                            _parabolicQuadType = FeElementTypeParabolicQuad.CPE8R; break;
                    case "CAX8":
                        if (_parabolicQuadType != FeElementTypeParabolicQuad.None)
                            _parabolicQuadType = FeElementTypeParabolicQuad.CAX8; break;
                    case "CAX8R":
                        if (_parabolicQuadType != FeElementTypeParabolicQuad.None)
                            _parabolicQuadType = FeElementTypeParabolicQuad.CAX8R; break;
                    // Tetrahedral
                    case "C3D4":
                        if (_linearTetraType != FeElementTypeLinearTetra.None)
                            _linearTetraType = FeElementTypeLinearTetra.C3D4; break;
                    //
                    case "C3D10":
                        if (_parabolicTetraType != FeElementTypeParabolicTetra.None)
                            _parabolicTetraType = FeElementTypeParabolicTetra.C3D10; break;
                    case "C3D10T":
                        if (_parabolicTetraType != FeElementTypeParabolicTetra.None)
                            _parabolicTetraType = FeElementTypeParabolicTetra.C3D10T; break;
                    // Pyramid
                    case "C3D5":
                        if (_linearPyramidType != FeElementTypeLinearPyramid.None)
                            _linearPyramidType = FeElementTypeLinearPyramid.C3D5; break;
                    //
                    case "C3D13":
                        if (_parabolicPyramidType != FeElementTypeParabolicPyramid.None)
                            _parabolicPyramidType = FeElementTypeParabolicPyramid.C3D13; break;
                    // Wedge
                    case "C3D6":
                        if (_linearWedgeType != FeElementTypeLinearWedge.None)
                            _linearWedgeType = FeElementTypeLinearWedge.C3D6; break;
                    //
                    case "C3D15":
                        if (_parabolicWedgeType != FeElementTypeParabolicWedge.None)
                            _parabolicWedgeType = FeElementTypeParabolicWedge.C3D15; break;
                    // Hexahedral
                    case "C3D8":
                        if (_linearHexaType != FeElementTypeLinearHexa.None)
                            _linearHexaType = FeElementTypeLinearHexa.C3D8; break;
                    case "C3D8R":
                        if (_linearHexaType != FeElementTypeLinearHexa.None)
                            _linearHexaType = FeElementTypeLinearHexa.C3D8R; break;
                    case "C3D8I":
                        if (_linearHexaType != FeElementTypeLinearHexa.None)
                            _linearHexaType = FeElementTypeLinearHexa.C3D8I; break;
                    //
                    case "C3D20":
                        if (_parabolicHexaType != FeElementTypeParabolicHexa.None)
                            _parabolicHexaType = FeElementTypeParabolicHexa.C3D20; break;
                    case "C3D20R":
                        if (_parabolicHexaType != FeElementTypeParabolicHexa.None)
                            _parabolicHexaType = FeElementTypeParabolicHexa.C3D20R; break;
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
            properties.LinearBeamType = _linearBeamType;
            properties.ParabolicBeamType = _parabolicBeamType;
            //
            properties.LinearTriaType = _linearTriaType;
            properties.ParabolicTriaType = _parabolicTriaType;
            properties.LinearQuadType = _linearQuadType;
            properties.ParabolicQuadType = _parabolicQuadType;
            //
            properties.LinearTetraType = _linearTetraType;
            properties.ParabolicTetraType = _parabolicTetraType;
            properties.LinearPyramidType = _linearPyramidType;
            properties.ParabolicPyramidType = _parabolicPyramidType;
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
            if (_linearBeamType != FeElementTypeLinearBeam.None &&
                properties.LinearBeamType != FeElementTypeLinearBeam.None)
                _linearBeamType = properties.LinearBeamType;
            if (_parabolicBeamType != FeElementTypeParabolicBeam.None &&
                properties.ParabolicBeamType != FeElementTypeParabolicBeam.None)
                _parabolicBeamType = properties.ParabolicBeamType;
            //
            if (_linearTriaType != FeElementTypeLinearTria.None &&
                properties.LinearTriaType != FeElementTypeLinearTria.None)
                _linearTriaType = properties.LinearTriaType;
            if (_parabolicTriaType != FeElementTypeParabolicTria.None &&
                properties.ParabolicTriaType != FeElementTypeParabolicTria.None)
                _parabolicTriaType = properties.ParabolicTriaType;
            if (_linearQuadType != FeElementTypeLinearQuad.None &&
                properties.LinearQuadType != FeElementTypeLinearQuad.None)
                _linearQuadType = properties.LinearQuadType;
            if (_parabolicQuadType != FeElementTypeParabolicQuad.None &&
                properties.ParabolicQuadType != FeElementTypeParabolicQuad.None)
                _parabolicQuadType = properties.ParabolicQuadType;
            //
            if (_linearTetraType != FeElementTypeLinearTetra.None &&
                properties.LinearTetraType != FeElementTypeLinearTetra.None)
                _linearTetraType = properties.LinearTetraType;
            if (_parabolicTetraType != FeElementTypeParabolicTetra.None &&
                properties.ParabolicTetraType != FeElementTypeParabolicTetra.None)
                _parabolicTetraType = properties.ParabolicTetraType;

            if (_linearPyramidType != FeElementTypeLinearPyramid.None &&
                properties.LinearPyramidType != FeElementTypeLinearPyramid.None)
                _linearPyramidType = properties.LinearPyramidType;
            if (_parabolicPyramidType != FeElementTypeParabolicPyramid.None &&
                properties.ParabolicPyramidType != FeElementTypeParabolicPyramid.None)
                _parabolicPyramidType = properties.ParabolicPyramidType;

            if (_linearWedgeType != FeElementTypeLinearWedge.None &&
                properties.LinearWedgeType != FeElementTypeLinearWedge.None)
                _linearWedgeType = properties.LinearWedgeType;
            if (_parabolicWedgeType != FeElementTypeParabolicWedge.None &&
                properties.ParabolicWedgeType != FeElementTypeParabolicWedge.None)
                _parabolicWedgeType = properties.ParabolicWedgeType;
            if (_linearHexaType != FeElementTypeLinearHexa.None &&
                properties.LinearHexaType != FeElementTypeLinearHexa.None)
                _linearHexaType = properties.LinearHexaType;
            if (_parabolicHexaType != FeElementTypeParabolicHexa.None &&
                properties.ParabolicHexaType != FeElementTypeParabolicHexa.None)
                _parabolicHexaType = properties.ParabolicHexaType;
        }
        //
        public List<Enum> GetElementTypeEnums()
        {
            List<Enum> elementTypeEnums = new List<Enum>();
            //
            if (_linearBeamType != FeElementTypeLinearBeam.None) elementTypeEnums.Add(_linearBeamType);
            if (_parabolicBeamType != FeElementTypeParabolicBeam.None) elementTypeEnums.Add(_parabolicBeamType);
            //
            if (_linearTriaType != FeElementTypeLinearTria.None) elementTypeEnums.Add(_linearTriaType);
            if (_parabolicTriaType != FeElementTypeParabolicTria.None) elementTypeEnums.Add(_parabolicTriaType);
            if (_linearQuadType != FeElementTypeLinearQuad.None) elementTypeEnums.Add(_linearQuadType);
            if (_parabolicQuadType != FeElementTypeParabolicQuad.None) elementTypeEnums.Add(_parabolicQuadType);
            //
            if (_linearTetraType != FeElementTypeLinearTetra.None) elementTypeEnums.Add(_linearTetraType);
            if (_parabolicTetraType != FeElementTypeParabolicTetra.None) elementTypeEnums.Add(_parabolicTetraType);
            if (_linearPyramidType != FeElementTypeLinearPyramid.None) elementTypeEnums.Add(_linearPyramidType);
            if (_parabolicPyramidType != FeElementTypeParabolicPyramid.None) elementTypeEnums.Add(_parabolicPyramidType);
            if (_linearWedgeType != FeElementTypeLinearWedge.None) elementTypeEnums.Add(_linearWedgeType);
            if (_parabolicWedgeType != FeElementTypeParabolicWedge.None) elementTypeEnums.Add(_parabolicWedgeType);
            if (_linearHexaType != FeElementTypeLinearHexa.None) elementTypeEnums.Add(_linearHexaType);
            if (_parabolicHexaType != FeElementTypeParabolicHexa.None) elementTypeEnums.Add(_parabolicHexaType);
            //
            return elementTypeEnums;
        }
        public void SetElementTypeEnums(List<Enum> elementTypeEnums)
        {
            if (elementTypeEnums != null)
            {
                foreach (var elementEnum in elementTypeEnums)
                {
                    if (elementEnum is FeElementTypeLinearBeam lb &&
                        _linearBeamType != FeElementTypeLinearBeam.None)
                        _linearBeamType = lb;
                    if (elementEnum is FeElementTypeParabolicBeam pb &&
                        _parabolicBeamType != FeElementTypeParabolicBeam.None)
                        _parabolicBeamType = pb;
                    //
                    if (elementEnum is FeElementTypeLinearTria lt &&
                        _linearTriaType != FeElementTypeLinearTria.None)
                        _linearTriaType = lt;
                    else if (elementEnum is FeElementTypeParabolicTria pt &&
                        _parabolicTriaType != FeElementTypeParabolicTria.None)
                        _parabolicTriaType = pt;
                    else if (elementEnum is FeElementTypeLinearQuad lq &&
                        _linearQuadType != FeElementTypeLinearQuad.None)
                        _linearQuadType = lq;
                    else if (elementEnum is FeElementTypeParabolicQuad pq &&
                        _parabolicQuadType != FeElementTypeParabolicQuad.None)
                        _parabolicQuadType = pq;
                    //
                    else if (elementEnum is FeElementTypeLinearTetra lte &&
                        _linearTetraType != FeElementTypeLinearTetra.None)
                        _linearTetraType = lte;
                    else if (elementEnum is FeElementTypeParabolicTetra pte &&
                        _parabolicTetraType != FeElementTypeParabolicTetra.None)
                        _parabolicTetraType = pte;
                    else if (elementEnum is FeElementTypeLinearPyramid lpe &&
                        _linearPyramidType != FeElementTypeLinearPyramid.None)
                        _linearPyramidType = lpe;
                    else if (elementEnum is FeElementTypeParabolicPyramid ppe &&
                        _parabolicPyramidType != FeElementTypeParabolicPyramid.None)
                        _parabolicPyramidType = ppe;
                    else if (elementEnum is FeElementTypeLinearWedge lw &&
                        _linearWedgeType != FeElementTypeLinearWedge.None)
                        _linearWedgeType = lw;
                    else if (elementEnum is FeElementTypeParabolicWedge pw &&
                        _parabolicWedgeType != FeElementTypeParabolicWedge.None)
                        _parabolicWedgeType = pw;
                    else if (elementEnum is FeElementTypeLinearHexa lh &&
                        _linearHexaType != FeElementTypeLinearHexa.None)
                        _linearHexaType = lh;
                    else if (elementEnum is FeElementTypeParabolicHexa ph &&
                        _parabolicHexaType != FeElementTypeParabolicHexa.None)
                        _parabolicHexaType = ph;
                }
            }
        }
        public void UpdateElementTypeEnums(Dictionary<Type, HashSet<Enum>> elementTypeEnums)
        {
            // After update select the first available element type
            if (elementTypeEnums != null)
            {
                HashSet<Enum> elementEnums;
                //
                if (_linearBeamType != FeElementTypeLinearBeam.None &&
                    elementTypeEnums.TryGetValue(typeof(FeElementTypeLinearBeam), out elementEnums) &&
                    !elementEnums.Contains(_linearBeamType))
                    _linearBeamType = (FeElementTypeLinearBeam)elementEnums.First();
                if (_parabolicBeamType != FeElementTypeParabolicBeam.None &&
                    elementTypeEnums.TryGetValue(typeof(FeElementTypeParabolicBeam), out elementEnums) &&
                    !elementEnums.Contains(_parabolicBeamType))
                    _parabolicBeamType = (FeElementTypeParabolicBeam)elementEnums.First();
                //
                if (_linearTriaType != FeElementTypeLinearTria.None &&
                    elementTypeEnums.TryGetValue(typeof(FeElementTypeLinearTria), out elementEnums) &&
                    !elementEnums.Contains(_linearTriaType))
                    _linearTriaType = (FeElementTypeLinearTria)elementEnums.First();
                if (_parabolicTriaType != FeElementTypeParabolicTria.None &&
                    elementTypeEnums.TryGetValue(typeof(FeElementTypeParabolicTria), out elementEnums) &&
                    !elementEnums.Contains(_parabolicTriaType))
                    _parabolicTriaType = (FeElementTypeParabolicTria)elementEnums.First();
                if (_linearQuadType != FeElementTypeLinearQuad.None &&
                    elementTypeEnums.TryGetValue(typeof(FeElementTypeLinearQuad), out elementEnums) &&
                    !elementEnums.Contains(_linearQuadType))
                    _linearQuadType = (FeElementTypeLinearQuad)elementEnums.First();
                if (_parabolicQuadType != FeElementTypeParabolicQuad.None &&
                    elementTypeEnums.TryGetValue(typeof(FeElementTypeParabolicQuad), out elementEnums) &&
                    !elementEnums.Contains(_parabolicQuadType))
                    _parabolicQuadType = (FeElementTypeParabolicQuad)elementEnums.First();
                //
                if (_linearTetraType != FeElementTypeLinearTetra.None &&
                    elementTypeEnums.TryGetValue(typeof(FeElementTypeLinearTetra), out elementEnums) &&
                    !elementEnums.Contains(_linearTetraType))
                    _linearTetraType = (FeElementTypeLinearTetra)elementEnums.First();
                if (_parabolicTetraType != FeElementTypeParabolicTetra.None &&
                    elementTypeEnums.TryGetValue(typeof(FeElementTypeParabolicTetra), out elementEnums) &&
                    !elementEnums.Contains(_parabolicTetraType))
                    _parabolicTetraType = (FeElementTypeParabolicTetra)elementEnums.First();
                if (_linearPyramidType != FeElementTypeLinearPyramid.None &&
                   elementTypeEnums.TryGetValue(typeof(FeElementTypeLinearPyramid), out elementEnums) &&
                   !elementEnums.Contains(_linearPyramidType))
                    _linearPyramidType = (FeElementTypeLinearPyramid)elementEnums.First();
                if (_parabolicPyramidType != FeElementTypeParabolicPyramid.None &&
                    elementTypeEnums.TryGetValue(typeof(FeElementTypeParabolicPyramid), out elementEnums) &&
                    !elementEnums.Contains(_parabolicPyramidType))
                    _parabolicPyramidType = (FeElementTypeParabolicPyramid)elementEnums.First();
                if (_linearWedgeType != FeElementTypeLinearWedge.None &&
                    elementTypeEnums.TryGetValue(typeof(FeElementTypeLinearWedge), out elementEnums) &&
                    !elementEnums.Contains(_linearWedgeType))
                    _linearWedgeType = (FeElementTypeLinearWedge)elementEnums.First();
                if (_parabolicWedgeType != FeElementTypeParabolicWedge.None &&
                    elementTypeEnums.TryGetValue(typeof(FeElementTypeParabolicWedge), out elementEnums) &&
                    !elementEnums.Contains(_parabolicWedgeType))
                    _parabolicWedgeType = (FeElementTypeParabolicWedge)elementEnums.First();
                if (_linearHexaType != FeElementTypeLinearHexa.None &&
                    elementTypeEnums.TryGetValue(typeof(FeElementTypeLinearHexa), out elementEnums) &&
                    !elementEnums.Contains(_linearHexaType))
                    _linearHexaType = (FeElementTypeLinearHexa)elementEnums.First();
                if (_parabolicHexaType != FeElementTypeParabolicHexa.None &&
                    elementTypeEnums.TryGetValue(typeof(FeElementTypeParabolicHexa), out elementEnums) &&
                    !elementEnums.Contains(_parabolicHexaType))
                    _parabolicHexaType = (FeElementTypeParabolicHexa)elementEnums.First();
            }
        }
        public string GetElementType(FeElement element)
        {
            if (element is LinearBeamElement) return _linearBeamType.ToString();
            else if (element is ParabolicBeamElement) return _parabolicBeamType.ToString();
            //
            else if (element is LinearTriangleElement) return _linearTriaType.ToString();
            else if (element is ParabolicTriangleElement) return _parabolicTriaType.ToString();
            else if (element is LinearQuadrilateralElement) return _linearQuadType.ToString();
            else if (element is ParabolicQuadrilateralElement) return _parabolicQuadType.ToString();
            //
            else if (element is LinearTetraElement) return _linearTetraType.ToString();
            else if (element is ParabolicTetraElement) return _parabolicTetraType.ToString();
            else if (element is LinearPyramidElement) return _linearPyramidType.ToString();
            else if (element is ParabolicPyramidElement) return _parabolicPyramidType.ToString();
            else if (element is LinearWedgeElement) return _linearWedgeType.ToString();
            else if (element is ParabolicWedgeElement) return _parabolicWedgeType.ToString();
            else if (element is LinearHexaElement) return _linearHexaType.ToString();
            else if (element is ParabolicHexaElement) return _parabolicHexaType.ToString();
            else throw new NotSupportedException();
        }





    }
}

