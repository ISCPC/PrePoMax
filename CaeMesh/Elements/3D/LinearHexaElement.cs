using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
    [Serializable]
    public class LinearHexaElement : FeElement3D
    {
        // Variables                                                                                                                
        private static int vtkCellTypeInt = (int)vtkCellType.VTK_HEXAHEDRON;
        private static double b = 0.25;


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public LinearHexaElement(int id, int[] nodeIds)
           : base(id, nodeIds)
        {
        }
        public LinearHexaElement(int id, int partId, int[] nodeIds)
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
            // the node ids are sorted 
            // S1 = 1-2-3-4 . 0-1-2-3 . 0-1-2-3
            // S2 = 5-8-7-6 . 4-7-6-5 . 4-5-6-7
            // S3 = 1-5-6-2 . 0-4-5-1 . 0-1-4-5
            // S4 = 2-6-7-3 . 1-5-6-2 . 1-2-5-6
            // S5 = 3-7-8-4 . 2-6-7-3 . 2-3-6-7
            // S6 = 4-8-5-1 . 3-7-4-0 . 0-3-4-7

            if (nodeIds[2] == 2) return FeFaceName.S1;
            else if (nodeIds[0] == 4) return FeFaceName.S2;
            else if (nodeIds[1] == 1) return FeFaceName.S3;
            else if (nodeIds[0] == 1) return FeFaceName.S4;
            else if (nodeIds[0] == 2) return FeFaceName.S5;
            else if (nodeIds[0] == 0) return FeFaceName.S6;
            else throw new NotSupportedException();
        }
        public override int[] GetNodeIdsFromFaceName(FeFaceName faceName)
        {
            // S1 = 1-2-3-4 . 0-1-2-3
            // S2 = 5-8-7-6 . 4-7-6-5
            // S3 = 1-5-6-2 . 0-4-5-1
            // S4 = 2-6-7-3 . 1-5-6-2
            // S5 = 3-7-8-4 . 2-6-7-3
            // S6 = 4-8-5-1 . 3-7-4-0
            switch (faceName)
            {
                case FeFaceName.S1:
                    return new int[] { NodeIds[0], NodeIds[1], NodeIds[2], NodeIds[3] };
                case FeFaceName.S2:
                    return new int[] { NodeIds[4], NodeIds[7], NodeIds[6], NodeIds[5] };
                case FeFaceName.S3:
                    return new int[] { NodeIds[0], NodeIds[4], NodeIds[5], NodeIds[1] };
                case FeFaceName.S4:
                    return new int[] { NodeIds[1], NodeIds[5], NodeIds[6], NodeIds[2] };
                case FeFaceName.S5:
                    return new int[] { NodeIds[2], NodeIds[6], NodeIds[7], NodeIds[3] };
                case FeFaceName.S6:
                    return new int[] { NodeIds[3], NodeIds[7], NodeIds[4], NodeIds[0] };
                default:
                    throw new NotSupportedException();
            }
        }
        public override int[] GetVtkCellFromFaceName(FeFaceName faceName)
        {
            // Invert the surface normal to point outwards
            // S1 = 1-2-3-4 . 0-1-2-3 . 0-3-2-1
            // S2 = 5-8-7-6 . 4-7-6-5 . 4-5-6-7
            // S3 = 1-5-6-2 . 0-4-5-1 . 0-1-5-4
            // S4 = 2-6-7-3 . 1-5-6-2 . 1-2-6-5
            // S5 = 3-7-8-4 . 2-6-7-3 . 2-3-7-6
            // S6 = 4-8-5-1 . 3-7-4-0 . 3-0-4-7
            switch (faceName)
            {
                case FeFaceName.S1:
                    return new int[] { NodeIds[0], NodeIds[3], NodeIds[2], NodeIds[1] };
                case FeFaceName.S2:
                    return new int[] { NodeIds[4], NodeIds[5], NodeIds[6], NodeIds[7] };
                case FeFaceName.S3:
                    return new int[] { NodeIds[0], NodeIds[1], NodeIds[5], NodeIds[4] };
                case FeFaceName.S4:
                    return new int[] { NodeIds[1], NodeIds[2], NodeIds[6], NodeIds[5] };
                case FeFaceName.S5:
                    return new int[] { NodeIds[2], NodeIds[3], NodeIds[7], NodeIds[6] };
                case FeFaceName.S6:
                    return new int[] { NodeIds[3], NodeIds[0], NodeIds[4], NodeIds[7] };
                default:
                    throw new NotSupportedException();
            }
        }
        public override int[][] GetAllVtkCells()
        {
            // use Method: GetVtkCellFromFaceName(FeFaceName faceName)
            int[][] cells = new int[6][];

            cells[0] = new int[] { NodeIds[0], NodeIds[3], NodeIds[2], NodeIds[1] };
            cells[1] = new int[] { NodeIds[4], NodeIds[5], NodeIds[6], NodeIds[7] };
            cells[2] = new int[] { NodeIds[0], NodeIds[1], NodeIds[5], NodeIds[4] };
            cells[3] = new int[] { NodeIds[1], NodeIds[2], NodeIds[6], NodeIds[5] };
            cells[4] = new int[] { NodeIds[2], NodeIds[3], NodeIds[7], NodeIds[6] };
            cells[5] = new int[] { NodeIds[3], NodeIds[0], NodeIds[4], NodeIds[7] };

            return cells;
        }
        public override Dictionary<FeFaceName, double> GetFaceNamesAndAreasFromNodeSet(HashSet<int> nodeSet,
                                                                                       Dictionary<int, FeNode> nodes,
                                                                                       bool edgeFaces)
        {
            int significantNodes = 8;
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
                // If five or more nodes were missed: break
                if (i + 1 - count >= 5) break;
            }
            // S1 = 1-2-3-4 . 0-1-2-3
            // S2 = 5-8-7-6 . 4-7-6-5
            // S3 = 1-5-6-2 . 0-4-5-1
            // S4 = 2-6-7-3 . 1-5-6-2
            // S5 = 3-7-8-4 . 2-6-7-3
            // S6 = 4-8-5-1 . 3-7-4-0
            Dictionary<FeFaceName, double> faces = new Dictionary<FeFaceName, double>();
            //
            if (faceNodeIds[0] && faceNodeIds[1] && faceNodeIds[2] && faceNodeIds[3]) faces.Add(FeFaceName.S1, GetArea(FeFaceName.S1, nodes));
            if (faceNodeIds[4] && faceNodeIds[7] && faceNodeIds[6] && faceNodeIds[5]) faces.Add(FeFaceName.S2, GetArea(FeFaceName.S2, nodes));
            if (faceNodeIds[0] && faceNodeIds[4] && faceNodeIds[5] && faceNodeIds[1]) faces.Add(FeFaceName.S3, GetArea(FeFaceName.S3, nodes));
            if (faceNodeIds[1] && faceNodeIds[5] && faceNodeIds[6] && faceNodeIds[2]) faces.Add(FeFaceName.S4, GetArea(FeFaceName.S4, nodes));
            if (faceNodeIds[2] && faceNodeIds[6] && faceNodeIds[7] && faceNodeIds[3]) faces.Add(FeFaceName.S5, GetArea(FeFaceName.S5, nodes));
            if (faceNodeIds[3] && faceNodeIds[7] && faceNodeIds[4] && faceNodeIds[0]) faces.Add(FeFaceName.S6, GetArea(FeFaceName.S6, nodes));
            //
            return faces;
        }
        public override double[] GetEquivalentForcesFromFaceName(FeFaceName faceName)
        {
            return new double[] { b, b, b, b };
        }
        public override double[] GetEquivalentForcesFromFaceName(FeFaceName faceName, double[] nodalValues)
        {
            return GetEquivalentForces(typeof(LinearQuadrilateralElement), nodalValues);
        }
        public override double GetArea(FeFaceName faceName, Dictionary<int, FeNode> nodes)
        {
            int[] cell = GetVtkCellFromFaceName(faceName);
            return GeometryTools.RectangleArea(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]], nodes[cell[3]]);
        }
        public override double[] GetFaceCG(FeFaceName faceName, Dictionary<int, FeNode> nodes, out double area)
        {
            int[] cell = GetVtkCellFromFaceName(faceName);
            double[] cg = GeometryTools.RectangleCG(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]], nodes[cell[3]], out area);
            return cg;
        }
        public override FeElement DeepCopy()
        {
            return new LinearHexaElement(Id, PartId, NodeIds.ToArray());
        }
    }
}
