using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CaeMesh;
using CaeGlobals;

namespace FileInOut.Input
{
    static public class StlFileReader
    {
        public static FeMesh Read(string fileName)
        {
            if (File.Exists(fileName))
            {
                QuantumConcepts.Formats.StereoLithography.STLDocument stlFile = null;
                //
                System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                //
                using (Stream stream = File.OpenRead(fileName))
                {
                    watch.Start();
                    stlFile = QuantumConcepts.Formats.StereoLithography.STLDocument.Read(stream, true);
                    watch.Stop();
                }
                //
                FeNode node;
                int[] nodeIds;
                LinearTriangleElement element;
                Dictionary<int, FeNode> nodes = new Dictionary<int, FeNode>();
                Dictionary<int, FeElement> elements = new Dictionary<int, FeElement>();
                //
                CompareFeNodeCoods comparer = new CompareFeNodeCoods();
                Dictionary<FeNode, int> nodeMap = new Dictionary<FeNode, int>(stlFile.Facets.Count, comparer);
                BoundingBox box = new BoundingBox(); 
                int nodeId;
                int localCount;
                //
                foreach (var facet in stlFile.Facets)
                {
                    localCount = 0;
                    nodeIds = new int[3];
                    foreach (var v in facet.Vertices)
                    {
                        node = new FeNode(nodes.Count + 1, v.X, v.Y, v.Z);
                        box.IncludeNode(node);
                        //
                        if (nodeMap.TryGetValue(node, out nodeId))
                        {
                            node.Id = nodeId;
                        }
                        else
                        {
                            nodes.Add(node.Id, node);
                            nodeMap.Add(node, node.Id);
                        }
                        nodeIds[localCount++] = node.Id;
                    }
                    element = new LinearTriangleElement(elements.Count + 1, nodeIds);
                    elements.Add(element.Id, element);
                }
                //
                double epsilon = 1E-6;
                double max = box.GetDiagonal();
                //
                MergeNodes(nodes, elements, epsilon * max);
                //
                FeMesh mesh = new FeMesh(nodes, elements, MeshRepresentation.Geometry, ImportOptions.ImportStlParts);
                //
                //string namePrefix = Path.GetFileNameWithoutExtension(fileName).Replace(' ', '_');
                //FeMesh mesh = new FeMesh(nodes, elements, MeshRepresentation.Geometry, namePrefix);
                //
                //if (mesh.Parts.Count == 1)
                //{
                //    GeometryPart part = mesh.Parts.Values.First() as GeometryPart;
                //    mesh.Parts.Clear();
                //    part.Name = namePrefix;
                //    mesh.Parts.Add(part.Name, part);
                //}
                return mesh;
            }
            //
            return null;
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
                    if (mergeMap.ContainsKey(sortedNodes[j].Id)) continue;       // this node was merged and does not exist anymore

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
