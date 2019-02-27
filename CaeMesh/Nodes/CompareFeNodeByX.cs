using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public class CompareFeNodeByX : IComparer<FeNode>
    {
        public int Compare(FeNode n1, FeNode n2)
        {
            if (n1.X > n2.X) return 1;
            else if (n1.X < n2.X) return -1;
            else return 0;
        }
    }
}
