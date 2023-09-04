using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using System.Drawing;
using System.Drawing.Design;
using CaeModel;

namespace PrePoMax
{
    [Serializable]
    public class ViewSurfaceSpring : ViewSpring
    {
        // Variables                                                                                                                


        // Properties                                                                                                               
        [CategoryAttribute("Region")]
        [OrderedDisplayName(1, 10, "Surface")]
        [DescriptionAttribute("Select the surface for the creation of the constraint definition.")]
        [Id(2, 2)]
        public string SurfaceName { get { return _spring.RegionName; } set { _spring.RegionName = value; } }
        //
        [CategoryAttribute("Stiffness definition")]
        [OrderedDisplayName(0, 10, "Stiffness per area")]
        [DescriptionAttribute("Select yes if stiffness is defined per area of the selected surface.")]
        [Id(1, 3)]
        public bool StiffnessPerArea
        {
            get { return ((SurfaceSpring)_spring).StiffnessPerArea; }
            set
            {
                ((SurfaceSpring)_spring).StiffnessPerArea = value;
                UpdateVisibility();
            }
        }
        //
        [CategoryAttribute("Stiffness")]
        [OrderedDisplayName(0, 10, "K1")]
        [DescriptionAttribute("Value of the stiffness in the direction of the first axis.")]
        [TypeConverter(typeof(EquationForcePerLengthConverter))]
        [Id(1, 4)]
        public override EquationString K1 { get { return _spring.K1.Equation; } set { _spring.K1.Equation = value; } }
        //
        [CategoryAttribute("Stiffness")]
        [OrderedDisplayName(1, 10, "K2")]
        [DescriptionAttribute("Value of the stiffness in the direction of the second axis.")]
        [TypeConverter(typeof(EquationForcePerLengthConverter))]
        [Id(2, 4)]
        public override EquationString K2 { get { return _spring.K2.Equation; } set { _spring.K2.Equation = value; } }
        //
        [CategoryAttribute("Stiffness")]
        [OrderedDisplayName(2, 10, "K3")]
        [DescriptionAttribute("Value of the stiffness in the direction of the third axis.")]
        [TypeConverter(typeof(EquationForcePerLengthConverter))]
        [Id(3, 4)]
        public override EquationString K3 { get { return _spring.K3.Equation; } set { _spring.K3.Equation = value; } }
        //                                  
        //
        [CategoryAttribute("Stiffness per area")]
        [OrderedDisplayName(0, 10, "K1")]
        [DescriptionAttribute("Value of the stiffness per area in the direction of the first axis.")]
        [TypeConverter(typeof(EquationForcePerVolumeConverter))]
        [Id(1, 4)]
        public EquationString K1A { get { return _spring.K1.Equation; } set { _spring.K1.Equation = value; } }
        //
        [CategoryAttribute("Stiffness per area")]
        [OrderedDisplayName(1, 10, "K2")]
        [DescriptionAttribute("Value of the stiffness per area in the direction of the second axis.")]
        [TypeConverter(typeof(EquationForcePerVolumeConverter))]
        [Id(2, 4)]
        public EquationString K2A { get { return _spring.K2.Equation; } set { _spring.K2.Equation = value; } }
        //
        [CategoryAttribute("Stiffness per area")]
        [OrderedDisplayName(2, 10, "K3")]
        [DescriptionAttribute("Value of the stiffness per area in the direction of the third axis.")]
        [TypeConverter(typeof(EquationForcePerVolumeConverter))]
        [Id(3, 4)]
        public EquationString K3A { get { return _spring.K3.Equation; } set { _spring.K3.Equation = value; } }


        // Constructors                                                                                                             
        public ViewSurfaceSpring(SurfaceSpring surfaceSpring)
            : base(surfaceSpring)
        {
            // The order is important
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(MasterSelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SurfaceName));
            //
            SetBase(_spring, regionTypePropertyNamePairs, null);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            // 2D
            DynamicCustomTypeDescriptor.GetProperty(nameof(K3)).SetIsBrowsable(!_spring.TwoD);
            //
            DynamicCustomTypeDescriptor.RenameBooleanPropertyToYesNo(nameof(StiffnessPerArea));
            //
            UpdateVisibility();
        }


        // Methods                                                                                                                  
        public override Constraint GetBase()
        {
            return _spring;
        }
        public void PopulateDropDownLists(string[] surfaceNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.SurfaceName, surfaceNames);
            //
            PopulateDropDownLists(regionTypeListItemsPairs, null);
        }
        private void UpdateVisibility()
        {
            bool visibe = StiffnessPerArea;
            DynamicCustomTypeDescriptor.GetProperty(nameof(K1)).SetIsBrowsable(!visibe);
            DynamicCustomTypeDescriptor.GetProperty(nameof(K2)).SetIsBrowsable(!visibe);
            DynamicCustomTypeDescriptor.GetProperty(nameof(K3)).SetIsBrowsable(!visibe);
            //
            DynamicCustomTypeDescriptor.GetProperty(nameof(K1A)).SetIsBrowsable(visibe);
            DynamicCustomTypeDescriptor.GetProperty(nameof(K2A)).SetIsBrowsable(visibe);
            DynamicCustomTypeDescriptor.GetProperty(nameof(K3A)).SetIsBrowsable(visibe);
        }
    }

}
