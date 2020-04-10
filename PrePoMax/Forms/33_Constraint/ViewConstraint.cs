using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;

namespace PrePoMax
{
    public abstract class ViewConstraint : ViewMasterSlaveMultiRegion
    {
        // Variables                                                                                                                


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the constraint.")]
        [Id(1, 1)]
        public abstract string Name { get; set; }


        // Methods                                                                                                                  
        public abstract CaeModel.Constraint GetBase();
    }
}
