using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public class ThermalConductivity : MaterialProperty, ISerializable
    {
        // Variables                                                                                                                
        private EquationContainer[][] _thermalConductivityTemp;


        // Properties                                                                                                               
        public EquationContainer[][] ThermalConductivityTemp
        {
            get { return _thermalConductivityTemp; }
            set { SetThermalConductivityTemp(value); }
        }


        // Constructors                                                                                                             
        public ThermalConductivity(double[][] thermalConductivityTemp)
        {
            SetThermalConductivityTemp(thermalConductivityTemp, false);
        }
        public ThermalConductivity(EquationContainer[][] thermalConductivityTemp)
        {
            SetThermalConductivityTemp(thermalConductivityTemp, false);
        }
        public ThermalConductivity(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_thermalConductivityTemp":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double[][] values)
                            SetThermalConductivityTemp(values, false);
                        else
                            SetThermalConductivityTemp((EquationContainer[][])entry.Value, false);
                        break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        private void SetThermalConductivityTemp(double[][] value, bool checkEquation = true)
        {
            _thermalConductivityTemp = new EquationContainer[value.Length][];
            //
            for (int i = 0; i < value.Length; i++)
            {
                _thermalConductivityTemp[i] = new EquationContainer[2];
                _thermalConductivityTemp[i][0] = new EquationContainer(typeof(StringThermalConductivityConverter), value[i][0]);
                _thermalConductivityTemp[i][1] = new EquationContainer(typeof(StringTemperatureConverter), value[i][1]);
            }
            SetThermalConductivityTemp(_thermalConductivityTemp, checkEquation);
        }
        private void SetThermalConductivityTemp(EquationContainer[][] value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _thermalConductivityTemp, value, new Func<double, double>[] { CheckPositive, null },
                                          checkEquation);
        }
        //
        private double CheckPositive(double value)
        {
            if (value <= 0) throw new CaeException(_positive);
            else return value;
        }
        // IContainsEquations
        public override void CheckEquations()
        {
            for (int i = 0; i < _thermalConductivityTemp.Length; i++)
            {
                _thermalConductivityTemp[i][0].CheckEquation();
                _thermalConductivityTemp[i][1].CheckEquation();
            }
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_thermalConductivityTemp", _thermalConductivityTemp, typeof(EquationContainer[][]));
        }
    }
}
