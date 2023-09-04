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
    public class STLoad : Load, ISerializable
    {
        // Variables                                                                                                                
        private string _surfaceName;            //ISerializable
        private RegionTypeEnum _regionType;     //ISerializable
        private EquationContainer _f1;          //ISerializable
        private EquationContainer _f2;          //ISerializable
        private EquationContainer _f3;          //ISerializable
        private EquationContainer _magnitude;   //ISerializable


        // Properties                                                                                                               
        public override string RegionName { get { return _surfaceName; } set { _surfaceName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public string SurfaceName { get { return _surfaceName; } set { _surfaceName = value; } }
        public EquationContainer F1 { get { UpdateEquations(); return _f1; } set { SetF1(value); } }
        public EquationContainer F2 { get { UpdateEquations(); return _f2; } set { SetF2(value); } }
        public EquationContainer F3 { get { UpdateEquations(); return _f3; } set { SetF3(value); } }
        public EquationContainer Magnitude { get { UpdateEquations(); return _magnitude; } set { SetMagnitude(value); } }


        // Constructors                                                                                                             
        public STLoad(string name, string surfaceName, RegionTypeEnum regionType, double f1, double f2, double f3, bool twoD,
                      bool complex, double phaseDeg)
            : base(name, twoD, complex, phaseDeg)
        {
            _surfaceName = surfaceName;
            _regionType = regionType;
            //
            double mag = Math.Sqrt(f1 * f1 + f2 * f2 + f3 * f3);
            F1 = new EquationContainer(typeof(StringForceConverter), f1);
            F2 = new EquationContainer(typeof(StringForceConverter), f2);
            F3 = new EquationContainer(typeof(StringForceConverter), f3);
            Magnitude = new EquationContainer(typeof(StringForceConverter), mag);
        }
        public STLoad(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_surfaceName":
                        _surfaceName = (string)entry.Value; break;
                    case "_regionType":
                        _regionType = (RegionTypeEnum)entry.Value; break;
                    case "_f1":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueF1)
                            F1 = new EquationContainer(typeof(StringForceConverter), valueF1);
                        else
                            SetF1((EquationContainer)entry.Value, false);
                        break;
                    case "_f2":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueF2)
                            F2 = new EquationContainer(typeof(StringForceConverter), valueF2);
                        else
                            SetF2((EquationContainer)entry.Value, false);
                        break;
                    case "_f3":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueF3)
                            F3 = new EquationContainer(typeof(StringForceConverter), valueF3);
                        else
                            SetF3((EquationContainer)entry.Value, false);
                        break;
                    case "_magnitude":
                        SetMagnitude((EquationContainer)entry.Value, false); break;
                    default:
                        break;
                }
            }
            // Compatibility for version v1.4.0
            if (_magnitude == null)
            {
                double mag = Math.Sqrt(_f1.Value * _f1.Value + _f2.Value * _f2.Value + _f3.Value * _f3.Value);
                Magnitude = new EquationContainer(typeof(StringForceConverter), mag);
            }
        }


        // Methods                                                                                                                  
        private void UpdateEquations()
        {
            try
            {
                // If error catch it silently
                if (_f1.IsEquation() || _f2.IsEquation() || _f3.IsEquation()) FEquationChanged();
                else if (_magnitude.IsEquation()) MagnitudeEquationChanged();
            }
            catch (Exception ex) { }
        }
        private void SetF1(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _f1, value, null, FEquationChanged, checkEquation);
        }
        private void SetF2(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _f2, value, null, FEquationChanged, checkEquation);
        }
        private void SetF3(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _f3, value, Check2D, FEquationChanged, checkEquation);
        }
        private void SetMagnitude(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _magnitude, value, CheckMagnitude, MagnitudeEquationChanged, checkEquation);
        }
        //
        private void FEquationChanged()
        {
            double mag = Math.Sqrt(_f1.Value * _f1.Value + _f2.Value * _f2.Value + _f3.Value * _f3.Value);
            _magnitude.SetEquationFromValue(mag, false);
        }
        private void MagnitudeEquationChanged()
        {
            double mag = Math.Sqrt(_f1.Value * _f1.Value + _f2.Value * _f2.Value + _f3.Value * _f3.Value);
            double r;
            if (mag == 0) r = 0;
            else r = _magnitude.Value / mag;
            _f1.SetEquationFromValue(_f1.Value * r, false);
            _f2.SetEquationFromValue(_f2.Value * r, false);
            _f3.SetEquationFromValue(_f3.Value * r, false);
        }
        //
        private double Check2D(double value)
        {
            if (_twoD) return 0;
            else return value;
        }
        private double CheckMagnitude(double value)
        {
            if (value < 0) throw new Exception("Value of the force load magnitude must be non-negative.");
            else return value;
        }
        // IContainsEquations
        public override void CheckEquations()
        {
            base.CheckEquations();
            //
            _f1.CheckEquation();
            _f2.CheckEquation();
            _f3.CheckEquation();
            _magnitude.CheckEquation();
        }

        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_surfaceName", _surfaceName, typeof(string));
            info.AddValue("_regionType", _regionType, typeof(RegionTypeEnum));
            info.AddValue("_f1", _f1, typeof(EquationContainer));
            info.AddValue("_f2", _f2, typeof(EquationContainer));
            info.AddValue("_f3", _f3, typeof(EquationContainer));
            info.AddValue("_magnitude", _magnitude, typeof(EquationContainer));
        }
    }
}
