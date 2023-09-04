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
    public class ThermalExpansion : MaterialProperty, ISerializable
    {
        // Variables                                                                                                                
        private EquationContainer[][] _thermalExpansionTemp;        //ISerializable
        private EquationContainer _zeroTemperature;                 //ISerializable


        // Properties                                                                                                               
        public EquationContainer[][] ThermalExpansionTemp
        {
            get { return _thermalExpansionTemp; }
            set { SetThermalExpansionTemp(value); }
        }
        
        public EquationContainer ZeroTemperature { get { return _zeroTemperature; } set { SetZeroTemperature(value); } }


        // Constructors                                                                                                             
        public ThermalExpansion(double[][] thermalExpansionTemp)
        {
            SetThermalExpansionTemp(thermalExpansionTemp, false);
            ZeroTemperature = new EquationContainer(typeof(StringTemperatureConverter), 20);
        }
        public ThermalExpansion(EquationContainer[][] thermalExpansionTemp)
        {
            SetThermalExpansionTemp(thermalExpansionTemp, false);
            ZeroTemperature = new EquationContainer(typeof(StringTemperatureConverter), 20);
        }
        public ThermalExpansion(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_thermalExpansionTemp":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double[][] values)
                            SetThermalExpansionTemp(values, false);
                        else
                            SetThermalExpansionTemp((EquationContainer[][])entry.Value, false);
                        break;
                    case "_zeroTemperature":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueT)
                            ZeroTemperature = new EquationContainer(typeof(StringTemperatureConverter), valueT);
                        else
                            SetZeroTemperature((EquationContainer)entry.Value, false);
                        break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        private void SetThermalExpansionTemp(double[][] value, bool checkEquation = true)
        {
            _thermalExpansionTemp = new EquationContainer[value.Length][];
            //
            for (int i = 0; i < value.Length; i++)
            {
                _thermalExpansionTemp[i] = new EquationContainer[2];
                _thermalExpansionTemp[i][0] = new EquationContainer(typeof(StringThermalExpansionConverter), value[i][0]);
                _thermalExpansionTemp[i][1] = new EquationContainer(typeof(StringTemperatureConverter), value[i][1]);
            }
            SetThermalExpansionTemp(_thermalExpansionTemp, checkEquation);
        }
        private void SetThermalExpansionTemp(EquationContainer[][] value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _thermalExpansionTemp, value, new Func<double, double>[] { CheckPositive, null },
                                          checkEquation);
        }
        private void SetZeroTemperature(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _zeroTemperature, value, null, checkEquation);
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
            for (int i = 0; i < _thermalExpansionTemp.Length; i++)
            {
                _thermalExpansionTemp[i][0].CheckEquation();
                _thermalExpansionTemp[i][1].CheckEquation();
            }
            _zeroTemperature.CheckEquation();
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_thermalExpansionTemp", _thermalExpansionTemp, typeof(EquationContainer[][]));
            info.AddValue("_zeroTemperature", _zeroTemperature, typeof(EquationContainer));
        }
    }
}
