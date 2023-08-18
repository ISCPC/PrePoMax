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

namespace CaeGlobals
{
    public class EquationPowerConverter : StringPowerConverter
    {
        // Variables                                                                                                                


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public EquationPowerConverter()
        {
        }


        // Methods                                                                                                                  
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // Convert from equation to string
            return EquationToString.ConvertFromEquationToString(context, culture, value, base.ConvertFrom);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            // Convert from string to equation
            return EquationToString.ConvertFromStringToEquation(context, culture, value, destinationType,
                                                                base.ConvertFrom, base.ConvertTo);
        }
    }


}