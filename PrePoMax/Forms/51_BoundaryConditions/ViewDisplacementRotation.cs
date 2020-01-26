using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using CaeGlobals;

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

        [OrderedDisplayName(6, 20, "U1")]
        [CategoryAttribute("DOF")]
        [DescriptionAttribute("Displacement in the direction of the first axis.")]
        [TypeConverter(typeof(StringDOFConverter))]
        public double U1 { get { return _displacementRotation.U1; } set { _displacementRotation.U1 = value; } }
        
        [OrderedDisplayName(7, 20, "U2")]
        [CategoryAttribute("DOF")]
        [DescriptionAttribute("Displacement in the direction of the second axis.")]
        [TypeConverter(typeof(StringDOFConverter))]
        public double U2 { get { return _displacementRotation.U2; } set { _displacementRotation.U2 = value; } }

        [OrderedDisplayName(8, 20, "U3")]
        [CategoryAttribute("DOF")]
        [DescriptionAttribute("Displacement in the direction of the third axis.")]
        [TypeConverter(typeof(StringDOFConverter))]
        public double U3 { get { return _displacementRotation.U3; } set { _displacementRotation.U3 = value; } }

        [OrderedDisplayName(9, 20, "UR1")]
        [CategoryAttribute("DOF")]
        [DescriptionAttribute("Rotation around the first axis.")]
        [TypeConverter(typeof(StringDOFConverter))]
        public double UR1 { get { return _displacementRotation.UR1; } set { _displacementRotation.UR1 = value; } }

        [OrderedDisplayName(10, 20, "UR2")]
        [CategoryAttribute("DOF")]
        [DescriptionAttribute("Rotation around the second axis.")]
        [TypeConverter(typeof(StringDOFConverter))]
        public double UR2 { get { return _displacementRotation.UR2; } set { _displacementRotation.UR2 = value; } }

        [OrderedDisplayName(11, 20, "UR3")]
        [CategoryAttribute("DOF")]
        [DescriptionAttribute("Rotation around the third axis.")]
        [TypeConverter(typeof(StringDOFConverter))]
        public double UR3 { get { return _displacementRotation.UR3; } set { _displacementRotation.UR3 = value; } }


        // Constructors                                                                                                             
        public ViewDisplacementRotation(CaeModel.DisplacementRotation displacementRotation)
        {
            // The order is important
            _displacementRotation = displacementRotation;

            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.NodeSetName, CaeGlobals.Tools.GetPropertyName(() => this.NodeSetName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, CaeGlobals.Tools.GetPropertyName(() => this.SurfaceName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ReferencePointName, CaeGlobals.Tools.GetPropertyName(() => this.ReferencePointName));

            base.SetBase(_displacementRotation, regionTypePropertyNamePairs);
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public override CaeModel.BoundaryCondition GetBase()
        {
            return (CaeModel.BoundaryCondition)_displacementRotation;
        }
        public void PopululateDropDownLists(string[] nodeSetNames, string[] surfaceNames, string[] referencePointNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.NodeSetName, nodeSetNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.SurfaceName, surfaceNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.ReferencePointName, referencePointNames);
            base.PopululateDropDownLists(regionTypeListItemsPairs);
        }
    }

}
