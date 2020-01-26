using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeGlobals
{
    public static class Geometry
    {
        public static double PointToSegmentDistance(double[] x, double[] a1, double[] a2)
        {
            Vec3D v0 = new Vec3D(x);
            Vec3D v1 = new Vec3D(a1);
            Vec3D v2 = new Vec3D(a2);

            // http://mathworld.wolfram.com/Point-LineDistance3-Dimensional.html
            double t = -Vec3D.DotProduct(v1 - v0, v2 - v1) / (v2 - v1).Len2;

            if (t <= 0)
                return (v0 - v1).Len;
            else if (t >= 1)
                return (v0 - v2).Len;
            else
                return Vec3D.CrossProduct(v0 - v1, v0 - v2).Len / (v2 - v1).Len;
        }
        public static double SegmentToSegmentDistance(double[] a1, double[] a2, double[] b1, double[] b2)
        {
            Vec3D p1 = new Vec3D(a1);
            Vec3D d1 = new Vec3D(a2) - p1;          // Direction
            Vec3D p2 = new Vec3D(b1);
            Vec3D d2 = new Vec3D(b2) - p2;          // Direction
            // https://en.wikipedia.org/wiki/Skew_lines#Distance
            // v1 = p1 + t1*d1
            // v2 = p2 + t2*d2
            // Min distance line normal
            Vec3D n = Vec3D.CrossProduct(d1, d2);
            // The plane formed by the translations of Line 1 along n 
            Vec3D n1 = Vec3D.CrossProduct(d1, Vec3D.CrossProduct(d2, d1));
            // The plane formed by the translations of Line 2 along n 
            Vec3D n2 = Vec3D.CrossProduct(d2, Vec3D.CrossProduct(d1, d2));
            //
            double div1 = Vec3D.DotProduct(d1, n2);
            double div2 = Vec3D.DotProduct(d2, n1);
            // Unparallel lines
            if (div1 != 0 && div2 != 0)
            {
                double t1 = Vec3D.DotProduct(p2 - p1, n2) / div1;
                double t2 = Vec3D.DotProduct(p1 - p2, n1) / div2;
                //
                Vec3D c1;
                if (t1 <= 0) c1 = p1;
                else if (t1 >= 1) c1 = new Vec3D(a2);
                else c1 = p1 + t1 * d1;
                Vec3D c2;
                if (t2 <= 0) c2 = p2;
                else if (t2 >= 1) c2 = new Vec3D(b2);
                else c2 = p2 + t2 * d2;
                //
                return (c1 - c2).Len;
            }
            // Parellel or coincident lines
            else
            {
                double dist = Vec3D.CrossProduct(p1 - p2, p1 - (p2 + d2)).Len / d2.Len;
                // Coincident lines - two poit to segment possibilities
                if (dist == 0)
                {
                    return Math.Min(Math.Min((p1 - p2).Len, (p1 - (p2 + d2)).Len),
                                    Math.Min(((p1 + d1) - p2).Len, ((p1 + d1) - (p2 + d2)).Len));
                }
                // Parellel lines
                else
                {
                    return dist;
                }
            }
        }
    }
}
