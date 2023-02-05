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
    public class ViewSteadyStateDynamics : ViewStep
    {
        // Variables                                                                                                                
        private CaeModel.SteadyStateDynamics _steadyStep;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(3, 10, "Harmonic")]
        [DescriptionAttribute("Select yes for harmonic periodic loading and no for nonharmonic periodic loading.")]
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
        public double FrequencyLower { get { return _steadyStep.FrequencyLower; } set { _steadyStep.FrequencyLower = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(5, 10, "Upper frequency bound")]
        [DescriptionAttribute("Upper bound of the frequency range.")]
        [TypeConverter(typeof(StringFrequencyConverter))]
        public double FrequencyUpper { get { return _steadyStep.FrequencyUpper; } set { _steadyStep.FrequencyUpper = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(6, 10, "Number of data points")]
        [DescriptionAttribute("Number of data points within the frequency range.")]
        [TypeConverter(typeof(StringIntegerConverter))]
        public int NumDataPoints { get { return _steadyStep.NumDataPoints; } set { _steadyStep.NumDataPoints = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(7, 10, "Bias")]
        [DescriptionAttribute("Distribution bias of the data points within the frequency range (use 1 for equal spacing).")]
        [TypeConverter(typeof(StringDoubleConverter))]
        public double Bias { get { return _steadyStep.Bias; } set { _steadyStep.Bias = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(8, 10, "Number of Fourier terms")]
        [DescriptionAttribute("Number of Fourier terms the nonharmonic loading is expanded in.")]
        [TypeConverter(typeof(StringIntegerConverter))]
        public int NumFourierTerms { get { return _steadyStep.NumFourierTerms; } set { _steadyStep.NumFourierTerms = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(9, 10, "Lower time bound")]
        [DescriptionAttribute("Lower bound of the time range.")]
        [TypeConverter(typeof(StringTimeConverter))]
        public double TimeLower { get { return _steadyStep.TimeLower; } set { _steadyStep.TimeLower = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(9, 10, "Upper time bound")]
        [DescriptionAttribute("Upper bound of the time range.")]
        [TypeConverter(typeof(StringTimeConverter))]
        public double TimeUpper { get { return _steadyStep.TimeUpper; } set { _steadyStep.TimeUpper = value; } }




        // Constructors                                                                                                             
        public ViewSteadyStateDynamics(CaeModel.SteadyStateDynamics step, bool installProvider = true)
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
        public override CaeModel.Step GetBase()
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
            
        }
    }
}
