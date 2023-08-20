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
    public class MomentLoad : Load, ISerializable
    {
        // Variables                                                                                                                
        private string _regionName;                 //ISerializable
        private RegionTypeEnum _regionType;         //ISerializable
        private int _nodeId;                        //ISerializable
        private EquationContainer _m1;              //ISerializable
        private EquationContainer _m2;              //ISerializable
        private EquationContainer _m3;              //ISerializable
        private EquationContainer _magnitude;       //ISerializable


        // Properties                                                                                                               
        public override string RegionName { get { return _regionName; } set { _regionName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public int NodeId { get { return _nodeId; } set { _nodeId = value; } }
        public EquationContainer M1
        {
            get { return _m1; }
            set
            {
                string prevEquation = _m1 != null ? _m1.Equation : value.Equation;
                _m1 = value;
                _m1.CheckValue = Check2D;
                _m1.EquationChanged = M1EquationChanged;
                //
                _m1.CheckEquation();
                if (prevEquation != _m1.Equation) M1EquationChanged();
            }
        }
        public EquationContainer M2
        {
            get { return _m2; }
            set
            {
                string prevEquation = _m2 != null ? _m2.Equation : value.Equation;
                _m2 = value;
                _m2.CheckValue = Check2D;
                _m2.EquationChanged = M2EquationChanged;
                //
                _m2.CheckEquation();
                if (prevEquation != _m2.Equation) M2EquationChanged();
            }
        }
        public EquationContainer M3
        {
            get { return _m3; }
            set
            {
                string prevEquation = _m3 != null ? _m3.Equation : value.Equation;
                _m3 = value;
                _m3.EquationChanged = M3EquationChanged;
                //
                if (prevEquation != _m3.Equation) M3EquationChanged();
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
        public double GetDirection(int direction)
        {
            if (direction == 0) return M1.Value;
            else if (direction == 1) return M2.Value;
            else if (direction == 2) return M3.Value;
            else throw new NotSupportedException();
        }


        // Constructors                                                                                                             
        public MomentLoad(string name, int nodeId, double m1, double m2, double m3, bool twoD, bool complex, double phaseDeg)
            : this(name, null, RegionTypeEnum.NodeId, m1, m2, m3, twoD, complex, phaseDeg)
        {
            _nodeId = nodeId;   // set the nodeId
        }
        public MomentLoad(string name, string regionName, RegionTypeEnum regionType, double m1, double m2, double m3,
                          bool twoD, bool complex, double phaseDeg)
            : base(name, twoD, complex, phaseDeg) 
        {
            _regionName = regionName;
            RegionType = regionType;
            _nodeId = -1;
            //
            double mag = Math.Sqrt(m1 * m1 + m2 * m2 + m3 * m3);
            M1 = new EquationContainer(typeof(StringMomentConverter), m1);
            M2 = new EquationContainer(typeof(StringMomentConverter), m2);
            M3 = new EquationContainer(typeof(StringMomentConverter), m3);
            Magnitude = new EquationContainer(typeof(StringMomentConverter), mag);
        }
        
        public MomentLoad(SerializationInfo info, StreamingContext context)
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
                    case "_m1":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueM1)
                            M1 = new EquationContainer(typeof(StringMomentConverter), valueM1);
                        else
                            M1 = (EquationContainer)entry.Value;
                        break;
                    case "_m2":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueM2)
                            M2 = new EquationContainer(typeof(StringMomentConverter), valueM2);
                        else
                            M2 = (EquationContainer)entry.Value;
                        break;
                    case "_m3":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueM3)
                            M3 = new EquationContainer(typeof(StringMomentConverter), valueM3);
                        else
                            M3 = (EquationContainer)entry.Value;
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
                double mag = Math.Sqrt(_m1.Value * _m1.Value + _m2.Value * _m2.Value + _m3.Value * _m3.Value);
                Magnitude = new EquationContainer(typeof(StringMomentConverter), mag);
            }
        }


        // Methods                                                                                                                  
        private void M1EquationChanged()
        {
            double mag = Math.Sqrt(_m1.Value * _m1.Value + _m2.Value * _m2.Value + _m3.Value * _m3.Value);
            _magnitude.SetEquationFromValue(mag, false);
        }
        private void M2EquationChanged()
        {
            double mag = Math.Sqrt(_m1.Value * _m1.Value + _m2.Value * _m2.Value + _m3.Value * _m3.Value);
            _magnitude.SetEquationFromValue(mag, false);
        }
        private void M3EquationChanged()
        {
            double mag = Math.Sqrt(_m1.Value * _m1.Value + _m2.Value * _m2.Value + _m3.Value * _m3.Value);
            _magnitude.SetEquationFromValue(mag, false);
        }
        private void MagnitudeEquationChanged()
        {
            double mag = Math.Sqrt(_m1.Value * _m1.Value + _m2.Value * _m2.Value + _m3.Value * _m3.Value);
            double r;
            if (mag == 0) r = 0;
            else r = _magnitude.Value / mag;
            _m1.SetEquationFromValue(_m1.Value * r, false);
            _m2.SetEquationFromValue(_m2.Value * r, false);
            _m3.SetEquationFromValue(_m3.Value * r, false);
        }
        private double Check2D(double value)
        {
            if (_twoD) return 0;
            else return value;
        }
        private double CheckMagnitude(double value)
        {
            if (value < 0) throw new Exception("Value of the moment load magnitude must be non-negative.");
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
            info.AddValue("_m1", _m1, typeof(EquationContainer));
            info.AddValue("_m2", _m2, typeof(EquationContainer));
            info.AddValue("_m3", _m3, typeof(EquationContainer));
            info.AddValue("_magnitude", _magnitude, typeof(EquationContainer));
        }
    }
}
