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
    public class DoubleValueContainer
    {
        // Variables                                                                                                                
        private Type _stringDoubleConverterType;
        private string _equation;
        private double _value;
        private Func<double, double> _checkValue;


        // Properties                                                                                                               
        public double Value
        {
            get { return _value; }
            set
            {
                if (_checkValue != null) value = _checkValue(value);
                //
                _value = value;
                _equation = null;
            }
        }
        public string Equation
        {
            get
            {
                if (_stringDoubleConverterType == null) return null;
                //
                TypeConverter stringDoubleConverter = (TypeConverter)Activator.CreateInstance(_stringDoubleConverterType);
                //
                if (_equation == null) _equation = (string)stringDoubleConverter.ConvertTo(_value, typeof(string));
                return _equation;
            }
            set
            {
                TypeConverter stringDoubleConverter = (TypeConverter)Activator.CreateInstance(_stringDoubleConverterType);
                //
                Value = (double)stringDoubleConverter.ConvertFrom(value);   // get the equation result - check the value
                _equation = value;                                          // save the equation
            }
        }
        public Func<double, double> CheckValue
        {
            get { return _checkValue; }
            set
            {
                _checkValue = value;
                if (_checkValue != null)
                {
                    double checkedValue = _checkValue(_value);
                    if (checkedValue != _value) Value = checkedValue;
                }
            }
        }


        // Constructors                                                                                                             
        public DoubleValueContainer(Type stringDoubleConverterType, double value, Func<double, double> checkValue = null)
        {
            _stringDoubleConverterType = stringDoubleConverterType;
            _checkValue = checkValue;
            Value = value;  // also sets the equation no null
        }


        // Methods                                                                                                                  
        public void SetValue(double value, bool applyCheck)
        {
            if (applyCheck) Value = value;
            else
            {
                _value = value;
                _equation = null;
            }
        }
    }
}
