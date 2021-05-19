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
    public class ViewRadiateFlux : ViewLoad
    {
        // Variables                                                                                                                
        private RadiateFlux _radiateFlux;


        // Properties                                                                                                               
        public override string Name { get { return _radiateFlux.Name; } set { _radiateFlux.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Cavity radiation")]
        [DescriptionAttribute("Selected faces belong to a radiation in a cavity.")]
        [Id(2, 1)]
        public bool CavityRadiation
        {
            get { return _radiateFlux.CavityRadiation; }
            set
            {
                _radiateFlux.CavityRadiation = value;
                DynamicCustomTypeDescriptor.GetProperty(nameof(CavityName)).SetIsBrowsable(_radiateFlux.CavityRadiation);
            }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Cavity name")]
        [DescriptionAttribute("To separate cavities enter the cavity name (at most 3 characters).")]
        [Id(3, 1)]
        public string CavityName { get { return _radiateFlux.CavityName; } set { _radiateFlux.CavityName = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Surface")]
        [DescriptionAttribute("Select the surface for the creation of the load.")]
        [Id(3, 2)]
        public string SurfaceName { get { return _radiateFlux.SurfaceName; } set {_radiateFlux.SurfaceName = value;} }
        //
        [CategoryAttribute("Parameters")]
        [OrderedDisplayName(2, 10, "Sink temperature")]
        [DescriptionAttribute("Value of the sink temperature.")]
        [TypeConverter(typeof(StringTemperatureConverter))]
        [Id(3, 3)]
        public double SinkTemperature { get { return _radiateFlux.SinkTemperature; } set { _radiateFlux.SinkTemperature = value; } }
        //
        [CategoryAttribute("Parameters")]
        [OrderedDisplayName(3, 10, "Emissivity")]
        [DescriptionAttribute("Value of the surface emissivity.")]
        [Id(4, 3)]
        public double Emissivity { get { return _radiateFlux.Emissivity; } set { _radiateFlux.Emissivity = value; } }
        //
        public override System.Drawing.Color Color { get { return _radiateFlux.Color; } set { _radiateFlux.Color = value; } }


        // Constructors                                                                                                             
        public ViewRadiateFlux(RadiateFlux radiateFlux)
        {
            _radiateFlux = radiateFlux;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SurfaceName));
            //
            SetBase(_radiateFlux, regionTypePropertyNamePairs);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            // Now lets display Yes/No instead of True/False
            DynamicCustomTypeDescriptor.RenameBooleanPropertyToYesNo(nameof(CavityRadiation));
            //
            CavityRadiation = _radiateFlux.CavityRadiation; // update CavityName visibility
        }


        // Methods                                                                                                                  
        public override Load GetBase()
        {
            return _radiateFlux;
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
