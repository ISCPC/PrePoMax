using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public class ParabolicTetraElement : FeElement3D
    {
        // Variables                                                                                                                
        private static int vtkCellTypeInt = (int)vtkCellType.VTK_QUADRATIC_TETRA; 
        private static double a = 1.0 / 3.0;


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public ParabolicTetraElement(int id, int[] nodeIds)
          : base(id, nodeIds)
        {
        }
        public ParabolicTetraElement(int id, int partId, int[] nodeIds)
            : base(id, partId, nodeIds)
        {
        }


        // Methods                                                                                                                  
        public override int[] GetVtkNodeIds()
        {
            // return copy
            return NodeIds.ToArray();
        }
        public override int GetVtkCellType()
        {
            return vtkCellTypeInt;
        }
        public override FeFaceName GetFaceNameFromSortedNodeIds(int[] nodeIds)
        {
            // the parameter node ids is sorted 
            // only first three nodes are important for face determination
            // S1 = 1-2-3-4-5-6 . 0-1-2
            // S2 = 1-4-2-7-8-4 . 0-1-3
            // S3 = 2-4-3-8-9-5 . 1-2-3
            // S4 = 3-4-1-9-7-6 . 0-2-3
            
            if (nodeIds[2] == 2) return FeFaceName.S1;
            else if (nodeIds[1] == 1) return FeFaceName.S2;
            else if (nodeIds[0] == 1) return FeFaceName.S3;
            else return FeFaceName.S4;
        }
        public override int[] GetNodeIdsFromFaceName(FeFaceName faceName)
        {
            // S1 = 1-2-3-5-6-7  . 0-1-2-4-5-6
            // S2 = 1-4-2-8-9-5  . 0-3-1-7-8-4
            // S3 = 2-4-3-9-10-6 . 1-3-2-8-9-5
            // S4 = 3-4-1-10-8-7 . 2-3-0-9-7-6
            if (faceName == FeFaceName.S1) return new int[] { NodeIds[0], NodeIds[1], NodeIds[2],
                                                              NodeIds[4], NodeIds[5], NodeIds[6] };
            if (faceName == FeFaceName.S2) return new int[] { NodeIds[0], NodeIds[3], NodeIds[1],
                                                              NodeIds[7], NodeIds[8], NodeIds[4] };
            if (faceName == FeFaceName.S3) return new int[] { NodeIds[1], NodeIds[3], NodeIds[2],
                                                              NodeIds[8], NodeIds[9], NodeIds[5] };
            if (faceName == FeFaceName.S4) return new int[] { NodeIds[2], NodeIds[3], NodeIds[0],
                                                              NodeIds[9], NodeIds[7], NodeIds[6] };
            else throw new NotSupportedException();
        }
        public override int[] GetVtkCellFromFaceName(FeFaceName faceName)
        {
            // invert the surface normal . switch the second and third index
            // S1 = 1-2-3-5-6-7  . 0-2-1-6-5-4
            // S2 = 1-4-2-8-9-5  . 0-1-3-4-8-7
            // S3 = 2-4-3-9-10-6 . 1-2-3-5-9-8
            // S4 = 3-4-1-10-8-7 . 2-0-3-6-7-9
            switch (faceName)
            {
                case FeFaceName.S1:
                    return new int[] { NodeIds[0], NodeIds[2], NodeIds[1], NodeIds[6], NodeIds[5], NodeIds[4] };
                case FeFaceName.S2:
                    return new int[] { NodeIds[0], NodeIds[1], NodeIds[3], NodeIds[4], NodeIds[8], NodeIds[7] };
                case FeFaceName.S3:
                    return new int[] { NodeIds[1], NodeIds[2], NodeIds[3], NodeIds[5], NodeIds[9], NodeIds[8] };
                case FeFaceName.S4:
                    return new int[] { NodeIds[2], NodeIds[0], NodeIds[3], NodeIds[6], NodeIds[7], NodeIds[9] };
                default:
                    throw new NotSupportedException();
            }
        }
        public override int[][] GetAllVtkCells()
        {
            // use Method: GetVtkCellFromFaceName(FeFaceName faceName)
            int[][] cells = new int[4][];

            cells[0] = new int[] { NodeIds[0], NodeIds[2], NodeIds[1], NodeIds[6], NodeIds[5], NodeIds[4] };
            cells[1] = new int[] { NodeIds[0], NodeIds[1], NodeIds[3], NodeIds[4], NodeIds[8], NodeIds[7] };
            cells[2] = new int[] { NodeIds[1], NodeIds[2], NodeIds[3], NodeIds[5], NodeIds[9], NodeIds[8] };
            cells[3] = new int[] { NodeIds[2], NodeIds[0], NodeIds[3], NodeIds[6], NodeIds[7], NodeIds[9] };

            return cells;
        }
        public override Dictionary<FeFaceName, double> GetFaceNamesAndAreasFromNodeSet(HashSet<int> nodeSet,
                                                                                       Dictionary<int, FeNode> nodes,
                                                                                       bool edgeFaces)
        {
            // Check only first 4 nodes (as in linear element)
            int significantNodes = 4;
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
            // S1 = 1-2-3 . 0-1-2
            // S2 = 1-4-2 . 0-3-1
            // S3 = 2-4-3 . 1-3-2
            // S4 = 3-4-1 . 2-3-0
            Dictionary<FeFaceName, double> faces = new Dictionary<FeFaceName, double>();
            //
            if (count >= 3)
            {
                if (faceNodeIds[0] && faceNodeIds[1] && faceNodeIds[2]) faces.Add(FeFaceName.S1, GetArea(FeFaceName.S1, nodes));
                if (faceNodeIds[0] && faceNodeIds[3] && faceNodeIds[1]) faces.Add(FeFaceName.S2, GetArea(FeFaceName.S2, nodes));
                if (faceNodeIds[1] && faceNodeIds[3] && faceNodeIds[2]) faces.Add(FeFaceName.S3, GetArea(FeFaceName.S3, nodes));
                if (faceNodeIds[2] && faceNodeIds[3] && faceNodeIds[0]) faces.Add(FeFaceName.S4, GetArea(FeFaceName.S4, nodes));
            }
            //
            return faces;
        }
        public override double[] GetEquivalentForcesFromFaceName(FeFaceName faceName)
        {
            return new double[] { 0, 0, 0, a, a, a };
        }
        public override double[] GetEquivalentForcesFromFaceName(FeFaceName faceName, double[] nodalValues)
        {
            return GetEquivalentForces(typeof(ParabolicTriangleElement), nodalValues);
        }
        public override double GetArea(FeFaceName faceName, Dictionary<int, FeNode> nodes)
        {
            int[] cell = GetVtkCellFromFaceName(faceName);
            return GeometryTools.TriangleArea(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]],
                                              nodes[cell[3]], nodes[cell[4]], nodes[cell[5]]);
        }
        public override double[] GetFaceCG(FeFaceName faceName, Dictionary<int, FeNode> nodes, out double area)
        {
            int[] cell = GetVtkCellFromFaceName(faceName);
            double[] cg = GeometryTools.TriangleCG(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]],
                                                   nodes[cell[3]], nodes[cell[4]], nodes[cell[5]], out area);
            return cg;
        }
        public override FeElement DeepCopy()
        {
            return new ParabolicTetraElement(Id, PartId, NodeIds.ToArray());
        }
    }
}
