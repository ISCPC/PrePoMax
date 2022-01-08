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
    public abstract class ViewSpring : ViewConstraint
    {
        // Variables                                                                                                                
        protected CaeModel.SpringConstraint _springConstraint;


        // Properties                                                                                                               
        public override string Name { get { return _springConstraint.Name; } set { _springConstraint.Name = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(0, 10, "Region type")]
        [DescriptionAttribute("Select the region type for the creation of the constraint definition.")]
        [Id(1, 2)]
        public override string MasterRegionType { get { return base.MasterRegionType; } set { base.MasterRegionType = value; } }
        //
        [CategoryAttribute("Stiffness")]
        [OrderedDisplayName(0, 10, "K1")]
        [DescriptionAttribute("Value of the stiffness per node in the direction of the first axis.")]
        [TypeConverter(typeof(StringForcePerLenghtConverter))]
        [Id(1, 3)]
        public double K1 { get { return _springConstraint.K1; } set { _springConstraint.K1 = value; } }
        //
        [CategoryAttribute("Stiffness")]
        [OrderedDisplayName(1, 10, "K2")]
        [DescriptionAttribute("Value of the stiffness per node in the direction of the second axis.")]
        [TypeConverter(typeof(StringForcePerLenghtConverter))]
        [Id(2, 3)]
        public double K2 { get { return _springConstraint.K2; } set { _springConstraint.K2 = value; } }
        //
        [CategoryAttribute("Stiffness")]
        [OrderedDisplayName(2, 10, "K3")]
        [DescriptionAttribute("Value of the stiffness per node in the direction of the third axis.")]
        [TypeConverter(typeof(StringForcePerLenghtConverter))]
        [Id(3, 3)]
        public double K3 { get { return _springConstraint.K3; } set { _springConstraint.K3 = value; } }
        //
        [Category("Appearance")]
        [DisplayName("Color")]
        [Description("Select the constraint color.")]
        [Editor(typeof(UserControls.ColorEditorEx), typeof(UITypeEditor))]
        [Id(1, 10)]
        public Color Color { get { return _springConstraint.MasterColor; } set { _springConstraint.MasterColor = value; } }


        // Constructors                                                                                                             
        public ViewSpring(CaeModel.SpringConstraint springConstraint)
        {
            _springConstraint = springConstraint;
        }


        // Methods                                                                                                                  
        public override CaeModel.Constraint GetBase()
        {
            return _springConstraint;
        }
        
    }

}
