using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;
using UnitsNet.Units;
using UnitsNet;
using System.Security.AccessControl;

namespace CaeGlobals
{
    public class EquationDensityFromConverter2 : StringDensityFromConverter
    {
        // Variables                                                                                                                


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public EquationDensityFromConverter2()
        {
        }


        // Methods                                                                                                                  
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // Convert from string to equation
            if (value is string valueString)
            {
                string equation = valueString.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)[0];
                // Test the conversion
                base.ConvertFrom(context, culture, equation);
                // Return the equation
                return new EquationString(equation);
            }
            else return ConvertFrom(context, culture, value);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            // Convert from equation to string
            if (destinationType == typeof(string) && value is EquationString equationString)
            {
                if (equationString.IsEquation())
                {
                    double result = (double)base.ConvertFrom(context, culture, equationString.Equation);
                    string resultStr = (string)base.ConvertTo(context, culture, result, typeof(string));
                    return equationString.Equation.Trim().Replace(" ", "") + "; (" + resultStr + ")";
                }
                else return equationString.Equation;
            }
            return ConvertTo(context, culture, value, destinationType);
        }
    }


}