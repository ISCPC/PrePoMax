using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.ComponentModel;
using DynamicTypeDescriptor;

namespace PrePoMax.Forms
{
    [Serializable]
    public class ViewFeReferencePoint
    {
        // Variables                                                                                                                
        private FeReferencePoint _referencePoint;
        private DynamicCustomTypeDescriptor _dctd = null;
        private ItemSetData _rpCoordinates;

        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [DisplayName("\tName")]
        [DescriptionAttribute("Name of the reference point.")]
        [Id(1, 1)]
        public string Name { get { return _referencePoint.Name; } set { _referencePoint.Name = value; } }

        [CategoryAttribute("Data")]
        [DisplayName("Create by/from")]
        [DescriptionAttribute("Select the way the surface will be created.")]
        [Id(2, 1)]
        public FeReferencePointCreatedFrom CreateReferencePointFrom
        {
            get { return _referencePoint.CreatedFrom; }
            set
            {
                _referencePoint.CreatedFrom = value;
                SetPropertiesVisibility();
            }
        }

        [CategoryAttribute("Data")]
        [DisplayName("Node set")]
        [DescriptionAttribute("Select the node set for the reference point creation.")]
        [Id(3, 1)]
        public string NodeSetName
        { 
            get { return _referencePoint.CreatedFromNodeSetName; } 
            set 
            {
                _referencePoint.CreatedFromNodeSetName = value;
            } 
        }

        [CategoryAttribute("Coordinates")]
        [DisplayName("Select the point")]
        [DescriptionAttribute("Select the point for the location of the reference point.")]
        [EditorAttribute(typeof(SinglePointDataEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Id(4, 2)]
        public ItemSetData RpCoordinates { get { return _rpCoordinates; } set { if (value != _rpCoordinates) _rpCoordinates = value; } }

        [CategoryAttribute("Coordinates")]
        [DisplayName("X")]
        [DescriptionAttribute("X coordinate of the reference point.")]
        [Id(5, 2)]
        public double X { get { return _referencePoint.X; } set { _referencePoint.X = value; } }
        
        [CategoryAttribute("Coordinates")]
        [DisplayName("Y")]
        [DescriptionAttribute("Y coordinate of the reference point.")]
        [Id(6, 2)]
        public double Y { get { return _referencePoint.Y; } set { _referencePoint.Y = value; } }
        
        [CategoryAttribute("Coordinates")]
        [DisplayName("Z")]
        [DescriptionAttribute("Z coordinate of the reference point.")]
        [Id(7, 2)]
        public double Z { get { return _referencePoint.Z; } set { _referencePoint.Z = value; } }


        // Constructors                                                                                                             
        public ViewFeReferencePoint(FeReferencePoint referencePoint)
        {
            _referencePoint = referencePoint;
            _dctd = ProviderInstaller.Install(this);

            _rpCoordinates = new ItemSetData(); // needed to display ItemSetData.ToString()
            _rpCoordinates.ToStringType = ItemSetDataToStringType.SelectSinglePoint;

            SetPropertiesVisibility();
        }


        // Methods                                                                                                                  
        public FeReferencePoint GetBase()
        {
            return _referencePoint;
        }
        public void PopululateDropDownList(string[] nodeSetNames)
        {
            StandardValueAttribute sva;
            CustomPropertyDescriptor cpd;

            if (nodeSetNames.Length > 0)
            {
                // NodeSetName
                cpd = _dctd.GetProperty("NodeSetName");
                cpd.StatandardValues.Clear();
                cpd.PropertyFlags |= PropertyFlags.ExclusiveStandardValues;

                foreach (var name in nodeSetNames)
                {
                    sva = new StandardValueAttribute(name);
                    cpd.StatandardValues.Add(sva);
                }
            }
            else
            {
                CreateReferencePointFrom = FeReferencePointCreatedFrom.Coordinates;
                cpd = _dctd.GetProperty("CreateReferencePointFrom");
                cpd.SetIsBrowsable(false);
            }
        }
        private void SetPropertiesVisibility()
        {
            if (_referencePoint.CreatedFrom == FeReferencePointCreatedFrom.Coordinates)
            {
                _dctd.GetProperty("NodeSetName").SetIsBrowsable(false);
                _dctd.GetProperty("RpCoordinates").SetIsBrowsable(true);
                _dctd.GetProperty("X").SetIsReadOnly(false);
                _dctd.GetProperty("Y").SetIsReadOnly(false);
                _dctd.GetProperty("Z").SetIsReadOnly(false);
            }
            else
            {
                _dctd.GetProperty("NodeSetName").SetIsBrowsable(true);
                _dctd.GetProperty("RpCoordinates").SetIsBrowsable(false);
                _dctd.GetProperty("X").SetIsReadOnly(true);
                _dctd.GetProperty("Y").SetIsReadOnly(true);
                _dctd.GetProperty("Z").SetIsReadOnly(true);

                if (_referencePoint.CreatedFromNodeSetName == null && _dctd.GetProperty("NodeSetName").StatandardValues.Count > 0)
                {
                    NodeSetName = _dctd.GetProperty("NodeSetName").StatandardValues.First().ToString();
                }
            }
        }
    }
}
