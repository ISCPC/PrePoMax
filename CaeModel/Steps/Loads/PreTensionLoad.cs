using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using DynamicTypeDescriptor;
using CaeGlobals;
using System.Runtime.Serialization;

namespace CaeModel
{
    [Serializable]
    public enum PreTensionLoadType
    {
        [StandardValue("Force", Description = "Pre-tension by force.")]
        Force = 0,
        [StandardValue("Displacement", Description = "Pre-tension by displacement.")]
        Displacement = 1
    }
    //
    [Serializable]
    public class PreTensionLoad : Load, ISerializable
    {
        // Variables                                                                                                                
        private string _surfaceName;                    //ISerializable
        private RegionTypeEnum _regionType;             //ISerializable
        private bool _autoComputeDirection;             //ISerializable
        private EquationContainer _x;                   //ISerializable
        private EquationContainer _y;                   //ISerializable
        private EquationContainer _z;                   //ISerializable
        private PreTensionLoadType _type;               //ISerializable
        private EquationContainer _forceMagnitude;      //ISerializable
        private EquationContainer _dispMagnitude;       //ISerializable


        // Properties                                                                                                               
        public override string RegionName { get { return _surfaceName; } set { _surfaceName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public string SurfaceName { get { return _surfaceName; } set { _surfaceName = value; } }
        public bool AutoComputeDirection { get { return _autoComputeDirection; } set { _autoComputeDirection = value; } }
        public EquationContainer X { get { return _x; } set { SetX(value); } }
        public EquationContainer Y { get { return _y; } set { SetY(value); } }
        public EquationContainer Z { get { return _z; } set { SetZ(value); } }
        public PreTensionLoadType Type { get { return _type; } set { _type = value; } }
        public EquationContainer ForceMagnitude
        {
            get { return _forceMagnitude; }
            set { SetForceMagnitude(value); }
        }
        public EquationContainer DisplacementMagnitude
        {
            get { return _dispMagnitude; }
            set { SetDisplacementMagnitude(value); }
        }
        

        // Constructors                                                                                                             
        public PreTensionLoad(string name, string surfaceName, RegionTypeEnum regionType, double magnitude, bool twoD)
            : this(name, surfaceName, regionType, 0, 0, 0, magnitude, twoD)
        {
            _autoComputeDirection = true;
        }
        public PreTensionLoad(string name, string surfaceName, RegionTypeEnum regionType,
                              double x, double y, double z, double magnitude, bool twoD)
            : base(name, twoD) 
        {
            _surfaceName = surfaceName;
            _regionType = regionType;
            _autoComputeDirection = false;
            X = new EquationContainer(typeof(StringLengthConverter), x);
            Y = new EquationContainer(typeof(StringLengthConverter), y);
            Z = new EquationContainer(typeof(StringLengthConverter), z);
            ForceMagnitude = new EquationContainer(typeof(StringForceConverter), magnitude);
            DisplacementMagnitude = new EquationContainer(typeof(StringLengthFixedDOFConverter), magnitude);
        }
        public PreTensionLoad(SerializationInfo info, StreamingContext context)
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
                    case "_autoComputeDirection":
                        _autoComputeDirection = (bool)entry.Value; break;
                    case "_x":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueX)
                            X = new EquationContainer(typeof(StringLengthConverter), valueX);
                        else
                            SetX((EquationContainer)entry.Value, false);
                        break;
                    case "_y":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueY)
                            Y = new EquationContainer(typeof(StringLengthConverter), valueY);
                        else
                            SetY((EquationContainer)entry.Value, false);
                        break;
                    case "_z":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueZ)
                            Z = new EquationContainer(typeof(StringLengthConverter), valueZ);
                        else
                            SetZ((EquationContainer)entry.Value, false);
                        break;
                    case "_type":
                        _type = (PreTensionLoadType)entry.Value; break;
                    case "_magnitude":
                        // Compatibility for version v1.4.0
                        double valueMag = (double)entry.Value;
                        ForceMagnitude = new EquationContainer(typeof(StringForceConverter), valueMag);
                        DisplacementMagnitude = new EquationContainer(typeof(StringLengthFixedDOFConverter), valueMag);
                        break;
                    case "_forceMagnitude":
                        SetForceMagnitude((EquationContainer)entry.Value, false); break;
                    case "_dispMagnitude":
                        SetDisplacementMagnitude((EquationContainer)entry.Value, false); break;
                    default:
                        break;
                }
            }
        }

        // Methods                                                                                                                  
        private void SetX(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _x, value, null, checkEquation);
        }
        private void SetY(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _y, value, null, checkEquation);
        }
        private void SetZ(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _z, value, Check2D, checkEquation);
        }
        private void SetForceMagnitude(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _forceMagnitude, value, null, checkEquation);
        }
        private void SetDisplacementMagnitude(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _dispMagnitude, value, null, checkEquation);
        }
        //
        private double Check2D(double value)
        {
            if (_twoD) return 0;
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
            _forceMagnitude.CheckEquation();
            _dispMagnitude.CheckEquation();
        }
        //
        public void SetMagnitudeValue(double value)
        {
            if (_type == PreTensionLoadType.Force) _forceMagnitude.SetEquationFromValue(value);
            else if (_type == PreTensionLoadType.Displacement) _dispMagnitude.SetEquationFromValue(value);
            else throw new NotSupportedException();
        }
        public double GetMagnitudeValue()
        {
            if (_type == PreTensionLoadType.Force) return _forceMagnitude.Value;
            else if (_type == PreTensionLoadType.Displacement) return _dispMagnitude.Value;
            else throw new NotSupportedException();
        }

        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_surfaceName", _surfaceName, typeof(string));
            info.AddValue("_regionType", _regionType, typeof(RegionTypeEnum));
            info.AddValue("_autoComputeDirection", _autoComputeDirection, typeof(bool));
            info.AddValue("_x", _x, typeof(EquationContainer));
            info.AddValue("_y", _y, typeof(EquationContainer));
            info.AddValue("_z", _z, typeof(EquationContainer));
            info.AddValue("_type", _type, typeof(PreTensionLoadType));
            info.AddValue("_forceMagnitude", _forceMagnitude, typeof(EquationContainer));
            info.AddValue("_dispMagnitude", _dispMagnitude, typeof(EquationContainer));
        }
    }
}
