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
    public class StringAccelerationConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static AccelerationUnit _accelerationUnit = AccelerationUnit.MeterPerSecondSquared;


        // Properties                                                                                                               
        public static string SetUnit
        {
            set
            {
                if (value == "") _accelerationUnit = (AccelerationUnit)MyUnit.NoUnit;
                else _accelerationUnit = Acceleration.ParseUnit(value);
            }
        }


        // Constructors                                                                                                             
        public StringAccelerationConverter()
        {
        }


        // Methods                                                                                                                  
        public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
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
                    Acceleration acceleration = Acceleration.Parse(valueString);
                    if ((int)_accelerationUnit != MyUnit.NoUnit) acceleration = acceleration.ToUnit(_accelerationUnit);
                    valueDouble = acceleration.Value;
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
                        if ((int)_accelerationUnit != MyUnit.NoUnit)
                            valueString += " " + Acceleration.GetAbbreviation(_accelerationUnit);
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