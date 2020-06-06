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
    public class StringEnergyPerVolumeConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static EnergyUnit _energyUnit = EnergyUnit.Joule;
        protected static VolumeUnit _volumeUnit = VolumeUnit.CubicMeter;
        protected static string _inlb = "in·lb";
        protected static string error = "Unable to parse quantity. Expected the form \"{value} {unit abbreviation}" +
                                        "\", such as \"5.5 m\". The spacing is optional.";


        // Properties                                                                                                               
        public static string SetEnergyUnit
        {
            set
            {
                if (value == _inlb) _energyUnit = (EnergyUnit)100;
                else { _energyUnit = Energy.ParseUnit(value); }
            }
        }
        public static string SetVolumeUnit { set { _volumeUnit = Volume.ParseUnit(value); } }
        public static string GetUnitAbbreviation()
        {
            return Energy.GetAbbreviation(_energyUnit) + "/" + Volume.GetAbbreviation(_volumeUnit);
        }


        // Constructors                                                                                                             
        public StringEnergyPerVolumeConverter()
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
                    valueString = valueString.Trim().Replace(" ", "");
                    //
                    string[] tmp = valueString.Split('/');
                    if (tmp.Length != 2) throw new FormatException(error);
                    //
                    StringEnergyConverter converter = new StringEnergyConverter();
                    double energy = (double)converter.ConvertFromString(tmp[0]);
                    //
                    VolumeUnit volumeUnit = Volume.ParseUnit(tmp[1]);
                    Volume volume = Volume.From(1, volumeUnit).ToUnit(_volumeUnit);
                    valueDouble = energy / volume.Value;
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
                        return value + " " + GetUnitAbbreviation();
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
    }


}