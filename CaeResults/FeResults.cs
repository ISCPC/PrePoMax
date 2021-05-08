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
        private Dictionary<int, int> _nodeIdsLookUp;            // [globalId][resultsId]
        [NonSerialized]
        private Dictionary<FieldData, Field> _fields;

        private string _hashName;
        private string _fileName;
        private FeMesh _mesh;
        private Dictionary<int, FieldData> _fieldLookUp;
        private DateTime _dateTime;
        private UnitSystem _unitSystem;


        // Properties                                                                                                               
        public string HashName { get { return _hashName; } set { _hashName = value; } }
        public string FileName { get { return _fileName; } set { _fileName = value; } }
        public FeMesh Mesh { get { return _mesh; } set { _mesh = value; } }
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
                FeMesh.WriteToBinaryFile(results.Mesh, bw);

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

            _nodeIdsLookUp = nodeIdsLookUp;
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
                    case "NDTEMP":
                        unitConverter = new StringTemperatureConverter();
                        unitAbbreviation = _unitSystem.TemperatureUnitAbbreviation;
                        break;
                    default:
                        throw new NotSupportedException();
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
                    // Error
                    case "ERROR":
                        unitConverter = new DoubleConverter();
                        unitAbbreviation = "?";
                        break;
                    default:
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
        //
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
                    foreach (var entry in _fields)
                    {
                        if (entry.Key.Name.ToUpper() == fieldData.Name.ToUpper() &&
                            entry.Key.StepId == fieldData.StepId &&
                            entry.Key.StepIncrementId == fieldData.StepIncrementId)
                        {
                            float[] allValues = entry.Value.GetComponentValues(fieldData.Component);
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
                            break;
                        }
                    }
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
            return GetScaledExtremeValues(partName, fieldData, 0, 0);
        }
        public NodesExchangeData GetScaledExtremeValues(string partName, FieldData fieldData, float absoluteScale,
                                                        float relativeScale)
        {
            if (!fieldData.Valid) return null;
            //
            NodesExchangeData nodesData = new NodesExchangeData();
            nodesData.Ids = new int[2];
            nodesData.Coor = new double[2][];
            nodesData.Values = new float[2];
            int minId = -1;
            int maxId = -1;

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
                    if (fieldEntry.Key.Name == fieldData.Name && fieldEntry.Key.StepId == fieldData.StepId && fieldEntry.Key.StepIncrementId == fieldData.StepIncrementId)
                    {
                        float value;
                        float[] values = fieldEntry.Value.GetComponentValues(fieldData.Component);

                        nodesData.Values[0] = float.MaxValue;
                        nodesData.Values[1] = -float.MaxValue;

                        foreach (var nodeId in _mesh.Parts[partName].NodeLabels)
                        {
                            value = values[_nodeIdsLookUp[nodeId]];
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

                        if (relativeScale < 0)  // swap min and max
                        {
                            int tmp = minId; minId = maxId; maxId = tmp;
                            float tmpD = nodesData.Values[0]; nodesData.Values[0] = nodesData.Values[1]; nodesData.Values[1] = tmpD;
                        }

                        nodesData.Ids[0] = minId;
                        nodesData.Ids[1] = maxId;

                        nodesData.Coor[0] = _mesh.Nodes[minId].Coor;
                        nodesData.Coor[1] = _mesh.Nodes[maxId].Coor;
                        ScaleNodeCoordinates(absoluteScale, fieldEntry.Key.StepId, fieldEntry.Key.StepIncrementId, nodesData.Ids, ref nodesData.Coor);

                        nodesData.Values[0] *= relativeScale;
                        nodesData.Values[1] *= relativeScale;

                        //nodesData.Values[0] = fieldEntry.Value.GetComponentMin(fieldData.Component) * relativeScale;
                        //nodesData.Values[1] = fieldEntry.Value.GetComponentMax(fieldData.Component) * relativeScale;
                        break;
                    }
                }
            }
            return nodesData;
        }
        // Scaled results
        public void GetScaledNodesAndValues(FieldData fieldData, float scale, int[] nodeIds, out double[][] nodeCoor,
                                            out float[] values)
        {
            nodeCoor = new double[nodeIds.Length][];
            for (int i = 0; i < nodeIds.Length; i++)
            {
                nodeCoor[i] = Mesh.Nodes[nodeIds[i]].Coor;
            }

            values = GetValues(fieldData, nodeIds);
            if (scale != 0) ScaleNodeCoordinates(scale, fieldData.StepId, fieldData.StepIncrementId, nodeIds, ref nodeCoor);
        }
        public void GetUndeformedNodesAndCells(BasePart part, out double[][] nodeCoor, out int[][] cells, out int[] cellTypes)
        {
            int[] nodeIds, cellIds;
            _mesh.GetVisualizationNodesAndCells(part, out nodeIds, out nodeCoor, out cellIds, out cells, out cellTypes);
        }
        //
        public PartExchangeData GetScaledAllNodesCellsAndValues(FeGroup elementSet, FieldData fData, float scale)
        {
            PartExchangeData pData = new PartExchangeData();
            _mesh.GetAllNodesAndCells(elementSet, out pData.Nodes.Ids, out pData.Nodes.Coor, out pData.Cells.Ids,
                                      out pData.Cells.CellNodeIds, out pData.Cells.Types);
            if (!fData.Valid) pData.Nodes.Values = null;
            else pData.Nodes.Values = GetValues(fData, pData.Nodes.Ids);
            if (scale != 0)
                ScaleNodeCoordinates(scale, fData.StepId, fData.StepIncrementId, pData.Nodes.Ids, ref pData.Nodes.Coor);
            return pData;
        }
        public PartExchangeData GetScaledEdgesCells(int[][] edgeCells, FieldData fData, float scale)
        {
            PartExchangeData pData = new PartExchangeData();
            _mesh.GetNodesAndCellsForEdges(edgeCells, out pData.Nodes.Ids, out pData.Nodes.Coor,
                                           out pData.Cells.CellNodeIds, out pData.Cells.Types);
            if (scale != 0)
                ScaleNodeCoordinates(scale, fData.StepId, fData.StepIncrementId, pData.Nodes.Ids,
                                     ref pData.Nodes.Coor);
            return pData;
        }
        public PartExchangeData GetScaledVisualizationNodesCellsAndValues(BasePart part, FieldData fData, float scale)
        {
            PartExchangeData pData = new PartExchangeData();
            _mesh.GetVisualizationNodesAndCells(part, out pData.Nodes.Ids, out pData.Nodes.Coor, out pData.Cells.Ids,
                                                out pData.Cells.CellNodeIds, out pData.Cells.Types);
            if (!fData.Valid) pData.Nodes.Values = null;
            else
            {
                pData.Nodes.Values = GetValues(fData, pData.Nodes.Ids);
                pData.ExtremeNodes = GetScaledExtremeValues(part.Name, fData, scale, 1);
            }
            if (scale != 0)
                ScaleNodeCoordinates(scale, fData.StepId, fData.StepIncrementId, pData.Nodes.Ids, ref pData.Nodes.Coor);
            return pData;
        }
        public PartExchangeData GetScaledEdgesNodesAndCells(FeGroup elementSet, FieldData fData, float scale)
        {
            PartExchangeData resultData = new PartExchangeData();
            _mesh.GetNodesAndCellsForModelEdges(elementSet, out resultData.Nodes.Ids, out resultData.Nodes.Coor,
                                                out resultData.Cells.CellNodeIds, out resultData.Cells.Types);
            if (scale != 0)
                ScaleNodeCoordinates(scale, fData.StepId, fData.StepIncrementId, resultData.Nodes.Ids, ref resultData.Nodes.Coor);
            return resultData;
        }
        // Animation        
        public PartExchangeData GetScaleFactorAnimationDataVisualizationNodesCellsAndValues(BasePart part, FieldData fData,
                                                                                            float scale, int numFrames)
        {
            PartExchangeData pData = new PartExchangeData();
            _mesh.GetVisualizationNodesAndCells(part, out pData.Nodes.Ids, out pData.Nodes.Coor, out pData.Cells.Ids,
                                                out pData.Cells.CellNodeIds, out pData.Cells.Types);
            if (!fData.Valid) pData.Nodes.Values = null;
            else
            {
                pData.Nodes.Values = GetValues(fData, pData.Nodes.Ids);
                pData.ExtremeNodes = GetExtremeValues(part.Name, fData);
            }
            //
            pData.NodesAnimation = new NodesExchangeData[numFrames];
            pData.ExtremeNodesAnimation = new NodesExchangeData[numFrames];
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
                //
                pData.NodesAnimation[i] = new NodesExchangeData();
                ScaleNodeCoordinates(absoluteScale, fData.StepId, fData.StepIncrementId, pData.Nodes.Ids, pData.Nodes.Coor,
                                     out pData.NodesAnimation[i].Coor);
                //
                if (invariant) relativeScale = Math.Abs(relativeScale);
                ScaleValues(relativeScale, pData.Nodes.Values, out pData.NodesAnimation[i].Values);
                pData.ExtremeNodesAnimation[i] = GetScaledExtremeValues(part.Name, fData, absoluteScale, relativeScale);
            }
            //
            return pData;
        }
        public PartExchangeData GetScaleFactorAnimationDataEdgesNodesAndCells(BasePart part, FieldData fData, float scale,
                                                                              int numFrames)
        {
            PartExchangeData pData = new PartExchangeData();
            _mesh.GetNodesAndCellsForModelEdges(part, out pData.Nodes.Ids, out pData.Nodes.Coor, out pData.Cells.CellNodeIds,
                                                out pData.Cells.Types);
            pData.NodesAnimation = new NodesExchangeData[numFrames];
            //
            float[] ratios;
            if (fData.Type == StepType.Frequency || fData.Type == StepType.Buckling) ratios = GetRelativeModalScales(numFrames);
            else ratios = GetRelativeScales(numFrames);
            //
            float absoluteScale;
            bool invariant = IsComponentInvariant(fData);
            //
            for (int i = 0; i < numFrames; i++)
            {
                absoluteScale = ratios[i] * scale;
                //
                pData.NodesAnimation[i] = new NodesExchangeData();
                ScaleNodeCoordinates(absoluteScale, fData.StepId, fData.StepIncrementId, pData.Nodes.Ids, pData.Nodes.Coor,
                                     out pData.NodesAnimation[i].Coor);
            }
            //
            return pData;
        }
        public PartExchangeData GetScaleFactorAnimationDataAllNodesCellsAndValues(FeGroup elementSet, FieldData fData, float scale,
                                                                                  int numFrames)
        {
            PartExchangeData pData = new PartExchangeData();
            _mesh.GetAllNodesAndCells(elementSet, out pData.Nodes.Ids, out pData.Nodes.Coor, out pData.Cells.Ids, out pData.Cells.CellNodeIds, out pData.Cells.Types);
            if (!fData.Valid) pData.Nodes.Values = null;
            else
            {
                pData.Nodes.Values = GetValues(fData, pData.Nodes.Ids);
            }
            //
            pData.NodesAnimation = new NodesExchangeData[numFrames];
            pData.ExtremeNodesAnimation = new NodesExchangeData[numFrames];
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
                //
                pData.NodesAnimation[i] = new NodesExchangeData();
                ScaleNodeCoordinates(absoluteScale, fData.StepId, fData.StepIncrementId, pData.Nodes.Ids, pData.Nodes.Coor, out pData.NodesAnimation[i].Coor);
                //
                if (invariant) relativeScale = Math.Abs(relativeScale);
                ScaleValues(relativeScale, pData.Nodes.Values, out pData.NodesAnimation[i].Values);
                pData.ExtremeNodesAnimation[i] = GetScaledExtremeValues(elementSet.Name, fData, absoluteScale, relativeScale);
            }
            //
            return pData;
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
        public PartExchangeData GetTimeIncrementAnimationDataVisualizationNodesCellsAndValues(BasePart part, FieldData fData,
                                                                                              float scale)
        {
            return GetTimeIncrementAnimationData(part, fData, scale, GetScaledVisualizationNodesCellsAndValues);
        }
        public PartExchangeData GetTimeIncrementAnimationDataVisualizationEdgesNodesAndCells(BasePart part, FieldData fData,
                                                                                             float scale)
        {
            return GetTimeIncrementAnimationData(part, fData, scale, GetScaledEdgesNodesAndCells);
        }
        public PartExchangeData GetTimeIncrementAnimationDataAllNodesCellsAndValues(BasePart part, FieldData fData, float scale)
        {
            return GetTimeIncrementAnimationData(part, fData, scale, GetScaledAllNodesCellsAndValues);
        }
        private PartExchangeData GetTimeIncrementAnimationData(BasePart part, FieldData fData, float scale, Func<BasePart, FieldData,
                                                               float, PartExchangeData> GetGeometryData)
        {
            PartExchangeData pData = GetGeometryData(part, fData, scale);

            // get all existing increments
            Dictionary<int, int[]> existingStepIncrementIds = GetExistingIncrementIds(fData.Name, fData.Component);
            // count all existing increments
            int numFrames = 0;
            foreach (var entry in existingStepIncrementIds) numFrames += entry.Value.Length;

            // create animation frames
            pData.NodesAnimation = new NodesExchangeData[numFrames];
            pData.ExtremeNodesAnimation = new NodesExchangeData[numFrames];

            FieldData tmpFieldData = new FieldData(fData);  // create a copy
            int count = 0;
            foreach (var entry in existingStepIncrementIds)
            {
                tmpFieldData.StepId = entry.Key;
                foreach (var incrementId in entry.Value)
                {
                    tmpFieldData.StepIncrementId = incrementId;

                    PartExchangeData animData = GetGeometryData(part, tmpFieldData, scale);
                    pData.NodesAnimation[count] = animData.Nodes;
                    pData.ExtremeNodesAnimation[count] = animData.ExtremeNodes;
                    count++;
                }
            }

            return pData;
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

                float[][] disp = new float[3][];
                disp[0] = GetValues(new FieldData(name, "U1", stepId, stepIncrementId), globalNodeIds);
                disp[1] = GetValues(new FieldData(name, "U2", stepId, stepIncrementId), globalNodeIds);
                disp[2] = GetValues(new FieldData(name, "U3", stepId, stepIncrementId), globalNodeIds);

                scaledNodes = new double[nodes.GetLength(0)][];
                for (int i = 0; i < nodes.GetLength(0); i++) scaledNodes[i] = nodes[i].ToArray();  // copy coordinates

                if (disp[0] != null && disp[1] != null && disp[2] != null)
                {
                    for (int i = 0; i < nodes.GetLength(0); i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            scaledNodes[i][j] += scale * disp[j][i];
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
                //
                float[][] disp = new float[3][];
                disp[0] = GetValues(new FieldData(name, "U1", stepId, stepIncrementId), globalNodeIds);
                disp[1] = GetValues(new FieldData(name, "U2", stepId, stepIncrementId), globalNodeIds);
                disp[2] = GetValues(new FieldData(name, "U3", stepId, stepIncrementId), globalNodeIds);
                //
                if (disp[0] != null && disp[1] != null && disp[2] != null)
                {
                    for (int i = 0; i < nodes.GetLength(0); i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            nodes[i][j] += scale * disp[j][i];
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

    }
}
