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
        private static readonly int vtkCellTypeInt = (int)vtkCellType.VTK_TRIANGLE;
        private static readonly double a = 1.0 / 3.0;
        private static readonly double b = 1.0 / 2.0;


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
            //     S3 = 1-2 . 0-1
            //     S4 = 2-3 . 1-2
            //     S5 = 3-1 . 2-0
            switch (faceName)
            {
                case FeFaceName.S1:
                    return new int[] { NodeIds[0], NodeIds[2], NodeIds[1] };
                case FeFaceName.S2:
                    return new int[] { NodeIds[0], NodeIds[1], NodeIds[2] };
                case FeFaceName.S3:
                    return new int[] { NodeIds[0], NodeIds[1] };
                case FeFaceName.S4:
                    return new int[] { NodeIds[1], NodeIds[2] };
                case FeFaceName.S5:
                    return new int[] { NodeIds[2], NodeIds[0] };
                default:
                    throw new NotSupportedException();
            }
        }
        public override int[] GetVtkCellFromFaceName(FeFaceName faceName)
        {
            // NEG S1 = 1-3-2 . 0-2-1
            // POS S2 = 1-2-3 . 0-1-2
            //     S3 = 1-2 . 0-1
            //     S4 = 2-3 . 1-2
            //     S5 = 3-1 . 2-0
            switch (faceName)
            {
                case FeFaceName.S1:
                    return new int[] { NodeIds[0], NodeIds[2], NodeIds[1] };
                case FeFaceName.S2:
                    return new int[] { NodeIds[0], NodeIds[1], NodeIds[2] };
                case FeFaceName.S3:
                    return new int[] { NodeIds[0], NodeIds[1] };
                case FeFaceName.S4:
                    return new int[] { NodeIds[1], NodeIds[2] };
                case FeFaceName.S5:
                    return new int[] { NodeIds[2], NodeIds[0] };
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
                                                                                       Dictionary<int, FeNode> nodes,
                                                                                       bool edgeFaces)
        {
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
            //
            Dictionary<FeFaceName, double> faces = new Dictionary<FeFaceName, double>();
            //
            if (edgeFaces)
            {
                if (count >= 2)
                {
                    // S3 = 1-2 . 0-1
                    // S4 = 2-3 . 1-2
                    // S5 = 3-1 . 2-0
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
                return new double[] { a, a, a };
            else if (faceName == FeFaceName.S3 || faceName == FeFaceName.S4 || faceName == FeFaceName.S5)
                return new double[] { b, b };
            else throw new NotSupportedException();
        }
        public override double[] GetEquivalentForcesFromFaceName(FeFaceName faceName, double[] nodalValues)
        {
            if (faceName == FeFaceName.S1 || faceName == FeFaceName.S2)
                return GetEquivalentForces(typeof(LinearTriangleElement), nodalValues);
            else if (faceName == FeFaceName.S3 || faceName == FeFaceName.S4 || faceName == FeFaceName.S5)
                return GetEquivalentForces(typeof(LinearBeamElement), nodalValues);
            else throw new NotSupportedException();
        }
        public override double GetArea(FeFaceName faceName, Dictionary<int, FeNode> nodes)
        {
            int[] cell = GetVtkCellFromFaceName(faceName);
            if (cell.Length == 3)
                return GeometryTools.TriangleArea(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]]);
            else if (cell.Length == 2)
                return GeometryTools.EdgeLength(nodes[cell[0]], nodes[cell[1]]);
            else throw new NotSupportedException();
        }
        public override double[] GetFaceCG(FeFaceName faceName, Dictionary<int, FeNode> nodes, out double area)
        {
            int[] cell = GetVtkCellFromFaceName(faceName);
            if (cell.Length == 3)
                return GeometryTools.TriangleCG(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]], out area);
            else if (cell.Length == 2)
                return GeometryTools.EdgeCG(nodes[cell[0]], nodes[cell[1]], out area);
            else throw new NotSupportedException();
        }
        public override FeElement DeepCopy()
        {
            return new LinearTriangleElement(Id, PartId, NodeIds.ToArray());
        }
        //
        public void FlipNormal()
        {
            int tmp = NodeIds[1];
            NodeIds[1] = NodeIds[2];
            NodeIds[2] = tmp;
        }
    }
}
