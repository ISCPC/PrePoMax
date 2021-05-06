using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using System.Drawing;

namespace PrePoMax
{
    [Serializable]
    public class ViewTemperatureBC : ViewBoundaryCondition
    {
        // Variables                                                                                                                
        private CaeModel.TemperatureBC _temperatureBC;


        // Properties                                                                                                               
        public override string Name { get { return _temperatureBC.Name; } set { _temperatureBC.Name = value; } }
        public override string NodeSetName { get { return _temperatureBC.RegionName; } set { _temperatureBC.RegionName = value; } }
        public override string ReferencePointName
        {
            get { return _temperatureBC.RegionName; }
            set { _temperatureBC.RegionName = value; }
        }
        public override string SurfaceName { get { return _temperatureBC.RegionName; } set { _temperatureBC.RegionName = value; } }
        //
        [CategoryAttribute("Temperature magnitude")]
        [OrderedDisplayName(0, 10, "Magnitude")]
        [DescriptionAttribute("Value of the temperature.")]
        [TypeConverter(typeof(StringTemperatureConverter))]
        [Id(1, 3)]
        public double Temperature { get { return _temperatureBC.Temperature; } set { _temperatureBC.Temperature = value; } }
        //
        public override Color Color { get { return _temperatureBC.Color; } set { _temperatureBC.Color = value; } }


        // Constructors                                                                                                             
        public ViewTemperatureBC(CaeModel.TemperatureBC temperatureBC)
        {
            // The order is important
            _temperatureBC = temperatureBC;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.NodeSetName, nameof(NodeSetName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SurfaceName));
            // Must be here to correctly hide the RPs defined in base class
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ReferencePointName, nameof(ReferencePointName));
            //
            base.SetBase(_temperatureBC, regionTypePropertyNamePairs);
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public override CaeModel.BoundaryCondition GetBase()
        {
            return _temperatureBC;
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
