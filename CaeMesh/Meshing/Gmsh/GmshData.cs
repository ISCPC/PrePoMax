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
        public Dictionary<int, double> VertexIdMeshSize;
        public Dictionary<int, int> EdgeIdNumElements;
        public MeshSetupItem[] GmshSetupItems;
        public bool Preview;

        public GmshData(string geometryFileName, string inpFileName, MeshingParameters partMeshingParameters,
                        Dictionary<int, double> vertexIdMeshSize, Dictionary<int, int> edgeIdNumElements,
                        MeshSetupItem[] gmshSetupItems, bool preview)
        {
            if (gmshSetupItems.Length != 1)
                throw new CaeException("Currently, for a single part, only one active mesh setup item of the type: " +
                    "Shell gmsh, Tetrahedral gmsh, Transfinite mesh, Extrude mesh or Revolve mesh is possible.");
            //
            GeometryFileName = geometryFileName;
            InpFileName = inpFileName;
            PartMeshingParameters = partMeshingParameters;
            VertexIdMeshSize = vertexIdMeshSize;
            EdgeIdNumElements = edgeIdNumElements;
            GmshSetupItems = gmshSetupItems;
            Preview = preview;
        }

        public void WriteToFile(string fileName)
        {
            this.DumpToFile(fileName);
        }
    }
}
