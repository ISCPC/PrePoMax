using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using DynamicTypeDescriptor;
using System.Runtime.Serialization;
using System.Drawing;

namespace CaeMesh
{
    [Serializable]
    public enum FeReferencePointCreatedFrom
    {
        [StandardValue("Selection", Description = "Selection/Coordinates", DisplayName = "Selection/Coordinates")]
        Selection,
        [StandardValue("BetweenTwoPoints", Description = "Between two points", DisplayName = "Between two points")]
        BetweenTwoPoints,
        [StandardValue("CircleCenter", Description = "Circle center by 3 points", DisplayName = "Circle center by 3 points")]
        CircleCenter,
        [StandardValue("CenterOfGravity", Description = "Center of gravity", DisplayName = "Center of gravity")]
        CenterOfGravity,
        [StandardValue("BoundingBoxCenter", Description = "Bounding box center", DisplayName = "Bounding box center")]
        BoundingBoxCenter
    }

    [Serializable]
    public class FeReferencePoint : NamedClass, IMultiRegion, ISerializable, IContainsEquations
    {
        // Variables                                                                                                                
        public const string RefName = "_ref_";
        public const string RotName = "_rot_";
        private EquationContainer _x;                           //ISerializable
        private EquationContainer _y;                           //ISerializable
        private EquationContainer _z;                           //ISerializable
        private FeReferencePointCreatedFrom _createdFrom;       //ISerializable
        private string _regionName;                 // new      //ISerializable
        private RegionTypeEnum _regionType;         // new      //ISerializable
        [NonSerialized] private int[] _creationIds;             //IMultiRegion - no used
        [NonSerialized] private Selection _creationData;        //IMultiRegion - no used
        private int _createdFromRefNodeId1;                     //ISerializable
        private int _createdFromRefNodeId2;                     //ISerializable
        private string _refNodeSetName;                         //ISerializable
        private string _rotNodeSetName;                         //ISerializable
        private bool _twoD;                                     //ISerializable
        private Color _color;                                   //ISerializable


        // Properties                                                                                                               
        public EquationContainer X { get { return _x; } set { SetX(value); } }
        public EquationContainer Y { get { return _y; } set { SetY(value); } }
        public EquationContainer Z { get { return _z; } set { SetZ(value); } }
        public FeReferencePointCreatedFrom CreatedFrom
        { 
            get { return _createdFrom; }
            set
            {
                if (_createdFrom != value)
                {
                    ClearKeepCoordinates();
                    _createdFrom = value;
                }
            }
        }
        public string RegionName { get { return _regionName; } set { _regionName = value; } }
        public RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public int[] CreationIds { get { return _creationIds; } set { _creationIds = value; } }
        public Selection CreationData { get { return _creationData; } set { _creationData = value; } }
        public int CreatedFromRefNodeId1 { get { return _createdFromRefNodeId1; } set { _createdFromRefNodeId1 = value; } }
        public int CreatedFromRefNodeId2 { get { return _createdFromRefNodeId2; } set { _createdFromRefNodeId2 = value; } }
        public string RefNodeSetName { get { return _refNodeSetName; } set { _refNodeSetName = value; } }
        public string RotNodeSetName { get { return _rotNodeSetName; } set { _rotNodeSetName = value; } }
        public bool TwoD { get { return _twoD; } }
        public Color Color { get { return _color; } set { _color = value; } }


        // Constructors                                                                                                             
        public FeReferencePoint(string name, double x, double y)
            : this(name, x, y, 0)
        {
            _twoD = true;
        }
        public FeReferencePoint(string name, FeNode refNode1, int refNode2Id)
           : this(name, refNode1.X, refNode1.Y, refNode1.Z)
        {
            _createdFromRefNodeId1 = refNode1.Id;
            _createdFromRefNodeId2 = refNode2Id;
        }
        public FeReferencePoint(string name, double x, double y, double z)
            : base(name)
        {
            Clear();
            //
            _twoD = false;
            _createdFrom = FeReferencePointCreatedFrom.Selection;
            //
            _x.SetEquationFromValue(x);
            _y.SetEquationFromValue(y);
            _z.SetEquationFromValue(z);
        }
        public FeReferencePoint(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Compatibility for version v0.6.0
            Clear();
            //
            bool version052 = false;
            string createdFromNodeSetName = null;
            // Compatibility for version v1.1.1
            _twoD = false;
            //
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
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
                    case "_createdFrom":
                        _createdFrom = (FeReferencePointCreatedFrom)entry.Value; break;
                    case "_regionName":
                        _regionName = (string)entry.Value; break;
                    case "_regionType":
                        _regionType = (RegionTypeEnum)entry.Value; break;
                    case "_createdFromRefNodeId1":
                        _createdFromRefNodeId1 = (int)entry.Value; break;                    
                    case "_createdFromRefNodeId2":
                        _createdFromRefNodeId2 = (int)entry.Value; break;
                    case "_refNodeSetName":
                        _refNodeSetName = (string)entry.Value; break;
                    case "_rotNodeSetName":
                        _rotNodeSetName = (string)entry.Value; break;
                    case "_twoD":
                        _twoD = (bool)entry.Value; break;
                    case "_color":
                        _color = (Color)entry.Value; break;
                    // Compatibility for version v0.5.2
                    case "_createdFromNodeSetName":
                        version052 = true;
                        createdFromNodeSetName = (string)entry.Value; break;
                }
            }            
            // Compatibility for version v0.5.2
            if (version052)
            {
                // This is null if reference point created from coordinates
                _regionName = createdFromNodeSetName;
                _regionType = RegionTypeEnum.NodeSetName;
            }
            //
            //Z = _z; // is it 2D
        }


        // Methods                                                                                                                  
        private void SetX(EquationContainer value, bool checkEquation = true)
        {
            SetAndCheck(ref _x, value, null, checkEquation);
        }
        private void SetY(EquationContainer value, bool checkEquation = true)
        {
            SetAndCheck(ref _y, value, null, checkEquation);
        }
        private void SetZ(EquationContainer value, bool checkEquation = true)
        {
            SetAndCheck(ref _z, value, Check2D, checkEquation);
        }
        //
        private double Check2D(double value)
        {
            if (_twoD) return 0;
            else return value;
        }
        // IContainsEquations
        public void CheckEquations()
        {
            _x.CheckEquation();
            _y.CheckEquation();
            _z.CheckEquation();
        }
        //
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
        private void Clear()
        {
            if (_x == null) _x = new EquationContainer(typeof(StringLengthConverter), 0);
            else _x.SetEquationFromValue(0);
            if (_y == null) _y = new EquationContainer(typeof(StringLengthConverter), 0);
            else _y.SetEquationFromValue(0);
            if (_z == null) _z = new EquationContainer(typeof(StringLengthConverter), 0);
            else _z.SetEquationFromValue(0);
            //
            ClearKeepCoordinates();
            _color = Color.Yellow;
        }
        private void ClearKeepCoordinates()
        {
            _createdFrom = FeReferencePointCreatedFrom.Selection;
            _createdFromRefNodeId1 = -1;
            _createdFromRefNodeId2 = -1;
            _regionName = null;
            _regionType = RegionTypeEnum.NodeSetName;
        }
        public double[] Coor()
        {
            return new double[] { _x.Value, _y.Value, _z.Value };
        }
        public void UpdateCoordinates(double[] centerOfGravity)
        {
            _x.SetEquationFromValue(centerOfGravity[0]);
            _y.SetEquationFromValue(centerOfGravity[1]);
            _z.SetEquationFromValue(centerOfGravity[2]);
        }
        public void UpdateCoordinates(double[][] boundingBox)
        {
            _x.SetEquationFromValue((boundingBox[0][0] + boundingBox[0][1]) / 2);
            _y.SetEquationFromValue((boundingBox[1][0] + boundingBox[1][1]) / 2);
            _z.SetEquationFromValue((boundingBox[2][0] + boundingBox[2][1]) / 2);
        }

        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_x", _x, typeof(EquationContainer));
            info.AddValue("_y", _y, typeof(EquationContainer));
            info.AddValue("_z", _z, typeof(EquationContainer));
            info.AddValue("_createdFrom", _createdFrom, typeof(FeReferencePointCreatedFrom));
            info.AddValue("_regionName", _regionName, typeof(string));
            info.AddValue("_regionType", _regionType, typeof(RegionTypeEnum));
            info.AddValue("_createdFromRefNodeId1", _createdFromRefNodeId1, typeof(int));
            info.AddValue("_createdFromRefNodeId2", _createdFromRefNodeId2, typeof(int));
            info.AddValue("_refNodeSetName", _refNodeSetName, typeof(string));
            info.AddValue("_rotNodeSetName", _rotNodeSetName, typeof(string));
            info.AddValue("_twoD", _twoD, typeof(bool));
            info.AddValue("_color", _color, typeof(Color));
        }
    }
}
