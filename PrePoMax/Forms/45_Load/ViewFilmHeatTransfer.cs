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
    public class ViewFilmHeatTransfer : ViewLoad
    {
        // Variables                                                                                                                
        private FilmHeatTransfer _filmHeatTransfer;


        // Properties                                                                                                               
        public override string Name { get { return _filmHeatTransfer.Name; } set { _filmHeatTransfer.Name = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Surface")]
        [DescriptionAttribute("Select the surface for the creation of the load.")]
        [Id(3, 2)]
        public string SurfaceName { get { return _filmHeatTransfer.SurfaceName; } set {_filmHeatTransfer.SurfaceName = value;} }
        //
        [CategoryAttribute("Parameters")]
        [OrderedDisplayName(0, 10, "Sink temperature")]
        [DescriptionAttribute("Value of the sink temperature.")]
        [TypeConverter(typeof(StringTemperatureConverter))]
        [Id(1, 3)]
        public double SinkTemperature
        {
            get { return _filmHeatTransfer.SinkTemperature; }
            set { _filmHeatTransfer.SinkTemperature = value; }
        }
        //
        [CategoryAttribute("Parameters")]
        [OrderedDisplayName(1, 10, "Film coefficient")]
        [DescriptionAttribute("Value of the film coefficient.")]
        [TypeConverter(typeof(StringHeatTransferCoefficientConverter))]
        [Id(2, 3)]
        public double FilmCoefficient
        {
            get { return _filmHeatTransfer.FilmCoefficient; }
            set { _filmHeatTransfer.FilmCoefficient = value; }
        }
        //
        public override string AmplitudeName
        {
            get { return _filmHeatTransfer.AmplitudeName; }
            set { _filmHeatTransfer.AmplitudeName = value; }
        }
        public override System.Drawing.Color Color
        {
            get { return _filmHeatTransfer.Color; }
            set { _filmHeatTransfer.Color = value; }
        }


        // Constructors                                                                                                             
        public ViewFilmHeatTransfer(FilmHeatTransfer filmHeatTransfer)
        {
            _filmHeatTransfer = filmHeatTransfer;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SurfaceName));
            //
            SetBase(_filmHeatTransfer, regionTypePropertyNamePairs);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public override Load GetBase()
        {
            return _filmHeatTransfer;
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
