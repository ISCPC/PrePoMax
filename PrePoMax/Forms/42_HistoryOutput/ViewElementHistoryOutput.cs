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
    public enum ViewElementHistoryVariable
    {
        // must start at 1 for the UI to work
        [StandardValue("S", Description = "True (Cauchy) stresses.")]
        S = 1,

        [StandardValue("E", Description = "Total Lagrangian strain or total Eulerian strain.")]
        E = 2,

        [StandardValue("ME", Description = "Mechanical Lagrangian strain or mechanical Eulerian strain.")]
        ME = 4,

        [StandardValue("PEEQ", Description = "Equivalent plastic strain.")]
        PEEQ = 8,

        [StandardValue("ENER", Description = "Energy density.")]
        ENER = 16,

        [StandardValue("ELSE", Description = "Internal energy.")]
        ELSE = 32,

        [StandardValue("EVOL", Description = "Volume.")]
        EVOL = 64
    }

    [Serializable]
    public class ViewElementHistoryOutput : ViewHistoryOutput
    {
        // Variables                                                                                                                
        private CaeModel.ElementHistoryOutput _historyOutput;


        // Properties                                                                                                               
        public override string Name { get { return _historyOutput.Name; } set { _historyOutput.Name = value; } }
        public override int Frequency { get { return _historyOutput.Frequency; } set { _historyOutput.Frequency = value; } }

        [OrderedDisplayName(3, 10, "Element set")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Select the element set which will be used for the history output definition.")]
        public string ElementSetName { get { return _historyOutput.RegionName; } set { _historyOutput.RegionName = value; } }

        [OrderedDisplayName(4, 10, "Variables to output")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Element history variables")]
        public ViewElementHistoryVariable Variables 
        { 
            get
            { 
                return (ViewElementHistoryVariable)_historyOutput.Variables; 
            } 
            set
            { 
                _historyOutput.Variables = (CaeModel.ElementHistoryVariable)value;
            } 
        }

        [OrderedDisplayName(5, 10, "Totals")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("The parameter totals only applies to whole element variables (ELSE, EVOL).")]
        public CaeModel.TotalsTypeEnum TotalsType { get { return _historyOutput.TotalsType; } set { _historyOutput.TotalsType = value; } }


        // Constructors                                                                                                             
        public ViewElementHistoryOutput(CaeModel.ElementHistoryOutput historyOutput)
        {
            // The order is important
            _historyOutput = historyOutput;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, CaeGlobals.Tools.GetPropertyName(() => this.SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ElementSetName, CaeGlobals.Tools.GetPropertyName(() => this.ElementSetName));
            //
            base.SetBase(_historyOutput, regionTypePropertyNamePairs);
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public override CaeModel.HistoryOutput GetBase()
        {
            return _historyOutput;
        }
        public void PopululateDropDownLists(string[] elementSetNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.ElementSetName, elementSetNames);
            base.PopululateDropDownLists(regionTypeListItemsPairs);
        }
        public override void UpdateRegionVisibility()
        {
            base.UpdateRegionVisibility();
            //
            CustomPropertyDescriptor cpd;
            if (base.RegionType == RegionTypeEnum.Selection.ToFriendlyString())
            {
                cpd = base.DynamicCustomTypeDescriptor.GetProperty("SelectionHidden");
                cpd.SetIsBrowsable(false);
            }
        }
    }



   
}
