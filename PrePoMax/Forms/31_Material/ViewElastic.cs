using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DynamicTypeDescriptor;
using CaeGlobals;

namespace PrePoMax.PropertyViews
{
    [Serializable]
    public class ViewElastic : ViewMaterialProperty
    {
        // Variables                                                                                                                
        private CaeModel.Elastic _elastic;


        // Properties                                                                                                               
        public override string Name
        {
            get { return "Elastic"; }
        }

        [Browsable(false)]
        public override CaeModel.MaterialProperty Base
        {
            get { return _elastic; }
        }

        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 2, "Young's modulus")]
        [DescriptionAttribute("The value of the Young's modulus.")]
        [TypeConverter(typeof(CaeGlobals.StringPressureConverter))]
        public double YoungsModulus { get { return _elastic.YoungsModulus; } set { _elastic.YoungsModulus = value; } }

        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 2, "Poisson's ratio")]
        [DescriptionAttribute("The value of the Poisson's ratio.")]
        public double PoissonsRatio { get { return _elastic.PoissonsRatio; } set { _elastic.PoissonsRatio = value; } }


        // Constructors                                                                                                             
        public ViewElastic(CaeModel.Elastic elastic)
        {
            _elastic = elastic;
            //
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }

        // Methods                                                                                                                  
    }
}
