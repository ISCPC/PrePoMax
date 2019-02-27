using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using CaeGlobals;

namespace PrePoMax
{
    [Serializable]
    public class ViewGravityLoad : ViewLoad
    {
        // Variables                                                                                                                
        private CaeModel.GravityLoad _gLoad;


        // Properties                                                                                                               
        public override string Name { get { return _gLoad.Name; } set { _gLoad.Name = value; } }

        [OrderedDisplayName(1, 10, "Region type")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the region type which will be used for the load.")]
        public override string RegionType { get { return base.RegionType; } set { base.RegionType = value; } }


        [OrderedDisplayName(2, 10, "Part")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the part which will be used for the load.")]
        public string PartName { get { return _gLoad.RegionName; } set { _gLoad.RegionName = value; } }

        [OrderedDisplayName(3, 10, "Element set")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the element set which will be used for the load.")]
        public string ElementSetName { get { return _gLoad.RegionName; } set { _gLoad.RegionName = value; } }

        [OrderedDisplayName(4, 10, "F1")]
        [CategoryAttribute("Force components")]
        [DescriptionAttribute("Force per item in the direction of the first axis.")]
        public double F1 { get { return _gLoad.F1; } set { _gLoad.F1 = value; } }
        
        [OrderedDisplayName(5, 10, "F2")]
        [CategoryAttribute("Force components")]
        [DescriptionAttribute("Force per item in the direction of the second axis.")]
        public double F2 { get { return _gLoad.F2; } set { _gLoad.F2 = value; } }

        [OrderedDisplayName(6, 10, "F3")]
        [CategoryAttribute("Force components")]
        [DescriptionAttribute("Force per item in the direction of the third axis.")]
        public double F3 { get { return _gLoad.F3; } set { _gLoad.F3 = value; } }

        [OrderedDisplayName(7, 10, "Magnitude")]
        [CategoryAttribute("Force magnitude")]
        [DescriptionAttribute("Force magnitude.")]
        public double Flength
        {
            get { return Math.Sqrt(_gLoad.F1 * _gLoad.F1 + _gLoad.F2 * _gLoad.F2 + _gLoad.F3 * _gLoad.F3); }
            set
            {
                if (value <= 0)
                    throw new Exception("The magnitude of the force must be greater than 0.");

                double len = Math.Sqrt(_gLoad.F1 * _gLoad.F1 + _gLoad.F2 * _gLoad.F2 + _gLoad.F3 * _gLoad.F3);
                double r;
                if (len == 0) r = 0;
                else r = value / len;
                _gLoad.F1 *= r;
                _gLoad.F2 *= r;
                _gLoad.F3 *= r;
            }
        }


        // Constructors                                                                                                             
        public ViewGravityLoad(CaeModel.GravityLoad gLoad)
        {
            // the order is important
            _gLoad = gLoad;

            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.PartName, CaeGlobals.Tools.GetPropertyName(() => this.PartName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ElementSetName, CaeGlobals.Tools.GetPropertyName(() => this.ElementSetName));

            base.SetBase(_gLoad, regionTypePropertyNamePairs);
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }



        // Methods                                                                                                                  
        public override CaeModel.Load GetBase()
        {
            return _gLoad;
        }
        public void PopululateDropDownLists(string[] partNames, string[] elementSetNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.PartName, partNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.ElementSetName, elementSetNames);
            base.PopululateDropDownLists(regionTypeListItemsPairs);
        }
    }

}
