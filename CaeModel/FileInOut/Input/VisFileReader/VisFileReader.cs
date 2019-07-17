using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CaeMesh;

namespace FileInOut.Input
{
    static public class VisFileReader
    {
        private static string[] spaceSplitter = new string[] { " " };
        private static string[] lineSplitter = new string[] { "\r", "\n" };
        private static string[] allSplitter = new string[] { " ", "\r", "\n" };

        public static FeMesh Read(string fileName)
        {
            if (File.Exists(fileName))
            {
                string data = File.ReadAllText(fileName);

                string[] faceData = data.Split(new string[] { "Face number: " }, StringSplitOptions.RemoveEmptyEntries);

                Dictionary<int, FeNode> nodes = new Dictionary<int, FeNode>();
                Dictionary<int, FeElement> elements = new Dictionary<int, FeElement>();

                BoundingBox bBox = new BoundingBox();
                int offsetNodeId = 0;
                int offsetElementId = 0;

                for (int i = 1; i < faceData.Length; i++)   // start with 1 to skip first line ********
                {
                    ReadFace(faceData[i], ref offsetNodeId, nodes, ref offsetElementId, elements, ref bBox);
                }


                double epsilon = 1E-4;
                double max = bBox.GetDiagonal();
                MergeNodes(nodes, elements, epsilon * max);

                FeMesh mesh = new FeMesh(nodes, elements, MeshRepresentation.Geometry);

                mesh.ConvertLineFeElementsToEdges();

                mesh.RemoveElementsByType<FeElement1D>();
                mesh.RemoveElementsByType<FeElement3D>();

                return mesh;
            }

            return null;
        }

        private static void ReadFace(string faceData, 
                                     ref int offsetNodeId, Dictionary<int, FeNode> nodes, 
                                     ref int offsetElementId, Dictionary<int, FeElement> elements,
                                     ref BoundingBox bBox)
        {
            int numOfNodes = 0;
            int numOfElements = 0;
            string[] data = faceData.Split(new string[] { "*", "Number of "}, StringSplitOptions.RemoveEmptyEntries);

            string[] tmp = data[0].Split(allSplitter, StringSplitOptions.RemoveEmptyEntries);
            int orientation = int.Parse(tmp[3]);
            bool reverse = orientation == 1;

            for (int i = 1; i < data.Length; i++)
            {
                if (data[i].StartsWith("nodes"))
                {
                    numOfNodes = ReadNodes(data[i], offsetNodeId, nodes, ref bBox);
                }
                else if (data[i].StartsWith("triangles"))
                {
                    numOfElements = ReadTriangles(data[i], reverse, offsetNodeId, offsetElementId, elements);
                    offsetElementId += numOfElements;
                }
                else if (data[i].StartsWith("edges"))
                {
                    numOfElements = ReadEdges(data[i], offsetNodeId, offsetElementId, elements);
                    offsetElementId += numOfElements;
                }
            }
            offsetNodeId += numOfNodes;
        }

        private static int ReadNodes(string nodeData, int offsetNodeId, Dictionary<int, FeNode> nodes, ref BoundingBox bBox)
        {
            string[] data = nodeData.Split(lineSplitter, StringSplitOptions.RemoveEmptyEntries);
            string[] tmp;
            FeNode node;
            
            for (int i = 1; i < data.Length; i++)   // skip first row: Number of nodes: 14
            {
                tmp = data[i].Split(spaceSplitter, StringSplitOptions.RemoveEmptyEntries);
                node = new FeNode();
                node.Id = int.Parse(tmp[0]) + offsetNodeId;
                node.X = double.Parse(tmp[1]);
                node.Y = double.Parse(tmp[2]);
                node.Z = double.Parse(tmp[3]);

                bBox.CheckNode(node);

                nodes.Add(node.Id, node);
            }

            return data.Length - 1; // return number of read nodes
        }
        private static int ReadTriangles(string elementData, bool reverse, int offsetNodeId, int offsetElementId,
                                         Dictionary<int, FeElement> elements)
        {
            string[] data = elementData.Split(lineSplitter, StringSplitOptions.RemoveEmptyEntries);
            string[] tmp;
            int id;
            int[] nodeIds;
            LinearTriangleElement element;

            for (int i = 1; i < data.Length; i++)   // skip first row: Number of elements: 23
            {
                tmp = data[i].Split(spaceSplitter, StringSplitOptions.RemoveEmptyEntries);
                id = int.Parse(tmp[0]) + offsetElementId;
                nodeIds = new int[3];
                nodeIds[0] = int.Parse(tmp[1]) + offsetNodeId;
                if (reverse)
                {
                    nodeIds[1] = int.Parse(tmp[3]) + offsetNodeId;
                    nodeIds[2] = int.Parse(tmp[2]) + offsetNodeId;
                }
                else
                {
                    nodeIds[1] = int.Parse(tmp[2]) + offsetNodeId;
                    nodeIds[2] = int.Parse(tmp[3]) + offsetNodeId;
                }
                element = new LinearTriangleElement(id, nodeIds);

                elements.Add(element.Id, element);
            }

            return data.Length - 1; // return number of read elements
        }
        private static int ReadEdges(string elementData, int offsetNodeId, int offsetElementId,
                                     Dictionary<int, FeElement> elements)
        {
            string[] data = elementData.Split(lineSplitter, StringSplitOptions.RemoveEmptyEntries);
            string[] allIds;
            int id;
            int internalId = 1;
            int[] nodeIds;
            LinearBeamElement element;

            for (int i = 1; i < data.Length; i++)   // skip first row: Number of edges: 4
            {
                allIds = data[i].Split(spaceSplitter, StringSplitOptions.RemoveEmptyEntries);

                for (int j = 1; j < allIds.Length - 1; j++)
                {
                    id = internalId + offsetElementId;
                    nodeIds = new int[2];
                    nodeIds[0] = int.Parse(allIds[j]) + offsetNodeId;
                    nodeIds[1] = int.Parse(allIds[j + 1]) + offsetNodeId;

                    element = new LinearBeamElement(id, nodeIds);
                    elements.Add(element.Id, element);

                    internalId++;
                }
            }

            return internalId - 1; // return number of read edges
        }

        private static void MergeNodes(Dictionary<int, FeNode> nodes, Dictionary<int, FeElement> elements, double epsilon)
        {
            int count = 0;
            FeNode[] sortedNodes = new FeNode[nodes.Count];

            // sort the nodes by x
            foreach (var entry in nodes)
            {
                sortedNodes[count++] = entry.Value;
            }
            IComparer<FeNode> comparerByX = new CompareFeNodeByX();
            Array.Sort(sortedNodes, comparerByX);

            // create a map of node ids to be merged to another node id
            Dictionary<int, int> mergeMap = new Dictionary<int, int>();
            for (int i = 0; i < sortedNodes.Length - 1; i++)
            {
                if (mergeMap.ContainsKey(sortedNodes[i].Id)) continue;       // this node was merged and does not exist anymore

                for (int j = i + 1; j < sortedNodes.Length; j++)
                {
                    if (mergeMap.ContainsKey(sortedNodes[j].Id)) continue;   // this node was merged and does not exist anymore

                    if (Math.Abs(sortedNodes[i].X - sortedNodes[j].X) < epsilon)
                    {
                        if (Math.Abs(sortedNodes[i].Y - sortedNodes[j].Y) < epsilon)
                        {
                            if (Math.Abs(sortedNodes[i].Z - sortedNodes[j].Z) < epsilon)
                            {
                                mergeMap.Add(sortedNodes[j].Id, sortedNodes[i].Id);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }

            }

            // apply the map to the elements
            int newId;
            HashSet<int> nodeIds = new HashSet<int>();
            List<int> elementIdsToRemove = new List<int>();
            foreach (var entry in elements)
            {
                nodeIds.Clear();
                for (int i = 0; i < entry.Value.NodeIds.Length; i++)
                {
                    if (mergeMap.TryGetValue(entry.Value.NodeIds[i], out newId))
                        entry.Value.NodeIds[i] = newId;

                    nodeIds.Add(entry.Value.NodeIds[i]);
                }
                if (nodeIds.Count != entry.Value.NodeIds.Length) elementIdsToRemove.Add(entry.Key);
            }

            // remove collapsed elements
            foreach (var elementId in elementIdsToRemove)
            {
                elements.Remove(elementId);
                // might be also necessary to remove some nodes ?
            }

            // remove unused nodes
            foreach (var entry in mergeMap)
            {
                nodes.Remove(entry.Key);
            }
        }


    }
}
