using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    // Used to encode the selected geometry items into ItemTypePart id
    [Serializable]
    public enum GeometryType
    {
        Unknown = -1,
        Vertex = 1,
        Edge = 2,
        SolidSurface = 3,
        ShellFrontSurface = 4,
        ShellBackSurface = 5,
        ShellEdgeSurface = 6,
        Part = 9
    }
}
