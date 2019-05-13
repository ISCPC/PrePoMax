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
    public abstract class ViewHistoryOutput : ViewMultiRegion
    {
        // Variables                                                                                                                


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the history output.")]
        public abstract string Name { get; set; }

        [OrderedDisplayName(1, 10, "Region type")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the region type which will be used for the section definition.")]
        public override string RegionType { get { return base.RegionType; } set { base.RegionType = value; } }


        // Constructors                                                                                                             


        // Methods
        public abstract CaeModel.HistoryOutput GetBase();
    }
}
