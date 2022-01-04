using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using System.ComponentModel;
using DynamicTypeDescriptor;
using CaeGlobals;

namespace PrePoMax
{
    public abstract class ViewConstraint : ViewMasterSlaveMultiRegion
    {
        // Variables                                                                                                                
        [Browsable(false)]
        public virtual string MasterSelectionHidden { get; set; }
        //
        [Browsable(false)]
        public virtual string SlaveSelectionHidden { get; set; }


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the constraint.")]
        [Id(1, 1)]
        public abstract string Name { get; set; }


        // Methods                                                                                                                  
        public abstract CaeModel.Constraint GetBase();
        public override void UpdateRegionVisibility()
        {
            base.UpdateRegionVisibility();
            // Master
            if (base.MasterRegionType == RegionTypeEnum.Selection.ToFriendlyString())
                base.DynamicCustomTypeDescriptor.GetProperty(nameof(MasterSelectionHidden)).SetIsBrowsable(false);
            // Slave
            if (base.SlaveRegionType == RegionTypeEnum.Selection.ToFriendlyString())
                base.DynamicCustomTypeDescriptor.GetProperty(nameof(SlaveSelectionHidden)).SetIsBrowsable(false);
        }
    }
}
