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
        private int _numOfNodeSets;
        private int _numOfSurfaces;

        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [DisplayName("Name")]
        [DescriptionAttribute("Name of the reference point.")]
        [Id(1, 1)]
        public string Name { get { return _referencePoint.Name; } set { _referencePoint.Name = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(1, 10, "Create by/from")]
        [DescriptionAttribute("Select the method for the creation of the reference point.")]
        [Id(1, 2)]
        public string CreatedFrom
        {
            get { return _referencePoint.CreatedFrom.ToString(); }
            set
            {
                if (Enum.TryParse(value, out FeReferencePointCreatedFrom createdFrom))
                {
                    _referencePoint.CreatedFrom = createdFrom;
                    SetPropertiesVisibility();
                }
            }
        }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Region type")]
        [DescriptionAttribute("Select the region type for the creation of the reference point.")]
        [Id(2, 2)]
        public override string RegionType { get { return base.RegionType; } set { base.RegionType = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(3, 10, "Node set")]
        [DescriptionAttribute("Select the node set for the creation of the reference point.")]
        [Id(3, 2)]
        public string NodeSetName { get { return _referencePoint.RegionName; } set { _referencePoint.RegionName = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(4, 10, "Surface")]
        [DescriptionAttribute("Select the surface for the creation of the reference point.")]
        [Id(4, 2)]
        public string SurfaceName { get { return _referencePoint.RegionName; } set { _referencePoint.RegionName = value; } }
        //
        [CategoryAttribute("Coordinates")]
        [DisplayName("X")]
        [DescriptionAttribute("X coordinate of the reference point.")]
        [Id(2, 3)]
        public double X { get { return _referencePoint.X; } set { _referencePoint.X = value; } }
        //
        [CategoryAttribute("Coordinates")]
        [DisplayName("Y")]
        [DescriptionAttribute("Y coordinate of the reference point.")]
        [Id(3, 3)]
        public double Y { get { return _referencePoint.Y; } set { _referencePoint.Y = value; } }
        //
        [CategoryAttribute("Coordinates")]
        [DisplayName("Z")]
        [DescriptionAttribute("Z coordinate of the reference point.")]
        [Id(4, 3)]
        public double Z { get { return _referencePoint.Z; } set { _referencePoint.Z = value; } }


        // Constructors                                                                                                             
        public ViewFeReferencePoint(FeReferencePoint referencePoint)
        {
            // the order is important
            _referencePoint = referencePoint;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.NodeSetName, nameof(this.NodeSetName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(this.SurfaceName));
            base.SetBase(_referencePoint, regionTypePropertyNamePairs);
            //
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
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
            CustomPropertyDescriptor cpd;
            // CreatedFrom
            cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(CreatedFrom));
            cpd.StatandardValues.Clear();
            cpd.StatandardValues.Add(new StandardValueAttribute(FeReferencePointCreatedFrom.Selection.ToString()));
            if (_numOfNodeSets + _numOfSurfaces > 0)
            {
                cpd.StatandardValues.Add(new StandardValueAttribute(FeReferencePointCreatedFrom.BoundingBoxCenter.ToString()));
                cpd.StatandardValues.Add(new StandardValueAttribute(FeReferencePointCreatedFrom.CenterOfGravity.ToString()));
            }
            //
            SetPropertiesVisibility();
        }
       
        private void SetPropertiesVisibility()
        {
            DynamicCustomTypeDescriptor dctd = base.DynamicCustomTypeDescriptor;
            //
            if (CreatedFrom == FeReferencePointCreatedFrom.Selection.ToString())
            {
                dctd.GetProperty(() => RegionType).SetIsBrowsable(false);
                dctd.GetProperty(() => NodeSetName).SetIsBrowsable(false);
                dctd.GetProperty(() => SurfaceName).SetIsBrowsable(false);
                dctd.GetProperty(() => X).SetIsReadOnly(false);
                dctd.GetProperty(() => Y).SetIsReadOnly(false);
                dctd.GetProperty(() => Z).SetIsReadOnly(false);
            }
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
                dctd.GetProperty(() => X).SetIsReadOnly(true);
                dctd.GetProperty(() => Y).SetIsReadOnly(true);
                dctd.GetProperty(() => Z).SetIsReadOnly(true);
            }
        }
    }
}
