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
    public class StringDensityConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static DensityUnit _densityUnit = DensityUnit.TonnePerCubicMillimeter;


        // Properties                                                                                                               
        public static string SetUnit
        {
            set
            {
                if (value == "") _densityUnit = (DensityUnit)MyUnit.NoUnit;
                else _densityUnit = Density.ParseUnit(value);
            }
        }


        // Constructors                                                                                                             
        public StringDensityConverter()
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
                    UnitsNet.Density density = UnitsNet.Density.Parse(valueString);
                    if ((int)_densityUnit != MyUnit.NoUnit) density = density.ToUnit(_densityUnit);
                    valueDouble = density.Value;
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
                        if ((int)_densityUnit != MyUnit.NoUnit) valueString += " " + UnitsNet.Density.GetAbbreviation(_densityUnit);
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