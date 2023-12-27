using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CaeMesh;
using CaeGlobals;
using System.Collections;

namespace FileInOut.Input
{
    static public class ObjFileReader
    {
        private static string[] spaceSplitter = new string[] { " " };
        private static string[] lineSplitter = new string[] { "\r", "\n" };
        private static string[] allSplitter = new string[] { " ", "\r", "\n" };

        public static FeMesh Read(string fileName)
        {
            if (File.Exists(fileName))
            {
                string[] lines = File.ReadAllLines(fileName);
                //
                Dictionary<int, FeNode> nodes = new Dictionary<int, FeNode>();
                Dictionary<int, FeElement> elements = new Dictionary<int, FeElement>();
                //
                BoundingBox bBox = new BoundingBox();
                //
                List<string> vertexLines = new List<string>();
                List<string> faceLines = new List<string>();
                // Read vertices and lines
                for (int i = 0; i < lines.Length; i++)
                {
                    lines[i] = lines[i].Trim().ToUpper();
                    if (lines[i].StartsWith("V")) vertexLines.Add(lines[i]);
                    else if (lines[i].StartsWith("F")) faceLines.Add(lines[i]);
                }
                // Read nodes
                ReadNodes(vertexLines, nodes, ref bBox);
                // Read elements
                ReadFaces(faceLines, nodes, elements);
                //
                FeMesh mesh = new FeMesh(nodes, elements, MeshRepresentation.Mesh, null, null, false, ImportOptions.DetectEdges);
                //
                return mesh;
            }
            //
            return null;
        }
        private static void ReadNodes(List<string> lines, Dictionary<int, FeNode> nodes, ref BoundingBox bBox)
        {
            int count = 1;
            string[] tmp;
            FeNode node;
            //
            foreach (var line in lines)
            {
                tmp = line.Split(spaceSplitter, StringSplitOptions.RemoveEmptyEntries);
                node = new FeNode();
                node.Id = count++;
                node.X = double.Parse(tmp[1]);
                node.Y = double.Parse(tmp[2]);
                node.Z = double.Parse(tmp[3]);
                //
                bBox.IncludeNode(node);
                //
                nodes.Add(node.Id, node);
            }
        }
        private static void ReadFaces(List<string> lines, Dictionary<int, FeNode> nodes, Dictionary<int, FeElement> elements)
        {
            int id = 1;
            int loopId = -1;
            int firstId;
            int maxId;
            int[] nodeIds;
            double min;
            double minAngle;
            double maxAngle;
            string[] tmp;
            string[] data;
            string[] splitter = new string[] { "/" };
            FeElement element;
            LinearTriangleElement lte;
            List<FeElement>[] loopElements;
            //
            foreach (var line in lines)
            {
                data = line.Split(spaceSplitter, StringSplitOptions.RemoveEmptyEntries);
                //
                nodeIds = new int[data.Length - 1];
                for (int i = 0; i < nodeIds.Length; i++)
                {
                    tmp = data[i + 1].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                    nodeIds[i] = int.Parse(tmp[0]);
                }
                // Triangles
                if (nodeIds.Length == 3)
                {
                    element = new LinearTriangleElement(id++, nodeIds);
                    elements.Add(element.Id, element);
                }
                // Quads
                else if (nodeIds.Length == 4)
                {
                    element = new LinearQuadrilateralElement(id++, nodeIds);
                    elements.Add(element.Id, element);
                }
                // Split multi-polygons to triangles
                else if (nodeIds.Length > 4)
                {
                    maxId = -1;
                    maxAngle = 0;
                    loopElements = new List<FeElement>[nodeIds.Length];
                    // Find max min angle
                    for (int i = 0; i < nodeIds.Length; i++)
                    {
                        loopId = id;
                        minAngle = double.MaxValue;
                        loopElements[i] = new List<FeElement>();
                        //
                        for (int j = 0; j < nodeIds.Length - 2; j++)
                        {
                            lte = new LinearTriangleElement(loopId++, new int[] { nodeIds[0], nodeIds[j + 1], nodeIds[j + 2] });
                            //
                            min = lte.GetMinAngleDeg(nodes);
                            if (min < minAngle) minAngle = min;
                            //
                            loopElements[i].Add(lte);
                        }
                        //
                        if (minAngle > maxAngle)
                        {
                            maxAngle = minAngle;
                            maxId = i;
                        }
                        // Loop nodes
                        firstId = nodeIds[0];
                        Array.Copy(nodeIds, 1, nodeIds, 0, nodeIds.Length - 1);
                        nodeIds[nodeIds.Length - 1] = firstId;
                    }
                    //
                    id += loopElements[maxId].Count;
                    foreach (var loopElement in loopElements[maxId]) elements.Add(loopElement.Id, loopElement);
                }
            }
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
                                     Dictionary<int, FeElement> elements, Dictionary<int, HashSet<int>> edgeIdNodeIds,
                                     Dictionary<int, GeomCurveType> edgeTypes)
        {
            string[] data = elementData.Split(lineSplitter, StringSplitOptions.RemoveEmptyEntries);
            GeomCurveType edgeType;
            GeomCurveType prevEdgeType;
            string[] splitData;
            int id;
            int internalId = 1;
            int[] nodeIds;
            LinearBeamElement element;
            //
            int edgeId;
            HashSet<int> edgeNodeIds = new HashSet<int>();
            HashSet<int> edgeNodeIdsOut;
            //
            for (int i = 1; i < data.Length; i++)   // skip first row: Number of edges: 4
            {
                splitData = data[i].Split(spaceSplitter, StringSplitOptions.RemoveEmptyEntries);
                edgeType = (GeomCurveType)Enum.Parse(typeof(GeomCurveType), splitData[0]);
                edgeId = int.Parse(splitData[1]);
                //
                edgeNodeIds.Clear();
                for (int j = 2; j < splitData.Length - 1; j++)
                {
                    id = internalId + offsetElementId;
                    nodeIds = new int[2];
                    nodeIds[0] = int.Parse(splitData[j]) + offsetNodeId;
                    nodeIds[1] = int.Parse(splitData[j + 1]) + offsetNodeId;
                    //
                    edgeNodeIds.Add(nodeIds[0]);
                    edgeNodeIds.Add(nodeIds[1]);
                    //
                    element = new LinearBeamElement(id, nodeIds);
                    elements.Add(element.Id, element);
                    //
                    internalId++;
                }
                //
                if (edgeIdNodeIds.TryGetValue(edgeId, out edgeNodeIdsOut)) edgeNodeIdsOut.UnionWith(edgeNodeIds);
                else edgeIdNodeIds.Add(edgeId, new HashSet<int>(edgeNodeIds));      // create a copy!!!
                //
                if (edgeTypes.TryGetValue(edgeId, out prevEdgeType))
                {
                    if (prevEdgeType != edgeType) throw new NotSupportedException();
                }
                else edgeTypes.Add(edgeId, edgeType);
            }
            //
            return internalId - 1; // return number of read edges
        }

        private static void MergeNodes(Dictionary<int, FeNode> nodes,
                                       Dictionary<int, FeElement> elements,
                                       Dictionary<int, HashSet<int>> surfaceIdNodeIds,
                                       Dictionary<int, HashSet<int>> edgeIdNodeIds,
                                       double epsilon,
                                       out int[] mergedNodes)
        {
            int count = 0;
            FeNode[] sortedNodes = new FeNode[nodes.Count];
            // Sort the nodes by x
            foreach (var entry in nodes) sortedNodes[count++] = entry.Value;
            //
            IComparer<FeNode> comparerByX = new CompareFeNodeByX();
            Array.Sort(sortedNodes, comparerByX);
            // Create a map of node ids to be merged to another node id
            Dictionary<int, int> oldIdNewIdMap = new Dictionary<int, int>();
            for (int i = 0; i < sortedNodes.Length - 1; i++)
            {
                if (oldIdNewIdMap.ContainsKey(sortedNodes[i].Id)) continue;       // this node was merged and does not exist anymore
                //
                for (int j = i + 1; j < sortedNodes.Length; j++)
                {
                    if (oldIdNewIdMap.ContainsKey(sortedNodes[j].Id)) continue;   // this node was merged and does not exist anymore

                    if (Math.Abs(sortedNodes[i].X - sortedNodes[j].X) < epsilon)
                    {
                        if (Math.Abs(sortedNodes[i].Y - sortedNodes[j].Y) < epsilon)
                        {
                            if (Math.Abs(sortedNodes[i].Z - sortedNodes[j].Z) < epsilon)
                            {
                                oldIdNewIdMap.Add(sortedNodes[j].Id, sortedNodes[i].Id);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            // Collect close nodes into bins
            HashSet<int> allNodeIds;
            Dictionary<int, HashSet<int>> newNodeIdAllNodeIds = new Dictionary<int, HashSet<int>>();
            foreach (var entry in oldIdNewIdMap)
            {
                if (newNodeIdAllNodeIds.TryGetValue(entry.Value, out allNodeIds)) allNodeIds.Add(entry.Key);
                else newNodeIdAllNodeIds.Add(entry.Value, new HashSet<int>() { entry.Value, entry.Key });
            }
            // Find the smallest node id
            oldIdNewIdMap.Clear();
            int[] sortedNodeIds;
            foreach (var entry in newNodeIdAllNodeIds)
            {
                sortedNodeIds = entry.Value.ToArray();
                Array.Sort(sortedNodeIds);
                //
                for (int i = 1; i < sortedNodeIds.Length; i++) oldIdNewIdMap.Add(sortedNodeIds[i], sortedNodeIds[0]);
            }
             // Remove unused nodes
            mergedNodes = oldIdNewIdMap.Keys.ToArray();
            foreach (int mergedNode in mergedNodes) nodes.Remove(mergedNode);
            // Apply the map to the elements
            int newId;
            HashSet<int> nodeIds = new HashSet<int>();
            List<int> elementIdsToRemove = new List<int>();
            foreach (var entry in elements)
            {
                nodeIds.Clear();
                for (int i = 0; i < entry.Value.NodeIds.Length; i++)
                {
                    if (oldIdNewIdMap.TryGetValue(entry.Value.NodeIds[i], out newId)) entry.Value.NodeIds[i] = newId;
                    //
                    nodeIds.Add(entry.Value.NodeIds[i]);
                }
                if (nodeIds.Count != entry.Value.NodeIds.Length) elementIdsToRemove.Add(entry.Key);
            }
            // Remove collapsed elements
            foreach (var elementId in elementIdsToRemove)
            {
                elements.Remove(elementId);
                // Might be also necessary to remove some nodes ?
            }
            // Surface node ids
            HashSet<int> newIds = new HashSet<int>();
            foreach (var entry in surfaceIdNodeIds)
            {
                newIds.Clear();
                foreach (var nodeId in entry.Value)
                {
                    if (oldIdNewIdMap.TryGetValue(nodeId, out newId)) newIds.Add(newId);
                    else newIds.Add(nodeId);
                }
                entry.Value.Clear();
                entry.Value.UnionWith(newIds);
            }
            // Edge node ids
            foreach (var entry in edgeIdNodeIds)
            {
                newIds.Clear();
                foreach (var nodeId in entry.Value)
                {
                    if (oldIdNewIdMap.TryGetValue(nodeId, out newId)) newIds.Add(newId);
                    else newIds.Add(nodeId);
                }
                entry.Value.Clear();
                entry.Value.UnionWith(newIds);
            }

        }
        private static void MergeEdgeElements(Dictionary<int, FeElement> elements)
        {
            int[] key;
            FeElement[] elementsToRemove;
            List<FeElement> elementsToMerge;
            CompareIntArray comparer = new CompareIntArray();
            Dictionary<int[], List<FeElement>> nodeIdsElements = new Dictionary<int[], List<FeElement>>(comparer);
            foreach (var entry in elements)
            {
                if (entry.Value is FeElement1D edgeElement)
                {
                    key = edgeElement.NodeIds;
                    Array.Sort(key);
                    if (nodeIdsElements.TryGetValue(key, out elementsToMerge)) elementsToMerge.Add(edgeElement);
                    else nodeIdsElements.Add(key, new List<FeElement>() { edgeElement });
                }
            }
            //
            foreach (var entry in nodeIdsElements)
            {
                if (entry.Value.Count > 1)
                {
                    elementsToRemove = entry.Value.ToArray();
                    for (int i = 1; i < elementsToRemove.Length; i++)
                    {
                        elements.Remove(elementsToRemove[i].Id);
                    }
                }
            }
        }

    }
}
