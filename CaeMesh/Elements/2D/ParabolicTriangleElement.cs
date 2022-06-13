using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public class ParabolicTriangleElement : FeElement2D
    {
        // Variables                                                                                                                
        private static readonly int vtkCellTypeInt = (int)vtkCellType.VTK_QUADRATIC_TRIANGLE;
        private static readonly double a = 1.0 / 3.0;
        private static readonly double b = 1.0 / 6.0;
        private static readonly double c = 4.0 / 6.0;


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public ParabolicTriangleElement(int id, int[] nodeIds)
         : base(id, nodeIds)
        {
        }
        public ParabolicTriangleElement(int id, int partId, int[] nodeIds)
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
            // NEG S1 = 1-3-2-6-5-4  . 0-2-1-5-4-3
            // POS S2 = 1-2-3-4-5-6  . 0-1-2-3-4-5
            //     S3 = 1-2-4 . 0-1-3
            //     S4 = 2-3-5 . 1-2-4
            //     S5 = 3-1-6 . 2-0-5
            switch (faceName)
            {
                case FeFaceName.S1:
                    return new int[] { NodeIds[0], NodeIds[2], NodeIds[1], NodeIds[5], NodeIds[4], NodeIds[3] };
                case FeFaceName.S2:
                    return new int[] { NodeIds[0], NodeIds[1], NodeIds[2], NodeIds[3], NodeIds[4], NodeIds[5] };
                case FeFaceName.S3:
                    return new int[] { NodeIds[0], NodeIds[1], NodeIds[3] };
                case FeFaceName.S4:
                    return new int[] { NodeIds[1], NodeIds[2], NodeIds[4] };
                case FeFaceName.S5:
                    return new int[] { NodeIds[2], NodeIds[0], NodeIds[5] };
                default:
                    throw new NotSupportedException();
            }
        }
        public override int[] GetVtkCellFromFaceName(FeFaceName faceName)
        {
            // NEG S1 = 1-3-2-6-5-4  . 0-2-1-5-4-3
            // POS S2 = 1-2-3-4-5-6  . 0-1-2-3-4-5
            //     S3 = 1-2-4 . 0-1-3
            //     S4 = 2-3-5 . 1-2-4
            //     S5 = 3-1-6 . 2-0-5
            switch (faceName)
            {
                case FeFaceName.S1:
                    return new int[] { NodeIds[0], NodeIds[2], NodeIds[1], NodeIds[5], NodeIds[4], NodeIds[3] };
                case FeFaceName.S2:
                    return new int[] { NodeIds[0], NodeIds[1], NodeIds[2], NodeIds[3], NodeIds[4], NodeIds[5] };
                case FeFaceName.S3:
                    return new int[] { NodeIds[0], NodeIds[1], NodeIds[3] };
                case FeFaceName.S4:
                    return new int[] { NodeIds[1], NodeIds[2], NodeIds[4] };
                case FeFaceName.S5:
                    return new int[] { NodeIds[2], NodeIds[0], NodeIds[5] };
                default:
                    throw new NotSupportedException();
            }
        }
        public override int[][] GetAllVtkCells()
        {
            int[][] cells = new int[5][];
            //
            cells[0] = new int[] { NodeIds[0], NodeIds[2], NodeIds[1], NodeIds[5], NodeIds[4], NodeIds[3] };
            cells[1] = new int[] { NodeIds[0], NodeIds[1], NodeIds[2], NodeIds[3], NodeIds[4], NodeIds[5] };
            cells[2] = new int[] { NodeIds[0], NodeIds[1], NodeIds[3] };
            cells[3] = new int[] { NodeIds[1], NodeIds[2], NodeIds[4] };
            cells[4] = new int[] { NodeIds[2], NodeIds[0], NodeIds[5] };
            //
            return cells;
        }
        public override Dictionary<FeFaceName, double> GetFaceNamesAndAreasFromNodeSet(HashSet<int> nodeSet,
                                                                                       Dictionary<int, FeNode> nodes,
                                                                                       bool edgeFaces)
        {
            // Check only first 3 nodes (as in linear element)
            int significantNodes = 3;
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
                // If two or more nodes were missed: break
                if (i + 1 - count >= 2) break;
            }
            Dictionary<FeFaceName, double> faces = new Dictionary<FeFaceName, double>();
            //
            if (edgeFaces)
            {
                if (count >= 2)
                {
                    // S3 = 1-2-4 . 0-1-3
                    // S4 = 2-3-5 . 1-2-4
                    // S5 = 3-1-6 . 2-0-5
                    if (faceNodeIds[0] && faceNodeIds[1]) faces.Add(FeFaceName.S3, GetArea(FeFaceName.S3, nodes));
                    if (faceNodeIds[1] && faceNodeIds[2]) faces.Add(FeFaceName.S4, GetArea(FeFaceName.S4, nodes));
                    if (faceNodeIds[2] && faceNodeIds[0]) faces.Add(FeFaceName.S5, GetArea(FeFaceName.S5, nodes));
                }
            }
            else if (count == 3)
            {
                // POS S2 = 1-2-3 . 0-1-2
                if (faceNodeIds[0] && faceNodeIds[1] && faceNodeIds[2]) faces.Add(FeFaceName.S2, GetArea(FeFaceName.S2, nodes));
            }
            //
            return faces;
        }
        public override double[] GetEquivalentForcesFromFaceName(FeFaceName faceName)
        {
            if (faceName == FeFaceName.S1 || faceName == FeFaceName.S2)
                return new double[] { 0, 0, 0, a, a, a };
            else if (faceName == FeFaceName.S3 || faceName == FeFaceName.S4 || faceName == FeFaceName.S5)
                return new double[] { b, b, c };
            else throw new NotSupportedException();
        }
        public override double[] GetEquivalentForcesFromFaceName(FeFaceName faceName, double[] nodalValues)
        {
            if (faceName == FeFaceName.S1 || faceName == FeFaceName.S2)
                return GetEquivalentForces(typeof(ParabolicTriangleElement), nodalValues);
            else if (faceName == FeFaceName.S3 || faceName == FeFaceName.S4 || faceName == FeFaceName.S5)
                return GetEquivalentForces(typeof(ParabolicBeamElement), nodalValues);
            else throw new NotSupportedException();
        }
        public override double GetArea(FeFaceName faceName, Dictionary<int, FeNode> nodes)
        {
            int[] cell = GetVtkCellFromFaceName(faceName);
            if (cell.Length == 6)
                return GeometryTools.TriangleArea(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]],
                                                  nodes[cell[3]], nodes[cell[4]], nodes[cell[5]]);
            else if (cell.Length == 3)
                return GeometryTools.EdgeLength(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]]);
            else throw new NotSupportedException();
        }
        public override double[] GetFaceCG(FeFaceName faceName, Dictionary<int, FeNode> nodes, out double area)
        {
            int[] cell = GetVtkCellFromFaceName(faceName);
            if (cell.Length == 6)
                return GeometryTools.TriangleCG(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]],
                                                nodes[cell[3]], nodes[cell[4]], nodes[cell[5]], out area);
            else if (cell.Length == 3)
                return GeometryTools.EdgeCG(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]], out area);
            else throw new NotSupportedException();
        }
        public override FeElement DeepCopy()
        {
            return new ParabolicTriangleElement(Id, PartId, NodeIds.ToArray());
        }
    }
}
