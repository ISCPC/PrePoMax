using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.Runtime.Serialization;

namespace CaeModel
{    
    [Serializable]
    public class TemperatureBC : BoundaryCondition, ISerializable
    {
        // Variables                                                                                                                
        private EquationContainer _temperature;        //ISerializable


        // Properties                                                                                                               
        public EquationContainer Temperature { get { return _temperature; } set { SetTemperature(value); } }


        // Constructors                                                                                                             
        public TemperatureBC(string name, string regionName, RegionTypeEnum regionType, double temperature, bool twoD)
            : base(name, regionName, regionType, twoD, false, 0) 
        {
            Temperature = new EquationContainer(typeof(StringTemperatureConverter), temperature);
        }
        public TemperatureBC(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_temperature":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double value)
                            Temperature = new EquationContainer(typeof(StringTemperatureConverter), value);
                        else
                            SetTemperature((EquationContainer)entry.Value, false);
                        break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        private void SetTemperature(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _temperature, value, null, checkEquation);
        }
        // IContainsEquations
        public override void CheckEquations()
        {
            base.CheckEquations();
            //
            _temperature.CheckEquation();
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_temperature", _temperature, typeof(EquationContainer));
        }
    }
}
