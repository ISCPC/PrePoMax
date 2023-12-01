using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.Runtime.Serialization;
using System.Xml.Linq;


namespace CaeModel
{
    [Serializable]
    public class PointSpringData
    {
        // Variables                                                                                                                
        private string _name;
        private string _regionName;
        private RegionTypeEnum _regionType;
        private int _nodeId;
        private double _k1;
        private double _k2;
        private double _k3;


        // Properties                                                                                                               
        public string Name { get { return _name; } set { _name = value; } }
        public string RegionName { get { return _regionName; } set { _regionName = value; } }
        public RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        //
        public int NodeId { get { return _nodeId; } set { _nodeId = value; } }
        public double K1 { get { return _k1; } set { _k1 = value; } }
        public double K2 { get { return _k2; } set { _k2 = value; } }
        public double K3 { get { return _k3; } set { _k3 = value; } }


        // Constructors                                                                                                             
        public PointSpringData(PointSpring pointSpring)
           : this(pointSpring.Name, -1, pointSpring.K1.Value, pointSpring.K2.Value, pointSpring.K3.Value)
        {
            _regionName = pointSpring.RegionName;
            _regionType = pointSpring.RegionType;
        }
        public PointSpringData(string name, int nodeId, double k1, double k2, double k3)
        {
            _name = name;
            _regionName = "";
            _regionType = RegionTypeEnum.NodeId;
            //
            _nodeId = nodeId;
            _k1 = k1;
            _k2 = k2;
            _k3 = k3;
        }


        // Methods                                                                                                                  
        public int[] GetSpringDirections()
        {
            List<int> directions = new List<int>();
            if (_k1 != 0) directions.Add(1);
            if (_k2 != 0) directions.Add(2);
            if (_k3 != 0) directions.Add(3);
            return directions.ToArray();
        }
        public double[] GetSpringStiffnessValues()
        {
            List<double> values = new List<double>();
            if (_k1 != 0) values.Add(_k1);
            if (_k2 != 0) values.Add(_k2);
            if (_k3 != 0) values.Add(_k3);
            return values.ToArray();
        }
    }
}

