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
            double t = -Vec3D.DotProduct(v1 - v0, v2 - v1) / (v2 - v1).Len2;

            if (t <= 0)
                return (v0 - v1).Len;
            else if (t >= 1)
                return (v0 - v2).Len;
            else
                return Vec3D.CrossProduct(v0 - v1, v0 - v2).Len / (v2 - v1).Len;
        }
    }
}
