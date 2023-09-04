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
    public class Density : MaterialProperty, ISerializable
    {
        // Variables                                                                                                                
        private EquationContainer[][] _densityTemp;         //ISerializable


        // Properties                                                                                                               
        public EquationContainer[][] DensityTemp { get { return _densityTemp; } set { SetDensityTemp(value); } }


        // Constructors                                                                                                             
        public Density(double[][] densityTemp)
        {
            SetDensityTemp(densityTemp, false);
        }
        public Density(EquationContainer[][] densityTemp)
        {
            SetDensityTemp(densityTemp, false);
        }
        public Density(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_densityTemp":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double[][] values)
                            SetDensityTemp(values, false);
                        else
                            SetDensityTemp((EquationContainer[][])entry.Value, false);
                        break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        private void SetDensityTemp(double[][] value, bool checkEquation = true)
        {
            _densityTemp = new EquationContainer[value.Length][];
            //
            for (int i = 0; i < value.Length; i++)
            {
                _densityTemp[i] = new EquationContainer[2];
                _densityTemp[i][0] = new EquationContainer(typeof(StringDensityConverter), value[i][0]);
                _densityTemp[i][1] = new EquationContainer(typeof(StringTemperatureConverter), value[i][1]);
            }
            SetDensityTemp(_densityTemp, checkEquation);
        }
        private void SetDensityTemp(EquationContainer[][] value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _densityTemp, value, new Func<double, double>[] { CheckPositive, null },
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
            for (int i = 0; i < _densityTemp.Length; i++)
            {
                _densityTemp[i][0].CheckEquation();
                _densityTemp[i][1].CheckEquation();
            }
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_densityTemp", _densityTemp, typeof(EquationContainer[][]));
        }
    }
}
