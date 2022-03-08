using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicTypeDescriptor;

namespace CaeGlobals
{
    [Serializable]
    public enum RegionTypeEnum
    {
        [StandardValue("None")]
        None = -1,
        //
        [StandardValue("Part name")]
        PartName,
        //
        [StandardValue("Node id")]
        NodeId,
        //
        [StandardValue("Node set name")]
        NodeSetName,
        //
        [StandardValue("Element id")]
        ElementId,
        //
        [StandardValue("Element set name")]
        ElementSetName,
        //
        [StandardValue("Surface name")]
        SurfaceName,
        //
        [StandardValue("Reference point name")]
        ReferencePointName,
        //
        [StandardValue("Selection")]
        Selection = 20,
        //
        [StandardValue("Contact pair")]
        ContactPair = 30
    }

    public static class RegionTypeExtensionMethods
    {
        public static string ToFriendlyString(this RegionTypeEnum regionType)
        {
            switch (regionType)
            {
                case RegionTypeEnum.None:
                    return "None";
                case RegionTypeEnum.PartName:
                    return "Part name";
                case RegionTypeEnum.NodeId:
                    return "Node id";
                case RegionTypeEnum.NodeSetName:
                    return "Node set name";
                case RegionTypeEnum.ElementId:
                    return "Element id";
                case RegionTypeEnum.ElementSetName:
                    return "Element set name";                
                case RegionTypeEnum.SurfaceName:
                    return "Surface name";
                case RegionTypeEnum.ReferencePointName:
                    return "Reference point name";
                case RegionTypeEnum.Selection:
                    return "Selection";
                case RegionTypeEnum.ContactPair:
                    return "Contact pair";
                default:
                    throw new NotSupportedException();
            }
        }
  
    }
}
