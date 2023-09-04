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
    public class ViewCLoad : ViewLoad
    {
        // Variables                                                                                                                
        private CaeModel.CLoad _cLoad;


        // Properties                                                                                                               
        public override string Name { get { return _cLoad.Name; } set { _cLoad.Name = value; } }
        //       
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Node set")]
        [DescriptionAttribute("Select the node set for the creation of the load.")]
        [Id(3, 2)]
        public string NodeSetName { get { return _cLoad.RegionName; } set { _cLoad.RegionName = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(3, 10, "Reference point")]
        [DescriptionAttribute("Select the reference point for the creation of the load.")]
        [Id(4, 2)]
        public string ReferencePointName { get { return _cLoad.RegionName; } set { _cLoad.RegionName = value; } }
        //
        [CategoryAttribute("Force components")]
        [OrderedDisplayName(0, 10, "F1")]
        [DescriptionAttribute("Value of the force component per node in the direction of the first axis.")]
        [TypeConverter(typeof(EquationForceConverter))]
        [Id(1, 3)]
        public EquationString F1 { get { return _cLoad.F1.Equation; } set { _cLoad.F1.Equation = value; } }
        //
        [CategoryAttribute("Force components")]
        [OrderedDisplayName(1, 10, "F2")]
        [DescriptionAttribute("Value of the force component per node in the direction of the second axis.")]
        [TypeConverter(typeof(EquationForceConverter))]
        [Id(2, 3)]
        public EquationString F2 { get { return _cLoad.F2.Equation; } set { _cLoad.F2.Equation = value; } }
        //
        [CategoryAttribute("Force components")]
        [OrderedDisplayName(2, 10, "F3")]
        [DescriptionAttribute("Value of the force component per node in the direction of the third axis.")]
        [TypeConverter(typeof(EquationForceConverter))]
        [Id(3, 3)]
        public EquationString F3 { get { return _cLoad.F3.Equation; } set { _cLoad.F3.Equation = value; } }
        //
        [CategoryAttribute("Force magnitude")]
        [OrderedDisplayName(3, 10, "Magnitude")]
        [DescriptionAttribute("Value of the force load magnitude per node.")]
        [TypeConverter(typeof(EquationForceConverter))]
        [Id(1, 4)]
        public EquationString Magnitude { get { return _cLoad.Magnitude.Equation; } set { _cLoad.Magnitude.Equation = value; } }
        //
        [CategoryAttribute("Force phase")]
        [OrderedDisplayName(0, 10, "Phase")]
        [DescriptionAttribute("Value of the force phase.")]
        [TypeConverter(typeof(EquationAngleDegConverter))]
        [Id(1, 5)]
        public EquationString Phase { get { return _cLoad.PhaseDeg.Equation; } set { _cLoad.PhaseDeg.Equation = value; } }
        //
        public override string AmplitudeName { get { return _cLoad.AmplitudeName; } set { _cLoad.AmplitudeName = value; } }
        public override System.Drawing.Color Color { get { return _cLoad.Color; } set { _cLoad.Color = value; } }


        // Constructors                                                                                                             
        public ViewCLoad(CaeModel.CLoad cLoad)
        {
            // The order is important
            _cLoad = cLoad;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.NodeSetName, nameof(NodeSetName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ReferencePointName, nameof(ReferencePointName));
            SetBase(_cLoad, regionTypePropertyNamePairs);
            //
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            // 2D
            DynamicCustomTypeDescriptor.GetProperty(nameof(F3)).SetIsBrowsable(!cLoad.TwoD);
            // Phase
            DynamicCustomTypeDescriptor.GetProperty(nameof(Phase)).SetIsBrowsable(_cLoad.Complex);
        }


        // Methods                                                                                                                  
        public override CaeModel.Load GetBase()
        {
            return _cLoad;
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
