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
    public class EquationContainer
    {
        // Variables                                                                                                                
        private Type _stringDoubleConverterType;
        private string _equation;
        [NonSerialized]
        private Func<double, double> _checkValue;
        [NonSerialized]
        private Action _equationChanged;


        // Properties                                                                                                               
        public double Value { get { return GetValueFromEquation(_equation); } }
        //public string Equation { get { return _equation; } set { SetEquation(value, true); } }
        public EquationString Equation
        {
            get { return new EquationString(_equation); }
            set { SetEquation(value.Equation, true); }
        }
        public string String { get { return _equation; } }
        public Func<double, double> CheckValue { get { return _checkValue; } set { _checkValue = value; } }
        public Action EquationChanged { get { return _equationChanged; } set { _equationChanged = value; } }
       

        // Constructors                                                                                                             
        public EquationContainer(Type stringDoubleConverterType, double value, Func<double, double> checkValue = null)
        {
            _stringDoubleConverterType = stringDoubleConverterType;
            _checkValue = checkValue;
            SetEquationFromValue(value, false);
        }


        // Methods                                                                                                                  
        private string GetEquationFromValue(double value)
        {
            if (_stringDoubleConverterType == null) throw new NotSupportedException();
            TypeConverter stringDoubleConverter = (TypeConverter)Activator.CreateInstance(_stringDoubleConverterType);
            return (string)stringDoubleConverter.ConvertTo(value, typeof(string));
        }
        private double GetValueFromEquation(string equation)
        {
            if (_stringDoubleConverterType == null) throw new NotSupportedException();
            TypeConverter stringDoubleConverter = (TypeConverter)Activator.CreateInstance(_stringDoubleConverterType);
            return (double)stringDoubleConverter.ConvertFrom(equation);
        }
        public void SetConverterType(Type stringDoubleConverterType)
        {
            if (IsEquation())
            {
                _stringDoubleConverterType = stringDoubleConverterType;
            }
            else
            {
                double value = Value;
                _stringDoubleConverterType = stringDoubleConverterType;
                SetEquationFromValue(value);
            }
            
        }
        public void SetEquation(EquationString equation)
        {
            SetEquation(equation.Equation);
        }
        public void SetEquation(string equation, bool enableEquationChanged = false)
        {
            try
            {
                if (equation == null || equation.Trim() == "") equation = "0";
                //
                equation = equation.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)[0];
                double equationValue = GetValueFromEquation(equation);
                double checkedValue = _checkValue != null ? _checkValue(equationValue) : equationValue;
                if (equationValue != checkedValue)
                {
                    _equation = GetEquationFromValue(checkedValue);
                    if (enableEquationChanged) _equationChanged?.Invoke();
                }
                else
                {
                    if (_equation != equation)
                    {
                        if (equation.StartsWith("=")) equation = equation.Replace(" ", "");
                        _equation = equation;
                        if (enableEquationChanged) _equationChanged?.Invoke();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CaeException("Equation error: " + equation + Environment.NewLine + ex.Message);
            }
        }
        public void SetEquationFromValue(double value, bool enableEquationChanged = false)
        {
            SetEquation(GetEquationFromValue(value), enableEquationChanged);
        }
        public void CheckEquation()
        {
            SetEquation(_equation);
        }
        public bool IsEquation()
        {
            if (_equation != null && _equation.StartsWith("=")) return true;
            else return false;
        }
        //
        public static void SetAndCheck(ref EquationContainer variable, EquationContainer value, Func<double, double> CheckValue,
                                          bool check)
        {
            SetAndCheck(ref variable, value, CheckValue, null, check);
        }
        public static void SetAndCheck(ref EquationContainer[][] variable, EquationContainer[][] value,
                                       Func<double, double>[] CheckValue, bool check)
        {
            Func<double, double> checkFunction;
            variable = new EquationContainer[value.Length][];
            //
            for (int i = 0; i < value.Length; i++)
            {
                variable[i] = new EquationContainer[value[i].Length];
                for (int j = 0; j < value[i].Length; j++)
                {
                    if (CheckValue == null) checkFunction = null;
                    else checkFunction = CheckValue[j];
                    SetAndCheck(ref variable[i][j], value[i][j], checkFunction, null, check);
                }
            }
        }

        public static void SetAndCheck(ref EquationContainer variable, EquationContainer value, Func<double, double> CheckValue,
                                           Action EquationChangedCallback, bool check)
        {
            if (value == null)
            {
                variable = null;
                return;
            }
            //
            string prevEquation = variable != null ? variable.String : value.String;
            //
            value.CheckValue = CheckValue;
            value.EquationChanged = EquationChangedCallback;
            //
            if (check)
            {
                value.CheckEquation();
                if (variable != null && prevEquation != variable.String) EquationChangedCallback?.Invoke();
            }
            //
            variable = value;
        }
    }
}
