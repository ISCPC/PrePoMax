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
        public static void Write(string fileName, BasePart part, FeMesh mesh, bool keepModelEdges, bool keepVetexEdges)
        {
            VisualizationData vis = part.Visualization;
            // File
            StringBuilder sb = new StringBuilder();
            WriteHeading(sb);
            // Vertices
            Dictionary<int, int> oldNodeIdNewId;
            FeNode node;
            List<double[]> nodeCoorNodeId = new List<double[]>();
            foreach (var nodeId in part.NodeLabels)
            {
                node = mesh.Nodes[nodeId];
                nodeCoorNodeId.Add(new double[] { node.X, node.Y, node.Z, node.Id });
            }
            WriteVertices(sb, nodeCoorNodeId, out oldNodeIdNewId);
            // Corners
            List<int> cornerIds = new List<int>();
            for (int i = 0; i < vis.VertexNodeIds.Length; i++) cornerIds.Add(oldNodeIdNewId[vis.VertexNodeIds[i]]);
            WriteCorners(sb, cornerIds);
            // Triangles
            int elementId;
            FeElement element;
            List<int[]> elementNodeIdsSurfaceId = new List<int[]>();
            for (int i = 0; i < vis.CellIdsByFace.Length; i++)
            {
                for (int j = 0; j < vis.CellIdsByFace[i].Length; j++)
                {
                    elementId = vis.CellIds[vis.CellIdsByFace[i][j]];
                    element = mesh.Elements[elementId];
                    if (element is LinearTriangleElement)
                    {
                        elementNodeIdsSurfaceId.Add(new int[] { oldNodeIdNewId[element.NodeIds[0]],
                                                                oldNodeIdNewId[element.NodeIds[1]],
                                                                oldNodeIdNewId[element.NodeIds[2]],
                                                                i + 1 });
                    }
                }
            }
            WriteTriangles(sb, elementNodeIdsSurfaceId);
            // Edges
            int id1, id2;
            int edgeId = 1;
            List<int> edgeIds;
            // Collect all edge cells connected to a vertex
            Dictionary<int, List<int>> vertexEdgeIds = new Dictionary<int, List<int>>();
            for (int i = 0; i < vis.VertexNodeIds.Length; i++)
                vertexEdgeIds.Add(oldNodeIdNewId[vis.VertexNodeIds[i]], new List<int>());
            //
            List<int[]> edgeNodeIdsEdgeId = new List<int[]>();
            for (int i = 0; i < vis.EdgeCellIdsByEdge.Length; i++)
            {
                for (int j = 0; j < vis.EdgeCellIdsByEdge[i].Length; j++)
                {
                    id1 = vis.EdgeCells[vis.EdgeCellIdsByEdge[i][j]][0];
                    id2 = vis.EdgeCells[vis.EdgeCellIdsByEdge[i][j]][1];
                    edgeNodeIdsEdgeId.Add(new int[] { oldNodeIdNewId[id1], oldNodeIdNewId[id2], i + 1 });
                    //
                    if (vertexEdgeIds.TryGetValue(id1, out edgeIds)) edgeIds.Add(edgeId);
                    if (vertexEdgeIds.TryGetValue(id2, out edgeIds)) edgeIds.Add(edgeId);
                    //
                    edgeId++;
                }
            }
            //
            if (keepModelEdges) WriteEdges(sb, edgeNodeIdsEdgeId);
            // Ridges - all edges are ridges
            int[] ridgeIds = new int[vis.EdgeCells.Length];
            for (int i = 0; i < ridgeIds.Length; i++) ridgeIds[i] = i + 1;
            WriteRidges(sb, ridgeIds);
            // Required edges - keep edge cells connected to the vertices with only 2 edge cells
            HashSet<int> requiredEdgeIds = new HashSet<int>();
            foreach (var cornerEntry in vertexEdgeIds)
            {
                if (cornerEntry.Value.Count == 2) requiredEdgeIds.UnionWith(cornerEntry.Value);
            }
            if (keepVetexEdges) WriteRequiredEdges(sb, requiredEdgeIds.ToArray());
            // End
            WriteEnd(sb);
            //
            File.WriteAllText(fileName, sb.ToString());
        }
        public static void WriteSplit(string fileName, GeometryPart part, FeMesh mesh, bool keepModelEdges)
        {
            int nodeId = mesh.MaxNodeId + 1;
            int elementId = 1;
            Dictionary<int, FeNode> nodes = new Dictionary<int, FeNode>();
            Dictionary<int, FeElement> elements = new Dictionary<int, FeElement>();
            //
            CompareIntArray comparer = new CompareIntArray();
            Dictionary<int[], int> edgeByKeyMidNodeId = new Dictionary<int[], int>(comparer);
            //
            int[] cell;
            int[] key;
            VisualizationData vis = part.Visualization;
            Vec3D v1, v2, v3, v4, v5, v6;
            int id1, id2, id3, id4, id5, id6, existingId;
            //
            for (int i = 0; i < vis.CellIdsByFace.Length; i++)
            {
                for (int j = 0; j < vis.CellIdsByFace[i].Length; j++)
                {
                    cell = vis.Cells[vis.CellIdsByFace[i][j]];
                    id1 = cell[0];
                    id2 = cell[1];
                    id3 = cell[2];
                    //
                    key = GetSortedKey(id1, id2);
                    if (!edgeByKeyMidNodeId.TryGetValue(key, out existingId))
                    {
                        existingId = nodeId++;
                        edgeByKeyMidNodeId.Add(key, existingId);
                    }
                    id4 = existingId;
                    key = GetSortedKey(id2, id3);
                    if (!edgeByKeyMidNodeId.TryGetValue(key, out existingId))
                    {
                        existingId = nodeId++;
                        edgeByKeyMidNodeId.Add(key, existingId);
                    }
                    id5 = existingId;
                    key = GetSortedKey(id3, id1);
                    if (!edgeByKeyMidNodeId.TryGetValue(key, out existingId))
                    {
                        existingId = nodeId++;
                        edgeByKeyMidNodeId.Add(key, existingId);
                    }
                    id6 = existingId;
                    //
                    v1 = new Vec3D(mesh.Nodes[id1].Coor);
                    v2 = new Vec3D(mesh.Nodes[id2].Coor);
                    v3 = new Vec3D(mesh.Nodes[id3].Coor);
                    v4 = (v1 + v2) * 0.5;
                    v5 = (v2 + v3) * 0.5;
                    v6 = (v3 + v1) * 0.5;
                    //
                    if (!nodes.ContainsKey(id1)) nodes.Add(id1, new FeNode(id1, v1.Coor));
                    if (!nodes.ContainsKey(id2)) nodes.Add(id2, new FeNode(id2, v2.Coor));
                    if (!nodes.ContainsKey(id3)) nodes.Add(id3, new FeNode(id3, v3.Coor));
                    if (!nodes.ContainsKey(id4)) nodes.Add(id4, new FeNode(id4, v4.Coor));
                    if (!nodes.ContainsKey(id5)) nodes.Add(id5, new FeNode(id5, v5.Coor));
                    if (!nodes.ContainsKey(id6)) nodes.Add(id6, new FeNode(id6, v6.Coor));
                    //
                    elements.Add(elementId, new LinearTriangleElement(elementId++, 1, new int[] { id4, id6, id1 }));
                    elements.Add(elementId, new LinearTriangleElement(elementId++, 1, new int[] { id4, id5, id6 }));
                    elements.Add(elementId, new LinearTriangleElement(elementId++, 1, new int[] { id4, id2, id5 }));
                    elements.Add(elementId, new LinearTriangleElement(elementId++, 1, new int[] { id5, id3, id6 }));
                }
            }
            //
            FeMesh meshSplit = new FeMesh(nodes, elements, MeshRepresentation.Geometry);
            StlFileWriter.Write(@"C:\Temp\out.stl", meshSplit, meshSplit.Parts.Keys.ToArray());
            int numOfVertices = nodes.Count;
            int numOfTriangles = elements.Count;
            //
            double[] coor;
            FeNode node;
            int count = 1;
            int vertexPartId = 1;
            Dictionary<int, int> nodeIdVertexId = new Dictionary<int, int>();
            // Vertices
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("MeshVersionFormatted 2");
            sb.AppendLine("Dimension 3");
            sb.AppendLine("Vertices");
            sb.AppendLine(numOfVertices.ToString());
            //
            foreach (var entry in nodes)
            {
                node = entry.Value;
                coor = node.Coor;
                sb.AppendFormat("{0} {1} {2} {3}{4}", coor[0], coor[1], coor[2], vertexPartId, Environment.NewLine);
                nodeIdVertexId.Add(node.Id, count);
                count++;
            }
            // Triangles
            sb.AppendLine("Triangles");
            sb.AppendLine(numOfTriangles.ToString());
            //
            foreach (var entry in elements)
            {
                cell = entry.Value.NodeIds;
                sb.AppendFormat("{0} {1} {2} {3}{4}", nodeIdVertexId[cell[0]],
                                                      nodeIdVertexId[cell[1]],
                                                      nodeIdVertexId[cell[2]],
                                                      entry.Value.PartId, // part id
                                                      Environment.NewLine);
            }

            for (int i = 0; i < vis.CellIdsByFace.Length; i++)
            {
                for (int j = 0; j < vis.CellIdsByFace[i].Length; j++)
                {
                    cell = vis.Cells[vis.CellIdsByFace[i][j]];
                    sb.AppendFormat("{0} {1} {2} {3}{4}", nodeIdVertexId[cell[0]],
                                                          nodeIdVertexId[cell[1]],
                                                          nodeIdVertexId[cell[2]],
                                                          i, // part id
                                                          Environment.NewLine);
                }
            }
            //// Edges
            //sb.AppendLine("Ridges");
            //sb.AppendLine(vis.EdgeCells.Length.ToString());
            //int edgePartId = 1;
            //for (int i = 0; i < vis.EdgeCells.Length; i++)
            //{
            //    sb.AppendFormat("{0} {1} {2}{3}", nodeIdVertexId[vis.EdgeCells[i][0]],
            //                                      nodeIdVertexId[vis.EdgeCells[i][1]],
            //                                      edgePartId,
            //                                      Environment.NewLine);
            //}
            // End
            sb.AppendLine("End");
            //
            File.WriteAllText(fileName, sb.ToString());
        }
        //
        public static void WriteShellElements(string fileName, int[] elementIds, BasePart part, FeMesh mesh, bool keepModelEdges)
        {
            VisualizationData vis = part.Visualization;
            // Collect node ids
            FeElement element;
            HashSet<int> nodeIds = new HashSet<int>();
            foreach (var elementId in elementIds)
            {
                element = mesh.Elements[elementId];
                nodeIds.UnionWith(element.NodeIds);
            }
            // File
            StringBuilder sb = new StringBuilder();
            WriteHeading(sb);
            // Vertices
            Dictionary<int, int> oldNodeIdNewId;
            FeNode node;
            List<double[]> nodeCoorNodeId = new List<double[]>();
            foreach (var nodeId in nodeIds)
            {
                node = mesh.Nodes[nodeId];
                nodeCoorNodeId.Add(new double[] { node.X, node.Y, node.Z, node.Id });
            }
            WriteVertices(sb, nodeCoorNodeId, out oldNodeIdNewId);
            // Triangles
            //FeElement element;
            //foreach (var elementId in elementIds)
            //{

            //}
            //{

            //}
            //List<int[]> elementNodeIdsSurfaceId = new List<int[]>();
            //for (int i = 0; i < vis.CellIdsByFace.Length; i++)
            //{
            //    for (int j = 0; j < vis.CellIdsByFace[i].Length; j++)
            //    {
            //        elementId = vis.CellIds[vis.CellIdsByFace[i][j]];
            //        element = mesh.Elements[elementId];
            //        if (element is LinearTriangleElement)
            //        {
            //            elementNodeIdsSurfaceId.Add(new int[] { oldNodeIdNewId[element.NodeIds[0]],
            //                                                    oldNodeIdNewId[element.NodeIds[1]],
            //                                                    oldNodeIdNewId[element.NodeIds[2]],
            //                                                    i + 1 });
            //        }
            //    }
            //}
            //WriteLinearTriangles(sb, elementIds, vis, oldNodeIdNewId);
            // Edges



            
            // End
            WriteEnd(sb);
            //
            File.WriteAllText(fileName, sb.ToString());
        }
        //                                                                                                                          
        private static void WriteHeading(StringBuilder sb)
        {            
            sb.AppendLine("MeshVersionFormatted 2");
            sb.AppendLine("Dimension 3");
        }
        private static void WriteVertices(StringBuilder sb, List<double[]> nodeCoorNodeId, out Dictionary<int, int> oldNodeIdNewId)
        {
            int count = 1;
            oldNodeIdNewId = new Dictionary<int, int>();
            // Vertices
            sb.AppendLine();
            sb.AppendLine("Vertices");
            sb.AppendLine(nodeCoorNodeId.Count.ToString());
            //
            foreach (var nodeData in nodeCoorNodeId)
            {
                sb.AppendFormat("{0} {1} {2} {3}{4}", nodeData[0], nodeData[1], nodeData[2],
                                                      (int)(nodeData[3]), Environment.NewLine);
                oldNodeIdNewId.Add((int)nodeData[3], count);
                count++;
            }
        }
        private static void WriteCorners(StringBuilder sb, List<int> cornerIds)
        {
            sb.AppendLine();
            sb.AppendLine("Corners");
            sb.AppendLine(cornerIds.Count.ToString());
            //
            foreach (var cornerId in cornerIds)
            {
                sb.AppendFormat("{0}{1}", cornerId, Environment.NewLine);
            }
        }
        private static void WriteTriangles(StringBuilder sb, List<int[]> elementNodeIdsElementId)
        {
            sb.AppendLine();
            sb.AppendLine("Triangles");
            sb.AppendLine(elementNodeIdsElementId.Count.ToString());
            //
            foreach (var elementData in elementNodeIdsElementId)
            {
                sb.AppendFormat("{0} {1} {2} {3}{4}", elementData[0],
                                                      elementData[1],
                                                      elementData[2],
                                                      elementData[3],
                                                      Environment.NewLine);
            }
        }
        private static void WriteEdges(StringBuilder sb, List<int[]> edgeCellEdgeId)
        {
            sb.AppendLine();
            sb.AppendLine("Edges");
            sb.AppendLine(edgeCellEdgeId.Count.ToString());
            foreach (var edgeData in edgeCellEdgeId)
            {
                sb.AppendFormat("{0} {1} {2}{3}", edgeData[0],
                                                  edgeData[1],
                                                  edgeData[2],
                                                  Environment.NewLine);
            }
        }
        private static void WriteRidges(StringBuilder sb, int[] ridgeIds)
        {
            sb.AppendLine();
            sb.AppendLine("Ridges");
            sb.AppendLine(ridgeIds.Length.ToString());
            for (int i = 0; i < ridgeIds.Length; i++)
            {
                sb.AppendFormat("{0}{1}", ridgeIds[i], Environment.NewLine);
            }
        }
        private static void WriteRequiredEdges(StringBuilder sb, int[] requiredEdgeIds)
        {
            sb.AppendLine();
            sb.AppendLine("RequiredEdges");
            sb.AppendLine(requiredEdgeIds.Length.ToString());
            for (int i = 0; i < requiredEdgeIds.Length; i++)
            {
                sb.AppendFormat("{0}{1}", requiredEdgeIds[i], Environment.NewLine);
            }
        }
        private static void WriteEnd(StringBuilder sb)
        {
            sb.AppendLine();
            sb.AppendLine("End");
        }
        //
        private static int[] GetSortedKey(int id1, int id2)
        {
            if (id1 < id2) return new int[] { id1, id2};
            else return new int[] { id2, id1 };
        }
    }
}
