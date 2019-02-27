using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public class WireElement : FeElement1D
    {
        // Variables                                                                                                                


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public WireElement(int N1, int N2)
        {
            NodeIDs = new int[] { N1, N2 };
        }

        // Methods                                                                                                                  
        public override int[] GetVtkNodeIds()
        {
            // return copy
            return NodeIDs.ToArray();
        }

        public override FeFaceName GetFaceNameFromNodeIds(int[] nodeIds)
        {
            throw new NotImplementedException();
        }

        public override int[] GetNodeIdsFromFaceName(FeFaceName faceName)
        {
            throw new NotImplementedException();
        }

        public override int[] GetCellFromFaceName(FeFaceName faceName)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<FeFaceName, double> GetFaceNamesAndAreasFromNodeSet(HashSet<int> nodeSet, Dictionary<int, FeNode> nodes)
        {
            throw new NotImplementedException();
        }

        public override double[] GetEquivalentForcesFromFaceName()
        {
            throw new NotImplementedException();
        }

        public override double GetArea(FeFaceName faceName, Dictionary<int, FeNode> nodes)
        {
            throw new NotImplementedException();
        }
    }
}
