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
            throw new NotImplementedException();
        }
        public override int[] GetVtkCellFromFaceName(FeFaceName faceName)
        {
            throw new NotImplementedException();
        }
        public override Dictionary<FeFaceName, double> GetFaceNamesAndAreasFromNodeSet(HashSet<int> nodeSet, Dictionary<int, FeNode> nodes)
        {
            throw new NotImplementedException();
        }
        public override double[] GetEquivalentForcesFromFaceName(FeFaceName faceName)
        {
            throw new NotImplementedException();
        }
        public override double GetArea(FeFaceName faceName, Dictionary<int, FeNode> nodes)
        {
            throw new NotImplementedException();
        }
        public override FeElement DeepCopy()
        {
            return new LinearQuadrilateralElement(Id, PartId, NodeIds.ToArray());
        }
    }
}
