﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using CaeModel;
using System.Drawing.Design;
using CaeJob;

namespace PrePoMax
{
    [Serializable]
    public class ViewSteadyStateDynamics : ViewStep
    {
        // Variables                                                                                                                
        private SteadyStateDynamics _steadyStep;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(3, 10, "Harmonic")]
        [DescriptionAttribute("Select yes for harmonic periodic loading and no for nonharmonic periodic loading.")]
        [Id(4, 1)]
        public bool Harmonic
        {
            get { return _steadyStep.Harmonic; }
            set
            {
                _steadyStep.Harmonic = value;
                UpdateVisibility();
            }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(4, 10, "Lower frequency bound")]
        [DescriptionAttribute("Lower bound of the frequency range.")]
        [TypeConverter(typeof(StringFrequencyConverter))]
        [Id(5, 1)]
        public double FrequencyLower { get { return _steadyStep.FrequencyLower; } set { _steadyStep.FrequencyLower = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(5, 10, "Upper frequency bound")]
        [DescriptionAttribute("Upper bound of the frequency range.")]
        [TypeConverter(typeof(StringFrequencyConverter))]
        [Id(6, 1)]
        public double FrequencyUpper { get { return _steadyStep.FrequencyUpper; } set { _steadyStep.FrequencyUpper = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(6, 10, "Number of data points")]
        [DescriptionAttribute("Number of data points within the frequency range.")]
        [TypeConverter(typeof(StringIntegerConverter))]
        [Id(7, 1)]
        public int NumDataPoints { get { return _steadyStep.NumDataPoints; } set { _steadyStep.NumDataPoints = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(7, 10, "Bias")]
        [DescriptionAttribute("Distribution bias of the data points within the frequency range (use 1 for equal spacing).")]
        [TypeConverter(typeof(StringDoubleConverter))]
        [Id(8, 1)]
        public double Bias { get { return _steadyStep.Bias; } set { _steadyStep.Bias = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(8, 10, "Number of Fourier terms")]
        [DescriptionAttribute("Number of Fourier terms the nonharmonic loading is expanded in.")]
        [TypeConverter(typeof(StringIntegerConverter))]
        [Id(9, 1)]
        public int NumFourierTerms { get { return _steadyStep.NumFourierTerms; } set { _steadyStep.NumFourierTerms = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(9, 10, "Lower time bound")]
        [DescriptionAttribute("Lower bound of the time range.")]
        [TypeConverter(typeof(StringTimeConverter))]
        [Id(10, 1)]
        public double TimeLower { get { return _steadyStep.TimeLower; } set { _steadyStep.TimeLower = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(10, 10, "Upper time bound")]
        [DescriptionAttribute("Upper bound of the time range.")]
        [TypeConverter(typeof(StringTimeConverter))]
        [Id(11, 1)]
        public double TimeUpper { get { return _steadyStep.TimeUpper; } set { _steadyStep.TimeUpper = value; } }
        //
        [CategoryAttribute("Damping")]
        [OrderedDisplayName(0, 10, "Damping type")]
        [DescriptionAttribute("Select the modal damping type.")]
        [Id(1, 2)]
        public ModalDampingTypeEnum DampingType
        {
            get { return _steadyStep.ModalDamping.DampingType; }
            set
            {
                _steadyStep.ModalDamping.DampingType = value;
                UpdateVisibility();
            }
        }
        //
        [CategoryAttribute("Damping")]
        [OrderedDisplayName(1, 10, "Alpha")]
        [DescriptionAttribute("Stiffness-proportional damping coefficient of the Rayleigh damping.")]
        [TypeConverter(typeof(StringTimeConverter))]
        [Id(2, 2)]
        public double Alpha { get { return _steadyStep.ModalDamping.Alpha; } set { _steadyStep.ModalDamping.Alpha = value; } }
        //
        [CategoryAttribute("Damping")]
        [OrderedDisplayName(2, 10, "Beta")]
        [DescriptionAttribute("Mass-proportional damping coefficient of the Rayleigh damping.")]
        [TypeConverter(typeof(StringReciprocalTimeConverter))]
        [Id(3, 2)]
        public double Beta { get { return _steadyStep.ModalDamping.Beta; } set { _steadyStep.ModalDamping.Beta = value; } }
        //
        [CategoryAttribute("Damping")]
        [OrderedDisplayName(3, 10, "Damping ratio")]
        [DescriptionAttribute("Viscous damping ratio between the damping coefficient and the critical damping coefficient.")]
        [Id(4, 2)]
        public double ViscousDampingRatio
        {
            get { return _steadyStep.ModalDamping.ViscousDampingRatio; }
            set { _steadyStep.ModalDamping.ViscousDampingRatio = value; }
        }
        [CategoryAttribute("Damping")]
        [OrderedDisplayName(3, 10, "Damping ratios")]
        [DescriptionAttribute("Viscous damping ratios between the damping coefficient and the critical damping coefficient.")]
        [Editor(typeof(Forms.DampingRatiosAndValuesUIEditor), typeof(UITypeEditor))]
        [Id(4, 2)]
        public List<DampingRatioAndRange> DampingRatiosAndRanges
        {
            get { return _steadyStep.ModalDamping.DampingRatiosAndRanges; }
            set { _steadyStep.ModalDamping.DampingRatiosAndRanges = value; }
        }


        // Constructors                                                                                                             
        public ViewSteadyStateDynamics(SteadyStateDynamics step, bool installProvider = true)
            : base(step)
        {
            _steadyStep = step;
            //
            if (installProvider)
            {
                InstallProvider();
                UpdateVisibility();
            }
        }


        // Methods                                                                                                                  
        public override Step GetBase()
        {
            return _steadyStep;
        }
        public override void InstallProvider()
        {
            base.InstallProvider();
            //
            _dctd.RenameBooleanProperty(nameof(Harmonic), "Yes", "No");
        }
        public override void UpdateVisibility()
        {
            base.UpdateVisibility();
            //
            _dctd.GetProperty(nameof(NumFourierTerms)).SetIsBrowsable(!_steadyStep.Harmonic);
            _dctd.GetProperty(nameof(TimeLower)).SetIsBrowsable(!_steadyStep.Harmonic);
            _dctd.GetProperty(nameof(TimeUpper)).SetIsBrowsable(!_steadyStep.Harmonic);
            //
            //
            bool browsable = _steadyStep.ModalDamping.DampingType == ModalDampingTypeEnum.Constant;
            _dctd.GetProperty(nameof(ViscousDampingRatio)).SetIsBrowsable(browsable);
            //
            browsable = _steadyStep.ModalDamping.DampingType == ModalDampingTypeEnum.Direct;
            _dctd.GetProperty(nameof(DampingRatiosAndRanges)).SetIsBrowsable(browsable);
            //
            browsable = _steadyStep.ModalDamping.DampingType == ModalDampingTypeEnum.Rayleigh;
            _dctd.GetProperty(nameof(Alpha)).SetIsBrowsable(browsable);
            _dctd.GetProperty(nameof(Beta)).SetIsBrowsable(browsable);
        }
    }
}