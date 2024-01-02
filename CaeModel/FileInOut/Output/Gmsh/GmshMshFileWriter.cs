using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CaeMesh;
using CaeGlobals;
using CaeModel;
using System.Xml.Linq;

namespace FileInOut.Output
{
    //                                                              //
    //  https://gmsh.info//doc/texinfo/gmsh.html#Gmsh-file-formats  //
    //                                                              //
    static public class GmshMshFileWriter
    {
        private static Dictionary<Type, int> typeMap = new Dictionary<Type, int>()
        {
            { typeof(LinearBeamElement), 1 },
            { typeof(ParabolicBeamElement), 8 },
            //
            { typeof(LinearTriangleElement), 2 },
            { typeof(ParabolicTriangleElement), 9 },
            { typeof(LinearQuadrilateralElement), 3 },
            { typeof(ParabolicQuadrilateralElement), 16 },
            //
            { typeof(LinearTetraElement), 4 },
            { typeof(ParabolicTetraElement), 11 },
            { typeof(LinearPyramidElement), 7 },
            { typeof(ParabolicPyramidElement), 19 },
            { typeof(LinearWedgeElement), 6 },
            { typeof(ParabolicWedgeElement), 18 },
            { typeof(LinearHexaElement), 5 },
            { typeof(ParabolicHexaElement), 17 },
        };
        private static Dictionary<Type, int> dimMap = new Dictionary<Type, int>()
        {
            { typeof(LinearBeamElement), 1 },
            { typeof(ParabolicBeamElement), 1 },
            //
            { typeof(LinearTriangleElement), 2 },
            { typeof(ParabolicTriangleElement), 2 },
            { typeof(LinearQuadrilateralElement), 2 },
            { typeof(ParabolicQuadrilateralElement), 2 },
            //
            { typeof(LinearTetraElement), 3 },
            { typeof(ParabolicTetraElement), 3 },
            { typeof(LinearPyramidElement), 3 },
            { typeof(ParabolicPyramidElement), 3 },
            { typeof(LinearWedgeElement), 3 },
            { typeof(ParabolicWedgeElement), 3 },
            { typeof(LinearHexaElement), 3 },
            { typeof(ParabolicHexaElement), 3 },
        };
        public static void Write(string fileName, FeMesh mesh)
        {
            // Split elements
            Type type;
            int[] nodeIds;
            HashSet<int> nodeIdsHash;
            List<FeElement> elements;
            Dictionary<Type, List<FeElement>> elementTypeElements = new Dictionary<Type, List<FeElement>>();
            Dictionary<int, HashSet<int>> dimNodeIds = new Dictionary<int, HashSet<int>>();
            foreach (var entry in mesh.Elements)
            {
                type = entry.Value.GetType();
                //
                if (dimNodeIds.TryGetValue(dimMap[type], out nodeIdsHash)) nodeIdsHash.UnionWith(entry.Value.NodeIds);
                else dimNodeIds.Add(dimMap[type], new HashSet<int>(entry.Value.NodeIds));
                //
                if (elementTypeElements.TryGetValue(type, out elements)) elements.Add(entry.Value);
                else elementTypeElements.Add(type, new List<FeElement> { entry.Value });
            }
            // Sort node ids per dimension
            int count = 0;
            int min = int.MaxValue;
            int max = -int.MaxValue;
            Dictionary<int, int[]> dimSortedNodeIds = new Dictionary<int, int[]>();
            foreach (var entry in dimNodeIds)
            {
                nodeIds = entry.Value.ToArray();
                Array.Sort(nodeIds);
                dimSortedNodeIds[entry.Key] = nodeIds;
                //
                count += nodeIds.Length;
                if (nodeIds[0] < min) min = nodeIds[0];
                if (nodeIds[nodeIds.Length - 1] > max) max = nodeIds[nodeIds.Length - 1];
            }
            // File
            StringBuilder sb = new StringBuilder();
            // Header
            sb.AppendLine("$MeshFormat");
            sb.AppendLine("4.1 0 8     MSH4.1, ASCII");
            sb.AppendLine("$EndMeshFormat");
            // Nodes
            sb.AppendLine("$Nodes");
            // 1 6 1 6     1 entity bloc, 6 nodes total, min/max node tags: 1 and 6
            sb.AppendFormat("{0} {1} {2} {3}{4}", dimSortedNodeIds.Count, count, min, max, Environment.NewLine);
            //
            foreach (var entry in dimSortedNodeIds)
            {
                // 2 1 0 6     2D entity(surface) 1, no parametric coordinates, 6 nodes
                sb.AppendFormat("{0} {1} {2} {3}{4}", entry.Key, 1, 0, entry.Value.Length, Environment.NewLine);
                //
                FeNode node;
                for (int i = 0; i < entry.Value.Length; i++)
                {
                    node = mesh.Nodes[entry.Value[i]];
                    sb.AppendFormat("{0}{1}", node.Id, Environment.NewLine);
                }
                for (int i = 0; i < entry.Value.Length; i++)
                {
                    node = mesh.Nodes[entry.Value[i]];
                    sb.AppendFormat("{0} {1} {2}{3}", node.X, node.Y, node.Z, Environment.NewLine);
                }
            }
            sb.AppendLine("$EndNodes");
            // Elements
            //$Elements
            //1 2 1 2     1 entity bloc, 2 elements total, min/ max element tags: 1 and 2
            //2 1 3 2     2D entity(surface) 1, element type 3(4 - node quad), 2 elements
            sb.AppendLine("$Elements");
            //
           
            //1 2 1 2     1 entity bloc, 2 elements total, min/ max element tags: 1 and 2
            sb.AppendFormat("{0} {1} {2} {3}{4}", elementTypeElements.Count, mesh.Elements.Count(), 1, mesh.MaxElementId,
                            Environment.NewLine);
            //
            foreach (var entry in elementTypeElements)
            {
                //2 1 3 2     2D entity(surface) 1, element type 3(4 - node quad), 2 elements
                sb.AppendFormat("{0} {1} {2} {3}{4}", dimMap[entry.Key], 1, typeMap[entry.Key], entry.Value.Count, Environment.NewLine);
                //
                foreach (var element in entry.Value)
                {
                    nodeIds = element.GetGmshNodeIds();
                    //
                    sb.Append(element.Id);
                    for (int i = 0; i < nodeIds.Length; i++)
                    {
                        sb.Append(" ");
                        sb.Append(nodeIds[i]);
                    }
                    sb.AppendLine();
                }
            }
            sb.AppendLine("$EndElements");
            //
            File.WriteAllText(fileName, sb.ToString());
        }
        
    }
}
