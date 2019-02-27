using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public class CompareFeNodeCoods : IEqualityComparer<FeNode>
    {
        public bool Equals(FeNode n1, FeNode n2)
        {
            if (n1.X != n2.X) return false;
            else if (n1.Y != n2.Y) return false;
            else if (n1.Z != n2.Z) return false;
            else return true;
        }

        public int GetHashCode(FeNode n)
        {
            int hash = 23;
            hash = hash * 31 + Hash(n.X);
            hash = hash * 31 + Hash(n.Y);
            hash = hash * 31 + Hash(n.Z);
            return hash;
        }

        private static int Hash(double value)
        {
            long bits = BitConverter.DoubleToInt64Bits(value);
            return (int)(bits ^ (bits >> 32));
        }
    }
}
