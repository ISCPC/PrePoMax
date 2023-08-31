using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using CaeMesh;

namespace CaeModel
{
    [Serializable]
    public abstract class MaterialProperty : ISerializable
    {
        // Variables                                                                                                                
        [NonSerialized]
        protected const string _positive = "The value must be larger than 0.";


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public MaterialProperty()
        {
        }
        public MaterialProperty(SerializationInfo info, StreamingContext context)
        {
        }


        // Methods                                                                                                                  
        protected static void SetAndCheck(ref EquationContainer[][] variable, EquationContainer[][] value,
                                          Func<double, double> CheckValue, bool check)
        {
            variable = new EquationContainer[value.Length][];
            //
            for (int i = 0; i < value.Length; i++)
            {
                variable[i] = new EquationContainer[2];
                SetAndCheck(ref variable[i][0], value[i][0], CheckValue, null, check);
                SetAndCheck(ref variable[i][1], value[i][1], CheckValue, null, check);
            }
        }
        protected static void SetAndCheck(ref EquationContainer variable, EquationContainer value, Func<double, double> CheckValue,
                                          bool check)
        {
            SetAndCheck(ref variable, value, CheckValue, null, check);
        }
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
        // ISerialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
