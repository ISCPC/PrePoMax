using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public abstract class FeElement1D : FeElement
    {
        // Properties                                                                                                               
        
            
        // Constructors                                                                                                             
        public FeElement1D(int id, int[] nodeIds)
            : base(id, nodeIds)
        {
        }

        public FeElement1D(int id, int partId, int[] nodeIds)
            : base(id, partId, nodeIds)
        {
        }
       
    }
}
