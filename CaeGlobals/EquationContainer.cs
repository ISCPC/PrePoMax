using NCalc.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
        public string Equation { get { return _equation; } set { SetEquation(value, true); } }
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
        public void SetEquation(string equation, bool enableEquationChanged = false)
        {
            try
            {
                if (equation == null || equation.Trim() == "") equation = "0";
                //
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
                        _equation = equation.Replace(" ", "");
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
        
    }
}
