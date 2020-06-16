using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    [Serializable]
    [Flags]
    public enum UpdateType
    {
        Check = 0x01,
        DrawMesh = 0x02,
        ResetCamera = 0x04,
        RedrawSymbols = 0x08
    }
}
