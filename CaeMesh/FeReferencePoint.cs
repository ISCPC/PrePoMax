using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using DynamicTypeDescriptor;

namespace CaeMesh
{
    [Serializable]
    public enum FeReferencePointCreatedFrom
    {
        [StandardValue("Coordinates", DisplayName = "Coordinates")]
        Coordinates,
        [StandardValue("NodeSetCG", DisplayName = "Center of gravity")]
        NodeSetCG,
        [StandardValue("NodeSetBB", DisplayName = "Bounding box center")]
        NodeSetBB
    }

    [Serializable]
    public class FeReferencePoint : NamedClass
    {
        // Variables                                                                                                                
        private double _x;
        private double _y;
        private double _z;
        private FeReferencePointCreatedFrom _createdFrom;
        private int _createdFromRefNodeId1;
        private int _createdFromRefNodeId2;
        private string _nodeSetName;


        // Properties                                                                                                               
        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }
        public double Z { get { return _z; } set { _z = value; } }
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
        public int CreatedFromRefNodeId1 { get { return _createdFromRefNodeId1; } set { _createdFromRefNodeId1 = value; } }
        public int CreatedFromRefNodeId2 { get { return _createdFromRefNodeId2; } set { _createdFromRefNodeId2 = value; } }
        public string NodeSetName { get { return _nodeSetName; } set { _nodeSetName = value; } }


        // Constructors                                                                                                             
        public FeReferencePoint(string name, double x, double y, double z)
            :base(name)
        {
            Clear();
            _createdFrom = FeReferencePointCreatedFrom.Coordinates;
            _x = x;
            _y = y;
            _z = z;
        }
        public FeReferencePoint(string name, FeNode refNode1, int refNode2Id)
            : base(name)
        {
            Clear();
            _createdFrom = FeReferencePointCreatedFrom.Coordinates;
            _createdFromRefNodeId1 = refNode1.Id;
            _createdFromRefNodeId2 = refNode2Id;
            _x = refNode1.X;
            _y = refNode1.Y;
            _z = refNode1.Z;
        }
        public FeReferencePoint(string name, string nodeSetName, FeReferencePointCreatedFrom createdFrom)
            : base(name)
        {
            Clear();
            _nodeSetName = nodeSetName;
            _createdFrom = createdFrom;
        }


        // Methods                                                                                                                  
        private void Clear()
        {
            _x = 0;
            _y = 0;
            _z = 0;
            ClearKeepCoordinates();
        }
        private void ClearKeepCoordinates()
        {
            _createdFrom = FeReferencePointCreatedFrom.Coordinates;
            _createdFromRefNodeId1 = -1;
            _createdFromRefNodeId2 = -1;
            _nodeSetName = null;
        }
        public double[] Coor()
        {
            return new double[] { _x, _y, _z };
        }
        public void UpdateCoordinates(FeNodeSet nodeSet)
        {
            if (_createdFrom == FeReferencePointCreatedFrom.NodeSetCG)
            {
                _x = nodeSet.CenterOfGravity[0];
                _y = nodeSet.CenterOfGravity[1];
                _z = nodeSet.CenterOfGravity[2];
            }
            else if (_createdFrom == FeReferencePointCreatedFrom.NodeSetBB)
            {
                _x = (nodeSet.BoundingBox[0][0] + nodeSet.BoundingBox[0][1]) / 2;
                _y = (nodeSet.BoundingBox[1][0] + nodeSet.BoundingBox[1][1]) / 2;
                _z = (nodeSet.BoundingBox[2][0] + nodeSet.BoundingBox[2][1]) / 2;
            }
        }

    }
}
