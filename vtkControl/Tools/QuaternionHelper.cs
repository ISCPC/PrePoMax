using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;


namespace vtkControl
{
    static class QuaternionHelper
    {
        public static double[][] TransponseMatrix3x3(double[][] a)
        {
            double[][] b = new double[3][];
            for (int i = 0; i < 3; i++)
            {
                b[i] = new double[3];
                for (int j = 0; j < 3; j++)
                {
                    b[i][j] = a[j][i];
                }
            }
            return b;
        }

        public static double[][] Matrix3x3FromQuaternion(double[] q)
        {
            //
            // http://www.euclideanspace.com/maths/geometry/rotations/conversions/quaternionToMatrix/index.htm
            //

            double sqw = q[0] * q[0];
            double sqx = q[1] * q[1];
            double sqy = q[2] * q[2];
            double sqz = q[3] * q[3];

            // invs (inverse square length) is only required if quaternion is not already normalised
            double invs = 1 / (sqx + sqy + sqz + sqw);

            double[][] m = new double[3][];
            m[0] = new double[3];
            m[1] = new double[3];
            m[2] = new double[3];
            m[0][0] = (sqx - sqy - sqz + sqw) * invs;           // since sqw + sqx + sqy + sqz =1/invs*invs
            m[1][1] = (-sqx + sqy - sqz + sqw) * invs;
            m[2][2] = (-sqx - sqy + sqz + sqw) * invs;

            double tmp1 = q[1] * q[2];
            double tmp2 = q[3] * q[0];
            m[1][0] = 2.0 * (tmp1 + tmp2) * invs;
            m[0][1] = 2.0 * (tmp1 - tmp2) * invs;

            tmp1 = q[1] * q[3];
            tmp2 = q[2] * q[0];
            m[2][0] = 2.0 * (tmp1 - tmp2) * invs;
            m[0][2] = 2.0 * (tmp1 + tmp2) * invs;
            tmp1 = q[2] * q[3];
            tmp2 = q[1] * q[0];
            m[2][1] = 2.0 * (tmp1 + tmp2) * invs;
            m[1][2] = 2.0 * (tmp1 - tmp2) * invs;

            return m;
        }

        public static IntPtr IntPtrFromQuaternion(double[] q)
        {
            //IntPtr p = Marshal.AllocCoTaskMem(Marshal.SizeOf(q[0]) * q.Length);
            IntPtr p = Marshal.AllocHGlobal(Marshal.SizeOf(q[0]) * q.Length);
            Marshal.Copy(q, 0, p, q.Length);
            return p;
        }

        public static double[] QuaternionFromIntPtr(IntPtr p)
        {
            double[] q = new double[4];
            Marshal.Copy(p, q, 0, q.Length);
            return q;
        }

        public static double[] QuaternionFromMatrix3x3(double[][] a)
        {
            //
            // http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/christian.htm
            //

            double[] q = new double[4];

            double trace = a[0][0] + a[1][1] + a[2][2]; // I removed + 1.0f; see discussion with Ethan
            if (trace > 0) // I changed M_EPSILON to 0
            {
                double s = 0.5f / Math.Sqrt(trace + 1.0f);
                q[0] = 0.25f / s;
                q[1] = (a[2][1] - a[1][2]) * s;
                q[2] = (a[0][2] - a[2][0]) * s;
                q[3] = (a[1][0] - a[0][1]) * s;
            }
            else
            {
                if (a[0][0] > a[1][1] && a[0][0] > a[2][2])
                {
                    double s = 2.0f * Math.Sqrt(1.0f + a[0][0] - a[1][1] - a[2][2]);
                    q[0] = (a[2][1] - a[1][2]) / s;
                    q[1] = 0.25f * s;
                    q[2] = (a[0][1] + a[1][0]) / s;
                    q[3] = (a[0][2] + a[2][0]) / s;
                }
                else if (a[1][1] > a[2][2])
                {
                    double s = 2.0f * Math.Sqrt(1.0f + a[1][1] - a[0][0] - a[2][2]);
                    q[0] = (a[0][2] - a[2][0]) / s;
                    q[1] = (a[0][1] + a[1][0]) / s;
                    q[2] = 0.25f * s;
                    q[3] = (a[1][2] + a[2][1]) / s;
                }
                else
                {
                    double s = 2.0f * Math.Sqrt(1.0f + a[2][2] - a[0][0] - a[1][1]);
                    q[0] = (a[1][0] - a[0][1]) / s;
                    q[1] = (a[0][2] + a[2][0]) / s;
                    q[2] = (a[1][2] + a[2][1]) / s;
                    q[3] = 0.25f * s;
                }
            }
            return q;
        }
        
        public static double[] QuaternionSlerp(double[] qa, double[] qb, double t)
        {
            //
            //http://www.euclideanspace.com/maths/algebra/realNormedAlgebra/quaternions/slerp/
            //

            // quaternion to return
            double[] qm = new double[4];

            // Calculate angle between them.
            double cosHalfTheta = qa[0] * qb[0] + qa[1] * qb[1] + qa[2] * qb[2] + qa[3] * qb[3];
            // if qa=qb or qa=-qb then theta = 0 and we can return qa
            if (Math.Abs(cosHalfTheta) >= 1.0)
            {
                qm[0] = qa[0]; qm[1] = qa[1]; qm[2] = qa[2]; qm[3] = qa[3];
                return qm;
            }
            // Calculate temporary values.
            double halfTheta = Math.Acos(cosHalfTheta);
            double sinHalfTheta = Math.Sqrt(1.0 - cosHalfTheta * cosHalfTheta);
            // if theta = 180 degrees then result is not fully defined
            // we could rotate around any axis normal to qa or qb
            if (Math.Abs(sinHalfTheta) < 0.001)
            { // fabs is floating point absolute
                qm[0] = (qa[0] * 0.5 + qb[0] * 0.5);
                qm[1] = (qa[1] * 0.5 + qb[1] * 0.5);
                qm[2] = (qa[2] * 0.5 + qb[2] * 0.5);
                qm[3] = (qa[3] * 0.5 + qb[3] * 0.5);
                return qm;
            }
            double ratioA = Math.Sin((1 - t) * halfTheta) / sinHalfTheta;
            double ratioB = Math.Sin(t * halfTheta) / sinHalfTheta;
            //calculate Quaternion.
            qm[0] = (qa[0] * ratioA + qb[0] * ratioB);
            qm[1] = (qa[1] * ratioA + qb[1] * ratioB);
            qm[2] = (qa[2] * ratioA + qb[2] * ratioB);
            qm[3] = (qa[3] * ratioA + qb[3] * ratioB);

            return qm;
        }

        public static double[] VectorLerp(double[] v1, double[] v2, double t)
        {
            double[] v = new double[3];

            if (t == 0) return v1;
            else if (t == 1) return v2;

            v[0] = v1[0] + t * (v2[0] - v1[0]);
            v[1] = v1[1] + t * (v2[1] - v1[1]);
            v[2] = v1[2] + t * (v2[2] - v1[2]);

            return v;
        }
    }
}
