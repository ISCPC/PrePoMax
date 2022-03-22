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
    public class ViewSubmodelBC : ViewBoundaryCondition
    {
        // Variables                                                                                                                
        private CaeModel.SubmodelBC _submodel;


        // Properties                                                                                                               
        public override string Name { get { return _submodel.Name; } set { _submodel.Name = value; } }
        public override string NodeSetName { get { return _submodel.RegionName; } set { _submodel.RegionName = value; } }
        public override string ReferencePointName { get { return _submodel.RegionName; } set { _submodel.RegionName = value; } }
        public override string SurfaceName { get { return _submodel.RegionName; } set { _submodel.RegionName = value; } }

        [CategoryAttribute("Data")]
        [OrderedDisplayName(9, 10, "Step number")]
        [DescriptionAttribute("Enter the global model step number from which to read the displacements.")]
        [Id(2, 1)]
        public int StepNumber { get { return _submodel.StepNumber; } set { _submodel.StepNumber = value; } }
        //
        [CategoryAttribute("DOF")]
        [OrderedDisplayName(0, 10, "U1")]
        [DescriptionAttribute("Displacement in the direction of the first axis.")]
        [Id(1, 3)]
        public bool U1 { get { return _submodel.U1; } set { _submodel.U1 = value; } }
        //
        [CategoryAttribute("DOF")]
        [OrderedDisplayName(1, 10, "U2")]
        [DescriptionAttribute("Displacement in the direction of the second axis.")]
        [Id(2, 3)]
        public bool U2 { get { return _submodel.U2; } set { _submodel.U2 = value; } }
        //
        [CategoryAttribute("DOF")]
        [OrderedDisplayName(2, 10, "U3")]
        [DescriptionAttribute("Displacement in the direction of the third axis.")]
        [Id(3, 3)]
        public bool U3 { get { return _submodel.U3; } set { _submodel.U3 = value; } }
        //
        [CategoryAttribute("DOF")]
        [OrderedDisplayName(3, 10, "UR1")]
        [DescriptionAttribute("Rotation around the first axis.")]
        [Id(4, 3)]
        public bool UR1 { get { return _submodel.UR1; } set { _submodel.UR1 = value; } }
        //
        [CategoryAttribute("DOF")]
        [OrderedDisplayName(4, 10, "UR2")]
        [DescriptionAttribute("Rotation around the second axis.")]
        [Id(5, 3)]
        public bool UR2 { get { return _submodel.UR2; } set { _submodel.UR2 = value; } }
        //
        [CategoryAttribute("DOF")]
        [OrderedDisplayName(5, 10, "UR3")]
        [DescriptionAttribute("Rotation around the third axis.")]
        [Id(6, 3)]
        public bool UR3 { get { return _submodel.UR3; } set { _submodel.UR3 = value; } }
        //
        [Browsable(false)]
        public override string AmplitudeName { get { return _submodel.AmplitudeName; } set { _submodel.AmplitudeName = value; } }
        public override Color Color { get { return _submodel.Color; } set { _submodel.Color = value; } }


        // Constructors                                                                                                             
        public ViewSubmodelBC(CaeModel.SubmodelBC submodel)
        {
            // The order is important
            _submodel = submodel;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.NodeSetName, nameof(NodeSetName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SurfaceName));
            // Must be here to correctly hide the RPs defined in base class
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ReferencePointName, nameof(ReferencePointName));
            //
            SetBase(_submodel, regionTypePropertyNamePairs);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            // Now lets display Unconstrained/From global model instead of True/False
            for (int i = 1; i <= 3; i++)
            {
                DynamicCustomTypeDescriptor.RenameBooleanProperty("U" + i, "From global model", "Unconstrained");
                DynamicCustomTypeDescriptor.RenameBooleanProperty("UR" + i, "From global model", "Unconstrained");
            }
            // 2D
            DynamicCustomTypeDescriptor.GetProperty(nameof(U3)).SetIsBrowsable(!_submodel.TwoD);
            DynamicCustomTypeDescriptor.GetProperty(nameof(UR1)).SetIsBrowsable(!_submodel.TwoD);
            DynamicCustomTypeDescriptor.GetProperty(nameof(UR2)).SetIsBrowsable(!_submodel.TwoD);
        }


        // Methods                                                                                                                  
        public override CaeModel.BoundaryCondition GetBase()
        {
            return _submodel;
        }
        public void PopulateDropDownLists(string[] nodeSetNames, string[] surfaceNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.NodeSetName, nodeSetNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.SurfaceName, surfaceNames);
            PopulateDropDownLists(regionTypeListItemsPairs);
        }
    }

}
