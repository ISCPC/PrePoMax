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
    public class ViewStaticStep : ViewStep
    {
        // Variables                                                                                                                
        private CaeModel.StaticStep _step;



        // Properties                                                                                                               
        public override string Name { get { return _step.Name; } set { _step.Name = value; } }       

        [CategoryAttribute("Data")]
        [ReadOnly(false)]
        [OrderedDisplayName(1, 10, "Nlgeom")]
        [DescriptionAttribute("Enable/disable the nonlinear effects of large deformations and large displacements.")]
        public bool Nlgeom 
        { 
            get { return _step.Nlgeom; } 
            set 
            { 
                _step.Nlgeom = value;
                //if (!_step.Nlgeom) _step.Direct = false;
            } 
        }

        [CategoryAttribute("Data")]
        [ReadOnly(false)]
        [OrderedDisplayName(2, 10, "Incrementation")]
        [DescriptionAttribute("Select the incrementation type.")]
        public CaeModel.IncrementationTypeEnum IncrementationType
        { 
            get { return _step.IncrementationType; }
            set
            {
                _step.IncrementationType = value;
                _step.Direct = _step.IncrementationType == CaeModel.IncrementationTypeEnum.Direct;
            }
        }
        



        [CategoryAttribute("Incrementation")]
        [ReadOnly(false)]
        [OrderedDisplayName(0, 10, "Direct")]
        [DescriptionAttribute("By using the 'Direct' keyword automatic incrementation of nonlinear step is switched off.")]
        public bool Direct { get { return _step.Direct; } set { _step.Direct = value; } }

        [CategoryAttribute("Incrementation")]
        [ReadOnly(false)]
        [OrderedDisplayName(1, 10, "Max increments")]
        [DescriptionAttribute("The maximum number of increments in the step.")]
        public int MaxIncrements { get { return _step.MaxIncrements; } set { _step.MaxIncrements = value; } }

        [CategoryAttribute("Incrementation")]
        [ReadOnly(false)]
        [OrderedDisplayName(2, 10, "Time period")]
        [DescriptionAttribute("Time period of the step.")]
        [TypeConverter(typeof(CaeGlobals.StringTimeConverter))]
        public double TimePeriod { get { return _step.TimePeriod; } set { _step.TimePeriod = value; } }
       
        [CategoryAttribute("Incrementation")]
        [ReadOnly(false)]
        [OrderedDisplayName(3, 10, "Initial time increment")]
        [DescriptionAttribute("Initial time increment of the step.")]
        [TypeConverter(typeof(CaeGlobals.StringTimeConverter))]
        public double InitialTimeIncrement { get { return _step.InitialTimeIncrement; } set { _step.InitialTimeIncrement = value; } }

        [CategoryAttribute("Incrementation")]
        [ReadOnly(false)]
        [OrderedDisplayName(4, 10, "Min time increment")]
        [DescriptionAttribute("Minimum time increment allowed.")]
        [TypeConverter(typeof(CaeGlobals.StringTimeConverter))]
        public double MinTimeIncrement { get { return _step.MinTimeIncrement; } set { _step.MinTimeIncrement = value; } }

        [CategoryAttribute("Incrementation")]
        [ReadOnly(false)]
        [OrderedDisplayName(5, 10, "Max time increment")]
        [DescriptionAttribute("Maximum time increment allowed.")]
        [TypeConverter(typeof(CaeGlobals.StringTimeConverter))]
        public double MaxTimeIncrement { get { return _step.MaxTimeIncrement; } set { _step.MaxTimeIncrement = value; } }


        public override CaeModel.Step Base { get { return _step; } set { _step = (CaeModel.StaticStep)value; } }



        // Constructors                                                                                                             
        public ViewStaticStep(CaeModel.StaticStep step)
        {
            if (step == null)
                throw new ArgumentNullException();
            //
            _step = step;
            _dctd = ProviderInstaller.Install(this);
            //
            _dctd.RenameBooleanPropertyToOnOff(nameof(Nlgeom));
            _dctd.RenameBooleanPropertyToOnOff(nameof(Direct));
            //
            UpdateFieldView();
        }


        // Methods
        public override void UpdateFieldView()
        {
            _dctd.GetProperty(nameof(Direct)).SetIsBrowsable(false);
            //
            if (_step.IncrementationType == CaeModel.IncrementationTypeEnum.Default)
            {
                _dctd.GetProperty(nameof(TimePeriod)).SetIsBrowsable(false);
                _dctd.GetProperty(nameof(MaxIncrements)).SetIsBrowsable(false);
                _dctd.GetProperty(nameof(InitialTimeIncrement)).SetIsBrowsable(false);
                _dctd.GetProperty(nameof(MinTimeIncrement)).SetIsBrowsable(false);
                _dctd.GetProperty(nameof(MaxTimeIncrement)).SetIsBrowsable(false);
            }
            else if (_step.IncrementationType == CaeModel.IncrementationTypeEnum.Automatic)
            {
                _dctd.GetProperty(nameof(TimePeriod)).SetIsBrowsable(true);
                _dctd.GetProperty(nameof(MaxIncrements)).SetIsBrowsable(true);
                _dctd.GetProperty(nameof(InitialTimeIncrement)).SetIsBrowsable(true);
                _dctd.GetProperty(nameof(MinTimeIncrement)).SetIsBrowsable(true);
                _dctd.GetProperty(nameof(MaxTimeIncrement)).SetIsBrowsable(true);
            }
            else if (_step.IncrementationType == CaeModel.IncrementationTypeEnum.Direct)
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
