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
        private static int vtkCellTypeInt = (int)vtkCellType.VTK_QUADRATIC_TRIANGLE;
        private static double a = 1.0 / 3.0;


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
            //return NodeIds.ToArray();
            //
            // Return S1
            return new int[] { NodeIds[0], NodeIds[2], NodeIds[1], NodeIds[5], NodeIds[4], NodeIds[3] };
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
            switch (faceName)
            {
                case FeFaceName.S1:
                    return new int[] { NodeIds[0], NodeIds[2], NodeIds[1], NodeIds[5], NodeIds[4], NodeIds[3] };
                case FeFaceName.S2:
                    return new int[] { NodeIds[0], NodeIds[1], NodeIds[2], NodeIds[3], NodeIds[4], NodeIds[5] };
                default:
                    throw new NotSupportedException();
            }
        }
        public override int[] GetVtkCellFromFaceName(FeFaceName faceName)
        {
            // NEG S1 = 1-3-2-6-5-4  . 0-2-1-5-4-3
            // POS S2 = 1-2-3-4-5-6  . 0-1-2-3-4-5
            switch (faceName)
            {
                case FeFaceName.S1:
                    return new int[] { NodeIds[0], NodeIds[2], NodeIds[1], NodeIds[5], NodeIds[4], NodeIds[3] };
                case FeFaceName.S2:
                    return new int[] { NodeIds[0], NodeIds[1], NodeIds[2], NodeIds[3], NodeIds[4], NodeIds[5] };
                default:
                    throw new NotSupportedException();
            }
        }
        public override int[][] GetAllVtkCells()
        {
            throw new NotImplementedException();
        }
        public override Dictionary<FeFaceName, double> GetFaceNamesAndAreasFromNodeSet(HashSet<int> nodeSet,
                                                                                       Dictionary<int, FeNode> nodes)
        {
            // Check only first 3 nodes (as in linear element)
            int significantNodes = 3;
            //
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
                if (i >= 1 && count <= i - 1) break;
            }
            // NEG S1 = 1-3-2 . 0-2-1
            Dictionary<FeFaceName, double> faces = new Dictionary<FeFaceName, double>();
            //
            if (count >= 3)
            {
                if (faceNodeIds[0] && faceNodeIds[2] && faceNodeIds[1]) faces.Add(FeFaceName.S1, GetArea(FeFaceName.S1, nodes));
            }
            //
            return faces;
        }
        public override double[] GetEquivalentForcesFromFaceName(FeFaceName faceName)
        {
            return new double[] { 0, 0, 0, a, a, a };
        }
        public override double GetArea(FeFaceName faceName, Dictionary<int, FeNode> nodes)
        {
            int[] cell = GetVtkCellFromFaceName(faceName);
            return GeometryTools.TriangleArea(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]],
                                              nodes[cell[3]], nodes[cell[4]], nodes[cell[5]]);
        }
        public override double[] GetCG(FeFaceName faceName, Dictionary<int, FeNode> nodes, out double area)
        {
            int[] cell = GetVtkCellFromFaceName(faceName);
            double[] cg = GeometryTools.TriangleCG(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]],
                                                   nodes[cell[3]], nodes[cell[4]], nodes[cell[5]], out area);
            return cg;
        }
        public override FeElement DeepCopy()
        {
            return new ParabolicTriangleElement(Id, PartId, NodeIds.ToArray());
        }
    }
}
