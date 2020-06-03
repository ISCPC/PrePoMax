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

namespace CaeGlobals
{
    [Serializable]
    public enum UnitSystemType
    {
        Undefined,
        M_KG_S_C,
        MM_TON_S_C
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
        FrequencyUnit _frequencyUnit;               //ISerializable


        // Properties                                                                                                               
        public UnitSystemType UnitSystemType { get { return _unitSystemType; } }
        // Base units
        public LengthUnit LengthUnit { get { return _lengthUnit; } }
        public AngleUnit AngleUnit { get { return _angleUnit; } }
        public MassUnit MassUnit { get { return _massUnit; } }
        public DurationUnit TimeUnit { get { return _timeUnit; } }
        public TemperatureUnit TemperatureUnit { get { return _temperatureUnit; } }
        // Derived units
        public AreaUnit AreaUnit { get { return _areaUnit; } }
        public VolumeUnit VolumeUnit { get { return _volumeUnit; } }
        public SpeedUnit SpeedUnit { get { return _speedUnit; } }
        public RotationalSpeedUnit RotationalSpeedUnit { get { return _rotationalSpeedUnit; } }
        public AccelerationUnit AccelerationUnit { get { return _accelerationUnit; } }
        public ForceUnit ForceUnit { get { return _forceUnit; } }
        public TorqueUnit MomentUnit { get { return _momentUnit; } }
        public PressureUnit PressureUnit { get { return _pressureUnit; } }
        public DensityUnit DensityUnit { get { return _densityUnit; } }
        public FrequencyUnit FrequencyUnit { get { return _frequencyUnit; } }
        //
        // Abbreviations                                                                                
        //
        // Base units
        public string LengthUnitAbbreviation { get { return Length.GetAbbreviation(_lengthUnit); } }
        public string AngleUnitAbbreviation { get { return Angle.GetAbbreviation(_angleUnit); } }
        public string MassUnitAbbreviation { get { return Mass.GetAbbreviation(_massUnit); } }
        public string TimeUnitAbbreviation { get { return Duration.GetAbbreviation(_timeUnit); } }
        public string TemperatureUnitAbbreviation { get { return Temperature.GetAbbreviation(_temperatureUnit); } }
        // Derived units
        public string AreaUnitAbbreviation { get { return Area.GetAbbreviation(_areaUnit); } }
        public string VolumeUnitAbbreviation { get { return Volume.GetAbbreviation(_volumeUnit); } }
        public string SpeedUnitAbbreviation { get { return Speed.GetAbbreviation(_speedUnit); } }
        public string RotationalSpeedUnitAbbreviation { get { return RotationalSpeed.GetAbbreviation(_rotationalSpeedUnit); } }
        public string AccelerationUnitAbbreviation { get { return Acceleration.GetAbbreviation(_accelerationUnit); } }
        public string ForceUnitAbbreviation { get { return Force.GetAbbreviation(_forceUnit); } }
        public string MomentUnitAbbreviation { get { return Torque.GetAbbreviation(_momentUnit); } }
        public string PressureUnitAbbreviation { get { return Pressure.GetAbbreviation(_pressureUnit); } }
        public string DensityUnitAbbreviation { get { return UnitsNet.Density.GetAbbreviation(_densityUnit); } }
        public string FrequencyUnitAbbreviation { get { return Frequency.GetAbbreviation(_frequencyUnit); } }


        // Constructors                                                                                                             
        public UnitSystem()
            : this(UnitSystemType.MM_TON_S_C)
        {
        }
        public UnitSystem(UnitSystemType unitSystemType)
        {
            _unitSystemType = unitSystemType;
            //
            switch (_unitSystemType)
            {
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
            StringRotationalSpeedConverter.SetUnit = RotationalSpeedUnitAbbreviation;
            StringAccelerationConverter.SetUnit = AccelerationUnitAbbreviation;
            StringForceConverter.SetUnit = ForceUnitAbbreviation;
            StringMomentConverter.SetUnit = MomentUnitAbbreviation;
            StringPressureConverter.SetUnit = PressureUnitAbbreviation;
            StringPressureFromConverter.SetUnit = PressureUnitAbbreviation;     // not really necessary
            StringDensityConverter.SetUnit = DensityUnitAbbreviation;
            // Contact
            StringForcePerVolumeConverter.SetForceUnit = ForceUnitAbbreviation;
            StringForcePerVolumeConverter.SetVolumeUnit = VolumeUnitAbbreviation;
            StringForcePerVolumeDefaultConverter.SetForceUnit = ForceUnitAbbreviation;
            StringForcePerVolumeDefaultConverter.SetVolumeUnit = VolumeUnitAbbreviation;
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
            info.AddValue("_frequencyUnit", _frequencyUnit, typeof(FrequencyUnit));
        }
    }
}
