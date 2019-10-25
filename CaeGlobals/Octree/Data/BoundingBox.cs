// <copyright file="BoundingBox.cs">
//     Distributed under the BSD Licence (see LICENCE file).
//     
//     Copyright (c) 2014, Nition, http://www.momentstudio.co.nz/
//     Copyright (c) 2017, Máté Cserép, http://codenet.hu
//     All rights reserved.
// </copyright>
namespace Octree
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an axis aligned bounding box (AABB).
    /// </summary>
    /// <remarks>
    /// This class was inspired by the Bounds type of the Unity Engine and 
    /// designed with the exact same interface to provide maximum compatibility.
    /// </remarks>
    public struct BoundingBox
    {
        Point _min;
        Point _max;
        Point _center;
        Point _extents;

        /// <summary>
        /// Gets or sets the center of the bounding box.
        /// </summary>
        public Point Center
        {
            get { return _center; }
            set
            {
                _center = value;
                UpdateMinMaxFromCenterOrExtents();
            }
        }

        /// <summary>
        /// Gets or sets the extents of the bounding box. This is always half of the <see cref="Size"/>.
        /// </summary>
        public Point Extents
        {
            get { return _extents; }
            set
            {
                _extents = value;
                UpdateMinMaxFromCenterOrExtents();
            }
        }

        /// <summary>
        /// Gets or sets the size of the bounding box. This is always twice as large as the <see cref="Extents"/>.
        /// </summary>
        public Point Size
        {
            get { return Extents * 2; }
            set { Extents = value * 0.5f; }
        }

        /// <summary>
        /// Gets or sets the minimal point of the box.
        /// </summary>
        /// <remarks>
        /// This is always equal to <c>center-extents</c>.
        /// </remarks>
        public Point Min
        {
            get { return _min; }
            set { SetMinMax(value, _max); }
        }

        /// <summary>
        /// Gets or sets the maximal point of the box.
        /// </summary>
        /// <remarks>
        /// This is always equal to <c>center+extents</c>.
        /// </remarks>
        public Point Max
        {
            get { return _max; }
            set { SetMinMax(_min, value); }
        }

        private void UpdateMinMaxFromCenterOrExtents()
        {
            SetMinMax(Center - Extents, Center + Extents);
        }

        /// <summary>
        /// Creates a new bounding box.
        /// </summary>
        /// <param name="center">The center of the box.</param>
        /// <param name="size">The size of the box.</param>
        public BoundingBox(Point center, Point size)
        {
            _center = center;
            _extents = size * 0.5f;
            _min = _max = new Point();
            UpdateMinMaxFromCenterOrExtents();
        }

        /// <summary>
        /// Sets the bounds to the min and max value of the box.
        /// </summary>
        /// <param name="min">The minimal point.</param>
        /// <param name="max">The maximal point.</param>
        public void SetMinMax(Point min, Point max)
        {
            _min = min;
            _max = max;
            _extents = (_max - _min) * 0.5f;
            _center = _min + _extents;
        }

        /// <summary>
        /// Grows the bounding box include the point.
        /// </summary>
        /// <param name="point">The specified point to include.</param>
        public void Encapsulate(Point point)
        {
            SetMinMax(Point.Min(Min, point), Point.Max(Max, point));
        }

        /// <summary>
        /// Grows the bounding box include the other box.
        /// </summary>
        /// <param name="box">The specified box to include.</param>
        public void Encapsulate(BoundingBox box)
        {
            Encapsulate(box.Center - box.Extents);
            Encapsulate(box.Center + box.Extents);
        }

        /// <summary>
        /// Expands the bounds by increasing its <see cref="Size"/> by <paramref name="amount"/> along each side.
        /// </summary>
        /// <param name="amount">The expansions for each dimension.</param>
        public void Expand(double amount)
        {
            amount *= 0.5f;
            Extents += new Point(amount, amount, amount);
        }

        /// <summary>
        /// Expands the bounds by increasing its <see cref="Size"/> by <paramref name="amount"/> along each side.
        /// </summary>
        /// <param name="amount">The expansions for each dimension in order.</param>
        public void Expand(Point amount)
        {
            Extents += amount * 0.5f;
        }

        /// <summary>
        /// Determines whether the box contains the point.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <returns><c>true</c> if the box contains the point; otherwise, <c>false</c>.</returns>
        public bool Contains(Point point)
        {
            return 
                Min.X <= point.X && Max.X >= point.X && 
                Min.Y <= point.Y && Max.Y >= point.Y && 
                Min.Z <= point.Z && Max.Z >= point.Z;
        }

        /// <summary>
        /// Determines whether the bounding box intersects with another box.
        /// </summary>
        /// <param name="box">The box to test.</param>
        /// <returns><c>true</c> if the bounding box intersects with another box, <c>false</c> otherwise.</returns>
        public bool Intersects(BoundingBox box)
        {
            return 
                Min.X <= box.Max.X && Max.X >= box.Min.X && 
                Min.Y <= box.Max.Y && Max.Y >= box.Min.Y && 
                Min.Z <= box.Max.Z && Max.Z >= box.Min.Z;
        }

        /// <summary>
        /// Determines whether the bounding box intersects with a ray.
        /// </summary>
        /// <param name="ray">The ray to test.</param>
        /// <returns><c>true</c> if the box intersects with the ray, <c>false</c> otherwise.</returns>
        public bool IntersectRay(Ray ray)
        {
            double distance;
            return IntersectRay(ray, out distance);
        }

        /// <summary>
        /// Determines whether the bounding box intersects with a ray.
        /// </summary>
        /// <param name="ray">The ray to test.</param>
        /// <param name="distance">The calculated distance from the origin of the ray to the box along the ray.</param>
        /// <returns><c>true</c> if the box intersects with the ray, <c>false</c> otherwise.</returns>
        public bool IntersectRay(Ray ray, out double distance)
        {
            Point dirFrac = new Point(
                1f / ray.Direction.X,
                1f / ray.Direction.Y,
                1f / ray.Direction.Z
            );

            double t1 = (Min.X - ray.Origin.X) * dirFrac.X;
            double t2 = (Max.X - ray.Origin.X) * dirFrac.X;
            double t3 = (Min.Y - ray.Origin.Y) * dirFrac.Y;
            double t4 = (Max.Y - ray.Origin.Y) * dirFrac.Y;
            double t5 = (Min.Z - ray.Origin.Z) * dirFrac.Z;
            double t6 = (Max.Z - ray.Origin.Z) * dirFrac.Z;

            double tmin = Math.Max(Math.Max(Math.Min(t1, t2), Math.Min(t3, t4)), Math.Min(t5, t6));
            double tmax = Math.Min(Math.Min(Math.Max(t1, t2), Math.Max(t3, t4)), Math.Max(t5, t6));

            // if tmax < 0, ray (line) is intersecting AABB, but the whole AABB is behind us
            if (tmax < 0)
            {
                distance = tmax;
                return false;
            }

            // if tmin > tmax, ray doesn't intersect AABB
            if (tmin > tmax)
            {
                distance = tmax;
                return false;
            }

            distance = tmin;
            return true;
        }

        /// <summary>
        /// Determines whether the bounding box intersects with a plane.
        /// </summary>
        /// <param name="plane">The plane to test.</param>
        /// <param name="distance">The minimum distance from the bounding box to the plane .</param>
        /// <returns><c>true</c> if the box intersects with the plane, <c>false</c> otherwise.</returns>
        public bool IntersectPlane(Plane plane, out double distance)
        {
            // https://gdbooks.gitbooks.io/3dcollisions/content/Chapter2/static_aabb_plane.html

            // Compute the projection interval radius of b onto L(t) = b.c + t * p.n
            double r = Extents.X * Math.Abs(plane.Normal.X) + Extents.Y * Math.Abs(plane.Normal.Y) + Extents.Z * Math.Abs(plane.Normal.Z);
            
            // Compute distance of box center from plane
            double s = Point.Dot(plane.Normal, Center) + plane.D;

            // Intersection occurs when distance s falls within [-r,+r] interval
            if (Math.Abs(s) <= r)
            {
                distance = 0;
                return true;
            }
            else
            {
                if (s > 0) distance = s - r;
                else distance = s + r;
                return false;
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return Center.GetHashCode() ^ Extents.GetHashCode() << 2;
        }

        /// <summary>
        /// Determines whether the specified object as a <see cref="BoundingBox" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="BoundingBox" /> object to compare with this instance.</param>
        /// <returns><c>true</c> if the specified box is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object other)
        {
            bool result;
            if (!(other is BoundingBox))
            {
                result = false;
            }
            else
            {
                BoundingBox box = (BoundingBox)other;
                result = (Center.Equals(box.Center) && Extents.Equals(box.Extents));
            }
            return result;
        }

        /// <summary>
        /// Returns a nicely formatted string for this bounding box.
        /// </summary>
        public override string ToString()
        {
            return String.Format("Center: {0}, Extents: {1}", 
                Center, 
                Extents
            );
        }

        /// <summary>
        /// Returns a nicely formatted string for this bounding box.
        /// </summary>
        /// <param name="format">The format for the center and the extent.</param>
        public string ToString(string format)
        {
            return String.Format("Center: {0}, Extents: {1}",
                Center.ToString(format),
                Extents.ToString(format)
            );
        }

        /// <summary>
        /// Determines whether two bounding boxes are equal.
        /// </summary>
        /// <param name="lhs">The first box.</param>
        /// <param name="rhs">The second box.</param>
        public static bool operator ==(BoundingBox lhs, BoundingBox rhs)
        {
            return lhs.Center == rhs.Center && lhs.Extents == rhs.Extents;
        }

        /// <summary>
        /// Determines whether two bounding boxes are different.
        /// </summary>
        /// <param name="lhs">The first box.</param>
        /// <param name="rhs">The second box.</param>
        public static bool operator !=(BoundingBox lhs, BoundingBox rhs)
        {
            return !(lhs == rhs);
        }
    }
}

