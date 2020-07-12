using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.ComponentModel;
using DynamicTypeDescriptor;
using System.Drawing.Design;
using CaeResults;

namespace PrePoMax.Forms
{
    [Serializable]
    public class ViewSymetry : ViewTransformation
    {
        // Variables                                                                                                                      
        private Symetry _symetry;


        // Properties                                                                                                               
        public override string Name
        {
            get { return "Symetry"; }
        }
        //
        [Category("Data")]
        [OrderedDisplayName(0, 10, "Symetry plane")]
        [DescriptionAttribute("Select the symetry plane.")]
        [Id(1, 1)]
        public SymetryPlaneEnum SymetryPlane { get { return _symetry.SymetryPlane; } set { _symetry.SymetryPlane = value; } }
        //
        [Category("Symetry point coordinates")]
        [OrderedDisplayName(0, 10, "Select the symetry point")]
        [DescriptionAttribute("Select the symetry point.")]
        [EditorAttribute(typeof(SinglePointDataEditor), typeof(UITypeEditor))]
        [Id(1, 2)]
        public ItemSetData SymetryPointItemSet
        {
            get { return _startPointItemSetData; }
            set { _startPointItemSetData = value; }
        }
        //
        [Category("Symetry point coordinates")]
        [OrderedDisplayName(1, 10, "X")]
        [Description("X coordinate of the point.")]
        [TypeConverter(typeof(CaeGlobals.StringLengthConverter))]
        [Id(1, 2)]
        public double SymetryPointX { get { return _symetry.PointCoor[0]; } set { _symetry.PointCoor[0] = value; } }
        //
        [Category("Symetry point coordinates")]
        [OrderedDisplayName(2, 10, "Y")]
        [Description("Y coordinate of the point.")]
        [TypeConverter(typeof(CaeGlobals.StringLengthConverter))]
        [Id(1, 2)]
        public double SymetryPointY { get { return _symetry.PointCoor[1]; } set { _symetry.PointCoor[1] = value; } }
        //
        [Category("Symetry point coordinates")]
        [OrderedDisplayName(3, 10, "Z")]
        [Description("Z coordinate of the point.")]
        [TypeConverter(typeof(CaeGlobals.StringLengthConverter))]
        [Id(1, 2)]
        public double SymetryPointZ { get { return _symetry.PointCoor[2]; } set { _symetry.PointCoor[2] = value; } }
        


        // Constructors                                                                                                             
        public ViewSymetry(Symetry symetry)
        {
            _symetry = symetry;
            //
            _dctd = ProviderInstaller.Install(this);
            _dctd.CategorySortOrder = CustomSortOrder.AscendingById;
            _dctd.PropertySortOrder = CustomSortOrder.AscendingById;
            //
            _startPointItemSetData = new ItemSetData(); // needed to display ItemSetData.ToString()
            _startPointItemSetData.ToStringType = ItemSetDataToStringType.SelectSinglePoint;
            //
            //_dctd.RenameBooleanProperty(nameof(Copy), "Copy and scale", "Scale");
        }


        // Methods                                                                                                                  
        public override Transformation Base { get { return _symetry; } }
    }
}
