using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.Runtime.Serialization;
using System.Drawing;

namespace CaeModel
{
    [Serializable]
    public abstract class Section : NamedClass, IMultiRegion, IContainsEquations, ISerializable
    {
        // Variables                                                                                                                
        private string _materialName;               //ISerializable
        private string _regionName;                 //ISerializable
        private RegionTypeEnum _regionType;         //ISerializable
        private int[] _creationIds;                 //ISerializable
        private Selection _creationData;            //ISerializable
        private EquationContainer _thickness;       //ISerializable
        private bool _twoD;                         //ISerializable


        // Properties                                                                                                               
        public string MaterialName { get { return _materialName; } set { _materialName = value; } }
        public string RegionName { get { return _regionName; } set { _regionName = value; } }
        public RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public int[] CreationIds { get { return _creationIds; } set { _creationIds = value; } }
        public Selection CreationData { get { return _creationData; } set { _creationData = value; } }
        public EquationContainer Thickness { get { return _thickness; } set { SetThickness(value); } }
        public bool TwoD { get { return _twoD; } }


        // Constructors                                                                                                             
        public Section(string name, string materialName, string regionName, RegionTypeEnum regionType, double thickness, bool twoD) 
            : base(name)
        {
            _materialName = materialName;
            _regionName = regionName;
            _regionType = regionType;
            _creationIds = null;
            _creationData = null;
            Thickness = new EquationContainer(typeof(StringLengthConverter), thickness);
            _twoD = twoD;
        }
        public Section(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_materialName":
                    case "Section+_materialName":       // Compatibility for version v1.4.0
                        _materialName = (string)entry.Value; break;
                    case "_regionName":
                    case "Section+_regionName":         // Compatibility for version v1.4.0
                        _regionName = (string)entry.Value; break;
                    case "_regionType":
                    case "Section+_regionType":         // Compatibility for version v1.4.0
                        _regionType = (RegionTypeEnum)entry.Value; break;
                    case "_creationIds":
                    case "Section+_creationIds":        // Compatibility for version v1.4.0
                        _creationIds = (int[])entry.Value; break;
                    case "_creationData":
                    case "Section+_creationData":       // Compatibility for version v1.4.0
                        _creationData = (Selection)entry.Value; break;
                    case "_thickness":
                    case "Section+_thickness":          // Compatibility for version v1.4.0
                        // Compatibility for version v1.4.0
                        if (entry.Value is double value)
                            Thickness = new EquationContainer(typeof(StringLengthConverter), value);
                        else
                            SetThickness((EquationContainer)entry.Value, false);
                        break;
                    case "_twoD":
                    case "Section+_twoD":               // Compatibility for version v1.4.0
                        _twoD = (bool)entry.Value; break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        private void SetThickness(EquationContainer value, bool checkEquation = true)
        {
            SetAndCheck(ref _thickness, value, CheckPositive, checkEquation);
        }
        protected static void SetAndCheck(ref EquationContainer variable, EquationContainer value, Func<double, double> CheckValue,
                                          Action EquationChangedCallback, bool check)
        {
            if (value == null)
            {
                variable = null;
                return;
            }
            //
            string prevEquation = variable != null ? variable.Equation : value.Equation;
            //
            value.CheckValue = CheckValue;
            value.EquationChanged = EquationChangedCallback;
            //
            if (check)
            {
                value.CheckEquation();
                if (variable != null && prevEquation != variable.Equation) EquationChangedCallback?.Invoke();
            }
            //
            variable = value;
        }
        protected static void SetAndCheck(ref EquationContainer variable, EquationContainer value, Func<double, double> CheckValue,
                                          bool check)
        {
            SetAndCheck(ref variable, value, CheckValue, null, check);
        }
        //
        private double CheckPositive(double value)
        {
            if (value <= 0) throw new Exception("Value of the section thickness must be larger than zero.");
            else return value;
        }
        // IContainsEquations
        public virtual void CheckEquations()
        {
            _thickness.CheckEquation();
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_materialName", _materialName, typeof(string));
            info.AddValue("_regionName", _regionName, typeof(string));
            info.AddValue("_regionType", _regionType, typeof(RegionTypeEnum));
            info.AddValue("_creationIds", _creationIds, typeof(int[]));
            info.AddValue("_creationData", _creationData, typeof(Selection));
            info.AddValue("_thickness", _thickness, typeof(EquationContainer));
            info.AddValue("_twoD", _twoD, typeof(bool));
        }
    }

}
