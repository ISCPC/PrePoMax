using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.Runtime.Serialization;
using Calculix = FileInOut.Output.Calculix;


namespace CaeModel
{
    [Serializable]
    public class FeModel : ISerializable
    {
        // Variables                                                                                                                
        private FeMesh _geometry;                                                               //ISerializable
        private FeMesh _mesh;                                                                   //ISerializable
        private OrderedDictionary<string, Material> _materials;                                 //ISerializable
        private OrderedDictionary<string, Section> _sections;                                   //ISerializable
        private OrderedDictionary<string, Constraint> _constraints;                             //ISerializable
        private StepCollection _stepCollection;                                                 //ISerializable
        private OrderedDictionary<int[], Calculix.CalculixUserKeyword> _calculixUserKeywords;   //ISerializable


        // Properties                                                                                                               
        public string Name { get; set; }
        public FeMesh Geometry { get { return _geometry; } }
        public FeMesh Mesh { get { return _mesh; } }
        public IDictionary<string, Material> Materials { get { return _materials; } }
        public IDictionary<string, Section> Sections { get { return _sections; } }
        public IDictionary<string, Constraint> Constraints { get { return _constraints; } }
        public StepCollection StepCollection { get { return _stepCollection; } }
        public OrderedDictionary<int[], Calculix.CalculixUserKeyword> CalculixUserKeywords 
        { 
            get { return _calculixUserKeywords; } 
            set 
            {
                _calculixUserKeywords = value;
            } 
        }


        // Constructors                                                                                                             
        public FeModel(string name)
        {
            Name = name;
            _materials = new OrderedDictionary<string, Material>();
            _sections = new OrderedDictionary<string, Section>();
            _constraints = new OrderedDictionary<string, Constraint>();
            _stepCollection = new StepCollection();
        }

        public FeModel(SerializationInfo info, StreamingContext context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "<Name>k__BackingField":               // Compatibility for version v.0.5.1
                    case "_name":
                        Name = (string)entry.Value; break;
                    case "_geometry":
                        _geometry = (FeMesh)entry.Value; break;
                    case "_mesh":
                        _mesh = (FeMesh)entry.Value; break;
                    case "_materials":
                        if (entry.Value is Dictionary<string, Material> md)
                        {
                            // Compatibility for version v.0.5.1
                            md.OnDeserialization(null);
                            _materials = new OrderedDictionary<string, Material>(md);
                        }
                        else if (entry.Value is OrderedDictionary<string, Material> mod) _materials = mod;
                        else if (entry.Value == null) _materials = null;
                        else throw new NotSupportedException();
                        break;
                    case "_sections":
                        if (entry.Value is Dictionary<string, Section> sd)
                        {
                            // Compatibility for version v.0.5.1
                            sd.OnDeserialization(null);
                            _sections = new OrderedDictionary<string, Section>(sd);
                        }
                        else if (entry.Value is OrderedDictionary<string, Section> sod) _sections = sod;
                        else if (entry.Value == null) _sections = null;
                        else throw new NotSupportedException();
                        break;
                    case "_constraints":
                        if (entry.Value is Dictionary<string, Constraint> cd)   
                        {
                            // Compatibility for version v.0.5.1
                            cd.OnDeserialization(null);
                            _constraints = new OrderedDictionary<string, Constraint>(cd);
                        }
                        else if (entry.Value is OrderedDictionary<string, Constraint> cod) _constraints = cod;
                        else if (entry.Value == null) _constraints = null;
                        else throw new NotSupportedException();
                        break;
                    case "_stepCollection":
                        _stepCollection = (StepCollection)entry.Value; break;
                    case "_calculixUserKeywords":
                        if (entry.Value is Dictionary<int[], Calculix.CalculixUserKeyword> cukd)
                        {
                            // Compatibility for version v.0.5.1
                            cukd.OnDeserialization(null);
                            _calculixUserKeywords = new OrderedDictionary<int[], Calculix.CalculixUserKeyword>(cukd);
                        }
                        else if (entry.Value is OrderedDictionary<int[], Calculix.CalculixUserKeyword> cukod)
                            _calculixUserKeywords = cukod;
                        else if (entry.Value == null) _calculixUserKeywords = null;
                        else throw new NotSupportedException();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        // Static methods
        public static void WriteToFile(FeModel model, System.IO.BinaryWriter bw)
        {
            // write geometry
            if (model == null || model.Geometry == null)
            {
                bw.Write((int)0);
            }
            else
            {
                bw.Write((int)1);
                FeMesh.WriteToBinaryFile(model.Geometry, bw);
            }

            // write mesh
            if (model == null || model.Mesh == null)
            {
                bw.Write((int)0);
            }
            else
            {
                bw.Write((int)1);
                FeMesh.WriteToBinaryFile(model.Mesh, bw);
            }
        }
        public static void ReadFromFile(FeModel model, System.IO.BinaryReader br)
        {
            // read geometry
            int geometryExists = br.ReadInt32();
            if (geometryExists == 1)
            {
                FeMesh.ReadFromBinaryFile(model.Geometry, br);
            }

            // read mesh
            int meshExists = br.ReadInt32();
            if (meshExists == 1)
            {
                FeMesh.ReadFromBinaryFile(model.Mesh, br);
            }
        }


        // Methods                                                                                                                  
        public string[] CheckValidity(List<Tuple<NamedClass, string>> items)
        {
            // Tuple<NamedClass, string>   ...   Tuple<invalidItem, stepName>

            if (_mesh == null) return new string[0];

            List<string> invalidItems = new List<string>();
            bool valid;
            invalidItems.AddRange(_mesh.CheckValidity(items));

            // Sections
            Section section;
            foreach (var entry in _sections)
            {
                section = entry.Value;
                valid = _materials.ContainsKey(section.MaterialName) 
                        && ((section.RegionType == RegionTypeEnum.PartName && _mesh.Parts.ContainsKey(section.RegionName))
                        || (section.RegionType == RegionTypeEnum.ElementSetName && _mesh.ElementSets.ContainsValidKey(section.RegionName)));
                SetItemValidity(section, valid, items);
                if (!valid && section.Active) invalidItems.Add("Section: " + section.Name);
            }

            // Constraints
            Constraint constraint;
            foreach (var entry in _constraints)
            {
                constraint = entry.Value;
                if (constraint is RigidBody rb)
                {
                    valid = (_mesh.ReferencePoints.ContainsValidKey(rb.ReferencePointName))
                            && ((rb.RegionType == RegionTypeEnum.NodeSetName && _mesh.NodeSets.ContainsValidKey(rb.RegionName))
                            || (rb.RegionType == RegionTypeEnum.SurfaceName && _mesh.Surfaces.ContainsKey(rb.RegionName)));
                    SetItemValidity(rb, valid, items);
                    if (!valid && constraint.Active) invalidItems.Add("Constraint: " + constraint.Name);
                }
                else if (constraint is Tie t)
                {
                    valid = (_mesh.Surfaces.ContainsValidKey(t.SlaveSurfaceName))
                            && (_mesh.Surfaces.ContainsValidKey(t.MasterSurfaceName))
                            && (t.SlaveSurfaceName != t.MasterSurfaceName);
                    SetItemValidity(t, valid, items);
                    if (!valid && constraint.Active) invalidItems.Add("Constraint: " + constraint.Name);
                }
                else throw new NotSupportedException();
            }

            // Steps
            HistoryOutput historyOutput;
            BoundaryCondition boundaryCondition;
            Load load;
            FeSurface s;
            foreach (var step in _stepCollection.StepsList)
            {
                // History output
                foreach (var hoEntry in step.HistoryOutputs)
                {
                    historyOutput = hoEntry.Value;
                    if (historyOutput is NodalHistoryOutput nho)
                    {
                        valid = (nho.RegionType == RegionTypeEnum.NodeSetName && _mesh.NodeSets.ContainsValidKey(nho.RegionName))
                                || (nho.RegionType == RegionTypeEnum.SurfaceName && _mesh.Surfaces.ContainsValidKey(nho.RegionName))
                                || (nho.RegionType == RegionTypeEnum.ReferencePointName && _mesh.ReferencePoints.ContainsValidKey(nho.RegionName));
                        SetItemValidity(nho, valid, items);
                        if (!valid && nho.Active) invalidItems.Add("History output: " + step.Name + ", " + nho.Name);
                    }
                    else if (historyOutput is ElementHistoryOutput eho)
                    {
                        valid = _mesh.ElementSets.ContainsValidKey(eho.RegionName);
                        SetItemValidity(eho, valid, items);
                        if (!valid && eho.Active) invalidItems.Add("History output: " + step.Name + ", " + eho.Name);
                    }
                    else throw new NotSupportedException();
                }

                // Boundary conditions
                foreach (var bcEntry in step.BoundaryConditions)
                {
                    boundaryCondition = bcEntry.Value;
                    if (boundaryCondition is DisplacementRotation dr)
                    {
                        valid = (dr.RegionType == RegionTypeEnum.NodeSetName && _mesh.NodeSets.ContainsValidKey(dr.RegionName))
                                || (dr.RegionType == RegionTypeEnum.SurfaceName && (_mesh.Surfaces.ContainsValidKey(dr.RegionName)))
                                || (dr.RegionType == RegionTypeEnum.ReferencePointName && (_mesh.ReferencePoints.ContainsValidKey(dr.RegionName)));
                        SetItemValidity(dr, valid, items);
                        if (!valid && dr.Active) invalidItems.Add("Boundary condition: " + step.Name + ", " + dr.Name);
                    }
                    else throw new NotSupportedException();
                }

                // Loads
                foreach (var loadEntry in step.Loads)
                {
                    load = loadEntry.Value;
                    if (load is CLoad cl)
                    {
                        valid = (cl.RegionType == RegionTypeEnum.NodeSetName && _mesh.NodeSets.ContainsValidKey(cl.RegionName))
                                || (cl.RegionType == RegionTypeEnum.ReferencePointName && (_mesh.ReferencePoints.ContainsValidKey(cl.RegionName)));
                        SetItemValidity(cl, valid, items);
                        if (!valid && cl.Active) invalidItems.Add("Load: " + step.Name + ", " + cl.Name);
                    }
                    else if (load is MomentLoad ml)
                    {
                        valid = (ml.RegionType == RegionTypeEnum.NodeSetName && _mesh.NodeSets.ContainsValidKey(ml.RegionName))
                                || (ml.RegionType == RegionTypeEnum.ReferencePointName && (_mesh.ReferencePoints.ContainsValidKey(ml.RegionName)));
                        SetItemValidity(ml, valid, items);
                        if (!valid && ml.Active) invalidItems.Add("Load: " + step.Name + ", " + ml.Name);
                    }
                    else if (load is DLoad dl)
                    {
                        valid = (_mesh.Surfaces.TryGetValue(dl.SurfaceName, out s) && s.Valid);
                        SetItemValidity(dl, valid, items);
                        if (!valid && dl.Active) invalidItems.Add("Load: " + step.Name + ", " + dl.Name);
                    }
                    else if (load is STLoad stl)
                    {
                        valid = (_mesh.Surfaces.TryGetValue(stl.SurfaceName, out s) && s.Valid);
                        SetItemValidity(stl, valid, items);
                        if (!valid && stl.Active) invalidItems.Add("Load: " + step.Name + ", " + stl.Name);
                    }
                    else if (load is GravityLoad gl)
                    {
                        valid = (gl.RegionType == RegionTypeEnum.PartName && _mesh.Parts.ContainsValidKey(gl.RegionName))
                                || (gl.RegionType == RegionTypeEnum.ElementSetName && (_mesh.ElementSets.ContainsValidKey(gl.RegionName)));
                        SetItemValidity(gl, valid, items);
                        if (!valid && gl.Active) invalidItems.Add("Load: " + step.Name + ", " + gl.Name);
                    }
                    else if (load is CentrifLoad cf)
                    {
                        valid = (cf.RegionType == RegionTypeEnum.PartName && _mesh.Parts.ContainsValidKey(cf.RegionName))
                                || (cf.RegionType == RegionTypeEnum.ElementSetName && (_mesh.ElementSets.ContainsValidKey(cf.RegionName)));
                        SetItemValidity(cf, valid, items);
                        if (!valid && cf.Active) invalidItems.Add("Load: " + step.Name + ", " + cf.Name);
                    }
                    else throw new NotSupportedException();
                }
            }

            return invalidItems.ToArray();
        }
        private void SetItemValidity(NamedClass item, bool validity, List<Tuple<NamedClass, string>> items)
        {
            if (item.Valid != validity)
            {
                item.Valid = validity;
                items.Add(new Tuple<NamedClass, string>(item, null));
            }
        }


        public int CheckSectionAssignments()
        {
            HashSet<int> elementIds = new HashSet<int>();
            foreach (var entry in _sections)
            {
                if (entry.Value.RegionType == RegionTypeEnum.PartName)
                {
                    elementIds.UnionWith(_mesh.Parts[entry.Value.RegionName].Labels);
                }
                else if (entry.Value.RegionType == RegionTypeEnum.ElementSetName)
                {
                    elementIds.UnionWith(_mesh.ElementSets[entry.Value.RegionName].Labels);
                }
                else throw new NotSupportedException();
            }

            var notSpecifiedIds = _mesh.Elements.Keys.Except(elementIds);

            return notSpecifiedIds.Count();
        }

        public void RemoveLostUserKeywords(Action<int> SetNumberOfUserKeywords)
        {
            FileInOut.Output.CalculixFileWriter.RemoveLostUserKeywords(this);
            SetNumberOfUserKeywords?.Invoke(_calculixUserKeywords.Count);
        }
        public void UpdateUserKeywordIndices_(object itemToRemove, Action<int> SetNumberOfUserKeywords)
        {
            int parentLevel;
            int itemLevel;
            int[] userIndices;
            List<int[]> allKeywordIndices = FileInOut.Output.CalculixFileWriter.GetKeywordIndices(this, itemToRemove);
            OrderedDictionary<int[], Calculix.CalculixUserKeyword> userKeywordsCopy;
            userKeywordsCopy = new OrderedDictionary<int[], Calculix.CalculixUserKeyword>();

            foreach (var keywordIndices in allKeywordIndices)
            {
                userKeywordsCopy.Clear();
                foreach (var entry in _calculixUserKeywords)
                {
                    userIndices = entry.Key;
                    if (IndicesOfTheSameParent(userIndices, keywordIndices, out parentLevel))
                    {
                        itemLevel = parentLevel + 1;
                        if (userIndices[itemLevel] == keywordIndices[itemLevel])
                        {
                            // delete keyword
                        }
                        else if (userIndices[itemLevel] > keywordIndices[itemLevel])
                        {
                            // renumber
                            userIndices[itemLevel]--;
                            userKeywordsCopy.Add(userIndices, entry.Value);
                        }
                        else
                        {
                            // copy
                            userKeywordsCopy.Add(entry.Key, entry.Value);
                        }
                    }
                    else
                    {
                        // copy
                        userKeywordsCopy.Add(entry.Key, entry.Value);
                    }
                }
                _calculixUserKeywords = userKeywordsCopy;
            }
            SetNumberOfUserKeywords?.Invoke(_calculixUserKeywords.Count);
        }
        private bool IndicesOfTheSameParent(int[] userIndices, int[] deletedItemIndces, out int parentLevel)
        {
            // 6, 0, 2  ... userIndices
            // 6, 0     ... deletedItemIndces
            parentLevel = deletedItemIndces.Length - 1 - 1;
            for (int i = 0; i <= parentLevel; i++)
            {
                if (userIndices[i] != deletedItemIndces[i]) return false;
            }
            return true;
        }

        // Input //
        public bool ImportGeometryFromStlFile(string fileName)
        {
            FeMesh mesh = FileInOut.Input.StlFileReader.Read(fileName);

            bool noErrors = true;
            foreach (var entry in mesh.Parts)
            {
                if (entry.Value is GeometryPart gp && gp.ErrorElementIds != null)
                {
                    noErrors = false;
                    break;
                }
            }

            ImportGeometry(mesh);

            return noErrors;
        }
        public string ImportGeometryFromBrepFile(string visFileName, string brepFileName, bool mergeParts)
        {
            FeMesh mesh = FileInOut.Input.VisFileReader.Read(visFileName);

            if (mesh.Parts.Count > 1)
                throw new Exception("The geometry contains more than one part.");
            else
            {
                if (mesh.Parts.GetValueByIndex(0) is GeometryPart gp)
                {
                    gp.CADFileData = System.IO.File.ReadAllText(brepFileName);
                    gp.SmoothShaded = true;
                }
            }

            ImportGeometry(mesh);

            return null;
        }
        public void ImportMeshFromUnvFile(string fileName)
        {
            FeMesh mesh = FileInOut.Input.UnvFileReader.Read(fileName, FileInOut.Input.ElementsToImport.Solid);

            ImportMesh(mesh);
        }
        public void ImportMeshFromVolFile(string fileName)
        {
            FeMesh mesh = FileInOut.Input.VolFileReader.Read(fileName, FileInOut.Input.ElementsToImport.Solid);

            ImportMesh(mesh);
        }
        public void ImportGeneratedMeshFromVolFile(string fileName, string partName, bool convertToSecondorder)
        {
            // called after meshing in PrePoMax
            FeMesh mesh = FileInOut.Input.VolFileReader.Read(fileName, 
                                                             FileInOut.Input.ElementsToImport.Solid,
                                                             convertToSecondorder);
            // Rename the imported part
            string[] partNames = mesh.Parts.Keys.ToArray();
            if (partNames.Length != 1) throw new Exception("Mesh file does not contain exactly one part.");
            BasePart part = mesh.Parts[partNames[0]];
            part.Name = partName;
            mesh.Parts.Remove(partNames[0]);
            mesh.Parts.Add(part.Name, part);
            ImportMesh(mesh);
            // recolor parts at the end of the import
            if (_geometry.Parts.ContainsKey(partName)) part.Color = _geometry.Parts[partName].Color;
        }
        public List<string> ImportMeshFromInpFile(string fileName, Action<string> WriteDataToOutput)
        {
            FileInOut.Input.InpFileReader.Read(fileName, FileInOut.Input.ElementsToImport.Solid, this, WriteDataToOutput);
            return FileInOut.Input.InpFileReader.Errors;
        }
        private void ImportGeometry(FeMesh mesh)
        {
            if (_geometry == null)
            {
                mesh.ResetPartsColor();
                _geometry = mesh;
            }
            else
            {
                _geometry.AddMesh(mesh);
            }
        }
        public void ImportMesh(FeMesh mesh)
        {
            if (_mesh == null)
            {
                mesh.ResetPartsColor();
                _mesh = mesh;
            }
            else
            {
                _mesh.AddMesh(mesh);
            }
        }

        // Loads
        public CLoad[] GetNodalLoadsFromSurfaceTraction(STLoad load)
        {
            FeElement element;
            double A;
            int nodeId;
            int[] nodeIds;
            double[] equForces;
            Dictionary<int, double> nodalForces = new Dictionary<int, double>();

            double aSum = 0;

            FeSurface surface = _mesh.Surfaces[load.SurfaceName];
            foreach (var entry in surface.ElementFaces)
            {
                foreach (var elementId in _mesh.ElementSets[entry.Value].Labels)
                {
                    element = _mesh.Elements[elementId];
                    A = element.GetArea(entry.Key, _mesh.Nodes);
                    aSum += A;
                    nodeIds = element.GetNodeIdsFromFaceName(entry.Key);
                    equForces = element.GetEquivalentForcesFromFaceName(entry.Key);

                    for (int i = 0; i < nodeIds.Length; i++)
                    {
                        nodeId = nodeIds[i];
                        if (nodalForces.ContainsKey(nodeId)) nodalForces[nodeId] += A * equForces[i];
                        else nodalForces.Add(nodeId, A * equForces[i]);
                    }
                }
            }

            List<CLoad> loads = new List<CLoad>();
            foreach (var entry in nodalForces)
            {
                if (entry.Value != 0 && (load.F1 != 0 || load.F2 != 0 || load.F3 != 0))
                {
                    loads.Add(new CLoad("_cLoad_" + entry.Key.ToString(), entry.Key,
                                        load.F1 / surface.Area * entry.Value,
                                        load.F2 / surface.Area * entry.Value,
                                        load.F3 / surface.Area * entry.Value));
                }
            }

            return loads.ToArray();
        }

        // ISerialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // using typeof() works also for null fields
            info.AddValue("_name", Name, typeof(string));
            info.AddValue("_geometry", _geometry, typeof(FeMesh));
            info.AddValue("_mesh", _mesh, typeof(FeMesh));
            info.AddValue("_materials", _materials, typeof(OrderedDictionary<string, Material>));
            info.AddValue("_sections", _sections, typeof(OrderedDictionary<string, Section>));
            info.AddValue("_constraints", _constraints, typeof(OrderedDictionary<string, Constraint>));
            info.AddValue("_stepCollection", _stepCollection, typeof(StepCollection));
            info.AddValue("_calculixUserKeywords", _calculixUserKeywords, typeof(OrderedDictionary<int[], Calculix.CalculixUserKeyword>));
        }
    }
}
