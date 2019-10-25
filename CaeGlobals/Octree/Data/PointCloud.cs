using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octree
{
    public class PointCloud : IIntersect
    {
        public int Id;
        public Point[] Points;
        public readonly BoundingBox BoundingBox;

        // Constructors                                                                                                             
        public PointCloud(int id, Point[] points)
        {
            Id = id;
            Points = points;
            BoundingBox = new BoundingBox(Points[0], Point.Zero);
            for (int i = 0; i < Points.Length; i++) BoundingBox.Encapsulate(Points[i]);
        }


        // Methods                                                                                                                  
        public double[][] GetCoor()
        {
            double[][] coor = new double[Points.Length][];
            for (int i = 0; i < Points.Length; i++) coor[i] = Points[i].Coor;
            return coor;
        }

        // Interface
        public bool Intersect(Plane plane, out double distance)
        {
            distance = 0;
            double sign;
            double[] dist = new double[Points.Length];

            // Check if all points are on the same plane side
            dist[0] = Plane.Distance(plane, Points[0]);
            sign = Math.Sign(dist[0]);
            if (sign == 0) return true; // point on plane

            for (int i = 1; i < Points.Length; i++)
            {
                dist[i] = Plane.Distance(plane, Points[i]);
                if (sign != Math.Sign(dist[i])) return true;    // oposite point
            }

            // Find minimal signed distance
            distance = dist[0];
            if (sign > 0)
            {
                // all distances positive
                for (int i = 1; i < dist.Length; i++)
                {
                    if (dist[i] < distance) distance = dist[i];
                }
            }
            else
            {
                // all distances negative
                for (int i = 1; i < dist.Length; i++)
                {
                    if (dist[i] > distance) distance = dist[i];
                }
            }
            return false;
        }
    }
}
