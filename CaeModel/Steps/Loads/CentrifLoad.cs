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
    public class CentrifLoad : Load, ISerializable
    {
        // Variables                                                                                                                
        private string _regionName;                         //ISerializable
        private RegionTypeEnum _regionType;                 //ISerializable
        private DoubleValueContainer _x;                    //ISerializable
        private DoubleValueContainer _y;                    //ISerializable
        private DoubleValueContainer _z;                    //ISerializable
        private DoubleValueContainer _n1;                   //ISerializable
        private DoubleValueContainer _n2;                   //ISerializable
        private DoubleValueContainer _n3;                   //ISerializable
        private DoubleValueContainer _rotationalSpeed;      //ISerializable
        private bool _axisymmetric;                         //ISerializable


        // Properties                                                                                                               
        public override string RegionName { get { return _regionName; } set { _regionName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public DoubleValueContainer X
        {
            get { return _x; }
            set
            {
                _x = value;
                _x.CheckValue = CheckZero;  // perform the check
            }
        }
        public DoubleValueContainer Y
        {
            get { return _y; }
            set
            {
                _y = value;
                _y.CheckValue = CheckZero;  // perform the check
            }
        }
        public DoubleValueContainer Z
        {
            get { return _z; }
            set
            {
                _z = value;
                _z.CheckValue = CheckZero;  // perform the check
            }
        }
        public DoubleValueContainer N1
        {
            get { return _n1; }
            set
            {
                _n1 = value;
                _n1.CheckValue = CheckZero; // perform the check
            }
        }
        public DoubleValueContainer N2
        {
            get { return _n2; }
            set
            {
                _n2 = value;
                _n2.CheckValue = CheckOne;  // perform the check
            }
        }
        public DoubleValueContainer N3
        {
            get { return _n3; }
            set
            {
                _n3 = value;
                _n3.CheckValue = CheckZero; // perform the check
            }
        }
        public double RotationalSpeed2 { get { return Math.Pow(_rotationalSpeed.Value, 2); } }
        public DoubleValueContainer RotationalSpeed
        {
            get
            {
                return _rotationalSpeed;
            } 
            set
            {
                _rotationalSpeed = value;
                _rotationalSpeed.CheckValue = CheckNonNegative; // perform the check
            }
        }
        public bool Axisymmetric
        {
            get { return _axisymmetric; }
            set
            {
                if (_axisymmetric != value)
                {
                    _axisymmetric = value;
                    //
                    X = _x;     // account for axisymmetric
                    Y = _y;     // account for axisymmetric
                    Z = _z;     // account for axisymmetric
                    //
                    N1 = _n1;   // account for axisymmetric
                    N2 = _n2;   // account for axisymmetric
                    N3 = _n3;   // account for axisymmetric
                }
            }
        }


        // Constructors                                                                                                             
        public CentrifLoad(string name, string regionName, RegionTypeEnum regionType,
                           bool twoD, bool axisymmetric, bool complex, double phaseDeg)
            : this(name, regionName, regionType, new double[] { 0, 0, 0 }, new double[] { 0, 0, 0 }, 0,
                   twoD, axisymmetric, complex, phaseDeg)
        {
        }
        public CentrifLoad(string name, string regionName, RegionTypeEnum regionType, double[] point, double[] normal,
                           double rotationalSpeed, bool twoD, bool axisymmetric, bool complex, double phaseDeg)
            : base(name, twoD, complex, phaseDeg)
        {
            Axisymmetric = axisymmetric;    // account for axisymmetric
            //
            _regionName = regionName;
            RegionType = regionType;
            //
            _x = new DoubleValueContainer(typeof(StringLengthConverter), point[0], CheckZero);
            _y = new DoubleValueContainer(typeof(StringLengthConverter), point[1], CheckZero);
            _z = new DoubleValueContainer(typeof(StringLengthConverter), point[2], CheckZero);
            //
            _n1 = new DoubleValueContainer(typeof(StringLengthConverter), normal[0], CheckZero);
            _n2 = new DoubleValueContainer(typeof(StringLengthConverter), normal[1], CheckOne);
            _n3 = new DoubleValueContainer(typeof(StringLengthConverter), normal[2], CheckZero);
            //
            _rotationalSpeed = new DoubleValueContainer(typeof(StringRotationalSpeedConverter), rotationalSpeed, CheckNonNegative);
        }
        public CentrifLoad(SerializationInfo info, StreamingContext context)
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
                    case "_x":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valX)
                            _x = new DoubleValueContainer(typeof(StringLengthConverter), valX, CheckZero);
                        else
                            X = (DoubleValueContainer)entry.Value;
                        break;
                    case "_y":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valY)
                            _y = new DoubleValueContainer(typeof(StringLengthConverter), valY, CheckZero);
                        else
                            Y = (DoubleValueContainer)entry.Value;
                        break;
                    case "_z":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valZ)
                            _z = new DoubleValueContainer(typeof(StringLengthConverter), valZ, CheckZero);
                        else
                            Z = (DoubleValueContainer)entry.Value;
                        break;
                    case "_n1":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valN1)
                            _n1 = new DoubleValueContainer(typeof(StringLengthConverter), valN1, CheckZero);
                        else
                            N1 = (DoubleValueContainer)entry.Value;
                        break;
                    case "_n2":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valN2)
                            _n2 = new DoubleValueContainer(typeof(StringLengthConverter), valN2, CheckOne);
                        else
                            N2 = (DoubleValueContainer)entry.Value;
                        break;
                    case "_n3":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valN3)
                            _n3 = new DoubleValueContainer(typeof(StringLengthConverter), valN3, CheckZero);
                        else
                            N3 = (DoubleValueContainer)entry.Value;
                        break;
                    case "_rotationalSpeed":
                    case "<RotationalSpeed2>k__BackingField":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valRot)
                            _rotationalSpeed = new DoubleValueContainer(typeof(StringRotationalSpeedConverter),
                                                                        Math.Sqrt(Math.Abs(valRot)), CheckNonNegative);
                        else
                            RotationalSpeed = (DoubleValueContainer)entry.Value;
                        break;
                    case "_axisymmetric":
                        _axisymmetric = (bool)entry.Value; break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        private double CheckZero(double value) 
        {
            if (_axisymmetric) return 0;
            else return value;
        }
        private double CheckOne(double value)
        {
            if (_axisymmetric) return 1;
            else return value;
        }
        private double CheckNonNegative(double value)
        {
            if (value < 0) throw new CaeException("The value of the rotational speed must be non-negative.");
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
            info.AddValue("_x", _x, typeof(DoubleValueContainer));
            info.AddValue("_y", _y, typeof(DoubleValueContainer));
            info.AddValue("_z", _z, typeof(DoubleValueContainer));
            info.AddValue("_n1", _n1, typeof(DoubleValueContainer));
            info.AddValue("_n2", _n2, typeof(DoubleValueContainer));
            info.AddValue("_n3", _n3, typeof(DoubleValueContainer));
            info.AddValue("_rotationalSpeed", _rotationalSpeed, typeof(DoubleValueContainer));
            info.AddValue("_axisymmetric", _axisymmetric, typeof(bool));
        }
    }
}
