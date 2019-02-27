using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeGlobals
{
    [Serializable]
    public class CompareIntArray : IEqualityComparer<int[]>
    {
        public bool Equals(int[] x, int[] y)
        {
            if (x.Length != y.Length) return false;

            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] != y[i]) return false;
            }
            return true;
        }

        public int GetHashCode(int[] x)
        {
            int hash = 23;
            for (int i = 0; i < x.Length; i++)
            {
                hash = hash * 31 + x[i];
            }
            return hash;
        }
    }
}
