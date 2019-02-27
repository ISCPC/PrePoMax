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
    public class ViewCentrifLoad : ViewLoad
    {
        // Variables                                                                                                                
        private CaeModel.CentrifLoad _cLoad;


        // Properties                                                                                                               
        [Id(1, 1)]
        public override string Name { get { return _cLoad.Name; } set { _cLoad.Name = value; } }

        [OrderedDisplayName(1, 10, "Region type")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the region type which will be used for the load.")]
        [Id(1, 1)]
        public override string RegionType { get { return base.RegionType; } set { base.RegionType = value; } }

        [OrderedDisplayName(2, 10, "Part")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the part which will be used for the load.")]
        [Id(1, 1)]
        public string PartName { get { return _cLoad.RegionName; } set { _cLoad.RegionName = value; } }

        [OrderedDisplayName(3, 10, "Element set")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the element set which will be used for the load.")]
        [Id(1, 1)]
        public string ElementSetName { get { return _cLoad.RegionName; } set { _cLoad.RegionName = value; } }


        [OrderedDisplayName(0, 10, "X")]
        [CategoryAttribute("Axis point")]
        [DescriptionAttribute("X coordinate of the axis point.")]
        [Id(1, 2)]
        public double X { get { return _cLoad.X; } set { _cLoad.X = value; } }

        [OrderedDisplayName(1, 10, "Y")]
        [CategoryAttribute("Axis point")]
        [DescriptionAttribute("X coordinate of the axis point.")]
        [Id(1, 2)]
        public double Y { get { return _cLoad.Y; } set { _cLoad.Y = value; } }

        [OrderedDisplayName(2, 10, "Z")]
        [CategoryAttribute("Axis point")]
        [DescriptionAttribute("X coordinate of the axis point.")]
        [Id(1, 2)]
        public double Z { get { return _cLoad.Z; } set { _cLoad.Z = value; } }


        [OrderedDisplayName(0, 10, "N1")]
        [CategoryAttribute("Axis direction")]
        [DescriptionAttribute("Axis component in the direction of the first axis.")]
        [Id(1, 3)]
        public double N1 { get { return _cLoad.N1; } set { _cLoad.N1 = value; } }
        
        [OrderedDisplayName(1, 10, "N2")]
        [CategoryAttribute("Axis direction")]
        [DescriptionAttribute("Axis component in the direction of the second axis.")]
        [Id(1, 3)]
        public double N2 { get { return _cLoad.N2; } set { _cLoad.N2 = value; } }

        [OrderedDisplayName(2, 10, "N3")]
        [CategoryAttribute("Axis direction")]
        [DescriptionAttribute("Axis component in the direction of the third axis.")]
        [Id(1, 3)]
        public double N3 { get { return _cLoad.N3; } set { _cLoad.N3 = value; } }

        [OrderedDisplayName(0, 10, "Rot. speed square")]
        [CategoryAttribute("Magnitude")]
        [DescriptionAttribute("The square of the rotational speed omega.")]
        [Id(1, 4)]
        public double RotationalSpeed2 { get { return _cLoad.RotationalSpeed2; } set { _cLoad.RotationalSpeed2 = value; } }



        // Constructors                                                                                                             
        public ViewCentrifLoad(CaeModel.CentrifLoad cLoad)
        {
            // the order is important
            _cLoad = cLoad;

            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.PartName, CaeGlobals.Tools.GetPropertyName(() => this.PartName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ElementSetName, CaeGlobals.Tools.GetPropertyName(() => this.ElementSetName));

            base.SetBase(_cLoad, regionTypePropertyNamePairs);
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }



        // Methods                                                                                                                  
        public override CaeModel.Load GetBase()
        {
            return _cLoad;
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
