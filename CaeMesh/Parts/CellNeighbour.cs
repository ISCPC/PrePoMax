using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    public class CellNeighbour
    {
        // Properties                                                                                                               
        public int Id1 { get; set; }
        public int Id2 { get; set; }
        public int[] Cell1 { get; set; }


        // Constructors                                                                                                             
        public CellNeighbour(int id1, int id2, int[] cell1)
        {
            Id1 = id1;
            Id2 = id2;
            Cell1 = cell1;
        }

        public CellNeighbour(CellNeighbour cellNeighbour)
        {
            Id1 = cellNeighbour.Id1;
            Id2 = cellNeighbour.Id2;
            if (cellNeighbour.Cell1 != null) Cell1 = cellNeighbour.Cell1.ToArray();
        }
    }
}
