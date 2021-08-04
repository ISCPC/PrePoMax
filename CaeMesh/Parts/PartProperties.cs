using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public struct PartProperties
    {
        // Only the properties, that can not be changed by any other edit form (like MeshingParameters...)
        public string Name
        {
            get { return _name; }
            set
            {
                CaeGlobals.NamedClass.CheckNameForErrors(ref value);
                _name = value;
            }
        }
        public PartType PartType;
        public System.Drawing.Color Color;
        public bool ColorContours;
        public int NumberOfElements;
        public int NumberOfNodes;
        //
        public FeElementTypeLinearTria LinearTriaType;
        public FeElementTypeParabolicTria ParabolicTriaType;
        public FeElementTypeLinearQuad LinearQuadType;
        public FeElementTypeParabolicQuad ParabolicQuadType;
        //
        public FeElementTypeLinearTetra LinearTetraType;
        public FeElementTypeParabolicTetra ParabolicTetraType;
        public FeElementTypeLinearWedge LinearWedgeType;
        public FeElementTypeParabolicWedge ParabolicWedgeType;
        public FeElementTypeLinearHexa LinearHexaType;
        public FeElementTypeParabolicHexa ParabolicHexaType;
        //
        private string _name;
    }
}
