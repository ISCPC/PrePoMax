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
        Beam = 0,
        Shell = 1,
        Solid = 2,
        All = Beam | Shell | Solid
    }
}
