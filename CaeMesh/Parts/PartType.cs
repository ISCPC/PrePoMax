using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public enum PartType
    {        
        Solid = 0,
        SolidAsShell = 1,
        Shell = 2,
        Wire = 3,
        Compound = 10,
        Unknown = 100
    }
}
