using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    static class GeometryTools
    {
        static public double TriangleArea(FeNode n1, FeNode n2, FeNode n3)
        {
            //Heron's formula
            double a = Math.Sqrt(Math.Pow(n1.X - n2.X, 2) + Math.Pow(n1.Y - n2.Y, 2) + Math.Pow(n1.Z - n2.Z, 2));
            double b = Math.Sqrt(Math.Pow(n2.X - n3.X, 2) + Math.Pow(n2.Y - n3.Y, 2) + Math.Pow(n2.Z - n3.Z, 2));
            double c = Math.Sqrt(Math.Pow(n3.X - n1.X, 2) + Math.Pow(n3.Y - n1.Y, 2) + Math.Pow(n3.Z - n1.Z, 2));
            double s = (a + b + c) / 2;
            return Math.Sqrt(s * (s - a) * (s - b) * (s - c));
        }
    }
}
