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
            // Only keywords from the model, not user keywords
            // Allways add a title keyword to get all possible keyword types to the keyword editor
            //
            // Collect pre-tension loads
            string name;
            List<PreTensionLoad> preTensionLoadsList;
            OrderedDictionary<string, List<PreTensionLoad>> preTensionLoads = new OrderedDictionary<string, List<PreTensionLoad>>();
            foreach (var step in model.StepCollection.StepsList)
            {
                foreach (var entry in step.Loads)
                {
                    if (entry.Value is PreTensionLoad ptl)
                    {
                        name = ptl.SurfaceName;
                        if (!ptl.AutoComputeDirection) name += "@" + ptl.X.ToString() + ptl.Y.ToString() + ptl.Z.ToString();
                        //
                        if (preTensionLoads.TryGetValue(name, out preTensionLoadsList)) preTensionLoadsList.Add(ptl);
                        else preTensionLoads.Add(name, new List<PreTensionLoad>() { ptl });
                    }
                }
            }
            // Prepare reference points
            Dictionary<string, int[]> referencePointsNodeIds = new Dictionary<string, int[]>();
            if (model.Mesh != null)
            {
                // Fill reference point nodes
                int id = model.Mesh.MaxNodeId;
                foreach (var entry in model.Mesh.ReferencePoints)
                {
                    referencePointsNodeIds.Add(entry.Key, new int[] { id + 1, id + 2 });
                    id += 2;
                }
                foreach (var entry in preTensionLoads)
                {
                    referencePointsNodeIds.Add(entry.Key, new int[] { id + 1 });
                    id ++;
                }
            }
            //
            CalTitle title;
            List<CalculixKeyword> keywords = new List<CalculixKeyword>();
            // Heading
            title = new CalTitle("Heading", "");
            keywords.Add(title);
            AppendHeading(model, title);
            // Submodel
            string[] nodeSetNames = GetAllSubmodelNodeSetNames(model);
            if (nodeSetNames.Length > 0)
            {
                title = new CalTitle("Submodel", "");
                keywords.Add(title);
                AppendSubmodel(model, nodeSetNames, title);
            }
            // Nodes
            title = new CalTitle("Nodes", "");
            keywords.Add(title);
            AppendNodes(model, referencePointsNodeIds, title);
            // Elements
            title = new CalTitle("Elements", "");
            keywords.Add(title);
            AppendElements(model, title);
            // Node sets
            title = new CalTitle("Node sets", "");
            keywords.Add(title);
            AppendNodeSets(model, referencePointsNodeIds, title);
            // Element sets
            title = new CalTitle("Element sets", "");
            keywords.Add(title);
            AppendElementSets(model, title);
            // Surfaces
            title = new CalTitle("Surfaces", "");
            keywords.Add(title);
            AppendSurfaces(model, title);
            // Materials
            title = new CalTitle("Materials", "");
            keywords.Add(title);
            AppendMaterials(model, title);
            // Sections
            title = new CalTitle("Sections", "");
            keywords.Add(title);
            AppendSections(model, title);
            // Pre-tension sections
            title = new CalTitle("Pre-tension sections", "");
            keywords.Add(title);
            AppendPreTensionSections(preTensionLoads, referencePointsNodeIds, title);
            // Constraints
            title = new CalTitle("Constraints", "");
            keywords.Add(title);
            AppendConstraints(model, referencePointsNodeIds, title);
            // Steps
            title = new CalTitle("Steps", "");
            keywords.Add(title);
            AppendSteps(model, referencePointsNodeIds, title);
            //
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
                bool deactivated = false;
                CalculixKeyword keywordParent = keywords[indices[0]];
                if (keywordParent is CalDeactivated) deactivated = true;
                // Find a parent
                for (int i = 1; i < indices.Length - 1; i++)
                {
                    if (indices[i] < keywordParent.Keywords.Count)
                    {
                        keywordParent = keywordParent.Keywords[indices[i]];
                        if (keywordParent is CalDeactivated) deactivated = true;
                    }
                    else return false;
                }

                // Add the keyword
                if (keywordParent.Keywords.Count < indices[indices.Length - 1]) return false;

                if (!deactivated) keywordParent.Keywords.Insert(indices[indices.Length - 1], keyword);
                else keywordParent.Keywords.Insert(indices[indices.Length - 1], new CalDeactivated("User keyword"));
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

        static private string[] GetAllSubmodelNodeSetNames(FeModel model)
        {
            List<string> nodeSetNames = new List<string>();
            foreach (var step in model.StepCollection.StepsList)
            {
                foreach (var entry in step.BoundaryConditions)
                {
                    if (entry.Value is SubmodelBC sm)
                    {
                        if (sm.RegionType == CaeGlobals.RegionTypeEnum.SurfaceName) 
                            nodeSetNames.Add(model.Mesh.Surfaces[sm.RegionName].NodeSetName);
                        else nodeSetNames.Add(sm.RegionName);
                    }
                } 
            }
            return nodeSetNames.ToArray();
        }

        static private void AppendHeading(FeModel model, CalculixKeyword parent)
        {
            CalHeading heading = new CalHeading(model.Name);
            parent.AddKeyword(heading);
        }
        static private void AppendSubmodel(FeModel model, string[] nodeSetNames, CalculixKeyword parent)
        {
            //*Submodel, TYPE = NODE, INPUT = Model.frd
            CalSubmodel submodel = new CalSubmodel(model.Properties.GlobalResultsFileName, nodeSetNames);
            parent.AddKeyword(submodel);
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
        static private void AppendNodeSets(FeModel model, Dictionary<string, int[]> referencePointsNodeIds, CalculixKeyword parent)
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
                    else parent.AddKeyword(new CalDeactivated(entry.Value.Name));
                }
                FeReferencePoint rp;
                FeNodeSet rpNodeSet;
                foreach (var entry in referencePointsNodeIds)
                {
                    if (model.Mesh.ReferencePoints.TryGetValue(entry.Key, out rp))
                    {
                        rp.RefNodeSetName = rp.Name + FeReferencePoint.RefName + entry.Value[0];
                        rp.RotNodeSetName = rp.Name + FeReferencePoint.RotName + entry.Value[1];

                        rpNodeSet = new FeNodeSet(rp.RefNodeSetName, new int[] { entry.Value[0] });
                        nodeSet = new CalNodeSet(rpNodeSet);
                        parent.AddKeyword(nodeSet);

                        rpNodeSet = new FeNodeSet(rp.RotNodeSetName, new int[] { entry.Value[1] });
                        nodeSet = new CalNodeSet(rpNodeSet);
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
                    else parent.AddKeyword(new CalDeactivated(entry.Value.Name));
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
                    else parent.AddKeyword(new CalDeactivated(entry.Value.Name));
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
                    //
                    foreach (var property in entry.Value.Properties)
                    {
                        if (property is Density) material.AddKeyword(new CalDensity(property as Density));
                        else if (property is Elastic) material.AddKeyword(new CalElastic(property as Elastic));
                        else if (property is Plastic) material.AddKeyword(new CalPlastic(property as Plastic));
                        else throw new NotImplementedException();
                    }
                }
                else parent.AddKeyword(new CalDeactivated(entry.Value.Name));
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
                        if (entry.Value is SolidSection ss)
                        {
                            if (ss.RegionType == RegionTypeEnum.Selection)
                                solidSection = new CalSolidSection(ss, model.Mesh.GetPartNamesByIds(ss.CreationIds));
                            else solidSection = new CalSolidSection(ss);
                            parent.AddKeyword(solidSection);
                        }
                        else throw new NotImplementedException();
                    }
                    else parent.AddKeyword(new CalDeactivated(entry.Value.Name));
                }
            }
        }
        static private void AppendPreTensionSections(OrderedDictionary<string, List<PreTensionLoad>> preTensionLoads,
                                                     Dictionary<string, int[]> referencePointsNodeIds,
                                                     CalculixKeyword parent)
        {
            if (preTensionLoads != null)
            {
                int nodeId;
                bool atLeastOneActive;
                PreTensionLoad ptl;
                foreach (var preTensionOnSurfaceEntry in preTensionLoads)
                {
                    atLeastOneActive = false;
                    foreach (var item in preTensionOnSurfaceEntry.Value) atLeastOneActive |= item.Active;
                    // Take the first one since all the others are the same
                    ptl = preTensionOnSurfaceEntry.Value[0];
                    if (atLeastOneActive)
                    {
                        nodeId = referencePointsNodeIds[preTensionOnSurfaceEntry.Key][0];
                        CalPreTensionSection preTension;
                        if (ptl.AutoComputeDirection) preTension = new CalPreTensionSection(ptl.SurfaceName, nodeId);
                        else preTension = new CalPreTensionSection(ptl.SurfaceName, nodeId, ptl.X, ptl.Y, ptl.Z);
                        parent.AddKeyword(preTension);
                    }
                    else parent.AddKeyword(new CalDeactivated("Pre-tension " + ptl.SurfaceName));
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
                    else parent.AddKeyword(new CalDeactivated(entry.Value.Name));
                }
            }
        }

        static private void AppendSteps(FeModel model, Dictionary<string, int[]> referencePointsNodeIds, CalculixKeyword parent)
        {
            CalTitle title;
            foreach (var step in model.StepCollection.StepsList)
            {
                if (step is InitialStep) continue;
                //
                title = new CalTitle(step.Name, "");
                parent.AddKeyword(title);
                //
                CalculixKeyword calStep;
                if (step.Active) calStep = new CalStep(step);
                else calStep = new CalDeactivated(step.Name);
                title.AddKeyword(calStep);
                // Step type
                if (step.Active)
                {
                    if (step is StaticStep staticStep)
                    {
                        CalStaticStep calStaticStep = new CalStaticStep(staticStep);
                        calStep.AddKeyword(calStaticStep);
                    }
                    else if (step is FrequencyStep frequencyStep)
                    {
                        CalFrequencyStep calFrequencyStep = new CalFrequencyStep(frequencyStep);
                        calStep.AddKeyword(calFrequencyStep);
                    }
                    else if (step is BuckleStep buckleStep)
                    {
                        CalBuckleStep calBuckleStep = new CalBuckleStep(buckleStep);
                        calStep.AddKeyword(calBuckleStep);
                    }
                    else throw new NotImplementedException();
                }
                else calStep.AddKeyword(new CalDeactivated(step.GetType().ToString()));
                //
                // Boundary conditions
                if (step.Active) title = new CalTitle("Boundary conditions", "*Boundary, op=New");
                else title = new CalTitle("Boundary conditions", "");
                calStep.AddKeyword(title);
                //
                foreach (var bcEntry in step.BoundaryConditions)
                {
                    if (step.Active && bcEntry.Value.Active)
                        AppendBoundaryCondition(model, bcEntry.Value, referencePointsNodeIds, title);
                    else title.AddKeyword(new CalDeactivated(bcEntry.Value.Name));
                }
                // Loads
                if (step.Active && step.Loads.Count > 0)
                    title = new CalTitle("Loads", "*Dload, op=New" + Environment.NewLine + "*Cload, op=New");
                else title = new CalTitle("Loads", "");
                calStep.AddKeyword(title);
                //
                foreach (var loadEntry in step.Loads)
                {
                    if (step.Active && loadEntry.Value.Active) AppendLoad(model, step, loadEntry.Value, referencePointsNodeIds, title);
                    else title.AddKeyword(new CalDeactivated(loadEntry.Value.Name));
                }
                // History outputs
                title = new CalTitle("History outputs", "");
                calStep.AddKeyword(title);
                //
                foreach (var historyOutputEntry in step.HistoryOutputs)
                {
                    if (step.Active && historyOutputEntry.Value.Active) AppendHistoryOutput(model, historyOutputEntry.Value, title);
                    else title.AddKeyword(new CalDeactivated(historyOutputEntry.Value.Name));
                }
                // Field outputs
                title = new CalTitle("Field outputs", "");
                calStep.AddKeyword(title);
                //
                foreach (var fieldOutputEntry in step.FieldOutputs)
                {
                    if (step.Active && fieldOutputEntry.Value.Active) AppendFieldOutput(model, fieldOutputEntry.Value, title);
                    else title.AddKeyword(new CalDeactivated(fieldOutputEntry.Value.Name));
                }
                //
                title = new CalTitle("End step", "");
                calStep.AddKeyword(title);
                if (step.Active)
                {
                    CalEndStep endStep = new CalEndStep();
                    title.AddKeyword(endStep);
                }
                else title.AddKeyword(new CalDeactivated(step.Name));
            }
        }
        static private void AppendBoundaryCondition(FeModel model, BoundaryCondition boundaryCondition,
                                                    Dictionary<string, int[]> referencePointsNodeIds, CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                if (boundaryCondition is DisplacementRotation dispRot)
                {
                    string nodeSetNameOfSurface = null;
                    if (dispRot.RegionType == CaeGlobals.RegionTypeEnum.SurfaceName)
                        nodeSetNameOfSurface = model.Mesh.Surfaces[dispRot.RegionName].NodeSetName;
                    CalDisplacementRotation calDisplacementRotation = new CalDisplacementRotation(dispRot, referencePointsNodeIds, nodeSetNameOfSurface);
                    parent.AddKeyword(calDisplacementRotation);
                }
                else if (boundaryCondition is SubmodelBC sm)
                {
                    string surfaceNodeSetName = null;
                    if (sm.RegionType == CaeGlobals.RegionTypeEnum.SurfaceName)
                        surfaceNodeSetName = model.Mesh.Surfaces[sm.RegionName].NodeSetName;
                    CalSubmodelBC calSubmodelBC = new CalSubmodelBC(sm, surfaceNodeSetName);
                    parent.AddKeyword(calSubmodelBC);
                }
                else throw new NotImplementedException();
            }
        }
        static private void AppendLoad(FeModel model, Step step, Load load, Dictionary<string, int[]> referencePointsNodeIds,
                                       CalculixKeyword parent)
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
                else if (load is PreTensionLoad ptl)
                {
                    string name = ptl.SurfaceName;
                    if (!ptl.AutoComputeDirection) name += "@" + ptl.X.ToString() + ptl.Y.ToString() + ptl.Z.ToString();
                    //
                    CalculixKeyword calKey;
                    if (ptl.Type == PreTensionLoadType.Force)
                    {
                        int nodeId = referencePointsNodeIds[name][0];
                        CLoad cLoad = new CLoad(ptl.Name, nodeId, ptl.Magnitude, 0, 0);
                        calKey = new CalCLoad(cLoad, referencePointsNodeIds);
                    }
                    else if (ptl.Type == PreTensionLoadType.Displacement)
                    {
                        DisplacementRotation dr = new DisplacementRotation(ptl.Name, name, RegionTypeEnum.ReferencePointName);
                        dr.U1 = ptl.Magnitude;
                        calKey = new CalDisplacementRotation(dr, referencePointsNodeIds, null);
                    }
                    else throw new NotSupportedException();
                    //
                    parent.AddKeyword(calKey);
                }
                else throw new NotImplementedException();
            }
        }
        
        static private void AppendHistoryOutput(FeModel model, HistoryOutput historyOutput, CalculixKeyword parent)
        {
            if (historyOutput is NodalHistoryOutput nho)
            {
                CalNodePrint nodePrint;
                if (nho.RegionType == RegionTypeEnum.ReferencePointName)
                {
                    FeReferencePoint rp = model.Mesh.ReferencePoints[nho.RegionName];
                    nodePrint = new CalNodePrint(model, nho, rp.RefNodeSetName);
                    parent.AddKeyword(nodePrint);
                    nodePrint = new CalNodePrint(model, nho, rp.RotNodeSetName);
                    parent.AddKeyword(nodePrint);
                }
                else
                {
                    nodePrint = new CalNodePrint(model, nho);
                    parent.AddKeyword(nodePrint);
                }
            }
            else if (historyOutput is ElementHistoryOutput eho)
            {
                CalElPrint elPrint = new CalElPrint(eho);
                parent.AddKeyword(elPrint);
            }
            else throw new NotImplementedException();
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
