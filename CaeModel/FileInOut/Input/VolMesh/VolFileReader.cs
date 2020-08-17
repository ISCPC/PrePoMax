using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CaeMesh;

namespace FileInOut.Input
{
    [Serializable]
    public static class VolFileReader
    {
        // Methods                                                                                                                  
        static public FeMesh Read(string fileName, ElementsToImport elementsToImport)
        {
            return Read(fileName, elementsToImport, false);
        }
        static public FeMesh Read(string fileName, ElementsToImport elementsToImport, bool convertToSecondOrder)
        {
            if (fileName != null && File.Exists(fileName))
            {
                string[] lines = File.ReadAllLines(fileName);
                //
                List<List<string>> dataSets = GetDataSets(lines);
                //
                int elementStartId = 1;
                Dictionary<int, FeNode> nodes = null;
                Dictionary<int, FeElement> elements = new Dictionary<int,FeElement>();
                // Geometry: itemId, allNodeIds
                Dictionary<int, HashSet<int>> surfaceIdNodeIds = new Dictionary<int, HashSet<int>>();
                Dictionary<int, HashSet<int>> edgeIdNodeIds = new Dictionary<int, HashSet<int>>();
                HashSet<int> vertexNodeIds = new HashSet<int>();
                //
                foreach (List<string> dataSet in dataSets)
                {
                    if (dataSet[0] == VolKeywords.points.ToString()) // Nodes
                    {
                        nodes = GetNodes(dataSet.ToArray());
                    }
                    else if (dataSet[0] == VolKeywords.volumeelements.ToString()) // 3D Elements
                    {
                        AddVolumeElements(dataSet.ToArray(), elements, ref elementStartId);
                    }
                    else if (dataSet[0] == VolKeywords.surfaceelements.ToString() ||
                             dataSet[0] == VolKeywords.surfaceelementsuv.ToString()) // 2D Elements
                    {
                        AddSurfaceElements(dataSet.ToArray(), elements, ref elementStartId, surfaceIdNodeIds);
                    }
                    else if (dataSet[0] == VolKeywords.edgesegmentsgi2.ToString()) // 1D Elements
                    {
                        AddLineElements(dataSet.ToArray(), elements, ref elementStartId, edgeIdNodeIds, vertexNodeIds);
                    }
                }
                //
                FeMesh mesh = new FeMesh(nodes, elements, MeshRepresentation.Mesh, null, null, convertToSecondOrder,
                                         ImportOptions.None);
                //
                mesh.ConvertLineFeElementsToEdges(vertexNodeIds);
                //
                mesh.RenumberVisualizationSurfaces(surfaceIdNodeIds);
                mesh.RenumberVisualizationEdges(edgeIdNodeIds);
                //
                if (elementsToImport != ElementsToImport.All)
                {
                    if (!elementsToImport.HasFlag(ElementsToImport.Beam)) mesh.RemoveElementsByType<FeElement1D>();
                    if (!elementsToImport.HasFlag(ElementsToImport.Shell)) mesh.RemoveElementsByType<FeElement2D>();
                    if (!elementsToImport.HasFlag(ElementsToImport.Solid)) mesh.RemoveElementsByType<FeElement3D>();
                }
                //
                return mesh;
            }
            //
            return null;
        }

        static private List<List<string>> GetDataSets(string[] lines)
        {
            int count = 0;
            List<string> dataSet = new List<string>();
            List<List<string>> dataSets = new List<List<string>>();

            HashSet<string> keywords = new HashSet<string>(Enum.GetNames(typeof(VolKeywords)));

            for (int i = 0; i < lines.Length; i++)
            {
                if (keywords.Contains(lines[i]))
                {
                    count++;
                    if (count == 1)
                    {
                        dataSets.Add(dataSet);
                        dataSet = new List<string>();
                        dataSet.Add(lines[i]);
                        count = 0;
                    }
                }
                else
                    dataSet.Add(lines[i]);
            }
            dataSets.Add(dataSet);

            return dataSets;
        }
        static private Dictionary<int, FeNode> GetNodes(string[] lines)
        {
            Dictionary<int, FeNode> nodes = new Dictionary<int, FeNode>();
            int id = 1;
            int N = int.Parse(lines[1]);
            FeNode node;
            string[] record1;
            string[] splitter = new string[] { " " };


            // line 0 is the line with the Keyword
            for (int i = 2; i < N + 2; i ++)
            {
                record1 = lines[i].Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                node = new FeNode();
                node.Id = id;
                node.X = double.Parse(record1[0]);
                node.Y = double.Parse(record1[1]);
                node.Z = double.Parse(record1[2]);

                nodes.Add(id, node);
                id++;
            }

            return nodes;
        }
        static private void AddVolumeElements(string[] lines, Dictionary<int, FeElement> elements, ref int elementStartId)
        {
            int numNodes;
            int N = int.Parse(lines[1]);
            string[] record;
            string[] splitter = new string[] { " " };
            FeElement3D element = null;
            // Line 0 is the line with the Keyword
            for (int i = 2; i < N + 2; i++)
            {
                record = lines[i].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                numNodes = int.Parse(record[1]);
                switch (numNodes)
                {
                    case 4:
                        element = GetLinearTetraElement(elementStartId, record);
                        break;
                    case 6:
                        element = GetLinearWedgeElement(elementStartId, record);
                        break;
                    case 10:
                        element = GetParabolicTetraElement(elementStartId, record);
                        break;
                    default:
                        throw new NotSupportedException();
                }
                elements.Add(elementStartId, element);
                elementStartId++;
            }
        }
        static private void AddSurfaceElements(string[] lines, Dictionary<int, FeElement> elements, ref int startId,
                                               Dictionary<int, HashSet<int>> surfaceIdNodeIds)
        {
            //# surfnr    bcnr   domin  domout      np      p1      p2      p3  
            //surfaceelements                                                   
            //440                                                               
            // 2 1 1 0 3 46 47 57                                               
            //                                                                  
            // 2 1 1 0 6 2 1 407 6824 6825 6420                                 

            int numNodes;
            int N = int.Parse(lines[1]);
            int surfId;
            string[] record;
            string[] splitter = new string[] { " " };
            HashSet<int> surface;

            FeElement2D element = null;

            // line 0 is the line with the Keyword                              
            for (int i = 2; i < N + 2; i++)
            {
                record = lines[i].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                numNodes = int.Parse(record[4]);
                switch (numNodes)
                {
                    case 3:
                        element = GetLinearTriangleElement(startId, record);
                        break;
                    case 4:
                        element = GetLinearQuadrilateralElement(startId, record);
                        break;
                    case 6:
                        element = GetParabolicTriangleElement(startId, record);
                        break;
                    case 8:
                        element = GetParabolicQuadrilateralElement(startId, record);
                        break;
                    default:
                        throw new NotSupportedException();
                }
                elements.Add(element.Id, element);
                
                surfId = int.Parse(record[1]);
                if (surfaceIdNodeIds.TryGetValue(surfId, out surface)) surface.UnionWith(element.NodeIds);
                else surfaceIdNodeIds.Add(surfId, new HashSet<int>(element.NodeIds));   // create a copy!!!

                startId++;
            }
        }
        static private void AddLineElements(string[] lines, Dictionary<int, FeElement> elements, ref int startId,
                                            Dictionary<int, HashSet<int>> edges, HashSet<int> vertexNodeIds)
        {
            //# surfid  0   p1   p2   trignum1    trignum2   domin/surfnr1    domout/surfnr2   ednr1   dist1   ednr2   dist2  
            //edgesegmentsgi2                                                                                                 
            //170                                                                                                             
            //       1       0       1       9       -1       -1        0        0        1         -100        1        -49.9
            //       1       0       9       2       -1       -1        0        0        2        -49.9        1           -0
            //       1       0      10       3      -1        -1        0        0        3         50.1        2           -0
            int N = int.Parse(lines[1]);
            int edgeId;
            int prevSurfaceId = -1;
            int surfceId;
            string[] record;
            string[] splitter = new string[] { " " };
            HashSet<int> edge;
            FeElement1D element = null;
            Dictionary<int, int> nodeIdCount;
            Dictionary<int, Dictionary<int, int>> edgeIdNodeIdCount = new Dictionary<int, Dictionary<int, int>>();
            // Line 0 is the line with the Keyword
            for (int i = 2; i < N + 2; i++)
            {
                record = lines[i].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                // Surface id
                surfceId = int.Parse(record[0]);
                // Edge id
                edgeId = int.Parse(record[10]);
                // If this is a new surface
                if (surfceId != prevSurfaceId)
                {
                    foreach (var edgeEntry in edgeIdNodeIdCount)
                    {
                        foreach (var nodeEntry in edgeEntry.Value)
                        {
                            if (nodeEntry.Value == 1) vertexNodeIds.Add(nodeEntry.Key);
                        }
                    }
                    edgeIdNodeIdCount.Clear();
                    prevSurfaceId = surfceId;
                }
                // Element
                element = GetLinearBeamElement(startId, record);
                elements.Add(startId, element);
                // Count all nodes of a singe surface
                if (!edgeIdNodeIdCount.TryGetValue(edgeId, out nodeIdCount))
                {
                    nodeIdCount = new Dictionary<int, int>();
                    edgeIdNodeIdCount.Add(edgeId, nodeIdCount);
                }
                for (int j = 0; j < 2; j++)
                {
                    if (nodeIdCount.ContainsKey(element.NodeIds[j])) nodeIdCount[element.NodeIds[j]]++;
                    else nodeIdCount.Add(element.NodeIds[j], 1);
                }
                // Add nodes to an edge
                if (edges.TryGetValue(edgeId, out edge)) edge.UnionWith(element.NodeIds);
                else edges.Add(edgeId, new HashSet<int>(element.NodeIds)); // create a copy!!!
                //
                startId++;
            }
            //
            foreach (var edgeEntry in edgeIdNodeIdCount)
            {
                foreach (var nodeEntry in edgeEntry.Value)
                {
                    if (nodeEntry.Value == 1) vertexNodeIds.Add(nodeEntry.Key);
                }
            }
        }
      

        //  LINEAR ELEMENTS                                                                                        
        static private LinearBeamElement GetLinearBeamElement(int id, string[] record)
        {
            int n = 2;
            int partId = int.Parse(record[0]);
            int[] nodes = new int[n];
            //
            for (int i = 0; i < n; i++) nodes[i] = int.Parse(record[i + 2]);
            //
            return new LinearBeamElement(id, -1, nodes);
        }
        static private LinearTriangleElement GetLinearTriangleElement(int id, string[] record)
        {
            int n = 3;
            int partId = int.Parse(record[1]);
            int[] nodes = new int[n];
            //
            nodes[0] = int.Parse(record[5]);
            nodes[1] = int.Parse(record[6]);
            nodes[2] = int.Parse(record[7]);
            //
            return new LinearTriangleElement(id, -1, nodes);
        }
        static private LinearQuadrilateralElement GetLinearQuadrilateralElement(int id, string[] record)
        {
            int n = 4;
            int partId = int.Parse(record[1]);
            int[] nodes = new int[n];
            //
            nodes[0] = int.Parse(record[5]);
            nodes[1] = int.Parse(record[6]);
            nodes[2] = int.Parse(record[7]);
            nodes[3] = int.Parse(record[8]);
            //
            return new LinearQuadrilateralElement(id, -1, nodes);
        }
        static private LinearTetraElement GetLinearTetraElement(int id, string[] record)
        {
            int n = 4;
            int partId = int.Parse(record[0]);
            int[] nodes = new int[n];
            //
            nodes[0] = int.Parse(record[2]);
            nodes[1] = int.Parse(record[4]);    // swap 3 and 4
            nodes[2] = int.Parse(record[3]);
            nodes[3] = int.Parse(record[5]);
            //
            return new LinearTetraElement(id, partId, nodes);
        }
        static private LinearWedgeElement GetLinearWedgeElement(int id, string[] record)
        {
            int n = 6;
            int partId = int.Parse(record[0]);
            int[] nodes = new int[n];
            //
            nodes[0] = int.Parse(record[2]);
            nodes[1] = int.Parse(record[4]);    // swap 3 and 4
            nodes[2] = int.Parse(record[3]);
            nodes[3] = int.Parse(record[5]);
            nodes[4] = int.Parse(record[7]);
            nodes[5] = int.Parse(record[6]);
            //
            return new LinearWedgeElement(id, partId, nodes);
        }


        //  PARABOLIC ELEMENTS                                                                                           
        static private ParabolicTriangleElement GetParabolicTriangleElement(int id, string[] record)
        {
            int n = 6;
            int partId = int.Parse(record[1]);
            int[] nodes = new int[n];
            //
            nodes[0] = int.Parse(record[5]);
            nodes[1] = int.Parse(record[6]);
            nodes[2] = int.Parse(record[7]);
            nodes[3] = int.Parse(record[10]);
            nodes[4] = int.Parse(record[8]);
            nodes[5] = int.Parse(record[9]);
            //
            return new ParabolicTriangleElement(id, -1, nodes);
        }
        static private ParabolicQuadrilateralElement GetParabolicQuadrilateralElement(int id, string[] record)
        {
            int n = 8;
            int partId = int.Parse(record[1]);
            int[] nodes = new int[n];
            //
            nodes[0] = int.Parse(record[5]);
            nodes[1] = int.Parse(record[6]);
            nodes[2] = int.Parse(record[7]);
            nodes[3] = int.Parse(record[8]);
            nodes[4] = int.Parse(record[9]);
            nodes[5] = int.Parse(record[12]);
            nodes[6] = int.Parse(record[10]);
            nodes[7] = int.Parse(record[11]);
            //
            return new ParabolicQuadrilateralElement(id, -1, nodes);
        }
        static private ParabolicTetraElement GetParabolicTetraElement(int id, string[] record)
        {
            int n = 10;
            int partId = int.Parse(record[0]);
            int[] nodes = new int[n];
            //
            nodes[0] = int.Parse(record[2]);
            nodes[1] = int.Parse(record[4]);    // swap 3 and 4
            nodes[2] = int.Parse(record[3]);
            nodes[3] = int.Parse(record[5]);
            nodes[4] = int.Parse(record[7]);
            nodes[5] = int.Parse(record[9]);
            nodes[6] = int.Parse(record[6]);
            nodes[7] = int.Parse(record[8]);
            nodes[8] = int.Parse(record[11]);
            nodes[9] = int.Parse(record[10]);
            //
            return new ParabolicTetraElement(id, partId, nodes);
        }

    }
}
