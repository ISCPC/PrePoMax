using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using System.Drawing;
using UnitsNet;
using UnitsNet.Units;
using System.Runtime.Serialization;
using DynamicTypeDescriptor;
using System.ComponentModel;


namespace CaeGlobals
{
    [Serializable]
    public enum UnitSystemType
    {
        [Description("Undefined")]
        Undefined = 0,
        //
        [Description("m, kg, s, °C")]
        M_KG_S_C = 1,
        //
        [Description("mm, ton, s, °C")]
        MM_TON_S_C = 2,
        //
        [Description("m, ton, s, °C")]
        M_TON_S_C = 3,
        //
        [Description("in, lb, s, °F")]
        IN_LB_S_F = 20,
        //
        [Description("Unitless")]
        UNIT_LESS = 40
    }


    [Serializable]
    public class UnitSystem : ISerializable
    {
        // Variables                                                                                                                
        private UnitSystemType _unitSystemType;                             //ISerializable
        // Base units
        private LengthUnit _lengthUnit;                                     //ISerializable
        private AngleUnit _angleUnit;                                       //ISerializable
        private MassUnit _massUnit;                                         //ISerializable
        private DurationUnit _timeUnit;                                     //ISerializable
        private TemperatureUnit _temperatureUnit;                           //ISerializable
        private TemperatureDeltaUnit _temperatureDeltaUnit;                 //ISerializable
        // Derived units
        private AreaUnit _areaUnit;                                         //ISerializable
        private VolumeUnit _volumeUnit;                                     //ISerializable
        private SpeedUnit _speedUnit;                                       //ISerializable
        private RotationalSpeedUnit _rotationalSpeedUnit;                   //ISerializable
        private AccelerationUnit _accelerationUnit;                         //ISerializable
        private ForceUnit _forceUnit;                                       //ISerializable
        private ForcePerLengthUnit _forcePerLengthUnit;                     //ISerializable
        private TorqueUnit _momentUnit;                                     //ISerializable
        private PressureUnit _pressureUnit;                                 //ISerializable
        private DensityUnit _densityUnit;                                   //ISerializable
        private EnergyUnit _energyUnit;                                     //ISerializable
        private PowerUnit _powerUnit;                                       //ISerializable
        private FrequencyUnit _frequencyUnit;                               //ISerializable
        // Thermal units
        private CoefficientOfThermalExpansionUnit _thermalExpansionUnit;    //ISerializable
        private ThermalConductivityUnit _thermalConductivityUnit;           //ISerializable


        // Properties                                                                                                               
        public UnitSystemType UnitSystemType { get { return _unitSystemType; } }
        // Abbreviations                                                                                
        //
        // Base units
        public string LengthUnitAbbreviation
        {
            get
            {
                if ((int)_lengthUnit == MyUnit.NoUnit) return "";
                else return Length.GetAbbreviation(_lengthUnit);
            }
        }
        public string AngleUnitAbbreviation
        {
            get
            {
                if ((int)_angleUnit == MyUnit.NoUnit) return "";
                else return Angle.GetAbbreviation(_angleUnit);
            }
        }
        public string MassUnitAbbreviation
        {
            get
            {
                if ((int)_massUnit == MyUnit.NoUnit) return "";
                else return Mass.GetAbbreviation(_massUnit);
            }
        }
        public string TimeUnitAbbreviation
        {
            get
            {
                if ((int)_timeUnit == MyUnit.NoUnit) return "";
                else return Duration.GetAbbreviation(_timeUnit);
            }
        }
        public string TemperatureUnitAbbreviation
        {
            get
            {
                if ((int)_temperatureUnit == MyUnit.NoUnit) return "";
                else return Temperature.GetAbbreviation(_temperatureUnit);
            }
        }
        public string TemperatureDeltaUnitAbbreviation
        {
            get
            {
                if ((int)_temperatureDeltaUnit == MyUnit.NoUnit) return "";
                else return TemperatureDelta.GetAbbreviation(_temperatureDeltaUnit);
            }
        }
        // Derived units
        public string AreaUnitAbbreviation
        {
            get
            {
                if ((int)_areaUnit == MyUnit.NoUnit) return "";
                else return Area.GetAbbreviation(_areaUnit);

            }
        }
        public string VolumeUnitAbbreviation
        {
            get
            {
                if ((int)_volumeUnit == MyUnit.NoUnit) return "";
                else return Volume.GetAbbreviation(_volumeUnit);
            }
        }
        public string SpeedUnitAbbreviation
        {
            get
            {
                if ((int)_speedUnit == MyUnit.NoUnit) return "";
                else return Speed.GetAbbreviation(_speedUnit);
            }
        }
        public string RotationalSpeedUnitAbbreviation
        {
            get
            {
                if ((int)_rotationalSpeedUnit == MyUnit.NoUnit) return "";
                else return RotationalSpeed.GetAbbreviation(_rotationalSpeedUnit);
            }
        }
        public string AccelerationUnitAbbreviation
        {
            get
            {
                if ((int)_accelerationUnit == MyUnit.NoUnit) return "";
                else return Acceleration.GetAbbreviation(_accelerationUnit);
            }
        }
        public string ForceUnitAbbreviation
        {
            get
            {
                if ((int)_forceUnit == MyUnit.NoUnit) return "";
                else return Force.GetAbbreviation(_forceUnit);
            }
        }
        public string ForcePerLengthUnitAbbreviation
        {
            get
            {
                if ((int)_forceUnit == MyUnit.NoUnit) return "";
                else return ForcePerLength.GetAbbreviation(_forcePerLengthUnit);
            }
        }
        public string MomentUnitAbbreviation
        {
            get
            {
                if ((int)_momentUnit == MyUnit.NoUnit) return "";
                else return Torque.GetAbbreviation(_momentUnit);
            }
        }
        public string PressureUnitAbbreviation
        {
            get
            {
                if ((int)_pressureUnit == MyUnit.NoUnit) return "";
                else return Pressure.GetAbbreviation(_pressureUnit);
            }
        }
        public string DensityUnitAbbreviation
        {
            get
            {
                if ((int)_densityUnit == MyUnit.NoUnit) return "";
                else return UnitsNet.Density.GetAbbreviation(_densityUnit);
            }
        }
        public string EnergyUnitAbbreviation
        {
            get
            {
                return StringEnergyConverter.GetUnitAbbreviation(_energyUnit);
            }
        }
        public string EnergyPerVolumeUnitAbbreviation
        {
            get
            {
                return StringEnergyPerVolumeConverter.GetUnitAbbreviation(_energyUnit, _volumeUnit);
            }
        }
        public string PowerUnitAbbreviation
        {
            get
            {
                return StringPowerConverter.GetUnitAbbreviation(_powerUnit);
            }
        }
        public string FrequencyUnitAbbreviation 
        { 
            get 
            {
                if ((int)_frequencyUnit == MyUnit.NoUnit) return "";
                else return Frequency.GetAbbreviation(_frequencyUnit); 
            } 
        }
        // Thermal units
        public string ThermalExpansionUnitAbbreviation
        {
            get
            {
                if ((int)_thermalExpansionUnit == MyUnit.NoUnit) return "";
                else return CoefficientOfThermalExpansion.GetAbbreviation(_thermalExpansionUnit);
            }
        }
        public string ThermalConductivityUnitAbbreviation
        {
            get
            {
                return StringThermalConductivityConverter.GetUnitAbbreviation(_powerUnit, _lengthUnit, _temperatureDeltaUnit,
                                                                              _thermalConductivityUnit);
            }
        }
        public string SpecificHeatUnitAbbreviation
        {
            get
            {
                return StringSpecificHeatConverter.GetUnitAbbreviation(_energyUnit, _massUnit, _temperatureDeltaUnit);
            }
        }


        // Constructors                                                                                                             
        public UnitSystem()
            : this(UnitSystemType.Undefined)
        {
        }
        public UnitSystem(UnitSystemType unitSystemType)
        {
            _unitSystemType = unitSystemType;
            //
            switch (_unitSystemType)
            {
                case UnitSystemType.UNIT_LESS:
                    _lengthUnit = (LengthUnit)MyUnit.NoUnit;
                    _angleUnit = (AngleUnit)MyUnit.NoUnit;
                    _massUnit = (MassUnit)MyUnit.NoUnit;
                    _timeUnit = (DurationUnit)MyUnit.NoUnit;
                    _temperatureUnit = (TemperatureUnit)MyUnit.NoUnit;
                    _temperatureDeltaUnit = (TemperatureDeltaUnit)MyUnit.NoUnit;
                    //
                    _areaUnit = (AreaUnit)MyUnit.NoUnit;
                    _volumeUnit = (VolumeUnit)MyUnit.NoUnit;
                    _speedUnit = (SpeedUnit)MyUnit.NoUnit;
                    _rotationalSpeedUnit = (RotationalSpeedUnit)MyUnit.NoUnit;
                    _accelerationUnit = (AccelerationUnit)MyUnit.NoUnit;
                    _forceUnit = (ForceUnit)MyUnit.NoUnit;
                    _forcePerLengthUnit = (ForcePerLengthUnit)MyUnit.NoUnit;
                    _momentUnit = (TorqueUnit)MyUnit.NoUnit;
                    _pressureUnit = (PressureUnit)MyUnit.NoUnit;
                    _densityUnit = (DensityUnit)MyUnit.NoUnit;
                    _energyUnit = (EnergyUnit)MyUnit.NoUnit;
                    _powerUnit = (PowerUnit)MyUnit.NoUnit;
                    _frequencyUnit = (FrequencyUnit)MyUnit.NoUnit;
                    //
                    _thermalExpansionUnit = (CoefficientOfThermalExpansionUnit)MyUnit.NoUnit;
                    _thermalConductivityUnit = ThermalConductivityUnit.Undefined;
                    break;
                case UnitSystemType.Undefined:
                case UnitSystemType.M_KG_S_C:
                    _lengthUnit = LengthUnit.Meter;
                    _angleUnit = AngleUnit.Radian;
                    _massUnit = MassUnit.Kilogram;
                    _timeUnit = DurationUnit.Second;
                    _temperatureUnit = TemperatureUnit.DegreeCelsius;
                    _temperatureDeltaUnit = TemperatureDeltaUnit.DegreeCelsius;
                    //
                    _areaUnit = AreaUnit.SquareMeter;
                    _volumeUnit = VolumeUnit.CubicMeter;
                    _speedUnit = SpeedUnit.MeterPerSecond;
                    _rotationalSpeedUnit = RotationalSpeedUnit.RadianPerSecond;
                    _accelerationUnit = AccelerationUnit.MeterPerSecondSquared;
                    _forceUnit = ForceUnit.Newton;
                    _forcePerLengthUnit = ForcePerLengthUnit.NewtonPerMeter;
                    _momentUnit = TorqueUnit.NewtonMeter;
                    _pressureUnit = PressureUnit.Pascal;
                    _densityUnit = DensityUnit.KilogramPerCubicMeter;
                    _energyUnit = EnergyUnit.Joule;
                    _powerUnit = PowerUnit.Watt;
                    _frequencyUnit = FrequencyUnit.Hertz;
                    //
                    _thermalExpansionUnit = CoefficientOfThermalExpansionUnit.InverseDegreeCelsius;
                    _thermalConductivityUnit = ThermalConductivityUnit.Undefined;
                    break;
                case UnitSystemType.MM_TON_S_C:
                    _lengthUnit = LengthUnit.Millimeter;
                    _angleUnit = AngleUnit.Radian;
                    _massUnit = MassUnit.Tonne;
                    _timeUnit = DurationUnit.Second;
                    _temperatureUnit = TemperatureUnit.DegreeCelsius;
                    _temperatureDeltaUnit = TemperatureDeltaUnit.DegreeCelsius;
                    //
                    _areaUnit = AreaUnit.SquareMillimeter;
                    _volumeUnit = VolumeUnit.CubicMillimeter;
                    _speedUnit = SpeedUnit.MillimeterPerSecond;
                    _rotationalSpeedUnit = RotationalSpeedUnit.RadianPerSecond;
                    _accelerationUnit = AccelerationUnit.MillimeterPerSecondSquared;
                    _forceUnit = ForceUnit.Newton;
                    _forcePerLengthUnit = ForcePerLengthUnit.NewtonPerMillimeter;
                    _momentUnit = TorqueUnit.NewtonMillimeter;
                    _pressureUnit = PressureUnit.Megapascal;
                    _densityUnit = DensityUnit.TonnePerCubicMillimeter;
                    _energyUnit = EnergyUnit.Millijoule;
                    _powerUnit = PowerUnit.Milliwatt;
                    _frequencyUnit = FrequencyUnit.Hertz;
                    //
                    _thermalExpansionUnit = CoefficientOfThermalExpansionUnit.InverseDegreeCelsius;
                    _thermalConductivityUnit = ThermalConductivityUnit.Undefined;
                    break;
                case UnitSystemType.M_TON_S_C:
                    _lengthUnit = LengthUnit.Meter;
                    _angleUnit = AngleUnit.Radian;
                    _massUnit = MassUnit.Tonne;
                    _timeUnit = DurationUnit.Second;
                    _temperatureUnit = TemperatureUnit.DegreeCelsius;
                    _temperatureDeltaUnit = TemperatureDeltaUnit.DegreeCelsius;
                    //
                    _areaUnit = AreaUnit.SquareMeter;
                    _volumeUnit = VolumeUnit.CubicMeter;
                    _speedUnit = SpeedUnit.MeterPerSecond;
                    _rotationalSpeedUnit = RotationalSpeedUnit.RadianPerSecond;
                    _accelerationUnit = AccelerationUnit.MeterPerSecondSquared;
                    _forceUnit = ForceUnit.Kilonewton;
                    _forcePerLengthUnit = ForcePerLengthUnit.KilonewtonPerMeter;
                    _momentUnit = TorqueUnit.KilonewtonMeter;
                    _pressureUnit = PressureUnit.Kilopascal;
                    _densityUnit = DensityUnit.TonnePerCubicMeter;
                    _energyUnit = EnergyUnit.Kilojoule;
                    _powerUnit = PowerUnit.Kilowatt;
                    _frequencyUnit = FrequencyUnit.Hertz;
                    //
                    _thermalExpansionUnit = CoefficientOfThermalExpansionUnit.InverseDegreeCelsius;
                    _thermalConductivityUnit = ThermalConductivityUnit.Undefined;
                    break;
                case UnitSystemType.IN_LB_S_F:
                    _lengthUnit = LengthUnit.Inch;
                    _angleUnit = AngleUnit.Radian;
                    _massUnit = MassUnit.Pound;
                    _timeUnit = DurationUnit.Second;
                    _temperatureUnit = TemperatureUnit.DegreeFahrenheit;
                    _temperatureDeltaUnit = TemperatureDeltaUnit.DegreeFahrenheit;
                    //
                    _areaUnit = AreaUnit.SquareInch;
                    _volumeUnit = VolumeUnit.CubicInch;
                    _speedUnit = SpeedUnit.InchPerSecond;
                    _rotationalSpeedUnit = RotationalSpeedUnit.RadianPerSecond;
                    _accelerationUnit = AccelerationUnit.InchPerSecondSquared;
                    _forceUnit = ForceUnit.PoundForce;
                    _forcePerLengthUnit = ForcePerLengthUnit.PoundForcePerInch;
                    _momentUnit = TorqueUnit.PoundForceInch;
                    _pressureUnit = PressureUnit.PoundForcePerSquareInch;
                    _densityUnit = DensityUnit.PoundPerCubicInch;
                    _energyUnit = MyUnit.InchPound; // EnergyUnit.InchPound;
                    _powerUnit = MyUnit.InchPoundPerSecond;
                    _frequencyUnit = FrequencyUnit.Hertz;
                    //
                    _thermalExpansionUnit = CoefficientOfThermalExpansionUnit.InverseDegreeFahrenheit;
                    _thermalConductivityUnit = MyUnit.InchPoundPerSecondInchFahrenheit;
                    break;
                default:
                    break;
            }
            //
            SetConverterUnits();
        }
        // ISerialization
        public UnitSystem(SerializationInfo info, StreamingContext context)
            : this()
        {
            foreach (SerializationEntry entry in info)
            {
                bool isTemperatureDelataUnitDefined = false;    // compatibility for version 1.0.0
                bool isForcePerLengtUnitDefined = false;        // compatibility for version 0.9.0
                bool isPowerUnitDefined = false;                // compatibility for version 1.0.0
                bool isThermalExpansionUnitDefined = false;     // compatibility for version 1.0.0
                bool isThermalConductivityUnitDefined = false;  // compatibility for version 1.0.0
                switch (entry.Name)
                {
                    // Base units
                    case "_unitSystemType":
                        _unitSystemType = (UnitSystemType)entry.Value; break;
                    case "_lengthUnit":
                        _lengthUnit = (LengthUnit)entry.Value; break;
                    case "_angleUnit":
                        _angleUnit = (AngleUnit)entry.Value; break;
                    case "_massUnit":
                        _massUnit = (MassUnit)entry.Value; break;
                    case "_timeUnit":
                        _timeUnit = (DurationUnit)entry.Value; break;
                    case "_temperatureUnit":
                        _temperatureUnit = (TemperatureUnit)entry.Value; break;
                    case "_temperatureDeltaUnit":
                        _temperatureDeltaUnit = (TemperatureDeltaUnit)entry.Value;
                        isTemperatureDelataUnitDefined = true; break;
                    // Derived units
                    case "_areaUnit":
                        _areaUnit = (AreaUnit)entry.Value; break;
                    case "_volumeUnit":
                        _volumeUnit = (VolumeUnit)entry.Value; break;
                    case "_speedUnit":
                        _speedUnit = (SpeedUnit)entry.Value; break;
                    case "_rotationalSpeedUnit":
                        _rotationalSpeedUnit = (RotationalSpeedUnit)entry.Value; break;
                    case "_accelerationUnit":
                        _accelerationUnit = (AccelerationUnit)entry.Value; break;
                    case "_forceUnit":
                        _forceUnit = (ForceUnit)entry.Value; break;
                    case "_forcePerLengthUnit":
                        _forcePerLengthUnit = (ForcePerLengthUnit)entry.Value;
                        isForcePerLengtUnitDefined = true; break;
                    case "_momentUnit":
                        _momentUnit = (TorqueUnit)entry.Value; break;
                    case "_pressureUnit":
                        _pressureUnit = (PressureUnit)entry.Value; break;
                    case "_densityUnit":
                        _densityUnit = (DensityUnit)entry.Value; break;
                    case "_energyUnit":
                        _energyUnit = (EnergyUnit)entry.Value; break;
                    case "_powerUnit":
                        _powerUnit = (PowerUnit)entry.Value;
                        isPowerUnitDefined = true; break;
                    case "_frequencyUnit":
                        _frequencyUnit = (FrequencyUnit)entry.Value; break;
                    // Thermal units
                    case "_thermalExpansionUnit":
                        _thermalExpansionUnit = (CoefficientOfThermalExpansionUnit)entry.Value;
                        isThermalExpansionUnitDefined = true; break;
                    case "_thermalConductivityUnit":
                        _thermalConductivityUnit = (ThermalConductivityUnit)entry.Value;
                        isThermalConductivityUnitDefined = true; break;
                    default:
                        throw new NotSupportedException();
                }
                // Compatibility
                if (!isTemperatureDelataUnitDefined)
                {
                    UnitSystem system = new UnitSystem(_unitSystemType);
                    _temperatureDeltaUnit = system._temperatureDeltaUnit;
                }
                if (!isForcePerLengtUnitDefined)
                {
                    UnitSystem system = new UnitSystem(_unitSystemType);
                    _forcePerLengthUnit = system._forcePerLengthUnit;
                }
                if (!isPowerUnitDefined)
                {
                    UnitSystem system = new UnitSystem(_unitSystemType);
                    _powerUnit = system._powerUnit;
                }
                if (!isThermalExpansionUnitDefined)
                {
                    UnitSystem system = new UnitSystem(_unitSystemType);
                    _thermalExpansionUnit = system._thermalExpansionUnit;
                }
                if (!isThermalConductivityUnitDefined)
                {
                    UnitSystem system = new UnitSystem(_unitSystemType);
                    _thermalConductivityUnit = system._thermalConductivityUnit;
                }
            }
            //
            SetConverterUnits();
        }


        // Methods                                                                                                                  
        public void SetConverterUnits()
        {
            // Base units
            StringLengthConverter.SetUnit = LengthUnitAbbreviation;
            StringLengthDOFConverter.SetUnit = LengthUnitAbbreviation;
            StringLengthFixedDOFConverter.SetUnit = LengthUnitAbbreviation;
            StringLengthDefaultConverter.SetUnit = LengthUnitAbbreviation;
            StringAngleConverter.SetUnit = AngleUnitAbbreviation;
            StringAngleDOFConverter.SetUnit = AngleUnitAbbreviation;
            StringAngleFixedDOFConverter.SetUnit = AngleUnitAbbreviation;
            StringTimeConverter.SetUnit = TimeUnitAbbreviation;
            // Derived units
            StringAreaConverter.SetUnit = AreaUnitAbbreviation;
            StringVolumeConverter.SetUnit = VolumeUnitAbbreviation;
            StringRotationalSpeedConverter.SetUnit = RotationalSpeedUnitAbbreviation;
            StringAccelerationConverter.SetUnit = AccelerationUnitAbbreviation;
            StringForceConverter.SetUnit = ForceUnitAbbreviation;
            StringForcePerLenghtConverter.SetUnit = ForcePerLengthUnitAbbreviation;
            StringMomentConverter.SetUnit = MomentUnitAbbreviation;
            StringPressureConverter.SetUnit = PressureUnitAbbreviation;
            StringDensityConverter.SetUnit = DensityUnitAbbreviation;
            StringEnergyConverter.SetUnit = EnergyUnitAbbreviation;
            StringEnergyPerVolumeConverter.SetEnergyUnit = EnergyUnitAbbreviation;
            StringEnergyPerVolumeConverter.SetVolumeUnit = VolumeUnitAbbreviation;
            StringPowerConverter.SetUnit = PowerUnitAbbreviation;
            // Contact
            StringForcePerVolumeConverter.SetForceUnit = ForceUnitAbbreviation;
            StringForcePerVolumeConverter.SetVolumeUnit = VolumeUnitAbbreviation;
            StringForcePerVolumeDefaultConverter.SetForceUnit = ForceUnitAbbreviation;
            StringForcePerVolumeDefaultConverter.SetVolumeUnit = VolumeUnitAbbreviation;
            // Thermal
            StringThermalExpansionConverter.SetUnit = ThermalExpansionUnitAbbreviation;
            if (_thermalConductivityUnit == MyUnit.InchPoundPerSecondInchFahrenheit)
            {
                StringThermalConductivityConverter.SetUnit = MyUnit.InchPoundPerSecondInchFahrenheitAbbreviation;
            }
            else
            {
                StringThermalConductivityConverter.SetPowerUnit = PowerUnitAbbreviation;
                StringThermalConductivityConverter.SetLengthUnit = LengthUnitAbbreviation;
                StringThermalConductivityConverter.SetTemperatureDeltaUnit = TemperatureDeltaUnitAbbreviation;
            }
            StringSpecificHeatConverter.SetEnergyUnit = EnergyUnitAbbreviation;
            StringSpecificHeatConverter.SetMassUnit = MassUnitAbbreviation;
            StringSpecificHeatConverter.SetTemperatureDeltaUnit = TemperatureDeltaUnitAbbreviation;
        }
        public double Convert(double value, TypeConverter converter, UnitSystem toSystem)
        {
            // Use this method to allow for added units like: Energy: in.lb
            SetConverterUnits();
            string valueWithUnit = converter.ConvertToString(value);
            toSystem.SetConverterUnits();
            double result = (double)converter.ConvertFrom(valueWithUnit);
            SetConverterUnits();
            return result;
        }
        // ISerialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // using typeof() works also for null fields
            info.AddValue("_unitSystemType", _unitSystemType, typeof(UnitSystemType));
            // Base units
            info.AddValue("_lengthUnit", _lengthUnit, typeof(LengthUnit));
            info.AddValue("_angleUnit", _angleUnit, typeof(AngleUnit));
            info.AddValue("_massUnit", _massUnit, typeof(MassUnit));
            info.AddValue("_timeUnit", _timeUnit, typeof(DurationUnit));
            info.AddValue("_temperatureUnit", _temperatureUnit, typeof(TemperatureUnit));
            info.AddValue("_temperatureDeltaUnit", _temperatureDeltaUnit, typeof(TemperatureDeltaUnit));
            // Derived units
            info.AddValue("_areaUnit", _areaUnit, typeof(AreaUnit));
            info.AddValue("_volumeUnit", _volumeUnit, typeof(VolumeUnit));
            info.AddValue("_speedUnit", _speedUnit, typeof(SpeedUnit));
            info.AddValue("_rotationalSpeedUnit", _rotationalSpeedUnit, typeof(RotationalSpeedUnit));
            info.AddValue("_accelerationUnit", _accelerationUnit, typeof(AccelerationUnit));
            info.AddValue("_forceUnit", _forceUnit, typeof(ForceUnit));
            info.AddValue("_forcePerLengthUnit", _forcePerLengthUnit, typeof(ForcePerLengthUnit));
            info.AddValue("_momentUnit", _momentUnit, typeof(TorqueUnit));
            info.AddValue("_pressureUnit", _pressureUnit, typeof(PressureUnit));
            info.AddValue("_densityUnit", _densityUnit, typeof(DensityUnit));
            info.AddValue("_energyUnit", _energyUnit, typeof(EnergyUnit));
            info.AddValue("_powerUnit", _powerUnit, typeof(EnergyUnit));
            info.AddValue("_frequencyUnit", _frequencyUnit, typeof(FrequencyUnit));
            // thermal units
            info.AddValue("_thermalExpansionUnit", _thermalExpansionUnit, typeof(CoefficientOfThermalExpansionUnit));
            info.AddValue("_thermalConductivityUnit", _thermalConductivityUnit, typeof(ThermalConductivityUnit));
        }
    }
}
