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
    public abstract class SpringConstraint : Constraint, ISerializable
    {
        // Variables                                                                                                                
        private bool _checkPositive;        //ISerializable
        private EquationContainer _k1;      //ISerializable
        private EquationContainer _k2;      //ISerializable
        private EquationContainer _k3;      //ISerializable


        // Properties                                                                                                               
        public string RegionName { get { return MasterRegionName; } set { MasterRegionName = value; } }
        public RegionTypeEnum RegionType { get { return MasterRegionType; } set { MasterRegionType = value; } }
        //
        public int[] CreationIds { get { return MasterCreationIds; } set { MasterCreationIds = value; } }
        public Selection CreationData { get { return MasterCreationData; } set { MasterCreationData = value; } }
        //
        public EquationContainer K1 { get { return _k1; } set { SetK1(value); } }
        public EquationContainer K2 { get { return _k2; } set { SetK2(value); } }
        public EquationContainer K3 { get { return _k3; } set { SetK3(value); } }


        // Constructors                                                                                                             
        public SpringConstraint(string name, string regionName, RegionTypeEnum regionType, bool twoD, bool checkPositive)
            : this(name, regionName, regionType, 0, 0, 0, twoD, checkPositive)
        {
        }
        public SpringConstraint(string name, string regionName, RegionTypeEnum regionType, double k1, double k2, double k3,
                                bool twoD, bool checkPositive)
            : base(name, regionName, regionType, "", RegionTypeEnum.None, twoD)
        {
            _checkPositive = checkPositive;
            K1 = new EquationContainer(typeof(StringForcePerLengthConverter), k1);
            K2 = new EquationContainer(typeof(StringForcePerLengthConverter), k2);
            K3 = new EquationContainer(typeof(StringForcePerLengthConverter), k3);
        }
        public SpringConstraint(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Compatibility for version v1.5.2
            _checkPositive = false;
            //
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_checkPositive":
                        _checkPositive = (bool)entry.Value; break;
                    case "_k1":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueK1)
                            K1 = new EquationContainer(typeof(StringForcePerLengthConverter), valueK1);
                        else
                            SetK1((EquationContainer)entry.Value, false);
                        break;
                    case "_k2":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueK2)
                            K2 = new EquationContainer(typeof(StringForcePerLengthConverter), valueK2);
                        else
                            SetK2((EquationContainer)entry.Value, false);
                        break;
                    case "_k3":
                        // Compatibility for version v1.4.0
                        if (entry.Value is double valueK3)
                            K3 = new EquationContainer(typeof(StringForcePerLengthConverter), valueK3);
                        else
                            SetK3((EquationContainer)entry.Value, false);
                        break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        private void SetK1(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _k1, value, CheckPositive, checkEquation);
        }
        private void SetK2(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _k2, value, CheckPositive, checkEquation);
        }
        private void SetK3(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _k3, value, CheckPositiveAnd2D, checkEquation);
        }
        //
        protected double CheckPositive(double value)
        {
            if (_checkPositive && value < 0) return 0;
            else return value;
        }
        protected double CheckPositiveAnd2D(double value)
        {
            if ((_checkPositive && value < 0) || TwoD) return 0;
            else return value;
        }
        public int[] GetSpringDirections()
        {
            List<int> directions = new List<int>();
            if (_k1.Value != 0) directions.Add(1);
            if (_k2.Value != 0) directions.Add(2);
            if (_k3.Value != 0) directions.Add(3);
            return directions.ToArray();
        }
        public double[] GetSpringStiffnessValues()
        {
            List<double> values = new List<double>();
            if (_k1.Value != 0) values.Add(_k1.Value);
            if (_k2.Value != 0) values.Add(_k2.Value);
            if (_k3.Value != 0) values.Add(_k3.Value);
            return values.ToArray();
        }
        // IContainsEquations
        public override void CheckEquations()
        {
            base.CheckEquations();
            //
            _k1.CheckEquation();
            _k2.CheckEquation();
            _k3.CheckEquation();
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_checkPositive", _checkPositive, typeof(bool));
            info.AddValue("_k1", _k1, typeof(EquationContainer));
            info.AddValue("_k2", _k2, typeof(EquationContainer));
            info.AddValue("_k3", _k3, typeof(EquationContainer));
        }
    }
}
