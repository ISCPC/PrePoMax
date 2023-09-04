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
    public class ViewSTLoad : ViewLoad
    {
        // Variables                                                                                                                
        private CaeModel.STLoad _stLoad;


        // Properties                                                                                                               
        public override string Name { get { return _stLoad.Name; } set { _stLoad.Name = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Surface")]
        [DescriptionAttribute("Select the surface for the creation of the load.")]
        [Id(3, 2)]
        public string SurfaceName { get { return _stLoad.SurfaceName; } set { _stLoad.SurfaceName = value; } }
        //
        [CategoryAttribute("Force components")]
        [OrderedDisplayName(0, 10, "F1")]
        [DescriptionAttribute("Value of the force component in the direction of the first axis.")]
        [TypeConverter(typeof(EquationForceConverter))]
        [Id(1, 3)]
        public EquationString F1 { get { return _stLoad.F1.Equation; } set { _stLoad.F1.Equation = value; } }
        //
        [CategoryAttribute("Force components")]
        [OrderedDisplayName(1, 10, "F2")]
        [DescriptionAttribute("Value of the force component in the direction of the second axis.")]
        [TypeConverter(typeof(EquationForceConverter))]
        [Id(2, 3)]
        public EquationString F2 { get { return _stLoad.F2.Equation; } set { _stLoad.F2.Equation = value; } }
        //
        [CategoryAttribute("Force components")]
        [OrderedDisplayName(2, 10, "F3")]
        [DescriptionAttribute("Value of the force component in the direction of the third axis.")]
        [TypeConverter(typeof(EquationForceConverter))]
        [Id(3, 3)]
        public EquationString F3 { get { return _stLoad.F3.Equation; } set { _stLoad.F3.Equation = value; } }
        //
        [CategoryAttribute("Force magnitude")]
        [OrderedDisplayName(0, 10, "Magnitude")]
        [DescriptionAttribute("Value of the surface traction load magnitude.")]
        [TypeConverter(typeof(EquationForceConverter))]
        [Id(1, 4)]
        public EquationString Magnitude { get { return _stLoad.Magnitude.Equation; } set { _stLoad.Magnitude.Equation = value; } }
        //
        [CategoryAttribute("Force phase")]
        [OrderedDisplayName(0, 10, "Phase")]
        [DescriptionAttribute("Value of the surface traction phase.")]
        [TypeConverter(typeof(EquationAngleDegConverter))]
        [Id(1, 5)]
        public EquationString Phase { get { return _stLoad.PhaseDeg.Equation; } set { _stLoad.PhaseDeg.Equation = value; } }
        //
        public override string AmplitudeName { get { return _stLoad.AmplitudeName; } set { _stLoad.AmplitudeName = value; } }
        public override System.Drawing.Color Color { get { return _stLoad.Color; } set { _stLoad.Color = value; } }


        // Constructors                                                                                                             
        public ViewSTLoad(CaeModel.STLoad stLoad)
        {
            // The order is important
            _stLoad = stLoad;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SurfaceName));
            //
            SetBase(_stLoad, regionTypePropertyNamePairs);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            // 2D
            DynamicCustomTypeDescriptor.GetProperty(nameof(F3)).SetIsBrowsable(!stLoad.TwoD);
            // Phase
            DynamicCustomTypeDescriptor.GetProperty(nameof(Phase)).SetIsBrowsable(stLoad.Complex);
        }



        // Methods                                                                                                                  
        public override CaeModel.Load GetBase()
        {
            return _stLoad;
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
