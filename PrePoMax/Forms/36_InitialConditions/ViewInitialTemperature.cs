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
    public class ViewInitialTemperature : ViewInitialCondition
    {
        // Variables                                                                                                                
        private CaeModel.InitialTemperature _initialTemperature;


        // Properties                                                                                                               
        public override string Name { get { return _initialTemperature.Name; } set { _initialTemperature.Name = value; } }
        //
        [OrderedDisplayName(1, 10, "Temperature")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Set the value for the temperature.")]
        [TypeConverter(typeof(StringTemperatureConverter))]
        public double Temperature
        {
            get { return _initialTemperature.Temperature; }
            set { _initialTemperature.Temperature = value; }
        }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Node set")]
        [DescriptionAttribute("Select the node set for the creation of the initial temperature.")]
        public string NodeSetName { get { return _initialTemperature.RegionName; } set { _initialTemperature.RegionName = value; } }
        ////
        //[CategoryAttribute("Region")]
        //[OrderedDisplayName(3, 10, "Reference point")]
        //[DescriptionAttribute("Select the reference point for the creation of the initial temperature.")]
        //public string ReferencePointName { get { return _initialTemperature.RegionName; } set { _initialTemperature.RegionName = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(4, 10, "Surface")]
        [DescriptionAttribute("Select the surface for the creation of the initial temperature.")]
        public string SurfaceName { get { return _initialTemperature.RegionName; } set { _initialTemperature.RegionName = value; } }

       
        // Constructors                                                                                                             
        public ViewInitialTemperature(CaeModel.InitialTemperature initialTemperature)
        {
            // The order is important
            _initialTemperature = initialTemperature;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.NodeSetName, nameof(NodeSetName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SurfaceName));
            //
            base.SetBase(_initialTemperature, regionTypePropertyNamePairs);
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public override CaeModel.InitialCondition GetBase()
        {
            return _initialTemperature;
        }
        public void PopululateDropDownLists(string[] nodeSetNames, string[] surfaceNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.NodeSetName, nodeSetNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.SurfaceName, surfaceNames);
            base.PopululateDropDownLists(regionTypeListItemsPairs);
        }
    }



   
}
