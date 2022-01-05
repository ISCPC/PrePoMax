using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using System.Drawing;
using System.Drawing.Design;

namespace PrePoMax
{
    [Serializable]
    public class ViewPointSpring : ViewConstraint
    {
        // Variables                                                                                                                
        private CaeModel.PointSpring _pointSpring;


        // Properties                                                                                                               
        public override string Name { get { return _pointSpring.Name; } set { _pointSpring.Name = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(0, 10, "Region type")]
        [DescriptionAttribute("Select the region type for the creation of the constraint definition.")]
        [Id(1, 2)]
        public override string MasterRegionType { get { return base.MasterRegionType; } set { base.MasterRegionType = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(1, 10, "Node set")]
        [DescriptionAttribute("Select the node set for the creation of the constraint definition.")]
        [Id(2, 2)]
        public string NodeSetName { get { return _pointSpring.RegionName; } set { _pointSpring.RegionName = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Reference point")]
        [DescriptionAttribute("Select the reference point for the creation of the constraint definition.")]
        [Id(3, 2)]
        public string ReferencePointName { get { return _pointSpring.RegionName; } set { _pointSpring.RegionName = value; } }
        //
        [CategoryAttribute("DOF")]
        [OrderedDisplayName(0, 10, "K1")]
        [DescriptionAttribute("Value of the stiffness per node in the direction of the first axis.")]
        [TypeConverter(typeof(StringForcePerLenghtConverter))]
        [Id(1, 3)]
        public double K1 { get { return _pointSpring.K1; } set { _pointSpring.K1 = value; } }
        //
        [CategoryAttribute("DOF")]
        [OrderedDisplayName(1, 10, "K2")]
        [DescriptionAttribute("Value of the stiffness per node in the direction of the second axis.")]
        [TypeConverter(typeof(StringForcePerLenghtConverter))]
        [Id(2, 3)]
        public double K2 { get { return _pointSpring.K2; } set { _pointSpring.K2 = value; } }
        //
        [CategoryAttribute("DOF")]
        [OrderedDisplayName(2, 10, "K3")]
        [DescriptionAttribute("Value of the stiffness per node in the direction of the third axis.")]
        [TypeConverter(typeof(StringForcePerLenghtConverter))]
        [Id(3, 3)]
        public double K3 { get { return _pointSpring.K3; } set { _pointSpring.K3 = value; } }
        //
        [CategoryAttribute("DOF")]
        [OrderedDisplayName(3, 10, "KT1")]
        [DescriptionAttribute("Value of the torsional stiffness per node around the first axis.")]
        [Id(4, 3)]
        public double KT1 { get { return _pointSpring.KT1; } set { _pointSpring.KT1 = value; } }
        //
        [CategoryAttribute("DOF")]
        [OrderedDisplayName(4, 10, "KT2")]
        [DescriptionAttribute("Value of the torsional stiffness per node around the second axis.")]
        [Id(5, 3)]
        public double KT2 { get { return _pointSpring.KT2; } set { _pointSpring.KT2 = value; } }
        //
        [CategoryAttribute("DOF")]
        [OrderedDisplayName(5, 10, "KT3")]
        [DescriptionAttribute("Value of the torsional stiffness per node around the third axis.")]
        [Id(6, 3)]
        public double KT3 { get { return _pointSpring.KT3; } set { _pointSpring.KT3 = value; } }
        //
        [Category("Appearance")]
        [DisplayName("Color")]
        [Description("Select the constraint color.")]
        [Editor(typeof(UserControls.ColorEditorEx), typeof(UITypeEditor))]
        [Id(1, 10)]
        public Color Color { get { return _pointSpring.MasterColor; } set { _pointSpring.MasterColor = value; } }


        // Constructors                                                                                                             
        public ViewPointSpring(CaeModel.PointSpring pointSpring)
        {
            // The order is important
            _pointSpring = pointSpring;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(MasterSelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.NodeSetName, nameof(NodeSetName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ReferencePointName, nameof(ReferencePointName));
            //
            SetBase(_pointSpring, regionTypePropertyNamePairs, null);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            //
            UpdateRegionVisibility();
        }


        // Methods                                                                                                                  
        public override CaeModel.Constraint GetBase()
        {
            return _pointSpring;
        }
        public void PopululateDropDownLists(string[] nodeSetNames, string[] referencePointNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.NodeSetName, nodeSetNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.ReferencePointName, referencePointNames);
            //
            PopululateDropDownLists(regionTypeListItemsPairs, null);
        }
        public override void UpdateRegionVisibility()
        {
            base.UpdateRegionVisibility();
            //
            bool visible = !DynamicCustomTypeDescriptor.GetProperty(nameof(ReferencePointName)).IsBrowsable;

            visible = false;

            // 2D
            DynamicCustomTypeDescriptor.GetProperty(nameof(K3)).SetIsBrowsable(!_pointSpring.TwoD);
            //
            DynamicCustomTypeDescriptor.GetProperty(nameof(KT1)).SetIsBrowsable(visible && !_pointSpring.TwoD);
            DynamicCustomTypeDescriptor.GetProperty(nameof(KT2)).SetIsBrowsable(visible && !_pointSpring.TwoD);
            DynamicCustomTypeDescriptor.GetProperty(nameof(KT3)).SetIsBrowsable(visible);
        }
    }

}
