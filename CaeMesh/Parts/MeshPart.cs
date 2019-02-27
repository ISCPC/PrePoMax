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
        protected FeElementTypeLinearTetra _linearTetraType;
        protected FeElementTypeParabolicTetra _parabolicTetraType;
        protected FeElementTypeLinearWedge _linearWedgeType;
        protected FeElementTypeParabolicWedge _parabolicWedgeType;
        protected FeElementTypeLinearHexa _linearHexaType;
        protected FeElementTypeParabolicHexa _parabolicHexaType;


        // Properties                                                                                                               
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
            : base((BasePart)part)
        {
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
            _linearTetraType = FeElementTypeLinearTetra.None;
            _parabolicTetraType = FeElementTypeParabolicTetra.None;
            _linearWedgeType = FeElementTypeLinearWedge.None;
            _parabolicWedgeType = FeElementTypeParabolicWedge.None;
            _linearHexaType = FeElementTypeLinearHexa.None;
            _parabolicHexaType = FeElementTypeParabolicHexa.None;

            HashSet<Type> types = new HashSet<Type>(_elementTypes);
            if (types.Contains(typeof(LinearTetraElement))) _linearTetraType = FeElementTypeLinearTetra.C3D4;
            if (types.Contains(typeof(ParabolicTetraElement))) _parabolicTetraType = FeElementTypeParabolicTetra.C3D10;
            if (types.Contains(typeof(LinearWedgeElement))) _linearWedgeType = FeElementTypeLinearWedge.C3D6;
            if (types.Contains(typeof(ParabolicWedgeElement))) _parabolicWedgeType = FeElementTypeParabolicWedge.C3D15;
            if (types.Contains(typeof(LinearHexaElement))) _linearHexaType = FeElementTypeLinearHexa.C3D8;
            if (types.Contains(typeof(ParabolicHexaElement))) _parabolicHexaType = FeElementTypeParabolicHexa.C3D20;
        }
        public void CopyElementTypesFrom(MeshPart part)
        {
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
                    // normal elements                              

                    // tetrahedral
                    case "C3D4":
                        _linearTetraType = FeElementTypeLinearTetra.C3D4;
                        break;
                    case "C3D10":
                        _parabolicTetraType = FeElementTypeParabolicTetra.C3D10;
                        break;
                    // wedge
                    case "C3D6":
                        _linearWedgeType = FeElementTypeLinearWedge.C3D6;
                        break;
                    case "C3D15":
                        _parabolicWedgeType = FeElementTypeParabolicWedge.C3D15;
                        break;
                    // hexahedral
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

