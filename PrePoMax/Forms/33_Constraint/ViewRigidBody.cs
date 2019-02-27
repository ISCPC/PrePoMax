using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;

namespace PrePoMax
{
    [Serializable]
    public class ViewRigidBody : ViewConstraint
    {
        // Variables                                                                                                                
        private CaeModel.RigidBody _rigidBody;


        // Properties                                                                                                               
        public override string Name { get { return _rigidBody.Name; } set { _rigidBody.Name = value; } }

        [OrderedDisplayName(1, 10, "Reference point")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the reference point which will be used for the rigid body definition.")]
        public string ReferencePointName { get { return _rigidBody.ReferencePointName; } set { _rigidBody.ReferencePointName = value; } }

        [OrderedDisplayName(2, 10, "Region type")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the region type which will be used for the rigid body definition.")]
        public override string RegionType { get { return base.RegionType; } set { base.RegionType = value; } }

        [OrderedDisplayName(3, 10, "Node set")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the node set which will be used for the rigid body definition.")]
        public string NodeSetName { get { return _rigidBody.RegionName; } set { _rigidBody.RegionName = value; } }

        [OrderedDisplayName(4, 10, "Surface")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the surface which will be used for the rigid body definition.")]
        public string SurfaceName { get { return _rigidBody.RegionName; } set { _rigidBody.RegionName = value; } }


        // Constructors                                                                                                             
        public ViewRigidBody(CaeModel.RigidBody rigidBody)
        {
            // the order is important
            _rigidBody = rigidBody;

            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.NodeSetName, CaeGlobals.Tools.GetPropertyName(() => this.NodeSetName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, CaeGlobals.Tools.GetPropertyName(() => this.SurfaceName));

            base.SetBase(_rigidBody, regionTypePropertyNamePairs);
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }



        // Methods                                                                                                                  
        public override CaeModel.Constraint GetBase()
        {
            return _rigidBody;
        }
        public void PopululateDropDownLists(string[] referencePointNames, string[] nodeSetNames, string[] surfaceNames)
        {
            base.DynamicCustomTypeDescriptor.PopulateProperty(() => this.ReferencePointName, referencePointNames);

            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.NodeSetName, nodeSetNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.SurfaceName, surfaceNames);
            base.PopululateDropDownLists(regionTypeListItemsPairs);
        }
    }

}
