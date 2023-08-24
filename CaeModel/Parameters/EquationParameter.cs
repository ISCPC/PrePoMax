using CaeGlobals;
using CaeMesh;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeModel
{
    [Serializable]
    public class EquationParameter : NamedClass
    {
        // Variables                                                                                                                
        private EquationContainer _equation;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Parameter name.")]
        public override string Name
        {
            get { return _name; }
            set
            {
                base.Name = value;
                if (_equation != null) CheckSelfReference(value, _equation.Equation);
            }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Value/Equation")]
        [DescriptionAttribute("Parameter equation.")]
        [TypeConverter(typeof(EquationDoubleConverter))]
        public string Equation
        {
            get { return _equation.Equation; }
            set { CheckSelfReference(_name, value); _equation.Equation = value; }
        }
        //
        [ReadOnly(true)]
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Evaluates to")]
        [DescriptionAttribute("Parameter value.")]
        public double Value { get { return _equation.Value; } }
        

        // Constructors                                                                                                             
        public EquationParameter()
        {
            _name = "Name";
            _equation = new EquationContainer(typeof(StringDoubleConverter), 0);
            //_equation.SetEquation("=");
        }


        // Methods                                                                                                                  
        private void CheckSelfReference(string name, string equation)
        {
            if (equation != null)
            {
                equation = equation.Trim();
                if (equation.StartsWith("="))
                {
                    equation = equation.Substring(1, equation.Length - 1);
                    HashSet<string> parameters = MyNCalc.GetParameters(equation);
                    if (parameters.Contains(name))
                        throw new CaeException("The equation contains self reference to the parameter name.");
                }
            }
        }
        
    }
}
