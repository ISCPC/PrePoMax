using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using CaeResults;

namespace CaeModel
{
    [Serializable]
    public class ImportedPressure : VariablePressure
    {
        // Variables                                                                                                                
        private string _fileName;
        private string _pressureTime;
        private string _pressureVariableName;
        private double _scaleFactor;
        [NonSerialized] PartExchangeData _ped;


        // Properties                                                                                                               
        
        public string FileName { get { return _fileName; } set { _fileName = value; } }
        public string PressureTime { get { return _pressureTime; } set { _pressureTime = value; } }
        public string PressureVariableName { get { return _pressureVariableName; } set { _pressureVariableName = value; } }
        public double ScaleFactor { get { return _scaleFactor; } set { _scaleFactor = value; } }
        public PartExchangeData PartExchangeData { get { return _ped; } }


        // Constructors                                                                                                             
        public ImportedPressure(string name, string surfaceName, RegionTypeEnum regionType, bool twoD)
            : base(name, surfaceName, regionType, twoD)
        {
            _fileName = null;
            _pressureTime = null;
            _pressureVariableName = null;
            _scaleFactor = 1;
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
        public FeResults ImportPressure()
        {
            FeResults results = OpenFoamFileReader.Read(_fileName, double.Parse(_pressureTime), _pressureVariableName);
            if (results == null) throw new CaeException("No pressure was imported.");
            //
            FieldData[] fieldData = results.GetAllFieldData();
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
            //
            _ped = results.GetAllNodesCellsAndValues(pressureData);
            //
            return results;
        }
        public override double GetPressureForPoint(double[] point)
        {
            int i = _ped.Nodes.Ids[0];

            return 1;   
        }
    }
}
