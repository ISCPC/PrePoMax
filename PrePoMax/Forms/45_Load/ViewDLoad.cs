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
    public class ViewDLoad : ViewLoad
    {
        // Variables                                                                                                                
        private DLoad _dLoad;


        // Properties                                                                                                               
        public override string Name { get { return _dLoad.Name; } set { _dLoad.Name = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Surface")]
        [DescriptionAttribute("Select the surface for the creation of the load.")]
        [Id(3, 2)]
        public string SurfaceName { get { return _dLoad.SurfaceName; } set {_dLoad.SurfaceName = value;} }
        //
        [CategoryAttribute("Magnitude")]
        [OrderedDisplayName(0, 10, "Pressure")]
        [DescriptionAttribute("Value of the pressure load.")]
        [TypeConverter(typeof(StringPressureConverter))]
        [Id(1, 3)]
        public double Magnitude { get { return _dLoad.Magnitude; } set { _dLoad.Magnitude = value; } }
        //
        public override string AmplitudeName { get { return _dLoad.AmplitudeName; } set { _dLoad.AmplitudeName = value; } }
        public override System.Drawing.Color Color { get { return _dLoad.Color; } set { _dLoad.Color = value; } }


        // Constructors                                                                                                             
        public ViewDLoad(DLoad dLoad)
        {
            _dLoad = dLoad;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SurfaceName));
            //
            SetBase(_dLoad, regionTypePropertyNamePairs);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public override Load GetBase()
        {
            return _dLoad;
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
