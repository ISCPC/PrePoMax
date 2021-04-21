using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;
using UnitsNet;
using UnitsNet.Units;

namespace CaeGlobals
{
    public class StringExpansionConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static CoefficientOfThermalExpansionUnit _expansionUnit = CoefficientOfThermalExpansionUnit.InverseDegreeCelsius;


        // Properties                                                                                                               
        public static string SetUnit
        {
            set
            {
                if (value == "") _expansionUnit = (CoefficientOfThermalExpansionUnit)MyUnit.NoUnit;
                else _expansionUnit = CoefficientOfThermalExpansion.ParseUnit(value);
            }
        }


        // Constructors                                                                                                             
        public StringExpansionConverter()
        {
        }


        // Methods                                                                                                                  
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
            else return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // Convert from string
            if (value is string valueString)
            {
                double valueDouble;
                //
                if (!double.TryParse(valueString, out valueDouble))
                {
                    CoefficientOfThermalExpansion expansion = CoefficientOfThermalExpansion.Parse(valueString);
                    if ((int)_expansionUnit != MyUnit.NoUnit) expansion = expansion.ToUnit(_expansionUnit);
                    valueDouble = expansion.Value;
                }
                //
                return valueDouble;
            }
            else return base.ConvertFrom(context, culture, value);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            // Convert to string
            try
            {
                if (destinationType == typeof(string))
                {
                    if (value is double valueDouble)
                    {
                        string valueString = valueDouble.ToString();
                        if ((int)_expansionUnit != MyUnit.NoUnit)
                            valueString += " " + CoefficientOfThermalExpansion.GetAbbreviation(_expansionUnit);
                        return valueString;
                    }
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
            catch
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}