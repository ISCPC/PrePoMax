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
                    //else if (element is ParabolicTriangleElement)
                    //{
                    //    elementNodeIdsSurfaceId.Add(new int[] { oldNodeIdNewId[element.NodeIds[0]],
                    //                                            oldNodeIdNewId[element.NodeIds[1]],
                    //                                            oldNodeIdNewId[element.NodeIds[2]],
                    //                                            i + 1 });
                    //}
                    else throw new NotSupportedException();
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
            // Required edges - keep edge cells connected to the vertices with only 2 edge cells: on the outside of the rectangle
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
        public static void WriteShellElementsFix(string fileName, int[] elementIds, BasePart part, FeMesh mesh, bool keepModelEdges)
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
            // Elements
            int elementId;
            FeElement element;
            List<int[]> triangleNodeIdsSurfaceId = new List<int[]>();
            List<int[]> quadNodeIdsSurfaceId = new List<int[]>();
            for (int i = 0; i < vis.CellIdsByFace.Length; i++)
            {
                for (int j = 0; j < vis.CellIdsByFace[i].Length; j++)
                {
                    elementId = vis.CellIds[vis.CellIdsByFace[i][j]];
                    element = mesh.Elements[elementId];
                    if (element is LinearTriangleElement || element is ParabolicTriangleElement)
                    {
                        triangleNodeIdsSurfaceId.Add(new int[] { oldNodeIdNewId[element.NodeIds[0]],
                                                                     oldNodeIdNewId[element.NodeIds[1]],
                                                                     oldNodeIdNewId[element.NodeIds[2]],
                                                                     i + 1 });
                    }
                    else if (element is LinearQuadrilateralElement || element is ParabolicQuadrilateralElement)
                    {
                        quadNodeIdsSurfaceId.Add(new int[] { oldNodeIdNewId[element.NodeIds[0]],
                                                                 oldNodeIdNewId[element.NodeIds[1]],
                                                                 oldNodeIdNewId[element.NodeIds[2]],
                                                                 oldNodeIdNewId[element.NodeIds[3]],
                                                                 i + 1 });
                    }
                    else throw new NotSupportedException();
                }
            }
            // Triangles
            WriteTriangles(sb, triangleNodeIdsSurfaceId);
            // Quadrilaterals
            WriteQuadrilaterals(sb, quadNodeIdsSurfaceId);
            // RequiredElements                                                                     
            int[] requiredTriangleIds = part.Labels.Except(elementIds).ToArray();
            WriteRequiredTriangles(sb, requiredTriangleIds);
            // Edges
            int id1, id2;
            List<int[]> edgeNodeIdsEdgeId = new List<int[]>();
            for (int i = 0; i < vis.EdgeCellIdsByEdge.Length; i++)
            {
                for (int j = 0; j < vis.EdgeCellIdsByEdge[i].Length; j++)
                {
                    id1 = vis.EdgeCells[vis.EdgeCellIdsByEdge[i][j]][0];
                    id2 = vis.EdgeCells[vis.EdgeCellIdsByEdge[i][j]][1];
                    edgeNodeIdsEdgeId.Add(new int[] { oldNodeIdNewId[id1], oldNodeIdNewId[id2], i + 1 });
                }
            }
            //
            if (keepModelEdges) WriteEdges(sb, edgeNodeIdsEdgeId);
            // Ridges - all edges are ridges
            int[] ridgeIds = new int[vis.EdgeCells.Length];
            for (int i = 0; i < ridgeIds.Length; i++) ridgeIds[i] = i + 1;
            WriteRidges(sb, ridgeIds);
            // Required edges - keep edge cells connected to the vertices with only 2 edge cells: on the outside of the rectangle
            //HashSet<int> requiredEdgeIds = new HashSet<int>();
            //foreach (var cornerEntry in vertexEdgeIds)
            //{
            //    if (cornerEntry.Value.Count == 2) requiredEdgeIds.UnionWith(cornerEntry.Value);
            //}
            //if (keepVetexEdges) WriteRequiredEdges(sb, requiredEdgeIds.ToArray());
            // End
            WriteEnd(sb);
            //
            File.WriteAllText(fileName, sb.ToString());
        }
        public static void WriteShellElements(string fileName, int[] elementIds, BasePart part, FeMesh mesh, bool keepModelEdges)
        {
            VisualizationData vis = part.Visualization;
            // File                                                                                 
            StringBuilder sb = new StringBuilder();
            WriteHeading(sb);
            // Vertices                                                                             
            int nodeId;
            FeNode node;
            FeElement element;
            Dictionary<int, int> oldNodeIdNewId;
            HashSet<int> allNodeIds = new HashSet<int>();
            List<double[]> nodeCoorNodeId = new List<double[]>();
            // Use only vertices of the selected element set
            for (int i = 0; i < elementIds.Length; i++)
            {
                element = mesh.Elements[elementIds[i]];
                for (int j = 0; j < element.NodeIds.Length; j++)
                {
                    nodeId = element.NodeIds[j];
                    if (!allNodeIds.Contains(nodeId))
                    {
                        node = mesh.Nodes[nodeId];
                        nodeCoorNodeId.Add(new double[] { node.X, node.Y, node.Z, node.Id });
                        allNodeIds.Add(nodeId);
                    }
                }
            }
            WriteVertices(sb, nodeCoorNodeId, out oldNodeIdNewId);
            // Corners                                                                              
            List<int> cornerIds = new List<int>();
            for (int i = 0; i < vis.VertexNodeIds.Length; i++)
            {
                nodeId = vis.VertexNodeIds[i];
                if (allNodeIds.Contains(nodeId)) cornerIds.Add(oldNodeIdNewId[nodeId]);
            }
            WriteCorners(sb, cornerIds);
            // Elements                                                                             
            int elementId;
            HashSet<int> allElementIds = new HashSet<int>(elementIds); // for speedup
            List<int[]> triangleNodeIdsSurfaceId = new List<int[]>();
            List<int[]> quadNodeIdsSurfaceId = new List<int[]>();
            for (int i = 0; i < vis.CellIdsByFace.Length; i++)
            {
                for (int j = 0; j < vis.CellIdsByFace[i].Length; j++)
                {
                    elementId = vis.CellIds[vis.CellIdsByFace[i][j]];
                    if (allElementIds.Contains(elementId))
                    {
                        element = mesh.Elements[elementId];
                        if (element is LinearTriangleElement || element is ParabolicTriangleElement)
                        {
                            triangleNodeIdsSurfaceId.Add(new int[] { oldNodeIdNewId[element.NodeIds[0]],
                                                                     oldNodeIdNewId[element.NodeIds[1]],
                                                                     oldNodeIdNewId[element.NodeIds[2]],
                                                                     i + 1 });
                        }
                        else if (element is LinearQuadrilateralElement || element is ParabolicQuadrilateralElement)
                        {
                            triangleNodeIdsSurfaceId.Add(new int[] { oldNodeIdNewId[element.NodeIds[0]],
                                                                     oldNodeIdNewId[element.NodeIds[1]],
                                                                     oldNodeIdNewId[element.NodeIds[2]],
                                                                     i + 1 });
                            triangleNodeIdsSurfaceId.Add(new int[] { oldNodeIdNewId[element.NodeIds[0]],
                                                                     oldNodeIdNewId[element.NodeIds[2]],
                                                                     oldNodeIdNewId[element.NodeIds[3]],
                                                                     i + 1 });
                            //quadNodeIdsSurfaceId.Add(new int[] { oldNodeIdNewId[element.NodeIds[0]],
                            //                                     oldNodeIdNewId[element.NodeIds[1]],
                            //                                     oldNodeIdNewId[element.NodeIds[2]],
                            //                                     oldNodeIdNewId[element.NodeIds[3]],
                            //                                     i + 1 });
                        }
                        else throw new NotSupportedException();
                    }
                }
            }
            // Triangles
            WriteTriangles(sb, triangleNodeIdsSurfaceId);
            // Quadrilaterals
            WriteQuadrilaterals(sb, quadNodeIdsSurfaceId);
            // Edges                                                                                
            int[] key;
            int id1, id2;
            CompareIntArray comparer = new CompareIntArray();
            Dictionary<int[], int> edgeKeys = new Dictionary<int[], int>(comparer);
            List<int[]> edgeNodeIdsEdgeId = new List<int[]>();
            // Free edges
            HashSet<int[]> freeEdges = GetFreeEdges(part);
            // Model edges - first since they are numbered by the EdgeCellIdsByEdge
            if (keepModelEdges)
            {
                for (int i = 0; i < vis.EdgeCellIdsByEdge.Length; i++)
                {
                    for (int j = 0; j < vis.EdgeCellIdsByEdge[i].Length; j++)
                    {
                        id1 = vis.EdgeCells[vis.EdgeCellIdsByEdge[i][j]][0];
                        id2 = vis.EdgeCells[vis.EdgeCellIdsByEdge[i][j]][1];
                        key = GetSortedKey(id1, id2);
                        if (!edgeKeys.ContainsKey(key) && (allNodeIds.Contains(id1) && allNodeIds.Contains(id2)))
                        {                            
                            edgeNodeIdsEdgeId.Add(new int[] { oldNodeIdNewId[id1], oldNodeIdNewId[id2], i + 1 });
                            edgeKeys.Add(key, edgeNodeIdsEdgeId.Count);
                        }
                    }
                }
            }
            // Boundary edges
            int edgeId;
            int[] edgeCell;
            List<int> requiredEdgeIds = new List<int>();
            GeometryPart subPart = mesh.CreateGeometryPartFromElementIds(elementIds);
            for (int i = 0; i < subPart.FreeEdgeCellIds.Length; i++)
            {
                edgeCell = subPart.Visualization.EdgeCells[subPart.FreeEdgeCellIds[i]];
                id1 = edgeCell[0];
                id2 = edgeCell[1];
                key = GetSortedKey(id1, id2);
                // Add boundary edges to all edges
                if (!edgeKeys.TryGetValue(key, out edgeId))
                {
                    edgeNodeIdsEdgeId.Add(new int[] { oldNodeIdNewId[id1], oldNodeIdNewId[id2], 0 });
                    edgeId = edgeNodeIdsEdgeId.Count;
                    edgeKeys.Add(key, edgeId);
                }
                // Free edges must not be kept
                if (!freeEdges.Contains(key))  requiredEdgeIds.Add(edgeId);
            }
            WriteEdges(sb, edgeNodeIdsEdgeId);
            // Ridges - all edges are ridges                                                        
            //int[] ridgeIds = new int[edgeNodeIdsEdgeId.Count];
            //for (int i = 0; i < ridgeIds.Length; i++) ridgeIds[i] = i + 1;
            //WriteRidges(sb, requiredEdgeIds.ToArray());
            // Required edges
            WriteRequiredEdges(sb, requiredEdgeIds.ToArray());
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
            //
            if (nodeCoorNodeId == null || nodeCoorNodeId.Count == 0) return;
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
            if (cornerIds == null || cornerIds.Count == 0) return;
            //
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
            if (elementNodeIdsElementId == null || elementNodeIdsElementId.Count == 0) return;
            //
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
        private static void WriteQuadrilaterals(StringBuilder sb, List<int[]> elementNodeIdsElementId)
        {
            if (elementNodeIdsElementId == null || elementNodeIdsElementId.Count == 0) return;
            //
            sb.AppendLine();
            sb.AppendLine("Quadrilaterals");
            sb.AppendLine(elementNodeIdsElementId.Count.ToString());
            //
            foreach (var elementData in elementNodeIdsElementId)
            {
                sb.AppendFormat("{0} {1} {2} {3} {4}{5}", elementData[0],
                                                          elementData[1],
                                                          elementData[2],
                                                          elementData[3],
                                                          elementData[4],   //  id
                                                          Environment.NewLine);
            }
        }
        private static void WriteRequiredTriangles(StringBuilder sb, int[] requiredTrianglesIds)
        {
            if (requiredTrianglesIds == null || requiredTrianglesIds.Length == 0) return;
            //
            sb.AppendLine();
            sb.AppendLine("RequiredTriangles");
            sb.AppendLine(requiredTrianglesIds.Length.ToString());
            for (int i = 0; i < requiredTrianglesIds.Length; i++)
            {
                sb.AppendFormat("{0}{1}", requiredTrianglesIds[i], Environment.NewLine);
            }
        }
        private static void WriteEdges(StringBuilder sb, List<int[]> edgeCellEdgeId)
        {
            if (edgeCellEdgeId == null || edgeCellEdgeId.Count == 0) return;
            //
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
            if (ridgeIds == null || ridgeIds.Length == 0) return;
            //
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
            if (requiredEdgeIds == null || requiredEdgeIds.Length == 0) return;
            //
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
        private static HashSet<int[]> GetFreeEdges(BasePart part)
        {
            int[][] cells = part.Visualization.Cells;
            CompareIntArray comparer = new CompareIntArray();
            Dictionary<int[], byte[]> allEdges = new Dictionary<int[], byte[]>(comparer);
            //
            int[] key;
            byte[] count;
            int[][] cellEdges;
            // Get all edges
            for (int i = 0; i < cells.Length; i++)
            {
                cellEdges = FeMesh.GetVisualizationEdgeCells(cells[i]);
                //
                foreach (var cellEdge in cellEdges)
                {
                    key = cellEdge.ToArray();
                    Array.Sort(key);
                    //
                    if (allEdges.TryGetValue(key, out count)) count[0]++;
                    else allEdges.Add(key, new byte[] { 1 });
                }
            }
            //
            HashSet<int[]> freeEdges = new HashSet<int[]>(comparer);
            foreach (var entry in allEdges)
            {
                if (entry.Value[0] == 1) freeEdges.Add(entry.Key);
            }
            //
            return freeEdges;
        }
        private static int[] GetSortedKey(int id1, int id2)
        {
            if (id1 < id2) return new int[] { id1, id2};
            else return new int[] { id2, id1 };
        }
    }
}
