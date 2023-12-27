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
        public static void Write(string fileName, FeMesh mesh)
        {
            // File
            StringBuilder sb = new StringBuilder();
            // Header
            sb.AppendLine("$MeshFormat");
            sb.AppendLine("4.1 0 8     MSH4.1, ASCII");
            sb.AppendLine("$EndMeshFormat");
            // Nodes
            // $Nodes
            // 1 6 1 6     1 entity bloc, 6 nodes total, min/max node tags: 1 and 6
            // 2 1 0 6     2D entity(surface) 1, no parametric coordinates, 6 nodes
            sb.AppendLine("$Nodes");
            sb.AppendFormat("{0} {1} {2} {3}{4}", 1, mesh.Nodes.Count(), 1, mesh.MaxNodeId, Environment.NewLine);
            sb.AppendFormat("{0} {1} {2} {3}{4}", 2, 1, 0, mesh.Nodes.Count(), Environment.NewLine);
            //
            FeNode node;
            foreach (var entry in mesh.Nodes)
            {
                node = entry.Value;
                sb.AppendFormat("{0}{1}", node.Id, Environment.NewLine);
            }
            foreach (var entry in mesh.Nodes)
            {
                node = entry.Value;
                sb.AppendFormat("{0} {1} {2}{3}", node.X, node.Y, node.Z, Environment.NewLine);
            }
            sb.AppendLine("$EndNodes");
            // Elements
            //$Elements
            //1 2 1 2     1 entity bloc, 2 elements total, min/ max element tags: 1 and 2
            //2 1 3 2     2D entity(surface) 1, element type 3(4 - node quad), 2 elements
            sb.AppendLine("$Elements");
            
            //
            Type type;
            List<FeElement> elements;
            Dictionary<Type, List<FeElement>> elementTypeElements = new Dictionary<Type, List<FeElement>>();
            foreach (var entry in mesh.Elements)
            {
                type = entry.Value.GetType();
                //
                if (elementTypeElements.TryGetValue(type, out elements)) elements.Add(entry.Value);
                else elementTypeElements.Add(type, new List<FeElement> { entry.Value });
            }

            sb.AppendFormat("{0} {1} {2} {3}{4}", elementTypeElements.Count, mesh.Elements.Count(), 1, mesh.MaxElementId, Environment.NewLine);

            foreach (var entry in elementTypeElements)
            {
                if (entry.Key == typeof(LinearTriangleElement))
                {
                    sb.AppendFormat("{0} {1} {2} {3}{4}", 2, 1, 2, entry.Value.Count, Environment.NewLine);
                }
                else if (entry.Key == typeof(LinearQuadrilateralElement))
                {
                    sb.AppendFormat("{0} {1} {2} {3}{4}", 2, 1, 3, entry.Value.Count, Environment.NewLine);
                }
                else throw new NotSupportedException();
                //
                foreach (var element in entry.Value)
                {
                    sb.Append(element.Id);
                    for (int i = 0; i < element.NodeIds.Length; i++)
                    {
                        sb.Append(" ");
                        sb.Append(element.NodeIds[i]);
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
