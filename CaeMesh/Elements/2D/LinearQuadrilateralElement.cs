using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public class LinearQuadrilateralElement : FeElement2D
    {
        // Variables                                                                                                                
        private static int vtkCellTypeInt = (int)vtkCellType.VTK_QUAD;
        private static double a = 1.0 / 4.0;
        private static double b = 1.0 / 2.0;


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public LinearQuadrilateralElement(int id, int[] nodeIds)
          : base(id, nodeIds)
        {
        }
        public LinearQuadrilateralElement(int id, int partId, int[] nodeIds)
            : base(id, partId, nodeIds)
        {
        }


        // Methods                                                                                                                  
        public override int[] GetVtkNodeIds()
        {
            // return a copy -> ToArray
            return NodeIds.ToArray();
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
            // NEG S1 = 1-4-3-2 . 0-3-2-1
            // POS S2 = 1-2-3-4 . 0-1-2-3
            //     S3 = 1-2 . 0-1
            //     S4 = 2-3 . 1-2
            //     S5 = 3-4 . 2-3
            //     S6 = 4-1 . 3-0
            switch (faceName)
            {
                case FeFaceName.S1:
                    return new int[] { NodeIds[0], NodeIds[3], NodeIds[2], NodeIds[1] };
                case FeFaceName.S2:
                    return new int[] { NodeIds[0], NodeIds[1], NodeIds[2], NodeIds[3] };
                case FeFaceName.S3:
                    return new int[] { NodeIds[0], NodeIds[1] };
                case FeFaceName.S4:
                    return new int[] { NodeIds[1], NodeIds[2] };
                case FeFaceName.S5:
                    return new int[] { NodeIds[2], NodeIds[3] };
                case FeFaceName.S6:
                    return new int[] { NodeIds[3], NodeIds[0] };
                default:
                    throw new NotSupportedException();
            }
        }
        public override int[] GetVtkCellFromFaceName(FeFaceName faceName)
        {
            // NEG S1 = 1-4-3-2 . 0-3-2-1
            // POS S2 = 1-2-3-4 . 0-1-2-3
            //     S3 = 1-2 . 0-1
            //     S4 = 2-3 . 1-2
            //     S5 = 3-4 . 2-3
            //     S6 = 4-1 . 3-0
            switch (faceName)
            {
                case FeFaceName.S1:
                    return new int[] { NodeIds[0], NodeIds[3], NodeIds[2], NodeIds[1] };
                case FeFaceName.S2:
                    return new int[] { NodeIds[0], NodeIds[1], NodeIds[2], NodeIds[3] };
                case FeFaceName.S3:
                    return new int[] { NodeIds[0], NodeIds[1] };
                case FeFaceName.S4:
                    return new int[] { NodeIds[1], NodeIds[2] };
                case FeFaceName.S5:
                    return new int[] { NodeIds[2], NodeIds[3] };
                case FeFaceName.S6:
                    return new int[] { NodeIds[3], NodeIds[0] };
                default:
                    throw new NotSupportedException();
            }
        }
        public override int[][] GetAllVtkCells()
        {
            int[][] cells = new int[6][];
            //
            cells[0] = new int[] { NodeIds[0], NodeIds[3], NodeIds[2], NodeIds[1] };
            cells[1] = new int[] { NodeIds[0], NodeIds[1], NodeIds[2], NodeIds[3] };
            cells[2] = new int[] { NodeIds[0], NodeIds[1] };
            cells[3] = new int[] { NodeIds[1], NodeIds[2] };
            cells[4] = new int[] { NodeIds[2], NodeIds[3] };
            cells[5] = new int[] { NodeIds[3], NodeIds[0] };
            //
            return cells;
        }
        public override Dictionary<FeFaceName, double> GetFaceNamesAndAreasFromNodeSet(HashSet<int> nodeSet,
                                                                                       Dictionary<int, FeNode> nodes)
        {
            int significantNodes = NodeIds.Length;
            bool[] faceNodeIds = new bool[significantNodes];
            //
            int count = 0;
            for (int i = 0; i < significantNodes; i++)
            {
                if (nodeSet.Contains(NodeIds[i]))
                {
                    faceNodeIds[i] = true;
                    count++;
                }
                if (i >= 4 && count <= i - 4) break;
            }
            // POS S2 = 1-2-3-4 . 0-1-2-3
            Dictionary<FeFaceName, double> faces = new Dictionary<FeFaceName, double>();
            //
            if (faceNodeIds[0] && faceNodeIds[1] && faceNodeIds[2] && faceNodeIds[3])
                faces.Add(FeFaceName.S2, GetArea(FeFaceName.S2, nodes));
            //
            return faces;
        }
        public override double[] GetEquivalentForcesFromFaceName(FeFaceName faceName)
        {
            if (faceName == FeFaceName.S1 || faceName == FeFaceName.S2)
                return new double[] { a, a, a, a };
            else if (faceName == FeFaceName.S3 || faceName == FeFaceName.S4 ||
                     faceName == FeFaceName.S5 || faceName == FeFaceName.S6)
                return new double[] { b, b };
            else throw new NotSupportedException();
        }
        public override double GetArea(FeFaceName faceName, Dictionary<int, FeNode> nodes)
        {
            int[] cell = GetVtkCellFromFaceName(faceName);
            if (cell.Length == 4)
                return GeometryTools.RectangleArea(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]], nodes[cell[3]]);
            else if (cell.Length == 2)
                return GeometryTools.EdgeLength(nodes[cell[0]], nodes[cell[1]]);
            else throw new NotSupportedException();
        }
        public override double[] GetCG(FeFaceName faceName, Dictionary<int, FeNode> nodes, out double area)
        {
            int[] cell = GetVtkCellFromFaceName(faceName);
            if (cell.Length == 4)
                return GeometryTools.RectangleCG(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]], nodes[cell[3]], out area);
            else if (cell.Length == 2)
                return GeometryTools.EdgeCG(nodes[cell[0]], nodes[cell[1]], out area);
            else throw new NotSupportedException();
        }
        public override FeElement DeepCopy()
        {
            return new LinearQuadrilateralElement(Id, PartId, NodeIds.ToArray());
        }
    }
}
