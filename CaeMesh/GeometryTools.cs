using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    static class GeometryTools
    {
        static public double EdgeLength(FeNode n1, FeNode n2)
        {
            return Math.Sqrt(Math.Pow(n1.X - n2.X, 2) + Math.Pow(n1.Y - n2.Y, 2) + Math.Pow(n1.Z - n2.Z, 2));
        }
        static public double EdgeLength(FeNode n1, FeNode n2, FeNode n3)
        {
            double length = EdgeLength(n1, n3);
            length += EdgeLength(n3, n2);
            return length;
        }
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
            double area = TriangleArea(n4, n6, n1);
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
            double area = TriangleArea(n8, n1, n5);
            area += TriangleArea(n8, n5, n7);
            area += TriangleArea(n8, n7, n4);
            //
            area += TriangleArea(n6, n3, n7);
            area += TriangleArea(n6, n7, n5);
            area += TriangleArea(n6, n5, n2);
            return area;
        }
        //
        static public double[] EdgeCG(FeNode n1, FeNode n2, out double length)
        {
            length = EdgeLength(n1, n2);
            //
            double[] cg = new double[3];
            cg[0] = (n1.X + n2.X) / 2;
            cg[1] = (n1.Y + n2.Y) / 2;
            cg[2] = (n1.Z + n2.Z) / 2;
            return cg;
        }
        static public double[] EdgeCG(FeNode n1, FeNode n2, FeNode n3, out double length)
        {
            double[] cg = new double[3];
            double[] cg1;
            double l;
            //
            cg1 = EdgeCG(n1, n3, out l);
            cg[0] += cg1[0] * l;
            cg[1] += cg1[1] * l;
            cg[2] += cg1[2] * l;
            length = l;
            //
            cg1 = EdgeCG(n3, n2, out l);
            cg[0] += cg1[0] * l;
            cg[1] += cg1[1] * l;
            cg[2] += cg1[2] * l;
            length += l;
            //
            cg[0] /= length;
            cg[1] /= length;
            cg[2] /= length;
            //
            return cg;
        }
        static public double[] TriangleCG(FeNode n1, FeNode n2, FeNode n3, out double area)
        {
            area = TriangleArea(n1, n2, n3);
            //
            double[] cg = new double[3];
            cg[0] = (n1.X + n2.X + n3.X) / 3;
            cg[1] = (n1.Y + n2.Y + n3.Y) / 3;
            cg[2] = (n1.Z + n2.Z + n3.Z) / 3;
            return cg;
        }
        static public double[] TriangleCG(FeNode n1, FeNode n2, FeNode n3, FeNode n4, FeNode n5, FeNode n6,
                                          out double area)
        {
            double[] cg = new double[3];            
            double[] cg1;
            double a;
            //
            cg1 = TriangleCG(n4, n6, n1, out a);
            cg[0] += cg1[0] * a;
            cg[1] += cg1[1] * a;
            cg[2] += cg1[2] * a;
            area = a;
            //
            cg1 = TriangleCG(n4, n5, n6, out a);
            cg[0] += cg1[0] * a;
            cg[1] += cg1[1] * a;
            cg[2] += cg1[2] * a;
            area += a;
            //
            cg1 = TriangleCG(n4, n2, n5, out a);
            cg[0] += cg1[0] * a;
            cg[1] += cg1[1] * a;
            cg[2] += cg1[2] * a;
            area += a;
            //
            cg1 = TriangleCG(n6, n5, n3, out a);
            cg[0] += cg1[0] * a;
            cg[1] += cg1[1] * a;
            cg[2] += cg1[2] * a;
            area += a;
            //
            cg[0] /= area;
            cg[1] /= area;
            cg[2] /= area;
            //
            return cg;
        }
        static public double[] RectangleCG(FeNode n1, FeNode n2, FeNode n3, FeNode n4, out double area)
        {
            double[] cg = new double[3];
            double[] cg1;
            double a;
            //
            cg1 = TriangleCG(n1, n2, n3, out a);
            cg[0] += cg1[0] * a;
            cg[1] += cg1[1] * a;
            cg[2] += cg1[2] * a;
            area = a;
            //
            cg1 = TriangleCG(n1, n3, n4, out a);
            cg[0] += cg1[0] * a;
            cg[1] += cg1[1] * a;
            cg[2] += cg1[2] * a;
            area += a;
            //
            cg[0] /= area;
            cg[1] /= area;
            cg[2] /= area;
            //
            return cg;
        }
        static public double[] RectangleCG(FeNode n1, FeNode n2, FeNode n3, FeNode n4, FeNode n5, FeNode n6,
                                           FeNode n7, FeNode n8, out double area)
        {
            double[] cg = new double[3];
            double[] cg1;
            double a;
            //
            cg1 = TriangleCG(n8, n1, n5, out a);
            cg[0] += cg1[0] * a;
            cg[1] += cg1[1] * a;
            cg[2] += cg1[2] * a;
            area = a;
            //
            cg1 = TriangleCG(n8, n5, n7, out a);
            cg[0] += cg1[0] * a;
            cg[1] += cg1[1] * a;
            cg[2] += cg1[2] * a;
            area += a;
            //
            cg1 = TriangleCG(n8, n7, n4, out a);
            cg[0] += cg1[0] * a;
            cg[1] += cg1[1] * a;
            cg[2] += cg1[2] * a;
            area += a;
            //
            //
            cg1 = TriangleCG(n6, n3, n7, out a);
            cg[0] += cg1[0] * a;
            cg[1] += cg1[1] * a;
            cg[2] += cg1[2] * a;
            area += a;
            //
            cg1 = TriangleCG(n6, n7, n5, out a);
            cg[0] += cg1[0] * a;
            cg[1] += cg1[1] * a;
            cg[2] += cg1[2] * a;
            area += a;
            //
            cg1 = TriangleCG(n6, n5, n2, out a);
            cg[0] += cg1[0] * a;
            cg[1] += cg1[1] * a;
            cg[2] += cg1[2] * a;
            area += a;
            //
            cg[0] /= area;
            cg[1] /= area;
            cg[2] /= area;
            //
            return cg;
        }
    }
}
