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
    public class CompressionOnlyBC : BoundaryCondition, ISerializable
    {
        // Variables                                                                                                                
        private EquationContainer _springStiffness;
        private EquationContainer _tensileForceAtNegInfinity;


        // Properties                                                                                                               
        public EquationContainer SpringStiffness { get { return _springStiffness; } set { SetSpringStiffness(value); } }
        public EquationContainer TensileForceAtNegInfinity
        {
            get { return _tensileForceAtNegInfinity; }
            set { SetTensileForceAtNegInfinity(value); }
        }


        // Constructors                                                                                                             
        public CompressionOnlyBC(string name, string regionName, RegionTypeEnum regionType, bool twoD)
            : base(name, regionName, regionType, twoD, false, 0)
        {

            SpringStiffness = new EquationContainer(typeof(StringForcePerLengthConverter), 10E12);
            TensileForceAtNegInfinity = new EquationContainer(typeof(StringForceConverter), 10E-3);
        }
        public CompressionOnlyBC(SerializationInfo info, StreamingContext context)
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
                        SetTensileForceAtNegInfinity((EquationContainer)entry.Value, false);
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
        private void SetTensileForceAtNegInfinity(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _tensileForceAtNegInfinity, value, null, checkEquation);
        }
        // IContainsEquations
        public override void CheckEquations()
        {
            base.CheckEquations();
            //
            _springStiffness.CheckEquation();
            _tensileForceAtNegInfinity.CheckEquation();
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_springStiffness", _springStiffness, typeof(EquationContainer));
            info.AddValue("_tensileForceAtNegInfinity", _tensileForceAtNegInfinity, typeof(EquationContainer));
        }
    }
}
