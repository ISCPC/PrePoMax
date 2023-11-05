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
    public class CompressionOnly : Constraint, ISerializable
    {
        // Variables                                                                                                                
        private EquationContainer _springStiffness;                 //ISerializable
        private EquationContainer _tensileForceAtNegativeInfinity;  //ISerializable


        // Properties                                                                                                               
        public string RegionName { get { return MasterRegionName; } set { MasterRegionName = value; } }
        public RegionTypeEnum RegionType { get { return MasterRegionType; } set { MasterRegionType = value; } }
        //
        public int[] CreationIds { get { return MasterCreationIds; } set { MasterCreationIds = value; } }
        public Selection CreationData { get { return MasterCreationData; } set { MasterCreationData = value; } }
        //
        public EquationContainer SpringStiffness { get { return _springStiffness; } set { SetSpringStiffness(value); } }
        public EquationContainer TensileForceAtNegativeInfinity
        {
            get { return _tensileForceAtNegativeInfinity; }
            set { SetTensileForceAtNegativeInfinity(value); }
        }
        

        // Constructors                                                                                                             
        public CompressionOnly(string name, string regionName, RegionTypeEnum regionType, bool twoD)
            : base(name, regionName, regionType, "", RegionTypeEnum.None, twoD)
        {
            SpringStiffness = new EquationContainer(typeof(StringForcePerLengthDefaultConverter), double.NaN);
            TensileForceAtNegativeInfinity = new EquationContainer(typeof(StringForceDefaultConverter), double.NaN);
        }
        public CompressionOnly(SerializationInfo info, StreamingContext context)
           : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_springStiffness":
                        SetSpringStiffness((EquationContainer)entry.Value, false);
                        break;
                    case "_tensileForceAtNegInfinity":
                        SetTensileForceAtNegativeInfinity((EquationContainer)entry.Value, false);
                        break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        private void SetSpringStiffness(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _springStiffness, value, null, checkEquation);
        }
        private void SetTensileForceAtNegativeInfinity(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _tensileForceAtNegativeInfinity, value, null, checkEquation);
        }
        // IContainsEquations
        public override void CheckEquations()
        {
            base.CheckEquations();
            //
            _springStiffness.CheckEquation();
            _tensileForceAtNegativeInfinity.CheckEquation();
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_springStiffness", _springStiffness, typeof(EquationContainer));
            info.AddValue("_tensileForceAtNegInfinity", _tensileForceAtNegativeInfinity, typeof(EquationContainer));
        }
    }
}
