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
    [EnumResource("PrePoMax.Properties.Resources")]
    [Editor(typeof(StandardValueEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [Flags]
    public enum ViewElementFieldVariable
    {
        // Must start at 1 for the UI to work
        [StandardValue("S", Description = "Stresses.")]
        S = 1,
        //
        [StandardValue("E", Description = "Total strains.")]
        E = 2,
        //
        [StandardValue("ME", Description = "Mechanical strains.")]
        ME = 4,
        //
        [StandardValue("PEEQ", Description = "Equivalent plastic strain.")]
        PEEQ = 8,
        //
        [StandardValue("ENER", Description = "Energy density.")]
        ENER = 16,
        //
        [StandardValue("ERR", Description = "Extrapolation error estimator for stress calculations. ZZS and ERR are mutually exclusive.")]
        ERR = 32,
        //
        [StandardValue("ZZS", Description = "Zienkiewicz-Zhu improved stress. ZZS and ERR are mutually exclusive.")]
        ZZS = 64
    }

    [Serializable]
    public class ViewElementFieldOutput : ViewFieldOutput
    {
        // Variables                                                                                                                
        private CaeModel.ElementFieldOutput _fieldOutput;
        private DynamicCustomTypeDescriptor _dctd;


        // Properties                                                                                                               
        public override string Name { get { return _fieldOutput.Name; } set { _fieldOutput.Name = value; } }
        public override int Frequency { get { return _fieldOutput.Frequency; } set { _fieldOutput.Frequency = value; } }
        //
        [OrderedDisplayName(2, 10, "Variables to output")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Element field variables")]
        public ViewElementFieldVariable Variables
        {
            get
            {
                return (ViewElementFieldVariable)_fieldOutput.Variables;
            }
            set
            {
                _fieldOutput.Variables = (CaeModel.ElementFieldVariable)value;
            }
        }
        //
        public override CaeModel.FieldOutput Base { get { return _fieldOutput; } set { _fieldOutput = (CaeModel.ElementFieldOutput)value; } }


        // Constructors                                                                                                             
        public ViewElementFieldOutput(CaeModel.ElementFieldOutput fieldOutput)
        {
            _fieldOutput = fieldOutput;
            _dctd = ProviderInstaller.Install(this);
        }


    }

}
