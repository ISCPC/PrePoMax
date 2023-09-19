using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.Drawing;
using System.Runtime.Serialization;

namespace CaeModel
{
    [Serializable]
    public class BoundaryCondition : NamedClass, IMultiRegion, IContainsEquations, ISerializable
    {
        // Variables                                                                                                                
        private string _regionName;                             //ISerializable
        private RegionTypeEnum _regionType;                     //ISerializable
        private int[] _creationIds;                             //ISerializable
        private Selection _creationData;                        //ISerializable
        protected bool _twoD;                                   //ISerializable
        protected string _amplitudeName;                        //ISerializable
        protected bool _complex;                                //ISerializable
        protected EquationContainer _phaseDeg;                  //ISerializable
        protected Color _color;                                 //ISerializable
        public const string DefaultAmplitudeName = "Default";


        // Properties                                                                                                               
        public string RegionName { get { return _regionName; } set { _regionName = value; } }
        public RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public int[] CreationIds { get { return _creationIds; } set { _creationIds = value; } }
        public Selection CreationData { get { return _creationData; } set { _creationData = value; } }
        public bool TwoD { get { return _twoD; } }
        public string AmplitudeName
        {
            get
            {
                if (_amplitudeName == null) return DefaultAmplitudeName;
                else return _amplitudeName;
            }
            set
            {
                _amplitudeName = value;
                if (_amplitudeName == DefaultAmplitudeName) _amplitudeName = null;
            }
        }
        public bool Complex { get { return _complex; } set { _complex = value; } }
        public EquationContainer PhaseDeg { get { return _phaseDeg; } set { SetPhaseDeg(value); } }
        public Color Color
        {
            get
            {
                // Compatibility for version v0.6.0
                if (_color == Color.Empty) _color = Color.Lime;
                //
                return _color;
            }
            set { _color = value; }
        }


        // Constructors                                                                                                             
        public BoundaryCondition(string name, string regionName, RegionTypeEnum regionType, bool twoD,
                                 bool complex, double phaseDeg)
            : base(name) 
        {
            _regionName = regionName;
            _regionType = regionType;
            _creationIds = null;
            _creationData = null;
            _twoD = twoD;
            _amplitudeName = null;
            _complex = complex;
            PhaseDeg = new EquationContainer(typeof(StringAngleDegConverter), phaseDeg);
            _color = Color.Lime;
        }
        public BoundaryCondition(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_regionName":
                    case "BoundaryCondition+_regionName":               // Compatibility for version v1.4.0
                        _regionName = (string)entry.Value; break;
                    case "_regionType":
                    case "BoundaryCondition+_regionType":               // Compatibility for version v1.4.0
                        _regionType = (RegionTypeEnum)entry.Value; break;
                    case "_creationIds":
                    case "BoundaryCondition+_creationIds":              // Compatibility for version v1.4.0
                        _creationIds = (int[])entry.Value; break;
                    case "_creationData":
                    case "BoundaryCondition+_creationData":             // Compatibility for version v1.4.0
                        _creationData = (Selection)entry.Value; break;
                    case "_twoD":
                    case "BoundaryCondition+_twoD":                     // Compatibility for version v1.4.0
                        _twoD = (bool)entry.Value; break;
                    case "_amplitudeName":
                    case "BoundaryCondition+_amplitudeName":            // Compatibility for version v1.4.0
                        _amplitudeName = (string)entry.Value; break;
                    case "_complex":
                    case "BoundaryCondition+_complex":                  // Compatibility for version v1.4.0
                        _complex = (bool)entry.Value; break;
                    case "_phaseDeg":
                    case "BoundaryCondition+_phaseDeg":                 // Compatibility for version v1.4.0
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valuePhase)
                            PhaseDeg = new EquationContainer(typeof(StringAngleDegConverter), valuePhase);
                        else
                            SetPhaseDeg((EquationContainer)entry.Value, false);
                        break;
                    case "_color":
                    case "BoundaryCondition+_color":                    // Compatibility for version v1.4.0
                        _color = (Color)entry.Value; break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        private void SetPhaseDeg(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _phaseDeg, value, CheckAngle, checkEquation);
        }
        //
        private double CheckAngle(double value)
        {
            return Tools.GetPhase360(value);
        }
        // IContainsEquations
        public virtual void CheckEquations()
        {
            _phaseDeg.CheckEquation();
        }
        public virtual bool TryCheckEquations()
        {
            try
            {
                CheckEquations();
                return true;
            }
            catch (Exception ex) { return false; }
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_regionName", _regionName, typeof(string));
            info.AddValue("_regionType", _regionType, typeof(RegionTypeEnum));
            info.AddValue("_creationIds", _creationIds, typeof(int[]));
            info.AddValue("_creationData", _creationData, typeof(Selection));
            info.AddValue("_twoD", _twoD, typeof(bool));
            info.AddValue("_amplitudeName", _amplitudeName, typeof(string));
            info.AddValue("_complex", _complex, typeof(bool));
            info.AddValue("_phaseDeg", _phaseDeg, typeof(EquationContainer));
            info.AddValue("_color", _color, typeof(Color));
        }
    }
}
