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
using System.Diagnostics.Eventing.Reader;

namespace CaeGlobals
{
    public class StringDoubleArrayConverter : TypeConverter
    {
        // Variables                                                                                                                


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public StringDoubleArrayConverter()
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
                string[] tmp = valueString.Split(new string[] { ";", " " }, StringSplitOptions.RemoveEmptyEntries);
                double[] values = new double[tmp.Length];
                for (int i = 0; i < tmp.Length; i++)
                {
                    if (!double.TryParse(tmp[i], out values[i]))
                        throw new FormatException();
                }
                return values;
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
                    if (value is double[] values)
                    {
                        string valueString = "";
                        for (int i = 0; i < values.Length; i++)
                        {
                            if (i != 0) valueString += " ";
                            valueString += values[i] + ";";
                        }
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