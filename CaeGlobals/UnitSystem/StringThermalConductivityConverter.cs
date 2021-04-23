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
    public class StringThermalConductivityConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static PowerUnit _powerUnit = PowerUnit.Watt;
        protected static LengthUnit _lengthUnit = LengthUnit.Meter;
        protected static TemperatureDeltaUnit _temperatureDeltaUnit = TemperatureDeltaUnit.DegreeCelsius;
        protected static ThermalConductivityUnit _thermalConductivityUnit = MyUnit.InchPoundPerSecondInchFahrenheit;
        protected static string error = "Unable to parse quantity. Expected the form \"{value} {unit abbreviation}" +
                                        "\", such as \"5.5 m\". The spacing is optional.";


        // Properties                                                                                                               
        public static string SetPowerUnit
        {
            set
            {
                if (value == "")
                {
                    _powerUnit = (PowerUnit)MyUnit.NoUnit;
                    _lengthUnit = (LengthUnit)MyUnit.NoUnit;
                    _temperatureDeltaUnit = (TemperatureDeltaUnit)MyUnit.NoUnit;
                }
                else _powerUnit = Power.ParseUnit(value);
                //
                _thermalConductivityUnit = ThermalConductivityUnit.Undefined;
            }
        }
        public static string SetLengthUnit
        { 
            set 
            {
                if (value == "")
                {
                    _powerUnit = (PowerUnit)MyUnit.NoUnit;
                    _lengthUnit = (LengthUnit)MyUnit.NoUnit;
                    _temperatureDeltaUnit = (TemperatureDeltaUnit)MyUnit.NoUnit;
                }
                else _lengthUnit = Length.ParseUnit(value);
                //
                _thermalConductivityUnit = ThermalConductivityUnit.Undefined;
            }
        }
        public static string SetTemperatureDeltaUnit
        {
            set
            {
                if (value == "")
                {
                    _powerUnit = (PowerUnit)MyUnit.NoUnit;
                    _lengthUnit = (LengthUnit)MyUnit.NoUnit;
                    _temperatureDeltaUnit = (TemperatureDeltaUnit)MyUnit.NoUnit;
                }
                else _temperatureDeltaUnit = TemperatureDelta.ParseUnit(value);
                //
                _thermalConductivityUnit = ThermalConductivityUnit.Undefined;
            }
        }
        public static string SetUnit
        {
            set
            {
                if (value == MyUnit.InchPoundPerSecondInchFahrenheitAbbreviation)
                    _thermalConductivityUnit = MyUnit.InchPoundPerSecondInchFahrenheit;
                else throw new NotSupportedException();
            }
        }
        public static string GetUnitAbbreviation(PowerUnit powerUnit, LengthUnit lengthUnit,
                                                 TemperatureDeltaUnit temperatureDeltaUnit,
                                                 ThermalConductivityUnit thermalConductivityUnit)
        {
            string unit;
            if (thermalConductivityUnit == MyUnit.InchPoundPerSecondInchFahrenheit)
                unit = MyUnit.InchPoundPerSecondInchFahrenheitAbbreviation;
            else if ((int)powerUnit == MyUnit.NoUnit || (int)lengthUnit == MyUnit.NoUnit ||
                     (int)temperatureDeltaUnit == MyUnit.NoUnit)
                unit = "";
            else unit = Power.GetAbbreviation(powerUnit) + "/(" + Length.GetAbbreviation(lengthUnit) + "·" +
                        TemperatureDelta.GetAbbreviation(temperatureDeltaUnit) + ")";
            return unit.Replace("∆", "");
        }
        
        
        // Constructors                                                                                                             
        public StringThermalConductivityConverter()
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
                        string valueString = valueDouble.ToString();
                        string unit = GetUnitAbbreviation(_powerUnit, _lengthUnit, _temperatureDeltaUnit, _thermalConductivityUnit);
                        if (unit.Length > 0) valueString += " " + unit;
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
        private static double ConvertForcePerVolume(string valueWithUnitString)
        {            
            valueWithUnitString = valueWithUnitString.Trim().Replace(" ", "");
            // My unit
            if (valueWithUnitString.Contains(MyUnit.InchPoundPerSecondInchFahrenheitAbbreviation))
            {
                double valueDouble;
                valueWithUnitString = valueWithUnitString.Replace(MyUnit.InchPoundPerSecondInchFahrenheitAbbreviation, "");
                if (_thermalConductivityUnit == MyUnit.InchPoundPerSecondInchFahrenheit &&
                    double.TryParse(valueWithUnitString, out valueDouble))
                {
                    return valueDouble;
                }
                else throw new ArgumentException(error);
            }
            // Other units
            string[] tmp = valueWithUnitString.Split('/');
            if (tmp.Length != 2) throw new FormatException(error);
            Power power = Power.Parse(tmp[0]);
            // NoUnit
            if ((int)_powerUnit == MyUnit.NoUnit || (int)_lengthUnit == MyUnit.NoUnit || (int)_temperatureDeltaUnit == MyUnit.NoUnit)
                return (double)power.Value;
            else power = power.ToUnit(_powerUnit);
            //
            tmp = tmp[1].Replace("(", "").Replace(")", "").Split(new string[] { "*", "·" }, StringSplitOptions.RemoveEmptyEntries);
            if (tmp.Length != 2) throw new FormatException(error);
            //
            LengthUnit lengthUnit = Length.ParseUnit(tmp[0]);
            Length length = Length.From(1, lengthUnit).ToUnit(_lengthUnit);
            //
            if (!tmp[1].Contains("∆")) tmp[1] = "∆" + tmp[1];
            TemperatureDeltaUnit temperatureDeltaUnit = TemperatureDelta.ParseUnit(tmp[1]);
            TemperatureDelta temperatureDelta = TemperatureDelta.From(1, temperatureDeltaUnit).ToUnit(_temperatureDeltaUnit);
            //
            double value = (double)power.Value / (length.Value * temperatureDelta.Value);
            return value;
        }
    }


}