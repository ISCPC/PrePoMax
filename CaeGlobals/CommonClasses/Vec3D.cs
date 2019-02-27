using System;

namespace CaeGlobals
{
    public class Vec3D
    {
        // Variables                                                                                                                
        public double X;
        public double Y;
        public double Z;


        // Properties                                                                                                               
        public double Len { get { return Math.Sqrt(X * X + Y * Y + Z * Z); } }
        public double Len2 { get { return X * X + Y * Y + Z * Z; } }
        public double[] Coor { get { return new double[] { X, Y, Z }; } }


        // Constructors                                                                                                             
        public Vec3D()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }
        public Vec3D(double x0, double y0, double z0)
        {
            X = x0;
            Y = y0;
            Z = z0;
        }
        public Vec3D(double[] xyz)
        {
            X = xyz[0];
            Y = xyz[1];
            Z = xyz[2];
        }


        // Methods                                                                                                                  
        public void Sub(Vec3D a)
        {
            X = X - a.X;
            Y = Y - a.Y;
            Z = Z - a.Z;
        }
        public double Normalize()
        {
            double n = Math.Sqrt(X * X + Y * Y + Z * Z);
            if (n > 0)
            {
                X = X / n;
                Y = Y / n;
                Z = Z / n;
            }
            return n;
        }
        public void MultBy(double a)
        {
            X = X * a;
            Y = Y * a;
            Z = Z * a;
        }

        #region STATIC UTILITIES

        // Copy
        public static void Copy(double[] vec1, ref double[] vec)
        {
            vec[0] = vec1[0];
            vec[1] = vec1[1];
            vec[2] = vec1[2];
        }
        public static void CopyNormalize(double[] vec1, ref double[] vec)
        {
            vec[0] = vec1[0];
            vec[1] = vec1[1];
            vec[2] = vec1[2];
            Vec3D.Normalize(ref vec);
        }

        // Normalize vector
        public static void Normalize(ref double[] vec)
        {
            double n = Math.Sqrt(vec[0] * vec[0] + vec[1] * vec[1] + vec[2] * vec[2]);
            if (n > 0)
            {
                vec[0] = vec[0] / n;
                vec[1] = vec[1] / n;
                vec[2] = vec[2] / n;
            }
        }

        // Get normalized direction vector
        public static void GetNorDirVec(double[] p1, double[] p2, ref double[] vec)
        {
            vec[0] = p2[0] - p1[0];
            vec[1] = p2[1] - p1[1];
            vec[2] = p2[2] - p1[2];
            Vec3D.Normalize(ref vec);
        }
        public static Vec3D GetNorDirVec(double[] p1, double[] p2)
        {
            Vec3D vector = new Vec3D();
            vector.X = p2[0] - p1[0];
            vector.Y = p2[1] - p1[1];
            vector.Z = p2[2] - p1[2];
            vector.Normalize();
            return vector;
        }

        // Add
        public static Vec3D operator +(Vec3D v1, Vec3D v2)
        {
            return new Vec3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }
        // Subtract
        public static Vec3D operator -(Vec3D v1, Vec3D v2)
        {
            return new Vec3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }
        // Multiply
        public static Vec3D operator *(double a, Vec3D v)
        {
            return new Vec3D(a * v.X, a * v.Y, a * v.Z);
        }
        public static Vec3D operator *(Vec3D v, double a)
        {
            return new Vec3D(a * v.X, a * v.Y, a * v.Z);
        }
        // Cross product
        public static void CrossProduct(Vec3D u, Vec3D v, ref Vec3D vec)
        {
            vec.X = u.Y * v.Z - v.Y * u.Z;
            vec.Y = v.X * u.Z - u.X * v.Z;
            vec.Z = u.X * v.Y - v.X * u.Y;
        }
        public static Vec3D CrossProduct(Vec3D u, Vec3D v)
        {
            Vec3D vector = new Vec3D();
            vector.X = u.Y * v.Z - v.Y * u.Z;
            vector.Y = v.X * u.Z - u.X * v.Z;
            vector.Z = u.X * v.Y - v.X * u.Y;
            return vector;
        }
        public static void CrossProduct(double[] u, double[] v, ref double[] vec)
        {
            vec[0] = u[1] * v[2] - v[1] * u[2];
            vec[1] = v[0] * u[2] - u[0] * v[2];
            vec[2] = u[0] * v[1] - v[0] * u[1];
        }

        // Dot product
        public static double DotProduct(Vec3D u, Vec3D v)
        {
            return (u.X * v.X + u.Y * v.Y + u.Z * v.Z);
        }
        #endregion

    }
}
