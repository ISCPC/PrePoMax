using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public class LinearSpringSection : Section, ISerializable
    {
        // Variables                                                                                                                
        private int _direction;                 //ISerializable
        private EquationContainer _stiffness;   //ISerializable


        // Properties                                                                                                               
        public int Direction { get { return _direction; } set { _direction = value; } }
        public EquationContainer Stiffness { get { return _stiffness; } set { SetStiffness(value); } }


        // Constructors                                                                                                             
        public LinearSpringSection(string name, string elementSetName, int direction, double stiffness, bool twoD)
            : base(name, null, elementSetName, RegionTypeEnum.ElementSetName, 1, twoD)
        {
            Stiffness = new EquationContainer(typeof(StringForcePerLengthConverter), stiffness);
        }
        public LinearSpringSection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_direction":
                        _direction = (int)entry.Value;
                        break;
                    case "_stiffness":
                        SetStiffness((EquationContainer)entry.Value, false);
                        break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        private void SetStiffness(EquationContainer value, bool checkEquation = true)
        {
            EquationContainer.SetAndCheck(ref _stiffness, value, null, checkEquation);
        }
        // IContainsEquations
        public override void CheckEquations()
        {
            base.CheckEquations();
            //
            _stiffness.CheckEquation();
        }
        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_direction", _direction, typeof(int));
            info.AddValue("_stiffness", _stiffness, typeof(EquationContainer));
        }
    }
}
