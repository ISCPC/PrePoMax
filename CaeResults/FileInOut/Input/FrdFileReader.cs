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
    [Serializable]
    public static class FrdFileReader
    {
        // Methods                                                                                                                  
        static public FeResults Read(string fileName)
        {
            if (fileName != null && File.Exists(fileName))
            {
                List<string> lines = new List<string>();

                if (!CaeGlobals.Tools.WaitForFileToUnlock(fileName, 5000)) return null;

                using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    while (!streamReader.EndOfStream) lines.Add(streamReader.ReadLine()); // faster than streamReader.ReadToEnd().Split ...

                    streamReader.Close();
                    fileStream.Close();
                }
                if (lines.Count < 5) return null;

                List<string[]> dataSets = GetDataSets(lines.ToArray());

                string setID;
                Dictionary<int, FeNode> nodes = null;
                Dictionary<int, int> nodeIdsLookUp = null;
                Dictionary<int, FeElement> elements = null;

                FeResults result = new FeResults(fileName);
                Field field;
                FieldData fieldData = null;
                FieldData prevFieldData = null;

                bool constantWidth = true;

                foreach (string[] dataSet in dataSets)
                {
                    setID = dataSet[0];
                    if (setID.StartsWith("    1C"))  // Data
                    {
                        result.DateTime = GetDateTime(dataSet);
                    }
                    else if (setID.StartsWith("    2C")) // Nodes
                    {
                        constantWidth = IsConstantWidth(dataSet);
                        nodes = GetNodes(dataSet, constantWidth, out nodeIdsLookUp);
                    }
                    else if (setID.StartsWith("    3C")) // Elements
                    {
                        elements = GetElements(dataSet);
                        //elements = GetElementsFast(dataSet));
                    }
                    else if (setID.StartsWith("    1PSTEP")) // Fields
                    {
                        GetField(dataSet, constantWidth, prevFieldData, out fieldData, out field);
                        result.AddFiled(fieldData, field);
                        prevFieldData = fieldData.DeepClone();
                    }
                }

                FeMesh mesh = new FeMesh(nodes, elements, MeshRepresentation.Results);
                mesh.ResetPartsColor();
                result.SetMesh(mesh, nodeIdsLookUp);

                return result;
            }

            return null;
        }
        static bool IsConstantWidth(string[] lines)
        {
            // sample frd lines
            // ccx 2.12 mkl
            // -1         1-1.35800E+03-3.18000E+02-9.99201E-13
            // -1      1300 7.15000E+02-3.19150E+02 8.66850E+02
            //
            // ccx 2.10 kwip
            // -1         1-1.35800E+03-3.18000E+02-9.99201E-13
            //
            // ccx 2.12 kwip
            // -1         1-1.35800E+03-3.18000E+02-9.99201E-13
            // -1      1300 7.15000E+02-3.19150E+02 8.66850E+02
            //
            // ccx 2.13 kwip
            // -1         1-1.35800E+03-3.18000E+02-9.99201E-13
            // -1      1300 7.15000E+02-3.19150E+02 8.66850E+02
            //
            // ccx 2.10 CalculiX-GE-OSS-2.10-win-x64
            // -1         1-1.35800E+003-3.18000E+002-9.99201E-013
            // -1      13007.15000E+002-3.19150E+0028.66850E+002
            // -1     160327.95006E+0024.15000E+0021.50000E+002
            //
            // line 0 is the line with the SetID, number of nodes ...
            //

            int len = lines[1].Length;
            for (int i = 2; i < lines.Length; i++)
            {
                if (lines[i].Length != len)
                    return false;
            }
            return true;


            int start = 13;
            string values = lines[1].Substring(start, lines[1].Length - start);
            if (values.Contains(' ')) return true;
            if (values[0] == ' ' && values[12] == ' ' && values[24] == ' ') return true;
            return false;
        }
        static private DateTime GetDateTime(string[] lines)
        {
            DateTime dateTime = new DateTime();
            DateTime time = new DateTime();
            string[] tmp;


            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("1UDATE"))
                {
                    tmp = lines[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    dateTime = DateTime.Parse(tmp[1]);
                    //dateTime = dateTime.AddHours(-12);
                }
                else if (lines[i].Contains("1UTIME"))
                {
                    tmp = lines[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    time = DateTime.Parse(tmp[1]);
                }
            }

            dateTime = dateTime.AddHours(time.Hour);
            dateTime = dateTime.AddMinutes(time.Minute);
            dateTime = dateTime.AddSeconds(time.Second);

            return dateTime;
        }
        static private List<string[]> GetDataSets(string[] lines)
        {
            List<string> dataSet = new List<string>();
            List<string[]> dataSets = new List<string[]>();

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] == " -3")
                {
                    dataSets.Add(dataSet.ToArray());
                    dataSet = new List<string>();
                }
                else if ((lines[i].StartsWith("    2C") || lines[i].StartsWith("    3C")) && dataSet.Count > 0)
                {
                    dataSets.Add(dataSet.ToArray());
                    dataSet = new List<string>();
                    dataSet.Add(lines[i]);
                }
                else
                    dataSet.Add(lines[i]);
            }

            return dataSets;
        }
        static private Dictionary<int, FeNode> GetNodes(string[] lines, bool constantWidth, out Dictionary<int, int> nodeIdsLookUp)
        {
            Dictionary<int, FeNode> nodes = new Dictionary<int, FeNode>();
            nodeIdsLookUp = new Dictionary<int, int>();
            int id;
            FeNode node;
            int width;
            string[] splitter = new string[] { " " };

            int start;

            if (constantWidth)
            {
                width = 12;
                // line 0 is the line with the SetID, number of nodes ...
                for (int i = 1; i < lines.Length; i++)
                {
                    start = 3;
                    id = int.Parse(lines[i].Substring(start, 10));
                    start += 10;
                    node = new FeNode();
                    node.Id = id;
                    node.X = double.Parse(lines[i].Substring(start, width));
                    start += width;
                    node.Y = double.Parse(lines[i].Substring(start, width));
                    start += width;
                    node.Z = double.Parse(lines[i].Substring(start, width));

                    nodes.Add(id, node);
                    nodeIdsLookUp.Add(id, i - 1);
                }
            }
            else
            {
                // line 0 is the line with the SetID, number of nodes ...
                for (int i = 1; i < lines.Length; i++)
                {
                    start = 3;
                    id = int.Parse(lines[i].Substring(start, 10));
                    start += 10;
                    node = new FeNode();
                    node.Id = id;

                    if (lines[i][start] == '-') width = 13;
                    else width = 12;
                    node.X = double.Parse(lines[i].Substring(start, width));
                    start += width;

                    if (lines[i][start] == '-') width = 13;
                    else width = 12;
                    node.Y = double.Parse(lines[i].Substring(start, width));
                    start += width;

                    if (lines[i][start] == '-') width = 13;
                    else width = 12;
                    node.Z = double.Parse(lines[i].Substring(start, width));

                    nodes.Add(id, node);
                    nodeIdsLookUp.Add(id, i - 1);
                }
            }


            return nodes;
        }
        static private Dictionary<int, FeElement> GetElements(string[] lines)
        {
            Dictionary<int, FeElement> elements = new Dictionary<int, FeElement>();
            int id;
            FrdFeDescriptorId feDescriptorId;
            FeElement element;
            string[] record1;
            string[] record2;
            string[] splitter = new string[] { " " };

            // line 0 is the line with the SetID, number of elements
            for (int i = 1; i < lines.Length; i++)
            {
                record1 = lines[i].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                id = int.Parse(record1[1]);
                feDescriptorId = (FrdFeDescriptorId)int.Parse(record1[2]);

                switch (feDescriptorId)
                {
                    // LINEAR ELEMENTS                                                                                              

                    case FrdFeDescriptorId.SolidLinearTetrahedron:
                        // linear tetrahedron element
                        record1 = lines[++i].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                        element = GetLinearTetraElement(id, record1);
                        elements.Add(id, element);
                        break;
                    case FrdFeDescriptorId.SolidLinearWedge:
                        // linear wedge element
                        record1 = lines[++i].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                        element = GetLinearWedgeElement(id, record1);
                        elements.Add(id, element);
                        break;
                    case FrdFeDescriptorId.SolidLinearHexahedron:
                        // linear hexahedron element
                        record1 = lines[++i].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                        element = GetLinearHexaElement(id, record1);
                        elements.Add(id, element);
                        break;

                    // PARABOLIC ELEMENTS                                                                                           

                    case FrdFeDescriptorId.SolidParabolicTetrahedron:
                        // parabolic tetrahedron element
                        record1 = lines[++i].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                        element = GetParabolicTetraElement(id, record1);
                        elements.Add(id, element);
                        break;
                    case FrdFeDescriptorId.SolidParabolicWedge:
                        // parabolic wedge element
                        record1 = lines[++i].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                        record2 = lines[++i].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                        element = GetParabolicWedgeElement(id, record1, record2);
                        elements.Add(id, element);
                        break;
                    case FrdFeDescriptorId.SolidParabolicHexahedron:
                        // parabolic hexahedron element
                        record1 = lines[++i].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                        record2 = lines[++i].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                        element = GetParabolicHexaElement(id, record1, record2);
                        elements.Add(id, element);
                        break;
                    default:
                        throw new Exception("The element type " + feDescriptorId.ToString() + " is not supported.");
                }
            }

            return elements;
        }
        static private void GetField(string[] lines, bool constantWidth, FieldData prevFieldData, out FieldData fieldData, out Field field)
        {
            // STATIC STEP

            //                              field#  stepIncrement#       step# - COMMENTS
            //    1PSTEP                        25               1           2
            //             time        numOfvalues
            //  100CL  107 2.000000000         613                     0    7           1


            //    1PSTEP                        25           1           2               
            //  100CL  107 2.000000000         613                     0    7           1
            // -4  DISP        4    1                                                    
            // -5  D1          1    2    1    0                                          
            // -5  D2          1    2    2    0                                          
            // -5  D3          1    2    3    0                                          
            // -5  ALL         1    2    0    0    1ALL                                  
            // -1         1-9.27159E-04 1.48271E-04-1.46752E-04                          

            // MODAL STEP

            //    1PSTEP                         5           1           2          
            //    1PGM                1.000000E+00                                  
            //    1PGK                7.155923E+05                                  
            //    1PHID                         -1                                  
            //    1PSUBC                         0                                  
            //    1PMODE                         1                                   - frequency
            //  100CL  102 1.34633E+02        1329                     2    2MODAL      1
            // -4  DISP        4    1
            // -5  D1          1    2    1    0
            // -5  D2          1    2    2    0
            // -5  D3          1    2    3    0
            // -5  ALL         1    2    0    0    1ALL

            // BUCKLING STEP

            //                              field#  stepIncrement#       step# - COMMENTS
            //    1PSTEP                         4           1           1
            //  100CL  104  93.9434614        1329                     4    4           1
            //  -4  DISP        4    1
            //  -5  D1          1    2    1    0
            //  -5  D2          1    2    2    0
            //  -5  D3          1    2    3    0
            //  -5  ALL         1    2    0    0    1ALL

            int userDefinedBlockId = -1;
            float time;
            int stepId;
            int stepIncrementId;
            bool modal = false;
            bool buckling = false;

            string[] record;
            string[] splitter = new string[] { " " };

            int n;
            string name;
            List<string> components = new List<string>();
            
            float[][] values;

            int lineNum = 0;
            record = lines[lineNum++].Split(splitter, StringSplitOptions.RemoveEmptyEntries);       // 1PSTEP
            stepId = int.Parse(record[3]);
            stepIncrementId = int.Parse(record[2]);

            // find 100C line - user field data
            while (!lines[lineNum].TrimStart().StartsWith("100C")) lineNum++;
            record = lines[lineNum++].Split(splitter, StringSplitOptions.RemoveEmptyEntries);       // 100CL
            userDefinedBlockId = int.Parse(record[1]);
            time = float.Parse(record[2]);
            n = int.Parse(record[3]);
            buckling = int.Parse(record[4]) == 4;                                                   // buckling switch

            if (buckling) 
            {
                if (prevFieldData == null) // this is the first step
                {
                    stepId = 1;
                    stepIncrementId = 1;
                }
                else if (prevFieldData.Buckling == false) // this is the first increment in the buckling step
                {
                    stepId = prevFieldData.StepId + 1;
                    stepIncrementId = 1;
                }
                else if (userDefinedBlockId != prevFieldData.UserDefinedBlockId) // this is the new increment in the buckling step
                {
                    stepId = prevFieldData.StepId;
                    stepIncrementId = prevFieldData.StepIncrementId + 1;
                }
                else // this is the same increment
                {
                    stepId = prevFieldData.StepId;
                    stepIncrementId = prevFieldData.StepIncrementId;
                }
            }
            // check for modal analysis
            else if (record.Length > 5 && record[5].Contains("MODAL"))
            {
                modal = true;
                record = lines[lineNum - 2].Split(splitter, StringSplitOptions.RemoveEmptyEntries); // 1PMODE
                int freqNum = int.Parse(record[1]);
                stepIncrementId = freqNum;
            }

            record = lines[lineNum++].Split(splitter, StringSplitOptions.RemoveEmptyEntries);       // -4  DISP
            name = record[1];

            while (true)
            {
                record = lines[lineNum++].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                if (record[0] == "-5") components.Add(record[1]);
                else break;
            }
            lineNum--;

            // check if the line number equals the numebr of lines
            if (lines.Length - lineNum != n) n = lines.Length - lineNum;

            values = new float[components.Count][];
            for (int i = 0; i < values.GetLength(0); i++) values[i] = new float[n];

            int start;
            int width;

            if (constantWidth)
            {
                width = 12;
                for (int i = 0; i < n; i++)
                {
                    start = 13;
                    for (int j = 0; j < components.Count; j++)
                    {
                        if (start + width > lines[i + lineNum].Length) continue;

                        values[j][i] = float.Parse(lines[i + lineNum].Substring(start, width));
                        start += width;
                    }
                }
            }
            else
            {
                // -1         12.55297E-0036.74788E-003-2.86735E-001
                // -1         29.68149E-0045.40225E-003-2.82345E-001
                // -1         32.35261E-0031.24270E-002-2.97771E-001
                // -1         4-9.84216E-0035.62909E-003-2.73047E-001
                // -1         53.91981E-0031.22846E-002-2.98849E-001
                // -1         6-1.16413E-0027.22403E-003-2.74940E-001
                for (int i = 0; i < n; i++)
                {
                    start = 13;
                    for (int j = 0; j < components.Count; j++)
                    {
                        if (start >= lines[i + lineNum].Length) continue;

                        if (lines[i + lineNum][start] == '-') width = 13;
                        else width = 12;

                        if (start + width > lines[i + lineNum].Length) continue;

                        values[j][i] = float.Parse(lines[i + lineNum].Substring(start, width));
                        start += width;
                    }
                }
            }

            fieldData = new FieldData(name);
            // component ??
            fieldData.UserDefinedBlockId = userDefinedBlockId;
            fieldData.Time = time;
            fieldData.StepId = stepId;
            fieldData.StepIncrementId = stepIncrementId;
            fieldData.Modal = modal;
            fieldData.Buckling = buckling;

            switch (name)
            {
                case "DISP":
                    field = CreateVectorField(name, components, values);
                    break;
                case "FORC":
                    field = CreateVectorField(name, components, values);
                    break;
                case "STRESS":
                    field = CreateStressField(name, components, values);
                    break;
                default:
                    field = new Field(name);
                    for (int i = 0; i < components.Count; i++)
                    {
                        field.AddComponent(components[i], values[i]);
                    }
                    break;
            }
        }
        static private Field CreateVectorField(string name, List<string> components, float[][] values)
        {
            Field field = new Field(name);

            float[] magnitude = new float[values[0].Length];
            for (int i = 0; i < magnitude.Length; i++)
            {
                magnitude[i] = (float)Math.Sqrt(Math.Pow(values[0][i], 2) + Math.Pow(values[1][i], 2) + Math.Pow(values[2][i], 2));
            }
            field.AddComponent("ALL", magnitude, true);

            for (int i = 0; i < components.Count; i++)
            {
                // Field ALL is allready added
                if (components[i] != "ALL") field.AddComponent(components[i], values[i]);
            }

            return field;
        }
        static private Field CreateStressField(string name, List<string> components, float[][] values)
        {
            // https://en.wikiversity.org/wiki/Principal_stresses

            Field field = new Field(name);

            if (values.Length == 6)
            {
                float[] vonMises = new float[values[0].Length];
                for (int i = 0; i < vonMises.Length; i++)
                {
                    vonMises[i] = (float)Math.Sqrt(0.5f * (
                                                             Math.Pow(values[0][i] - values[1][i], 2)
                                                           + Math.Pow(values[1][i] - values[2][i], 2)
                                                           + Math.Pow(values[2][i] - values[0][i], 2)
                                                           + 6 * (
                                                                    Math.Pow(values[3][i], 2)
                                                                  + Math.Pow(values[4][i], 2)
                                                                  + Math.Pow(values[5][i], 2)
                                                                  )
                                                           )
                                                  );
                }
                field.AddComponent("MISES", vonMises, true);
            }

            for (int i = 0; i < components.Count; i++)
            {
                field.AddComponent(components[i], values[i]);
            }

            if (values.Length == 6) ComputeAndAddPrincipalStresses(field, values);

            return field;
        }
        static private void ComputeAndAddPrincipalStresses(Field field, float[][] values)
        {
            // https://en.wikipedia.org/wiki/Cubic_function#General_solution_to_the_cubic_equation_with_arbitrary_coefficients
            // https://en.wikiversity.org/wiki/Principal_stresses

            float[] s0 = new float[values[0].Length];
            float[] s1 = new float[values[0].Length];
            float[] s2 = new float[values[0].Length];
            float[] s3 = new float[values[0].Length];

            float s11;
            float s22;
            float s33;
            float s12;
            float s23;
            float s31;

            double I1;
            double I2;
            double I3;

            double sp1, sp2, sp3;
            sp1 = sp2 = sp3 = 0;

            for (int i = 0; i < s1.Length; i++)
            {
                s11 = values[0][i];
                s22 = values[1][i];
                s33 = values[2][i];
                s12 = values[3][i];
                s23 = values[4][i];
                s31 = values[5][i];

                I1 = s11 + s22 + s33;
                I2 = s11 * s22 + s22 * s33 + s33 * s11 - Math.Pow(s12, 2.0) - Math.Pow(s23, 2.0) - Math.Pow(s31, 2.0);
                I3 = s11 * s22 * s33 - s11 * Math.Pow(s23, 2.0) - s22 * Math.Pow(s31, 2.0) - s33 * Math.Pow(s12, 2.0) + 2.0 * s12 * s23 * s31;

                CaeGlobals.Tools.SolveQubicEquationDepressedCubic(1.0, -I1, I2, -I3, ref sp1, ref sp2, ref sp3);
                CaeGlobals.Tools.Sort3_descending(ref sp1, ref sp2, ref sp3);

                s0[i] = Math.Abs(sp1) > Math.Abs(sp3) ? (float)sp1 : (float)sp3;
                s1[i] = (float)sp1;
                s2[i] = (float)sp2;
                s3[i] = (float)sp3;

                if (float.IsNaN(s0[i])) s0[i] = 0;
                if (float.IsNaN(s1[i])) s1[i] = 0;
                if (float.IsNaN(s2[i])) s2[i] = 0;
                if (float.IsNaN(s3[i])) s3[i] = 0;
            }

            field.AddComponent("SGN-MAX-ABS-PRI", s0, true);
            field.AddComponent("PRINCIPAL-MAX", s1, true);
            field.AddComponent("PRINCIPAL-MID", s2, true);
            field.AddComponent("PRINCIPAL-MIN", s3, true);
        }


        // LINEAR ELEMENTS                                                                                              
        static private LinearTetraElement GetLinearTetraElement(int id, string[] record)
        {
            int[] nodes = new int[4];
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i] = int.Parse(record[i + 1]);
            }
            return new LinearTetraElement(id, nodes);
        }
        static private LinearWedgeElement GetLinearWedgeElement(int id, string[] record)
        {
            int[] nodes = new int[6];
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i] = int.Parse(record[i + 1]);
            }
            return new LinearWedgeElement(id, nodes);
        }
        static private LinearHexaElement GetLinearHexaElement(int id, string[] record)
        {
            int[] nodes = new int[8];

            nodes[0] = int.Parse(record[1]);
            nodes[1] = int.Parse(record[2]);
            nodes[2] = int.Parse(record[3]);
            nodes[3] = int.Parse(record[4]);
            nodes[4] = int.Parse(record[5]);
            nodes[5] = int.Parse(record[6]);
            nodes[6] = int.Parse(record[7]);
            nodes[7] = int.Parse(record[8]);

            return new LinearHexaElement(id, nodes);
        }


        // PARABOLIC ELEMENTS                                                                                           
        static private ParabolicTetraElement GetParabolicTetraElement(int id, string[] record)
        {
            int[] nodes = new int[10];
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i] = int.Parse(record[i + 1]);
            }
            return new ParabolicTetraElement(id, nodes);
        }
        static private ParabolicWedgeElement GetParabolicWedgeElement(int id, string[] record1, string[] record2)
        {
            int[] nodes = new int[15];
            for (int i = 0; i < 9; i++) // first 9 nodes are OK
            {
                nodes[i] = int.Parse(record1[i + 1]);
            }

            // swap last 3 nodes with forelast 3 nodes
            nodes[9] = int.Parse(record2[3]);
            nodes[10] = int.Parse(record2[4]);
            nodes[11] = int.Parse(record2[5]);
            nodes[12] = int.Parse(record1[10]);
            nodes[13] = int.Parse(record2[1]);
            nodes[14] = int.Parse(record2[2]);

            return new ParabolicWedgeElement(id, nodes);
        }
        static private ParabolicHexaElement GetParabolicHexaElement(int id, string[] record1, string[] record2)
        {
            int[] nodes = new int[20];

            // first 12 nodes are OK
            for (int i = 0; i < 10; i++)
            {
                nodes[i] = int.Parse(record1[i + 1]);
            }
            nodes[10] = int.Parse(record2[1]);
            nodes[11] = int.Parse(record2[2]);
            
            // swap last 3 nodes with forelast 3 nodes
            nodes[12] = int.Parse(record2[7]);
            nodes[13] = int.Parse(record2[8]);
            nodes[14] = int.Parse(record2[9]);
            nodes[15] = int.Parse(record2[10]);

            nodes[16] = int.Parse(record2[3]);
            nodes[17] = int.Parse(record2[4]);
            nodes[18] = int.Parse(record2[5]);
            nodes[19] = int.Parse(record2[6]);

            return new ParabolicHexaElement(id, nodes);
        }

        //                                                                                                          


    }
}
