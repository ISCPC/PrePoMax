using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public class ParabolicQuadrilateralElement : FeElement2D
    {
        // Variables                                                                                                                
        private static int vtkCellTypeInt = (int)vtkCellType.VTK_QUADRATIC_QUAD;
        private static double a = 1.0 / 3.0;
        private static double b = -a / 4;


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public ParabolicQuadrilateralElement(int id, int[] nodeIds)
         : base(id, nodeIds)
        {
        }
        public ParabolicQuadrilateralElement(int id, int partId, int[] nodeIds)
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
            return new int[] { NodeIds[0], NodeIds[3], NodeIds[2], NodeIds[1],
                               NodeIds[7], NodeIds[6], NodeIds[5], NodeIds[4] };
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
            // NEG S1 = 1-4-3-2-8-7-6-5 . 0-3-2-1-7-6-5-4
            // POS S2 = 1-2-3-4-5-6-7-8 . 0-1-2-3-4-5-6-7
            switch (faceName)
            {
                case FeFaceName.S1:
                    return new int[] { NodeIds[0], NodeIds[3], NodeIds[2], NodeIds[1],
                                       NodeIds[7], NodeIds[6], NodeIds[5], NodeIds[4] };
                case FeFaceName.S2:
                    return new int[] { NodeIds[0], NodeIds[1], NodeIds[2], NodeIds[3],
                                       NodeIds[4], NodeIds[5], NodeIds[6], NodeIds[7] };
                default:
                    throw new NotSupportedException();
            }
        }
        public override int[] GetVtkCellFromFaceName(FeFaceName faceName)
        {
            // NEG S1 = 1-4-3-2-8-7-6-5 . 0-3-2-1-7-6-5-4
            // POS S2 = 1-2-3-4-5-6-7-8 . 0-1-2-3-4-5-6-7
            switch (faceName)
            {
                case FeFaceName.S1:
                    return new int[] { NodeIds[0], NodeIds[3], NodeIds[2], NodeIds[1],
                                       NodeIds[7], NodeIds[6], NodeIds[5], NodeIds[4] };
                case FeFaceName.S2:
                    return new int[] { NodeIds[0], NodeIds[1], NodeIds[2], NodeIds[3],
                                       NodeIds[4], NodeIds[5], NodeIds[6], NodeIds[7] };
                default:
                    throw new NotSupportedException();
            }
        }
        public override int[][] GetAllVtkCells()
        {
            throw new NotImplementedException();
        }
        public override Dictionary<FeFaceName, double> GetFaceNamesAndAreasFromNodeSet(HashSet<int> nodeSet, Dictionary<int, FeNode> nodes)
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
                if (i >= 4 && count <= i - 4) break;
            }
            // NEG S1 = 1-4-3-2 . 0-3-2-1
            Dictionary<FeFaceName, double> faces = new Dictionary<FeFaceName, double>();
            //
            if (faceNodeIds[0] && faceNodeIds[3] && faceNodeIds[2] && faceNodeIds[1])
                faces.Add(FeFaceName.S1, GetArea(FeFaceName.S1, nodes));
            //
            return faces;
        }
        public override double[] GetEquivalentForcesFromFaceName(FeFaceName faceName)
        {
            return new double[] { b, b, b, b, a, a, a, a };
        }
        public override double GetArea(FeFaceName faceName, Dictionary<int, FeNode> nodes)
        {
            int[] cell = GetVtkCellFromFaceName(faceName);
            return GeometryTools.RectangleArea(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]], nodes[cell[3]],
                                               nodes[cell[4]], nodes[cell[5]], nodes[cell[6]], nodes[cell[7]]);
        }
        public override double[] GetCG(FeFaceName faceName, Dictionary<int, FeNode> nodes, out double area)
        {
            int[] cell = GetVtkCellFromFaceName(faceName);
            double[] cg = GeometryTools.RectangleCG(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]], nodes[cell[3]],
                                                    nodes[cell[4]], nodes[cell[5]], nodes[cell[6]], nodes[cell[7]], out area);
            return cg;
        }
        public override FeElement DeepCopy()
        {
            return new ParabolicQuadrilateralElement(Id, PartId, NodeIds.ToArray());
        }
    }
}
