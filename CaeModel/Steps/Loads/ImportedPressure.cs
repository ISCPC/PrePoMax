using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public class ImportedPressure : Load
    {
        // Variables                                                                                                                
        private string _surfaceName;
        private RegionTypeEnum _regionType;
        private string _fileName;
        private string _pressureTime;
        private string _pressureVariableName;
        private double _scaleFactor;


        // Properties                                                                                                               
        public override string RegionName { get { return _surfaceName; } set { _surfaceName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public string SurfaceName { get { return _surfaceName; } set { _surfaceName = value; } }
        public string FileName { get { return _fileName; } set { _fileName = value; } }
        public string PressureTime { get { return _pressureTime; } set { _pressureTime = value; } }
        public string PressureVariableName { get { return _pressureVariableName; } set { _pressureVariableName = value; } }
        public double ScaleFactor { get { return _scaleFactor; } set { _scaleFactor = value; } }


        // Constructors                                                                                                             
        public ImportedPressure(string name, string surfaceName, RegionTypeEnum regionType, bool twoD)
            : base(name, twoD)
        {
            _surfaceName = surfaceName;
            _regionType = regionType;
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
    }
}
