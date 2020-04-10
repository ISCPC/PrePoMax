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
    public class ViewTie : ViewConstraint
    {
        // Variables                                                                                                                
        private string _selectionHidden;
        private CaeModel.Tie _tie;
        

        // Properties                                                                                                                      
        public override string Name { get { return _tie.Name; } set { _tie.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Position tolerance")]
        [DescriptionAttribute("Enter the value of largest distance from the master surface for which the slave nodes will be included in the tie constraint."
                            + " Default value equals 2.5 % of the typical element size.")]
        [TypeConverter(typeof(StringPosTolConverter))]
        [Id(2, 1)]
        public double PositionTolerance { get { return _tie.PositionTolerance; } set { _tie.PositionTolerance = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Adjust")]
        [DescriptionAttribute("Set adjust to No to prevent the projection of the slave nodes on the master surface.")]
        [Id(3, 1)]
        public bool Adjust { get { return _tie.Adjust; } set { _tie.Adjust = value; } }                
        // MASTER ------------------------------------------------------------------------------------------------------------------
        [CategoryAttribute("Master Region")]
        [OrderedDisplayName(0, 10, "Master region type")]
        [DescriptionAttribute("Select the master region type which will be used for the tie definition.")]
        [Id(1, 2)]
        public override string MasterRegionType { get { return base.MasterRegionType; } set { base.MasterRegionType = value; } }
        //
        [CategoryAttribute("Master Region")]
        [OrderedDisplayName(1, 10, "Hidden")]
        [DescriptionAttribute("Hidden.")]
        [Id(2, 2)]
        //
        public string MasterSelectionHidden { get { return _selectionHidden; } set { _selectionHidden = value; } }
        [CategoryAttribute("Master Region")]
        [OrderedDisplayName(2, 10, "Master surface")]
        [DescriptionAttribute("Select the master surface which will be used for the tie definition.")]
        [Id(3, 2)]
        public string MasterSurfaceName { get { return _tie.MasterRegionName; } set { _tie.MasterRegionName = value; } }
        // SLAVE -------------------------------------------------------------------------------------------------------------------
        [CategoryAttribute("Slave Region")]
        [OrderedDisplayName(3, 10, "Slave region type")]
        [DescriptionAttribute("Select the slave region type which will be used for the tie definition.")]
        [Id(1, 3)]
        public override string SlaveRegionType { get { return base.SlaveRegionType; } set { base.SlaveRegionType = value; } }
        //
        [CategoryAttribute("Slave Region")]
        [OrderedDisplayName(4, 10, "Hidden")]
        [DescriptionAttribute("Hidden.")]
        [Id(2, 3)]
        public string SlaveSelectionHidden { get { return _selectionHidden; } set { _selectionHidden = value; } }
        //
        [CategoryAttribute("Slave Region")]
        [OrderedDisplayName(5, 10, "Slave surface")]
        [DescriptionAttribute("Select the slave surface which will be used for the tie definition.")]
        [Id(3, 3)]
        public string SlaveSurfaceName { get { return _tie.SlaveRegionName; } set { _tie.SlaveRegionName = value; } }


        // Constructors                                                                                                             
        public ViewTie(CaeModel.Tie tie)
        {
            _tie = tie;
            // Master
            Dictionary<RegionTypeEnum, string> masterRegionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            masterRegionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(MasterSelectionHidden));
            masterRegionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(MasterSurfaceName));
            // Slave
            Dictionary<RegionTypeEnum, string> slaveRegionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            slaveRegionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SlaveSelectionHidden));
            slaveRegionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SlaveSurfaceName));
            //
            base.SetBase(tie, masterRegionTypePropertyNamePairs, slaveRegionTypePropertyNamePairs);
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            //
            ApplyAdjustYesNo();
        }


        // Methods                                                                                                                  
        public override CaeModel.Constraint GetBase()
        {
            return _tie;
        }
        protected void ApplyAdjustYesNo()
        {
            CustomPropertyDescriptor cpd = base.DynamicCustomTypeDescriptor.GetProperty("Adjust");
            //
            foreach (StandardValueAttribute sva in cpd.StatandardValues)
            {
                if ((bool)sva.Value == true) sva.DisplayName = "Yes";
                else sva.DisplayName = "No";
            }
        }
        public void PopululateDropDownLists(string[] surfaceNames)
        {
            Dictionary<RegionTypeEnum, string[]> masterRegionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            masterRegionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            masterRegionTypeListItemsPairs.Add(RegionTypeEnum.SurfaceName, surfaceNames);
            //
            Dictionary<RegionTypeEnum, string[]> slaveTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            slaveTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            slaveTypeListItemsPairs.Add(RegionTypeEnum.SurfaceName, surfaceNames);
            base.PopululateDropDownLists(masterRegionTypeListItemsPairs, slaveTypeListItemsPairs);
        }
        public override void UpdateRegionVisibility()
        {
            base.UpdateRegionVisibility();
            // Hide SelectionHidden
            DynamicTypeDescriptor.CustomPropertyDescriptor cpd;
            // Master
            if (base.MasterRegionType == RegionTypeEnum.Selection.ToFriendlyString())
            {
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(() => MasterSelectionHidden);
                cpd.SetIsBrowsable(false);
            }
            // Slave
            if (base.SlaveRegionType == RegionTypeEnum.Selection.ToFriendlyString())
            {
                cpd = base.DynamicCustomTypeDescriptor.GetProperty(() => SlaveSelectionHidden);
                cpd.SetIsBrowsable(false);
            }
        }
    }

}
