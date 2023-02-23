using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicTypeDescriptor;
using CaeGlobals;

namespace CaeResults
{
    [Serializable]
    public enum ComplexResultTypeEnum
    {
        Real,
        Imaginary,
        Magnitude,
        Phase,
        Angle,
        Max,
        AngleAtMax,
        Min,
        AngleAtMin
    }
   
}
