using System;
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
    public class ViewSurfaceSpring : ViewSpring
    {
        // Variables                                                                                                                


        // Properties                                                                                                               
        [CategoryAttribute("Region")]
        [OrderedDisplayName(1, 10, "Surface")]
        [DescriptionAttribute("Select the surface for the creation of the constraint definition.")]
        [Id(2, 2)]
        public string SurfaceName { get { return _springConstraint.RegionName; } set { _springConstraint.RegionName = value; } }


        // Constructors                                                                                                             
        public ViewSurfaceSpring(CaeModel.SurfaceSpring surfaceSpring)
            : base(surfaceSpring)
        {
            // The order is important
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(MasterSelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SurfaceName));
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
        public void PopululateDropDownLists(string[] surfaceNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.SurfaceName, surfaceNames);
            //
            PopululateDropDownLists(regionTypeListItemsPairs, null);
        }
    }

}
