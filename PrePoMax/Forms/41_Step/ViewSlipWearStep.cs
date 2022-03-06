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
