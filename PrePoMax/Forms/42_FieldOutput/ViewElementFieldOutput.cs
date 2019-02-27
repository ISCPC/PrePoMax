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
    public enum ViewElementVariable
    {
        [StandardValue("E", Description = "Strains.")]
        E = 1,

        [StandardValue("PEEQ", Description = "Equivalent plastic strain.")]
        PEEQ = 2,

        [StandardValue("S", Description = "Stresses.")]
        S = 4,

        [StandardValue("ENER", Description = "Energy density.")]
        ENER = 8,

        [StandardValue("ERR", Description = "Extrapolation error estimator for stress calculations.")]
        ERR = 16
    }

    [Serializable]
    public class ViewElementFieldOutput : ViewFieldOutput
    {
        // Variables                                                                                                                
        private CaeModel.ElementFieldOutput _fieldOutput;
        private DynamicCustomTypeDescriptor _dctd = null;

        // Properties                                                                                                               
        public override string Name { get { return _fieldOutput.Name; } set { _fieldOutput.Name = value; } }

        [OrderedDisplayName(1, 10, "Variables to output")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Element field variables")]
        public ViewElementVariable Variables
        {
            get
            {
                return (ViewElementVariable)_fieldOutput.Variables;
            }
            set
            {
                _fieldOutput.Variables = (CaeModel.ElementVariable)value;
            }
        }

        public override CaeModel.FieldOutput Base { get { return _fieldOutput; } set { _fieldOutput = (CaeModel.ElementFieldOutput)value; } }


        // Constructors                                                                                                             
        public ViewElementFieldOutput(CaeModel.ElementFieldOutput fieldOutput)
        {
            _fieldOutput = fieldOutput;
            _dctd = ProviderInstaller.Install(this);
        }


    }

}
