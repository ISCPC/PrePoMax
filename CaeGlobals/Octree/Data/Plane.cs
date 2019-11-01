

namespace Octree
{
    using System;
    using System.Runtime.Serialization;

    //                          
    // a*x + b*y + c*z + d = 0  
    //                          
    [Serializable]
    public class Plane
    {
        private Point _point;
        private Point _normal;
        private double _d;
        /// <summary>
        /// Gets or sets the D parameter of the plane.
        /// </summary>
        public double D
        {
            get { return _d; }
            set
            {
                _d = value;
                _point.Coor = (_normal * -_d).Coor;
            }
        }

        /// <summary>
        /// Gets or sets the plane normal.
        /// </summary>
        public Point Normal
        {
            get { return _normal; }
            set
            {
                _normal.Coor = value.Coor;
                // point remains the same
                _d = -(Point.Dot(_normal, _point));
            }
        }

        /// <summary>
        /// Gets or sets the plane point.
        /// </summary>
        public Point Point
        {
            get
            {
                return _point;
            }
            set
            {
                _point.Coor = value.Coor;
                // normal remains the same
                _d = -(Point.Dot(_normal, _point));

            }
        }

        /// <summary>
        /// Creates a new plane with the given parameters.
        /// </summary>
        /// <param name="point">The point on the plane.</param>
        /// <param name="normal">The normal of the plane.</param>
        public Plane(double[] point, double[] normal)
        {
            SetPointAndNormal(point, normal);
        }

        /// <summary>
        /// Creates a new plane with the given parameters.
        /// </summary>
        /// <param name="a">The a parameter of the plane.</param>
        /// <param name="b">The b parameter of the plane.</param>
        /// <param name="c">The c parameter of the plane.</param>
        /// <param name="d">The d parameter of the plane.</param>
        public Plane(double a, double b, double c, double d)
        {
            _normal = new Point(a, b, c).Normalized;
            _d = d;
            _point = _normal * -_d;
        }

        /// <summary>
        /// Creates a new plane with the given nornal and d parameter.
        /// </summary>
        /// <param name="normal">The normal of the plane.</param>
        /// <param name="d">The d parameter of the plane.</param>
        public Plane(Point normal, double d)
        {
            _normal = normal.Normalized;
            _d = d;
            _point = _normal * -_d;
        }

        public void SetPointAndNormal(double[] point, double[] normal)
        {
            _point = new Point(point);
            _normal = new Point(normal).Normalized;
            _d = -(Point.Dot(_normal, _point));
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
