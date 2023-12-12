using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public class LinearPyramidElement : FeElement3D
    {
        // Variables                                                                                                                
        private static int vtkCellTypeInt = (int)vtkCellType.VTK_PYRAMID;
        private static double a = 1.0 / 3.0;
        private static double b = 0.25;


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public LinearPyramidElement(int id, int[] nodeIds)
           : base(id, nodeIds)
        {
        }
        public LinearPyramidElement(int id, int partId, int[] nodeIds)
            : base(id, partId, nodeIds)
        {
        }


        // Methods                                                                                                                  
        public override int[] GetVtkNodeIds()
        {
            // return a copy
            return NodeIds.ToArray();
        }
        public override int GetVtkCellType()
        {
            return vtkCellTypeInt;
        }
        public override FeFaceName GetFaceNameFromSortedNodeIds(int[] nodeIds)
        {
            // The node ids are sorted 
            // S1 = 1-4-3-2 . 0-3-2-1 . 0-1-2-3
            // S2 = 1-2-5   . 0-1-4   . 0-1-4  
            // S3 = 2-3-5   . 1-2-4   . 1-2-4  
            // S4 = 3-4-5   . 2-3-4   . 2-3-4  
            // S5 = 1-5-4   . 0-4-3   . 0-3-4  
            if (nodeIds[2] == 2) return FeFaceName.S1;
            else if (nodeIds[1] == 1) return FeFaceName.S2;
            else if (nodeIds[1] == 2) return FeFaceName.S3;
            else if (nodeIds[0] == 2) return FeFaceName.S4;
            else if (nodeIds[0] == 0) return FeFaceName.S5;
            else throw new NotSupportedException();
        }
        public override int[] GetNodeIdsFromFaceName(FeFaceName faceName)
        {
            // S1 = 1-4-3-2 . 0-3-2-1
            // S2 = 1-2-5   . 0-1-4  
            // S3 = 2-3-5   . 1-2-4  
            // S4 = 3-4-5   . 2-3-4  
            // S5 = 1-5-4   . 0-4-3  
            switch (faceName)
            {
                case FeFaceName.S1:
                    return new int[] { NodeIds[0], NodeIds[3], NodeIds[2], NodeIds[1] };
                case FeFaceName.S2:
                    return new int[] { NodeIds[0], NodeIds[1], NodeIds[4] };
                case FeFaceName.S3:
                    return new int[] { NodeIds[1], NodeIds[2], NodeIds[4] };
                case FeFaceName.S4:
                    return new int[] { NodeIds[2], NodeIds[3], NodeIds[4] };
                case FeFaceName.S5:
                    return new int[] { NodeIds[0], NodeIds[4], NodeIds[3] };
                default:
                    throw new NotSupportedException();
            }
        }
        public override int[] GetVtkCellFromFaceName(FeFaceName faceName)
        {
            // Invert the surface normal to point outwards
            // S1 = 1-4-3-2 . 0-3-2-1 . 0-3-2-1
            // S2 = 1-2-5   . 0-1-4   . 0-1-4  
            // S3 = 2-3-5   . 1-2-4   . 1-2-4  
            // S4 = 3-4-5   . 2-3-4   . 2-3-4  
            // S5 = 1-5-4   . 0-4-3   . 0-4-3  
            switch (faceName)
            {
                case FeFaceName.S1:
                    return new int[] { NodeIds[0], NodeIds[3], NodeIds[2], NodeIds[1] };
                case FeFaceName.S2:
                    return new int[] { NodeIds[0], NodeIds[1], NodeIds[4] };
                case FeFaceName.S3:
                    return new int[] { NodeIds[1], NodeIds[2], NodeIds[4] };
                case FeFaceName.S4:
                    return new int[] { NodeIds[2], NodeIds[3], NodeIds[4] };
                case FeFaceName.S5:
                    return new int[] { NodeIds[0], NodeIds[4], NodeIds[3] };
                default:
                    throw new NotSupportedException();
            }
        }
        public override int[][] GetAllVtkCells()
        {
            // Use Method: GetVtkCellFromFaceName(FeFaceName faceName)
            int[][] cells = new int[5][];
            //
            cells[0] = new int[] { NodeIds[0], NodeIds[3], NodeIds[2], NodeIds[1] };
            cells[1] = new int[] { NodeIds[0], NodeIds[1], NodeIds[4] };
            cells[2] = new int[] { NodeIds[1], NodeIds[2], NodeIds[4] };
            cells[3] = new int[] { NodeIds[2], NodeIds[3], NodeIds[4] };
            cells[4] = new int[] { NodeIds[0], NodeIds[4], NodeIds[3] };
            //
            return cells;
        }
        public override Dictionary<FeFaceName, double> GetFaceNamesAndAreasFromNodeSet(HashSet<int> nodeSet,
                                                                                       Dictionary<int, FeNode> nodes,
                                                                                       bool edgeFaces)
        {
            int significantNodes = 5;
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
                // If three or more nodes were missed: break
                if (i + 1 - count >= 3) break;
            }
            // S1 = 1-4-3-2 . 0-3-2-1
            // S2 = 1-2-5   . 0-1-4  
            // S3 = 2-3-5   . 1-2-4  
            // S4 = 3-4-5   . 2-3-4  
            // S5 = 1-5-4   . 0-4-3  
            Dictionary<FeFaceName, double> faces = new Dictionary<FeFaceName, double>();
            //
            if (count >= 3)
            {
                if (faceNodeIds[0] && faceNodeIds[3] && faceNodeIds[2] && faceNodeIds[1])
                    faces.Add(FeFaceName.S1, GetArea(FeFaceName.S1, nodes));
                if (faceNodeIds[0] && faceNodeIds[1] && faceNodeIds[4])
                    faces.Add(FeFaceName.S2, GetArea(FeFaceName.S2, nodes));
                if (faceNodeIds[1] && faceNodeIds[2] && faceNodeIds[4])
                    faces.Add(FeFaceName.S3, GetArea(FeFaceName.S3, nodes));
                if (faceNodeIds[2] && faceNodeIds[3] && faceNodeIds[4])
                    faces.Add(FeFaceName.S4, GetArea(FeFaceName.S4, nodes));
                if (faceNodeIds[0] && faceNodeIds[4] && faceNodeIds[3])
                    faces.Add(FeFaceName.S5, GetArea(FeFaceName.S5, nodes));
            }
            //
            return faces;
        }
        public override double[] GetEquivalentForcesFromFaceName(FeFaceName faceName)
        {
            if (faceName == FeFaceName.S1)
                return new double[] { b, b, b, b };
            else if (faceName == FeFaceName.S2 || faceName == FeFaceName.S3 ||
                     faceName == FeFaceName.S4 || faceName == FeFaceName.S5)
                //return new double[] { a, a, a };
                throw new NotImplementedException();
            else throw new NotSupportedException();
        }
        public override double[] GetEquivalentForcesFromFaceName(FeFaceName faceName, double[] nodalValues)
        {
            if (faceName == FeFaceName.S1)
                return GetEquivalentForces(typeof(LinearQuadrilateralElement), nodalValues);
            else if (faceName == FeFaceName.S2 || faceName == FeFaceName.S3 ||
                     faceName == FeFaceName.S4 || faceName == FeFaceName.S5)
                //return GetEquivalentForces(typeof(LinearTriangleElement), nodalValues);
                throw new NotImplementedException();
            else throw new NotSupportedException();
        }
        public override double GetArea(FeFaceName faceName, Dictionary<int, FeNode> nodes)
        {
            int[] cell = GetVtkCellFromFaceName(faceName);
            if (cell.Length == 3)   // faceName == FeFaceName.S1
                return GeometryTools.TriangleArea(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]]);
            else
                return GeometryTools.RectangleArea(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]], nodes[cell[3]]);
        }
        public override double[] GetFaceCG(FeFaceName faceName, Dictionary<int, FeNode> nodes, out double area)
        {
            double[] cg;
            int[] cell = GetVtkCellFromFaceName(faceName);
            if (cell.Length == 3)   // faceName == FeFaceName.S1
                cg = GeometryTools.TriangleCG(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]], out area);
            else
                cg = GeometryTools.RectangleCG(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]], nodes[cell[3]], out area);
            return cg;
        }
        public override FeElement DeepCopy()
        {
            return new LinearPyramidElement(Id, PartId, NodeIds.ToArray());
        }
    }
}

