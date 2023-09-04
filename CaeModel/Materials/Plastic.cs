using CaeGlobals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CaeModel
{
    [Serializable]
    public enum PlasticHardening
    {
        Isotropic,
        Kinematic,
        Combined
    }

    [Serializable]
    public class Plastic : MaterialProperty, ISerializable
    {
        // Variables                                                                                                                
        private EquationContainer[][] _stressStrainTemp;        //ISerializable
        private PlasticHardening _hardening;                    //ISerializable


        // Properties                                                                                                               
        public EquationContainer[][] StressStrainTemp { get { return _stressStrainTemp; } set { SetStressStrainTemp(value); } }
        public PlasticHardening Hardening { get { return _hardening; } set {_hardening = value; } }


        // Constructors                                                                                                             
        public Plastic(double[][] stressStrainTemp)
        {
            SetStressStrainTemp(stressStrainTemp, false);
            _hardening = PlasticHardening.Isotropic;
        }
        public Plastic(EquationContainer[][] stressStrainTemp)
        {
            SetStressStrainTemp(stressStrainTemp, false);
            _hardening = PlasticHardening.Isotropic;
        }
        public Plastic(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_stressStrainTemp":
                    case "<StressStrainTemp>k__BackingField":       // Compatibility for version v1.4.0
                        // Compatibility for version v1.4.0
                        if (entry.Value is double[][] values)
                            SetStressStrainTemp(values, false);
                        else
                            SetStressStrainTemp((EquationContainer[][])entry.Value, false);
                        break;
                    case "_hardening":
                    case "<Hardening>k__BackingField":              // Compatibility for version v1.4.0
                        _hardening = (PlasticHardening)entry.Value; break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        private void SetStressStrainTemp(double[][] value, bool checkEquation = true)
        {
            _stressStrainTemp = new EquationContainer[value.Length][];
            //
            for (int i = 0; i < value.Length; i++)
            {
                _stressStrainTemp[i] = new EquationContainer[3];
                _stressStrainTemp[i][0] = new EquationContainer(typeof(StringPressureConverter), value[i][0]);
                _stressStrainTemp[i][1] = new EquationContainer(typeof(StringDoubleConverter), value[i][1]);
                _stressStrainTemp[i][2] = new EquationContainer(typeof(StringTemperatureConverter), value[i][2]);
            }
            SetStressStrainTemp(_stressStrainTemp, checkEquation);
        }
        private void SetStressStrainTemp(EquationContainer[][] value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _stressStrainTemp, value, null, checkEquation);
        }
        // IContainsEquations
        public override void CheckEquations()
        {
            for (int i = 0; i < _stressStrainTemp.Length; i++)
            {
                _stressStrainTemp[i][0].CheckEquation();
                _stressStrainTemp[i][1].CheckEquation();
            }
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_stressStrainTemp", _stressStrainTemp, typeof(EquationContainer[][]));
            info.AddValue("_hardening", _hardening, typeof(PlasticHardening));
        }
    }
}
