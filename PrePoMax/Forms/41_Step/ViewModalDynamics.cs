using System;
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
    public class ViewModalDynamics : ViewStep
    {
        // Variables                                                                                                                
        private ModalDynamics _modalStep;


        // Properties                                                                                                               
        [CategoryAttribute("Damping")]
        [OrderedDisplayName(0, 10, "Damping type")]
        [DescriptionAttribute("Select the modal damping type.")]
        [Id(1, 2)]
        public ModalDampingTypeEnum DampingType
        {
            get { return _modalStep.ModalDamping.DampingType; }
            set
            {
                _modalStep.ModalDamping.DampingType = value;
                UpdateVisibility();
            }
        }
        //
        [CategoryAttribute("Damping")]
        [OrderedDisplayName(1, 10, "Alpha")]
        [DescriptionAttribute("Mass-proportional damping coefficient of the Rayleigh damping.")]
        [TypeConverter(typeof(StringReciprocalTimeConverter))]
        [Id(2, 2)]
        public double Alpha { get { return _modalStep.ModalDamping.Alpha; } set { _modalStep.ModalDamping.Alpha = value; } }
        //
        [CategoryAttribute("Damping")]
        [OrderedDisplayName(2, 10, "Beta")]
        [DescriptionAttribute("Stiffness-proportional damping coefficient of the Rayleigh damping.")]
        [TypeConverter(typeof(StringTimeConverter))]
        [Id(3, 2)]
        public double Beta { get { return _modalStep.ModalDamping.Beta; } set { _modalStep.ModalDamping.Beta = value; } }
        //
        [CategoryAttribute("Damping")]
        [OrderedDisplayName(3, 10, "Damping ratio")]
        [DescriptionAttribute("Viscous damping ratio between the damping coefficient and the critical damping coefficient.")]
        [Id(4, 2)]
        public double ViscousDampingRatio
        {
            get { return _modalStep.ModalDamping.ViscousDampingRatio; }
            set { _modalStep.ModalDamping.ViscousDampingRatio = value; }
        }
        [CategoryAttribute("Damping")]
        [OrderedDisplayName(3, 10, "Damping ratios")]
        [DescriptionAttribute("Viscous damping ratios between the damping coefficient and the critical damping coefficient.")]
        [Editor(typeof(Forms.DampingRatiosAndValuesUIEditor), typeof(UITypeEditor))]
        [Id(4, 2)]
        public List<DampingRatioAndRange> DampingRatiosAndRanges
        {
            get { return _modalStep.ModalDamping.DampingRatiosAndRanges; }
            set { _modalStep.ModalDamping.DampingRatiosAndRanges = value; }
        }


        // Constructors                                                                                                             
        public ViewModalDynamics(ModalDynamics step, bool installProvider = true)
            : base(step)
        {
            _modalStep = step;
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
            return _modalStep;
        }
        public override void InstallProvider()
        {
            base.InstallProvider();
        }
        public override void UpdateVisibility()
        {
            base.UpdateVisibility();
            //
            bool browsable = _modalStep.ModalDamping.DampingType == ModalDampingTypeEnum.Constant;
            _dctd.GetProperty(nameof(ViscousDampingRatio)).SetIsBrowsable(browsable);
            //
            browsable = _modalStep.ModalDamping.DampingType == ModalDampingTypeEnum.Direct;
            _dctd.GetProperty(nameof(DampingRatiosAndRanges)).SetIsBrowsable(browsable);
            //
            browsable = _modalStep.ModalDamping.DampingType == ModalDampingTypeEnum.Rayleigh;
            _dctd.GetProperty(nameof(Alpha)).SetIsBrowsable(browsable);
            _dctd.GetProperty(nameof(Beta)).SetIsBrowsable(browsable);
        }
    }
}
