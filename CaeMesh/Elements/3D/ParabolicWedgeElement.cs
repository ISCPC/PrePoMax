using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public class ParabolicWedgeElement : FeElement3D
    {
        // Variables                                                                                                                
        private static int vtkCellTypeInt = (int)vtkCellType.VTK_QUADRATIC_WEDGE;
        private static double a = 1.0 / 3.0;
        private static double b = -a / 4;


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public ParabolicWedgeElement(int id, int[] nodeIds)
          : base(id, nodeIds)
        {
        }
        public ParabolicWedgeElement(int id, int partId, int[] nodeIds)
            : base(id, partId, nodeIds)
        {
        }


        // Methods                                                                                                                  
        public override int[] GetVtkNodeIds()
        {
            // return a copy -> ToArray
            return new int[] { NodeIds[0], NodeIds[2], NodeIds[1], NodeIds[3], NodeIds[5], NodeIds[4],
                               NodeIds[8], NodeIds[7], NodeIds[6], NodeIds[11], NodeIds[10], NodeIds[9],
                               NodeIds[12], NodeIds[14], NodeIds[13]};
        }
        public override int GetVtkCellType()
        {
            return vtkCellTypeInt;
        }
        public override FeFaceName GetFaceNameFromSortedNodeIds(int[] nodeIds)
        {
            // the parameter node ids is sorted 
            // only first three/four nodes are important for face determination
            // S1 = 1-2-3-7-8-9        . 0-1-2   . 0-1-2  
            // S2 = 4-5-6-10-11-12     . 3-4-5   . 3-4-5  
            // S3 = 1-2-5-4-7-14-10-13 . 0-1-4-3 . 0-1-3-4
            // S4 = 2-3-6-5-8-15-11-14 . 1-2-5-4 . 1-2-4-5
            // S5 = 3-1-4-6-9-13-12-15 . 2-0-3-5 . 0-2-3-5

            if (nodeIds[2] == 2) return FeFaceName.S1;
            else if (nodeIds[0] == 3) return FeFaceName.S2;
            else if (nodeIds[1] == 1) return FeFaceName.S3;
            else if (nodeIds[0] == 1) return FeFaceName.S4;
            else if (nodeIds[0] == 0) return FeFaceName.S5;
            else throw new NotSupportedException();
        }
        public override int[] GetNodeIdsFromFaceName(FeFaceName faceName)
        {
            // S1 = 1-2-3-7-8-9        . 0-1-2-6-7-8       
            // S2 = 4-5-6-10-11-12     . 3-4-5-9-10-11     
            // S3 = 1-2-5-4-7-14-10-13 . 0-1-4-3-6-13-9-12 
            // S4 = 2-3-6-5-8-15-11-14 . 1-2-5-4-7-14-10-13
            // S5 = 3-1-4-6-9-13-12-15 . 2-0-3-5-8-12-11-14
            switch (faceName)
            {
                case FeFaceName.S1:
                    return new int[] { NodeIds[0], NodeIds[1], NodeIds[2], NodeIds[6], NodeIds[7], NodeIds[8] };
                case FeFaceName.S2:
                    return new int[] { NodeIds[3], NodeIds[4], NodeIds[5], NodeIds[9], NodeIds[10], NodeIds[11] };
                case FeFaceName.S3:
                    return new int[] { NodeIds[0], NodeIds[1], NodeIds[4], NodeIds[3], NodeIds[6], NodeIds[13],
                                       NodeIds[9], NodeIds[12] };
                case FeFaceName.S4:
                    return new int[] { NodeIds[1], NodeIds[2], NodeIds[5], NodeIds[4], NodeIds[7], NodeIds[14],
                                       NodeIds[10], NodeIds[13] };
                case FeFaceName.S5:
                    return new int[] { NodeIds[2], NodeIds[0], NodeIds[3], NodeIds[5], NodeIds[8], NodeIds[12],
                                       NodeIds[11], NodeIds[14] };
                default:
                    throw new NotSupportedException();
            }
        }
        public override int[] GetVtkCellFromFaceName(FeFaceName faceName)
        {
            // invert the surface normal to point outwards
            // S1 = 1-2-3-7-8-9        . 0-1-2-6-7-8        . 0-2-1-8-7-6                             
            // S2 = 4-5-6-10-11-12     . 3-4-5-9-10-11      . 3-4-5-9-10-11      - is already outwards
            // S3 = 1-2-5-4-7-14-10-13 . 0-1-4-3-6-13-9-12  . 0-1-4-3-6-13-9-12  - is already outwards
            // S4 = 2-3-6-5-8-15-11-14 . 1-2-5-4-7-14-10-13 . 1-2-5-4-7-14-10-13 - is already outwards
            // S5 = 3-1-4-6-9-13-12-15 . 2-0-3-5-8-12-11-14 . 2-0-3-5-8-12-11-14 - is already outwards
            switch (faceName)
            {
                case FeFaceName.S1:
                    return new int[] { NodeIds[0], NodeIds[2], NodeIds[1], NodeIds[8], NodeIds[7], NodeIds[6] };
                case FeFaceName.S2:
                    return new int[] { NodeIds[3], NodeIds[4], NodeIds[5], NodeIds[9], NodeIds[10], NodeIds[11] };
                case FeFaceName.S3:
                    return new int[] { NodeIds[0], NodeIds[1], NodeIds[4], NodeIds[3], NodeIds[6], NodeIds[13],
                                       NodeIds[9], NodeIds[12] };
                case FeFaceName.S4:
                    return new int[] { NodeIds[1], NodeIds[2], NodeIds[5], NodeIds[4], NodeIds[7], NodeIds[14],
                                       NodeIds[10], NodeIds[13] };
                case FeFaceName.S5:
                    return new int[] { NodeIds[2], NodeIds[0], NodeIds[3], NodeIds[5], NodeIds[8], NodeIds[12],
                                       NodeIds[11], NodeIds[14] };
                default:
                    throw new NotSupportedException();
            }
        }
        public override int[][] GetAllVtkCells()
        {
            // use Method: GetVtkCellFromFaceName(FeFaceName faceName)
            int[][] cells = new int[5][];

            cells[0] = new int[] { NodeIds[0], NodeIds[2], NodeIds[1], NodeIds[8], NodeIds[7], NodeIds[6] };
            cells[1] = new int[] { NodeIds[3], NodeIds[4], NodeIds[5], NodeIds[9], NodeIds[10], NodeIds[11] };
            cells[2] = new int[] { NodeIds[0], NodeIds[1], NodeIds[4], NodeIds[3], NodeIds[6], NodeIds[13],
                                   NodeIds[9], NodeIds[12] };
            cells[3] = new int[] { NodeIds[1], NodeIds[2], NodeIds[5], NodeIds[4], NodeIds[7], NodeIds[14],
                                   NodeIds[10], NodeIds[13] };
            cells[4] = new int[] { NodeIds[2], NodeIds[0], NodeIds[3], NodeIds[5], NodeIds[8], NodeIds[12],
                                   NodeIds[11], NodeIds[14] };

            return cells;
        }
        public override Dictionary<FeFaceName, double> GetFaceNamesAndAreasFromNodeSet(HashSet<int> nodeSet,
                                                                                       Dictionary<int, FeNode> nodes,
                                                                                       bool edgeFaces)
        {
            // Check only first 6 nodes (as in linear element)
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
                if (faceNodeIds[0] && faceNodeIds[1] && faceNodeIds[4] && faceNodeIds[3])
                    faces.Add(FeFaceName.S3, GetArea(FeFaceName.S3, nodes));
                if (faceNodeIds[1] && faceNodeIds[2] && faceNodeIds[5] && faceNodeIds[4])
                    faces.Add(FeFaceName.S4, GetArea(FeFaceName.S4, nodes));
                if (faceNodeIds[2] && faceNodeIds[0] && faceNodeIds[3] && faceNodeIds[5])
                    faces.Add(FeFaceName.S5, GetArea(FeFaceName.S5, nodes));
            }
            //
            return faces;
        }
        public override double[] GetEquivalentForcesFromFaceName(FeFaceName faceName)
        {
            if (faceName == FeFaceName.S1 || faceName == FeFaceName.S2)
                return new double[] { 0, 0, 0, a, a, a };
            else if (faceName == FeFaceName.S3 || faceName == FeFaceName.S4 || faceName == FeFaceName.S5)
                return new double[] { b, b, b, b, a, a, a, a };
            else throw new NotSupportedException();
        }
        public override double[] GetEquivalentForcesFromFaceName(FeFaceName faceName, double[] nodalValues)
        {
            if (faceName == FeFaceName.S1 || faceName == FeFaceName.S2)
                return GetEquivalentForces(typeof(ParabolicTriangleElement), nodalValues);
            else if (faceName == FeFaceName.S3 || faceName == FeFaceName.S4 || faceName == FeFaceName.S5)
                return GetEquivalentForces(typeof(ParabolicQuadrilateralElement), nodalValues);
            else throw new NotSupportedException();
        }
        public override double GetArea(FeFaceName faceName, Dictionary<int, FeNode> nodes)
        {
            int[] cell = GetVtkCellFromFaceName(faceName);
            if (cell.Length == 6)   //(faceName == FeFaceName.S1 || faceName == FeFaceName.S2)
                return GeometryTools.TriangleArea(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]],
                                                  nodes[cell[3]], nodes[cell[4]], nodes[cell[5]]);
            else
                return GeometryTools.RectangleArea(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]], nodes[cell[3]],
                                                   nodes[cell[4]], nodes[cell[5]], nodes[cell[6]], nodes[cell[7]]);
        }
        public override double[] GetFaceCG(FeFaceName faceName, Dictionary<int, FeNode> nodes, out double area)
        {
            double[] cg;
            int[] cell = GetVtkCellFromFaceName(faceName);
            if (cell.Length == 6)   //(faceName == FeFaceName.S1 || faceName == FeFaceName.S2)
                cg = GeometryTools.TriangleCG(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]],
                                              nodes[cell[3]], nodes[cell[4]], nodes[cell[5]], out area);
            else
                cg = GeometryTools.RectangleCG(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]], nodes[cell[3]],
                                               nodes[cell[4]], nodes[cell[5]], nodes[cell[6]], nodes[cell[7]], out area);
            return cg;
        }
        public override FeElement DeepCopy()
        {
            return new ParabolicWedgeElement(Id, PartId, NodeIds.ToArray());
        }
    }
}
