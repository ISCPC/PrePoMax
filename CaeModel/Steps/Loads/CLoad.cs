using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.Runtime.Serialization;
using FileInOut.Output.Calculix;
using System.Drawing.Design;

namespace CaeModel
{
    [Serializable]
    public class CLoad : Load, ISerializable
    {
        // Variables                                                                                                                
        private string _regionName;             //ISerializable
        private RegionTypeEnum _regionType;     //ISerializable
        private int _nodeId;                    //ISerializable
        private EquationContainer _f1;          //ISerializable
        private EquationContainer _f2;          //ISerializable
        private EquationContainer _f3;          //ISerializable
        private EquationContainer _magnitude;   //ISerializable


        // Properties                                                                                                               
        public override string RegionName { get { return _regionName; } set { _regionName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public int NodeId { get { return _nodeId; } set { _nodeId = value; } }
        public EquationContainer F1
        {
            get { return _f1; }
            set
            {
                string prevEquation = _f1 != null ? _f1.Equation : value.Equation;
                _f1 = value;
                _f1.EquationChanged = F1EquationChanged;
                //
                if (prevEquation != _f1.Equation) F1EquationChanged();
            }
        }
        public EquationContainer F2
        {
            get { return _f2; }
            set
            {
                string prevEquation = _f2 != null ? _f2.Equation : value.Equation;
                _f2 = value;
                _f2.EquationChanged = F2EquationChanged;
                //
                if (prevEquation != _f2.Equation) F2EquationChanged();
            }
        }
        public EquationContainer F3
        {
            get { return _f3; }
            set
            {
                string prevEquation = _f3 != null ? _f3.Equation : value.Equation;
                _f3 = value;
                _f3.CheckValue = Check2D;
                _f3.EquationChanged = F3EquationChanged;
                //
                _f3.CheckEquation();
                if (prevEquation != _f3.Equation) F3EquationChanged();
            }
        }
        public EquationContainer Magnitude
        {
            get { return _magnitude; }
            set
            {
                string prevEquation = _magnitude != null ? _magnitude.Equation : value.Equation;
                _magnitude = value;
                _magnitude.CheckValue = CheckMagnitude;
                _magnitude.EquationChanged = MagnitudeEquationChanged;
                //
                _magnitude.CheckEquation();
                if (prevEquation != _magnitude.Equation) MagnitudeEquationChanged();
            }
        }
        public double GetComponent(int direction)
        {
            if (direction == 0) return F1.Value;
            else if (direction == 1) return F2.Value;
            else return F3.Value;
        }


        // Constructors                                                                                                             
        public CLoad(string name, int nodeId, double f1, double f2, double f3, bool twoD, bool complex, double phaseDeg)
            : this(name, null, RegionTypeEnum.NodeId, f1, f2, f3, twoD, complex, phaseDeg)
        {
            _nodeId = nodeId;   // set the nodeId
        }
        public CLoad(string name, string regionName, RegionTypeEnum regionType, double f1, double f2, double f3,
                     bool twoD, bool complex, double phaseDeg)
            : base(name, twoD, complex, phaseDeg)
        {
            _regionName = regionName;
            RegionType = regionType;
            _nodeId = -1;
            //
            double mag = Math.Sqrt(f1 * f1 + f2 * f2 + f3 * f3);
            F1 = new EquationContainer(typeof(StringForceConverter), f1);
            F2 = new EquationContainer(typeof(StringForceConverter), f2);
            F3 = new EquationContainer(typeof(StringForceConverter), f3);
            Magnitude = new EquationContainer(typeof(StringForceConverter), mag);
        }
        public CLoad(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_regionName":
                        _regionName = (string)entry.Value; break;
                    case "_regionType":
                        _regionType = (RegionTypeEnum)entry.Value; break;
                    case "_nodeId":
                        _nodeId = (int)entry.Value; break;
                    case "_f1":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueF1)
                            F1 = new EquationContainer(typeof(StringForceConverter), valueF1);
                        else
                            F1 = (EquationContainer)entry.Value;
                        break;
                    case "_f2":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueF2)
                            F2 = new EquationContainer(typeof(StringForceConverter), valueF2);
                        else
                            F2 = (EquationContainer)entry.Value;
                        break;
                    case "_f3":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueF3)
                            F3 = new EquationContainer(typeof(StringForceConverter), valueF3);
                        else
                            F3 = (EquationContainer)entry.Value;
                        break;
                    case "_magnitude":
                        Magnitude = (EquationContainer)entry.Value; break;
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
        private void F1EquationChanged()
        {
            double mag = Math.Sqrt(_f1.Value * _f1.Value + _f2.Value * _f2.Value + _f3.Value * _f3.Value);
            _magnitude.SetEquationFromValue(mag, false);
        }
        private void F2EquationChanged()
        {
            double mag = Math.Sqrt(_f1.Value * _f1.Value + _f2.Value * _f2.Value + _f3.Value * _f3.Value);
            _magnitude.SetEquationFromValue(mag, false);
        }
        private void F3EquationChanged()
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

        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_regionName", _regionName, typeof(string));
            info.AddValue("_regionType", _regionType, typeof(RegionTypeEnum));
            info.AddValue("_nodeId", _nodeId, typeof(int));
            info.AddValue("_f1", _f1, typeof(EquationContainer));
            info.AddValue("_f2", _f2, typeof(EquationContainer));
            info.AddValue("_f3", _f3, typeof(EquationContainer));
            info.AddValue("_magnitude", _magnitude, typeof(EquationContainer));
        }
    }
}
