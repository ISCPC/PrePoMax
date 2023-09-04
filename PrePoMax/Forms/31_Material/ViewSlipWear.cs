using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DynamicTypeDescriptor;
using CaeGlobals;
using CaeModel;

namespace PrePoMax
{
    [Serializable]
    public class ViewSlipWear : ViewMaterialProperty
    {
        // Variables                                                                                                                
        private SlipWear _slipWear;


        // Properties                                                                                                               
        public override string Name
        {
            get { return "Slip Wear"; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Hardness")]
        [DescriptionAttribute("The value of the surface hardness.")]
        [TypeConverter(typeof(EquationPressureConverter))]
        public EquationString Hardness { get { return _slipWear.Hardness.Equation; } set { _slipWear.Hardness.Equation = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Wear coefficient")]
        [DescriptionAttribute("The value of the wear coefficient.")]
        [TypeConverter(typeof(EquationDoubleConverter))]
        public EquationString WearCoefficient
        {
            get { return _slipWear.WearCoefficient.Equation; }
            set { _slipWear.WearCoefficient.Equation = value; }
        }


        // Constructors                                                                                                             
        public ViewSlipWear(SlipWear slipWear)
        {
            _slipWear = slipWear;
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public override MaterialProperty GetBase()
        {
            return _slipWear;
        }
        public double GetHardnessValue()
        {
            return _slipWear.Hardness.Value;
        }
        public double GetWearCoefficientValue()
        {
            return _slipWear.WearCoefficient.Value;
        }
    }
}
