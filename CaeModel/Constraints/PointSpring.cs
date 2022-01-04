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
    public class PointSpring : Constraint, ISerializable
    {
        // Variables                                                                                                                
        protected bool _twoD;           //ISerializable
        private double _k1;             //ISerializable
        private double _k2;             //ISerializable
        private double _k3;             //ISerializable
        private double _kt1;            //ISerializable
        private double _kt2;            //ISerializable
        private double _kt3;            //ISerializable


        // Properties                                                                                                               
        public string RegionName { get { return MasterRegionName; } set { MasterRegionName = value; } }
        public RegionTypeEnum RegionType { get { return MasterRegionType; } set { MasterRegionType = value; } }
        //
        public int[] CreationIds { get { return MasterCreationIds; } set { MasterCreationIds = value; } }
        public Selection CreationData { get { return MasterCreationData; } set { MasterCreationData = value; } }
        //
        public bool TwoD { get { return _twoD; } }
        public double K1 { get { return _k1; } set { _k1 = value; if (_k1 < 0) _k1 = 0; } }
        public double K2 { get { return _k2; } set { _k2 = value; if (_k2 < 0) _k2 = 0; } }
        public double K3 { get { return _k3; } set { _k3 = value; if (_k3 < 0 || _twoD) _k3 = 0; } }
        public double KT1 { get { return _kt1; } set { _kt1 = value; if (_kt1 < 0 || _twoD) _kt1 = 0; } }
        public double KT2 { get { return _kt2; } set { _kt2 = value; if (_kt2 < 0 || _twoD) _kt2 = 0; } }
        public double KT3 { get { return _kt3; } set { _kt3 = value; if (_kt3 < 0) _kt3 = 0; } }


        // Constructors                                                                                                             
        public PointSpring(string name, string regionName, RegionTypeEnum regionType, bool twoD)
            : base(name, regionName, regionType, "", RegionTypeEnum.None)
        {
            _twoD = twoD;
            K1 = 0;
            K2 = 0;
            K3 = 0;
            KT1 = 0;
            KT2 = 0;
            KT3 = 0;
        }
        public PointSpring(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_twoD":
                        _twoD = (bool)entry.Value; break;
                    case "_k1":
                        _k1 = (double)entry.Value; break;
                    case "_k2":
                        _k2 = (double)entry.Value; break;
                    case "_k3":
                        _k3 = (double)entry.Value; break;
                    case "_kt1":
                        _kt1 = (double)entry.Value; break;
                    case "_kt2":
                        _kt2 = (double)entry.Value; break;
                    case "_kt3":
                        _kt3 = (double)entry.Value; break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        public int[] GetConstrainedDirections()
        {
            List<int> directions = new List<int>();
            if (_k1 > 0) directions.Add(1);
            if (_k2 > 0) directions.Add(2);
            if (_k3 > 0) directions.Add(3);
            if (_kt1 > 0) directions.Add(4);
            if (_kt2 > 0) directions.Add(5);
            if (_kt3 > 0) directions.Add(6);
            return directions.ToArray();
        }
        public double[] GetConstrainValues()
        {
            List<double> values = new List<double>();
            if (_k1 > 0) values.Add(_k1);
            if (_k2 > 0) values.Add(_k2);
            if (_k3 > 0) values.Add(_k3);
            if (_kt1 > 0) values.Add(_kt1);
            if (_kt2 > 0) values.Add(_kt2);
            if (_kt3 > 0) values.Add(_kt3);
            return values.ToArray();
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_twoD", _twoD, typeof(bool));
            info.AddValue("_k1", _k1, typeof(double));
            info.AddValue("_k2", _k2, typeof(double));
            info.AddValue("_k3", _k3, typeof(double));
            info.AddValue("_kt1", _kt1, typeof(double));
            info.AddValue("_kt2", _kt2, typeof(double));
            info.AddValue("_kt3", _kt3, typeof(double));
        }
    }
}
