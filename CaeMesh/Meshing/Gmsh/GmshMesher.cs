using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CaeMesh.Meshing;
using GmshCommon;
using CaeMesh;
using System.IO;

namespace CaeMesh
{
    public class GmshMesher
    {
        // Variables                                                                                                                
        private string _geometryFileName;
        private bool _isOCC;
        private string _inpFileName;
        private MeshingParameters _partMeshingParameters;
        private Dictionary<int, double> _vertexIdMeshSize;
        private MeshSetupItem[] _gmshSetupItems;
        private Action<string> _writeOutput;
        private bool _preview;
        private string _error;
        private Thread _thread;
        private int _currentLogLine;


        public GmshMesher(GmshData gmshData, Action<string> writeOutput)
            : this(gmshData.GeometryFileName, gmshData.InpFileName, gmshData.PartMeshingParameters,
                   gmshData.VertexIdMeshSize, gmshData.GmshSetupItems, writeOutput, gmshData.Preview)
        {
            
        }
        public GmshMesher(string geometryFileName, string inpFileName, MeshingParameters partMeshingParameters,
                          Dictionary<int, double> vertexIdMeshSize, MeshSetupItem[] gmshSetupItems,
                          Action<string> writeOutput, bool preview)
        {
            _geometryFileName = geometryFileName;
            _inpFileName = inpFileName;
            _partMeshingParameters = partMeshingParameters;
            _vertexIdMeshSize = vertexIdMeshSize;
            _gmshSetupItems = gmshSetupItems;
            _writeOutput = writeOutput;
            _preview = preview;
            //
            if (geometryFileName.EndsWith("brep")) _isOCC = true;
            else if (geometryFileName.EndsWith("stl")) _isOCC = false;
            else throw new NotSupportedException();
        }
        public string CreateMesh()
        {
            try
            {
                Gmsh.InitializeGmsh();
                Gmsh.Add("Model-1");
                Gmsh.Logger.Start();
                _currentLogLine = 0;
                //
                _thread = new Thread(new ThreadStart(() => CreateMesh(_geometryFileName, _inpFileName, _partMeshingParameters,
                                                                      _vertexIdMeshSize, _gmshSetupItems, _writeOutput, _preview)));
                _thread.Start();
                //
                int count = 0;
                while (_thread.ThreadState == ThreadState.Running)
                {
                    if (count++ % 10 == 0) WriteLog();
                    Thread.Sleep(100);
                }
                // Wait for the worker thread to finish
                _thread.Join();
                //
                WriteLog();
                //
                return _error;
            }
            catch (Exception ex)
            {
                _error = ex.Message;
                _writeOutput?.Invoke(_error);
                _thread?.Abort();
                //
                return _error;
            }
            finally
            {
                Gmsh.Logger.Stop();
                Gmsh.FinalizeGmsh();
                // Check if the thread was not aborted on error
                if (_error != null) File.Delete(_inpFileName);
            }
        }
        //
        private void CreateMesh(string geometryFileName, string inpFileName, MeshingParameters partMeshingParameters,
                                Dictionary<int, double> vertexIdMeshSize, MeshSetupItem[] gmshSetupItems,
                                Action<string> writeOutput, bool preview)
        {
            MeshSetupItem meshSetupItem = gmshSetupItems[0];
            //
            if (meshSetupItem is GmshSetupItem gsi)
            {
                Tuple<int, int>[] outDimTags;
                if (_isOCC) Gmsh.OCC.ImportShapes(geometryFileName, out outDimTags, false, "");
                else
                {
                    double angleDeg = 30;
                    Gmsh.Merge(geometryFileName);
                    Gmsh.Mesh.RemoveDuplicateNodes();
                    Gmsh.Mesh.ClassifySurfaces(angleDeg * Math.PI / 180, true, false, angleDeg * Math.PI / 180, true);
                    Gmsh.Mesh.CreateGeometry();
                    Gmsh.GetEntities(out outDimTags, 2);
                    int[] surfaceIds = new int[outDimTags.Length];
                    for (int i = 0; i < outDimTags.Length; i++) surfaceIds[i] = outDimTags[i].Item2;
                    int volumeId = Gmsh.Geo.AddSurfaceLoop(surfaceIds);
                    Gmsh.Geo.AddVolume(new int[] { volumeId });
                }
                //
                Synchronize(); // must be here
                // Mesh size
                //Tuple<int, int>[] surfaceDimTags;
                //Gmsh.GetEntities(out surfaceDimTags, 2);
                //foreach (var surfaceDimTag in surfaceDimTags) Gmsh.Mesh.SetSizeFromBoundary(2, surfaceDimTag.Item2, 0);
                //
                double scaleFactor = 1;
                //
                Gmsh.SetNumber("Mesh.MeshSizeMin", partMeshingParameters.MinH * scaleFactor);
                Gmsh.SetNumber("Mesh.MeshSizeMax", partMeshingParameters.MaxH * scaleFactor);
                Gmsh.SetNumber("Mesh.MeshSizeFromCurvature", 2 * Math.PI * partMeshingParameters.ElementsPerCurve);
                // Local mesh size
                outDimTags = new Tuple<int, int>[1];
                foreach (var entry in vertexIdMeshSize)
                {
                    outDimTags[0] = new Tuple<int, int>(0, entry.Key);
                    Gmsh.OCC.SetSize(outDimTags, entry.Value);
                }
                Synchronize(); // must be here for mesh refinement
                // 2D meshing algorithm
                Gmsh.SetNumber("Mesh.Algorithm", (int)gsi.AlgorithmMesh2D);
                // 3D meshing algorithm
                Gmsh.SetNumber("Mesh.Algorithm3D", (int)gsi.AlgorithmMesh3D);
                // Recombine
                bool recombine = gsi.AlgorithmRecombine != GmshAlgorithmRecombineEnum.None;
                if (recombine)
                {
                    Gmsh.SetNumber("Mesh.RecombinationAlgorithm", (int)gsi.AlgorithmRecombine);
                    Gmsh.SetNumber("Mesh.RecombineMinimumQuality", gsi.RecombineMinQuality);
                }
                // Transfinite
                bool transfiniteVolume = gsi is TransfiniteMesh && gsi.TransfiniteThreeSided && gsi.TransfiniteFourSided;
                if (_isOCC && (gsi.TransfiniteThreeSided || gsi.TransfiniteFourSided))
                    //Gmsh.Mesh.SetTransfiniteAutomatic(gsi.TransfiniteAngleRad, recombine);
                    SetTransfiniteSurfaces(gsi.TransfiniteThreeSided, gsi.TransfiniteFourSided, transfiniteVolume, recombine);
                //
                if (gsi is ShellGmsh)
                    ShellGmsh(gsi, partMeshingParameters, preview);
                else if (gsi is TetrahedralGmsh)
                    TetrahedralGmsh(gsi, partMeshingParameters, preview);
                else if (gsi is TransfiniteMesh)
                    TransfiniteMesh(gsi, partMeshingParameters, preview);
                else if (gsi is ExtrudeMesh || gsi is RevolveMesh)
                    ExtrudeRevolveMesh(gsi, partMeshingParameters, preview);
                else throw new NotSupportedException("MeshSetupItemTypeException");
            }
            else throw new NotSupportedException("MeshSetupItemTypeException");
            // Element order
            if (!preview && partMeshingParameters.SecondOrder)
            {
                if (!partMeshingParameters.MidsideNodesOnGeometry) Gmsh.SetNumber("Mesh.SecondOrderLinear", 1); // first
                Gmsh.SetNumber("Mesh.HighOrderOptimize", 1);                                                    // second
                Gmsh.SetNumber("Mesh.SecondOrderIncomplete", 1);                                                // second
                Gmsh.Mesh.SetOrder(2);                                                                          // third
            }
            // Output
            Gmsh.Write(inpFileName);
            //
            writeOutput?.Invoke("Meshing done.");
            writeOutput?.Invoke("");
            double elapsedTime = Gmsh.GetNumber("Mesh.CpuTime");
            writeOutput?.Invoke("Elapsed time [s]: " + Math.Round(elapsedTime, 5));
            writeOutput?.Invoke("");
            //
            _error = null;
        }
        //
        private static void ShellGmsh(GmshSetupItem gmshSetupItem, MeshingParameters meshingParameters, bool preview)
        {
            bool recombine = gmshSetupItem.AlgorithmRecombine != GmshAlgorithmRecombineEnum.None;
            // Recombine all
            if (recombine) Gmsh.SetNumber("Mesh.RecombineAll", 1);
            //
            if (preview)
            {
                Gmsh.Generate(2);
            }
            else
            {
                Gmsh.Generate(2);
            }
        }
        private static void TetrahedralGmsh(GmshSetupItem gmshSetupItem, MeshingParameters meshingParameters, bool preview)
        {
            if (preview) Gmsh.Generate(1);
            else Gmsh.Generate(3);
        }
        private static void TransfiniteMesh(GmshSetupItem gmshSetupItem, MeshingParameters meshingParameters, bool preview)
        {
            if (preview) Gmsh.Generate(1);
            else Gmsh.Generate(3);
        }
        private static void ExtrudeRevolveMesh(GmshSetupItem gmshSetupItem, MeshingParameters meshingParameters, bool preview)
        {
            ExtrudeMesh extrudeMesh = null;
            RevolveMesh revolveMesh = null;
            //
            if (gmshSetupItem is ExtrudeMesh em) extrudeMesh = em;
            else if (gmshSetupItem is RevolveMesh rm) revolveMesh = rm;
            else throw new NotSupportedException();
            //
            if (extrudeMesh != null)
            {
                if (extrudeMesh.ExtrudeCenter == null || extrudeMesh.Direction == null)
                    throw new CaeGlobals.CaeException("The extrude direction could not be determined.");
            }
            else if (revolveMesh != null)
            {
                if (revolveMesh.AxisCenter == null || revolveMesh.AxisDirection == null)
                    throw new CaeGlobals.CaeException("The revolve direction could not be determined.");
            }
            //
            int surfaceId;
            HashSet<int> allExtrudeSurfaceIds = new HashSet<int>();
            bool recombine = gmshSetupItem.AlgorithmRecombine != GmshAlgorithmRecombineEnum.None;
            Tuple<int, int>[] extrudeDimTags = new Tuple<int, int>[gmshSetupItem.CreationIds.Length];            
            // Collect surfaces
            for (int i = 0; i < gmshSetupItem.CreationIds.Length; i++)
            {
                surfaceId = FeMesh.GetItemIdFromGeometryId(gmshSetupItem.CreationIds[i]) + 1;
                extrudeDimTags[i] = new Tuple<int, int>(2, surfaceId);
                allExtrudeSurfaceIds.Add(surfaceId);
                // Recombine surfaces
                if (recombine) Gmsh.Mesh.SetRecombine(2, surfaceId);
            }
            // Layers - size
            int numEl;
            if (gmshSetupItem.ElementSizeType == ElementSizeTypeEnum.ScaleFactor)
            {
                double edgeLength;
                if (extrudeMesh != null)
                {
                    edgeLength = Math.Sqrt(Math.Pow(extrudeMesh.Direction[0], 2) +
                                           Math.Pow(extrudeMesh.Direction[1], 2) +
                                           Math.Pow(extrudeMesh.Direction[2], 2));
                    
                }
                else if (revolveMesh != null)
                {
                    edgeLength = revolveMesh.MiddleR * revolveMesh.AngleDeg * Math.PI / 180;
                }
                else throw new NotSupportedException();
                //
                numEl = (int)Math.Round(edgeLength / meshingParameters.MaxH / gmshSetupItem.ElementScaleFactor, 0);
            }
            else if (gmshSetupItem.ElementSizeType == ElementSizeTypeEnum.NumberOfLayers)
                numEl = gmshSetupItem.NumberOfLayers;
            else throw new NotSupportedException("ExtrudedElementSizeTypeEnumException");
            //
            if (numEl < 1) numEl = 1;
            //
            int[] numElements = new int[] { numEl };
            double[] height = new double[] { 1 };
            //
            if (preview)
            {
                Tuple<int, int>[] allDimTags = Gmsh.OCC.GetEntities(2);
                List<Tuple<int, int>> toRemoveDimTags = new List<Tuple<int, int>>() { new Tuple<int, int>(3, 1) };  // add solid
                for (int i = 0; i < allDimTags.Length; i++)
                {
                    if (!allExtrudeSurfaceIds.Contains(allDimTags[i].Item2)) toRemoveDimTags.Add(allDimTags[i]);
                }
                //
                Gmsh.OCC.Remove(toRemoveDimTags.ToArray(), false);
                //
                Gmsh.OCC.Synchronize(); // must be here
                //
                Gmsh.Generate(2);
            }
            else
            {
                Tuple<int, int>[] outDimTags;
                // Extrude
                if (extrudeMesh != null)
                {
                    Gmsh.Geo.Extrude(extrudeDimTags,
                                     extrudeMesh.Direction[0],
                                     extrudeMesh.Direction[1],
                                     extrudeMesh.Direction[2],
                                     out outDimTags, numElements, height, true);
                }
                // Revolve
                else if (revolveMesh != null)
                {
                    Gmsh.Geo.Revolve(extrudeDimTags,
                                     revolveMesh.AxisCenter[0],
                                     revolveMesh.AxisCenter[1],
                                     revolveMesh.AxisCenter[2],
                                     revolveMesh.AxisDirection[0],
                                     revolveMesh.AxisDirection[1],
                                     revolveMesh.AxisDirection[2],
                                     revolveMesh.AngleDeg * Math.PI / 180,
                                     out outDimTags, numElements, height, true);
                }
                else throw new NotSupportedException();
                // Volume check 
                if (CheckMeshVolume(outDimTags))
                {
                    Gmsh.OCC.Remove(new Tuple<int, int>[] { new Tuple<int, int>(3, 1) }, true);
                    //
                    Gmsh.OCC.Synchronize(); // must be here
                    //
                    Gmsh.Generate(3);
                }
                else
                {
                    throw new CaeGlobals.CaeException("The volume of the extruded mesh is not equal to the volume " +
                        "of the geometry it represents. Try selecting other surfaces for the extrusion.");
                }
            }
        }
        //
        public static bool CheckMeshVolume(Tuple<int, int>[] outDimTags)
        {
            double volumeOut;
            double initialVolume;
            double extrudedVolume = 0;
            //
            Gmsh.OCC.GetMass(3, 1, out initialVolume);
            //
            foreach (var outDimTag in outDimTags)
            {
                if (outDimTag.Item1 == 3)
                {
                    Gmsh.OCC.GetMass(3, outDimTag.Item2, out volumeOut);
                    extrudedVolume += volumeOut;
                }
            }
            double maxVolume = Math.Max(initialVolume, extrudedVolume);
            if (Math.Abs(initialVolume - extrudedVolume) > 1E-2 * maxVolume) return false;
            //
            return true;
                
        }
        public static bool CheckMeshVolume(GeometryPart part, ExtrudeMesh extrudeMesh, 
                                           Func<GeometryPart, string> ExportCADPartGeometryToDefaultFile)
        {
            int surfaceId;
            string brepFileName = ExportCADPartGeometryToDefaultFile(part);
            // Initialize
            Gmsh.InitializeGmsh();
            Gmsh.Add("Brep_model");
            // Import
            Tuple<int, int>[] outDimTags;
            Gmsh.OCC.ImportShapes(brepFileName, out outDimTags, false, "");
            Gmsh.OCC.Synchronize(); // must be here
            // Get surfaces to extrude
            Tuple<int, int>[] dimTags = new Tuple<int, int>[extrudeMesh.CreationIds.Length];
            for (int i = 0; i < extrudeMesh.CreationIds.Length; i++)
            {
                surfaceId = FeMesh.GetItemIdFromGeometryId(extrudeMesh.CreationIds[i]) + 1;
                dimTags[i] = new Tuple<int, int>(2, surfaceId);
            }
            // Setup layers
            int[] numElements = new int[] { 1 };
            double[] height = new double[] { 1 };
            // Extrude
            Gmsh.Geo.Extrude(dimTags, 0, 0, -50, out outDimTags, numElements, height, false);
            //
            double massOut;
            double mass1 = 0;
            double massExtruded = 0;
            //
            Gmsh.OCC.GetMass(3, 1, out massOut);
            mass1 += massOut;
            //
            foreach (var outDimTag in outDimTags)
            {
                if (outDimTag.Item1 == 3)
                {
                    Gmsh.OCC.GetMass(3, outDimTag.Item2, out massOut);
                    massExtruded += massOut;
                }
            }
            return false;
        }
        private void SetTransfiniteSurfaces(bool transfiniteThreeSided, bool transfiniteFourSided, bool transfiniteVolume,
                                            bool recombine)
        {
            int[] volumeIds;
            int[] edgeIds;
            int[][] edgeVertexIds;
            int[] vertexIds;
            HashSet<int> surfaceVertexIds = new HashSet<int>();
            //
            int edgeDim = 1;
            int edgeId;
            int surfaceDim = 2;
            int surfaceId;
            int volumeId;
            double edgeLength;
            double[] edgeLengths;
            //
            Edge edge;
            Edge existingEdge;
            Dictionary<int, Edge> edgeIdEdge = new Dictionary<int, Edge>();
            Surface surface;
            Dictionary<int, Surface> surfaceIdSurface = new Dictionary<int, Surface>();
            Volume volume;
            Volume existingVolume;
            Dictionary<int, Volume> volumeIdVolume = new Dictionary<int, Volume>();
            //
            Tuple<int, int>[] vertexDimTags;
            Tuple<int, int>[] edgeDimTags;
            Tuple<int, int>[] surfaceDimTags;
            Tuple<int, int>[] volumeDimTags;
            //
            Gmsh.GetEntities(out vertexDimTags, 0);
            Gmsh.GetEntities(out edgeDimTags, 1);
            Gmsh.GetEntities(out surfaceDimTags, 2);
            Gmsh.GetEntities(out volumeDimTags, 3);
            // Collect volume faces
            foreach (var surfaceDimTag in surfaceDimTags)
            {
                surfaceId = surfaceDimTag.Item2;
                if (surfaceIdSurface.ContainsKey(surfaceId)) continue;
                //
                Gmsh.GetAdjacencies(surfaceDim, surfaceId, out volumeIds, out edgeIds);
                // Get edge orientations
                //
                //Gmsh.GetBoundary(new Tuple<int, int>[] { new Tuple<int, int>(3, 1) }, out edgeDimTags, false, true, false);
                //
                // Surface
                surfaceVertexIds.Clear();
                edgeVertexIds = new int[edgeIds.Length][];
                edgeLengths = new double[edgeIds.Length];
                for (int j = 0; j < edgeIds.Length; j++)
                {
                    edgeId = edgeIds[j];
                    Gmsh.GetAdjacencies(edgeDim, edgeId, out _, out vertexIds);
                    if (_isOCC) Gmsh.OCC.GetMass(edgeDim, edgeId, out edgeLength);
                    else edgeLength = 1;
                    edge = new Edge(edgeId, vertexIds, surfaceId, edgeLength);
                    edgeLengths[j] = edgeLength;
                    //
                    if (edgeIdEdge.TryGetValue(edgeId, out existingEdge))
                        existingEdge.SurfaceIds.UnionWith(edge.SurfaceIds);
                    else edgeIdEdge.Add(edge.Id, edge);
                    //
                    edgeVertexIds[j] = edge.VertexIds;
                    surfaceVertexIds.UnionWith(vertexIds);
                }
                //
                for (int i = 0; i < volumeIds.Length; i++)
                {
                    volumeId = volumeIds[i];
                    volume = new Volume(volumeId, surfaceId);
                    //
                    if (volumeIdVolume.TryGetValue(volumeId, out existingVolume))
                        existingVolume.SurfaceIds.UnionWith(volume.SurfaceIds);
                    else volumeIdVolume.Add(volume.Id, volume);
                }
                //
                surface = new Surface(surfaceId, surfaceVertexIds.ToArray(), edgeIds, edgeLengths, edgeVertexIds);
                surfaceIdSurface.Add(surface.Id, surface);
            }
            // Check if a volume is a transfinite volume                                                                            
            foreach (var entry in volumeIdVolume)
            {
                volumeId = entry.Key;
                volume = entry.Value;
                //
                if (!transfiniteVolume) volume.Transfinite = false;
                else if (volumeId > 0) volume.Transfinite = true;
                else volume.Transfinite = false;
                //
                if (volume.SurfaceIds.Count == 5 || volume.SurfaceIds.Count == 6)
                {
                    foreach (var volSurfaceId in volume.SurfaceIds)
                    {
                        surface = surfaceIdSurface[volSurfaceId];
                        if (surface.Transfinite)
                        {
                            if (surface.Triangular && transfiniteThreeSided) volume.TriSurfIds.Add(volSurfaceId);
                            else if (surface.Quadrangular && transfiniteFourSided) volume.QuadSurfIds.Add(volSurfaceId);
                        }
                        else { volume.Transfinite = false; break; }
                    }
                    if (volume.Transfinite)
                    {
                        if (!(volume.NumQuadSurfaces == 6 || (volume.NumTriSurfaces == 2 && volume.NumQuadSurfaces == 3)))
                            volume.Transfinite = false;
                    }
                }
                else volume.Transfinite = false;
                //
                if (!volume.Transfinite)
                {
                    foreach (var volumeSurfaceId in volume.SurfaceIds) surfaceIdSurface[volumeSurfaceId].Recombine = false;
                }
                // Fix 5 sided volumes
                if (volume.Transfinite && volume.SurfaceIds.Count == 5)
                {
                    HashSet<int> connectingEdgeIds = new HashSet<int>();
                    foreach (var quadSurfaceId in volume.QuadSurfIds)
                        connectingEdgeIds.UnionWith(surfaceIdSurface[quadSurfaceId].EdgeIds);
                    foreach (var triSurfaceId in volume.TriSurfIds)
                        connectingEdgeIds.ExceptWith(surfaceIdSurface[triSurfaceId].EdgeIds);
                    //
                    surface = surfaceIdSurface[volume.TriSurfIds[0]];
                    int firstSurfaceVertexId = surface.VertexIds[0];
                    int secondSurfaceVertexId = -1;
                    //
                    foreach (var connectingEdgeId in connectingEdgeIds)
                    {
                        edge = edgeIdEdge[connectingEdgeId];
                        if (edge.VertexIds[0] == firstSurfaceVertexId) { secondSurfaceVertexId = edge.VertexIds[1]; break; }
                        else if (edge.VertexIds[1] == firstSurfaceVertexId) { secondSurfaceVertexId = edge.VertexIds[0]; break; }
                    }
                    //
                    if (secondSurfaceVertexId != -1)
                    {
                        surface = surfaceIdSurface[volume.TriSurfIds[1]];
                        surface.SetFirstVertexId(secondSurfaceVertexId);
                    }
                }
            }
            // Edge graph
            Node<Edge> edgeNode;
            Graph<Edge> edgeGraph = new Graph<Edge>();
            Dictionary<int, Node<Edge>> edgeIdNodeEdge = new Dictionary<int, Node<Edge>>();
            // Add opposite edges as connections to graph
            foreach (var entry in surfaceIdSurface)
            {
                surface = entry.Value;
                if (surface.Transfinite)
                {
                    // Add nodes to graph
                    for (int i = 0; i < surface.EdgeIds.Length; i++)
                    {
                        edgeId = surface.EdgeIds[i];
                        if (!edgeIdNodeEdge.ContainsKey(edgeId))
                        {
                            edgeNode = new Node<Edge>(edgeIdEdge[edgeId]);
                            edgeIdNodeEdge.Add(edgeId, edgeNode);
                            edgeGraph.AddNode(edgeNode);
                        }
                    }
                    // Add connections to graph
                    edgeGraph.AddUndirectedEdge(edgeIdNodeEdge[surface.OppositeEdgesA[0]],
                                                edgeIdNodeEdge[surface.OppositeEdgesA[1]]);
                    if (surface.OppositeEdgesB != null)
                    {
                        edgeGraph.AddUndirectedEdge(edgeIdNodeEdge[surface.OppositeEdgesB[0]],
                                                    edgeIdNodeEdge[surface.OppositeEdgesB[1]]);
                    }
                }
                
            }
            // Get groups of connected edges
            List<Graph<Edge>> edgeGroups = edgeGraph.GetConnectedSubgraphs();
            //
            int min;
            int max;
            int value;
            HashSet<int> groupEdgeIds;
            IntPtr[] nodeTagsIntPtr;
            double[] coor;
            // Create node mesh to get number of nodes for each edge
            Gmsh.Generate(1);
            //
            Dictionary<int, int> edgeIdNumElements = new Dictionary<int, int>();
            foreach (var edgeGroup in edgeGroups)
            {
                if (edgeGroup.Nodes.Count <= 1) continue;
                //
                min = int.MaxValue;
                max = -int.MaxValue;
                groupEdgeIds = new HashSet<int>();
                foreach (var edgeNodeFromGroup in edgeGroup.Nodes)
                {
                    Gmsh.Mesh.GetNodes(out nodeTagsIntPtr, out coor, 1, edgeNodeFromGroup.Value.Id, true, false);
                    //
                    value = nodeTagsIntPtr.Length;
                    if (value < min) min = value;
                    if (value > max) max = value;
                }
                //
                value = (int)Math.Round((min + max) / 2.0);
                foreach (var edgeNodeFromGroup in edgeGroup.Nodes)
                {
                    Gmsh.Mesh.SetTransfiniteCurve(edgeNodeFromGroup.Value.Id, value);
                    edgeIdNumElements.Add(edgeNodeFromGroup.Value.Id, value);
                }
            }
            //
            Gmsh.Mesh.Clear(); // must clear the mesh
            //
            foreach (var entry in surfaceIdSurface)
            {
                surface = entry.Value;
                if (surface.Transfinite)
                {
                    // "Left", "Right", "AlternateLeft" and "AlternateRight"
                    if (surface.Triangular && transfiniteThreeSided)
                        Gmsh.Mesh.SetTransfiniteSurface(surface.Id, "Left", surface.VertexIds);
                    else if (surface.Quadrangular && transfiniteFourSided)
                        Gmsh.Mesh.SetTransfiniteSurface(surface.Id, "Left");
                    //
                    if (recombine && surface.Recombine) Gmsh.Mesh.SetRecombine(2, surface.Id);
                    Gmsh.Mesh.SetSmoothing(2, surface.Id, 100);
                }
            }
            //
            foreach (var entry in volumeIdVolume)
            {
                if (entry.Value.Transfinite) Gmsh.Mesh.SetTransfiniteVolume(entry.Key);
                //Gmsh.Mesh.SetOutwardOrientation(volumeDimTag.Item2);
            }
        }
        //
        private void Synchronize()
        {
            if (_isOCC) Gmsh.OCC.Synchronize();
            else Gmsh.Geo.Synchronize();
        }
        private void WriteLog()
        {
            if (Gmsh.IsInitialized() == 1)
            {
                string[] loggerLines = Gmsh.Logger.Get();
                if (loggerLines != null && loggerLines.Length > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = _currentLogLine; i < loggerLines.Length; i++)
                        sb.AppendLine(loggerLines[i]);
                    //
                    _currentLogLine = loggerLines.Length;
                    //
                    _writeOutput(sb.ToString());
                }
            }
        }
        //                                                                                                                          
        private class Volume : IComparable<Volume>
        {
            public int Id;
            public HashSet<int> SurfaceIds;
            public List<int> TriSurfIds;
            public List<int> QuadSurfIds;
            public bool Transfinite;
            //
            public int NumTriSurfaces { get { return TriSurfIds.Count(); } }
            public int NumQuadSurfaces { get { return QuadSurfIds.Count(); } }
            //
            public Volume(int id, int surfaceId)
            {
                Id = id;
                SurfaceIds = new HashSet<int> { surfaceId };
                TriSurfIds = new List<int>();
                QuadSurfIds = new List<int>();
                Transfinite = false;
            }
            //
            public int CompareTo(Volume other)
            {
                if (Id < other.Id) return 1;
                else if (Id > other.Id) return -1;
                else return 0;
            }
        }
        private class Surface : IComparable<Surface>
        {
            public int[] OppositeEdgesA;
            public int[] OppositeEdgesB;
            public int Id;
            public int[] VertexIds;
            public int[] EdgeIds;
            private int[][] _edgeVertexIds;
            public bool Triangular;
            public bool Quadrangular;
            public bool HasHole;
            public bool Collapsed;
            public bool Transfinite;
            public bool Recombine;
            //
            public Surface(int id, int[] vertexIds, int[] edgeIds, double[] edgeLengths, int[][] edgeVertexIds)
            {
                Id = id;
                VertexIds = vertexIds;
                EdgeIds = edgeIds;
                _edgeVertexIds = edgeVertexIds;
                Triangular = false;
                Quadrangular = false;
                HasHole = false;
                Collapsed = false;
                Transfinite = false;
                Recombine = true;
                // Fix collapsed triangular faces
                if (VertexIds.Length == 3 && edgeIds.Length == 4)
                {
                    int count = 0;
                    int[] fixedEdgeIds = new int[3];
                    double[] fixedEdgeLengths = new double[3];
                    int[][] fixedEdgeVertexIds = new int[3][];
                    for (int i = 0; i < _edgeVertexIds.Length; i++)
                    {
                        if (_edgeVertexIds[i][0] != _edgeVertexIds[i][1])
                        {
                            if (count < 3)
                            {
                                fixedEdgeIds[count] = edgeIds[i];
                                fixedEdgeLengths[count] = edgeLengths[i];
                                fixedEdgeVertexIds[count] = _edgeVertexIds[i];
                                count++;
                            }
                            else Collapsed = true;
                        }
                    }
                    if (!Collapsed)
                    {
                        edgeIds = fixedEdgeIds;
                        edgeLengths = fixedEdgeLengths;
                        _edgeVertexIds = fixedEdgeVertexIds;
                    }
                }
                //
                if (VertexIds.Length == 3 && edgeIds.Length == 3)
                {
                    double delta1 = Math.Abs(edgeLengths[0] - edgeLengths[1]);
                    double delta2 = Math.Abs(edgeLengths[1] - edgeLengths[2]);
                    double delta3 = Math.Abs(edgeLengths[2] - edgeLengths[0]);
                    double min = Math.Min(delta1, delta2);
                    min = Math.Min(min, delta3);
                    //
                    int firstVertexId;
                    if (delta1 == min)
                    {
                        firstVertexId = _edgeVertexIds[0].Intersect(_edgeVertexIds[1]).First();
                        OppositeEdgesA = new int[2] { edgeIds[0], edgeIds[1] };
                    }
                    else if (delta2 == min)
                    {
                        firstVertexId = _edgeVertexIds[1].Intersect(_edgeVertexIds[2]).First();
                        OppositeEdgesA = new int[2] { edgeIds[1], edgeIds[2] };
                    }
                    else
                    {
                        firstVertexId = _edgeVertexIds[2].Intersect(_edgeVertexIds[0]).First();
                        OppositeEdgesA = new int[2] { edgeIds[2], edgeIds[0] };
                    }
                    Queue<int> queue = new Queue<int>(VertexIds);
                    while (queue.Peek() != firstVertexId)
                        queue.Enqueue(queue.Dequeue());
                    VertexIds = queue.ToArray();
                    //
                    OppositeEdgesB = null;
                    //
                    Triangular = true;
                    Transfinite = true;
                }
                else if (VertexIds.Length == 4 && edgeIds.Length == 4)
                {
                    // Find opposite edge to the first edge
                    OppositeEdgesA = new int[2] { edgeIds[0], -1 };
                    for (int i = 1; i < edgeIds.Length; i++)
                    {
                        if (edgeVertexIds[0].Intersect(edgeVertexIds[i]).Count() == 0) OppositeEdgesA[1] = edgeIds[i];
                        else if (edgeVertexIds[0].Intersect(edgeVertexIds[i]).Count() == 2) HasHole = true;
                    }
                    OppositeEdgesB = edgeIds.Except(OppositeEdgesA).ToArray();
                    //
                    Quadrangular = true;
                    if (!HasHole) Transfinite = true;
                }
                else if (VertexIds.Length != edgeIds.Length)
                    Collapsed = true;
            }
            public void SetFirstVertexId(int firstVertexId)
            {
                Queue<int> queue = new Queue<int>(VertexIds);
                while (queue.Peek() != firstVertexId)
                    queue.Enqueue(queue.Dequeue());
                VertexIds = queue.ToArray();
                //
                if (firstVertexId == _edgeVertexIds[0].Intersect(_edgeVertexIds[1]).First())
                {
                    OppositeEdgesA = new int[2] { EdgeIds[0], EdgeIds[1] };
                }
                else if (firstVertexId == _edgeVertexIds[1].Intersect(_edgeVertexIds[2]).First())
                {
                    OppositeEdgesA = new int[2] { EdgeIds[1], EdgeIds[2] };
                }
                else if (firstVertexId == _edgeVertexIds[2].Intersect(_edgeVertexIds[0]).First())
                {
                    OppositeEdgesA = new int[2] { EdgeIds[2], EdgeIds[0] };
                }
                //
                OppositeEdgesB = null;
            }
            //
            public int CompareTo(Surface other)
            {
                if (Id < other.Id) return 1;
                else if (Id > other.Id) return -1;
                else return 0;
            }
        }
        private class Edge : IComparable<Edge>
        {
            public int Id;
            public int[] VertexIds;
            public HashSet<int> SurfaceIds;
            public double Length;
            //
            public Edge(int id, int[] vertexIds, int surfaceId, double length)
            {
                Id = id;
                VertexIds = vertexIds;
                SurfaceIds = new HashSet<int> { surfaceId };
                Length = length;
            }
            //
            public int CompareTo(Edge other)
            {
                if (Id < other.Id) return 1;
                else if (Id > other.Id) return -1;
                else return 0;
            }
        }
    }
}
