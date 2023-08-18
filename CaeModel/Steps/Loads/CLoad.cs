using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.Runtime.Serialization;
using FileInOut.Output.Calculix;

namespace CaeModel
{
    [Serializable]
    public class CLoad : Load, ISerializable
    {
        // Variables                                                                                                                
        private string _regionName;                 //ISerializable
        private RegionTypeEnum _regionType;         //ISerializable
        private int _nodeId;                        //ISerializable
        private DoubleValueContainer _f1;           //ISerializable
        private DoubleValueContainer _f2;           //ISerializable
        private DoubleValueContainer _f3;           //ISerializable
        private DoubleValueContainer _magnitude;    //ISerializable


        // Properties                                                                                                               
        public override string RegionName { get { return _regionName; } set { _regionName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public int NodeId { get { return _nodeId; } set { _nodeId = value; } }
        public DoubleValueContainer F1
        {
            get { return _f1; }
            set
            {
                _f1 = value;
                _f1.CheckValue = CheckF1;   // perform the check
            }
        }
        public DoubleValueContainer F2
        {
            get { return _f2; }
            set
            {
                _f2 = value;
                _f2.CheckValue = CheckF2;   // perform the check
            }
        }
        public DoubleValueContainer F3
        {
            get { return _f3; }
            set
            {
                _f3 = value;
                _f3.CheckValue = CheckF3;   // perform the check
            }
        }
        public DoubleValueContainer Magnitude
        {
            get { return _magnitude; }
            set
            {
                _magnitude = value;
                _magnitude.CheckValue = CheckMagnitude;   // perform the check
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
            _f1 = new DoubleValueContainer(typeof(StringForceConverter), f1);           // no check
            _f2 = new DoubleValueContainer(typeof(StringForceConverter), f2);           // no check
            _f3 = new DoubleValueContainer(typeof(StringForceConverter), f3);           // no check
            _magnitude = new DoubleValueContainer(typeof(StringForceConverter), mag);   // no check
            // Perform all checks after the variables are created
            F1 = _f1;
            F2 = _f2;
            F3 = _f3;
            Magnitude = _magnitude;
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
                            _f1 = new DoubleValueContainer(typeof(StringForceConverter), valueF1);
                        else
                            _f1 = (DoubleValueContainer)entry.Value;        // no check since other components might not exist
                        break;
                    case "_f2":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueF2)
                            _f2 = new DoubleValueContainer(typeof(StringForceConverter), valueF2);
                        else
                            _f2 = (DoubleValueContainer)entry.Value;        // no check since other components might not exist
                        break;
                    case "_f3":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueF3)
                            _f3 = new DoubleValueContainer(typeof(StringForceConverter), valueF3);
                        else
                            _f3 = (DoubleValueContainer)entry.Value;        // no check since other components might not exist
                        break;
                    case "_magnitude":
                        _magnitude = (DoubleValueContainer)entry.Value;     // no check since other components might not exist
                        break;
                    default:
                        break;
                }
            }
            // Compatibility for version v1.4.0
            if (_magnitude == null)
            {
                double mag = Math.Sqrt(_f1.Value * _f1.Value + _f2.Value * _f2.Value + _f3.Value * _f3.Value);
                _magnitude = new DoubleValueContainer(typeof(StringForceConverter), mag);
            }
            // Perform all checks after the variables are created
            F1 = _f1;
            F2 = _f2;
            F3 = _f3;
            Magnitude = _magnitude;
        }

        // Methods                                                                                                                  
        private double CheckF1(double value)
        {
            if (_f1.Value != value || _magnitude.Equation != null)
            {
                double mag = Math.Sqrt(value * value + _f2.Value * _f2.Value + _f3.Value * _f3.Value);
                _magnitude.SetValue(mag, false);
            }
            return value;
        }
        private double CheckF2(double value)
        {
            if (_f2.Value != value || _magnitude.Equation != null)
            {
                double mag = Math.Sqrt(_f1.Value * _f1.Value + value * value + _f3.Value * _f3.Value);
                _magnitude.SetValue(mag, false);
            }
            return value;
        }
        private double CheckF3(double value)
        {
            if (_twoD) value = 0;
            if (_f3.Value != value || _magnitude.Equation != null)
            {
                double mag = Math.Sqrt(_f1.Value * _f1.Value + _f2.Value * _f2.Value + value * value);
                _magnitude.SetValue(mag, false);
            }
            return value;
        }
        private double CheckMagnitude(double value)
        {
            if (value < 0) throw new Exception("Value of the force load magnitude must be positive.");
            else
            {
                if (_magnitude.Value != value || _f1.Equation != null || _f2.Equation != null || _f3.Equation != null)
                {
                    double mag = Math.Sqrt(_f1.Value * _f1.Value + _f2.Value * _f2.Value + _f3.Value * _f3.Value);
                    double r;
                    if (mag == 0) r = 0;
                    else r = value / mag;
                    _f1.SetValue(_f1.Value * r, false);
                    _f2.SetValue(_f2.Value * r, false);
                    _f3.SetValue(_f3.Value * r, false);
                }
            }
            return value;
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
            info.AddValue("_f1", _f1, typeof(DoubleValueContainer));
            info.AddValue("_f2", _f2, typeof(DoubleValueContainer));
            info.AddValue("_f3", _f3, typeof(DoubleValueContainer));
            info.AddValue("_magnitude", _magnitude, typeof(DoubleValueContainer));
        }
    }
}
