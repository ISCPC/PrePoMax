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
        private string _selectionTmp;
        private CaeModel.Section _section;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the section.")]
        public string Name { get { return _section.Name; } set { _section.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Material")]
        [DescriptionAttribute("Select the material for the section definition.")]
        public string MaterialName { get { return _section.MaterialName; } set { _section.MaterialName = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Create by/from")]        
        [DescriptionAttribute("Select the way the section will be created.")]
        public override string RegionType
        {
            get { return base.RegionType; }
            set 
            {
                base.RegionType = value;
            } 
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(3, 10, "Tmp")]
        [DescriptionAttribute("Tmp.")]
        public string SelectionTmp { get { return _selectionTmp; } set { _selectionTmp = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(4, 10, "Part")]        
        [DescriptionAttribute("Select the part which will be used for the section definition.")]
        public string PartName { get { return _section.RegionName; } set { _section.RegionName = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(5, 10, "Element set")]
        [DescriptionAttribute("Select the element set which will be used for the section definition.")]
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
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, CaeGlobals.Tools.GetPropertyName(() => this.SelectionTmp));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.PartName, CaeGlobals.Tools.GetPropertyName(() => this.PartName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ElementSetName, CaeGlobals.Tools.GetPropertyName(() => this.ElementSetName));
            //
            base.SetBase(_section, regionTypePropertyNamePairs);
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }
        public virtual CaeModel.Section GetBase()
        {
            return _section;
        }
        public void PopululateDropDownLists(string[] materialNames, string[] partNames, string[] elementSetNames)
        {
            base.DynamicCustomTypeDescriptor.PopulateProperty(() => this.MaterialName, materialNames);
            //
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Tmp" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.PartName, partNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.ElementSetName, elementSetNames);
            base.PopululateDropDownLists(regionTypeListItemsPairs);
            //
        }
        public override void UpdateRegionVisibility()
        {
            base.UpdateRegionVisibility();
            //
            CustomPropertyDescriptor cpd;
            if (base.RegionType == RegionTypeEnum.Selection.ToFriendlyString())
            {
                cpd = base.DynamicCustomTypeDescriptor.GetProperty("SelectionTmp");
                cpd.SetIsBrowsable(false);
            }
        }
    }
   

}
