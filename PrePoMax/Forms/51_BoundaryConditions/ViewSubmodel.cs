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
    public class ViewSubmodel : ViewBoundaryCondition
    {
        // Variables                                                                                                                
        private CaeModel.SubmodelBC _submodel;


        // Properties                                                                                                               
        public override string Name { get { return _submodel.Name; } set { _submodel.Name = value; } }
        public override string NodeSetName { get { return _submodel.RegionName; } set { _submodel.RegionName = value; } }
        public override string ReferencePointName { get { return _submodel.RegionName; } set { _submodel.RegionName = value; } }
        public override string SurfaceName { get { return _submodel.RegionName; } set { _submodel.RegionName = value; } }

        [OrderedDisplayName(5, 20, "Step number")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Enter the global model step number from which to read the displacements.")]
        public int StepNumber { get { return _submodel.StepNumber; } set { _submodel.StepNumber = value; } }

        [OrderedDisplayName(6, 20, "U1")]
        [CategoryAttribute("DOF")]
        [DescriptionAttribute("Displacement in the direction of the first axis.")]
        public bool U1 { get { return _submodel.U1; } set { _submodel.U1 = value; } }
        
        [OrderedDisplayName(7, 20, "U2")]
        [CategoryAttribute("DOF")]
        [DescriptionAttribute("Displacement in the direction of the second axis.")]
        public bool U2 { get { return _submodel.U2; } set { _submodel.U2 = value; } }

        [OrderedDisplayName(8, 20, "U3")]
        [CategoryAttribute("DOF")]
        [DescriptionAttribute("Displacement in the direction of the third axis.")]
        public bool U3 { get { return _submodel.U3; } set { _submodel.U3 = value; } }

        [OrderedDisplayName(9, 20, "UR1")]
        [CategoryAttribute("DOF")]
        [DescriptionAttribute("Rotation around the first axis.")]
        public bool UR1 { get { return _submodel.UR1; } set { _submodel.UR1 = value; } }

        [OrderedDisplayName(10, 20, "UR2")]
        [CategoryAttribute("DOF")]
        [DescriptionAttribute("Rotation around the second axis.")]
        public bool UR2 { get { return _submodel.UR2; } set { _submodel.UR2 = value; } }

        [OrderedDisplayName(11, 20, "UR3")]
        [CategoryAttribute("DOF")]
        [DescriptionAttribute("Rotation around the third axis.")]
        public bool UR3 { get { return _submodel.UR3; } set { _submodel.UR3 = value; } }


        // Constructors                                                                                                             
        public ViewSubmodel(CaeModel.SubmodelBC submodel)
        {
            // the order is important
            _submodel = submodel;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.NodeSetName, CaeGlobals.Tools.GetPropertyName(() => this.NodeSetName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, CaeGlobals.Tools.GetPropertyName(() => this.SurfaceName));
            // Must be here to correctly hide it for different region types
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ReferencePointName, CaeGlobals.Tools.GetPropertyName(() => this.ReferencePointName));
            //
            base.SetBase(_submodel, regionTypePropertyNamePairs);
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            // 
            CustomPropertyDescriptor cpd = null;
            // now lets display Unconstrained/From global model instead of True/False
            for (int i = 1; i <= 3; i++)
            {
                cpd = DynamicCustomTypeDescriptor.GetProperty("U" + i);
                foreach (StandardValueAttribute sva in cpd.StatandardValues)
                {
                    if ((bool)sva.Value == true) sva.DisplayName = "From global model";
                    else sva.DisplayName = "Unconstrained";
                }

                cpd = DynamicCustomTypeDescriptor.GetProperty("UR" + i);
                foreach (StandardValueAttribute sva in cpd.StatandardValues)
                {
                    if ((bool)sva.Value == true) sva.DisplayName = "From global model";
                    else sva.DisplayName = "Unconstrained";
                }
            }
        }


        // Methods                                                                                                                  
        public override CaeModel.BoundaryCondition GetBase()
        {
            return (CaeModel.BoundaryCondition)_submodel;
        }
        public void PopululateDropDownLists(string[] nodeSetNames, string[] surfaceNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.NodeSetName, nodeSetNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.SurfaceName, surfaceNames);
            base.PopululateDropDownLists(regionTypeListItemsPairs);
        }
    }

}
