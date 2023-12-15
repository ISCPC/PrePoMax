using CaeGlobals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace CaeMesh
{
    [Serializable]
    public class GmshData
    {
        // Variables                                                                                                                
        public string GeometryFileName;
        public string InpFileName;
        public MeshingParameters PartMeshingParameters;
        public MeshSetupItem[] GmshSetupItems;
        public Dictionary<int, FeNode> VertexNodes;
        public Dictionary<int, double> VertexNodeIdMeshSize;
        public Dictionary<int[], int> EdgeVertexNodeIdsNumElements;
        public bool Preview;


        public GmshData(string geometryFileName, string inpFileName, MeshingParameters partMeshingParameters,
                        MeshSetupItem[] gmshSetupItems, Dictionary<int, FeNode> vertexNodes,
                        Dictionary<int, double> vertexNodeIdMeshSize, Dictionary<int[], int> edgeVertexNodeIdsNumElements,
                        bool preview)
        {
            if (gmshSetupItems.Length != 1)
                throw new CaeException("Currently, for a single part, only one active mesh setup item of the type: " +
                    "Shell gmsh, Tetrahedral gmsh, Transfinite mesh, Extrude mesh or Revolve mesh is possible.");
            //
            GeometryFileName = geometryFileName;
            InpFileName = inpFileName;
            PartMeshingParameters = partMeshingParameters;
            GmshSetupItems = gmshSetupItems;
            VertexNodes = vertexNodes;
            VertexNodeIdMeshSize = vertexNodeIdMeshSize;
            EdgeVertexNodeIdsNumElements = edgeVertexNodeIdsNumElements;
            Preview = preview;
        }

        public void WriteToFile(string fileName)
        {
            this.DumpToFile(fileName);
        }
    }
}
