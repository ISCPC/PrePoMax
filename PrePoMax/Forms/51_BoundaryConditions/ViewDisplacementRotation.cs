using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using System.Drawing;

namespace PrePoMax
{
    [Serializable]
    public class ViewDisplacementRotation : ViewBoundaryCondition
    {
        // Variables                                                                                                                
        private CaeModel.DisplacementRotation _displacementRotation;


        // Properties                                                                                                               
        public override string Name { get { return _displacementRotation.Name; } set { _displacementRotation.Name = value; } }
        public override string NodeSetName { get { return _displacementRotation.RegionName; } set { _displacementRotation.RegionName = value; } }
        public override string ReferencePointName { get { return _displacementRotation.RegionName; } set { _displacementRotation.RegionName = value; } }
        public override string SurfaceName { get { return _displacementRotation.RegionName; } set { _displacementRotation.RegionName = value; } }
        //
        [CategoryAttribute("DOF")]
        [OrderedDisplayName(0, 10, "U1")]
        [DescriptionAttribute("Displacement in the direction of the first axis.")]
        [TypeConverter(typeof(StringDOFConverter))]
        [Id(1, 3)]
        public double U1 { get { return _displacementRotation.U1; } set { _displacementRotation.U1 = value; } }
        //
        [CategoryAttribute("DOF")]
        [OrderedDisplayName(1, 10, "U2")]        
        [DescriptionAttribute("Displacement in the direction of the second axis.")]
        [TypeConverter(typeof(StringDOFConverter))]
        [Id(2, 3)]
        public double U2 { get { return _displacementRotation.U2; } set { _displacementRotation.U2 = value; } }
        //
        [CategoryAttribute("DOF")]
        [OrderedDisplayName(2, 10, "U3")]
        [DescriptionAttribute("Displacement in the direction of the third axis.")]
        [TypeConverter(typeof(StringDOFConverter))]
        [Id(3, 3)]
        public double U3 { get { return _displacementRotation.U3; } set { _displacementRotation.U3 = value; } }
        //
        [CategoryAttribute("DOF")]
        [OrderedDisplayName(3, 10, "UR1")]
        [DescriptionAttribute("Rotation around the first axis.")]
        [TypeConverter(typeof(StringAngleDOFConverter))]
        [Id(4, 3)]
        public double UR1 { get { return _displacementRotation.UR1; } set { _displacementRotation.UR1 = value; } }
        //
        [CategoryAttribute("DOF")]
        [OrderedDisplayName(4, 10, "UR2")]
        [DescriptionAttribute("Rotation around the second axis.")]
        [TypeConverter(typeof(StringAngleDOFConverter))]
        [Id(5, 3)]
        public double UR2 { get { return _displacementRotation.UR2; } set { _displacementRotation.UR2 = value; } }
        //
        [CategoryAttribute("DOF")]
        [OrderedDisplayName(5, 10, "UR3")]
        [DescriptionAttribute("Rotation around the third axis.")]
        [TypeConverter(typeof(StringAngleDOFConverter))]
        [Id(6, 3)]
        public double UR3 { get { return _displacementRotation.UR3; } set { _displacementRotation.UR3 = value; } }
        //
        public override Color Color { get { return _displacementRotation.Color; } set { _displacementRotation.Color = value; } }


        // Constructors                                                                                                             
        public ViewDisplacementRotation(CaeModel.DisplacementRotation displacementRotation, string lengthUnit, string angleUnit)
        {
            // The order is important
            _displacementRotation = displacementRotation;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.NodeSetName, nameof(NodeSetName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SurfaceName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ReferencePointName, nameof(ReferencePointName));
            //
            base.SetBase(_displacementRotation, regionTypePropertyNamePairs);
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            //
            StringDOFConverter.SetUnit = lengthUnit;
            StringAngleDOFConverter.SetUnit = angleUnit;
        }


        // Methods                                                                                                                  
        public override CaeModel.BoundaryCondition GetBase()
        {
            return _displacementRotation;
        }
        public void PopululateDropDownLists(string[] nodeSetNames, string[] surfaceNames, string[] referencePointNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.NodeSetName, nodeSetNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.SurfaceName, surfaceNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.ReferencePointName, referencePointNames);
            base.PopululateDropDownLists(regionTypeListItemsPairs);
        }
    }

}
