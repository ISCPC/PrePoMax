using System;
using System.Collections.Generic;
using System.Linq;

namespace CaeMesh
{
    [Serializable]
    public class GmshSurface : IComparable<GmshSurface>
    {
        public int[] OppositeEdgesA;
        public int[] OppositeEdgesB;
        public int Id;
        public int[] VertexIds;
        public int[] EdgeIds;
        private int[][] _edgeVertexIds;
        public bool Triangular;
        public bool Quadrangular;
        public bool HasHole;
        public bool Collapsed;
        public bool Transfinite;
        public bool Recombine;
        //
        public GmshSurface(int id, int[] vertexIds, int[] edgeIds, double[] edgeLengths, int[][] edgeVertexIds)
        {
            Id = id;
            VertexIds = vertexIds;
            EdgeIds = edgeIds;
            _edgeVertexIds = edgeVertexIds;
            Triangular = false;
            Quadrangular = false;
            HasHole = false;
            Collapsed = false;
            Transfinite = false;
            Recombine = false;
            // Fix collapsed triangular faces
            if (VertexIds.Length == 3 && edgeIds.Length == 4)
            {
                int count = 0;
                int[] fixedEdgeIds = new int[3];
                double[] fixedEdgeLengths = new double[3];
                int[][] fixedEdgeVertexIds = new int[3][];
                for (int i = 0; i < _edgeVertexIds.Length; i++)
                {
                    if (_edgeVertexIds[i][0] != _edgeVertexIds[i][1])
                    {
                        if (count < 3)
                        {
                            fixedEdgeIds[count] = edgeIds[i];
                            fixedEdgeLengths[count] = edgeLengths[i];
                            fixedEdgeVertexIds[count] = _edgeVertexIds[i];
                            count++;
                        }
                        else Collapsed = true;
                    }
                }
                if (!Collapsed)
                {
                    edgeIds = fixedEdgeIds;
                    edgeLengths = fixedEdgeLengths;
                    _edgeVertexIds = fixedEdgeVertexIds;
                }
            }
            //
            if (VertexIds.Length == 3 && edgeIds.Length == 3)
            {
                double delta1 = Math.Abs(edgeLengths[0] - edgeLengths[1]);
                double delta2 = Math.Abs(edgeLengths[1] - edgeLengths[2]);
                double delta3 = Math.Abs(edgeLengths[2] - edgeLengths[0]);
                double min = Math.Min(delta1, delta2);
                min = Math.Min(min, delta3);
                //
                int firstVertexId;
                if (delta1 == min)
                {
                    firstVertexId = _edgeVertexIds[0].Intersect(_edgeVertexIds[1]).First();
                    OppositeEdgesA = new int[2] { edgeIds[0], edgeIds[1] };
                }
                else if (delta2 == min)
                {
                    firstVertexId = _edgeVertexIds[1].Intersect(_edgeVertexIds[2]).First();
                    OppositeEdgesA = new int[2] { edgeIds[1], edgeIds[2] };
                }
                else
                {
                    firstVertexId = _edgeVertexIds[2].Intersect(_edgeVertexIds[0]).First();
                    OppositeEdgesA = new int[2] { edgeIds[2], edgeIds[0] };
                }
                Queue<int> queue = new Queue<int>(VertexIds);
                while (queue.Peek() != firstVertexId)
                    queue.Enqueue(queue.Dequeue());
                VertexIds = queue.ToArray();
                //
                OppositeEdgesB = null;
                //
                Triangular = true;
                Transfinite = true;
            }
            else if (VertexIds.Length == 4 && edgeIds.Length == 4)
            {
                // Find opposite edge to the first edge
                OppositeEdgesA = new int[2] { edgeIds[0], -1 };
                for (int i = 1; i < edgeIds.Length; i++)
                {
                    if (edgeVertexIds[0].Intersect(edgeVertexIds[i]).Count() == 0) OppositeEdgesA[1] = edgeIds[i];
                    else if (edgeVertexIds[0].Intersect(edgeVertexIds[i]).Count() == 2) HasHole = true;
                }
                OppositeEdgesB = edgeIds.Except(OppositeEdgesA).ToArray();
                //
                Quadrangular = true;
                if (!HasHole) Transfinite = true;
            }
            else if (VertexIds.Length != edgeIds.Length)
                Collapsed = true;
        }
        public void SetFirstVertexId(int firstVertexId)
        {
            Queue<int> queue = new Queue<int>(VertexIds);
            while (queue.Peek() != firstVertexId)
                queue.Enqueue(queue.Dequeue());
            VertexIds = queue.ToArray();
            //
            if (firstVertexId == _edgeVertexIds[0].Intersect(_edgeVertexIds[1]).First())
            {
                OppositeEdgesA = new int[2] { EdgeIds[0], EdgeIds[1] };
            }
            else if (firstVertexId == _edgeVertexIds[1].Intersect(_edgeVertexIds[2]).First())
            {
                OppositeEdgesA = new int[2] { EdgeIds[1], EdgeIds[2] };
            }
            else if (firstVertexId == _edgeVertexIds[2].Intersect(_edgeVertexIds[0]).First())
            {
                OppositeEdgesA = new int[2] { EdgeIds[2], EdgeIds[0] };
            }
            //
            OppositeEdgesB = null;
        }
        //
        public int CompareTo(GmshSurface other)
        {
            if (Id < other.Id) return 1;
            else if (Id > other.Id) return -1;
            else return 0;
        }
    }
}
