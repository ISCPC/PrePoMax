using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CaeMesh;
using QuantumConcepts.Formats.StereoLithography;

namespace FileInOut.Output
{
    static public class StlFileWriter
    {
        public static void Write(string fileName, FeMesh mesh, string[] partNames)
        {
            List<Facet> facets = new List<Facet>();
            Normal normal;
            FeElement element;
            int[] nodeIds;
            double[] coor;
            //
            foreach (var partName in partNames)
            {
                foreach (int elementId in mesh.Parts[partName].Labels)
                {
                    element = mesh.Elements[elementId];
                    if (element is LinearTriangleElement)
                    {
                        Vertex[] vertices = new Vertex[3];
                        nodeIds = element.NodeIds;
                        for (int i = 0; i < nodeIds.Length; i++)
                        {
                            coor = mesh.Nodes[nodeIds[i]].Coor;
                            vertices[i] = new Vertex((float)coor[0], (float)coor[1], (float)coor[2]);
                        }
                        normal = ComputeNormal(vertices);
                        facets.Add(new Facet(normal, vertices, 0));
                    }
                }
            }
            //
            STLDocument stlFile = new STLDocument("part", facets);
            stlFile.SaveAsText(fileName);
            //
            //using (Stream stream = File.OpenWrite(fileName))
            //{
            //   stlFile.WriteBinary(stream);
            //}
        }

        public static void Write(string fileName, List<double[][]> stlTriangles)
        {
            List<Facet> facets = new List<Facet>();
            Normal normal;
            //
            foreach (var triangle in stlTriangles)
            {
                Vertex[] vertices = new Vertex[3];
                for (int i = 0; i < triangle.Length; i++)
                {
                    vertices[i] = new Vertex((float)triangle[i][0], (float)triangle[i][1], (float)triangle[i][2]);
                }
                normal = ComputeNormal(vertices);
                facets.Add(new Facet(normal, vertices, 0));
                Facet f = new Facet();
            }
            //
            STLDocument stlFile = new STLDocument("part", facets);
            stlFile.SaveAsText(fileName);
            //
            //using (Stream stream = File.OpenWrite(fileName))
            //{
            //   stlFile.WriteBinary(stream);
            //}
        }
        //
        private static Normal ComputeNormal(Vertex[] vertices)
        {
            float[] v1 = new float[] { vertices[1].X - vertices[0].X, vertices[1].Y - vertices[0].Y, vertices[1].Z - vertices[0].Z };
            float[] v2 = new float[] { vertices[2].X - vertices[0].X, vertices[2].Y - vertices[0].Y, vertices[2].Z - vertices[0].Z };

            Normal normal = new Normal(v1[1] * v2[2] - v1[2] * v2[1], -(v1[0] * v2[2] - v1[2] * v2[0]), v1[0] * v2[1] - v1[1] * v2[0]);
            float len = (float)Math.Sqrt(Math.Pow(normal.X, 2) + Math.Pow(normal.Y, 2) + Math.Pow(normal.Z, 2));

            if (len > 0)
            {
                normal.X /= len;
                normal.Y /= len;
                normal.Z /= len;
            }

            return normal;
        }
    }
}
