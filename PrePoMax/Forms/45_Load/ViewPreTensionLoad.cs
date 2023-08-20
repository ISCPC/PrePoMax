using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using CaeModel;

namespace PrePoMax
{
    [Serializable]
    public class ViewPreTensionLoad : ViewLoad
    {
        // Variables                                                                                                                
        private CustomPropertyDescriptor cpd = null;
        private CaeModel.PreTensionLoad _preTenLoad;


        // Properties                                                                                                               
        public override string Name { get { return _preTenLoad.Name; } set { _preTenLoad.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Type")]
        [DescriptionAttribute("Select the pre-tension type.")]
        [Id(2, 1)]
        public PreTensionLoadType Type 
        { 
            get { return _preTenLoad.Type; } 
            set 
            {
                if (_preTenLoad.Type != value)
                {
                    _preTenLoad.Type = value;
                    //
                    _preTenLoad.SetMagnitudeValue(0);
                }
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(ForceMagnitude));
                cpd.SetIsBrowsable(_preTenLoad.Type == CaeModel.PreTensionLoadType.Force);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(DisplacementMagnitude));
                cpd.SetIsBrowsable(_preTenLoad.Type == CaeModel.PreTensionLoadType.Displacement);
            }
        }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Surface")]
        [DescriptionAttribute("Select the surface for the creation of the load.")]
        [Id(3, 2)]
        public string SurfaceName { get { return _preTenLoad.SurfaceName; } set { _preTenLoad.SurfaceName = value; } }
        //
        [CategoryAttribute("Pre-tension direction")]
        [OrderedDisplayName(0, 10, "Auto compute")]
        [DescriptionAttribute("Auto compute the pre-tension direction.")]
        [Id(1, 3)]
        public bool AutoComputeDirection 
        { 
            get { return _preTenLoad.AutoComputeDirection; }
            set 
            {
                _preTenLoad.AutoComputeDirection = value;
                //
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(X));
                cpd.SetIsBrowsable(!_preTenLoad.AutoComputeDirection);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(Y));
                cpd.SetIsBrowsable(!_preTenLoad.AutoComputeDirection);
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(Z));
                cpd.SetIsBrowsable(!_preTenLoad.AutoComputeDirection);
            }
        }
        //
        [CategoryAttribute("Pre-tension direction")]
        [OrderedDisplayName(1, 10, "X")]
        [DescriptionAttribute("X component of the pre-tension direction.")]
        [TypeConverter(typeof(EquationLengthConverter))]
        [Id(2, 3)]
        public string X { get { return _preTenLoad.X.Equation; } set { _preTenLoad.X.Equation = value; } }
        //
        [CategoryAttribute("Pre-tension direction")]
        [OrderedDisplayName(2, 10, "Y")]
        [DescriptionAttribute("Y component of the pre-tension direction.")]
        [TypeConverter(typeof(EquationLengthConverter))]
        [Id(3, 3)]
        public string Y { get { return _preTenLoad.Y.Equation; } set { _preTenLoad.Y.Equation = value; } }
        //
        [CategoryAttribute("Pre-tension direction")]
        [OrderedDisplayName(3, 10, "Z")]
        [DescriptionAttribute("Z component of the pre-tension direction.")]
        [TypeConverter(typeof(EquationLengthConverter))]
        [Id(4, 3)]
        public string Z { get { return _preTenLoad.Z.Equation; } set { _preTenLoad.Z.Equation = value; } }
        //
        [CategoryAttribute("Magnitude")]
        [OrderedDisplayName(0, 10, "Force")]
        [DescriptionAttribute("Value of the force for the pre-tension load.")]
        [TypeConverter(typeof(EquationForceConverter))]
        [Id(1, 4)]
        public string ForceMagnitude
        {
            get { return _preTenLoad.ForceMagnitude.Equation; }
            set { _preTenLoad.ForceMagnitude.Equation = value; }
        }
        //
        [CategoryAttribute("Magnitude")]
        [OrderedDisplayName(0, 10, "Displacement")]
        [DescriptionAttribute("Value of the displacement for the pre-tension load.")]
        [TypeConverter(typeof(EquationLengthFixedDOFConverter))]
        [Id(1, 5)]
        public string DisplacementMagnitude
        {
            get { return _preTenLoad.DisplacementMagnitude.Equation; }
            set { _preTenLoad.DisplacementMagnitude.Equation = value; }
        }
        //
        public override string AmplitudeName
        {
            get { return _preTenLoad.AmplitudeName; }
            set { _preTenLoad.AmplitudeName = value; }
        }
        public override System.Drawing.Color Color { get { return _preTenLoad.Color; } set { _preTenLoad.Color = value; } }


        // Constructors                                                                                                             
        public ViewPreTensionLoad(CaeModel.PreTensionLoad preTenLoad)
        {
            // The order is important
            _preTenLoad = preTenLoad;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SurfaceName));
            //
            base.SetBase(_preTenLoad, regionTypePropertyNamePairs);
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            // Now lets display Yes/No instead of True/False
            base.DynamicCustomTypeDescriptor.RenameBooleanPropertyToYesNo("AutoComputeDirection");
            // 2D
            DynamicCustomTypeDescriptor.GetProperty(nameof(Z)).SetIsBrowsable(!preTenLoad.TwoD);
            // Set initial visibilities
            Type = _preTenLoad.Type;
            AutoComputeDirection = _preTenLoad.AutoComputeDirection;
        }


        // Methods                                                                                                                  
        public override CaeModel.Load GetBase()
        {
            return _preTenLoad;
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
