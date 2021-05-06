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
    public class ViewDefinedTemperature : ViewDefinedField
    {
        // Variables                                                                                                                
        private CaeModel.DefinedTemperature _definedTemperature;


        // Properties                                                                                                               
        public override string Name { get { return _definedTemperature.Name; } set { _definedTemperature.Name = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Node set")]
        [DescriptionAttribute("Select the node set for the creation of the defined temperature.")]
        public string NodeSetName { get { return _definedTemperature.RegionName; } set { _definedTemperature.RegionName = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(4, 10, "Surface")]
        [DescriptionAttribute("Select the surface for the creation of the defined temperature.")]
        public string SurfaceName { get { return _definedTemperature.RegionName; } set { _definedTemperature.RegionName = value; } }
        //
        [OrderedDisplayName(0, 10, "Temperature magnitude")]
        [CategoryAttribute("Magnitude")]
        [DescriptionAttribute("Value of the defined temperature magnitude.")]
        [TypeConverter(typeof(StringTemperatureConverter))]
        public double Temperature
        {
            get { return _definedTemperature.Temperature; }
            set { _definedTemperature.Temperature = value; }
        }

        // Constructors                                                                                                             
        public ViewDefinedTemperature(CaeModel.DefinedTemperature definedTemperature)
        {
            // The order is important
            _definedTemperature = definedTemperature;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.NodeSetName, nameof(NodeSetName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SurfaceName));
            //
            base.SetBase(_definedTemperature, regionTypePropertyNamePairs);
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public override CaeModel.DefinedField GetBase()
        {
            return _definedTemperature;
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
