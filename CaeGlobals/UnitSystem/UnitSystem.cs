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
        [Description("in, lb, s, °C")]
        IN_LB_S_C = 20,
        //
        [Description("Unitless")]
        UNIT_LESS = 40
    }


    [Serializable]
    public class UnitSystem : ISerializable
    {
        // Variables                                                                                                                
        UnitSystemType _unitSystemType;             //ISerializable
        // Base units
        LengthUnit _lengthUnit;                     //ISerializable
        AngleUnit _angleUnit;                       //ISerializable
        MassUnit _massUnit;                         //ISerializable
        DurationUnit _timeUnit;                     //ISerializable
        TemperatureUnit _temperatureUnit;           //ISerializable
        // Derived units
        AreaUnit _areaUnit;                         //ISerializable
        VolumeUnit _volumeUnit;                     //ISerializable
        SpeedUnit _speedUnit;                       //ISerializable
        RotationalSpeedUnit _rotationalSpeedUnit;   //ISerializable
        AccelerationUnit _accelerationUnit;         //ISerializable
        ForceUnit _forceUnit;                       //ISerializable
        TorqueUnit _momentUnit;                     //ISerializable
        PressureUnit _pressureUnit;                 //ISerializable
        DensityUnit _densityUnit;                   //ISerializable
        EnergyUnit _energyUnit;                     //ISerializable
        FrequencyUnit _frequencyUnit;               //ISerializable


        // Properties                                                                                                               
        public UnitSystemType UnitSystemType { get { return _unitSystemType; } }
        //// Base units
        //public LengthUnit LengthUnit { get { return _lengthUnit; } }
        //public AngleUnit AngleUnit { get { return _angleUnit; } }
        //public MassUnit MassUnit { get { return _massUnit; } }
        //public DurationUnit TimeUnit { get { return _timeUnit; } }
        //public TemperatureUnit TemperatureUnit { get { return _temperatureUnit; } }
        //// Derived units
        //public AreaUnit AreaUnit { get { return _areaUnit; } }
        //public VolumeUnit VolumeUnit { get { return _volumeUnit; } }
        //public SpeedUnit SpeedUnit { get { return _speedUnit; } }
        //public RotationalSpeedUnit RotationalSpeedUnit { get { return _rotationalSpeedUnit; } }
        //public AccelerationUnit AccelerationUnit { get { return _accelerationUnit; } }
        //public ForceUnit ForceUnit { get { return _forceUnit; } }
        //public TorqueUnit MomentUnit { get { return _momentUnit; } }
        //public PressureUnit PressureUnit { get { return _pressureUnit; } }
        //public DensityUnit DensityUnit { get { return _densityUnit; } }
        //public EnergyUnit EnergyUnit { get { return _energyUnit; } }
        //public FrequencyUnit FrequencyUnit { get { return _frequencyUnit; } }
        //
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
                if ((int)_energyUnit == MyUnit.NoUnit) return "";
                else if (_energyUnit == MyUnit.InchPound) return "in·lb";
                else return Energy.GetAbbreviation(_energyUnit);
            }
        }
        public string EnergyPerVolumeUnitAbbreviation
        {
            get
            {
                if ((int)_energyUnit == MyUnit.NoUnit || (int)_volumeUnit == MyUnit.NoUnit) return "";
                else return StringEnergyPerVolumeConverter.GetUnitAbbreviation();
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
                    //
                    _areaUnit = (AreaUnit)MyUnit.NoUnit;
                    _volumeUnit = (VolumeUnit)MyUnit.NoUnit;
                    _speedUnit = (SpeedUnit)MyUnit.NoUnit;
                    _rotationalSpeedUnit = (RotationalSpeedUnit)MyUnit.NoUnit;
                    _accelerationUnit = (AccelerationUnit)MyUnit.NoUnit;
                    _forceUnit = (ForceUnit)MyUnit.NoUnit;
                    _momentUnit = (TorqueUnit)MyUnit.NoUnit;
                    _pressureUnit = (PressureUnit)MyUnit.NoUnit;
                    _densityUnit = (DensityUnit)MyUnit.NoUnit;
                    _energyUnit = (EnergyUnit)MyUnit.NoUnit;
                    _frequencyUnit = (FrequencyUnit)MyUnit.NoUnit;
                    break;
                case UnitSystemType.Undefined:
                case UnitSystemType.M_KG_S_C:
                    _lengthUnit = LengthUnit.Meter;
                    _angleUnit = AngleUnit.Radian;
                    _massUnit = MassUnit.Kilogram;
                    _timeUnit = DurationUnit.Second;
                    _temperatureUnit = TemperatureUnit.DegreeCelsius;
                    //
                    _areaUnit = AreaUnit.SquareMeter;
                    _volumeUnit = VolumeUnit.CubicMeter;
                    _speedUnit = SpeedUnit.MeterPerSecond;
                    _rotationalSpeedUnit = RotationalSpeedUnit.RadianPerSecond;
                    _accelerationUnit = AccelerationUnit.MeterPerSecondSquared;
                    _forceUnit = ForceUnit.Newton;
                    _momentUnit = TorqueUnit.NewtonMeter;
                    _pressureUnit = PressureUnit.Pascal;
                    _densityUnit = DensityUnit.KilogramPerCubicMeter;
                    _energyUnit = EnergyUnit.Joule;
                    _frequencyUnit = FrequencyUnit.Hertz;
                    break;
                case UnitSystemType.MM_TON_S_C:
                    _lengthUnit = LengthUnit.Millimeter;
                    _angleUnit = AngleUnit.Radian;
                    _massUnit = MassUnit.Tonne;
                    _timeUnit = DurationUnit.Second;
                    _temperatureUnit = TemperatureUnit.DegreeCelsius;
                    //
                    _areaUnit = AreaUnit.SquareMillimeter;
                    _volumeUnit = VolumeUnit.CubicMillimeter;
                    _speedUnit = SpeedUnit.MillimeterPerSecond;
                    _rotationalSpeedUnit = RotationalSpeedUnit.RadianPerSecond;
                    _accelerationUnit = AccelerationUnit.MillimeterPerSecondSquared;
                    _forceUnit = ForceUnit.Newton;
                    _momentUnit = TorqueUnit.NewtonMillimeter;
                    _pressureUnit = PressureUnit.Megapascal;
                    _densityUnit = DensityUnit.TonnePerCubicMillimeter;
                    _energyUnit = EnergyUnit.Millijoule;
                    _frequencyUnit = FrequencyUnit.Hertz;
                    break;
                case UnitSystemType.M_TON_S_C:
                    _lengthUnit = LengthUnit.Meter;
                    _angleUnit = AngleUnit.Radian;
                    _massUnit = MassUnit.Tonne;
                    _timeUnit = DurationUnit.Second;
                    _temperatureUnit = TemperatureUnit.DegreeCelsius;
                    //
                    _areaUnit = AreaUnit.SquareMeter;
                    _volumeUnit = VolumeUnit.CubicMeter;
                    _speedUnit = SpeedUnit.MeterPerSecond;
                    _rotationalSpeedUnit = RotationalSpeedUnit.RadianPerSecond;
                    _accelerationUnit = AccelerationUnit.MeterPerSecondSquared;
                    _forceUnit = ForceUnit.Kilonewton;
                    _momentUnit = TorqueUnit.KilonewtonMeter;
                    _pressureUnit = PressureUnit.Kilopascal;
                    _densityUnit = DensityUnit.TonnePerCubicMeter;
                    _energyUnit = EnergyUnit.Kilojoule;
                    _frequencyUnit = FrequencyUnit.Hertz;
                    break;
                case UnitSystemType.IN_LB_S_C:
                    _lengthUnit = LengthUnit.Inch;
                    _angleUnit = AngleUnit.Radian;
                    _massUnit = MassUnit.Pound;
                    _timeUnit = DurationUnit.Second;
                    _temperatureUnit = TemperatureUnit.DegreeCelsius;
                    //
                    _areaUnit = AreaUnit.SquareInch;
                    _volumeUnit = VolumeUnit.CubicInch;
                    _speedUnit = SpeedUnit.InchPerSecond;
                    _rotationalSpeedUnit = RotationalSpeedUnit.RadianPerSecond;
                    _accelerationUnit = AccelerationUnit.InchPerSecondSquared;
                    _forceUnit = ForceUnit.PoundForce;
                    _momentUnit = TorqueUnit.PoundForceInch;
                    _pressureUnit = PressureUnit.PoundForcePerSquareInch;
                    _densityUnit = DensityUnit.PoundPerCubicInch;
                    _energyUnit = MyUnit.InchPound; // EnergyUnit.InchPound;
                    _frequencyUnit = FrequencyUnit.Hertz;
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
                    case "_momentUnit":
                        _momentUnit = (TorqueUnit)entry.Value; break;
                    case "_pressureUnit":
                        _pressureUnit = (PressureUnit)entry.Value; break;
                    case "_densityUnit":
                        _densityUnit = (DensityUnit)entry.Value; break;
                    case "_energyUnit":
                        _energyUnit = (EnergyUnit)entry.Value; break;
                    case "_frequencyUnit":
                        _frequencyUnit = (FrequencyUnit)entry.Value; break;
                    default:
                        throw new NotSupportedException();
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
            StringMomentConverter.SetUnit = MomentUnitAbbreviation;
            StringPressureConverter.SetUnit = PressureUnitAbbreviation;
            StringPressureFromConverter.SetUnit = PressureUnitAbbreviation;     // not really necessary
            StringDensityConverter.SetUnit = DensityUnitAbbreviation;
            StringEnergyConverter.SetUnit = EnergyUnitAbbreviation;
            StringEnergyPerVolumeConverter.SetEnergyUnit = EnergyUnitAbbreviation;
            StringEnergyPerVolumeConverter.SetVolumeUnit = VolumeUnitAbbreviation;
            // Contact
            StringForcePerVolumeConverter.SetForceUnit = ForceUnitAbbreviation;
            StringForcePerVolumeConverter.SetVolumeUnit = VolumeUnitAbbreviation;
            StringForcePerVolumeDefaultConverter.SetForceUnit = ForceUnitAbbreviation;
            StringForcePerVolumeDefaultConverter.SetVolumeUnit = VolumeUnitAbbreviation;
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
            // Derived units
            info.AddValue("_areaUnit", _areaUnit, typeof(AreaUnit));
            info.AddValue("_volumeUnit", _volumeUnit, typeof(VolumeUnit));
            info.AddValue("_speedUnit", _speedUnit, typeof(SpeedUnit));
            info.AddValue("_rotationalSpeedUnit", _rotationalSpeedUnit, typeof(RotationalSpeedUnit));
            info.AddValue("_accelerationUnit", _accelerationUnit, typeof(AccelerationUnit));
            info.AddValue("_forceUnit", _forceUnit, typeof(ForceUnit));
            info.AddValue("_momentUnit", _momentUnit, typeof(TorqueUnit));
            info.AddValue("_pressureUnit", _pressureUnit, typeof(PressureUnit));
            info.AddValue("_densityUnit", _densityUnit, typeof(DensityUnit));
            info.AddValue("_energyUnit", _energyUnit, typeof(EnergyUnit));
            info.AddValue("_frequencyUnit", _frequencyUnit, typeof(FrequencyUnit));
        }
    }
}
