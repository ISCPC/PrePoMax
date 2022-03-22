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
    public class ViewMultiRegion
    {
        // Variables                                                                                                                
        private IMultiRegion _multiRegion;
        private Dictionary<RegionTypeEnum, string> _regionTypePropertyNamePairs;
        private Dictionary<string, RegionTypeEnum> _regionNameRegionTypePairs;
        private DynamicCustomTypeDescriptor _dctd = null;


        // Properties                                                                                                               
        [Browsable(false)]
        public virtual string RegionType
        {
            // This function returns strings bacause using enum RegionType shows all items in the drop down box
            get
            {
                if (_multiRegion == null) return "";
                if (_regionTypePropertyNamePairs.ContainsKey(_multiRegion.RegionType)) return _multiRegion.RegionType.ToFriendlyString();
                else throw new NotSupportedException();
            }
            set
            {
                _multiRegion.RegionType =_regionNameRegionTypePairs[value];
                CustomPropertyDescriptor cpd = _dctd.GetProperty(_regionTypePropertyNamePairs[_multiRegion.RegionType]);
                _multiRegion.RegionName = cpd.StatandardValues.First().ToString();
                //
                UpdateRegionVisibility();
            }
        }

        [Browsable(false)]
        public DynamicCustomTypeDescriptor DynamicCustomTypeDescriptor { get { return _dctd; } set { _dctd = value; } }


        // Constructors                                                                                                             
        public ViewMultiRegion()
        {
        }


        // Methods                                                                                                                  
        public void SetBase(IMultiRegion multiRegion, Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs)
        {
            _multiRegion = multiRegion;
            _regionTypePropertyNamePairs = regionTypePropertyNamePairs;
            //
            _regionNameRegionTypePairs = new Dictionary<string, RegionTypeEnum>();
            foreach (var entry in _regionTypePropertyNamePairs)
                _regionNameRegionTypePairs.Add(entry.Key.ToFriendlyString(), entry.Key);
        }
        public void PopulateDropDownLists(Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs)
        {
            foreach (var entry in regionTypeListItemsPairs)
                _dctd.PopulateProperty(_regionTypePropertyNamePairs[entry.Key], entry.Value);
            // Prepare the region drop down list
            PopulateDropDownListForRegionType(regionTypeListItemsPairs);
            // Update visible dorp down lists
            UpdateRegionVisibility();
        }
        private void PopulateDropDownListForRegionType(Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs)
        {
            List<string> types = new List<string>();
            bool selection = false;
            foreach (var entry in regionTypeListItemsPairs)
            {
                if (entry.Value.Length > 0) types.Add(entry.Key.ToFriendlyString());
                if (entry.Key == RegionTypeEnum.Selection) selection = true;
            }
            //
            int numOfItemsToBeBrowsable = 2;
            if (selection) numOfItemsToBeBrowsable = 1;
            _dctd.PopulateProperty(nameof(RegionType), types.ToArray(), false, numOfItemsToBeBrowsable);
        }
        public virtual void UpdateRegionVisibility()
        {
            bool browsable;
            foreach (var entry in _regionTypePropertyNamePairs)
            {
                browsable = _multiRegion.RegionType == entry.Key;
                _dctd.GetProperty(entry.Value).SetIsBrowsable(browsable);
            }
        }
    }
}
