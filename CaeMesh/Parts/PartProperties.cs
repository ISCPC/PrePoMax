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
        public string Name;
        public PartType PartType;
        public System.Drawing.Color Color;
        public bool ColorContours;
        public int NumberOfElements;
        public int NumberOfNodes;
        public FeElementTypeLinearTetra LinearTetraType;
        public FeElementTypeParabolicTetra ParabolicTetraType;
        public FeElementTypeLinearWedge LinearWedgeType;
        public FeElementTypeParabolicWedge ParabolicWedgeType;
        public FeElementTypeLinearHexa LinearHexaType;
        public FeElementTypeParabolicHexa ParabolicHexaType;
        public MeshingParameters MeshingParameters;
    }
}
