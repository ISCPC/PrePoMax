﻿using System;

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
        public double[] Coor
        {
            get { return new double[] { X, Y, Z }; }
            set 
            {
                X = value[0];
                Y = value[1];
                Z = value[2];
            }
        }


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
        public Vec3D(Vec3D vec)
        {
            X = vec.X;
            Y = vec.Y;
            Z = vec.Z;
        }


        // Methods                                                                                                                  
        public double Normalize()
        {
            double n = Math.Sqrt(X * X + Y * Y + Z * Z);
            if (n > 0 && n != 1)
            {
                X /= n;
                Y /= n;
                Z /= n;
            }
            return n;
        }
        public void Abs()
        {
            if (X < 0) X = -X;
            if (Y < 0) Y = -Y;
            if (Z < 0) Z = -Z;
        }
        public Vec3D DeepCopy()
        {
            return new Vec3D(this);
        }
        public double[] CoorRounded(int numDigits)
        {
            return new double[] { Math.Round(X, numDigits),
                                  Math.Round(Y, numDigits),
                                  Math.Round(Z, numDigits) };
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
        public static Vec3D GetDirVec(double[] p1, double[] p2)
        {
            Vec3D vector = new Vec3D();
            vector.X = p2[0] - p1[0];
            vector.Y = p2[1] - p1[1];
            vector.Z = p2[2] - p1[2];
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

        // Angle
        public static double GetAngleAtP2Deg(Vec3D p1, Vec3D p2, Vec3D p3)
        {
            Vec3D v1 = p1 - p2;
            Vec3D v2 = p3 - p2;
            v1.Normalize();
            v2.Normalize();
            //
            double dot = Vec3D.DotProduct(v1, v2);
            if (dot > 1) dot = 1;
            else if (dot < -1) dot = -1;
            double angle = Math.Acos(dot);
            // Convert the angle from radians to degrees
            angle *= (180.0 / Math.PI);
            //
            return angle;
        }

        // Circle
        public static void GetCircle(Vec3D baseV1, Vec3D baseV2, Vec3D baseV3, out double r, out Vec3D center, out Vec3D axis)
        {
            Vec3D n12 = baseV1 - baseV2;
            Vec3D n21 = baseV2 - baseV1;
            Vec3D n23 = baseV2 - baseV3;
            Vec3D n32 = baseV3 - baseV2;
            Vec3D n13 = baseV1 - baseV3;
            Vec3D n31 = baseV3 - baseV1;
            Vec3D n12xn23 = CrossProduct(n12, n23);
            //
            r = (n12.Len * n23.Len * n31.Len) / (2 * n12xn23.Len);
            //
            double denominator = 2 * n12xn23.Len2;
            double alpha = n23.Len2 * DotProduct(n12, n13) / denominator;
            double beta = n13.Len2 * DotProduct(n21, n23) / denominator;
            double gama = n12.Len2 * DotProduct(n31, n32) / denominator;
            //
            center = alpha * baseV1 + beta * baseV2 + gama * baseV3;
            //
            axis = CrossProduct(n21, n32);
            axis.Normalize();
        }
        public static void GetCircle(Vec3D baseV1, Vec3D baseV2, Vec3D baseV3, out double r, out double arcAngleDeg, 
                                     out Vec3D center, out Vec3D axis)
        {
            Vec3D n12 = baseV1 - baseV2;
            Vec3D n21 = baseV2 - baseV1;
            Vec3D n23 = baseV2 - baseV3;
            Vec3D n32 = baseV3 - baseV2;
            Vec3D n13 = baseV1 - baseV3;
            Vec3D n31 = baseV3 - baseV1;
            Vec3D n12xn23 = Vec3D.CrossProduct(n12, n23);
            //
            r = (n12.Len * n23.Len * n31.Len) / (2 * n12xn23.Len);
            //
            double denominator = 2 * n12xn23.Len2;
            double alpha = n23.Len2 * DotProduct(n12, n13) / denominator;
            double beta = n13.Len2 * DotProduct(n21, n23) / denominator;
            double gama = n12.Len2 * DotProduct(n31, n32) / denominator;
            //
            center = alpha * baseV1 + beta * baseV2 + gama * baseV3;
            //
            axis = CrossProduct(n21, n32);
            axis.Normalize();
            //
            Vec3D r1 = baseV1 - center;
            Vec3D r2 = baseV2 - center;
            Vec3D r3 = baseV3 - center;
            r1.Normalize();
            r2.Normalize();
            r3.Normalize();
            arcAngleDeg = Math.Acos(DotProduct(r1, r2)) * 180 / Math.PI;
            arcAngleDeg += Math.Acos(DotProduct(r2, r3)) * 180 / Math.PI;
        }
        public static void GetCircleR(Vec3D baseV1, Vec3D baseV2, Vec3D baseV3, out double r)
        {
            Vec3D n12 = baseV1 - baseV2;
            Vec3D n23 = baseV2 - baseV3;
            Vec3D n31 = baseV3 - baseV1;
            Vec3D n12xn23 = CrossProduct(n12, n23);
            //
            r = (n12.Len * n23.Len * n31.Len) / (2 * n12xn23.Len);
        }



        #endregion

    }
}
