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

namespace PrePoMax.Forms
{
    [Serializable]
    public class ScaleParameters
    {
        // Variables                                                                                                                      
        private DynamicCustomTypeDescriptor _dctd = null;
        private ItemSetData _scaleCenterItemSetData;
        private double[] _scaleCenter;
        private double _scaleFactorX;
        private double _scaleFactorY;
        private double _scaleFactorZ;
        private bool _copy;


        // Properties                                                                                                               
        [Category("Data")]
        [OrderedDisplayName(0, 10, "Operation")]
        [DescriptionAttribute("Select the move/copy operation.")]
        [Id(1, 1)]
        public bool Copy { get { return _copy; } set { _copy = value; } }
        //
        [Category("Center point coordinates")]
        [OrderedDisplayName(0, 10, "Select the center point")]
        [DescriptionAttribute("Select the center point.")]
        [EditorAttribute(typeof(SinglePointDataEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Id(1, 2)]
        public ItemSetData ScaleCenterItemSet
        {
            get { return _scaleCenterItemSetData; }
            set
            {
                if (value != _scaleCenterItemSetData)
                    _scaleCenterItemSetData = value;
            }
        }
        //
        [Category("Center point coordinates")]
        [OrderedDisplayName(1, 10, "X")]
        [Description("X coordinate of the center point.")]
        [TypeConverter(typeof(CaeGlobals.StringLengthConverter))]
        [Id(1, 2)]
        public double CenterX { get { return _scaleCenter[0]; } set { _scaleCenter[0] = value; } }
        //
        [Category("Center point coordinates")]
        [OrderedDisplayName(2, 10, "Y")]
        [Description("Y coordinate of the center point.")]
        [TypeConverter(typeof(CaeGlobals.StringLengthConverter))]
        [Id(1, 2)]
        public double CenterY { get { return _scaleCenter[1]; } set { _scaleCenter[1] = value; } }
        //
        [Category("Center point coordinates")]
        [OrderedDisplayName(3, 10, "Z")]
        [Description("Z coordinate of the center point.")]
        [TypeConverter(typeof(CaeGlobals.StringLengthConverter))]
        [Id(1, 2)]
        public double CenterZ { get { return _scaleCenter[2]; } set { _scaleCenter[2] = value; } }
        //
        [Category("Scale factors")]
        [OrderedDisplayName(0, 10, "X")]
        [Description("Scale factor in the X direction.")]
        [Id(1, 3)]
        public double FactorX
        {
            get { return _scaleFactorX; }
            set
            {
                _scaleFactorX = value;
                if (_scaleFactorX <= 0) _scaleFactorX = 1;
            }
        }
        //
        [Category("Scale factors")]
        [OrderedDisplayName(0, 10, "Y")]
        [Description("Scale factor in the Y direction.")]
        [Id(1, 3)]
        public double FactorY
        {
            get { return _scaleFactorY; }
            set 
            {
                _scaleFactorY = value;
                if (_scaleFactorY <= 0) _scaleFactorY = 1;
            }
        }
        //
        [Category("Scale factors")]
        [OrderedDisplayName(0, 10, "Z")]
        [Description("Scale factor in the Z direction.")]
        [Id(1, 3)]
        public double FactorZ
        {
            get { return _scaleFactorZ; }
            set
            {
                _scaleFactorZ = value;
                if (_scaleFactorZ <= 0) _scaleFactorZ = 1;
            }
        }


        // Constructors                                                                                                             
        public ScaleParameters()
        {
            Clear();
            //
            _dctd = ProviderInstaller.Install(this);
            _dctd.CategorySortOrder = CustomSortOrder.AscendingById;
            _dctd.PropertySortOrder = CustomSortOrder.AscendingById;
            //
            _scaleCenterItemSetData = new ItemSetData(); // needed to display ItemSetData.ToString()
            _scaleCenterItemSetData.ToStringType = ItemSetDataToStringType.SelectSinglePoint;
            //
            _dctd.RenameBooleanProperty(nameof(Copy), "Copy and scale", "Scale");
        }


        // Methods                                                                                                                  
        public void Clear()
        {
            _copy = false;
            //
            _scaleCenter = new double[3];
            //
            _scaleFactorX = 1;
            _scaleFactorY = 1;
            _scaleFactorZ = 1;
        }
    }
}
