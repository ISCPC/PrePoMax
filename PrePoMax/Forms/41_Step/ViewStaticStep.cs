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

        [CategoryAttribute("Incrementation")]
        [ReadOnly(false)]
        [OrderedDisplayName(0, 10, "Max increments")]
        [DescriptionAttribute("The maximum number of increments in the step.")]
        public int MaxIncrements { get { return _step.MaxIncrements; } set { _step.MaxIncrements = value; } }

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
                if (!_step.Nlgeom) _step.Direct = false;
            } 
        }

        [CategoryAttribute("Data")]
        [ReadOnly(false)]
        [OrderedDisplayName(2, 10, "Time period")]
        [DescriptionAttribute("Time period of the step.")]
        public double TimePeriod { get { return _step.TimePeriod; } set { _step.TimePeriod = value; } }

        [CategoryAttribute("Data")]
        [ReadOnly(false)]
        [OrderedDisplayName(3, 10, "Direct")]
        [DescriptionAttribute("By using the 'Direct' keyword automatic incrementation of nonlinear step is switched off.")]
        public bool Direct { get { return _step.Direct; } set { _step.Direct = value; } }

        [CategoryAttribute("Incrementation")]
        [ReadOnly(false)]
        [OrderedDisplayName(1, 10, "Initial time increment")]
        [DescriptionAttribute("Initial time increment of the step.")]
        public double InitialTimeIncrement { get { return _step.InitialTimeIncrement; } set { _step.InitialTimeIncrement = value; } }

        [CategoryAttribute("Incrementation")]
        [ReadOnly(false)]
        [OrderedDisplayName(2, 10, "Min time increment")]
        [DescriptionAttribute("Minimum time increment allowed.")]
        public double MinTimeIncrement { get { return _step.MinTimeIncrement; } set { _step.MinTimeIncrement = value; } }

        [CategoryAttribute("Incrementation")]
        [ReadOnly(false)]
        [OrderedDisplayName(3, 10, "Max time increment")]
        [DescriptionAttribute("Maximum time increment allowed.")]
        public double MaxTimeIncrement { get { return _step.MaxTimeIncrement; } set { _step.MaxTimeIncrement = value; } }


        public override CaeModel.Step Base { get { return _step; } set { _step = (CaeModel.StaticStep)value; } }



        // Constructors                                                                                                             
        public ViewStaticStep(CaeModel.StaticStep step)
        {
            if (step == null)
                throw new ArgumentNullException();

            _step = step;
            _dctd = ProviderInstaller.Install(this);

            RenameTrueFalseForBooleanToOnOff("Nlgeom");
            RenameTrueFalseForBooleanToOnOff("Direct");

            UpdateFieldView();
        }


        // Methods
        public override void UpdateFieldView()
        {
            if (_step.Nlgeom)
            {
                _dctd.GetProperty("Direct").SetIsBrowsable(true);
                _dctd.GetProperty("TimePeriod").SetIsBrowsable(true);
                _dctd.GetProperty("MaxIncrements").SetIsBrowsable(true);
                _dctd.GetProperty("InitialTimeIncrement").SetIsBrowsable(true);

                if (_step.Direct)
                {
                    _dctd.GetProperty("MinTimeIncrement").SetIsBrowsable(false);
                    _dctd.GetProperty("MaxTimeIncrement").SetIsBrowsable(false);
                }
                else
                {
                    _dctd.GetProperty("MinTimeIncrement").SetIsBrowsable(true);
                    _dctd.GetProperty("MaxTimeIncrement").SetIsBrowsable(true);
                }
            }
            else
            {
                _dctd.GetProperty("Direct").SetIsBrowsable(false);
                _dctd.GetProperty("TimePeriod").SetIsBrowsable(false);
                _dctd.GetProperty("MaxIncrements").SetIsBrowsable(false);
                _dctd.GetProperty("InitialTimeIncrement").SetIsBrowsable(false);
                _dctd.GetProperty("MinTimeIncrement").SetIsBrowsable(false);
                _dctd.GetProperty("MaxTimeIncrement").SetIsBrowsable(false);
            }
            
        }

    }
}
