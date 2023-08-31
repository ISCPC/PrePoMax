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
                double result = (double)ConvertFrom(context, culture, valueString);
                string resultStr = (string)ConvertTo(context, culture, result, typeof(string));
                if (valueString.Contains("=")) return valueString.Trim().Replace(" ", "") + "; (" + resultStr + ")";
                else return resultStr;
            }
            return ConvertTo(context, culture, value, destinationType);
        }

        //public static object ConvertFromEquationToString2(ITypeDescriptorContext context, CultureInfo culture, object value,
        //                                                Func<ITypeDescriptorContext, CultureInfo, object, object> ConvertFrom)
        //{
        //    if (value is string valueString)
        //    {
        //        valueString = valueString.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)[0];
        //        EquationString es = new EquationString(valueString);
        //        return es;
        //    }
        //    else return ConvertFrom(context, culture, value);
        //}
        //public static object ConvertFromStringToEquation2(ITypeDescriptorContext context, CultureInfo culture,
        //                                                object value, Type destinationType,
        //                                                Func<ITypeDescriptorContext, CultureInfo, object, object> ConvertFrom,
        //                                                Func<ITypeDescriptorContext, CultureInfo, object, Type, object> ConvertTo)
        //{
        //    if (destinationType == typeof(string))
        //    {
        //        string valueString = ((EquationString)value).Equation;
        //        double result = (double)ConvertFrom(context, culture, valueString);
        //        string resultStr = (string)ConvertTo(context, culture, result, typeof(string));
        //        if (valueString.Contains("=")) return valueString.Trim().Replace(" ", "") + "; (" + resultStr + ")";
        //        else return resultStr;
        //    }
        //    return ConvertTo(context, culture, value, destinationType);
        //}
    }
}
