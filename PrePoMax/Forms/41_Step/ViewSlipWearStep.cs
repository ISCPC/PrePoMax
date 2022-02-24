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
    public class ViewSlipWearStep : ViewStaticStep
    {
        // Variables                                                                                                                
        private CaeModel.SlipWearStep _slipWearStep;


        // Properties                                                                                                               
        [CategoryAttribute("Slip wear settings")]
        [OrderedDisplayName(0, 10, "Number of cycles")]
        [DescriptionAttribute("Set the numer of wear cycles.")]
        public int NumOfCycles { get { return _slipWearStep.NumOfCycles; } set { _slipWearStep.NumOfCycles = value; } }


        // Constructors                                                                                                             
        public ViewSlipWearStep(CaeModel.SlipWearStep step, bool installProvider = true)
            : base(step, false) // delay provider installation
        {
            _slipWearStep = step;
            if (installProvider) InstallProvider(); // install provider
        }


        // Methods
        public override CaeModel.Step GetBase()
        {
            return _slipWearStep;
        }
    }
}
