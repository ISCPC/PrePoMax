using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using System.Runtime.Serialization;
using CaeResults;
using FileInOut.Output.Calculix;
using System.Xml.Linq;

namespace CaeModel
{
    [Serializable]
    public class Elastic : MaterialProperty, ISerializable
    {
        // Variables                                                                                                                
        private EquationContainer[][] _youngsPoissonsTemp;         //ISerializable


        // Properties                                                                                                               
        public EquationContainer[][] YoungsPoissonsTemp
        {
            get { return _youngsPoissonsTemp; }
            set { SetYoungsPoissonsTemp(value); }
        }


        // Constructors                                                                                                             
        public Elastic(double[][] youngsPoissonsTemp)
        {
            SetYoungsPoissonsTemp(youngsPoissonsTemp, false);
        }
        public Elastic(EquationContainer[][] youngsPoissonsTemp)
        {
            SetYoungsPoissonsTemp(youngsPoissonsTemp, false);
        }
        public Elastic(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_youngsPoissonsTemp":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double[][] values)
                            SetYoungsPoissonsTemp(values, false);
                        else
                            SetYoungsPoissonsTemp((EquationContainer[][])entry.Value, false);
                        break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        private void SetYoungsPoissonsTemp(double[][] value, bool checkEquation = true)
        {
            _youngsPoissonsTemp = new EquationContainer[value.Length][];
            //
            for (int i = 0; i < value.Length; i++)
            {
                _youngsPoissonsTemp[i] = new EquationContainer[3];
                _youngsPoissonsTemp[i][0] = new EquationContainer(typeof(StringPressureConverter), value[i][0]);
                _youngsPoissonsTemp[i][1] = new EquationContainer(typeof(StringDoubleConverter), value[i][1]);
                _youngsPoissonsTemp[i][2] = new EquationContainer(typeof(StringTemperatureConverter), value[i][2]);
            }
            SetYoungsPoissonsTemp(_youngsPoissonsTemp, checkEquation);
        }
        private void SetYoungsPoissonsTemp(EquationContainer[][] value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _youngsPoissonsTemp, value, new Func<double, double>[] { CheckPositive, null, null },
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
            for (int i = 0; i < _youngsPoissonsTemp.Length; i++)
            {
                _youngsPoissonsTemp[i][0].CheckEquation();
                _youngsPoissonsTemp[i][1].CheckEquation();
                _youngsPoissonsTemp[i][2].CheckEquation();
            }
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_youngsPoissonsTemp", _youngsPoissonsTemp, typeof(EquationContainer[][]));
        }
    }
}
