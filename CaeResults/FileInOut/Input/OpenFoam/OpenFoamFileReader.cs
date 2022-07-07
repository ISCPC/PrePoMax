using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CaeMesh;
using CaeGlobals;

namespace CaeResults
{
    public static class OpenFoamFileReader
    {
        // Methods                                                                                                                  
        static public FeResults Read(string fileName)
        {
            if (fileName != null && File.Exists(fileName))
            {
                string baseFolder = Path.GetDirectoryName(fileName);
                string meshFolder = Path.Combine(baseFolder, "constant", "polyMesh");
                if (Directory.Exists(meshFolder))
                {
                    string pointsFile = Path.Combine(meshFolder, "points");
                    string boundaryFile = Path.Combine(meshFolder, "boundary");
                    string facesFile = Path.Combine(meshFolder, "faces");
                    string ownerFile = Path.Combine(meshFolder, "owner");
                    //
                    if (File.Exists(pointsFile) && File.Exists(boundaryFile) && File.Exists(facesFile)  && File.Exists(ownerFile))
                    {
                        FeResults result = new FeResults(fileName);
                        //
                        Dictionary<int, int> nodeIdsLookUp;
                        Dictionary<int, FeNode> nodes = GetNodes(pointsFile, out nodeIdsLookUp);
                        //
                        Dictionary<int, HashSet<int>> faceIdNodeIds;
                        HashSet<int> boundaryFaceIds = GetBoundaryFaceIds(boundaryFile);
                        Dictionary<int, FeElement> elements = GetBoundariesAsTriangles(facesFile, boundaryFaceIds,
                                                                                       out faceIdNodeIds);
                        Dictionary<int, HashSet<int>> cellIdNodeIds = GetCellIdNodeIds(ownerFile, boundaryFaceIds, faceIdNodeIds);
                        Dictionary<int, int> nodeIdCellCount = GetNodeIdCellCount(cellIdNodeIds);

                        FeMesh mesh = new FeMesh(nodes, elements, MeshRepresentation.Results);
                        mesh.ResetPartsColor();
                        result.SetMesh(mesh, nodeIdsLookUp);
                        //
                        return result;
                    }



                    
                }
            }
            //
            return null;
        }

        static private Dictionary<int, FeNode> GetNodes(string fileName, out Dictionary<int, int> nodeIdsLookUp)
        {
            string[] lines = Tools.GetLinesFromFile(fileName);
            //
            int lineId = 0;
            lines = SkipCommentsAndEmptyLines(lines);
            SkipFoamFile(lines, ref lineId);
            //
            string[] tmp;
            string[] splitter = new string[] { "(", ",", " ", ")" };
            int numOfNodes;
            FeNode node;
            Dictionary<int, FeNode> nodes = new Dictionary<int, FeNode>();
            nodeIdsLookUp = new Dictionary<int, int>();
            //
            numOfNodes = int.Parse(lines[lineId++]);
            // Skip bracket
            if (lines[lineId].StartsWith("(")) lineId++;
            for (int i = 0; i < numOfNodes; i++)
            {
                tmp = lines[lineId].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                if (tmp.Length == 3)
                {
                    node = new FeNode(i, double.Parse(tmp[0]), double.Parse(tmp[1]), double.Parse(tmp[2]));
                    nodes.Add(i, node);
                    nodeIdsLookUp.Add(i, i);
                }
                else throw new NotSupportedException();
                //
                lineId++;
            }
            //
            return nodes;
        }
        static private HashSet<int> GetBoundaryFaceIds(string fileName)
        {
            string[] lines = Tools.GetLinesFromFile(fileName);
            //
            int lineId = 0;
            lines = SkipCommentsAndEmptyLines(lines);
            SkipFoamFile(lines, ref lineId);
            //
            int numOfBoundaries;
            string line;
            // Read the number of boundaries
            numOfBoundaries = int.Parse(lines[lineId++]);
            // Get boundaries
            List<string> boundary = new List<string>();
            List<List<string>> boundaries = new List<List<string>>();
            //
            for (int i = lineId; i < lines.Length; i++)
            {
                line = lines[i].Trim();
                //
                if (line[0] == '(' || line[0] == ')' || line[0] == '{' || line[0] == '}')
                {
                    if (boundary.Count > 0)
                    {
                        boundaries.Add(boundary);
                        boundary = new List<string>();
                    }
                }
                else
                {
                    boundary.Add(line);
                }
            }
            // Get ids
            string[] tmp;
            string[] splitter = new string[] { " ", ";" };
            int numberOfFaces;
            int startFaceId;
            HashSet<int> boundaryFaceIds = new HashSet<int>();
            foreach (var boundaryLines in boundaries)
            {
                numberOfFaces = -1;
                startFaceId = -1;
                // Boundary name
                if (boundaryLines.Count <= 1)
                { }
                else
                {
                    foreach (var boundaryLine in boundaryLines)
                    {
                        tmp = boundaryLine.ToUpper().Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                        if (tmp[0] == "NFACES") numberOfFaces = int.Parse(tmp[1]);
                        else if (tmp[0] == "STARTFACE") startFaceId = int.Parse(tmp[1]);
                    }
                }
                //
                if (numberOfFaces > 0 && startFaceId > 0)
                {
                    for (int i = startFaceId; i < startFaceId + numberOfFaces; i++) boundaryFaceIds.Add(i);
                }
            }
            //
            return boundaryFaceIds;
        }
        static private Dictionary<int, FeElement> GetBoundariesAsTriangles(string fileName, HashSet<int> boundaryFaceIds,
                                                                           out Dictionary<int, HashSet<int>> faceIdNodeIds)
        {
            string[] lines = Tools.GetLinesFromFile(fileName);
            //
            int lineId = 0;
            lines = SkipCommentsAndEmptyLines(lines);
            SkipFoamFile(lines, ref lineId);
            //
            string[] tmp;
            string[] splitter = new string[] { "(", ",", " ", ")" };
            int numOfNodes;
            int[] nodeIds;
            int[] faceNodeIds;
            int numOfFaces;
            int triangleId = 0;
            FeElement triangle;
            Dictionary<int, FeElement> triangles = new Dictionary<int, FeElement>();
            // Read number of faces
            numOfFaces = int.Parse(lines[lineId++]);
            // Skip bracket
            if (lines[lineId].StartsWith("(")) lineId++;
            //
            HashSet<int> faceNodeIdsHash;
            faceIdNodeIds = new Dictionary<int, HashSet<int>>();
            for (int i = 0; i < numOfFaces; i++)
            {
                if (boundaryFaceIds.Contains(i))
                {
                    tmp = lines[lineId].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                    //
                    if (int.TryParse(tmp[0], out numOfNodes))
                    {
                        if (tmp.Length == numOfNodes + 1)
                        {
                            faceNodeIds = new int[tmp.Length - 1];
                            for (int j = 0; j < tmp.Length - 1; j++) faceNodeIds[j] = int.Parse(tmp[j + 1]);
                            // Read boudary as triangles
                            for (int j = 0; j < faceNodeIds.Length - 2; j++)
                            {
                                nodeIds = new int[] { faceNodeIds[0], faceNodeIds[j + 1], faceNodeIds[j + 2] };
                                triangle = new LinearTriangleElement(triangleId++, nodeIds);
                                triangles.Add(triangle.Id, triangle);
                            }
                            //
                            faceNodeIdsHash = new HashSet<int>(faceNodeIds);
                            faceIdNodeIds.Add(i, faceNodeIdsHash);
                        }
                        else throw new NotSupportedException();
                    }
                }
                //
                lineId++;
            }
            //
            return triangles;
        }
        static private Dictionary<int, HashSet<int>> GetCellIdNodeIds(string fileName, HashSet<int> boundaryFaceIds,
                                                                      Dictionary<int, HashSet<int>> faceIdNodeIds)
        {
            string[] lines = Tools.GetLinesFromFile(fileName);
            //
            int lineId = 0;
            lines = SkipCommentsAndEmptyLines(lines);
            SkipFoamFile(lines, ref lineId);
            //
            int cellId;
            int numOfCells;
            HashSet<int> cellNodeIds;
            Dictionary<int, HashSet<int>> cellIdNodeIds = new Dictionary<int, HashSet<int>>();
            // Read number of cells
            numOfCells = int.Parse(lines[lineId++]);
            // Skip bracket
            if (lines[lineId].StartsWith("(")) lineId++;
            //
            for (int i = 0; i < numOfCells; i++)
            {
                if (boundaryFaceIds.Contains(i))
                {
                    cellId = int.Parse(lines[lineId].Trim());
                    if (cellIdNodeIds.TryGetValue(cellId, out cellNodeIds)) cellNodeIds.UnionWith(faceIdNodeIds[i]);
                    else cellIdNodeIds.Add(cellId, new HashSet<int>(faceIdNodeIds[i]));
                }
                //
                lineId++;
            }
            return cellIdNodeIds;
        }
        static private Dictionary<int, int> GetNodeIdCellCount(Dictionary<int, HashSet<int>> cellIdNodeIds)
        { 
            Dictionary<int, int> nodeIdCellCount = new Dictionary<int, int>();
            foreach (var entry in cellIdNodeIds)
            {
                foreach (var nodeId in entry.Value)
                {
                    if (nodeIdCellCount.ContainsKey(nodeId)) nodeIdCellCount[nodeId]++;
                    else nodeIdCellCount.Add(nodeId, 1);
                }
            }
            return nodeIdCellCount;
        }
        //
        static private string[] SkipCommentsAndEmptyLines(string[] lines)
        {
            string line;
            List<string> dataLines = new List<string>();
            //
            for (int i = 0; i < lines.Length; i++)
            {
                line = lines[i].Trim();
                if (line.Length > 0 && line[0] != '/' && line[0] != '\\' && line[0] != '|')
                {
                    dataLines.Add(lines[i]);
                }
            }
            //
            return dataLines.ToArray();
        }
        static private void SkipFoamFile(string[] lines, ref int lineId)
        {
            if (lines[lineId].Trim().ToUpper().StartsWith("FOAMFILE"))
            {
                while (!lines[lineId].Trim().StartsWith("}")) lineId++;
            }
            lineId++;
        }
        //
        

    }
}
