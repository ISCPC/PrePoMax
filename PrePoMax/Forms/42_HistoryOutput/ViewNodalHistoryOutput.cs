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
    [EnumResource("PrePoMax.Properties.Resources")]
    [Editor(typeof(StandardValueEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [Flags]
    public enum ViewNodalHistoryVariable
    {
        // must start at 1 for the UI to work
        [StandardValue("RF", Description = "Reaction forces.")]
        RF = 1,

        [StandardValue("U", Description = "Displacements.")]
        U = 2
    }

    [Serializable]
    //[ClassResource(BaseName = "PrePoMax.Properties.Resources", KeyPrefix = "ViewNodalFieldOutput_")]
    public class ViewNodalHistoryOutput : ViewHistoryOutput
    {
        // Variables                                                                                                                
        private CaeModel.NodalHistoryOutput _historyOutput;


        // Properties                                                                                                               
        public override string Name { get { return _historyOutput.Name; } set { _historyOutput.Name = value; } }
        public override int Frequency { get { return _historyOutput.Frequency; } set { _historyOutput.Frequency = value; } }

        [OrderedDisplayName(3, 10, "Node set")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the node set which will be used for the history output definition.")]
        public string NodeSetName { get { return _historyOutput.RegionName; } set { _historyOutput.RegionName = value; } }

        [OrderedDisplayName(4, 10, "Reference point")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the reference point which will be used for the history output definition.")]
        public string ReferencePointName { get { return _historyOutput.RegionName; } set { _historyOutput.RegionName = value; } }

        [OrderedDisplayName(5, 10, "Surface")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the surface which will be used for the history output definition.")]
        public string SurfaceName { get { return _historyOutput.RegionName; } set { _historyOutput.RegionName = value; } }

        [OrderedDisplayName(6, 10, "Variables to output")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Nodal history variables")]
        public ViewNodalHistoryVariable Variables 
        { 
            get
            { 
                return (ViewNodalHistoryVariable)_historyOutput.Variables; 
            } 
            set
            { 
                _historyOutput.Variables = (CaeModel.NodalHistoryVariable)value;
            } 
        }

        [OrderedDisplayName(7, 10, "Totals")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("The parameter totals only applies to external forces.")]
        public CaeModel.TotalsTypeEnum TotalsType { get { return _historyOutput.TotalsType; } set { _historyOutput.TotalsType = value; } }


        // Constructors                                                                                                             
        public ViewNodalHistoryOutput(CaeModel.NodalHistoryOutput historyOutput)
        {
            // the order is important
            _historyOutput = historyOutput;

            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.NodeSetName, CaeGlobals.Tools.GetPropertyName(() => this.NodeSetName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, CaeGlobals.Tools.GetPropertyName(() => this.SurfaceName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ReferencePointName, CaeGlobals.Tools.GetPropertyName(() => this.ReferencePointName));

            base.SetBase(_historyOutput, regionTypePropertyNamePairs);
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public override CaeModel.HistoryOutput GetBase()
        {
            return _historyOutput;
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
