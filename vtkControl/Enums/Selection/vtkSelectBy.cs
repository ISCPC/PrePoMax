using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vtkControl
{
    public enum vtkSelectBy
    {
        Off,
        Id,
        Node,
        Element,
        Edge,
        Surface,
        EdgeAngle,
        SurfaceAngle,
        Part,

        QueryNode,
        QueryElement,
        QueryPart
    }
}
