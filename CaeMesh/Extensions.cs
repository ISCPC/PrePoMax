using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    public static class Extensions
    {
        public static bool HasEdges(this PartType partType)
        {
            if (partType == PartType.Solid || partType == PartType.SolidAsShell || partType == PartType.Shell ||
                partType == PartType.Wire) return true;
            else throw new NotSupportedException();
        }
        //
       
    }
}
