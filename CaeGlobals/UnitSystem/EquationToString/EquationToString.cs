using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeGlobals
{
    internal static class EquationToString
    {
        public static object ConvertFromEquationToString(ITypeDescriptorContext context, CultureInfo culture, object value,
                                                        Func<ITypeDescriptorContext, CultureInfo, object, object> ConvertFrom)
        {
            if (value is string valueString)
            {
                valueString = valueString.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)[0];
                return valueString;
            }
            else return ConvertFrom(context, culture, value);
        }

        public static object ConvertFromStringToEquation(ITypeDescriptorContext context, CultureInfo culture,
                                                        object value, Type destinationType,
                                                        Func<ITypeDescriptorContext, CultureInfo, object, object> ConvertFrom,
                                                        Func<ITypeDescriptorContext, CultureInfo, object, Type, object> ConvertTo)
        {
            if (value is string valueString)
            {
                double result = Convert.ToDouble(ConvertFrom(context, culture, valueString));
                string resultStr = (string)ConvertTo(context, culture, result, typeof(string));
                if (valueString.Contains("=")) return valueString.Trim().Replace(" ", "") + "; (" + resultStr + ")";
                else return resultStr;
            }
            return ConvertTo(context, culture, value, destinationType);
        }
        //
        public static object ConvertFromStringToEquationString(ITypeDescriptorContext context, CultureInfo culture, object value,
                                                               Func<ITypeDescriptorContext, CultureInfo, object, object> ConvertFrom)
        {
            if (value is string valueString)
            {
                string equation;
                if (valueString.Length > 0)
                    equation = valueString.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)[0];
                else equation = valueString;
                // Test the conversion
                ConvertFrom(context, culture, equation);
                // Return the equation
                return new EquationString(equation);
            }
            else return ConvertFrom(context, culture, value);
        }
        //
        public static object ConvertToStringFromEquationString(ITypeDescriptorContext context, CultureInfo culture,
                                                               object value, Type destinationType,
                                                               Func<ITypeDescriptorContext, CultureInfo, object, object> ConvertFrom,
                                                               Func<ITypeDescriptorContext, CultureInfo, object, Type, object> ConvertTo,
                                                               bool addResult = true)
        {
            try
            {
                if (destinationType == typeof(string) && value is EquationString equationString)
                {
                    double result = Convert.ToDouble(ConvertFrom(context, culture, equationString.Equation));
                    string resultStr = (string)ConvertTo(context, culture, result, typeof(string));
                    //
                    if (equationString.IsEquation())
                    {
                        string equation = equationString.Equation.Trim().Replace(" ", "");
                        if (addResult) return equation + "; (" + resultStr + ")";
                        else return equation;
                    }
                    else return resultStr;
                }
                return ConvertTo(context, culture, value, destinationType);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("Parameter was not defined")) return ((EquationString)value).Equation + "; (Error)";
                else throw ex;
            }
        }




       
    }
}
