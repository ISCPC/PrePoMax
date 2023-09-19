using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using System.Drawing.Design;
using CaeModel;

namespace PrePoMax
{
    [Serializable]
    public class ViewHydrostaticPressureLoad : ViewLoad
    {
        // Variables                                                                                                                
        private HydrostaticPressure _hpLoad;
        private ItemSetData _singlePointItemSetData;
        private ItemSetData _twoPointPointItemSetData;


        // Properties                                                                                                               
        public override string Name { get { return _hpLoad.Name; } set { _hpLoad.Name = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Surface")]
        [DescriptionAttribute("Select the surface for the creation of the load.")]
        [Id(3, 2)]
        public string SurfaceName { get { return _hpLoad.RegionName; } set { _hpLoad.RegionName = value; } }
        //                                                                                                                          
        [Category("First point coordinates")]
        [OrderedDisplayName(0, 10, "By selection")]
        [DescriptionAttribute("Use selection for the definition of the first point.")]
        [EditorAttribute(typeof(SinglePointDataEditor), typeof(UITypeEditor))]
        [Id(1, 3)]
        public ItemSetData FirstPointItemSet
        {
            get { return _singlePointItemSetData; }
            set
            {
                if (value != _singlePointItemSetData)
                    _singlePointItemSetData = value;
            }
        }
        //
        [CategoryAttribute("First point coordinates")]
        [OrderedDisplayName(1, 10, "X")]
        [DescriptionAttribute("X coordinate of the first point.")]
        [TypeConverter(typeof(EquationLengthConverter))]
        [Id(2, 3)]
        public EquationString X1 { get { return _hpLoad.X1.Equation; } set { _hpLoad.X1.Equation = value; } }
        //
        [CategoryAttribute("First point coordinates")]
        [OrderedDisplayName(2, 10, "Y")]
        [DescriptionAttribute("Y coordinate of the first point.")]
        [TypeConverter(typeof(EquationLengthConverter))]
        [Id(3, 3)]
        public EquationString Y1 { get { return _hpLoad.Y1.Equation; } set { _hpLoad.Y1.Equation = value; } }
        //
        [CategoryAttribute("First point coordinates")]
        [OrderedDisplayName(3, 10, "Z")]
        [DescriptionAttribute("Z coordinate of the first point.")]
        [TypeConverter(typeof(EquationLengthConverter))]
        [Id(4, 3)]
        public EquationString Z1 { get { return _hpLoad.Z1.Equation; } set { _hpLoad.Z1.Equation = value; } }
        //                                                                                                              
        [CategoryAttribute("First point pressure magnitude")]
        [OrderedDisplayName(0, 10, "Magnitude")]
        [DescriptionAttribute("Pressure magnitude at the first point.")]
        [TypeConverter(typeof(EquationPressureConverter))]
        [Id(1, 4)]
        public EquationString FirstPointPressure
        {
            get { return _hpLoad.FirstPointPressure.Equation; }
            set { _hpLoad.FirstPointPressure.Equation = value; }
        }
        //                                                                                                              
        [Category("Second point coordinates")]
        [OrderedDisplayName(0, 10, "By selection ")]        // must be a different name than for the first point !!!
        [DescriptionAttribute("Use selection for the definition of the second point.")]
        [EditorAttribute(typeof(SinglePointDataEditor), typeof(UITypeEditor))]
        [Id(1, 5)]
        public ItemSetData SecondPointItemSet
        {
            get { return _singlePointItemSetData; }
            set
            {
                if (value != _singlePointItemSetData)
                    _singlePointItemSetData = value;
            }
        }
        //
        [CategoryAttribute("Second point coordinates")]    // must be a different name than for the first point !!!
        [OrderedDisplayName(1, 10, "X ")]
        [DescriptionAttribute("X coordinate of the second point.")]
        [TypeConverter(typeof(EquationLengthConverter))]
        [Id(2, 5)]
        public EquationString X2 { get { return _hpLoad.X2.Equation; } set { _hpLoad.X2.Equation = value; } }
        //
        [CategoryAttribute("Second point coordinates")]    // must be a different name than for the first point !!!
        [OrderedDisplayName(2, 10, "Y ")]
        [DescriptionAttribute("Y coordinate of the second point.")]
        [TypeConverter(typeof(EquationLengthConverter))]
        [Id(3, 5)]
        public EquationString Y2 { get { return _hpLoad.Y2.Equation; } set { _hpLoad.Y2.Equation = value; } }
        //
        [CategoryAttribute("Second point coordinates")]    // must be a different name than for the first point !!!
        [OrderedDisplayName(3, 10, "Z ")]
        [DescriptionAttribute("Z coordinate of the second point.")]
        [TypeConverter(typeof(EquationLengthConverter))]
        [Id(4, 5)]
        public EquationString Z2 { get { return _hpLoad.Z2.Equation; } set { _hpLoad.Z2.Equation = value; } }
        //                                                                                                                          
        [CategoryAttribute("Second point pressure magnitude")]        // must be a different name than for the first point !!!
        [OrderedDisplayName(0, 10, "Magnitude ")]
        [DescriptionAttribute("Pressure magnitude at the second point.")]
        [TypeConverter(typeof(EquationPressureConverter))]
        [Id(1, 6)]
        public EquationString SecondPointPressure
        {
            get { return _hpLoad.SecondPointPressure.Equation; }
            set { _hpLoad.SecondPointPressure.Equation = value; }
        }
        //                                                                                                                          
        [Category("Pressure change direction")]
        [OrderedDisplayName(0, 10, "By selection  ")]    // must be a different name than for the first point !!!
        [DescriptionAttribute("Use selection for the definition of the pressure direction.")]
        [EditorAttribute(typeof(SinglePointDataEditor), typeof(UITypeEditor))]
        [Id(1, 7)]
        public ItemSetData PressureDirectionItemSet
        {
            get { return _twoPointPointItemSetData; }
            set
            {
                if (value != _twoPointPointItemSetData)
                    _twoPointPointItemSetData = value;
            }
        }
        //
        [CategoryAttribute("Pressure change direction")]
        [OrderedDisplayName(0, 10, "N1")]
        [DescriptionAttribute("Direction component in the direction of the first axis.")]
        [TypeConverter(typeof(EquationLengthConverter))]
        [Id(2, 7)]
        public EquationString N1 { get { return _hpLoad.N1.Equation; } set { _hpLoad.N1.Equation = value; } }
        //
        [CategoryAttribute("Pressure change direction")]
        [OrderedDisplayName(1, 10, "N2")]
        [DescriptionAttribute("Direction component in the direction of the second axis.")]
        [TypeConverter(typeof(EquationLengthConverter))]
        [Id(3, 7)]
        public EquationString N2 { get { return _hpLoad.N2.Equation; } set { _hpLoad.N2.Equation = value; } }
        //
        [CategoryAttribute("Pressure change direction")]
        [OrderedDisplayName(2, 10, "N3")]
        [DescriptionAttribute("Direction component in the direction of the third axis.")]
        [TypeConverter(typeof(EquationLengthConverter))]
        [Id(4, 7)]
        public EquationString N3 { get { return _hpLoad.N3.Equation; } set { _hpLoad.N3.Equation = value; } }
        //
        [CategoryAttribute("Pressure phase")]
        [OrderedDisplayName(0, 10, "Phase")]
        [DescriptionAttribute("Value of the pressure phase.")]
        [TypeConverter(typeof(EquationAngleDegConverter))]
        [Id(1, 8)]
        public EquationString Phase { get { return _hpLoad.PhaseDeg.Equation; } set { _hpLoad.PhaseDeg.Equation = value; } }
        //
        //
        [CategoryAttribute("Pressure cutoff")]
        [OrderedDisplayName(0, 10, "Cutoff")]
        [DescriptionAttribute("A positive pressure cutoff sets all positive pressure values to 0, " +
                              "while a negative pressure cutoff sets all negative pressure values to 0.")]
        [Id(1, 9)]
        public HydrostaticPressureCutoffEnum HydrostaticPressureType
        {
            get { return _hpLoad.HydrostaticPressureCutoff; }
            set { _hpLoad.HydrostaticPressureCutoff = value; }
        }
        //
        public override string AmplitudeName { get { return _hpLoad.AmplitudeName; } set { _hpLoad.AmplitudeName = value; } }
        public override System.Drawing.Color Color { get { return _hpLoad.Color; } set { _hpLoad.Color = value; } }


        // Constructors                                                                                                             
        public ViewHydrostaticPressureLoad(HydrostaticPressure hpLoad)
        {
            // The order is important
            _hpLoad = hpLoad;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SurfaceName));
            //
            SetBase(_hpLoad, regionTypePropertyNamePairs);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            //
            _singlePointItemSetData = new ItemSetData(); // needed to display ItemSetData.ToString()
            _singlePointItemSetData.ToStringType = ItemSetDataToStringType.SelectSinglePoint;
            _twoPointPointItemSetData = new ItemSetData(); // needed to display ItemSetData.ToString()
            _twoPointPointItemSetData.ToStringType = ItemSetDataToStringType.SelectTwoPoints;
            // 2D
            DynamicCustomTypeDescriptor.GetProperty(nameof(Z1)).SetIsBrowsable(!hpLoad.TwoD);
            DynamicCustomTypeDescriptor.GetProperty(nameof(Z2)).SetIsBrowsable(!hpLoad.TwoD);
            DynamicCustomTypeDescriptor.GetProperty(nameof(N3)).SetIsBrowsable(!hpLoad.TwoD);
            // Phase
            DynamicCustomTypeDescriptor.GetProperty(nameof(Phase)).SetIsBrowsable(hpLoad.Complex);
        }


        // Methods                                                                                                                  
        public override Load GetBase()
        {
            return _hpLoad;
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
