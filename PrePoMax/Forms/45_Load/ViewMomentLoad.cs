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
    public class ViewMomentLoad : ViewLoad
    {
        // Variables                                                                                                                
        private CaeModel.MomentLoad _momentLoad;


        // Properties                                                                                                               
        public override string Name { get { return _momentLoad.Name; } set { _momentLoad.Name = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Node set")]
        [DescriptionAttribute("Select the node set for the creation of the load.")]
        [Id(3, 2)]
        public string NodeSetName { get { return _momentLoad.RegionName; } set { _momentLoad.RegionName = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(3, 10, "Reference point")]
        [DescriptionAttribute("Select the reference point for the creation of the load.")]
        [Id(4, 2)]
        public string ReferencePointName { get { return _momentLoad.RegionName; } set { _momentLoad.RegionName = value; } }
        //
        [CategoryAttribute("Moment components")]
        [OrderedDisplayName(0, 10, "M1")]
        [DescriptionAttribute("Value of the moment component per node in the direction of the first axis.")]
        [TypeConverter(typeof(EquationMomentConverter))]
        [Id(1, 3)]
        public EquationString M1 { get { return _momentLoad.M1.Equation; } set { _momentLoad.M1.Equation = value; } }
        //
        [CategoryAttribute("Moment components")]
        [OrderedDisplayName(1, 10, "M2")]
        [DescriptionAttribute("Value of the moment component per node in the direction of the second axis.")]
        [TypeConverter(typeof(EquationMomentConverter))]
        [Id(2, 3)]
        public EquationString M2 { get { return _momentLoad.M2.Equation; } set { _momentLoad.M2.Equation = value; } }
        //
        [CategoryAttribute("Moment components")]
        [OrderedDisplayName(2, 10, "M3")]
        [DescriptionAttribute("Value of the moment component per node in the direction of the third axis.")]
        [TypeConverter(typeof(EquationMomentConverter))]
        [Id(3, 3)]
        public EquationString M3 { get { return _momentLoad.M3.Equation; } set { _momentLoad.M3.Equation = value; } }
        //
        [CategoryAttribute("Moment magnitude")]
        [OrderedDisplayName(3, 10, "Magnitude")]
        [DescriptionAttribute("Value of the moment load magnitude per node.")]
        [TypeConverter(typeof(EquationMomentConverter))]
        [Id(1, 4)]
        public EquationString Magnitude
        {
            get { return _momentLoad.Magnitude.Equation; }
            set { _momentLoad.Magnitude.Equation = value; }
        }
        //
        [CategoryAttribute("Moment phase")]
        [OrderedDisplayName(0, 10, "Phase")]
        [DescriptionAttribute("Value of the moment phase.")]
        [TypeConverter(typeof(EquationAngleDegConverter))]
        [Id(1, 5)]
        public EquationString Phase
        {
            get { return _momentLoad.PhaseDeg.Equation; }
            set { _momentLoad.PhaseDeg.Equation = value; }
        }
        //
        public override string AmplitudeName
        {
            get { return _momentLoad.AmplitudeName; }
            set { _momentLoad.AmplitudeName = value; }
        }
        public override System.Drawing.Color Color { get { return _momentLoad.Color; } set { _momentLoad.Color = value; } }


        // Constructors                                                                                                             
        public ViewMomentLoad(CaeModel.MomentLoad momentLoad)
        {
            // The order is important
            _momentLoad = momentLoad;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.NodeSetName, nameof(NodeSetName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ReferencePointName, nameof(ReferencePointName));
            //
            SetBase(_momentLoad, regionTypePropertyNamePairs);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            // 2D
            DynamicCustomTypeDescriptor.GetProperty(nameof(M1)).SetIsBrowsable(!momentLoad.TwoD);
            DynamicCustomTypeDescriptor.GetProperty(nameof(M2)).SetIsBrowsable(!momentLoad.TwoD);
            // Phase
            DynamicCustomTypeDescriptor.GetProperty(nameof(Phase)).SetIsBrowsable(momentLoad.Complex);
        }


        // Methods                                                                                                                  
        public override CaeModel.Load GetBase()
        {
            return _momentLoad;
        }
        public void PopulateDropDownLists(string[] nodeSetNames, string[] referencePointNames, string[] amplitudeNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.NodeSetName, nodeSetNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.ReferencePointName, referencePointNames);
            PopulateDropDownLists(regionTypeListItemsPairs);
            //
            PopulateAmplitudeNames(amplitudeNames);
        }
      
    }

}
