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
    public class StringAngleConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static AngleUnit _angleUnit = AngleUnit.Radian;


        // Properties                                                                                                               
        public static string SetUnit
        {
            set
            {
                if (value == "") _angleUnit = (AngleUnit)MyUnit.NoUnit;
                else _angleUnit = Angle.ParseUnit(value);
            }
        }


        // Constructors                                                                                                             
        public StringAngleConverter()
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
                    Angle angle = Angle.Parse(valueString);
                    if ((int)_angleUnit != MyUnit.NoUnit) angle = angle.ToUnit(_angleUnit);
                    valueDouble = angle.Value;
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
                        if ((int)_angleUnit != MyUnit.NoUnit) valueString += " " + Angle.GetAbbreviation(_angleUnit);
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