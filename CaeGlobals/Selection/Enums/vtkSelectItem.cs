using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeGlobals
{
    [Serializable]
    public enum vtkSelectItem
    {   
        None,
        Node,           // nodeIds
        Element,        // elementIds
        Edge,           // geometryEdgeIds
        Surface,        // faceIds
        Geometry,       // geometryIds
        Part,           // partIds
    }
}
