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
    public class StringAreaConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static AreaUnit _areaUnit = AreaUnit.SquareMeter;


        // Properties                                                                                                               
        public static string SetUnit
        {
            set
            {
                if (value == "") _areaUnit = (AreaUnit)MyUnit.NoUnit;
                else _areaUnit = Area.ParseUnit(value);
            }
        }


        // Constructors                                                                                                             
        public StringAreaConverter()
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
                    Area area = Area.Parse(valueString);
                    if ((int)_areaUnit != MyUnit.NoUnit) area = area.ToUnit(_areaUnit);
                    valueDouble = area.Value;
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
                        if ((int)_areaUnit != MyUnit.NoUnit) valueString += " " + Area.GetAbbreviation(_areaUnit);
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