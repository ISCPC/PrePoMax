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
        private InterpolatorEnum _interpolatorType;
        private float _scaleFactor;
        //
        private FileInfo _oldFileInfo;
        //
        [NonSerialized]
        private ResultsInterpolator _interpolator;


        // Properties                                                                                                               
        public string FileName { get { return _fileName; } set { _fileName = value; } }
        public string PressureTime { get { return _pressureTime; } set { _pressureTime = value; } }
        public string PressureVariableName { get { return _pressureVariableName; } set { _pressureVariableName = value; } }
        public InterpolatorEnum InterpolatorType { get { return _interpolatorType; } set { _interpolatorType = value; } }
        public float ScaleFactor { get { return _scaleFactor; } set { _scaleFactor = value; } }


        // Constructors                                                                                                             
        public ImportedPressure(string name, string surfaceName, RegionTypeEnum regionType, bool twoD)
            : base(name, surfaceName, regionType, twoD)
        {
            _fileName = null;
            _pressureTime = null;
            _pressureVariableName = null;
            _interpolatorType = InterpolatorEnum.ClosestNode;
            _scaleFactor = 1;
            //
            _oldFileInfo = null;
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
        public void ImportPressure()
        {
            bool updateData = false;
            FileInfo fileInfo = new FileInfo(_fileName);
            //
            if (fileInfo.Exists)
            {
                if (_interpolator == null) updateData |= true;  // each time the load is changed it is Cloned -> _interpolator = null
                //
                else if (_oldFileInfo == null) updateData = true;
                else if (fileInfo.Name != _oldFileInfo.Name) updateData = true;
                // Files have the same name - check if newer
                else if (fileInfo.LastWriteTimeUtc < _oldFileInfo.LastWriteTimeUtc) updateData = true;
            }
            else
            {
                string nofile = "The file from which the pressure should be imported does not exist.";
                throw new CaeException(nofile);
            }
            //
            if (updateData)
            {
                _oldFileInfo = fileInfo;
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
                // Initialize interpolator
                _interpolator = new ResultsInterpolator(results.GetAllNodesCellsAndValues(pressureData));
            }
        }
        public void GetPressureAndDistanceForPoint(double[] point, out double[] distance, out double value)
        {
            _interpolator.InterpolateAt(point, _interpolatorType, out distance, out value);
            value *= _scaleFactor;
        }
        public override double GetPressureForPoint(double[] point)
        {
            _interpolator.InterpolateAt(point, _interpolatorType, out double[] distance, out double value);
            return value * _scaleFactor;
        }
        
    }
}
