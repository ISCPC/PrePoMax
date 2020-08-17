using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CaeMesh;
using CaeGlobals;
using CaeModel;
using FileInOut.Output.Calculix;

namespace FileInOut.Input
{
    [Serializable]
    static public class InpFileReader
    {
        // Variables                                                                                                                
        static private List<string> _errors;
        static string[] splitterComma = new string[] { "," };
        static string[] splitterEqual = new string[] { "=" };
        static string[] splitter = new string[] { " ", ",", "\t" };


        // Callbacks                                                                                                                
        static private Action<string> WriteDataToOutputStatic;


        // Properties                                                                                                               
        static public List<string> Errors { get { return _errors; } }


        // Methods                                                                                                                  
        static public void Read(string fileName, ElementsToImport elementsToImport, FeModel model, Action<string> WriteDataToOutput)
        {
            WriteDataToOutputStatic = WriteDataToOutput;
            _errors = new List<string>();

            if (fileName != null && File.Exists(fileName))
            {
                string[] lines = File.ReadAllLines(fileName);
                string[] splitter = new string[] { "," };

                string[] dataSet;
                string[][] dataSets = GetDataSets(lines);

                Dictionary<int, FeNode> nodes = null;
                Dictionary<int, FeElement> elements = new Dictionary<int, FeElement>();

                string name;
                int[] ids;
                List<InpElementSet> inpElementTypeSets = new List<InpElementSet>();

                string keyword;
                int lineNumber;
                // nodes and elements
                for (int i = 0; i < dataSets.Length; i++)
                {
                    dataSet = dataSets[i];
                    keyword = dataSet[0].Split(splitter, StringSplitOptions.RemoveEmptyEntries)[0].Trim().ToUpper();

                    if (keyword == "*NODE") // Nodes
                    {
                        WriteDataToOutputStatic("Reading keyword line: " + dataSet[0]);
                        nodes = GetNodes(dataSet);
                    }
                    else if (keyword == "*ELEMENT") // Elements
                    {
                        WriteDataToOutputStatic("Reading keyword line: " + dataSet[0]);
                        AddElements(dataSet, ref elements, ref inpElementTypeSets);
                    }
                }
                FeMesh mesh = new FeMesh(nodes, elements, MeshRepresentation.Mesh, inpElementTypeSets); // element sets are used to separate elements into parts

                // element sets from element keywords
                int[] labels;
                foreach (var inpElementTypeSet in inpElementTypeSets)
                {
                    if (inpElementTypeSet.Name != null) // name is null when no elset is defined by *Element keyword
                    {
                        labels = inpElementTypeSet.ElementLabels.ToArray();
                        mesh.AddElementSet(new FeElementSet(inpElementTypeSet.Name, labels));
                        //mesh.AddNodeSetFromElementSet(inpElementTypeSet.Name);
                    }
                }

                // node and element sets, surfaces, reference points
                lineNumber = 0;

                Dictionary<string, Constraint> constraints = new Dictionary<string, Constraint>();
                Dictionary<string, FeReferencePoint> referencePoints = new Dictionary<string, FeReferencePoint>();
                Dictionary<string, Material> materials = new Dictionary<string, Material>();


                // JAW 21 July 2020
                // Create object for Sections within the input file
                var sections = new Dictionary<string, SolidSection>();
                var secCounter = 0; // section counter for Section name
                // Create object for Steps within the input file
                var steps = new Dictionary<string, StaticStep>();
                // section counter for Section names
                var stepCounter = 0;
                // Boundary condition counter for Section names
                var bCCounter = 0;
                // End of JAW Mods

                for (int i = 0; i < dataSets.Length; i++)
                {
                    dataSet = dataSets[i];
                    keyword = dataSet[0].Split(splitter, StringSplitOptions.RemoveEmptyEntries)[0].Trim().ToUpper();

                    if (keyword == "*NSET")
                    {
                        WriteDataToOutputStatic("Reading keyword line: " + dataSet[0]);
                        GetNodeOrElementSet("NSET", dataSet, mesh, out name, out ids);
                        if (NamedClass.CheckNameError(name) != null) AddError(NamedClass.CheckNameError(name));
                        else if (ids != null) mesh.AddNodeSet(new FeNodeSet(name, ids));
                    }
                    else if (keyword == "*ELSET")
                    {
                        WriteDataToOutputStatic("Reading keyword line: " + dataSet[0]);
                        GetNodeOrElementSet("ELSET", dataSet, mesh, out name, out ids);
                        if (NamedClass.CheckNameError(name) != null) AddError(NamedClass.CheckNameError(name));
                        else if (ids != null) mesh.AddElementSet(new FeElementSet(name, ids));
                    }
                    else if (keyword == "*SURFACE")
                    {
                        WriteDataToOutputStatic("Reading keyword line: " + dataSet[0]);
                        FeSurface surface = GetSurface(dataSet, lineNumber);
                        if (surface != null) mesh.AddSurface(surface);
                    }
                    else if (keyword == "*RIGID BODY")
                    {
                        WriteDataToOutputStatic("Reading keyword line: " + dataSet[0]);
                        GetRigidBody(dataSet, lineNumber, nodes, constraints, referencePoints);
                    }
                    else if (keyword == "*MATERIAL")
                    {
                        WriteDataToOutputStatic("Reading keyword line: " + dataSet[0]);
                        Material material = GetMaterial(dataSets, i, lineNumber);
                        if (material != null) materials.Add(material.Name, material);
                    }
                    else if (keyword == "*SOLID SECTION") // JAW 21 July 2020 - Capture solid sections
                    {
                        WriteDataToOutputStatic("Reading keyword line: " + dataSet[0]);
                        SolidSection section = GetSection(dataSets, i, lineNumber);
                        if (section != null)
                        {
                            secCounter++;
                            section.Name += secCounter;
                            sections.Add(section.Name, section);
                        }
                    } // End of JAW Mods
                    else if (keyword == "*STEP")
                    {
                        WriteDataToOutputStatic("Reading keyword line: " + dataSet[0]);
                        StaticStep step = GetStep(dataSets, i, lineNumber);
                        if (step != null)
                        {
                            stepCounter++;
                            step.Name += stepCounter;
                            // Create object to store list of steps in *.inp files
                            var stepColl = new StepCollection();

                            stepColl.AddStep(step);
                            foreach (var entryStep in stepColl.StepsList) model.StepCollection.AddStep(entryStep);
                            //foreach (var entryStep in stepColl.StepsList) model.StepCollection.AddStep(entryStep);
                            //stepColl.ReplaceStep(step,step);
                            // step.Name += String.Join("", stepCounter);
                            //stepColl.RemoveStep();
                            // If there is more than 1 step, incrementally adjust the names of the boundary
                            // conditions for the additional steps
                            /*if (stepCounter > 1)
                            {
                                for (var bc = 1; bc < step.BoundaryConditions.Count(); bc++)
                                    step.
                            }

                            stepColl.AddStep(step);
                            var bCCounterStor = bCCounter; // Counter from previous step
                            */
                        }
                    }
                    lineNumber += dataSet.Length;
                }

                //try to output errors as TextReader ...


                foreach (var entry in referencePoints) mesh.ReferencePoints.Add(entry.Key, entry.Value);

                if (elementsToImport != ElementsToImport.All)
                {
                    if (elementsToImport != ElementsToImport.Beam) mesh.RemoveElementsByType<FeElement1D>();
                    if (elementsToImport != ElementsToImport.Shell) mesh.RemoveElementsByType<FeElement2D>();
                    if (elementsToImport != ElementsToImport.Solid) mesh.RemoveElementsByType<FeElement3D>();
                }

                model.ImportMesh(mesh, null);


                // add model items
                foreach (var entry in constraints) model.Constraints.Add(entry.Key, entry.Value);
                foreach (var entry in materials) model.Materials.Add(entry.Key, entry.Value);
                foreach (var entry in sections) model.Sections.Add(entry.Key, entry.Value);
                //foreach (var entryStep in stepColl.StepsList) model.StepCollection.AddStep(entryStep);
            }
        }

        static private string[][] GetDataSets(string[] lines)
        {
            List<string> dataSet = null;
            List<string[]> dataSets = new List<string[]>();

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length < 2) continue;
                if (lines[i][0] == '*' && lines[i][1] == '*') continue;

                if (lines[i][0] == '*' && lines[i][1] != '*')
                {
                    // first keyword
                    if (dataSet != null) dataSets.Add(dataSet.ToArray());
                    dataSet = new List<string>();
                }
                if (dataSet != null) dataSet.Add(lines[i]);
            }

            // add last data set
            if (dataSet != null) dataSets.Add(dataSet.ToArray());

            return dataSets.ToArray();
        }

        static private Dictionary<int, FeNode> GetNodes(string[] lines)
        {
            Dictionary<int, FeNode> nodes = new Dictionary<int, FeNode>();
            int id;
            FeNode node;
            string[] record;
            string[] splitter = new string[] { " ", ",", "\t" };

            // line 0 is the line with the keyword
            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("*")) continue;

                record = lines[i].Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                id = int.Parse(record[0]);
                node = new FeNode();
                node.Id = id;
                node.X = double.Parse(record[1]);
                node.Y = double.Parse(record[2]);
                node.Z = double.Parse(record[3]);

                nodes.Add(id, node);
            }

            return nodes;
        }
        static private void AddElements(string[] lines, ref Dictionary<int, FeElement> elements, ref List<InpElementSet> inpElementTypeSets)
        {
            try
            {
                FeElement element;

                string elementType = null;
                string elementSetName = null;
                string[] record1;
                string[] record2;

                // *Element, type=C3D4, ELSET=PART1
                record1 = lines[0].Split(splitterComma, StringSplitOptions.RemoveEmptyEntries);

                foreach (var rec in record1)
                {
                    record2 = rec.Split(splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                    if (record2.Length == 2)
                    {
                        if (record2[0].Trim().ToUpper() == "TYPE") elementType = record2[1].Trim().ToUpper();
                        else if (record2[0].Trim().ToUpper() == "ELSET") elementSetName = record2[1].Trim();
                    }
                }
                if (elementType == null) return;

                List<int> elementIds = new List<int>();
                // line 0 is the line with the keyword
                for (int i = 1; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("*")) continue;

                    switch (elementType)
                    {
                        // LINEAR ELEMENTS                                                                                          

                        case "C3D4":
                            // linear tetrahedron element
                            element = GetLinearTetraElement(lines[i].Split(splitter, StringSplitOptions.RemoveEmptyEntries));
                            break;
                        case "C3D6":
                            // linear wedge element
                            element = GetLinearWedgeElement(lines[i].Split(splitter, StringSplitOptions.RemoveEmptyEntries));
                            break;
                        case "C3D8":
                            // linear hexahedron element
                            element = GetLinearHexaElement(lines[i].Split(splitter, StringSplitOptions.RemoveEmptyEntries));
                            break;
                        case "C3D8R":
                            // linear hexahedron element
                            element = GetLinearHexaElement(lines[i].Split(splitter, StringSplitOptions.RemoveEmptyEntries));
                            break;
                        case "C3D8I":
                            // linear hexahedron element
                            element = GetLinearHexaElement(lines[i].Split(splitter, StringSplitOptions.RemoveEmptyEntries));
                            break;

                        // PARABOLIC ELEMENTS                                                                                       

                        case "C3D10":
                            // parabolic tetrahedron element
                            element = GetParabolicTetraElement(ref i, lines, splitter);
                            break;
                        case "C3D15":
                            // parabolic wedge element
                            element = GetParabolicWedgeElement(ref i, lines, splitter);
                            break;
                        // parabolic hexahedron element
                        case "C3D20":
                            element = GetParabolicHexaElement(ref i, lines, splitter);
                            break;
                        // parabolic hexahedron element
                        case "C3D20R":
                            element = GetParabolicHexaElement(ref i, lines, splitter);
                            break;

                        default:
                            //System.Windows.Forms.MessageBox.Show("The element type '" + elementType + "' is not supported.");
                            //break;
                            throw new Exception("The element type '" + elementType + "' is not supported.");
                    }
                    elementIds.Add(element.Id);
                    elements.Add(element.Id, element);
                }

                // find element set
                InpElementSet inpSet = null;
                foreach (var entry in inpElementTypeSets)
                {
                    if (entry.Name == elementSetName)   // both names can be null - must be like this for sets without names
                    {
                        inpSet = entry;
                        break;
                    }
                }
                // create new set
                if (inpSet == null)
                {
                    inpSet = new InpElementSet(elementSetName, new HashSet<string>(), new HashSet<int>());
                    inpElementTypeSets.Add(inpSet);
                }
                // add type and labels
                inpSet.InpElementTypeNames.Add(elementType);
                inpSet.ElementLabels.UnionWith(elementIds);
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
            }
        }
        static private void GetNodeOrElementSet(string keywordName, string[] lines, FeMesh mesh, out string name, out int[] ids)
        {
            name = null;
            ids = null;
            //
            try
            {
                bool generate = false;
                string[] record1;
                string[] record2;
                // *NSET,NSET=SET1,GENERATE
                record1 = lines[0].Split(splitterComma, StringSplitOptions.RemoveEmptyEntries);
                //
                foreach (var rec in record1)
                {
                    record2 = rec.Split(splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                    if (record2.Length == 1)
                    {
                        if (record2[0].Trim().ToUpper() == "GENERATE") generate = true;
                    }
                    else if (record2.Length == 2)
                    {
                        if (record2[0].Trim().ToUpper() == keywordName.ToUpper()) name = record2[1].Trim();
                    }
                }
                if (name == null) WriteDataToOutputStatic("There is a set name missing in the line: " + lines[0]);
                //
                List<int> setIds = new List<int>();
                if (generate)   // can be written in more than one line
                {
                    // Line 0 is the line with the keyword
                    for (int i = 1; i < lines.Length; i++)
                    {
                        record1 = lines[i].Split(splitterComma, StringSplitOptions.RemoveEmptyEntries);
                        //
                        if (record1.Length <= 3)
                        {
                            int start = int.Parse(record1[0]);
                            int end = int.Parse(record1[1]);
                            int step = 1;
                            if (record1.Length == 3) step = int.Parse(record1[2]);

                            for (int j = start; j <= end; j += step) setIds.Add(j);
                        }
                        else throw new CaeGlobals.CaeException("When node set is defined using GENERATE parameter at most three node ids are expected.");
                    }
                    ids = setIds.ToArray();
                }
                else
                {
                    int intId;
                    string setName;
                    FeNodeSet nodeSet;
                    FeElementSet elementSet;
                    BasePart part;
                    //
                    for (int i = 1; i < lines.Length; i++)
                    {
                        record1 = lines[i].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string id in record1)
                        {
                            if (int.TryParse(id, out intId)) setIds.Add(intId);
                            else
                            {
                                setName = id;
                                if (keywordName == "NSET" && mesh.NodeSets.TryGetValue(setName, out nodeSet))
                                {
                                    setIds.AddRange(nodeSet.Labels);
                                }
                                else if (keywordName == "ELSET" && mesh.ElementSets.TryGetValue(setName, out elementSet))
                                {
                                    setIds.AddRange(elementSet.Labels);
                                }
                                else if (keywordName == "ELSET" && mesh.Parts.TryGetValue(setName, out part))
                                {
                                    setIds.AddRange(part.Labels);
                                }
                                else WriteDataToOutputStatic("The set named '" + setName + "' was not found.");
                            }
                        }
                    }
                    ids = setIds.ToArray();
                    Array.Sort(ids);
                }
            }
            catch (Exception ex)
            {
                WriteDataToOutputStatic(ex.Message);
            }
        }
        static private FeSurface GetSurface(string[] lines, int firstLineNumber)
        {
            // *Surface, name=Surface-2, type=Element
            // internal-2_Surface-2_S2, S2
            // internal-3_Surface-2_S6, S6
            // 15, S5
            // 81, S5

            // *Surface, type = NODE, name = _T0_Part - 1 - 1_SN, internal
            // _T0_Part-1-1_SN

            string name = null;
            FeSurfaceType type = FeSurfaceType.Element;
            string[] record1;
            string[] record2;

            try
            {
                record1 = lines[0].Split(splitterComma, StringSplitOptions.RemoveEmptyEntries);

                foreach (var rec in record1)
                {
                    record2 = rec.Split(splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                    if (record2.Length == 2)
                    {
                        if (record2[0].Trim().ToUpper() == "NAME") name = record2[1].Trim();
                        else if (record2[0].Trim().ToUpper() == "TYPE")
                        {
                            if (record2[1].Trim().ToUpper() == "ELEMENT") type = FeSurfaceType.Element;
                            else if (record2[1].Trim().ToUpper() == "NODE") type = FeSurfaceType.Node;
                            else
                            {
                                _errors.Add("Line " + firstLineNumber + ": Surface type '" + record2[1].Trim() + "' is not supported");
                                return null;
                            }
                        }
                    }
                }
                if (name == null)
                {
                    _errors.Add("Line " + firstLineNumber + ": Surface name is not defined");
                    return null;
                }

                FeSurface surface = new FeSurface(name);
                surface.Type = type;

                if (type == FeSurfaceType.Element)
                {
                    surface.CreatedFrom = FeSurfaceCreatedFrom.Faces;

                    // line 0 is the line with the keyword
                    FeFaceName faceName;
                    string elementSetName;
                    for (int i = 1; i < lines.Length; i++)
                    {
                        record1 = lines[i].Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                        if (record1.Length == 2)
                        {
                            elementSetName = record1[0];
                            faceName = FeFaceName.Empty;
                            Enum.TryParse<FeFaceName>(record1[1].ToUpper(), out faceName);
                            surface.AddElementFace(faceName, elementSetName);
                        }
                        else
                        {
                            _errors.Add("Line " + firstLineNumber + i + ": When surface face is defined an element set name and face type are expected");
                            return null;
                        }
                    }
                }
                else if (type == FeSurfaceType.Node)
                {
                    surface.CreatedFrom = FeSurfaceCreatedFrom.NodeSet;

                    if (lines.Length != 2)
                    {
                        _errors.Add("Line " + firstLineNumber + 2 + ": When surface with type NODE is defined one node set name is expected");
                        return null;
                    }

                    // line 0 is the line with the keyword    
                    record1 = lines[1].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                    if (record1.Length == 1)
                    {
                        surface.CreatedFromNodeSetName = record1[0];
                    }
                    else
                    {
                        _errors.Add("Line " + firstLineNumber + 1 + ": When surface with type NODE is defined one node set name is expected");
                        return null;
                    }

                }
                return surface;
            }
            catch
            {
                if (name != null) name = " " + name;
                else name = "";
                _errors.Add("Line " + firstLineNumber + ": Failed to import surface" + name);
                return null;
            }
        }

        static private void GetRigidBody(string[] lines, int firstLineNumber, Dictionary<int, FeNode> nodes,
                                         Dictionary<string, Constraint> constraints,
                                         Dictionary<string, FeReferencePoint> referencePoints)
        {
            try
            {
                if (lines.Length > 1)
                {
                    _errors.Add("Line " + firstLineNumber + ": Only one line expected for the rigid body");
                    return;
                }

                string nodeSetName;
                int nodeId1 = -1;
                int nodeId2 = -1;
                string[] record1;
                string[] record2;

                //*Rigid body, Nset=internal-2_Element-Selection, Ref node=57885, Rot node=57886
                record1 = lines[0].Split(splitterComma, StringSplitOptions.RemoveEmptyEntries);
                if (record1.Length != 4)
                {
                    _errors.Add("Line " + firstLineNumber + ": Unsupported line formatting for the rigid body");
                    return;
                }

                record2 = record1[1].Split(splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                nodeSetName = record2[1];

                record2 = record1[2].Split(splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                nodeId1 = int.Parse(record2[1]);

                record2 = record1[3].Split(splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                nodeId2 = int.Parse(record2[1]);

                // check for existing reference point
                string referencePointName = null;
                foreach (var entry in referencePoints)
                {
                    if (entry.Value.CreatedFromRefNodeId1 == nodeId1)
                    {
                        referencePointName = entry.Key;
                        break;
                    }
                }
                // create a new reference point if it was not found
                if (referencePointName == null)
                {
                    referencePointName = NamedClass.GetNewValueName(referencePoints.Keys, "RP-");
                    FeNode node = nodes[nodeId1];
                    FeReferencePoint referencePoint = new FeReferencePoint(referencePointName, node, nodeId2);
                    referencePoints.Add(referencePointName, referencePoint);
                }

                string rigidBodyName = NamedClass.GetNewValueName(constraints.Keys, "Constraint-");
                RigidBody rigidBody = new RigidBody(rigidBodyName, referencePointName, nodeSetName, RegionTypeEnum.NodeSetName);
                constraints.Add(rigidBodyName, rigidBody);
            }
            catch
            {
                _errors.Add("Line " + firstLineNumber + ": Failed to import rigid body");
            }
        }

        static private Material GetMaterial(string[][] dataSets, int dataSetId, int firstLineNumber)
        {
            // *Material, Name=S235


            Material material = null;
            string name = null;
            string[] record1;
            string[] record2;

            try
            {
                string[] dataSet = dataSets[dataSetId];
                record1 = dataSet[0].Split(splitterComma, StringSplitOptions.RemoveEmptyEntries);

                foreach (var rec in record1)
                {
                    record2 = rec.Split(splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                    if (record2.Length == 2)
                    {
                        if (record2[0].Trim().ToUpper() == "NAME")
                        {
                            name = record2[1].Trim();
                            break;
                        }
                    }
                }
                firstLineNumber += dataSet.Length;
                dataSetId++;

                if (name != null)
                {
                    string keyword;
                    material = new Material(name);

                    for (int i = dataSetId; i < dataSets.Length; i++)
                    {
                        dataSet = dataSets[i];    // next keyword
                        keyword = dataSet[0].Split(splitter, StringSplitOptions.RemoveEmptyEntries)[0].Trim().ToUpper();

                        if (keyword == "*DENSITY")
                        {
                            Density density = GetMaterialDensity(dataSet, firstLineNumber);
                            if (density != null) material.AddProperty(density);
                        }
                        else if (keyword == "*ELASTIC")
                        {
                            Elastic elastic = GetMaterialElasticity(dataSet, firstLineNumber);
                            if (elastic != null) material.AddProperty(elastic);
                        }
                        else if (keyword == "*PLASTIC")
                        {
                            Plastic plastic = GetMaterialPlasticity(dataSet, firstLineNumber);
                            if (plastic != null) material.AddProperty(plastic);
                        }
                        else
                        {
                            break;
                        }

                        firstLineNumber += dataSet.Length;
                    }
                }
                else _errors.Add("Line " + firstLineNumber + ": Material name not found");

                return material;
            }
            catch
            {
                if (name != null) name = " " + name;
                else name = "";
                _errors.Add("Line " + firstLineNumber + ": Failed to import material" + name);
                return null;
            }
        }
        static private Density GetMaterialDensity(string[] lines, int firstLineNumber)
        {
            // *Density
            // 7.85E-09
            // JAW 17 Jul 2020
            // Sometimes the density value keyword has a comma at the end (7.85e-09,)
            // so we read the line expecting a comma at the end of the density value

            string[] record1;

            try
            {
                record1 = lines[1].Split(splitterComma, StringSplitOptions.RemoveEmptyEntries);
                double rho = double.Parse(record1[0]);
                // End of JAW Mods
                Density density = new Density(rho);
                return density;
            }
            catch
            {
                _errors.Add("Line " + firstLineNumber + ": Failed to import density");
                return null;
            }
        }
        static private Elastic GetMaterialElasticity(string[] lines, int firstLineNumber)
        {
            // *Elastic
            // 210000, 0.3

            string[] record1;

            try
            {
                record1 = lines[1].Split(splitterComma, StringSplitOptions.RemoveEmptyEntries);

                double E = double.Parse(record1[0]);
                double v = double.Parse(record1[1]);

                Elastic elastic = new Elastic(E, v);
                return elastic;
            }
            catch
            {
                _errors.Add("Line " + firstLineNumber + ": Failed to import elasticity");
                return null;
            }
        }
        static private Plastic GetMaterialPlasticity(string[] lines, int firstLineNumber)
        {
            // *Plastic, Hardening=Kinematic
            // 235, 0
            // 400, 0.2

            string[] record1;

            try
            {
                PlasticHardening hardening = PlasticHardening.Isotropic;
                double[] stressStrain;
                List<double[]> stressStrains = new List<double[]>();

                record1 = lines[0].Split(splitterComma, StringSplitOptions.RemoveEmptyEntries);
                if (record1.Length > 1)
                {
                    record1 = record1[1].Split(splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                    hardening = (PlasticHardening)Enum.Parse(typeof(PlasticHardening), record1[1]);
                }

                for (int i = 1; i < lines.Length; i++)
                {
                    stressStrain = new double[2];
                    record1 = lines[i].Split(splitterComma, StringSplitOptions.RemoveEmptyEntries);
                    stressStrain[0] = double.Parse(record1[0]);
                    stressStrain[1] = double.Parse(record1[1]);
                    stressStrains.Add(stressStrain);
                }

                Plastic plastic = new Plastic(stressStrains.ToArray());
                plastic.Hardening = hardening;
                return plastic;
            }
            catch
            {
                _errors.Add("Line " + firstLineNumber + ": Failed to import plasticity");
                return null;
            }
        }
        // JAW 20 July 2020
        // Get section is fashioned after GetMaterial
        static private SolidSection GetSection(string[][] dataSets, int dataSetId, int firstLineNumber)
        {
            // Solid section can exist in two (2) formats as shown below:

            // *Solid Section, ELSET=STEEL_A, MATERIAL=STEEL_A

            // *Solid Section, MATERIAL=STEEL_A, ELSET=STEEL_A

            string regionName = null;
            string materialName = null;

            try
            {
                string[] dataSet = dataSets[dataSetId];
                string[] record1 = dataSet[0].Split(splitterComma, StringSplitOptions.RemoveEmptyEntries);

                foreach (var rec in record1)
                {
                    string[] record2 = rec.Split(splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                    if (record2.Length != 2) continue;
                    if (record2[0].Trim().ToUpper() == "ELSET") regionName = record2[1].Trim();
                    else if (record2[0].Trim().ToUpper() == "MATERIAL") materialName = record2[1].Trim();
                }

                var section = new SolidSection("Section-", materialName, regionName, RegionTypeEnum.ElementSetName);

                return section;
            }
            catch
            {
                _errors.Add("Line " + firstLineNumber + ": Failed to import section");
                return null;
            }
        }

        // JAW 20 July 2020
        // Get Step
        static private StaticStep GetStep(string[][] dataSets, int dataSetId, int firstLineNumber)
        {
            // *STEP, NAME=STEP-1, NLGEOM=NO, PERTURBATION -- ABAQUS

            // *STEP,NLGEOM,INC = 1000 -- CALCULIX

            var step = new StaticStep("Step-");
            var bCCounter = 0;
            try
            {
                string[] dataSet = dataSets[dataSetId];
                string[] record1 = dataSet[0].Split(splitterComma, StringSplitOptions.RemoveEmptyEntries);
                // Capture the options  on the step line
                foreach (var rec in record1)
                {
                    string[] record2 = rec.Split(splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                    if (record2.Length == 2) // Look for ABAQUS Style "NLGEOM = YES"
                    {
                        if (record2[0].Trim().ToUpper() == "NLGEOM")
                        {
                            if (record2[1].Trim() == "YES") step.Nlgeom = true;
                        }

                        else if (record2[0].Trim().ToUpper() == "INC")
                        {
                            step.MaxIncrements = int.Parse(record2[1].Trim());
                        }
                    }
                    else if (record2.Length == 1) // Look for Calculix stye "NLGEOM"
                    {
                        if (record2[0] == "NLGEOM") step.Nlgeom = true;

                    }
                }

                // Determine other Keywords in Step
                firstLineNumber += dataSet.Length;
                dataSetId++;

                for (var i = dataSetId; i < dataSets.Length; i++)
                {
                    dataSet = dataSets[i]; // next keyword
                    var keyword = dataSet[0].Split(splitter, StringSplitOptions.RemoveEmptyEntries)[0].Trim()
                        .ToUpper();

                    if ((keyword == "*STATIC") && (step.Nlgeom == true))
                    {
                        // Read NLGEOM incremental data
                        string[] record3 = dataSet[1].Split(splitterComma, StringSplitOptions.RemoveEmptyEntries);

                        step.InitialTimeIncrement = double.Parse(record3[0]);
                        step.TimePeriod = double.Parse(record3[1]);
                        step.MinTimeIncrement = double.Parse(record3[2]);
                        step.MaxTimeIncrement = double.Parse(record3[3]);
                    }

                    if (keyword == "*BOUNDARY")

                    {
                        AddStepBoundaryCondition(step, dataSet, firstLineNumber);
                    }

                    if (keyword == "*CLOAD")
                    {
                        AddStepCLoad(step, dataSet, firstLineNumber);
                    }

                    if (keyword == "*DLOAD")
                    {
                        AddStepDLoad(step, dataSet, firstLineNumber);
                    }

                    if (keyword == "*END") break; // *END STEP

                    firstLineNumber += dataSet.Length;
                }
                return step;
            }
            catch
            {
                _errors.Add("Line " + firstLineNumber + ": Failed to import step");
                return null;
            }

        }

        static private StaticStep AddStepBoundaryCondition(StaticStep step, string[] lines, int firstLineNumber)
        {
            // *Boundary
            // Set_1, 2, 2
            // Set_1, 3, 3, 0.001

            try
            {
                var name = "BC-";
                for (var bci = 1; bci < lines.Length; bci++)
                {
                    string[] recordBC = lines[bci].Split(splitterComma, StringSplitOptions.RemoveEmptyEntries);

                    // Create DisplacementRotation BC

                    // To avoid duplicate boundary condition names of additional steps,
                    // a random letter is included in the numbering count.
                    // This option is used until a better solution is implemented

                    Random rand = new Random(Guid.NewGuid().GetHashCode());
                    var num = rand.Next(0, 26); // Zero to 25
                    char let = (char)('A' + num);
                    // Get regionName
                    var regionName = recordBC[0].Trim();
                    var bCond = new DisplacementRotation(name + let, regionName, RegionTypeEnum.NodeSetName);
                    bCond.Name += (step.BoundaryConditions.Count + 1);
                    // Get the prescribed displacement
                    double dofValue = 0;
                    if (recordBC.Length == 4) dofValue = double.Parse(recordBC[3]);
                    // Assign DOF prescribed displacement
                    for (var dofi = 1; dofi < 3; dofi++)
                    {
                        var dValue = int.Parse(recordBC[dofi]);

                        switch (dValue)
                        {
                            case 1:
                                bCond.U1 = dofValue;
                                break;
                            case 2:
                                bCond.U2 = dofValue;
                                break;
                            case 3:
                                bCond.U3 = dofValue;
                                break;
                            case 4:
                                bCond.UR1 = dofValue;
                                break;
                            case 5:
                                bCond.UR2 = dofValue;
                                break;
                            case 6:
                                bCond.UR3 = dofValue;
                                break;
                        }
                    }
                    step.AddBoundaryCondition(bCond);
                }
                return step;
            }
            catch
            {
                _errors.Add("Line " + firstLineNumber + ": Failed to import boundary condition");
                return null;
            }
        }


        static private StaticStep AddStepCLoad(StaticStep step, string[] lines, int firstLineNumber)
        {
            // *CLoad
            // LD_BRTIP, 2, 10000 - Concentrated Force (f1,f2,f3)
            // LD_BRTIP, 6, 10000 - Moment (m1,m2,m3)

            try
            {
                // CLoad can either be a concentrated force or moment
                var nameCF = "CF-";
                var nameMom = "MF-";

                for (var bci = 1; bci < lines.Length; bci++)
                {
                    string[] recordCL = lines[bci].Split(splitterComma, StringSplitOptions.RemoveEmptyEntries);

                    // To avoid duplicate loading condition names for additional steps,
                    // a random letter is included in the numbering count.
                    // This option is used until a better solution is implemented

                    Random rand = new Random(Guid.NewGuid().GetHashCode());
                    var num = rand.Next(0, 26); // Zero to 25
                    char let = (char)('A' + num);

                    // Get regionName
                    var regionName = recordCL[0].Trim();
                    // Get degree of freedom and value
                    var dof = int.Parse(recordCL[1]); ;
                    var dofValue = double.Parse(recordCL[2]);
                    var cfLoad = new CLoad(nameCF + let, regionName, RegionTypeEnum.NodeSetName, 0.0, 0.0, 0.0);
                    var mLoad = new MomentLoad(nameMom + let, regionName, RegionTypeEnum.NodeSetName, 0.0, 0.0, 0.0);
                    switch (dof)
                    {
                        case 1:
                            cfLoad.F1 = dofValue;
                            cfLoad.Name += (step.Loads.Count + 1);
                            step.AddLoad(cfLoad);
                            break;
                        case 2:
                            cfLoad.F2 = dofValue;
                            cfLoad.Name += (step.Loads.Count + 1);
                            step.AddLoad(cfLoad);
                            break;
                        case 3:
                            cfLoad.F3 = dofValue;
                            cfLoad.Name += (step.Loads.Count + 1);
                            step.AddLoad(cfLoad);
                            break;
                        case 4:
                            mLoad.M1 = dofValue;
                            mLoad.Name += (step.Loads.Count + 1);
                            step.AddLoad(mLoad);
                            break;
                        case 5:
                            mLoad.M2 = dofValue;
                            mLoad.Name += (step.Loads.Count + 1);
                            step.AddLoad(mLoad);
                            break;
                        case 6:
                            mLoad.M3 = dofValue;
                            mLoad.Name += (step.Loads.Count + 1);
                            step.AddLoad(mLoad);
                            break;
                    }
                }
                return step;
            }
            catch
            {
                _errors.Add("Line " + firstLineNumber + ": Failed to import loading condition");
                return null;
            }
        }

        static private StaticStep AddStepDLoad(StaticStep step, string[] lines, int firstLineNumber)
        {
            // *DLoad
            // Eall, GRAV, 9.81, 0, 0, -1

            try
            {
                // DLoad as gravity force is the only force covered at this point
                var nameGrav = "Grav-";

                for (var bci = 1; bci < lines.Length; bci++)
                {
                    string[] recordDL = lines[bci].Split(splitterComma, StringSplitOptions.RemoveEmptyEntries);

                    // Create Concentrated Force Object

                    // To avoid duplicate loading condition names for additional steps,
                    // a random letter is included in the numbering count.
                    // This option is used until a better solution is implemented

                    Random rand = new Random(Guid.NewGuid().GetHashCode());
                    var num = rand.Next(0, 26); // Zero to 25
                    char let = (char)('A' + num);

                    // Get regionName
                    var regionName = recordDL[0].Trim();
                    // Get loading type
                    var loadingType = recordDL[1].Trim();
                    // Get Gravity value
                    var gValue = double.Parse(recordDL[2]); ;
                    var gLoad = new GravityLoad(nameGrav, regionName, RegionTypeEnum.ElementSetName, 0.0, 0.0, 0.0);

                    if (int.Parse(recordDL[3]) != 0)
                    {
                        gLoad.F1 = double.Parse(recordDL[3]) * gValue;
                        gLoad.Name += (step.Loads.Count + 1);
                        step.AddLoad(gLoad);
                    }

                    if (int.Parse(recordDL[4]) != 0)
                    {
                        gLoad.F2 = double.Parse(recordDL[4]) * gValue;
                        step.AddLoad(gLoad);
                    }

                    if (int.Parse(recordDL[5]) != 0)
                    {
                        gLoad.F3 = double.Parse(recordDL[5]) * gValue;
                        step.AddLoad(gLoad);
                    }
                }
                return step;
            }
            catch
            {
                _errors.Add("Line " + firstLineNumber + ": Failed to import gravity loading");
                return null;
            }
        }

        //  LINEAR ELEMENTS                                                                                        
        static private LinearTetraElement GetLinearTetraElement(string[] record)
        {
            // 1, 598, 1368, 1306, 1291
            int id = int.Parse(record[0]);
            int[] nodes = new int[4];
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i] = int.Parse(record[i + 1]);
            }
            return new LinearTetraElement(id, nodes);
        }
        static private LinearWedgeElement GetLinearWedgeElement(string[] record)
        {
            // 1,  132,  133,  495,  646,  647, 1009
            int id = int.Parse(record[0]);
            int[] nodes = new int[6];
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i] = int.Parse(record[i + 1]);
            }
            return new LinearWedgeElement(id, nodes);
        }
        static private LinearHexaElement GetLinearHexaElement(string[] record)
        {
            // 1, 598, 1368, 1306, 1291, 3, 4, 5, 6
            int id = int.Parse(record[0]);
            int[] nodes = new int[8];
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i] = int.Parse(record[i + 1]);
            }
            return new LinearHexaElement(id, nodes);
        }


        //  PARABOLIC ELEMENTS                                                                                    
        static private ParabolicTetraElement GetParabolicTetraElement(ref int lineId, string[] lines, string[] splitter)
        {
            // 1, 5518, 5519, 4794, 5898, 19815, 19819, 19817, 
            // 19816, 19818, 19820
            int n = 10;
            int[] nodes = new int[n];

            // First row
            string[] record = lines[lineId].Split(splitter, StringSplitOptions.RemoveEmptyEntries);

            int id = int.Parse(record[0]);

            int count = 0;
            for (int i = 1; i < record.Length; i++)
            {
                nodes[count++] = int.Parse(record[i]);
            }

            // Second row
            if (record.Length < n + 1)
            {
                lineId++;
                record = lines[lineId].Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < record.Length; i++)
                {
                    nodes[count++] = int.Parse(record[i]);
                }
            }

            return new ParabolicTetraElement(id, nodes);
        }
        static private ParabolicWedgeElement GetParabolicWedgeElement(ref int lineId, string[] lines, string[] splitter)
        {
            // 1, 5518, 5519, 4794, 5898, 19815, 19819, 19817, 
            // 19816, 19818, 19820, 1, 2, 3, 4, 5
            int n = 15;
            int[] nodes = new int[n];

            // First row
            string[] record = lines[lineId].Split(splitter, StringSplitOptions.RemoveEmptyEntries);

            int id = int.Parse(record[0]);

            int count = 0;
            for (int i = 1; i < record.Length; i++)
            {
                nodes[count++] = int.Parse(record[i]);
            }

            // Second row
            if (record.Length < n + 1)
            {
                lineId++;
                record = lines[lineId].Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < record.Length; i++)
                {
                    nodes[count++] = int.Parse(record[i]);
                }
            }

            return new ParabolicWedgeElement(id, nodes);
        }
        static private ParabolicHexaElement GetParabolicHexaElement(ref int lineId, string[] lines, string[] splitter)
        {
            // *ELEMENT, TYPE = C3D20R
            // 1,1,10,47,19,37,57,78,72,9,45,
            // 46,20,56,76,77,73,38,55,75,70

            int n = 20;
            int[] nodes = new int[n];

            // First row
            string[] record = lines[lineId].Split(splitter, StringSplitOptions.RemoveEmptyEntries);

            int id = int.Parse(record[0]);

            int count = 0;
            for (int i = 1; i < record.Length; i++)
            {
                nodes[count++] = int.Parse(record[i]);
            }

            // Second row
            if (record.Length < n + 1)
            {
                lineId++;
                record = lines[lineId].Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < record.Length; i++)
                {
                    nodes[count++] = int.Parse(record[i]);
                }
            }

            return new ParabolicHexaElement(id, nodes);
        }

        static private void AddError(string error)
        {
            Errors.Add(error);
            WriteDataToOutputStatic(error);
        }










    }
}
