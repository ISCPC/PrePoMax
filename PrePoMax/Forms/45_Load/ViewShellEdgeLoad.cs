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
    public class ViewShellEdgeLoad : ViewLoad
    {
        // Variables                                                                                                                
        private ShellEdgeLoad _shellEdgeLoad;


        // Properties                                                                                                               
        public override string Name { get { return _shellEdgeLoad.Name; } set { _shellEdgeLoad.Name = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Surface")]
        [DescriptionAttribute("Select the surface for the creation of the load.")]
        [Id(3, 2)]
        public string SurfaceName { get { return _shellEdgeLoad.SurfaceName; } set {_shellEdgeLoad.SurfaceName = value;} }
        //
        [CategoryAttribute("Edge load magnitude")]
        [OrderedDisplayName(0, 10, "Magnitude")]
        [DescriptionAttribute("Magnitude of the edge load.")]
        [TypeConverter(typeof(EquationForcePerLengthConverter))]
        [Id(1, 3)]
        public string Magnitude
        {
            get { return _shellEdgeLoad.Magnitude.Equation; }
            set { _shellEdgeLoad.Magnitude.Equation = value; }
        }
        //
        [CategoryAttribute("Edge load phase")]
        [OrderedDisplayName(0, 10, "Phase")]
        [DescriptionAttribute("Phase of the edge load.")]
        [TypeConverter(typeof(EquationAngleDegConverter))]
        [Id(1, 4)]
        public string Phase { get { return _shellEdgeLoad.PhaseDeg.Equation; } set { _shellEdgeLoad.PhaseDeg.Equation = value; } }
        //
        public override string AmplitudeName
        {
            get { return _shellEdgeLoad.AmplitudeName; }
            set { _shellEdgeLoad.AmplitudeName = value; }
        }
        public override System.Drawing.Color Color { get { return _shellEdgeLoad.Color; } set { _shellEdgeLoad.Color = value; } }


        // Constructors                                                                                                             
        public ViewShellEdgeLoad(ShellEdgeLoad shellEdgeLoad)
        {
            _shellEdgeLoad = shellEdgeLoad;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SurfaceName));
            //
            SetBase(_shellEdgeLoad, regionTypePropertyNamePairs);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            // Phase
            DynamicCustomTypeDescriptor.GetProperty(nameof(Phase)).SetIsBrowsable(_shellEdgeLoad.Complex);
        }


        // Methods                                                                                                                  
        public override Load GetBase()
        {
            return _shellEdgeLoad;
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
