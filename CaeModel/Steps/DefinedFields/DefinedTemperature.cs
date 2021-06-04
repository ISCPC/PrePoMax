using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using System.ComponentModel;
using DynamicTypeDescriptor;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public enum DefinedTemperatureTypeEnum
    {
        [StandardValue("ByValue", DisplayName = "By value")]
        ByValue,
        [StandardValue("FromFile", DisplayName = "From file")]
        FromFile
    }
    //
    [Serializable]
                
    public class DefinedTemperature : DefinedField
    {
        // Variables                                                                                                                
        private DefinedTemperatureTypeEnum _definedTemperatureType;
        private double _temperature;
        private string _fileName;
        private int _stepNumber;


        // Properties                                                                                                               
        public DefinedTemperatureTypeEnum Type { get { return _definedTemperatureType; } set { _definedTemperatureType = value; } }
        public double Temperature { get { return _temperature; } set { _temperature = value; } }
        public string FileName { get { return _fileName; } set { _fileName = value; } }
        public int StepNumber
        {
            get { return _stepNumber; }
            set
            {
                _stepNumber = value;
                if (_stepNumber < 1) _stepNumber = 1;
            }
        }


        // Constructors                                                                                                             
        public DefinedTemperature(string name, string regionName, RegionTypeEnum regionType, double temperature)
            : base(name, regionName, regionType)
        {
            _definedTemperatureType = DefinedTemperatureTypeEnum.ByValue;
            _temperature = temperature;
            _fileName = null;
            _stepNumber = 1;
        }


        // Methods                                                                                                                  
    }
}
