using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.ComponentModel;

namespace CaeResults
{
    [Serializable]
    public class FeResults
    {
        // Variables                                                                                                                
        [NonSerialized]
        private Dictionary<int, int> _nodeIdsLookUp;            // [globalId][resultsId] for values
        [NonSerialized]
        private Dictionary<FieldData, Field> _fields;
        [NonSerialized]
        private Dictionary<int, FeNode> _undeformedNodes;
        [NonSerialized]
        private float _scale;
        [NonSerialized]
        private int _stepId;
        [NonSerialized]
        private int _stepIncrementId;
        //
        private string _hashName;
        private string _fileName;
        private FeMesh _mesh;
        private Dictionary<int, FieldData> _fieldLookUp;
        private HistoryResults _history;
        private DateTime _dateTime;
        private UnitSystem _unitSystem;


        // Properties                                                                                                               
        public Dictionary<int, FeNode> UndeformedNodes { get { return _undeformedNodes; } }
        public string HashName { get { return _hashName; } set { _hashName = value; } }
        public string FileName { get { return _fileName; } set { _fileName = value; } }
        public FeMesh Mesh { get { return _mesh; } set { _mesh = value; } }
        public HistoryResults History { get { return _history; } set { _history = value; } }
        public DateTime DateTime { get { return _dateTime; } set { _dateTime = value; } }
        public UnitSystem UnitSystem { get { return _unitSystem; } set { _unitSystem = value; } }        


        // Constructor                                                                                                              
        public FeResults(string fileName)
        {
            _fileName = fileName;
            _hashName = Tools.GetRandomString(8);
            _mesh = null;
            _nodeIdsLookUp = null;
            _fields = new Dictionary<FieldData, Field>();
            _fieldLookUp = new Dictionary<int, FieldData>();
            _history = null;
            _unitSystem = new UnitSystem();
        }


        // Static methods                                                                                                           
        public static void WriteToFile(FeResults results, System.IO.BinaryWriter bw)
        {
            if (results == null)
            {
                bw.Write((int)0);
            }
            else
            {
                bw.Write((int)1);
                // Mesh
                Dictionary<int, FeNode> tmp = results.Mesh.Nodes;
                results.Mesh.Nodes = results._undeformedNodes;
                FeMesh.WriteToBinaryFile(results.Mesh, bw);
                results.Mesh.Nodes = tmp;
                // Node lookup
                if (results._nodeIdsLookUp == null) bw.Write((int)0);
                else
                {
                    bw.Write((int)1);

                    bw.Write(results._nodeIdsLookUp.Count);
                    foreach (var entry in results._nodeIdsLookUp)
                    {
                        bw.Write(entry.Key);
                        bw.Write(entry.Value);
                    }
                }
                // Fields
                if (results._fields == null) bw.Write((int)0);
                else
                {
                    bw.Write((int)1);

                    bw.Write((int)results._fields.Count);
                    foreach (var entry in results._fields)
                    {
                        FieldData.WriteToFile(entry.Key, bw);
                        Field.WriteToFile(entry.Value, bw);
                    }
                }
            }
        }
        public static void ReadFromFile(FeResults results, System.IO.BinaryReader br)
        {
            int numItems;
            FieldData fieldData;
            Field field;

            int exist = br.ReadInt32();
            if (exist == 1)
            {
                // Mesh
                FeMesh.ReadFromBinaryFile(results.Mesh, br);
                results.InitializeUndeformedNodes();
                // Node lookup
                exist = br.ReadInt32();
                if (exist == 1)
                {
                    numItems = br.ReadInt32();
                    results._nodeIdsLookUp = new Dictionary<int, int>();
                    for (int i = 0; i < numItems; i++) results._nodeIdsLookUp.Add(br.ReadInt32(), br.ReadInt32());
                }

                // Fields
                exist = br.ReadInt32();
                if (exist == 1)
                {
                    numItems = br.ReadInt32();
                    results._fields = new Dictionary<FieldData, Field>();
                    for (int i = 0; i < numItems; i++)
                    {
                        fieldData = FieldData.ReadFromFile(br);
                        field = Field.ReadFromFile(br);
                        results._fields.Add(fieldData, field);
                    }
                }
            }
        }


        // Methods                                                                                                                  
        public void SetMesh(FeMesh mesh, Dictionary<int, int> nodeIdsLookUp)
        {
            _mesh = mesh;
            InitializeUndeformedNodes();
            //
            _nodeIdsLookUp = nodeIdsLookUp;
        }
        public string[] AddPartsFromMesh(FeMesh mesh, string[] partNames)
        {
            _mesh.Nodes = _undeformedNodes;
            string[] addedPartNames = _mesh.AddPartsFromMesh(mesh, partNames, null, false);
            InitializeUndeformedNodes();
            return addedPartNames;
        }
        private void InitializeUndeformedNodes()
        {
            _undeformedNodes = new Dictionary<int, FeNode>();
            foreach (var entry in _mesh.Nodes) _undeformedNodes.Add(entry.Key, new FeNode(entry.Value));
            _scale = 0;
            _stepId = 0;
            _stepIncrementId = 0;
        }
        public void SetMeshDeformation(float scale, int stepId, int stepIncrementId)
        {
            if (scale != _scale || stepId != _stepId || stepIncrementId != _stepIncrementId)
            {
                _scale = scale;
                _stepId = stepId;
                _stepIncrementId = stepIncrementId;
                Dictionary<int, FeNode> deformedNodes = null;
                //
                if (_scale != 0)
                {
                    string name = "DISP";
                    string[] componentNames = GetComponentNames(name);
                    // Components can be deleted
                    if (componentNames.Contains("U1") && componentNames.Contains("U2") && componentNames.Contains("U3"))
                    {
                        Field U = GetField(new FieldData(name, "", _stepId, _stepIncrementId));
                        if (U != null)
                        {
                            //
                            float[][] disp = new float[3][];
                            disp[0] = U.GetComponentValues("U1");
                            disp[1] = U.GetComponentValues("U2");
                            disp[2] = U.GetComponentValues("U3");
                            //
                            if (disp[0] != null && disp[1] != null && disp[2] != null)
                            {
                                int resultNodeId;
                                FeNode deformedNode;
                                deformedNodes = new Dictionary<int, FeNode>();
                                //
                                foreach (var entry in _undeformedNodes)
                                {
                                    // Result parts
                                    if (_nodeIdsLookUp.TryGetValue(entry.Key, out resultNodeId))
                                    {
                                        deformedNode = new FeNode(entry.Key,
                                                                  entry.Value.X + _scale * disp[0][resultNodeId],
                                                                  entry.Value.Y + _scale * disp[1][resultNodeId],
                                                                  entry.Value.Z + _scale * disp[2][resultNodeId]);
                                    }
                                    // Geometry parts
                                    else deformedNode = entry.Value;
                                    //
                                    deformedNodes.Add(deformedNode.Id, deformedNode);
                                }
                            }
                        }
                    }
                }
                if (deformedNodes == null)
                {
                    deformedNodes = new Dictionary<int, FeNode>();
                    foreach (var entry in _undeformedNodes) deformedNodes.Add(entry.Key, entry.Value);
                }
                //
                _mesh.Nodes = deformedNodes;
            }
        }
        public void SetPartDeformation(BasePart part, float scale, int stepId, int stepIncrementId)
        {
            bool scaled = false;
            FeNode unDeformedNode;
            //
            if (scale != 0)
            {
                string name = "DISP";
                string[] componentNames = GetComponentNames(name);
                // Components can be deleted
                if (componentNames.Contains("U1") && componentNames.Contains("U2") && componentNames.Contains("U3"))
                {
                    Field U = GetField(new FieldData(name, "", stepId, stepIncrementId));
                    if (U != null)
                    {
                        //
                        float[][] disp = new float[3][];
                        disp[0] = U.GetComponentValues("U1");
                        disp[1] = U.GetComponentValues("U2");
                        disp[2] = U.GetComponentValues("U3");
                        //
                        if (disp[0] != null && disp[1] != null && disp[2] != null)
                        {
                            scaled = true;
                            int resultNodeId;
                            FeNode deformedNode;
                            //
                            foreach (var nodeId in part.NodeLabels)
                            {
                                resultNodeId = _nodeIdsLookUp[nodeId];
                                unDeformedNode = _undeformedNodes[nodeId];
                                //
                                deformedNode = new FeNode(unDeformedNode.Id,
                                                          unDeformedNode.X + scale * disp[0][resultNodeId],
                                                          unDeformedNode.Y + scale * disp[1][resultNodeId],
                                                          unDeformedNode.Z + scale * disp[2][resultNodeId]);
                                _mesh.Nodes[deformedNode.Id] = deformedNode;
                            }
                        }
                    }
                }
            }
            if (!scaled)
            {
                foreach (var nodeId in part.NodeLabels)
                {
                    unDeformedNode = _undeformedNodes[nodeId];
                    _mesh.Nodes[unDeformedNode.Id] = unDeformedNode;
                }
            }
        }
        //
        public TypeConverter GetFieldUnitConverter(string fieldDataName, string componentName)
        {
            GetFieldUnitConverterAndAbbrevation(fieldDataName, componentName, out TypeConverter unitConverter,
                                                  out string unitAbbreviation);
            return unitConverter;
        }
        public string GetFieldUnitAbbrevation(string fieldDataName, string componentName)
        {
            GetFieldUnitConverterAndAbbrevation(fieldDataName, componentName, out TypeConverter unitConverter,
                                                out string unitAbbreviation);
            return unitAbbreviation;
        }
        public void GetFieldUnitConverterAndAbbrevation(string fieldDataName, string componentName, out TypeConverter unitConverter,
                                                        out string unitAbbreviation)
        {
            unitConverter = new DoubleConverter();
            unitAbbreviation = "?";
            try
            {
                switch (fieldDataName.ToUpper())
                {
                    case "NONE":
                        unitConverter = new DoubleConverter();
                        unitAbbreviation = "";
                        break;
                    case "DISP":
                        unitConverter = new StringLengthConverter();
                        unitAbbreviation = _unitSystem.LengthUnitAbbreviation;
                        break;
                    case "STRESS":
                    case "ZZSTR":
                        unitConverter = new StringPressureConverter();
                        unitAbbreviation = _unitSystem.PressureUnitAbbreviation;
                        break;
                    case "TOSTRAIN": 
                    case "MESTRAIN":
                    case "PE":
                        unitConverter = new DoubleConverter();
                        unitAbbreviation = "/";
                        break;
                    case "FORC":
                        unitConverter = new StringForceConverter();
                        unitAbbreviation = _unitSystem.ForceUnitAbbreviation;
                        break;
                    case "ENER":
                        unitConverter = new StringEnergyPerVolumeConverter();
                        unitAbbreviation = _unitSystem.EnergyPerVolumeUnitAbbreviation;
                        break;
                    case "ERROR":
                        unitConverter = new DoubleConverter();
                        unitAbbreviation = "%";
                        break;
                    case "CONTACT":
                        {
                            switch (componentName.ToUpper())
                            {
                                case "COPEN":
                                case "CSLIP1":
                                case "CSLIP2":
                                    unitConverter = new StringLengthConverter();
                                    unitAbbreviation = _unitSystem.LengthUnitAbbreviation;
                                    break;
                                case "CPRESS":
                                case "CSHEAR1":
                                case "CSHEAR2":
                                    unitConverter = new StringPressureConverter();
                                    unitAbbreviation = _unitSystem.PressureUnitAbbreviation;
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    // Thermal
                    case "NDTEMP":
                        unitConverter = new StringTemperatureConverter();
                        unitAbbreviation = _unitSystem.TemperatureUnitAbbreviation;
                        break;
                    case "FLUX":
                        unitConverter = new StringPowerPerAreaConverter();
                        unitAbbreviation = _unitSystem.PowerPerAreaUnitAbbreviation;
                        break;
                    case "RFL":
                        unitConverter = new StringPowerConverter();
                        unitAbbreviation = _unitSystem.PowerUnitAbbreviation;
                        break;
                    case "HERROR":
                        unitConverter = new DoubleConverter();
                        unitAbbreviation = "%";
                        break;
                    default:
                        //if (System.Diagnostics.Debugger.IsAttached) throw new NotSupportedException();
                        //
                        unitConverter = new DoubleConverter();
                        unitAbbreviation = "?";
                        break;
                }
            }
            catch
            {
            }
        }
        //
        public TypeConverter GetHistoryUnitConverter(string fieldName, string componentName)
        {
            GetHistoryUnitConverterAndAbbrevation(fieldName, componentName, out TypeConverter unitConverter,
                                                  out string unitAbbreviation);
            return unitConverter;
        }
        public string GetHistoryUnitAbbrevation(string fieldName, string componentName)
        {
            GetHistoryUnitConverterAndAbbrevation(fieldName, componentName, out TypeConverter unitConverter,
                                                  out string unitAbbreviation);
            return unitAbbreviation;
        }
        public void GetHistoryUnitConverterAndAbbrevation(string fieldName, string componentName, out TypeConverter unitConverter,
                                                          out string unitAbbreviation)
        {
            unitConverter = new DoubleConverter();
            unitAbbreviation = "?";
            try
            {
                switch (fieldName.ToUpper())
                {
                    case "TIME":
                        unitConverter = new StringTimeConverter();
                        unitAbbreviation = _unitSystem.TimeUnitAbbreviation;
                        break;
                    case "DISPLACEMENTS":
                    case "RELATIVE CONTACT DISPLACEMENT":
                    case "CENTER OF GRAVITY CG":
                    case "MEAN SURFACE NORMAL":
                        if (componentName.ToUpper().StartsWith("UR"))
                        {
                            unitConverter = new StringAngleConverter();
                            unitAbbreviation = _unitSystem.AngleUnitAbbreviation;
                        }
                        else
                        {
                            unitConverter = new StringLengthConverter();
                            unitAbbreviation = _unitSystem.LengthUnitAbbreviation;
                        }
                        break;
                    case "SURFACE AREA":
                        unitConverter = new StringAreaConverter();
                        unitAbbreviation = _unitSystem.AreaUnitAbbreviation;
                        break;
                    case "VOLUME":
                    case "TOTAL VOLUME":
                        unitConverter = new StringVolumeConverter();
                        unitAbbreviation = _unitSystem.VolumeUnitAbbreviation;
                        break;
                    case "FORCES":
                    case "TOTAL FORCE":
                    case "NORMAL SURFACE FORCE":
                    case "SHEAR SURFACE FORCE":
                    case "TOTAL SURFACE FORCE":
                        if (componentName.ToUpper().StartsWith("RM"))
                        {
                            unitConverter = new StringMomentConverter();
                            unitAbbreviation = _unitSystem.MomentUnitAbbreviation;
                        }
                        else
                        {
                            unitConverter = new StringForceConverter();
                            unitAbbreviation = _unitSystem.ForceUnitAbbreviation;
                        }
                        break;
                    case "STRESSES":
                    case "CONTACT STRESS":
                        unitConverter = new StringPressureConverter();
                        unitAbbreviation = _unitSystem.PressureUnitAbbreviation;
                        break;
                    case "STRAINS":
                    case "MECHANICAL STRAINS":
                    case "EQUIVALENT PLASTIC STRAIN":
                        unitConverter = new DoubleConverter();
                        unitAbbreviation = "/";
                        break;
                    case "INTERNAL ENERGY":
                    case "TOTAL INTERNAL ENERGY":
                    case "CONTACT PRINT ENERGY":
                        unitConverter = new StringEnergyConverter();
                        unitAbbreviation = _unitSystem.EnergyUnitAbbreviation;
                        break;
                    case "INTERNAL ENERGY DENSITY":
                        unitConverter = new StringEnergyPerVolumeConverter();
                        unitAbbreviation = _unitSystem.EnergyPerVolumeUnitAbbreviation;
                        break;
                    case "TOTAL NUMBER OF CONTACT ELEMENTS":
                        unitConverter = new DoubleConverter();
                        unitAbbreviation = "/";
                        break;
                    case "MOMENT ABOUT ORIGIN":
                    case "MOMENT ABOUT CG":
                        unitConverter = new StringMomentConverter();
                        unitAbbreviation = _unitSystem.MomentUnitAbbreviation;
                        break;
                    // Thermal
                    case "TEMPERATURES":
                        unitConverter = new StringTemperatureConverter();
                        unitAbbreviation = _unitSystem.TemperatureUnitAbbreviation;
                        break;
                    case "HEAT GENERATION":
                    case "TOTAL HEAT GENERATION":
                    case "BODY HEATING":
                    case "TOTAL BODY HEATING":
                        unitConverter = new StringPowerConverter();
                        unitAbbreviation = _unitSystem.PowerUnitAbbreviation;
                        break;
                    case "HEAT FLUX":
                        unitConverter = new StringPowerPerAreaConverter();
                        unitAbbreviation = _unitSystem.PowerPerAreaUnitAbbreviation;
                        break;
                    // Error
                    case "ERROR":
                    default:
                        if (System.Diagnostics.Debugger.IsAttached) throw new NotSupportedException();
                        //
                        unitConverter = new DoubleConverter();
                        unitAbbreviation = "?";
                        break;
                }
            }
            catch
            {
            }
        }
        //
        public void CopyPartsFromMesh(FeMesh mesh)
        {
            _mesh.Parts.Clear();

            ResultPart part;
            foreach (var entry in mesh.Parts)
            {
                part = new ResultPart(entry.Value);
                _mesh.Parts.Add(entry.Key, part);

                foreach (var elementId in part.Labels) _mesh.Elements[elementId].PartId = part.PartId;
            }
        }
        public void SetPartPropertiesFromMesh(FeMesh mesh)
        {
            //// the meshes must be equal
            //List<Tuple<string, BasePart>> newNameOldPartPairs = new List<Tuple<string, BasePart>>();
            //foreach (var entry1 in _parts)
            //{
            //    foreach (var entry2 in mesh.Parts)
            //    {
            //        if (entry1.Value.IsEqual(entry2.Value))
            //        {
            //            if (entry1.Value.Name != entry2.Value.Name)
            //            {
            //                newNameOldPartPairs.Add(new Tuple<string, BasePart>(entry2.Key, entry1.Value));
            //                break;
            //            }
            //        }
            //    }
            //}

            //foreach (var newNameOldPartPair in newNameOldPartPairs)
            //{
            //    if (!_parts.ContainsKey(newNameOldPartPair.Item1))  // error check in more than one new part is equal to the same old part
            //    {
            //        if (_parts.Remove(newNameOldPartPair.Item2.Name))
            //        {
            //            newNameOldPartPair.Item2.Name = newNameOldPartPair.Item1;
            //            _parts.Add(newNameOldPartPair.Item1, newNameOldPartPair.Item2);
            //        }
            //    }
            //}

            //// set color
            //BasePart part;
            //foreach (var entry in _parts)
            //{
            //    if (mesh.Parts.TryGetValue(entry.Key, out part))
            //    {
            //        entry.Value.Color = part.Color;
            //    }
            //}
        }
        //
        public void AddFiled(FieldData fieldData, Field field)
        {
            _fieldLookUp.Add(_fields.Count, fieldData);
            _fields.Add(fieldData, field);
        }
        public Field GetField(FieldData fieldData)
        {
            foreach (var entry in _fields)
            {
                if (entry.Key.Name.ToUpper() == fieldData.Name.ToUpper() &&
                    entry.Key.StepId == fieldData.StepId &&
                    entry.Key.StepIncrementId == fieldData.StepIncrementId)
                {
                    return entry.Value;
                }
            }
            return null;
        }
        //
        public string[] GetComponentNames()
        {
            HashSet<string> componentNames = new HashSet<string>();
            foreach (var entry in _fields)
            {
                componentNames.UnionWith(entry.Value.GetCmponentNames());
            }
            return componentNames.ToArray();
        }
        public string[] GetComponentNames(string fieldName)
        {
            foreach (var entry in _fields)
            {
                if (entry.Key.Name.ToUpper() == fieldName.ToUpper())
                {
                    return entry.Value.GetCmponentNames();
                }
            }
            return null;
        }
        public FieldData[] GetAllFieldData()
        {
            return _fields.Keys.ToArray();
        }
        public FieldData GetFieldData(string name, string component, int stepId, int stepIncrementId)
        {
            FieldData result;
            // Empty results
            if (stepId == -1 && stepIncrementId == -1)
            {
                result = new FieldData(name, component, stepId, stepIncrementId);
                result.Type = StepType.Static;
                result.Valid = false;
                return result;
            }
            // Zero step
            else if (stepId == 1 && stepIncrementId == 0)
            {
                result = new FieldData(name, component, stepId, stepIncrementId);
                result.Type = StepType.Static;
                return result;
            }
            // Find the result
            foreach (var entry in _fields)
            {
                if (entry.Key.Name == name && entry.Key.StepId == stepId && entry.Key.StepIncrementId == stepIncrementId)
                {
                    result = new FieldData(entry.Key);
                    result.Component = component;
                    return result;
                }
            }
            // Nothing found
            result = new FieldData(name, component, stepId, stepIncrementId);
            result.Type = StepType.Static;
            result.Valid = false;
            return result;
            //return null;
        }
        public FieldData GetFirstComponentOfTheFirstFieldAtLastIncrement()
        {
            string name = GetAllFieldNames()[0];
            string component = GetComponentNames(name)[0];
            int stepId = GetAllStepIds().Last();
            int stepIncrementId = GetIncrementIds(stepId).Last();
            //
            return GetFieldData(name, component, stepId, stepIncrementId);
        }
        public FieldData GetFirstComponentOfTheFirstFieldAtDefaultIncrement()
        {
            string[] names = GetAllFieldNames();
            FieldData fieldData;
            if (names.Length == 0)
            {
                // There is no data
                fieldData = GetFieldData("None", "None", -1, -1);
                fieldData.Valid = false;
            }
            else
            {
                int stepId = GetAllStepIds().Last();
                int stepIncrementId = GetIncrementIds(stepId).Last();
                string name = GetStepFieldNames(stepId)[0];
                string component = GetComponentNames(name)[0];
                //
                fieldData = GetFieldData(name, component, stepId, stepIncrementId);
                if (fieldData == null)
                {
                    // There is no data
                    fieldData = GetFieldData("None", "None", -1, -1);
                }
                else if (fieldData.Type == StepType.Frequency)
                {
                    stepIncrementId = GetIncrementIds(stepId).First();
                    fieldData = GetFieldData(name, component, stepId, stepIncrementId);
                }
            }
            return fieldData;
        }
        public string[] GetAllFieldNames()
        {
            List<string> names = new List<string>();
            foreach (var entry in _fields)
            {
                if (!names.Contains(entry.Key.Name)) names.Add(entry.Key.Name);
            }
            return names.ToArray();
        }
        public NamedClass[] GetFieldsAsNamedItems()
        {
            string[] names = GetAllFieldNames();
            NamedClass[] fields = new NamedClass[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                fields[i] = new EmptyNamedClass(names[i]);
            }
            return fields;
        }
        public string[] GetStepFieldNames(int stepId)
        {
            List<string> names = new List<string>();
            foreach (var entry in _fields)
            {
                if (entry.Key.StepId == stepId && !names.Contains(entry.Key.Name))
                    names.Add(entry.Key.Name);
            }
            return names.ToArray();
        }
        public int[] GetAllStepIds()
        {
            HashSet<int> ids = new HashSet<int>();
            foreach (var entry in _fields)
            {
                ids.Add(entry.Key.StepId);
            }
            return ids.ToArray();
        }
        public int[] GetIncrementIds(int stepId)
        {
            HashSet<int> incrementIds = new HashSet<int>();
            foreach (var entry in _fields)
            {
                if (entry.Key.StepId == stepId)
                {
                    if (entry.Key.Type == StepType.Static && stepId == 1 && incrementIds.Count == 0)
                        incrementIds.Add(0);   // zero increment for the first step - static only
                    //
                    incrementIds.Add(entry.Key.StepIncrementId);
                }
            }
            return incrementIds.ToArray();
        }
        public Dictionary<int, int[]> GetExistingIncrementIds(string fieldName, string component)
        {
            int[] stepIds = GetAllStepIds();
            Dictionary<int, int[]> existingIncrementIds = new Dictionary<int, int[]>();
            if (stepIds.Length == 0)
            {
                existingIncrementIds.Add(-1, new int[] { -1 });
            }
            else
            {
                List<int> stepIncrementIds;
                string fieldHash;
                HashSet<string> fieldHashes = GetAllFieldHashes();
                //
                foreach (int stepId in stepIds)
                {
                    stepIncrementIds = new List<int>();
                    foreach (int incrementId in GetIncrementIds(stepId))
                    {
                        fieldHash = GetFieldHash(fieldName, component, stepId, incrementId);
                        //if (FieldExists(fieldName, component, stepId, incrementId)) stepIncrementIds.Add(incrementId);
                        if (fieldHashes.Contains(fieldHash)) stepIncrementIds.Add(incrementId);
                    }
                    existingIncrementIds.Add(stepId, stepIncrementIds.ToArray());
                }
            }
            return existingIncrementIds;
        }
        public Dictionary<int, int[]> GetAllExistingIncrementIds()
        {
            int[] stepIds = GetAllStepIds();
            Dictionary<int, int[]> existingIncrementIds = new Dictionary<int, int[]>();
            if (stepIds.Length == 0)
            {
                existingIncrementIds.Add(-1, new int[] { -1 });
            }
            else
            {
                foreach (int stepId in stepIds)
                {
                    existingIncrementIds.Add(stepId, GetIncrementIds(stepId));
                }
            }
            return existingIncrementIds;
        }
        public bool FieldExists(string fieldName, string component, int stepId, int stepIncrementId)
        {
            foreach (var entry in _fields)
            {
                if ((entry.Key.Name.ToUpper() == fieldName.ToUpper() && entry.Value.ContainsComponent(component) && stepId == 1 && stepIncrementId == 0) ||  // zero increment
                    (entry.Key.Name.ToUpper() == fieldName.ToUpper() && entry.Value.ContainsComponent(component) && entry.Key.StepId == stepId && entry.Key.StepIncrementId == stepIncrementId))
                    return true;
            }
            return false;
        }
        private HashSet<string> GetAllFieldHashes()
        {
            HashSet<string> fieldHashes = new HashSet<string>();
            foreach (var fieldEntry in _fields)
            {
                foreach (var componentName in fieldEntry.Value.GetCmponentNames())
                {
                    fieldHashes.Add(GetFieldHash(fieldEntry.Key.Name, componentName, 1, 0));
                    fieldHashes.Add(GetFieldHash(fieldEntry.Key.Name, componentName, fieldEntry.Key.StepId, fieldEntry.Key.StepIncrementId));
                }
            }
            return fieldHashes;
        }
        private string GetFieldHash(string fieldName, string component, int stepId, int stepIncrementId)
        {
            return fieldName.ToUpper() + "_" + component.ToUpper() + "_" + stepId.ToString() + "_" + stepIncrementId.ToString();
        }
        //
        public float GetIncrementTime(int stepId, int stepIncrementId)
        {
            if (stepIncrementId == 0) return 0; // zero increment
            foreach (var entry in _fields)
            {
                if (entry.Key.StepId == stepId && entry.Key.StepIncrementId == stepIncrementId)
                {
                    return entry.Key.Time;
                }
            }
            return -1;
        }
        public float[] GetValues(FieldData fieldData, int[] globalNodeIds)
        {
            float[] values = null;
            //
            if (fieldData.Valid)
            {
                if (fieldData.StepIncrementId == 0)   // zero increment
                {
                    if (fieldData.StepId == 1)        // first step
                    {
                        values = new float[globalNodeIds.Length];
                    }
                }
                else
                {
                    Field field = GetField(fieldData);
                    //
                    float[] allValues = field.GetComponentValues(fieldData.Component);
                    values = new float[globalNodeIds.Length];
                    int globalId;
                    int localId;
                    for (int i = 0; i < globalNodeIds.Length; i++)
                    {
                        globalId = globalNodeIds[i];
                        if (_nodeIdsLookUp.TryGetValue(globalId, out localId) && localId >= 0 && localId < allValues.Length)
                            values[i] = allValues[localId];
                        else
                            return null;
                    }

                    //// Remove frst step
                    //fieldData.StepIncrementId = 1;
                    ////
                    //foreach (var entry in _fields)
                    //{
                    //    if (entry.Key.Name.ToUpper() == fieldData.Name.ToUpper() &&
                    //        entry.Key.StepId == fieldData.StepId &&
                    //        entry.Key.StepIncrementId == fieldData.StepIncrementId)
                    //    {
                    //        float[] allValues = entry.Value.GetComponentValues(fieldData.Component);
                    //        int globalId;
                    //        int localId;
                    //        for (int i = 0; i < globalNodeIds.Length; i++)
                    //        {
                    //            globalId = globalNodeIds[i];
                    //            if (_nodeIdsLookUp.TryGetValue(globalId, out localId) && localId >= 0 && localId < allValues.Length)
                    //                values[i] -= allValues[localId];
                    //            else
                    //                return null;
                    //        }
                    //        break;
                    //    }
                    //}
                }
            }
            return values;
        }
        public bool IsComponentInvariant(FieldData fieldData)
        {
            if (fieldData.StepIncrementId == 0)   // zero increment
            {
                if (fieldData.StepId == 1)        // first step
                    return false;
            }
            else
            {
                foreach (var entry in _fields)
                {
                    if (entry.Key.Name.ToUpper() == fieldData.Name.ToUpper() && 
                        entry.Key.StepId == fieldData.StepId && 
                        entry.Key.StepIncrementId == fieldData.StepIncrementId)
                    {
                        return entry.Value.IsComponentInvariant(fieldData.Component);
                    }
                }
            }
            return false;
        }
        public NodesExchangeData GetExtremeValues(string partName, FieldData fieldData)
        {
            return GetScaledExtremeValues(partName, fieldData, 0);
        }
        public NodesExchangeData GetScaledExtremeValues(string partName, FieldData fieldData, float relativeScale)
        {
            if (!fieldData.Valid) return null;
            //
            NodesExchangeData nodesData = new NodesExchangeData();
            nodesData.Ids = new int[2];
            nodesData.Coor = new double[2][];
            nodesData.Values = new float[2];
            int minId = -1;
            int maxId = -1;            
            //
            if (fieldData.StepIncrementId == 0)     // zero increment
            {
                if (fieldData.StepId == 1)          // first step / zero increment
                {
                    minId = _nodeIdsLookUp.Keys.First();
                    maxId = minId;
                    nodesData.Ids[0] = minId;
                    nodesData.Ids[1] = maxId;
                    nodesData.Coor[0] = _mesh.Nodes[minId].Coor;
                    nodesData.Coor[1] = _mesh.Nodes[maxId].Coor;
                    nodesData.Values[0] = 0;
                    nodesData.Values[1] = 0;
                }
            }
            else
            {                
                foreach (var fieldEntry in _fields)
                {
                    if (fieldEntry.Key.Name == fieldData.Name && fieldEntry.Key.StepId == fieldData.StepId &&
                        fieldEntry.Key.StepIncrementId == fieldData.StepIncrementId)
                    {
                        int id;
                        float value;
                        BasePart basePart;
                        float[] values = fieldEntry.Value.GetComponentValues(fieldData.Component);
                        //
                        basePart = _mesh.Parts[partName];
                        //
                        if (_nodeIdsLookUp.TryGetValue(basePart.NodeLabels[0], out id) && id < values.Length)
                        {
                                value = values[id];
                                //
                                nodesData.Values[0] = value;
                                minId = basePart.NodeLabels[0];
                                nodesData.Values[1] = value;
                                maxId = basePart.NodeLabels[0];
                        }
                        else
                        {
                            nodesData.Values[0] = float.MaxValue;
                            nodesData.Values[1] = -float.MaxValue;
                        }
                        //
                        foreach (var nodeId in basePart.NodeLabels)
                        {
                            if (_nodeIdsLookUp.TryGetValue(nodeId, out id) && id < values.Length)
                            {
                                value = values[id];
                                if (value < nodesData.Values[0])
                                {
                                    nodesData.Values[0] = value;
                                    minId = nodeId;
                                }
                                else if (value > nodesData.Values[1])
                                {
                                    nodesData.Values[1] = value;
                                    maxId = nodeId;
                                }
                            }
                        }
                        //
                        if (relativeScale < 0)  // swap min and max
                        {
                            int tmp = minId; minId = maxId; maxId = tmp;
                            float tmpD = nodesData.Values[0]; nodesData.Values[0] = nodesData.Values[1]; nodesData.Values[1] = tmpD;
                        }
                        //
                        nodesData.Ids[0] = minId;
                        nodesData.Ids[1] = maxId;
                        //
                        nodesData.Coor[0] = _mesh.Nodes[minId].Coor;
                        nodesData.Coor[1] = _mesh.Nodes[maxId].Coor;
                        //
                        nodesData.Values[0] *= relativeScale;
                        nodesData.Values[1] *= relativeScale;
                        //
                        break;
                    }
                }
            }
            return nodesData;
        }
        // History
        public NamedClass[] GetHistoriyOutputsAsNamedItems()
        {
            string[] names = _history.Sets.Keys.ToArray();
            NamedClass[] historyOutputs = new NamedClass[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                historyOutputs[i] = new EmptyNamedClass(names[i]);
            }
            return historyOutputs;
        }
        // Remove
        public void RemoveResultFieldOutputs(string[] fieldOutputNames)
        {
            if (_fields != null)
            {
                List<FieldData> fieldsToRemove = new List<FieldData>();
                foreach (var entry in _fields)
                {
                    if (fieldOutputNames.Contains(entry.Key.Name)) fieldsToRemove.Add(entry.Key);
                }
                //
                foreach (var fieldToRemove in fieldsToRemove)
                {
                    _fields.Remove(fieldToRemove);
                }
            }
        }
        public void RemoveResultFieldOutputComponents(string fieldOutputName, string[] componentNames)
        {
            if (_fields != null)
            {
                foreach (var entry in _fields)
                {
                    if (entry.Key.Name == fieldOutputName)
                    {
                        foreach (var componentName in componentNames)
                        {
                            entry.Value.RemoveComponent(componentName);
                        }
                    }
                }
            }
        }
        //
        public void RemoveResultHistoryResultSets(string[] historyResultSetNames)
        {
            if (_history != null)
            {
                List<string> setsToRemove = new List<string>();
                foreach (var entry in _history.Sets)
                {
                    if (historyResultSetNames.Contains(entry.Key)) setsToRemove.Add(entry.Key);
                }
                //
                foreach (var setToRemove in setsToRemove)
                {
                    _history.Sets.Remove(setToRemove);
                }
            }
        }
        public void RemoveResultHistoryResultFields(string historyResultSetName, string[] historyResultFieldNames)
        {
            if (_history != null)
            {
                List<string> fieldsToRemove = new List<string>();
                foreach (var entry in _history.Sets[historyResultSetName].Fields)
                {
                    if (historyResultFieldNames.Contains(entry.Key)) fieldsToRemove.Add(entry.Key);
                }
                //
                foreach (var fieldToRemove in fieldsToRemove)
                {
                    _history.Sets[historyResultSetName].Fields.Remove(fieldToRemove);
                }
            }
        }
        public void RemoveResultHistoryResultCompoments(string historyResultSetName,
                                                        string historyResultFieldName,
                                                        string[] historyResultComponentNames)
        {
            if (_history != null)
            {
                List<string> componentsToRemove = new List<string>();
                foreach (var entry in _history.Sets[historyResultSetName].Fields[historyResultFieldName].Components)
                {
                    if (historyResultComponentNames.Contains(entry.Key)) componentsToRemove.Add(entry.Key);
                }
                //
                foreach (var componentToRemove in componentsToRemove)
                {
                    _history.Sets[historyResultSetName].Fields[historyResultFieldName].Components.Remove(componentToRemove);
                }
            }
        }


        public void GetClosestFieldComponent(string fieldName, string componentName,
                                             out string closestFieldName, out string closestComponentName)
        {
            closestFieldName = null;
            closestComponentName = null;
            string[] existingComponentNames = GetComponentNames(fieldName);
            //
            for (int i = 0; i < existingComponentNames.Length; i++)
            {
                if (existingComponentNames[i] == componentName)
                {
                    if (i + 1 < existingComponentNames.Length) closestComponentName = existingComponentNames[i + 1];
                    else if (i > 1) closestComponentName = existingComponentNames[i - 1];
                }
            }
            // Component found
            if (closestComponentName != null)
            {
                closestFieldName = fieldName;
            }
            // No components found in the same field
            else
            {
                string[] existingFieldNames = GetAllFieldNames();
                for (int i = 0; i < existingFieldNames.Length; i++)
                {
                    if (existingFieldNames[i] == fieldName)
                    {
                        // Find the next field output with at least one component
                        while (++i < existingFieldNames.Length)
                        {
                            existingComponentNames = GetComponentNames(existingFieldNames[i]);
                            if (existingComponentNames.Length > 0)
                            {
                                closestFieldName = existingFieldNames[i];
                                closestComponentName = existingComponentNames[0];
                                return;
                            }
                        }
                        // Find the prevous field output with at least one component
                        while (--i > 0)
                        {
                            existingComponentNames = GetComponentNames(existingFieldNames[i]);
                            if (existingComponentNames.Length > 0)
                            {
                                closestFieldName = existingFieldNames[i];
                                closestComponentName = existingComponentNames[0];
                                return;
                            }
                        }
                    }
                }
                // No field with at least one component found
                closestFieldName = null;
                closestComponentName = null;
            }
        }
        // Scaled results values
        public void GetNodesAndValues(FieldData fieldData, int[] nodeIds, out double[][] nodeCoor, out float[] values)
        {
            nodeCoor = new double[nodeIds.Length][];
            for (int i = 0; i < nodeIds.Length; i++)
            {
                nodeCoor[i] = Mesh.Nodes[nodeIds[i]].Coor;
            }
            values = GetValues(fieldData, nodeIds);
        }
        //
        public PartExchangeData GetAllNodesCellsAndValues(FeGroup elementSet, FieldData fData)
        {
            PartExchangeData pData = new PartExchangeData();
            _mesh.GetAllNodesAndCells(elementSet, out pData.Nodes.Ids, out pData.Nodes.Coor, out pData.Cells.Ids,
                                      out pData.Cells.CellNodeIds, out pData.Cells.Types);
            if (!fData.Valid) pData.Nodes.Values = null;
            else pData.Nodes.Values = GetValues(fData, pData.Nodes.Ids);
            return pData;
        }
        public PartExchangeData GetVisualizationNodesCellsAndValues(BasePart part, FieldData fData)
        {
            PartExchangeData pData = new PartExchangeData();
            _mesh.GetVisualizationNodesAndCells(part, out pData.Nodes.Ids, out pData.Nodes.Coor, out pData.Cells.Ids,
                                                out pData.Cells.CellNodeIds, out pData.Cells.Types);
            if (!fData.Valid) pData.Nodes.Values = null;
            else
            {
                pData.Nodes.Values = GetValues(fData, pData.Nodes.Ids);
                pData.ExtremeNodes = GetScaledExtremeValues(part.Name, fData, 1);
            }
            return pData;
        }
        public PartExchangeData GetEdgesNodesAndCells(FeGroup elementSet, FieldData fData)  // must use fData
        {
            PartExchangeData resultData = new PartExchangeData();
            _mesh.GetNodesAndCellsForModelEdges(elementSet, out resultData.Nodes.Ids, out resultData.Nodes.Coor,
                                                out resultData.Cells.CellNodeIds, out resultData.Cells.Types);
            return resultData;
        }
        public void GetUndeformedNodesAndCells(BasePart part, out double[][] nodeCoor, out int[][] cells, out int[] cellTypes)
        {
            int[] nodeIds, cellIds;
            Dictionary<int, FeNode> tmp = _mesh.Nodes;
            _mesh.Nodes = _undeformedNodes;
            _mesh.GetVisualizationNodesAndCells(part, out nodeIds, out nodeCoor, out cellIds, out cells, out cellTypes);
            _mesh.Nodes = tmp;
        }
        public void GetUndeformedModelEdges(BasePart part, out double[][] nodeCoor, out int[][] cells, out int[] cellTypes)
        {
            int[] nodeIds;
            Dictionary<int, FeNode> tmp = _mesh.Nodes;
            _mesh.Nodes = _undeformedNodes;
            _mesh.GetNodesAndCellsForModelEdges(part, out nodeIds, out nodeCoor, out cells, out cellTypes);
            _mesh.Nodes = tmp;
        }
        // Animation        
        public void GetScaleFactorAnimationData(BasePart part, FieldData fData, float scale, int numFrames,
                                                out PartExchangeData modelResultData,
                                                out PartExchangeData modelEdgesResultData,
                                                out PartExchangeData locatorResultData)
        {
            modelEdgesResultData = null;
            bool modelEdges = part.PartType.HasEdges() && part.Visualization.EdgeCells != null;
            // Undeforme part
            SetPartDeformation(part, 0, 0, 0);
            // Model
            modelResultData = new PartExchangeData();
            _mesh.GetVisualizationNodesAndCells(part, out modelResultData.Nodes.Ids, out modelResultData.Nodes.Coor,
                                                out modelResultData.Cells.Ids, out modelResultData.Cells.CellNodeIds,
                                                out modelResultData.Cells.Types);
            // Edges
            if (modelEdges)
            {
                modelEdgesResultData = new PartExchangeData();
                _mesh.GetNodesAndCellsForModelEdges(part, out modelEdgesResultData.Nodes.Ids,
                                                    out modelEdgesResultData.Nodes.Coor,
                                                    out modelEdgesResultData.Cells.CellNodeIds,
                                                    out modelEdgesResultData.Cells.Types);
            }
            // Locator
            locatorResultData = new PartExchangeData();
            _mesh.GetAllNodesAndCells(part, out locatorResultData.Nodes.Ids, out locatorResultData.Nodes.Coor, 
                                      out locatorResultData.Cells.Ids, out locatorResultData.Cells.CellNodeIds,
                                      out locatorResultData.Cells.Types);
            // Values
            if (!fData.Valid)
            {
                modelResultData.Nodes.Values = null;
                locatorResultData.Nodes.Values = null;
            }
            else
            {
                modelResultData.Nodes.Values = GetValues(fData, modelResultData.Nodes.Ids);
                modelResultData.ExtremeNodes = GetExtremeValues(part.Name, fData);
                locatorResultData.Nodes.Values = GetValues(fData, locatorResultData.Nodes.Ids);
            }
            // Aniumation
            modelResultData.NodesAnimation = new NodesExchangeData[numFrames];
            modelResultData.ExtremeNodesAnimation = new NodesExchangeData[numFrames];
            if (modelEdges) modelEdgesResultData.NodesAnimation = new NodesExchangeData[numFrames];
            locatorResultData.NodesAnimation = new NodesExchangeData[numFrames];
            //
            float[] ratios;
            if (fData.Type == StepType.Frequency || fData.Type == StepType.Buckling) ratios = GetRelativeModalScales(numFrames);
            else ratios = GetRelativeScales(numFrames);
            //
            float absoluteScale;
            float relativeScale;
            bool invariant = IsComponentInvariant(fData);
            //
            for (int i = 0; i < numFrames; i++)
            {
                relativeScale = ratios[i];
                absoluteScale = relativeScale * scale;
                // Deform mesh
                SetPartDeformation(part, absoluteScale, fData.StepId, fData.StepIncrementId);
                // Get scaled model
                modelResultData.NodesAnimation[i] = new NodesExchangeData();
                modelResultData.NodesAnimation[i].Coor = _mesh.GetNodeSetCoor(modelResultData.Nodes.Ids);
                // Get scaled edges
                if (modelEdges)
                {
                    modelEdgesResultData.NodesAnimation[i] = new NodesExchangeData();
                    modelEdgesResultData.NodesAnimation[i].Coor = _mesh.GetNodeSetCoor(modelEdgesResultData.Nodes.Ids);
                }
                // Get scaled locator
                locatorResultData.NodesAnimation[i] = new NodesExchangeData();
                locatorResultData.NodesAnimation[i].Coor = _mesh.GetNodeSetCoor(locatorResultData.Nodes.Ids);
                // Scale values
                if (invariant) relativeScale = Math.Abs(relativeScale);
                // Model
                ScaleValues(relativeScale, modelResultData.Nodes.Values, out modelResultData.NodesAnimation[i].Values);
                modelResultData.ExtremeNodesAnimation[i] = GetScaledExtremeValues(part.Name, fData, relativeScale);
                // Locator
                ScaleValues(relativeScale, locatorResultData.Nodes.Values, out locatorResultData.NodesAnimation[i].Values);
            }
        }
        public void GetTimeIncrementAnimationData(BasePart part, FieldData fData, float scale,
                                                  out PartExchangeData modelResultData,
                                                  out PartExchangeData modelEdgesResultData,
                                                  out PartExchangeData locatorResultData)
        {
            modelEdgesResultData = null;
            bool modelEdges = part.PartType.HasEdges() && part.Visualization.EdgeCells != null;
            // Undeforme part
            SetPartDeformation(part, 0, 0, 0);
            // Model
            modelResultData = GetVisualizationNodesCellsAndValues(part, fData);
            // Edges
            if (modelEdges) modelEdgesResultData = GetEdgesNodesAndCells(part, fData);
            // Locator
            locatorResultData = GetAllNodesCellsAndValues(part, fData);
            // Get all existing increments
            Dictionary<int, int[]> existingStepIncrementIds = GetExistingIncrementIds(fData.Name, fData.Component);
            // Count all existing increments
            int numFrames = 0;
            foreach (var entry in existingStepIncrementIds) numFrames += entry.Value.Length;
            // Create animation frames
            modelResultData.NodesAnimation = new NodesExchangeData[numFrames];
            modelResultData.ExtremeNodesAnimation = new NodesExchangeData[numFrames];
            if (modelEdges) modelEdgesResultData.NodesAnimation = new NodesExchangeData[numFrames];
            locatorResultData.NodesAnimation = new NodesExchangeData[numFrames];
            //
            PartExchangeData data;
            FieldData tmpFieldData = new FieldData(fData);  // create a copy
            int count = 0;
            foreach (var entry in existingStepIncrementIds)
            {
                tmpFieldData.StepId = entry.Key;
                foreach (var incrementId in entry.Value)
                {
                    tmpFieldData.StepIncrementId = incrementId;
                    // Deform part
                    SetPartDeformation(part, scale, tmpFieldData.StepId, tmpFieldData.StepIncrementId);
                    // Model
                    data = GetVisualizationNodesCellsAndValues(part, tmpFieldData);
                    modelResultData.NodesAnimation[count] = data.Nodes;
                    modelResultData.ExtremeNodesAnimation[count] = data.ExtremeNodes;
                    // Edges
                    if (modelEdges)
                    {
                        data = GetEdgesNodesAndCells(part, tmpFieldData);
                        modelEdgesResultData.NodesAnimation[count] = data.Nodes;
                    }
                    // Locator
                    data = GetAllNodesCellsAndValues(part, tmpFieldData);
                    locatorResultData.NodesAnimation[count] = data.Nodes;
                    //
                    count++;
                }
            }
        }
        //
        private float[] GetRelativeScales(int numFrames)
        {
            float ratio = 1f / (numFrames - 1);
            float[] ratios = new float[numFrames];
            for (int i = 0; i < numFrames; i++) ratios[i] = i * ratio;
            return ratios;
        }
        private float[] GetRelativeModalScales(int numFrames)
        {
            float ratio = 2f / (numFrames - 1);
            float[] ratios = new float[numFrames];
            for (int i = 0; i < numFrames; i++)
            {
                ratios[i] = (float)Math.Sin((-1 + i * ratio) * Math.PI / 2);     // end with 1
            }
            return ratios;
        }
        //
        public void ScaleNodeCoordinates(float scale, int stepId, int stepIncrementId, int[] globalNodeIds, double[][] nodes,
                                         out double[][] scaledNodes)
        {
            if (scale == 0)
            {
                scaledNodes = nodes;
            }
            else
            {
                string name = "DISP";
                string[] componentNames = GetComponentNames(name);
                scaledNodes = new double[nodes.Length][];
                for (int i = 0; i < nodes.Length; i++) scaledNodes[i] = nodes[i].ToArray();  // copy coordinates
                // Components can be deleted
                if (componentNames.Contains("U1") && componentNames.Contains("U2") && componentNames.Contains("U3"))
                {
                    float[][] disp = new float[3][];
                    disp[0] = GetValues(new FieldData(name, "U1", stepId, stepIncrementId), globalNodeIds);
                    disp[1] = GetValues(new FieldData(name, "U2", stepId, stepIncrementId), globalNodeIds);
                    disp[2] = GetValues(new FieldData(name, "U3", stepId, stepIncrementId), globalNodeIds);
                    //
                    if (disp[0] != null && disp[1] != null && disp[2] != null)
                    {
                        for (int i = 0; i < nodes.Length; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                scaledNodes[i][j] += scale * disp[j][i];
                            }
                        }
                    }
                }
            }
        }
        public void ScaleNodeCoordinates(float scale, int stepId, int stepIncrementId, int[] globalNodeIds, ref double[][] nodes)
        {
            if (scale != 0)
            {
                string name = "DISP";
                string[] componentNames = GetComponentNames(name);
                // Components can be deleted
                if (componentNames.Contains("U1") && componentNames.Contains("U2") && componentNames.Contains("U3"))
                {
                    float[][] disp = new float[3][];
                    disp[0] = GetValues(new FieldData(name, "U1", stepId, stepIncrementId), globalNodeIds);
                    disp[1] = GetValues(new FieldData(name, "U2", stepId, stepIncrementId), globalNodeIds);
                    disp[2] = GetValues(new FieldData(name, "U3", stepId, stepIncrementId), globalNodeIds);
                    //
                    if (disp[0] != null && disp[1] != null && disp[2] != null)
                    {
                        for (int i = 0; i < nodes.Length; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                nodes[i][j] += scale * disp[j][i];
                            }
                        }
                    }
                }
            }
        }
        //
        public void ScaleValues(float scale, float[] values, out float[] scaledValues)
        {
            if (values == null) scaledValues = null;
            else
            {
                scaledValues = new float[values.Length];
                if (scale != 0)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        scaledValues[i] = values[i] * scale;
                    }
                }
            }
        }
        //
        public float GetMaxDisplacement()
        {
            float max = -float.MaxValue;
            float fieldMax;
            foreach (var entry in _fields)
            {
                if (entry.Key.Name == "DISP")
                {
                    fieldMax = entry.Value.GetComponentMax("ALL");
                    if (fieldMax > max) max = fieldMax;
                }
            }
            return max;
        }
        public float GetMaxDisplacement(int stepId, int stepIncrementId)
        {
            float max = -float.MaxValue;
            foreach (var entry in _fields)
            {
                if (entry.Key.StepId == stepId && entry.Key.StepIncrementId == stepIncrementId && entry.Key.Name == "DISP")
                {
                    max = entry.Value.GetComponentMax("ALL");
                }
            }
            return max;
        }
        //
        public double GetEdgeLength(int geometryEdgeId)
        {
            int[] itemTypePartIds = FeMesh.GetItemTypePartIdsFromGeometryId(geometryEdgeId);
            BasePart part = _mesh.GetPartById(itemTypePartIds[2]);
            int[] nodeIds = part.Visualization.GetOrderedNodeIdsForEdge(itemTypePartIds[0]);
            //
            return 0;
        }
        
        // Wear
        public void ComputeWearFields()
        {
            FieldData fieldData;
            Field field;
            FieldData wearData;
            Field wear;
            float[] values;
            float[] newValues;
            float[] prevValues = null;
            //
            string[] fieldNames = GetAllFieldNames();
            foreach (var fieldName in fieldNames)
            {
                if (fieldName == "CONTACT")
                {
                    string[] componentNames = GetComponentNames(fieldName);
                    //
                    int[] stepIds = GetAllStepIds();
                    //
                    for (int i = 0; i < stepIds.Length; i++)
                    {
                        int[] stepIncrementIds = GetIncrementIds(stepIds[i]);
                        //
                        for (int j = 0; j < stepIncrementIds.Length; j++)
                        {
                            fieldData = GetFieldData(fieldName, "CSLIP1", stepIds[i], stepIncrementIds[j]);
                            field = GetField(fieldData);
                            //
                            if (field == null) values = new float[1];
                            else values = field.GetComponentValues("CSLIP1");
                            //
                            newValues = new float[values.Length];
                            //
                            if (prevValues != null)
                            { 
                                for (int k = 0; k < prevValues.Length; k++)
                                {
                                    if (prevValues[k] != 0 && values[k] != 0) newValues[k] = values[k] - prevValues[k];
                                    else newValues[k] = 0;
                                }
                            }
                            //
                            wear = new Field("WEAR");
                            wear.AddComponent("CSLIP1R", newValues);
                            wearData = new FieldData("WEAR", "CSLIP1R", stepIds[i], stepIncrementIds[j]);
                            wearData.Type = fieldData.Type;
                            //
                            AddFiled(wearData, wear);
                            //
                            prevValues = values;
                        }
                    }
                }
            }

            //AddFiled();

        }

    }
}
