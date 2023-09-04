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
    public class ViewElasticWithDensity : ViewMaterialProperty
    {
        // Variables                                                                                                                
        private CaeModel.ElasticWithDensity _elasticWithDensity;


        // Properties                                                                                                               
        public override string Name
        {
            get { return "ElasticWithDensity"; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 5, "Young's modulus")]
        [DescriptionAttribute("The value of the Young's modulus.")]
        [TypeConverter(typeof(EquationPressureConverter))]
        public EquationString YoungsModulus
        {
            get { return _elasticWithDensity.YoungsModulus.Equation; }
            set { _elasticWithDensity.YoungsModulus.Equation = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 5, "Poisson's ratio")]
        [DescriptionAttribute("The value of the Poisson's ratio.")]
        [TypeConverter(typeof(EquationDoubleConverter))]
        public EquationString PoissonsRatio
        {
            get { return _elasticWithDensity.PoissonsRatio.Equation; }
            set { _elasticWithDensity.PoissonsRatio.Equation = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 5, "Density")]
        [DescriptionAttribute("The value of the density.")]
        [TypeConverter(typeof(EquationDensityConverter))]
        public EquationString Density
        {
            get { return _elasticWithDensity.Density.Equation; }
            set { _elasticWithDensity.Density.Equation = value; }
        }


        // Constructors                                                                                                             
        public ViewElasticWithDensity(CaeModel.ElasticWithDensity elasticWithDensity)
        {
            _elasticWithDensity = elasticWithDensity;
            //
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public override CaeModel.MaterialProperty GetBase()
        {
            return _elasticWithDensity;
        }
        public double GetYoungsModulusValue()
        {
            return _elasticWithDensity.YoungsModulus.Value;
        }
        public double GetDensityValue()
        {
            return _elasticWithDensity.Density.Value;
        }
    }
}
