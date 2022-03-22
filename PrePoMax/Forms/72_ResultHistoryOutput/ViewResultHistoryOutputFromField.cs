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
    public class ViewResultHistoryOutputFromField : ViewResultHistoryOutput
    {
        // Variables                                                                                                                
        private CaeResults.ResultHistoryOutputFromField _historyOutput;
        private Dictionary<string, string[]> _filedNameComponentNames;

        // Properties                                                                                                               
        public override string Name { get { return _historyOutput.Name; } set { _historyOutput.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Field name")]
        [DescriptionAttribute("Filed name for the history output.")]
        public string FieldName
        {
            get { return _historyOutput.FieldName; }
            set
            {
                if (_historyOutput.FieldName != value)
                {
                    _historyOutput.FieldName = value;
                    UpdateComponents();
                }
            }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Component name")]
        [DescriptionAttribute("Component name for the history output.")]
        public string ComponentName { get { return _historyOutput.ComponentName; } set { _historyOutput.ComponentName = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Node set")]
        [DescriptionAttribute("Select the node set for the creation of the history output.")]
        public string NodeSetName { get { return _historyOutput.RegionName; } set { _historyOutput.RegionName = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(3, 10, "Surface")]
        [DescriptionAttribute("Select the surface for the creation of the history output.")]
        public string SurfaceName { get { return _historyOutput.RegionName; } set { _historyOutput.RegionName = value; } }

       
        // Constructors                                                                                                             
        public ViewResultHistoryOutputFromField(CaeResults.ResultHistoryOutputFromField historyOutput)
        {
            // The order is important
            _historyOutput = historyOutput;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.NodeSetName, nameof(NodeSetName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SurfaceName));
            //
            SetBase(_historyOutput, regionTypePropertyNamePairs);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public override CaeResults.ResultHistoryOutput GetBase()
        {
            return _historyOutput;
        }
        public void PopulateDropDownLists(string[] nodeSetNames, string[] surfaceNames,
                                            Dictionary<string, string[]> filedNameComponentNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.NodeSetName, nodeSetNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.SurfaceName, surfaceNames);
            base.PopulateDropDownLists(regionTypeListItemsPairs);
            //
            _filedNameComponentNames = filedNameComponentNames;
            DynamicCustomTypeDescriptor.PopulateProperty(nameof(FieldName), _filedNameComponentNames.Keys.ToArray());
            //
            UpdateComponents();
        }

        private void UpdateComponents()
        {
            string[] componentNames;
            if (_filedNameComponentNames.TryGetValue(_historyOutput.FieldName, out componentNames) &&
                componentNames.Length > 1)
            {
                DynamicCustomTypeDescriptor.PopulateProperty(nameof(ComponentName), componentNames);
                if (!componentNames.Contains(ComponentName)) ComponentName = componentNames[0];
            }
        }
    }



   
}
