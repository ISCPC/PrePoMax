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
        [StandardValue("ScaleFactor", DisplayName = "Scale factor")]
        ScaleFactor,
        [StandardValue("NumberOfElements", DisplayName = "Number of elements")]
        NumberOfElements,
        [StandardValue("MultiLayerd", DisplayName = "Multi-layered")]
        MultiLayerd,
    }
}
