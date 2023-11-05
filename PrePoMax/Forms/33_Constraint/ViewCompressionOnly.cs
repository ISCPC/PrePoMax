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

namespace PrePoMax
{
    [Serializable]
    public class ViewCompressionOnly : ViewConstraint
    {
        // Variables                                                                                                                
        protected CaeModel.CompressionOnly _constraint;


        // Properties                                                                                                               
        public override string Name { get { return _constraint.Name; } set { _constraint.Name = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(0, 10, "Region type")]
        [DescriptionAttribute("Select the region type for the creation of the constraint definition.")]
        [Id(1, 2)]
        public override string MasterRegionType { get { return base.MasterRegionType; } set { base.MasterRegionType = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(1, 10, "Surface")]
        [DescriptionAttribute("Select the surface for the creation of the constraint definition.")]
        [Id(2, 2)]
        public string SurfaceName { get { return _constraint.RegionName; } set { _constraint.RegionName = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Spring stiffness")]
        [DescriptionAttribute("Value of the spring stiffness for the gap property definition.")]
        [TypeConverter(typeof(EquationForcePerLengthDefaultConverter))]
        [Id(1, 3)]
        public virtual EquationString SpringStiffness
        {
            get { return _constraint.SpringStiffness.Equation; }
            set { _constraint.SpringStiffness.Equation = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Tensile force")]
        [DescriptionAttribute("Value of the tensile force for the gap property definition.")]
        [TypeConverter(typeof(EquationForceDefaultConverter))]
        [Id(2, 3)]
        public virtual EquationString TensileForceAtNegativeInfinity
        {
            get { return _constraint.TensileForceAtNegativeInfinity.Equation; }
            set { _constraint.TensileForceAtNegativeInfinity.Equation = value; }
        }
        //
        [Category("Appearance")]
        [DisplayName("Color")]
        [Description("Select the constraint color.")]
        [Editor(typeof(UserControls.ColorEditorEx), typeof(UITypeEditor))]
        [Id(1, 10)]
        public Color Color { get { return _constraint.MasterColor; } set { _constraint.MasterColor = value; } }


        // Constructors                                                                                                             
        public ViewCompressionOnly(CaeModel.CompressionOnly compressionOnlyConstraint)
        {
            _constraint = compressionOnlyConstraint;
            // The order is important
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(MasterSelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SurfaceName));
            SetBase(_constraint, regionTypePropertyNamePairs, null);
            //
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            //
            StringForcePerLengthDefaultConverter.SetInitialValue =
                CaeModel.GapSection.InitialSpringStiffness.ToString();
            StringForceDefaultConverter.SetInitialValue =
                CaeModel.GapSection.InitialTensileForceAtNegativeInfinity.ToString();
        }


        // Methods                                                                                                                  
        public override CaeModel.Constraint GetBase()
        {
            return _constraint;
        }
        public void PopulateDropDownLists(string[] surfaceNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.SurfaceName, surfaceNames);
            //
            PopulateDropDownLists(regionTypeListItemsPairs, null);
        }
    }

}
