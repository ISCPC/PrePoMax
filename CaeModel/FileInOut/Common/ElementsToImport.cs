using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileInOut.Input
{
    [Serializable]
    [Flags]
    public enum ElementsToImport
    {
        // Must start from 1 othervise the 0 value has no effect
        Beam = 1,
        Shell = 2,
        Solid = 4,
        All = Beam | Shell | Solid
    }
}
