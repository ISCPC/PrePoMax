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
    public class ViewSection : ViewMultiRegion
    {
        // Variables                                                                                                                
        private string _selectionHidden;
        private CaeModel.Section _section;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the section.")]
        public string Name { get { return _section.Name; } set { _section.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Material")]
        [DescriptionAttribute("Select the material for the creation of the section.")]
        public string MaterialName { get { return _section.MaterialName; } set { _section.MaterialName = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(0, 10, "Region type")]
        [DescriptionAttribute("Select the region type for the creation of the section.")]
        public override string RegionType { get { return base.RegionType; } set { base.RegionType = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(1, 10, "Hidden")]
        [DescriptionAttribute("Hidden.")]
        public string SelectionHidden { get { return _selectionHidden; } set { _selectionHidden = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Part")]        
        [DescriptionAttribute("Select the part for the creation of the section.")]
        public string PartName { get { return _section.RegionName; } set { _section.RegionName = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(3, 10, "Element set")]
        [DescriptionAttribute("Select the element set for the creation of the section.")]
        public string ElementSetName { get { return _section.RegionName; } set { _section.RegionName = value; } }


        // Constructors                                                                                                             
        public ViewSection()
        {
        }


        // Methods
        public void SetBase(CaeModel.Section section)
        {
            _section = section;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.PartName, nameof(PartName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ElementSetName, nameof(ElementSetName));
            //
            base.SetBase(_section, regionTypePropertyNamePairs);
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }
        public virtual CaeModel.Section GetBase()
        {
            return _section;
        }
        public void PopulateDropDownLists(string[] materialNames, string[] partNames, string[] elementSetNames)
        {
            base.DynamicCustomTypeDescriptor.PopulateProperty(() => this.MaterialName, materialNames);
            //
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.PartName, partNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.ElementSetName, elementSetNames);
            base.PopulateDropDownLists(regionTypeListItemsPairs);
        }
        public override void UpdateRegionVisibility()
        {
            base.UpdateRegionVisibility();
            //
            CustomPropertyDescriptor cpd;
            if (base.RegionType == RegionTypeEnum.Selection.ToFriendlyString())
            {
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(SelectionHidden));
                cpd.SetIsBrowsable(false);
            }
        }
    }
   

}
