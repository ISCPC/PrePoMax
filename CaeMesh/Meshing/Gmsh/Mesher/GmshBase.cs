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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using CaeGlobals;
using System.Drawing.Printing;
using System.Windows.Forms.VisualStyles;

namespace CaeMesh
{
    public class GmshBase
    {
        // Variables                                                                                                                
        private GmshData _gmshData;
        private Action<string> _writeOutput;
        private bool _isOCC;
        private string _error;
        private Thread _thread;
        private int _currentLogLine;


        // Properties                                                                                                               
        public GmshData GmshData { get { return _gmshData; } }


        public GmshBase(GmshData gmshData, Action<string> writeOutput)
        {
            _gmshData = gmshData;
            _writeOutput = writeOutput;
            //
            if (_gmshData.GeometryFileName.EndsWith("brep")) _isOCC = true;
            else if (_gmshData.GeometryFileName.EndsWith("stl")) _isOCC = false;
            else throw new NotSupportedException();
        }
        public string CreateMesh()
        {
            return RunInBackground(CreateMeshBackground);
        }
        public string GetOccNormals()
        {
            return RunInBackground(GetOccNormalsBackground);
        }
        //
        private string RunInBackground(Action action)
        {
            try
            {
                Gmsh.Initialize();
                Gmsh.Model.Add("Model-1");
                Gmsh.Logger.Start();
                _currentLogLine = 0;
                // Common options
                Gmsh.Option.SetNumber("Geometry.OCCScaling", 1);
                //
                _thread = new Thread(new ThreadStart(() => action()));
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
                Gmsh.FinalizeAll();
                // Check if the thread was not aborted on error
                if (_error != null) File.Delete(_gmshData.InpFileName);
            }
        }
        // Meshing                                                                                                                  
        private void CreateMeshBackground()
        {
            try
            {
                if (_gmshData.GmshSetupItems.Length != 1)
                    throw new CaeException("Currently, for a single part, only one active mesh setup item of the type: " +
                        "Shell gmsh, Tetrahedral gmsh, Transfinite mesh, Extrude mesh or Revolve mesh is possible.");
                //
                MeshSetupItem meshSetupItem = _gmshData.GmshSetupItems[0];
                //
                if (meshSetupItem is GmshSetupItem gsi)
                {
                    Tuple<int, int>[] outDimTags;
                    if (_isOCC) Gmsh.Model.OCC.ImportShapes(_gmshData.GeometryFileName, out outDimTags, false, "");
                    else
                    {   // Discrete stl geometry
                        double angleDeg = 30;
                        Gmsh.Merge(_gmshData.GeometryFileName);
                        Gmsh.Model.Mesh.RemoveDuplicateNodes();
                        Gmsh.Model.Mesh.ClassifySurfaces(angleDeg * Math.PI / 180, true, true, 3 * angleDeg * Math.PI / 180, true);
                        Gmsh.Model.Mesh.CreateGeometry();
                        Gmsh.Model.GetEntities(out outDimTags, 2);
                        int[] surfaceIds = new int[outDimTags.Length];
                        for (int i = 0; i < outDimTags.Length; i++) surfaceIds[i] = outDimTags[i].Item2;
                        int volumeId = Gmsh.Model.Geo.AddSurfaceLoop(surfaceIds);
                        Gmsh.Model.Geo.AddVolume(new int[] { volumeId });
                    }
                    //
                    Synchronize(); // must be here
                    // Mesh size
                    SetMeshSizes();
                    // 2D meshing algorithm
                    Gmsh.Option.SetNumber("Mesh.Algorithm", (int)gsi.AlgorithmMesh2D);
                    // 3D meshing algorithm
                    Gmsh.Option.SetNumber("Mesh.Algorithm3D", (int)gsi.AlgorithmMesh3D);
                    // Recombine
                    bool recombine = gsi.AlgorithmRecombine != GmshAlgorithmRecombineEnum.None;
                    if (recombine)
                    {
                        Gmsh.Option.SetNumber("Mesh.RecombinationAlgorithm", (int)gsi.AlgorithmRecombine);
                        Gmsh.Option.SetNumber("Mesh.RecombineMinimumQuality", gsi.RecombineMinQuality);
                    }
                    // Transfinite
                    bool transfiniteVolume = gsi is TransfiniteMesh && gsi.TransfiniteThreeSided && gsi.TransfiniteFourSided;
                    if (_isOCC && (gsi.TransfiniteThreeSided || gsi.TransfiniteFourSided))
                        //Gmsh.Mesh.SetTransfiniteAutomatic(gsi.TransfiniteAngleRad, recombine);
                        SetTransfiniteSurfaces(gsi.TransfiniteThreeSided, gsi.TransfiniteFourSided, transfiniteVolume, recombine);
                    //
                    if (gsi is ShellGmsh)
                        ShellGmsh(gsi, _gmshData.Preview);
                    else if (gsi is TetrahedralGmsh)
                        TetrahedralGmsh(gsi, _gmshData.Preview);
                    else if (gsi is TransfiniteMesh)
                        TransfiniteMesh(_gmshData.Preview);
                    else if (gsi is ExtrudeMesh || gsi is RevolveMesh)
                        ExtrudeRevolveMesh(gsi, _gmshData.PartMeshingParameters, _gmshData.Preview);
                    else throw new NotSupportedException("MeshSetupItemTypeException");
                }
                else throw new NotSupportedException("MeshSetupItemTypeException");
                // Element order
                if (!_gmshData.Preview && _gmshData.PartMeshingParameters.SecondOrder)
                {
                    if (!_gmshData.PartMeshingParameters.MidsideNodesOnGeometry) 
                        Gmsh.Option.SetNumber("Mesh.SecondOrderLinear", 1);    // first
                    // Create incomplete second order elements: 8-node quads, 20-node hexas, etc.
                    Gmsh.Option.SetNumber("Mesh.SecondOrderIncomplete", 1);    // second
                    Gmsh.Model.Mesh.SetOrder(2);                              // third
                    // Optimize high order
                    if (gsi.OptimizeHighOrder != GmshOptimizeHighOrderEnum.None)
                    {
                        Tuple<int, int>[] dimTags = new Tuple<int, int>[0];
                        Gmsh.Model.Mesh.Optimize(gsi.OptimizeHighOrder.ToString(), false, 1, dimTags);
                    }
                }
                // Output
                Gmsh.Write(_gmshData.InpFileName);
                //Gmsh.Write(@"C:\Temp\mesh.msh");
                //
                _writeOutput?.Invoke("Meshing done.");
                _writeOutput?.Invoke("");
                double elapsedTime = Gmsh.Option.GetNumber("Mesh.CpuTime");
                _writeOutput?.Invoke("Elapsed time [s]: " + Math.Round(elapsedTime, 5));
                _writeOutput?.Invoke("");
                //
                _error = null;
            }
            catch (Exception ex)
            {
                _error = ex.Message;
            }
        }
        private void SetMeshSizes()
        {
            RenumberGmshDataVerticesByCoor();
            // Mesh size
            //Tuple<int, int>[] surfaceDimTags;
            //Gmsh.GetEntities(out surfaceDimTags, 2);
            //foreach (var surfaceDimTag in surfaceDimTags) Gmsh.Mesh.SetSizeFromBoundary(2, surfaceDimTag.Item2, 0);
            //
            double scaleFactor = 1;
            //
            Gmsh.Option.SetNumber("Mesh.MeshSizeMin", _gmshData.PartMeshingParameters.MinH * scaleFactor);
            Gmsh.Option.SetNumber("Mesh.MeshSizeMax", _gmshData.PartMeshingParameters.MaxH * scaleFactor);
            Gmsh.Option.SetNumber("Mesh.MeshSizeFromCurvature", 2 * Math.PI * _gmshData.PartMeshingParameters.ElementsPerCurve);
            // Local vertex mesh size
            Tuple<int, int>[] dimTags = new Tuple<int, int>[1];
            foreach (var entry in _gmshData.VertexNodeIdMeshSize)
            {
                dimTags[0] = new Tuple<int, int>(0, entry.Key);
                Gmsh.Model.OCC.SetSize(dimTags, entry.Value);
            }
            // Local edge mesh size
            int edgeId;
            int numOfElements;
            int numOfNodes;
            //
            Gmsh.Model.GetEntities(out dimTags, 1);
            //
            foreach (var entry in dimTags)
            {
                edgeId = entry.Item2;
                if (_gmshData.EdgeIdNumElements.TryGetValue(edgeId, out numOfElements))
                {
                    numOfNodes = numOfElements + 1;
                    Gmsh.Model.Mesh.SetTransfiniteCurve(edgeId, numOfNodes);
                }
            }
            //
            Synchronize(); // must be here for mesh refinement
        }
        private void RenumberGmshDataVerticesByCoor()
        {
            double[] coor;
            Tuple<int, int>[] pointDimTags;
            Gmsh.Model.GetEntities(out pointDimTags, 0);
            Dictionary<int, double[]> pointIdCoor = new Dictionary<int, double[]>();
            foreach (var item in pointDimTags)
            {
                Gmsh.Model.GetValue(item.Item1, item.Item2, new double[3], out coor);
                pointIdCoor.Add(item.Item2, coor);
            }
            int minId;
            double minDistance;
            double dx;
            double dy;
            double dz;
            double[] coorN;
            double[] coorG;
            Dictionary<int, int> netGenIdGmshId = new Dictionary<int, int>();
            foreach (var netGenEntry in _gmshData.VertexNodes)
            {
                minId = -1;
                minDistance = double.MaxValue;
                coorN = netGenEntry.Value.Coor;
                foreach (var gmshEntry in pointIdCoor)
                {
                    coorG = gmshEntry.Value;
                    dx = Math.Abs(coorN[0] - coorG[0]);
                    if (dx <= minDistance)
                    {
                        dy = Math.Abs(coorN[1] - coorG[1]);
                        if (dy <= minDistance)
                        {
                            dz = Math.Abs(coorN[2] - coorG[2]);
                            if (dz <= minDistance)
                            {
                                minDistance = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2) + Math.Pow(dz, 2));
                                minId = gmshEntry.Key;
                                if (minDistance == 0) break;
                            }
                        }
                    }
                }
                //
                netGenIdGmshId.Add(netGenEntry.Key, minId);
            }
            // Renumber vertex node ids
            Dictionary<int, double> vertexIdMeshSize = new Dictionary<int, double>();
            foreach (var entry in _gmshData.VertexNodeIdMeshSize) vertexIdMeshSize[netGenIdGmshId[entry.Key]] = entry.Value;
            _gmshData.VertexNodeIdMeshSize = vertexIdMeshSize;
            // Renumber edge vertex node ids
            int edgeId;
            int oldEdgeId = -1;
            double min;
            double d2;
            int[] vertexIds;
            int[] surfaceIds;
            double x;
            double y;
            double z;
            Vec3D xyz;
            Vec3D cog;
            List<GmshIdLocation> idLocationList;
            Tuple<int, int>[] dimTags;
            Dictionary<int, int> oldEdgeIdEdgeId = new Dictionary<int, int>();
            Gmsh.Model.GetEntities(out dimTags, 1);
            //
            foreach (var entry in dimTags)
            {
                edgeId = entry.Item2;
                Gmsh.Model.GetAdjacencies(1, edgeId, out surfaceIds, out vertexIds);
                Array.Sort(vertexIds);
                Gmsh.Model.OCC.GetCenterOfMass(1, edgeId, out x, out y, out z);
                xyz = new Vec3D(x, y, z);
                //
                if (_gmshData.EdgeVertexNodeIdsEdgeId.TryGetValue(vertexIds, out idLocationList))
                {
                    if (idLocationList.Count() == 1)
                    {
                        oldEdgeId = idLocationList[0].Id;
                    }
                    else
                    {
                        min = double.MaxValue;
                        foreach (var idLocation in idLocationList)
                        {
                            cog = new Vec3D(idLocation.Location);
                            d2 = (cog - xyz).Len2;
                            if (d2 < min)
                            {
                                min = d2;
                                oldEdgeId = idLocation.Id;
                            }
                        }
                    }
                    //
                    oldEdgeIdEdgeId[oldEdgeId] = edgeId;
                }
            }
            //
            Dictionary<int,int> edgeIdNumElements = new Dictionary<int, int>();
            foreach (var entry in _gmshData.EdgeIdNumElements)
            {
                edgeIdNumElements[oldEdgeIdEdgeId[entry.Key]] = entry.Value;
            }
            _gmshData.EdgeIdNumElements = edgeIdNumElements;
        }
        //
        private void ShellGmsh(GmshSetupItem gmshSetupItem, bool preview)
        {
            bool recombine = gmshSetupItem.AlgorithmRecombine != GmshAlgorithmRecombineEnum.None;
            // Recombine all
            if (recombine) Gmsh.Option.SetNumber("Mesh.RecombineAll", 1);
            //
            if (preview) Gmsh.Model.Mesh.Generate(2);
            else Gmsh.Model.Mesh.Generate(2);
            // Optimize first order
            if (gmshSetupItem.OptimizeFirstOrderShell != GmshOptimizeFirstOrderShellEnum.None)
            {
                Tuple<int, int>[] dimTags = new Tuple<int, int>[0];
                Gmsh.Model.Mesh.Optimize(gmshSetupItem.OptimizeFirstOrderShell.ToString(), false, 10, dimTags);
            }
        }
        private void TetrahedralGmsh(GmshSetupItem gmshSetupItem, bool preview)
        {
            if (preview) Gmsh.Model.Mesh.Generate(1);
            else Gmsh.Model.Mesh.Generate(3);
            // Optimize first order
            if (gmshSetupItem.OptimizeFirstOrderSolid != GmshOptimizeFirstOrderSolidEnum.None)
            {
                Tuple<int, int>[] dimTags = new Tuple<int, int>[0];
                Gmsh.Model.Mesh.Optimize(gmshSetupItem.OptimizeFirstOrderSolid.ToString(), false, 10, dimTags);
            }
        }
        private void TransfiniteMesh(bool preview)
        {
            if (preview) Gmsh.Model.Mesh.Generate(1);
            else Gmsh.Model.Mesh.Generate(3);
        }
        private void ExtrudeRevolveMesh(GmshSetupItem gmshSetupItem, MeshingParameters meshingParameters, bool preview)
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
                if (recombine) Gmsh.Model.Mesh.SetRecombine(2, surfaceId);
            }
            // Layers - size
            int numEl;
            int[] numElements;
            double[] height;
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
                if (numEl < 1) numEl = 1;
                numElements = new int[] { numEl };
                height = new double[] { 1 };
            }
            else if (gmshSetupItem.ElementSizeType == ElementSizeTypeEnum.NumberOfElements)
            {
                numEl = gmshSetupItem.NumberOfElements;
                if (numEl < 1) numEl = 1;
                numElements = new int[] { numEl };
                height = new double[] { 1 };
            }
            else if (gmshSetupItem.ElementSizeType == ElementSizeTypeEnum.MultiLayerd)
            {
                numElements = gmshSetupItem.NumOfElementsPerLayer;
                double sum = 0;
                height = new double[gmshSetupItem.NormalizedLayerSizes.Length];
                for (int i = 0; i < height.Length; i++)
                {
                    sum += gmshSetupItem.NormalizedLayerSizes[i];
                    height[i] = sum;
                }
            }
            else throw new NotSupportedException("ExtrudedElementSizeTypeEnumException");
            //
            if (preview)
            {
                Tuple<int, int>[] allDimTags = Gmsh.Model.OCC.GetEntities(2);
                List<Tuple<int, int>> toRemoveDimTags = new List<Tuple<int, int>>() { new Tuple<int, int>(3, 1) };  // add solid
                for (int i = 0; i < allDimTags.Length; i++)
                {
                    if (!allExtrudeSurfaceIds.Contains(allDimTags[i].Item2)) toRemoveDimTags.Add(allDimTags[i]);
                }
                //
                Gmsh.Model.OCC.Remove(toRemoveDimTags.ToArray(), false);
                //
                Gmsh.Model.OCC.Synchronize(); // must be here
                //
                Gmsh.Model.Mesh.Generate(2);
            }
            else
            {
                Tuple<int, int>[] outDimTags;
                // Extrude
                if (extrudeMesh != null)
                {
                    Gmsh.Model.OCC.Extrude(extrudeDimTags,
                                           extrudeMesh.Direction[0],
                                           extrudeMesh.Direction[1],
                                           extrudeMesh.Direction[2],
                                           out outDimTags, numElements, height, true);
                }
                // Revolve
                else if (revolveMesh != null)
                {
                    Gmsh.Model.OCC.Revolve(extrudeDimTags,
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
                    Gmsh.Model.OCC.Remove(new Tuple<int, int>[] { new Tuple<int, int>(3, 1) }, true);
                    //
                    Gmsh.Model.OCC.Synchronize(); // must be here
                    //
                    Gmsh.Model.Mesh.Generate(3);
                }
                else
                {
                    throw new CaeGlobals.CaeException("The volume of the extruded mesh is not equal to the volume " +
                        "of the geometry it represents. Try selecting other surfaces for the extrusion.");
                }
            }
        }
        //
        public bool CheckMeshVolume(Tuple<int, int>[] outDimTags)
        {
            double volumeOut;
            double initialVolume;
            double extrudedVolume = 0;
            //
            Gmsh.Model.OCC.GetMass(3, 1, out initialVolume);
            //
            foreach (var outDimTag in outDimTags)
            {
                if (outDimTag.Item1 == 3)
                {
                    Gmsh.Model.OCC.GetMass(3, outDimTag.Item2, out volumeOut);
                    extrudedVolume += volumeOut;
                }
            }
            double maxVolume = Math.Max(initialVolume, extrudedVolume);
            if (Math.Abs(initialVolume - extrudedVolume) > 1E-2 * maxVolume) return false;
            //
            return true;
                
        }
        public bool CheckMeshVolume(GeometryPart part, ExtrudeMesh extrudeMesh, 
                                           Func<GeometryPart, string> ExportCADPartGeometryToDefaultFile)
        {
            int surfaceId;
            string brepFileName = ExportCADPartGeometryToDefaultFile(part);
            // Initialize
            Gmsh.Initialize();
            Gmsh.Model.Add("Brep_model");
            // Import
            Tuple<int, int>[] outDimTags;
            Gmsh.Model.OCC.ImportShapes(brepFileName, out outDimTags, false, "");
            Gmsh.Model.OCC.Synchronize(); // must be here
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
            Gmsh.Model.OCC.Extrude(dimTags, 0, 0, -50, out outDimTags, numElements, height, false);
            //
            double massOut;
            double mass1 = 0;
            double massExtruded = 0;
            //
            Gmsh.Model.OCC.GetMass(3, 1, out massOut);
            mass1 += massOut;
            //
            foreach (var outDimTag in outDimTags)
            {
                if (outDimTag.Item1 == 3)
                {
                    Gmsh.Model.OCC.GetMass(3, outDimTag.Item2, out massOut);
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
            //bool invertEdge;
            int edgeDim = 1;
            int edgeId;
            int surfaceDim = 2;
            int surfaceId;
            int volumeId;
            double edgeLength;
            double[] edgeLengths;
            //
            GmshEdge edge;
            GmshEdge existingEdge;
            Dictionary<int, GmshEdge> edgeIdEdge = new Dictionary<int, GmshEdge>();
            GmshSurface surface;
            Dictionary<int, GmshSurface> surfaceIdSurface = new Dictionary<int, GmshSurface>();
            GmshVolume volume;
            GmshVolume existingVolume;
            Dictionary<int, GmshVolume> volumeIdVolume = new Dictionary<int, GmshVolume>();
            //
            Tuple<int, int>[] vertexDimTags;
            Tuple<int, int>[] edgeDimTags;
            Tuple<int, int>[] surfaceDimTags;
            Tuple<int, int>[] volumeDimTags;
            //
            Gmsh.Model.GetEntities(out vertexDimTags, 0);
            Gmsh.Model.GetEntities(out edgeDimTags, 1);
            Gmsh.Model.GetEntities(out surfaceDimTags, 2);
            Gmsh.Model.GetEntities(out volumeDimTags, 3);
            // Collect volume faces
            foreach (var surfaceDimTag in surfaceDimTags)
            {
                surfaceId = surfaceDimTag.Item2;
                if (surfaceIdSurface.ContainsKey(surfaceId)) continue;
                //
                Gmsh.Model.GetAdjacencies(surfaceDim, surfaceId, out volumeIds, out edgeIds);
                // Get edge orientations
                //Gmsh.GetBoundary(new Tuple<int, int>[] { new Tuple<int, int>(surfaceDim, surfaceId) },
                //                 out edgeDimTags, false, true, false);
                // Surface
                surfaceVertexIds.Clear();
                edgeVertexIds = new int[edgeIds.Length][];
                edgeLengths = new double[edgeIds.Length];
                for (int j = 0; j < edgeIds.Length; j++)
                {
                    edgeId = edgeIds[j];
                    //
                    Gmsh.Model.GetAdjacencies(edgeDim, edgeId, out _, out vertexIds);
                    //
                    Array.Sort(vertexIds);
                    //
                    if (_isOCC) Gmsh.Model.OCC.GetMass(edgeDim, edgeId, out edgeLength);
                    else edgeLength = 1;
                    edge = new GmshEdge(edgeId, vertexIds, surfaceId, edgeLength);
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
                    volume = new GmshVolume(volumeId, surfaceId);
                    //
                    if (volumeIdVolume.TryGetValue(volumeId, out existingVolume))
                        existingVolume.SurfaceIds.UnionWith(volume.SurfaceIds);
                    else volumeIdVolume.Add(volume.Id, volume);
                }
                //
                surface = new GmshSurface(surfaceId, surfaceVertexIds.ToArray(), edgeIds, edgeLengths, edgeVertexIds);
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
                if (volume.Transfinite)
                {
                    foreach (var volumeSurfaceId in volume.SurfaceIds) surfaceIdSurface[volumeSurfaceId].Recombine = true;
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
            Node<GmshEdge> edgeNode;
            Graph<GmshEdge> edgeGraph = new Graph<GmshEdge>();
            Dictionary<int, Node<GmshEdge>> edgeIdNodeEdge = new Dictionary<int, Node<GmshEdge>>();
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
                            edgeNode = new Node<GmshEdge>(edgeIdEdge[edgeId]);
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
            List<Graph<GmshEdge>> edgeGroups = edgeGraph.GetConnectedSubgraphs();
            //
            int min;
            int max;
            int value;
            int numOfElements;
            int overriddenNumElements;
            HashSet<int> groupEdgeIds;
            IntPtr[] nodeTagsIntPtr;
            double[] coor;
            // Create edge mesh to get number of nodes for each edge
            Gmsh.Model.Mesh.Generate(1);
            //
            Dictionary<int, int> edgeIdNumElements = new Dictionary<int, int>();
            foreach (var edgeGroup in edgeGroups)
            {
                if (edgeGroup.Nodes.Count <= 1) continue;
                //
                min = int.MaxValue;
                max = -int.MaxValue;
                overriddenNumElements = -1;
                groupEdgeIds = new HashSet<int>();
                foreach (var edgeNodeFromGroup in edgeGroup.Nodes)
                {
                    //if (_gmshData.EdgeVertexNodeIdsNumElements.TryGetValue(edgeNodeFromGroup.Value.VertexIds, out numOfElements))
                    //    overriddenNumElements = numOfElements;
                    //else
                    {
                        Gmsh.Model.Mesh.GetNodes(out nodeTagsIntPtr, out coor, 1, edgeNodeFromGroup.Value.Id, true, false);
                        //
                        value = nodeTagsIntPtr.Length;
                        if (value < min) min = value;
                        if (value > max) max = value;
                    }
                }
                //
                if (overriddenNumElements > 0) 
                    value = overriddenNumElements + 1;
                else value = (int)Math.Round((min + max) / 2.0);
                //
                foreach (var edgeNodeFromGroup in edgeGroup.Nodes)
                {
                    Gmsh.Model.Mesh.SetTransfiniteCurve(edgeNodeFromGroup.Value.Id, value);
                    edgeIdNumElements.Add(edgeNodeFromGroup.Value.Id, value);
                }
            }
            //
            Gmsh.Model.Mesh.Clear(); // must clear the mesh
            //
            foreach (var entry in surfaceIdSurface)
            {
                surface = entry.Value;
                if (surface.Transfinite)
                {
                    // "Left", "Right", "AlternateLeft" and "AlternateRight"
                    if (surface.Triangular && transfiniteThreeSided)
                        Gmsh.Model.Mesh.SetTransfiniteSurface(surface.Id, "AlternateLeft", surface.VertexIds);
                    else if (surface.Quadrangular && transfiniteFourSided)
                        Gmsh.Model.Mesh.SetTransfiniteSurface(surface.Id, "AlternateLeft");
                    //
                    if (recombine && surface.Recombine) Gmsh.Model.Mesh.SetRecombine(2, surface.Id);
                    Gmsh.Model.Mesh.SetSmoothing(2, surface.Id, 100);
                }
            }
            //
            foreach (var entry in volumeIdVolume)
            {
                if (entry.Value.Transfinite) Gmsh.Model.Mesh.SetTransfiniteVolume(entry.Key);
                //Gmsh.Mesh.SetOutwardOrientation(volumeDimTag.Item2);
            }
        }
        // Tools                                                                                                                    
        private void GetOccNormalsBackground()
        {
            try
            {
                if (_gmshData.Coor == null || _gmshData.Coor.Length == 0) return;
                //
                Tuple<int, int>[] outDimTags;
                Gmsh.Model.OCC.ImportShapes(_gmshData.GeometryFileName, out outDimTags, false, "");
                //
                Synchronize(); // must be here
                //
                double[] concatenatedCoor = new double[3 * _gmshData.Coor.Length];
                for (int i = 0; i < _gmshData.Coor.Length; i++) Array.Copy(_gmshData.Coor[i], 0, concatenatedCoor, 3 * i, 3);
                double[] concatenatedParametricCoord;
                Gmsh.Model.GetParametrization(2, 1, concatenatedCoor, out concatenatedParametricCoord);
                double[] concatenatedNormals;
                Gmsh.Model.GetNormal(1, concatenatedParametricCoord, out concatenatedNormals);
                //
                _gmshData.Normals = new double[concatenatedNormals.Length / 3][];
                for (int i = 0; i < _gmshData.Normals.Length; i ++)
                {
                    _gmshData.Normals[i] = new double[3];
                    Array.Copy(concatenatedNormals, 3 * i, _gmshData.Normals[i], 0, 3);
                }
                //
                _error = null;
            }
            catch (Exception ex)
            {
                _error = ex.Message;
            }
        }
        private void Synchronize()
        {
            if (_isOCC) Gmsh.Model.OCC.Synchronize();
            else Gmsh.Model.Geo.Synchronize();
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
    }
}
