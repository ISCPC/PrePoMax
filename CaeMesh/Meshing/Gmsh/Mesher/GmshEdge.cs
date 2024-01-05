using System;
using System.Collections.Generic;
using System.Linq;

namespace CaeMesh
{
    [Serializable]
    public class GmshEdge : IComparable<GmshEdge>
    {
        public int Id;
        public int[] VertexIds;
        public HashSet<int> SurfaceIds;
        public double Length;
        //
        public GmshEdge(int id, int[] vertexIds, int surfaceId, double length)
        {
            Id = id;
            VertexIds = vertexIds;
            SurfaceIds = new HashSet<int> { surfaceId };
            Length = length;
        }
        //
        public int CompareTo(GmshEdge other)
        {
            if (Id < other.Id) return 1;
            else if (Id > other.Id) return -1;
            else return 0;
        }
    }
}
