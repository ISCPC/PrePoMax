using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeGlobals
{
    public enum vtkSelectBy
    {
        // General
        Off,
        // Mesh based
        Id,
        Node,
        Element,
        Edge,
        Surface,
        EdgeAngle,
        SurfaceAngle,
        Part,
        // Geometry
        Geometry,
        GeometryEdgeAngle,
        GeometrySurfaceAngle,
        // Querry
        QueryNode,
        QueryElement,
        QueryPart
    }
}
