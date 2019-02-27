using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeGlobals
{
    [Serializable]
    public class CompareDoubleArray : IEqualityComparer<double[]>
    {
        public bool Equals(double[] x, double[] y)
        {
            if (x.Length != y.Length) return false;

            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] != y[i]) return false;
            }
            return true;
        }

        public int GetHashCode(double[] x)
        {
            if (x == null) { return 0; }
            int hash = 23;
            for (int i = 0; i < x.Length; i++)
            {
                hash = hash * 31 + Hash(x[i]);
            }
            return hash;
        }

        private static int Hash(double value)
        {
            long bits = BitConverter.DoubleToInt64Bits(value);
            return (int)(bits ^ (bits >> 32));
        }
    }
}
