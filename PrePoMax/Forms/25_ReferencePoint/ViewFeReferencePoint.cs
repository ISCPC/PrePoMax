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
    public class ViewFeReferencePoint : ViewMultiRegion
    {
        // Variables                                                                                                                
        private FeReferencePoint _referencePoint;
        private ItemSetData _rpCoordinates;
        private int _numOfNodeSets;
        private int _numOfSurfaces;

        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Name")]
        [DescriptionAttribute("Name of the reference point.")]
        [Id(1, 1)]
        public string Name { get { return _referencePoint.Name; } set { _referencePoint.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Create by/from")]
        [DescriptionAttribute("Select the way the reference point will be created.")]
        [Id(2, 1)]
        public FeReferencePointCreatedFrom CreatedFrom
        {
            get { return _referencePoint.CreatedFrom; }
            set
            {
                _referencePoint.CreatedFrom = value;
                SetPropertiesVisibility();
            }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(3, 10, "Region type")]
        [DescriptionAttribute("Select the region type which will be used for the reference point.")]
        [Id(3, 1)]
        public override string RegionType { get { return base.RegionType; } set { base.RegionType = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(4, 10, "Node set")]
        [DescriptionAttribute("Select the node set which will be used for the reference point.")]
        [Id(4, 1)]
        public string NodeSetName { get { return _referencePoint.RegionName; } set { _referencePoint.RegionName = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(5, 10, "Surface")]
        [DescriptionAttribute("Select the surface which will be used for the reference point.")]
        [Id(5, 1)]
        public string SurfaceName { get { return _referencePoint.RegionName; } set { _referencePoint.RegionName = value; } }
        //
        [CategoryAttribute("Coordinates")]
        [DisplayName("Select the point")]
        [DescriptionAttribute("Select the point for the location of the reference point.")]
        [EditorAttribute(typeof(SinglePointDataEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Id(1, 2)]
        public ItemSetData RpCoordinates { get { return _rpCoordinates; } set { if (value != _rpCoordinates) _rpCoordinates = value; } }
        //
        [CategoryAttribute("Coordinates")]
        [DisplayName("X")]
        [DescriptionAttribute("X coordinate of the reference point.")]
        [Id(2, 2)]
        public double X { get { return _referencePoint.X; } set { _referencePoint.X = value; } }
        //
        [CategoryAttribute("Coordinates")]
        [DisplayName("Y")]
        [DescriptionAttribute("Y coordinate of the reference point.")]
        [Id(3, 2)]
        public double Y { get { return _referencePoint.Y; } set { _referencePoint.Y = value; } }
        //
        [CategoryAttribute("Coordinates")]
        [DisplayName("Z")]
        [DescriptionAttribute("Z coordinate of the reference point.")]
        [Id(4, 2)]
        public double Z { get { return _referencePoint.Z; } set { _referencePoint.Z = value; } }


        // Constructors                                                                                                             
        public ViewFeReferencePoint(FeReferencePoint referencePoint)
        {
            // the order is important
            _referencePoint = referencePoint;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.NodeSetName, CaeGlobals.Tools.GetPropertyName(() => this.NodeSetName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, CaeGlobals.Tools.GetPropertyName(() => this.SurfaceName));
            base.SetBase(_referencePoint, regionTypePropertyNamePairs);
            //
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            //
            _rpCoordinates = new ItemSetData(); // needed to display ItemSetData.ToString()
            _rpCoordinates.ToStringType = ItemSetDataToStringType.SelectSinglePoint;
        }


        // Methods                                                                                                                  
        public FeReferencePoint GetBase()
        {
            return _referencePoint;
        }

        public void PopululateDropDownLists(string[] nodeSetNames, string[] surfaceNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.NodeSetName, nodeSetNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.SurfaceName, surfaceNames);
            base.PopululateDropDownLists(regionTypeListItemsPairs);
            //
            _numOfNodeSets = nodeSetNames.Length;
            _numOfSurfaces = surfaceNames.Length;
            //
            SetPropertiesVisibility();
        }
       
        private void SetPropertiesVisibility()
        {
            DynamicCustomTypeDescriptor dctd = base.DynamicCustomTypeDescriptor;
            //
            if (CreatedFrom == FeReferencePointCreatedFrom.Coordinates)
            {
                dctd.GetProperty(() => RegionType).SetIsBrowsable(false);
                dctd.GetProperty(() => NodeSetName).SetIsBrowsable(false);
                dctd.GetProperty(() => SurfaceName).SetIsBrowsable(false);
                dctd.GetProperty(() => RpCoordinates).SetIsBrowsable(true);
                dctd.GetProperty(() => X).SetIsReadOnly(false);
                dctd.GetProperty(() => Y).SetIsReadOnly(false);
                dctd.GetProperty(() => Z).SetIsReadOnly(false);
                //
                dctd.GetProperty(() => CreatedFrom).SetIsBrowsable(_numOfNodeSets + _numOfSurfaces != 0);
            }
            else
            {
                // If there is nothing to show - recall this method
                if (_numOfNodeSets + _numOfSurfaces == 0) CreatedFrom = FeReferencePointCreatedFrom.Coordinates;
                // Else at least one drop down menu is not empty - show it
                else
                {
                    dctd.GetProperty(() => RegionType).SetIsBrowsable(true);
                    //
                    if (_numOfNodeSets > 0 && _referencePoint.RegionType == RegionTypeEnum.NodeSetName)
                        RegionType = RegionTypeEnum.NodeSetName.ToFriendlyString();
                    else
                        RegionType = RegionTypeEnum.SurfaceName.ToFriendlyString();
                    //
                    if (_numOfSurfaces > 0 && _referencePoint.RegionType == RegionTypeEnum.SurfaceName)
                        RegionType = RegionTypeEnum.SurfaceName.ToFriendlyString();
                    else
                        RegionType = RegionTypeEnum.NodeSetName.ToFriendlyString();
                    //
                    dctd.GetProperty(() => RpCoordinates).SetIsBrowsable(false);
                    dctd.GetProperty(() => X).SetIsReadOnly(true);
                    dctd.GetProperty(() => Y).SetIsReadOnly(true);
                    dctd.GetProperty(() => Z).SetIsReadOnly(true);
                }
            }
        }
    }
}
