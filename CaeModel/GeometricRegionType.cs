using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeModel
{
    [Serializable]
    public enum GeometricRegionType
    {
        Node,
        NodeSet,
        Element,
        ElementSet,
        ReferencePoint
    }
}
