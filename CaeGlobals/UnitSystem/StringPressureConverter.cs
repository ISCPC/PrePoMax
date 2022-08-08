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
    public class StringPressureConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static PressureUnit _pressureUnit = PressureUnit.Pascal;
        

        // Properties                                                                                                               
        public static string SetUnit
        {
            set
            {
                if (value == "") _pressureUnit = (PressureUnit)MyUnit.NoUnit;
                else _pressureUnit = Pressure.ParseUnit(value);
            }
        }
        

        // Constructors                                                                                                             
        public StringPressureConverter()
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
                    valueDouble = ConvertToCrrentUnits(valueString);
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
                        if ((int)_pressureUnit != MyUnit.NoUnit) valueString += " " + Pressure.GetAbbreviation(_pressureUnit);
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
        //
        public static double ConvertToCrrentUnits(string valueWithUnitString)
        {
            Pressure pressure = Pressure.Parse(valueWithUnitString);
            if ((int)_pressureUnit != MyUnit.NoUnit) pressure = pressure.ToUnit(_pressureUnit);
            return pressure.Value;
        }
    }

}