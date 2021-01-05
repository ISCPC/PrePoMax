using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public class LinearTriangleElement : FeElement2D
    {
        // Variables                                                                                                                
        private static int vtkCellTypeInt = (int)vtkCellType.VTK_TRIANGLE;
        private static double a = 1.0 / 3.0;


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public LinearTriangleElement(int id, int[] nodeIds)
         : base(id, nodeIds)
        {
        }
        public LinearTriangleElement(int id, int partId, int[] nodeIds)
            : base(id, partId, nodeIds)
        {
        }


        // Methods                                                                                                                  
        public override int[] GetVtkNodeIds()
        {
            // return a copy -> ToArray
            return NodeIds.ToArray();
            //
            // Return S1
            //return new int[] { NodeIds[0], NodeIds[2], NodeIds[1] };
        }
        public override int GetVtkCellType()
        {
            return vtkCellTypeInt;
        }
        public override FeFaceName GetFaceNameFromSortedNodeIds(int[] nodeIds)
        {
            throw new NotImplementedException();
        }
        public override int[] GetNodeIdsFromFaceName(FeFaceName faceName)
        {
            // NEG S1 = 1-3-2 . 0-2-1
            // POS S2 = 1-2-3 . 0-1-2
            switch (faceName)
            {
                case FeFaceName.S1:
                    return new int[] { NodeIds[0], NodeIds[2], NodeIds[1] };
                case FeFaceName.S2:
                    return new int[] { NodeIds[0], NodeIds[1], NodeIds[2] };
                default:
                    throw new NotSupportedException();
            }
        }
        public override int[] GetVtkCellFromFaceName(FeFaceName faceName)
        {
            // NEG S1 = 1-3-2 . 0-2-1
            // POS S2 = 1-2-3 . 0-1-2
            switch (faceName)
            {
                case FeFaceName.S1:
                    return new int[] { NodeIds[0], NodeIds[2], NodeIds[1] };
                case FeFaceName.S2:
                    return new int[] { NodeIds[0], NodeIds[1], NodeIds[2] };
                default:
                    throw new NotSupportedException();
            }
        }
        public override int[][] GetAllVtkCells()
        {
            int[][] cells = new int[5][];
            //
            cells[0] = new int[] { NodeIds[0], NodeIds[2], NodeIds[1] };
            cells[1] = new int[] { NodeIds[0], NodeIds[1], NodeIds[2] };
            cells[2] = new int[] { NodeIds[0], NodeIds[1] };
            cells[3] = new int[] { NodeIds[1], NodeIds[2] };
            cells[4] = new int[] { NodeIds[2], NodeIds[0] };
            //
            return cells;
        }
        public override Dictionary<FeFaceName, double> GetFaceNamesAndAreasFromNodeSet(HashSet<int> nodeSet,
                                                                                       Dictionary<int, FeNode> nodes)
        {
            bool[] faceNodeIds = new bool[NodeIds.Length];
            //
            int count = 0;
            for (int i = 0; i < NodeIds.Length; i++)
            {
                if (nodeSet.Contains(NodeIds[i]))
                {
                    faceNodeIds[i] = true;
                    count++;
                }
                if (i >= 1 && count <= i - 1) break;
            }
            // POS S2 = 1-2-3 . 0-1-2
            Dictionary<FeFaceName, double> faces = new Dictionary<FeFaceName, double>();
            //
            if (count == 3)
            {
                if (faceNodeIds[0] && faceNodeIds[1] && faceNodeIds[2]) faces.Add(FeFaceName.S2, GetArea(FeFaceName.S2, nodes));
            }
            //
            return faces;
        }
        public override double[] GetEquivalentForcesFromFaceName(FeFaceName faceName)
        {
            return new double[] { a, a, a };
        }
        public override double GetArea(FeFaceName faceName, Dictionary<int, FeNode> nodes)
        {
            int[] cell = GetVtkCellFromFaceName(faceName);
            return GeometryTools.TriangleArea(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]]);
        }
        public override double[] GetCG(FeFaceName faceName, Dictionary<int, FeNode> nodes, out double area)
        {
            int[] cell = GetVtkCellFromFaceName(faceName);
            double[] cg = GeometryTools.TriangleCG(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]], out area);
            return cg;
        }
        public override FeElement DeepCopy()
        {
            return new LinearTriangleElement(Id, PartId, NodeIds.ToArray());
        }
    }
}
