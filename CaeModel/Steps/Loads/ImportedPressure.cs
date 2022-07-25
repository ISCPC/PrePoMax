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
        private double _scaleFactor;
        private PartExchangeData _sourceData;
        [NonSerialized] Dictionary<double[], float[]> _coorDistanceValue;
        //
        private FileInfo _oldFileInfo;
        private string _oldPressureTime;
        private string _oldPressureVariableName;
        private InterpolatorEnum _oldInterpolator;
        private double _oldScaleFactor;
        private int _oldMeshNodesHash;


        // Properties                                                                                                               
        public string FileName { get { return _fileName; } set { _fileName = value; } }
        public string PressureTime { get { return _pressureTime; } set { _pressureTime = value; } }
        public string PressureVariableName { get { return _pressureVariableName; } set { _pressureVariableName = value; } }
        public InterpolatorEnum Interpolator { get { return _interpolator; } set { _interpolator = value; } }
        public double ScaleFactor { get { return _scaleFactor; } set { _scaleFactor = value; } }
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
            bool update = false;
            FileInfo fileInfo = new FileInfo(_fileName);
            int meshNodesHash = targetMesh.GetNodesHash();
            //
            if (fileInfo.Exists)
            {
                if (_oldFileInfo == null) update = true;
                //
                else if (fileInfo.Name != _oldFileInfo.Name) update = true;
                // Files have the same name - check if newer
                else if (fileInfo.LastWriteTimeUtc < _oldFileInfo.LastWriteTimeUtc) update = true;
                //
                else if (_pressureTime != _oldPressureTime) update = true;
                else if (_pressureVariableName != _oldPressureVariableName) update = true;
                else if (_interpolator != _oldInterpolator) update = true;
                else if (_scaleFactor != _oldScaleFactor) update = true;
                else if (meshNodesHash != _oldMeshNodesHash) update = true;
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
            if (update)
            {
                _oldFileInfo = fileInfo;
                _oldPressureTime = _pressureTime;
                _oldPressureVariableName = _pressureVariableName;
                _oldInterpolator = _interpolator;
                _oldScaleFactor = _scaleFactor;
                _oldMeshNodesHash = meshNodesHash;
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
                // Scale values
                float scale = (float)_scaleFactor;
                if (scale != 1)
                {
                    for (int i = 0; i < _sourceData.Nodes.Values.Length; i++) _sourceData.Nodes.Values[i] *= scale;
                }
                // Get target part exchange data
                PartExchangeData targetData = new PartExchangeData();
                targetMesh.GetAllNodesAndCells(out targetData.Nodes.Ids, out targetData.Nodes.Coor, out targetData.Cells.Ids,
                                               out targetData.Cells.CellNodeIds, out targetData.Cells.Types);
                //
                ResultsInterpolators.InterpolateScalarResults(_sourceData, targetData, _interpolator, out distances, out values);
                //
                CompareDoubleArray comparer = new CompareDoubleArray();
                _coorDistanceValue = new Dictionary<double[], float[]>(comparer);
                for (int i = 0; i < targetData.Nodes.Coor.Length; i++)
                {
                    _coorDistanceValue.Add(targetData.Nodes.Coor[i], new float[] { distances[i], values[i] });
                }
            }
            //
            if (_coorDistanceValue == null || update)
            {

            }
        }
        private void Interpolate()
        {

        }
        public double GetDistanceForPoint(double[] point)
        {
            float[] distanceValue = _coorDistanceValue[point];
            return distanceValue[0];
        }
        public override double GetPressureForPoint(double[] point)
        {
            float[] distanceValue = _coorDistanceValue[point];
            return distanceValue[1];
        }
        
    }
}
