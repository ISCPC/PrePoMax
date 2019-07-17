using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeGlobals
{
    [Serializable]
    public class SelectionNodeInvert : SelectionNode
    {
        // Variables                                                                                                                



        // Properties                                                                                                               



        // Constructors                                                                                                             
        public SelectionNodeInvert()
            : base(vtkSelectOperation.Invert)
        {
        }

        // Methods                                                                                                                  
    }
}
