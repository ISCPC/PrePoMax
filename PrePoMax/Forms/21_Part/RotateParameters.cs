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
    public class RotateParameters
    {
        // Variables                                                                                                                      
        private DynamicCustomTypeDescriptor _dctd = null;
        private ItemSetData _startPointItemSetData;
        private ItemSetData _endPointItemSetData;
        private double[] _startPoint;
        private double[] _endPoint;
        private double _angleDeg;
        private bool _copy;


        // Properties                                                                                                               
        [Category("Data")]
        [OrderedDisplayName(0, 10, "Operation")]
        [DescriptionAttribute("Select the move/copy operation.")]
        [Id(1, 1)]
        public bool Copy { get { return _copy; } set { _copy = value; } }


        [Category("Rotation axis")]
        [OrderedDisplayName(0, 10, "Select the start point")]
        [DescriptionAttribute("Select the start point.")]
        [EditorAttribute(typeof(SinglePointDataEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Id(1, 2)]
        public ItemSetData StartPointItemSet
        {
            get { return _startPointItemSetData; }
            set
            {
                if (value != _startPointItemSetData)
                    _startPointItemSetData = value;
            }
        }

        [Category("Rotation axis")]
        [OrderedDisplayName(1, 10, "X")]
        [Description("X coordinate of the start point.")]
        [Id(1, 2)]
        public double X1 { get { return _startPoint[0]; } set { _startPoint[0] = value; } }

        [Category("Rotation axis")]
        [OrderedDisplayName(2, 10, "Y")]
        [Description("Y coordinate of the start point.")]
        [Id(1, 2)]
        public double Y1 { get { return _startPoint[1]; } set { _startPoint[1] = value; } }

        [Category("Rotation axis")]
        [OrderedDisplayName(3, 10, "Z")]
        [Description("Z coordinate of the start point.")]
        [Id(1, 2)]
        public double Z1 { get { return _startPoint[2]; } set { _startPoint[2] = value; } }

        [Category("Rotation axis")]
        [OrderedDisplayName(4, 10, "Select the end point")]
        [DescriptionAttribute("Select the end point.")]
        [EditorAttribute(typeof(SinglePointDataEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Id(1, 2)]
        public ItemSetData EndPointItemSet
        {
            get { return _endPointItemSetData; }
            set
            {
                if (value != _endPointItemSetData)
                    _endPointItemSetData = value;
            }
        }

        [Category("Rotation axis")]
        [OrderedDisplayName(5, 10, "X")]
        [Description("X coordinate of the end point.")]
        [Id(1, 2)]
        public double X2 { get { return _endPoint[0]; } set { _endPoint[0] = value; } }

        [Category("Rotation axis")]
        [OrderedDisplayName(6, 10, "Y")]
        [Description("Y coordinate of the end point.")]
        [Id(1, 2)]
        public double Y2 { get { return _endPoint[1]; } set { _endPoint[1] = value; } }

        [Category("Rotation axis")]
        [OrderedDisplayName(7, 10, "Z")]
        [Description("Z coordinate of the end point.")]
        [Id(1, 2)]
        public double Z2 { get { return _endPoint[2]; } set { _endPoint[2] = value; } }


        [Category("Rotation angle")]
        [OrderedDisplayName(0, 10, "Angle [°]")]
        [Description("Rotation angle in degrees.")]
        [Id(1, 3)]
        public double AngleDeg { get { return _angleDeg; } set { _angleDeg = value; } }


        // Constructors                                                                                                             
        public RotateParameters()
        {
            Clear();

            _dctd = ProviderInstaller.Install(this);
            _dctd.CategorySortOrder = CustomSortOrder.AscendingById;
            _dctd.PropertySortOrder = CustomSortOrder.AscendingById;

            _startPointItemSetData = new ItemSetData(); // needed to display ItemSetData.ToString()
            _startPointItemSetData.ToStringType = ItemSetDataToStringType.SelectSinglePoint;
            _endPointItemSetData = new ItemSetData();   // needed to display ItemSetData.ToString()
            _endPointItemSetData.ToStringType = ItemSetDataToStringType.SelectSinglePoint;

            RenameTrueFalseForBooleanProperty("Copy", "Copy and rotate", "Rotate");
        }


        // Methods                                                                                                                  
        public void Clear()
        {
            _copy = false;
            _startPoint = new double[3];
            _endPoint = new double[3];
            _angleDeg = 0;
        }

        protected void RenameTrueFalseForBooleanProperty(string propertyName, string trueName, string falseName)
        {
            CustomPropertyDescriptor cpd = _dctd.GetProperty(propertyName);

            foreach (StandardValueAttribute sva in cpd.StatandardValues)
            {
                if ((bool)sva.Value == true) sva.DisplayName = trueName;
                else sva.DisplayName = falseName;
            }
        }

    }
}
