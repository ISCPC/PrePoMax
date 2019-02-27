using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using System.ComponentModel;
using CaeGlobals;

namespace PrePoMax
{
    [Serializable]
    public abstract class ViewConstraint : ViewMultiRegion
    {
        // Variables                                                                                                                


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [ReadOnly(false)]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the constraint.")]
        public abstract string Name { get; set; }


        // Constructors                                                                                                             
       

        // Methods                                                                                                                  
        public abstract CaeModel.Constraint GetBase();
    }
}
