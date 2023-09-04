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
        private EquationContainer _x;                       //ISerializable
        private EquationContainer _y;                       //ISerializable
        private EquationContainer _z;                       //ISerializable
        private EquationContainer _n1;                      //ISerializable
        private EquationContainer _n2;                      //ISerializable
        private EquationContainer _n3;                      //ISerializable
        private EquationContainer _rotationalSpeed;         //ISerializable
        private bool _axisymmetric;                         //ISerializable


        // Properties                                                                                                               
        public override string RegionName { get { return _regionName; } set { _regionName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public EquationContainer X { get { return _x; } set { SetX(value); } }
        public EquationContainer Y { get { return _y; } set { SetY(value); } }
        public EquationContainer Z { get { return _z; } set { SetZ(value); } }
        public EquationContainer N1 { get { return _n1; } set { SetN1(value); } }
        public EquationContainer N2 { get { return _n2; } set { SetN2(value); } }
        public EquationContainer N3 { get { return _n3; } set { SetN3(value); } }
        public double RotationalSpeed2 { get { return Math.Pow(_rotationalSpeed.Value, 2); } }
        public EquationContainer RotationalSpeed { get { return _rotationalSpeed; } set { SetRotationalSpeed(value); } }
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
            X = new EquationContainer(typeof(StringLengthConverter), point[0]);
            Y = new EquationContainer(typeof(StringLengthConverter), point[1]);
            Z = new EquationContainer(typeof(StringLengthConverter), point[2]);
            //
            N1 = new EquationContainer(typeof(StringLengthConverter), normal[0]);
            N2 = new EquationContainer(typeof(StringLengthConverter), normal[1]);
            N3 = new EquationContainer(typeof(StringLengthConverter), normal[2]);
            //
            RotationalSpeed = new EquationContainer(typeof(StringRotationalSpeedConverter), rotationalSpeed);
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
                            X = new EquationContainer(typeof(StringLengthConverter), valX);
                        else
                            SetX((EquationContainer)entry.Value, false);
                        break;
                    case "_y":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valY)
                            Y = new EquationContainer(typeof(StringLengthConverter), valY);
                        else
                            SetY((EquationContainer)entry.Value, false);
                        break;
                    case "_z":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valZ)
                            Z = new EquationContainer(typeof(StringLengthConverter), valZ);
                        else
                            SetZ((EquationContainer)entry.Value, false);
                        break;
                    case "_n1":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valN1)
                            N1 = new EquationContainer(typeof(StringLengthConverter), valN1);
                        else
                            SetN1((EquationContainer)entry.Value, false);
                        break;
                    case "_n2":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valN2)
                            N2 = new EquationContainer(typeof(StringLengthConverter), valN2);
                        else
                            SetN2((EquationContainer)entry.Value, false);
                        break;
                    case "_n3":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valN3)
                            N3 = new EquationContainer(typeof(StringLengthConverter), valN3);
                        else
                            SetN3((EquationContainer)entry.Value, false);
                        break;
                    case "_rotationalSpeed":
                    case "<RotationalSpeed2>k__BackingField":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valRot)
                            RotationalSpeed = new EquationContainer(typeof(StringRotationalSpeedConverter),
                                                                    Math.Sqrt(Math.Abs(valRot)));
                        else
                            SetRotationalSpeed((EquationContainer)entry.Value, false);
                        break;
                    case "_axisymmetric":
                        _axisymmetric = (bool)entry.Value; break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        private void SetX(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _x, value, CheckZero, checkEquation);
        }
        private void SetY(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _y, value, CheckZero, checkEquation);
        }
        private void SetZ(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _z, value, CheckZero, checkEquation);
        }
        private void SetN1(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _n1, value, CheckZero, checkEquation);
        }
        private void SetN2(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _n2, value, CheckOne, checkEquation);
        }
        private void SetN3(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _n3, value, CheckZero, checkEquation);
        }
        private void SetRotationalSpeed(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _rotationalSpeed, value, CheckNonNegative, checkEquation);
        }
        //
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
        // IContainsEquations
        public override void CheckEquations()
        {
            base.CheckEquations();
            //
            _x.CheckEquation();
            _y.CheckEquation();
            _z.CheckEquation();
            _n1.CheckEquation();
            _n2.CheckEquation();
            _n3.CheckEquation();
            _rotationalSpeed.CheckEquation();
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_regionName", _regionName, typeof(string));
            info.AddValue("_regionType", _regionType, typeof(RegionTypeEnum));
            info.AddValue("_x", _x, typeof(EquationContainer));
            info.AddValue("_y", _y, typeof(EquationContainer));
            info.AddValue("_z", _z, typeof(EquationContainer));
            info.AddValue("_n1", _n1, typeof(EquationContainer));
            info.AddValue("_n2", _n2, typeof(EquationContainer));
            info.AddValue("_n3", _n3, typeof(EquationContainer));
            info.AddValue("_rotationalSpeed", _rotationalSpeed, typeof(EquationContainer));
            info.AddValue("_axisymmetric", _axisymmetric, typeof(bool));
        }
    }
}
