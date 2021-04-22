using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;


namespace PrePoMax
{
    [Serializable]
    public class ViewUnitSystem
    {
        // Variables                                                                                                                
        private UnitSystem _unitSystem;


        // Properties                                                                                                               
        [CategoryAttribute("Base units")]
        [OrderedDisplayName(0, 10, "Length unit")]
        [DescriptionAttribute("Default length unit.")]
        [Id(1, 1)]
        public string LengthUnitAbbreviation { get { return _unitSystem.LengthUnitAbbreviation; } }
        [CategoryAttribute("Base units")]
        [OrderedDisplayName(1, 10, "Angle unit")]
        [DescriptionAttribute("Default angle unit.")]
        [Id(2, 1)]
        public string AngleUnitAbbreviation { get { return _unitSystem.AngleUnitAbbreviation; } }
        //
        [CategoryAttribute("Base units")]
        [OrderedDisplayName(2, 10, "Mass unit")]
        [DescriptionAttribute("Default mass unit.")]
        [Id(3, 1)]
        public string MassUnitAbbreviation { get { return _unitSystem.MassUnitAbbreviation; } }
        //
        [CategoryAttribute("Base units")]
        [OrderedDisplayName(3, 10, "Time unit")]
        [DescriptionAttribute("Default time unit.")]
        [Id(4, 1)]
        public string TimeUnitAbbreviation { get { return _unitSystem.TimeUnitAbbreviation; } }
        //
        [CategoryAttribute("Base units")]
        [OrderedDisplayName(4, 10, "Temperature unit")]
        [DescriptionAttribute("Default temperature unit.")]
        [Id(5, 1)]
        public string TemperatureUnitAbbreviation { get { return _unitSystem.TemperatureUnitAbbreviation; } }
        // Derived units
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(0, 20, "Area unit")]
        [DescriptionAttribute("Default area unit.")]
        [Id(1, 1)]
        public string AreaUnitAbbreviation { get { return _unitSystem.AreaUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(1, 20, "Volume unit")]
        [DescriptionAttribute("Default volume unit.")]
        [Id(2, 1)]
        public string VolumeUnitAbbreviation { get { return _unitSystem.VolumeUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(2, 20, "Velocity unit")]
        [DescriptionAttribute("Default velocity unit.")]
        [Id(3, 1)]
        public string SpeedUnitAbbreviation { get { return _unitSystem.SpeedUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(3, 20, "Angular velocity unit")]
        [DescriptionAttribute("Default angular velocity unit.")]
        [Id(4, 1)]
        public string RotationalSpeedUnitAbbreviation { get { return _unitSystem.RotationalSpeedUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(4, 20, "Acceleration unit")]
        [DescriptionAttribute("Default acceleration unit.")]
        [Id(5, 1)]
        public string AccelerationUnitAbbreviation { get { return _unitSystem.AccelerationUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(5, 20, "Force unit")]
        [DescriptionAttribute("Default force unit.")]
        [Id(6, 1)]
        public string ForceUnitAbbreviation { get { return _unitSystem.ForceUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(6, 20, "Force per length unit")]
        [DescriptionAttribute("Default force per length unit.")]
        [Id(7, 1)]
        public string ForcePerLengthUnitAbbreviation { get { return _unitSystem.ForcePerLengthUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(7, 20, "Moment unit")]
        [DescriptionAttribute("Default moment unit.")]
        [Id(8, 1)]
        public string MomentUnitAbbreviation { get { return _unitSystem.MomentUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(8, 20, "Pressure unit")]
        [DescriptionAttribute("Default pressure unit.")]
        [Id(9, 1)]
        public string PressureUnitAbbreviation { get { return _unitSystem.PressureUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(9, 20, "Density unit")]
        [DescriptionAttribute("Default density unit.")]
        [Id(10, 1)]
        public string DensityUnitAbbreviation { get { return _unitSystem.DensityUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(10, 20, "Energy unit")]
        [DescriptionAttribute("Default energy unit.")]
        [Id(11, 1)]
        public string EnergyUnitAbbreviation { get { return _unitSystem.EnergyUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(11, 20, "Frequency")]
        [DescriptionAttribute("Default frequency unit.")]
        [Id(12, 1)]
        public string FrequencyUnitAbbreviation { get { return _unitSystem.FrequencyUnitAbbreviation; } }
        //
        [CategoryAttribute("Derived units")]
        [OrderedDisplayName(12, 20, "Thermal expansion coefficient")]
        [DescriptionAttribute("Default thermal expansion coefficient.")]
        [Id(13, 1)]
        public string ThermalExpansion { get { return _unitSystem.ThermalExpansionUnitAbbreviation; } }


        // Constructors                                                                                                             
        public ViewUnitSystem(UnitSystem unitSystem)
        {
            _unitSystem = unitSystem;
        }


        // Methods                                                                                                                  
        public UnitSystem GetBase()
        {
            return _unitSystem;
        }
    }

}
