﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using System.Drawing;
using System.Drawing.Design;

namespace PrePoMax
{
    [Serializable]
    public class ViewPointSpring : ViewSpring
    {
        // Variables                                                                                                                


        // Properties                                                                                                               
        [CategoryAttribute("Region")]
        [OrderedDisplayName(1, 10, "Node set")]
        [DescriptionAttribute("Select the node set for the creation of the constraint definition.")]
        [Id(2, 2)]
        public string NodeSetName { get { return _springConstraint.RegionName; } set { _springConstraint.RegionName = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Reference point")]
        [DescriptionAttribute("Select the reference point for the creation of the constraint definition.")]
        [Id(3, 2)]
        public string ReferencePointName
        {
            get { return _springConstraint.RegionName; }
            set { _springConstraint.RegionName = value; }
        }


        // Constructors                                                                                                             
        public ViewPointSpring(CaeModel.PointSpring pointSpring)
            : base(pointSpring)
        {
            // The order is important
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(MasterSelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.NodeSetName, nameof(NodeSetName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ReferencePointName, nameof(ReferencePointName));
            //
            SetBase(_springConstraint, regionTypePropertyNamePairs, null);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            // 2D
            DynamicCustomTypeDescriptor.GetProperty(nameof(K3)).SetIsBrowsable(!_springConstraint.TwoD);
        }


        // Methods                                                                                                                  
        public override CaeModel.Constraint GetBase()
        {
            return _springConstraint;
        }
        public void PopululateDropDownLists(string[] nodeSetNames, string[] referencePointNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.NodeSetName, nodeSetNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.ReferencePointName, referencePointNames);
            //
            PopululateDropDownLists(regionTypeListItemsPairs, null);
        }
    }

}
