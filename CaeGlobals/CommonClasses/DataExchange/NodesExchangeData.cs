using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CaeGlobals
{
    [Serializable]
    public class NodesExchangeData
    {
        public int[] Ids;
        public double[][] Coor;
        public double[][] Normals;
        public float[] Values;

        public NodesExchangeData()
        {
            Ids = null;
            Coor = null;
            Normals = null;
            Values = null;
        }
    }
}
