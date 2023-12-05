using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using System.Runtime.Serialization;

namespace CaeModel
{
    [Serializable]
    public class SlipWear : MaterialProperty, ISerializable
    {
        // Variables                                                                                                                
        private EquationContainer _hardness;            //ISerializable
        private EquationContainer _wearCoefficient;     //ISerializable


        // Properties                                                                                                               
        public EquationContainer Hardness { get { return _hardness; } set { SetHardness(value); } }
        public EquationContainer WearCoefficient { get { return _wearCoefficient; } set { SetWearCoefficient(value); } }


        // Constructors                                                                                                             
        public SlipWear(double hardness, double wearCoefficient)
        {
            // The constructor must wotk with H = 0; K = 0
            SetHardness(new EquationContainer(typeof(StringPressureConverter), hardness), false);
            SetWearCoefficient(new EquationContainer(typeof(StringDoubleConverter), wearCoefficient), false);
        }
        public SlipWear(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_hardness":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueH)
                            Hardness = new EquationContainer(typeof(StringPressureConverter), valueH);
                        else
                            SetHardness((EquationContainer)entry.Value, false);
                        break;
                    case "_wearCoefficient":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueW)
                            WearCoefficient = new EquationContainer(typeof(StringDoubleConverter), valueW);
                        else
                            SetWearCoefficient((EquationContainer)entry.Value, false);
                        break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        private void SetHardness(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _hardness, value, CheckPositive, checkEquation);
        }
        private void SetWearCoefficient(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _wearCoefficient, value, CheckPositive, checkEquation);
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
            _hardness.CheckEquation();
            _wearCoefficient.CheckEquation();
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_hardness", _hardness, typeof(EquationContainer));
            info.AddValue("_wearCoefficient", _wearCoefficient, typeof(EquationContainer));
        }
    }
}
