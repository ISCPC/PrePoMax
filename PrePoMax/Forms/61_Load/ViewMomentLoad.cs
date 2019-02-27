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
    public class ViewMomentLoad : ViewLoad
    {
        // Variables                                                                                                                
        private CaeModel.MomentLoad _momentLoad;


        // Properties                                                                                                               
        public override string Name { get { return _momentLoad.Name; } set { _momentLoad.Name = value; } }

        [OrderedDisplayName(1, 10, "Region type")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the region type which will be used for the load.")]
        public override string RegionType { get { return base.RegionType; } set { base.RegionType = value; } }

        [OrderedDisplayName(2, 10, "Node set")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the node set which will be used for the load.")]
        public string NodeSetName { get { return _momentLoad.RegionName; } set { _momentLoad.RegionName = value; } }

        [OrderedDisplayName(3, 10, "Reference point")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the reference point which will be used for the load.")]
        public string ReferencePointName { get { return _momentLoad.RegionName; } set { _momentLoad.RegionName = value; } }

        [OrderedDisplayName(4, 10, "M1")]
        [CategoryAttribute("Moment components")]
        [DescriptionAttribute("Moment per item in the direction of the first axis.")]
        public double F1 { get { return _momentLoad.M1; } set { _momentLoad.M1 = value; } }
        
        [OrderedDisplayName(5, 10, "M2")]
        [CategoryAttribute("Moment components")]
        [DescriptionAttribute("Moment per item in the direction of the second axis.")]
        public double F2 { get { return _momentLoad.M2; } set { _momentLoad.M2 = value; } }

        [OrderedDisplayName(6, 10, "M3")]
        [CategoryAttribute("Moment components")]
        [DescriptionAttribute("Moment per item in the direction of the third axis.")]
        public double F3 { get { return _momentLoad.M3; } set { _momentLoad.M3 = value; } }


        // Constructors                                                                                                             
        public ViewMomentLoad(CaeModel.MomentLoad momentLoad)
        {
            // the order is important
            _momentLoad = momentLoad;

            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.NodeSetName, CaeGlobals.Tools.GetPropertyName(() => this.NodeSetName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ReferencePointName, CaeGlobals.Tools.GetPropertyName(() => this.ReferencePointName));

            base.SetBase(_momentLoad, regionTypePropertyNamePairs);
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }



        // Methods                                                                                                                  
        public override CaeModel.Load GetBase()
        {
            return _momentLoad;
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
