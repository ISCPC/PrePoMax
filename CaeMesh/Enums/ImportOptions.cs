using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public enum ImportOptions
    {
        DetectEdges,
        ImportStlParts,
        ImportCADShellParts,
        ImportOneCADSolidPart
    }
}
