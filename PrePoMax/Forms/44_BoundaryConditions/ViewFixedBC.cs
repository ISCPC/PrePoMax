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
    public class ViewFixedBC : ViewBoundaryCondition
    {
        // Variables                                                                                                                
        private CaeModel.FixedBC _fixedBC;


        // Properties                                                                                                               
        public override string Name { get { return _fixedBC.Name; } set { _fixedBC.Name = value; } }
        public override string NodeSetName { get { return _fixedBC.RegionName; } set { _fixedBC.RegionName = value; } }
        public override string ReferencePointName { get { return _fixedBC.RegionName; } set { _fixedBC.RegionName = value; } }
        public override string SurfaceName { get { return _fixedBC.RegionName; } set { _fixedBC.RegionName = value; } }
        //
        [Browsable(false)]
        public override string AmplitudeName { get { return _fixedBC.AmplitudeName; } set { _fixedBC.AmplitudeName = value; } }
        public override Color Color { get { return _fixedBC.Color; } set { _fixedBC.Color = value; } }


        // Constructors                                                                                                             
        public ViewFixedBC(CaeModel.FixedBC fixedBC)
        {
            // the order is important
            _fixedBC = fixedBC;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.NodeSetName, nameof(NodeSetName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SurfaceName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ReferencePointName, nameof(ReferencePointName));
            //
            SetBase(_fixedBC, regionTypePropertyNamePairs);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public override CaeModel.BoundaryCondition GetBase()
        {
            return _fixedBC;
        }
        public void PopulateDropDownLists(string[] nodeSetNames, string[] surfaceNames, string[] referencePointNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.NodeSetName, nodeSetNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.SurfaceName, surfaceNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.ReferencePointName, referencePointNames);
            PopulateDropDownLists(regionTypeListItemsPairs);
        }
    }

}
