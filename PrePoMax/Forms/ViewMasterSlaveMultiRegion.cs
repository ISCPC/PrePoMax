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
    public class ViewMasterSlaveMultiRegion
    {
        // Variables                                                                                                                
        private IMasterSlaveMultiRegion _region;
        private Dictionary<RegionTypeEnum, string> _masterRegionTypePropertyNamePairs;
        private Dictionary<string, RegionTypeEnum> _masterRegionNameRegionTypePairs;
        private Dictionary<RegionTypeEnum, string> _slaveRegionTypePropertyNamePairs;
        private Dictionary<string, RegionTypeEnum> _slaveRegionNameRegionTypePairs;
        private DynamicCustomTypeDescriptor _dctd = null;


        // Properties                                                                                                               
        [Browsable(false)]
        public virtual string MasterRegionType
        {
            // This function returns strings bacause using enum RegionType shows all items in the drop down box
            get
            {
                if (_region == null) return "";
                if (_masterRegionTypePropertyNamePairs.ContainsKey(_region.MasterRegionType))
                    return _region.MasterRegionType.ToFriendlyString();
                else throw new NotSupportedException();
            }
            set
            {
                _region.MasterRegionType =_masterRegionNameRegionTypePairs[value];
                CustomPropertyDescriptor cpd;
                cpd = _dctd.GetProperty(_masterRegionTypePropertyNamePairs[_region.MasterRegionType]);
                _region.MasterRegionName = cpd.StatandardValues.First().ToString();
                //
                UpdateRegionVisibility();
            }
        }
        //
        [Browsable(false)]
        public virtual string SlaveRegionType
        {
            // This function returns strings bacause using enum RegionType shows all items in the drop down box
            get
            {
                if (_region == null || _region.SlaveRegionType == RegionTypeEnum.None) return "";
                else if (_slaveRegionTypePropertyNamePairs.ContainsKey(_region.SlaveRegionType))
                    return _region.SlaveRegionType.ToFriendlyString();
                else throw new NotSupportedException();
            }
            set
            {
                _region.SlaveRegionType = _slaveRegionNameRegionTypePairs[value];
                CustomPropertyDescriptor cpd;
                cpd = _dctd.GetProperty(_slaveRegionTypePropertyNamePairs[_region.SlaveRegionType]);
                _region.SlaveRegionName = cpd.StatandardValues.First().ToString();
                //
                UpdateRegionVisibility();
            }
        }
        //
        [Browsable(false)]
        public DynamicCustomTypeDescriptor DynamicCustomTypeDescriptor { get { return _dctd; } set { _dctd = value; } }


        // Constructors                                                                                                             
        public ViewMasterSlaveMultiRegion()
        {
        }


        // Methods                                                                                                                  
        public void SetBase(IMasterSlaveMultiRegion region, Dictionary<RegionTypeEnum, string> masterRegionTypePropertyNamePairs,
                            Dictionary<RegionTypeEnum, string> slaveRegionTypePropertyNamePairs)
        {
            _region = region;
            _masterRegionTypePropertyNamePairs = masterRegionTypePropertyNamePairs;
            _slaveRegionTypePropertyNamePairs = slaveRegionTypePropertyNamePairs;
            //
            _masterRegionNameRegionTypePairs = new Dictionary<string, RegionTypeEnum>();
            if (_masterRegionTypePropertyNamePairs != null)
            {
                foreach (var entry in _masterRegionTypePropertyNamePairs)
                    _masterRegionNameRegionTypePairs.Add(entry.Key.ToFriendlyString(), entry.Key);
            }
            //
            _slaveRegionNameRegionTypePairs = new Dictionary<string, RegionTypeEnum>();
            if (_slaveRegionTypePropertyNamePairs != null)
            {
                foreach (var entry in _slaveRegionTypePropertyNamePairs)
                    _slaveRegionNameRegionTypePairs.Add(entry.Key.ToFriendlyString(), entry.Key);
            }
        }
        public void PopulateDropDownLists(Dictionary<RegionTypeEnum, string[]> masterRegionTypeListItemsPairs,
                                            Dictionary<RegionTypeEnum, string[]> slaveRegionTypeListItemsPairs)
        {
            // Master
            if (masterRegionTypeListItemsPairs != null)
            {
                foreach (var entry in masterRegionTypeListItemsPairs)
                    _dctd.PopulateProperty(_masterRegionTypePropertyNamePairs[entry.Key], entry.Value);
            }
            // Slave
            if (slaveRegionTypeListItemsPairs != null)
            {
                foreach (var entry in slaveRegionTypeListItemsPairs)
                    _dctd.PopulateProperty(_slaveRegionTypePropertyNamePairs[entry.Key], entry.Value);
            }
            // Prepare the region drop down list
            PopulateDropDownListForRegionType(masterRegionTypeListItemsPairs, slaveRegionTypeListItemsPairs);
            // Update visible dorp down lists
            UpdateRegionVisibility();
        }
        private void PopulateDropDownListForRegionType(Dictionary<RegionTypeEnum, string[]> masterRegionTypeListItemsPairs,
                                                         Dictionary<RegionTypeEnum, string[]> slaveRegionTypeListItemsPairs)
        {
            // Master
            List<string> types = new List<string>();
            bool selection = false;
            foreach (var entry in masterRegionTypeListItemsPairs)
            {
                if (entry.Value.Length > 0) types.Add(entry.Key.ToFriendlyString());
                if (entry.Key == RegionTypeEnum.Selection) selection = true;
            }
            //
            int numOfItemsToBeBrowsable = 2;
            if (selection) numOfItemsToBeBrowsable = 1;
            _dctd.PopulateProperty(() => this.MasterRegionType, types.ToArray(), false, numOfItemsToBeBrowsable);
            // Slave
            if (slaveRegionTypeListItemsPairs != null)
            {
                types = new List<string>();
                selection = false;
                foreach (var entry in slaveRegionTypeListItemsPairs)
                {
                    if (entry.Value.Length > 0) types.Add(entry.Key.ToFriendlyString());
                    if (entry.Key == RegionTypeEnum.Selection) selection = true;
                }
                //
                numOfItemsToBeBrowsable = 2;
                if (selection) numOfItemsToBeBrowsable = 1;
                _dctd.PopulateProperty(() => this.SlaveRegionType, types.ToArray(), false, numOfItemsToBeBrowsable);
            }
        }
        public virtual void UpdateRegionVisibility()
        {
            bool browsable;
            // Master
            foreach (var entry in _masterRegionTypePropertyNamePairs)
            {
                browsable = _region.MasterRegionType == entry.Key;
                _dctd.GetProperty(entry.Value).SetIsBrowsable(browsable);
            }
            // Slave
            if (_slaveRegionTypePropertyNamePairs != null)
            {
                foreach (var entry in _slaveRegionTypePropertyNamePairs)
                {
                    browsable = _region.SlaveRegionType == entry.Key;
                    _dctd.GetProperty(entry.Value).SetIsBrowsable(browsable);
                }
            }
        }
    }
}
