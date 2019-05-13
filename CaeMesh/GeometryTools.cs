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

        static public double TriangleArea(FeNode n1, FeNode n2, FeNode n3, FeNode n4, FeNode n5, FeNode n6)
        {
            double area = 0;
            area += TriangleArea(n4, n6, n1);
            area += TriangleArea(n4, n5, n6);
            area += TriangleArea(n4, n2, n5);
            area += TriangleArea(n6, n5, n3);
            return area;
        }

        static public double RectangleArea(FeNode n1, FeNode n2, FeNode n3, FeNode n4)
        {
            double area = 0;
            area += TriangleArea(n1, n2, n3);
            area += TriangleArea(n1, n3, n4);
            return area;
        }

        static public double RectangleArea(FeNode n1, FeNode n2, FeNode n3, FeNode n4,
                                           FeNode n5, FeNode n6, FeNode n7, FeNode n8)
        {
            double area = 0;
            area += TriangleArea(n8, n1, n5);
            area += TriangleArea(n8, n5, n7);
            area += TriangleArea(n8, n7, n4);

            area += TriangleArea(n6, n3, n7);
            area += TriangleArea(n6, n7, n5);
            area += TriangleArea(n6, n5, n2);
            return area;
        }
    }
}
