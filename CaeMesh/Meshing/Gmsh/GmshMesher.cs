using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CaeMesh.Meshing;
using GmshCommon;
using static GmshCommon.Gmsh;

namespace CaeMesh
{
    public static class GmshMesher
    {
        // Variables                                                                                                                


        public static string CreateMesh(string brepFileName, string inpFileName, MeshingParameters partMeshingParameters,
                                        Dictionary<int, double> vertexIdMeshSize, MeshSetupItem[] gmshSetupItems,
                                        Action<string> writeOutput, bool preview)
        {
            try
            {
                Gmsh.InitializeGmsh();
                Gmsh.Add("Brep_model");
                Gmsh.Logger.Start();
                //
                if (gmshSetupItems.Length != 1)
                    throw new CaeGlobals.CaeException("Currently, for a single part, only one mesh setup item of the type: "+
                        "Tetrahedral gmsh, Extrude mesh or Revolve mesh is possible.");
                //
                MeshSetupItem meshSetupItem = gmshSetupItems[0];
                if (meshSetupItem is GmshSetupItem gsi)
                {
                    bool recombine = gsi.AlgorithmRecombine != GmshAlgorithmRecombineEnum.None;
                    //
                    Tuple<int, int>[] outDimTags;
                    Gmsh.OCC.ImportShapes(brepFileName, out outDimTags, false, "");
                    //
                    Gmsh.OCC.Synchronize(); // must be here
                    // Mesh size
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
                    Gmsh.OCC.Synchronize(); // must be here for mesh refinement
                    // 2D meshing algorithm
                    Gmsh.SetNumber("Mesh.Algorithm", (int)gsi.AlgorithmMesh2D);
                    // 3D meshing algorithm
                    Gmsh.SetNumber("Mesh.Algorithm3D", (int)gsi.AlgorithmMesh3D);
                    // Recombine
                    if (recombine)
                    {
                        // Set recombine algorithm
                        Gmsh.SetNumber("Mesh.RecombinationAlgorithm", (int)gsi.AlgorithmRecombine);
                        // Recombine quality
                        Gmsh.SetNumber("Mesh.RecombineMinimumQuality", gsi.RecombineMinQuality);
                    }
                    // Transfinite
                    if (gsi.Transfinite && gsi.AlgorithmMesh2D != GmshAlgorithmMesh2DEnum.BAMG)
                        Gmsh.Mesh.SetTransfiniteAutomatic(gsi.TransfiniteAngleRad);
                    //
                    if (meshSetupItem is ShellGmsh)
                        ShellGmsh(gsi, partMeshingParameters, preview);
                    else if (meshSetupItem is TetrahedralGmsh)
                        TetrahedralGmsh(gsi, partMeshingParameters, preview);
                    else if (meshSetupItem is TransfiniteMesh)
                        TransfiniteMesh(gsi, partMeshingParameters, preview);
                    else if (meshSetupItem is ExtrudeMesh || meshSetupItem is RevolveMesh)
                        ExtrudeRevolveMesh(gsi, partMeshingParameters, preview);
                    else throw new NotSupportedException("MeshSetupItemTypeException");
                }
                else throw new NotSupportedException("MeshSetupItemTypeException");

                // Gmsh GEO tutorial 11
                //Gmsh.Mesh.Recombine();
                //Gmsh.SetNumber("Mesh.SubdivisionAlgorithm", 1);
                //Gmsh.Mesh.Refine();

                // Element order
                if (!preview && partMeshingParameters.SecondOrder)
                {
                    if (!partMeshingParameters.MidsideNodesOnGeometry) Gmsh.SetNumber("Mesh.SecondOrderLinear", 1); // first
                    Gmsh.SetNumber("Mesh.HighOrderOptimize", 1);                                                // second
                    Gmsh.SetNumber("Mesh.SecondOrderIncomplete", 1);                                            // second
                    Gmsh.Mesh.SetOrder(2);                                                                      // third
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
                return null;
            }
            catch (Exception ex)
            {
                string error;
                if (ex.Message.StartsWith("External component"))
                {
                    error = "Gmsh error" + Environment.NewLine;
                    string[] loggerLines = Gmsh.Logger.Get();
                    int numLines = loggerLines.Length > 5 ? 5 : loggerLines.Length;
                    //
                    for (int i = loggerLines.Length - numLines; i < loggerLines.Length; i++)
                        error += "Line " + i + ": " + loggerLines[i] + Environment.NewLine;
                }
                else
                {
                    error = ex.Message;
                }
                //
                writeOutput?.Invoke(error);
                return error;
            }
            finally
            {
                Gmsh.Logger.Stop();
                Gmsh.FinalizeGmsh();
            }
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
                    Gmsh.Geo.Extrude(extrudeDimTags, extrudeMesh.Direction[0], extrudeMesh.Direction[1], extrudeMesh.Direction[2],
                                     out outDimTags, numElements, height, true);
                }
                // Revolve
                else if (revolveMesh != null)
                {
                    Gmsh.Geo.Revolve(extrudeDimTags, revolveMesh.AxisCenter[0], revolveMesh.AxisCenter[1], revolveMesh.AxisCenter[2],
                                     revolveMesh.AxisDirection[0], revolveMesh.AxisDirection[1], revolveMesh.AxisDirection[2],
                                     revolveMesh.AngleDeg * Math.PI / 180,
                                     out outDimTags, numElements, height, true);
                }
                else throw new NotSupportedException();
                // Volume check 
                double volumeOut;
                double initialVolume = 0;
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
                //if (Math.Abs(initialVolume - extrudedVolume) > 1E-2 * maxVolume)
                //    throw new CaeGlobals.CaeException("The volume of the extruded mesh is not equal to the volume of the geometry " +
                //        "it represents. Try selecting other surfaces for the extrusion.");
                //
                Gmsh.OCC.Remove(new Tuple<int, int>[] { new Tuple<int, int>(3, 1) }, true);
                //
                Gmsh.OCC.Synchronize(); // must be here
                //
                Gmsh.Generate(3);
            }
            //
            //
            //Tuple<int, int>[] removeDimTags = new Tuple<int, int>[] { new Tuple<int, int>(3, 2), new Tuple<int, int>(3, 3) };
            //Gmsh.OCC.RemoveAllDuplicates();
            //Gmsh.Mesh.RemoveDuplicateNodes(removeDimTags);
            //Gmsh.Mesh.RemoveDuplicateElements();
            //Gmsh.Mesh.RemoveDuplicateNodes();
        }
        //
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
        private static Dictionary<int,double> GetPointIdElementSize(FeMeshRefinement[] partMeshRefinements)
        {



            return null;
        }
        private static void SetTransfiniteSurfaces()
        {
            int[] volumeIds;
            int[] surfaceIds;
            int[] edgeIds;
            int[] nodeIds;
            //
            Tuple<int, int>[] surfaceDimTags;
            HashSet<int> vertexIds = new HashSet<int>();
            //
            Gmsh.GetEntities(out surfaceDimTags, 2);
            //
            foreach (var surfaceDimTag in surfaceDimTags)
            {
                Gmsh.GetAdjacencies(surfaceDimTag.Item1, surfaceDimTag.Item2, out volumeIds, out edgeIds);
                //
                vertexIds.Clear();
                foreach (var edgeId in edgeIds)
                {
                    Gmsh.GetAdjacencies(1, edgeId, out surfaceIds, out nodeIds);
                    vertexIds.UnionWith(nodeIds);
                }
                //
                if (vertexIds.Count == 3 || vertexIds.Count == 4)
                {
                    Gmsh.Mesh.SetTransfiniteSurface(surfaceDimTag.Item2, "Left", vertexIds.ToArray());
                }
            }
        }
        private static void SetTransfiniteSurfaces(Tuple<int, int>[] surfaceDimTags)
        {
            int[] volumeIds;
            int[] surfaceIds;
            int[] edgeIds;
            int[] nodeIds;
            //
            HashSet<int> vertexIds = new HashSet<int>();
            //
            Gmsh.GetEntities(out surfaceDimTags, 2);
            //
            foreach (var surfaceDimTag in surfaceDimTags)
            {
                Gmsh.GetAdjacencies(surfaceDimTag.Item1, surfaceDimTag.Item2, out volumeIds, out edgeIds);
                //
                vertexIds.Clear();
                foreach (var edgeId in edgeIds)
                {
                    Gmsh.GetAdjacencies(1, edgeId, out surfaceIds, out nodeIds);
                    vertexIds.UnionWith(nodeIds);
                }
                //
                if (vertexIds.Count == 3 || vertexIds.Count == 4)
                {
                    Gmsh.Mesh.SetTransfiniteSurface(surfaceDimTag.Item2, "Left", vertexIds.ToArray());
                }
            }
        }
    }
}
