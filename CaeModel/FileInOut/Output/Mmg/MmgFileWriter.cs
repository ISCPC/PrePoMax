using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CaeMesh;
using CaeGlobals;

namespace FileInOut.Output
{
    static public class MmgFileWriter
    {
        public static void Write(string fileName, FeMesh mesh, string[] partNames)
        {
            BasePart part;
            int numOfVertices = 0;
            int numOfTriangles = 0;
            //
            foreach (var partName in partNames)
            {
                part = mesh.Parts[partName];
                if (part is GeometryPart)
                {
                    numOfVertices += part.NodeLabels.Length;
                    numOfTriangles += part.Labels.Length;
                }
                else
                {
                    numOfVertices += part.NodeLabels.Length;
                    foreach (var elementId in part.Labels)
                    {
                        if (mesh.Elements[elementId] is LinearQuadrilateralElement ||
                            mesh.Elements[elementId] is ParabolicQuadrilateralElement)
                        {
                            numOfTriangles += 2;
                        }
                        else numOfTriangles += 1;
                    }
                }
            }
            
            //
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("MeshVersionFormatted 2");
            sb.AppendLine("Dimension 3");
            // Vertices
            sb.AppendLine("Vertices");
            sb.AppendLine(numOfVertices.ToString());
            //
            FeNode node;
            FeElement element;
            double[] coor;
            int[] nodeIds;
            int count;
            Dictionary<int, int> nodeIdVertexId = new Dictionary<int, int>();
            //
            count = 1;
            int vertexPartId = 1;
            foreach (var partName in partNames)
            {
                part = mesh.Parts[partName];
                //
                foreach (var nodeId in part.NodeLabels)
                {
                    node = mesh.Nodes[nodeId];
                    coor = node.Coor;
                    sb.AppendFormat("{0} {1} {2} {3}{4}", coor[0], coor[1], coor[2], vertexPartId, Environment.NewLine);
                    nodeIdVertexId.Add(node.Id, count);
                    count++;
                }
            }
            // Triangles
            sb.AppendLine("Triangles");
            sb.AppendLine(numOfTriangles.ToString());
            int trianglePartId = 1;
            int[] key;
            int[] indices;
            CaeGlobals.CompareIntArray comparer = new CompareIntArray();
            Dictionary<int[], int> edgeMap = new Dictionary<int[], int>(comparer);
            //
            foreach (var partName in partNames)
            {
                part = mesh.Parts[partName];
                //
                foreach (var elementId in part.Labels)
                {
                    element = mesh.Elements[elementId];
                    nodeIds = element.NodeIds;
                    if (element is LinearTriangleElement || element is ParabolicTriangleElement)
                    {
                        sb.AppendFormat("{0} {1} {2} {3}{4}", nodeIdVertexId[nodeIds[0]],
                                                              nodeIdVertexId[nodeIds[1]],
                                                              nodeIdVertexId[nodeIds[2]],
                                                              trianglePartId, // part id
                                                              Environment.NewLine);
                        //
                        indices = new int[] { 0, 1, 2 };
                        for (int i = 0; i < indices.Length; i++)
                        {
                            key = GetSortedKey(nodeIds[indices[i]], nodeIds[indices[(i + 1) % 3]]);
                            if (edgeMap.ContainsKey(key)) edgeMap[key]++;
                            else edgeMap.Add(key, 1);
                        }
                    }
                    else if (element is LinearQuadrilateralElement || element is ParabolicQuadrilateralElement)
                    {
                        sb.AppendFormat("{0} {1} {2} {3}{4}", nodeIdVertexId[nodeIds[0]],
                                                              nodeIdVertexId[nodeIds[1]],
                                                              nodeIdVertexId[nodeIds[2]],
                                                              trianglePartId, // part id
                                                              Environment.NewLine);
                        //
                        indices = new int[] { 0, 1, 2 };
                        for (int i = 0; i < indices.Length; i++)
                        {
                            key = GetSortedKey(nodeIds[indices[i]], nodeIds[indices[(i + 1) % 3]]);
                            if (edgeMap.ContainsKey(key)) edgeMap[key]++;
                            else edgeMap.Add(key, 1);
                        }
                        //
                        sb.AppendFormat("{0} {1} {2} {3}{4}", nodeIdVertexId[nodeIds[0]],
                                                              nodeIdVertexId[nodeIds[2]],
                                                              nodeIdVertexId[nodeIds[3]],
                                                              trianglePartId, // part id
                                                              Environment.NewLine);
                        indices = new int[] { 0, 2, 3 };
                        for (int i = 0; i < indices.Length; i++)
                        {
                            key = GetSortedKey(nodeIds[indices[i]], nodeIds[indices[(i + 1) % 3]]);
                            if (edgeMap.ContainsKey(key)) edgeMap[key]++;
                            else edgeMap.Add(key, 1);
                        }
                    }
                }
            }
            // Edges
            sb.AppendLine("Edges");
            sb.AppendLine(edgeMap.Count().ToString());
            count = 1;
            int edgePartId = 1;
            List<int> requiredEdgeIds = new List<int>();
            foreach (var entry in edgeMap)
            {
                if (entry.Value == 1)
                {
                    key = entry.Key;
                    sb.AppendFormat("{0} {1} {2}{3}", nodeIdVertexId[key[0]],
                                                        nodeIdVertexId[key[1]],
                                                        edgePartId,
                                                        Environment.NewLine);
                    requiredEdgeIds.Add(count++);
                }
            }
            // Required edges
            sb.AppendLine("RequiredEdges");
            sb.AppendLine(requiredEdgeIds.Count().ToString());
            foreach (var edgeId in requiredEdgeIds)
            {
                sb.AppendFormat("{0}{1}", edgeId, Environment.NewLine);
            }
            // End
            sb.AppendLine("End");
            //
            File.WriteAllText(fileName, sb.ToString());
        }
        private static int[] GetSortedKey(int id1, int id2)
        {
            if (id1 < id2) return new int[] { id1, id2};
            else return new int[] { id2, id1 };
        }
    }
}
