using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeGlobals
{
    public static class Geometry
    {
        public static double PointToSegmentDistance(double[] x, double[] p1, double[] p2)
        {
            Vec3D v0 = new Vec3D(x);
            Vec3D v1 = new Vec3D(p1);
            Vec3D v2 = new Vec3D(p2);

            // http://mathworld.wolfram.com/Point-LineDistance3-Dimensional.html
            double d0 = Vec3D.CrossProduct(v0 - v1, v0 - v2).Len / (v2 - v1).Len;
            double d1 = (v0 - v1).Len;
            double d2 = (v0 - v2).Len;
            double d3 = (v1 - v2).Len;

            if (d3 > d1 && d3 > d2)
            {
                // d3 is the longest side of the triangle so the closest point is between segment end points
                return d0;
            }
            else return Math.Min(d1, d2);
        }
    }
}
