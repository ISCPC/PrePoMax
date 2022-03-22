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
    public class ViewDFlux : ViewLoad
    {
        // Variables                                                                                                                
        private DFlux _flux;


        // Properties                                                                                                               
        public override string Name { get { return _flux.Name; } set { _flux.Name = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Surface")]
        [DescriptionAttribute("Select the surface for the creation of the load.")]
        [Id(3, 2)]
        public string SurfaceName { get { return _flux.SurfaceName; } set {_flux.SurfaceName = value;} }
        //
        [CategoryAttribute("Magnitude")]
        [OrderedDisplayName(0, 10, "Flux per area")]
        [DescriptionAttribute("Value of the flux per area.")]
        [TypeConverter(typeof(StringPowerPerAreaConverter))]
        [Id(1, 3)]
        public double Magnitude { get { return _flux.Magnitude; } set { _flux.Magnitude = value; } }
        //
        public override string AmplitudeName { get { return _flux.AmplitudeName; } set { _flux.AmplitudeName = value; } }
        public override System.Drawing.Color Color { get { return _flux.Color; } set { _flux.Color = value; } }


        // Constructors                                                                                                             
        public ViewDFlux(DFlux flux)
        {
            _flux = flux;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SurfaceName));
            //
            SetBase(_flux, regionTypePropertyNamePairs);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public override Load GetBase()
        {
            return _flux;
        }
        public void PopulateDropDownLists(string[] surfaceNames, string[] amplitudeNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.SurfaceName, surfaceNames);
            PopulateDropDownLists(regionTypeListItemsPairs);
            //
            PopulateAmplitudeNames(amplitudeNames);
        }
    }
}
