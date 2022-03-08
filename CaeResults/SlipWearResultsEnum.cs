using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicTypeDescriptor;

namespace CaeResults
{
    [Serializable]
    public enum SlipWearResultsEnum
    {
        [StandardValue("All", DisplayName = "All")]
        All,
        //
        [StandardValue("SlipWearSteps", DisplayName = "Slip wear steps", Description = "SWS")]
        SlipWearSteps,
        //
        [StandardValue("LastIncrementOfSlipWearSteps", DisplayName = "Last increment of slip wear steps")]
        LastIncrementOfSlipWearSteps,
        //
        [StandardValue("LastIncrementOfLastSlipWearStep", DisplayName = "Last increment of last slip wear step")]
        LastIncrementOfLastSlipWearStep
    }
}
