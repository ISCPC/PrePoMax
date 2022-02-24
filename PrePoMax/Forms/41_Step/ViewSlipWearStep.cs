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
    public class ViewSlipWearStep : ViewStaticStep
    {
        // Variables                                                                                                                
        private CaeModel.SlipWearStep _slipWearStep;



        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Nlgeom")]
        [DescriptionAttribute("Enable/disable the nonlinear effects of large deformations and large displacements.")]
        public bool Nlgeom { get { return _slipWearStep.Nlgeom; } set { _slipWearStep.Nlgeom = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(10, 10, "Incrementation")]
        [DescriptionAttribute("Select the incrementation type.")]
        public CaeModel.IncrementationTypeEnum IncrementationType
        { 
            get { return _slipWearStep.IncrementationType; }
            set
            {
                _slipWearStep.IncrementationType = value;
                _slipWearStep.Direct = _slipWearStep.IncrementationType == CaeModel.IncrementationTypeEnum.Direct;
            }
        }
        //
        [CategoryAttribute("Incrementation")]
        [OrderedDisplayName(0, 10, "Direct")]
        [DescriptionAttribute("By using the 'Direct' keyword automatic incrementation of nonlinear step is switched off.")]
        public bool Direct { get { return _slipWearStep.Direct; } set { _slipWearStep.Direct = value; } }
        //
        [CategoryAttribute("Incrementation")]
        [OrderedDisplayName(1, 10, "Max increments")]
        [DescriptionAttribute("The maximum number of increments in the step.")]
        public int MaxIncrements { get { return _slipWearStep.MaxIncrements; } set { _slipWearStep.MaxIncrements = value; } }
        //
        [CategoryAttribute("Incrementation")]
        [OrderedDisplayName(2, 10, "Time period")]
        [DescriptionAttribute("Time period of the step.")]
        [TypeConverter(typeof(StringTimeConverter))]
        public double TimePeriod { get { return _slipWearStep.TimePeriod; } set { _slipWearStep.TimePeriod = value; } }
        //
        [CategoryAttribute("Incrementation")]
        [OrderedDisplayName(3, 10, "Initial time increment")]
        [DescriptionAttribute("Initial time increment of the step.")]
        [TypeConverter(typeof(StringTimeConverter))]
        public double InitialTimeIncrement
        {
            get { return _slipWearStep.InitialTimeIncrement; }
            set { _slipWearStep.InitialTimeIncrement = value; }
        }
        //
        [CategoryAttribute("Incrementation")]
        [OrderedDisplayName(4, 10, "Min time increment")]
        [DescriptionAttribute("Minimum time increment allowed.")]
        [TypeConverter(typeof(StringTimeConverter))]
        public double MinTimeIncrement
        {
            get { return _slipWearStep.MinTimeIncrement; }
            set { _slipWearStep.MinTimeIncrement = value; }
        }
        //
        [CategoryAttribute("Incrementation")]
        [OrderedDisplayName(5, 10, "Max time increment")]
        [DescriptionAttribute("Maximum time increment allowed.")]
        [TypeConverter(typeof(StringTimeConverter))]
        public double MaxTimeIncrement
        {
            get { return _slipWearStep.MaxTimeIncrement; }
            set { _slipWearStep.MaxTimeIncrement = value; }
        }


        // Constructors                                                                                                             
        public ViewStaticStep(CaeModel.StaticStep step, bool installProvider = true)
            : base(step)
        {
            _slipWearStep = step;
            if (installProvider) InstallProvider();
        }


        // Methods
        public override CaeModel.Step GetBase()
        {
            return _slipWearStep;
        }
        protected void InstallProvider()
        {
            _dctd = ProviderInstaller.Install(this);
            //
            _dctd.RenameBooleanPropertyToOnOff(nameof(Nlgeom));
            _dctd.RenameBooleanPropertyToOnOff(nameof(Direct));
            //
            UpdateVisibility();
        }
        public override void UpdateVisibility()
        {
            _dctd.GetProperty(nameof(Direct)).SetIsBrowsable(false);
            //
            if (_slipWearStep.IncrementationType == CaeModel.IncrementationTypeEnum.Default)
            {
                _dctd.GetProperty(nameof(TimePeriod)).SetIsBrowsable(false);
                _dctd.GetProperty(nameof(MaxIncrements)).SetIsBrowsable(false);
                _dctd.GetProperty(nameof(InitialTimeIncrement)).SetIsBrowsable(false);
                _dctd.GetProperty(nameof(MinTimeIncrement)).SetIsBrowsable(false);
                _dctd.GetProperty(nameof(MaxTimeIncrement)).SetIsBrowsable(false);
            }
            else if (_slipWearStep.IncrementationType == CaeModel.IncrementationTypeEnum.Automatic)
            {
                _dctd.GetProperty(nameof(TimePeriod)).SetIsBrowsable(true);
                _dctd.GetProperty(nameof(MaxIncrements)).SetIsBrowsable(true);
                _dctd.GetProperty(nameof(InitialTimeIncrement)).SetIsBrowsable(true);
                _dctd.GetProperty(nameof(MinTimeIncrement)).SetIsBrowsable(true);
                _dctd.GetProperty(nameof(MaxTimeIncrement)).SetIsBrowsable(true);
            }
            else if (_slipWearStep.IncrementationType == CaeModel.IncrementationTypeEnum.Direct)
            {
                _dctd.GetProperty(nameof(TimePeriod)).SetIsBrowsable(true);
                _dctd.GetProperty(nameof(MaxIncrements)).SetIsBrowsable(true);
                _dctd.GetProperty(nameof(InitialTimeIncrement)).SetIsBrowsable(true);
                _dctd.GetProperty(nameof(MinTimeIncrement)).SetIsBrowsable(false);
                _dctd.GetProperty(nameof(MaxTimeIncrement)).SetIsBrowsable(false);
            }
            else throw new NotSupportedException();
        }

    }
}
