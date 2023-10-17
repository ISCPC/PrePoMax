using DynamicTypeDescriptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh.Meshing
{
    [Serializable]
    public enum ElementSizeTypeEnum
    {
        [StandardValue("NumberOfLayers", DisplayName = "Number of layers")]
        NumberOfLayers,
        [StandardValue("ScaleFactor", DisplayName = "Scale factor")]
        ScaleFactor
    }
}
