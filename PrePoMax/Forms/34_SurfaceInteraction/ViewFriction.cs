using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicTypeDescriptor;
using System.ComponentModel;
using CaeGlobals;

namespace PrePoMax.PropertyViews
{
    [Serializable]
    public class ViewFriction : ViewSurfaceInteractionProperty
    {
        // Variables                                                                                                                
        private CaeModel.Friction _friction;


        // Properties                                                                                                               
        [Browsable(false)]
        public override string Name
        {
            get { return "Friction"; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Friction coefficient")]
        [DescriptionAttribute("The friction coefficient (µ > 0).")]
        public double Coefficient { get { return _friction.Coefficient; } set { _friction.Coefficient = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Stik slope")]
        [DescriptionAttribute("The ratio between the shear stress and the relative tangential displacement in contact (λ > 0).")]
        [TypeConverter(typeof(CaeGlobals.StringForcePerVolumeDefaultConverter))]
        public double StikSlope { get { return _friction.StikSlope; } set { _friction.StikSlope = value; } }
        //
        [Browsable(false)]
        public override CaeModel.SurfaceInteractionProperty Base
        {
            get { return _friction; }
        }


        // Constructors                                                                                                             
        public ViewFriction(CaeModel.Friction friction)
        {
            _friction = friction;
            //
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            //
            CaeGlobals.StringForcePerVolumeDefaultConverter.SetInitialValue = "100000 N/mm^3";
        }


        // Methods                                                                                                                  
    }
}
