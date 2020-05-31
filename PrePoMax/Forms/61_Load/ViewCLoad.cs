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
        [DescriptionAttribute("Force component per node in the direction of the first axis.")]
        [TypeConverter(typeof(CaeModel.StringForceConverter))]
        [Id(1, 3)]
        public double F1 { get { return _cLoad.F1; } set { _cLoad.F1 = value; } }
        //
        [CategoryAttribute("Force components")]
        [OrderedDisplayName(1, 10, "F2")]
        [DescriptionAttribute("Force component per node in the direction of the second axis.")]
        [TypeConverter(typeof(CaeModel.StringForceConverter))]
        [Id(2, 3)]
        public double F2 { get { return _cLoad.F2; } set { _cLoad.F2 = value; } }
        //
        [CategoryAttribute("Force components")]
        [OrderedDisplayName(2, 10, "F3")]
        [DescriptionAttribute("Force component per node in the direction of the third axis.")]
        [TypeConverter(typeof(CaeModel.StringForceConverter))]
        [Id(3, 3)]
        public double F3 { get { return _cLoad.F3; } set { _cLoad.F3 = value; } }
        //
        [CategoryAttribute("Force magnitude")]
        [OrderedDisplayName(3, 10, "Magnitude")]
        [DescriptionAttribute("The magnitude of the force load per node.")]
        [TypeConverter(typeof(CaeModel.StringForceConverter))]
        [Id(1, 4)]        
        public double Flength
        {
            get { return Math.Sqrt(_cLoad.F1 * _cLoad.F1 + _cLoad.F2 * _cLoad.F2 + _cLoad.F3 * _cLoad.F3); }
            set
            {
                if (value <= 0)
                    throw new Exception("The magnitude of the force must be greater than 0.");

                double len = Math.Sqrt(_cLoad.F1 * _cLoad.F1 + _cLoad.F2 * _cLoad.F2 + _cLoad.F3 * _cLoad.F3);
                double r;
                if (len == 0) r = 0;
                else r = value / len;
                _cLoad.F1 *= r;
                _cLoad.F2 *= r;
                _cLoad.F3 *= r;
            }
        }
        //
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
            base.SetBase(_cLoad, regionTypePropertyNamePairs);
            //
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public override CaeModel.Load GetBase()
        {
            return _cLoad;
        }
        public void PopululateDropDownLists(string[] nodeSetNames, string[] referencePointNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.NodeSetName, nodeSetNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.ReferencePointName, referencePointNames);
            base.PopululateDropDownLists(regionTypeListItemsPairs);
        }
        
    }

}
