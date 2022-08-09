﻿using System;
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
                    valueDouble = ConvertToCurrentUnits(valueString);
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
        //
        public static double ConvertToCurrentUnits(string valueWithUnitString)
        {
            try
            {
                UnitsNet.Density density = UnitsNet.Density.Parse(valueWithUnitString);
                if ((int)_densityUnit != MyUnit.NoUnit) density = density.ToUnit(_densityUnit);
                return density.Value;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + Environment.NewLine + Environment.NewLine + SupportedUnitAbbreviations());
            }
        }
        public static string SupportedUnitAbbreviations()
        {
            string abb;
            string supportedUnitAbbreviations = "Supported density abbreviations: ";
            var allUnits = UnitsNet.Density.Units;
            for (int i = 0; i < allUnits.Length; i++)
            {
                abb = UnitsNet.Density.GetAbbreviation(allUnits[i]);
                if (abb != null) abb.Trim();
                if (abb.Length > 0) supportedUnitAbbreviations += abb;
                if (i != allUnits.Length - 1) supportedUnitAbbreviations += ", ";
            }
            supportedUnitAbbreviations += ".";
            //
            return supportedUnitAbbreviations;
        }
    }

}