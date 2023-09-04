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
    public class SpecificHeat : MaterialProperty, ISerializable
    {
        // Variables                                                                                                                
        private EquationContainer[][] _specificHeatTemp;        //ISerializable


        // Properties                                                                                                               
        public EquationContainer[][] SpecificHeatTemp { get { return _specificHeatTemp; } set { SetSpecificHeatTemp(value); } }


        // Constructors                                                                                                             
        public SpecificHeat(double[][] specificHeatTemp)
        {
            SetSpecificHeatTemp(specificHeatTemp, false);
        }
        public SpecificHeat(EquationContainer[][] specificHeatTemp)
        {
            SetSpecificHeatTemp(specificHeatTemp, false);
        }
        public SpecificHeat(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_specificHeatTemp":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double[][] values)
                            SetSpecificHeatTemp(values, false);
                        else
                            SetSpecificHeatTemp((EquationContainer[][])entry.Value, false);
                        break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        private void SetSpecificHeatTemp(double[][] value, bool checkEquation = true)
        {
            _specificHeatTemp = new EquationContainer[value.Length][];
            //
            for (int i = 0; i < value.Length; i++)
            {
                _specificHeatTemp[i] = new EquationContainer[2];
                _specificHeatTemp[i][0] = new EquationContainer(typeof(StringSpecificHeatConverter), value[i][0]);
                _specificHeatTemp[i][1] = new EquationContainer(typeof(StringTemperatureConverter), value[i][1]);
            }
            SetSpecificHeatTemp(_specificHeatTemp, checkEquation);
        }
        private void SetSpecificHeatTemp(EquationContainer[][] value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _specificHeatTemp, value, new Func<double, double>[] { CheckPositive, null },
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
            for (int i = 0; i < _specificHeatTemp.Length; i++)
            {
                _specificHeatTemp[i][0].CheckEquation();
                _specificHeatTemp[i][1].CheckEquation();
            }
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_specificHeatTemp", _specificHeatTemp, typeof(EquationContainer[][]));
        }
    }
}
