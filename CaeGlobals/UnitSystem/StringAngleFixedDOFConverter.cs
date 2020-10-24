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
    public class StringAngleFixedDOFConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static AngleUnit _angleUnit = AngleUnit.Radian;
        //
        protected ArrayList values;
        protected string _fixed = "Fixed";


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
        public StringAngleFixedDOFConverter()
        {
            // Initializes the standard values list with defaults.
            values = new ArrayList(new double[] { double.PositiveInfinity, 0});
        }


        // Methods                                                                                                                  

        // Indicates this converter provides a list of standard values.
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        // Returns a StandardValuesCollection of standard value objects.
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            // Passes the local integer array.
            StandardValuesCollection svc = new StandardValuesCollection(values);
            return svc;
        }

        // Returns true for a sourceType of string to indicate that 
        // conversions from string to integer are supported. (The 
        // GetStandardValues method requires a string to native type 
        // conversion because the items in the drop-down list are 
        // translated to string.)
        public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
            else return base.CanConvertFrom(context, sourceType);
        }

        // If the type of the value to convert is string, parses the string 
        // and returns the integer to set the value of the property to. 
        // This example first extends the integer array that supplies the 
        // standard values collection if the user-entered value is not 
        // already in the array.
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // Convert from string
            if (value is string valueString)
            {
                double valueDouble;
                if (String.Equals(valueString, _fixed)) valueDouble = double.PositiveInfinity;
                else if (!double.TryParse(valueString, out valueDouble))
                {
                    Angle angle = Angle.Parse(valueString);
                    if ((int)_angleUnit != MyUnit.NoUnit) angle = angle.ToUnit(_angleUnit);
                    valueDouble = angle.Value;
                }
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
                        if (double.IsPositiveInfinity(valueDouble)) return _fixed;
                        else
                        {
                            string valueString = valueDouble.ToString();
                            if ((int)_angleUnit != MyUnit.NoUnit) valueString += " " + Angle.GetAbbreviation(_angleUnit);
                            return valueString;
                        }
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