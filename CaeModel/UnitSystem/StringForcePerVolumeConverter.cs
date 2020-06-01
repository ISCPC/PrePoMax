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

namespace CaeModel
{
    public class StringForcePerVolumeConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static ForceUnit _forceUnit = ForceUnit.Newton;
        protected static VolumeUnit _volumeUnit = VolumeUnit.CubicMeter;


        // Properties                                                                                                               
        public static string SetForceUnit { set { _forceUnit = Force.ParseUnit(value); } }
        public static string SetVolumeUnit { set { _volumeUnit = Volume.ParseUnit(value); } }


        // Constructors                                                                                                             
        public StringForcePerVolumeConverter()
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
                if (!double.TryParse(valueString, out valueDouble))
                {
                    valueDouble = ConvertForcePerVolume(valueString);
                }
                return valueDouble;
            }
            else return base.ConvertFrom(context, culture, value);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            try
            {
                if (destinationType == typeof(string))
                {
                    if (value is double valueDouble)
                    {
                        return value + " " + Force.GetAbbreviation(_forceUnit) + "/" + Volume.GetAbbreviation(_volumeUnit);
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
        private static double ConvertForcePerVolume(string valueWithUnitString)
        {
            string error = "Unable to parse quantity. Expected the form \"{value} {unit abbreviation}" +
                           "\", such as \"5.5 m\". The spacing is optional.";
            valueWithUnitString = valueWithUnitString.Trim().Replace(" ", "");
            //
            string[] tmp = valueWithUnitString.Split('/');
            if (tmp.Length != 2) throw new FormatException(error);
            Force force = Force.Parse(tmp[0]).ToUnit(_forceUnit);
            //
            VolumeUnit volumeUnit = Volume.ParseUnit(tmp[1]);
            Volume volume = Volume.From(1, volumeUnit).ToUnit(_volumeUnit);
            double value = force.Value / volume.Value;
            return value;
        }
    }


}