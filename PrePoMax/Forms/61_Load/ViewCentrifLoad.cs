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
        private CaeModel.CentrifLoad _cenLoad;


        // Properties                                                                                                               
        public override string Name { get { return _cenLoad.Name; } set { _cenLoad.Name = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Part")]
        [DescriptionAttribute("Select the part for the creation of the load.")]
        [Id(3, 2)]
        public string PartName { get { return _cenLoad.RegionName; } set { _cenLoad.RegionName = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(3, 10, "Element set")]
        [DescriptionAttribute("Select the element set for the creation of the load.")]
        [Id(4, 2)]
        public string ElementSetName { get { return _cenLoad.RegionName; } set { _cenLoad.RegionName = value; } }
        //
        [CategoryAttribute("Axis point")]
        [OrderedDisplayName(0, 10, "X")]
        [DescriptionAttribute("X coordinate of the axis point.")]
        [TypeConverter(typeof(CaeModel.StringLengthConverter))]
        [Id(1, 3)]
        public double X { get { return _cenLoad.X; } set { _cenLoad.X = value; } }
        //
        [CategoryAttribute("Axis point")]
        [OrderedDisplayName(1, 10, "Y")]
        [DescriptionAttribute("X coordinate of the axis point.")]
        [TypeConverter(typeof(CaeModel.StringLengthConverter))]
        [Id(2, 3)]
        public double Y { get { return _cenLoad.Y; } set { _cenLoad.Y = value; } }
        //
        [CategoryAttribute("Axis point")]
        [OrderedDisplayName(2, 10, "Z")]
        [DescriptionAttribute("X coordinate of the axis point.")]
        [TypeConverter(typeof(CaeModel.StringLengthConverter))]
        [Id(3, 3)]
        public double Z { get { return _cenLoad.Z; } set { _cenLoad.Z = value; } }
        //
        [CategoryAttribute("Axis direction")]
        [OrderedDisplayName(0, 10, "N1")]
        [DescriptionAttribute("Axis component in the direction of the first axis.")]
        [TypeConverter(typeof(CaeModel.StringLengthConverter))]
        [Id(1, 4)]
        public double N1 { get { return _cenLoad.N1; } set { _cenLoad.N1 = value; } }
        //
        [CategoryAttribute("Axis direction")]
        [OrderedDisplayName(1, 10, "N2")]
        [DescriptionAttribute("Axis component in the direction of the second axis.")]
        [TypeConverter(typeof(CaeModel.StringLengthConverter))]
        [Id(2, 4)]
        public double N2 { get { return _cenLoad.N2; } set { _cenLoad.N2 = value; } }
        //
        [CategoryAttribute("Axis direction")]
        [OrderedDisplayName(2, 10, "N3")]
        [DescriptionAttribute("Axis component in the direction of the third axis.")]
        [TypeConverter(typeof(CaeModel.StringLengthConverter))]
        [Id(3, 4)]
        public double N3 { get { return _cenLoad.N3; } set { _cenLoad.N3 = value; } }
        //
        [CategoryAttribute("Magnitude")]
        [OrderedDisplayName(0, 10, "Rotational speed")]
        [DescriptionAttribute("The rotational speed around the axis defined by the point and direction.")]
        [TypeConverter(typeof(CaeModel.StringRotationalSpeedConverter))]
        [Id(1, 5)]
        public double RotationalSpeed { get { return _cenLoad.RotationalSpeed; } set { _cenLoad.RotationalSpeed = value; } }
        //
        public override System.Drawing.Color Color { get { return _cenLoad.Color; } set { _cenLoad.Color = value; } }


        // Constructors                                                                                                             
        public ViewCentrifLoad(CaeModel.CentrifLoad cLoad)
        {
            // The order is important
            _cenLoad = cLoad;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.PartName, nameof(PartName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ElementSetName, nameof(ElementSetName));
            //
            base.SetBase(_cenLoad, regionTypePropertyNamePairs);
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public override CaeModel.Load GetBase()
        {
            return _cenLoad;
        }
        public void PopululateDropDownLists(string[] partNames, string[] elementSetNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.PartName, partNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.ElementSetName, elementSetNames);
            base.PopululateDropDownLists(regionTypeListItemsPairs);
        }
    }

}
