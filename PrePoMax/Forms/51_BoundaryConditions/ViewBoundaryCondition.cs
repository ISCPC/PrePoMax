using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;

namespace PrePoMax
{
    [Serializable]
    public abstract class ViewBoundaryCondition : ViewMultiRegion
    {
        // Variables                                                                                                                
        private string _selectionHidden;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [ReadOnly(false)]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the boundary condition.")]
        [Id(1, 1)]
        public abstract string Name { get; set; }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(0, 10, "Region type")]
        [DescriptionAttribute("Select the region type for the creation of the boundary condition.")]
        [Id(1, 2)]
        public override string RegionType { get { return base.RegionType; } set { base.RegionType = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(1, 10, "Hidden")]
        [DescriptionAttribute("Hidden.")]
        [Id(2, 2)]
        public string SelectionHidden { get { return _selectionHidden; } set { _selectionHidden = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Node set")]
        [DescriptionAttribute("Select the node set for the creation of the boundary condition.")]
        [Id(3, 2)]
        public abstract string NodeSetName { get; set; }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(3, 10, "Surface")]
        [DescriptionAttribute("Select the surface for the creation of the boundary condition.")]
        [Id(4, 2)]
        public abstract string SurfaceName { get; set; }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(4, 10, "Reference point")]
        [DescriptionAttribute("Select the reference point for the creation of the boundary condition.")]
        [Id(5, 2)]
        public abstract string ReferencePointName { get; set; }


        // Constructors                                                                                                             


        // Methods
        public abstract CaeModel.BoundaryCondition GetBase();
        public override void UpdateRegionVisibility()
        {
            base.UpdateRegionVisibility();
            // Hide SelectionHidden
            DynamicTypeDescriptor.CustomPropertyDescriptor cpd;
            if (base.RegionType == RegionTypeEnum.Selection.ToFriendlyString())
            {
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(() => SelectionHidden);
                cpd.SetIsBrowsable(false);
            }
        }
    }



}
