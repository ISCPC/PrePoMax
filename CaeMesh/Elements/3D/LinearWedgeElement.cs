using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public class LinearWedgeElement : FeElement3D
    {
        // Variables                                                                                                                
        private static int vtkCellTypeInt = (int)vtkCellType.VTK_WEDGE;
        private static double a = 1.0 / 3.0;
        private static double b = 0.25;


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public LinearWedgeElement(int id, int[] nodeIds)
           : base(id, nodeIds)
        {
        }
        public LinearWedgeElement(int id, int partId, int[] nodeIds)
            : base(id, partId, nodeIds)
        {
        }


        // Methods                                                                                                                  
        public override int[] GetVtkNodeIds()
        {
            // return a copy
            return new int[] { NodeIds[0], NodeIds[2], NodeIds[1], NodeIds[3], NodeIds[5], NodeIds[4] };
        }
        public override int GetVtkCellType()
        {
            return vtkCellTypeInt;
        }
        public override FeFaceName GetFaceNameFromSortedNodeIds(int[] nodeIds)
        {
            // the node ids are sorted 
            // S1 = 1-2-3   . 0-1-2   . 0-1-2  
            // S2 = 4-5-6   . 3-4-5   . 3-4-5  
            // S3 = 1-2-5-4 . 0-1-4-3 . 0-1-3-4
            // S4 = 2-3-6-5 . 1-2-5-4 . 1-2-4-5
            // S5 = 3-1-4-6 . 2-0-3-5 . 0-2-3-5

            if (nodeIds[2] == 2) return FeFaceName.S1;
            else if (nodeIds[0] == 3) return FeFaceName.S2;
            else if (nodeIds[1] == 1) return FeFaceName.S3;
            else if (nodeIds[0] == 1) return FeFaceName.S4;
            else if (nodeIds[0] == 0) return FeFaceName.S5;
            else throw new NotSupportedException();
        }
        public override int[] GetNodeIdsFromFaceName(FeFaceName faceName)
        {
            // S1 = 1-2-3   . 0-1-2  
            // S2 = 4-5-6   . 3-4-5  
            // S3 = 1-2-5-4 . 0-1-4-3
            // S4 = 2-3-6-5 . 1-2-5-4
            // S5 = 3-1-4-6 . 2-0-3-5
            switch (faceName)
            {
                case FeFaceName.S1:
                    return new int[] { NodeIds[0], NodeIds[1], NodeIds[2] };
                case FeFaceName.S2:
                    return new int[] { NodeIds[3], NodeIds[4], NodeIds[5] };
                case FeFaceName.S3:
                    return new int[] { NodeIds[0], NodeIds[1], NodeIds[4], NodeIds[3] };
                case FeFaceName.S4:
                    return new int[] { NodeIds[1], NodeIds[2], NodeIds[5], NodeIds[4] };
                case FeFaceName.S5:
                    return new int[] { NodeIds[2], NodeIds[0], NodeIds[3], NodeIds[5] };
                default:
                    throw new NotSupportedException();
            }
        }
        public override int[] GetVtkCellFromFaceName(FeFaceName faceName)
        {
            // invert the surface normal to point outwards
            // S1 = 1-2-3   . 0-1-2   . 0-2-1  
            // S2 = 4-5-6   . 3-4-5   . 3-4-5   - is already outwards
            // S3 = 1-2-5-4 . 0-1-4-3 . 0-1-4-3 - is already outwards
            // S4 = 2-3-6-5 . 1-2-5-4 . 1-2-5-4 - is already outwards
            // S5 = 3-1-4-6 . 2-0-3-5 . 2-0-3-5 - is already outwards
            switch (faceName)
            {
                case FeFaceName.S1:
                    return new int[] { NodeIds[0], NodeIds[2], NodeIds[1] } ;
                case FeFaceName.S2:
                    return new int[] { NodeIds[3], NodeIds[4], NodeIds[5] };
                case FeFaceName.S3:
                    //returnnew int[] { NodeIDs[0], NodeIDs[1], NodeIDs[4], NodeIDs[3] };
                    return new int[] { NodeIds[1], NodeIds[4], NodeIds[3], NodeIds[0] };
                case FeFaceName.S4:
                    //returnnew int[] { NodeIDs[1], NodeIDs[2], NodeIDs[5], NodeIDs[4] };
                    return new int[] { NodeIds[2], NodeIds[5], NodeIds[4], NodeIds[1] };
                case FeFaceName.S5:
                    //returnnew int[] { NodeIDs[2], NodeIDs[0], NodeIDs[3], NodeIDs[5] };
                    return new int[] { NodeIds[0], NodeIds[3], NodeIds[5], NodeIds[2] };
                default:
                    throw new NotSupportedException();
            }
        }
        public override int[][] GetAllVtkCells()
        {
            // use Method: GetVtkCellFromFaceName(FeFaceName faceName)
            int[][] cells = new int[5][];

            cells[0] = new int[] { NodeIds[0], NodeIds[2], NodeIds[1] };
            cells[1] = new int[] { NodeIds[3], NodeIds[4], NodeIds[5] };
            cells[2] = new int[] { NodeIds[1], NodeIds[4], NodeIds[3], NodeIds[0] };
            cells[3] = new int[] { NodeIds[2], NodeIds[5], NodeIds[4], NodeIds[1] };
            cells[4] = new int[] { NodeIds[0], NodeIds[3], NodeIds[5], NodeIds[2] };

            return cells;
        }
        public override Dictionary<FeFaceName, double> GetFaceNamesAndAreasFromNodeSet(HashSet<int> nodeSet,
                                                                                       Dictionary<int, FeNode> nodes,
                                                                                       bool edgeFaces)
        {
            int significantNodes = 6;
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
                // If four or more nodes were missed: break
                if (i + 1 - count >= 4) break;
            }
            // S1 = 1-2-3   . 0-1-2  
            // S2 = 4-5-6   . 3-4-5  
            // S3 = 1-2-5-4 . 0-1-4-3
            // S4 = 2-3-6-5 . 1-2-5-4
            // S5 = 3-1-4-6 . 2-0-3-5
            Dictionary<FeFaceName, double> faces = new Dictionary<FeFaceName, double>();
            //
            if (count >= 3)
            {
                if (faceNodeIds[0] && faceNodeIds[1] && faceNodeIds[2]) faces.Add(FeFaceName.S1, GetArea(FeFaceName.S1, nodes));
                if (faceNodeIds[3] && faceNodeIds[4] && faceNodeIds[5]) faces.Add(FeFaceName.S2, GetArea(FeFaceName.S2, nodes));
                if (faceNodeIds[0] && faceNodeIds[1] && faceNodeIds[4] && faceNodeIds[3]) faces.Add(FeFaceName.S3, GetArea(FeFaceName.S3, nodes));
                if (faceNodeIds[1] && faceNodeIds[2] && faceNodeIds[5] && faceNodeIds[4]) faces.Add(FeFaceName.S4, GetArea(FeFaceName.S4, nodes));
                if (faceNodeIds[2] && faceNodeIds[0] && faceNodeIds[3] && faceNodeIds[5]) faces.Add(FeFaceName.S5, GetArea(FeFaceName.S5, nodes));
            }
            //
            return faces;
        }
        public override double[] GetEquivalentForcesFromFaceName(FeFaceName faceName)
        {
            if (faceName == FeFaceName.S1 || faceName == FeFaceName.S2) 
                return new double[] { a, a, a };
            else if (faceName == FeFaceName.S3 || faceName == FeFaceName.S4 || faceName == FeFaceName.S5)
                return new double[] { b, b, b, b };
            else throw new NotSupportedException();
        }
        public override double[] GetEquivalentForcesFromFaceName(FeFaceName faceName, double[] nodalValues)
        {
            if (faceName == FeFaceName.S1 || faceName == FeFaceName.S2)
                return GetEquivalentForces(typeof(LinearTriangleElement), nodalValues);
            else if (faceName == FeFaceName.S3 || faceName == FeFaceName.S4 || faceName == FeFaceName.S5)
                return GetEquivalentForces(typeof(LinearQuadrilateralElement), nodalValues);
            else throw new NotSupportedException();
        }
        public override double GetArea(FeFaceName faceName, Dictionary<int, FeNode> nodes)
        {
            int[] cell = GetVtkCellFromFaceName(faceName);
            if (cell.Length == 3)   //(faceName == FeFaceName.S1 || faceName == FeFaceName.S2)
                return GeometryTools.TriangleArea(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]]);
            else
                return GeometryTools.RectangleArea(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]], nodes[cell[3]]);
        }
        public override double[] GetFaceCG(FeFaceName faceName, Dictionary<int, FeNode> nodes, out double area)
        {
            double[] cg;
            int[] cell = GetVtkCellFromFaceName(faceName);
            if (cell.Length == 3)   //(faceName == FeFaceName.S1 || faceName == FeFaceName.S2)
                cg = GeometryTools.TriangleCG(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]], out area);
            else
                cg = GeometryTools.RectangleCG(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]], nodes[cell[3]], out area);
            return cg;
        }
        public override FeElement DeepCopy()
        {
            return new LinearWedgeElement(Id, PartId, NodeIds.ToArray());
        }
    }
}
