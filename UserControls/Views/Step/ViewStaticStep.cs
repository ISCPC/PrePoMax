using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;

namespace UserControls
{
    [Serializable]
    public class ViewStaticStep : ViewStep
    {
        // Variables                                                                                                                
        private CaeModel.StaticStep _step;


        // Properties                                                                                                               
        public override string Name { get { return _step.Name; } set { _step.Name = value; } }

        public override bool Nlgeom { get { return _step.Nlgeom; } set { _step.Nlgeom = value; } }

        [CategoryAttribute("Data")]
        [ReadOnly(false)]
        [OrderedDisplayName(2, 10, "Time period")]
        [DescriptionAttribute("Time period of the step..")]
        public double TimePeriod { get { return _step.TimePeriod; } set { _step.TimePeriod = value; } }

        [CategoryAttribute("Data")]
        [ReadOnly(false)]
        [OrderedDisplayName(3, 10, "Initial time increment")]
        [DescriptionAttribute("Initial time increment of the step.")]
        public double InitialTimeIncrement { get { return _step.InitialTimeIncrement; } set { _step.InitialTimeIncrement = value; } }

        public override CaeModel.Step Base { get { return _step; } set { _step = (CaeModel.StaticStep)value; } }

        // Constructors                                                                                                             
        public ViewStaticStep(CaeModel.StaticStep step)
        {
            _step = step;
        }

        // Methods

    }
}
