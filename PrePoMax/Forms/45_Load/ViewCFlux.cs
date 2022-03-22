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
    public class ViewCFlux : ViewLoad
    {
        // Variables                                                                                                                
        private CaeModel.CFlux _cFlux;


        // Properties                                                                                                               
        public override string Name { get { return _cFlux.Name; } set { _cFlux.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Add flux")]
        [DescriptionAttribute("Add the flux to the previously defined fluxes in the selected nodes.")]
        [Id(3, 1)]
        public bool AddFlux { get { return _cFlux.AddFlux; } set { _cFlux.AddFlux = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Node set")]
        [DescriptionAttribute("Select the node set for the creation of the load.")]
        [Id(3, 2)]
        public string NodeSetName { get { return _cFlux.RegionName; } set { _cFlux.RegionName = value; } }
        //
        [CategoryAttribute("Magnitude")]
        [OrderedDisplayName(0, 10, "Flux")]
        [DescriptionAttribute("Value of the flux per node.")]
        [TypeConverter(typeof(StringPowerConverter))]
        [Id(1, 3)]
        public double Magnitude { get { return _cFlux.Magnitude; } set { _cFlux.Magnitude = value; } }
        //
        public override string AmplitudeName { get { return _cFlux.AmplitudeName; } set { _cFlux.AmplitudeName = value; } }
        public override System.Drawing.Color Color { get { return _cFlux.Color; } set { _cFlux.Color = value; } }


        // Constructors                                                                                                             
        public ViewCFlux(CaeModel.CFlux cFlux)
        {
            // The order is important
            _cFlux = cFlux;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.NodeSetName, nameof(NodeSetName));
            SetBase(cFlux, regionTypePropertyNamePairs);
            //
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            //
            DynamicCustomTypeDescriptor.RenameBooleanPropertyToYesNo(nameof(AddFlux));
        }


        // Methods                                                                                                                  
        public override CaeModel.Load GetBase()
        {
            return _cFlux;
        }
        public void PopulateDropDownLists(string[] nodeSetNames, string[] amplitudeNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.NodeSetName, nodeSetNames);
            PopulateDropDownLists(regionTypeListItemsPairs);
            //
            PopulateAmplitudeNames(amplitudeNames);
        }
        
    }

}
