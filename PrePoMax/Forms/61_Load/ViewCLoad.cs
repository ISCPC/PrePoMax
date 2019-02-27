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
    public class ViewCLoad : ViewLoad
    {
        // Variables                                                                                                                
        private CaeModel.CLoad _cLoad;


        // Properties                                                                                                               
        public override string Name { get { return _cLoad.Name; } set { _cLoad.Name = value; } }

        [OrderedDisplayName(1, 10, "Region type")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the region type which will be used for the load.")]
        public override string RegionType { get { return base.RegionType; } set { base.RegionType = value; } }


        [OrderedDisplayName(2, 10, "Node set")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the node set which will be used for the load.")]
        public string NodeSetName { get { return _cLoad.RegionName; } set { _cLoad.RegionName = value; } }

        [OrderedDisplayName(3, 10, "Reference point")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the reference point which will be used for the load.")]
        public string ReferencePointName { get { return _cLoad.RegionName; } set { _cLoad.RegionName = value; } }

        [OrderedDisplayName(4, 10, "F1")]
        [CategoryAttribute("Force components")]
        [DescriptionAttribute("Force per item in the direction of the first axis.")]
        public double F1 { get { return _cLoad.F1; } set { _cLoad.F1 = value; } }
        
        [OrderedDisplayName(5, 10, "F2")]
        [CategoryAttribute("Force components")]
        [DescriptionAttribute("Force per item in the direction of the second axis.")]
        public double F2 { get { return _cLoad.F2; } set { _cLoad.F2 = value; } }

        [OrderedDisplayName(6, 10, "F3")]
        [CategoryAttribute("Force components")]
        [DescriptionAttribute("Force per item in the direction of the third axis.")]
        public double F3 { get { return _cLoad.F3; } set { _cLoad.F3 = value; } }

        [OrderedDisplayName(7, 10, "Magnitude")]
        [CategoryAttribute("Force magnitude")]
        [DescriptionAttribute("Force magnitude.")]
        public double Flength
        {
            get { return Math.Sqrt(_cLoad.F1 * _cLoad.F1 + _cLoad.F2 * _cLoad.F2 + _cLoad.F3 * _cLoad.F3); }
            set
            {
                if (value <= 0)
                    throw new Exception("The magnitude of the force must be greater than 0.");

                double len = Math.Sqrt(_cLoad.F1 * _cLoad.F1 + _cLoad.F2 * _cLoad.F2 + _cLoad.F3 * _cLoad.F3);
                double r;
                if (len == 0) r = 0;
                else r = value / len;
                _cLoad.F1 *= r;
                _cLoad.F2 *= r;
                _cLoad.F3 *= r;
            }
        }


        // Constructors                                                                                                             
        public ViewCLoad(CaeModel.CLoad cLoad)
        {
            // the order is important
            _cLoad = cLoad;

            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.NodeSetName, CaeGlobals.Tools.GetPropertyName(() => this.NodeSetName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ReferencePointName, CaeGlobals.Tools.GetPropertyName(() => this.ReferencePointName));

            base.SetBase(_cLoad, regionTypePropertyNamePairs);
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }



        // Methods                                                                                                                  
        public override CaeModel.Load GetBase()
        {
            return _cLoad;
        }
        public void PopululateDropDownLists(string[] nodeSetNames, string[] referencePointNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.NodeSetName, nodeSetNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.ReferencePointName, referencePointNames);
            base.PopululateDropDownLists(regionTypeListItemsPairs);
        }
    }

}
