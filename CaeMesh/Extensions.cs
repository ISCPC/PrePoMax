using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    public static class Extensions
    {
        public static bool HasEdges(this PartType partType)
        {
            if (partType == PartType.Solid || partType == PartType.SolidAsShell || partType == PartType.Shell ||
                partType == PartType.Wire) return true;
            else throw new NotSupportedException();
        }
        //
        public static bool IsEdge(this GeometryType geometryType)
        {
            if (geometryType == GeometryType.Edge ||
                geometryType == GeometryType.ShellEdgeSurface) return true;
            else return false;
        }
        public static bool IsShellSurface(this GeometryType geometryType)
        {
            if (geometryType == GeometryType.ShellFrontSurface ||
                geometryType == GeometryType.ShellBackSurface) return true;
            else return false;
        }
        public static bool IsSurface(this GeometryType geometryType)
        {
            if (geometryType == GeometryType.SolidSurface ||
                geometryType == GeometryType.ShellFrontSurface ||
                geometryType == GeometryType.ShellBackSurface) return true;
            else return false;
        }
       
    }
}
