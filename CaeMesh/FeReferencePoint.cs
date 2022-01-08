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
    public class FeReferencePoint : NamedClass, IMultiRegion, ISerializable
    {
        // Variables                                                                                                                
        public const string RefName = "_ref_";
        public const string RotName = "_rot_";
        private double _x;                                      //ISerializable
        private double _y;                                      //ISerializable
        private double _z;                                      //ISerializable
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
        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }
        public double Z
        {
            get { return _z; }
            set
            {
                _z = value;
                if (_twoD) _z = 0;
            }
        }
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
            :this(name, x, y, 0)
        {
            _twoD = true;
        }
        public FeReferencePoint(string name, double x, double y, double z)
            :base(name)
        {
            Clear();
            _createdFrom = FeReferencePointCreatedFrom.Selection;
            _x = x;
            _y = y;
            _z = z;
            _twoD = false;
        }
        public FeReferencePoint(string name, FeNode refNode1, int refNode2Id)
            : base(name)
        {
            Clear();
            _createdFrom = FeReferencePointCreatedFrom.Selection;
            _createdFromRefNodeId1 = refNode1.Id;
            _createdFromRefNodeId2 = refNode2Id;
            _x = refNode1.X;
            _y = refNode1.Y;
            _z = refNode1.Z;
            _twoD = false;
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
                        _x = (double)entry.Value; break;
                    case "_y":
                        _y = (double)entry.Value; break;
                    case "_z":
                        _z = (double)entry.Value; break;
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
            Z = _z; // is it 2D
        }


        // Methods                                                                                                                  
        private void Clear()
        {
            _x = 0;
            _y = 0;
            _z = 0;
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
            return new double[] { _x, _y, _z };
        }
        public void UpdateCoordinates(double[] centerOfGravity)
        {
            _x = centerOfGravity[0];
            _y = centerOfGravity[1];
            Z = centerOfGravity[2]; // is it 2D
        }
        public void UpdateCoordinates(double[][] boundingBox)
        {
            _x = (boundingBox[0][0] + boundingBox[0][1]) / 2;
            _y = (boundingBox[1][0] + boundingBox[1][1]) / 2;
            Z = (boundingBox[2][0] + boundingBox[2][1]) / 2; // is it 2D
        }

        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_x", _x, typeof(double));
            info.AddValue("_y", _y, typeof(double));
            info.AddValue("_z", _z, typeof(double));
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
