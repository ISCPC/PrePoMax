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
    public class SubmodelBC : BoundaryCondition, ISerializable
    {
        // Variables                                                                                                                
        private EquationContainer _stepNumber;      //ISerializable
        private bool _u1;                           //ISerializable
        private bool _u2;                           //ISerializable
        private bool _u3;                           //ISerializable
        private bool _ur1;                          //ISerializable
        private bool _ur2;                          //ISerializable
        private bool _ur3;                          //ISerializable


        // Properties                                                                                                               
        public EquationContainer StepNumber { get { return _stepNumber; } set { SetStepNumber(value); } }
        public bool U1 { get { return _u1; } set { _u1 = value; } }
        public bool U2 { get { return _u2; } set { _u2 = value; } }
        public bool U3 { get { return _u3; } set { _u3 = value; if (_twoD) _u3 = false; } }
        public bool UR1 { get { return _ur1; } set { _ur1 = value; if (_twoD) _ur1 = false; } }
        public bool UR2 { get { return _ur2; } set { _ur2 = value; if (_twoD) _ur2 = false; } }
        public bool UR3 { get { return _ur3; } set { _ur3 = value; } }


        // Constructors                                                                                                             
        public SubmodelBC(string name, string regionName, RegionTypeEnum regionType, bool twoD)
            : base(name, regionName, regionType, twoD, false, 0) 
        {
            StepNumber = new EquationContainer(typeof(StringIntegerConverter), 1);
            U1 = false;
            U2 = false;
            U3 = false;
            UR1 = false;
            UR2 = false;
            UR3 = false;
        }
        public SubmodelBC(SerializationInfo info, StreamingContext context)
           : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_stepNumber":
                        // Compatibility for version v1.4.0
                        if (entry.Value is int value)
                            StepNumber = new EquationContainer(typeof(StringIntegerConverter), value);
                        else
                            SetStepNumber((EquationContainer)entry.Value, false);
                        break;
                    case "_u1":
                        _u1 = (bool)entry.Value; break;
                    case "_u2":
                        _u2 = (bool)entry.Value; break;
                    case "_u3":
                        _u3 = (bool)entry.Value; break;
                    case "_ur1":
                        _ur1 = (bool)entry.Value; break;
                    case "_ur2":
                        _ur2 = (bool)entry.Value; break;
                    case "_ur3":
                        _ur3 = (bool)entry.Value; break;
                    default:
                        break;
                }
            }
        }

        // Methods                                                                                                                  
        private void SetStepNumber(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _stepNumber, value, CheckStepNumber, checkEquation);
        }
        //
        private double CheckStepNumber(double value)
        {
            if (value < 1) return 1;
            else return value;
        }
        //
        public int[] GetConstrainedDirections()
        {
            List<int> directions = new List<int>();
            if (_u1) directions.Add(1);
            if (_u2) directions.Add(2);
            if (_u3) directions.Add(3);
            if (_ur1) directions.Add(4);
            if (_ur2) directions.Add(5);
            if (_ur3) directions.Add(6);
            return directions.ToArray();
        }
        // IContainsEquations
        public override void CheckEquations()
        {
            base.CheckEquations();
            //
            _stepNumber.CheckEquation();
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_stepNumber", _stepNumber, typeof(EquationContainer));
            info.AddValue("_u1", _u1, typeof(bool));
            info.AddValue("_u2", _u2, typeof(bool));
            info.AddValue("_u3", _u3, typeof(bool));
            info.AddValue("_ur1", _ur1, typeof(bool));
            info.AddValue("_ur2", _ur2, typeof(bool));
            info.AddValue("_ur3", _ur3, typeof(bool));
        }
    }
}
