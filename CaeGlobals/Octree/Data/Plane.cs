

namespace Octree
{
    using System;
    using System.Runtime.Serialization;

    //                          
    // a*x + b*y + c*z + d = 0  
    //                          

    public struct Plane
    {
        /// <summary>
        /// Gets or sets the D parameter of the plane.
        /// </summary>
        public double D { get; }

        /// <summary>
        /// Gets or sets the plane normal.
        /// </summary>
        public Point Normal { get; }

        /// <summary>
        /// Creates a new plane with the given parameters.
        /// </summary>
        /// <param name="a">The a parameter of the plane.</param>
        /// <param name="b">The b parameter of the plane.</param>
        /// <param name="c">The c parameter of the plane.</param>
        /// <param name="d">The d parameter of the plane.</param>
        public Plane(double a, double b, double c, double d)
        {
            Normal = new Point(a, b, c).Normalized;
            D = d;
        }

        /// <summary>
        /// Creates a new plane with the given nornal and d parameter.
        /// </summary>
        /// <param name="normal">The normal of the plane.</param>
        /// <param name="d">The d parameter of the plane.</param>
        public Plane(Point normal, double d)
        {
            Normal = normal.Normalized;
            D = d;
        }


        /// <summary>
        /// Returns the distance between the plane and point.
        /// </summary>
        /// <param name="plane">The plane.</param>
        /// <param name="point">The point.</param>
        /// <returns>The distance.</returns>
        public static double Distance(Plane plane, Point point)
        {
            return Point.Dot(plane.Normal, point) + plane.D;
        }
    }
}
