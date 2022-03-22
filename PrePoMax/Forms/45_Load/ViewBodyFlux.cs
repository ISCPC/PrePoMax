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
    public class ViewBodyFlux : ViewLoad
    {
        // Variables                                                                                                                
        private CaeModel.BodyFlux _flux;


        // Properties                                                                                                               
        public override string Name { get { return _flux.Name; } set { _flux.Name = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Part")]
        [DescriptionAttribute("Select the part for the creation of the load.")]
        [Id(3, 2)]
        public string PartName { get { return _flux.RegionName; } set { _flux.RegionName = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(3, 10, "Element set")]
        [DescriptionAttribute("Select the element set for the creation of the load.")]
        [Id(4, 2)]
        public string ElementSetName { get { return _flux.RegionName; } set { _flux.RegionName = value; } }
        //
        [CategoryAttribute("Magnitude")]
        [OrderedDisplayName(0, 10, "Flux per volume")]
        [DescriptionAttribute("Value of the flux per volume.")]
        [TypeConverter(typeof(StringPowerPerVolumeConverter))]
        [Id(1, 4)]
        public double Magnitude { get { return _flux.Magnitude; } set { _flux.Magnitude = value; } }
        //
        public override string AmplitudeName { get { return _flux.AmplitudeName; } set { _flux.AmplitudeName = value; } }
        public override System.Drawing.Color Color { get { return _flux.Color; } set { _flux.Color = value; } }


        // Constructors                                                                                                             
        public ViewBodyFlux(CaeModel.BodyFlux flux)
        {
            // The order is important
            _flux = flux;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.PartName, nameof(PartName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ElementSetName, nameof(ElementSetName));
            //
            SetBase(_flux, regionTypePropertyNamePairs);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public override CaeModel.Load GetBase()
        {
            return _flux;
        }
        public void PopulateDropDownLists(string[] partNames, string[] elementSetNames, string[] amplitudeNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.PartName, partNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.ElementSetName, elementSetNames);
            PopulateDropDownLists(regionTypeListItemsPairs);
            //
            PopulateAmplitudeNames(amplitudeNames);
        }
    }

}
