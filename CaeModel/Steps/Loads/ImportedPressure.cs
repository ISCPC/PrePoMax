using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using CaeResults;
using System.IO;

namespace CaeModel
{
    [Serializable]
    public class ImportedPressure : VariablePressure
    {
        // Variables                                                                                                                
        private string _fileName;
        private string _pressureTime;
        private string _pressureVariableName;
        private InterpolatorEnum _interpolator;
        private float _scaleFactor;
        [NonSerialized]
        private PartExchangeData _sourceData;
        [NonSerialized]
        private Dictionary<double[], float[]> _coorDistanceValue;
        //
        private FileInfo _oldFileInfo;
        private string _oldPressureTime;
        private string _oldPressureVariableName;
        private InterpolatorEnum _oldInterpolator;
        private int _oldMeshNodesHash;
        private int _oldElementSetHash;


        // Properties                                                                                                               
        public string FileName { get { return _fileName; } set { _fileName = value; } }
        public string PressureTime { get { return _pressureTime; } set { _pressureTime = value; } }
        public string PressureVariableName { get { return _pressureVariableName; } set { _pressureVariableName = value; } }
        public InterpolatorEnum Interpolator { get { return _interpolator; } set { _interpolator = value; } }
        public float ScaleFactor { get { return _scaleFactor; } set { _scaleFactor = value; } }
        public PartExchangeData SourceData { get { return _sourceData; } set { _sourceData = value; } }


        // Constructors                                                                                                             
        public ImportedPressure(string name, string surfaceName, RegionTypeEnum regionType, bool twoD)
            : base(name, surfaceName, regionType, twoD)
        {
            _fileName = null;
            _pressureTime = null;
            _pressureVariableName = null;
            _interpolator = InterpolatorEnum.ClosestNode;
            _scaleFactor = 1;
            _sourceData = null;
            _coorDistanceValue = null;
            //
            _oldFileInfo = null;
            _oldPressureTime = null;
            _oldPressureVariableName = null;
            _oldInterpolator = InterpolatorEnum.ClosestNode;
            _oldMeshNodesHash = 0;
            _oldElementSetHash = 0;
        }


        // Methods                                                                                                                  
        public bool IsProperlyDefined(out string error)
        {
            error = "";
            if (!System.IO.File.Exists(_fileName))
            {
                error = "The selected file does not exist.";
                return false;
            }
            //
            return true;
        }
        public void ImportPressure(FeMesh targetMesh)
        {
            bool updateData = false;
            bool updateInterpolation = false;
            FileInfo fileInfo = new FileInfo(_fileName);
            int meshNodesHash = targetMesh.GetHashCode();
            FeSurface surface = targetMesh.Surfaces[_surfaceName];
            FeElementSet elementSet = new FeElementSet("surface", targetMesh.GetSurfaceElementIds(_surfaceName));
            int elementSetHash = elementSet.GetHashCode();
            //
            if (fileInfo.Exists)
            {
                if (_oldFileInfo == null) updateData = true;
                else if (_sourceData == null) updateData = true;
                //
                else if (fileInfo.Name != _oldFileInfo.Name) updateData = true;
                // Files have the same name - check if newer
                else if (fileInfo.LastWriteTimeUtc < _oldFileInfo.LastWriteTimeUtc) updateData = true;
                //


                //always update !!?!?!?


                else if (_pressureTime != _oldPressureTime) updateData = true;
                else if (_pressureVariableName != _oldPressureVariableName) updateData = true;
                //
                else if (_interpolator != _oldInterpolator) updateInterpolation = true;
                else if (meshNodesHash != _oldMeshNodesHash) updateInterpolation = true;
                else if (elementSetHash != _oldElementSetHash) updateInterpolation = true;
                else if (_coorDistanceValue == null) updateInterpolation = true;
            }
            else
            {
                string nofile = "The file from which the pressure should be imported does not exist.";
                if (_sourceData == null) throw new CaeException(nofile);
                else MessageBoxes.ShowWarning(nofile + " Using previously imported results.");
            }
            //
            float[] distances;
            float[] values;
            if (updateData)
            {
                _oldFileInfo = fileInfo;
                _oldPressureTime = _pressureTime;
                _oldPressureVariableName = _pressureVariableName;
                // Get results
                FeResults results = OpenFoamFileReader.Read(_fileName, double.Parse(_pressureTime), _pressureVariableName);
                if (results == null) throw new CaeException("No pressure was imported.");
                // Get pressure field data
                FieldData[] fieldData = results.GetAllFieldData(); // use GetResults for the first time to check existance
                Dictionary<string, string[]> filedNameComponentNames = results.GetAllFiledNameComponentNames();
                if (fieldData == null || fieldData.Length != 1) throw new CaeException("Pressure field could not be found.");
                //
                FieldData pressureData = fieldData[0];
                //
                string[] componentNames;
                filedNameComponentNames.TryGetValue(fieldData[0].Name, out componentNames);
                if (componentNames.Length != 1) throw new CaeException("Component of the pressure field could not be found.");
                //
                pressureData.Component = componentNames[0];
                // Get source part exchange data
                _sourceData = results.GetAllNodesCellsAndValues(pressureData);
            }
            if (updateData || updateInterpolation)
            {
                _oldInterpolator = _interpolator;
                _oldMeshNodesHash = meshNodesHash;
                _oldElementSetHash = elementSetHash;
                // Get target part exchange data
                PartExchangeData targetData = new PartExchangeData();
                
                //
                targetMesh.GetSetNodesAndCells(elementSet, out targetData.Nodes.Ids, out targetData.Nodes.Coor,
                                               out targetData.Cells.Ids, out targetData.Cells.CellNodeIds,
                                               out targetData.Cells.Types);
                //targetMesh.GetAllNodesAndCells(out targetData.Nodes.Ids, out targetData.Nodes.Coor, out targetData.Cells.Ids,
                //                               out targetData.Cells.CellNodeIds, out targetData.Cells.Types);
                ResultsInterpolators.InterpolateScalarResults(_sourceData, targetData, _interpolator, out distances, out values);
                //
                CompareDoubleArray comparer = new CompareDoubleArray();
                _coorDistanceValue = new Dictionary<double[], float[]>(comparer);
                for (int i = 0; i < targetData.Nodes.Coor.Length; i++)
                {
                    _coorDistanceValue.Add(targetData.Nodes.Coor[i], new float[] { distances[i], values[i] });
                }
            }
        }
        public double GetDistanceForPoint(double[] point)
        {
            float[] distanceValue;
            if (_coorDistanceValue.TryGetValue(point, out distanceValue)) return distanceValue[0];
            else return float.NaN;
        }
        public override double GetPressureForPoint(double[] point)
        {
            float[] distanceValue;
            // Scale value
            if (_coorDistanceValue.TryGetValue(point, out distanceValue)) return distanceValue[1] * _scaleFactor;
            else return float.NaN;
        }
        
    }
}
