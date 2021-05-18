using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using DynamicTypeDescriptor;
using System.ComponentModel;
using CaeGlobals;
using CaeModel;

namespace PrePoMax
{
    [Serializable]
    public class ViewRadiateLoad : ViewLoad
    {
        // Variables                                                                                                                
        private RadiateLoad _radiateLoad;


        // Properties                                                                                                               
        public override string Name { get { return _radiateLoad.Name; } set { _radiateLoad.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Cavity radiation")]
        [DescriptionAttribute("Selected faces belong to a radiation in a cavity.")]
        [Id(2, 1)]
        public bool CavityRadiation
        {
            get { return _radiateLoad.CavityRadiation; }
            set
            {
                _radiateLoad.CavityRadiation = value;
                DynamicCustomTypeDescriptor.GetProperty(nameof(CavityName)).SetIsBrowsable(_radiateLoad.CavityRadiation);
            }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Cavity name")]
        [DescriptionAttribute("To separate cavities enter the cavity name (at most 3 characters).")]
        [Id(3, 1)]
        public string CavityName { get { return _radiateLoad.CavityName; } set { _radiateLoad.CavityName = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Surface")]
        [DescriptionAttribute("Select the surface for the creation of the load.")]
        [Id(3, 2)]
        public string SurfaceName { get { return _radiateLoad.SurfaceName; } set {_radiateLoad.SurfaceName = value;} }
        //
        [CategoryAttribute("Parameters")]
        [OrderedDisplayName(2, 10, "Sink temperature")]
        [DescriptionAttribute("Value of the sink temperature.")]
        [TypeConverter(typeof(StringTemperatureConverter))]
        [Id(3, 3)]
        public double SinkTemperature { get { return _radiateLoad.SinkTemperature; } set { _radiateLoad.SinkTemperature = value; } }
        //
        [CategoryAttribute("Parameters")]
        [OrderedDisplayName(3, 10, "Emissivity")]
        [DescriptionAttribute("Value of the surface emissivity.")]
        [Id(4, 3)]
        public double Emissivity { get { return _radiateLoad.Emissivity; } set { _radiateLoad.Emissivity = value; } }
        //
        public override System.Drawing.Color Color { get { return _radiateLoad.Color; } set { _radiateLoad.Color = value; } }


        // Constructors                                                                                                             
        public ViewRadiateLoad(RadiateLoad radiateLoad)
        {
            _radiateLoad = radiateLoad;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SurfaceName));
            //
            SetBase(_radiateLoad, regionTypePropertyNamePairs);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            // Now lets display Yes/No instead of True/False
            DynamicCustomTypeDescriptor.RenameBooleanPropertyToYesNo(nameof(CavityRadiation));
            //
            CavityRadiation = _radiateLoad.CavityRadiation; // update CavityName visibility
        }


        // Methods                                                                                                                  
        public override Load GetBase()
        {
            return _radiateLoad;
        }
        public void PopululateDropDownLists(string[] surfaceNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.SurfaceName, surfaceNames);
            PopululateDropDownLists(regionTypeListItemsPairs);
        }
    }
}
