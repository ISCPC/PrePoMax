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
        [DynamicTypeDescriptor.StandardValue("SolidAsShell", DisplayName = "Solid as shell")]
        SolidAsShell = 1,
        Shell = 2,
        Wire = 3,
        Compound = 10,
        Unknown = 100
    }
    public static class Extensions
    {
        public static bool HasEdges(this PartType partType)
        {
            if (partType == PartType.Solid || partType == PartType.SolidAsShell || partType == PartType.Shell ||
                partType == PartType.Wire) return true;
            else throw new NotSupportedException();
        }
    }
}
