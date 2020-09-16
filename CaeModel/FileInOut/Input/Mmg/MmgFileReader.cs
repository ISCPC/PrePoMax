using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CaeMesh;

namespace FileInOut.Input
{
    static public class MmgFileReader
    {
        public static FeMesh Read(string fileName)
        {
            if (File.Exists(fileName))
            {
                FeNode node;
                LinearTriangleElement triangle;
                Dictionary<int, FeNode> nodes = new Dictionary<int, FeNode>();
                Dictionary<int, FeElement> elements = new Dictionary<int, FeElement>();
                //
                int numOfNodes;
                int numOfElements;
                string[] splitter = new string[] { " " };
                string[] tmp;
                string[] lines = File.ReadAllLines(fileName);
                //
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].ToUpper().StartsWith("VERTICES"))
                    {
                        i++;
                        if (i < lines.Length)
                        {
                            numOfNodes = int.Parse(lines[i]);
                            i++;
                            for (int j = 0; j < numOfNodes && i + j < lines.Length; j++)
                            {
                                tmp = lines[i + j].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                                if (tmp.Length >= 3)
                                {
                                    node = new FeNode();
                                    node.Id = j + 1;
                                    node.X = double.Parse(tmp[0]);
                                    node.Y = double.Parse(tmp[1]);
                                    node.Z = double.Parse(tmp[2]);
                                    nodes.Add(node.Id, node);
                                }
                            }
                        }
                    }
                    else if (lines[i].ToUpper().StartsWith("TRIANGLES"))
                    {
                        i++;
                        if (i < lines.Length)
                        {
                            numOfElements = int.Parse(lines[i]);
                            i++;
                            for (int j = 0; j < numOfElements && i + j < lines.Length; j++)
                            {
                                tmp = lines[i + j].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                                if (tmp.Length >= 3)
                                {
                                    triangle = new LinearTriangleElement(j + 1, new int[3]);
                                    triangle.NodeIds[0] = int.Parse(tmp[0]);
                                    triangle.NodeIds[1] = int.Parse(tmp[1]);
                                    triangle.NodeIds[2] = int.Parse(tmp[2]);
                                    elements.Add(triangle.Id, triangle);
                                }
                            }
                        }
                    }
                }
                //
                FeMesh mesh = new FeMesh(nodes, elements, MeshRepresentation.Mesh, null, null, false,
                                         ImportOptions.None);
                //
                return mesh;
            }
            //
            return null;
        }

    }
}
