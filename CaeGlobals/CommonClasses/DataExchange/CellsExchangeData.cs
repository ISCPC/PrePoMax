using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CaeGlobals
{
    [Serializable]
    public class CellsExchangeData
    {
        public int[] Ids;
        public int[][] CellNodeIds;
        public int[] Types;
        public float[] Values;

        public CellsExchangeData()
        {
            Ids = null;
            CellNodeIds = null;
            Types = null;
            Values = null;
        }
    }
}
