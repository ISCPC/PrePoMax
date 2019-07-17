using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public enum FeFaceName
    {
        Empty,
        S1,
        S2,
        S3,
        S4,
        S5,
        S6
    }

    [Serializable]
    public abstract class FeElement
    {
        // Properties                                                                                                               
        public int Id;
        public int PartId;
        public int[] NodeIds;


        public FeElement(int id, int[] nodeIds)
            : this(id, -1, nodeIds)
        {
        }
        public FeElement(int id, int partId, int[] nodeIds)
        {
            Id = id;
            PartId = partId;
            NodeIds = nodeIds;
        }
       

        // Methods                                                                                                                  
        public static bool IsParabolic(FeElement element)
        {
            return element is ParabolicBeamElement || element is ParabolicTriangleElement || element is ParabolicQuadrilateralElement ||
                   element is ParabolicTetraElement || element is ParabolicWedgeElement || element is ParabolicHexaElement; 
        }


        // Abstract methods                                                                                                         
        abstract public int[] GetVtkNodeIds();
        abstract public int GetVtkCellType();
        abstract public FeFaceName GetFaceNameFromSortedNodeIds(int[] nodeIds);
        abstract public int[] GetNodeIdsFromFaceName(FeFaceName faceName);
        abstract public int[] GetVtkCellFromFaceName(FeFaceName faceName);
        abstract public Dictionary<FeFaceName, double> GetFaceNamesAndAreasFromNodeSet(HashSet<int> nodeSet, Dictionary<int, FeNode> nodes);
        abstract public double[] GetEquivalentForcesFromFaceName(FeFaceName faceName);
        abstract public double GetArea(FeFaceName faceName, Dictionary<int, FeNode> nodes);
        abstract public FeElement DeepCopy();
    }
}
