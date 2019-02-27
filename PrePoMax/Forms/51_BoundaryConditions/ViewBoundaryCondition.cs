using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;

namespace PrePoMax
{
    [Serializable]
    public abstract class ViewBoundaryCondition : ViewMultiRegion
    {
        // Variables                                                                                                                


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [ReadOnly(false)]
        [OrderedDisplayName(0, 20, "Name")]
        [DescriptionAttribute("Name of the boundary condition.")]
        public abstract string Name { get; set; }

        [OrderedDisplayName(1, 20, "Region type")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the region type which will be used for the boundary condition.")]
        public override string RegionType { get { return base.RegionType; } set { base.RegionType = value; } }

        [OrderedDisplayName(2, 20, "Node set")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the node set which will be used for the boundary condition.")]
        public abstract string NodeSetName { get; set; }

        [OrderedDisplayName(3, 20, "Surface")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the surface which will be used for the boundary condition.")]
        public abstract string SurfaceName { get; set; }

        [OrderedDisplayName(4, 20, "Reference point")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the reference point which will be used for the boundary condition.")]
        public abstract string ReferencePointName { get; set; }


        // Constructors                                                                                                             


        // Methods
        public abstract CaeModel.BoundaryCondition GetBase();
    }



}
