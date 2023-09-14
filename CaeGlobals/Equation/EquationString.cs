using NCalc.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnitsNet;

namespace CaeGlobals
{
    [Serializable]
    public class EquationString
    {
        // Variables                                                                                                                
        private string _equation;


        // Properties                                                                                                               
        public string Equation { get { return _equation; } set { _equation = value; } }


        // Constructors                                                                                                             
        public EquationString()
            : this(null)
        {
        }
        public EquationString(string equation)
        {
            _equation = equation;
        }


        // Operators                                                                                                                
        public static bool operator ==(EquationString es1, EquationString es2) => es1.Equation == es2.Equation;
        public static bool operator !=(EquationString es1, EquationString es2) => es1.Equation != es2.Equation;
        //
        //public static implicit operator string(EquationString equationString)
        //{
        //    return equationString._equation;
        //}
        //public static implicit operator EquationString(string equation)
        //{
        //    return new EquationString(equation);
        //}
        //

        // Methods                                                                                                                  
        public bool IsEquation()
        {
            if (_equation != null && _equation.StartsWith("=")) return true;
            else return false;
        }
    }
}
