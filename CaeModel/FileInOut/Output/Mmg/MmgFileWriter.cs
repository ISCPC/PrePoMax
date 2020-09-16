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
                if (!(part is GeometryPart)) throw new CaeException("Mmg export only supports geometry parts.");
                numOfVertices += part.NodeLabels.Length;
                numOfTriangles += part.Labels.Length;
            }
            //
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("MeshVersionFormatted 1");
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
            Dictionary<int, int> oldIdNewIdMap = new Dictionary<int, int>();
            //
            count = 1;
            foreach (var partName in partNames)
            {
                part = mesh.Parts[partName];
                //
                foreach (var nodeId in part.NodeLabels)
                {
                    node = mesh.Nodes[nodeId];
                    coor = node.Coor;
                    sb.AppendFormat("{0} {1} {2} {3}{4}", coor[0], coor[1], coor[2], "0", Environment.NewLine);
                    oldIdNewIdMap.Add(node.Id, count);
                    count++;
                }
            }
            // Triangles
            sb.AppendLine("Triangles");
            sb.AppendLine(numOfTriangles.ToString());
            count = 1;
            foreach (var partName in partNames)
            {
                part = mesh.Parts[partName];
                //
                foreach (var elementId in part.Labels)
                {
                    element = mesh.Elements[elementId];
                    nodeIds = element.NodeIds;
                    sb.AppendFormat("{0} {1} {2} {3}{4}", oldIdNewIdMap[nodeIds[0]],
                                                          oldIdNewIdMap[nodeIds[1]],
                                                          oldIdNewIdMap[nodeIds[2]],
                                                          count, // part id
                                                          Environment.NewLine);
                }
                count++;
            }
            // End
            sb.AppendLine("End");
            //
            File.WriteAllText(fileName, sb.ToString());
        }
    }
}
