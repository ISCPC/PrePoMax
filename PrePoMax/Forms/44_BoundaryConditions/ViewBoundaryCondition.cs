using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing.Design;
using DynamicTypeDescriptor;
using CaeGlobals;

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
        //
        [CategoryAttribute("Time/Frequency")]
        [OrderedDisplayName(0, 10, "Amplitude")]
        [DescriptionAttribute("Select the amplitude for the boundary condition.")]
        [Id(1, 9)]
        public abstract string AmplitudeName { get; set; }
        //
        [Category("Appearance")]
        [DisplayName("Color")]
        [Description("Select boundary condition color.")]
        [Editor(typeof(UserControls.ColorEditorEx), typeof(UITypeEditor))]
        [Id(1, 10)]
        public abstract System.Drawing.Color Color { get; set; }

        // Constructors                                                                                                             


        // Methods
        public abstract CaeModel.BoundaryCondition GetBase();
        public override void UpdateRegionVisibility()
        {
            base.UpdateRegionVisibility();
            // Hide SelectionHidden
            if (base.RegionType == RegionTypeEnum.Selection.ToFriendlyString())
                DynamicCustomTypeDescriptor.GetProperty(nameof(SelectionHidden)).SetIsBrowsable(false);
        }
        public void PopulateAmplitudeNames(string[] amplitudeNames)
        {
            List<string> names = new List<string>();
            names.Add(CaeModel.BoundaryCondition.DefaultAmplitudeName);
            names.AddRange(amplitudeNames);
            DynamicCustomTypeDescriptor.PopulateProperty(nameof(AmplitudeName), names.ToArray(), false, 2);
        }
    }



}
