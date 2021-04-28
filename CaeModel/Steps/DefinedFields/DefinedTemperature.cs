using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using System.ComponentModel;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
                
    public class DefinedTemperature : DefinedField
    {
        // Variables                                                                                                                
        private double _temperature;
        private string _fileName;
        private int _stepNumber;


        // Properties                                                                                                               
        public double Temperature { get { return _temperature; } set { _temperature = value; } }
        public string FileName { get { return _fileName; } set { _fileName = value; } }
        public int StepNumber { get { return _stepNumber; } set { _stepNumber = value; } }


        // Constructors                                                                                                             
        public DefinedTemperature(string name, string regionName, RegionTypeEnum regionType)
            : base(name, regionName, regionType)
        {
            _temperature = 0;
            _fileName = null;
            _stepNumber = -1;
        }


        // Methods                                                                                                                  
    }
}
