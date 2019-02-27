using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeMesh;
using System.IO;
using FileInOut.Output.Calculix;
using CaeGlobals;

namespace FileInOut.Output
{
    [Serializable]
    public static class CalculixFileWriter
    {
        // Methods                                                                                                                  
        static public void Write(string fileName, FeModel model)
        {
            List<CalculixKeyword> keywords = GetAllKeywords(model);

            // write file
            StringBuilder sb = new StringBuilder();
            foreach (var keyword in keywords)
            {
                WriteKeywordRecursively(sb, keyword);
            }
            System.IO.File.WriteAllText(fileName, sb.ToString());
        }

        static public List<CalculixKeyword> GetAllKeywords(FeModel model)
        {
            List<CalculixKeyword> keywords = GetModelKeywords(model);

            // Add user keywords
            if (model.CalculixUserKeywords != null)
            {
                foreach (var entry in model.CalculixUserKeywords)
                {
                    AddUserKeywordByIndices(keywords, entry.Key, entry.Value.DeepClone()); // deep clone to prevent the changes in user keywords
                }
            }

            return keywords;
        }
        static public List<CalculixKeyword> GetModelKeywords(FeModel model)
        {
            // only keywords from the model, not user keywords
            // allways add a title keyword to get all possible keyword types to the keyword editor

            Dictionary<string, int[]> referencePointsNodeIds = new Dictionary<string, int[]>();
            if (model.Mesh != null)
            {
                // fill reference point nodes
                int id = model.Mesh.MaxNodeId;
                foreach (var entry in model.Mesh.ReferencePoints)
                {
                    referencePointsNodeIds.Add(entry.Value.Name, new int[] { id + 1, id + 2 });
                    id += 2;
                }
            }

            CalTitle title;
            List<CalculixKeyword> keywords = new List<CalculixKeyword>();

            // heading
            title = new CalTitle("Heading", "");
            keywords.Add(title);
            AppendHeading(model, title);

            // mesh
            title = new CalTitle("Nodes", "");
            keywords.Add(title);
            AppendNodes(model, referencePointsNodeIds, title);

            title = new CalTitle("Elements", "");
            keywords.Add(title);
            AppendElements(model, title);

            title = new CalTitle("Node sets", "");
            keywords.Add(title);
            AppendNodeSets(model, title);

            title = new CalTitle("Element sets", "");
            keywords.Add(title);
            //AppendParts(model, title);
            AppendElementSets(model, title);

            title = new CalTitle("Surfaces", "");
            keywords.Add(title);
            AppendSurfaces(model, title);

            //
            title = new CalTitle("Materials", "");
            keywords.Add(title);
            AppendMaterials(model, title);

            title = new CalTitle("Sections", "");
            keywords.Add(title);
            AppendSections(model, title);

            title = new CalTitle("Constraints", "");
            keywords.Add(title);
            AppendConstraints(model, referencePointsNodeIds, title);

            // Steps
            title = new CalTitle("Steps", "");
            keywords.Add(title);

            AppendSteps(model, referencePointsNodeIds, title);

            return keywords;
        }
        static private bool AddUserKeywordByIndices(List<CalculixKeyword> keywords, int[] indices, CalculixKeyword keyword)
        {
            if (indices.Length == 1)
            {
                keywords.Insert(indices[0], keyword);
            }
            else
            {
                CalculixKeyword keywordParent = keywords[indices[0]];

                for (int i = 1; i < indices.Length - 1; i++)
                {
                    if (indices[i] < keywordParent.Keywords.Count) keywordParent = keywordParent.Keywords[indices[i]];
                    else return false;
                }

                keywordParent.Keywords.Insert(indices[indices.Length - 1], keyword);
            }
            return true;
        }


        static public void RemoveLostUserKeywords(FeModel model)
        {
            List<CalculixKeyword> keywords = GetModelKeywords(model);

            // add user keywords
            List<int[]> keywordKeysToRemove = new List<int[]>();
            if (model.CalculixUserKeywords != null)
            {
                foreach (var entry in model.CalculixUserKeywords)
                {
                    if (!AddUserKeywordByIndices(keywords, entry.Key, entry.Value.DeepClone())) keywordKeysToRemove.Add(entry.Key);
                }
            }

            // remove lost user keywords
            foreach (var indices in keywordKeysToRemove)
            {
                model.CalculixUserKeywords.Remove(indices);
            }
        }
        static public List<int[]> GetKeywordIndices(FeModel model, object item)
        {
            List<CalculixKeyword> keywords = GetModelKeywords(model);
            List<int[]> allIndices = new List<int[]>();
            Queue<int> indices = new Queue<int>();
            int count = 0;
            foreach (var keyword in keywords)
            {
                indices.Enqueue(count);
                if (GetKeywordIndicesByName(keyword, ref indices, item))
                {
                    allIndices.Add(indices.ToArray());
                }
                indices.Dequeue();
                count++;
            }
            return allIndices;
        }
        static private bool GetKeywordIndicesByName(CalculixKeyword keyword, ref Queue<int> indices, object item)
        {
            if (keyword.BaseItem == item) return true;

            int count = 0;
            foreach (var childkeyword in keyword.Keywords)
            {
                indices.Enqueue(count);
                if (GetKeywordIndicesByName(childkeyword, ref indices, item))
                    return true;
                indices.Dequeue();
                count++;
            }
            return false;
        }


        static public string GetShortKeywordData(CalculixKeyword keyword)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(keyword.GetKeywordString());
            if (!(keyword is CalNode) && !(keyword is CalElement) && !(keyword is CalNodeSet) && !(keyword is CalElementSet))
                sb.Append(keyword.GetDataString());
            else
            {
                //int n = 10;
                //string[] lines = keyword.GetDataString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                //for (int i = 0; i < Math.Min(n,  lines.Length); i++) sb.AppendLine(lines[i]);
                //if (lines.Length > n) sb.AppendLine("... hidden data ...");
                sb.AppendLine("... hidden data ...");
            }
            return sb.ToString();
        }
        static private void WriteKeywordRecursively(StringBuilder sb, CalculixKeyword keyword)
        {
            sb.Append(keyword.GetKeywordString());
            sb.Append(keyword.GetDataString());

            foreach (var childkeyword in keyword.Keywords)
            {
                WriteKeywordRecursively(sb, childkeyword);
            }
        }

        static private void AppendHeading(FeModel model, CalculixKeyword parent)
        {
            CalHeading heading = new CalHeading(model.Name);
            parent.AddKeyword(heading);
        }
        static private void AppendNodes(FeModel model, Dictionary<string, int[]> referencePointsNodeIds, CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                CalNode node = new CalNode(model, referencePointsNodeIds);
                parent.AddKeyword(node);
            }

        }
        static private void AppendElements(FeModel model, CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                Dictionary<string, List<FeElement>> elementTypes = new Dictionary<string, List<FeElement>>();
                List<FeElement> elements;

                string type;
                FeElement element;
                MeshPart part;
                CalElement elementKeyword;

                foreach (var entry in model.Mesh.Parts)
                {
                    elementTypes.Clear();
                    part = (MeshPart)entry.Value;

                    foreach (int elementId in part.Labels)
                    {
                        element = model.Mesh.Elements[elementId];
                        if (part.LinearTetraType != FeElementTypeLinearTetra.None && element is LinearTetraElement) type = part.LinearTetraType.ToString();
                        else if (part.LinearWedgeType != FeElementTypeLinearWedge.None && element is LinearWedgeElement) type = part.LinearWedgeType.ToString();
                        else if (part.LinearHexaType != FeElementTypeLinearHexa.None && element is LinearHexaElement) type = part.LinearHexaType.ToString();
                        else if (part.ParabolicTetraType != FeElementTypeParabolicTetra.None && element is ParabolicTetraElement) type = part.ParabolicTetraType.ToString();
                        else if (part.ParabolicWedgeType != FeElementTypeParabolicWedge.None && element is ParabolicWedgeElement) type = part.ParabolicWedgeType.ToString();
                        else if (part.ParabolicHexaType != FeElementTypeParabolicHexa.None && element is ParabolicHexaElement) type = part.ParabolicHexaType.ToString();
                        else throw new NotImplementedException();

                        // add element to the coresponding type
                        if (elementTypes.TryGetValue(type, out elements)) elements.Add(element);
                        else elementTypes.Add(type, new List<FeElement>() { element });
                    }

                    foreach (var typeEntry in elementTypes)
                    {
                        elementKeyword = new CalElement(typeEntry.Key, part.Name, typeEntry.Value, part);
                        parent.AddKeyword(elementKeyword);
                    }
                }
            }
        }
        static private void AppendNodeSets(FeModel model, CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                CalNodeSet nodeSet;
                foreach (var entry in model.Mesh.NodeSets)
                {
                    if (entry.Value.Active)
                    {
                        nodeSet = new CalNodeSet(entry.Value);
                        parent.AddKeyword(nodeSet);
                    }
                }
            }
        }
        static private void AppendParts(FeModel model, CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                CalElementSet part;
                foreach (var entry in model.Mesh.Parts)
                {
                    if (entry.Value.Active)
                    {
                        part = new CalElementSet(entry.Value);
                        parent.AddKeyword(part);
                    }
                }
            }
        }
        static private void AppendElementSets(FeModel model, CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                CalElementSet elementSet;
                foreach (var entry in model.Mesh.ElementSets)
                {
                    if (entry.Value.Active)
                    {
                        elementSet = new CalElementSet(entry.Value);
                        parent.AddKeyword(elementSet);
                    }
                }
            }
        }
        static private void AppendSurfaces(FeModel model, CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                CalSurface surface;
                foreach (var entry in model.Mesh.Surfaces)
                {
                    if (entry.Value.Active)
                    {
                        surface = new CalSurface(entry.Value);
                        parent.AddKeyword(surface);
                    }
                }
            }
        }



        static private void AppendMaterials(FeModel model, CalculixKeyword parent) 
        {
            CalMaterial material;
            foreach (var entry in model.Materials)
            {
                if (entry.Value.Active)
                {
                    material = new CalMaterial(entry.Value);
                    parent.AddKeyword(material);

                    foreach (var property in entry.Value.Properties)
                    {
                        if (property is Density) material.AddKeyword(new CalDensity(property as Density));
                        else if (property is Elastic) material.AddKeyword(new CalElastic(property as Elastic));
                        else if (property is Plastic) material.AddKeyword(new CalPlastic(property as Plastic));
                        else throw new NotImplementedException();
                    }
                }
            }
            
        }
        static private void AppendSections(FeModel model, CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                CalSolidSection solidSection;
                foreach (var entry in model.Sections)
                {
                    if (entry.Value.Active)
                    {
                        if (entry.Value is SolidSection)
                        {
                            solidSection = new CalSolidSection(entry.Value as SolidSection);
                            parent.AddKeyword(solidSection);
                        }
                        else throw new NotImplementedException();
                    }
                }
            }
        }
        static private void AppendConstraints(FeModel model, Dictionary<string, int[]> referencePointsNodeIds, CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                foreach (var entry in model.Constraints)
                {
                    if (entry.Value.Active)
                    {
                        if (entry.Value is RigidBody rigidBody)
                        {
                            string surfaceNodeSetName = null;
                            if (rigidBody.RegionType == CaeGlobals.RegionTypeEnum.SurfaceName) surfaceNodeSetName = model.Mesh.Surfaces[rigidBody.RegionName].NodeSetName;
                            CalRigidBody calRigidBody = new CalRigidBody(rigidBody, referencePointsNodeIds, surfaceNodeSetName);
                            parent.AddKeyword(calRigidBody);
                        }
                        else if (entry.Value is Tie tie)
                        {
                            CalTie calTie = new CalTie(tie);
                            parent.AddKeyword(calTie);
                        }
                        else throw new NotImplementedException();
                    }
                }
            }
        }

        static private void AppendSteps(FeModel model, Dictionary<string, int[]> referencePointsNodeIds, CalculixKeyword parent)
        {
            CalTitle title;
            foreach (var entry in model.StepCollection.Steps)
            {
                if (entry.Value is InitialStep) continue;

                if (entry.Value.Active)
                {
                    title = new CalTitle(entry.Value.Name, "");
                    parent.AddKeyword(title);

                    Step step = entry.Value;
                    CalStep calStep = new CalStep(step);
                    title.AddKeyword(calStep);

                    if (entry.Value is StaticStep staticStep)
                    {
                        CalStaticStep calStaticStep = new CalStaticStep(staticStep);
                        calStep.AddKeyword(calStaticStep);
                    }
                    else if (entry.Value is FrequencyStep frequencyStep)
                    {
                        CalFrequencyStep calFrequencyStep = new CalFrequencyStep(frequencyStep);
                        calStep.AddKeyword(calFrequencyStep);
                    }
                    else throw new NotImplementedException();

                    // boundary conditions
                    title = new CalTitle("Boundary conditions", "*Boundary, op=NEW");
                    calStep.AddKeyword(title);

                    foreach (var bcEntry in step.BoundaryConditions)
                    {
                        if (bcEntry.Value.Active)
                        {
                            AppendBoundaryCondition(model, bcEntry.Value, referencePointsNodeIds, title);
                        }
                    }

                    // loads
                    title = new CalTitle("Loads", "*Dload, op=NEW" + Environment.NewLine + "*Cload, op=NEW");
                    calStep.AddKeyword(title);

                    foreach (var loadEntry in step.Loads)
                    {
                        if (loadEntry.Value.Active)
                        {
                            AppendLoad(model, loadEntry.Value, referencePointsNodeIds, title);
                        }
                    }

                    // field outputs
                    title = new CalTitle("Field outputs", "");
                    calStep.AddKeyword(title);

                    foreach (var fieldOutputEntry in step.FieldOutputs)
                    {
                        if (fieldOutputEntry.Value.Active)
                        {
                            AppendFieldOutput(model, fieldOutputEntry.Value, title);
                        }
                    }

                    title = new CalTitle("End step", "");
                    calStep.AddKeyword(title);
                    CalEndStep endStep = new CalEndStep();
                    title.AddKeyword(endStep);
                }
            }
        }
        static private void AppendBoundaryCondition(FeModel model, BoundaryCondition boundaryCondition, Dictionary<string, int[]> referencePointsNodeIds, CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                if (boundaryCondition is DisplacementRotation)
                {
                    DisplacementRotation dispRot = (DisplacementRotation)boundaryCondition;
                    string surfaceNodeSetName = null;
                    if (dispRot.RegionType == CaeGlobals.RegionTypeEnum.SurfaceName) surfaceNodeSetName = model.Mesh.Surfaces[dispRot.RegionName].NodeSetName;
                    CalDisplacementRotation displacementRotation = new CalDisplacementRotation(boundaryCondition as DisplacementRotation, referencePointsNodeIds, surfaceNodeSetName);
                    parent.AddKeyword(displacementRotation);
                }
                else throw new NotImplementedException();
            }
        }
        static private void AppendLoad(FeModel model, Load load, Dictionary<string, int[]> referencePointsNodeIds, CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                if (load is CLoad cl)
                {
                    CalCLoad cload = new CalCLoad(cl, referencePointsNodeIds);
                    parent.AddKeyword(cload);
                }
                else if (load is DLoad dl)
                {
                    CalDLoad dload = new CalDLoad(model.Mesh.Surfaces, dl);
                    parent.AddKeyword(dload);
                }
                else if (load is MomentLoad ml)
                {
                    CalMomentLoad mload = new CalMomentLoad(ml, referencePointsNodeIds);
                    parent.AddKeyword(mload);
                }
                else if (load is STLoad stl)
                {
                    CalSTLoad stload = new CalSTLoad(model, stl, referencePointsNodeIds);
                    parent.AddKeyword(stload);
                }
                else if (load is GravityLoad gl)
                {
                    CalGravityLoad gload = new CalGravityLoad(gl);
                    parent.AddKeyword(gload);
                }
                else if (load is CentrifLoad cfl)
                {
                    CalCentrifLoad cload = new CalCentrifLoad(cfl);
                    parent.AddKeyword(cload);
                }
                else throw new NotImplementedException();
            }
        }
        static private void AppendCloadData(StringBuilder sb, CLoad cLoad, Dictionary<string, int[]> referencePointsNodeIds, FeModel model)
        {
            int[] rpNodeIds = null;
            if (cLoad.RegionType == CaeGlobals.RegionTypeEnum.ReferencePointName) rpNodeIds = referencePointsNodeIds[cLoad.RegionName];

            List<int> directions = new List<int>();
            if (cLoad.F1 != 0) directions.Add(1);
            if (cLoad.F2 != 0) directions.Add(2);
            if (cLoad.F3 != 0) directions.Add(3);

            foreach (var dir in directions)
            {
                if (cLoad.RegionType == CaeGlobals.RegionTypeEnum.NodeId)
                    sb.AppendFormat("{0}, {1}, {2}", cLoad.NodeId, dir, cLoad.GetDirection(dir - 1).ToString());
                else if (cLoad.RegionType == CaeGlobals.RegionTypeEnum.NodeSetName) // node set
                    sb.AppendFormat("{0}, {1}, {2}", cLoad.RegionName, dir, cLoad.GetDirection(dir - 1).ToString());
                else if (cLoad.RegionType == CaeGlobals.RegionTypeEnum.ReferencePointName) // reference point
                    sb.AppendFormat("{0}, {1}, {2}", rpNodeIds[0], dir, cLoad.GetDirection(dir - 1).ToString());
                
                sb.AppendLine();
            }
        }
        static private void AppendMomentLoadData(StringBuilder sb, MomentLoad momentLoad, Dictionary<string, int[]> referencePointsNodeIds, FeModel model)
        {
            int[] rpNodeIds = null;
            if (momentLoad.RegionType == CaeGlobals.RegionTypeEnum.ReferencePointName) rpNodeIds = referencePointsNodeIds[momentLoad.RegionName];

            List<int> directions = new List<int>();
            if (momentLoad.M1 != 0) directions.Add(1);
            if (momentLoad.M2 != 0) directions.Add(2);
            if (momentLoad.M3 != 0) directions.Add(3);

            foreach (var dir in directions)
            {
                if (momentLoad.RegionType == CaeGlobals.RegionTypeEnum.NodeId)
                    sb.AppendFormat("{0}, {1}, {2}", momentLoad.NodeId, dir, momentLoad.GetDirection(dir - 1).ToString());
                else if (momentLoad.RegionType == CaeGlobals.RegionTypeEnum.ReferencePointName) // reference point
                    sb.AppendFormat("{0}, {1}, {2}", rpNodeIds[1], dir, momentLoad.GetDirection(dir - 1).ToString());

                sb.AppendLine();
            }
        }

        static private void AppendFieldOutput(FeModel model, FieldOutput fieldOutput, CalculixKeyword parent)
        {
            if (fieldOutput is NodalFieldOutput)
            {
                CalNodeFile nodeFile = new CalNodeFile(fieldOutput as NodalFieldOutput);
                parent.AddKeyword(nodeFile);
            }
            else if (fieldOutput is ElementFieldOutput)
            {
                CalElFile elFile = new CalElFile(fieldOutput as ElementFieldOutput);
                parent.AddKeyword(elFile);
            }
            else throw new NotImplementedException();
        }

        





        static void AppendTitle(StringBuilder sb, string title)
        {
            sb.AppendLine("************************************************************");
            sb.AppendLine("** " + title);
            sb.AppendLine("************************************************************");
        }
    }
}
